/* ***************************************************************************
{                                                                             }
{ Audio Tools Library (Freeware)                                              }
{ Class TAACfile - for manipulating with AAC file information                 }
{                                                                             }
{ Uses:                                                                       }
{   - Class TID3v1                                                            }
{   - Class TID3v2                                                            }
{                                                                             }
{ Copyright (c) 2001,2002 by Jurgen Faul                                      }
{ E-mail: jfaul@gmx.de                                                        }
{ http://jfaul.de/atl                                                         }
{                                                                             }
{ 6th May 2005 - Translated to C# by Zeugma 440
{ Version 1.0 (2 October 2002)                                                }
{   - Support for AAC files with ADIF or ADTS header                          }
{   - File info: file size, type, channels, sample rate, bit rate, duration   }
{   - Class TID3v1: reading & writing support for ID3v1 tags                  }
{   - Class TID3v2: reading & writing support for ID3v2 tags                  }
{                                                                             }
{ ************************************************************************** */

using System;
using System.IO;
using ATL.Logging;

namespace ATL.AudioReaders.BinaryLogic
{
	class TAACfile : AudioDataReader
	{

		// Header type codes

		public const byte AAC_HEADER_TYPE_UNKNOWN = 0;                       // Unknown
		public const byte AAC_HEADER_TYPE_ADIF = 1;                          // ADIF
		public const byte AAC_HEADER_TYPE_ADTS = 2;                          // ADTS

		// Header type names
		public static String[] AAC_HEADER_TYPE = { "Unknown", "ADIF", "ADTS" };

		// MPEG version codes
		public const byte AAC_MPEG_VERSION_UNKNOWN = 0;                      // Unknown
		public const byte AAC_MPEG_VERSION_2 = 1;                            // MPEG-2
		public const byte AAC_MPEG_VERSION_4 = 2;                            // MPEG-4

		// MPEG version names
		public static String[] AAC_MPEG_VERSION = { "Unknown", "MPEG-2", "MPEG-4" };

		// Profile codes
		public const byte AAC_PROFILE_UNKNOWN = 0;                           // Unknown
		public const byte AAC_PROFILE_MAIN = 1;                              // Main
		public const byte AAC_PROFILE_LC = 2;                                // LC
		public const byte AAC_PROFILE_SSR = 3;                               // SSR
		public const byte AAC_PROFILE_LTP = 4;                               // LTP

		// Profile names
		public static String[] AAC_PROFILE =  
		{ "Unknown", "AAC Main", "AAC LC", "AAC SSR", "AAC LTP" };

		// Bit rate type codes
		public const byte AAC_BITRATE_TYPE_UNKNOWN = 0;                      // Unknown
		public const byte AAC_BITRATE_TYPE_CBR = 1;                          // CBR
		public const byte AAC_BITRATE_TYPE_VBR = 2;                          // VBR

		// Bit rate type names
		public static String[] AAC_BITRATE_TYPE = { "Unknown", "CBR", "VBR" };
    
