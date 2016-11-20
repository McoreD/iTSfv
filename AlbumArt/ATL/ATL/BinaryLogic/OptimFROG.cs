// ***************************************************************************
//                                                                            
// Audio Tools Library (Freeware)                                             
// Class TOptimFROG - for manipulating with OptimFROG file information        
//                                                                            
// Uses:                                                                      
//   - Class TID3v1                                                           
//   - Class TID3v2                                                           
//   - Class TAPEtag                                                          
//                                                                            
// Copyright (c) 2001,2002 by Jurgen Faul                                     
// E-mail: jfaul@gmx.de                                                       
// http://jfaul.de/atl                                                        
// Portions (c) 2003 Erik Stenborg                                            
//                                                                            
// 12th May 2005 - Translated to C# by Zeugma 440
//
// Version 1.0 (10 July 2003)                                                 
//   - Support for OptimFROG files via modification of TMonkey class by Jurgen
//   - Class TID3v1: reading & writing support for ID3v1 tags                 
//   - Class TID3v2: reading & writing support for ID3v2 tags                 
//   - Class TAPEtag: reading & writing support for APE tags                  
//                                                                            
// ***************************************************************************

using System;
using System.IO;

namespace ATL.AudioReaders.BinaryLogic
{
	class TOptimFrog : AudioDataReader
	{

		private String[] OFR_COMPRESSION = new String[10] 
		{
			"fast", "normal", "high", "extra",
			"best", "ultra", "insane", "highnew", "extranew", "bestnew"};

		private sbyte[] OFR_BITS = new sbyte[11] 
	{
		8, 8, 16, 16, 24, 24, 32, 32,
		-32, -32, -32 }; //negative value corresponds to floating point type.

		private String[] OFR_CHANNELMODE = new String[2] {"Mono", "Stereo"};

					
		// Real structure of OptimFROG header
		public class TOfrHeader
		{
			public char[] ID = new char[4];                      // Always 'OFR '
			public uint Size;
			public uint Length;
			public ushort HiLength;
			public byte SampleType;
			public byte ChannelMode;
			public int SampleRate;
			public ushort EncoderID;
			public byte CompressionID;
			public void Reset()
			{
				Array.Clear(ID,0,ID.Length);
				Size = 0;
				Length = 0;
				HiLength = 0;
				SampleType = 0;
				ChannelMode = 0;
				SampleRate = 0;
				EncoderID = 0;
				CompressionID = 0;
			}
		}

    
		private long FFileLength;
		private TOfrHeader FHeader = new TOfrHeader();
		private TID3v1 FID3v1;
		private TID3v2 FID3v2;
		private TAPEtag FAPEtag;
      
		public long FileLength // File length (bytes)
		{
			get { return this.FFileLength; }
		}
		public TOfrHeader Header // OptimFROG header
		{
			get { return this.FHeader; }
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
		public bool Valid // True if header valid
		{
			get { return this.FGetValid(); }
		}	
		public String Version // Encoder version
		{
			get { return this.FGetVersion(); }
		}									   
		public String Compression // Compression level
		{
			get { return this.FGetCompression(); }
		}	
		public String ChannelMode // Channel mode
		{
			get { return this.FGetChannelMode(); }
		}
		public sbyte Bits // Bits per sample
		{
			get { return this.FGetBits(); }
		}		  		
		public long Samples // Number of samples
		{
			get { return this.FGetSamples(); }
		}
		public int SampleRate // Sample rate (Hz)
		{
			get { return this.FGetSampleRate(); }
		}
		public double BitRate	// Bitrate; this is a workaround since the theoretical method 
								// doesn't seem to give correct results
		{
			get { return ((this.FFileLength - FHeader.Size)*8 / (Duration*1000) ); }
		}
		public bool IsVBR
		{
			get { return false; }
		}
		public int CodecFamily
		{
			get { return AudioReaderFactory.CF_LOSSLESS; }
		}
		public double Duration // Duration (seconds)
		{
			get { return this.FGetDuration(); }
		}			
		public double Ratio // Compression ratio (%)
		{
			get { return this.FGetRatio(); }
		}	

		// ********************** Private functions & voids *********************

		private void FResetData()
		{
			// Reset data
			FFileLength = 0;
			FHeader.Reset();
			FID3v1.ResetData();
			FID3v2.ResetData();
			FAPEtag.ResetData();
		}

		// ---------------------------------------------------------------------------

		private bool FGetValid()
		{
			return (
				Utils.StringEqualsArr("OFR ", FHeader.ID) &&
				(FHeader.SampleRate > 0) &&
				((0 <= FHeader.SampleType) && (FHeader.SampleType <= 10)) &&
				((0 <= FHeader.ChannelMode) &&(FHeader.ChannelMode <= 1)) &&
				((0 <= FHeader.CompressionID >> 3) && (FHeader.CompressionID >> 3 <= 9)) );
		}

		// ---------------------------------------------------------------------------

		private String FGetVersion()
		{
			// Get encoder version
			return  ( ((FHeader.EncoderID >> 4) + 4500) / 1000 ).ToString().Substring(0,5); // Pas exactement...
		}

		// ---------------------------------------------------------------------------

		private String FGetCompression()
		{
			// Get compression level
			return OFR_COMPRESSION[FHeader.CompressionID >> 3];
		}

		// ---------------------------------------------------------------------------

		private sbyte FGetBits()
		{
			// Get number of bits per sample
			return OFR_BITS[FHeader.SampleType];
		}

		// ---------------------------------------------------------------------------

		private String FGetChannelMode()
		{
			// Get channel mode
			return OFR_CHANNELMODE[FHeader.ChannelMode];
		}

		// ---------------------------------------------------------------------------

		private long FGetSamples()
		{
			//uint[] Res = new uint[2]; // absolute Result		

			// Get number of samples
			/*
		  Res[0] = Header.Length >> Header.ChannelMode;
		  Res[1] = Header.HiLength >> Header.ChannelMode;*/

			return ( ((Header.Length >> Header.ChannelMode) * 0x00000001) +
				((Header.HiLength >> Header.ChannelMode) * 0x00010000) );
		}

		// ---------------------------------------------------------------------------

		private double FGetDuration()
		{
			double nbSamples = (double)FGetSamples();
			// Get song duration
			if (FHeader.SampleRate > 0)
				return nbSamples / FHeader.SampleRate;
			else
				return 0;
		}

		// ---------------------------------------------------------------------------

		private int FGetSampleRate()
		{
			return Header.SampleRate;
		}

		// ---------------------------------------------------------------------------

		private double FGetRatio()
		{
			// Get compression ratio
			if (FGetValid())
				return (double)FFileLength /
					(FGetSamples() * (FHeader.ChannelMode+1) * Math.Abs(FGetBits()) / 8 + 44) * 100;
			else
				return 0;
		}

		// ********************** Public functions & voids **********************

		public TOptimFrog()
		{
			// Create object  
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
			FileStream fs = null;		
			BinaryReader Source = null;

			bool result = false;  

			try
			{
				// Reset data and search for file tag
				FResetData();
				FID3v1.ReadFromFile(FileName);
				FID3v2.ReadFromFile(FileName);
				FAPEtag.ReadFromFile(FileName);
				// Set read-access, open file and get file length
				fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
				fs.Lock(0,fs.Length);
				Source = new BinaryReader(fs);

				FFileLength = fs.Length;
				// Read header data
				Source.BaseStream.Seek(ID3v2.Size, SeekOrigin.Begin);
    
				//Source.Read(FHeader, sizeof(FHeader));

				FHeader.ID = Source.ReadChars(4);
				FHeader.Size = Source.ReadUInt32();
				FHeader.Length = Source.ReadUInt32();
				FHeader.HiLength = Source.ReadUInt16();
				FHeader.SampleType = Source.ReadByte();
				FHeader.ChannelMode = Source.ReadByte();
				FHeader.SampleRate = Source.ReadInt32();
				FHeader.EncoderID = Source.ReadUInt16();
				FHeader.CompressionID = Source.ReadByte();				

				if ( Utils.StringEqualsArr("OFR ",FHeader.ID) )
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
				if (Source != null) Source.Close();
			}
			return result;
		}

	}
}