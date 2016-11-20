using System;
using System.Reflection;
using System.Windows.Input;

namespace AlbumArtDownloader
{
	public partial class Logo : System.Windows.Controls.UserControl
	{
		public Logo()
		{
			InitializeComponent();
		}

		public string Version
		{
			get
			{
				return String.Format("version {0}", Assembly.GetEntryAssembly().GetName().Version);
			}
		}
	}
}