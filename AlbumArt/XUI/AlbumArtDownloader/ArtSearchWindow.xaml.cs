using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.Linq;
using AlbumArtDownloader.Controls;
using AlbumArtDownloader.Scripts;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.Windows.Documents;


namespace AlbumArtDownloader
{
	public partial class ArtSearchWindow : System.Windows.Window, INotifyPropertyChanged, IAppWindow
	{
		public static class Commands
		{
			public static RoutedUICommand GetMoreScripts = new RoutedUICommand("GetMoreScripts", "GetMoreScripts", typeof(Commands));
			public static RoutedUICommand ShowAutoDownloadedScripts = new RoutedUICommand("ShowAutoDownloadedScripts", "ShowAutoDownloadedScripts", typeof(Commands));
			public static RoutedUICommand SaveAsPreset = new RoutedUICommand("SaveAsPreset", "SaveAsPreset", typeof(Commands));
		}

		private Sources mSources = new Sources();

		private Thread mAutoDownloadFullSizeImagesThread;
		private AutoResetEvent mAutoDownloadFullSizeImagesTrigger = new AutoResetEvent(true);
		private Queue<AlbumArt> mResultsToAutoDownloadFullSizeImages = new Queue<AlbumArt>();
		private CommandBinding mStopAllCommandBinding;
		private bool mPreventSecondarySearch = false;

