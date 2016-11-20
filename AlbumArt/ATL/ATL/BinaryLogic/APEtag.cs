/* *************************************************************************** }
{                                                                             }
{ Audio Tools Library (Freeware)                                              }
{ Class TAPEtag - for manipulating with APE tags                              }
{                                                                             }
{ Copyright (c) 2001,2002 by Jurgen Faul                                      }
{ E-mail: jfaul@gmx.de                                                        }
{ http://jfaul.de/atl                                                         }
{                                                                             }
{ 7th May 2005 - Translated to C# by Zeugma 440								  }
{ Version 1.0 (21 April 2002)                                                 }
{   - Reading & writing support for APE 1.0 tags                              }
{   - Reading support for APE 2.0 tags (UTF-8 decoding)                       }
{   - Tag info: title, artist, album, track, year, genre, comment, copyright  }
{                                                                             }
{ ************************************************************************** */

using System;
using System.IO;

namespace ATL.AudioReaders.BinaryLogic
{
	public class TAPEtag : MetaDataReader
	{
		private bool FExists;
		private int FVersion;
		private int FSize;
		private String FTitle;
		private String FArtist;
		private String FAlbum;
		private byte FTrack;
		private String FYear;
		private String FGenre;
		private String FComment;
		private String FCopyright;
      
		public bool Exists // True if tag found
		{
			get { return this.FExists; }
		}
		public int Version // Tag version
		{
			get { return this.FVersion; }
		}
		public int Size // Total tag size
		{
			get { return this.FSize; }
		}
		public String Title // Song title
		{
			get { return this.FTitle ; } 
			set { FSetTitle(value); }
		}
		public String Artist // Artist name
		{
			get { return this.FArtist; }
			set { FSetArtist(value); }
		}
		public String Album // Album title
		{
			get { return this.FAlbum; }
			set { FSetAlbum(value); }
		}
		public ushort Track // Track number
		{
			get { return (ushort)this.FTrack; }
			set { FSetTrack((byte)value); }
		}
		public String Year // Release year
		{
			get { return this.FYear; }
			set { FSetYear(value); }
		}
		public String Genre // Genre name
		{
			get { return this.FGenre; }
			set { FSetGenre(value); }
		}
		public String Comment // Comment
		{
			get { return this.FComment; }
			set { FSetComment(value); }
		}
		public String Copyright // (c)
		{
			get { return this.FCopyright; }
			set { FSetCopyright(value); }
		}

		// Tag ID
		public const String ID3V1_ID = "TAG";                               // ID3v1
		public const String APE_ID = "APETAGEX";							// APE

		// Size constants
		public const byte ID3V1_TAG_SIZE = 128;								// ID3v1 tag
		public const byte APE_TAG_FOOTER_SIZE = 32;							// APE tag footer
		public const byte APE_TAG_HEADER_SIZE = 32;							// APE tag header

		// First version of APE tag
		public const int APE_VERSION_1_0 = 1000;

		// Max. number of supported tag fields
		public const byte APE_FIELD_COUNT = 8;

		// Names of supported tag fields
		public String[] APE_FIELD = new String[APE_FIELD_COUNT]
		{
			"Title", "Artist", "Album", "Track", "Year", "Genre",
			"Comment", "Copyright" };

		// APE tag data - for internal use
		private class TagInfo
		{
			// Real structure of APE footer
			public char[] ID = new char[8];                              // Always "APETAGEX"
			public int Version;			                                       // Tag version
			public int Size;				                     // Tag size including footer
			public int Fields;					                          // Number of fields
			public int Flags;						                             // Tag flags
			public char [] Reserved = new char[8];                  // Reserved for later use
			// Extended data
			public byte DataShift;		                           // Used if ID3v1 tag found
			public int FileSize;		                                 // File size (bytes)
			public String[] Field = new String[APE_FIELD_COUNT];   // Information from fields
		}

		// ********************* Auxiliary functions & voids ********************

