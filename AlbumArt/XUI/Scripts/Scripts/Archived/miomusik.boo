import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class MioMusik(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "MioMusik"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)
		query = EncodeUrl(artist + " " + album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.miomusik.com/search.php?q=" + query)
		
		matches = Regex("<img src='(?<url>[^']+?)_small.jpg' alt='(?<title>[^']+)'", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count

		for match as Match in matches:
			url = "http://www.miomusik.com" + match.Groups["url"].Value
			
			results.Add(url + "_small.jpg", match.Groups["title"].Value, url + ".php", -1, -1, url + "_large.jpg", CoverType.Front)

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter