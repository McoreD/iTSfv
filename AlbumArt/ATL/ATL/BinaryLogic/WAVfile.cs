// ***************************************************************************
//                                                                            
// Audio Tools Library (Freeware)                                             
// Class TWAVfile - for extracting information from WAV file header           
//                                                                            
// Copyright (c) 2001,2002 by Jurgen Faul                                     
// E-mail: jfaul@gmx.de                                                       
// http://jfaul.de/atl                                                        
//
// 14th May 2005 - Translated to C# by Zeugma 440
//                                                                            
// Version 1.2 (14 January 2002)                                              
//   - Fixed bug with calculating of duration                                 
//   - Some class properties added/changed                                    
//                                                                            
// Version 1.1 (9 October 2001)                                               
//   - Fixed bug with WAV header detection                                    
//                                                                            
// Version 1.0 (31 July 2001)                                                 
//   - Info: channel mode, sample rate, bits per sample, file size, duration  
//                                                                            
// ***************************************************************************

using System;
using System.IO;

namespace ATL.AudioReaders.BinaryLogic
{
	class TWAVfile : AudioDataReader
	{
		// Format type names
		public const String WAV_FORMAT_UNKNOWN = "Unknown";
		public const String WAV_FORMAT_PCM = "Windows PCM";
		public const String WAV_FORMAT_ADPCM = "Microsoft ADPCM";
		public const String WAV_FORMAT_ALAW = "A-LAW";
		public const String WAV_FORMAT_MULAW = "MU-LAW";
		public const String WAV_FORMAT_DVI_IMA_ADPCM = "DVI/IMA ADPCM";
		public const String WAV_FORMAT_MP3 = "MPEG Layer III";

		// Used with ChannelModeID property
		public const byte WAV_CM_MONO = 1;                     // Index for mono mode
		public const byte WAV_CM_STEREO = 2;                 // Index for stereo mode

		// Channel mode names
		public String[] WAV_MODE = new String[3] {"Unknown", "Mono", "Stereo"};

		private bool FValid;
		private ushort FFormatID;
		private byte FChannelNumber;
		private uint FSampleRate;
		private uint FBytesPerSecond;
		private ushort FBlockAlign;
		private byte FBitsPerSample;
		private int FSampleNumber;
		private ushort FHeaderSize;
		private uint FFileSize;
      
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
		public ushort FormatID // Format type code
		{
			get { return this.FFormatID; }
		}	
		public String Format // Format type name
		{
			get { return this.FGetFormat(); }
		}	
		public byte ChannelNumber // Number of channels
		{
			get { return this.FChannelNumber; }
		}		 
		public String ChannelMode // Channel mode name
		{
			get { return this.FGetChannelMode(); }
		}	
		public uint SampleRate // Sample rate (hz)
		{
			get { return this.FSampleRate; }
		}
		public byte BitsPerSample // Bits/sample
		{
			get { return this.FBitsPerSample; }
		}
		public double BitRate // Bitrate (Kbit/s)
		{
			get { return Math.Round( (double)this.FBitsPerSample * this.FSampleRate * this.ChannelNumber /1000 ); }
		}
		public bool IsVBR
		{
			get { return false; }
		}
		public int CodecFamily
		{
			get { return AudioReaderFactory.CF_LOSSLESS; }
		}
		public uint BytesPerSecond // Bytes/second
		{
			get { return this.FBytesPerSecond; }
		}
		public ushort BlockAlign // Block alignment
		{
			get { return this.FBlockAlign; }
		}			
		public ushort HeaderSize // Header size (bytes)
		{
			get { return this.FHeaderSize; }
		}	
		public uint FileSize // File size (bytes)
		{
			get { return this.FFileSize; }
		}
		public double Duration // Duration (seconds)
		{
			get { return this.FGetDuration(); }
		}
  

		private const String DATA_CHUNK = "data";                        // Data chunk ID

		// WAV file header data
		private class WAVRecord
		{
			// RIFF file header
			public char[] RIFFHeader = new char[4];                         // Must be "RIFF"
			public int FileSize;                                // Must be "RealFileSize - 8"
			public char[] WAVEHeader = new char[4];                         // Must be "WAVE"
			// Format information
			public char[] FormatHeader = new char[4];                       // Must be "fmt "
			public int FormatSize;                                             // Format size
			public ushort FormatID;                                       // Format type code
			public ushort ChannelNumber;                                // Number of channels
			public int SampleRate;                                        // Sample rate (hz)
			public int BytesPerSecond;                                        // Bytes/second
			public ushort BlockAlign;                                      // Block alignment
			public ushort BitsPerSample;                                       // Bits/sample
			public char[] DataHeader = new char[4];                          // Can be "data"
			public int SampleNumber;                          // Number of samples (optional)

			public void Reset()
			{
				Array.Clear(RIFFHeader,0,RIFFHeader.Length);
				FileSize = 0;
				Array.Clear(WAVEHeader,0,WAVEHeader.Length);
				Array.Clear(FormatHeader,0,FormatHeader.Length);
				FormatSize = 0;
				FormatID = 0;
				ChannelNumber = 0;
				SampleRate = 0;
				BytesPerSecond = 0;
				BlockAlign = 0;
				BitsPerSample = 0;
				Array.Clear(DataHeader,0,DataHeader.Length);
				SampleNumber = 0;
			}
		}

		// ********************* Auxiliary functions & voids ********************

