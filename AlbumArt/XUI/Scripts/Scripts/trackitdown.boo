import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class TrackItDown(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "trackitdown"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "Dance, Club, Electronic"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("https://www.trackitdown.net/cloudsearch/keyword?q=%22" + EncodeUrl(album) + "%22&artist=%22" + EncodeUrl(artist) + "%22");
		
		matches = Regex("<strong itemprop=\"name\">(?<title>[^<]+)<.*?<div class=\"coverImage\"><a href=\"(?<url>[^\"]+?)\".*?src=\"(?<image>[^\"_]+)_.*?<a [^>]*?itemprop=\"author\"[^>]*?>(?<artist>[^<]+)<", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			image = match.Groups["image"].Value
			results.Add(image + "__small.png", match.Groups["artist"].Value + " - " + match.Groups["title"].Value, "https://www.trackitdown.net" + match.Groups["url"].Value, -1, -1, image + "_original.jpg", CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;
