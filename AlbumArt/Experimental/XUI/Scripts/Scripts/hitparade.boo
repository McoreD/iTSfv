import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class HitParade(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "hitparade.ch"
	Version as string:
		get: return "0.2"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		searchPage as string = GetPage("http://hitparade.ch/search.asp?cat=a&artist=${artist}&title=${album}")
		
		albumMatches = Regex("<a href=(?<url>showitem\\.asp\\?interpret=[^&]+&titel=[^&]+&cat=a)>", RegexOptions.IgnoreCase).Matches(searchPage)
		
		results.EstimatedCount = albumMatches.Count / 2 //Regex produces two matches per real result
		
		skip = false
		for albumMatch as Match in albumMatches:
			if skip:
				skip = false
			else:
				skip = true //Skip the next one, as there are two matches per result
				
				url = "http://hitparade.ch/" + albumMatch.Groups["url"].Value
				albumPage as string = GetPage(url)
				
				title = Regex("<title>(?<title>.+) - hitparade.ch</title>", RegexOptions.IgnoreCase).Match(albumPage).Groups["title"].Value
				artMatch = Regex("http://hitparade.ch/cdimage\\.html\\?(?<image>[^']+)", RegexOptions.IgnoreCase).Match(albumPage)
				if artMatch.Success:
					image = artMatch.Groups["image"].Value
					
					results.Add("http://hitparade.ch/cdimg/${image}", title, url, -1, -1, "http://hitparade.ch/cdimages/${image}", CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;