using System;

namespace ATL.AudioReaders.BinaryLogic
{
	/// <summary>
	/// Dummy metadata provider
	/// </summary>
	public class DummyTag : MetaDataReader
	{
		public bool Exists
		{
			get { return true; }
		}
		public String Title
		{
			get { return ""; }
		}
		public String Artist
		{
			get { return ""; }
		}
		public String Comment
		{
			get { return ""; }
		}
		public String Genre
		{
			get { return ""; }
		}
		public ushort Track
		{
			get { return 0; }
		}
		public String Year
		{
			get { return ""; }
		}
		public String Album
		{
			get { return ""; }
		}

		public DummyTag()
		{			
		}		
	}
}
