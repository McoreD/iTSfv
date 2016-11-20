using AlbumArtDownloader.Scripts;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AlbumArtDownloader
{
	public class AlbumArt : DependencyObject, INotifyPropertyChanged
	{
		private readonly Source mSource;
		private readonly object mFullSizeCallbackParameter;
		private readonly string mSuggestedFilenameExtension;
		
		public event PropertyChangedEventHandler PropertyChanged;

		public AlbumArt(Source source, byte[] thumbnailData, string name, string infoUri, int width, int height, object fullSizeCallbackParameter, CoverType coverType, string suggestedFilenameExtension)
		{
			mSource = source;
			BitmapData = thumbnailData;
			ResultName = name;
			InfoUri = infoUri;
			SetImageDimensions(width, height);
			mFullSizeCallbackParameter = fullSizeCallbackParameter;
			CoverType = coverType;
			mSuggestedFilenameExtension = suggestedFilenameExtension;
		}
		
		/// <summary>
		/// Constructs an AlbumArt with an already known full-size image
		/// </summary>
		public AlbumArt(Source source, string name, string infoUri, int width, int height, byte[] fullSizeImageData, CoverType coverType)
		{
			mSource = source;
			ResultName = name;
			InfoUri = infoUri;
			CoverType = coverType;

			BitmapData = fullSizeImageData;
			FileSize = fullSizeImageData.LongLength;

			if (width == -1 && height == -1 && Image != null)
			{
				//Have to decode the image data to get width and height
				SetImageDimensions(Image.PixelWidth, Image.PixelHeight);
			}
			else
			{
				//Believe the dimensions specified
				SetImageDimensions(width, height);
			}

			mIsFullSize = true;
		}

		#region Dependency Properties
		public static readonly DependencyProperty DefaultFilePathPatternProperty = DependencyProperty.Register("DefaultFilePathPattern", typeof(string), typeof(AlbumArt), new FrameworkPropertyMetadata(String.Empty, new PropertyChangedCallback(OnDefaultFilePathPatternChanged)));
		public string DefaultFilePathPattern
		{
			get { return (string)GetValue(DefaultFilePathPatternProperty); }
			set { SetValue(DefaultFilePathPatternProperty, value); }
		}
		private static void OnDefaultFilePathPatternChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			sender.CoerceValue(FilePathProperty);
		}

		public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(AlbumArt), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnFilePathChanged), new CoerceValueCallback(CoerceFilePath)));
		public string FilePath
		{
			get { return (string)GetValue(FilePathProperty); }
			set { SetValue(FilePathProperty, value); }
		}
		private static void OnFilePathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			AlbumArt albumArt = (AlbumArt)sender;
			albumArt.IsSaved = false; //Not saved if the file path has changed
		}
		private static object CoerceFilePath(DependencyObject sender, object value)
		{
			AlbumArt albumArt = (AlbumArt)sender;

			if (String.IsNullOrEmpty((string)value))
			{
				albumArt.IsCustomFilePath = false;

				//Construct the default file path
				string filePath = albumArt.DefaultFilePathPattern
									.Replace("%name%", Common.MakeSafeForPath(albumArt.ResultName))
									.Replace("%source%", Common.MakeSafeForPath(albumArt.SourceName))
									.Replace("%size%", String.Format("{0} x {1}", albumArt.ImageWidth, albumArt.ImageHeight))
                                    .Replace("%extension%", albumArt.ImageFileDefaultExtension)
									.Replace("%preset%", Common.MakeSafeForPath(albumArt.Preset))
									.Replace("%type%", Common.MakeSafeForPath(albumArt.CoverType.ToString()));

				filePath = ReplaceCustomTypeString(filePath, albumArt.CoverType);

				return filePath;
			}
			else
			{
				albumArt.IsCustomFilePath = true;
				return value;
			}
		}

		private static string ReplaceCustomTypeString(string path, CoverType coverType)
		{
			return Regex.Replace(path, @"%type(?:\((?<names>[^)]*)\))?%",
				new MatchEvaluator(delegate(Match match)
				{
					string name;
					string[] names = match.Groups["names"].Value.Split(',');
					if (names.Length > (int)coverType)
					{
						name = names[(int)coverType];
					}
					else
					{
						//No custom name provided
						name = coverType.ToString();
					}
					return name;
				}),
				RegexOptions.IgnoreCase);
		}


		#region DefaultPresetValue
		public static readonly DependencyProperty DefaultPresetValueProperty = DependencyProperty.Register("DefaultPresetValue", typeof(string), typeof(AlbumArt), new FrameworkPropertyMetadata(String.Empty, new PropertyChangedCallback(OnDefaultPresetValueChanged)));

		public string DefaultPresetValue
		{
			get { return (string)GetValue(DefaultPresetValueProperty); }
			set { SetValue(DefaultPresetValueProperty, value); }
		}
		private static void OnDefaultPresetValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			sender.CoerceValue(FilePathProperty);
		}
		#endregion
		

		#endregion
		#region Properties
		private byte[] mBitmapData;
		/// <summary>The Album Art, as original source data. Backs the <see cref="BitmapImage"/></summary>
		private byte[] BitmapData
		{
			get
			{
				return mBitmapData;
			}
			set
			{
				if (mBitmapData != value)
				{
					mBitmapData = value;

					//Reset the cached image decoder (it will be recreated from the new bitmap data), and notify of the change.
					mCachedImageDecoder = null;

					NotifyPropertyChanged("Image");
					NotifyPropertyChanged("ImageFileExtensions");
					NotifyPropertyChanged("ImageFileDefaultExtension");
				}
			}
		}

		/// <summary>
		/// Returns the contents of the <see cref="Image"/> as a stream
		/// </summary>
		/// <returns></returns>
		internal Stream GetBitmapDataStream()
		{
			if (BitmapData == null)
			{
				return null;
			}
			return new MemoryStream(BitmapData);
		}

		private BitmapDecoder mCachedImageDecoder;
		private BitmapDecoder ImageDecoder
		{
			get
			{
				if (mCachedImageDecoder == null && BitmapData != null)
				{
					Dispatcher.VerifyAccess();
					//Create the image source from the bitmap data
					mCachedImageDecoder = CreateImageDecoderForBitmapData(GetBitmapDataStream());
				}
				return mCachedImageDecoder;
			}
		}

		private BitmapDecoder CreateImageDecoderForBitmapData(Stream bitmapDataStream)
		{
			try
			{
				var decoder = BitmapDecoder.Create(bitmapDataStream, BitmapCreateOptions.None, BitmapCacheOption.None); // Don't cache, as the data is already coming straight from memory.
				var frame = decoder.Frames.FirstOrDefault();
				if (frame != null)
				{
					// Ensure the pixels can be read from the frame
					int stride = frame.PixelWidth * (frame.Format.BitsPerPixel / 8);
					byte[] data = new byte[stride * frame.PixelHeight];
					frame.CopyPixels(data, stride, 0);
				}
				return decoder;
			}
			catch (Exception)
			{
				System.Diagnostics.Trace.WriteLine(String.Format("Could not construct bitmap from data for: \"{0}\" from \"{1}\"", ResultName, SourceName));
			}
			return null;
		}

		public BitmapSource Image
		{
			get 
			{
				if (BitmapData == null || ImageDecoder == null)
				{
					return null;
				}
				return ImageDecoder.Frames[0];
			}
		}

		public IEnumerable<String> ImageFileExtensions
		{
			get
			{
				if (BitmapData == null || ImageDecoder == null)
				{
					return new string[0];
				}

				if (!IsFullSize && !String.IsNullOrEmpty(mSuggestedFilenameExtension))
				{
					// If the full size image has not bee donwloaded, and there's a suggested filename extension, then use the suggested extension.
					return new[] {mSuggestedFilenameExtension};
				}

				return from s in ImageDecoder.CodecInfo.FileExtensions.Split(',') select s.Substring(1);
			}
		}

		public string ImageFileDefaultExtension
		{
			get
			{
				// Default to the first available extension for the image
				var extension = ImageFileExtensions.FirstOrDefault();

				//Hack for consistency with previous versions - use .jpg by default for jpeg files, not .jpeg
				if ("jpeg".Equals(extension, StringComparison.OrdinalIgnoreCase))
				{
					return "jpg";
				}
				return extension;
			}
		}

		public event EventHandler ImageSizeChanged;

		private int mImageWidth;
		public int ImageWidth
		{
			get { return mImageWidth; }
			private set
			{
				if (mImageWidth != value)
				{
					mImageWidth = value;
					NotifyPropertyChanged("ImageWidth");
					CoerceValue(FilePathProperty);

					//Raise the ImageSizeChanged event
					var temp = ImageSizeChanged;
					if (temp != null)
					{
						temp(this, EventArgs.Empty);
					}
				}
			}
		}

		private int mImageHeight;
		public int ImageHeight
		{
			get { return mImageHeight; }
			private set
			{
				if (mImageHeight != value)
				{
					mImageHeight = value;
					NotifyPropertyChanged("ImageHeight");
					CoerceValue(FilePathProperty);

					//Raise the ImageSizeChanged event
					var temp = ImageSizeChanged;
					if (temp != null)
					{
						temp(this, EventArgs.Empty);
					}
				}
			}
		}

		/// <summary>
		/// Area of the image, in pixels. Intended for use with sorting.
		/// </summary>
		public double ImageArea
		{
			get
			{
				return ImageHeight * ImageWidth;
			}
		}

		/// <summary>
		/// Sets the width and height of the image in a single operation, to avoid redundant coercion and ImageSizeChanged events.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public void SetImageDimensions(int width, int height)
		{
			bool changed = false;
			if (mImageWidth != width)
			{
				changed = true;
				mImageWidth = width;
				NotifyPropertyChanged("ImageWidth");
			}
			if (mImageHeight != height)
			{
				changed = true;
				mImageHeight = height;
				NotifyPropertyChanged("ImageHeight");
			}
			if (changed)
			{
				CoerceValue(FilePathProperty);

				NotifyPropertyChanged("ImageArea");

				//Raise the ImageSizeChanged event
				var temp = ImageSizeChanged;
				if (temp != null)
				{
					temp(this, EventArgs.Empty);
				}
			}
		}

		private long mFileSize = -1;
		public long FileSize
		{
			get { return mFileSize; }
			internal set
			{
				if (mFileSize != value)
				{
					mFileSize = value;
					NotifyPropertyChanged("FileSize");
				}
			}
		}


		private string mResultName;
		public string ResultName
		{
			get { return mResultName; }
			internal set
			{
				if (mResultName != value)
				{
					mResultName = value;
					NotifyPropertyChanged("ResultName");
					CoerceValue(FilePathProperty);
				}
			}
		}

		private string mInfoUri;
		public string InfoUri
		{
			get { return mInfoUri; }
			internal set
			{
				if (mInfoUri != value)
				{
					mInfoUri = value;
					NotifyPropertyChanged("InfoUri");
				}
			}
		}
		

		private string mPreset;
		public string Preset
		{
			get 
			{
				if (mPreset == null)
				{
					return DefaultPresetValue;
				}
				return mPreset; 
			}
			internal set
			{
				if (mPreset != value)
				{
					mPreset = value;
					NotifyPropertyChanged("Preset");
					CoerceValue(FilePathProperty);
				}
			}
		}

		public bool IsSourceLocal
		{
			//TODO: Add possibility of other local sources? Perhaps a flag on a sources?
			get { return mSource is LocalFilesSource; }
		}

		public string SourceName
		{
			get { return mSource.Name; }
		}

		public string SourceCategory
		{
			get { return mSource.Category; }
		}

		private bool mIsCustomFilePath;
		public bool IsCustomFilePath
		{
			get { return mIsCustomFilePath; }
			private set
			{
				if (mIsCustomFilePath != value)
				{
					mIsCustomFilePath = value;
					NotifyPropertyChanged("IsCustomFilePath");
				}
			}
		}
		private bool mIsSaved;
		public bool IsSaved
		{
			get { return mIsSaved; }
			internal set
			{
				if (mIsSaved != value)
				{
					mIsSaved = value;
					NotifyPropertyChanged("IsSaved");
				}
			}
		}
		private bool mIsSaving;
		public bool IsSaving
		{
			get { return mIsSaving; }
			private set
			{
				if (mIsSaving != value)
				{
					mIsSaving = value;
					NotifyPropertyChanged("IsSaving");
				}
			}
		}
		private bool mIsDownloading;
		public bool IsDownloading
		{
			get { return mIsDownloading; }
			private set
			{
				if (mIsDownloading != value)
				{
					mIsDownloading = value;
					NotifyPropertyChanged("IsDownloading");
				}
			}
		}

		public event EventHandler CoverTypeChanged;
		
		private CoverType mCoverType = CoverType.Unknown;
		public CoverType CoverType
		{
			get { return mCoverType; }
			set
			{
				if (mCoverType != value)
				{
					mCoverType = value;
					NotifyPropertyChanged("CoverType");
					CoerceValue(FilePathProperty);

					//Raise the CoverTypeChanged event
					var temp = CoverTypeChanged;
					if (temp != null)
					{
						temp(this, EventArgs.Empty);
					}
				}
			}
		}		
		#endregion

		private void NotifyPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler temp = PropertyChanged;
			if (temp != null)
			{
				temp(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#region Retrieve Full Size Image
		private object mRetrieveFullSizeImageSynchronization = new object();
		private List<WaitCallback> mOnFullSizeRetrievedCallbacks = new List<WaitCallback>();
		private List<EventWaitHandle> mOnFullSizeRetrievedWaitHandles = new List<EventWaitHandle>();
		
		/// <summary>
		/// Begins an asynchronous retrieval of the full size image
		/// </summary>
		public void RetrieveFullSizeImage()
		{
			RetrieveFullSizeImage(null, null);
		}
		/// <summary>
		/// Begins an asynchronous retrieval of the full size image, notifying a callback on completion
		/// <param name="callback">Called when the retrieval completes. State object will be the AlbumArt instance.</param>
		/// </summary>
		public void RetrieveFullSizeImage(WaitCallback callback)
		{
			RetrieveFullSizeImage(callback, null);
		}
		/// <summary>
		/// Begins an asynchronous retrieval of the full size image, signalling a wait handle on completion
		/// <param name="waitHandle">Signalled when the retrieval completes</param>
		/// </summary>
		public void RetrieveFullSizeImage(EventWaitHandle waitHandle)
		{
			RetrieveFullSizeImage(null, waitHandle);
		}
		private void RetrieveFullSizeImage(WaitCallback callback, EventWaitHandle waitHandle)
		{
			lock(mRetrieveFullSizeImageSynchronization)
			{
				if (!mIsFullSize)
				{
					if (callback != null)
					{
						//Add this callback to the list of callbacks to call when the retrieval finishes.
						mOnFullSizeRetrievedCallbacks.Add(callback);
					}
					if (waitHandle != null)
					{
						mOnFullSizeRetrievedWaitHandles.Add(waitHandle);
					}

					if (!mIsDownloading)
					{
						//Start downloading
						IsDownloading = true;
						ThreadPool.QueueUserWorkItem(new WaitCallback(RetrieveFullSizeImageWorker));
					}
					return;
				}
			}

			//Already full size.
			//Raise the callback and wait handle anyway, in case anything is waiting on it
			if (callback != null)
			{
				callback(this);
			}
			if (waitHandle != null)
			{
				waitHandle.Set();
			}
			
		}

		private void RetrieveFullSizeImageWorker(object state)
		{
			var fullSizeImageData = mSource.RetrieveFullSizeImageData(mFullSizeCallbackParameter);
			if (fullSizeImageData != null) //If it is null, just use the thumbnail image
			{
				//Attempt to create an image decoder for this data. If it fails, then this is not image data so fall back on thumbnail image
				//If it succeeds, the image decoder must be discarded anyway, as it's not been created on the dispatcher thread.
				if (CreateImageDecoderForBitmapData(new MemoryStream(fullSizeImageData)) != null)
				{
					//This is valid image data, so assign it
					BitmapData = fullSizeImageData;
				}
			}

			List<WaitCallback> callbacks;
			lock (mRetrieveFullSizeImageSynchronization)
			{
				mIsFullSize = true;
				mIsDownloading = false;

				//Signal all the wait handles to continue
				foreach (var waitHandle in mOnFullSizeRetrievedWaitHandles)
				{
					waitHandle.Set();
				}

				//Take a copy of the callbacks to call, but call them outside of the lock.
				callbacks = new List<WaitCallback>(mOnFullSizeRetrievedCallbacks);
				mOnFullSizeRetrievedCallbacks.Clear();
			}

			//Update the values, notify of changes, and invoke callbacks in the main dispatcher thread.
			Dispatcher.Invoke(DispatcherPriority.DataBind, new ThreadStart(delegate
			{
				if (Image != null) //Image should *never* be null, but this might be what's causing reported crashes at this location.
				{
					SetImageDimensions(Image.PixelWidth, Image.PixelHeight);
					FileSize = BitmapData.LongLength;
				}
				else
				{
					System.Diagnostics.Debug.Fail("Image was unexpectedly null");
				}

				NotifyPropertyChanged("IsFullSize");
				NotifyPropertyChanged("IsDownloading");
				CoerceValue(FilePathProperty);
				
				foreach (var callback in callbacks)
				{
					callback(this);
				}
			}));
		}


		private bool mIsFullSize = false;
		/// <summary>If true, then the current value of Image is the full sized image, and no further retrieval is necessary.</summary>
		public bool IsFullSize
		{
			get { return mIsFullSize; }
		}
		#endregion

		#region Saving
		/// <summary>
		/// Begins an asynchronous Save operation, including downloading the full size image.
		/// The operation is synchronous until download begins, then asynch.
		/// </summary>
		internal void Save(Window dialogOwner)
		{
			var filePath = FilePath;
			if (String.IsNullOrEmpty(filePath))
			{
				SaveAs(dialogOwner);
				return;
			}
			//Check that it is possible to save to FilePath
			try
			{
				//Check if FilePath already exists
				if (File.Exists(filePath))
				{
					//Confirm overwrite
					filePath = OverwriteExistingWarning.Show(filePath, CreateSaveAsDialog(), dialogOwner);
					if (filePath == null) //Cancelled
					{
						return;
					}

					if (filePath != FilePath)
					{
						// Update the file path to indicate where it was actually saved
						FilePath = filePath;
					}
				}

				DirectoryInfo folder = new DirectoryInfo(Path.GetDirectoryName(filePath));
				if (!folder.Exists)
					folder.Create();

				//Create a dummy file so that other save operations will notice if they would overwrite them
				File.Create(filePath, 1).Close();
			}
			catch (Exception e)
			{
				MessageBox.Show(dialogOwner, String.Format("Could not save image '{0}':\n\n{1}", filePath, e.Message), "Album Art Downloader", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			IsSaving = true;
			
			//Pass the current filepath as a variable so that if the FilePath property changes, it will still be this filepath that's saved to, not the new one.
			RetrieveFullSizeImage(new WaitCallback(delegate { SaveInternal(filePath); }));
		}

		/// <summary>
		/// Begins an asynchronous Save As operation, including downloading the full size image.
		/// The operation is synchronous until download begins (including the dialog), then asynch.
		/// </summary>
		internal void SaveAs(Window dialogOwner)
		{
			SaveFileDialog saveFileDialog = CreateSaveAsDialog();

			if (saveFileDialog.ShowDialog(dialogOwner).GetValueOrDefault(false))
			{
				//HACK: DefaultExt doesn't actually seem to work, so force it by adding the default extension if none was provided
				string filename = saveFileDialog.FileName;
				if(!Path.HasExtension(filename) && !File.Exists(filename))
				{
					filename = Path.ChangeExtension(filename, saveFileDialog.DefaultExt);
				}
				FilePath = filename;

				Save(dialogOwner);
			}
		}

		private SaveFileDialog CreateSaveAsDialog()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.FileName = FilePath;
			saveFileDialog.DefaultExt = ImageFileDefaultExtension;
			saveFileDialog.AddExtension = true;
			saveFileDialog.OverwritePrompt = false; //That will be handled by Save();
			saveFileDialog.Filter = String.Format("Image Files ({0})|{0}|All Files|*.*", String.Join(";", (from ext in ImageFileExtensions select "*." + ext).ToArray()));
			saveFileDialog.ValidateNames = false;
			return saveFileDialog;
		}

		//Performs the actual save operation, as a result of the full size image retreival completing
		private void SaveInternal(string filePath)
		{
			System.Diagnostics.Debug.Assert(mIsFullSize, "Full size image was not retrieved");
			
			try
			{
				File.WriteAllBytes(filePath, BitmapData);
			}
			catch (Exception e)
			{
				MessageBox.Show(String.Format("Unexpected faliure saving image to: \"{0}\"\n\n{1}", filePath, e.Message), "Album Art Downloader", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			IsSaving = false;
			IsSaved = true;
		}
		#endregion

		#region Copy to Clipboard

		/// <summary>
		/// Begins an asynchronous Copy to Clipboard operation, including downloading the full size image.
		/// </summary>
		internal void CopyToClipboard()
		{
			RetrieveFullSizeImage(new WaitCallback(CopyToClipboardInternal));
		}

		private void CopyToClipboardInternal(object sender)
		{
			System.Diagnostics.Debug.Assert(mIsFullSize, "Full size image was not retrieved");
			System.Diagnostics.Debug.Assert(Image is BitmapSource, "Image is not a BitmapSource");

			try
			{
				Clipboard.SetImage((BitmapSource)Image);
			}
			catch (Exception e)
			{
				MessageBox.Show(String.Format("Unexpected faliure copying image to clipboard\n\n" + e.Message), "Album Art Downloader", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		#endregion

		/// <summary>
		/// Removes this result from the source that it belongs to.
		/// </summary>
		public void Remove()
		{
			mSource.Results.Remove(this);
		}
	}
}
