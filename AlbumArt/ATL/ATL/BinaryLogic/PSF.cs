// ***************************************************************************
//                                                                            
// Audio Tools Library (Freeware)                                             
// Class TPSFFile - for extracting information from PSF header and tag
//                                                                            
// Version 1.0 (22nd April 2006) by Zeugma 440
//   -	Support for PSF, according to Neil Corlett's specifications v. 1.6
//
// ***************************************************************************
using System;
using System.IO;
using System.Collections;
using ATL.Logging;

namespace ATL.AudioReaders.BinaryLogic
{
	/// <summary>
	/// Description résumée de PSF.
	/// </summary>
	public class TPSFFile : AudioDataReader, MetaDataReader
	{
		// Format Type Names
		public const String PSF_FORMAT_UNKNOWN = "Unknown";
		public const String PSF_FORMAT_PSF1 = "Playstation";
		public const String PSF_FORMAT_PSF2 = "Playstation 2";
		public const String PSF_FORMAT_SSF = "Saturn";
		public const String PSF_FORMAT_DSF = "Dreamcast";
		public const String PSF_FORMAT_USF = "Nintendo 64";
		public const String PSF_FORMAT_QSF = "Capcom QSound";

		// Tag predefined fields
		public const String TAG_TITLE = "title";
		public const String TAG_ARTIST = "artist";
		public const String TAG_GAME = "game";
		public const String TAG_YEAR = "year";
		public const String TAG_GENRE = "genre";
		public const String TAG_COMMENT = "comment";
		public const String TAG_COPYRIGHT = "copyright";
		public const String TAG_LENGTH = "length";
		public const String TAG_FADE = "fade";

		private const String PSF_FORMAT_TAG = "PSF";
		private const String TAG_HEADER = "[TAG]";
		private const uint HEADER_LENGTH = 16;

		private const int PSF_DEFAULT_DURATION = 180; // 3 minutes

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

		private class PSFHeader
		{
			public String FormatTag;					// Format tag (should be PSF_FORMAT_TAG)
			public byte VersionByte;					// Version mark
			public uint ReservedAreaLength;				// Length of reserved area (bytes)
			public uint CompressedProgramLength;		// Length of compressed program (bytes)

			public void Reset()
			{
				FormatTag = "";
				VersionByte = 0;
				ReservedAreaLength = 0;
				CompressedProgramLength = 0;
			}
		}

		private class PSFTag
		{
			public String TagHeader;					// Tag header (should be TAG_HEADER)
			public Hashtable tags;						// Tags

			public void Reset()
			{
				TagHeader = "";
				tags = new Hashtable();
			}
		}

		// === CONSTRUCTOR ===

		public TPSFFile()
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
			FDuration = PSF_DEFAULT_DURATION;
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

		private bool readHeader(ref BinaryReader source, ref PSFHeader header)
		{
			header.FormatTag = Utils.GetStringFromCharArray( Utils.ReadTrueChars(source, 3) );
			if (PSF_FORMAT_TAG == header.FormatTag)
			{
				header.VersionByte = source.ReadByte();
				header.ReservedAreaLength = source.ReadUInt32();
				header.CompressedProgramLength = source.ReadUInt32();
				return true;
			}
			else
			{
				return false;
			}
		}

		private String readPSFLine(ref BinaryReader source)
		{
			String result = "";
			char c = '\0';
			
			//char c = Utils.ReadTrueChar(source);
			if (source.BaseStream.Position < source.BaseStream.Length)
				c = source.ReadChar(); // Should be okay for UTF-8 support

			while( ((byte)c != 0x0A) && (source.BaseStream.Position < source.BaseStream.Length) )
			{
				if ((byte)c < 0x020) c = ' ';
				result += c;
				c = source.ReadChar();
			}

			return result.Trim();
		}

