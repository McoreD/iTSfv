using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.Windows;
using AlbumArtDownloader.Scripts;
using AlbumArtDownloader.Controls;
using Microsoft.Win32;

namespace AlbumArtDownloader
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class App : System.Windows.Application, IPriorInstance
	{
		internal static readonly string DotNetDownloadPage = "http://www.microsoft.com/downloads/details.aspx?FamilyId=AB99342F-5D1A-413D-8319-81DA479AB0D7";

		/// <summary>
		/// Framework 3.5 prior to SP1 has some bugs that need workarounds.
		/// </summary>
		public static bool UsePreSP1Compatibility { get; private set; }

		public static bool RestartOnExit { get; set; }

		/// <summary>
		/// Application Entry Point.
		/// </summary>
		[System.STAThreadAttribute()]
		public static void Main(string[] args)
		{
			try
			{
				#region .net framework problem detection
				bool foundNet35 = false;
				bool foundNet35SP1 = false;

				try
				{
					using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5"))
					{
						foundNet35 = (key.GetValue("Install") as int?).GetValueOrDefault() == 1;
						foundNet35SP1 = (key.GetValue("SP") as int?).GetValueOrDefault() >= 1;
					}
				}
				catch (Exception e)
				{
					//If there was an exception, then it probably isn't installed
					System.Diagnostics.Trace.TraceError("Could not find .net 3.5 framework: " + e.Message);
				}
				if (!foundNet35)
				{
					if (MessageBox.Show("The required Microsoft .NET Framework version 3.5 is not installed. Album Art Downloader XUI will now exit.\n\nWould you like to visit the download page now?", "Album Art Downloader XUI", MessageBoxButton.YesNo, MessageBoxImage.Stop) == MessageBoxResult.Yes)
					{
						System.Diagnostics.Process.Start(DotNetDownloadPage);
					}
					Environment.Exit(-1); //Ensure exit
					return;
				}
				else if (!foundNet35SP1)
				{
					//Show the SP1 missing dialog
					if (!AlbumArtDownloader.Properties.Settings.Default.IgnoreSP1Missing)
					{
						if (!new MissingFrameworkSP1().ShowDialog().GetValueOrDefault())
						{
							//Save the "Don't show me again" setting
							try
							{
								AlbumArtDownloader.Properties.Settings.Default.Save();
							}
							catch (Exception ex)
							{
								System.Diagnostics.Trace.TraceError("Could not save main settings: " + ex.Message);
							}


							Environment.Exit(-1); //Ensure exit
							return;
						}
					}
					App.UsePreSP1Compatibility = true;

				}
				#endregion

				#region Config File Problem detection
				try
				{
					string appVersion = AlbumArtDownloader.Properties.Settings.Default.ApplicationVersion;
					System.Diagnostics.Trace.TraceInformation("Successfully read application version from settings: " + appVersion);
				}
				catch (ConfigurationErrorsException ex)
				{
					System.Diagnostics.Trace.TraceError("Could not load settings: " + ex.Message);

					//Show the self-service config file problem solver
					new ConfigFileProblem(ex).ShowDialog();
					return;
				}
				#endregion

				if (Array.Exists(args, new Predicate<string>(delegate(string arg) { return arg.Length > 1 && arg.Substring(1).Equals("separateInstance", StringComparison.OrdinalIgnoreCase); })))
				{
					//Start a separate process instance
					new AlbumArtDownloader.App().Run();
				}
				else
				{
					//Start a single-instance process, or connect to an existing one
					const string channelUri = "net.pipe://localhost/AlbumArtDownloader/SingleInstance";
					if (!InstanceMutex.QueryPriorInstance(args, channelUri))
					{
						InstanceMutex.RunAppAsServiceHost(new AlbumArtDownloader.App(), channelUri);
					}
				}
			}
#if ERROR_REPORTING
			catch (Exception e)
			{
				StreamWriter errorLog = null;

				Assembly entryAssembly = Assembly.GetEntryAssembly();
				string filename = Path.Combine(Path.GetDirectoryName(entryAssembly.Location), "errorlog.txt");
				try
				{
					errorLog = File.CreateText(filename);
				}
				catch(Exception)
				{
					try
					{
						filename = Path.Combine(Path.GetTempPath(), "AAD_errorlog.txt");
						errorLog = File.CreateText(filename);
					}
					catch (Exception logError)
					{
						MessageBox.Show("Album Art Downloader has encountered a fatal error, and has had to close.\n\nAdditionally, an error occured when trying to write an error log file: " + filename + "\n\n" + logError.Message);
					}
				}
				if (errorLog != null)
				{
					using (errorLog)
					{
						errorLog.WriteLine("Album Art Downloader has encountered a fatal error, and has had to close.");
						errorLog.WriteLine("If you wish to report this error, please include this information, which");
						errorLog.WriteLine("has been written to the file: " + filename);
						errorLog.WriteLine();
						errorLog.WriteLine("App version: {0}, running on {1} ({2} bit)", entryAssembly.GetName().Version, Environment.OSVersion, IntPtr.Size == 8 ? "64" : "32");
						errorLog.WriteLine();
						errorLog.WriteLine(e);
					}
					System.Diagnostics.Process.Start(filename);
				}
				Environment.Exit(-1); //Ensure exit
			}
#endif
			finally
			{
				if (RestartOnExit)
				{
					System.Diagnostics.Process.Start(Assembly.GetEntryAssembly().Location, Common.GetCommandArgs());
				}
			}
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			UpgradeSettings();
			
#if EPHEMERAL_SETTINGS
			AlbumArtDownloader.Properties.Settings.Default.Reset();
#endif

			AssignDefaultSettings();
			ApplyDefaultProxyCredentials();

			//Assign the application-wide GoToPage hyperlink handler
			System.Windows.Input.CommandManager.RegisterClassCommandBinding(typeof(Window), new System.Windows.Input.CommandBinding(System.Windows.Input.NavigationCommands.GoToPage, new System.Windows.Input.ExecutedRoutedEventHandler(GoToPageExec)));

			//Only shut down if the Exit button is pressed
			ShutdownMode = ShutdownMode.OnExplicitShutdown;
			if (!Splashscreen.ShowIfRequired())
			{
				//Splashscreen returned false, so exit
				Shutdown();
				return;
			}

			LoadScripts(); //TODO: Should this be done showing the splashscren? Faliures to load scripts could be shown in the details area...

			//Now shut down when all the windows are closed
			ShutdownMode = ShutdownMode.OnLastWindowClose;

			if (!ProcessCommandArgs(e.Args))
			{
				Shutdown();
				return;
			}
		}

		/// <summary>
		/// Called when a new instance of the application was run, and this instance was already running.
		/// </summary>
		public void Signal(string[] args)
		{
			Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Threading.ThreadStart(delegate
			{
				ProcessCommandArgs(args);
			}));
			//No need to exit if no window was shown.
		}

		/// <summary>
		/// Process command args. Returns True if a new search window was shown, False if it was not.
		/// </summary>
		private bool ProcessCommandArgs(string[] args)
		{
			//valuedParameters is a list of parameters which must have values - they can not be just switches.
			string[] valuedParameters = { "artist", "ar", "album", "al", "path", "p", "localimagespath", 
										  "sources", "s", "exclude", "es", "include", "i", 
										  "sort", "o", "group", "g", "minsize", "mn", "maxsize", "mx",
										  "covertype", "t"};
			Arguments arguments = new Arguments(args, valuedParameters);
			if (arguments.Contains("?"))
			{
				ShowCommandArgs();
				return false;
			}

			bool? autoClose = null;
			bool showSearchWindow = false, showFileBrowser = false, showFoobarBrowser = false, showConfigFile = false;
			bool startFoobarBrowserSearch = false;
			bool showMinimized = false;
			bool forceNewWindow = false;

			string artist = null, album = null, path = null, localImagesPath = null, fileBrowser = null, sortField = null;
			ListSortDirection sortDirection = ListSortDirection.Ascending;
			Grouping? grouping = null;
			int? minSize = null, maxSize = null;
			AllowedCoverType? coverType = null;

			List<String> useSources = new List<string>();
			List<String> excludeSources = new List<string>();
			List<String> includeSources = new List<string>();
			string errorMessage = null;
			bool skipNext = false;
			foreach (Parameter parameter in arguments)
			{
				if (skipNext)
				{
					skipNext = false;
					continue;
				}
				//Check un-named parameters
				if (parameter.Name == null)
				{
					showSearchWindow = true;

					//For un-named parameters, use compatibility mode: 3 args,  "<artist>" "<album>" "<path to save image>"
					switch (arguments.IndexOf(parameter))
					{
						case 0:
							artist = parameter.Value;
							break;
						case 1:
							album = parameter.Value;
							break;
						case 2:
							path = parameter.Value;
							break;
						default:
							errorMessage = "Only the first three parameters may be un-named";
							break;
					}
				}
				else
				{
					//Check named parameters
					switch (parameter.Name.ToLower()) //Case insensitive parameter names
					{
						case "artist":
						case "ar":
							artist = parameter.Value;
							showSearchWindow = true;
							break;
						case "album":
						case "al":
							album = parameter.Value;
							showSearchWindow = true;
							break;
						case "path":
						case "p":
							path = PathFix(parameter.Value);
							//Compatibility mode: if an "f" parameter, for filename, is provided, append it to the path.
							string filename;
							if (arguments.TryGetParameterValue("f", out filename))
							{
								path = Path.Combine(path, filename);
							}
							showSearchWindow = true;
							break;
						case "f":
							break; //See case "p" for handling of this parameter
						case "localimagespath":
							localImagesPath = PathFix(parameter.Value);
							showSearchWindow = true;
							break;
						case "autoclose":
						case "ac":
							if (parameter.Value.Equals("off", StringComparison.InvariantCultureIgnoreCase))
							{
								autoClose = false;
							}
							else
							{
								autoClose = true;
							}
							break;
						case "sources":
						case "s":
							useSources.AddRange(parameter.Value.Split(','));
							showSearchWindow = true;
							break;
						case "exclude":
						case "es":
							excludeSources.AddRange(parameter.Value.Split(','));
							showSearchWindow = true;
							break;
						case "ae": //Compatibility: Show Existing Album Art
							excludeSources.Add("Local Files");
							showSearchWindow = true;
							break;
						case "include":
						case "i":
							includeSources.AddRange(parameter.Value.Split(','));
							showSearchWindow = true;
							break;
						case "pf": //Compatibility: Show pictures in folder
							break; //Not currently supported
						case "filebrowser":
							fileBrowser = PathFix(parameter.Value);
							showFileBrowser = true;
							break;
						case "foobarbrowser":
							startFoobarBrowserSearch = parameter.Value.Equals("search", StringComparison.InvariantCultureIgnoreCase);
							showFoobarBrowser = true;
							break;
						case "sort":
						case "o":
							string sortName = null;
							if (parameter.Value.EndsWith("-"))
							{
								sortDirection = ListSortDirection.Descending;
							}
							else if (parameter.Value.EndsWith("+"))
							{
								sortDirection = ListSortDirection.Ascending;
							}
							sortName = parameter.Value.TrimEnd('-', '+');

							switch (sortName.ToLower())
							{
								case "name":
								case "n":
									sortField = "ResultName";
									break;
								case "size":
								case "s":
									sortField = "ImageWidth";
									break;
								case "source":
								case "o":
									sortField = "SourceName";
									break;
								case "type":
								case "t":
									sortField = "CoverType";
									break;
								case "area":
								case "a":
									sortField = "ImageArea";
									break;
								default:
									errorMessage = "Unexpected sort field: " + sortName;
									break;
							}
							break;
						case "group":
						case "g":
							switch (parameter.Value.ToLower())
							{
								case "none":
								case "n":
									grouping = Grouping.None;
									break;
								case "local":
								case "l":
									grouping = Grouping.Local;
									break;
								case "source":
								case "o":
									grouping = Grouping.Source;
									break;
								case "type":
								case "t":
									grouping = Grouping.Type;
									break;
								case "size":
								case "s":
									grouping = Grouping.Size;
									break;
								default:
									errorMessage = "Unexpected grouping: " + parameter.Value;
									break;
							}
							break;
						case "minsize":
						case "mn":
							try
							{
								minSize = Int32.Parse(parameter.Value);
							}
							catch(Exception e)
							{
								errorMessage = "The /minSize parameter must be a number: " + parameter.Value + "\n  " + e.Message;
							}
							break;
						case "maxsize":
						case "mx":
							try
							{
								maxSize = Int32.Parse(parameter.Value);
							}
							catch (Exception e)
							{
								errorMessage = "The /maxSize parameter must be a number: " + parameter.Value + "\n  " + e.Message;
							}
							break;
						case "covertype":
						case "t":
							coverType = default(AllowedCoverType);
							foreach (String allowedCoverType in parameter.Value.ToLower().Split(','))
							{
								switch (allowedCoverType)
								{
									case "front":
									case "f":
										coverType |= AllowedCoverType.Front;
										break;
									case "back":
									case "b":
										coverType |= AllowedCoverType.Back;
										break;
									case "inside":
									case "i":
										coverType |= AllowedCoverType.Inside;
										break;
									case "cd":
									case "c":
										coverType |= AllowedCoverType.CD;
										break;
									case "unknown":
									case "u":
										coverType |= AllowedCoverType.Unknown;
										break;
									case "any":
									case "a":
										coverType |= AllowedCoverType.Any;
										break;
									default:
										errorMessage = "Unrecognized cover type: " + parameter.Value;
										break;
								}
							}
							break;
						case "update":
							//Force an immediate check for updates
							Updates.CheckForUpdates(true);
							break;
						case "getscripts":
							//Force an immediate check for new scripts
							Updates.ShowNewScripts();
							break;
						case "separateinstance":
							//This will already have been handled earlier, in Main()
							break;
						case "config":
							showConfigFile = true;
							break;
						case "minimized":
							showMinimized = true;
							break;
						case "new":
							forceNewWindow = true;
							break;
						case "minaspect":
						case "ma":
						case "orientation":
						case "r":
						case "sequence":
						case "seq":
						case "listsources":
						case "l":
							System.Diagnostics.Debug.Fail("Unexpected command line parameter (valid for aad.exe, though): " + parameter.Name);
							break;
						default:
							errorMessage = "Unexpected command line parameter: " + parameter.Name;
							break;
					}
				}
				if (errorMessage != null)
					break; //Stop parsing args if there was an error
			}
			if (errorMessage != null) //Problem with the command args, so display the error, and the help
			{
				ShowCommandArgs(errorMessage);
				return false;
			}

			if(!String.IsNullOrEmpty(sortField))
			{
				//Set the sort
				AlbumArtDownloader.Properties.Settings.Default.ResultsSorting = new SortDescription(sortField, sortDirection);
			}
			if (grouping.HasValue)
			{
				//Set the grouping
				AlbumArtDownloader.Properties.Settings.Default.ResultsGrouping = grouping.Value;
			}
			if (minSize.HasValue)
			{
				if (minSize.Value == 0)
				{
					//0 would have no effect, so assume it means no filtering.
					AlbumArtDownloader.Properties.Settings.Default.UseMinimumImageSize = false;
				}
				else
				{
					//Set the minimum size
					AlbumArtDownloader.Properties.Settings.Default.MinimumImageSize = minSize.Value;
					AlbumArtDownloader.Properties.Settings.Default.UseMinimumImageSize = true;
				}
			}
			if (maxSize.HasValue)
			{
				if (maxSize.Value == 0)
				{
					//0 would result in no images at all, so assume it means no filtering.
					AlbumArtDownloader.Properties.Settings.Default.UseMinimumImageSize = false;
				}
				else
				{
					//Set the minimum size
					AlbumArtDownloader.Properties.Settings.Default.MaximumImageSize = maxSize.Value;
					AlbumArtDownloader.Properties.Settings.Default.UseMaximumImageSize = true;
				}
			}
			if (coverType.HasValue)
			{
				AlbumArtDownloader.Properties.Settings.Default.AllowedCoverTypes = coverType.Value;
			}

			//If the setting for using system codepage for id3 tags is set, instruct TagLib to do so.
			if (AlbumArtDownloader.Properties.Settings.Default.UseSystemCodepageForID3Tags)
			{
				TagLib.ByteVector.UseBrokenLatin1Behavior = true;
			}

			if (!showFileBrowser && !showFoobarBrowser && !showSearchWindow && !showConfigFile) //If no windows will be shown, show the search window
				showSearchWindow = true;

			if (showFileBrowser)
			{
				FileBrowser browserWindow = new FileBrowser();
				if(showMinimized)
				{
					browserWindow.WindowState = WindowState.Minimized;
				}
				browserWindow.Show();

				if (!String.IsNullOrEmpty(fileBrowser))
				{
					browserWindow.Search(fileBrowser,
											AlbumArtDownloader.Properties.Settings.Default.FileBrowseSubfolders, //TODO: Should the browse subfolders flag be a command line parameter?
											AlbumArtDownloader.Properties.Settings.Default.FileBrowseImagePath,
											AlbumArtDownloader.Properties.Settings.Default.FileBrowseUsePathPattern ? AlbumArtDownloader.Properties.Settings.Default.FileBrowsePathPattern : null
											); 
				}
			}

			if (showFoobarBrowser)
			{
				FoobarBrowser browserWindow = new FoobarBrowser();
				if (showMinimized)
				{
					browserWindow.WindowState = WindowState.Minimized;
				}
				browserWindow.Show();

				if (startFoobarBrowserSearch)
				{
					browserWindow.Search(AlbumArtDownloader.Properties.Settings.Default.FileBrowseImagePath); //TODO: Should foobar browser have a separate path setting?
				}
			}

			if (showSearchWindow)
			{
				ArtSearchWindow searchWindow = null;
				if ( (artist != null || album != null) && //If doing a new search
					 !forceNewWindow && //And not forcing a new window
					 !AlbumArtDownloader.Properties.Settings.Default.OpenResultsInNewWindow && //And the option is to open results in the same window
					 Windows.Count == 1) //And only one window is open
				{
					searchWindow = Windows[0] as ArtSearchWindow; //And if that window is an ArtSearchWindow, then re-use it
				}

				if (searchWindow == null)
				{
					searchWindow = new ArtSearchWindow();
					if (showMinimized)
					{
						searchWindow.WindowState = WindowState.Minimized;
					}

					SearchQueue.EnqueueSearchWindow(searchWindow, false); //Don't load from settings on show, otherwise they'll override the settings specified on the command line
				}
				else
				{
					searchWindow.Activate(); //Bring the window to the foreground
				}

				if (autoClose.HasValue)
					searchWindow.OverrideAutoClose(autoClose.Value);
				if (path != null)
					searchWindow.SetDefaultSaveFolderPattern(path);
				if (localImagesPath != null)
					searchWindow.SetLocalImagesPath(localImagesPath);
				if (useSources.Count > 0)
					searchWindow.UseSources(useSources);
				if (excludeSources.Count > 0)
					searchWindow.ExcludeSources(excludeSources);
				if (includeSources.Count > 0)
					searchWindow.IncludeSources(includeSources);

				if (artist != null || album != null)
				{
					searchWindow.Search(artist, album);
					if (SearchQueue.Queue.Count == 1)
					{
						//This is the first item enqueued, so show the queue manager window
						SearchQueue.ShowManagerWindow();
					}
				}
				else
				{
					//Showing a new search window without performing a search, so force show it.
					SearchQueue.ForceSearchWindow(searchWindow);
				}
			}

			if (showConfigFile)
			{
				AlbumArtDownloader.Properties.Settings.Default.Save();
				string configName = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
				System.Diagnostics.Process.Start("notepad.exe", configName);

				if (!showFileBrowser && !showFoobarBrowser && !showSearchWindow) //If no other windows will be shown, exit now
				{
					return false;
				}
			}

			if (AlbumArtDownloader.Properties.Settings.Default.AutoUpdateEnabled)
			{
				//Check for updates if enough time has elapsed.
				Updates.CheckForUpdates(false);
			}

			return true;
		}

		/// <summary>
		/// Hack to fix .net args processing. Removes trailing " and replaces it by \
		/// <remarks>
		/// If the command line includes, for example /path "c:\folder\", then the last two
		/// characters are interpreted as an escaped " mark. This hack fixes that.
		/// </remarks>
		/// </summary>
		private static string PathFix(string pathParam)
		{
			if (!String.IsNullOrEmpty(pathParam) && pathParam[pathParam.Length - 1] == '\"')
			{
				return pathParam.Substring(0, pathParam.Length - 1) + "\\";
			}
			return pathParam;
		}

		//Any other settings loaded will also require upgrading, if the main settings do, so set this flag to indicate that.
		private bool mSettingsUpgradeRequired;
		private void UpgradeSettings()
		{
			//Settings may need upgrading from an earlier version
			string currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			if (AlbumArtDownloader.Properties.Settings.Default.ApplicationVersion != currentVersion)
			{
				System.Diagnostics.Debug.WriteLine("Upgrading settings");
				mSettingsUpgradeRequired = true;

				//Upgrading will forget the value of this setting, if it was changed prior to upgrading the settings (which it will have been, if the MissingFrameworkSP1 dialog was shown
				bool ignoreSP1Missing = AlbumArtDownloader.Properties.Settings.Default.IgnoreSP1Missing;

				AlbumArtDownloader.Properties.Settings.Default.Upgrade();
				AlbumArtDownloader.Properties.Settings.Default.ApplicationVersion = currentVersion;

				AlbumArtDownloader.Properties.Settings.Default.IgnoreSP1Missing = ignoreSP1Missing;
			}
		}

		/// <summary>
		/// Show the command args helper screen, without any error message
		/// </summary>
		private void ShowCommandArgs()
		{
			ShowCommandArgs(null);
		}
		/// <summary>
		/// Show the command args helper screen, with the specified error message
		/// </summary>
		private void ShowCommandArgs(string errorMessage)
		{
			new CommandArgsHelp().ShowDialog(errorMessage);
		}

		/// <summary>
		/// Assign sensible defaults to values without hard-coded defaults
		/// </summary>
		private void AssignDefaultSettings()
		{
			if (AlbumArtDownloader.Properties.Settings.Default.DefaultSavePath == "%default%")
				AlbumArtDownloader.Properties.Settings.Default.DefaultSavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), @"Album Art\%artist%\%album%\Folder%preset%.%extension%");

			if (AlbumArtDownloader.Properties.Settings.Default.FileBrowseRoot == "%default%")
				AlbumArtDownloader.Properties.Settings.Default.FileBrowseRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
		}

		/// <summary>
		/// The config file mechanism for sepecifying a proxy server to use 
		/// (http://msdn2.microsoft.com/en-us/library/kd3cf2ex.aspx)
		/// does not allow specification of credentials for that proxy, so apply
		/// credentials stored in our own settings.
		/// </summary>
		private void ApplyDefaultProxyCredentials()
		{
			System.Net.NetworkCredential credentials = AlbumArtDownloader.Properties.Settings.Default.ProxyCredentials;
			if (credentials != null)
			{
				System.Net.WebRequest.DefaultWebProxy.Credentials = credentials;
			}
		}

		/// <summary>
		/// Load all the scripts from dlls in the Scripts folder.
		/// <remarks>Boo scripts should previously have been compiled into
		/// a dll (boo script cache.dll)</remarks> 
		/// </summary>
		private void LoadScripts()
		{
			mScripts = new List<IScript>();
			foreach (string scriptsPath in ScriptsPaths)
			{
				foreach (string dllFile in Directory.GetFiles(scriptsPath, "*.dll"))
				{
					try
					{
						Assembly assembly = Assembly.LoadFile(dllFile);
						foreach (Type type in assembly.GetTypes())
						{
							try
							{
								IScript script = null;
								//Check for types implementing IScript
								if (typeof(IScript).IsAssignableFrom(type))
								{
									if (!type.IsAbstract)
									{
										script = (IScript)Activator.CreateInstance(type);
									}
								}
								//Check for static scripts (for backwards compatibility)
								else if (type.Namespace == "CoverSources")
								{
									script = new StaticScript(type);
								}

								if (script != null)
									mScripts.Add(script);
							}
							catch (Exception e)
							{
								//Skip the type. Does this need to display a user error message?
								System.Diagnostics.Debug.Fail(String.Format("Could not load script: {0}\n\n{1}", type.Name, e.Message));
							}
						}
					}
					catch (Exception e)
					{
						//Skip the assembly
						System.Diagnostics.Debug.Fail(String.Format("Could not load assembly: {0}\n\n{1}", dllFile, e.Message));
					}
				}
			}
		}

		private List<IScript> mScripts;
		public IEnumerable<IScript> Scripts
		{
			get
			{
				if (mScripts == null)
					throw new InvalidOperationException("Must call LoadScripts() before using this property");

				return mScripts;
			}
		}

		/// <summary>
		/// Returns each path that scripts may be found in, in reverse priority order (scripts with the same name in the last path override scripts in first path)
		/// </summary>
		public static IEnumerable<string> ScriptsPaths
		{
			get
			{
				//Scripts may be in a "scripts" subfolder of the application folder
				yield return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Scripts");

				//They may also be in the scripts cache file path.
				yield return Path.GetDirectoryName(BooScriptsCacheFile);
			}
		}

		private static readonly string sBooScriptCacheDll = "boo script cache.dll";
		private static string mBooScriptsCacheFile;
		internal static string BooScriptsCacheFile
		{
			get
			{
				if (mBooScriptsCacheFile == null)
					mBooScriptsCacheFile = Path.Combine(Path.Combine(Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath), "Scripts"), sBooScriptCacheDll);

				return mBooScriptsCacheFile;
			}
		}

		protected override void OnExit(ExitEventArgs e)
		{
			//Save all the settings
			try
			{
				AlbumArtDownloader.Properties.Settings.Default.Save();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.TraceError("Could not save main settings: " + ex.Message);
			}

			foreach (SourceSettings sourceSettings in mSourceSettings.Values)
			{
				try
				{
					sourceSettings.Save();
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.TraceError("Could not save source settings: " + ex.Message);
				}
			}

			base.OnExit(e);
		}

		private Dictionary<string, SourceSettings> mSourceSettings = new Dictionary<String, SourceSettings>();
		public SourceSettings GetSourceSettings(string sourceName, SourceSettingsCreator sourceSettingsCreator)
		{
			SourceSettings sourceSettings;
			if (!mSourceSettings.TryGetValue(sourceName, out sourceSettings))
			{
				sourceSettings = sourceSettingsCreator(sourceName);
				if (mSettingsUpgradeRequired)
					sourceSettings.Upgrade();

#if EPHEMERAL_SETTINGS
				sourceSettings.Reset();
#endif

				mSourceSettings.Add(sourceName, sourceSettings);
			}

			return sourceSettings;
		}

		private SearchQueue mSearchQueue;
		/// <summary>
		/// Handles queueing up of searches so that multiple searches can be kicked off without
		/// them actually starting until previous ones have finished.
		/// </summary>
		public SearchQueue SearchQueue
		{
			get
			{
				if (mSearchQueue == null)
					mSearchQueue = new SearchQueue();
				return mSearchQueue;
			}
		}

		/// <summary>
		/// Command handler for all GoToPage commands (hyperlinks)
		/// </summary>
		private static void GoToPageExec(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			string uriString = e.Parameter as String;
			if (!String.IsNullOrEmpty(uriString))
			{
				try
				{
					//Ensure that this the parameter is a Uri
					Uri uri = new Uri(uriString, UriKind.Absolute);
					if (uri.IsFile)
					{
						//If the Uri is a file, then display it in explorer rather than executing it (safer too!)
						System.Diagnostics.Process.Start("explorer.exe", "/select,\"" + uri.AbsoluteUri + "\"");
					}
					else
					{
						System.Diagnostics.Process.Start(uri.AbsoluteUri);
					}
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.TraceError("Could open web address: {0}\n\t{1}", uriString, ex.Message);
				}
			}
			else if (e.Parameter is Window)
			{
				((Window)e.Parameter).Activate();
			}
		}
	}
}