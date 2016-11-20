using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using AlbumArtDownloader.Scripts;

namespace AlbumArtDownloader
{
	public class Updates
	{
		//Only have one viewer window open at any one time. This is that window.
		private static UpdatesViewer sUpdatesViewer;
		private static NewScriptsViewer sNewScriptsViewer;

		private static bool sRestartPending;

		/// <summary>If any scripts have been automatically downloaded, they will appear in this list so that the user can be informed</summary>
		public static ObservableCollection<ScriptUpdate> AutoDownloadedScripts = new ObservableCollection<ScriptUpdate>();

		/// <summary>
		/// Performs a check for available updates, if <see cref="Properties.Settings.Default.AutoUpdateCheckInterval"/> has elapsed since the last check was made.
		/// <param name="forceDisplayViewer">If true, an update will always be performed, and the updates viewer will be displayed even if no updates are available</param>
		/// </summary>
		public static void CheckForUpdates(bool forceCheck)
		{
			TimeSpan timeSinceLastCheck = DateTime.Now - Properties.Settings.Default.LastUpdateCheck;
			if (forceCheck || timeSinceLastCheck > Properties.Settings.Default.AutoUpdateCheckInterval)
			{
				Properties.Settings.Default.LastUpdateCheck = DateTime.Now;
			
				//A check is due, so start the thread to asynchronously download and process the update data
				ThreadPool.QueueUserWorkItem(new WaitCallback(PerformUpdateCheck),
					new PerformUpdateCheckParameters(forceCheck ? PerformUpdateCheckParameters.UI.Updates : PerformUpdateCheckParameters.UI.None));
			}
		}