		public ArtSearchWindow()
		{
			mAutoDownloadFullSizeImagesThread = new Thread(new ThreadStart(AutoDownloadFullSizeImagesWorker));
			mAutoDownloadFullSizeImagesThread.Name = "Auto Download Full Size Images";
			mAutoDownloadFullSizeImagesThread.Priority = ThreadPriority.BelowNormal;

			InitializeComponent();

			//Bind the SelectAll checkbox
			Binding selectAllBinding = new Binding("AllEnabled");
			selectAllBinding.Source = mSources;
			selectAllBinding.Mode = BindingMode.TwoWay;
			BindingOperations.SetBinding(mSelectAll, CheckBox.IsCheckedProperty, selectAllBinding);

			//Bind the Search button being enabled
			Binding sourceEnabledBinding = new Binding("AllEnabled");
			sourceEnabledBinding.Source = mSources;
			sourceEnabledBinding.Mode = BindingMode.OneWay;
			sourceEnabledBinding.Converter = new NotFalseConverter();
			BindingOperations.SetBinding(mSearch, Button.IsEnabledProperty, sourceEnabledBinding);

			mSources.CombinedResults.CollectionChanged += new NotifyCollectionChangedEventHandler(OnResultsChanged);

			//Commands:
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, new ExecutedRoutedEventHandler(FindExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, new ExecutedRoutedEventHandler(CopyExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, new ExecutedRoutedEventHandler(SaveExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, new ExecutedRoutedEventHandler(SaveAsExec)));
			CommandBindings.Add(new CommandBinding(Commands.SaveAsPreset, new ExecutedRoutedEventHandler(SaveAsPresetExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, new ExecutedRoutedEventHandler(DeleteExec)));
			CommandBindings.Add(new CommandBinding(CommonCommands.Preview, new ExecutedRoutedEventHandler(PreviewExec)));
			CommandBindings.Add(new CommandBinding(Commands.GetMoreScripts, new ExecutedRoutedEventHandler(GetMoreScriptsExec), new CanExecuteRoutedEventHandler(GetMoreScriptsCanExec)));
			CommandBindings.Add(new CommandBinding(Commands.ShowAutoDownloadedScripts, new ExecutedRoutedEventHandler(ShowAutoDownloadedScriptsExec)));

			//Stop All is bound only when doing a search (so the Stop All button only appears while searching)
			mStopAllCommandBinding = new CommandBinding(ApplicationCommands.Stop, new ExecutedRoutedEventHandler(StopExec));
			
			mSourcesViewer.ItemsSource = mSources;
			mResultsViewer.ItemsSource = mSources.CombinedResults;

			mSourcesViewer.Items.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
			mSourcesViewer.Items.SortDescriptions.Add(new SortDescription("Category", ListSortDirection.Ascending));
			mSourcesViewer.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

			foreach (IScript script in ((App)Application.Current).Scripts)
			{
				ScriptSource scriptSource = new ScriptSource(script);
				mSources.Add(scriptSource);
				scriptSource.HighlightResults += OnSourceHighlightResults;
				//Hook the complete event to know when to hide the Stop All button
				scriptSource.SearchCompleted += OnSourceSearchCompleted;
				//Hook the Property Changed event to know when to recalculate whether the search will be extended
				scriptSource.PropertyChanged += OnSourcePropertyChanged;
			}

			LocalFilesSource localFilesSource = new LocalFilesSource();
			localFilesSource.DefaultFilePath = mDefaultSaveFolder.PathPattern;
			//Update the default file path if the pattern changes
			mDefaultSaveFolder.PathPatternChanged += delegate(object sender, DependencyPropertyChangedEventArgs e) { localFilesSource.DefaultFilePath = (string)e.NewValue; };
			mSources.Add(localFilesSource);
			localFilesSource.SearchCompleted += OnSourceSearchCompleted;
			localFilesSource.PropertyChanged += OnSourcePropertyChanged;

			LoadSettings();
			//Initial value of AutoClose is taken from settings. May be overriden by command line parameters
			AutoClose = Properties.Settings.Default.AutoClose;

			this.Loaded += new RoutedEventHandler(OnLoaded);

			//Tab auto-select all for Artist and Album boxes
			mArtist.GotKeyboardFocus += OnAutoSelectTextBoxFocusChange;
			mAlbum.GotKeyboardFocus += OnAutoSelectTextBoxFocusChange;

			// Always auto-select all for Invalid Replacement Character box
			mInvalidReplacementCharacter.GotKeyboardFocus += OnAutoSelectWithClickTextBoxFocusChange;

			//If the default file path pattern does not containg %preset%, the presets context menu is coerced into being hidden
			mDefaultSaveFolder.PathPatternChanged += delegate { CoerceValue(PresetsContextMenuProperty); };

			//Notify when scripts have been auto downloaded
			Updates.AutoDownloadedScripts.CollectionChanged += OnAutoDownloadedScriptsChanged;
		}

		private void OnAutoDownloadedScriptsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			NotifyPropertyChanged("AutoDownloadedScriptsPresent");
		}

		private void OnAutoSelectTextBoxFocusChange(object sender, KeyboardFocusChangedEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			//If the textbox gains focus, but not from app gaining focus, and not from mouse press or context menu, then select its contents
			if (textBox != null && e.OldFocus != null && !BelongsToContextMenu(e.OldFocus as FrameworkElement) && Mouse.LeftButton != MouseButtonState.Pressed)
			{
				textBox.SelectAll();
			}
		}

		private void OnAutoSelectWithClickTextBoxFocusChange(object sender, KeyboardFocusChangedEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			//If the textbox gains focus, but not from app gaining focus or context menu, then select its contents
			if (textBox != null && e.OldFocus != null && !BelongsToContextMenu(e.OldFocus as FrameworkElement))
			{
				textBox.SelectAll();
			}
		}

		private static bool BelongsToContextMenu(FrameworkElement element)
		{
			while (element != null)
			{
				if (element is ContextMenu)
				{
					return true;
				}
				element = element.Parent as FrameworkElement;
			}
			return false;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			//Assign sensible defaults to things that depend on layout
			if (Properties.Settings.Default.PanelWidth < 0)
			{
				Properties.Settings.Default.PanelWidth = mResultsViewer.ActualWidth / 2;
				mResultsViewer.AutoSizePanels();
			}
		}
		
		#region Auto Download Full Size Images
		private AutoResetEvent mWaitForImage = new AutoResetEvent(false);

		/// <summary>
		/// Thread worker for downloading full size images
		/// </summary>
		private void AutoDownloadFullSizeImagesWorker()
		{
			do
			{
				mAutoDownloadFullSizeImagesTrigger.WaitOne(); //Wait until something has changed to look at

				while (Properties.Settings.Default.AutoDownloadFullSizeImages != AutoDownloadFullSizeImages.Never)
				{
					AlbumArt resultToProcess;
					int queueCount;
					lock (mResultsToAutoDownloadFullSizeImages)
					{
						queueCount = mResultsToAutoDownloadFullSizeImages.Count;
						if (queueCount == 0)
						{
							break; //Finished downloading all the pending results to download, so skip back round to the waiting.
						}
						else
						{
							resultToProcess = mResultsToAutoDownloadFullSizeImages.Dequeue();
						}
					}
					if (!resultToProcess.IsDownloading && !resultToProcess.IsFullSize && 
							(
								Properties.Settings.Default.AutoDownloadFullSizeImages == AutoDownloadFullSizeImages.Always ||
								(Properties.Settings.Default.AutoDownloadFullSizeImages == AutoDownloadFullSizeImages.WhenSizeUnknown && resultToProcess.ImageWidth == -1 && resultToProcess.ImageHeight == -1)
							)
						)
					{
						//If the queue length has increased, then update the maximum to be the new queue length.
						mAutoDownloadingProgressMaximum = Math.Max(mAutoDownloadingProgressMaximum, queueCount);
						//The progress is reset to be counted from the end, so if there are 5 items remaining, the progress should be 5 points from the end. (so if new items keep getting added, the progress will look static)
						mAutoDownloadingProgressValue = mAutoDownloadingProgressMaximum - queueCount;
						NotifyChangeOfAutoDownloadingProgress();

						resultToProcess.RetrieveFullSizeImage(mWaitForImage);
						//Wait until it is finished to move on to the next one, which triggers the trigger.
						mWaitForImage.WaitOne();

						mAutoDownloadingProgressValue++;
						NotifyChangeOfAutoDownloadingProgress();
					}
				}

				//Queue has been completed, so hide the progress bar.
				if (mAutoDownloadingProgressMaximum > 0)
				{
					mAutoDownloadingProgressValue = mAutoDownloadingProgressMaximum = 0;
					NotifyChangeOfAutoDownloadingProgress();
				}
							
			} while (true);
		}

		//Public members for binding the auto downloading progress bar to:
		private int mAutoDownloadingProgressMaximum;
		public int AutoDownloadingProgressMaximum { get { return mAutoDownloadingProgressMaximum; } }

		private int mAutoDownloadingProgressValue;
		public int AutoDownloadingProgressValue { get { return mAutoDownloadingProgressValue; } }

		private volatile bool mNotifyChangeOfAutoDownloadingProgressPending;
		private void NotifyChangeOfAutoDownloadingProgress()
		{
			//Don't queue up a whole bunch of notifiactions - once one is pended, ignore further notifications until that one completes
			if (!mNotifyChangeOfAutoDownloadingProgressPending)
			{
				mNotifyChangeOfAutoDownloadingProgressPending = true;

				//The auto downloading progress variables have changed, so add an Update to the dispatcher (low priority)
				Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
				{
					NotifyPropertyChanged("AutoDownloadingProgressMaximum");
					NotifyPropertyChanged("AutoDownloadingProgressValue");
					mNotifyChangeOfAutoDownloadingProgressPending = false;
				}));
			}
		}

