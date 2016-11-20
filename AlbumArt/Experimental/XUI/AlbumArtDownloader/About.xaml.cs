using System;
using System.Windows.Input;

namespace AlbumArtDownloader
{
	public partial class About : System.Windows.Window
	{
		public About()
		{
			InitializeComponent();

			CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, new ExecutedRoutedEventHandler(CloseExec)));
		}

		private void CloseExec(object sender, ExecutedRoutedEventArgs e)
		{
			this.Close();
		}
	}
}