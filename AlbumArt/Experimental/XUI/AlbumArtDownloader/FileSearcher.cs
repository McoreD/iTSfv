//Taken from http://msdn.microsoft.com/msdnmag/issues/05/12/NETMatters/
//This code enumerates files one at a time, rather than using Directory.GetFiles,
//which has to enumerate all of them before returning.

// Stephen Toub
// stoub@microsoft.com

// Adapted by Alex Vallat for searching Directories too.

using System;
using System.IO;
using System.Linq;
using System.Security;
using System.ComponentModel;
using System.Collections.Generic;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace NetMatters
{
	public static class FileSearcher
	{
		private sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
			[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
			private SafeFindHandle() : base(true) { }

			protected override bool ReleaseHandle()
			{
				return FindClose(this.handle);
			}

			[DllImport("kernel32.dll")]
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [SuppressUnmanagedCodeSecurity]
            private static extern bool FindClose(IntPtr handle);
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
		private static extern SafeFindHandle FindFirstFile(string lpFileName, [In, Out, MarshalAs(UnmanagedType.LPStruct)] WIN32_FIND_DATA lpFindFileData);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern bool FindNextFile(SafeFindHandle hndFindFile, [In, Out, MarshalAs(UnmanagedType.LPStruct)] WIN32_FIND_DATA lpFindFileData);

		[DllImport("kernel32.dll", SetLastError = true)]
        private static extern ErrorModes SetErrorMode(ErrorModes newMode);

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		[BestFitMapping(false)]
		private class WIN32_FIND_DATA
		{
			public FileAttributes dwFileAttributes;
			public ComTypes.FILETIME ftCreationTime;
			public ComTypes.FILETIME ftLastAccessTime;
			public ComTypes.FILETIME ftLastWriteTime;
			public int nFileSizeHigh;
			public int nFileSizeLow;
			public int dwReserved0;
			public int dwReserved1;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string cFileName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
			public string cAlternateFileName;
		}

        private const int ERROR_FILE_NOT_FOUND = 0x2;
		private const int ERROR_ACCESS_DENIED = 0x5;
		private const int ERROR_NO_MORE_FILES = 0x12;

        [Flags]
        private enum ErrorModes
        {
            /// <summary>Use the system default, which is to display all error dialog boxes.</summary>
            Default = 0x0,
            /// <summary>
            /// The system does not display the critical-error-handler message box. 
            /// Instead, the system sends the error to the calling process.
            /// </summary>
            FailCriticalErrors = 0x1,
            /// <summary>
            /// 64-bit Windows:  The system automatically fixes memory alignment faults and makes them 
            /// invisible to the application. It does this for the calling process and any descendant processes.
            /// After this value is set for a process, subsequent attempts to clear the value are ignored.
            /// </summary>
            NoGpFaultErrorBox = 0x2,
            /// <summary>
            /// The system does not display the general-protection-fault message box. 
            /// This flag should only be set by debugging applications that handle general 
            /// protection (GP) faults themselves with an exception handler.
            /// </summary>
            NoAlignmentFaultExcept = 0x4,
            /// <summary>
            /// The system does not display a message box when it fails to find a file. 
            /// Instead, the error is returned to the calling process.
            /// </summary>
            NoOpenFileErrorBox = 0x8000
        }

		public static IEnumerable<FileInfo> GetFiles(DirectoryInfo dir, string pattern, SearchOption searchOption)
		{
			return GetFileSystemInfos(dir, pattern, searchOption, false, true).Cast<FileInfo>();
		}
		public static IEnumerable<DirectoryInfo> GetDirectories(DirectoryInfo dir, string pattern, SearchOption searchOption)
		{
			return GetFileSystemInfos(dir, pattern, searchOption, true, false).Cast<DirectoryInfo>();
		}
		public static IEnumerable<FileSystemInfo> GetFileSystemInfos(DirectoryInfo dir, string pattern, SearchOption searchOption, bool includeDirectories, bool includeFiles)
		{
            // We suppressed this demand for each p/invoke call, so demand it upfront once
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();

			// Validate parameters
			if (dir == null) throw new ArgumentNullException("dir");
			if (pattern == null) throw new ArgumentNullException("pattern");

			// Setup
			WIN32_FIND_DATA findData = new WIN32_FIND_DATA();
			Stack<DirectoryInfo> directories = new Stack<DirectoryInfo>();
			directories.Push(dir);

			// Process each directory
            ErrorModes origErrorMode = SetErrorMode(ErrorModes.FailCriticalErrors);
			try
			{
				while (directories.Count > 0)
				{
					// Get the name of the next directory and the corresponding search pattern
					dir = directories.Pop();
					string dirPath = dir.FullName.Trim();
					if (dirPath.Length == 0) continue;
					char lastChar = dirPath[dirPath.Length - 1];
					if (lastChar != Path.DirectorySeparatorChar && lastChar != Path.AltDirectorySeparatorChar)
					{
						dirPath += Path.DirectorySeparatorChar;
					}

					// Process all files in that directory
					SafeFindHandle handle = FindFirstFile(dirPath + pattern, findData);
					if (handle.IsInvalid)
					{
						int error = Marshal.GetLastWin32Error();
						if (error == ERROR_ACCESS_DENIED || 
                            error == ERROR_FILE_NOT_FOUND) continue;
						else throw new Win32Exception(error);
					}
					else
					{
						try
						{
							do
							{
								if ((findData.dwFileAttributes & FileAttributes.Directory) == 0)
								{
									if (includeFiles)
									{
										yield return new FileInfo(dirPath + findData.cFileName);
									}
								}
								else
								{
									if (includeDirectories && findData.cFileName != "." && findData.cFileName != "..")
									{
										yield return new DirectoryInfo(dirPath + findData.cFileName);
									}
								}
							}
							while (FindNextFile(handle, findData));
							int error = Marshal.GetLastWin32Error();
							if (error != ERROR_NO_MORE_FILES) throw new Win32Exception(error);
						}
						finally { handle.Dispose(); }
					}

					// Add all child directories if that's what the user wants
					if (searchOption == SearchOption.AllDirectories)
					{
						try
						{
							foreach (DirectoryInfo childDir in GetDirectories(dir, "*", SearchOption.TopDirectoryOnly)) //Top only, as we're all ready handling recursion here.
							{
								try
								{
									if ((File.GetAttributes(childDir.FullName) & FileAttributes.ReparsePoint) == 0)
									{
										directories.Push(childDir);
									}
								}
								catch (Exception childDirEx)
								{
									//Can't get subfolders
									System.Diagnostics.Trace.WriteLine("Can't search inside: " + childDir.Name);
									System.Diagnostics.Trace.Indent();
									System.Diagnostics.Trace.WriteLine(childDirEx.Message);
									System.Diagnostics.Trace.Unindent();
								}
							}
						}
						catch (Exception e)
						{
							//Can't get subfolders
							System.Diagnostics.Trace.WriteLine("Can't search inside: " + dirPath);
							System.Diagnostics.Trace.Indent();
							System.Diagnostics.Trace.WriteLine(e.Message);
							System.Diagnostics.Trace.Unindent();
						}
					}
				}
			}
			finally { SetErrorMode(origErrorMode); }
		}
	}
}