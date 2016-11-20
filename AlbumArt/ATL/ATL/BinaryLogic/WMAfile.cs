// ***************************************************************************
//                                                                            
// Audio Tools Library (Freeware)                                             
// Class TWMAfile - for extracting information from WMA file header           
//                                                                            
// Copyright (c) 2001,2002 by Jurgen Faul                                     
// E-mail: jfaul@gmx.de                                                       
// http://jfaul.de/atl                                                        
//
// Version 1.1 (19th August 2005)
//	 - Support for WMA 9 (FormatTag)
//	 - Fixed a buf that prevented extended metadata to be read properly
//
// 14th May 2005 - Translated to C# by Zeugma 440
//                                                                            
// Version 1.0 (29 April 2002)                                                
//   - Support for Windows Media Audio (versions 7, 8)                        
//   - File info: file size, channel mode, sample rate, duration, bit rate    
//   - WMA tag info: title, artist, album, track, year, genre, comment        
//                                                                            
// ***************************************************************************

using System;
using System.IO;
using ATL.Logging;

namespace ATL.AudioReaders.BinaryLogic
{
	class TWMAfile : AudioDataReader, MetaDataReader
	{
		// Channel modes
		public const byte WMA_CM_UNKNOWN = 0;                                               // Unknown
		public const byte WMA_CM_MONO = 1;                                                     // Mono
		public const byte WMA_CM_STEREO = 2;                                                 // Stereo

		// Channel mode names
		public String[] WMA_MODE = new String[3] {"Unknown", "Mono", "Stereo"};

		private bool FValid;
		private int FFileSize;
		private byte FChannelModeID;
		private int FSampleRate;
		private double FDuration;
		private int FBitRate;
		private bool FIsVBR;
		private bool FIsLossless;
		private String FTitle;
		private String FArtist;
		private String FAlbum;
		private int FTrack;
		private String FYear;
		private String FGenre;
		private String FComment;
				
		public TAPEtag APEtag // No APEtag here; declared for interface compliance
		{
			get{ return new TAPEtag(); }
		}
		public TID3v1 ID3v1 // No ID3v1 here; declared for interface compliance
		{
			get{ return new TID3v1(); }
		}
		public TID3v2 ID3v2 // No ID3v2 here; declared for interface compliance
		{
			get{ return new TID3v2(); }
		}
		public bool Valid // True if valid data
		{
			get { return this.FValid; }
		}
		public bool Exists // for compatibility with other tag readers
		{
			get { return true; }
		}
		public int FileSize // File size (bytes)
		{
			get { return this.FFileSize; }
		}	
		public byte ChannelModeID // Channel mode code
		{
			get { return this.FChannelModeID; }
		}	
		public String ChannelMode // Channel mode name
		{
			get { return this.FGetChannelMode(); }
		}
		public int SampleRate // Sample rate (hz)
		{
			get { return this.FSampleRate; }
		}	
		public double Duration // Duration (seconds)
		{
			get { return this.FDuration; }
		}	
		public double BitRate // Bit rate (kbit)
		{
			get { return (double)this.FBitRate; }
		}
		public bool IsVBR
		{
			get { return this.FIsVBR; }
		}
		public int CodecFamily
		{
			get 
			{ 
				if (FIsLossless)
				{
					return AudioReaderFactory.CF_LOSSLESS;
				}
				else
				{
					return AudioReaderFactory.CF_LOSSY;
				}
			}
		}
		public bool IsStreamed
		{
			get { return true; }
		}
		public String Title // Song title
		{
			get { return this.FTitle; }
		}	
		public String Artist // Artist name
		{
			get { return this.FArtist; }
		}
		public String Album // Album name
		{
			get { return this.FAlbum; }
		}	
		public ushort Track // Track number
		{
			get { return (ushort)this.FTrack; }
		}	
		public String Year // Year
		{
			get { return this.FYear; }
		}	
		public String Genre // Genre name
		{
			get { return this.FGenre; }
		}	
		public String Comment // Comment
		{
			get { return this.FComment; }
		}
			
