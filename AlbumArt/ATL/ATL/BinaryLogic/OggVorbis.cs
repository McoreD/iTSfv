// ***************************************************************************
//                                                                            
// Audio Tools Library (Freeware)                                             
// Class TOggVorbis - for manipulating with Ogg Vorbis file information       
//                                                                            
// Copyright (c) 2001,2002 by Jurgen Faul                                     
// E-mail: jfaul@gmx.de                                                       
// http://jfaul.de/atl                                                        
//
// 11th May 2005 - Translated to C# by Zeugma 440
//                                                                            
// Version 1.7 (20 August 2003) by Madah                                      
//   - Minor fix: changed FSampleRate into Integer                            
//     ... so that samplerates>65535 works.                                   
//                                                                            
// Version 1.6 (2 October 2002)                                               
//   - Writing support for Vorbis tag                                         
//   - Changed several properties                                             
//   - Fixed bug with long Vorbis tag fields                                  
//                                                                            
// Version 1.2 (18 February 2002)                                             
//   - Added property BitRateNominal                                          
//   - Fixed bug with Vorbis tag fields                                       
//                                                                            
// Version 1.1 (21 October 2001)                                              
//   - Support for UTF-8                                                      
//   - Fixed bug with vendor info detection                                   
//                                                                            
// Version 1.0 (15 August 2001)                                               
//   - File info: file size, channel mode, sample rate, duration, bit rate    
//   - Vorbis tag: title, artist, album, track, date, genre, comment, vendor  
//                                                                            
// ***************************************************************************

using System;
using System.IO;

namespace ATL.AudioReaders.BinaryLogic
{
	class TOggVorbis : AudioDataReader, MetaDataReader
	{
		// Used with ChannelModeID property
		private const byte VORBIS_CM_MONO = 1;				// Code for mono mode		
		private const byte VORBIS_CM_STEREO = 2;			// Code for stereo mode
		private const byte VORBIS_CM_MULTICHANNEL = 6;		// Code for Multichannel Mode

		// Channel mode names
		private String[] VORBIS_MODE = new String[4]
		{"Unknown", "Mono", "Stereo", "Multichannel"};
    
		private int FFileSize;
		private byte FChannelModeID;
		private int FSampleRate;
		private ushort FBitRateNominal;
		private int FSamples;
		private int FID3v2Size;
		private String FTitle;
		private String FArtist;
		private String FAlbum;
		private ushort FTrack;
		private String FDate;
		private String FGenre;
		private String FComment;
		private String FVendor;
      
