using System;

namespace AlbumArtDownloader.Scripts
{
	/// <summary>
	/// Scripts should implement this interface if they want to belong to any specific category.
	/// Otherwise, they will be grouped in the "General" category.
	/// </summary>
	public interface ICategorised
	{
		string Category { get; }
	}
}
