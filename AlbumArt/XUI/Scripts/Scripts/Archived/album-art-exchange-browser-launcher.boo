import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class AAXBrowserLauncher(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Album Art Exchange (Browser)"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		query = EncodeUrl("\"${artist}\" \"${album}\"")
		url = "http://www.albumartexchange.com/covers.php?q=${query}"

		// Launch the brower to that page
		System.Diagnostics.Process.Start(url);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return null