		public TAPEtag APEtag // No APEtag here; declared for leveling
		{
			get{ return new TAPEtag(); }
		}
		public TID3v1 ID3v1 // No ID3v1 here; declared for leveling
		{
			get{ return new TID3v1(); }
		}
		public TID3v2 ID3v2 // No ID3v2 here; declared for leveling
		{
			get{ return new TID3v2(); }
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
		public ushort BitRateNominal // Nominal bit rate
		{
			get { return this.FBitRateNominal; }
		}
		public bool IsVBR
		{
			get { return true; }
		}
		public int CodecFamily
		{
			get { return AudioReaderFactory.CF_LOSSY; }
		}
		public String Title // Song title
		{
			get { return this.FTitle; }
			set { this.FTitle = value; }
		}	
		public String Artist // Artist name
		{
			get { return this.FArtist; }
			set { this.FArtist = value; }
		}			  
		public String Album // Album name
		{
			get { return this.FAlbum; }
			set { this.FAlbum = value; }
		}			  
		public ushort Track // Track number
		{
			get { return this.FTrack; }
			set { this.FTrack = value; }
		}			  
		public String Year // Year
		{
			get { return this.FDate; }
			set { this.FDate = value; }
		}			  
		public String Genre // Genre name
		{
			get { return this.FGenre; }
			set { this.FGenre = value; }
		}			  
		public String Comment // Comment
		{
			get { return this.FComment; }
			set { this.FComment = value; }
		}	
		public String Vendor // Vendor string
		{
			get { return this.FVendor; }
		}	
		public double Duration // Duration (seconds)
		{
			get { return this.FGetDuration(); }
		}	
		public double BitRate // Average bit rate
		{
			get { return this.FGetBitRate(); }
		}	
		public bool Exists // True if ID3v2 tag exists
		{
			get { return this.FHasID3v2(); }
		}	
		public bool Valid // True if file valid
		{
			get { return this.FIsValid(); }
		}	
  
		// Ogg page header ID
		private const String OGG_PAGE_ID = "OggS";

		// Vorbis parameter frame ID
		private String VORBIS_PARAMETERS_ID = (char)1 + "vorbis";

		// Vorbis tag frame ID
		private String VORBIS_TAG_ID = (char)3 + "vorbis";

		// Max. number of supported comment fields
		private const sbyte VORBIS_FIELD_COUNT = 9;

		// Names of supported comment fields
		private String[] VORBIS_FIELD = new String[VORBIS_FIELD_COUNT] 
			{
				"TITLE", "ARTIST", "ALBUM", "TRACKNUMBER", "DATE", "GENRE", "COMMENT",
				"PERFORMER", "DESCRIPTION"};

		// CRC table for checksum calculating
		private uint[] CRC_TABLE = new uint[(0xFF)+1] {
														  0x00000000, 0x04C11DB7, 0x09823B6E, 0x0D4326D9, 0x130476DC, 0x17C56B6B,
														  0x1A864DB2, 0x1E475005, 0x2608EDB8, 0x22C9F00F, 0x2F8AD6D6, 0x2B4BCB61,
														  0x350C9B64, 0x31CD86D3, 0x3C8EA00A, 0x384FBDBD, 0x4C11DB70, 0x48D0C6C7,
														  0x4593E01E, 0x4152FDA9, 0x5F15ADAC, 0x5BD4B01B, 0x569796C2, 0x52568B75,
														  0x6A1936C8, 0x6ED82B7F, 0x639B0DA6, 0x675A1011, 0x791D4014, 0x7DDC5DA3,
														  0x709F7B7A, 0x745E66CD, 0x9823B6E0, 0x9CE2AB57, 0x91A18D8E, 0x95609039,
														  0x8B27C03C, 0x8FE6DD8B, 0x82A5FB52, 0x8664E6E5, 0xBE2B5B58, 0xBAEA46EF,
														  0xB7A96036, 0xB3687D81, 0xAD2F2D84, 0xA9EE3033, 0xA4AD16EA, 0xA06C0B5D,
														  0xD4326D90, 0xD0F37027, 0xDDB056FE, 0xD9714B49, 0xC7361B4C, 0xC3F706FB,
														  0xCEB42022, 0xCA753D95, 0xF23A8028, 0xF6FB9D9F, 0xFBB8BB46, 0xFF79A6F1,
														  0xE13EF6F4, 0xE5FFEB43, 0xE8BCCD9A, 0xEC7DD02D, 0x34867077, 0x30476DC0,
														  0x3D044B19, 0x39C556AE, 0x278206AB, 0x23431B1C, 0x2E003DC5, 0x2AC12072,
														  0x128E9DCF, 0x164F8078, 0x1B0CA6A1, 0x1FCDBB16, 0x018AEB13, 0x054BF6A4,
														  0x0808D07D, 0x0CC9CDCA, 0x7897AB07, 0x7C56B6B0, 0x71159069, 0x75D48DDE,
														  0x6B93DDDB, 0x6F52C06C, 0x6211E6B5, 0x66D0FB02, 0x5E9F46BF, 0x5A5E5B08,
														  0x571D7DD1, 0x53DC6066, 0x4D9B3063, 0x495A2DD4, 0x44190B0D, 0x40D816BA,
														  0xACA5C697, 0xA864DB20, 0xA527FDF9, 0xA1E6E04E, 0xBFA1B04B, 0xBB60ADFC,
														  0xB6238B25, 0xB2E29692, 0x8AAD2B2F, 0x8E6C3698, 0x832F1041, 0x87EE0DF6,
														  0x99A95DF3, 0x9D684044, 0x902B669D, 0x94EA7B2A, 0xE0B41DE7, 0xE4750050,
														  0xE9362689, 0xEDF73B3E, 0xF3B06B3B, 0xF771768C, 0xFA325055, 0xFEF34DE2,
														  0xC6BCF05F, 0xC27DEDE8, 0xCF3ECB31, 0xCBFFD686, 0xD5B88683, 0xD1799B34,
														  0xDC3ABDED, 0xD8FBA05A, 0x690CE0EE, 0x6DCDFD59, 0x608EDB80, 0x644FC637,
														  0x7A089632, 0x7EC98B85, 0x738AAD5C, 0x774BB0EB, 0x4F040D56, 0x4BC510E1,
														  0x46863638, 0x42472B8F, 0x5C007B8A, 0x58C1663D, 0x558240E4, 0x51435D53,
														  0x251D3B9E, 0x21DC2629, 0x2C9F00F0, 0x285E1D47, 0x36194D42, 0x32D850F5,
														  0x3F9B762C, 0x3B5A6B9B, 0x0315D626, 0x07D4CB91, 0x0A97ED48, 0x0E56F0FF,
														  0x1011A0FA, 0x14D0BD4D, 0x19939B94, 0x1D528623, 0xF12F560E, 0xF5EE4BB9,
														  0xF8AD6D60, 0xFC6C70D7, 0xE22B20D2, 0xE6EA3D65, 0xEBA91BBC, 0xEF68060B,
														  0xD727BBB6, 0xD3E6A601, 0xDEA580D8, 0xDA649D6F, 0xC423CD6A, 0xC0E2D0DD,
														  0xCDA1F604, 0xC960EBB3, 0xBD3E8D7E, 0xB9FF90C9, 0xB4BCB610, 0xB07DABA7,
														  0xAE3AFBA2, 0xAAFBE615, 0xA7B8C0CC, 0xA379DD7B, 0x9B3660C6, 0x9FF77D71,
														  0x92B45BA8, 0x9675461F, 0x8832161A, 0x8CF30BAD, 0x81B02D74, 0x857130C3,
														  0x5D8A9099, 0x594B8D2E, 0x5408ABF7, 0x50C9B640, 0x4E8EE645, 0x4A4FFBF2,
														  0x470CDD2B, 0x43CDC09C, 0x7B827D21, 0x7F436096, 0x7200464F, 0x76C15BF8,
														  0x68860BFD, 0x6C47164A, 0x61043093, 0x65C52D24, 0x119B4BE9, 0x155A565E,
														  0x18197087, 0x1CD86D30, 0x029F3D35, 0x065E2082, 0x0B1D065B, 0x0FDC1BEC,
														  0x3793A651, 0x3352BBE6, 0x3E119D3F, 0x3AD08088, 0x2497D08D, 0x2056CD3A,
														  0x2D15EBE3, 0x29D4F654, 0xC5A92679, 0xC1683BCE, 0xCC2B1D17, 0xC8EA00A0,
														  0xD6AD50A5, 0xD26C4D12, 0xDF2F6BCB, 0xDBEE767C, 0xE3A1CBC1, 0xE760D676,
														  0xEA23F0AF, 0xEEE2ED18, 0xF0A5BD1D, 0xF464A0AA, 0xF9278673, 0xFDE69BC4,
														  0x89B8FD09, 0x8D79E0BE, 0x803AC667, 0x84FBDBD0, 0x9ABC8BD5, 0x9E7D9662,
														  0x933EB0BB, 0x97FFAD0C, 0xAFB010B1, 0xAB710D06, 0xA6322BDF, 0xA2F33668,
														  0xBCB4666D, 0xB8757BDA, 0xB5365D03, 0xB1F740B4};

		private class ID3v2Header
		{
			public char[] ID = new char[3];
			public byte Version;
			public byte Revision;
			public byte Flags;
			public byte[] Size = new byte[4];

			public void Reset()
			{
				Array.Clear(ID,0,ID.Length);
				Version = 0;
				Revision = 0;
				Flags = 0;
				Array.Clear(Size,0,Size.Length);
			}
		}

		// Ogg page header
		private class OggHeader 
		{
			public char[] ID = new char[4];                           // Always "OggS"
			public byte StreamVersion;                     // Stream structure version
			public byte TypeFlag;                                  // Header type flag
			public long AbsolutePosition;                 // Absolute granule position
			public int Serial;                                 // Stream serial number
			public int PageNumber;                             // Page sequence number
			public int Checksum;                                      // Page checksum
			public byte Segments;                           // Number of page segments
			public byte[] LacingValues = new byte[0xFF]; // Lacing values - segment sizes

			public void Reset()
			{
				Array.Clear(ID,0,ID.Length);
				StreamVersion = 0;
				TypeFlag = 0;
				AbsolutePosition = 0;
				Serial = 0;
				PageNumber = 0;
				Checksum = 0;
				Segments = 0;
				Array.Clear(LacingValues,0,LacingValues.Length);
			}
		}

		// Vorbis parameter header
		private class VorbisHeader
		{
			public char[] ID = new char[7];                    // Always #1 + "vorbis"
			public byte[] BitstreamVersion = new byte[4];  // Bitstream version number
			public byte ChannelMode;                             // Number of channels
			public int SampleRate;                                 // Sample rate (hz)
			public int BitRateMaximal;                         // Bit rate upper limit
			public int BitRateNominal;                             // Nominal bit rate
			public int BitRateMinimal;                         // Bit rate lower limit
			public byte BlockSize;             // Coded size for small and long blocks
			public byte StopFlag;                                          // Always 1

			public void Reset()
			{
				Array.Clear(ID,0,ID.Length);
				Array.Clear(BitstreamVersion,0,BitstreamVersion.Length);
				ChannelMode = 0;
				SampleRate = 0;
				BitRateMaximal = 0;
				BitRateNominal = 0;
				BitRateMinimal = 0;
				BlockSize = 0;
				StopFlag = 0;
			}
		}

		// Vorbis tag data
		private class VorbisTag
		{
			public char[] ID = new char[7];                    // Always #3 + "vorbis"
			public int Fields;                                 // Number of tag fields
			public String[] FieldData = new String [VORBIS_FIELD_COUNT+1]; // Tag field data

			public void Reset()
			{
				Array.Clear(ID,0,ID.Length);
				Fields = 0;
				// Don't fill this with null, rather with "" :)
				for (int i=0; i<VORBIS_FIELD_COUNT+1; i++)
				{
					FieldData[i] = "";
				}
			}
		}

		// File data
		private class FileInfo
		{
			public OggHeader FPage = new OggHeader();
			public OggHeader SPage = new OggHeader();
			public OggHeader LPage = new OggHeader();   // First, second and last page
			public VorbisHeader Parameters = new VorbisHeader(); // Vorbis parameter header
			public VorbisTag Tag = new VorbisTag();                 // Vorbis tag data
			public int FileSize;                                  // File size (bytes)
			public int Samples;                             // Total number of samples
			public int ID3v2Size;                            // ID3v2 tag size (bytes)
			public int SPagePos;                        // Position of second Ogg page
			public int TagEndPos;                                  // Tag end position

			public void Reset()
			{
				FPage.Reset();
				SPage.Reset();
				LPage.Reset();
				Parameters.Reset();
				Tag.Reset();
				FileSize = 0;
				Samples = 0;
				ID3v2Size = 0;
				SPagePos = 0;
				TagEndPos = 0;
			}
		}

		// ********************* Auxiliary functions & voids ********************

		private String DecodeUTF8(String Source)
		{
			/*uint Index;
			uint SourceLength;
			uint FChar;
			uint NChar;*/

			// Convert UTF-8 to unicode
			//# should be automated with C#. Needs testing...
			String result = Source;

			/*
		  Index = 0;
		  SourceLength = Length(Source);
		  while Index < SourceLength do
		  {
			Inc(Index);
			FChar = Ord(Source[Index]);
			if FChar >== 0x80 then
			{
			  Inc(Index);
			  if Index > SourceLength then exit;
			  FChar = FChar and 0x3F;
			  if (FChar and 0x20) <> 0 then
			  {
				FChar = FChar and 0x1F;
				NChar = Ord(Source[Index]);
				if (NChar and 0xC0) <> 0x80 then  exit;
				FChar = (FChar shl 6) or (NChar and 0x3F);
				Inc(Index);
				if Index > SourceLength then exit;
			  }
			  NChar = Ord(Source[Index]);
			  if (NChar and 0xC0) <> 0x80 then exit;
			  Result = Result + WideChar((FChar shl 6) or (NChar and 0x3F));
			end
			else
			  Result = Result + WideChar(FChar);
		  }*/
			return result;
		}

		// ---------------------------------------------------------------------------

		private String EncodeUTF8(String Source)
		{
			/*uint Index;
			uint SourceLength;
			uint CChar;*/
  
			// Convert unicode to UTF-8
			// #Same comment as with DecodeUTF8
			String result = Source;
  
			/*Index = 0;
		  SourceLength = Length(Source);
		  while Index < SourceLength do
		  {
			Inc(Index);
			CChar = Cardinal(Source[Index]);
			if CChar <== 0x7F then
			  Result = Result + Source[Index]
			else if CChar > 0x7FF then
			{
			  Result = Result + Char(0xE0 or (CChar shr 12));
			  Result = Result + Char(0x80 or ((CChar shr 6) and 0x3F));
			  Result = Result + Char(0x80 or (CChar and 0x3F));
			end
			else
			{
			  Result = Result + Char(0xC0 or (CChar shr 6));
			  Result = Result + Char(0x80 or (CChar and 0x3F));
			}
		  }*/
			return result;
		}

		// ---------------------------------------------------------------------------

		private int GetID3v2Size(BinaryReader Source)
		{  
			ID3v2Header Header = new ID3v2Header();

			// Get ID3v2 tag size (if exists)
			int result = 0;
			Source.BaseStream.Seek(0, SeekOrigin.Begin);
  	
			Header.ID = Source.ReadChars(3);
			Header.Version = Source.ReadByte();
			Header.Revision = Source.ReadByte();
			Header.Flags = Source.ReadByte();
			Header.Size = Source.ReadBytes(4);

			if ( Utils.StringEqualsArr("ID3",Header.ID) )
			{
				result =
					Header.Size[0] * 0x200000 +
					Header.Size[1] * 0x4000 +
					Header.Size[2] * 0x80 +
					Header.Size[3] + 10;
				if (0x10 == (Header.Flags & 0x10)) result += 10;
				if (result > Source.BaseStream.Length) result = 0;
			}
			return result;
		}

		// ---------------------------------------------------------------------------

		private void SetTagItem(String Data, ref FileInfo Info)
		{
			int Separator;	
			String FieldID;
			String FieldData;

			// Set Vorbis tag item if supported comment field found
			Separator = Data.IndexOf("=");
			if (Separator > 0)
			{
				FieldID = Data.Substring(0,Separator).ToUpper();		
				FieldData = Data.Substring(Separator + 1, Data.Length - FieldID.Length - 1);
				for (int index=0; index < VORBIS_FIELD_COUNT; index++)
					if (VORBIS_FIELD[index] == FieldID)
						Info.Tag.FieldData[index+1] = DecodeUTF8(FieldData.Trim());  
			}
			else
				if ("" == Info.Tag.FieldData[0]) Info.Tag.FieldData[0] = Data;		
		}

		// ---------------------------------------------------------------------------

		private void ReadTag(BinaryReader Source, ref FileInfo Info)
		{
			int Index;
			int Size;
			long Position;
			char[] Data = new char[250];

			// Read Vorbis tag
			Index = 0;
			do
			{
				Array.Clear(Data,0,Data.Length);

				Size = Source.ReadInt32();	  

				Position = Source.BaseStream.Position;
				if (Size > 250) Data = Source.ReadChars(250);
				else Data = Source.ReadChars(Size);
    
				// Set Vorbis tag item
				SetTagItem( new String(Data).Trim(), ref Info);
				Source.BaseStream.Seek(Position + Size, SeekOrigin.Begin);
				if (0 == Index) Info.Tag.Fields = Source.ReadInt32();
				Index++;
			}
			while (Index <= Info.Tag.Fields);
			Info.TagEndPos = (int)Source.BaseStream.Position;
		}

		// ---------------------------------------------------------------------------

		private long GetSamples(BinaryReader Source)
		{  
			int DataIndex;	
			// Using byte instead of char here to avoid mistaking range of bytes for unicode chars
			byte[] Data = new byte[251];
			OggHeader Header = new OggHeader();

			// Get total number of samples
			int result = 0;

			for (int index=1; index<=50; index++)
			{
				DataIndex = (int)(Source.BaseStream.Length - (/*Data.Length*/251 - 10) * index - 10);
				Source.BaseStream.Seek(DataIndex, SeekOrigin.Begin);
				Data = Source.ReadBytes(251);

				// Get number of PCM samples from last Ogg packet header
				for (int iterator=251 - 10; iterator>=0; iterator--)
				{
					char[] tempArray = new char[4] { (char)Data[iterator],
													   (char)Data[iterator + 1],
													   (char)Data[iterator + 2],
													   (char)Data[iterator + 3] };
					if ( Utils.StringEqualsArr(OGG_PAGE_ID,tempArray) ) 
					{
						Source.BaseStream.Seek(DataIndex + iterator, SeekOrigin.Begin);
        
						Header.ID = Source.ReadChars(4);
						Header.StreamVersion = Source.ReadByte();
						Header.TypeFlag = Source.ReadByte();
						Header.AbsolutePosition = Source.ReadInt64();
						Header.Serial = Source.ReadInt32();
						Header.PageNumber = Source.ReadInt32();
						Header.Checksum = Source.ReadInt32();
						Header.Segments = Source.ReadByte();
						Header.LacingValues = Source.ReadBytes(0xFF);
						return Header.AbsolutePosition;
					}
				}
			}
			return result;
		}

		// ---------------------------------------------------------------------------

		private bool GetInfo(String FileName, ref FileInfo Info)
		{
			FileStream fs = null;
			BinaryReader Source = null;

			// Get info from file
			bool result = false;
  
			try
			{    
				// Set read-access and open file		
				fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);				
				fs.Lock(0,fs.Length);
				Source = new BinaryReader(fs);

				Info.FileSize = (int)fs.Length;
				Info.ID3v2Size = GetID3v2Size(Source);
				Source.BaseStream.Seek(Info.ID3v2Size, SeekOrigin.Begin);    

				Info.FPage.ID = Source.ReadChars(4);
				Info.FPage.StreamVersion = Source.ReadByte();
				Info.FPage.TypeFlag = Source.ReadByte();
				Info.FPage.AbsolutePosition = Source.ReadInt64();
				Info.FPage.Serial = Source.ReadInt32();
				Info.FPage.PageNumber = Source.ReadInt32();
				Info.FPage.Checksum = Source.ReadInt32();
				Info.FPage.Segments = Source.ReadByte();
				Info.FPage.LacingValues = Source.ReadBytes(0xFF);

				if ( Utils.StringEqualsArr(OGG_PAGE_ID,Info.FPage.ID) )
				{

					Source.BaseStream.Seek(Info.ID3v2Size + Info.FPage.Segments + 27, SeekOrigin.Begin);

					// Read Vorbis parameter header
					//Source.Read(Info.Parameters, 30);					
					Info.Parameters.ID = Source.ReadChars(7);
					Info.Parameters.BitstreamVersion = Source.ReadBytes(4);
					Info.Parameters.ChannelMode = Source.ReadByte();
					Info.Parameters.SampleRate = Source.ReadInt32();
					Info.Parameters.BitRateMaximal = Source.ReadInt32();
					Info.Parameters.BitRateNominal = Source.ReadInt32();
					Info.Parameters.BitRateMinimal = Source.ReadInt32();
					Info.Parameters.BlockSize = Source.ReadByte();
					Info.Parameters.StopFlag = Source.ReadByte();

					if ( Utils.StringEqualsArr(VORBIS_PARAMETERS_ID, Info.Parameters.ID) ) 
					{

						Info.SPagePos = (int)fs.Position;    

						Info.SPage.ID = Source.ReadChars(4);
						Info.SPage.StreamVersion = Source.ReadByte();
						Info.SPage.TypeFlag = Source.ReadByte();
						Info.SPage.AbsolutePosition = Source.ReadInt64();
						Info.SPage.Serial = Source.ReadInt32();
						Info.SPage.PageNumber = Source.ReadInt32();
						Info.SPage.Checksum = Source.ReadInt32();
						Info.SPage.Segments = Source.ReadByte();
						Info.SPage.LacingValues = Source.ReadBytes(0xFF);

						Source.BaseStream.Seek(Info.SPagePos + Info.SPage.Segments + 27, SeekOrigin.Begin);
		    
						Info.Tag.ID = Source.ReadChars(7);
						// Read Vorbis tag
						if ( Utils.StringEqualsArr(VORBIS_TAG_ID, Info.Tag.ID)) ReadTag(Source, ref Info); //# cast implicite douteux
		    
						// Get total number of samples
						Info.Samples = (int)GetSamples(Source);

						result = true;
					}
				}
			} 
			catch(Exception e)
			{
				//System.Console.WriteLine(e.StackTrace);	
				System.Console.WriteLine(e.Message);
				result = false;
			}
			
			if (fs != null)
			{				
				fs.Unlock(0,fs.Length);
				if (Source != null) Source.Close();				
			}

			return result;
		}

