using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using AlbumArtDownloader.Scripts;
using System.Windows.Data;

namespace AlbumArtDownloader
{
	public partial class AutoDownloader : Window, IAppWindow
	{
		public static class Commands
		{
			public static RoutedUICommand NewSearchWindow = new RoutedUICommand("New Search Window", "NewSearchWindow", typeof(Commands));
		}

		private Sources mAllSources = new Sources();
		private ObservableCollectionOfDisposables<AlbumArt> mResults = new ObservableCollectionOfDisposables<AlbumArt>();
		private Dictionary<Album, AlbumArt> mResultsLookup = new Dictionary<Album, AlbumArt>();
		private Dictionary<AlbumArt, Album> mReverseLookup = new Dictionary<AlbumArt, Album>();
		private List<Album> mAlbumsMissingResults = new List<Album>();
		private List<Source> mSourcesToSearch = new List<Source>();
		private ContextMenu mResultsContextMenu;
		private Thread mSearchThread;
		
		public AutoDownloader()
		{
			InitializeComponent();

			mResultsContextMenu = (ContextMenu)Resources["mResultsContextMenu"];

			CommandBindings.Add(new CommandBinding(AutoDownloaderQueue.Commands.ShowInResults, new ExecutedRoutedEventHandler(ShowInResultsExec)));
			CommandBindings.Add(new CommandBinding(Commands.NewSearchWindow, new ExecutedRoutedEventHandler(NewSearchWindowExec)));
			CommandBindings.Add(new CommandBinding(CommonCommands.Delete, new ExecutedRoutedEventHandler(DeleteArtFileExec)));
			CommandBindings.Add(new CommandBinding(CommonCommands.Rename, new ExecutedRoutedEventHandler(RenameArtFileExec)));
			
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Stop, new ExecutedRoutedEventHandler(StopExec)));

			CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, new ExecutedRoutedEventHandler(SaveExec)));
			CommandBindings.Add(new CommandBinding(CommonCommands.Preview, new ExecutedRoutedEventHandler(PreviewExec)));

			foreach (IScript script in ((App)Application.Current).Scripts)
			{
				ScriptSource source = new ScriptSource(script);
				source.LoadSettings();
				source.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);

				mAllSources.Add(source);
			}
			
			mSourcesViewer.ItemsSource = mAllSources;

			mResultsList.ItemsSource = mResults;
			mResultsList.ContextMenuOpening += new ContextMenuEventHandler(mResultsList_ContextMenuOpening);

			//Bind the SelectAll checkbox
			Binding selectAllBinding = new Binding("AllEnabled");
			selectAllBinding.Source = mAllSources;
			selectAllBinding.Mode = BindingMode.TwoWay;
			BindingOperations.SetBinding(mSelectAll, CheckBox.IsCheckedProperty, selectAllBinding);

			LoadSettings();

			UpdateSourceList();

			if (Properties.Settings.Default.ShowAutoDownloadWarning)
			{
				IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnIsVisibleChanged);
			}
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			//Assign sensible defaults to things that depend on layout
			if (Properties.Settings.Default.PanelWidth < 0)
			{
				Properties.Settings.Default.PanelWidth = mResultsList.ActualWidth / 2;
				mResultsList.AutoSizePanels();
			}
		}

		private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				//Window is being shown. Show warning
				Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new ThreadStart(delegate
				{
					var warning = new AutoDownloadWarning();
					warning.Owner = this;
					if (warning.ShowDialog().GetValueOrDefault())
					{
						//User chose to enqueue normal search windows instead.
						var cascade = new Common.WindowCascade();
						foreach (var album in mQueueList.Albums)
						{
							ArtSearchWindow searchWindow = Common.NewSearchWindow(this);
							cascade.Arrange(searchWindow);
							searchWindow.SetDefaultSaveFolderPattern(album.ArtFile, true); //Default save to the location where the image was searched for.
							searchWindow.Search(album.Artist, album.Name);
						}

						Close();
					}
				}));
			}
		}

		private void mResultsList_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			e.Handled = true;
			
			AlbumArt source = mResultsList.GetSourceAlbumArt(e);
			if (source != null)
			{
				//Show context menu for the source
				Album album = mReverseLookup[source];
				foreach (MenuItem menuItem in mResultsContextMenu.Items.OfType<MenuItem>())
				{
					if (menuItem.Command == CommonCommands.ShowInExplorer)
					{
						//To avoid having to re-implement ShowInExplorer, the parameter should be the path to the file to show
						menuItem.CommandParameter = album.ArtFile;
					}
					else
					{
						menuItem.CommandParameter = album;
					}
					menuItem.CommandTarget = this;
				}
				mResultsContextMenu.IsOpen = true;
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			SaveSettings();
		}

		protected override void OnClosed(EventArgs e)
		{
			//Close down the search thread.
			if (mSearchThread != null)
			{
				mSearchThread.Abort();
				mSearchThread = null;
			}

			mResults.Clear(); //This will dispose of the AlbumArts within it

			base.OnClosed(e);
		}

		public void Add(Album album)
		{
			//Substitute Album and Artist placeholders, as the AlbumArt won't do this
			album.ArtFile = album.ArtFile.Replace("%artist%", Common.MakeSafeForPath(album.Artist))
										 .Replace("%album%", Common.MakeSafeForPath(album.Name));

			album.ArtFileStatus = ArtFileStatus.Queued;
			mQueueList.Albums.Add(album);

			ProgressMax = mQueueList.Albums.Count;
		}

		#region Searching
		
		private void mStartSearch_Click(object sender, RoutedEventArgs e)
		{
			StartSearch();
		}
		private void StopExec(object sender, ExecutedRoutedEventArgs e)
		{
			AbortSearch();
		}

		private void StartSearch()
		{
			mStartSearch.IsEnabled = false;

			if (mQueueList.GetNextAlbum() == null)
			{
				//There are no albums queued, so reset all the missing ones to be queued.
				if (mAlbumsMissingResults.Count > 0)
				{
					mAlbumsMissingResults.ForEach(a => a.ArtFileStatus = ArtFileStatus.Queued);
					mAlbumsMissingResults.Clear();
				}
				else
				{
					System.Diagnostics.Debug.Fail("Start button should not be enabled if there is nothing to search for");
					return;
				}
			}

			System.Diagnostics.Debug.Assert(mSearchThread == null, "A search thread already exists!");
			mSearchThread = new Thread(new ThreadStart(SearchWorker));
			mSearchThread.Name = "Automatic Searcher";
			mSearchThread.Start();
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsEnabled" || e.PropertyName == "IsPrimary")
			{
				//A source has been changed, so recreate source list.
				UpdateSourceList();
			}
		}

		private void UpdateSourceList()
		{
			//Produce list of sources to search
			List<Source> primarySources = new List<Source>();
			List<Source> secondarySources = new List<Source>();
			foreach (var source in mAllSources)
			{
				if (source.IsEnabled)
				{
					source.FullSizeOnly = false;
					if (source.IsPrimary)
					{
						primarySources.Add(source);
					}
					else
					{
						secondarySources.Add(source);
					}
				}
			}
			lock (mSourcesToSearch)
			{
				mSourcesToSearch.Clear();
				mSourcesToSearch.AddRange(primarySources); //Search primary sources first
				mSourcesToSearch.AddRange(secondarySources); //Then secondary sources
			}
		}

		private void AbortSearch()
		{
			ProgressText = String.Empty;

			ReEnableStartButton();
			
			if (mSearchThread != null)
			{
				mSearchThread.Abort();
				mSearchThread = null;
			}
		}

		private void ReEnableStartButton()
		{
			//Re-enable the Start button if there are any more albums to search for.
			mStartSearch.IsEnabled = mAlbumsMissingResults.Count > 0 || mQueueList.GetNextAlbum() != null;
		}

		private ManualResetEvent mWaitForFullSizeImage = new ManualResetEvent(true);
		private ManualResetEvent mWaitForSourceCompleted = new ManualResetEvent(true);
		/// <summary>The search result found which matches all the criteria for the currently searched album, if any.</summary>
		private AlbumArt mSearchResult;

		private void SearchWorker()
		{
			Album album = null;

			Dispatcher.Invoke(DispatcherPriority.Send, new ThreadStart(delegate
			{
				album = mQueueList.GetNextAlbum();
				IsSearching = true;
			}));

			while(album != null)
			{
				album.ArtFileStatus = ArtFileStatus.Searching;
				
				Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
				{
					if (mQueueList.ShouldAutoscroll)
					{
						mQueueList.ScrollIntoView(album);
					}
				}));

				try
				{
					List<Source> currentSourcesToSearch;
					lock(mSourcesToSearch)
					{
						//Take a copy of the source list so that it doesn't get affected by changes)
						currentSourcesToSearch = new List<Source>(mSourcesToSearch);
					}
					foreach (var source in currentSourcesToSearch) //Try each source in turn
					{
						Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
						{
							ProgressText = String.Format("Searching {0} for {1} / {2}", source.Name, album.Artist, album.Name);
						}));

						source.Results.CollectionChanged += OnSourceFoundResult;
						source.QueryContinueSearch += OnSourceQueryContinue;
						source.SearchCompleted += OnSourceSearchComplete;
						
						mWaitForSourceCompleted.Reset();
						Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(delegate
						{
							//source.Search must be done in the main dispatcher thread, as that's the thread which the observable collection will be updated on.
							source.Search(album.Artist, album.Name);
						}));
						
						//Wait for the source to have finished searching
						mWaitForSourceCompleted.WaitOne();

						System.Diagnostics.Debug.Assert(source.IsSearching == false, "Source is supposed to have completed by now");

						//Done with this source - stop listening to its events.
						source.Results.CollectionChanged -= OnSourceFoundResult;
						source.QueryContinueSearch -= OnSourceQueryContinue;
						source.SearchCompleted -= OnSourceSearchComplete;

						//Was a valid result found in this source?
						if (mSearchResult != null)
						{
							var observableCollectionOfDisposables = source.Results as ObservableCollectionOfDisposables<AlbumArt>;
							if (observableCollectionOfDisposables != null)
							{
								//Detach the result from the collection so that it isn't disposed of when the source is re-used.
								observableCollectionOfDisposables.Detach(mSearchResult);
								//Responsibility for disposing of mSearchResult is now taken here, and will be passed on to mResults when it is added to that collection.
							}
							
							//No need to search other sources.
							break;
						}
					}

					//Was a valid result found in any source?
					if (mSearchResult != null)
					{
						//Yes, so save it and add it to the results list.

						Dispatcher.Invoke(DispatcherPriority.DataBind, new ThreadStart(delegate
						{
							mSearchResult.DefaultFilePathPattern = album.ArtFile;
							//If the file already exists, overwrite it
							if (System.IO.File.Exists(mSearchResult.FilePath))
							{
								try
								{
									System.IO.File.Delete(mSearchResult.FilePath);
								}
								catch (Exception ex)
								{
									System.Diagnostics.Trace.TraceError("Could not delete file \"{0}\": {1}", mSearchResult.FilePath, ex.Message);
								}
							}
							mSearchResult.Save();

							mResults.Add(mSearchResult);

							album.SetArtFile(mSearchResult.FilePath);

							Progress++;
						}));

						mResultsLookup.Add(album, mSearchResult);
						mReverseLookup.Add(mSearchResult, album);

						mSearchResult = null;
					}
					else
					{
						mAlbumsMissingResults.Add(album);
						album.ArtFileStatus = ArtFileStatus.Missing;
					}
				}
				catch(ThreadAbortException)
				{
					album.ArtFileStatus = ArtFileStatus.Queued;
					return;
				}


				//Get the next album
				Dispatcher.Invoke(DispatcherPriority.Send, new ThreadStart(delegate
				{
					album = mQueueList.GetNextAlbum();
				}));
			}

			Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
			{
				IsSearching = false;
				System.Diagnostics.Debug.Assert(mQueueList.Items.Count - mResults.Count == mAlbumsMissingResults.Count, "Unexpected albums missing results count");
				ProgressText = String.Format("Done. {0} albums found, {1} not found.", mResults.Count, mAlbumsMissingResults.Count);

				mSearchThread = null;

				ReEnableStartButton();
			}));
		}

		private void OnSourceFoundResult(object sender, NotifyCollectionChangedEventArgs e)
		{
			System.Diagnostics.Debug.Assert(e.Action == NotifyCollectionChangedAction.Add ||
											e.Action == NotifyCollectionChangedAction.Reset, //The collection will be initialised once, with Reset called.
											"Only expecting results to be added to the found results collection");

			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				System.Diagnostics.Debug.Assert(e.NewItems.Count == 1, "Expecting results to be added one at a time");
			
				AlbumArt result = (AlbumArt)e.NewItems[0];
				
				//Check if the result meets the required criteria
				var coverType = Common.MakeAllowedCoverType(result.CoverType);
				if ((coverType & Properties.Settings.Default.AllowedCoverTypes) == coverType)
				{
					//Album is of a type that's allowed
					//Both width and height must be bigger, so use the smallest of the two
					int size = (int)Math.Min(result.ImageWidth, result.ImageHeight);
					if (size == -1 || CheckImageSize(size)) //If the size is unspecified, or the specified size is within range
					{										//Then download the full size image, and check it's size when done.
						//Have the source wait until the full size image is downloaded
						mWaitForFullSizeImage.Reset();
						result.RetrieveFullSizeImage(OnResultImageDownloaded);
					}					
				}
			}
		}

		private bool CheckImageSize(int size)
		{
			return (!Properties.Settings.Default.UseMinimumImageSize || size >= Properties.Settings.Default.MinimumImageSize) &&
					(!Properties.Settings.Default.UseMaximumImageSize || size <= Properties.Settings.Default.MaximumImageSize);
		}

		private void OnResultImageDownloaded(object state)
		{
			AlbumArt result = (AlbumArt)state;
			//Check results dimensions
			int size = (int)Math.Min(result.ImageWidth, result.ImageHeight);
			if (CheckImageSize(size))
			{
				//Found a valid result!
				mSearchResult = result;
			}

			//Either continue searching for more results, or abort the search, but either way, stop waiting.
			mWaitForFullSizeImage.Set();
		}

		private void OnSourceQueryContinue(object sender, CancelEventArgs e)
		{
			mWaitForFullSizeImage.WaitOne(); //Wait before searching for the next result, until the current result is evaluated
			e.Cancel = mSearchResult != null; //If a valid result was found, then stop searching for any more.
		}

		private void OnSourceSearchComplete(object sender, EventArgs e)
		{
			//Stop waiting for the source to complete - it's just done so
			mWaitForSourceCompleted.Set();
		}
		#endregion

		#region Highlight Result
		private void ShowInResultsExec(object sender, ExecutedRoutedEventArgs e)
		{
			AlbumArt result;
			if(mResultsLookup.TryGetValue((Album)e.Parameter, out result))
			{
				var container = mResultsList.ItemContainerGenerator.ContainerFromItem(result) as FrameworkElement;
				container.BringIntoView();
				var adornerLayer = AdornerLayer.GetAdornerLayer(container);
				var highlighter = new HighlightResultAdorner(container);
				adornerLayer.Add(highlighter);
				DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.DataBind) { Interval = TimeSpan.FromSeconds(2) };
				timer.Tick += new EventHandler(delegate
				{
					timer.Stop();
					adornerLayer.Remove(highlighter);
				});
				timer.Start();
			}
		}

		private class HighlightResultAdorner : Adorner
		{
			public HighlightResultAdorner(UIElement adornedElement)
				: base(adornedElement)
			{}

			protected override void OnRender(DrawingContext drawingContext)
			{
				drawingContext.DrawRoundedRectangle(null, new Pen(SystemColors.HighlightBrush, 3),
					new Rect(2, 2, ActualWidth - 7, ActualHeight - 4), 3, 3);
			}
		}
		#endregion

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);

			//Add behaviour that if you click outside the queue, it loses focus (and therefore starts autoscrolling again)
			FocusManager.SetFocusedElement(this, this);
		}

		#region Progress Reporting

		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(AutoDownloader), new FrameworkPropertyMetadata(false));
		public bool IsSearching
		{
			get { return (bool)GetValue(IsSearchingProperty); }
			set { SetValue(IsSearchingProperty, value); }
		}

		public static readonly DependencyPropertyKey ProgressTextPropertyKey = DependencyProperty.RegisterReadOnly("ProgressText", typeof(string), typeof(AutoDownloader), new FrameworkPropertyMetadata(String.Empty));
		public string ProgressText
		{
			get { return (string)GetValue(ProgressTextPropertyKey.DependencyProperty); }
			private set { SetValue(ProgressTextPropertyKey, value); }
		}

		public static readonly DependencyPropertyKey ProgressPropertyKey = DependencyProperty.RegisterReadOnly("Progress", typeof(double), typeof(AutoDownloader), new FrameworkPropertyMetadata(0D));
		public double Progress
		{
			get { return (double)GetValue(ProgressPropertyKey.DependencyProperty); }
			private set { SetValue(ProgressPropertyKey, value); }
		}

		public static readonly DependencyPropertyKey ProgressMaxPropertyKey = DependencyProperty.RegisterReadOnly("ProgressMax", typeof(double), typeof(AutoDownloader), new FrameworkPropertyMetadata(0D));
		public double ProgressMax
		{
			get { return (double)GetValue(ProgressMaxPropertyKey.DependencyProperty); }
			private set { SetValue(ProgressMaxPropertyKey, value); }
		}
		#endregion

		#region Results List Command Execution
		#region ArtPanel native commands
		private void SaveExec(object sender, ExecutedRoutedEventArgs e)
		{
			AlbumArt albumArt = mResultsList.GetSourceAlbumArt(e);
			if (albumArt != null)
			{
				albumArt.Save();
			}
		}
		#endregion

		#region Menu Commands
		private void PreviewExec(object sender, ExecutedRoutedEventArgs e)
		{
			AlbumArt albumArt;
			if (mResultsLookup.TryGetValue((Album)e.Parameter, out albumArt))
			{
				//Show persistant preview window
				var previewWindow = Common.NewPreviewWindow(this);
				previewWindow.AlbumArt = albumArt;
			}
		}

		private void DeleteArtFileExec(object sender, ExecutedRoutedEventArgs e)
		{
			var album = (Album)e.Parameter;
			if (Common.DeleteFileToRecycleBin(album.ArtFile))
			{
				System.Diagnostics.Debug.Assert(!System.IO.File.Exists(album.ArtFile));
				//File doesn't exist any more
				album.ArtFileStatus = ArtFileStatus.Missing;

				var albumArt = mResultsLookup[album];
				mResults.Remove(albumArt);
				mResultsLookup.Remove(album);
				mReverseLookup.Remove(albumArt);
			}
		}

		private void RenameArtFileExec(object sender, ExecutedRoutedEventArgs e)
		{
			var album = (Album)e.Parameter;
			var renameWindow = new RenameArt(album.ArtFile);
			renameWindow.Owner = this;
			if (renameWindow.ShowDialog().GetValueOrDefault())
			{
				mResultsLookup[album].DefaultFilePathPattern = album.ArtFile = renameWindow.NewFile;
			}
		}

		private void NewSearchWindowExec(object sender, ExecutedRoutedEventArgs e)
		{
			IEnumerable<Album> albums;
			var albumParameter = e.Parameter as Album;
			if (albumParameter != null)
			{
				//Search for the specified album in a new search window
				albums = new Album[] { albumParameter };
			}
			else
			{
				//Search for all the selected albums in new search windows
				albums = mQueueList.SelectedItems.Cast<Album>();
			}

			var cascade = new Common.WindowCascade();
			foreach (Album album in albums)
			{
				ArtSearchWindow searchWindow = Common.NewSearchWindow(this);
				cascade.Arrange(searchWindow);
				searchWindow.SetDefaultSaveFolderPattern(album.ArtFile, true); //Default save to the location where the image was searched for.
				searchWindow.Search(album.Artist, album.Name);
			}
		}
		#endregion
		#endregion

		#region IAppWindow Members

		public void SaveSettings()
		{
			SaveSourceSettings();
		}

		public void LoadSettings()
		{
			LoadSourceSettings();
		}

		public string Description
		{
			get { return String.Format("{0}: {1} albums", Title, mQueueList.Albums.Count); }
		}

		private void LoadSourceSettings()
		{
			foreach (Source source in mAllSources)
			{
				source.LoadSettings();
			}
		}
		private void SaveSourceSettings()
		{
			foreach (Source source in mAllSources)
			{
				source.SaveSettings();
			}
		}
		#endregion
	}
}
