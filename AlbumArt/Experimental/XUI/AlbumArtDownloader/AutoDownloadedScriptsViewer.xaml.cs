using System;
using System.Linq;
using System.Windows;

namespace AlbumArtDownloader
{
	/// <summary>
	/// Window to show scripts that have been automatically downloaded
	/// </summary>
	public partial class AutoDownloadedScriptsViewer : Window
	{
		public AutoDownloadedScriptsViewer()
		{
			InitializeComponent();

			if (!Updates.AutoDownloadedScripts.Any())
			{
				//No updates available
				mLabel.Text = "No scripts have been automatically downloaded.";
				mDownloadedScriptViewer.Visibility = Visibility.Hidden;
				mRestartButton.IsEnabled = false;
			}
			else
			{
				mDownloadedScriptViewer.ItemsSource = Updates.AutoDownloadedScripts;
			}
		}

		private void OnRestartButtonClicked(object sender, RoutedEventArgs e)
		{
			App.RestartOnExit = true;
			Application.Current.Shutdown();
		}

		private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
		{
			Close();
		}

		protected override void OnClosed(EventArgs e)
		{
			//Scripts have been seen, so clear them.
			Updates.AutoDownloadedScripts.Clear();
			base.OnClosed(e);
		}
	}
}
