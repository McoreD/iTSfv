using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace AlbumArtDownloader
{
	/// <summary>
	/// An observable collection of albums, unique by artist and album name (case insensitive).
	/// </summary>
	internal class ObservableAlbumCollection : IList<Album>, INotifyCollectionChanged, IList
	{
		private Dictionary<string, Dictionary<string, Album>> mAlbumsByArtist = new Dictionary<string,Dictionary<string,Album>>();
		private List<Album> mAlbumsByIndex = new List<Album>(); //Maintain a separate list by index, for implementing IList

		private int mVersion; //Tick to check for enumeration invalidation

		public event NotifyCollectionChangedEventHandler CollectionChanged;
		private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			NotifyCollectionChangedEventHandler temp = CollectionChanged;
			if (temp != null)
			{
				temp(this, e);
			}
		}

		int IList.Add(object album)
		{
			Add((Album)album);
			return Count - 1;
		}
		public void Add(Album album)
		{
			Insert(Count, album);
		}

		void IList.Insert(int index, object album)
		{
			Insert(index, (Album)album);
		}
		public void Insert(int index, Album album)
		{
			if (index > Count || index < 0)
			{
				throw new ArgumentOutOfRangeException("index", index, null);
			}

			//Check for uniquness
			string artistNameKey = album.Artist.ToLowerInvariant();
			string albumNameKey = album.Name.ToLowerInvariant();
			
			Dictionary<string, Album> artistAlbums;
			lock (this)
			{
				if (mAlbumsByArtist.TryGetValue(artistNameKey, out artistAlbums))
				{
					if (!artistAlbums.ContainsKey(albumNameKey))
					{
						//Unique
						artistAlbums.Add(albumNameKey, album);
						mAlbumsByIndex.Insert(index, album);
						mVersion++;
						RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, album));
					}
				}
				else
				{
					//unique
					artistAlbums = new Dictionary<string, Album>();
					artistAlbums.Add(albumNameKey, album);
					mAlbumsByArtist.Add(artistNameKey, artistAlbums);
					mAlbumsByIndex.Insert(index, album);
					mVersion++;
					RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, album));
				}
			}
		}

		public void RemoveAt(int index)
		{
			Remove(this[index]);
		}

		void IList.Remove(object album)
		{
			Remove((Album)album);
		}
		public bool Remove(Album album)
		{
			string artistNameKey = album.Artist.ToLowerInvariant();
			string albumNameKey = album.Name.ToLowerInvariant();
			
			Dictionary<string, Album> artistAlbums;
			lock (this)
			{
				if (mAlbumsByArtist.TryGetValue(artistNameKey, out artistAlbums))
				{
					if (artistAlbums.TryGetValue(albumNameKey, out album))
					{
						artistAlbums.Remove(albumNameKey);
						mAlbumsByIndex.Remove(album);
						mVersion++;
						RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, album));
						return true;
					}
				}
			}
			return false;
		}

		public void Clear()
		{
			lock (this)
			{
				mAlbumsByArtist.Clear();
				mAlbumsByIndex.Clear();
				mVersion++;
				RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		bool IList.Contains(object album)
		{
			return Contains((Album)album);
		}
		public bool Contains(Album album)
		{
			string artistNameKey = album.Artist.ToLowerInvariant();
			string albumNameKey = album.Name.ToLowerInvariant();
			
			Dictionary<string, Album> artistAlbums;
			lock (this)
			{
				if (mAlbumsByArtist.TryGetValue(artistNameKey, out artistAlbums))
				{
					if (artistAlbums.ContainsKey(albumNameKey))
					{
						return true;
					}
				}
			}
			return false;
		}

		public int Count
		{
			get 
			{
				lock (this)
				{
					return mAlbumsByIndex.Count;
				}
			}
		}
		public IEnumerator<Album> GetEnumerator()
		{
			return mAlbumsByIndex.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)mAlbumsByIndex).GetEnumerator();
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((IList)mAlbumsByIndex).CopyTo(array, index);
		}
		public void CopyTo(Album[] array, int arrayIndex)
		{
			mAlbumsByIndex.CopyTo(array, arrayIndex);
		}

		int IList.IndexOf(object album)
		{
			return IndexOf((Album)album);
		}
		public int IndexOf(Album album)
		{
			//Obtain the stored album with the same artist and name as the specified one (as this collection maintains uniqueness by artist/name
			string artistNameKey = album.Artist.ToLowerInvariant();
			string albumNameKey = album.Name.ToLowerInvariant();

			Dictionary<string, Album> artistAlbums;
			lock (this)
			{
				if (mAlbumsByArtist.TryGetValue(artistNameKey, out artistAlbums))
				{
					Album canonicalAlbum;
					if (artistAlbums.TryGetValue(albumNameKey, out canonicalAlbum))
					{
						return mAlbumsByIndex.IndexOf(canonicalAlbum);
					}
				}
			}
			return -1;
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (Album)value;
			}
		}
		public Album this[int index]
		{
			get
			{
				return mAlbumsByIndex[index];
			}
			set
			{
				RemoveAt(index);
				Insert(index, value);
			}
		}

		bool IList.IsFixedSize
		{
			get { return false; }
		}

		bool ICollection.IsSynchronized
		{
			get { return ((IList)mAlbumsByIndex).IsSynchronized; }
		}

		object ICollection.SyncRoot
		{
			get { return ((IList)mAlbumsByIndex).SyncRoot; }
		}
	}
}
