using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.ComponentModel;

namespace AlbumArtDownloader
{
	internal class LocalFilesSource : Source
	{
		[DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal static extern int GdipCreateBitmapFromFile(string filename, out IntPtr bitmap);
		[DllImport("gdiplus.dll", ExactSpelling = true)]
		private static extern int GdipDisposeImage(HandleRef image);

		public LocalFilesSource()
		{
			//Ensure GDI+ is initialised
			Pen pen = Pens.Black;
		}

		public override string Name
		{
			get { return "Local Files"; }
		}

		public override string Author
		{
			get { return "Alex Vallat"; }
		}

		public override string Version
		{
			get { return typeof(LocalFilesSource).Assembly.GetName().Version.ToString(); }
		}

		public string GetSearchPath(string artist, string album)
		{
			string pathPattern = SearchPathPattern;
			
			//Replace %folder% and %filename% placeholders with the appropriate bits of the default file path
			string folder, filename;
			int lastSeparator = DefaultFilePath.LastIndexOfAny(new[] { '\\', '/' });
			if (lastSeparator > -1)
			{
				folder = DefaultFilePath.Substring(0, lastSeparator);
				filename = DefaultFilePath.Substring(lastSeparator + 1);
			}
			else
			{
				//No \'s in the path, so the whole thing is the filename and there is no folder part.
				folder = "";
				filename = DefaultFilePath;
			}
			pathPattern = pathPattern.Replace("%folder%", folder)
							.Replace("%filename%", filename);

			//Now replace all the other placeholders, and return the result
			return Common.SubstitutePlaceholders(pathPattern, artist, album);
		}

		protected override void SearchInternal(string artist, string album, AlbumArtDownloader.Scripts.IScriptResults results)
		{
			//Add the pattern used to the history list.
			CustomSettingsUI.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.DataBind, new System.Threading.ThreadStart(delegate
			{
				((LocalFilesSourceSettings)CustomSettingsUI).mSearchPathPatternBox.AddPatternToHistory();
			}));

			//Avoid duplicates
			StringDictionary addedFiles = new StringDictionary();

			string pathPattern = GetSearchPath(artist, album);

			foreach (string alternate in pathPattern.Split('|'))
			{
				int? embeddedIndex = null;
				string unembeddedPathPattern = alternate;
					
				if (EmbeddedArtHelpers.IsEmbeddedArtPath(alternate))
				{
					//TODO: Allow a pattern to specify multiple embedded images as "<?>" or similar.
					int i;
					EmbeddedArtHelpers.SplitToFilenameAndIndex(alternate, out unembeddedPathPattern, out i);
					embeddedIndex = i;
				}

				//Match path with wildcards
				foreach (string filename in Common.ResolvePathPattern(unembeddedPathPattern))
				{
					if (!addedFiles.ContainsKey(filename)) //Don't re-add a file that's already been added
					{
						addedFiles.Add(filename, null);

						if (embeddedIndex.HasValue)
						{
							//Read embedded image from file, rather than the file itself as an image
							TagLib.File fileTags = null;
							try
							{
								fileTags = TagLib.File.Create(filename, TagLib.ReadStyle.None);

								var embeddedPictures = fileTags.Tag.Pictures;
								if (embeddedIndex.Value == -1) //Special value indicating "all embedded images"
								{
									for (int i = 0; i < embeddedPictures.Length; i++)
									{
										AddEmbeddedPictureToResults(results, embeddedPictures, i, filename);
									}
								}
								else if (embeddedPictures.Length > embeddedIndex.Value)
								{
									//Found the embedded image
									AddEmbeddedPictureToResults(results, embeddedPictures, embeddedIndex.Value, filename);
								}
								else
								{
									System.Diagnostics.Trace.WriteLine("Skipping file missing specified embedded image in local file search: " + EmbeddedArtHelpers.GetEmbeddedFilePath(filename, embeddedIndex.Value));
								}
							}
							catch (Exception e)
							{
								System.Diagnostics.Trace.WriteLine("Skipping unreadable embedded image from file in local file search: " + EmbeddedArtHelpers.GetEmbeddedFilePath(filename, embeddedIndex.Value));
								System.Diagnostics.Trace.Indent();
								System.Diagnostics.Trace.WriteLine(e.Message);
								System.Diagnostics.Trace.Unindent();
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
							//Each filename is potentially an image, so try to load it
							try
							{
								IntPtr hBitmap;
								int status = GdipCreateBitmapFromFile(filename, out hBitmap);
								GdipDisposeImage(new HandleRef(this, hBitmap));
								if (status == 0)
								{
									//Successfully opened as image

									//Create an in-memory copy so that the bitmap file isn't in use, and can be replaced
									byte[] fileBytes = File.ReadAllBytes(filename); //Read the file, closing it after use
									Bitmap bitmap = new Bitmap(new MemoryStream(fileBytes)); //NOTE: Do not dispose of MemoryStream, or it will cause later saving of the bitmap to throw a generic GDI+ error (annoyingly)
									results.Add(bitmap, Path.GetFileName(filename), filename, bitmap.Width, bitmap.Height, null);
								}
								else
								{
									System.Diagnostics.Trace.WriteLine("Skipping non-bitmap file in local file search: " + filename);
								}
							}
							catch (Exception e)
							{
								System.Diagnostics.Trace.WriteLine("Skipping unreadable file in local file search: " + filename);
								System.Diagnostics.Trace.Indent();
								System.Diagnostics.Trace.WriteLine(e.Message);
								System.Diagnostics.Trace.Unindent();
							}
						}
					}
				}
			}
		}

		private static void AddEmbeddedPictureToResults(AlbumArtDownloader.Scripts.IScriptResults results, TagLib.IPicture[] embeddedPictures, int embeddedIndex, string filename)
		{
			//Create an in-memory copy so that the bitmap file isn't in use, and can be replaced
			Bitmap bitmap = new Bitmap(new MemoryStream(embeddedPictures[embeddedIndex].Data.Data)); //NOTE: Do not dispose of MemoryStream, or it will cause later saving of the bitmap to throw a generic GDI+ error (annoyingly)
			results.Add(bitmap, EmbeddedArtHelpers.GetEmbeddedFilePath(Path.GetFileName(filename), embeddedIndex), filename, bitmap.Width, bitmap.Height, null);
		}

		internal override Bitmap RetrieveFullSizeImage(object fullSizeCallbackParameter)
		{
			return (Bitmap)fullSizeCallbackParameter;
		}

		private string mSearchPathPattern;
		/// <summary>
		/// The path pattern to search for images in.
		/// </summary>
		public string SearchPathPattern
		{
			get 
			{
				return mSearchPathPattern;
			}
			set 
			{
				if (mSearchPathPattern != value)
				{
					mSearchPathPattern = value;

					SettingsChanged = true;
					NotifyPropertyChanged("SearchPathPattern");
				}
			}
		}

		private string mDefaultFilePath;
		/// <summary>
		/// The default file path, used to populate the %folder% and %filename% placeholders in SearchPathPattern
		/// </summary>
		public string DefaultFilePath
		{
			get { return mDefaultFilePath; }
			set
			{
				if (mDefaultFilePath != value)
				{
					mDefaultFilePath = value;
					NotifyPropertyChanged("DefaultFilePath");
				}
			}
		}

		#region Settings
		protected override SourceSettings GetSettings()
		{
			return base.GetSettingsCore(Settings.Creator);
		}

		protected override void LoadSettingsInternal(SourceSettings settings)
		{
			base.LoadSettingsInternal(settings);

			Settings localFilesSourceSettings = (Settings)settings;
			SearchPathPattern = localFilesSourceSettings.SearchPathPattern;
			
			LoadPathPatternHistory(localFilesSourceSettings);
		}
		
		protected override void SaveSettingsInternal(SourceSettings settings)
		{
			Settings localFilesSourceSettings = (Settings)settings;
			localFilesSourceSettings.SearchPathPattern = mSearchPathPattern;

			SavePathPatternHistory(localFilesSourceSettings);

			base.SaveSettingsInternal(localFilesSourceSettings);
		}

		private void LoadPathPatternHistory(Settings localFilesSourceSettings)
		{
			ICollection<String> history = ((LocalFilesSourceSettings)CustomSettingsUI).mSearchPathPatternBox.History;
			history.Clear();
			foreach (string historyItem in localFilesSourceSettings.SearchPathPatternHistory)
			{
				history.Add(historyItem);
			}
		}

		private void SavePathPatternHistory(Settings localFilesSourceSettings)
		{
			ICollection<String> history = ((LocalFilesSourceSettings)CustomSettingsUI).mSearchPathPatternBox.History;

			localFilesSourceSettings.SearchPathPatternHistory.Clear();
			foreach (string historyItem in history)
			{
				localFilesSourceSettings.SearchPathPatternHistory.Add(historyItem);
			}
		}

		protected override System.Windows.Controls.Control CreateCustomSettingsUI()
		{
			return new LocalFilesSourceSettings();
		}

		internal class Settings : SourceSettings
		{
			#region Creation
			//SourceSettings overrides should provide custom versions of these too
			public new static SourceSettingsCreator Creator
			{
				get
				{
					return new SourceSettingsCreator(Create);
				}
			}
			private static SourceSettings Create(string name)
			{
				return new Settings(name);
			}
			#endregion

			public Settings(string sourceName): base(sourceName)
			{
			}

			public override void Upgrade()
			{
				base.Upgrade();
				if (String.IsNullOrEmpty(SearchPathPattern))
				{
					//Previous setting was to not use the search path pattern, so replicate that.
					SearchPathPattern = "%folder%\\%filename%|%folder%\\*<*>";
				}
			}

			[DefaultSettingValueAttribute("%folder%\\%filename%|%folder%\\*<*>")]
			[UserScopedSetting]
			public string SearchPathPattern
			{
				get
				{
					return ((string)(this["SearchPathPattern"]));
				}
				set
				{
					this["SearchPathPattern"] = value;
				}
			}

			[DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" />")]
			[UserScopedSetting]
			public StringCollection SearchPathPatternHistory
			{
				get
				{
					return ((StringCollection)(this["SearchPathPatternHistory"]));
				}
				set
				{
					this["SearchPathPatternHistory"] = value;
				}
			}
		}
		#endregion
	}
}