		// ---------------------------------------------------------------------------

		private byte GetTrack(String TrackString)
		{
			int Index;
			int Value;	

			// Extract track from string
			Index = TrackString.IndexOf("/");
			try
			{
				if (-1 == Index) Value = Int32.Parse(TrackString);
				else Value = Int32.Parse(TrackString.Substring(0, Index));
				return (byte)Value;
			} 
			catch
			{				
				return 0;
			}	
		}

		// ---------------------------------------------------------------------------

		private MemoryStream BuildTag(FileInfo Info) // TStringStream
		{  
			int Fields;
			int Size;
			String FieldData;

			// Build Vorbis tag
			MemoryStream result = new MemoryStream();
			BinaryWriter wStream = new BinaryWriter(result);

			Fields = 0;
			for (int index=0; index < VORBIS_FIELD_COUNT; index++)
				if (Info.Tag.FieldData[index+1] != "") Fields++;

			// Write frame ID, vendor info and number of fields
			wStream.Write(Info.Tag.ID);
			Size = Info.Tag.FieldData[0].Length;
			wStream.Write(Size);
			wStream.Write(Info.Tag.FieldData[0]);
			wStream.Write(Fields);

			// Write tag fields
			for (int index=0; index < VORBIS_FIELD_COUNT; index++)
				if (Info.Tag.FieldData[index+1] != "")
				{
					FieldData = VORBIS_FIELD[index] +
						"=" + EncodeUTF8(Info.Tag.FieldData[index+1]);
					Size = FieldData.Length;
					wStream.Write(Size);
					wStream.Write(FieldData);
				}

			wStream.Close();
			return result;
		}

