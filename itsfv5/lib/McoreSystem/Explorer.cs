using System;
using System.IO;

namespace McoreSystem
{
	/// <summary>
	/// Summary description for Explorer.
	/// </summary>
	public class Explorer
	{
		public Explorer()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region "File System"

		#endregion

		public enum Units
		{
			B = 1,
			KiB = 1024,
			MiB = KiB*1024,
			GiB = MiB*1024,
			
		}

		public double GetFileSize(string FileName, int Units)
		{

            FileInfo fi = new System.IO.FileInfo(FileName);
			double fileSizeInBytes = fi.Length;

			double fileSize = fileSizeInBytes / Units;
		
			return fileSize;

		}

		public System.DateTime GetModifiedDate(string FileName)
		{
			FileInfo fi = new System.IO.FileInfo(FileName);
			System.DateTime date = fi.LastWriteTime.ToUniversalTime();

			return date;
		}
	}
}
