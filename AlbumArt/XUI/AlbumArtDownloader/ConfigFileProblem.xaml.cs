using System;
using System.Windows;
using System.Configuration;
using System.Reflection;
using System.IO;

namespace AlbumArtDownloader
{
	public partial class ConfigFileProblem : Window
	{
		private ConfigurationErrorsException mException;

		public ConfigFileProblem()
		{
			InitializeComponent();
		}

		public ConfigFileProblem(ConfigurationErrorsException exception) : this()
		{
			mException = exception;
		}

		private string GetConfigFilename()
		{
			if (mException == null)
				return null;

			string filename = mException.Filename;
			if (String.IsNullOrEmpty(filename))
			{
				ConfigurationErrorsException innerException = mException.InnerException as ConfigurationErrorsException;
				if (innerException != null && !String.IsNullOrEmpty(innerException.Filename))
				{
					filename = innerException.Filename;
				}
			}

			return filename;
		}

		private void ResetSettings(object sender, RoutedEventArgs e)
		{
			//Delete the settings file and restart
			string filename = GetConfigFilename();

			if (String.IsNullOrEmpty(filename))
			{
				OperationFailed("Reset settings", "Could not determine path to configuration file");
				return;
			}

			try
			{
				if (File.Exists(filename))
				{
					File.Delete(filename);
				}
				else
				{
					OperationFailed("Reset settings", "Could not find configuration file");
					return;
				}
			}
			catch (Exception deleteFileException)
			{
				OperationFailed("Reset settings", "Could not delete configuration file: " + deleteFileException.Message);
				return;
			}

			System.Diagnostics.Trace.TraceInformation("Settings reset to defaults, restarting");
			App.RestartOnExit = true; //Request restart once the dialog closes
		}

		private void OperationFailed(string operationName, string message)
		{
			MessageBox.Show(this, message, operationName + " failed", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void ViewFile(object sender, RoutedEventArgs e)
		{
			//Open a Windows Explorer window showing the config file
			string filename = GetConfigFilename();

			if (String.IsNullOrEmpty(filename))
			{
				OperationFailed("View configuration file", "Could not determine path to configuration file");
				return;
			}

			//TODO: Validation that this is a file path?
			System.Diagnostics.Process.Start("explorer.exe", "/select,\"" + filename + "\"");
		}

		private void ErrorReport(object sender, RoutedEventArgs e)
		{
			//Create an error log and show in notepad
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
					errorLog.WriteLine("Album Art Downloader encountered an error when attempting to read its");
					errorLog.WriteLine("configuration settings, and could not start.");
					errorLog.WriteLine("If you wish to report this error, please include this information, which");
					errorLog.WriteLine("has been written to the file: " + filename);
					errorLog.WriteLine();
					errorLog.WriteLine("App version: {0}, running on {1} ({2} bit)", entryAssembly.GetName().Version, Environment.OSVersion, IntPtr.Size == 8 ? "64" : "32");
					errorLog.WriteLine();
					errorLog.WriteLine(mException);
				}
				System.Diagnostics.Process.Start(filename);
			}
		}
	}
}
