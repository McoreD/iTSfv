using System;
using System.Collections.Generic;

namespace AlbumArtDownloader
{
	internal class SearchParameters
	{
		private string mArtist;
		private string mAlbum;
		private Dictionary<Source, object> mSources = new Dictionary<Source, object>();

		public SearchParameters(string artist, string album)
		{
			mArtist = artist;
			mAlbum = album;
		}

		public string Artist
		{
			get { return mArtist; }
		}
		public string Album
		{
			get { return mAlbum; }
		}

		public void AddSource(Source source)
		{
			mSources[source] = null;
		}

		public bool RemoveSource(Source source)
		{
			return mSources.Remove(source);
		}

		public bool ContainsSource(Source source)
		{
			return mSources.ContainsKey(source);
		}

		public int SourceCount
		{
			get { return mSources.Count; }
		}
	}
}