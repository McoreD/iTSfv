import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class DJshop(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "DJshop"
	Version as string:
		get: return "0.4"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)
		query = EncodeUrl(artist + " " + album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.djshop.de/Searchresults-CD/ex/s~searchresults,u~mp3,sort~relevance,p2~${query}/xe/searchresultsDownload.html")
		
		matches = Regex("<a href=\"(?<url>[^\"]+)\"[^>]+><img src=\"(?<imageUrlBase>https?://download\\.feiyr\\.com/[^\"]+?,)80(?<imageUrlTail>[^\"]+)\" alt=\"(?<title>[^\"]+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count

		for match as Match in matches:
			title = match.Groups["title"].Value
			imageUrlBase = match.Groups["imageUrlBase"].Value
			imageUrlTail = match.Groups["imageUrlTail"].Value
			
			results.Add(imageUrlBase + "80" + imageUrlTail, title, "http://www.djshop.de" + match.Groups["url"].Value, -1, -1, imageUrlBase + "1000" + imageUrlTail, CoverType.Front)

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter