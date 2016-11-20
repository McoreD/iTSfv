using System;

namespace AlbumArtDownloader.Controls
{
	/// <summary>
	/// Where to position the information, relative to the thumbnail
	/// </summary>
	public enum InformationLocation
	{
		Right,
		Bottom,
	}

	/// <summary>
	/// How to group the results
	/// </summary>
	public enum Grouping
	{
		/// <summary>No grouping</summary>
		None,
		/// <summary>2 groups: Local results, and Online results</summary>
		Local,
		/// <summary>Grouped by result source name</summary>
		Source,
		/// <summary>Grouped by result source category</summary>
		SourceCategory,
		/// <summary>Grouped by cover type (Front, CD, ...)</summary>
		Type,
		/// <summary>Grouped by general size category</summary>
		Size,
		/// <summary>Grouped by InfoUri</summary>
		InfoUri,
	}
}
