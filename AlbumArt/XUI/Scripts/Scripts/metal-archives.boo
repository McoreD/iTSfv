import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class EncyclopaediaMetallum(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "Encyclopaedia Metallum"
	Version as string:
		get: return "0.6"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "Punk, Metal, Rock"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResultsJson as string = GetPage("http://www.metal-archives.com/search/ajax-advanced/searching/albums/?bandName=${EncodeUrl(artist)}&releaseTitle=${EncodeUrl(album)}&sEcho=1&iDisplayStart=0&iDisplayLength=100")
		
		matches = Regex("/bands/[^>]+>(?<artist>[^<]+)</a>.+?/albums/[^/]+/[^/]+/(?<id>(?<idPart>\\d)+)\\\\\">(?<album>[^<]+)<", RegexOptions.IgnoreCase | RegexOptions.Singleline).Matches(searchResultsJson)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			name = match.Groups["artist"].Value + " - " + match.Groups["album"].Value
			
			url = "http://www.metal-archives.com/images/"
			
			idParts = match.Groups["idPart"].Captures
			for i in range(Math.Min(idParts.Count, 4)):
				url += idParts[i] + "/"

			url += match.Groups["id"].Value + ".jpg"

			results.Add(url, name, null, -1, -1, null, CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;