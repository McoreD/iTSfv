import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class Coverlandia(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "Coverlandia"
	Version as string:
		get: return "0.2"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "Fan-made covers"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://coverlandia.net/?s=FanMade+" + EncodeUrl(artist + " " + album))
		
		matches = Regex("<a href=\\\"(?<url>[^\"]+)\" class=\"title\">(?<title>[^<]+)<.*?<a onclick[^>]+? href=\"(?<full>[^\"]+)[^>]*><img[^>]+? src=\"(?<thumb>[^\"]+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			results.Add(match.Groups["thumb"].Value, System.Web.HttpUtility.HtmlDecode(match.Groups["title"].Value), match.Groups["url"].Value, -1, -1, match.Groups["full"].Value, CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;
