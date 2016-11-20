import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class GigaCrate(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "GigaCrate"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "Dance, Club, Electronic"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)
		query = EncodeUrl(artist + " " + album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.gigacrate.com/Music/MusicSearchResults.php?quicksearch[terms]=${query}&do_quicksearch=1")
		
		matches = Regex("<a href=\"/Albums/(?<id>\\d+)\" title[^>]+>(?<title>[^<]+)<", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count

		for match as Match in matches:
			title = match.Groups["title"].Value
			id = match.Groups["id"].Value
			urlBase = "http://www.gigacrate.com/images/album_art/${id}"

			results.Add(urlBase + "Small.jpg", title, "http://www.gigacrate.com/Albums/${id}", 500, 500, urlBase + "XL.jpg", CoverType.Front)

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter