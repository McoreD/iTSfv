import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class HitParade(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "hitparade.ch"
	Version as string:
		get: return "0.5"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		searchPage as string = GetPage("http://hitparade.ch/search.asp?cat=a&artist=${artist}&title=${album}")
		
		albumMatches = Regex("(?<!<!--)<a href=\"(?<url>/album/[^\"]+)\">", RegexOptions.IgnoreCase).Matches(searchPage)
		
		results.EstimatedCount = albumMatches.Count / 2 //Regex produces two matches per real result
		
		uniqueAlbumns = System.Collections.Generic.Dictionary[of string, object]()
		uniqueImages = System.Collections.Generic.Dictionary[of string, object]()

		for albumMatch as Match in albumMatches:
			url = "http://hitparade.ch" + albumMatch.Groups["url"].Value

			if (not uniqueAlbumns.ContainsKey(url)):
				uniqueAlbumns.Add(url, null)
				albumPage as string = GetPage(url)
				
				title = Regex("<title>(?<title>.+) - hitparade.ch</title>", RegexOptions.IgnoreCase).Match(albumPage).Groups["title"].Value
				artMatch = Regex("<img src=http://hitparade\\.ch/cdimag/(?<image>[^ ]+) ", RegexOptions.IgnoreCase).Match(albumPage)
				if artMatch.Success:
					image = artMatch.Groups["image"].Value

					if (not uniqueImages.ContainsKey(image)):
						uniqueImages.Add(image, null)
						results.Add("http://hitparade.ch/cdimag/${image}", title, url, -1, -1, "http://hitparade.ch/cdimages/${image}", CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;