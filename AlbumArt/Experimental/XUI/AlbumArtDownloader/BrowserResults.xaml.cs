using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AlbumArtDownloader.Controls;

namespace AlbumArtDownloader
{
	public partial class BrowserResults : SortableListView, IDisposable
	{
		public static class Commands
		{
			public static RoutedUICommand SelectMissing = new RoutedUICommand("Select Missing", "SelectMissing", typeof(Commands));
			public static RoutedUICommand GetArtwork = new RoutedUICommand("Get Artwork", "GetArtwork", typeof(Commands));
		}

		private ObservableAlbumCollection mAlbums;
		private Thread mArtFileSearchThread;
		private Queue<Album> mArtFileSearchQueue = new Queue<Album>();
		private AutoResetEvent mArtFileSearchTrigger = new AutoResetEvent(false);
		
		public BrowserResults()
		{
			InitializeComponent();

			Albums = new ObservableAlbumCollection(); //Have one by default

			CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, new ExecutedRoutedEventHandler(SelectAllExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, new ExecutedRoutedEventHandler(DeleteExec)));
			CommandBindings.Add(new CommandBinding(Commands.SelectMissing, new ExecutedRoutedEventHandler(SelectMissingExec)));
			CommandBindings.Add(new CommandBinding(Commands.GetArtwork, new ExecutedRoutedEventHandler(GetArtworkExec), new CanExecuteRoutedEventHandler(GetArtworkCanExec)));
			CommandBindings.Add(new CommandBinding(CommonCommands.Delete, new ExecutedRoutedEventHandler(DeleteArtFileExec), new CanExecuteRoutedEventHandler(CommonCommands.DeleteFileCanExec)));
			CommandBindings.Add(new CommandBinding(CommonCommands.Rename, new ExecutedRoutedEventHandler(RenameArtFileExec), new CanExecuteRoutedEventHandler(CommonCommands.RenameArtFileCanExec)));

			CreateArtFileSearchThread();
		}

		public void Dispose()
		{
			//Close down the search thread.
			if (mArtFileSearchThread != null)
			{
				mArtFileSearchThread.Abort();
				mArtFileSearchThread = null;
			}
		}
		
		#region Command Handlers
		private void DeleteExec(object sender, ExecutedRoutedEventArgs e)
		{
			foreach (Album selectedAlbum in new System.Collections.ArrayList(SelectedItems)) //Take copy of selected items list in case it is changed during removal
			{
				Albums.Remove(selectedAlbum);
			}
		}

		private void SelectAllExec(object sender, ExecutedRoutedEventArgs e)
		{
			AllSelected = !AllSelected.GetValueOrDefault(true); //Mimic behaviour of clicking on the checkbox.
		}

		private void SelectMissingExec(object sender, ExecutedRoutedEventArgs e)
		{
			SelectAll(); //Adding items to the selection programatically is irritatingly difficult, so remove them instead.
			for (int i = 0; i < Items.Count; i++)
			{
				Album album = (Album)Items[i];
				if (album.ArtFileStatus != ArtFileStatus.Missing)
				{
					SelectedItems.Remove(album);
				}
			}
		}

		private void GetArtworkCanExec(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = SelectedItems.Count > 0; //Can't execute if there aren't any selected items.
		}