		// ---------------------------------------------------------------------------

		private void SetLacingValues(ref FileInfo Info, int NewTagSize)
		{
			int Position;
			int Value;
			byte[] Buffer = new byte[0xFF];

			// Set new lacing values for the second Ogg page
			Position = 1;
			Value = 0;
  
			for (int index=Info.SPage.Segments-1; index>=0; index--)
			{
				if (Info.SPage.LacingValues[index] < 0xFF)
				{
					Position = index;
					Value = 0;
				}
				Value += Info.SPage.LacingValues[index];
			}
			Value = Value + NewTagSize -
				(Info.TagEndPos - Info.SPagePos - Info.SPage.Segments - 27);

			// Change lacing values at the {ning
			for (int index=0; index < Value / 0xFF; index++) Buffer[index] = 0xFF;

			Buffer[(Value / 0xFF) + 1] = (byte)(Value % 0xFF);

			if (Position < Info.SPage.Segments)
				for (int index=Position; index < Info.SPage.Segments; index++)
					Buffer[index - Position + (Value / 0xFF) + 1] =
						Info.SPage.LacingValues[index];

			Info.SPage.Segments = (byte)(Info.SPage.Segments - Position + (Value / 0xFF) + 1);
  
			for (int index=0; index < Info.SPage.Segments; index++)
				Info.SPage.LacingValues[index] = Buffer[index];
		}

