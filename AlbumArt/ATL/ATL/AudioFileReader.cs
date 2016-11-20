using System;
using System.IO;

namespace ATL.AudioReaders
{
	/// <summary>
	/// This class is the one which is _really_ called when encountering a file.
	/// It calls AudioReaderFactory and queries AudioDataReader/MetaDataReader to provide physical 
	/// _and_ meta information about the given file.
	/// </summary>
	public class AudioFileReader
	{	
		private AudioReaderFactory theFactory;					// Reader Factory
		private AudioDataReader audioData;						// Audio data reader used for this file
		private MetaDataReader metaData;						// Metadata reader used for this file
		private String thePath;									// Path of this file

		// ------------------------------------------------------------------------------------------

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path of the file to be parsed</param>
		public AudioFileReader(String path)
		{
			thePath = path;
			theFactory = AudioReaderFactory.GetInstance();
			audioData = theFactory.GetDataReader(path);
			metaData = theFactory.GetMetaReader(path, audioData);
		}


		/// <summary>
		/// Title of the track
		/// </summary>
		public String Title
		{
			get { return metaData.Title.Replace('\t',' ').Replace('\n',' ').Replace("\0",""); }
		}
		/// <summary>
		/// Artist
		/// </summary>
		public String Artist
		{
			get { return metaData.Artist.Replace('\t',' ').Replace('\n',' ').Replace("\0",""); }
		}
		/// <summary>
		/// Comments
		/// </summary>
		public String Comment
		{
			get { return metaData.Comment.Replace('\t',' ').Replace('\n',' ').Replace("\0",""); }
		}
		/// <summary>
		/// Genre
		/// </summary>
		public String Genre
		{
			get { return metaData.Genre.Replace('\t',' ').Replace('\n',' ').Replace("\0",""); }
		}
		/// <summary>
		/// Track number
		/// </summary>
		public int Track
		{
			get { return metaData.Track; }
		}
		/// <summary>
		/// Year
		/// </summary>
		public int Year
		{
			get { return FindYearInString(metaData.Year); }
		}
		/// <summary>
		/// Album title
		/// </summary>
		public String Album
		{
			get { return metaData.Album.Replace('\t',' ').Replace('\n',' ').Replace("\0",""); }
		}
		/// <summary>
		/// Track duration (seconds)
		/// </summary>
		public int Duration
		{
			get { return (int)Math.Round(audioData.Duration); }
		}
		/// <summary>
		/// Bitrate
		/// </summary>
		public int BitRate
		{
			get { return (int)Math.Round(audioData.BitRate); }
		}
		/// <summary>
		/// Codec family
		/// </summary>
		public int CodecFamily
		{
			get { return audioData.CodecFamily; }
		}


		/// <summary>
		/// Finds a year (4 consecutive numeric chars) in a string
		/// and converts it to an integer
		/// </summary>
		/// <param name="str">String to search the year into</param>
		/// <returns>Integer representation of the found year; 0 if no year has been found</returns>
		public static int FindYearInString(String str)
		{
			if (null == str) return 0;

			int startIndex = 0;			
			str = str.Trim();

			try
			{
				while (startIndex < str.Length - 4	&& ( !Char.IsNumber(str[startIndex])
					|| !Char.IsNumber(str[startIndex+1])
					|| !Char.IsNumber(str[startIndex+2])
					|| !Char.IsNumber(str[startIndex+3]) ))
				{
					startIndex++;
				}
				return Int32.Parse(str.Substring(startIndex,Math.Min(4,str.Length)));
			} 
			catch 
			{
				return 0;
			}
		}
	}
}
