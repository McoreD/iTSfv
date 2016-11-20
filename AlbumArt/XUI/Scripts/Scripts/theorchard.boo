import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class Ioda(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "The Orchard"
	Version as string:
		get: return "0.2"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "Independent"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results
		searchResultsHtml as string = GetPage("http://search.theorchard.com/search_catalog/list?artist=" + EncodeUrl(artist) + "&release=" + EncodeUrl(album) + "&type=music&redirected=1")
		
		matches = Regex("<a href=\"(?<url>[^\"]+)\"><img src=\"[^\"]+?cover/(?<image>[^\"]+)\".+?class=\"name\">(?<artist>[^<]+)<.+?class=\"name\">(?<album>[^<]+)<", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			image = match.Groups["image"].Value;
			title = match.Groups["artist"].Value + " - " + match.Groups["album"].Value

			results.Add("http://images.theorchard.com/release/cover/" + image, title, match.Groups["url"].Value, -1, -1, "http://images.theorchard.com/release/large_cover/" + image, CoverType.Front)

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;