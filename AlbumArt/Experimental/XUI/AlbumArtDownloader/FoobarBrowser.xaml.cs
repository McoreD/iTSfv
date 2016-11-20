using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Collections.Specialized;
using System.Windows.Data;
using System.Linq;

namespace AlbumArtDownloader
{
	/// <summary>
	/// Interaction logic for FoobarBrowser.xaml
	/// </summary>

	public partial class FoobarBrowser : System.Windows.Window, INotifyPropertyChanged, IAppWindow
	{
		Foobar2000.Application07Class mFoobar;
		PlaylistsCollection mPlaylists;

		private Thread mReadFoobarThread;
		private ObservableAlbumCollection mAlbums = new ObservableAlbumCollection();

		public FoobarBrowser()
		{
			InitializeComponent();
			LoadSettings();

			CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, new ExecutedRoutedEventHandler(FindExec), new CanExecuteRoutedEventHandler(FindCanExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Stop, new ExecutedRoutedEventHandler(StopExec)));

			IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnIsVisibleChanged);

			mResults.Albums = mAlbums;
			mResults.ProgressTextChanged += new EventHandler(OnResultsProgressTextChanged);
			mResults.StateChanged += new EventHandler(OnResultsStateChanged);
		}

		private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				//Window is being shown.
				ConnectToFoobar();
			}
		}

		private void ConnectToFoobar()
		{
			try
			{
				if (mPlaylists != null)
					mPlaylists.Dispose();

				mFoobar = new Foobar2000.Application07Class();
				mPlaylists = new PlaylistsCollection(mFoobar);
			}
			catch (Exception ex)
			{
				mFoobar = null;
				mPlaylists = null;
				System.Diagnostics.Trace.TraceWarning("Foobar2000 COM server could not be instantiated: " + ex.Message);
			}
			NotifyPropertyChanged("FoobarPresent");
			NotifyPropertyChanged("FoobarVersion");
			NotifyPropertyChanged("FoobarPlaylists");
			SelectedPlaylistIndex = 0; //Should always be "Entire Library"
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);
			SaveSettings();
		}
		protected override void OnClosed(EventArgs e)
		{
			AbortSearch();
			mResults.Dispose(); //Closes down the search thread
			
			if (mPlaylists != null)
			{
				mPlaylists.Dispose();
				mPlaylists = null;
			}
			base.OnClosed(e);
		}

		#region Settings
		private void LoadPathPatternHistory()
		{
			ICollection<String> history = mImagePathPatternBox.History;
			history.Clear();
			foreach (string historyItem in Properties.Settings.Default.FileBrowseImagePathHistory)
			{
				history.Add(historyItem);
			}
		}

		private void SavePathPatternHistory()
		{
			ICollection<String> history = mImagePathPatternBox.History;

			Properties.Settings.Default.FileBrowseImagePathHistory.Clear();
			foreach (string historyItem in history)
			{
				Properties.Settings.Default.FileBrowseImagePathHistory.Add(historyItem);
			}
		}

		public void SaveSettings()
		{
			SavePathPatternHistory();
			Properties.Settings.Default.FoobarBrowserResultsGrid = mResults.GetSettings();
		}
		public void LoadSettings()
		{
			LoadPathPatternHistory();
			mResults.ApplySettings(Properties.Settings.Default.FoobarBrowserResultsGrid);
		}
		#endregion

		#region Properties
		string IAppWindow.Description
		{
			get
			{
				return "Foobar Browser";
			}
		}
		public static readonly DependencyPropertyKey StatePropertyKey = DependencyProperty.RegisterReadOnly("State", typeof(BrowserState), typeof(FoobarBrowser), new FrameworkPropertyMetadata(BrowserState.Ready));
		public BrowserState State
		{
			get { return (BrowserState)GetValue(StatePropertyKey.DependencyProperty); }
			private set { SetValue(StatePropertyKey, value); }
		}

		public static readonly DependencyPropertyKey ProgressTextPropertyKey = DependencyProperty.RegisterReadOnly("ProgressText", typeof(string), typeof(FoobarBrowser), new FrameworkPropertyMetadata(String.Empty));
		public string ProgressText
		{
			get { return (string)GetValue(ProgressTextPropertyKey.DependencyProperty); }
			private set { SetValue(ProgressTextPropertyKey, value); }
		}
		public static readonly DependencyPropertyKey ProgressPropertyKey = DependencyProperty.RegisterReadOnly("Progress", typeof(double), typeof(FoobarBrowser), new FrameworkPropertyMetadata(0D));
		public double Progress
		{
			get { return (double)GetValue(ProgressPropertyKey.DependencyProperty); }
			private set { SetValue(ProgressPropertyKey, value); }
		}
		public static readonly DependencyPropertyKey ProgressMaxPropertyKey = DependencyProperty.RegisterReadOnly("ProgressMax", typeof(double), typeof(FoobarBrowser), new FrameworkPropertyMetadata(0D));
		public double ProgressMax
		{
			get { return (double)GetValue(ProgressMaxPropertyKey.DependencyProperty); }
			private set { SetValue(ProgressMaxPropertyKey, value); }
		}
		public static readonly DependencyPropertyKey ErrorTextPropertyKey = DependencyProperty.RegisterReadOnly("ErrorText", typeof(string), typeof(FoobarBrowser), new FrameworkPropertyMetadata(String.Empty));
		public string ErrorText
		{
			get { return (string)GetValue(ErrorTextPropertyKey.DependencyProperty); }
			private set { SetValue(ErrorTextPropertyKey, value); }
		}

		/// <summary>
		/// Returns True if the Foobar Automation server is available.
		/// </summary>
		public bool FoobarPresent
		{
			get
			{
				return mFoobar != null;
			}
		}

		public string FoobarVersion
		{
			get
			{
				if (!FoobarPresent)
					return null;

				try
				{
					return mFoobar.Name + ": " + mFoobar.ApplicationPath;
				}
				catch (COMException e)
				{
					return "Could not connect to Foobar2000 automation server: " + e.Message;
				}
			}
		}

		public IList<Foobar2000.Playlist07> FoobarPlaylists
		{
			get { return mPlaylists; }
		}

		private int mSelectedPlaylistIndex = 0;
		public int SelectedPlaylistIndex
		{
			get { return mSelectedPlaylistIndex; }
			set
			{
				if (value != mSelectedPlaylistIndex)
				{
					mSelectedPlaylistIndex = value;
					NotifyPropertyChanged("SelectedPlaylistIndex");
				}
			}
		}
		
		private Foobar2000.Playlist07 SelectedPlaylist
		{
			get { return mPlaylists[SelectedPlaylistIndex]; }
		}
		#endregion

		#region Command Handlers
		private void FindExec(object sender, ExecutedRoutedEventArgs e)
		{
			mImagePathPatternBox.AddPatternToHistory();
			Search(mImagePathPatternBox.PathPattern);
		}
		private void FindCanExec(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = FoobarPresent;
		}

		private void StopExec(object sender, ExecutedRoutedEventArgs e)
		{
			AbortSearch();
		}
		#endregion

		#region Foobar Media Library Searching
		/// <summary>
		/// Begins an asynchronous search of the foobar media library
		/// </summary>
		public void Search(string imagePathPattern)
		{
			//Ensure UI is synched
			mImagePathPatternBox.PathPattern = imagePathPattern;
			mResults.ImagePathPattern = imagePathPattern; //Set this once, rather than binding, so it is kept constant for a search.

			//Ensure a playlist is selected (choose Entire Library if none is)
			SelectedPlaylistIndex = Math.Max(SelectedPlaylistIndex, 0);
			
			AbortSearch(); //Abort any existing search
			mAlbums.Clear(); //Clear existing results
			
			//Test that the selected playlist can be accessed
			try
			{
				SelectedPlaylist.GetTracks(null);
			}
			catch (COMException)
			{
				//Couldn't be accessed. Try reconnecting (this will reset to Entire Library too)
				ConnectToFoobar();

				//Lets try that again now.
				try
				{
					SelectedPlaylist.GetTracks(null);
				}
				catch (COMException e)
				{
					//OK, really give up now.
					SetErrorState("Could not read from Foobar: " + e.Message);
					return;
				}
			}

			//Should be OK, so try reading the library
		
			System.Diagnostics.Debug.Assert(mReadFoobarThread == null, "A media library reader thread already exists!");
			mReadFoobarThread = new Thread(new ThreadStart(ReadFoobarWorker));
			mReadFoobarThread.Name = "Read Foobar";
			mReadFoobarThread.Start();
		}

		/// <summary>
		/// Aborts an asynchronous search, if one is running.
		/// </summary>
		public void AbortSearch()
		{
			//Abort the media file searching
			if (mReadFoobarThread != null)
			{
				mReadFoobarThread.Abort();
				mReadFoobarThread = null;
			}

			mResults.AbortSearch();
		}

		private void ReadFoobarWorker()
		{
			try
			{
				Foobar2000.Tracks07 tracks = SelectedPlaylist.GetTracks(null);
				//Now searching for something, so set the state to indicate that.
				//Also set the count of albums, for the progress bar
				Dispatcher.Invoke(DispatcherPriority.DataBind, new ThreadStart(delegate
				{
					State = BrowserState.FindingFiles;
					Progress = 0;
					ProgressMax = tracks.Count;
					ProgressText = "Reading Media Library...";
				}));
				foreach (Foobar2000.Track07 track in tracks)
				{
					string artistName = track.FormatTitle("%album artist%");
					string albumName = track.FormatTitle("%album%");
					string path = track.FormatTitle("%path%");
					try
					{
						path = System.IO.Path.GetDirectoryName(path);
					}
					catch (Exception e)
					{
						System.Diagnostics.Trace.WriteLine("Could not get file path for \""+artistName + "\" / \"" + albumName + "\": " + path);
						System.Diagnostics.Trace.Indent();
						System.Diagnostics.Trace.WriteLine(e.Message);
						System.Diagnostics.Trace.Unindent();
						continue; //skip this one, can't find the path.
					}
					
					Dispatcher.Invoke(DispatcherPriority.DataBind, new ThreadStart(delegate
					{
						Progress++;
						if (!(String.IsNullOrEmpty(artistName) && String.IsNullOrEmpty(albumName))) //No point adding it if no artist or album could be found.
						{
							mAlbums.Add(new Album(path, artistName, albumName));
						}
					}));
				}

				//Finished with the FindingFiles state, so now set the state to whatever the results state is (either FindingArt, or Done).
				Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
				{
					ProgressText = mResults.ProgressText;
					State = mResults.State;
				}));
			}
			catch (ThreadAbortException)
			{
				Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
				{
					State = BrowserState.Stopped;
				}));
			}
			catch (Exception e)
			{
				uint hResult = (uint)System.Runtime.InteropServices.Marshal.GetHRForException(e);

				if (e is COMException || 
					(hResult == 0x800706BE || //RPC failed
					 hResult == 0x80004002)) //No interface
				{
					SetErrorState("Lost connection to Foobar automation server while reading media library");
				}
				else
				{
					SetErrorState(String.Format("Error occurred while reading media library: {0}", e.Message));
				}
			}
		}

		private void OnResultsStateChanged(object sender, EventArgs e)
		{
			switch (mResults.State)
			{
				case BrowserState.Ready: //Not interested.
				case BrowserState.FindingArt:
					//Not interested, this will be set by SearchWorker when it finishes.
					break;
				case BrowserState.Done:
					//If we're currently FindingArt, then this does mean we're now done
					if (State == BrowserState.FindingArt)
						State = BrowserState.Done;
					break;
				case BrowserState.Error:
				case BrowserState.Stopped:
					//Inidicate this state
					State = mResults.State;
					break;
				case BrowserState.FindingFiles:
					System.Diagnostics.Debug.Fail("Unexpected state: Results should not be finding files");
					break;
				default:
					System.Diagnostics.Debug.Fail("Unexpected state");
					break;
			}
		}

		private void OnResultsProgressTextChanged(object sender, EventArgs e)
		{
			//Not binding, as we want to be able to update the progress text here directly too, but if the results indicates the progress text it would like displayed, we oblige here.
			if (!String.IsNullOrEmpty(mResults.ProgressText))
				ProgressText = mResults.ProgressText;
		}

		/// <summary>
		/// Sets the error state, with error message. Safe to call from the search worker thread.
		/// </summary>
		private void SetErrorState(string message)
		{
			Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
			{
				ProgressText = String.Empty;
				ErrorText = message;
				State = BrowserState.Error;
				NotifyPropertyChanged("FoobarVersion");
			}));
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

		/// <summary>
		/// Wrapper for observable collection of foobar playlists, plus an "Entire Library" item inserted at the top
		/// </summary>
		private class PlaylistsCollection : INotifyCollectionChanged, IList<Foobar2000.Playlist07>, IDisposable
		{
			private Dispatcher mDispatcher;
			private Foobar2000.Playlists07 mPlaylists;
			private Foobar2000.Playlist07 mEntireLibrary;

			public PlaylistsCollection(Foobar2000.Application07 foobar)
			{
				mDispatcher = Dispatcher.CurrentDispatcher;
				mPlaylists = foobar.Playlists;
				mEntireLibrary = new EntireLibraryMockPlaylist(foobar);

				mPlaylists.PlaylistAdded += OnPlaylistAdded;
				mPlaylists.PlaylistRemoved += OnPlaylistRemoved;
				mPlaylists.PlaylistsReordered += OnPlaylistsReordered;
			}

			#region Disposal
			~PlaylistsCollection()
			{
				Dispose(false);
			}

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (mPlaylists != null)
				{
					try
					{
						mPlaylists.PlaylistAdded -= OnPlaylistAdded;
						mPlaylists.PlaylistRemoved -= OnPlaylistRemoved;
						mPlaylists.PlaylistsReordered -= OnPlaylistsReordered;
					}
					catch (Exception) { }
					mPlaylists = null;
				}
			}
			#endregion

			#region Playlist event response
			private void OnPlaylistsReordered(int lFirstIndex, int lLastIndex)
			{
				RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}

			private void OnPlaylistRemoved(int lIndex, Foobar2000.Playlist07 oPlaylist)
			{
				RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oPlaylist, lIndex+ 1));
			}

			private void OnPlaylistAdded(int lIndex, Foobar2000.Playlist07 oPlaylist, string strName)
			{
				RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, oPlaylist, lIndex + 1));
			}

			public event NotifyCollectionChangedEventHandler CollectionChanged;
			private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
			{
				NotifyCollectionChangedEventHandler temp = CollectionChanged;
				if (temp != null)
				{
					mDispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate 
					{
						try
						{
							temp(this, e);
						}
						catch (ArgumentOutOfRangeException ex)
						{
							System.Diagnostics.Trace.TraceWarning("Error occurred when updating playlists collection, probably due to Foobar being closed: " + ex.Message);
							//Do a general reset
							temp(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
						}
						catch (InvalidOperationException ex)
						{
							System.Diagnostics.Trace.TraceWarning("Error occurred when updating playlists collection: " + ex.Message);
							Thread.Sleep(500); //Wait a bit, then try a general reset
							temp(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
						}
					}));
				}
			}
			#endregion

			#region Implemented IList<> members
			public IEnumerator<Foobar2000.Playlist07> GetEnumerator()
			{
				List<Foobar2000.Playlist07> result = new List<Foobar2000.Playlist07>(Count);
				result.Add(mEntireLibrary);
				try
				{
					result.AddRange(mPlaylists.Cast<Foobar2000.Playlist07>());
				}
				catch (COMException)
				{
					//If Foobar is not responsive, can't enumerate its playlists, so just return the Entire Library entry
					return result.GetRange(0, 1).GetEnumerator();
				}
				return result.GetEnumerator();
			}
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return (System.Collections.IEnumerator)GetEnumerator();
			}

			public int Count 
			{
				get
				{
					int innerCount = 0;
					try
					{
						innerCount = mPlaylists.Count;
					}
					catch (COMException) { } //If Foobar stops responding, treat as having 0 count

					return innerCount + 1; //+1 for the Entire Playlist entry
				} 
			}
			
			public Foobar2000.Playlist07 this[int index]
			{
				get
				{
					if (index == 0)
						return mEntireLibrary;

					return mPlaylists[index - 1];
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public bool IsReadOnly
			{
				get { return true; }
			}
			#endregion

			#region Unimplemented IList<> members
			public int IndexOf(Foobar2000.Playlist07 item)
			{
				throw new NotImplementedException();
			}

			public void Insert(int index, Foobar2000.Playlist07 item)
			{
				throw new NotImplementedException();
			}

			public void RemoveAt(int index)
			{
				throw new NotImplementedException();
			}

			public void Add(Foobar2000.Playlist07 item)
			{
				throw new NotImplementedException();
			}

			public void Clear()
			{
				throw new NotImplementedException();
			}

			public bool Contains(Foobar2000.Playlist07 item)
			{
				throw new NotImplementedException();
			}

			public void CopyTo(Foobar2000.Playlist07[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}

			public bool Remove(Foobar2000.Playlist07 item)
			{
				throw new NotImplementedException();
			}
			#endregion

			/// <summary>
			/// Class to mock a <see cref="Foobar2000.Playlist07"/> that has the
			/// entire library as its track listing.
			/// </summary>
			private class EntireLibraryMockPlaylist : Foobar2000.Playlist07
			{
				private Foobar2000.MediaLibrary07 mMediaLibrary;
				public EntireLibraryMockPlaylist(Foobar2000.Application07 foobar)
				{
					mMediaLibrary = foobar.MediaLibrary;
				}

				public Foobar2000.Tracks07 GetSortedTracks(string strSortFormat, object strQuery)
				{
					return mMediaLibrary.GetSortedTracks(strSortFormat, strQuery);
				}

				public Foobar2000.Tracks07 GetTracks(object strQuery)
				{
					return mMediaLibrary.GetTracks(strQuery);
				}

				public int Index
				{
					get { return -1; }
				}

				public string Name
				{
					get
					{
						return "Entire Library";
					}
					set { throw new NotImplementedException(); }
				}

				public bool DoDefaultAction(int lItemIndex)
				{
					throw new NotImplementedException();
				}
			}
		}
	}
}