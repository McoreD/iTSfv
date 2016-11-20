// ***************************************************************************
//                                                                            
// Audio Tools Library                                                        
// Class TTTA - for manipulating with TTA Files                               
//                                                                            
// http://mac.sourceforge.net/atl/                                            
// e-mail: macteam@users.sourceforge.net                                      
//                                                                            
// Copyright (c) 2004-2005 by Gambit                                          
//            
// 23rd Jun 2005 - Translated to C# by Zeugma 440
//
// Version 1.1 (April 2005) by Gambit                                         
//   - updated to unicode file access                                         
//                                                                            
// Version 1.0 (12 August 2004)                                               
//                                                                            
// This library is free software; you can redistribute it and/or              
// modify it under the terms of the GNU Lesser General Public                 
// License as published by the Free Software Foundation; either               
// version 2.1 of the License, or (at your option) any later version.         
//                                                                            
// This library is distributed in the hope that it will be useful,            
// but WITHOUT ANY WARRANTY; without even the implied warranty of             
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU          
// Lesser General Public License for more details.                            
//                                                                            
// You should have received a copy of the GNU Lesser General Public           
// License along with this library; if not, write to the Free Software        
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA  
//                                                                            
// ***************************************************************************

using System;
using System.IO;

namespace ATL.AudioReaders.BinaryLogic
{
	class TTTA : AudioDataReader
	{
		private class tta_header
		{
			//TTAid: array[0..3] of Char;
    
			public ushort AudioFormat;
			public ushort NumChannels;
			public ushort BitsPerSample;
			public uint SampleRate;
			public uint DataLength;
			public uint CRC32;
    
			public void Reset()    
			{
				AudioFormat = 0;
				NumChannels = 0;
				BitsPerSample = 0;
				SampleRate = 0;
				DataLength = 0;
				CRC32 = 0;
			}
		}

		// Private declarations
		private long FFileSize;
		private bool FValid;

		private uint FAudioFormat;
		private uint FChannels;
		private uint FBits;
		private uint FSampleRate;
		private uint FSamples;
		private uint FCRC32;

		private double FBitrate;
		private double FDuration;

		private TID3v1 FID3v1;
		private TID3v2 FID3v2;
		private TAPEtag FAPEtag;


		// Public declarations    
		public long FileSize
		{
			get { return FFileSize; }
		}
		public bool Valid
		{
			get { return FValid; }
		}    
    
		public uint Channels
		{
			get { return FChannels; }
		}
		public uint Bits
		{
			get { return FBits; }
		}
		public uint SampleRate
		{
			get { return FSampleRate; }
		}
        
		public double BitRate
		{
			get { return FBitrate; }
		}
		public bool IsVBR
		{
			get { return false; }
		}
		public int CodecFamily
		{
			get { return AudioReaderFactory.CF_LOSSY; }
		}
		public double Duration
		{
			get { return FDuration; }
		}
		public double Ratio
		{
			get { return FGetRatio(); }
		}

		public uint Samples // Number of samples
		{
			get { return FSamples; }	
		}
		public uint CRC32
		{
			get { return FCRC32; }	
		}
		public uint AudioFormat
		{
			get { return FAudioFormat; }	
		}
	
		public TID3v1 ID3v1 // ID3v1 tag data
		{
			get { return FID3v1; }	
		}
		public TID3v2 ID3v2 // ID3v2 tag data
		{
			get { return FID3v2; }	
		}
		public TAPEtag APEtag // APE tag data
		{
			get { return FAPEtag; }	
		}

		// ********************** Private functions & voids *********************

		private void FResetData()
		{
			// Reset all data
			FFileSize = 0;
			FValid = false;

			FAudioFormat = 0;
			FChannels = 0;
			FBits = 0;
			FSampleRate = 0;
			FSamples = 0;
			FCRC32 = 0;

			FBitrate = 0;
			FDuration = 0;

			FID3v1.ResetData();
			FID3v2.ResetData();
			FAPEtag.ResetData();
		}


		// ********************** Public functions & voids **********************

		public TTTA()
		{
			// Create object  
			FID3v1   = new TID3v1();
			FID3v2   = new TID3v2();
			FAPEtag  = new TAPEtag();
			FResetData();
		}

		/* -------------------------------------------------------------------------- */

		// No explicit destructors with C#

		/* -------------------------------------------------------------------------- */

		public bool ReadFromFile(String FileName)
		{  
			FileStream fs = null;
			BinaryReader source = null;

			char[] signatureChunk = new char[4];
			tta_header ttaheader = new tta_header();
			long TagSize;

			bool result = false;
  
  
			FResetData();
  
			// load tags first
			FID3v2.ReadFromFile(FileName);
			FID3v1.ReadFromFile(FileName);
			FAPEtag.ReadFromFile(FileName);
  
			// calulate total tag size
			TagSize = 0;
			if (FID3v1.Exists)  TagSize += 128;
			if (FID3v2.Exists)  TagSize += FID3v2.Size;
			if (FAPEtag.Exists)  TagSize += FAPEtag.Size;
  
			// begin reading data from file  
			try
			{
				fs = new FileStream(FileName,FileMode.Open, FileAccess.Read);
				fs.Lock(0,fs.Length);
				source = new BinaryReader(fs);

    	
				// seek past id3v2-tag
				if ( FID3v2.Exists )
				{
					fs.Seek(FID3v2.Size, SeekOrigin.Begin);
				}

				signatureChunk = source.ReadChars(4);
				if ( Utils.StringEqualsArr("TTA1",signatureChunk) ) 
				{    	
					// start looking for chunks
					ttaheader.Reset();
      		
					ttaheader.AudioFormat = source.ReadUInt16();
					ttaheader.NumChannels = source.ReadUInt16();
					ttaheader.BitsPerSample = source.ReadUInt16();
					ttaheader.SampleRate = source.ReadUInt32();
					ttaheader.DataLength = source.ReadUInt32();
					ttaheader.CRC32 = source.ReadUInt32();

					FFileSize = fs.Length;
					FValid = true;

					FAudioFormat = ttaheader.AudioFormat;
					FChannels = ttaheader.NumChannels;
					FBits = ttaheader.BitsPerSample;
					FSampleRate = ttaheader.SampleRate;
					FSamples = ttaheader.DataLength;
					FCRC32 = ttaheader.CRC32;

					FBitrate = (double)FFileSize * 8 / (FSamples / FSampleRate) / 1000;
					FDuration = (double)ttaheader.DataLength / ttaheader.SampleRate;

					result = true;
				}
			} 
			catch (Exception e) 
			{
				System.Console.WriteLine(e.StackTrace);
				result = false;
			}
  
			if (fs != null)  
			{
				fs.Unlock(0,fs.Length);
				if (source != null) source.Close();
			}

  
			return result;
		}

		/* -------------------------------------------------------------------------- */

		private double FGetRatio()
		{
			// Get compression ratio
			if ( FValid )
				return (double)FFileSize / (FSamples * (FChannels * FBits / 8) + 44) * 100;
			else
				return 0;
		}

		/* -------------------------------------------------------------------------- */

	}
}
