import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class Coverlandia(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Coverlandia"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://coverlandia.net/?s=" + EncodeUrl(artist + " " + album))
		
		matches = Regex("<a href=\\\"(?<url>[^\"]+)\" class=\"title\">(?<title>[^<]+)<.*?<a onclick[^>]+? href=\"(?<full>[^\"]+)[^>]*><img[^>]+? src=\"(?<thumb>[^\"]+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			image = match.Groups["image"].Value;

			results.Add(match.Groups["thumb"].Value, System.Web.HttpUtility.HtmlDecode(match.Groups["title"].Value), match.Groups["url"].Value, -1, -1, match.Groups["full"].Value, CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;