		private void GetArtworkExec(object sender, ExecutedRoutedEventArgs e)
		{
			AutoDownloader autoDownloader = null;

			if (SelectedItems.Count > 1 && Properties.Settings.Default.FileBrowseAutoDownload)
			{
				autoDownloader = new AutoDownloader();
			}
			else
			{
				//Warn if there are a lot of selected items
				if (SelectedItems.Count > Properties.Settings.Default.EnqueueWarning)
				{
					EnqueueWarning enqueueWarning = new EnqueueWarning();
					enqueueWarning.Owner = Window.GetWindow(this);
					enqueueWarning.NumberToEnqueue = SelectedItems.Count;

					if (!enqueueWarning.ShowDialog().GetValueOrDefault())
					{
						//Cancelled
						return;
					}

					//Trim the selection back to the number to enqueue
					while (SelectedItems.Count > enqueueWarning.NumberToEnqueue)
					{
						SelectedItems.RemoveAt(SelectedItems.Count - 1);
					}
				}
			}

			//The art file search pattern is used as the default path to save the found image to, but first:
			// *In case of alternates, use the first alternate only.
			string artFileSearchPattern = ImagePathPattern.Split(new[] {'|'}, 2)[0];
			// *Don't substitute placeholders, but do substitute recursive path matching with the simplest solution to it, just putting saving to the immediate subfolder
			artFileSearchPattern = artFileSearchPattern.Replace("**\\", "").Replace("**/", "");
			// *Also replace a wildcarded extension
			if (artFileSearchPattern.EndsWith(".*"))
			{
				artFileSearchPattern = artFileSearchPattern.Substring(0, artFileSearchPattern.Length - 2) + ".%extension%";
			}
			// *If the pattern ends in just a wildcard, replace with %name%.%extension%
			else if (artFileSearchPattern.EndsWith("*"))
			{
				artFileSearchPattern = artFileSearchPattern.Substring(0, artFileSearchPattern.Length - 1) + "%name%.%extension%";
			}
			//Replace other wildcards with the %name%, so that for local files search they become wildcards again
			artFileSearchPattern = artFileSearchPattern.Replace("*", "%name%");
			var cascade = new Common.WindowCascade();
			foreach (Album album in SelectedItems)
			{
				//If the image path is relative, get an absolute path for it.
				string rootedArtFileSearchPattern;
				if (Path.IsPathRooted(artFileSearchPattern))
				{
					rootedArtFileSearchPattern = artFileSearchPattern;
				}
				else
				{
					rootedArtFileSearchPattern = Path.Combine(album.BasePath, artFileSearchPattern);
				}

				if (autoDownloader != null)
				{
					album.ArtFile = rootedArtFileSearchPattern; //The destination filename to download to
					autoDownloader.Add(album);
				}
				else
				{
					ArtSearchWindow searchWindow = Common.NewSearchWindow(Window.GetWindow(this) as IAppWindow);
					cascade.Arrange(searchWindow);

					searchWindow.SetDefaultSaveFolderPattern(rootedArtFileSearchPattern, true); //Default save to the location where the image was searched for.
					searchWindow.Search(album.Artist, album.Name); //Kick off the search.

					//Watch for the window being closed to update the status of the artwork
					mSearchWindowAlbumLookup.Add(searchWindow, album);
					searchWindow.Closed += OnSearchWindowClosed;
				}
			}
			if (autoDownloader != null)
			{
				autoDownloader.Show();
			}
		}

		private Dictionary<ArtSearchWindow, Album> mSearchWindowAlbumLookup = new Dictionary<ArtSearchWindow, Album>();
		private void OnSearchWindowClosed(object sender, EventArgs e)
		{
			Album album;
			var searchWindow = sender as ArtSearchWindow;
			if(mSearchWindowAlbumLookup.TryGetValue(searchWindow, out album))
			{
				searchWindow.Closed -= OnSearchWindowClosed;
				mSearchWindowAlbumLookup.Remove(searchWindow);

				if (album.ArtFileStatus != ArtFileStatus.Present) //If it was present before, assume it's still present - no need to re-search
				{
					QueueAlbumForArtFileSearch(album);
				}
			}
		}

		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			base.OnMouseDoubleClick(e);
			