		// Object IDs
		private char[] WMA_HEADER_ID = new char[16] { (char)48,(char)38,(char)178,(char)117,(char)142,(char)102,(char)207,(char)17,(char)166,(char)217,(char)0,(char)170,(char)0,(char)98,(char)206,(char)108 };
		private char[] WMA_FILE_PROPERTIES_ID = new char[16] { (char)161,(char)220,(char)171,(char)140,(char)71,(char)169,(char)207,(char)17,(char)142,(char)228,(char)0,(char)192,(char)12,(char)32,(char)83,(char)101 };
		private char[] WMA_STREAM_PROPERTIES_ID = new char[16] { (char)145,(char)7,(char)220,(char)183,(char)183,(char)169,(char)207,(char)17,(char)142,(char)230,(char)0,(char)192,(char)12,(char)32,(char)83,(char)101 };
		private char[] WMA_CONTENT_DESCRIPTION_ID = new char[16] { (char)51,(char)38,(char)178,(char)117,(char)142,(char)102,(char)207,(char)17,(char)166,(char)217,(char)0,(char)170,(char)0,(char)98,(char)206,(char)108 };
		private char[] WMA_EXTENDED_CONTENT_DESCRIPTION_ID = new char[16] { (char)64,(char)164,(char)208,(char)210,(char)7,(char)227,(char)210,(char)17,(char)151,(char)240,(char)0,(char)160,(char)201,(char)94,(char)168,(char)80 };

		// Format IDs
		private const int WMA_ID				= 0x161;
		private const int WMA_PRO_ID			= 0x162;
		private const int WMA_LOSSLESS_ID		= 0x163;
		private const int WMA_GSM_CBR_ID		= 0x7A21;
		private const int WMA_GSM_VBR_ID		= 0x7A22;

		// Max. number of supported comment fields
		private const byte WMA_FIELD_COUNT = 7;

		// Names of supported comment fields
		private String[] WMA_FIELD_NAME = new String[WMA_FIELD_COUNT] 
{
	"WM/TITLE", "WM/AUTHOR", "WM/ALBUMTITLE", "WM/TRACK", "WM/YEAR",
	"WM/GENRE", "WM/DESCRIPTION" };

		// Max. number of characters in tag field
		private const byte WMA_MAX_STRING_SIZE = 250;

		/*
		// Object ID
		ObjectID = array [1..16] of Char;

		  // Tag data
		TagData == array [1..WMA_FIELD_COUNT] of WideString;
		*/

		// File data - for internal use
		private class FileData
		{
			public int FormatTag;										// Format ID tag
			public int FileSize;                                    // File size (bytes)
			public int MaxBitRate;                                // Max. bit rate (bps)
			public ushort Channels;                                // Number of channels
			public int SampleRate;                                   // Sample rate (hz)
			public int ByteRate;                                            // Byte rate
			public String[] Tag = new String[WMA_FIELD_COUNT];    // WMA tag information
			public void Reset()
			{
				FormatTag = 0;
				FileSize = 0;
				MaxBitRate = 0;
				Channels = 0;
				SampleRate = 0;
				ByteRate = 0;
				for (int i=0; i<Tag.Length; i++) Tag[i] = "";
			}
		}

		// ********************* Auxiliary functions & voids ********************

		private String ReadFieldString(BinaryReader Source, ushort DataSize)
		{		
			int StringSize;
			byte[] FieldData = new byte[WMA_MAX_STRING_SIZE * 2];

			// Read field data and convert to Unicode string
			String result = "";
			StringSize = Math.Max(0,(DataSize / 2)-1); // -1 to ignore the last null character, unused in C#

			if (StringSize > WMA_MAX_STRING_SIZE) StringSize = WMA_MAX_STRING_SIZE;
			FieldData = Source.ReadBytes(StringSize * 2);
			Source.BaseStream.Seek(DataSize - StringSize * 2, SeekOrigin.Current);
			for (int iterator=0; iterator<StringSize; iterator++)
				result = result + (char)( FieldData[iterator * 2] + ((byte)FieldData[(iterator * 2)+1] << 8) );

			return result;
		}

		// ---------------------------------------------------------------------------

