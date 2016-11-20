using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using AlbumArtDownloader.Controls;

namespace AlbumArtDownloader
{
	public partial class FileBrowser : System.Windows.Window, INotifyPropertyChanged, IAppWindow
	{
		/// <summary>The artist name to use when a Various Artists album is detected</summary>
		private static readonly string sVariousArtistsName = "Various Artists";

		private Thread mMediaFileSearchThread;
		private Queue<SearchThreadParameters> mMediaFileSearchQueue = new Queue<SearchThreadParameters>();
		private AutoResetEvent mMediaFileSearchTrigger = new AutoResetEvent(false);
		private ObservableAlbumCollection mAlbums = new ObservableAlbumCollection();

		public FileBrowser()
		{
			InitializeComponent();

			LoadSettings();

			mBrowse.Click += new RoutedEventHandler(OnBrowseForFilePath);

			CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, new ExecutedRoutedEventHandler(FindExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Stop, new ExecutedRoutedEventHandler(StopExec)));

			mResults.Albums = mAlbums;
			mResults.DragEnter += new DragEventHandler(OnResultsDragEnter);
			mResults.Drop += new DragEventHandler(OnResultsDragDrop);
			mResults.ProgressTextChanged += new EventHandler(OnResultsProgressTextChanged);
			mResults.StateChanged += new EventHandler(OnResultsStateChanged);