		private int FFileSize;
		private int FTotalFrames;
		private byte FHeaderTypeID;
		private byte FMPEGVersionID;
		private byte FProfileID;
		private byte FChannels;
		private int FSampleRate;
		private int FBitRate;			// (bit/s)
		private byte FBitRateTypeID;
		private TID3v1 FID3v1;
		private TID3v2 FID3v2;
		private TAPEtag FAPEtag;
    
	
		public int FileSize // File size (bytes)
		{
			get { return this.FFileSize; }         
		}
		public byte HeaderTypeID // Header type code
		{
			get { return this.FHeaderTypeID; }
		}
		public String HeaderType // Header type name
		{
			get { return this.FGetHeaderType(); }
		}
		public byte MPEGVersionID // MPEG version code
		{
			get { return this.FMPEGVersionID; }   
		}
		public String MPEGVersion // MPEG version name
		{
			get { return this.FGetMPEGVersion(); }
		}
		public byte ProfileID // Profile code
		{		
			get { return this.FProfileID; }
		}
		public String Profile // Profile name
		{
			get { return this.FGetProfile(); }
		}
		public byte Channels // Number of channels
		{
			get { return this.FChannels; }
		}
		public int SampleRate // Sample rate (Hz)
		{
			get { return this.FSampleRate; }
		}
		public double BitRate // Bit rate (Kbit/s)
		{
			get { return (int)Math.Round((double)this.FBitRate/1000); }
		}
		public byte BitRateTypeID // Bit rate type code
		{
			get { return this.FBitRateTypeID; }
		}
		public String BitRateType // Bit rate type name
		{
			get { return this.FGetBitRateType(); }
		}
		public double Duration // Duration (seconds)
		{
			get { return this.FGetDuration(); }
		}
		public bool Valid // true if data valid
		{
			get { return this.FIsValid(); }
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
		public bool IsVBR
		{
			get { return (AAC_BITRATE_TYPE_VBR == FBitRateTypeID); }
		}
		public int CodecFamily
		{
			get { return AudioReaderFactory.CF_LOSSY; }
		}

		// Sample rate values
		private static int[] SAMPLE_RATE = { 96000, 88200, 64000, 48000, 44100, 32000,
										24000, 22050, 16000, 12000, 11025, 8000, 
										0, 0, 0, 0};

		// ********************* Auxiliary functions & procedures ********************

		uint ReadBits(BinaryReader Source, int Position, int Count)
		{
			byte[] buffer = new byte[4];	
	
			// Read a number of bits from file at the given position
			Source.BaseStream.Seek(Position / 8, SeekOrigin.Begin); // integer division =^ div
			buffer = Source.ReadBytes(4);
			uint result = (uint)( (buffer[0] << 24) + (buffer[1] << 16) + (buffer[2] << 8) + buffer[3] );
			result = (result << (Position % 8)) >> (32 - Count);

			return result;
		}

		// ********************** Private functions & procedures *********************

		// Reset all variables
		void FResetData()
		{
			FFileSize = 0;
			FHeaderTypeID = AAC_HEADER_TYPE_UNKNOWN;
			FMPEGVersionID = AAC_MPEG_VERSION_UNKNOWN;
			FProfileID = AAC_PROFILE_UNKNOWN;
			FChannels = 0;
			FSampleRate = 0;
			FBitRate = 0;
			FBitRateTypeID = AAC_BITRATE_TYPE_UNKNOWN;
			FID3v1.ResetData();
			FID3v2.ResetData();
			FAPEtag.ResetData();
			FTotalFrames = 0;
		}

		// ---------------------------------------------------------------------------

		// Get header type name
		String FGetHeaderType()
		{  
			return AAC_HEADER_TYPE[FHeaderTypeID];
		}

		// ---------------------------------------------------------------------------

		// Get MPEG version name
		String FGetMPEGVersion()
		{  
			return AAC_MPEG_VERSION[FMPEGVersionID];
		}

		// ---------------------------------------------------------------------------

		// Get profile name
		String FGetProfile()
		{  
			return  AAC_PROFILE[FProfileID];
		}

		// ---------------------------------------------------------------------------

		// Get bit rate type name
		String FGetBitRateType()
		{  
			return AAC_BITRATE_TYPE[FBitRateTypeID];
		}

		// ---------------------------------------------------------------------------

		// Calculate duration time
		double FGetDuration()
		{
			if (0 == FBitRate) 
				return 0;
			else 
				return 8 * (FFileSize - ID3v2.Size) / FBitRate;
		}

		// ---------------------------------------------------------------------------

		// Check for file correctness
		bool FIsValid()
		{ 
			return ( (FHeaderTypeID != AAC_HEADER_TYPE_UNKNOWN) &&
				(FChannels > 0) && (FSampleRate > 0) && (FBitRate > 0) );
		}

		// ---------------------------------------------------------------------------

		// Get header type of the file
		byte FRecognizeHeaderType(BinaryReader Source)
		{
			byte result;
			char[] Header = new char[4]; 
		
			result = AAC_HEADER_TYPE_UNKNOWN;
			Source.BaseStream.Seek(FID3v2.Size, SeekOrigin.Begin);
			Header = Utils.ReadTrueChars(Source,4);
			//Header = Source.ReadChars(4);
			if ( Utils.StringEqualsArr("ADIF",Header) )
			{
				result = AAC_HEADER_TYPE_ADIF;
			}
			else if ( ( 0xFF == (byte)Header[0] ) && ( 0xF0 == ( ((byte)Header[0]) & 0xF0) ) )
			{
				result = AAC_HEADER_TYPE_ADTS;
			}
			return result;
		}

		// ---------------------------------------------------------------------------

		// Read ADIF header data
		void FReadADIF(BinaryReader Source)
		{
			int Position;

			Position = FID3v2.Size * 8 + 32;
			if ( 0 == ReadBits(Source, Position, 1) ) Position += 3;
			else Position += 75;
			if ( 0 == ReadBits(Source, Position, 1) ) FBitRateTypeID = AAC_BITRATE_TYPE_CBR;
			else FBitRateTypeID = AAC_BITRATE_TYPE_VBR;
		
			Position++;

			FBitRate = (int)ReadBits(Source, Position, 23);

			if ( AAC_BITRATE_TYPE_CBR == FBitRateTypeID ) Position += 51;
			else Position += 31;

			FMPEGVersionID = AAC_MPEG_VERSION_4;
			FProfileID = (byte)(ReadBits(Source, Position, 2) + 1);
			Position += 2;

			FSampleRate = SAMPLE_RATE[ReadBits(Source, Position, 4)];
			Position += 4;
			FChannels += (byte)ReadBits(Source, Position, 4);
			Position += 4;
			FChannels += (byte)ReadBits(Source, Position, 4);
			Position += 4;
			FChannels += (byte)ReadBits(Source, Position, 4);
			Position += 4;
			FChannels += (byte)ReadBits(Source, Position, 2);
		}

		// ---------------------------------------------------------------------------

		// Read ADTS header data
		void FReadADTS(BinaryReader Source)
		{
			int Frames = 0;
			int TotalSize = 0;
			int Position;
	  
			do
			{
				Frames++;
				Position = (FID3v2.Size + TotalSize) * 8;

				if ( ReadBits(Source, Position, 12) != 0xFFF ) break;
			
				Position += 12;

				if ( 0 == ReadBits(Source, Position, 1) )
					FMPEGVersionID = AAC_MPEG_VERSION_4;			
				else
					FMPEGVersionID = AAC_MPEG_VERSION_2;
			
				Position += 4;
				FProfileID = (byte)(ReadBits(Source, Position, 2) + 1);
				Position += 2;

				FSampleRate = SAMPLE_RATE[ReadBits(Source, Position, 4)];
				Position += 5;

				FChannels = (byte)ReadBits(Source, Position, 3);

				if ( AAC_MPEG_VERSION_4 == FMPEGVersionID )
					Position += 9;
				else 
					Position += 7;

				TotalSize += (int)ReadBits(Source, Position, 13);
				Position += 13;

				if ( 0x7FF == ReadBits(Source, Position, 11) ) 
					FBitRateTypeID = AAC_BITRATE_TYPE_VBR;
				else
					FBitRateTypeID = AAC_BITRATE_TYPE_CBR;

				if ( AAC_BITRATE_TYPE_CBR == FBitRateTypeID ) break;
				// more accurate
				//until (Frames != 1000) && (Source.Size > FID3v2.Size + TotalSize)
			}
			while (Source.BaseStream.Length > FID3v2.Size + TotalSize);
			FTotalFrames = Frames;
			FBitRate = (int)Math.Round(8 * (double)TotalSize / 1024 / Frames * FSampleRate);
		}

		// ********************** Public functions & procedures ********************** 

		// constructor
		public TAACfile()
		{
			FID3v1 = new TID3v1();
			FID3v2 = new TID3v2();
			FAPEtag = new TAPEtag();
			FResetData();		
		}

		// --------------------------------------------------------------------------- 

		// No explicit destructor in C#

		// --------------------------------------------------------------------------- 

		// Read data from file
		public bool ReadFromFile(String FileName)
		{		
			FileStream fs = null;
			BinaryReader Source = null;

			bool result = false;
	  
			FResetData();
			// At first search for tags, then try to recognize header type
			FID3v2.ReadFromFile(FileName);
			FID3v1.ReadFromFile(FileName);
			FAPEtag.ReadFromFile(FileName);
			try
			{
				fs = new FileStream(FileName, FileMode.Open);
				fs.Lock(0,fs.Length);
				Source = new BinaryReader(fs);

				FFileSize = (int)fs.Length;
				FHeaderTypeID = FRecognizeHeaderType(Source);
				// Read header data
				if ( AAC_HEADER_TYPE_ADIF == FHeaderTypeID ) FReadADIF(Source);
				if ( AAC_HEADER_TYPE_ADTS == FHeaderTypeID ) FReadADTS(Source);

				result = true;
			}
			catch (Exception e)
			{
				result = false;
				System.Console.WriteLine(e.StackTrace);
				LogDelegator.GetLogDelegate()( Log.LV_ERROR, e.Message);
			}

			if (fs != null)
			{
				fs.Unlock(0,fs.Length);
				if (Source != null) Source.Close();				
			}

			return result;
		}

	}
}