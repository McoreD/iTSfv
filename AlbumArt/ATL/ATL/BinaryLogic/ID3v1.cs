/* **************************************************************************
//                                                                            
// Audio Tools Library (Freeware)                                             
// Class TID3v1 - for manipulating with ID3v1 tags                            
//                                                                            
// Copyright (c) 2001,2002 by Jurgen Faul                                     
// E-mail: jfaul@gmx.de                                                       
// http://jfaul.de/atl                                                        
//                                      
// 21st January 2006 - Zeugma 440 - version 1.1 
//	 - Fixed an issue about reading null-terminated strings. Values after the '\0' char
//	 were previously read. Now the string stops at the first '\0'
// 8th May 2005 - Translated to C# by Zeugma 440
// Version 1.0 (25 July 2001)                                                 
//   - Reading & writing support for ID3v1.x tags                             
//   - Tag info: title, artist, album, track, year, genre, comment            
//                                                                            
// ************************************************************************** */


using System;
using System.IO;

/*type
  // Used in TID3v1 class
  String04 = string[4];                          // String with max. 4 symbols
  String30 = string[30];                        // String with max. 30 symbols*/
  
namespace ATL.AudioReaders.BinaryLogic
{
	public class TID3v1 : MetaDataReader
	{
		public const int MAX_MUSIC_GENRES = 148;        // Max. number of music genres
		public const int DEFAULT_GENRE = 255;               // Index for default genre

		// Used with VersionID property
		public const byte TAG_VERSION_1_0 = 1;                // Index for ID3v1.0 tag
		public const byte TAG_VERSION_1_1 = 2;                // Index for ID3v1.1 tag

		#region music genres
		public static String[] MusicGenre = new string[MAX_MUSIC_GENRES] 		// Genre names
		{	// Standard genres
			"Blues",
			"Classic Rock",
			"Country",
			"Dance",
			"Disco",
			"Funk",
			"Grunge",
			"Hip-Hop",
			"Jazz",
			"Metal",
			"New Age",
			"Oldies",
			"Other",
			"Pop",
			"R&B",
			"Rap",
			"Reggae",
			"Rock",
			"Techno",
			"Industrial",
			"Alternative",
			"Ska",
			"Death Metal",
			"Pranks",
			"Soundtrack",
			"Euro-Techno",
			"Ambient",
			"Trip-Hop",
			"Vocal",
			"Jazz+Funk",
			"Fusion",
			"Trance",
			"Classical",
			"Instrumental",
			"Acid",
			"House",
			"Game",
			"Sound Clip",
			"Gospel",
			"Noise",
			"AlternRock",
			"Bass",
			"Soul",
			"Punk",
			"Space",
			"Meditative",
			"Instrumental Pop",
			"Instrumental Rock",
			"Ethnic",
			"Gothic",
			"Darkwave",
			"Techno-Industrial",
			"Electronic",
			"Pop-Folk",
			"Eurodance",
			"Dream",
			"Southern Rock",
			"Comedy",
			"Cult",
			"Gangsta",
			"Top 40",
			"Christian Rap",
			"Pop/Funk",
			"Jungle",
			"Native American",
			"Cabaret",
			"New Wave",
			"Psychadelic",
			"Rave",
			"Showtunes",
			"Trailer",
			"Lo-Fi",
			"Tribal",
			"Acid Punk",
			"Acid Jazz",
			"Polka",
			"Retro",
			"Musical",
			"Rock & Roll",
			"Hard Rock",
			// Extended genres
			"Folk",
			"Folk-Rock",
			"National Folk",
			"Swing",
			"Fast Fusion",
			"Bebob",
			"Latin",
			"Revival",
			"Celtic",
			"Bluegrass",
			"Avantgarde",
			"Gothic Rock",
			"Progessive Rock",
			"Psychedelic Rock",
			"Symphonic Rock",
			"Slow Rock",
			"Big Band",
			"Chorus",
			"Easy Listening",
			"Acoustic",
			"Humour",
			"Speech",
			"Chanson",
			"Opera",
			"Chamber Music",
			"Sonata",
			"Symphony",
			"Booty Bass",
			"Primus",
			"Porn Groove",
			"Satire",
			"Slow Jam",
			"Club",
			"Tango",
			"Samba",
			"Folklore",
			"Ballad",
			"Power Ballad",
			"Rhythmic Soul",
			"Freestyle",
			"Duet",
			"Punk Rock",
			"Drum Solo",
			"A capella",
			"Euro-House",
			"Dance Hall",
			"Goa",
			"Drum & Bass",
			"Club-House",
			"Hardcore",
			"Terror",
			"Indie",
			"BritPop",
			"Negerpunk",
			"Polsk Punk",
			"Beat",
			"Christian Gangsta Rap",
			"Heavy Metal",
			"Black Metal",
			"Crossover",
			"Contemporary Christian",
			"Christian Rock",
			"Merengue",
			"Salsa",
			"Trash Metal",
			"Anime",
			"JPop",
			"Synthpop"
		};
		#endregion
	
