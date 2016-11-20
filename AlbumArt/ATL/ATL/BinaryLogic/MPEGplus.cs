// ***************************************************************************
//                                                                            
// Audio Tools Library (Freeware)                                             
// Class TMPEGplus - for manipulating with MPEGplus file information          
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
// 11th May 2005 - Translated to C# by Zeugma 440
//                                                                            
// Version 1.81 (27 September 2003)                                           
//   - changed minimal allowed bitrate to '3' (e.g. encoded digital silence)  
//                                                                            
// Version 1.8 (20 August 2003) by Madah                                      
//   - Will now read files with different samplerates correctly               
//   - Also changed GetProfileID() for this to work                           
//   - Added the ability to determine encoder used                            
//                                                                            
// Version 1.7 (7 June 2003) by Gambit                                        
//   - --quality 0 to 10 detection (all profiles)                             
//   - Stream Version 7.1 detected and supported                              
//                                                                            
// Version 1.6 (8 February 2002)                                              
//   - Fixed bug with property Corrupted                                      
//                                                                            
// Version 1.2 (2 August 2001)                                                
//   - Some class properties added/changed                                    
//                                                                            
// Version 1.1 (26 July 2001)                                                 
//   - Fixed reading problem with "read only" files                           
//                                                                            
// Version 1.0 (23 May 2001)                                                  
//   - Support for MPEGplus files (stream versions 4-7)                       
//   - Class TID3v1: reading & writing support for ID3v1 tags                 
//   - Class TID3v2: reading & writing support for ID3v2 tags                 
//   - Class TAPEtag: reading & writing support for APE tags                  
//                                                                            
// ***************************************************************************

using System;
using System.IO;

namespace ATL.AudioReaders.BinaryLogic
{
	class TMPEGplus : AudioDataReader
	{	
		// Used with ChannelModeID property
		private const byte MPP_CM_STEREO = 1;               // Index for stereo mode
		private const byte MPP_CM_JOINT_STEREO = 2;   // Index for joint-stereo mode

		// Channel mode names
		private String[] MPP_MODE = new String[3]
	{"Unknown", "Stereo", "Joint Stereo"};

		// Used with ProfileID property
		private const byte MPP_PROFILE_QUALITY0 = 9;        // '--quality 0' profile
		private const byte MPP_PROFILE_QUALITY1 = 10;       // '--quality 1' profile
		private const byte MPP_PROFILE_TELEPHONE = 11;        // 'Telephone' profile
		private const byte MPP_PROFILE_THUMB = 1;          // 'Thumb' (poor) quality
		private const byte MPP_PROFILE_RADIO = 2;        // 'Radio' (normal) quality
		private const byte MPP_PROFILE_STANDARD = 3;    // 'Standard' (good) quality
		private const byte MPP_PROFILE_XTREME = 4;   // 'Xtreme' (very good) quality
		private const byte MPP_PROFILE_INSANE = 5;   // 'Insane' (excellent) quality
		private const byte MPP_PROFILE_BRAINDEAD = 6; // 'BrainDead' (excellent) quality
		private const byte MPP_PROFILE_QUALITY9 = 7; // '--quality 9' (excellent) quality
		private const byte MPP_PROFILE_QUALITY10 = 8;  // '--quality 10' (excellent) quality
		private const byte MPP_PROFILE_UNKNOWN = 0;               // Unknown profile
		private const byte MPP_PROFILE_EXPERIMENTAL = 12;

		// Profile names
		private String[] MPP_PROFILE = new String[13]
	{
		"Unknown", "Thumb", "Radio", "Standard", "Xtreme", "Insane", "BrainDead",
		"--quality 9", "--quality 10", "--quality 0", "--quality 1", "Telephone", "Experimental"};
    
		private bool FValid;
		private byte FChannelModeID;
		private int FFileSize;
		private int FFrameCount;
		private int FSampleRate;
		private ushort FBitRate;
		private byte FStreamVersion;
		private byte FProfileID;
		private TID3v1 FID3v1;
		private TID3v2 FID3v2;
		private TAPEtag FAPEtag;
		private String FEncoder;
	
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
		public int FileSize // File size (bytes)
		{
			get { return this.FFileSize; }
		}	
		public int FrameCount // Number of frames
		{
			get { return this.FFrameCount; }
		}	
		public double BitRate // Bit rate
		{
			get { return (double)this.FGetBitRate(); }
		}
		public bool IsVBR
		{
			get { return true; }
		}
		public int CodecFamily
		{
			get { return AudioReaderFactory.CF_LOSSY; }
		}
		public byte StreamVersion // Stream version
		{
			get { return this.FStreamVersion; }
		}	
		public int SampleRate
		{
			get { return this.FSampleRate; }
		}
		public byte ProfileID // Profile code
		{
			get { return this.FProfileID; }
		}	
		public String Profile // Profile name
		{
			get { return this.FGetProfile(); }
		}	
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
		public double Duration // Duration (seconds)
		{
			get { return this.FGetDuration(); }
		}	
		public bool Corrupted // True if file corrupted
		{
			get { return this.FIsCorrupted(); }
		}		       
		public String Encoder // Encoder used
		{
			get { return this.FEncoder; }
		}

