using System;
using System.Linq;
using System.Windows;

namespace AlbumArtDownloader
{
	/// <summary>
	/// Window to show available updates
	/// </summary>
	public partial class UpdatesViewer : Window
	{
		private Updates mUpdates;

		public UpdatesViewer()
		{
			InitializeComponent();
		}

		public void Show(Updates updates)
		{
			if (updates.RestartPending)
			{
				DisplayRestartPending();
			}
			else if (updates.HasApplicationUpdate)
			{
				mLabel.Text = "A new version of Album Art Downloader XUI is available. Click on the link below to visit the download page:";
				mApplicationDownloadLink.Content = updates.ApplicationUpdateName;
				mApplicationDownloadLink.CommandParameter = updates.ApplicationUpdateUri.AbsoluteUri;
				mApplicationDownloadLink.ToolTip = updates.ApplicationUpdateUri.AbsoluteUri;

				mApplicationDownloadLink.Visibility = Visibility.Visible;
				mScriptUpdatesViewer.Visibility = Visibility.Hidden;

				mActionButton.IsEnabled = false;
				mCancelButton.Content = "Close";
			}
			else if (!updates.ScriptUpdates.Any())
			{
				//No updates available
				mLabel.Text = "No updates are currently available.";
				mScriptUpdatesViewer.Visibility = Visibility.Hidden;
				mActionButton.IsEnabled = false;
				mCancelButton.Content = "Close";
			}
			else
			{
				mScriptUpdatesViewer.ItemsSource = updates.ScriptUpdates;
			}

			mUpdates = updates;

			Show();
		}

		private void DisplayRestartPending()
		{
			mLabel.Text = "The updated scripts will be available once Album Art Downloader XUI has been restarted.";
			mScriptUpdatesViewer.Visibility = Visibility.Hidden;
			mActionButton.Content = "Restart";
			mCancelButton.Content = "Close";
		}

		private void OnActionButtonClicked(object sender, RoutedEventArgs e)
		{
			if (mUpdates.RestartPending)
			{
				//Restart
				App.RestartOnExit = true;
				Application.Current.Shutdown();
			}
			else
			{
				//Update
				mUpdates.DownloadSelectedScriptUpdates(mUpdates.ScriptUpdates);
				DisplayRestartPending();
			}
		}

		private void OnCancelButtonClicked(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}

	
}
