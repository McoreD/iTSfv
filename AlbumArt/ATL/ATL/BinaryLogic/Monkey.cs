// ***************************************************************************
//                                                                            
// Audio Tools Library (Freeware)                                             
// Class TMonkey - for manipulating with Monkey's Audio file information      
//                                                                            
// Uses:                                                                      
//   - Class TID3v1                                                           
//   - Class TID3v2                                                           
//   - Class TAPEtag                                                          
//                                                                            
// Copyright (c) 2001,2002 by Jurgen Faul                                     
// E-mail: jfaul@gmx.de                                                       
// http://jfaul.de/atl                                                        
//        
//
// 9th May 2005 - Translated to C# by Zeugma 440
//                                                                    
// Version 1.5 (22 August 2003) by MaDah                                      
//   - Added support for Monkey's Audio 3.98                                  
//   - Added/changed/removed some stuff                                       
//                                                                            
// Version 1.4 (29 July 2002)                                                 
//   - Correction for calculating of duration                                 
//                                                                            
// Version 1.1 (11 September 2001)                                            
//   - Added property Samples                                                 
//   - Removed WAV header information                                         
//                                                                            
// Version 1.0 (7 September 2001)                                             
//   - Support for Monkey's Audio files                                       
//   - Class TID3v1: reading & writing support for ID3v1 tags                 
//   - Class TID3v2: reading & writing support for ID3v2 tags                 
//   - Class TAPEtag: reading & writing support for APE tags                  
//                                                                            
// ***************************************************************************

using System;
using System.IO;

namespace ATL.AudioReaders.BinaryLogic
{
	class TMonkey : AudioDataReader
	{
		// Compression level codes
		public const int MONKEY_COMPRESSION_FAST       = 1000;  // Fast (poor)
		public const int MONKEY_COMPRESSION_NORMAL     = 2000;  // Normal (good)
		public const int MONKEY_COMPRESSION_HIGH       = 3000;  // High (very good)	
		public const int MONKEY_COMPRESSION_EXTRA_HIGH = 4000;  // Extra high (best)
		public const int MONKEY_COMPRESSION_INSANE     = 5000;  // Insane
		public const int MONKEY_COMPRESSION_BRAINDEAD  = 6000;  // BrainDead
	
		// Compression level names
		public String[] MONKEY_COMPRESSION = new String[7]
	{ "Unknown", "Fast", "Normal", "High", "Extra High", "Insane", "BrainDead" };

		// Format flags, only for Monkey's Audio <= 3.97
		public const byte MONKEY_FLAG_8_BIT          = 1;  // Audio 8-bit
		public const byte MONKEY_FLAG_CRC            = 2;  // New CRC32 error detection
		public const byte MONKEY_FLAG_PEAK_LEVEL     = 4;  // Peak level stored
		public const byte MONKEY_FLAG_24_BIT         = 8;  // Audio 24-bit
		public const byte MONKEY_FLAG_SEEK_ELEMENTS  = 16; // Number of seek elements stored
		public const byte MONKEY_FLAG_WAV_NOT_STORED = 32; // WAV header not stored

		// Channel mode names
		public String[] MONKEY_MODE = new String[3]
	{ "Unknown", "Mono", "Stereo" };
	
		private bool FValid;
    
		// Stuff loaded from the header:
		private int FVersion;
		private String FVersionStr;
		private int	FChannels;
		private int	FSampleRate;
		private int	FBits;
		private uint FPeakLevel;
		private double FPeakLevelRatio;
		private long FTotalSamples;
		private double FBitrate;
		private double FDuration;
		private int	FCompressionMode;
		private String FCompressionModeStr;
      
		// FormatFlags, only used with Monkey's <= 3.97
		private int FFormatFlags;
		private bool FHasPeakLevel;
		private bool FHasSeekElements;
		private bool FWavNotStored;

