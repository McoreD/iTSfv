// ***************************************************************************
//                                                                            
// Audio Tools Library                                                        
// Class TDTS - for manipulating with DTS Files                               
//                                                                            
// http://mac.sourceforge.net/atl/                                            
// e-mail: macteam@users.sourceforge.net                                      
//                                                                            
// Copyright (c) 2005 by Gambit                                               
//
// 23rd Jun 2005 - Translated to C# by Zeugma 440
//                                                                            
// Version 1.1 (April 2005) by Gambit                                         
//   - updated to unicode file access                                         
//                                                                            
// Version 1.0 (10 January 2005)                                              
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
using ATL.Logging;

namespace ATL.AudioReaders.BinaryLogic
{
	class TDTS : AudioDataReader
	{

		private static int[] BITRATES = new int[32] { 32, 56, 64, 96, 112, 128, 192, 224, 256,
														320, 384, 448, 512, 576, 640, 768, 960,
														1024, 1152, 1280, 1344, 1408, 1411, 1472,
														1536, 1920, 2048, 3072, 3840, 0, -1, 1 };
		//open, variable, lossless
   
		// Private declarations
		private long FFileSize;
		private bool FValid;

		private uint FChannels;
		private uint FBits;
		private uint FSampleRate;

		private ushort FBitrate;
		private double FDuration;

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

		public TAPEtag APEtag
		{
			get { return new TAPEtag(); }
		}
		public TID3v1 ID3v1
		{
			get { return new TID3v1(); }
		}
		public TID3v2 ID3v2
		{
			get { return new TID3v2(); }
		}

		// ********************** Private functions & voids *********************

		private void FResetData()
		{
			// Reset all data
			FFileSize = 0;
			FValid = false;

			FChannels = 0;
			FBits = 0;
			FSampleRate = 0;

			FBitrate = 0;
			FDuration = 0;
		}


		// ********************** Public functions & voids **********************

		public TDTS()
		{
			// Create object  
			FResetData();
		}

		/* -------------------------------------------------------------------------- */

		// No explicit destructors with C#

		/* -------------------------------------------------------------------------- */

		public bool ReadFromFile(String FileName)
		{
			FileStream fs = null;
			BinaryReader source = null;
  
			uint signatureChunk;  
			ushort tehWord;  
			byte[] gayDTS = new byte[8];

			bool result = false;
  
  
			FResetData();  

			try
			{
				fs = new FileStream(FileName,FileMode.Open, FileAccess.Read);
				fs.Lock(0,fs.Length);
				source = new BinaryReader(fs);
       	
				signatureChunk = source.ReadUInt32();
				if ( /*0x7FFE8001*/ 25230975 == signatureChunk ) 
				{
					Array.Clear(gayDTS,0,8);	
		
					fs.Seek(3, SeekOrigin.Current);
					gayDTS = source.ReadBytes(8);

					FFileSize = fs.Length;
					FValid = true;

					tehWord = (ushort)(gayDTS[1] | (gayDTS[0] << 8));
		
					switch ((tehWord & 0x0FC0) >> 6)
					{
						case 0: FChannels = 1; break;
						case 1:
						case 2:
						case 3:
						case 4: FChannels = 2; break;
						case 5:
						case 6: FChannels = 3; break;
						case 7:
						case 8: FChannels = 4; break;
						case 9: FChannels = 5; break;
						case 10:
						case 11:
						case 12: FChannels = 6; break;
						case 13: FChannels = 7; break;
						case 14:
						case 15: FChannels = 8; break;
						default: FChannels = 0; break;
					}

					switch ((tehWord & 0x3C) >> 2)
					{
						case 1: FSampleRate = 8000; break;
						case 2: FSampleRate = 16000; break;
						case 3: FSampleRate = 32000; break;
						case 6: FSampleRate = 11025; break;
						case 7: FSampleRate = 22050; break;
						case 8: FSampleRate = 44100; break;
						case 11: FSampleRate = 12000; break;
						case 12: FSampleRate = 24000; break;
						case 13: FSampleRate = 48000; break;
						default: FSampleRate = 0; break;
					}

					tehWord = 0;
					tehWord = (ushort)( gayDTS[2] | (gayDTS[1] << 8) );

					FBitrate = (ushort)BITRATES[(tehWord & 0x03E0) >> 5];

					tehWord = 0;
					tehWord = (ushort)( gayDTS[7] | (gayDTS[6] << 8) );

					switch ((tehWord & 0x01C0) >> 6) 
					{
						case 0:
						case 1: FBits = 16; break;
						case 2:
						case 3: FBits = 20; break;
						case 4:
						case 5: FBits = 24; break;
						default: FBits = 16; break;
					}

					FDuration = (double)FFileSize * 8 / 1000 / FBitrate;

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
				return (double)FFileSize / ((FDuration * FSampleRate) * (FChannels * FBits / 8) + 44) * 100;
			else
				return 0;
		}

		/* -------------------------------------------------------------------------- */

	}
}