		private void OnAutoDownloadFullSizeImagesChanged(object sender, RoutedEventArgs e)
		{
			if (Properties.Settings.Default.AutoDownloadFullSizeImages != AutoDownloadFullSizeImages.Never &&
				mAutoDownloadFullSizeImagesThread.ThreadState == ThreadState.Unstarted)
			{
				mAutoDownloadFullSizeImagesThread.Start();
			}

			if (Properties.Settings.Default.AutoDownloadFullSizeImages == AutoDownloadFullSizeImages.Always)
			{
				//Re-queue all images, to ensure they all get a chance at being auto-downloaded, even if they were skipped from having had a non-unknown size before.
				lock (mResultsToAutoDownloadFullSizeImages)
				{
					mResultsToAutoDownloadFullSizeImages.Clear();
					foreach (AlbumArt albumArt in mSources.CombinedResults)
					{
						mResultsToAutoDownloadFullSizeImages.Enqueue(albumArt);
					}
				}
			}
			mAutoDownloadFullSizeImagesTrigger.Set();
		}

		private void AddResultToAutoDownloadFullSizeImage(AlbumArt result)
		{
			if (!result.IsDownloading && !result.IsFullSize) //No need to auto-download if it is already downloading, or full sized.
			{
				lock (mResultsToAutoDownloadFullSizeImages)
				{
					mResultsToAutoDownloadFullSizeImages.Enqueue(result);
				}
				mAutoDownloadFullSizeImagesTrigger.Set();
			}
		}

		private void ClearAutoDownloadFullSizeImageResults()
		{
			lock (mResultsToAutoDownloadFullSizeImages)
			{
				mResultsToAutoDownloadFullSizeImages.Clear();
			}
		}


		#endregion

		#region Searching

		public static readonly DependencyProperty ArtistProperty = DependencyProperty.Register("Artist", typeof(string), typeof(ArtSearchWindow), new FrameworkPropertyMetadata(String.Empty, new PropertyChangedCallback(OnSearchParameterChanged)));
		public string Artist
		{
			get { return (string)GetValue(ArtistProperty); }
			set { SetValue(ArtistProperty, value); }
		}

		public static readonly DependencyProperty AlbumProperty = DependencyProperty.Register("Album", typeof(string), typeof(ArtSearchWindow), new FrameworkPropertyMetadata(String.Empty, new PropertyChangedCallback(OnSearchParameterChanged)));
		public string Album
		{
			get { return (string)GetValue(AlbumProperty); }
			set { SetValue(AlbumProperty, value); }
		}