		private void ReadTagStandard(BinaryReader Source, ref String[] Tag)
		{		
			ushort[] FieldSize = new ushort[5];
			String FieldValue;

			// Read standard tag data
			for (int i=0;i<5;i++)
				FieldSize[i] = Source.ReadUInt16();
  
			for (int iterator=0; iterator<5; iterator++)
				if (FieldSize[iterator] > 0 )
				{
					// Read field value
					FieldValue = ReadFieldString(Source, FieldSize[iterator]);
					// Set corresponding tag field if supported
					switch(iterator)
					{
						case 0: Tag[0] = FieldValue; break;
						case 1: Tag[1] = FieldValue; break;
						case 3: Tag[6] = FieldValue; break;
					}
				}
		}

		// ---------------------------------------------------------------------------

		private void ReadTagExtended(BinaryReader Source, ref String[] Tag)
		{
			ushort FieldCount;
			ushort DataSize;
			ushort DataType;
			String FieldName;
			String FieldValue = "";

			// Read extended tag data
			FieldCount = Source.ReadUInt16();
			for (int iterator1=0; iterator1 < FieldCount; iterator1++)
			{
				// Read field name
				DataSize = Source.ReadUInt16();
				FieldName = ReadFieldString(Source, DataSize);
				// Read value data type
				DataType = Source.ReadUInt16();
				DataSize = Source.ReadUInt16();
				
				// Read field value only if string (<-> DataType=0)
				// NB : DataType = 1
				if (0 == DataType) // Unicode string
				{
					FieldValue = ReadFieldString(Source, DataSize);
				}
				if (1 == DataType) // Byte array; not useful here
				{
					Source.BaseStream.Seek(DataSize, SeekOrigin.Current);
				}
				if (2 == DataType) // 32-bit Boolean; not useful here
				{
					Source.BaseStream.Seek(DataSize, SeekOrigin.Current);
				}
				if (3 == DataType) // 32-bit unsigned integer
				{
					FieldValue = (Source.ReadUInt32()+1).ToString();
				}
				if (4 == DataType) // 64-bit unsigned integer
				{
					FieldValue = Source.ReadUInt64().ToString();
				}
				if (4 == DataType) // 16-bit unsigned integer
				{
					FieldValue = Source.ReadUInt16().ToString();
				}					

				// Set corresponding tag field if supported
				for (int iterator2=0; iterator2<WMA_FIELD_COUNT; iterator2++)
					if ( WMA_FIELD_NAME[iterator2] == FieldName.Trim().ToUpper() )
					{
						Tag[iterator2] = FieldValue;
					}
			}
		}

		// ---------------------------------------------------------------------------

		private void ReadObject(char[] ID, BinaryReader Source, ref FileData Data)
		{
			// Read data from header object if supported
			if ( Utils.ArrEqualsArr(WMA_FILE_PROPERTIES_ID,ID) )
			{
				// Read file properties
				Source.BaseStream.Seek(80, SeekOrigin.Current);
				Data.MaxBitRate = Source.ReadInt32();
			}
			if ( Utils.ArrEqualsArr(WMA_STREAM_PROPERTIES_ID,ID) )
			{
				// Read stream properties
				Source.BaseStream.Seek(58, SeekOrigin.Current);
				Data.FormatTag = Source.ReadUInt16();
				Data.Channels = Source.ReadUInt16();
				Data.SampleRate = Source.ReadInt32();
				Data.ByteRate = Source.ReadInt32();    
			}
			if ( Utils.ArrEqualsArr(WMA_CONTENT_DESCRIPTION_ID,ID) )
			{
				// Read standard tag data
				Source.BaseStream.Seek(4, SeekOrigin.Current);
				ReadTagStandard(Source, ref Data.Tag);
			}
			if ( Utils.ArrEqualsArr(WMA_EXTENDED_CONTENT_DESCRIPTION_ID,ID) )
			{
				// Read extended tag data
				Source.BaseStream.Seek(4, SeekOrigin.Current);
				ReadTagExtended(Source, ref Data.Tag);
			}
		}

		// ---------------------------------------------------------------------------