		// Tagging
		private TID3v1 FID3v1;
		private TID3v2 FID3v2;
		private TAPEtag FAPEtag;
		//
		private long FFileSize;
    

		public long FileSize
		{
			get { return this.FFileSize; }
		}							 
		public bool	Valid
		{
			get { return this.FValid; }
		}
		public int Version
		{
			get { return this.FVersion; }
		}
		public String VersionStr
		{
			get { return this.FVersionStr; }
		}	
		public int Channels
		{
			get { return this.FChannels; }
		}	
		public int SampleRate
		{
			get { return this.FSampleRate; }
		}	
		public int Bits 
		{
			get { return this.FBits; }
		}		
		public double BitRate 
		{
			get { return this.FBitrate; }
		}
		public bool IsVBR
		{
			get { return false; }
		}
		public int CodecFamily
		{
			get { return AudioReaderFactory.CF_LOSSLESS; }
		}
		public double Duration
		{
			get { return this.FDuration; }
		}	
		public uint	PeakLevel
		{
			get { return this.FPeakLevel; }
		}
		public double PeakLevelRatio
		{
			get { return this.FPeakLevelRatio; }
		}
		public long	TotalSamples
		{
			get { return this.FTotalSamples; }
		}	
		public int CompressionMode
		{
			get { return this.FCompressionMode; }
		}	
		public String CompressionModeStr
		{
			get { return this.FCompressionModeStr; }
		}

		// FormatFlags, only used with Monkey's <= 3.97
		public int FormatFlags
		{
			get { return this.FFormatFlags; }
		}
		public bool	HasPeakLevel
		{
			get { return this.FHasPeakLevel; }
		}
		public bool	HasSeekElements
		{
			get { return this.FHasSeekElements; }
		}
		public bool	WavNotStored
		{
			get { return this.FWavNotStored; }
		}
    
		// Tagging
		public TID3v1 ID3v1 // ID3v1 tag data
		{
			get { return this.FID3v1; }
		}	
		public TID3v2 ID3v2 // ID3v2 tag data
		{
			get { return this.FID3v2; }
		}
		public TAPEtag APEtag // APE tag data
		{
			get { return this.FAPEtag; }
		}

		// Real structure of Monkey's Audio header
		// common header for all versions
		private class APE_HEADER
		{
			public char[] cID = new char[4]; // should equal 'MAC '
			public ushort nVersion;          // version number * 1000 (3.81 = 3810)
		}

		// old header for <= 3.97
		private struct APE_HEADER_OLD
		{
			public ushort nCompressionLevel; // the compression level
			public ushort nFormatFlags;      // any format flags (for future use)
			public ushort nChannels;         // the number of channels (1 or 2)
			public uint nSampleRate;         // the sample rate (typically 44100)
			public uint nHeaderBytes;        // the bytes after the MAC header that compose the WAV header
			public uint nTerminatingBytes;   // the bytes after that raw data (for extended info)
			public uint nTotalFrames;        // the number of frames in the file
			public uint nFinalFrameBlocks;   // the number of samples in the final frame
			public int nInt;
		}
		// new header for >= 3.98
		private struct APE_HEADER_NEW
		{
			public ushort nCompressionLevel;  // the compression level (see defines I.E. COMPRESSION_LEVEL_FAST)
			public ushort nFormatFlags;		// any format flags (for future use) Note: NOT the same flags as the old header!
			public uint nBlocksPerFrame;		// the number of audio blocks in one frame
			public uint nFinalFrameBlocks;	// the number of audio blocks in the final frame
			public uint nTotalFrames;			// the total number of frames
			public ushort nBitsPerSample;		// the bits per sample (typically 16)
			public ushort nChannels;			// the number of channels (1 or 2)
			public uint nSampleRate;			// the sample rate (typically 44100)
		}
		// data descriptor for >= 3.98
		private class APE_DESCRIPTOR
		{
			public ushort padded;					// padding/reserved (always empty)
			public uint nDescriptorBytes;			// the number of descriptor bytes (allows later expansion of this header)
			public uint nHeaderBytes;			    // the number of header APE_HEADER bytes
			public uint nSeekTableBytes;	        // the number of bytes of the seek table
			public uint nHeaderDataBytes;		    // the number of header data bytes (from original file)
			public uint nAPEFrameDataBytes;		    // the number of bytes of APE frame data
			public uint nAPEFrameDataBytesHigh;	    // the high order number of APE frame data bytes
			public uint nTerminatingDataBytes;		// the terminating data of the file (not including tag data)
			public byte[] cFileMD5 = new byte[16];	// the MD5 hash of the file (see notes for usage... it's a littly tricky)
		}

