// ***************************************************************************
//                                                                            
// Audio Tools Library (Freeware)                                             
// Class TSPCFile - for extracting information from SPC700 Files
//                                                                            
// Version 1.0 (1st May 2006) by Zeugma 440
//   -	Support for SPC, according to file format v0.30; inspired by the SNESamp source (ID666.cpp)
//
// ***************************************************************************
using System;
using System.IO;
using System.Collections;
using ATL.Logging;

namespace ATL.AudioReaders.BinaryLogic
{
	/// <summary>
	/// Description résumée de SPC.
	/// </summary>
	public class TSPCFile : AudioDataReader, MetaDataReader
	{
		private const String SPC_FORMAT_TAG = "SNES-SPC700 Sound File Data v0.30";
		private const String XTENDED_TAG = "xid6";

		private const int REGISTERS_LENGTH = 9;
		private const int AUDIODATA_LENGTH = 65792;
		private const int SPC_RAW_LENGTH = 66048;

		private const int HEADER_TEXT = 0;
		private const int HEADER_BINARY = 1;

		private const bool PREFER_BIN = false;

		private const int SPC_DEFAULT_DURATION = 180; // 3 minutes

		//Sub-chunk ID's
		private const byte XID6_SONG =	0x01;						//see ReadMe.Txt for format information
		private const byte XID6_GAME =	0x02;
		private const byte XID6_ARTIST =0x03;
		private const byte XID6_DUMPER =0x04;
		private const byte XID6_DATE =	0x05;
		private const byte XID6_EMU =	0x06;
		private const byte XID6_CMNTS =	0x07;
		private const byte XID6_INTRO =	0x30;
		private const byte XID6_LOOP =	0x31;
		private const byte XID6_END =	0x32;
		private const byte XID6_FADE =	0x33;
		private const byte XID6_MUTE =	0x34;
		private const byte XID6_LOOPX =	0x35;
		private const byte XID6_AMP =	0x36;
		private const byte XID6_OST =	0x10;
		private const byte XID6_DISC =	0x11;
		private const byte XID6_TRACK =	0x12;
		private const byte XID6_PUB =	0x13;
		private const byte XID6_COPY =	0x14;

		//Data types
		private const byte XID6_TVAL =	0x00;
		private const byte XID6_TSTR =	0x01;
		private const byte XID6_TINT =	0x04;

		//Timer stuff
		private const int XID6_MAXTICKS	 = 383999999;			//Max ticks possible for any field (99:59.99 * 64k)
		private const int XID6_TICKSMIN	 = 3840000;			  	//Number of ticks in a minute (60 * 64k)
		private const int XID6_TICKSSEC	 = 64000;			  	//Number of ticks in a second
		private const int XID6_TICKSMS	 = 64;			  		//Number of ticks in a millisecond
		private const int XID6_MAXLOOP	 = 9;				  	//Max loop times

		
		// Standard fields
		private bool FValid;
		private int FFileSize;
		private int FSampleRate;
		private double FDuration;
		private int FBitRate;
		private bool FTagExists;
		private String FTitle;
		private String FArtist;
		private String FAlbum;
		private int FTrack;
		private String FYear;
		private String FGenre;
		private String FComment;

		public TAPEtag APEtag // No APEtag here; declared for interface compliance
		{
			get{ return new TAPEtag(); }
		}
		public TID3v1 ID3v1 // No ID3v1 here; declared for interface compliance
		{
			get{ return new TID3v1(); }
		}
		public TID3v2 ID3v2 // No ID3v2 here; declared for interface compliance
		{
			get{ return new TID3v2(); }
		}
		public bool Valid // True if valid data
		{
			get { return this.FValid; }
		}
		public bool Exists // for compatibility with other tag readers
		{
			get { return FTagExists; }
		}
		public int FileSize // File size (bytes)
		{
			get { return this.FFileSize; }
		}	
		public int SampleRate // Sample rate (hz)
		{
			get { return this.FSampleRate; }
		}	
		public double Duration // Duration (seconds)
		{
			get { return this.FDuration; }
		}	
		public double BitRate // Bit rate (kbit)
		{
			get { return (double)this.FBitRate; }
		}
		public bool IsVBR
		{
			get { return false; }
		}
		public int CodecFamily
		{
			get { return AudioReaderFactory.CF_SEQ_WAV; }
		}
		public String Title // Song title
		{
			get { return this.FTitle; }
		}	
		public String Artist // Artist name
		{
			get { return this.FArtist; }
		}
		public String Album // Album name
		{
			get { return this.FAlbum; }
		}	
		public ushort Track // Track number
		{
			get { return (ushort)this.FTrack; }
		}	
		public String Year // Year
		{
			get { return this.FYear; }
		}	
		public String Genre // Genre name
		{
			get { return this.FGenre; }
		}	
		public String Comment // Comment
		{
			get { return this.FComment; }
		}

