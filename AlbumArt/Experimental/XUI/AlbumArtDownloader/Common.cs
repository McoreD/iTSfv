using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using AlbumArtDownloader.Scripts;
using System.Windows.Input;
using System.Runtime.InteropServices;

namespace AlbumArtDownloader
{
	internal static class Common
	{
		#region New Window
		public static ArtSearchWindow NewSearchWindow()
		{
			return NewSearchWindow(null);
		}
		public static ArtSearchWindow NewSearchWindow(IAppWindow existingWindow)
		{
			return NewSearchWindow(existingWindow, false);
		}
		/// <param name="forceShown">If true, the new search window will be immediately shown, rather than queued.</param>
		public static ArtSearchWindow NewSearchWindow(IAppWindow existingWindow, bool forceShown)
		{
			//Enqueue rather than opening the new window directly
			ArtSearchWindow newWindow = new ArtSearchWindow();
			SetupNewWindow(newWindow, existingWindow);
			SearchQueue searchQueue = ((App)Application.Current).SearchQueue;
			searchQueue.EnqueueSearchWindow(newWindow, true);

			if (forceShown)
			{
				//Ensure the new window is shown immediately, rather than queued.
				searchQueue.ForceSearchWindow(newWindow);
			}
			else if (searchQueue.Queue.Count == 1)
			{
				//This is the first item enqueued, so show the queue manager window
				searchQueue.ShowManagerWindow();
			}

			return newWindow;
		}

		public static FileBrowser NewFileBrowser()
		{
			return NewFileBrowser(null);
		}
		public static FileBrowser NewFileBrowser(IAppWindow existingWindow)
		{
            return (FileBrowser)ShowNewWindow(new FileBrowser(), existingWindow);
		}
        public static FileBrowserDetail NewFileBrowserDetail()
        {
            return NewFileBrowserDetail(null);
        }
        public static FileBrowserDetail NewFileBrowserDetail(IAppWindow existingWindow)
		{
			return (FileBrowserDetail)ShowNewWindow(new FileBrowserDetail(), existingWindow);
		}
		public static FoobarBrowser NewFoobarBrowser()
		{
			return NewFoobarBrowser(null);
		}
		public static FoobarBrowser NewFoobarBrowser(IAppWindow existingWindow)
		{
			return (FoobarBrowser)ShowNewWindow(new FoobarBrowser(), existingWindow);
		}

		public static ArtPreviewWindow NewPreviewWindow()
		{
			return NewPreviewWindow(null);
		}
		public static ArtPreviewWindow NewPreviewWindow(IAppWindow existingWindow)
		{
			return (ArtPreviewWindow)ShowNewWindow(new ArtPreviewWindow(), existingWindow);
		}

		private static IAppWindow ShowNewWindow(IAppWindow newWindow, IAppWindow oldWindow)
		{
			SetupNewWindow(newWindow, oldWindow);

			newWindow.Show();
			return newWindow;
		}

		private static void SetupNewWindow(IAppWindow newWindow, IAppWindow oldWindow)
		{
			if (oldWindow != null)
			{
				//Save values to settings so that the new window picks up on them
				oldWindow.SaveSettings();
				//Load the newly saved settings
				newWindow.LoadSettings();

				//Move the window a little, so that it is obvious it is a new window
				newWindow.Left = oldWindow.Left + 40;
				newWindow.Top = oldWindow.Top + 40;

				//TODO: Neater laying out of windows which would go off the screen. Note how Firefox handles this, for example, when opening lots of new non-maximised windows.
				//TODO: Multimonitor support.
				if (newWindow.Left + newWindow.Width > SystemParameters.PrimaryScreenWidth)
				{
					//For the present, just make sure that the window doesn't leave the screen.
					newWindow.Left = SystemParameters.PrimaryScreenWidth - newWindow.Width;
				}
				if (newWindow.Top + newWindow.Height > SystemParameters.PrimaryScreenHeight)
				{
					newWindow.Top = SystemParameters.PrimaryScreenHeight - newWindow.Height;
				}
			}
		}
		#endregion

