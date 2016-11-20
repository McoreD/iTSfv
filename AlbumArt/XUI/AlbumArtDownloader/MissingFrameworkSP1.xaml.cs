using System;
using System.Windows;
using System.Configuration;
using System.Reflection;
using System.IO;

namespace AlbumArtDownloader
{
	public partial class MissingFrameworkSP1 : Window
	{
		public MissingFrameworkSP1()
		{
			InitializeComponent();
		}

		private void DownloadPage(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start(App.DotNetDownloadPage);
			DialogResult = false;
		}

		private void Continue(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			Properties.Settings.Default.IgnoreSP1Missing = mDontAskAgain.IsChecked.GetValueOrDefault();
		}
		
	}
}