		// === PRIVATE STRUCTURES/SUBCLASSES ===

		private class SPCHeader
		{
			public const int TAG_IN_HEADER = 26;

			public String FormatTag;					// Format tag (should be SPC_FORMAT_TAG)
			public byte TagInHeader;					// Set to TAG_IN_HEADER if header contains ID666 info
			public byte VersionByte;					// Version mark

			public void Reset()
			{
				FormatTag = "";
				VersionByte = 0;
			}
		}

		private class ExtendedItem
		{
			public byte ID;
			public byte Type;
			public int Length;
			public object Data; // String or int32

			public void Reset()
			{
				ID = 0;
				Type = 0;
				Length = 0;
				Data = null;
			}
		}

		private class SPCExTags
		{
			public String FooterTag;					// Extended info tag (should be XTENDED_TAG)
			public uint FooterSize;						// Chunk size
			public Hashtable Items;						// List of ExtendedItems

			public void Reset()
			{
				FooterTag = "";
				FooterSize = 0;
				Items = new Hashtable();
			}
		}

		// === CONSTRUCTOR ===

		public TSPCFile()
		{
			// Create object
			FResetData();
		}

		// === PRIVATE METHODS ===

		private void FResetData()
		{
			// Reset variables
			FValid = false;
			FFileSize = 0;
			FSampleRate = 0;
			FDuration = SPC_DEFAULT_DURATION;
			FBitRate = 0;
			FTagExists = false;
			FTitle = "";
			FArtist = "";
			FAlbum = "";
			FTrack = 0;
			FYear = "";
			FGenre = "";
			FComment = "";
		}

		private bool readHeader(ref BinaryReader source, ref SPCHeader header)
		{
			header.FormatTag = Utils.GetStringFromCharArray( Utils.ReadTrueChars(source, 33) );
			if (SPC_FORMAT_TAG == header.FormatTag)
			{
				source.BaseStream.Seek(2,SeekOrigin.Current);
				header.TagInHeader = source.ReadByte();
				header.VersionByte = source.ReadByte();
				return true;
			}
			else
			{
				return false;
			}
		}

		private void readHeaderTags(ref BinaryReader source)
		{
			FTitle = Utils.GetStringFromCharArray(Utils.ReadTrueChars(source,32));
			FAlbum = Utils.GetStringFromCharArray(Utils.ReadTrueChars(source,32));
			source.BaseStream.Seek(16,SeekOrigin.Current); // Dumper name
			FComment = Utils.GetStringFromCharArray(Utils.ReadTrueChars(source,32));

			char[] date;
			char[] song;
			char[] fade;
			
			// NB : Dump date is used to determine if the tag is binary or text-based.
			// It won't be recorded as a property of TSPC
			date = Utils.ReadTrueChars(source,11);
			song = Utils.ReadTrueChars(source,3);
			fade = Utils.ReadTrueChars(source,5);
			
			bool bin;
			int dateRes = isText(date);
			int songRes = isText(song);
			int fadeRes = isText(fade);

			//if ( 0 == (dateRes | songRes | fadeRes) ) // No time nor date -> use default
			//{
				bin = true;
			//}
			//else
			if ((songRes != -1) && (fadeRes != -1)) // No time, or time is text
			{
				if (dateRes > 0)					//If date is text, then tag is text
				{
					bin = false;
				}
				else
					if (0 == dateRes)					//No date
				{
					bin = PREFER_BIN;				//Times could still be binary (ex. 56 bin = '8' txt)
				}
				else
					if (-1 == dateRes)					//Date contains invalid characters
				{
					bin = true;
					for (int i=4; i<8; i++)
					{
						bin = bin & (0 == (byte)date[i]);
					}
				}
			}
			else
			{
				bin = true;
			}

			int fadeVal;
			int songVal;

			if (bin)
			{
				fadeVal = (byte)fade[0]*0x0001 + 
					(byte)fade[1]*0x0010 + 
					(byte)fade[2]*0x0100 + 
					(byte)fade[3]*0x1000;
				if (fadeVal > 59999) fadeVal = 59999;

				songVal = (byte)song[0]*0x01 +
					(byte)song[1]*0x10;
				if (songVal > 959) songVal = 959;

				source.BaseStream.Seek(-1,SeekOrigin.Current); // We're one byte ahead
			}
			else
			{
				fadeVal = Int32.Parse( Utils.GetStringFromCharArray( fade ) );
				songVal = Int32.Parse( Utils.GetStringFromCharArray( song ) );
			}
			if (fadeVal + songVal > 0)
				FDuration = Math.Round((double)fadeVal/1000) + songVal;
			
			FArtist = Utils.GetStringFromCharArray(Utils.ReadTrueChars(source,32));
		}