			CreateMediaFileSearchThread();
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);
			SaveSettings();
		}
		protected override void OnClosed(EventArgs e)
		{
			AbortSearch();
			if (mMediaFileSearchThread != null)
			{
				mMediaFileSearchThread.Abort();
				mMediaFileSearchThread = null;
			}
			mResults.Dispose(); //Closes down the search thread
			base.OnClosed(e);
		}

		#region Settings
		private void LoadPathPatternHistory(ArtPathPatternBox patternBox, StringCollection settingsHistory)
		{
			ICollection<String> history = patternBox.History;
			history.Clear();
			foreach (string historyItem in settingsHistory)
			{
				history.Add(historyItem);
			}
		}

		private void SavePathPatternHistory(ArtPathPatternBox patternBox, StringCollection settingsHistory)
		{
			ICollection<String> history = patternBox.History;

			settingsHistory.Clear();
			foreach (string historyItem in history)
			{
				settingsHistory.Add(historyItem);
			}
		}

		public void SaveSettings()
		{
			SavePathPatternHistory(mImagePathPatternBox, Properties.Settings.Default.FileBrowseImagePathHistory);
			SavePathPatternHistory(mFilePathPattern, Properties.Settings.Default.FileBrowseFilePathPatternHistory);
			Properties.Settings.Default.FileBrowseResultsGrid = mResults.GetSettings();
		}
		public void LoadSettings()
		{
			LoadPathPatternHistory(mImagePathPatternBox, Properties.Settings.Default.FileBrowseImagePathHistory);
			LoadPathPatternHistory(mFilePathPattern, Properties.Settings.Default.FileBrowseFilePathPatternHistory);
			mResults.ApplySettings(Properties.Settings.Default.FileBrowseResultsGrid);
		}
		#endregion

		#region Properties
		string IAppWindow.Description
		{
			get
			{
				if (String.IsNullOrEmpty(mFilePathBox.Text))
					return "File Browser";

				return "File Browser: " + Path.GetFileName(mFilePathBox.Text);
			}
		}

		public static readonly DependencyPropertyKey StatePropertyKey = DependencyProperty.RegisterReadOnly("State", typeof(BrowserState), typeof(FileBrowser), new FrameworkPropertyMetadata(BrowserState.Ready));
		public BrowserState State
		{
			get { return (BrowserState)GetValue(StatePropertyKey.DependencyProperty); }
			private set { SetValue(StatePropertyKey, value); }
		}

		public static readonly DependencyPropertyKey ProgressTextPropertyKey = DependencyProperty.RegisterReadOnly("ProgressText", typeof(string), typeof(FileBrowser), new FrameworkPropertyMetadata(String.Empty));
		public string ProgressText
		{
			get { return (string)GetValue(ProgressTextPropertyKey.DependencyProperty); }
			private set { SetValue(ProgressTextPropertyKey, value); }
		}
		public static readonly DependencyPropertyKey ErrorTextPropertyKey = DependencyProperty.RegisterReadOnly("ErrorText", typeof(string), typeof(FileBrowser), new FrameworkPropertyMetadata(String.Empty));
		public string ErrorText
		{
			get { return (string)GetValue(ErrorTextPropertyKey.DependencyProperty); }
			private set { SetValue(ErrorTextPropertyKey, value); }
		}
		#endregion

		private void OnBrowseForFilePath(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.FileDialog filePathBrowser = (Microsoft.Win32.FileDialog)Resources["mFilePathBrowser"];
			filePathBrowser.InitialDirectory = mFilePathBox.Text;
			filePathBrowser.FileName = "[Filename will be ignored]";
			if (filePathBrowser.ShowDialog(this).GetValueOrDefault())
			{
				mFilePathBox.Text = System.IO.Path.GetDirectoryName(filePathBrowser.FileName);
			}
		}

		#region Drag and Drop
		private void OnResultsDragEnter(object sender, DragEventArgs e)
		{
			if(e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effects = DragDropEffects.Copy;
		}

		private void OnResultsDragDrop(object sender, DragEventArgs e)
		{
			if(e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				foreach (string filename in (string[])e.Data.GetData(DataFormats.FileDrop))
				{
					EnqueueMediaFileSearch(new SearchThreadParameters(filename, 
																		mIncludeSubfolders.IsChecked.GetValueOrDefault(),
																		mUseFilePathPattern.IsChecked.GetValueOrDefault() ? mFilePathPattern.PathPattern : null
																		));
				}
			}
		}
		#endregion

		#region Command Handlers
		private void FindExec(object sender, ExecutedRoutedEventArgs e)
		{
			mImagePathPatternBox.AddPatternToHistory();
			mFilePathPattern.AddPatternToHistory();
			Search(mFilePathBox.Text,
					mIncludeSubfolders.IsChecked.GetValueOrDefault(),
					mImagePathPatternBox.PathPattern,
					mUseFilePathPattern.IsChecked.GetValueOrDefault() ? mFilePathPattern.PathPattern : null);
		}

		private void StopExec(object sender, ExecutedRoutedEventArgs e)
		{
			AbortSearch();
		}
		#endregion

		#region Media File Searching
		/// <summary>
		/// Begins an asynchronous search for files, reading artist and album information from ID3 tags
		/// </summary>
		public void Search(string rootPath, bool includeSubfolders, string imagePathPattern)
		{
			Search(rootPath, includeSubfolders, imagePathPattern, null);
		}
		/// <summary>
		/// Begins an asynchronous search for files, reading artist and album information from the file
		/// path, according to <paramref name="filePathPattern"/>. If <paramref name="filePathPattern"/> is
		/// null, uses ID3 tags instead
		/// </summary>
		public void Search(string rootPath, bool includeSubfolders, string imagePathPattern, string filePathPattern)
		{
			//Ensure UI is synched
			mFilePathBox.Text = rootPath;
			mIncludeSubfolders.IsChecked = includeSubfolders;
			mImagePathPatternBox.PathPattern = imagePathPattern;

			AbortSearch(); //Abort any existing search
			mAlbums.Clear(); //Clear existing results

			EnqueueMediaFileSearch(new SearchThreadParameters(rootPath, includeSubfolders, filePathPattern));
		}

		/// <summary>
		/// Aborts an asynchronous search, if one is running.
		/// </summary>
		public void AbortSearch()
		{
			//Abort the media file searching
			lock (mMediaFileSearchQueue)
			{
				mMediaFileSearchQueue.Clear();
			}
			mMediaFileSearchThread.Abort();
			mMediaFileSearchThread = null;
			CreateMediaFileSearchThread();
			mResults.AbortSearch();
		}

		private void EnqueueMediaFileSearch(SearchThreadParameters parameters)
		{
			lock (mMediaFileSearchQueue)
			{
				mMediaFileSearchQueue.Enqueue(parameters);
			}
			mMediaFileSearchTrigger.Set();
		}

		private void CreateMediaFileSearchThread()
		{
			System.Diagnostics.Debug.Assert(mMediaFileSearchThread == null, "A media file search thread already exists!");
			mMediaFileSearchThread = new Thread(new ThreadStart(SearchWorker));
			mMediaFileSearchThread.Name = "File Browser Search";
			mMediaFileSearchThread.Start();
		}

		private void SearchWorker()
		{
			try
			{
				do
				{
					mMediaFileSearchTrigger.WaitOne(); //Wait until there is work to do

					Dispatcher.Invoke(DispatcherPriority.DataBind, new ThreadStart(delegate
					{
						mResults.ImagePathPattern = mImagePathPatternBox.PathPattern; //Set this once, rather than binding, so it is kept constant for a search.
					}));

					do //Loop through all the queued searches.
					{
						SearchThreadParameters parameters;
						lock (mMediaFileSearchQueue)
						{
							if (mMediaFileSearchQueue.Count == 0)
							{
								break; //Nothing to search for, so go back and wait until there is.
							}
							else
							{
								parameters = mMediaFileSearchQueue.Dequeue();
							}
						}
						//Now searching for something, so set the state to indicate that.
						Dispatcher.Invoke(DispatcherPriority.DataBind, new ThreadStart(delegate
						{
							State = BrowserState.FindingFiles;
							ProgressText = "Searching...";
						}));

						Regex pathPattern = null;
						try
						{
							pathPattern = parameters.CreatePathPatternRegex(); //This will be null if no path pattern is used (ID3's used instead)
						}
						catch (ArgumentException e)
						{
							SetErrorState("Error parsing file pattern regular expression: " + e.Message);
							continue;
						}
						
						DirectoryInfo root = null;
						try
						{
							root = new DirectoryInfo(parameters.RootPath);
						}
						catch (Exception e)
						{
							//Path not valid, so no images to find
							System.Diagnostics.Trace.WriteLine("Path not valid for file search: " + parameters.RootPath);
							System.Diagnostics.Trace.Indent();
							System.Diagnostics.Trace.WriteLine(e.Message);
							System.Diagnostics.Trace.Unindent();
							SetErrorState(String.Format("Could not search \"{0}\": {1}", parameters.RootPath, e.Message));
							return;
						}

						//Keep a list of all the albums added in this pass, for Various Artists detection.
						List<Album> addedAlbums = new List<Album>();

						if ((root.Attributes & FileAttributes.Directory) != FileAttributes.Directory)
						{
							//This isn't a directory, so try reading it as a single media file
							addedAlbums.Add(ReadMediaFile(new FileInfo(root.FullName), pathPattern));
						}
						else
						{
							//It is a directory, so read its contents

							try
							{
								if (parameters.FindDirectories)
								{
									//Include self
									addedAlbums.Add(ReadMediaFile(root, pathPattern));
								}

								foreach (FileSystemInfo file in NetMatters.FileSearcher.GetFileSystemInfos(root, "*", parameters.IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly, parameters.FindDirectories, !parameters.FindDirectories))
								{
									addedAlbums.Add(ReadMediaFile(file, pathPattern));
								}
							}
							catch (Exception e)
							{
								SetErrorState("Error occurred while searching: " + e.Message);
								continue;
							}
						}

						DetectVariousArtistsAlbums(addedAlbums);

					} while (true);
					//Will break out here when there are no more searches queued

					//Finished with the FindingFiles state, so now set the state to whatever the results state is (either FindingArt, or Done).
					Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
					{
						if (State != BrowserState.Error)
						{
							ProgressText = mResults.ProgressText;
							State = mResults.State;
						}
					}));
					//Return and wait until new searches are queued
				} while (true);
			}
			catch (ThreadAbortException)
			{
				Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
				{
					State = BrowserState.Stopped;
				}));
				return;
			}
		}

		/// <summary>Reads the album and artist data from specified media file or folder, and adds it to the <see cref="mAlbums"/> collection.</summary>
		/// <param name="filePathPattern">Use null to use the ID3 tags instead</param>
		/// <returns>The <see cref="Album"/> that was added to <see cref="mAlbums"/>, or <c>null</c> if none could be read.</returns>
		private Album ReadMediaFile(FileSystemInfo file, Regex filePathPattern)
		{
			if (file is DirectoryInfo && filePathPattern == null) //If a DirectoryInfo is used, then the filePathPattern must have ended in \.
			{
				throw new ArgumentException("Directories are only supported for pattern matching, not ID3 tags", "file");
			}

			Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
			{
				ProgressText = "Searching... " + file.Name;
			}));

			string artistName = null;
			string albumName = null;
			int? embeddedArtIndex = null;
			if (filePathPattern == null)
			{
				//Read ID3 Tags
				TagLib.File fileTags = null;
				try
				{
					fileTags = TagLib.File.Create(file.FullName, TagLib.ReadStyle.None);
					if (fileTags.Tag.AlbumArtists.Length == 0)
					{
						artistName = String.Join(" / ", fileTags.Tag.Performers);
					}
					else
					{
						artistName = String.Join(" / ", fileTags.Tag.AlbumArtists);
					}
					albumName = fileTags.Tag.Album;

					var embeddedPictures = fileTags.Tag.Pictures;
					if (embeddedPictures.Length > 0)
					{
						//There's an embedded picture
						//Check to see if there's a picture described as the front cover, to use in preference
						for (int i = 0; i < embeddedPictures.Length; i++)
						{
							if (embeddedPictures[i].Type == TagLib.PictureType.FrontCover)
							{
								embeddedArtIndex = i;
								break;
							}
						}
						if(!embeddedArtIndex.HasValue)
						{
							//None of the embedded pictures were tagged as "FrontCover", so just use the first picture
							embeddedArtIndex = 0;
						}
					}
				}
				catch (Exception e)
				{
					System.Diagnostics.Trace.WriteLine("TagLib# could not get artist and album information for file: " + file.FullName);
					System.Diagnostics.Trace.Indent();
					System.Diagnostics.Trace.WriteLine(e.Message);
					System.Diagnostics.Trace.Unindent();
					return null; //If this media file couldn't be read, just go on to the next one.				
				}
				finally
				{
					if (fileTags != null)
					{
						fileTags.Mode = TagLib.File.AccessMode.Closed;
					}
				}
			}
			else
			{
				//Read from file path
				Match match = filePathPattern.Match(file.FullName);
				if (match.Success)
				{
					artistName = match.Groups["artist"].Value;
					albumName = match.Groups["album"].Value;
				}
			}

			if (!(String.IsNullOrEmpty(artistName) && String.IsNullOrEmpty(albumName))) //No point adding it if no artist or album could be found.
			{
				string basePath;
				if (file is FileInfo)
				{
					basePath = ((FileInfo)file).DirectoryName;
				}
				else
				{
					System.Diagnostics.Debug.Assert(file is DirectoryInfo, "Expecting file to be one of FileInfo or DirectoryInfo");
					basePath = ((DirectoryInfo)file).FullName;
				}

				Album album = new Album(basePath, artistName, albumName);
				if (embeddedArtIndex.HasValue)
				{
					//Read the picture from the data
					album.SetArtFile(EmbeddedArtHelpers.GetEmbeddedFilePath(file.FullName, embeddedArtIndex.Value));
				}

				Dispatcher.Invoke(DispatcherPriority.DataBind, new ThreadStart(delegate
				{
					mAlbums.Add(album);
				}));

				return album;
			}

			return null;
		}

		/// <summary>
		/// Checks through a set of albums for those that should be concatenated into a single Various Artists album.
		/// </summary>
		/// <param name="addedAlbums"></param>
		private void DetectVariousArtistsAlbums(IList<Album> albums)
		{
			//Get the albums in each folder
			Dictionary<String, List<Album>> folders = new Dictionary<string, List<Album>>(albums.Count);
			foreach (Album album in albums)
			{
				if (album != null)
				{
					List<Album> folderAlbums;
					if (!folders.TryGetValue(album.BasePath, out folderAlbums))
					{
						folderAlbums = new List<Album>(albums.Count);
						folders.Add(album.BasePath, folderAlbums);
					}

					folderAlbums.Add(album);
				}
			}

			foreach (List<Album> folder in folders.Values)
			{
				//If the folder contains all the same album name, but different artists, then it is VA
				string albumName = folder[0].Name;
				string artist = folder[0].Artist;
				string basePath = folder[0].BasePath;

				bool variousArtists = false;
				foreach (Album album in folder)
				{
					if (album.Name != albumName)
					{
						//This is not all the same album
						variousArtists = false;
						break;
					}
					if (!variousArtists && album.Artist != artist)
					{
						variousArtists = true;
					}
				}

				if (variousArtists)
				{
					//This is a various artists album, so replace them with a single Various Artists album
					Dispatcher.Invoke(DispatcherPriority.DataBind, new ThreadStart(delegate
					{
						Album variousArtistsAlbum = new Album(basePath, sVariousArtistsName, albumName);
						if (!mAlbums.Contains(variousArtistsAlbum)) //If this album already exists, no need to add it again.
						{
							//Add replacement VA album
							mAlbums.Insert(mAlbums.IndexOf(folder[0]), variousArtistsAlbum);
						}
						
						//Remove individual albums, but copy art from at least one of them (if present)
						foreach (Album album in folder)
						{
							if(album.Artist != sVariousArtistsName) //If, by chance, one of the albums was already called "Various Artists", then it will have merged with the newly added one.
							{
								if (variousArtistsAlbum.ArtFileStatus != ArtFileStatus.Present && album.ArtFile != null)
								{
									variousArtistsAlbum.SetArtFile(album.ArtFile);
								}
								mAlbums.Remove(album);
							}
						}
					}));
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

		#region Search Thread Parameters
		private class SearchThreadParameters
		{
			/// <param name="pathPattern">Use null to indicate that ID3 tag matching should be used instead.</param>
			public SearchThreadParameters(string rootPath, bool includeSubfolders, string pathPattern)
			{
				RootPath = rootPath;
				IncludeSubfolders = includeSubfolders;
				PathPattern = pathPattern;

				//If the path pattern ends with \. (or /.) then this is a flag that directory matching should be used
				FindDirectories = pathPattern != null && (pathPattern.EndsWith(@"\.") || pathPattern.EndsWith(@"/."));
			}
			public string RootPath { get ; private set; }
			public bool IncludeSubfolders { get; private set; }
			public bool FindDirectories { get; private set; }
			public string PathPattern { get; private set; }

			/// <summary>
			/// Creates a regular expression from PathPattern that will match with named
			/// groups "artist" and "album".
			/// </summary>
			/// <returns></returns>
			public Regex CreatePathPatternRegex()
			{
				string regex = PathPattern;

				if (regex == null)
					return null; //No path matching (use ID3 tags)

				if(FindDirectories)
				{
					//Remove the \. from the end of the string, as it won't actually feature in the path as found, so wouldn't match
					System.Diagnostics.Debug.Assert(regex.EndsWith(@"\.") || regex.EndsWith(@"/."), "Shouldn't be removing this ending if it isn't there!");
					regex = regex.Substring(0, regex.Length - 2);
				}

				regex = Regex.Escape(regex) + "$"; //Start by escaping the whole thing, and requiring it to be the end of the path.
				regex = Regex.Replace(regex, @"(\\\\|/)", @"[\\/]"); //Replace all / or \ characters with [\/] to allow matching either path character
				regex = Regex.Replace(regex, @"\\(\*|\?)", @"[^\\/]$1?"); //Replace * (and ?) with [^\/]*? to match within the path segment only, and non-greedy
				//Restore literal regex parts

				Match literalRegexParts = Regex.Match(PathPattern, "\"\\s*([^\"]+?)\\s*\"");
				regex = Regex.Replace(regex, "\"[^\"]+\"", new MatchEvaluator(
					delegate(Match match)
					{
						//Read the value of the corresponding literal regex part
						string result = literalRegexParts.Groups[1].Value;
						//Prepare the next literal regex part ready for the next replacement.
						literalRegexParts = literalRegexParts.NextMatch();
						return result;
					}
				));

				regex = Regex.Replace(regex, "%(artist|album)%", @"(?<$1>[^\\/]+?)"); //Replace artist and album placeholders with capturing groups matching non-greedy in the path segment only.
				return new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);
			}
		}
		#endregion
	}
}