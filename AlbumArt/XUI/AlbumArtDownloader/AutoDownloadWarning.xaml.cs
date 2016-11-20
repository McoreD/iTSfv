using System;
using System.Windows;
using System.Configuration;
using System.Reflection;
using System.IO;

namespace AlbumArtDownloader
{
	public partial class AutoDownloadWarning : Window
	{
		public AutoDownloadWarning()
		{
			InitializeComponent();
		}

		private void OnNormalClicked(object sender, RoutedEventArgs e)
		{
			Properties.Settings.Default.FileBrowseAutoDownload = false;
			DialogResult = true;
		}

		private void OnAutomaticClicked(object sender, RoutedEventArgs e)
		{
			//Nothing needs to be done here.
			DialogResult = false;
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			Properties.Settings.Default.ShowAutoDownloadWarning = !mDontAskAgain.IsChecked.GetValueOrDefault();
		}
		
	}
}
