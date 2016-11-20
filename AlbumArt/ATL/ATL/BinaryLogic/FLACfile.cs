// ***************************************************************************
//                                                                            
// Audio Tools Library                                                        
// Class TFLACfile - for manipulating with FLAC file information              
//                                                                            
// http://mac.sourceforge.net/atl/                                            
// e-mail: macteam@users.sourceforge.net                                      
//                                                                            
// Copyright (c) 2000-2002 by Jurgen Faul                                     
// Copyright (c) 2003-2005 by The MAC Team                                    
//
// 24th Jun 2005 - Translated to C# by Zeugma 440
//
// Version 1.4 (April 2005) by Gambit                                         
//   - updated to unicode file access                                         
//                                                                            
// Version 1.3 (13 August 2004) by jtclipper                                  
//   - unit rewritten, VorbisComment is obsolete now                          
//                                                                            
// Version 1.2 (23 June 2004) by sundance                                     
//   - Check for ID3 tags (although not supported)                            
//   - Don"t parse for other FLAC metablocks if FLAC header is missing        
//                                                                            
// Version 1.1 (6 July 2003) by Erik                                          
//   - Class: Vorbis comments (native comment to FLAC files) added            
//                                                                            
// Version 1.0 (13 August 2002)                                               
//   - Info: channels, sample rate, bits/sample, file size, duration, ratio   
//   - Class TID3v1: reading & writing support for ID3v1 tags                 
//   - Class TID3v2: reading & writing support for ID3v2 tags                 
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
using System.Collections;
using System.IO;
using ATL.Logging;

namespace ATL.AudioReaders.BinaryLogic
{
	public class TFLACFile : AudioDataReader, MetaDataReader
	{

		private const int META_STREAMINFO      = 0;
		private const int META_PADDING         = 1;
		private const int META_APPLICATION     = 2;
		private const int META_SEEKTABLE       = 3;
		private const int META_VORBIS_COMMENT  = 4;
		private const int META_CUESHEET        = 5;


		private class TFlacHeader
		{
			public char[] StreamMarker  = new char[4]; //should always be "fLaC"
			public byte[] MetaDataBlockHeader = new byte[4];
			public byte[] Info = new byte[18];
			public byte[] MD5Sum = new byte[16];
    
			public void Reset()
			{
				Array.Clear(StreamMarker,0,4);
				Array.Clear(MetaDataBlockHeader,0,4);
				Array.Clear(Info,0,18);
				Array.Clear(MD5Sum,0,16);
			}
		}


		private class TMetaData
		{
			public byte[] MetaDataBlockHeader = new byte[4];
			public MemoryStream Data;
    
			public void Reset()
			{
				Array.Clear(MetaDataBlockHeader,0,4);
				Data.Flush();
			}
		}


		// Private declarations
		private TFlacHeader FHeader;
		private String FFileName;
		private int FPaddingIndex;
		private bool FPaddingLast;
		private bool FPaddingFragments;
		private int FVorbisIndex;
		private int FPadding;
		private int FVCOffset;
		private int FAudioOffset;
		private byte FChannels;
		private int FSampleRate;
		private byte FBitsPerSample;
		private double FBitrate;
		private int FFileLength;
		private long FSamples;

		// ArrayList of TMetaData
		private ArrayList aMetaBlockOther;
		//private TMetaData[] aMetaBlockOther;    

		// tag data
		private String FVendor;
		private int FTagSize;
		private bool FExists;

		private TID3v2 FID3v2;
    
		//# Should be accessible outside (no hardcoding there, kthxbye...)
		private static bool bTAG_PreserveDate = false;


		// Meta members (turned to private because of properties)
		private String FTrackString;
		private String FTitle;
		private String FArtist;
		private String FAlbum;
		private String FYear;
		private String FGenre;
		private String FComment;
    
		//extra
		public String xTones;
		public String xStyles;
		public String xMood;
		public String xSituation;
		public String xRating;
		public String xQuality;
		public String xTempo;
		public String xType;

		//
		public String Composer;
		public String Language;
		public String Copyright;
		public String Link;
		public String Encoder;
		public String Lyrics;
		public String Performer;
		public String License;
		public String Organization;
		public String Description;
		public String Location;
		public String Contact;
		public String ISRC;
    
		public ArrayList aExtraFields;
		//public String[][] aExtraFields;


		public TAPEtag APEtag // No APEtag here; declared for interface compliance
		{
			get{ return new TAPEtag(); }
		}
		public TID3v1 ID3v1 // No ID3v1 here; declared for interface compliance
		{
			get{ return new TID3v1(); }
		}
		public TID3v2 ID3v2
		{
			get{ return FID3v2; }
		}
		public byte Channels // Number of channels
		{
			get { return FChannels; }
		}
		public int SampleRate // Sample rate (hz)
		{
			get { return FSampleRate; }
		}
		public byte BitsPerSample // Bits per sample
		{
			get { return FBitsPerSample; }
		}
		public int FileLength // File length (bytes)
		{
			get { return FFileLength; }
		}
		public long Samples // Number of samples
		{
			get { return FSamples; }
		}
		public bool Valid // True if header valid
		{
			get { return FIsValid(); }
		}
		public double Duration // Duration (seconds)
		{
			get { return FGetDuration(); }
		}
		public double Ratio // Compression ratio (%)
		{
			get { return FGetRatio(); }
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
		public String ChannelMode 
		{
			get { return FGetChannelMode(); }
		}
		public bool Exists 
		{
			get { return FExists; }
		}
		public String Vendor 
		{
			get { return FVendor; }
		}
		public String FileName 
		{
			get { return FFileName; }
		}
		public int AudioOffset //offset of audio data
		{
			get { return FAudioOffset; }
		}
		public bool HasLyrics 
		{
			get { return FGetHasLyrics(); }
		}
		
		// PROPERTIES FOR METADATAREADER
		public String Title 
		{
			get { return this.FTitle; }
			set { FTitle = value; }
		}
		public String Artist
		{
			get { return this.FArtist; }
			set { FArtist = value; }
		}
		public String Album
		{
			get { return this.FAlbum; }
			set { FAlbum = value; }
		}	
		public String Year
		{
			get { return this.FYear; }
			set { FYear = value; }
		}	
		public String Comment
		{
			get { return this.FComment; }
			set { FComment = value; }
		}	
		public ushort Track
		{
			get { return (ushort)(Int32.Parse(FTrackString)); }
			set { FTrackString = value.ToString(); }
		}
		public String Genre
		{
			get { return FGenre; }
		}

		/* -------------------------------------------------------------------------- */

		private void FResetData( bool bHeaderInfo, bool bTagFields)
		{
			if (bHeaderInfo)  
			{
				FFileName = "";
				FPadding = 0;
				FPaddingLast = false;
				FPaddingFragments = false;
				FChannels = 0;
				FSampleRate = 0;
				FBitsPerSample = 0;
				FFileLength = 0;
				FSamples = 0;
				FVorbisIndex = 0;
				FPaddingIndex = 0;
				FVCOffset = 0;
				FAudioOffset = 0;

				for (int i = 0; i< aMetaBlockOther.Count ;i++) ((TMetaData)aMetaBlockOther[i]).Data.Close();
				aMetaBlockOther.Clear();      
			}

			//tag data
			if (bTagFields)  
			{
				FVendor = "";
				FTagSize = 0;
				FExists = false;

				FTitle = "";
				FArtist = "";
				FAlbum = "";
				FTrackString = "00";
				FYear = "";
				FGenre = "";
				FComment = "";
				//extra
				xTones = "";
				xStyles = "";
				xMood = "";
				xSituation = "";
				xRating = "";
				xQuality = "";
				xTempo = "";
				xType = "";

				//
				Composer = "";
				Language = "";
				Copyright = "";
				Link = "";
				Encoder = "";
				Lyrics = "";
				Performer = "";
				License = "";
				Organization = "";
				Description = "";
				Location = "";
				Contact = "";
				ISRC = "";
      
				foreach(ArrayList aList in aExtraFields)
				{
					aList.Clear();		
				}
				aExtraFields.Clear();
			}    
		}

		/* -------------------------------------------------------------------------- */
		// Check for right FLAC file data
		private bool FIsValid()
		{
			return ( ( Utils.StringEqualsArr("fLaC",FHeader.StreamMarker) ) &&
				(FChannels > 0) &&
				(FSampleRate > 0) &&
				(FBitsPerSample > 0) &&
				(FSamples > 0) );
		}

		/* -------------------------------------------------------------------------- */

		private double FGetDuration()
		{
			if ( (FIsValid()) && (FSampleRate > 0) )  
			{
				return (double)FSamples / FSampleRate;
			} 
			else 
			{
				return 0;
			}
		}

		/* -------------------------------------------------------------------------- */
		//   Get compression ratio
		private double FGetRatio()
		{
			if (FIsValid()) 
			{
				return (double)FFileLength / (FSamples * FChannels * FBitsPerSample / 8) * 100;
			} 
			else 
			{
				return 0;
			}
		}

		/* -------------------------------------------------------------------------- */
		//   Get channel mode
		private String FGetChannelMode()
		{
			String result;
			if (FIsValid())
			{
				switch(FChannels)
				{
					case 1 : result = "Mono"; break;
					case 2 : result = "Stereo"; break;
					default: result = "Multi Channel"; break;
				}
			} 
			else 
			{
				result = "";
			}
			return result;
		}

		/* -------------------------------------------------------------------------- */

		private bool FGetHasLyrics()
		{
			return ( Lyrics.Trim() != "" );
		}

		/* -------------------------------------------------------------------------- */

		public TFLACFile()
		{  
			FID3v2 = new TID3v2();
			FHeader = new TFlacHeader();
			aMetaBlockOther = new ArrayList();
			aExtraFields = new ArrayList();
			FResetData( true, true );
		}

		// No explicit destructor with C#

		/* -------------------------------------------------------------------------- */

		public bool ReadFromFile( String sFile)
		{
			FResetData( false, true );
			return GetInfo( sFile, true );
		}

		/* -------------------------------------------------------------------------- */

		public bool GetInfo( String sFile, bool bSetTags)
		{
			FileStream fs = null;
			BinaryReader r = null;

			byte[] aMetaDataBlockHeader = new byte[4];
			int iBlockLength;
			int iMetaType;
			int iIndex;
			bool bPaddingFound;

			bool result = true;
  
			bPaddingFound = false;
			FResetData( true, false );
  
			try
			{
				// Read data from ID3 tags
				FID3v2.ReadFromFile(sFile);

				// Set read-access and open file
				fs = new FileStream(sFile,FileMode.Open, FileAccess.Read);
				fs.Lock(0,fs.Length);
				r = new BinaryReader(fs);

				FFileLength = (int)fs.Length;
				FFileName = sFile;

				// Seek past the ID3v2 tag, if there is one
				if (FID3v2.Exists)  
				{
					fs.Seek(FID3v2.Size, SeekOrigin.Begin);
				}

				// Read header data    
				FHeader.Reset();
    
				FHeader.StreamMarker = r.ReadChars(4);
				FHeader.MetaDataBlockHeader = r.ReadBytes(4);
				FHeader.Info = r.ReadBytes(18);
				FHeader.MD5Sum = r.ReadBytes(16);

				// Process data if loaded and header valid    
				if ( Utils.StringEqualsArr("fLaC",FHeader.StreamMarker) )
				{
					FChannels      = (byte)( ((FHeader.Info[12] >> 1) & 0x7) + 1 );
					FSampleRate    = ( FHeader.Info[10] << 12 | FHeader.Info[11] << 4 | FHeader.Info[12] >> 4 );
					FBitsPerSample = (byte)( ((FHeader.Info[12] & 1) << 4) | (FHeader.Info[13] >> 4) + 1 );
					FSamples       = ( FHeader.Info[14] << 24 | FHeader.Info[15] << 16 | FHeader.Info[16] << 8 | FHeader.Info[17] );

					if ( 0 == (FHeader.MetaDataBlockHeader[1] & 0x80) ) // metadata block exists
					{
						iIndex = 0;
						do // read more metadata blocks if available
						{		  
							aMetaDataBlockHeader = r.ReadBytes(4);

							iIndex++; // metadatablock index
							iBlockLength = (aMetaDataBlockHeader[1] << 16 | aMetaDataBlockHeader[2] << 8 | aMetaDataBlockHeader[3]); //decode length
							if (iBlockLength <= 0) break; // can it be 0 ?

							iMetaType = (aMetaDataBlockHeader[0] & 0x7F); // decode metablock type

							if ( iMetaType == META_VORBIS_COMMENT )
							{  // read vorbis block
								FVCOffset = (int)fs.Position;
								FTagSize = iBlockLength;
								FVorbisIndex = iIndex;
								ReadTag(r, bSetTags); // set up fields
							}
							else 
							{
								if ((iMetaType == META_PADDING) && (! bPaddingFound) )  // we have padding block
								{ 
									FPadding = iBlockLength;                                            // if we find more skip & put them in metablock array
									FPaddingLast = ((aMetaDataBlockHeader[0] & 0x80) != 0);
									FPaddingIndex = iIndex;
									bPaddingFound = true;
									fs.Seek(FPadding, SeekOrigin.Current); // advance into file till next block or audio data start
								} 
								else // all other
								{ 
									if (iMetaType <= 5)   // is it a valid metablock ?
									{ 
										if (META_PADDING == iMetaType)  // set flag for fragmented padding blocks
										{ 
											FPaddingFragments = true;
										}
										AddMetaDataOther(aMetaDataBlockHeader, r, iBlockLength, iIndex);
									} 
									else 
									{
										FSamples = 0; //ops...
										break;
									}
								}
							}
						}
						while ( 0 == (aMetaDataBlockHeader[0] & 0x80) ); // while is not last flag ( first bit == 1 )
					}
				}
			} 
			catch (Exception e) 
			{
				System.Console.WriteLine(e.StackTrace);
				LogDelegator.GetLogDelegate()(Log.LV_ERROR,e.Message);
				result = false;
			}
    
			if (FIsValid())  
			{
				FAudioOffset = (int)fs.Position;  // we need that to rebuild the file if nedeed
				FBitrate = Math.Round( ( (double)( FFileLength - FAudioOffset ) / 1000 ) * 8 / FGetDuration() ); //time to calculate average bitrate
			} 
			else 
			{
				result = false;
			}
    
			if (fs != null)  
			{
				fs.Unlock(0,fs.Length);
				fs.Close();
			}

			return result;  
		}

		/* -------------------------------------------------------------------------- */

		public void AddMetaDataOther( byte[] aMetaHeader, BinaryReader stream, int iBlocklength, int iIndex )
		{
			TMetaData theMetaData = new TMetaData();

			theMetaData.MetaDataBlockHeader[0] = aMetaHeader[0];
			theMetaData.MetaDataBlockHeader[1] = aMetaHeader[1];
			theMetaData.MetaDataBlockHeader[2] = aMetaHeader[2];
			theMetaData.MetaDataBlockHeader[3] = aMetaHeader[3];
			// save content in a stream
			theMetaData.Data = new MemoryStream();
			theMetaData.Data.Position = 0;
				
			Utils.CopyMemoryStreamFrom(theMetaData.Data, stream, iBlocklength );

			aMetaBlockOther.Add(theMetaData);
		}

		/* -------------------------------------------------------------------------- */

		public void ReadTag( BinaryReader Source, bool bSetTagFields)
		{  
			int iCount;
			int iSize;
			int iSepPos;
			char[] Data;
			String sFieldID;
			String sFieldData;
			String dataString;

			iSize = Source.ReadInt32(); // vendor
			Data = new char[iSize];  
			Data = Source.ReadChars( iSize );  
			FVendor = new String( Data );

			iCount = Source.ReadInt32();  

			FExists = ( iCount > 0 );

			for (int i=0; i< iCount; i++)
			{
				iSize = Source.ReadInt32();
				Data = Source.ReadChars(iSize);
				dataString = new String(Data);

				//# check the Continue instruction below
				if (! bSetTagFields) continue; // if we don't want to re asign fields we skip
      
				iSepPos = dataString.IndexOf("=");
				if (iSepPos > 0)
				{
					sFieldID = dataString.Substring(0,iSepPos).ToUpper();
					//# check the indexes below
					sFieldData = DecodeUTF8( dataString.Substring(iSepPos+1,dataString.Length-iSepPos-1) ); // ( Copy( String( Data ), iSepPos + 1, MaxInt ) );

					if ( (sFieldID == "TRACKNUMBER") && (FTrackString == "00") )  
					{
						FTrackString = sFieldData;
					} 
					else if ( (sFieldID == "ARTIST") && (FArtist == "") ) 
					{
						FArtist = sFieldData;
					} 
					else if ( (sFieldID == "ALBUM") && (FAlbum == "") ) 
					{
						FAlbum = sFieldData;
					} 
					else if ( (sFieldID == "TITLE") && (FTitle == "") ) 
					{
						FTitle = sFieldData;
					} 
					else if ( (sFieldID == "DATE") && (FYear == "") ) 
					{
						FYear = sFieldData;
					} 
					else if ( (sFieldID == "GENRE") && (FGenre == "") ) 
					{
						FGenre = sFieldData;
					} 
					else if ( (sFieldID == "COMMENT") && (FComment == "") ) 
					{
						FComment = sFieldData;
					} 
					else if ( (sFieldID == "COMPOSER") && (Composer == "") ) 
					{
						Composer = sFieldData;
					} 
					else if ( (sFieldID == "LANGUAGE") && (Language == "") ) 
					{
						Language = sFieldData;
					} 
					else if ( (sFieldID == "COPYRIGHT") && (Copyright == "") ) 
					{
						Copyright = sFieldData;
					} 
					else if ( (sFieldID == "URL") && (Link == "") )  
					{
						Link = sFieldData;
					} 
					else if ( (sFieldID == "ENCODER") && (Encoder == "") ) 
					{
						Encoder = sFieldData;
					} 
					else if ( (sFieldID == "TONES") && (xTones == "") ) 
					{
						xTones = sFieldData;
					} 
					else if ( (sFieldID == "STYLES") && (xStyles == "") ) 
					{
						xStyles = sFieldData;
					} 
					else if ( (sFieldID == "MOOD") && (xMood == "") ) 
					{
						xMood = sFieldData;
					} 
					else if ( (sFieldID == "SITUATION") && (xSituation == "") ) 
					{
						xSituation = sFieldData;
					} 
					else if ( (sFieldID == "RATING") && (xRating == "") ) 
					{
						xRating = sFieldData;
					} 
					else if ( (sFieldID == "QUALITY") && (xQuality == "") ) 
					{
						xQuality = sFieldData;
					} 
					else if ( (sFieldID == "TEMPO") && (xTempo == "") ) 
					{
						xTempo = sFieldData;
					} 
					else if ( (sFieldID == "TYPE") && (xType == "") ) 
					{
						xType = sFieldData;
					} 
					else if ( (sFieldID == "LYRICS") && (Lyrics == "") ) 
					{
						Lyrics = sFieldData;
					} 
					else if ( (sFieldID == "PERFORMER") && (Performer == "") ) 
					{
						Performer = sFieldData;
					} 
					else if ( (sFieldID == "LICENSE") && (License == "") ) 
					{
						License = sFieldData;
					} 
					else if ( (sFieldID == "ORGANIZATION") && (Organization == "") ) 
					{
						Organization = sFieldData;
					} 
					else if ( (sFieldID == "DESCRIPTION") && (Description == "") ) 
					{
						Description = sFieldData;
					} 
					else if ( (sFieldID == "LOCATION") && (Location == "") ) 
					{
						Location = sFieldData;
					} 
					else if ( (sFieldID == "CONTACT") && (Contact == "") ) 
					{
						Contact = sFieldData;
					} 
					else if ( (sFieldID == "ISRC") && (ISRC == "") ) 
					{
						ISRC = sFieldData;
					} 
					else 
					{ // more fields
						AddExtraField( sFieldID, sFieldData );
					}

				}

			}

		}

		/* -------------------------------------------------------------------------- */

		private void AddExtraField(String sID, String sValue)
		{  
			ArrayList newItem = new ArrayList();

			newItem.Add(sID);
			newItem.Add(sValue);
  
			aExtraFields.Add(newItem);
		}

		/* -------------------------------------------------------------------------- */

		private void _WriteTagBuff( String sID, String sData, BinaryWriter w, ref int iFieldCount )
		{
			String sTmp;
			int iTmp;

			if (sData != "")
			{
				sTmp = sID + "=" + EncodeUTF8( sData );
				iTmp = sTmp.Length;
				w.Write( iTmp );
				//# check here : PASCAL Strings aren't null-terminated -> different result ??
				w.Write( sTmp );
				iFieldCount = iFieldCount + 1;
			}
		}


		public bool SaveToFile( String sFile)
		{
			return SaveToFile( sFile, false );
		}

		public bool SaveToFile( String sFile, bool bBasicOnly)
		{
			int iFieldCount;
			int iSize;
  
			MemoryStream VorbisBlock = null;
			BinaryWriter vw = null;
			MemoryStream Tag = null;
			BinaryWriter tw = null;
  
			bool result = false;

			try
			{
				Tag = new MemoryStream();
				tw  = new BinaryWriter(Tag);
				VorbisBlock = new MemoryStream();
				vw  = new BinaryWriter(VorbisBlock);
    
				if ( GetInfo( sFile, false ) ) //reload all except tag fields
				{
					iFieldCount = 0;

					_WriteTagBuff( "TRACKNUMBER", FTrackString, tw, ref iFieldCount );
					_WriteTagBuff( "ARTIST", FArtist, tw , ref iFieldCount);
					_WriteTagBuff( "ALBUM", FAlbum, tw, ref iFieldCount );
					_WriteTagBuff( "TITLE", FTitle, tw, ref iFieldCount );
					_WriteTagBuff( "DATE", FYear, tw, ref iFieldCount );
					_WriteTagBuff( "GENRE", FGenre, tw, ref iFieldCount );
					_WriteTagBuff( "COMMENT", FComment, tw, ref iFieldCount );
					_WriteTagBuff( "COMPOSER", Composer, tw, ref iFieldCount );
					_WriteTagBuff( "LANGUAGE", Language, tw, ref iFieldCount );
					_WriteTagBuff( "COPYRIGHT", Copyright, tw, ref iFieldCount );
					_WriteTagBuff( "URL", Link, tw, ref iFieldCount );
					_WriteTagBuff( "ENCODER", Encoder, tw, ref iFieldCount );

					_WriteTagBuff( "TONES", xTones, tw, ref iFieldCount );
					_WriteTagBuff( "STYLES", xStyles, tw, ref iFieldCount );
					_WriteTagBuff( "MOOD", xMood, tw, ref iFieldCount );
					_WriteTagBuff( "SITUATION", xSituation, tw, ref iFieldCount );
					_WriteTagBuff( "RATING", xRating, tw, ref iFieldCount );
					_WriteTagBuff( "QUALITY", xQuality, tw, ref iFieldCount );
					_WriteTagBuff( "TEMPO", xTempo, tw, ref iFieldCount );
					_WriteTagBuff( "TYPE", xType, tw, ref iFieldCount );

					if (! bBasicOnly)  
					{
						_WriteTagBuff( "PERFORMER", Performer, tw, ref iFieldCount );
						_WriteTagBuff( "LICENSE", License, tw, ref iFieldCount );
						_WriteTagBuff( "ORGANIZATION", Organization, tw, ref iFieldCount );
						_WriteTagBuff( "DESCRIPTION", Description, tw, ref iFieldCount );
						_WriteTagBuff( "LOCATION", Location, tw, ref iFieldCount );
						_WriteTagBuff( "CONTACT", Contact, tw, ref iFieldCount );
						_WriteTagBuff( "ISRC", ISRC, tw, ref iFieldCount );
						_WriteTagBuff( "LYRICS", Lyrics, tw, ref iFieldCount );

						for ( int i=0; i< aExtraFields.Count-1; i++) 
						{
							if ( ((String)((ArrayList)aExtraFields[i])[0]).Trim() != "")  _WriteTagBuff( (String)((ArrayList)aExtraFields[i])[0], (String)((ArrayList)aExtraFields[i])[1], tw, ref iFieldCount );
						}
					}

					// Write vendor info and number of fields	    
					if ("" == FVendor)  FVendor = "reference libFLAC 1.1.0 20030126"; // guess it
					iSize = FVendor.Length;
					vw.Write( iSize );
					vw.Write( FVendor );
					vw.Write( iFieldCount );	    
					
					Utils.CopyMemoryStreamFrom(VorbisBlock,Tag,0); // All tag data is here now
	    
					VorbisBlock.Position = 0;

					result = RebuildFile( sFile, VorbisBlock );
					FExists = (result && (Tag.Length > 0 ));
				}
			} 
			catch(Exception e) 
			{
				System.Console.WriteLine(e.StackTrace);
				LogDelegator.GetLogDelegate()(Log.LV_ERROR, e.Message);
				result = false;
			}
  
			if (vw != null) vw.Close();
			if (tw != null) tw.Close();
  
			return result;
		}

		/* -------------------------------------------------------------------------- */

		private bool RemoveFromFile( String sFile)
		{
			bool result;

			FResetData( false, true );
			result = SaveToFile( sFile );
			if ( FExists ) FExists = ! result;
			return result;
		}

		/* -------------------------------------------------------------------------- */
		// saves metablocks back to the file
		// always tries to rebuild header so padding exists after comment block and no more than 1 padding block exists
		private bool RebuildFile( String sFile, MemoryStream VorbisBlock)
		{  
			FileStream source = null;
			BinaryReader r = null;
			FileStream destination = null;
			BinaryWriter w = null;

			long iFileAge;
			int iNewPadding;
			int iMetaCount;
			int iExtraPadding;
			String sTmp;
			String BufferName = "";
			byte[] MetaDataBlockHeader = new byte[4];
			TFlacHeader oldHeader = new TFlacHeader();
			MemoryStream MetaBlocks = null;
			bool bRebuild;
			bool bRearrange;
			bool result = false;
  
			bRearrange = false;
			iExtraPadding = 0;
			if ( !File.Exists(sFile) ) return result;

			try
			{
				iFileAge = 0;
				if ( bTAG_PreserveDate ) iFileAge = File.GetLastWriteTime(sFile).Ticks;

				// re arrange other metadata in case of
				// 1. padding block is not aligned after vorbis comment
				// 2. insufficient padding - rearange upon file rebuild
				// 3. fragmented padding blocks
				iMetaCount = aMetaBlockOther.Count;
				if ( (FPaddingIndex != FVorbisIndex + 1) || (FPadding <= VorbisBlock.Length - FTagSize ) || FPaddingFragments ) 
				{
					MetaBlocks = new MemoryStream();
					for (int i = 0; i < iMetaCount; i++)
					{
						((TMetaData)aMetaBlockOther[ i ]).MetaDataBlockHeader[ 0 ] = (byte)( ((TMetaData)aMetaBlockOther[i]).MetaDataBlockHeader[ 0 ] & 0x7f ); // not last

						if (META_PADDING == ((TMetaData)aMetaBlockOther[ i ]).MetaDataBlockHeader[ 0 ])  
						{
							iExtraPadding += (int)( ((TMetaData)aMetaBlockOther[ i ]).Data.Length + 4 ); // add padding size plus 4 bytes of header block
						} 
						else 
						{
							((TMetaData)aMetaBlockOther[ i ]).Data.Position = 0;
							MetaBlocks.Write( ((TMetaData)aMetaBlockOther[ i ]).MetaDataBlockHeader,0,4 );

							Utils.CopyMemoryStreamFrom(MetaBlocks,((TMetaData)aMetaBlockOther[ i ]).Data, 0);
						}

					}
					MetaBlocks.Position = 0;
					bRearrange = true;
				}

				// set up file
				if (FPadding <= VorbisBlock.Length - FTagSize )  // no room rebuild the file from scratch
				{ 
					bRebuild = true;
					BufferName = sFile + "~";
					source = new FileStream( sFile, FileMode.Open, FileAccess.Read  ); // Set read-only and open old file, and create new
					r = new BinaryReader(source);
					destination = new FileStream( BufferName, FileMode.Create, FileAccess.Write);
					w = new BinaryWriter(destination);
		
					oldHeader.StreamMarker = r.ReadChars(4);
					oldHeader.MetaDataBlockHeader = r.ReadBytes(4);
					oldHeader.Info = r.ReadBytes(18);
					oldHeader.MD5Sum = r.ReadBytes(16);		
					oldHeader.MetaDataBlockHeader[0] = (byte)(oldHeader.MetaDataBlockHeader[0] & 0x7f ); //just in case no metadata existed

					//w.Write( oldHeader, 0, 32 );
					w.Write(oldHeader.StreamMarker);
					w.Write(oldHeader.MetaDataBlockHeader);
					w.Write(oldHeader.Info);
					w.Write(oldHeader.MD5Sum);

					Utils.CopyMemoryStreamFrom(w, MetaBlocks, 0);
				}
				else 
				{
					bRebuild = false;
					source = null;
					destination = new FileStream ( sFile, FileMode.Create, FileAccess.Write); // Set write-access and open file
					w = new BinaryWriter(destination);
					if (bRearrange)  
					{
						destination.Seek( 32, SeekOrigin.Begin );
						Utils.CopyMemoryStreamFrom(destination, MetaBlocks, 0);
					} 
					else 
					{
						destination.Seek( FVCOffset - 4, SeekOrigin.Begin);		  
					}
				}

				// finally write vorbis block
				MetaDataBlockHeader[0] = META_VORBIS_COMMENT;
				MetaDataBlockHeader[1] = (byte)(( VorbisBlock.Length >> 16 ) & 255 );
				MetaDataBlockHeader[2] = (byte)(( VorbisBlock.Length >> 8 ) & 255 );
				MetaDataBlockHeader[3] = (byte)( VorbisBlock.Length & 255 );
				destination.Write( MetaDataBlockHeader,0, 4 );
				Utils.CopyMemoryStreamFrom(destination,VorbisBlock,VorbisBlock.Length);

				// and add padding
				if (FPaddingLast || bRearrange)  
				{
					MetaDataBlockHeader[0] = META_PADDING | 0x80;
				} 
				else 
				{
					MetaDataBlockHeader[0] = META_PADDING;
				}
				if (bRebuild)  
				{
					iNewPadding = 4096; // why not...
				} 
				else 
				{
					if (FTagSize > VorbisBlock.Length )
					{ // tag got smaller increase padding
						iNewPadding = (int)(FPadding + FTagSize - VorbisBlock.Length) + iExtraPadding;
					} 
					else 
					{ // tag got bigger shrink padding
						iNewPadding = (int)(FPadding - VorbisBlock.Length + FTagSize ) + iExtraPadding;
					}
				}
				MetaDataBlockHeader[1] = (byte)(( iNewPadding >> 16 ) & 255 );
				MetaDataBlockHeader[2] = (byte)(( iNewPadding >> 8 ) & 255 );
				MetaDataBlockHeader[3] = (byte)( iNewPadding & 255 );
				w.Write(MetaDataBlockHeader,0,4);

				if ((FPadding != iNewPadding) | bRearrange )
				{ // fill the block with zeros
					sTmp = "";
					for (int i=0; i<iNewPadding; i++)
						sTmp += '\0';					
					w.Write( sTmp );
				}

				// finish
				if ( bRebuild )
				{ // time to put back the audio data...
					source.Seek( FAudioOffset, SeekOrigin.Begin );
					Utils.CopyMemoryStreamFrom(destination, source, source.Length - FAudioOffset);

					source.Close();
					destination.Close();

					//Replace old file and delete temporary file
					File.Delete( sFile );
					File.Copy( BufferName, sFile );
					result = true;
				} 
				else 
				{
					result = true;
					destination.Close();
				}

				// post save tasks
				DateTime newDateTime = new DateTime(iFileAge);
				if (bTAG_PreserveDate) File.SetLastWriteTime( sFile, newDateTime );
				if (bRearrange) MetaBlocks.Close();
			}
			catch (Exception e)
			{
				// Access error
				if ( File.Exists( BufferName ) )  File.Delete( BufferName );
				LogDelegator.GetLogDelegate()(Log.LV_ERROR,e.Message);
				System.Console.WriteLine(e.StackTrace);
			}

			// stream deallocation
			if (destination != null) destination.Close();
			if (source != null) source.Close();	

			return result;
		}

		/* -------------------------------------------------------------------------- */

		// Convert UTF-8 to Unicode
		private String DecodeUTF8(String Source)
		{
			//# COPY SAME FROM ID3vxx -?-
			return Source;
			/*
			var
			  Index, SourceLength, FChar, NChar: Cardinal;

			  // Convert UTF-8 to unicode
			  Result = "";
			  Index = 0;
			  SourceLength = Length(Source);
			  while Index < SourceLength do
			  {
				Inc(Index);
				FChar = Ord(Source[Index]);
				if FChar >== 0x80 
				{
				  Inc(Index);
				  if Index > SourceLength  exit;
				  FChar = FChar and 0x3F;
				  if (FChar and 0x20) <> 0 
				  {
					FChar = FChar and 0x1F;
					NChar = Ord(Source[Index]);
					if (NChar and 0xC0) <> 0x80   exit;
					FChar = (FChar shl 6) or (NChar and 0x3F);
					Inc(Index);
					if Index > SourceLength  exit;
				  }
				  NChar = Ord(Source[Index]);
				  if (NChar and 0xC0) <> 0x80  exit;
				  Result = Result + WideChar((FChar shl 6) or (NChar and 0x3F));
				end
				else
				  Result = Result + WideChar(FChar);
			  }
			  */
		}

		/* -------------------------------------------------------------------------- */

		private String EncodeUTF8(String Source)
		{
			byte FirstByte;
			byte SecondByte;
			char UnicodeChar;
			String result = Source;			

			if (null == Source) result = "";				
			// Convert string from unicode if needed and trim spaces
			if ( (Source != null) && (Source.Length > 0) && (TID3v2.UNICODE_ID == Source[0]) )
			{
				result = "";
				/*UnicodeEncoding uEnc = new UnicodeEncoding();
				result = uEnc.GetString(Encoding.Default.GetBytes(Source.Substring(1,Source.Length-1)));
				*/
		
				for (int index=0; (index * 2 + 2) < Source.Length; index++)
				{
					FirstByte = (byte)(Source[(index * 2) + 1]);
					SecondByte = (byte)(Source[(index * 2) + 2]);
					UnicodeChar = (char)(FirstByte | (SecondByte << 8));					
					if (0 == UnicodeChar) break;
					if (FirstByte < 0xFF) result = result + UnicodeChar;
				}
			}
			else
				if (result.Length >0)
				result = result.Substring(1,result.Length-1);

			return result.Trim();
			/*
			var
			  Index, SourceLength, CChar: Cardinal;

			  // Convert unicode to UTF-8
			  Result = "";
			  Index = 0;
			  SourceLength = Length(Source);
			  while Index < SourceLength do
			  {
				Inc(Index);
				CChar = Cardinal(Source[Index]);
				if CChar <== 0x7F 
				  Result = Result + Source[Index]
				else if CChar > 0x7FF 
				{
				  Result = Result + Char(0xE0 or (CChar shr 12));
				  Result = Result + Char(0x80 or ((CChar shr 6) and 0x3F));
				  Result = Result + Char(0x80 or (CChar and 0x3F));
				end
				else
				{
				  Result = Result + Char(0xC0 or (CChar shr 6));
				  Result = Result + Char(0x80 or (CChar and 0x3F));
				}
			  }
			  */
		}

		/* -------------------------------------------------------------------------- */

	}
}