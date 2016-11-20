// ***************************************************************************
//                                                                            
// Audio Tools Library (Freeware)                                             
// Class TTwinVQ - for extracting information from TwinVQ file header         
//                                                                            
// Copyright (c) 2001,2002 by Jurgen Faul                                     
// E-mail: jfaul@gmx.de                                                       
// http://jfaul.de/atl                                                        
//                                                                            
// 13th May 2005 - Translated to C# by Zeugma 440
//
// Version 1.1 (13 August 2002)                                               
//   - Added property Album                                                   
//   - Support for Twin VQ 2.0                                                
//                                                                            
// Version 1.0 (6 August 2001)                                                
//   - File info: channel mode, bit rate, sample rate, file size, duration    
//   - Tag info: title, comment, author, copyright, compressed file name      
//                                                                            
// ***************************************************************************

using System;
using System.IO;

namespace ATL.AudioReaders.BinaryLogic
{
	class TTwinVQ : AudioDataReader
	{
	 
		// Used with ChannelModeID property
		public const byte TWIN_CM_MONO = 1;               // Index for mono mode
		public const byte TWIN_CM_STEREO = 2;           // Index for stereo mode

		// Channel mode names
		public String[] TWIN_MODE = new String[3] {"Unknown", "Mono", "Stereo"};

		// Private declarations
		private bool FValid;
		private byte FChannelModeID;
		private byte FBitRate;
		private ushort FSampleRate;
		private uint FFileSize;
		private double FDuration;
		private String FTitle;
		private String FComment;
		private String FAuthor;
		private String FCopyright;
		private String FOriginalFile;
		private String FAlbum;
      
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
		public bool Valid // True if header valid
		{
			get { return this.FValid; }
		}	
		public byte ChannelModeID // Channel mode code
		{
			get { return this.FChannelModeID; }
		}
		public String ChannelMode // Channel mode name
		{
			get { return this.FGetChannelMode(); }
		}	
		public double BitRate // Total bit rate
		{
			get { return (double)this.FBitRate; }
		}		  
		public ushort SampleRate // Sample rate (hz)
		{
			get { return this.FSampleRate; }
		}
		public bool IsVBR
		{
			get { return false; }
		}
		public int CodecFamily
		{
			get { return AudioReaderFactory.CF_LOSSY; }
		}
		public uint FileSize // File size (bytes)
		{
			get { return this.FFileSize; }
		}
		public double Duration // Duration (seconds)
		{
			get { return this.FDuration; }
		}	
		public String Title // Title name
		{
			get { return this.FTitle; }
		}		  
		public String Comment // Comment
		{
			get { return this.FComment; }
		}	
		public String Author // Author name
		{
			get { return this.FAuthor; }
		}	
		public String Copyright // Copyright
		{
			get { return this.FCopyright; }
		}	
		public String OriginalFile // Original file name
		{
			get { return this.FOriginalFile; }
		}	
		public String Album // Album title
		{
			get { return this.FAlbum; }
		}
		public bool Corrupted // True if file corrupted
		{
			get { return this.FIsCorrupted(); }
		}	

		// Twin VQ header ID
		private const String TWIN_ID = "TWIN";
  
		// Max. number of supported tag-chunks
		private const byte TWIN_CHUNK_COUNT = 6;

		// Names of supported tag-chunks
		private String[] TWIN_CHUNK = new String[TWIN_CHUNK_COUNT]
	{ "NAME", "COMT", "AUTH", "(c) ", "FILE", "ALBM"};


		// TwinVQ chunk header
		private class ChunkHeader
		{
			public char[] ID = new char[4];                                // Chunk ID
			public uint Size;                                            // Chunk size
			public void Reset()
			{
				Array.Clear(ID,0,ID.Length);
				Size = 0;
			}
		}

		// File header data - for internal use
		private class HeaderInfo
		{
			// Real structure of TwinVQ file header
			public char[] ID = new char[4];                           // Always "TWIN"
			public char[] Version = new char[8];                         // Version ID
			public uint Size;                                           // Header size
			public ChunkHeader Common = new ChunkHeader();      // Common chunk header
			public uint ChannelMode;             // Channel mode: 0 - mono, 1 - stereo
			public uint BitRate;                                     // Total bit rate
			public uint SampleRate;                               // Sample rate (khz)
			public uint SecurityLevel;                                     // Always 0
			// Extended data
			public uint FileSize;                                 // File size (bytes)
			public String[] Tag = new String[TWIN_CHUNK_COUNT];     // Tag information
		}

		// ********************* Auxiliary functions & voids ********************

		private bool ReadHeader(String FileName, ref HeaderInfo Header)
		{
			bool result = true;
			FileStream fs = null;
			BinaryReader Source = null;

			try
			{
				// Set read-access and open file
				fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
				Source = new BinaryReader(fs);

				// Read header and get file size
				//BlockRead(Source, Header, 40, Transferred);
		
				Header.ID = Source.ReadChars(4);
				Header.Version = Source.ReadChars(8);
				Header.Size = Source.ReadUInt32();
				Header.Common.ID = Source.ReadChars(4);
				Header.Common.Size = Source.ReadUInt32();
				Header.ChannelMode = Source.ReadUInt32();
				Header.BitRate = Source.ReadUInt32();
				Header.SampleRate = Source.ReadUInt32();
				Header.SecurityLevel = Source.ReadUInt32();

				Header.FileSize = (uint)fs.Length;		
			} 
			catch (Exception e) 
			{
				System.Console.WriteLine(e.StackTrace);
				result = false;
			}
			if (Source != null) Source.Close();
			if (fs != null) fs.Close();

			return result;
		}

