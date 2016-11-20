import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class GooglePlay(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Google Play Music"
	Version as string:
		get: return "0.3"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("https://play.google.com/store/search?q=" + EncodeUrl("\"" + artist + "\" \"" + album + "\"") + "&c=music")
		
		matches = Regex("<img [^>]+?class=\"cover-image\"[^>]+? src=\"(?<img>[^=]+)[^\"]+\"(?:.(?!/store/music/artist/))+?<a class=\"title\" href=\"(?<url>/store/music/album/[^\"]+)\" title=\"(?<album>[^\"]+)\".*?<a class=\"subtitle\"[^>]+?title=\"(?<artist>[^\"]+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			img = match.Groups["img"].Value;
			
			results.Add(img + "=w170", match.Groups["artist"].Value + " - " + match.Groups["album"].Value, "https://play.google.com" + match.Groups["url"].Value, -1, -1, img + "=w0", CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;