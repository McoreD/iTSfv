using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using AlbumArtDownloader.TreeViewViewModel;

namespace AlbumArtDownloader
{
    /// <summary>
    /// Interaction logic for TreeBrowserResults.xaml
    /// </summary>
    public partial class TreeBrowserResults : TreeView, IDisposable
    {
        public TreeBrowserResults()
        {
            InitializeComponent();

            Albums = new ObservableAlbumCollection(); //Have one by default

            CommandBindings.Add(new CommandBinding(CommonCommands.Save, new ExecutedRoutedEventHandler(SaveExec)));
            
            CreateArtFileSearchThread();
		}
	
		private ObservableAlbumCollection mAlbums;
		private Thread mArtFileSearchThread;
		private Queue<Album> mArtFileSearchQueue = new Queue<Album>();
		private AutoResetEvent mArtFileSearchTrigger = new AutoResetEvent(false);

        private List<DiscViewModel> discList = new List<DiscViewModel>();
		
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
		private void SaveExec(object sender, ExecutedRoutedEventArgs e)
		{
            TagLib.File file = (TagLib.File)(e.Parameter);

            file.Save();
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
            ConvertIntoTreeItems(mAlbums);

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

                    ConvertIntoTreeItems(mAlbums);

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

        /// <summary>
        /// Method will go through all added albums, convert them into tree item hierarchy
        /// and display them
        /// 
        /// </summary>
        /// <param name="addedAlbums"></param>
        private void ConvertIntoTreeItems(ObservableAlbumCollection addedAlbums)
        {
            // hash map od disc and its tracks
            Dictionary<String, List<TagLib.File>> discs = new Dictionary<String, List<TagLib.File>>();

            // go through all given albums
            foreach (Album album in addedAlbums)
            {
                if (album == null)
                    continue;

                String albumName = album.Name;

                List<TagLib.File> values;

                // check if there is entry for given key
                bool contains = discs.ContainsKey(albumName);

                // there is no entry for current key - create one
                // and insert empty list of files
                if (!contains)
                {
                    values = new List<TagLib.File>();
                    discs.Add(albumName, values);
                }
                else
                {
                    // TODO: some check for null pointer etc
                    bool success = discs.TryGetValue(albumName, out values);
                }

                foreach (TagLib.File item in album.Tracks)
                {
                    values.Add(item);
                }
            }

            // prepare dataprovider for the treeview component
            discList = new List<DiscViewModel>();

            // go through hashmap of albums and add them all into 
            // treeview item model with all its children
            foreach (KeyValuePair<string, List<TagLib.File>> pair in discs)
            {
                TagLib.File[] tagLibFiles = pair.Value.ToArray();
                DiscViewModel disc = new DiscViewModel(pair.Key, tagLibFiles);
                discList.Add(disc);
            }

            base.DataContext = discList.ToArray();
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

		
		#endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TrackNameTextBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            Object obj = this.SelectedItem;
        }

        private void TrackNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
	}	
}