		private bool FExists;
		private byte FVersionID;
		private String FTitle; // String30
		private String FArtist; // String30
		private String FAlbum; // String30
		private String FYear; // String04
		private String FComment; // String30
		private byte FTrack;
		private byte FGenreID;
			
		public bool Exists // True if tag found
		{
			get { return this.FExists; }
		}
		public byte VersionID // Version code
		{
			get { return this.FVersionID; }
		}
		public String Title // Song title (String30)
		{
			get { return this.FTitle; }
			set { FSetTitle(value); }
		}
		public String Artist // Artist name (String30)
		{
			get { return this.FArtist; }
			set { FSetArtist(value); }
		}
		public String Album // Album name (String30)
		{
			get { return this.FAlbum; }
			set { FSetAlbum(value); }
		}	
		public String Year // Year (String04)
		{
			get { return this.FYear; }
			set { FSetYear(value); }
		}	
		public String Comment // Comment (String30)
		{
			get { return this.FComment; }
			set { FSetComment(value); }
		}	
		public ushort Track // Track number
		{
			get { return (ushort)this.FTrack; }
			set { FSetTrack((byte)value); }
		}
		public byte GenreID // Genre code
		{
			get { return this.FGenreID; }
			set { FSetGenreID(value); }
		}
		public String Genre // Genre name
		{
			get { return FGetGenre(); }
		}		

		// Real structure of ID3v1 tag
		private class TagRecord
		{
			public char[] Header = new char[3];                // Tag header - must be "TAG"
			public char[] Title = new char[30];                                // Title data
			public char[] Artist = new char[30];                              // Artist data
			public char[] Album = new char[30];                                // Album data
			public char[] Year = new char[4];                                   // Year data
			public char[] Comment = new char[30];                            // Comment data
			public byte Genre;                                                 // Genre data

			public void Reset()
			{
				Array.Clear(Header,0,Header.Length);
				Array.Clear(Title,0,Title.Length);
				Array.Clear(Artist,0,Artist.Length);
				Array.Clear(Album,0,Album.Length);
				Array.Clear(Year,0,Year.Length);
				Array.Clear(Comment,0,Comment.Length);
				Genre = 0;
			}
		}

		// ********************* Auxiliary functions & voids ********************

		bool ReadTag(String FileName, ref TagRecord TagData)
		{
			bool result;
			FileStream fs = null;
			BinaryReader SourceFile = null;


			try
			{
				result = true;
				// Set read-access and open file
				fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
				SourceFile = new BinaryReader(fs);
			
				// Read tag
				fs.Seek(-128, SeekOrigin.End);
			
				// ID3v1 tags are C-String(null-terminated)-based tags
				// they are not unicode-encoded, hence the use of ReadTrueChars
				TagData.Header = Utils.ReadTrueChars(SourceFile,3);
				TagData.Title = Utils.ReadTrueChars(SourceFile,30);
				TagData.Artist = Utils.ReadTrueChars(SourceFile,30);
				TagData.Album = Utils.ReadTrueChars(SourceFile,30);
				TagData.Year = Utils.ReadTrueChars(SourceFile,4);
				TagData.Comment = Utils.ReadTrueChars(SourceFile,30);
				TagData.Genre = SourceFile.ReadByte();
			} 
			catch (Exception e)
			{
				System.Console.WriteLine(e.Message);
				System.Console.WriteLine(e.StackTrace);
				result = false;
			}

			if (SourceFile != null) SourceFile.Close();
			if (fs != null) fs.Close();

			return result;
		}

		// ---------------------------------------------------------------------------

		bool RemoveTag(String FileName)
		{
			bool result = true;		
		
			try
			{				
				// Allow write-access and open file
				FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Write);
				// Delete tag
				fs.SetLength(fs.Length - 128);
				fs.Close();
			} 
			catch (Exception e)
			{
				System.Console.WriteLine(e.StackTrace);
				result = false;
			}
			return result;
		}

		// ---------------------------------------------------------------------------

