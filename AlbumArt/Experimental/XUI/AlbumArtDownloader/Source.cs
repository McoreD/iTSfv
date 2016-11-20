using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using AlbumArtDownloader.Controls;
using AlbumArtDownloader.Scripts;

namespace AlbumArtDownloader
{
	public abstract class Source : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler SearchCompleted;

		private ObservableCollectionOfDisposables<AlbumArt> mResults;
		private SourceSettings mSettings;
		private Control mCustomSettingsUI; 

		private Thread mSearchThread;

		public Source()
		{
			mResults = new ObservableCollectionOfDisposables<AlbumArt>();

			mCustomSettingsUI = CreateCustomSettingsUI();
			if(mCustomSettingsUI != null)
				mCustomSettingsUI.DataContext = this;
		}

		/// <summary>
		/// Override this to create the custom settings UI control to be displayed
		/// to allow editing of custom settings for the source.
		/// </summary>
		protected virtual System.Windows.Controls.Control CreateCustomSettingsUI()
		{
			return null;
		}

		public void LoadSettings()
		{
			mSettings = GetSettings();
			LoadSettingsInternal(mSettings);
		}

		public void SaveSettings()
		{
			SaveSettingsInternal(mSettings);
		}

		private bool mSettingsChanged = false;
		/// <summary>
		/// This flag is set true whenever a setting is changed, and
		/// reset to false when a search is performed. It can then be
		/// used to determine whether or not a new search is needed to
		/// be performed for this source (if the artist and album haven't
		/// changed).
		/// </summary>
		public bool SettingsChanged
		{
			get
			{
				return mSettingsChanged;
			}
			protected set
			{
				mSettingsChanged = value;
			}
		}

		/// <summary>
		/// Load the SourceSettings object for this source
		/// </summary>
		protected virtual SourceSettings GetSettings()
		{
			return GetSettingsCore(SourceSettings.Creator);
		}

		/// <summary>
		/// Load the SourceSettings object for this source, with the specified source settings creator
		/// </summary>
		protected SourceSettings GetSettingsCore(SourceSettingsCreator sourceSettingsCreator)
		{
			if (String.IsNullOrEmpty(Name))
				throw new InvalidOperationException("Cannot load settings for a source with no name");

			return ((App)Application.Current).GetSourceSettings(Name, sourceSettingsCreator);
		}

		/// <summary>
		/// Reads the values from the SourceSettings object
		/// </summary>
		/// <param name="settings"></param>
		protected virtual void LoadSettingsInternal(SourceSettings settings)
		{
			this.IsEnabled = settings.Enabled;
			this.UseMaximumResults = settings.UseMaximumResults;
			this.MaximumResults = settings.MaximumResults;
			this.IsPrimary = settings.IsPrimary;
			this.FullSizeOnly = settings.FullSizeOnly;
		}

		/// <summary>
		/// Saves the values to the SourceSettings object
		/// </summary>
		/// <param name="settings"></param>
		protected virtual void SaveSettingsInternal(SourceSettings settings)
		{
			settings.Enabled = this.IsEnabled;
			settings.UseMaximumResults = this.UseMaximumResults;
			settings.MaximumResults = this.MaximumResults;
			settings.IsPrimary = this.IsPrimary;
			settings.FullSizeOnly = this.FullSizeOnly;
		}

		#region Abstract members
		public abstract string Name {get;}
		public abstract string Author { get;}
		public abstract string Version { get;}

		/// <summary>
		/// Perform the actual internal searching operation
		/// This should not update any WPF controls, or
		/// perform any direct modification of property values.
		/// </summary>
		protected abstract void SearchInternal(string artist, string album, IScriptResults results);

		internal abstract Bitmap RetrieveFullSizeImage(object fullSizeCallbackParameter);
		#endregion

		#region Basic properties
		/// <summary>
		/// Null for no custom settings, or a control which should be displayed to allow custom
		/// settings for this source to be edited.
		/// </summary>
		public Control CustomSettingsUI
		{
			get
			{
				return mCustomSettingsUI;
			}
		}

		private bool mIsEnabled = true;
		public bool IsEnabled
		{
			get
			{
				return mIsEnabled;
			}
			set
			{
				if (mIsEnabled != value)
				{
					mIsEnabled = value;
					NotifyPropertyChanged("IsEnabled");

					if (!mIsEnabled && IsSearching)
					{
						AbortSearch();
					}
				}
			}
		}

		private bool mUseMaximumResults = true;
		public bool UseMaximumResults
		{
			get
			{
				return mUseMaximumResults;
			}
			set
			{
				if (mUseMaximumResults != value)
				{
					mUseMaximumResults = value;
					
					SettingsChanged = true;
					NotifyPropertyChanged("UseMaximumResults");
					if (UseMaximumResults && MaximumResults < EstimatedResultsCount)
						NotifyPropertyChanged("EstimatedResultsCount"); //This will be coerced to be no more than maximum results
				}
			}
		}