		// ---------------------------------------------------------------------------

		private int Swap32(int Figure) // Taken from ID3v2
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

		private byte GetChannelModeID(HeaderInfo Header)
		{
			// Get channel mode from header
			switch ( Swap32((int)(Header.ChannelMode >> 16)) )
			{
				case 0: return TWIN_CM_MONO;
				case 1: return TWIN_CM_STEREO;
				default: return 0;
			}
		}

		// ---------------------------------------------------------------------------

		private byte GetBitRate(HeaderInfo Header)
		{
			// Get bit rate from header
			return (byte)Swap32( (int)(Header.BitRate >> 16) );
		}

		// ---------------------------------------------------------------------------

		private ushort GetSampleRate(HeaderInfo Header)
		{
			ushort result = (ushort)Swap32((int)(Header.SampleRate >> 16));
			// Get real sample rate from header  
			switch(result)
			{
				case 11: result = 11025; break;
				case 22: result = 22050; break;
				case 44: result = 44100; break;
				default: result = (ushort)(result * 1000); break;
			}
			return result;
		}

		// ---------------------------------------------------------------------------

		private double GetDuration(HeaderInfo Header)
		{
			// Get duration from header
			return Math.Abs(((double)Header.FileSize - Swap32((int)(Header.Size >> 16)) - 20)) / 125 /
				Swap32((int)(Header.BitRate >> 16));
		}

		// ---------------------------------------------------------------------------

		private bool HeaderEndReached(ChunkHeader Chunk)
		{
			// Check for header end
			return ( ((byte)(Chunk.ID[1]) < 32) ||
				((byte)(Chunk.ID[2]) < 32) ||
				((byte)(Chunk.ID[3]) < 32) ||
				((byte)(Chunk.ID[4]) < 32) ||
				Utils.StringEqualsArr("DATA",Chunk.ID) );
		}

		// ---------------------------------------------------------------------------

		private void SetTagItem(String ID, String Data, ref HeaderInfo Header)
		{
			// Set tag item if supported tag-chunk found
			for (byte iterator=0; iterator<TWIN_CHUNK_COUNT;iterator++)
				if (ID == TWIN_CHUNK[iterator]) Header.Tag[iterator] = Data;
		}

		// ---------------------------------------------------------------------------

		private void ReadTag(String FileName, ref HeaderInfo Header)
		{ 
			ChunkHeader Chunk = new ChunkHeader();
			char[] Data = new char[250];
			FileStream fs = null;
			BinaryReader SourceFile = null;
  
			try
			{
				// Set read-access, open file
				fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
				SourceFile = new BinaryReader(fs);

				fs.Seek(16, SeekOrigin.Begin);
				do
				{
					Array.Clear(Data,0,Data.Length);
					// Read chunk header
					//BlockRead(SourceFile, Chunk, 8);
					Chunk.ID = SourceFile.ReadChars(4);
					Chunk.Size = SourceFile.ReadUInt32();		

					// Read chunk data and set tag item if chunk header valid
					if ( HeaderEndReached(Chunk) ) break;
			
					Data = SourceFile.ReadChars(Swap32((int)(Chunk.Size >> 16)) % 250);

					SetTagItem(new String(Chunk.ID), new String(Data), ref Header);
				}
				while (fs.Position < fs.Length);
			} 
			catch (Exception e) 
			{
				System.Console.WriteLine(e.StackTrace);
			}
			if (SourceFile != null) SourceFile.Close();
			if (fs != null) fs.Close();
		}

		// ********************** Private functions & voids *********************

		private void FResetData()
		{
			FValid = false;
			FChannelModeID = 0;
			FBitRate = 0;
			FSampleRate = 0;
			FFileSize = 0;
			FDuration = 0;
			FTitle = "";
			FComment = "";
			FAuthor = "";
			FCopyright = "";
			FOriginalFile = "";
			FAlbum = "";
		}

		// ---------------------------------------------------------------------------

		private String FGetChannelMode()
		{
			return TWIN_MODE[FChannelModeID];
		}

		// ---------------------------------------------------------------------------

		private bool FIsCorrupted()
		{
			// Check for file corruption
			return ( (FValid) &&
				((0 == FChannelModeID) ||
				(FBitRate < 8) || (FBitRate > 192) ||
				(FSampleRate < 8000) || (FSampleRate > 44100) ||
				(FDuration < 0.1) || (FDuration > 10000)) );
		}

		// ********************** Public functions & voids **********************

		public TTwinVQ()
		{
			FResetData();
		}

		// ---------------------------------------------------------------------------

		public bool ReadFromFile(String FileName)
		{
			HeaderInfo Header = new HeaderInfo();

			// Reset data and load header from file to variable
			FResetData();
  
			bool result = ReadHeader(FileName, ref Header);
			// Process data if loaded and header valid
			if ( (result) && Utils.StringEqualsArr(TWIN_ID,Header.ID) )
			{
				FValid = true;
				// Fill properties with header data
				FChannelModeID = GetChannelModeID(Header);
				FBitRate = GetBitRate(Header);
				FSampleRate = GetSampleRate(Header);
				FFileSize = Header.FileSize;
				FDuration = GetDuration(Header);
				// Get tag information and fill properties
				ReadTag(FileName, ref Header);
				FTitle = Header.Tag[0].Trim();
				FComment = Header.Tag[1].Trim();
				FAuthor = Header.Tag[2].Trim();
				FCopyright = Header.Tag[3].Trim();
				FOriginalFile = Header.Tag[4].Trim();
				FAlbum = Header.Tag[5].Trim();
			}
			return result;
		}

	}
}