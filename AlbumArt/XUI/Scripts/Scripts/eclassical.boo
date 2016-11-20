import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class eClassical(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "eClassical"
	Version as string:
		get: return "0.2"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "Classical"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.eclassical.com/en/search.php?op=search&text=" + EncodeUrl(artist + " " + album))
		
		matches = Regex("<a href=\"(?<url>[^\"]+)\"><img src=\"[^\"]+?/shop/thumbnails/shop/(?<img>[^\"]+?\\.jpg)[^\"]+\"[^>]+?alt=\"(?<title>[^\"]+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			img = match.Groups["img"].Value;

			results.Add("http://ecstatic.textalk.se/shop/thumbnails/shop/${img}_0_0_100_100_100_100_0.jpg", match.Groups["title"].Value, "http://www.eclassical.com" + match.Groups["url"].Value, -1, -1, "http://ecstatic.textalk.se/shop/${img}", CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;