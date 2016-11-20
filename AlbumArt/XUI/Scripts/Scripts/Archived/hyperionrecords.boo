import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class HyperionRecords(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "Hyperion Records"
	Version as string:
		get: return "0.2"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "Classical"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results
		searchResultsHtml as string = GetPage(GetPageStream("http://www.hyperion-records.co.uk/find.asp?f=" + EncodeUrl(artist + " " + album), null, true))
		
		matches = Regex("<a href=\"(?<url>al\\.asp[^\"]+)\"[^>]+>\\s*<img src=\"/thumbs_75/(?<image>[^\"]+)\" alt=\"(?<title>[^\"]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			url = "http://www.hyperion-records.co.uk/" + match.Groups["url"].Value
			image = match.Groups["image"].Value
			results.Add("http://www.hyperion-records.co.uk/thumbs_75/" + image, match.Groups["title"].Value, url, -1, -1, "http://www.hyperion-records.co.uk/jpegs/150dpi/" + image, CoverType.Front)

	def RetrieveFullSizeImage(value):
		return value