		private int mMaximumResults;
		public int MaximumResults
		{
			get
			{
				return mMaximumResults;
			}
			set
			{
				if (mMaximumResults != value)
				{
					mMaximumResults = value;

					SettingsChanged = true;
					NotifyPropertyChanged("MaximumResults");
					if(UseMaximumResults && MaximumResults < EstimatedResultsCount)
						NotifyPropertyChanged("EstimatedResultsCount"); //This will be coerced to be no more than maximum results
				}
			}
		}

		private bool mIsPrimary = false;
		public bool IsPrimary
		{
			get
			{
				return mIsPrimary;
			}
			set
			{
				if (mIsPrimary != value)
				{
					mIsPrimary = value;
					NotifyPropertyChanged("IsPrimary");
				}
			}
		}

		private bool mFullSizeOnly = false;
		public bool FullSizeOnly
		{
			get
			{
				return mFullSizeOnly;
			}
			set
			{
				if (mFullSizeOnly != value)
				{
					mFullSizeOnly = value;

					SettingsChanged = true;
					NotifyPropertyChanged("FullSizeOnly");
				}
			}
		}

		public ObservableCollection<AlbumArt> Results
		{
			get { return mResults; }
		}

		private bool mIsSearching;
		public bool IsSearching
		{
			get
			{
				return mIsSearching;
			}
			private set
			{
				mIsSearching = value;
				NotifyPropertyChanged("IsSearching");
			}
		}

		private int mEstimatedResultsCount;
		public int EstimatedResultsCount
		{
			get
			{
				//If there is a maximum set, then the estimated results count should never be more than that.
				if (UseMaximumResults && MaximumResults < mEstimatedResultsCount)
					return MaximumResults;

				return mEstimatedResultsCount;
			}
			private set
			{
				mEstimatedResultsCount = value;
				NotifyPropertyChanged("EstimatedResultsCount");
			}
		}

		protected void NotifyPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler temp = PropertyChanged;
			if (temp != null)
			{
				temp(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion

		/// <summary>
		/// Begins an asynchronous search. Results are added to the observable <see cref="Results"/> collection.
		/// </summary>
		public void Search(string artist, string album)
		{
			if (artist == null)
			{
				artist = String.Empty;
			}
			if (album == null)
			{
				album = String.Empty;
			}
			//Start a new search thread (which will take care of aborting the old one, and assigning itself as the current one)
			new Thread(new ParameterizedThreadStart(SearchWorker)) { Name = String.Format("{0} search", Name) }.Start(new SearchThreadParameters(Dispatcher.CurrentDispatcher, artist, album));
		}

		/// <summary>
		/// Abort the search without raising completion events,
		/// but waiting until the thread terminates synchronously.
		/// </summary>
		public void TerminateSearch() 
		{
			if (mSearchThread != null)
			{
				mSearchThread.Abort(true); //without completion
				mSearchThread.Join();
			}
		}
		
		/// <summary>
		/// Abort the search with raising completion events, asynchronously.
		/// </summary>
		public void AbortSearch()
		{
			if (mSearchThread != null)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(mSearchThread.Abort), false); //with completion
			}
		}

		private struct SearchThreadParameters
		{
			private Dispatcher mDispatcher;
			private string mArtist;
			private string mAlbum;
			public SearchThreadParameters(Dispatcher dispatcher, string artist, string album)
			{
				mDispatcher = dispatcher;
				mArtist = artist;
				mAlbum = album;
			}
			public Dispatcher Dispatcher { get { return mDispatcher; } }
			public string Artist { get { return mArtist; } }
			public string Album { get { return mAlbum; } }
		}

