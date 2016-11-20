using System;
using System.ComponentModel;

namespace AlbumArtDownloader.Controls
{
	/// <summary>
	/// A group description for grouping by whether the source is Local or Online.
	/// </summary>
	internal class LocalGroupDescription : GroupDescription
	{
		public override object GroupNameFromItem(object item, int level, System.Globalization.CultureInfo culture)
		{
			System.Diagnostics.Debug.Assert(level == 0, "Multiple levels are not supported");
			AlbumArt albumArt = item as AlbumArt;
			if (albumArt == null)
			{
				System.Diagnostics.Debug.Fail("Expecting to be grouping album art");
				return null;
			}

			return (albumArt.IsSourceLocal ? "Local" : "Online") + " results";
		}
	}
}
