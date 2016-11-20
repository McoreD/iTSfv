using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace AlbumArtDownloader.Controls
{
	//Inspired by http://www.codeproject.com/KB/dotnet/DataObjectEx.aspx
    internal class VirtualFileDragger : System.Windows.IDataObject
	{
		#region FILEDESCRIPTOR
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct FILEDESCRIPTOR
        {
			public FileDescriptorFlags dwFlags;
            public Guid clsid;
			public System.Drawing.Size sizel;
			public System.Drawing.Point pointl;
            public UInt32 dwFileAttributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public UInt32 nFileSizeHigh;
            public UInt32 nFileSizeLow;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public String cFileName;
        }

		private enum FileDescriptorFlags
		{
			FD_CLSID = 0x00000001,
			FD_SIZEPOINT = 0x00000002,
			FD_ATTRIBUTES = 0x00000004,
			FD_CREATETIME = 0x00000008,
			FD_ACCESSTIME = 0x00000010,
			FD_WRITESTIME = 0x00000020,
			FD_FILESIZE = 0x00000040,
			FD_PROGRESSUI = 0x00004000,
			FD_LINKUI = 0x00008000,
		}

		// Clipboard formats used for cut/copy/drag operations
		private const string CFSTR_PREFERREDDROPEFFECT = "Preferred DropEffect";
		private const string CFSTR_PERFORMEDDROPEFFECT = "Performed DropEffect";
		private const string CFSTR_FILEDESCRIPTORW = "FileGroupDescriptorW";
		private const string CFSTR_FILECONTENTS = "FileContents";
		#endregion

		private readonly String mFileName;
		private readonly Stream mFileContents;
		private readonly long mContentLength;

		public VirtualFileDragger(string fileName, Stream fileContents) : this(fileName, fileContents, 0)
		{
			try
			{
				mContentLength = fileContents.Length;
			}
			catch (NotSupportedException)
			{ }
		}

        public VirtualFileDragger(string fileName, Stream fileContents, long contentLength)
        {
			mFileName = fileName;
			mFileContents = fileContents;
			mContentLength = contentLength;
        }

		public bool GetDataPresent(string format, bool autoConvert)
		{
			return GetFormats(false).Contains(format, StringComparer.InvariantCultureIgnoreCase);
		}

		public string[] GetFormats(bool autoConvert)
		{
			return new[] { CFSTR_FILEDESCRIPTORW, CFSTR_FILECONTENTS, CFSTR_PERFORMEDDROPEFFECT };
		}

        public object GetData(string format, bool autoConvert)
        {
            if (String.Compare(format, CFSTR_FILEDESCRIPTORW, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return GetFileDescriptor();
            }
            else if (String.Compare(format, CFSTR_FILECONTENTS, StringComparison.OrdinalIgnoreCase) == 0)
            {
				return mFileContents;
            }
            else if (String.Compare(format, CFSTR_PERFORMEDDROPEFFECT, StringComparison.OrdinalIgnoreCase) == 0)
            {
                //TODO: Cleanup routines after paste has been performed
				return null;
            }
			return null;
        }

        private MemoryStream GetFileDescriptor()
        {
            MemoryStream fileDescriptorMemoryStream = new MemoryStream();
            // Write out the FILEGROUPDESCRIPTOR.cItems value
            fileDescriptorMemoryStream.Write(BitConverter.GetBytes(1), 0, sizeof(UInt32));

            FILEDESCRIPTOR fileDescriptor = new FILEDESCRIPTOR();
            
            fileDescriptor.cFileName = mFileName;
			if (mContentLength > 0)
			{
				fileDescriptor.nFileSizeHigh = (UInt32)(mContentLength >> 32);
				fileDescriptor.nFileSizeLow = (UInt32)(mContentLength & 0xFFFFFFFF);
				fileDescriptor.dwFlags = FileDescriptorFlags.FD_FILESIZE | FileDescriptorFlags.FD_PROGRESSUI;
			}
			else
			{
				fileDescriptor.dwFlags = FileDescriptorFlags.FD_PROGRESSUI;
			}

            // Marshal the FileDescriptor structure into a byte array and write it to the MemoryStream.
            Int32 fileDescriptorSize = Marshal.SizeOf(fileDescriptor);
            IntPtr fileDescriptorPointer = Marshal.AllocHGlobal(fileDescriptorSize);
            Marshal.StructureToPtr(fileDescriptor, fileDescriptorPointer, true);
            Byte[] fileDescriptorByteArray = new Byte[fileDescriptorSize];
            Marshal.Copy(fileDescriptorPointer, fileDescriptorByteArray, 0, fileDescriptorSize);
            Marshal.FreeHGlobal(fileDescriptorPointer);
            fileDescriptorMemoryStream.Write(fileDescriptorByteArray, 0, fileDescriptorByteArray.Length);

            return fileDescriptorMemoryStream;
        }

		#region IDataObject Members

		object System.Windows.IDataObject.GetData(Type format)
		{
			return GetData(format.FullName, true);
		}

		object System.Windows.IDataObject.GetData(string format)
		{
			return GetData(format, true);
		}

		bool System.Windows.IDataObject.GetDataPresent(Type format)
		{
			return GetDataPresent(format.FullName, true);
		}

		bool System.Windows.IDataObject.GetDataPresent(string format)
		{
			return GetDataPresent(format, true);
		}

		string[] System.Windows.IDataObject.GetFormats()
		{
			return GetFormats(true);
		}

		void System.Windows.IDataObject.SetData(string format, object data, bool autoConvert)
		{
			throw new NotImplementedException();
		}

		void System.Windows.IDataObject.SetData(Type format, object data)
		{
			throw new NotImplementedException();
		}

		void System.Windows.IDataObject.SetData(string format, object data)
		{
			throw new NotImplementedException();
		}

		void System.Windows.IDataObject.SetData(object data)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
