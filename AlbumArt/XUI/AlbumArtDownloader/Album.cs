using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace AlbumArtDownloader
{
	/// <summary>
	/// An album, as found by one of the Browser tasks
	/// </summary>
	public class Album: INotifyPropertyChanged
	{
		public Album(string basePath, string artistName, string albumName)
		{
			mBasePath = basePath;
			mName = albumName;
			mArtist = artistName;
			mArtFile = null;
		}

		private string mName;
		public string Name
		{
			get 
			{
				if (mName == null) //Disallow Null names
					return String.Empty;

				return mName; 
			}
		}

		private string mArtist;
		public string Artist
		{
			get 
			{
				if (mArtist == null) //Disallow Null artists
					return String.Empty;

				return mArtist; 
			}
		}

		private string mBasePath;
		/// <summary>
		/// The path relative to which images are found.
		/// </summary>
		public string BasePath
		{
			get 
			{
				//Null base paths are allowed, but then relative image search paths can't be used.
				return mBasePath; 
			}
		}

		/// <summary>
		/// The art file, or null if none has been found
		/// <remarks>Note that this will not automatically set <see cref="ArtFileSize"/>, that must
		/// be set separately if required, or use the <see cref="SetArtFile"/> to attempt to do so automatically</remarks>
		/// </summary>
		private string mArtFile;
		public string ArtFile
		{
			get { return mArtFile; }
			set
			{
				if (value != mArtFile)
				{
					mArtFile = value;
					NotifyPropertyChanged("ArtFile");
				}
			}
		}

		/// <summary>
		/// The art file filesize in bytes, or 0 if none has been found
		/// </summary>
		private long mArtFileSize;
		public long ArtFileSize
		{
			get { return mArtFileSize; }
			set
			{
				if (value != mArtFileSize)
				{
					mArtFileSize = value;
					NotifyPropertyChanged("ArtFileSize");
				}
			}
		}

		/// <summary>
		/// The art file dimensions width in pixels, or 0 if none has been found
		/// </summary>
		private int mArtFileWidth;
		public int ArtFileWidth
		{
			get { return mArtFileWidth; }
			set
			{
				if (value != mArtFileWidth)
				{
					mArtFileWidth = value;
					NotifyPropertyChanged("ArtFileWidth");
				}
			}
		}

		/// <summary>
		/// The art file dimensions height in pixels, or 0 if none has been found
		/// </summary>
		private int mArtFileHeight;
		public int ArtFileHeight
		{
			get { return mArtFileHeight; }
			set
			{
				if (value != mArtFileHeight)
				{
					mArtFileHeight = value;
					NotifyPropertyChanged("ArtFileHeight");
				}
			}
		}

		/// <summary>
		/// The art file last modified date in bytes, or null if none has been found
		/// </summary>
		private DateTime? mArtFileDate;
		public DateTime? ArtFileDate
		{
			get { return mArtFileDate; }
			set
			{
				if (value != mArtFileDate)
				{
					mArtFileDate = value;
					NotifyPropertyChanged("ArtFileDate");
				}
			}
		}

		private ArtFileStatus mArtFileStatus;
		public ArtFileStatus ArtFileStatus
		{
			get { return mArtFileStatus; }
			set
			{
				if (value != mArtFileStatus)
				{
					mArtFileStatus = value;
					NotifyPropertyChanged("ArtFileStatus");
				}
			}
		}

		/// <summary>
		/// Sets all properties related to an album having a file present:
		/// <see cref="ArtFile"/> set to <paramref name="filePath"/>
		/// <see cref="ArtFileStatus"/> set to <see cref="ArtFileStatus.Present"/>
		/// <see cref="ArtFileSize"/> set to the file size (or 0 if it could not be determined)
		/// <see cref="ArtFileWidth"/> and <see cref="ArtFileHeight"/> to the file image dimensions (or 0 if they could not be determined)
		/// </summary>
		/// <param name="filePath"></param>
		public void SetArtFile(string filePath)
		{
			ArtFile = filePath;
			ArtFileStatus = ArtFileStatus.Present;

			if (EmbeddedArtHelpers.IsEmbeddedArtPath(filePath))
			{
				//Get the size of the embedded image, not the size of the file itself
				try
				{
					var embeddedArt = EmbeddedArtHelpers.GetEmbeddedArt(filePath);
					if (embeddedArt != null)
					{
						int ignored;
						string unembeddedFilePath;
						EmbeddedArtHelpers.SplitToFilenameAndIndex(filePath, out unembeddedFilePath, out ignored);

						ArtFileDate = File.GetLastWriteTime(unembeddedFilePath);
						ArtFileSize = embeddedArt.Data.Count;

						using (var dataStream = new MemoryStream(embeddedArt.Data.Data, false))
						{
							var bitmapDecoder = BitmapDecoder.Create(dataStream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
							ArtFileWidth = bitmapDecoder.Frames[0].PixelWidth;
							ArtFileHeight = bitmapDecoder.Frames[0].PixelHeight;
						}
					}
				}
				catch (Exception)
				{
					//Ignore exceptions when reading the embedded artwork; it's not important at this stage
					ArtFileSize = 0;
					ArtFileWidth = ArtFileHeight = 0;
					ArtFileDate = null;
				}
			}
			else
			{
				//Not an embedded image, but an image file itself
				try
				{
					var fileInfo = new FileInfo(filePath);
					ArtFileSize = fileInfo.Length;
					ArtFileDate = fileInfo.LastWriteTime;
				}
				catch (Exception)
				{
					//Ignore exceptions when reading the filesize and date it's not important
					ArtFileSize = 0;
					ArtFileDate = null;
				}

				//Attempt to get the image dimesions
				try
				{
					using (var fileStream = File.OpenRead(filePath))
					{
						var bitmapDecoder = BitmapDecoder.Create(fileStream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
						ArtFileWidth = bitmapDecoder.Frames[0].PixelWidth;
						ArtFileHeight = bitmapDecoder.Frames[0].PixelHeight;
					}
				}
				catch (Exception)
				{
					//Ignore exceptions when reading the dimensions, they aren't important
					ArtFileWidth = ArtFileHeight = 0;
				}
			}
		}

		#region Property Notification
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler temp = PropertyChanged;
			if (temp != null)
			{
				temp(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion

	}
}