		// ID code for stream version 7 and 7.1
		private const long STREAM_VERSION_7_ID = 120279117;  // 120279117 = 'MP+' + #7
		private const long STREAM_VERSION_71_ID = 388714573;// 388714573 = 'MP+' + #23

		// File header data - for internal use
		private class HeaderRecord
		{
			public byte[] ByteArray = new byte[32];               // Data as byte array
			public int[] IntegerArray = new int[8];            // Data as integer array
			public int FileSize;                                           // File size
			public int ID3v2Size;                              // ID3v2 tag size (bytes)
		}

		// ********************* Auxiliary functions & voids ********************

		private bool ReadHeader(String FileName, ref HeaderRecord Header)
		{
			FileStream fs = null;
			BinaryReader SourceFile = null;

			bool result = true;
			try
			{	
				// Set read-access and open file
				fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
				SourceFile = new BinaryReader(fs);
		
				fs.Seek(Header.ID3v2Size, SeekOrigin.Begin);
		
				// Read header and get file size
				Header.ByteArray = SourceFile.ReadBytes(32);
				Header.FileSize = (int)fs.Length;		
		
				// if transfer is not complete
				int temp;
				for (int i=0; i<Header.IntegerArray.Length; i++)
				{
					temp =	Header.ByteArray[(i*4)]	*	0x00000001 + 
							Header.ByteArray[(i*4)+1] * 0x00000100 +
							Header.ByteArray[(i*4)+2] * 0x00010000 +
							Header.ByteArray[(i*4)+3] * 0x01000000;
					Header.IntegerArray[i] = temp;
				}
				//Array.Copy(Header.ByteArray, Header.IntegerArray, Header.ByteArray.Length);
			} 
			catch (Exception e)
			{    
				//System.Console.WriteLine(e.StackTrace);
				System.Console.WriteLine(e.Message);
				result = false;
			}

			if (SourceFile != null) SourceFile.Close();			

			return result;
		}

		// ---------------------------------------------------------------------------

		private byte GetStreamVersion(HeaderRecord Header)
		{
			byte result;

			// Get MPEGplus stream version
			if (STREAM_VERSION_7_ID == Header.IntegerArray[0])
				result = 7;
			else if (STREAM_VERSION_71_ID == Header.IntegerArray[0])
				result = 71;
			else
				switch( (Header.ByteArray[1] % 32) / 2 ) //Int division
				{
					case 3: result = 4; break;
					case 7: result = 5; break;
					case 11: result = 6; break;
					default: result = 0; break;
				}

			return result;
		}

		// ---------------------------------------------------------------------------

		private int GetSampleRate(HeaderRecord Header)
		{
			int[] mpp_samplerates = new int[4] { 44100, 48000, 37800, 32000 };

			/* get samplerate from header
			   note: this is the same byte where profile is stored
			*/
			return mpp_samplerates[Header.ByteArray[10] & 3];
		}

		// ---------------------------------------------------------------------------

		private String GetEncoder(HeaderRecord Header)
		{
			int EncoderID;
			String result = "";

			EncoderID = Header.ByteArray[10+2+15];   
			if (0 == EncoderID)
			{
				//FEncoder := 'Buschmann 1.7.0...9, Klemm 0.90...1.05';
			} 
			else 
			{
				switch ( EncoderID % 10 ) 
				{
					case 0:  result = "Release "+(EncoderID / 100)+"."+ ( (EncoderID / 10) % 10 ); break;
					case 2 : result = "Beta "+(EncoderID / 100)+"."+ (EncoderID % 100); break; // Not exactly...
					case 4: goto case 2;
					case 6: goto case 2;
					case 8: goto case 2;
					default: result = "--Alpha-- "+(EncoderID / 100)+"."+(EncoderID % 100); break;
				}
			}
		
			return result;
		}
		// ---------------------------------------------------------------------------

		private byte GetChannelModeID(HeaderRecord Header)
		{
			byte result;

			if ((7 == GetStreamVersion(Header)) || (71 == GetStreamVersion(Header)))
				// Get channel mode for stream version 7
				if ((Header.ByteArray[11] % 128) < 64) result = MPP_CM_STEREO;
				else result = MPP_CM_JOINT_STEREO;
			else
				// Get channel mode for stream version 4-6
				if (0 == (Header.ByteArray[2] % 128)) result = MPP_CM_STEREO;
			else result = MPP_CM_JOINT_STEREO;
	
			return result;
		}

		// ---------------------------------------------------------------------------

		private int GetFrameCount(HeaderRecord Header)
		{
			int result;

			// Get frame count
			if (4 == GetStreamVersion(Header) ) result = Header.IntegerArray[1] >> 16;
			else
				if ((5 <= GetStreamVersion(Header)) && (GetStreamVersion(Header) <= 71) )
				result = Header.IntegerArray[1];
			else result = 0; 
 
			return result;
		}

		// ---------------------------------------------------------------------------