		private bool ReadWAV(String FileName, ref WAVRecord WAVData)
		{
			bool result = true;
			FileStream fs = null;
			BinaryReader SourceFile = null;

			try
			{
				// Set read-access and open file
				fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
				SourceFile = new BinaryReader(fs);
    
				// Read header		
				WAVData.RIFFHeader = SourceFile.ReadChars(WAVData.RIFFHeader.Length);
				WAVData.FileSize = SourceFile.ReadInt32();
				WAVData.WAVEHeader = SourceFile.ReadChars(WAVData.WAVEHeader.Length);
				WAVData.FormatHeader = SourceFile.ReadChars(WAVData.FormatHeader.Length);
				WAVData.FormatSize = SourceFile.ReadInt32();
				WAVData.FormatID = SourceFile.ReadUInt16();
				WAVData.ChannelNumber = SourceFile.ReadUInt16();
				WAVData.SampleRate = SourceFile.ReadInt32();
				WAVData.BytesPerSecond = SourceFile.ReadInt32();
				WAVData.BlockAlign = SourceFile.ReadUInt16();
				WAVData.BitsPerSample = SourceFile.ReadUInt16();
				WAVData.DataHeader = SourceFile.ReadChars(WAVData.DataHeader.Length);

				// Read number of samples if exists
				if ( ! Utils.StringEqualsArr(DATA_CHUNK,WAVData.DataHeader))
				{
					SourceFile.BaseStream.Seek(WAVData.FormatSize + 28, SeekOrigin.Begin);
					WAVData.SampleNumber = SourceFile.ReadInt32();
				}
			} 
			catch (Exception e) 
			{
				System.Console.WriteLine(e.StackTrace);
				result = false;
			}
			if (SourceFile != null) SourceFile.Close();
			if (fs != null) fs.Close();

			return result;
		}

		// ---------------------------------------------------------------------------

		private bool HeaderIsValid(WAVRecord WAVData)
		{
			bool result = true;
			// Header validation
			if (! Utils.StringEqualsArr("RIFF",WAVData.RIFFHeader)) result = false;
			if (! Utils.StringEqualsArr("WAVE",WAVData.WAVEHeader)) result = false;
			if (! Utils.StringEqualsArr("fmt ",WAVData.FormatHeader)) result = false;
			if ( (WAVData.ChannelNumber != WAV_CM_MONO) &&
				(WAVData.ChannelNumber != WAV_CM_STEREO) ) result = false;

			return result;
		}

		// ********************** Private functions & voids *********************

		private void FResetData()
		{
			// Reset all data
			FValid = false;
			FFormatID = 0;
			FChannelNumber = 0;
			FSampleRate = 0;
			FBytesPerSecond = 0;
			FBlockAlign = 0;
			FBitsPerSample = 0;
			FSampleNumber = 0;
			FHeaderSize = 0;
			FFileSize = 0;
		}

		// ---------------------------------------------------------------------------

		private String FGetFormat()
		{
			// Get format type name
			switch (FFormatID)
			{
				case 1: return WAV_FORMAT_PCM;
				case 2: return WAV_FORMAT_ADPCM;
				case 6: return WAV_FORMAT_ALAW;
				case 7: return WAV_FORMAT_MULAW;
				case 17: return WAV_FORMAT_DVI_IMA_ADPCM;
				case 85: return WAV_FORMAT_MP3;
				default : return "";  
			}
		}

		// ---------------------------------------------------------------------------

		private String FGetChannelMode()
		{
			// Get channel mode name
			return WAV_MODE[FChannelNumber];
		}

		// ---------------------------------------------------------------------------

		private double FGetDuration()
		{
			// Get duration
			double result = 0;
			if (FValid)
			{
				if ((FSampleNumber == 0) && (FBytesPerSecond > 0))
					result = (double)(FFileSize - FHeaderSize) / FBytesPerSecond;
				if ((FSampleNumber > 0) && (FSampleRate > 0))
					result = (double)FSampleNumber / FSampleRate;
			}
			return result;
		}

		// ********************** Public functions & voids **********************

		public TWAVfile()
		{
			// Create object  
			FResetData();
		}

		// ---------------------------------------------------------------------------

		public bool ReadFromFile(String FileName)
		{
			WAVRecord WAVData = new WAVRecord();

			// Reset and load header data from file to variable
			FResetData();
			WAVData.Reset();
  
			bool result = ReadWAV(FileName, ref WAVData);
			// Process data if loaded and header valid
			if ( (result) && (HeaderIsValid(WAVData)) )
			{
				FValid = true;
				// Fill properties with header data
				FFormatID = WAVData.FormatID;
				FChannelNumber = (byte)WAVData.ChannelNumber;
				FSampleRate = (uint)WAVData.SampleRate;
				FBytesPerSecond = (uint)WAVData.BytesPerSecond;
				FBlockAlign = WAVData.BlockAlign;
				FBitsPerSample = (byte)WAVData.BitsPerSample;
				FSampleNumber = WAVData.SampleNumber;
				if ( Utils.StringEqualsArr(DATA_CHUNK, WAVData.DataHeader) ) FHeaderSize = 44;
				else FHeaderSize = (ushort)(WAVData.FormatSize + 40);
				FFileSize = (uint)WAVData.FileSize + 8;
				if (FHeaderSize > FFileSize) FHeaderSize = (ushort)FFileSize;
			}
			return result;
		}

	}
}