		private void SearchWorker(object state)
		{
			SearchThreadParameters parameters = (SearchThreadParameters)state;
			
			//Flag to indicate that the search thread should abort without raising completion events
			bool abortThreadWithoutCompletion = false;

			//If there is an existing search thread, terminate it
			TerminateSearch();
			//Become the new search thread
			mSearchThread = Thread.CurrentThread;
			//This can now be terminated itself, so catch abort exceptions
			try
			{
				SettingsChanged = false; //Clear search settings changed (flag is to indicate change since search started)

				parameters.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(delegate
				{
					Results.Clear();
					IsSearching = true;
				}));

				SearchInternal(parameters.Artist, parameters.Album, new ScriptResults(this, parameters.Dispatcher));
			}
			catch (ThreadAbortException e)
			{
				abortThreadWithoutCompletion = (e.ExceptionState as bool?).GetValueOrDefault();
			}
			finally
			{
				if (abortThreadWithoutCompletion)
				{
					//Don't use property setter, as no events should be raised.
					mIsSearching = false;
				}
				else
				{
					//Signal completion, at high priority, as this will hold up thread tear-down if it has to wait.
					parameters.Dispatcher.Invoke(DispatcherPriority.Send, new ThreadStart(delegate
					{
						IsSearching = false;
						RaiseSearchCompleted();
					}));
				}
			}
		}

		private void RaiseSearchCompleted()
		{
			EventHandler temp = SearchCompleted;
			if (temp != null)
			{
				temp(this, EventArgs.Empty);
			}
		}

		#region QueryContinue
		/// <summary>
		/// This event is raised before and after each search result is provided by the script.
		/// If set to Cancel, then the search will be aborted.
		/// It is called synchronously on the same thread as the search - the search will not
		/// continue until the event handler returns.
		/// </summary>
		public event CancelEventHandler QueryContinueSearch;

		/// <summary>
		/// This method is called before and after each search result is provided by the script.
		/// If it returns false, the search will be aborted.
		/// </summary>
		/// <returns>True to keep searching, False to abort.</returns>
		private bool QueryContinueSearchInternal()
		{
			if (UseMaximumResults && Results.Count >= MaximumResults)
			{
				return false; //Abort due to reaching maximum result count
			}

			CancelEventHandler temp = QueryContinueSearch;
			if (temp != null)
			{
				var e = new CancelEventArgs();
				temp(this, e);

				if (e.Cancel)
				{
					return false; //Abort due to event handler requesting cancellation
				}
			}
			return true; //Do not abort - keep searching
		}

		#endregion


		private class ScriptResults : IScriptResults
		{
			private Source mSource;
			private Dispatcher mDispatcher;

			public ScriptResults(Source source, Dispatcher dispatcher)
			{
				mSource = source;
				mDispatcher = dispatcher;
			}

			#region Redirects for obsolete members
			//This region can be copied and pasted for reuse
			public void SetCountEstimate(int count)
			{
				EstimatedCount = count;
			}
			public void AddThumb(string thumbnailUri, string name, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
			{
				Add(thumbnailUri, name, fullSizeImageWidth, fullSizeImageHeight, fullSizeImageCallback);
			}
			public void AddThumb(System.IO.Stream thumbnailStream, string name, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
			{
				Add(thumbnailStream, name, fullSizeImageWidth, fullSizeImageHeight, fullSizeImageCallback);
			}
			public void AddThumb(System.Drawing.Image thumbnailImage, string name, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
			{
				Add(thumbnailImage, name, fullSizeImageWidth, fullSizeImageHeight, fullSizeImageCallback);
			}
			#endregion

			public int EstimatedCount
			{
				get
				{
					return mSource.EstimatedResultsCount;
				}
				set
				{
					mDispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(delegate
					{
						mSource.EstimatedResultsCount = value;
					}));
				}
			}

			public void Add(object thumbnail, string name, object fullSizeImageCallback)
			{
				Add(thumbnail, name, -1, -1, fullSizeImageCallback);
			}
			public void Add(object thumbnail, string name, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
			{
				Add(thumbnail, name, String.Empty, fullSizeImageWidth, fullSizeImageHeight, fullSizeImageCallback);
			}
			public void Add(object thumbnail, string name, string infoUri, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
			{
				Add(thumbnail, name, infoUri, fullSizeImageWidth, fullSizeImageHeight, fullSizeImageCallback, CoverType.Unknown);
			}
			public void Add(object thumbnail, string name, object fullSizeImageCallback, CoverType coverType)
			{
				Add(thumbnail, name, String.Empty, -1, -1, fullSizeImageCallback, coverType);
			}
			
			public void Add(object thumbnail, string name, string infoUri, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback, CoverType coverType)
			{
				if (!mSource.QueryContinueSearchInternal())
				{
					//Break out of this search
					mSource.AbortSearch();
					return;
				}

				if (mSource.FullSizeOnly || fullSizeImageCallback == null)
				{
					Bitmap fullSize = null;
					if (fullSizeImageCallback != null) //If fullSizeImageCallback == null, then no full size image supplied, so the thumbnail is the full size image
					{
						//Try to get the full size image without getting the thumbnail
						fullSize = mSource.RetrieveFullSizeImage(fullSizeImageCallback);
					}
					if (fullSize == null)
					{
						//Fall back on using the thumbnail as the full size
						fullSize = BitmapHelpers.GetBitmap(thumbnail);
					}
					if (fullSize != null)
					{
						mDispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
						{
							mSource.Results.Add(new AlbumArt(mSource,
								name,
								infoUri,
								fullSize,
								coverType));
						}));
					}
				}
				else
				{
					Bitmap thumbnailBitmap = BitmapHelpers.GetBitmap(thumbnail);

					if (thumbnailBitmap != null)
					{
						mDispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
						{
							mSource.Results.Add(new AlbumArt(mSource,
								thumbnailBitmap,
								name,
								infoUri,
								fullSizeImageWidth,
								fullSizeImageHeight,
								fullSizeImageCallback,
								coverType));
						}));
					}
				}

				if (!mSource.QueryContinueSearchInternal())
				{
					//Break out of this search
					mSource.AbortSearch();
					return;
				}
			}
		}
	}
}