		/// <summary>
		/// Performs a check for available updates, then shows the New Available Scripts window if any new scripts are available.
		/// </summary>
		public static void ShowNewScripts()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(PerformUpdateCheck), new PerformUpdateCheckParameters(PerformUpdateCheckParameters.UI.NewScripts));
		}

		/// <summary>
		/// Downloads the latest Updates XML file and produces an <see cref="Updates"/> list of available
		/// updates from it
		/// </summary>
		private static void PerformUpdateCheck(object state)
		{
			try
			{
				PerformUpdateCheckParameters parameters = (PerformUpdateCheckParameters)state;

				Updates updates = new Updates();

				try
				{
					XmlDocument updatesXml = new XmlDocument();
					updatesXml.Load(Properties.Settings.Default.UpdatesURI.AbsoluteUri);

					Uri baseUri = new Uri(updatesXml.DocumentElement.GetAttribute("BaseURI"));

					//Check to see if there is an application update available
					XmlElement appUpdateXml = updatesXml.SelectSingleNode("/Updates/Application") as XmlElement;
					if (appUpdateXml != null)
					{
						Version newAppVersion = new Version(appUpdateXml.GetAttribute("Version"));

						if (newAppVersion > Assembly.GetEntryAssembly().GetName().Version)
						{
							//Theres a new application version, so return an update with just that
							Uri uri = new Uri(baseUri, appUpdateXml.GetAttribute("URI"));

							updates.SetAppUpdate(appUpdateXml.GetAttribute("Name"), uri);
						}
					}

					//Create a lookup of all current scripts and their versions
					Dictionary<String, String> scripts = new Dictionary<String, String>();
					foreach (IScript script in ((App)Application.Current).Scripts)
					{
						scripts.Add(script.Name, script.Version);
					}

					foreach (XmlElement scriptUpdateXml in updatesXml.SelectNodes("/Updates/Script"))
					{
						string name = scriptUpdateXml.GetAttribute("Name");
						string newVersion = scriptUpdateXml.GetAttribute("Version");

						//Check to see if there is an older version of this script to update
						string currentVersion;
						if (scripts.TryGetValue(name, out currentVersion))
						{
							if (currentVersion != newVersion)
							{

								updates.AddScriptUpdate(new ScriptUpdate(name, currentVersion, newVersion, GetDownloadFiles(baseUri, scriptUpdateXml)));
							}
						}
						else
						{
							updates.AddAvailableScript(new ScriptUpdate(name, null, newVersion, GetDownloadFiles(baseUri, scriptUpdateXml)));
						}
					}
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.TraceError("Could not parse update xml from \"{0}\": {1}", Properties.Settings.Default.UpdatesURI.AbsoluteUri, ex.Message);
				}

				Properties.Settings.Default.NewScriptsAvailable = updates.mAvailableScripts.Count > 0;

				if (parameters.ShowUI == PerformUpdateCheckParameters.UI.NewScripts)
				{
					parameters.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(delegate
					{
						if (sNewScriptsViewer != null)
						{
							sNewScriptsViewer.Close();
						}
						sNewScriptsViewer = new NewScriptsViewer();
						sNewScriptsViewer.Show(updates);
						sNewScriptsViewer.Closed += new EventHandler(delegate { sNewScriptsViewer = null; });
					}));
				}
				else
				{
					//If not showing the New Scripts UI, and no application update is available, then check for auto-downloading new scripts
					if (sNewScriptsViewer == null &&
						!updates.HasApplicationUpdate &&
						updates.mAvailableScripts.Count > 0 &&
						Properties.Settings.Default.AutoDownloadAllScripts)
					{
						//Automatically download all newly available scripts
						foreach (var script in updates.mAvailableScripts)
						{
							script.Download();
							sRestartPending = true;

							parameters.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new ThreadStart(delegate
							{
								AutoDownloadedScripts.Add(script);
							}));
						}

						Properties.Settings.Default.NewScriptsAvailable = false;
					}

					if (parameters.ShowUI == PerformUpdateCheckParameters.UI.Updates || //Show the updates viewer if specifically requested.
						updates.mScriptUpdates.Count > 0 || //If not requested, only show if there are updates to be shown
						updates.HasApplicationUpdate)
					{
						parameters.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(delegate
						{
							if (sUpdatesViewer != null)
							{
								sUpdatesViewer.Close();
							}
							sUpdatesViewer = new UpdatesViewer();
							sUpdatesViewer.Show(updates);
							sUpdatesViewer.Closed += new EventHandler(delegate { sUpdatesViewer = null; });
						}));
					}
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.TraceError("Check for online updates failed: {0}", ex.Message);
			}
		}

		private static IEnumerable<Uri> GetDownloadFiles(Uri baseUri, XmlElement scriptUpdateXml)
		{
			yield return new Uri(baseUri, scriptUpdateXml.GetAttribute("URI"));

			foreach (XmlElement dependencyXml in scriptUpdateXml.SelectNodes("Dependency"))
			{
				yield return new Uri(baseUri, dependencyXml.InnerText);
			}
		}

		private struct PerformUpdateCheckParameters
		{
			public enum UI
			{
				None,
				Updates,
				NewScripts
			}
			private readonly Dispatcher mDispatcher;
			private readonly UI mShowUI;

			public PerformUpdateCheckParameters(UI showUI)
			{
				mDispatcher = Dispatcher.CurrentDispatcher;
				mShowUI = showUI;
			}
			public Dispatcher Dispatcher { get { return mDispatcher; } }
			public UI ShowUI { get { return mShowUI; } }
		}


		#region Instance members
		private readonly List<ScriptUpdate> mScriptUpdates = new List<ScriptUpdate>();
		private readonly List<ScriptUpdate> mAvailableScripts = new List<ScriptUpdate>();
		
		private string mAppUpdateName;
		private Uri mAppUpdateUri;
		
		private Updates()
		{
		}

		private void AddScriptUpdate(ScriptUpdate scriptUpdate)
		{
			mScriptUpdates.Add(scriptUpdate);
		}

		private void AddAvailableScript(ScriptUpdate scriptUpdate)
		{
			mAvailableScripts.Add(scriptUpdate);
		}

		private void SetAppUpdate(string name, Uri uri)
		{
			mAppUpdateName = name;
			mAppUpdateUri = uri;
		}

		public bool HasApplicationUpdate { get { return mAppUpdateName != null; } }
		public string ApplicationUpdateName { get { return mAppUpdateName; } }
		public Uri ApplicationUpdateUri { get { return mAppUpdateUri; } }

		/// <summary>
		/// If a restart is pending, no further updates should be displayed until the application has been restarted.
		/// </summary>
		public bool RestartPending { get { return sRestartPending; } }

		/// <summary>
		/// Existing scripts for which updates are available
		/// </summary>
		public IEnumerable<ScriptUpdate> ScriptUpdates { get { return mScriptUpdates.AsReadOnly(); } }
		
		/// <summary>
		/// New scripts for which no previous version exists
		/// </summary>
		public IEnumerable<ScriptUpdate> AvailableScripts { get { return mAvailableScripts.AsReadOnly(); } }

		public void DownloadSelectedScriptUpdates(IEnumerable<ScriptUpdate> scriptUpdates)
		{
			foreach (ScriptUpdate scriptUpdate in scriptUpdates.Where(s => s.Selected))
			{
				scriptUpdate.Download();
				
				//A new script has been downloaded, so flag for restart required (which is static - remains set until restart!)
				sRestartPending = true;
			}
		}
		#endregion
	}

	public class ScriptUpdate
	{
		private readonly string mName;
		private readonly string mOldVersion;
		private readonly string mNewVersion;
		private readonly IEnumerable<Uri> mFiles;
		private bool mSelected;

		public ScriptUpdate(string name, string oldVersion, string newVersion, IEnumerable<Uri> files)
		{
			mName = name;
			mOldVersion = oldVersion;
			mNewVersion = newVersion;
			mFiles = files;
			
			mSelected = true;
		}

		public string Name { get { return mName; } }
		public string OldVersion { get { return mOldVersion; } }
		public string NewVersion { get { return mNewVersion; } }
		
		public bool Selected
		{
			get { return mSelected; }
			set { mSelected = value; }
		}

		public void Download()
		{
			foreach (Uri fileToDownload in mFiles)
			{
				string filename = Path.GetFileName(fileToDownload.LocalPath);

				string targetPath = null;

				foreach (string scriptsPath in App.ScriptsPaths)
				{
					//See whether the file can be written there
					try
					{
						targetPath = Path.Combine(scriptsPath, filename);
						File.OpenWrite(targetPath).Close();
						break; //If it reaches here, the file is writable
					}
					catch (Exception fileWriteException)
					{
						System.Diagnostics.Trace.TraceWarning("Could not download script update for {0} to \"{1}\": {2}", Name, targetPath, fileWriteException.Message);
						targetPath = null;
					}
				}

				if (targetPath == null)
				{
					System.Diagnostics.Trace.TraceError("Could not download script update for {0} from \"{1}\": No writable paths found.", Name, fileToDownload);
					return;
				}

				try
				{
					new System.Net.WebClient().DownloadFile(fileToDownload, targetPath + ".part");
					//If it reaches here, then it was successfull - replace the existing one
					File.Delete(targetPath);
					File.Move(targetPath + ".part", targetPath);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.TraceError("Could not download script update for {0} from \"{1}\": {2}", Name, fileToDownload, ex.Message);
				}
			}
		}
	}
}