		#region Cascade

		public class WindowCascade
		{
			/// <summary>
			/// When creating new multiple windows, offset each by this amount so that they aren't all on top of each other.
			/// </summary>
			private static readonly int sSearchWindowCascadeOffset = 20;
			private int mWindowCount;

			public WindowCascade()
			{ }

			public void Arrange(IAppWindow window)
			{
				window.Top += mWindowCount * sSearchWindowCascadeOffset;
				window.Left += mWindowCount * sSearchWindowCascadeOffset;

				//TODO: Neater laying out of windows which would go off the screen. Note how Firefox handles this, for example, when opening lots of new non-maximised windows.
				//TODO: Multimonitor support.
				if (window.Left + window.Width > SystemParameters.PrimaryScreenWidth)
				{
					//For the present, just make sure that the window doesn't leave the screen.
					window.Left = SystemParameters.PrimaryScreenWidth - window.Width;
				}
				if (window.Top + window.Height > SystemParameters.PrimaryScreenHeight)
				{
					window.Top = SystemParameters.PrimaryScreenHeight - window.Height;
				}

				mWindowCount++;
			}
		}
		#endregion
		
		#region EnumerableHelpers
		/// <summary>
		/// Takes a specified generic IEnumerable, and returns an unspecified one.
		/// </summary>
		public static System.Collections.IEnumerable UnspecifyEnumerable<T>(IEnumerable<T> enumerable)
		{
			foreach (T item in enumerable)
			{
				yield return item;
			}
		}
		#endregion

		#region Resolve Path with Wildcards
		/// <summary>
		/// Substitutes Artist and Album placeholders for an image search path pattern.
		/// </summary>
		public static string SubstitutePlaceholders(string pathPattern, string artist, string album)
		{
			string result = pathPattern.Replace("%artist%", Common.MakeSafeForPath(artist))
								.Replace("%album%", Common.MakeSafeForPath(album));
			
			//Replace these too, just in case path pattern was copied and pasted with them in, for example
			return Regex.Replace(result, @"%(?:name|extension|source|size|preset|type(?:\([^)]*\))?)%", "*", RegexOptions.IgnoreCase);
		}

