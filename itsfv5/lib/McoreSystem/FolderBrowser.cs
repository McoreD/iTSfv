using System;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace McoreSystem
{
	[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
	[ComVisible(true)]
	public class BrowseInfo 
	{
		public IntPtr hwndOwner;
		public IntPtr pidlRoot;
		public IntPtr pszDisplayName;
		public string lpszTitle;
		public int ulFlags;
		public IntPtr lpfn;
		public IntPtr lParam;
		public int iImage;
	} 

	public class Win32SDK
	{
		[DllImport("shell32.dll", PreserveSig=true, CharSet=CharSet.Auto)]
		public static extern IntPtr SHBrowseForFolder(BrowseInfo bi);

		[DllImport("shell32.dll", PreserveSig=true, CharSet=CharSet.Auto)]
		public static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

		[DllImport("shell32.dll", PreserveSig=true, CharSet=CharSet.Auto)]
		public static extern int SHGetSpecialFolderLocation(IntPtr hwnd, int csidl, ref IntPtr ppidl);
	}

	[Flags, Serializable]
	public enum BrowseFlags
	{
		BIF_DEFAULT				=	0x0000,
		BIF_BROWSEFORCOMPUTER	=	0x1000,
		BIF_BROWSEFORPRINTER	=	0x2000,
		BIF_BROWSEINCLUDEFILES	=	0x4000,
		BIF_BROWSEINCLUDEURLS	=	0x0080,
		BIF_DONTGOBELOWDOMAIN	=	0x0002,
		BIF_EDITBOX				=	0x0010,
		BIF_NEWDIALOGSTYLE		=	0x0040,
		BIF_NONEWFOLDERBUTTON	=	0x0200,
		BIF_RETURNFSANCESTORS	=	0x0008,
		BIF_RETURNONLYFSDIRS	=	0x0001,
		BIF_SHAREABLE			=	0x8000,
		BIF_STATUSTEXT			=	0x0004,
		BIF_UAHINT				=	0x0100,
		BIF_VALIDATE			=	0x0020,
		BIF_NOTRANSLATETARGETS	=	0x0400,
	}

	public class FolderBrowser : Component
	{
		private string m_strDirectoryPath;
		private string m_strTitle;
		private string m_strDisplayName;
		private BrowseFlags m_Flags;
		public FolderBrowser()
		{
			m_Flags = BrowseFlags.BIF_DEFAULT;
			m_strTitle = "";
		}

		public string DirectoryPath
		{
			get{return this.m_strDirectoryPath;}
		}

		public string DisplayName
		{
			get{return this.m_strDisplayName;}
		}

		public string Title
		{
			set{this.m_strTitle = value;}
		}

		public BrowseFlags Flags
		{
			set{this.m_Flags = value;}
		}

		public DialogResult ShowDialog()
		{
			BrowseInfo bi = new BrowseInfo();
			bi.pszDisplayName = IntPtr.Zero;
			bi.lpfn = IntPtr.Zero;
			bi.lParam = IntPtr.Zero;
			bi.lpszTitle = "Select Folder";
			IntPtr idListPtr = IntPtr.Zero;
			IntPtr pszPath = IntPtr.Zero;
			try
			{
				if (this.m_strTitle.Length != 0)
				{
					bi.lpszTitle = this.m_strTitle;
				}
				bi.ulFlags = (int)this.m_Flags;
				bi.pszDisplayName = Marshal.AllocHGlobal(256);
				// Call SHBrowseForFolder
				idListPtr = Win32SDK.SHBrowseForFolder(bi);
				// Check if the user cancelled out of the dialog or not.
				if (idListPtr == IntPtr.Zero)
				{
					return DialogResult.Cancel;
				}

				// Allocate ncessary memory buffer to receive the folder path.
				pszPath = Marshal.AllocHGlobal(256);
				// Call SHGetPathFromIDList to get folder path.
				bool bRet = Win32SDK.SHGetPathFromIDList(idListPtr, pszPath);
				// Convert the returned native poiner to string.
				m_strDirectoryPath = Marshal.PtrToStringAuto(pszPath);
				this.m_strDisplayName = Marshal.PtrToStringAuto(bi.pszDisplayName);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
				return DialogResult.Abort;
			}
			finally
			{
				// Free the memory allocated by shell.
				if (idListPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(idListPtr);
				}
				if (pszPath != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(pszPath);
				}
				if (bi != null)
				{
					Marshal.FreeHGlobal(bi.pszDisplayName);
				}
			}
			return DialogResult.OK;
		}

		private IntPtr GetStartLocationPath()
		{
			return IntPtr.Zero;
		}
	}
}						
			