		// ---------------------------------------------------------------------------

		private /*unsafe*/ void CalculateCRC(ref uint CRC, uint Data, uint Size) // # to check : not sure about ptr operations here
		{
			/*byte* Buffer;  

			// Calculate CRC through data
			Buffer = &Data;
			for (uint index=0; index < Size; index++)
			{
				CRC = (CRC << 8) ^ CRC_TABLE[((CRC >> 24) & 0xFF) ^ *Buffer];
				Buffer++;
			}*/
		}

		// ---------------------------------------------------------------------------

		/*unsafe*/ void SetCRC(FileStream Destination, FileInfo Info)
				   {  
					   /*uint Value;
					   byte[] Data = new byte[0xFF];

					   BinaryWriter wStream = new BinaryWriter(Destination);
					   BinaryReader rStream = new BinaryReader(Destination);

					   // Calculate and set checksum for Vorbis tag
					   Value = 0;
					   CalculateCRC(ref Value, &Info.SPage, (uint)(Info.SPage.Segments + 27));
					   rStream.BaseStream.Seek(Info.SPagePos + Info.SPage.Segments + 27, SeekOrigin.Begin);
					   for (int index=0; index < Info.SPage.Segments-1; index++)
						   if (Info.SPage.LacingValues[index] > 0)
						   {
							   Data = rStream.ReadBytes(Info.SPage.LacingValues[index]);
							   CalculateCRC(ref Value, Data, &Info.SPage.LacingValues[index]);
						   }
					   wStream.Seek(Info.SPagePos + 22, SeekOrigin.Begin);
					   wStream.Write(Value);

					   wStream.Close();
					   rStream.Close();*/
				   }