		bool ReadFooter(String FileName, ref TagInfo Tag)
		{	
			char[] tagID = new char[3];
			//int Transferred;
			bool result = true;
			FileStream fs = null;
			BinaryReader Source = null;			
  
			// Load footer from file to variable
			try
			{			
				// Set read-access and open file		
				fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
				Source = new BinaryReader(fs);
				Tag.FileSize = (int)fs.Length;
		
				// Check for existing ID3v1 tag
				Source.BaseStream.Seek(Tag.FileSize - ID3V1_TAG_SIZE, SeekOrigin.Begin);
				tagID = Source.ReadChars(3);
				if ( Utils.StringEqualsArr(ID3V1_ID,tagID) ) Tag.DataShift = ID3V1_TAG_SIZE;

				// Read footer data
				Source.BaseStream.Seek(Tag.FileSize - Tag.DataShift - APE_TAG_FOOTER_SIZE, SeekOrigin.Begin);
						
				Tag.ID = Utils.ReadTrueChars(Source,8);
				Tag.Version = Source.ReadInt32(); //
				Tag.Size = Source.ReadInt32();
				Tag.Fields = Source.ReadInt32();
				Tag.Flags = Source.ReadInt32();
				Tag.Reserved = Utils.ReadTrueChars(Source,8);

				//Source.ReadChars(BlockRead(SourceFile, Tag, APE_TAG_FOOTER_SIZE, Transferred);
		
				// if transfer is not complete
				//if (Transferred < APE_TAG_FOOTER_SIZE) result = false;
			} 
			catch (Exception e) 
			{	
				System.Console.WriteLine(e.Message+" ("+FileName+")");
				result = false;
			}
			if (Source != null)	Source.Close();
			if (fs != null)	fs.Close();

			return result;
		}

		// ---------------------------------------------------------------------------

		string ConvertFromUTF8(String Source)
		{
			return Source;
			/*int Iterator;
			int SourceLength;
			int FChar;
			int NChar;	
			String result;
	
			// Convert UTF-8 string to ANSI string
			result = "";
			Iterator = 0;
			SourceLength = Source.Length;
			while (Iterator < SourceLength)
			{
				Iterator++;
				FChar = (byte)Source[Iterator];
				if (FChar >= 0x80)
				{
					Iterator++;
					if (Iterator > SourceLength) break;
					FChar = FChar & 0x3F;
					if ((FChar & 0x20) != 0)
					{			
						FChar = FChar & 0x1F;
						NChar = (byte)Source[Iterator];
						if ((NChar & 0xC0) != 0x80) break;
						FChar = ((FChar << 6) | (NChar & 0x3F));
						Iterator++;
						if (Iterator > SourceLength) break;
					}	
					NChar = (byte)Source[Iterator];
					if ((NChar & 0xC0) != 0x80) break;
					result = result + ((FChar << 6) | (NChar & 0x3F));
				}
				else
					result = result + FChar;
			}
			return result;*/
		}

		// ---------------------------------------------------------------------------

		void SetTagItem(String FieldName, String FieldValue, ref TagInfo Tag)
		{		
			// Set tag item if supported field found
			for (int Iterator=0; Iterator < APE_FIELD_COUNT; Iterator++)		
				if ( FieldName.Replace("\0","").ToUpper() == APE_FIELD[Iterator].ToUpper() ) 
					if (Tag.Version > APE_VERSION_1_0)
						Tag.Field[Iterator] = ConvertFromUTF8(FieldValue);
					else
						Tag.Field[Iterator] = FieldValue;		
		}

		// ---------------------------------------------------------------------------

		void ReadFields(String FileName, ref TagInfo Tag)
		{		
			String FieldName;
			char[] FieldValue = new char[250];
			char NextChar;		
			int ValueSize;
			long ValuePosition;
			int FieldFlags;
			FileStream fs = null;
			BinaryReader SourceFile = null;

			try
			{
				// Set read-access, open file
				fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
				SourceFile = new BinaryReader(fs);
			
				SourceFile.BaseStream.Seek(Tag.FileSize - Tag.DataShift - Tag.Size,SeekOrigin.Begin);
				// Read all stored fields
				for (int iterator=0; iterator < Tag.Fields; iterator++)
				{
					Array.Clear(FieldValue,0,FieldValue.Length);

					ValueSize = SourceFile.ReadInt32();
					FieldFlags = SourceFile.ReadInt32();
					FieldName = "";
					do
					{
						NextChar = SourceFile.ReadChar();
						FieldName = FieldName + NextChar;
					}
					while ( 0 != (byte)NextChar );

					ValuePosition = fs.Position;

					FieldValue = SourceFile.ReadChars(ValueSize % 250);
					SetTagItem(FieldName.Trim(), new String(FieldValue).Trim(), ref Tag);
					SourceFile.BaseStream.Seek(ValuePosition + ValueSize, SeekOrigin.Begin);
				}			
			} 
			catch (Exception e) 
			{ 
				System.Console.WriteLine(e.Message);
				System.Console.WriteLine(e.StackTrace);
			}

			if (SourceFile != null) SourceFile.Close();
			if (fs != null) fs.Close();
		}

		// ---------------------------------------------------------------------------

		byte GetTrack(String TrackString)
		{
			int Index;
			byte Value;		

			// Get track from string
			if (null == TrackString) return 0;
			
			try
			{
				Index = TrackString.IndexOf('/');
				if (-1 == Index) Value = Byte.Parse(TrackString);
				else Value = Byte.Parse(TrackString.Substring(0, Index - 1));
			}
			catch
			{
				return 0;
			}
			return Value;			
		}

		// ---------------------------------------------------------------------------

		bool TruncateFile(String FileName, int TagSize)
		{
			bool result = true;

			try
			{
				// Allow write-access and open file
				FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Write);
				// Delete tag
				fs.SetLength(fs.Length - TagSize);			
				fs.Close();
			} 
			catch (Exception e)
			{
				System.Console.WriteLine(e.StackTrace);
				result = false;
			}
			return result;		
		}

		// ---------------------------------------------------------------------------

		void BuildFooter(ref TagInfo Tag)
		{	
			// Build tag footer
			Tag.ID = APE_ID.ToCharArray();
			Tag.Version = APE_VERSION_1_0;
			Tag.Size = APE_TAG_FOOTER_SIZE;
			for (int iterator=0; iterator < APE_FIELD_COUNT; iterator++)
			{
				if (Tag.Field[iterator] != "")
				{
					Tag.Size += (APE_FIELD[iterator] + Tag.Field[iterator]).Length + 10;
				}
				Tag.Fields++;
			}
		}

		// ---------------------------------------------------------------------------

		bool AddToFile(String FileName, Stream TagData)
		{
			FileStream fs = null;
			BinaryWriter TargetFile = null;
			bool result;

			try
			{
				// Add tag data to file
				fs = new FileStream(FileName, FileMode.Open, FileAccess.Write);
				fs.Lock(0,fs.Length); // Share Exclusive - originally, read denied
				TargetFile = new BinaryWriter(fs);

				TargetFile.BaseStream.Seek(0, SeekOrigin.End);
				TagData.Seek(0, SeekOrigin.Begin);
			
				byte[] bytes = new byte[TagData.Length];
				int numBytesToRead = (int) TagData.Length;
				int numBytesRead = 0;
				while (numBytesToRead > 0) 
				{
					// Read may return anything from 0 to numBytesToRead.
					int n = TagData.Read(bytes, numBytesRead, numBytesToRead);
					TargetFile.Write(bytes,0,n);

					// The end of the file is reached.
					if (0==n) break;
					numBytesRead += n;
					numBytesToRead -= n;				
				}
				//TargetFile.Write(TagData.ReadChars(TagData.Length));
				//FileData.CopyFrom(TagData, TagData.Size);
										
				result = true;
			} 
			catch (Exception e)
			{
				System.Console.WriteLine(e.StackTrace);
				result = false;
			}
			if (fs != null)
			{
				fs.Unlock(0,fs.Length);
				if (TargetFile != null) TargetFile.Close();
			}

			return result;
		}

		// ---------------------------------------------------------------------------

		private bool SaveTag(String FileName, TagInfo Tag)
		{	
			int ValueSize;
			int Flags;
			bool result;

			// Build and write tag fields and footer to stream
			MemoryStream TagData = new MemoryStream();
			BinaryWriter w = new BinaryWriter(TagData);

			for (int iterator=0; iterator < APE_FIELD_COUNT; iterator++)
				if ("" != Tag.Field[iterator])
				{
					ValueSize = Tag.Field[iterator].Length + 1;
					Flags = 0;
					w.Write(ValueSize);
					w.Write(Flags);
					w.Write(APE_FIELD[iterator] + 0x0);
					w.Write(Tag.Field[iterator] + 0x0);
				}
			BuildFooter(ref Tag);
			//TagData.Write(Tag, APE_TAG_FOOTER_SIZE);
		
			w.Write(Tag.ID);
			w.Write(Tag.Version);
			w.Write(Tag.Size);
			w.Write(Tag.Fields);
			w.Write(Tag.Flags);
			w.Write(Tag.Reserved);

			// Add created tag to file
			result = AddToFile(FileName, TagData);
			w.Close();
			TagData.Close();

			return result;
		}

		// ********************** Private functions & voids *********************

		void FSetTitle(String newTrack)
		{
			// Set song title
			FTitle = newTrack.Trim();	
		}

		// ---------------------------------------------------------------------------

		void FSetArtist(String NewArtist)
		{
			// Set artist name
			FArtist = NewArtist.Trim();
		}

		// ---------------------------------------------------------------------------

		void FSetAlbum(String NewAlbum)
		{
			// Set album title
			FAlbum = NewAlbum.Trim();
		}

		// ---------------------------------------------------------------------------

		void FSetTrack(byte NewTrack)
		{
			// Set track number
			FTrack = NewTrack;
		}

		// ---------------------------------------------------------------------------

		void FSetYear(String NewYear)
		{
			// Set release year
			FYear = NewYear.Trim();
		}

		// ---------------------------------------------------------------------------

		void FSetGenre(String NewGenre)
		{
			// Set genre name
			FGenre = NewGenre.Trim();
		}

		// ---------------------------------------------------------------------------

		void FSetComment(String NewComment)
		{
			// Set comment
			FComment = NewComment.Trim();
		}

		// ---------------------------------------------------------------------------

		void FSetCopyright(String NewCopyright)
		{
			// Set copyright information
			FCopyright = NewCopyright.Trim();
		}

		// ********************** Public functions & voids **********************

		public void TAPETag()
		{
			// Create object		
			ResetData();
		}

		// ---------------------------------------------------------------------------

		public void ResetData()
		{
			// Reset all variables
			FExists = false;
			FVersion = 0;
			FSize = 0;
			FTitle = "";
			FArtist = "";
			FAlbum = "";
			FTrack = 0;
			FYear = "";
			FGenre = "";
			FComment = "";
			FCopyright = "";
		}

		// ---------------------------------------------------------------------------

		public bool ReadFromFile(String FileName)
		{
			TagInfo Tag = new TagInfo();

			// Reset data and load footer from file to variable
			ResetData();
				
			Array.Clear(Tag.ID,0,Tag.ID.Length);
			Tag.Version = 0;
			Tag.Flags = 0;
			Tag.Fields = 0;
			Tag.Size = 0;
			Array.Clear(Tag.Reserved,0,Tag.Reserved.Length);
			Tag.DataShift = 0;
			Tag.FileSize = 0;
			for (int i=0; i<Tag.Field.Length; i++) Tag.Field[i] = "";

			bool result = ReadFooter(FileName, ref Tag);
		
			// Process data if loaded and footer valid
			if ( (result) && Utils.StringEqualsArr(APE_ID, Tag.ID) )
			{
				FExists = true;
				// Fill properties with footer data
				FVersion = Tag.Version;
				FSize = Tag.Size;
				// Get information from fields
				ReadFields(FileName, ref Tag);
				FTitle = Tag.Field[0];
				FArtist = Tag.Field[1];
				FAlbum = Tag.Field[2];
				FTrack = GetTrack(Tag.Field[3]);
				FYear = Tag.Field[4];
				FGenre = Tag.Field[5];
				FComment = Tag.Field[6];
				FCopyright = Tag.Field[7];
			}

			return result;
		}

		// ---------------------------------------------------------------------------

		public bool RemoveFromFile(String FileName)
		{
			TagInfo Tag = new TagInfo();
			// Remove tag from file if found
		
			Array.Clear(Tag.ID,0,Tag.ID.Length);
			Tag.Version = 0;
			Tag.Flags = 0;
			Tag.Fields = 0;
			Tag.Size = 0;
			Array.Clear(Tag.Reserved,0,Tag.Reserved.Length);
			Tag.DataShift = 0;
			Tag.FileSize = 0;
			Array.Clear(Tag.Field,0,Tag.Field.Length);

			if ( ReadFooter(FileName, ref Tag) )
			{
				if ( ! Utils.StringEqualsArr(APE_ID,Tag.ID)) Tag.Size = 0;
				if ((Tag.Flags >> 31) > 0) Tag.Size += APE_TAG_HEADER_SIZE;
				return TruncateFile(FileName, Tag.DataShift + Tag.Size);
			}
			else
				return false;
		}

		// ---------------------------------------------------------------------------

		public bool SaveToFile(String FileName)
		{
			TagInfo Tag = new TagInfo();
			// Prepare tag data and save to file
		
			Array.Clear(Tag.ID,0,Tag.ID.Length);
			Tag.Version = 0;
			Tag.Flags = 0;
			Tag.Fields = 0;
			Tag.Size = 0;
			Array.Clear(Tag.Reserved,0,Tag.Reserved.Length);
			Tag.DataShift = 0;
			Tag.FileSize = 0;
			Array.Clear(Tag.Field,0,Tag.Field.Length);

			Tag.Field[0] = FTitle;
			Tag.Field[1] = FArtist;
			Tag.Field[2] = FAlbum;
			if (FTrack > 0) Tag.Field[3] = FTrack.ToString();
			Tag.Field[4] = FYear;
			Tag.Field[5] = FGenre;
			Tag.Field[6] = FComment;
			Tag.Field[7] = FCopyright;

			// Delete old tag if exists and write new tag
			return ((RemoveFromFile(FileName)) && (SaveTag(FileName, Tag)));
		}

	}
}