			DependencyObject parent = e.OriginalSource as DependencyObject;
			while (parent != null)
			{
				if (parent is ListViewItem)
				{
					//A list item was double clicked on, so get artwork for it
					e.Handled = true;
					System.Diagnostics.Debug.Assert(SelectedItems.Count == 1, "Expecting only the double clicked item to be selected");
					GetArtworkExec(null, null);
					return;
				}
				else if (parent == this)
				{
					//A list item was not double clicked on, something else was
					break;
				}
				parent = VisualTreeHelper.GetParent(parent);
			}
			//Do nothing for double click happening elsewhere.
		}

		private void DeleteArtFileExec(object sender, ExecutedRoutedEventArgs e)
		{
			CommonCommands.DeleteFileExec(sender, e);
			CheckForChangedArt(e);
		}

		private void RenameArtFileExec(object sender, ExecutedRoutedEventArgs e)
		{
			CommonCommands.RenameArtFileExec(sender, e);
			CheckForChangedArt(e);
		}

		//Pass in the ExecutedRoutedEventArgs from the Exec method to check for changed artwork as a result of that command
		private void CheckForChangedArt(ExecutedRoutedEventArgs e)
		{
			var item = e.OriginalSource as ListViewItem;
			if (item != null)
			{
				var album = item.Content as Album;
				if (album != null)
				{
					QueueAlbumForArtFileSearch(album);
				}
			}
		}
		#endregion

		#region Select All
		private bool mSettingAllSelected = false; //Flag to prevent listening to IsSelected changes when setting them all
		private bool? mAllSelected = false;
		/// <summary>
		/// This can be set to true, to enable all sources, false, to disable them all,
		/// or null to leave them as they are. It will return true if all sources are
		/// Selected, false if they are all disabled, or null if they are mixed.
		/// </summary>
		public bool? AllSelected
		{
			get
			{
				return mAllSelected;
			}
			set
			{
				if (value != mAllSelected)
				{
					if (value.HasValue)
					{
						mSettingAllSelected = true;
						if (value.Value)
						{
							SelectAll();
						}
						else
						{
							SelectedItems.Clear();
						}
						mSettingAllSelected = false;
					}
					mAllSelected = value;
					NotifyPropertyChanged("AllSelected");
				}
			}
		}

		protected override void OnSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs e)
		{
			base.OnSelectionChanged(e);
		
			if (!mSettingAllSelected) //Ignore selection change while setting selection from AllSelected setter.
			{
				if (e.RemovedItems.Count > 0)
				{
					//Check to see if there is now nothing selected
					if (SelectedItems.Count == 0)
					{
						mAllSelected = false; //Don't change through the accessor, so it doesn't bother trying to reapply the selection
						NotifyPropertyChanged("AllSelected");
						return;
					}
				}
				if (e.AddedItems.Count > 0)
				{
					//Check to see if there is now all selected
					if (SelectedItems.Count == Items.Count)
					{
						mAllSelected = true; //Don't change through the accessor, so it doesn't bother trying to reapply the selection
						NotifyPropertyChanged("AllSelected");
						return;
					}
				}
				//Not all items are selected, so set property as mixed.
				mAllSelected = null;
				NotifyPropertyChanged("AllSelected");
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

		#region Properties
		private string mImagePathPattern;
		public string ImagePathPattern 
		{
			get
			{
				return mImagePathPattern;
			}
			set
			{
				if (value != mImagePathPattern)
				{
					mImagePathPattern = value;
					NotifyPropertyChanged("ImagePathPattern");
				}
			}
		}

		#region State
		private BrowserState mState = BrowserState.Ready;
		public BrowserState State
		{
			get
			{
				return mState;
			}
			private set
			{
				if (value != mState)
				{
					mState = value;
					Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
					{
						OnStateChanged(EventArgs.Empty);
						NotifyPropertyChanged("State");
					}));
				}
			}
		}
		public event EventHandler StateChanged;
		protected virtual void OnStateChanged(EventArgs e)
		{
			EventHandler temp = StateChanged;
			if (temp != null)
			{
				temp(this, e);
			}
		}
		#endregion

		#region ProgressText
		private string mProgressText;
		public string ProgressText
		{
			get
			{
				return mProgressText;
			}
			private set
			{
				if (value != mProgressText)
				{
					mProgressText = value;
					Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
					{
						OnProgressTextChanged(EventArgs.Empty);
						NotifyPropertyChanged("ProgressText");
					}));
				}
			}
		}
		public event EventHandler ProgressTextChanged;
		protected virtual void OnProgressTextChanged(EventArgs e)
		{
			EventHandler temp = ProgressTextChanged;
			if (temp != null)
			{
				temp(this, e);
			}
		}
		#endregion

		internal ObservableAlbumCollection Albums
		{
			get
			{
				return mAlbums;
			}
			set
			{
				if (value != mAlbums)
				{
					if (mAlbums != null)
					{
						mAlbums.CollectionChanged -= OnAlbumsCollectionChanged;
					}
					mAlbums = value;
					if (mAlbums != null)
					{
						mAlbums.CollectionChanged += OnAlbumsCollectionChanged;
					}

					ItemsSource = mAlbums;

					//Reset art file search queue to the new albums
					ClearArtFileSearchQueue();
					foreach (Album album in mAlbums)
					{
						QueueAlbumForArtFileSearch(album);
					}
					NotifyPropertyChanged("Albums");
				}
			}
		}
		#endregion

		#region ArtFile Searching
		public void AbortSearch()
		{
			ClearArtFileSearchQueue();

			//Restart the art file search thread
			mArtFileSearchThread.Abort();
			mArtFileSearchThread = null;
			CreateArtFileSearchThread();
		}

		private void OnAlbumsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (Album album in e.NewItems)
					{
						if (album.ArtFileStatus == ArtFileStatus.Unknown)
						{
							QueueAlbumForArtFileSearch(album);
						}
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (Album album in e.OldItems)
					{
						CancelQueuedAlbum(album);
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					ClearArtFileSearchQueue();
					break;
			}
		}

		private void QueueAlbumForArtFileSearch(Album album)
		{
			album.ArtFileStatus = ArtFileStatus.Queued;
			lock (mArtFileSearchQueue)
			{
				mArtFileSearchQueue.Enqueue(album);
			}
			mArtFileSearchTrigger.Set();
		}

		private void CancelQueuedAlbum(Album album)
		{
			album.ArtFileStatus = ArtFileStatus.Unknown;
			lock (mArtFileSearchQueue)
			{
				if (mArtFileSearchQueue.Contains(album))
				{
					Album[] queue = mArtFileSearchQueue.ToArray();
					mArtFileSearchQueue.Clear();
					foreach (Album queuedAlbum in queue)
					{
						if (queuedAlbum != album)
						{
							mArtFileSearchQueue.Enqueue(queuedAlbum);
						}
					}
				}
			}
		}

		private void ClearArtFileSearchQueue()
		{
			lock (mArtFileSearchQueue)
			{
				foreach (Album album in mArtFileSearchQueue)
				{
					album.ArtFileStatus = ArtFileStatus.Unknown;
				}
				mArtFileSearchQueue.Clear();
			}
		}

		private void CreateArtFileSearchThread()
		{
			System.Diagnostics.Debug.Assert(mArtFileSearchThread == null, "An art file search thread already exists!");
			mArtFileSearchThread = new Thread(new ThreadStart(ArtFileSearchWorker));
			mArtFileSearchThread.Name = "Art File Searcher";
			mArtFileSearchThread.Start();
		}

		private void ArtFileSearchWorker()
		{
			try
			{
				do
				{
					mArtFileSearchTrigger.WaitOne(); //Wait until there is work to do

					State = BrowserState.FindingArt;

					do //Loop through all the queued art.
					{
						Album album;
						lock (mArtFileSearchQueue)
						{
							if (mArtFileSearchQueue.Count == 0)
							{
								break; //Nothing to search for, so go back and wait until there is.
							}
							else
							{
								album = mArtFileSearchQueue.Dequeue();
							}
						}
						System.Diagnostics.Debug.Assert(album.ArtFileStatus == ArtFileStatus.Queued, "Expecting the album to be queued for searching");
						album.ArtFileStatus = ArtFileStatus.Searching;
						try
						{
							ProgressText = String.Format("Finding art... {0} / {1}", album.Artist, album.Name);
							
							foreach (string imagePathPatternAlternate in ImagePathPattern.Split('|'))
							{
								string artFileSearchPattern = Common.SubstitutePlaceholders(imagePathPatternAlternate, album.Artist, album.Name);
								Regex artFileCheckPattern = MakeCheckPattern(imagePathPatternAlternate, album.Artist, album.Name);

								if (!Path.IsPathRooted(artFileSearchPattern))
								{
									artFileSearchPattern = Path.Combine(album.BasePath, artFileSearchPattern);
								}
								foreach (string artFile in Common.ResolvePathPattern(artFileSearchPattern))
								{
									if (artFileCheckPattern.IsMatch(artFile))
									{
										album.SetArtFile(artFile);

										break; //Only use the first art file that matches, if there are multiple matches.
									}
								}
								//If a matching art file is found, don't search further alternates
								if (album.ArtFileStatus == ArtFileStatus.Present)
								{
									break;
								}
							}
						}
						catch (Exception)
						{
							album.ArtFileStatus = ArtFileStatus.Unknown; //It might not be missing, we just haven't found it before hitting an exception
						}
						if (album.ArtFileStatus != ArtFileStatus.Present) //If it wasn't found, then it's missing.
							album.ArtFileStatus = ArtFileStatus.Missing;
					} while (true);

					State = BrowserState.Done;
				} while (true);
			}
			catch (ThreadAbortException)
			{
				State = BrowserState.Stopped;
				return;
			}
		}

		/// <summary>A regex part that will match any of the valid extensions for image encoders</summary>
		private static string sImageEncoderExtensions = "(?:" + String.Join("|", (from encoder in ImageCodecInfo.GetImageEncoders() select encoder.FilenameExtension.Replace("*.", "") //Remove *.
																																									.Replace(';', '|')) //Use | instead of ; as a separator
																																									.ToArray()) + ")";
		/// <summary>A regex part that will match any of the valid values for cover type</summary>
		private static string sCoverTypes = "(?:" + String.Join("|", Enum.GetNames(typeof(AlbumArtDownloader.Scripts.CoverType))) + ")";
		
		/// <summary>Creates a regex that will match paths that can match pathPattern.</summary>
		private Regex MakeCheckPattern(string pathPattern, string artist, string album)
		{
			string result = pathPattern.Replace("%artist%", Common.MakeSafeForPath(artist))
										.Replace("%album%", Common.MakeSafeForPath(album));

			result = Regex.Replace(result, @".*?(^|\\|/)\.\.?(?=\\|/)", ""); //Strip off ./ or ../, and anything before it (as that can't be matched against the resulting paths, which will resolve it away)
			result = Regex.Escape(result) + "$"; //Escape the whole thing, and require it to be the end of the path
			result = Regex.Replace(result, @"(\\\\|/)", @"[\\/]"); //Replace all / or \ characters with [\/] to allow matching either path character
			result = Regex.Replace(result, @"\\(\*|\?)", @"[^\\/]$1?"); //Replace * (and ?) with [^\/]*? to match within the path segment only, and non-greedy

			//restrict the extension placeholder to be a valid extension
			result = result.Replace("%extension%", sImageEncoderExtensions);
			//restrict the size placeholder to be in the format sizes are written out in
			result = result.Replace("%size%", @"\d+ x \d+");
			//replace the type placeholder with the possible type values
			result = result.Replace("%type%", sCoverTypes);
			//replace custom type placeholder with the values it describes
			result = Regex.Replace(result, @"%type(?:\\\((?<names>[^)]*)\\\))%",
				new MatchEvaluator(delegate(Match match)
				{
					return "(?:" + match.Groups["names"].Value.Replace(',', '|') + ")";
				}),
				RegexOptions.IgnoreCase);
										
			//Replace remaining placeholders with [^\/]*? to match within the path segment
			result = Regex.Replace(result, @"%(?:name|source|preset)%", @"[^\\/]*?", RegexOptions.IgnoreCase);

			return new Regex(result, RegexOptions.IgnoreCase);
		}
		#endregion
	}	
}