		private int isText(char[] str)
		{
			int c = 0;

			while (c<str.Length && (((byte)str[c]>=0x30 && str[c]<=0x39) || '/'==str[c])) c++;
			if (c==str.Length || str[c]==0)
				return c;
			else
				return -1;
		}

		private void readExtendedData(ref BinaryReader source, ref SPCExTags footer)
		{
			footer.FooterTag = Utils.GetStringFromCharArray(Utils.ReadTrueChars(source,4));
			if (XTENDED_TAG == footer.FooterTag)
			{
				footer.FooterSize = source.ReadUInt32();
				
				ExtendedItem anItem;

				long i=source.BaseStream.Position;
				while(source.BaseStream.Position-i < footer.FooterSize)
				{
					anItem = new ExtendedItem();
					anItem.ID = source.ReadByte();
					anItem.Type = source.ReadByte();
					anItem.Length = source.ReadUInt16();

					switch(anItem.Type)
					{
						case XID6_TVAL :
							// nothing; value is stored into the Length field
							break;
						case XID6_TSTR :
							anItem.Data = Utils.GetStringFromCharArray(Utils.ReadTrueChars(source,anItem.Length));
							while(0 == source.ReadByte()); // Ending zeroes
							source.BaseStream.Seek(-1,SeekOrigin.Current);
							break;
						case XID6_TINT :
							anItem.Data = source.ReadInt32();
							break;
					}
					footer.Items.Add( anItem.ID, anItem );
				}
			}
		}

		// === PUBLIC METHODS ===

		public bool ReadFromFile(String fileName)
		{
			bool result = true;
			SPCHeader header = new SPCHeader();
			SPCExTags footer = new SPCExTags();

			FResetData();
			
			header.Reset();
			footer.Reset();

			FileStream fs = null;
			BinaryReader source = null;

			try
			{
				fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
				fs.Lock(0,fs.Length);
				source = new BinaryReader(fs);

				if ( !readHeader(ref source, ref header) ) throw new Exception("Not a PSF file");

				// Reads the header tag
				if (SPCHeader.TAG_IN_HEADER == header.TagInHeader)
				{
					source.BaseStream.Seek(REGISTERS_LENGTH,SeekOrigin.Current);
					readHeaderTags(ref source);
				}

				// Reads extended tag
				if (fs.Length > SPC_RAW_LENGTH)
				{
					source.BaseStream.Seek(SPC_RAW_LENGTH,SeekOrigin.Begin);
					readExtendedData(ref source, ref footer);
					
					if (footer.Items.ContainsKey(XID6_ARTIST)) FArtist = (String)((ExtendedItem)footer.Items[XID6_ARTIST]).Data;
					if (footer.Items.ContainsKey(XID6_CMNTS)) FComment = (String)((ExtendedItem)footer.Items[XID6_CMNTS]).Data;
					if (footer.Items.ContainsKey(XID6_SONG)) FTitle = (String)((ExtendedItem)footer.Items[XID6_SONG]).Data;
					if (footer.Items.ContainsKey(XID6_COPY)) FYear = ((ExtendedItem)footer.Items[XID6_COPY]).Length.ToString();
					if (footer.Items.ContainsKey(XID6_TRACK)) FTrack = ((ExtendedItem)footer.Items[XID6_TRACK]).Length >> 8;
					if (footer.Items.ContainsKey(XID6_OST)) FAlbum = (String)((ExtendedItem)footer.Items[XID6_OST]).Data;
					if (("" == FAlbum) && (footer.Items.ContainsKey(XID6_GAME))) FAlbum = (String)((ExtendedItem)footer.Items[XID6_GAME]).Data;
					
					long ticks = 0;
					if (footer.Items.ContainsKey(XID6_LOOP)) ticks += Math.Min(XID6_MAXTICKS, (int)((ExtendedItem)footer.Items[XID6_LOOP]).Data);
					if (footer.Items.ContainsKey(XID6_LOOPX)) ticks = ticks * Math.Min(XID6_MAXLOOP, (int)((ExtendedItem)footer.Items[XID6_LOOPX]).Length );
					if (footer.Items.ContainsKey(XID6_INTRO)) ticks += Math.Min(XID6_MAXTICKS, (int)((ExtendedItem)footer.Items[XID6_INTRO]).Data);
					if (footer.Items.ContainsKey(XID6_END)) ticks += Math.Min(XID6_MAXTICKS, (int)((ExtendedItem)footer.Items[XID6_END]).Data);
					
					if (ticks > 0)
						FDuration = Math.Round( (double)ticks / XID6_TICKSSEC );
				}
			}
			catch (Exception e) 
			{
				System.Console.WriteLine(e.StackTrace);
				LogDelegator.GetLogDelegate()(Log.LV_ERROR, e.Message);
				result = false;
			}

			if (fs != null)
			{
				fs.Unlock(0,fs.Length);
				fs.Close();
			}

			return result;
		}
	}

}