		private static Regex sPathPatternSplitter = new Regex(@"(?<fixed>(?:[^/\\*]*(?:[/\\]|$))*)(?<match>[^/\\]+)?[/\\]?(?<remainder>.*)", RegexOptions.Compiled);
		public static IEnumerable<string> ResolvePathPattern(string pathPattern)
		{
			Match match = sPathPatternSplitter.Match(pathPattern);

			if (match.Groups["match"].Success)
			{
				//Theres a wildcard part of the path that needs matching against.
				DirectoryInfo fixedPart = null;
				try
				{
					fixedPart = new DirectoryInfo(match.Groups["fixed"].Value);
				}
				catch (Exception e)
				{
					//Path not valid, so no images to find
					System.Diagnostics.Trace.WriteLine("Path not valid for file search: " + match.Groups["fixed"].Value);
					System.Diagnostics.Trace.Indent();
					System.Diagnostics.Trace.WriteLine(e.Message);
					System.Diagnostics.Trace.Unindent();
					yield break;
				}
				if (fixedPart == null || !fixedPart.Exists)
				{
					//Path not found, so no images to find
					System.Diagnostics.Trace.WriteLine("Path not found for file search: " + match.Groups["fixed"].Value);
					yield break;
				}

				//Find all the matching paths for the part of pattern specified
				string searchPattern = match.Groups["match"].Value;
				if (searchPattern == "**")
				{
					//Recursive folder matching wildcard
					//Go into subfolders
					Stack<DirectoryInfo> subfolders = new Stack<DirectoryInfo>();
					subfolders.Push(fixedPart); //Start with the current folder
					while (subfolders.Count > 0)
					{
						DirectoryInfo searchInFolder = subfolders.Pop();

						foreach (string result in ResolvePathPattern(Path.Combine(searchInFolder.FullName, match.Groups["remainder"].Value)))
						{
							yield return result;
						}

						try
						{
							foreach (DirectoryInfo subfolder in searchInFolder.GetDirectories())
							{
								if ((subfolder.Attributes & FileAttributes.ReparsePoint) == 0) //Don't recurse into reparse points
								{
									subfolders.Push(subfolder);
								}
							}
						}
						catch (Exception e)
						{
							//Can't get subfolders
							System.Diagnostics.Trace.WriteLine("Can't search inside: " + searchInFolder.FullName);
							System.Diagnostics.Trace.Indent();
							System.Diagnostics.Trace.WriteLine(e.Message);
							System.Diagnostics.Trace.Unindent();
						}
					}
				}
				else
				{
					//Normal wildcard
					FileSystemInfo[] fileSystemInfos;
					try
					{
						fileSystemInfos = fixedPart.GetFileSystemInfos(searchPattern);
					}
					catch (ArgumentException e)
					{
						System.Diagnostics.Trace.WriteLine("Path not valid for file search: " + fixedPart.FullName + "\\" + searchPattern);
						System.Diagnostics.Trace.Indent();
						System.Diagnostics.Trace.WriteLine(e.Message);
						System.Diagnostics.Trace.Unindent();
						yield break;
					}
					foreach (FileSystemInfo matchedPath in fileSystemInfos)
					{
						foreach (string result in ResolvePathPattern(Path.Combine(matchedPath.FullName, match.Groups["remainder"].Value)))
						{
							yield return result;
						}
					}
				}
			}
			else
			{
				//There is no wildcard part of the path remaining, so check if it exists
				if (Directory.Exists(pathPattern))
				{
					//It's a folder. Only files are required, so ignore it.
				}
				else if (File.Exists(pathPattern))
				{
					//It's a file, so return it
					yield return pathPattern;
				}
				else
				{
					//Path not found, so no images to find
					System.Diagnostics.Trace.WriteLine("Path not found for file search: " + match.Groups["fixed"].Value);
					yield break;
				}
			}
		}
		#endregion

		/// <summary>
		/// Gets the command args this app was started with, as executed, not parsed into args[]
		/// </summary>
		public static string GetCommandArgs() //Surely this should not be that tricky?
		{
			string commandLine = Environment.CommandLine;

			//Find either the space, which is a the delimeter, or a " mark, which bounds spaces
			int pos = 0;
			do
			{
				if (pos >= commandLine.Length)
					return String.Empty; //No command line args

				pos = commandLine.IndexOfAny(new char[] { ' ', '"' }, pos);
				if (pos == -1)
					return String.Empty; //No command line args

				if (commandLine[pos] == '"')
				{
					//Find the closing " mark. " marks can't be escaped in path names
					pos = commandLine.IndexOf('"', pos + 1) + 1;
					if (pos == 0)
					{
						//No command line args. Probably malformed command line too.
						System.Diagnostics.Trace.TraceWarning("Could not find closing \" mark in command line: " + commandLine);
						return String.Empty;
					}
					//Otherwise, go round again to find another quote, or alternatively a space
				}
				else
				{
					System.Diagnostics.Debug.Assert(commandLine[pos] == ' ', "Expecting a space here, if not a \" mark");
					//Everything past this point should now be the command args, as this is an unquoted space
					return commandLine.Substring(pos + 1);
				}
			} while (true);
		}

		/// <summary>
		/// Ensures that a string is safe to be part of a file path by replacing all illegal
		/// characters with underscores.
		/// </summary>
		public static string MakeSafeForPath(string value)
		{
			if (String.IsNullOrEmpty(value))
				return String.Empty;

			char[] invalid = Path.GetInvalidFileNameChars();
			char[] valueChars = value.ToCharArray();

			bool valueChanged = false;
			int invalidIndex = -1;
			while ((invalidIndex = value.IndexOfAny(invalid, invalidIndex + 1)) >= 0)
			{
				valueChars[invalidIndex] = '_';
				valueChanged = true;
			}
			if (valueChanged)
			{
				return new string(valueChars);
			}
			else //Don't perform the construction of the new string if not required
			{
				return value;
			}
		}

