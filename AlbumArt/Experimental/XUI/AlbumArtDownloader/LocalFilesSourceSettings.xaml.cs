using System;
using System.Windows.Controls;

namespace AlbumArtDownloader
{
	/// <summary>
	/// Interaction logic for LocalFilesSourceSettings.xaml
	/// </summary>

	public partial class LocalFilesSourceSettings : System.Windows.Controls.UserControl
	{
		public LocalFilesSourceSettings()
		{
			InitializeComponent();
			mSearchPathPatternBox.ToolTipOpening += new ToolTipEventHandler(OnToolTipOpening);
		}

		private void OnToolTipOpening(object sender, ToolTipEventArgs e)
		{
			//TODO: Can the actual current artist and album be substituted here?
			((Control)sender).ToolTip = ((LocalFilesSource)DataContext).GetSearchPath("%artist%", "%album%");
		}
	}
}