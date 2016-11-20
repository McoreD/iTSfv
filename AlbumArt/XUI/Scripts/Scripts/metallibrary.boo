import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class MetalLibrary(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "Metal Library"
	Version as string:
		get: return "0.3"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "Punk, Metal, Rock"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the artist page
		artistPrefix = artist.Replace(" ", ".")
		artistPage as string = GetPage("http://${artistPrefix}.metallibrary.ru")
		
		albumMatches = Regex("<a href=\"(?<url>/bands/discographies/[^\"]+)\">\"[^\"]*${album}[^\"]*\"</a>", RegexOptions.IgnoreCase).Matches(artistPage)
		
		results.EstimatedCount = albumMatches.Count
		
		for albumMatch as Match in albumMatches:
			albumPage as string = GetPage("http://www.metallibrary.ru" + albumMatch.Groups["url"].Value)
			
			artMatches = Regex("<img class=\"bordered\" src=\"(?<image>/bands/discographies/[^\"]+)\" alt=\"(?<title>[^\"]+)\"", RegexOptions.IgnoreCase).Matches(albumPage)
			
			for artMatch as Match in artMatches:
				name = artMatch.Groups["title"].Value.Replace("&quot;", "\"").Replace("&ndash;", "-")
				url = "http://www.metallibrary.ru" + artMatch.Groups["image"].Value
			
				results.Add(url, name, null, -1, -1, null, CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;