		/// <summary>
		/// Finds a visual child of the specified visual which is of the
		/// specified type, or null if there are none.
		/// </summary>
		public static TChild FindVisualChild<TChild>(DependencyObject obj)
			where TChild : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child is TChild)
				{
					return (TChild)child;
				}
				else
				{
					TChild descendant = FindVisualChild<TChild>(child);
					if (descendant != null)
					{
						return descendant;
					}
				}
			}
			return null;
		}

		public static AllowedCoverType MakeAllowedCoverType(CoverType scriptCoverType)
		{
			switch (scriptCoverType)
			{
				case CoverType.Unknown:
					return AllowedCoverType.Unknown;
				case CoverType.Front:
					return AllowedCoverType.Front;
				case CoverType.Back:
					return AllowedCoverType.Back;
				case CoverType.Inside:
					return AllowedCoverType.Inside;
				case CoverType.CD:
					return AllowedCoverType.CD;
			}
			return AllowedCoverType.Unknown;
		}

		#region Delete to Recycle Bin
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
		private struct SHFILEOPSTRUCT32
		{
			internal IntPtr hwnd;
			internal uint wFunc;
			[MarshalAs(UnmanagedType.LPTStr)]
			internal string pFrom;
			[MarshalAs(UnmanagedType.LPTStr)]
			internal string pTo;
			internal ushort fFlags;
			internal bool fAnyOperationsAborted;
			internal IntPtr hNameMappings;
			[MarshalAs(UnmanagedType.LPTStr)]
			internal string lpszProgressTitle;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct SHFILEOPSTRUCT64
		{
			internal IntPtr hwnd;
			internal uint wFunc;
			[MarshalAs(UnmanagedType.LPTStr)]
			internal string pFrom;
			[MarshalAs(UnmanagedType.LPTStr)]
			internal string pTo;
			internal ushort fFlags;
			internal bool fAnyOperationsAborted;
			internal IntPtr hNameMappings;
			[MarshalAs(UnmanagedType.LPTStr)]
			internal string lpszProgressTitle;
		}

		[DllImport("shell32.dll", EntryPoint = "SHFileOperation", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int SHFileOperation32(ref SHFILEOPSTRUCT32 lpFileOp);

		[DllImport("shell32.dll", EntryPoint = "SHFileOperation", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int SHFileOperation64(ref SHFILEOPSTRUCT64 lpFileOp);

		private const int FO_DELETE = 3;
		private const int FOF_ALLOWUNDO = 0x40;
		//private const int FOF_NOCONFIRMATION = 0x10;    //No prompt dialogs 

		public static bool DeleteFileToRecycleBin(string filePath)
		{
			int retVal;
			if (IntPtr.Size == 4) //32 bit
			{
				var shf = new SHFILEOPSTRUCT32();
				shf.wFunc = FO_DELETE;
				shf.fFlags = FOF_ALLOWUNDO;
				shf.pFrom = filePath + "\0";

				retVal = SHFileOperation32(ref shf);
			}
			else  //64 bit (almost exactly the same, but without "Pack = 1" set.
			{
				var shf = new SHFILEOPSTRUCT64();
				shf.wFunc = FO_DELETE;
				shf.fFlags = FOF_ALLOWUNDO;
				shf.pFrom = filePath + "\0";

				retVal = SHFileOperation64(ref shf);
			}
			return retVal == 0;
		}
		#endregion
	}

	public static class CommonCommands
	{
        /// <summary> Saves ID3 tags in file info passed as the parameter to the command </summary>
        public static RoutedUICommand Save = new RoutedUICommand("Save", "Save", typeof(CommonCommands));
		/// <summary>Displays the file passed in as the parameter to the command in Windows Explorer</summary>
		public static RoutedUICommand ShowInExplorer = new RoutedUICommand("Show in Explorer", "ShowInExplorer", typeof(CommonCommands));
		/// <summary>Displays the file in the preview window</summary>
		public static RoutedUICommand Preview = new RoutedUICommand("Preview", "Preview", typeof(CommonCommands));
		/// <summary>Shows a dialog to rename the file</summary>
		public static RoutedUICommand Rename = new RoutedUICommand("Rename", "Rename", typeof(CommonCommands));
		/// <summary>Deletes the file (to the recycle bin)</summary>
		public static RoutedUICommand Delete = new RoutedUICommand("Delete", "Delete", typeof(CommonCommands));

		static CommonCommands()
		{
			CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(ShowInExplorer, new ExecutedRoutedEventHandler(ShowInExplorerExec)));
			CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(Preview, new ExecutedRoutedEventHandler(PreviewExec)));
			CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(Delete, new ExecutedRoutedEventHandler(DeleteFileExec), new CanExecuteRoutedEventHandler(DeleteFileCanExec)));
			CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(Rename, new ExecutedRoutedEventHandler(RenameArtFileExec), new CanExecuteRoutedEventHandler(RenameArtFileCanExec)));
		}

		public static void ShowInExplorerExec(object sender, ExecutedRoutedEventArgs e)
		{
			string filePath = e.Parameter as string;
			if (!String.IsNullOrEmpty(filePath))
			{
				//TODO: Validation that this is a valid file path?
				if (EmbeddedArtHelpers.IsEmbeddedArtPath(filePath))
				{
					int ignored;
					EmbeddedArtHelpers.SplitToFilenameAndIndex(filePath, out filePath, out ignored);
				}
				System.Diagnostics.Process.Start("explorer.exe", "/select,\"" + filePath + "\"");
			}
		}

		public static void PreviewExec(object sender, ExecutedRoutedEventArgs e)
		{
			string filePath = e.Parameter as string;
			if (!String.IsNullOrEmpty(filePath))
			{
				LocalFilesSource source = new LocalFilesSource()
				{
					SearchPathPattern = filePath,
					DefaultFilePath = "",
					MaximumResults = 1
				};
				source.SearchCompleted += new EventHandler(delegate
				{
					if (source.Results.Count > 0)
					{
						var previewWindow = Common.NewPreviewWindow(GetWindow(sender) as IAppWindow);
						var albumArt = source.Results[0];
						albumArt.FilePath = filePath;
						previewWindow.AlbumArt = albumArt;
					}
					else
					{
						System.Diagnostics.Trace.TraceError("Could not obtain AlbumArt for local file: " + source.SearchPathPattern);
					}
				});
				source.Search(null, null);
			}
		}

		public static void DeleteFileExec(object sender, ExecutedRoutedEventArgs e)
		{
			string filePath = e.Parameter as string;
			if (!String.IsNullOrEmpty(filePath))
			{
				Common.DeleteFileToRecycleBin(filePath);
			}
		}

		public static void DeleteFileCanExec(object sender, CanExecuteRoutedEventArgs e)
		{
			string filePath = e.Parameter as string;
			//TODO: Check for file existence and permissions here too?
			e.CanExecute = !String.IsNullOrEmpty(filePath) && !EmbeddedArtHelpers.IsEmbeddedArtPath(filePath);
		}

		public static void RenameArtFileExec(object sender, ExecutedRoutedEventArgs e)
		{
			string filePath = e.Parameter as string;
			if (!String.IsNullOrEmpty(filePath))
			{
				var renameWindow = new RenameArt(filePath);
				renameWindow.Owner = GetWindow(sender);
				renameWindow.ShowDialog();
			}
		}

		public static void RenameArtFileCanExec(object sender, CanExecuteRoutedEventArgs e)
		{
			string filePath = e.Parameter as string;
			//TODO: Check for file existence and permissions here too?
			e.CanExecute = !String.IsNullOrEmpty(filePath) && !EmbeddedArtHelpers.IsEmbeddedArtPath(filePath);
		}

		private static Window GetWindow(object sender)
		{
			var dependencyObject = sender as DependencyObject;
			if (dependencyObject != null)
			{
				return Window.GetWindow(dependencyObject);
			}
			return null;
		}
	}
}
