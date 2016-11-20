using System;

namespace AlbumArtDownloader
{
	/// <summary>
	/// Options available for when to auto download full size images
	/// </summary>
	public enum AutoDownloadFullSizeImages
	{
		Never,
		WhenSizeUnknown,
		Always
	}

	/// <summary>
	/// States that the art file search can be in for the browsers
	/// </summary>
	public enum ArtFileStatus
	{
		Unknown,
		Queued,
		Searching,
		Present,
		Missing
	}

	/// <summary>
	/// States that the browser can be in
	/// </summary>
	public enum BrowserState
	{
		/// <summary>No search been performed yet.</summary>
		Ready,
		/// <summary>Finding media files with artist and album tags, and simultaneously finding art for them.</summary>
		FindingFiles,
		/// <summary>No longer finding media files, only finding art for albums already located.</summary>
		FindingArt,
		/// <summary>Search completed successfully.</summary>
		Done,
		/// <summary>Search stopped by user.</summary>
		Stopped,
		/// <summary>Search abandoned due to error.</summary>
		Error
	}

	/// <summary>
	/// Cover types that are allowed (filtered in)
	/// </summary>
	[Flags]
	public enum AllowedCoverType
	{
		//NOTE: These names currently appear in the UI
		Unknown = 0x1,
		Front = 0x2,
		Back = 0x4,
		Inside = 0x8,
		CD = 0x10,
		Any = Unknown | Front | Back | Inside | CD
	}
}