		// ---------------------------------------------------------------------------

		private bool RebuildFile(String FileName, MemoryStream Tag, FileInfo Info)
		{
			FileStream source = null;
			FileStream destination = null;

			BinaryReader tagReader = null;
			BinaryReader sourceReader = null;
			BinaryWriter destWriter = null;

			String BufferName = FileName + "~";

			bool result = false;
			// Rebuild the file with the new Vorbis tag
  	
			if ( ! File.Exists(FileName) ) return result;
			try
			{
				File.SetAttributes(FileName, FileAttributes.Normal);
				// Create file streams			
				tagReader = new BinaryReader(Tag);
				source = new FileStream(FileName, FileMode.Open, FileAccess.Read);
				sourceReader = new BinaryReader(source);
				destination = new FileStream(BufferName, FileMode.CreateNew , FileAccess.Write);
				destWriter = new BinaryWriter(destination);
    
				// Copy data blocks
				destWriter.Write(sourceReader.ReadBytes(Info.SPagePos));

				//destWriter.Write(Info.SPage, Info.SPage.Segments + 27);

				destWriter.Write(Info.SPage.ID);
				destWriter.Write(Info.SPage.StreamVersion);
				destWriter.Write(Info.SPage.TypeFlag);	  
				destWriter.Write(Info.SPage.AbsolutePosition);
				destWriter.Write(Info.SPage.Serial);
				destWriter.Write(Info.SPage.PageNumber);
				destWriter.Write(Info.SPage.Checksum);
				destWriter.Write(Info.SPage.Segments);
				for (int i=0;i<Info.SPage.Segments;i++)
				{
					destWriter.Write(Info.SPage.LacingValues[i]);
				}	  

				destWriter.Write(tagReader.ReadBytes((int)Tag.Length));
				sourceReader.BaseStream.Seek(Info.TagEndPos, SeekOrigin.Begin);
				destWriter.Write(sourceReader.ReadBytes((int)(source.Length - Info.TagEndPos)));
				SetCRC(destination, Info);
			
				// Replace old file and delete temporary file
				File.Delete (FileName);
				File.Move(BufferName, FileName);
				result = true;
			}
			catch (Exception e)
			{
				System.Console.WriteLine(e.StackTrace);
				if ( File.Exists(BufferName) ) File.Delete(BufferName);
				result = false;
			}
			if (destWriter != null) destWriter.Close();
			if (destination != null) destination.Close();
			if (sourceReader != null) sourceReader.Close();
			if (source != null) source.Close();  
			if (tagReader != null) tagReader.Close();

			return result;
		}