		// ********************** Private functions & voids *********************

		private void FResetData()
		{
			// Reset data
			FValid				= false;
			FVersion            = 0;
			FVersionStr         = "";
			FChannels  		    = 0;
			FSampleRate		    = 0;
			FBits      		    = 0;
			FPeakLevel          = 0;
			FPeakLevelRatio     = 0.0;
			FTotalSamples       = 0;
			FBitrate  		    = 0.0;
			FDuration		    = 0.0;
			FCompressionMode    = 0;
			FCompressionModeStr = "";
			FFormatFlags        = 0;
			FHasPeakLevel       = false;
			FHasSeekElements    = false;
			FWavNotStored       = false;
			FFileSize  		    = 0;
			FID3v1.ResetData();
			FID3v2.ResetData();
			FAPEtag.ResetData();
		}

		// ********************** Public functions & voids **********************

		public TMonkey()
		{
			// Create object  
			FID3v1 = new TID3v1();
			FID3v2 = new TID3v2();
			FAPEtag = new TAPEtag();
			FResetData();
		}

		// ---------------------------------------------------------------------------

		// No explicit destructors in C#

		// ---------------------------------------------------------------------------

		public bool ReadFromFile(String FileName)
		{
   
			APE_HEADER APE = new APE_HEADER();				// common header
			APE_HEADER_OLD APE_OLD = new APE_HEADER_OLD();	// old header   <= 3.97
			APE_HEADER_NEW APE_NEW = new APE_HEADER_NEW();	// new header   >= 3.98
			APE_DESCRIPTOR APE_DESC = new APE_DESCRIPTOR(); // extra header >= 3.98

			FileStream fs = null;
			BinaryReader SourceFile = null;

			int BlocksPerFrame;
			bool LoadSuccess;
			int TagSize;
			bool result = false;
   
			FResetData();
   
			// load tags first
			FID3v2.ReadFromFile(FileName);
			FID3v1.ReadFromFile(FileName);
			FAPEtag.ReadFromFile(FileName);
   
			// calculate total tag size
			TagSize = 0;
			if (FID3v1.Exists) TagSize += 128;
			if (FID3v2.Exists) TagSize += FID3v2.Size;
			if (FAPEtag.Exists) TagSize += FAPEtag.Size;
   
			// reading data from file
			LoadSuccess = false;

			try
			{
				try
				{         
					fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
					fs.Lock(0,fs.Length);
					SourceFile = new BinaryReader(fs);
					FFileSize = fs.Length;

					// seek past id3v2-tag
					if (FID3v2.Exists)
					{
						fs.Seek(FID3v2.Size, SeekOrigin.Begin);
					}
					// Read APE Format Header         
					Array.Clear(APE.cID,0,APE.cID.Length);
					APE.nVersion = 0;
	  
					APE.cID = SourceFile.ReadChars(4);
					APE.nVersion = SourceFile.ReadUInt16();

					if ( Utils.StringEqualsArr("MAC ",APE.cID) )
					{            
						FVersion = APE.nVersion;

						FVersionStr = ((double)FVersion / 1000).ToString().Substring(0,4); //Str(FVersion / 1000 : 4 : 2, FVersionStr);
            
						// Load New Monkey's Audio Header for version >= 3.98
						if (APE.nVersion >= 3980) 
						{
							APE_DESC.padded = 0;
							APE_DESC.nDescriptorBytes = 0;
							APE_DESC.nHeaderBytes = 0;
							APE_DESC.nSeekTableBytes = 0;
							APE_DESC.nHeaderDataBytes = 0;
							APE_DESC.nAPEFrameDataBytes = 0;
							APE_DESC.nAPEFrameDataBytesHigh = 0;
							APE_DESC.nTerminatingDataBytes = 0;
							Array.Clear(APE_DESC.cFileMD5,0,APE_DESC.cFileMD5.Length);

							APE_DESC.padded = SourceFile.ReadUInt16();
							APE_DESC.nDescriptorBytes = SourceFile.ReadUInt32();
							APE_DESC.nHeaderBytes = SourceFile.ReadUInt32();
							APE_DESC.nSeekTableBytes = SourceFile.ReadUInt32();
							APE_DESC.nHeaderDataBytes = SourceFile.ReadUInt32();
							APE_DESC.nAPEFrameDataBytes = SourceFile.ReadUInt32();
							APE_DESC.nAPEFrameDataBytesHigh = SourceFile.ReadUInt32();
							APE_DESC.nTerminatingDataBytes = SourceFile.ReadUInt32();
							APE_DESC.cFileMD5 = SourceFile.ReadBytes(16);

							// seek past description header
							if (APE_DESC.nDescriptorBytes != 52) fs.Seek(APE_DESC.nDescriptorBytes - 52, SeekOrigin.Current);
							// load new ape_header
							if (APE_DESC.nHeaderBytes > 24/*sizeof(APE_NEW)*/) APE_DESC.nHeaderBytes = 24/*sizeof(APE_NEW)*/;
                  				
							APE_NEW.nCompressionLevel = 0;
							APE_NEW.nFormatFlags = 0;
							APE_NEW.nBlocksPerFrame = 0;
							APE_NEW.nFinalFrameBlocks = 0;
							APE_NEW.nTotalFrames = 0;
							APE_NEW.nBitsPerSample = 0;
							APE_NEW.nChannels = 0;
							APE_NEW.nSampleRate = 0;

							APE_NEW.nCompressionLevel = SourceFile.ReadUInt16();
							APE_NEW.nFormatFlags = SourceFile.ReadUInt16();
							APE_NEW.nBlocksPerFrame = SourceFile.ReadUInt32();
							APE_NEW.nFinalFrameBlocks = SourceFile.ReadUInt32();
							APE_NEW.nTotalFrames = SourceFile.ReadUInt32();
							APE_NEW.nBitsPerSample = SourceFile.ReadUInt16();
							APE_NEW.nChannels = SourceFile.ReadUInt16();
							APE_NEW.nSampleRate = SourceFile.ReadUInt32();
				
							// based on MAC SDK 3.98a1 (APEinfo.h)
							FSampleRate       = (int)APE_NEW.nSampleRate;
							FChannels         = APE_NEW.nChannels;
							FFormatFlags      = APE_NEW.nFormatFlags;
							FBits             = APE_NEW.nBitsPerSample;
							FCompressionMode  = APE_NEW.nCompressionLevel;
							// calculate total uncompressed samples
							if (APE_NEW.nTotalFrames > 0)
							{
								FTotalSamples     = (long)(APE_NEW.nBlocksPerFrame) *
									(long)(APE_NEW.nTotalFrames-1) +
									(long)(APE_NEW.nFinalFrameBlocks);
							}
							LoadSuccess = true;
						}
						else 
						{
							// Old Monkey <= 3.97               

							APE_OLD.nCompressionLevel = 0;
							APE_OLD.nFormatFlags = 0;
							APE_OLD.nChannels = 0;
							APE_OLD.nSampleRate = 0;
							APE_OLD.nHeaderBytes = 0;
							APE_OLD.nTerminatingBytes = 0;
							APE_OLD.nTotalFrames = 0;
							APE_OLD.nFinalFrameBlocks = 0;
							APE_OLD.nInt = 0;

							APE_OLD.nCompressionLevel = SourceFile.ReadUInt16();
							APE_OLD.nFormatFlags = SourceFile.ReadUInt16();
							APE_OLD.nChannels = SourceFile.ReadUInt16();
							APE_OLD.nSampleRate = SourceFile.ReadUInt32();
							APE_OLD.nHeaderBytes = SourceFile.ReadUInt32();
							APE_OLD.nTerminatingBytes = SourceFile.ReadUInt32();
							APE_OLD.nTotalFrames = SourceFile.ReadUInt32();
							APE_OLD.nFinalFrameBlocks = SourceFile.ReadUInt32();
							APE_OLD.nInt = SourceFile.ReadInt32();				

							FCompressionMode  = APE_OLD.nCompressionLevel;
							FSampleRate       = (int)APE_OLD.nSampleRate;
							FChannels         = APE_OLD.nChannels;
							FFormatFlags      = APE_OLD.nFormatFlags;
							FBits = 16;
							if ( (APE_OLD.nFormatFlags & MONKEY_FLAG_8_BIT ) != 0) FBits =  8;
							if ( (APE_OLD.nFormatFlags & MONKEY_FLAG_24_BIT) != 0) FBits = 24;

							FHasSeekElements  = ( (APE_OLD.nFormatFlags & MONKEY_FLAG_PEAK_LEVEL   )  != 0);
							FWavNotStored     = ( (APE_OLD.nFormatFlags & MONKEY_FLAG_SEEK_ELEMENTS) != 0);
							FHasPeakLevel     = ( (APE_OLD.nFormatFlags & MONKEY_FLAG_WAV_NOT_STORED) != 0);
                  
							if (FHasPeakLevel)
							{
								FPeakLevel        = (uint)APE_OLD.nInt;
								FPeakLevelRatio   = (FPeakLevel / (1 << FBits) / 2.0) * 100.0;
							}

							// based on MAC_SDK_397 (APEinfo.cpp)
							if (FVersion >= 3950) 
								BlocksPerFrame = 73728 * 4;
							else if ( (FVersion >= 3900) || ((FVersion >= 3800) && (MONKEY_COMPRESSION_EXTRA_HIGH == APE_OLD.nCompressionLevel)) )
								BlocksPerFrame = 73728;
							else
								BlocksPerFrame = 9216;

							// calculate total uncompressed samples
							if (APE_OLD.nTotalFrames>0)
							{
								FTotalSamples =  (long)(APE_OLD.nTotalFrames-1) *
									(long)(BlocksPerFrame) +
									(long)(APE_OLD.nFinalFrameBlocks);
							}
							LoadSuccess = true;
               
						}
						if (LoadSuccess) 
						{
							// compression profile name
							if ( (0 == (FCompressionMode % 1000)) && (FCompressionMode<=6000) )
							{
								FCompressionModeStr = MONKEY_COMPRESSION[FCompressionMode / 1000]; // int division
							}
							else 
							{
								FCompressionModeStr = FCompressionMode.ToString();
							}
							// length
							if (FSampleRate>0) FDuration = ((double)FTotalSamples / FSampleRate);
							// average bitrate
							if (FDuration>0) FBitrate = 8*(FFileSize - (long)(TagSize)) / (FDuration*1000);
							// some extra sanity checks
							FValid   = ((FBits>0) && (FSampleRate>0) && (FTotalSamples>0) && (FChannels>0));
							result   = FValid;
						}
					}
				}
				finally
				{
					if (fs != null)
					{
						fs.Unlock(0,fs.Length);
						if (SourceFile != null) SourceFile.Close();
					}
				}
			}
			catch (Exception e)
			{
				System.Console.WriteLine(e.StackTrace);
				result = false;
			}
			return result;
		}

	}
}