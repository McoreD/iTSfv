// *************************************************************************** 
//                                                                             
// Audio Tools Library                                                         
// Class TAC3 - for manipulating with AC3 Files                                
//                                                                             
// http://mac.sourceforge.net/atl/                                             
// e-mail: macteam@users.sourceforge.net                                       
//                                                                             
// Copyright (c) 2005 by Gambit                                                
//                                
// 23rd June 2005 - Translated to C# by Zeugma 440
//
// Version 1.1 (April 2005) by Gambit                                          
//   - updated to unicode file access                                          
//                                                                             
// Version 1.0 (05 January 2005)                                               
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
	class TAC3 : AudioDataReader
	{
		private static int[] BITRATES = new int[19] { 32, 40, 48, 56, 64, 80, 96, 112, 128, 160,
														192, 224, 256, 320, 384, 448, 512, 576, 640 };
 
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

		public TAC3()
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
  
			ushort signatureChunk;
			byte tehByte;

			bool result = false;
  
			FResetData();

			try
			{
				fs = new FileStream(FileName,FileMode.Open, FileAccess.Read);
				fs.Lock(0,fs.Length);
				source = new BinaryReader(fs);
            
				signatureChunk = source.ReadUInt16();
            
				if ( /*0x0B77*/ 30475 == signatureChunk )
				{
					tehByte = 0;
		
					fs.Seek(2, SeekOrigin.Current);
					tehByte = source.ReadByte();

					FFileSize = fs.Length;
					FValid = true;

					switch (tehByte & 0xC0)
					{
						case 0: FSampleRate = 48000; break;
						case 0x40: FSampleRate = 44100; break;
						case 0x80: FSampleRate = 32000; break;
						default : FSampleRate = 0; break;
					}

					FBitrate = (ushort)BITRATES[(tehByte & 0x3F) >> 1];

					tehByte = 0;
	      	
					fs.Seek(1, SeekOrigin.Current);
					tehByte = source.ReadByte();

					switch (tehByte & 0xE0)
					{
						case 0: FChannels = 2; break;
						case 0x20: FChannels = 1; break;
						case 0x40: FChannels = 2; break;
						case 0x60: FChannels = 3; break;
						case 0x80: FChannels = 3; break;
						case 0xA0: FChannels = 4; break;
						case 0xC0: FChannels = 4; break;
						case 0xE0: FChannels = 5; break;
						default : FChannels = 0; break;
					}

					FBits = 16;
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