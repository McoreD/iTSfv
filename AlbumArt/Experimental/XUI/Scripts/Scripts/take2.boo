import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class Take2(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Take2"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.take2.co.za/search.php?post=1&advanced=1&method=word&stype=5&pagesize=100&search=" + EncodeUrl(artist + " " + album))
		
		matches = Regex("<img src=\"http://images.take2.co.za/covers/small/(?<image>[^\"]+)\"[^>]+?class=\"cover.+?<a href=\"(?<info>[^\"]+)\" class=\"itemtitle\">\\s*(?<title>[^<]+)</a>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			image = match.Groups["image"].Value;

			results.Add("http://images.take2.co.za/covers/small/" + image, System.Web.HttpUtility.HtmlDecode(match.Groups["title"].Value), "http://www.take2.co.za/" + match.Groups["info"].Value, -1, -1, "http://images.take2.co.za/covers/big/" + image, CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;
