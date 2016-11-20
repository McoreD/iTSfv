import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class SFRMusic(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "SFR Music"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)
		
		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://telecharger.musique.sfr.fr/chercher.action?keywordArtist=${artist}&keywordAlbum=${album}")
		
		matches = Regex("<td class=\"columnImg\"><a [^>]+?title=\"(?<title>[^\"]+)\" href=\"(?<url>[^\"]+)\"><img src=\"(?<image>[^\"]+?)&width=157&height=157\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count

		for match as Match in matches:
			image = match.Groups["image"].Value
			
			results.Add(image + "&width=157&height=157", match.Groups["title"].Value, "http://telecharger.musique.sfr.fr" + match.Groups["url"].Value, 600, 600, image + "&width=600&height=600", CoverType.Front)

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter