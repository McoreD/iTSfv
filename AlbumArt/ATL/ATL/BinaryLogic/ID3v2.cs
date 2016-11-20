// ***************************************************************************
//                                                                            
// Audio Tools Library (Freeware)                                             
// Class TID3v2 - for manipulating with ID3v2 tags                            
//                                                                            
// Copyright (c) 2001,2002 by Jurgen Faul                                     
// E-mail: jfaul@gmx.de                                                       
// http://jfaul.de/atl                                                        
//
// 8th Jul 2006 - Fixed a bug in ID3v1 genre recognition
//
// 30th April 2006 - Zeugma 440 - Enable UTF-8-encoded tags to be read
//
// 24th Jun 2005 - Zeugma 440 - Improved Genre recognition (added ID3v1-style recognition)
//             
// 8th May 2005 - Translated to C# by Zeugma 440
//                                                               
// Version 1.7 (2 October 2002)                                               
//   - Added property TrackString                                             
//                                                                            
// Version 1.6 (29 July 2002)                                                 
//   - Reading support for Unicode                                            
//   - Removed limitation for the track number                                
//                                                                            
// Version 1.5 (23 May 2002)                                                  
//   - Support for padding                                                    
//                                                                            
// Version 1.4 (24 March 2002)                                                
//   - Reading support for ID3v2.2.x & ID3v2.4.x tags                         
//                                                                            
// Version 1.3 (16 February 2002)                                             
//   - Fixed bug with property Comment                                        
//   - Added info: composer, encoder, copyright, language, link               
//                                                                            
// Version 1.2 (17 October 2001)                                              
//   - Writing support for ID3v2.3.x tags                                     
//   - Fixed bug with track number detection                                  
//   - Fixed bug with tag reading                                             
//                                                                            
// Version 1.1 (31 August 2001)                                               
//   - Added public void ResetData                                       
//                                                                            
// Version 1.0 (14 August 2001)                                               
//   - Reading support for ID3v2.3.x tags                                     
//   - Tag info: title, artist, album, track, year, genre, comment            
//                                                                            
// ***************************************************************************

using System;
using System.IO;
using System.Text;
using ATL.Logging;

namespace ATL.AudioReaders.BinaryLogic
{
	public class TID3v2 : MetaDataReader
	{
		public const byte TAG_VERSION_2_2 = 2;             // Code for ID3v2.2.x tag
		public const byte TAG_VERSION_2_3 = 3;             // Code for ID3v2.3.x tag
		public const byte TAG_VERSION_2_4 = 4;             // Code for ID3v2.4.x tag

		private bool FExists;
		private byte FVersionID;
		private int FSize;
		private String FTitle;
		private String FArtist;
		private String FAlbum;
		private ushort FTrack;
		private String FTrackString;
		private String FYear;
		private String FGenre;
		private String FComment;
		private String FComposer;
		private String FEncoder;
		private String FCopyright;
		private String FLanguage;
		private String FLink;
      
		public bool Exists // True if tag found
		{
			get { return this.FExists; }
		}
		public byte VersionID // Version code
		{
			get { return this.FVersionID; }
		}	
		public int Size // Total tag size
		{
			get { return this.FSize; }
		}
		public String Title // Song title
		{
			get { return this.FTitle; }
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
			get { return this.FTrack; }
			set { FSetTrack(value); }
		}
		public String TrackString // Track number (string)
		{ 
			get { return this.FTrackString; }
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
		public String Composer // Composer
		{
			get { return this.FComposer; }
			set { FSetComposer(value); }
		}		  
		public String Encoder // Encoder
		{
			get { return this.FEncoder; }
			set { FSetEncoder(value); }
		}		  
		public String Copyright // (c)
		{
			get { return this.FCopyright; }
			set { FSetCopyright(value); }
		}
		public String Language // Language
		{
			get { return this.FLanguage; }
			set { FSetLanguage(value); }
		}		  
		public String Link // URL link
		{
			get { return this.FLink; }
			set { FSetLink(value); }
		}

		// ID3v2 tag ID
		private const String ID3V2_ID = "ID3";

		// Max. number of supported tag frames
		private const byte ID3V2_FRAME_COUNT = 16;

		// Names of supported tag frames (ID3v2.3.x & ID3v2.4.x)
		private String[] ID3V2_FRAME_NEW = new String[ID3V2_FRAME_COUNT]
	{
		"TIT2", "TPE1", "TALB", "TRCK", "TYER", "TCON", "COMM", "TCOM", "TENC",
		"TCOP", "TLAN", "WXXX", "TDRC", "TOPE", "TIT1", "TOAL" };

		// Names of supported tag frames (ID3v2.2.x)
		private String[] ID3V2_FRAME_OLD = new String[ID3V2_FRAME_COUNT]
		{
			"TT2", "TP1", "TAL", "TRK", "TYE", "TCO", "COM", "TCM", "TEN",
			"TCR", "TLA", "WXX", "TOR", "TOA", "TT1", "TOT"};

		// Max. tag size for saving
		private const int ID3V2_MAX_SIZE = 4096;

		// Unicode ID
		public const char UNICODE_ID = (char)1;

		// Frame header (ID3v2.3.x & ID3v2.4.x)
		private class FrameHeaderNew 
		{
			public char[] ID = new char[4];                                // Frame ID
			public int Size;                                  // Size excluding header
			public ushort Flags;											  // Flags
		}

		// Frame header (ID3v2.2.x)
		private class FrameHeaderOld
		{
			public char[] ID = new char[3];                                // Frame ID
			public byte[] Size = new byte[3];                 // Size excluding header
		}

		// ID3v2 header data - for internal use
		private class TagInfo
		{
			// Real structure of ID3v2 header
			public char[] ID = new char[3];                            // Always "ID3"
			public byte Version;                                     // Version number
			public byte Revision;                                   // Revision number
			public byte Flags;                                         // Flags of tag
			public byte[] Size = new byte[4];             // Tag size excluding header
			// Extended data
			public int FileSize;		                          // File size (bytes)
			public String[] Frame = new String[ID3V2_FRAME_COUNT];
			// Information from frames
			public bool NeedRewrite;                        // Tag should be rewritten
			public int PaddingSize;                            // Padding size (bytes)
		}

		// ********************* Auxiliary functions & voids ********************

		private bool ReadHeader(String FileName, ref TagInfo Tag)
		{			
			bool result = true;
			FileStream fs = null;
			BinaryReader SourceFile = null;

			try
			{
				// Set read-access and open file		
				fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
				SourceFile = new BinaryReader(fs);

				// Read header and get file size
				Tag.ID = SourceFile.ReadChars(3);
				Tag.Version = SourceFile.ReadByte();
				Tag.Revision = SourceFile.ReadByte();
				Tag.Flags = SourceFile.ReadByte();
				Tag.Size = SourceFile.ReadBytes(4);

				//BlockRead(SourceFile, Tag, 10, Transferred);

				Tag.FileSize = (int)fs.Length;
					
				// if transfer is not complete
				//if Transferred < 10 then Result = false;
			} 
			catch (Exception e)
			{    
				System.Console.WriteLine(e.StackTrace);
				LogDelegator.GetLogDelegate()(Log.LV_ERROR,e.Message);
				result = false;
			}
		
			if (SourceFile != null) SourceFile.Close();
			if (fs != null) fs.Close();

			return result;
		}

		// ---------------------------------------------------------------------------

		private int GetTagSize(TagInfo Tag)
		{
			// Get total tag size
			int result =
				Tag.Size[0] * 0x200000 +
				Tag.Size[1] * 0x4000 +
				Tag.Size[2] * 0x80 + 
				Tag.Size[3] + 10;

			if (0x10 == (Tag.Flags & 0x10)) result += 10;
			if (result > Tag.FileSize) result = 0;

			return result;
		}

		// ---------------------------------------------------------------------------

		private void SetTagItem(String ID, String Data, ref TagInfo Tag)
		{		
			String FrameID;

			// Set tag item if supported frame found
			for (int iterator=0; iterator<ID3V2_FRAME_COUNT; iterator++)
			{
				if (Tag.Version > TAG_VERSION_2_2)
					FrameID = ID3V2_FRAME_NEW[iterator];
				else
					FrameID = ID3V2_FRAME_OLD[iterator];
    
				if ((ID == FrameID) /*&& (Data[0] <= UNICODE_ID)*/) // .NET reader should be able to handle unicode & UTF-8-encoded tags
					Tag.Frame[iterator] = Data;
			}
		}

		// ---------------------------------------------------------------------------

		private int Swap32(int Figure)
		{
			byte[] byteArray = new byte[4];		

			// The operations below are integer divisions : result should be an integer
			byteArray[0] = (byte)(Figure & 0xFF);
			byteArray[1] = (byte)((Figure & 0xFF00) / 0x100); 
			byteArray[2] = (byte)((Figure & 0xFF0000) / 0x10000); 
			byteArray[3] = (byte)((Figure & 0xFF000000) / 0x1000000); 

			// Swap 4 bytes
			return
				byteArray[0] * 0x1000000 +
				byteArray[1] * 0x10000 +
				byteArray[2] * 0x100 +
				byteArray[3];
		}

		// ---------------------------------------------------------------------------

		private void ReadFramesNew(String FileName, ref TagInfo Tag)
		{  
			FrameHeaderNew Frame  = new FrameHeaderNew();
			FileStream fs = null;
			BinaryReader SourceFile = null;
			char[] Data;
			long DataPosition;
			long DataSize;

			// Get information from frames (ID3v2.3.x & ID3v2.4.x)
			try
			{
				// Set read-access, open file
				fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
				SourceFile = new BinaryReader(fs);

				fs.Seek(10, SeekOrigin.Begin);
				while ( (fs.Position < GetTagSize(Tag)) && (fs.Position < fs.Length) ) 
				{
					//Array.Clear(Data,0,Data.Length);

					// Read frame header and check frame ID
					Frame.ID = SourceFile.ReadChars(4);
					Frame.Size = SourceFile.ReadInt32();
					Frame.Flags = SourceFile.ReadUInt16();    

					if ( ! ( Char.IsLetter(Frame.ID[0]) && Char.IsUpper(Frame.ID[0]) ) ) break;
				
					// Note data position and determine significant data size
					DataPosition = fs.Position;
					if ( Swap32(Frame.Size) > 500 ) DataSize = 500;
					else DataSize = Swap32(Frame.Size);
				
					byte[] bData = new byte[DataSize];
					Data = new char[DataSize];
					// Read frame data and set tag item if frame supported
					bData = SourceFile.ReadBytes((int)DataSize);
					Array.Copy(bData,Data,bData.Length);
					if ( 32768 != (Frame.Flags & 32768) ) SetTagItem(new String(Frame.ID), new String(Data), ref Tag); // Wipe out \0's to avoid string cuts
					fs.Seek(DataPosition + Swap32(Frame.Size), SeekOrigin.Begin);
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

		private void ReadFramesOld(String FileName, ref TagInfo Tag)
		{
			FrameHeaderOld Frame = new FrameHeaderOld();
			char [] Data = new char[500];
			long DataPosition;
			int FrameSize;
			int DataSize;

			// Get information from frames (ID3v2.2.x)
			try
			{
				// Set read-access, open file
				FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
				BinaryReader SourceFile = new BinaryReader(fs);

				fs.Seek(10, SeekOrigin.Begin);
				while ( (fs.Position < GetTagSize(Tag)) && ( fs.Position < fs.Length) ) 
				{
					Array.Clear(Data,0,Data.Length);				
				
					// Read frame header and check frame ID
					Frame.ID = SourceFile.ReadChars(3);
					Frame.Size = SourceFile.ReadBytes(3);				

					if ( ! ( Char.IsLetter(Frame.ID[0]) && Char.IsUpper(Frame.ID[0]) ) ) break;

					// Note data position and determine significant data size
					DataPosition = fs.Position;
					FrameSize = (Frame.Size[0] << 16) + (Frame.Size[1] << 8) + Frame.Size[2];
					if ( FrameSize > 500 ) DataSize = 500;
					else DataSize = FrameSize;
				
					// Read frame data and set tag item if frame supported
					Data = SourceFile.ReadChars(DataSize);				
					SetTagItem( new String(Frame.ID), new String(Data), ref Tag);
					fs.Seek(DataPosition + FrameSize, SeekOrigin.Begin);
				}
				SourceFile.Close();
				fs.Close();
			} 
			catch (Exception e)
			{
				System.Console.WriteLine(e.StackTrace);
			}
		}

		// ---------------------------------------------------------------------------

		private String GetANSI(String Source)
		{			
			byte FirstByte;
			byte SecondByte;
			char UnicodeChar;
			String result = Source;			

			if (null == Source) result = "";				
			// Convert string from unicode if needed and trim spaces
			if ((Source != null) && (Source.Length > 0) && (UNICODE_ID == Source[0]))
			{
				result = "";
				/*UnicodeEncoding uEnc = new UnicodeEncoding();
				result = uEnc.GetString(Encoding.Default.GetBytes(Source.Substring(1,Source.Length-1)));
				*/
				
				for (int index=0; (index * 2 + 2) < Source.Length; index++)
				{
					FirstByte = (byte)(Source[(index * 2) + 1]);
					SecondByte = (byte)(Source[(index * 2) + 2]);
					UnicodeChar = (char)(FirstByte | (SecondByte << 8));					
					if (0 == UnicodeChar) break;
					if (FirstByte < 0xFF) result = result + UnicodeChar;
				}
			}
			else
				if (result.Length >0)
					result = result.Substring(1,result.Length-1);

			return result.Trim();
		}

		// ---------------------------------------------------------------------------

		private String GetContent(String Content1, String Content2)
		{
			// Get content preferring the first content
			String result = GetANSI(Content1);
			if ("" == result) result = GetANSI(Content2);
			return result;
		}

		// ---------------------------------------------------------------------------

		private ushort ExtractTrack(String  TrackString)
		{
			String Track;
			int Index;
			int Value;		

			if (null == TrackString) return 0;

			// Extract track from string
			Track = GetANSI(TrackString);
			Index = Track.IndexOf('/');
  
			try
			{
				if (-1 == Index) Value = Int32.Parse(Track);
				else Value = Int32.Parse(Track.Substring(0, Index));
			}
			catch
			{				
				return 0;
			}
			return (ushort)Value;  
		}

		// ---------------------------------------------------------------------------

		private String ExtractYear(String YearString, String DateString)
		{
			// Extract year from strings
			String result = GetANSI(YearString);
			if ("" == result) 
			{
				result = GetANSI(DateString);
				if (result != "") result = result.Substring(0,4);
			}
			return result;
		}

		// ---------------------------------------------------------------------------

		private String ExtractGenre(String GenreString)
		{
			if (null == GenreString) return "";

			// Extract genre from string
			String result = GetANSI(GenreString);
			if ( result.IndexOf(')') > 0 ) 
			{
				int genreIndex = -1;
				try
				{
					genreIndex = Int32.Parse( result.Substring(result.IndexOf('(')+1,result.LastIndexOf(')')-result.IndexOf('(')-1) );
				}
				catch {}

				result = result.Remove(0, result.LastIndexOf(')')+1 );
				if (("" == result) && (genreIndex != -1) && (genreIndex < TID3v1.MusicGenre.Length)) result = TID3v1.MusicGenre[genreIndex];
			}
			return result;
		}

		// ---------------------------------------------------------------------------

		private String ExtractText(String SourceString, bool LanguageID)
		{
			String Source;
			String Separator;
			char EncodingID;

			// Extract significant text data from a complex field
			Source = SourceString;
			String result = "";

			if ( (Source != null) && (Source.Length > 0) )
			{
				EncodingID = Source[0];
				char[] tempArray = new char[2] { (char)0, (char)0 };
				if (UNICODE_ID == EncodingID) Separator = new String(tempArray);
				else Separator = "\0";
    
				if (LanguageID) Source = Source.Remove(0, 4);
				else Source = Source.Remove(0, 1);

				Source = Source.Remove(0, Source.IndexOf(Separator) + Separator.Length - 1);
				result = GetANSI(EncodingID + Source);
			}
			return result;
		}

		// ---------------------------------------------------------------------------

		private void BuildHeader(ref TagInfo Tag)
		{  
			int TagSize;

			// Calculate new tag size (without padding)
			TagSize = 10;
			for (int iterator=0; iterator < ID3V2_FRAME_COUNT; iterator++)
				if ( "" != Tag.Frame[iterator] )
					TagSize += Tag.Frame[iterator].Length + 11;
	
			// Check for ability to change existing tag
			Tag.NeedRewrite = (
				(! Utils.StringEqualsArr(ID3V2_ID, Tag.ID) ) ||
				(GetTagSize(Tag) < TagSize) ||
				(GetTagSize(Tag) > ID3V2_MAX_SIZE) );

			// Calculate padding size and set padded tag size
			if ( Tag.NeedRewrite )Tag.PaddingSize = ID3V2_MAX_SIZE - TagSize;
			else Tag.PaddingSize = GetTagSize(Tag) - TagSize;
  
			if (Tag.PaddingSize > 0) TagSize += Tag.PaddingSize;

			// Build tag header
			Tag.ID = ID3V2_ID.ToCharArray();
			Tag.Version = TAG_VERSION_2_3;
			Tag.Revision = 0;
			Tag.Flags = 0;

			// Convert tag size
			for (int iterator=0; iterator <4; iterator++)
				Tag.Size[iterator] = (byte)( ((TagSize - 10) >> ((3 - iterator) * 7)) & 0x7F );
		}

		// ---------------------------------------------------------------------------

		private bool ReplaceTag(String FileName, BinaryReader TagData)
		{
			// Replace old tag with new tag data
			bool result = false;
  
			if ( (! File.Exists(FileName) )  ) return result;
			try
			{
				File.SetAttributes(FileName, FileAttributes.Normal);
				TagData.BaseStream.Position = 0;

				FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.ReadWrite);
				BinaryWriter TargetFile = new BinaryWriter(fs);
	
				TargetFile.Write(TagData.ReadChars((int)TagData.BaseStream.Length));		
		
				TargetFile.Close();
				fs.Close();
				result = true;
			} 
			catch (Exception e)
			{
				System.Console.WriteLine(e.StackTrace);
				result = false;
			}
			return result;
		}

		// ---------------------------------------------------------------------------

		private bool RebuildFile(String FileName, BinaryReader TagData)
		{
			TagInfo Tag = new TagInfo();
			String BufferName = FileName + "~";
			bool result = false;

			FileStream fss = null;
			BinaryReader SourceFile = null;

			FileStream fst = null;
			BinaryWriter TargetFile = null;


			// Rebuild file with old file data and new tag data (optional)
  
			if (! File.Exists(FileName)) return result;
			try
			{	
				File.SetAttributes(FileName, FileAttributes.Normal);

				if ( ! ReadHeader(FileName, ref Tag) ) return result;
				if ( (null == TagData) && Utils.StringEqualsArr(ID3V2_ID, Tag.ID) ) return result;
	
		
				// Create file streams			
	
				// Set read-access and open file		
				fss = new FileStream(FileName, FileMode.Open, FileAccess.Read);
				SourceFile = new BinaryReader(fss);

				fst = new FileStream(BufferName, FileMode.CreateNew, FileAccess.Write);
				TargetFile = new BinaryWriter(fst);
	
				// Copy data blocks
				if ( Utils.StringEqualsArr(ID3V2_ID,Tag.ID) ) SourceFile.BaseStream.Seek(GetTagSize(Tag), SeekOrigin.Begin);
				if ( TagData != null ) 
				{
					TagData.BaseStream.Seek(0, SeekOrigin.Begin);
					TargetFile.Write(TagData.ReadChars((int)TagData.BaseStream.Length));
				}
				TargetFile.Write(SourceFile.ReadChars((int)(SourceFile.BaseStream.Length - SourceFile.BaseStream.Position)));
				// Replace old file and delete temporary file
				File.Delete(FileName);
				File.Move(BufferName, FileName);
				result = true;
			} 
			catch (Exception e) 
			{
				System.Console.WriteLine(e.StackTrace);
				// Access error
				if ( File.Exists(BufferName) ) File.Delete(BufferName);
			}
			if (TargetFile != null) TargetFile.Close();
			if (SourceFile != null) SourceFile.Close();
			if (fst != null) fst.Close();
			if (fss != null) fss.Close();

			return result;
		}

		// ---------------------------------------------------------------------------

		private bool SaveTag(String FileName, TagInfo Tag)
		{  		
			int FrameSize;
			byte[] Padding = new byte[ID3V2_MAX_SIZE];
			bool result;

			// Build and write tag header and frames to stream	
			MemoryStream TagData = new MemoryStream();
			BinaryWriter wTag = new BinaryWriter(TagData);
			BinaryReader rTag = new BinaryReader(TagData);

			BuildHeader(ref Tag);
			wTag.Write(Tag.ID);
			wTag.Write(Tag.Version);
			wTag.Write(Tag.Revision);
			wTag.Write(Tag.Flags);
			wTag.Write(Tag.Size);			  

			for (int iterator=0;iterator< ID3V2_FRAME_COUNT; iterator++)
				if ( Tag.Frame[iterator] != "" )
				{
					wTag.Write(ID3V2_FRAME_NEW[iterator]);
					FrameSize = Swap32(Tag.Frame[iterator].Length + 1);
					wTag.Write(FrameSize);
					wTag.Write('\0'+'\0'+'\0' + Tag.Frame[iterator]);
				}
			// Add padding
			Array.Clear(Padding,0,Padding.Length);
			if (Tag.PaddingSize > 0) wTag.Write(Padding, 0, Tag.PaddingSize);

			// Rebuild file or replace tag with new tag data
			if ( Tag.NeedRewrite ) result = RebuildFile(FileName, rTag);
			else result = ReplaceTag(FileName, rTag);

			wTag.Close();
			rTag.Close();
			TagData.Close();

			return result;
		}

		// ********************** Private functions & voids *********************

		private void FSetTitle(String newTrack)
		{
			// Set song title
			FTitle = newTrack.Trim();
		}

		// ---------------------------------------------------------------------------

		private void FSetArtist(String NewArtist)
		{
			// Set artist name
			FArtist = NewArtist.Trim();
		}

		// ---------------------------------------------------------------------------

		private void FSetAlbum(String NewAlbum)
		{
			// Set album title
			FAlbum = NewAlbum.Trim();
		}

		// ---------------------------------------------------------------------------

		private void FSetTrack(ushort NewTrack)
		{
			// Set track number
			FTrack = NewTrack;
		}

		// ---------------------------------------------------------------------------

		private void FSetYear(String NewYear)
		{
			// Set release year
			FYear = NewYear.Trim();
		}

		// ---------------------------------------------------------------------------

		private void FSetGenre(String NewGenre)
		{
			// Set genre name
			FGenre = NewGenre.Trim();
		}

		// ---------------------------------------------------------------------------

		private void FSetComment(String NewComment)
		{
			// Set comment
			FComment = NewComment.Trim();
		}

		// ---------------------------------------------------------------------------
	
		private void FSetComposer(String NewComposer)
		{
			// Set composer name
			FComposer = NewComposer.Trim();
		}

		// ---------------------------------------------------------------------------

		private void FSetEncoder(String NewEncoder)
		{
			// Set encoder name
			FEncoder = NewEncoder.Trim();
		}

		// ---------------------------------------------------------------------------

		private void FSetCopyright(String NewCopyright)
		{
			// Set copyright information
			FCopyright = NewCopyright.Trim();
		}

		// ---------------------------------------------------------------------------

		private void FSetLanguage(String NewLanguage)
		{
			// Set language
			FLanguage = NewLanguage.Trim();
		}

		// ---------------------------------------------------------------------------

		private void FSetLink(String NewLink)
		{
			// Set URL link
			FLink = NewLink.Trim();
		}

		// ********************** Public functions & voids **********************

		public TID3v2()
		{  
			ResetData();
		}

		// ---------------------------------------------------------------------------

		public void ResetData()
		{
			// Reset all variables
			FExists = false;
			FVersionID = 0;
			FSize = 0;
			FTitle = "";
			FArtist = "";
			FAlbum = "";
			FTrack = 0;
			FTrackString = "";
			FYear = "";
			FGenre = "";
			FComment = "";
			FComposer = "";
			FEncoder = "";
			FCopyright = "";
			FLanguage = "";
			FLink = "";
		}

		// ---------------------------------------------------------------------------

		public bool ReadFromFile(String FileName)
		{
			TagInfo Tag = new TagInfo();

			// Reset data and load header from file to variable
			ResetData();
			bool result = ReadHeader(FileName, ref Tag);
  
			// Process data if loaded and header valid
			if ( (result) && Utils.StringEqualsArr(ID3V2_ID,Tag.ID) )
			{
				FExists = true;
				// Fill properties with header data
				FVersionID = Tag.Version;
				FSize = GetTagSize(Tag);
				// Get information from frames if version supported
				if ( (TAG_VERSION_2_2 <= FVersionID) && (FVersionID <= TAG_VERSION_2_4) && (FSize > 0) ) 
				{
					if (FVersionID > TAG_VERSION_2_2) ReadFramesNew(FileName, ref Tag);
					else ReadFramesOld(FileName, ref Tag);

					FTitle = GetContent(Tag.Frame[0], Tag.Frame[14]);
					FArtist = GetContent(Tag.Frame[1], Tag.Frame[13]);
					FAlbum = GetContent(Tag.Frame[2], Tag.Frame[15]);
					FTrack = ExtractTrack(Tag.Frame[3]);
					FTrackString = GetANSI(Tag.Frame[3]);
					FYear = ExtractYear(Tag.Frame[4], Tag.Frame[12]);
					FGenre = ExtractGenre(Tag.Frame[5]);
					FComment = ExtractText(Tag.Frame[6], true);
					FComposer = GetANSI(Tag.Frame[7]);
					FEncoder = GetANSI(Tag.Frame[8]);
					FCopyright = GetANSI(Tag.Frame[9]);
					FLanguage = GetANSI(Tag.Frame[10]);
					FLink = ExtractText(Tag.Frame[11], false);
				}
			}

			return result;
		}

		// ---------------------------------------------------------------------------

		public bool SaveToFile(String FileName)
		{
			TagInfo Tag = new TagInfo();

			// Check for existing tag  
			Array.Clear(Tag.ID,0,Tag.ID.Length);
			Tag.Version = 0;
			Tag.Revision = 0;
			Tag.Flags = 0;
			Array.Clear(Tag.Size,0,Tag.Size.Length);
			Tag.FileSize = 0;
			Array.Clear(Tag.Frame,0,Tag.Frame.Length);
			Tag.NeedRewrite = false;
			Tag.PaddingSize = 0;	
	
			ReadHeader(FileName, ref Tag);
			// Prepare tag data and save to file
			Tag.Frame[0] = FTitle;
			Tag.Frame[1] = FArtist;
			Tag.Frame[2] = FAlbum;
			if (FTrack > 0) Tag.Frame[3] = FTrack.ToString();
			Tag.Frame[4] = FYear;
			Tag.Frame[5] = FGenre;
			if (FComment != "") Tag.Frame[6] = "eng" + '\0' + FComment;
			Tag.Frame[7] = FComposer;
			Tag.Frame[8] = FEncoder;
			Tag.Frame[9] = FCopyright;
			Tag.Frame[10] = FLanguage;
			if (FLink != "") Tag.Frame[11] = '\0' + FLink;
		
			return SaveTag(FileName, Tag);
		}

		// ---------------------------------------------------------------------------

		public bool RemoveFromFile(String FileName)
		{
			// Remove tag from file
			return RebuildFile(FileName, null);
		}

	}
}