		private static void OnSearchParameterChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			((ArtSearchWindow)sender).UpdateExtendSearch();
		}

		private SearchParameters mSearchParameters;
		private void FindExec(object sender, RoutedEventArgs e)
		{
			mPreventSecondarySearch = false;

			//Check to see whether current results can be extended, or a new search is required
			if (mSearchParameters != null)
			{
				//Can only extend if the artist and album to search for are identical
				if (Artist == mSearchParameters.Artist && Album == mSearchParameters.Album)
				{
					if (AlterSearch())
					{
						//If the search was successfully altered, no further work to be done
						return;
					}
					//Otherwise, if no alteration could be made, re-perform the search
					//TODO: Anything more sensible that could be done instead?
				}
			}
			if (mSources.CombinedResults.Count > 0 && Properties.Settings.Default.OpenResultsInNewWindow)
			{
				Common.NewSearchWindow(this).Search(Artist, Album);
			}
			else
			{
				StartSearch();
			}
		}

		/// <summary>
		/// Perform a search with the specified settings.
		/// If the window is held in a queue, defer the search until the window is shown.
		/// </summary>
		public void Search(string artist, string album)
		{
			Artist = artist;
			Album = album;
			if (IsVisible)
			{
				StartSearch();
			}
			else
			{
				this.IsVisibleChanged += SearchOnShown;
			}
		}

		private void SearchOnShown(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue) //If this window has just been shown
			{
				//Stop listening to the event
				this.IsVisibleChanged -= SearchOnShown;
				//and kick off the search
				StartSearch();
			}
		}

		/// <summary>
		/// Starts a search with the current settings
		/// </summary>
		private void StartSearch()
		{
			mPreventSecondarySearch = false; //New search, so allow secondaries
			
			mSearchParameters = new SearchParameters(Artist, Album);

			ProgressTotalSources = 0;

			mDefaultSaveFolder.AddPatternToHistory();
			//Check for no primary sources (means that every source is primary!)
			bool noSourceIsPrimary = !mSources.Any(s => s.IsPrimary && s.IsEnabled);
			foreach (Source source in mSources)
			{
				if (source.IsEnabled && (noSourceIsPrimary || source.IsPrimary)) //If the source is primary, or no sources are set to primary, include this source.
				{
					ProgressSourcesSearching++;
					ProgressTotalSources++;
					source.Search(mSearchParameters.Artist, mSearchParameters.Album);
					mSearchParameters.AddSource(source);
				}
				else
				{
					source.AbortSearch();
					source.Results.Clear();
				}
			}

			if (!CommandBindings.Contains(mStopAllCommandBinding))
			{
				CommandBindings.Add(mStopAllCommandBinding);
			}
		}

		/// <summary>
		/// Alters the existing search to include or exclude additional sources
		/// </summary>
		private bool AlterSearch()
		{
			bool searchPerformed = false;
			bool sourcesRemoved = false;
			foreach (Source source in mSources)
			{
				if (source.IsEnabled) //Inlcudes non-primary sources too.
				{
					bool performSearch = false;
					//Perform the search if the source was not previously searched
					if (!mSearchParameters.ContainsSource(source))
					{
						performSearch = true;
					}
					else
					{
						//Repeat the search if the maximum results settings for the source have changed.
						performSearch = source.SettingsChanged;
					}
					if(performSearch)
					{
						searchPerformed = true;

						ProgressSourcesSearching++;
						ProgressTotalSources++;
					
						source.Search(Artist, Album);

						mSearchParameters.AddSource(source);
					}
				}
				else
				{
					//Remove the results if the source was unselected
					source.AbortSearch();
					source.Results.Clear();

					if (mSearchParameters.RemoveSource(source))
					{
						sourcesRemoved = true;
					}
				}
			}
			if (searchPerformed && !CommandBindings.Contains(mStopAllCommandBinding))
			{
				CommandBindings.Add(mStopAllCommandBinding);
			}

			return searchPerformed || sourcesRemoved;
		}

		/// <summary>Cached value for the ExtendSearch property</summary>
		private bool? mExtendSearch;
		/// <summary>
		/// If true, when the Search button is clicked it will extend the existing search by adding new
		/// results from more sources, rather than performing a new search.
		/// </summary>
		public bool ExtendSearch
		{
			get
			{
				if (mExtendSearch.HasValue)
				{
					//Return the cached value
					return mExtendSearch.Value;
				}

				//Check to see whether current results can be extended, or a new search is required
				if (mSearchParameters == null)
				{
					//No current search, so search is New.
					return false;
				}
				//Can only extend if the artist and album to search for are identical
				if (!String.Equals(Artist, mSearchParameters.Artist, StringComparison.Ordinal) ||
					!String.Equals(Album, mSearchParameters.Album, StringComparison.Ordinal))
				{
					//Search terms mismatch, so search is New.
					return false;
				}
				//Check if there are any new sources to be searched
				foreach (Source source in mSources)
				{
					if (source.IsEnabled && //Inlcudes non-primary sources too.
						(!mSearchParameters.ContainsSource(source) ||  //The source was not previously searched (may have been newly selected, or may be due to Search First behaviour)
						 source.SettingsChanged)) //The source must be re-searched as its settings changed
					{
						return true;
					}
				}
				return false; //No new sources will be searched, so search won't be extended.
			}
		}

		private void UpdateExtendSearch()
		{
			mExtendSearch = null; //Clear the cached value
			NotifyPropertyChanged("ExtendSearch"); //Cause the property to be re-queried
		}

		private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			//If a source has changed (become enabled or disabled, or had its settings changed) then this may indicate that search will now be (or no longer be) extended
			UpdateExtendSearch();
		}

		private int mProgressTotalSources = 0;
		private int ProgressTotalSources
		{
			get { return mProgressTotalSources; }
			set
			{
				mProgressTotalSources = value;
				UpdateOverallSourcesProgress();
			}
		}

		private int mProgressSourcesSearching = 0;
		private int ProgressSourcesSearching
		{
			get { return mProgressSourcesSearching; }
			set
			{
				mProgressSourcesSearching = value;
				UpdateOverallSourcesProgress();
			}
		}

		private void UpdateOverallSourcesProgress()
		{
			if (TaskbarManager.IsPlatformSupported)
			{
				if (ProgressSourcesSearching == 0 && ProgressTotalSources == 0)
				{
					// Delay half a second before clearing the progress, so it looks like it's completed.
					TaskbarHelper.DelayedClearProgress(this);
				}
				else
				{
					int maximum = ProgressTotalSources;
					int current = Math.Max(0, Math.Min(maximum, maximum - ProgressSourcesSearching));
					System.Diagnostics.Debug.WriteLine(String.Format("Sources progress: {0} / {1}", current, maximum));

					TaskbarManager.Instance.SetProgressValue(current, maximum, this);
				}
			}
		}

		#endregion

		#region Results Updated
		private void OnResultsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Replace:
					foreach (AlbumArt art in e.NewItems)
					{
						BindAlbumArtProperties(art);
						AddResultToAutoDownloadFullSizeImage(art);
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					ClearAutoDownloadFullSizeImageResults();
					break;
			}
		}

		private void BindAlbumArtProperties(AlbumArt art)
		{
			var pathPatternBinding = new Binding
			{
				Source = mDefaultSaveFolder,
				Path = new PropertyPath(ArtPathPatternBox.PathPatternProperty),
				Mode = BindingMode.OneWay
			};

			var replacementCharacterBinding = new Binding
			{
				Source = mInvalidReplacementCharacter,
				Path = new PropertyPath(TextBox.TextProperty),
				Mode = BindingMode.OneWay
			};

			var defaultPathBinding = new MultiBinding()
			{
				Converter = new AlbumArtDefaultFilePathPatternSubstitution(),
				ConverterParameter = new string[] { Artist, Album }
			};
			defaultPathBinding.Bindings.Add(pathPatternBinding);
			defaultPathBinding.Bindings.Add(replacementCharacterBinding);

			BindingOperations.SetBinding(art, AlbumArt.DefaultFilePathPatternProperty, defaultPathBinding);

			var defaultPresetBinding = new Binding
			{
				Source = this,
				Path = new PropertyPath(DefaultPresetValueProperty),
				Mode = BindingMode.OneWay
			};
			BindingOperations.SetBinding(art, AlbumArt.DefaultPresetValueProperty, defaultPresetBinding);
		}

		private class AlbumArtDefaultFilePathPatternSubstitution : IMultiValueConverter
		{
			public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				string[] parameters = (string[])parameter;
				return ((string)values[0]).Replace("%artist%", Common.MakeSafeForPath(parameters[0]))
										  .Replace("%album%", Common.MakeSafeForPath(parameters[1]));
			}

			public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
			{
				return null; //Not used
			}
		}
		#endregion

		#region Closing
		private bool mAutoClose;
		/// <summary>
		/// If set true, the window will automatically be closed after a save (but not save as) operation
		/// To set AutoClose behaviour without saving the setting, use <see cref="OverrideAutoClose"/>.
		/// </summary>
		public bool AutoClose
		{
			get 
			{
				return mAutoClose; 
			}
			set 
			{
				//Assigns the value to the settings too
				Properties.Settings.Default.AutoClose = value;
				OverrideAutoClose(value);
			}
		}

		/// <summary>
		/// Sets the <see cref="AutoClose"/> behaviour without persisting the value in Settings.
		/// </summary>
		/// <param name="value"></param>
		public void OverrideAutoClose(bool value)
		{
			if (mAutoClose != value)
			{
				mAutoClose = value;
				NotifyPropertyChanged("AutoClose");
			}
		}
		private void AutoCloseOnSave(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsSaved" && ((AlbumArt)sender).IsSaved)
			{
				Close();
			}
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);

			SaveSettings();
		}

		protected override void OnClosed(EventArgs e)
		{
			//Unhook script download notification
			Updates.AutoDownloadedScripts.CollectionChanged -= OnAutoDownloadedScriptsChanged;

			string winName = ((IAppWindow)this).Description;

			//Tear down all the threads in the background
			if (!ThreadPool.QueueUserWorkItem(TerminateThreads, winName))
			{
				System.Diagnostics.Trace.TraceError("Failed to queue thread termination work item - terminating synchronously");
				//Attempt to tear down synchronously
				TerminateThreads(winName);
			}

			//HACK: Absolutely outrageous slimy hack that stops other open windows becoming unresponsive for no good reason.
			new System.Windows.Controls.Primitives.Popup() { Width = 0, Height = 0, IsOpen = true }.IsOpen = false;
			
			base.OnClosed(e);
		}

		private void TerminateThreads(object state)
		{
			string winName = state as string ?? String.Empty;
			System.Diagnostics.Trace.TraceInformation("Starting thread tear-down for " + winName);

			if (mAutoDownloadFullSizeImagesThread.ThreadState != ThreadState.Unstarted)
			{
				mAutoDownloadFullSizeImagesThread.Abort();
				mAutoDownloadFullSizeImagesThread.Join();
			}
			foreach (Source source in mSources)
			{
				source.TerminateSearch();
			}

			System.Diagnostics.Trace.TraceInformation("Finished thread tear-down for " + winName);
		}

		#endregion

		#region Source Settings
		private void LoadSourceSettings()
		{
			foreach (Source source in mSources)
			{
				source.LoadSettings();
			}
		}
		private void SaveSourceSettings()
		{
			foreach (Source source in mSources)
			{
				source.SaveSettings();
			}
		}
		/// <summary>
		/// Sets the LocalFilesSource image search path.
		/// </summary>
		/// <param name="path"></param>
		public void SetLocalImagesPath(string path)
		{
			//TODO: Could there ever be more than one local files source?
			LocalFilesSource localFilesSource = mSources.OfType<LocalFilesSource>().FirstOrDefault();
			
			if (localFilesSource != null)
			{
				localFilesSource.SearchPathPattern = path;
			}
		}

		public void OnSourceHighlightResults(object sender, EventArgs e)
		{
			var source = (Source)sender;
			if (source.Results.Any())
			{
				var highlighters = new List<Adorner>();
				var adornerLayer = AdornerLayer.GetAdornerLayer(mResultsViewer);

				bool first = true;
				foreach (var result in source.Results)
				{
					var container = mResultsViewer.ItemContainerGenerator.ContainerFromItem(result) as FrameworkElement;
					if (container != null)
					{
						if (first)
						{
							first = false;
							container.BringIntoView();
						}

						var highlighter = new HighlightResultAdorner(container);
						highlighters.Add(highlighter);
						adornerLayer.Add(highlighter);
					}
				}
				DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.DataBind) { Interval = TimeSpan.FromSeconds(2) };
				timer.Tick += new EventHandler(delegate
				{
					timer.Stop();
					foreach (var highlighter in highlighters)
					{
						adornerLayer.Remove(highlighter);
					}
				});
				timer.Start();
			}
		}
		#endregion

		#region Disabling Sources
		/// <summary>
		/// Disable all sources except those specified. Source names prefixed with F: will be marked as Search First (Primary), all others will not.
		/// </summary>
		public void UseSources(IEnumerable<string> useSources)
		{
			//Check for the special case of the "all" parameter here.
			if (useSources.Count() == 1 && useSources.First().Equals("all", StringComparison.OrdinalIgnoreCase))
			{
				//Select all sources
				foreach (Source source in mSources)
				{
					source.IsEnabled = true;
				}
			}
			else
			{
				foreach (Source source in mSources) //Go through all the sources
				{
					source.IsEnabled = false; //Disabled unless it's name matches
					foreach (string useSource in useSources) //Check against the list of sources to use
					{
						string sourceName; bool primary;
						ParseUseSource(useSource, out sourceName, out primary);

						//Use a case insensitive check
						if (source.Name.Equals(sourceName, StringComparison.InvariantCultureIgnoreCase))
						{
							//The source name matches, so use it. Enable it, and stop checking names.
							source.IsEnabled = true;
							source.IsPrimary = primary;
							break;
						}
					}
				}
			}
		}
		/// <summary>
		/// Disable the specified sources
		/// </summary>
		/// <param name="useSources"></param>
		public void ExcludeSources(IEnumerable<string> excludeSources)
		{
			SetSources(excludeSources, false);
		}
		/// <summary>
		/// Enable the specified sources
		/// </summary>
		/// <param name="useSources"></param>
		public void IncludeSources(IEnumerable<string> includeSources)
		{
			SetSources(includeSources, true);
		}
		private void SetSources(IEnumerable<string> useSources, bool enabled)
		{
			foreach (Source source in mSources) //Go through all the sources
			{
				foreach (string useSource in useSources) //Check against the list of sources to use
				{
					string sourceName; bool primary;
					ParseUseSource(useSource, out sourceName, out primary);

					//Use a case insensitive check
					if (source.Name.Equals(sourceName, StringComparison.InvariantCultureIgnoreCase))
					{
						//The source name matches, so disable the source, and stop checking names
						source.IsEnabled = enabled;

						//If the source hasn't been disabled, and has been prefixed with f:, then mark it as primary. Don't unmark existing sources if not specified with f:
						if (enabled && primary)
						{
							source.IsPrimary = true;
						}
						break;
					}
				}
			}
		}
		private static void ParseUseSource(string useSource, out string sourceName, out bool primary)
		{
			if (useSource.StartsWith("f:", StringComparison.OrdinalIgnoreCase))
			{
				sourceName = useSource.Substring(2); //Strip off the f: part
				primary = true;
			}
			else
			{
				sourceName = useSource;
				primary = false;
			}
		}
		#endregion

		#region Default Save Folder
		/// <summary>
		/// If true, the default save path has been temporarily fixed, and can not be modified from this window.
		/// </summary>
		private bool mDefaultSavePathIsTemporary;
		
		private void LoadDefaultSaveFolderHistory()
		{
			mDefaultSaveFolder.History.Clear();
			foreach (string historyItem in Properties.Settings.Default.DefaultSavePathHistory)
			{
				mDefaultSaveFolder.History.Add(historyItem);
			}
			mDefaultSaveFolder.PathPattern = Properties.Settings.Default.DefaultSavePath;
		}
		private void SaveDefaultSaveFolderHistory()
		{
			if (!mDefaultSavePathIsTemporary)
			{
				//Only save the default path if it isn't a temporary one
				Properties.Settings.Default.DefaultSavePath = mDefaultSaveFolder.PathPattern;
			}
			Properties.Settings.Default.DefaultSavePathHistory.Clear();
			foreach (string historyItem in mDefaultSaveFolder.History)
			{
				Properties.Settings.Default.DefaultSavePathHistory.Add(historyItem);
			}
		}
		/// <summary>
		/// Sets the default save folder pattern, optionally on a temporary basis so it won't be saved
		/// as the default in the settings.
		/// </summary>
		public void SetDefaultSaveFolderPattern(string path)
		{
			SetDefaultSaveFolderPattern(path, false);
		}
		public void SetDefaultSaveFolderPattern(string path, bool temporary)
		{
			mDefaultSavePathIsTemporary = temporary;
			if (mDefaultSavePathIsTemporary)
			{
				mNormalSaveFolderControls.Visibility = Visibility.Collapsed;
				mReadOnlySaveFolderControls.Visibility = Visibility.Visible;
			}
			else
			{
				mNormalSaveFolderControls.Visibility = Visibility.Visible;
				mReadOnlySaveFolderControls.Visibility = Visibility.Collapsed;
			}
			
			mDefaultSaveFolder.AddPatternToHistory(); //Save the previous value
			mDefaultSaveFolder.PathPattern = path; //Set the new value
		}
		/// <summary>
		/// Gets the default save folder pattern, with a flag to indicate whether it was set on
		/// a temporary basis by <see cref="SetDefaultSaveFolderPattern"/>
		/// </summary>
		public string GetDefaultSaveFolderPattern(out bool isTemporary)
		{
			isTemporary = mDefaultSavePathIsTemporary;
			return mDefaultSaveFolder.PathPattern;
		}
		#endregion

		#region Stop All
		private void OnSourceSearchCompleted(object sender, EventArgs e)
		{
			ProgressSourcesSearching--;

			//Check to see if any sources are still searching
			foreach (Source source in mSources)
			{
				if (source != sender && source.IsSearching)
					return; //At least one source is still searching, so don't remove the Stop All command
			}

			//If no results were found, perform an additional search to bring in any non-primary sources (if any)
			if (!mPreventSecondarySearch && mResultsViewer.Items.Count == 0 && mSources.Any(s => s.IsPrimary && s.IsEnabled))
			{
				mPreventSecondarySearch = true; //Don't 'tertiary' search!
				AlterSearch();
			}
			else
			{
				ProgressSourcesSearching = 0;
				ProgressTotalSources = 0;

				CommandBindings.Remove(mStopAllCommandBinding);
				CommandManager.InvalidateRequerySuggested();
			}
		}
		private void StopExec(object sender, ExecutedRoutedEventArgs e)
		{
			mPreventSecondarySearch = true; //Don't secondary search if stop is pressed during primary
			//Stop all the sources
			foreach (Source source in mSources)
			{
				source.AbortSearch();
			}
		}
		#endregion

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

		#region Command Execution
		private void CopyExec(object sender, ExecutedRoutedEventArgs e)
		{
			AlbumArt albumArt = mResultsViewer.GetSourceAlbumArt(e);
			if (albumArt != null)
			{
				albumArt.CopyToClipboard();
			}
		}

		private void SaveExec(object sender, ExecutedRoutedEventArgs e)
		{
			SaveExecInternal(mResultsViewer.GetSourceAlbumArt(e));
		}

		private void SaveAsPresetExec(object sender, ExecutedRoutedEventArgs e)
		{
			var albumArt = mResultsViewer.GetSourceAlbumArt(e);
			albumArt.FilePath = null; // When saving as a preset, always use the default path pattern
			albumArt.Preset = e.Parameter as String;
			SaveExecInternal(albumArt);
		}

		private void SaveExecInternal(AlbumArt albumArt)
		{
			if (albumArt != null)
			{
				if (AutoClose)
				{
					//The save operation is asynchronous, so connect the handler to watch for the save completing
					albumArt.PropertyChanged += AutoCloseOnSave;
				}

				albumArt.Save(this);
			}
		}

		private void SaveAsExec(object sender, ExecutedRoutedEventArgs e)
		{
			AlbumArt albumArt = mResultsViewer.GetSourceAlbumArt(e);
			if (albumArt != null)
			{
				albumArt.PropertyChanged -= AutoCloseOnSave; //No auto-close for SaveAs operation.

				albumArt.SaveAs(this);
			}
		}

		/// <summary>
		/// Removes the album art from the results list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DeleteExec(object sender, ExecutedRoutedEventArgs e)
		{
			AlbumArt albumArt = mResultsViewer.GetSourceAlbumArt(e);
			if (albumArt != null)
			{
				albumArt.Remove();
			}
		}

		private void PreviewExec(object sender, ExecutedRoutedEventArgs e)
		{
			AlbumArt albumArt = mResultsViewer.GetSourceAlbumArt(e);
			if (albumArt != null)
			{
				albumArt.RetrieveFullSizeImage();
				//Show persistant preview window
				var previewWindow = Common.NewPreviewWindow(this);
				previewWindow.AlbumArt = albumArt;
				//Bind to the presets context menu
				BindingOperations.SetBinding(previewWindow, ArtPreviewWindow.PresetsContextMenuProperty, new Binding()
				{
					Source = this,
					Path = new PropertyPath(PresetsContextMenuProperty),
					Mode = BindingMode.OneWay
				});
			}
		}

		private void GetMoreScriptsExec(object sender, ExecutedRoutedEventArgs e)
		{
			Updates.ShowNewScripts();
		}

		private void GetMoreScriptsCanExec(object sender, CanExecuteRoutedEventArgs e)
		{
			//If not performing automatic updates, this is always available
			if (!Properties.Settings.Default.AutoUpdateEnabled)
			{
				e.CanExecute = true;
			}
			else
			{
				//If performing automatic updates, then it's only available if the last time an update was perfomed, new scripts were available
				e.CanExecute = Properties.Settings.Default.NewScriptsAvailable;
			}
		}

		private void ShowAutoDownloadedScriptsExec(object sender, ExecutedRoutedEventArgs e)
		{
			new AutoDownloadedScriptsViewer().Show();
		}
		#endregion

		#region Presets
		private void UpdatePresetsMenu()
		{
			var contextMenu = new ContextMenu();

			//Start with the Save As item
			contextMenu.Items.Add(new MenuItem() { Header = "_Save As...", Command = ApplicationCommands.SaveAs });
			contextMenu.Items.Add(new Separator());

			bool first = true;
			foreach (var preset in Properties.Settings.Default.Presets)
			{
				var presetMenuItem = new MenuItem()
				{
					Header = preset.Name,
					InputGestureText = String.Empty,
					Command = Commands.SaveAsPreset,
					CommandParameter = preset.Value
				};
				if (first)
				{
					first = false;
					//Mark this menu item as the default
					presetMenuItem.FontWeight = FontWeights.Bold;
				}
				contextMenu.Items.Add(presetMenuItem);
			}

			//End with the Edit Presets menu item
			contextMenu.Items.Add(new Separator());
			var editPresets = new MenuItem() { Header = "Edit Presets..." };
			editPresets.Click += OnEditPresets;
			contextMenu.Items.Add(editPresets);

			PresetsContextMenu = contextMenu;
		}

		#region PresetsContextMenu
		private static readonly DependencyPropertyKey PresetsContextMenuPropertyKey = DependencyProperty.RegisterReadOnly("PresetsContextMenu", typeof(ContextMenu), typeof(ArtSearchWindow), new FrameworkPropertyMetadata(null, new CoerceValueCallback(CoercePresetsContextMenu)));
		public static readonly DependencyProperty PresetsContextMenuProperty = PresetsContextMenuPropertyKey.DependencyProperty;

		public object PresetsContextMenu
		{
			get { return (object)GetValue(PresetsContextMenuProperty); }
			private set { SetValue(PresetsContextMenuPropertyKey, value); }
		}

		private static object CoercePresetsContextMenu(DependencyObject sender, object baseValue)
		{
			if (!((ArtSearchWindow)sender).mDefaultSaveFolder.PathPattern.Contains("%preset%"))
			{
				return null;
			}
			return baseValue;
		}
		#endregion

		private void OnEditPresets(object sender, RoutedEventArgs e)
		{
			var editPresets = new EditPresets(Properties.Settings.Default.Presets);
			editPresets.Owner = this;
			if (editPresets.ShowDialog().GetValueOrDefault())
			{
				Properties.Settings.Default.Presets = editPresets.Presets.ToArray();
				UpdatePresetsMenu();
				UpdateDefaultPresetValue();
			}
		}

		private void UpdateDefaultPresetValue()
		{
			if (Properties.Settings.Default.Presets.Length > 0)
			{
				DefaultPresetValue = Properties.Settings.Default.Presets[0].Value;
			}
			else
			{
				DefaultPresetValue = String.Empty;
			}
		}
		
		#region DefaultPresetValue
		private static readonly DependencyPropertyKey DefaultPresetValuePropertyKey = DependencyProperty.RegisterReadOnly("DefaultPresetValue", typeof(string), typeof(ArtSearchWindow), new FrameworkPropertyMetadata(String.Empty));
		public static readonly DependencyProperty DefaultPresetValueProperty = DefaultPresetValuePropertyKey.DependencyProperty;

		public string DefaultPresetValue
		{
			get { return (string)GetValue(DefaultPresetValueProperty); }
			private set { SetValue(DefaultPresetValuePropertyKey, value); }
		}
		#endregion

		#endregion

		#region Updates
		/// <summary>
		/// Gets a value indicating whether any scripts have been automatically downloaded since the application was launched.
		/// <remarks>This can only be true if the <see cref="AutoDownloadAllScripts"/> setting is set true.</remarks>
		/// </summary>
		public bool AutoDownloadedScriptsPresent //Property change notification set up in constructor, by name.
		{
			get
			{
				return Updates.AutoDownloadedScripts.Any();
			}
		}

		#endregion

		#region IAppWindow
		public void LoadSettings()
		{
			System.Diagnostics.Trace.Write("Loading settings for: " + ((IAppWindow)this).Description + "... ");

			AlbumArtDownloader.Properties.WindowSettings.GetWindowSettings(this).LoadWindowState();
			LoadSourceSettings();

			if (!mDefaultSavePathIsTemporary) //If a temporary save path has been set, don't override it.
			{
				LoadDefaultSaveFolderHistory();
			}

			UpdatePresetsMenu();
			UpdateDefaultPresetValue();

			System.Diagnostics.Trace.WriteLine("done.");
		}
		public void SaveSettings()
		{
			System.Diagnostics.Trace.Write("Saving settings from: " + ((IAppWindow)this).Description + "... ");

			AlbumArtDownloader.Properties.WindowSettings.GetWindowSettings(this).SaveWindowState();
			SaveSourceSettings();
			SaveDefaultSaveFolderHistory();

			System.Diagnostics.Trace.WriteLine("done.");
		}
		string IAppWindow.Description
		{
			get
			{
				if (String.IsNullOrEmpty(Artist))
				{
					if (String.IsNullOrEmpty(Album))
					{
						return "Search";
					}
					return "Search: " + Album;
				}
				else if(String.IsNullOrEmpty(Album))
				{
					return "Search: " + Artist;
				}
				return String.Format("Search: {0} / {1}", Artist, Album);
			}
		}
		#endregion

		/// <summary>
		/// Used so that if any of the sources are selected, the search button is enabled
		/// </summary>
		private class NotFalseConverter : IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				return ((bool?)value).GetValueOrDefault(true);
			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				return null; //Not used
			}
		}
	}
}