using System;

namespace AlbumArtDownloader
{
	/// <summary>
	/// A main application window, such as a search window or browser.
	/// Applications implementing this interface will appear in the Windows menu
	/// </summary>
	internal interface IAppWindow
	{
		/// <summary>
		/// Save any outstanding values to settings.
		/// </summary>
		void SaveSettings();

		/// <summary>
		/// Load values from settings
		/// </summary>
		void LoadSettings();

		double Left { get; set; }
		double Top { get; set; }
		double Width { get; }
		double Height { get; }

		void Show();
		string Description { get; }

	}
}