		// ********************** Private functions & voids *********************

		private void FResetData()
		{
			// Reset variables
			FFileSize = 0;
			FChannelModeID = 0;
			FSampleRate = 0;
			FBitRateNominal = 0;
			FSamples = 0;
			FID3v2Size = 0;
			FTitle = "";
			FArtist = "";
			FAlbum = "";
			FTrack = 0;
			FDate = "";
			FGenre = "";
			FComment = "";
			FVendor = "";
		}

		// ---------------------------------------------------------------------------

		private String FGetChannelMode()
		{
			String result;
			// Get channel mode name
			if (FChannelModeID > 2) result = VORBIS_MODE[3]; 
			else
				result = VORBIS_MODE[FChannelModeID];

			return VORBIS_MODE[FChannelModeID];
		}

		// ---------------------------------------------------------------------------

		private double FGetDuration()
		{
			double result;
			// Calculate duration time
			if (FSamples > 0)
				if (FSampleRate > 0)
					result = ((double)FSamples / FSampleRate);
				else
					result = 0;
			else
				if ((FBitRateNominal > 0) && (FChannelModeID > 0))
				result = ((double)FFileSize - FID3v2Size) /
					(double)FBitRateNominal / FChannelModeID / 125 * 2;
			else
				result = 0;
		
			return result;
		}