		private bool readTag(ref BinaryReader source, ref PSFTag tag)
		{
			tag.TagHeader = Utils.GetStringFromCharArray( Utils.ReadTrueChars(source, 5) );
			if (TAG_HEADER == tag.TagHeader)
			{
				FTagExists = true;
				String s = readPSFLine(ref source);
				
				int equalIndex;
				String keyStr;
				String valueStr;

				while ( s != "" )
				{
					equalIndex = s.IndexOf("=");
					if (equalIndex != -1)
					{
						keyStr = s.Substring(0,equalIndex).Trim().ToLower();
						valueStr = s.Substring(equalIndex+1, s.Length-(equalIndex+1)).Trim();

						if (tag.tags.ContainsKey(keyStr))
						{
							tag.tags[keyStr] = tag.tags[keyStr] + " " + valueStr;
						}
						else
						{
							tag.tags[keyStr] = valueStr;
						}
					}

					s = readPSFLine(ref source);
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		private double parsePSFDuration(String durationStr)
		{
			String hStr = "";
			String mStr = "";
			String sStr = "";
			String dStr = "";
			double result = 0;

			int sepIndex;

			// decimal
			sepIndex = durationStr.LastIndexOf(".");
			if (-1 == sepIndex) sepIndex = durationStr.LastIndexOf(",");

			if (-1 != sepIndex)
			{
				sepIndex++;
				dStr = durationStr.Substring(sepIndex,durationStr.Length-sepIndex);
				durationStr = durationStr.Substring(0,sepIndex);
			}

			// seconds
			sepIndex = durationStr.LastIndexOf(":");
			
			sepIndex++;
			sStr = durationStr.Substring(sepIndex,durationStr.Length-sepIndex);

			durationStr = durationStr.Substring(0,Math.Max(0,sepIndex-1));

			// minutes
			if (durationStr.Length > 0)
			{
				sepIndex = durationStr.LastIndexOf(":");
				
				sepIndex++;
				mStr = durationStr.Substring(sepIndex,durationStr.Length-sepIndex);

				durationStr = durationStr.Substring(0,Math.Max(0,sepIndex-1));
			}

			// hours
			if (durationStr.Length > 0)
			{
				sepIndex = durationStr.LastIndexOf(":");
				
				sepIndex++;
				hStr = durationStr.Substring(sepIndex,durationStr.Length-sepIndex);
			}

			if (dStr != "") result = result + (Int32.Parse(dStr) / 10);
			if (sStr != "") result = result + (Int32.Parse(sStr));
			if (mStr != "") result = result + (Int32.Parse(mStr)*60);
			if (hStr != "") result = result + (Int32.Parse(hStr)*3600);
			
			return result;
		}

		// === PUBLIC METHODS ===

		public bool ReadFromFile(String fileName)
		{
			bool result = true;
			PSFHeader header = new PSFHeader();
			PSFTag tag = new PSFTag();

			FResetData();
			
			header.Reset();
			tag.Reset();

			FileStream fs = null;
			BinaryReader source = null;

			try
			{
				fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
				fs.Lock(0,fs.Length);
				source = new BinaryReader(fs);

				if ( !readHeader(ref source, ref header) ) throw new Exception("Not a PSF file");

				if (fs.Length > HEADER_LENGTH+header.CompressedProgramLength+header.ReservedAreaLength)
				{
					fs.Seek((long)(4+header.CompressedProgramLength+header.ReservedAreaLength),SeekOrigin.Current);
				
					if ( !readTag(ref source,ref tag) ) throw new Exception("Not a PSF tag");
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

			if (tag.tags.ContainsKey(TAG_GAME)) FAlbum = (String)tag.tags[TAG_GAME];
			if (tag.tags.ContainsKey(TAG_ARTIST)) FArtist = (String)tag.tags[TAG_ARTIST];
			if (tag.tags.ContainsKey(TAG_COMMENT)) FComment = (String)tag.tags[TAG_COMMENT];
			if (tag.tags.ContainsKey(TAG_TITLE)) FTitle = (String)tag.tags[TAG_TITLE];
			if (tag.tags.ContainsKey(TAG_YEAR)) FYear = (String)tag.tags[TAG_YEAR];
			if (tag.tags.ContainsKey(TAG_LENGTH)) 
			{
				FDuration = parsePSFDuration( (String)tag.tags[TAG_LENGTH] );				
			}
			if (tag.tags.ContainsKey(TAG_FADE)) 
			{
				FDuration += parsePSFDuration( (String)tag.tags[TAG_FADE] );				
			}

			return result;
		}
	}
}
