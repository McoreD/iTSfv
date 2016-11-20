using System;

namespace AlbumArtDownloader.Scripts
{
	/// <summary>
	/// Implement this interface to define a custom script for downloading images
	/// in any .net language. Any class implementing this interface in a dll in the
	/// scripts folder will be loaded and used as a script.
	/// </summary>
	public interface IScript
	{
		string Name { get; }
		string Author { get; }
		string Version { get; }

		void Search(string artist, string album, IScriptResults results);
		object RetrieveFullSizeImage(object fullSizeCallbackParameter);
	}
}
