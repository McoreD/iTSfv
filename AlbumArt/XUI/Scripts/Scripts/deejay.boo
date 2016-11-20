import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class Deejay(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "deejay.de"
	Version as string:
		get: return "0.5"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)
		query = EncodeUrl("\"${artist} - ${album}\"")

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.deejay.de/content.php?param=" + query)
		
		matches = Regex("href=\"(?<url>[^\"]+)\"><img src=\"[^\"]+?pics/[^/]+/(?<img>[^.]+.jpg)\" alt=\"(?<title>[^\"]+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count * 2

		for match as Match in matches:
			info = "http://www.deejay.de/" + match.Groups["url"].Value
			img = match.Groups["img"].Value
			title = match.Groups["title"].Value.Replace("&nbsp;", " ");
			
			if img.EndsWith("b.jpg"):
				coverType = CoverType.Back
			else:
				coverType = CoverType.Front

			results.Add("http://www.deejay.de/images/s/" + img, title, info, -1, -1, "http://www.deejay.de/images/xl/" + img, coverType)

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter

	def DeHtml(html):
		return Regex.Replace(html, "<[^>]+>", "");