		bool SaveTag(String FileName, TagRecord TagData)
		{	
			bool result = true;

			try
			{		
				// Allow write-access and open file
				FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Write);
				BinaryWriter TargetFile = new BinaryWriter(fs);

				// Write tag
				fs.Seek(fs.Length, SeekOrigin.Begin);
			
				TargetFile.Write(TagData.Header);
				TargetFile.Write(TagData.Title);
				TargetFile.Write(TagData.Artist);
				TargetFile.Write(TagData.Album);
				TargetFile.Write(TagData.Year);
				TargetFile.Write(TagData.Comment);
				TargetFile.Write(TagData.Genre);			
			
				TargetFile.Close();
				fs.Close();
			} 
			catch (Exception e)
			{			
				System.Console.WriteLine(e.StackTrace);
				result = false;
			}
			return result;
		}

		// ---------------------------------------------------------------------------

		byte GetTagVersion(TagRecord TagData)
		{
			byte result = TAG_VERSION_1_0;
			// Terms for ID3v1.1
			if ( (('\0' == TagData.Comment[28]) && ('\0' != TagData.Comment[29])) ||
				((32 == (byte)TagData.Comment[28]) && (32 != (byte)TagData.Comment[29])) )
				result = TAG_VERSION_1_1;

			return result;
		}

		// ********************** Private functions & voids *********************

		void FSetTitle(String newTrack)
		{
			FTitle = newTrack.TrimEnd();
		}

		// ---------------------------------------------------------------------------

		void FSetArtist(String NewArtist)
		{
			FArtist = NewArtist.TrimEnd();
		}

		// ---------------------------------------------------------------------------

		void FSetAlbum(String NewAlbum)
		{
			FAlbum = NewAlbum.TrimEnd();
		}

		// ---------------------------------------------------------------------------

		void FSetYear(String NewYear)
		{
			FYear = NewYear.TrimEnd();
		}

		// ---------------------------------------------------------------------------

		void FSetComment(String NewComment)
		{
			FComment = NewComment.TrimEnd();
		}

		// ---------------------------------------------------------------------------

		void FSetTrack(byte NewTrack)
		{
			FTrack = NewTrack;
		}

		// ---------------------------------------------------------------------------

		void FSetGenreID(byte NewGenreID)
		{
			FGenreID = NewGenreID;
		}

		// ---------------------------------------------------------------------------

		string FGetGenre()
		{
			String result = "";
			// Return an empty string if the current GenreID is not valid
			if ( FGenreID < MAX_MUSIC_GENRES ) result = MusicGenre[FGenreID];
			return result;
		}

		// ********************** Public functions & voids **********************

		public TID3v1()
		{		
			ResetData();
		}

		// ---------------------------------------------------------------------------

		public void ResetData()
		{
			FExists = false;
			FVersionID = TAG_VERSION_1_0;
			FTitle = "";
			FArtist = "";
			FAlbum = "";
			FYear = "";
			FComment = "";
			FTrack = 0;
			FGenreID = DEFAULT_GENRE;
		}

		// ---------------------------------------------------------------------------

		public bool ReadFromFile(String FileName)
		{
			TagRecord TagData = new TagRecord();
	
			// Reset and load tag data from file to variable
			ResetData();
			bool result = ReadTag(FileName, ref TagData);
			// Process data if loaded and tag header OK
			if ((result) && Utils.StringEqualsArr("TAG",TagData.Header))
			{
				FExists = true;
				FVersionID = GetTagVersion(TagData);
				// Fill properties with tag data
				FTitle = Utils.BuildStringCStyle(TagData.Title);
				FArtist = Utils.BuildStringCStyle(TagData.Artist).TrimEnd();
				FAlbum = Utils.BuildStringCStyle(TagData.Album).TrimEnd();
				FYear = Utils.BuildStringCStyle(TagData.Year).TrimEnd();
				if (TAG_VERSION_1_0 == FVersionID)
				{
					FComment = Utils.BuildStringCStyle(TagData.Comment).TrimEnd();
				}
				else
				{
					char[] newComment = new char[28];
					Array.Copy(TagData.Comment,0,newComment,0,28);
					FComment = Utils.BuildStringCStyle(newComment).TrimEnd();
					FTrack = (byte)TagData.Comment[29];
				}
				FGenreID = TagData.Genre;
			}
			return result;
		}

		// ---------------------------------------------------------------------------

		public bool RemoveFromFile(String FileName)
		{
			TagRecord TagData = new TagRecord();
			bool result;
	
			// Find tag
			result = ReadTag(FileName, ref TagData);
			// Delete tag if loaded and tag header OK
			if ( (result) && Utils.StringEqualsArr("TAG",TagData.Header) ) result = RemoveTag(FileName);
			return result;
		}

		// ---------------------------------------------------------------------------

		public bool SaveToFile(String FileName)
		{
			TagRecord TagData = new TagRecord();
	
			// Prepare tag record
			TagData.Reset();

			TagData.Header[0] = 'T';
			TagData.Header[1] = 'A';
			TagData.Header[2] = 'G';
		
			Array.Copy(FTitle.ToCharArray(), TagData.Title, 30);
			Array.Copy(FArtist.ToCharArray(), TagData.Artist, 30);
			Array.Copy(FAlbum.ToCharArray(), TagData.Album, 30);
			Array.Copy(FYear.ToCharArray(), TagData.Year, 4);
			Array.Copy(FComment.ToCharArray(), TagData.Comment, 30);

			if (FTrack > 0)
			{
				TagData.Comment[28] = '\0';
				TagData.Comment[29] = (char)FTrack;
			}
			TagData.Genre = FGenreID;
	
			// Delete old tag and write new tag
			return ( (RemoveFromFile(FileName)) && (SaveTag(FileName, TagData)) );
		}
	}
}