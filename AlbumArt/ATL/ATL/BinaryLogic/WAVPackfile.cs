// ***************************************************************************
//                                                                            
// Audio Tools Library                                                        
// Class TWAVPackFile - for manipulating with WAVPack Files                   
//                                                                            
// http://mac.sourceforge.net/atl/                                            
// e-mail: macteam@users.sourceforge.net                                      
//                                                                            
// Copyright (c) 2003-2005 by Mattias Dahlberg                                
//
// 23rd Jun 2005 - Translated to C# by Zeugma 440
//                                                                            
// Version 1.2 (09 August 2004) by jtclipper                                  
//   - updated to support WavPack version 4 files                             
//   - added encoder detection                                                
//                                                                            
// Version 1.1 (April 2004) by Gambit                                         
//   - Added Ratio and Samples property                                       
//                                                                            
// Version 1.0 (August 2003)                                                  
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
	class TWAVPackfile : AudioDataReader
	{
		private long FFileSize;
		private bool FValid;
		private int FFormatTag;
		private int FVersion;
		private int FChannels;
		private int FSampleRate;
		private int FBits;
	
		private double FBitrate;	
		private double FDuration;
	
		private String FEncoder;
		private TAPEtag FAPEtag;
		private int FTagSize;

		private long FSamples;
		private long FBSamples;


		public long FileSize
		{
			get { return FFileSize; }
		}
		public bool Valid
		{
			get { return FValid; }
		}    
    
		public int FormatTag
		{
			get  { return FFormatTag; }
		}
		public int Version
		{
			get  { return FVersion; }
		}    
    
		public int Channels
		{
			get { return FChannels; }
		}
		public int Bits
		{
			get { return FBits; }
		}
		public int SampleRate
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
			get { return AudioReaderFactory.CF_LOSSLESS; }
		}
		public double Duration
		{
			get { return FDuration; }
		}
		public String ChannelMode
		{
			get { return FGetChannelMode(); }
		}		
	
		public long Samples
		{
			get { return FSamples; }
		}
		public long BSamples
		{
			get { return FBSamples; }
		}
		public double Ration
		{
			get { return FGetRatio(); }
		}
		public String Encoder
		{
			get { return FEncoder; }
		}		

		public TAPEtag APEtag
		{
			get { return FAPEtag; }
		}
		public TID3v1 ID3v1
		{
			get { return new TID3v1(); }
		}
		public TID3v2 ID3v2
		{
			get { return new TID3v2(); }
		}



		private class wavpack_header3
		{
			public char[] ckID = new char[4];
			public uint ckSize;
			public ushort version;
			public ushort bits;
			public ushort flags;
			public ushort shift;
			public uint total_samples;
			public uint crc;
			public uint crc2;
			public char[] extension = new char[4];
			public byte extra_bc;
			public char[] extras = new char[3];
		
			public void Reset()
			{
				Array.Clear(ckID,0,4);
				ckSize = 0;
				version = 0;
				bits = 0;
				flags = 0;
				shift = 0;
				total_samples = 0;
				crc = 0;
				crc2 = 0;
				Array.Clear(extension,0,4);
				extra_bc = 0;
				Array.Clear(extras,0,3);
			}
		}

		private class wavpack_header4
		{
			public char[] ckID = new char[4];
			public uint ckSize;
			public ushort version;
			public byte track_no;
			public byte index_no;
			public uint total_samples;
			public uint block_index;
			public uint block_samples;
			public uint flags;
			public uint crc;
		
			public void Reset()
			{
				Array.Clear(ckID,0,4);
				ckSize = 0;
				version = 0;
				track_no = 0;
				index_no = 0;
				total_samples = 0;
				block_index = 0;
				block_samples = 0;
				flags = 0;
				crc = 0;
			}
		}

		private struct fmt_chunk
		{
			public ushort wformattag;
			public ushort wchannels;
			public uint dwsamplespersec;
			public uint dwavgbytespersec;
			public ushort wblockalign;
			public ushort wbitspersample;
		
			public void Reset()
			{
				wformattag = 0;
				wchannels = 0;
				dwsamplespersec = 0;
				dwavgbytespersec = 0;
				wblockalign = 0;
				wbitspersample = 0;
			}
		}

		private class riff_chunk
		{
			public char[] id = new char[4];
			public uint size;
		
			public void Reset()
			{
				Array.Clear(id,0,3);
				size = 0;
			}
		}

		//version 3 flags
		private const 	int 	MONO_FLAG_v3		= 1;		// not stereo
		private const 	int 	FAST_FLAG_v3		= 2;   		// non-adaptive predictor and stereo mode
		//private const 	int 	RAW_FLAG_v3			= 4;   		// raw mode (no .wav header)
		//private const 	int 	CALC_NOISE_v3		= 8;   		// calc noise in lossy mode (no longer stored)
		private const 	int 	HIGH_FLAG_v3		= 0x10;		// high quality mode (all modes)
		//private const 	int 	BYTES_3_v3			= 0x20;		// files have 3-byte samples
		//private const 	int 	OVER_20_v3			= 0x40;		// samples are over 20 bits
		private const 	int 	WVC_FLAG_v3			= 0x80;		// create/use .wvc (no longer stored)
		//private const 	int 	LOSSY_SHAPE_v3		= 0x100;	// noise shape (lossy mode only)
		//private const 	int 	VERY_FAST_FLAG_v3	= 0x200;	// double fast (no longer stored)
		private const 	int 	NEW_HIGH_FLAG_v3	= 0x400;	// new high quality mode (lossless only)
		//private const 	int 	CANCEL_EXTREME_v3	= 0x800;	// cancel EXTREME_DECORR
		//private const 	int 	CROSS_DECORR_v3		= 0x1000;	// decorrelate chans (with EXTREME_DECORR flag)
		//private const 	int 	NEW_DECORR_FLAG_v3	= 0x2000;	// new high-mode decorrelator
		//private const 	int 	JOINT_STEREO_v3		= 0x4000;	// joint stereo (lossy and high lossless)
		private const 	int 	EXTREME_DECORR_v3	= 0x8000;	// extra decorrelation (+ enables other flags)

		private static int[] sample_rates = new int[15] { 6000, 8000, 9600, 11025, 12000, 16000, 22050,
															24000, 32000, 44100, 48000, 64000, 88200, 96000, 192000 };

		// ---------------------------------------------------------------------------

		private void FResetData()
		{
			FFileSize = 0;
			FTagSize = 0;
			FValid = false;
			FFormatTag = 0;
			FChannels = 0;
			FSampleRate = 0;
			FBits = 0;
			FBitrate = 0;
			FDuration = 0;
			FVersion = 0;
			FEncoder = "";
			FSamples = 0;
			FBSamples = 0;
			FAPEtag.ResetData();
		}

		// ---------------------------------------------------------------------------

		public TWAVPackfile()
		{	
			FAPEtag  = new TAPEtag();
			FResetData();
		}

		// No explicit destructor with C#

		// ---------------------------------------------------------------------------

		public String FGetChannelMode()
		{
			switch( FChannels )
			{
				case 1: return "Mono"; 
				case 2: return "Stereo";
				default : return "Surround";
			}
		}

		// ---------------------------------------------------------------------------

		public bool ReadFromFile(String FileName)
		{    
			FileStream fs = null;
			BinaryReader source = null;
  
			char[] marker = new char[4];
  
			bool result = false;

			FResetData();
			FAPEtag.ReadFromFile(FileName);
			FTagSize = FAPEtag.Size;

			try
			{
				fs = new FileStream(FileName,FileMode.Open, FileAccess.Read);
				fs.Lock(0,fs.Length);
				source = new BinaryReader(fs);

				FFileSize = fs.Length;
    	
				//read first bytes    	
				Array.Clear(marker,0,4);
    	
				marker = source.ReadChars(4);
				fs.Seek( 0, SeekOrigin.Begin );
				
				if ( Utils.StringEqualsArr("RIFF",marker) )
				{
					result = _ReadV3( source );
				}
				else 
				{
					if ( Utils.StringEqualsArr("wvpk",marker) )
					{
						result = _ReadV4( source );
					}
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
				fs.Close();
			}

			return result;
		}

		// ---------------------------------------------------------------------------

		private bool _ReadV4( BinaryReader r )
		{
			wavpack_header4 wvh4 = new wavpack_header4();
			byte[] EncBuf = new byte[4096];
			int tempo;
			byte encoderbyte;

			bool result = false;
  
  
			wvh4.Reset();
  
			wvh4.ckID = r.ReadChars(4);
			wvh4.ckSize = r.ReadUInt32();
			wvh4.version = r.ReadUInt16();
			wvh4.track_no = r.ReadByte();
			wvh4.index_no = r.ReadByte();  
			wvh4.total_samples = r.ReadUInt32();
			wvh4.block_index = r.ReadUInt32();
			wvh4.block_samples = r.ReadUInt32();
			wvh4.flags = r.ReadUInt32();
			wvh4.crc = r.ReadUInt32();  

			if ( Utils.StringEqualsArr("wvpk",wvh4.ckID) )  // wavpack header found
			{
				result = true;
				FValid = true;
				FVersion = (wvh4.version >> 8);
				FChannels = (int)(2 - (wvh4.flags & 4));  // mono flag

				FBits = (int)((wvh4.flags & 3) * 16);   // bytes stored flag
				FSamples = wvh4.total_samples;
				FBSamples = wvh4.block_samples;
				FSampleRate = (int)((wvh4.flags & (0x1F << 23)) >> 23);
				if ( (FSampleRate > 14) || (FSampleRate < 0) )
				{
					FSampleRate = 44100;
				}
				else
				{
					FSampleRate = sample_rates[ FSampleRate ];
				}

				if (8 == (wvh4.flags & 8) )  // hybrid flag
				{
					FEncoder = "hybrid lossy";
				}
				else
				{ //if (2 == (wvh4.flags & 2) )  {  // lossless flag
					FEncoder = "lossless";
				}

				/*
					if ((wvh4.flags & 0x20) > 0)  // MODE_HIGH
					{
					  FEncoder = FEncoder + " (high)";
					end
					else if ((wvh4.flags & 0x40) > 0)  // MODE_FAST
					{
					  FEncoder = FEncoder + " (fast)";
					}
				*/

				FDuration = (double)wvh4.total_samples / FSampleRate;
				if ( FDuration > 0 )
					FBitrate = (FFileSize - FTagSize) * 8 / (double)(FSamples / (double)FSampleRate) / 1000;
    
				Array.Clear(EncBuf,0,4096);
				EncBuf = r.ReadBytes(4096);    
    
				for (tempo=0; tempo<4096; tempo++)
				{
					if (0x65 == EncBuf[tempo] )
					{
						if (0x02 == EncBuf[tempo + 1] )
						{
							encoderbyte = EncBuf[tempo + 2];
							switch (encoderbyte)
							{
								case 8: FEncoder = FEncoder + " (high)"; break;
								case 0: FEncoder = FEncoder + " (normal)"; break;
								case 2: FEncoder = FEncoder + " (fast)"; break;
								case 6: FEncoder = FEncoder + " (very fast)"; break;
							}
							break;
						}
					}	
				}

			}
			return result;
		}

		// ---------------------------------------------------------------------------

		private bool _ReadV3( BinaryReader r )
		{
			riff_chunk chunk = new riff_chunk();
			char[] wavchunk = new char[4];
			fmt_chunk fmt;
			bool hasfmt;
			long fpos;
			wavpack_header3 wvh3 = new wavpack_header3();
			bool result = false;
  
  
			hasfmt = false;

			// read and evaluate header
			chunk.Reset();
  
			chunk.id = r.ReadChars(4);
			chunk.size = r.ReadUInt32();
			wavchunk = r.ReadChars(4);
  
			if ( Utils.StringEqualsArr("WAVE",wavchunk) ) return result;

			// start looking for chunks
			chunk.Reset();
  
			while (r.BaseStream.Position < r.BaseStream.Length) 
			{
				chunk.id = r.ReadChars(4);
				chunk.size = r.ReadUInt32();
  	
				if (chunk.size <= 0) break;
     	
				fpos = r.BaseStream.Position;

				if ( Utils.StringEqualsArr("fmt ",chunk.id) )  // Format chunk found read it
				{
					if ( chunk.size >= 16/*sizeof(fmt_chunk)*/ )
					{
						fmt.wformattag = r.ReadUInt16();
						fmt.wchannels = r.ReadUInt16();
						fmt.dwsamplespersec = r.ReadUInt32();
						fmt.dwavgbytespersec = r.ReadUInt32();
						fmt.wblockalign = r.ReadUInt16();
						fmt.wbitspersample = r.ReadUInt16();

						hasfmt = true;
						result = true;
						FValid = true;
						FFormatTag = fmt.wformattag;
						FChannels = fmt.wchannels;
						FSampleRate = (int)fmt.dwsamplespersec;
						FBits = fmt.wbitspersample;
						FBitrate = (double)fmt.dwavgbytespersec / 125.0; // 125 == 1/8*1000
					} 
					else 
					{
						break;
					}
				}
				else         
				{
					if ( ( Utils.StringEqualsArr("data",chunk.id)) && hasfmt )
					{
						wvh3.Reset();
        		
						wvh3.ckID = r.ReadChars(4);
						wvh3.ckSize = r.ReadUInt32();
						wvh3.version = r.ReadUInt16();
						wvh3.bits = r.ReadUInt16();
						wvh3.flags = r.ReadUInt16();
						wvh3.shift = r.ReadUInt16();
						wvh3.total_samples = r.ReadUInt32();
						wvh3.crc = r.ReadUInt32();
						wvh3.crc2 = r.ReadUInt32();
						wvh3.extension = r.ReadChars(4);
						wvh3.extra_bc = r.ReadByte();
						wvh3.extras = r.ReadChars(3);
        								
						if ( Utils.StringEqualsArr("wvpk",wvh3.ckID) )  // wavpack header found
						{
							result = true;
							FValid = true;
							FVersion = wvh3.version;
							FChannels = 2 - (wvh3.flags & 1);  // mono flag
							FSamples = wvh3.total_samples;

							// Encoder guess
							if (wvh3.bits > 0)
							{
								if ( (wvh3.flags & NEW_HIGH_FLAG_v3) > 0 )
								{
									FEncoder = "hybrid";
									if ( (wvh3.flags & WVC_FLAG_v3) > 0 )
									{
										FEncoder += " lossless";
									} 
									else 
									{
										FEncoder += " lossy";                 					
									}
                 			
									if ((wvh3.flags & EXTREME_DECORR_v3) > 0) 
										FEncoder = FEncoder + " (high)";
								}
								else
								{
									if ( (wvh3.flags & (HIGH_FLAG_v3 | FAST_FLAG_v3)) == 0 )
									{
										FEncoder = ( wvh3.bits + 3 ).ToString() + "-bit lossy";
									} 
									else 
									{
										FEncoder = ( wvh3.bits + 3 ).ToString() + "-bit lossy";
										if ( (wvh3.flags & HIGH_FLAG_v3) > 0 )
										{
											FEncoder += " high";
										} 
										else 
										{
											FEncoder += " fast";
										}
									}
								}
							} 
							else 
							{
								if ( (wvh3.flags & HIGH_FLAG_v3) == 0 )  
								{
									FEncoder = "lossless (fast mode)";
								} 
								else 
								{
									if ( (wvh3.flags & EXTREME_DECORR_v3) > 0 )  
									{
										FEncoder = "lossless (high mode)";
									} 
									else 
									{
										FEncoder = "lossless";
									}
              				
								}
							}
				
							if ( FSampleRate <= 0)  FSampleRate = 44100;
							FDuration = (double)wvh3.total_samples / FSampleRate;
							if (FDuration > 0)  FBitrate = 8.0*(FFileSize  - (long)FTagSize  - (double)(wvh3.ckSize))/(FDuration*1000.0);
						}
						break;
					} 
					else // not a wv file
					{ 
						break;
					}
				}
			} // while

			return result;
		}

		// ---------------------------------------------------------------------------

		private double FGetRatio()
		{
			// Get compression ratio
			if ( FValid )
				return (double)FFileSize / (FSamples * (FChannels * FBits / 8) + 44) * 100;
			else
				return 0;
		}
     
		// ---------------------------------------------------------------------------

	}
}