		// ---------------------------------------------------------------------------

		private double FGetBitRate()
		{
			// Calculate average bit rate
			double result = 0;
			if (FGetDuration() > 0)
				result = Math.Round((FFileSize - FID3v2Size) / (double)FGetDuration() / 125);
	
			return result;
		}

		// ---------------------------------------------------------------------------

		private bool FHasID3v2()
		{
			// Check for ID3v2 tag
			return (FID3v2Size > 0);
		}

		// ---------------------------------------------------------------------------

		private bool FIsValid()
		{
			// Check for file correctness
			return ( ( ((VORBIS_CM_MONO <= FChannelModeID) && (FChannelModeID <= VORBIS_CM_STEREO)) || (VORBIS_CM_MULTICHANNEL == FChannelModeID) ) &&
				(FSampleRate > 0) && (FGetDuration() > 0.1) && (FGetBitRate() > 0) );
		}

		// ********************** Public functions & voids **********************

		public TOggVorbis()
		{
			// Object constructor
			FResetData();  
		}

		// ---------------------------------------------------------------------------

		// No explicit destructors with C#

		// ---------------------------------------------------------------------------

		public bool ReadFromFile(String FileName)
		{
			FileInfo Info = new FileInfo();
			bool result = false;

			// Read data from file  
			FResetData();
  
			Info.Reset();

			if ( GetInfo(FileName, ref Info) )
			{
				// Fill variables
				FFileSize = Info.FileSize;
				FChannelModeID = Info.Parameters.ChannelMode;
				FSampleRate = Info.Parameters.SampleRate;
				FBitRateNominal = (ushort)(Info.Parameters.BitRateNominal / 1000); // Integer division
				FSamples = Info.Samples;
				FID3v2Size = Info.ID3v2Size;
				FTitle = Info.Tag.FieldData[1];
				if (Info.Tag.FieldData[2] != "") FArtist = Info.Tag.FieldData[2];
				else FArtist = Info.Tag.FieldData[8];
				FAlbum = Info.Tag.FieldData[3];
				FTrack = GetTrack(Info.Tag.FieldData[4]);
				FDate = Info.Tag.FieldData[5];
				FGenre = Info.Tag.FieldData[6];
				if (Info.Tag.FieldData[7] != "") FComment = Info.Tag.FieldData[7];
				else FComment = Info.Tag.FieldData[9];
				FVendor = Info.Tag.FieldData[0];
				result = true;
			}
			return result;
		}

		// ---------------------------------------------------------------------------

		public bool SaveTag(String FileName)
		{
			FileInfo Info = new FileInfo();
			MemoryStream Tag;
			bool result = false;

			// Save Vorbis tag    
			Info.Reset();

			if ( GetInfo(FileName, ref Info) )
			{
				// Prepare tag data and save to file
				Info.Tag.FieldData[1] = FTitle.Trim();
				Info.Tag.FieldData[2] = FArtist.Trim();
				Info.Tag.FieldData[3] = FAlbum.Trim();
				if (FTrack > 0) Info.Tag.FieldData[4] = FTrack.ToString();
				else Info.Tag.FieldData[4] = "";
				Info.Tag.FieldData[5] = FDate.Trim();
				Info.Tag.FieldData[6] = FGenre.Trim();
				Info.Tag.FieldData[7] = FComment.Trim();
				Info.Tag.FieldData[8] = "";
				Info.Tag.FieldData[9] = "";
				Tag = BuildTag(Info);
				Info.SPage.Checksum = 0;
				SetLacingValues(ref Info, (int)Tag.Length);
				result = RebuildFile(FileName, Tag, Info);
				Tag.Close();
			}
			return result;
		}

		// ---------------------------------------------------------------------------

		public bool ClearTag(String FileName)
		{
			// Clear Vorbis tag
			FTitle = "";
			FArtist = "";
			FAlbum = "";
			FTrack = 0;
			FDate = "";
			FGenre = "";
			FComment = "";
			return SaveTag(FileName);
		}

	}
}