		private bool ReadData(String FileName, ref FileData Data)
		{ 
			FileStream fs = null;
			BinaryReader Source = null;
			char[] ID = new char[16];
			int ObjectCount;
			int ObjectSize;
			long Position;

			bool result;

			// Read file data
			try
			{
				fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
				fs.Lock(0,fs.Length);
				Source = new BinaryReader(fs);

				Data.FileSize = (int)fs.Length;
    
				// Check for existing header
				ID = Utils.ReadTrueChars(Source,16);

				if ( Utils.ArrEqualsArr(WMA_HEADER_ID,ID) )
				{
					fs.Seek(8, SeekOrigin.Current);
					ObjectCount = Source.ReadInt32();		  
					fs.Seek(2, SeekOrigin.Current);
					// Read all objects in header and get needed data
					for (int iterator=0; iterator<ObjectCount; iterator++)
					{
						Position = fs.Position;
						ID = Utils.ReadTrueChars(Source,16);
						ObjectSize = Source.ReadInt32();			
						ReadObject(ID, Source, ref Data);
						fs.Seek(Position + ObjectSize, SeekOrigin.Begin);				
					}
				}
				result = true;
			} 
			catch (Exception e) 
			{
				System.Console.WriteLine(e.StackTrace);
				LogDelegator.GetLogDelegate()(Log.LV_ERROR, e.Message);
				result = false;
			}
			if (fs != null)
			{
				fs.Unlock(0,fs.Length);
				fs.Close();
			}
			return result;
		}

		// ---------------------------------------------------------------------------

		private bool IsValid(FileData Data)
		{
			// Check for data validity
			return (
				(Data.MaxBitRate > 0) && (Data.MaxBitRate < 320000) &&
				((Data.Channels == WMA_CM_MONO) || (Data.Channels == WMA_CM_STEREO)) &&
				(Data.SampleRate >= 8000) && (Data.SampleRate <= 96000) &&
				(Data.ByteRate > 0) && (Data.ByteRate < 40000) );
		}

		// ---------------------------------------------------------------------------

		private int ExtractTrack(String TrackString)
		{  
			// Extract track from string
			int result = 0;
			try
			{
				result = Int32.Parse(TrackString);
			} 
			catch
			{			
				result = 0;
			}
	
			return result;
		}

		// ********************** Private functions & voids *********************

		private void FResetData()
		{
			// Reset variables
			FValid = false;
			FFileSize = 0;
			FChannelModeID = WMA_CM_UNKNOWN;
			FSampleRate = 0;
			FDuration = 0;
			FBitRate = 0;
			FIsVBR = false;
			FIsLossless = false;
			FTitle = "";
			FArtist = "";
			FAlbum = "";
			FTrack = 0;
			FYear = "";
			FGenre = "";
			FComment = "";
		}

		// ---------------------------------------------------------------------------

		private String FGetChannelMode()
		{
			// Get channel mode name
			return WMA_MODE[FChannelModeID];
		}

		// ********************** Public functions & voids **********************

		public TWMAfile()
		{
			// Create object  
			FResetData();
		}

		// ---------------------------------------------------------------------------

		public bool ReadFromFile(String FileName)
		{
			FileData Data = new FileData();

			// Reset variables and load file data
			FResetData();
			Data.Reset();

			bool result = ReadData(FileName, ref Data);

			// Process data if loaded and valid
			if ( result && IsValid(Data) )
			{
				FValid = true;
				// Fill properties with loaded data
				FFileSize = Data.FileSize;
				FChannelModeID = (byte)Data.Channels;
				FSampleRate = Data.SampleRate;
				FDuration = Data.FileSize * 8 / Data.MaxBitRate;
				FBitRate = Data.ByteRate * 8 / 1000;
				FIsVBR = (WMA_GSM_VBR_ID == Data.FormatTag);
				FIsLossless = (WMA_LOSSLESS_ID == Data.FormatTag);
				FTitle = Data.Tag[0].Trim();
				FArtist = Data.Tag[1].Trim();
				FAlbum = Data.Tag[2].Trim();
				FTrack = ExtractTrack(Data.Tag[3].Trim());
				FYear = Data.Tag[4].Trim();
				FGenre = Data.Tag[5].Trim();
				FComment = Data.Tag[6].Trim();
			}
	
			return result;
		}
	}
}