		private ushort GetBitRate(HeaderRecord Header)
		{
			// Try to get bit rate
			if ( (6 >= GetStreamVersion(Header)) /*|| (5 == GetStreamVersion(Header))*/ )
			{
				return (ushort)((Header.IntegerArray[0] >> 23)& 0x01FF);
			}
			else
			{
				return 0;				
			}
		}


		// ---------------------------------------------------------------------------

		private byte GetProfileID(HeaderRecord Header)
		{
			byte result = MPP_PROFILE_UNKNOWN;
			// Get MPEGplus profile (exists for stream version 7 only)
			if ( (7 == GetStreamVersion(Header)) || (71 == GetStreamVersion(Header)) )
				// ((and $F0) shr 4) is needed because samplerate is stored in the same byte!
				switch( ((Header.ByteArray[10] & 0xF0) >> 4) )
				{
					case 1: result = MPP_PROFILE_EXPERIMENTAL; break;
					case 5: result = MPP_PROFILE_QUALITY0; break;
					case 6: result = MPP_PROFILE_QUALITY1; break;
					case 7: result = MPP_PROFILE_TELEPHONE; break;
					case 8: result = MPP_PROFILE_THUMB; break;
					case 9: result = MPP_PROFILE_RADIO; break;
					case 10: result = MPP_PROFILE_STANDARD; break;
					case 11: result = MPP_PROFILE_XTREME; break;
					case 12: result = MPP_PROFILE_INSANE; break;
					case 13: result = MPP_PROFILE_BRAINDEAD; break;
					case 14: result = MPP_PROFILE_QUALITY9; break;
					case 15: result = MPP_PROFILE_QUALITY10; break;
				}

			return result;
		}

		// ********************** Private functions & voids *********************

		private void FResetData()
		{
			FValid = false;
			FChannelModeID = 0;
			FFileSize = 0;
			FFrameCount = 0;
			FBitRate = 0;
			FStreamVersion = 0;
			FSampleRate = 0;
			FEncoder = "";
			FProfileID = MPP_PROFILE_UNKNOWN;
			FID3v1.ResetData();
			FID3v2.ResetData();
			FAPEtag.ResetData();
		}

		// ---------------------------------------------------------------------------

		private String FGetChannelMode()
		{
			return MPP_MODE[FChannelModeID];
		}

		// ---------------------------------------------------------------------------

		private ushort FGetBitRate()
		{
			int CompressedSize;

			ushort result = FBitRate;
			// Calculate bit rate if not given
			CompressedSize = FFileSize - FID3v2.Size - FAPEtag.Size;
			if (FID3v1.Exists) FFileSize -= 128;
			if ((0 == result) && (FFrameCount > 0))
				result = (ushort)Math.Round((double)CompressedSize * 8 * ((double)FSampleRate/1000) / (double)FFrameCount / 1152);			

			return result;
		}

		// ---------------------------------------------------------------------------

		private String FGetProfile()
		{
			return MPP_PROFILE[FProfileID];
		}

		// ---------------------------------------------------------------------------

		private double FGetDuration()
		{
			// Calculate duration time
			if (FSampleRate > 0)
				return ((double)FFrameCount * 1152 / FSampleRate);
			else return 0;
		}

		// ---------------------------------------------------------------------------

		private bool FIsCorrupted()
		{
			// Check for file corruption
			return ( (FValid) && ((FGetBitRate() < 3) || (FGetBitRate() > 480)) );
		}

		// ********************** Public functions & voids **********************

		public TMPEGplus()
		{  
			FID3v1 = new TID3v1();
			FID3v2 = new TID3v2();
			FAPEtag = new TAPEtag();
			FResetData();
		}

		// ---------------------------------------------------------------------------

		// No explicit destructors with C#

		// ---------------------------------------------------------------------------

		public bool ReadFromFile(String FileName)
		{
			HeaderRecord Header = new HeaderRecord();
			bool result;

			// Reset data and load header from file to variable
			FResetData();
	
			Array.Clear(Header.ByteArray,0,Header.ByteArray.Length);
			Array.Clear(Header.IntegerArray,0,Header.IntegerArray.Length);
			Header.FileSize = 0;
			Header.ID3v2Size = 0;

			// At first try to load ID3v2 tag data, then header
			if ( FID3v2.ReadFromFile(FileName) ) Header.ID3v2Size = FID3v2.Size;

			result = ReadHeader(FileName, ref Header);
			// Process data if loaded and file valid
			if ((result) && (Header.FileSize > 0) && (GetStreamVersion(Header) > 0) )
			{
				FValid = true;
				// Fill properties with header data
				FSampleRate            = GetSampleRate(Header);				
				FChannelModeID         = GetChannelModeID(Header);
				FFileSize              = Header.FileSize;
				FFrameCount            = GetFrameCount(Header);				
				FBitRate               = GetBitRate(Header);
				FStreamVersion         = GetStreamVersion(Header);
				FProfileID             = GetProfileID(Header);
				FEncoder               = GetEncoder(Header);
				FID3v1.ReadFromFile(FileName);
				FAPEtag.ReadFromFile(FileName);
			}
			return result;
		}

	}
}