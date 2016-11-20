import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class LastFmArtist(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "LastFM Artist"
	Version as string:
		get: return "0.4"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)

		encodedArtist = EncodeUrl(artist)
		imagesHtml = GetPage("http://www.last.fm/music/${encodedArtist}/+images")

		imageIdMatches = Regex("<a href=\"/music/[^/]+/\\+images/(?<id>\\d+)\" class=\"pic\"").Matches(imagesHtml)
		
		results.EstimatedCount = imageIdMatches.Count
		
		for imageIdMatch as Match in imageIdMatches:
			id = imageIdMatch.Groups["id"].Value
			results.Add("http://userserve-ak.last.fm/serve/126b/${id}.jpg", artist, "http://www.last.fm/music/${encodedArtist}/+images/${id}", -1, -1, "http://userserve-ak.last.fm/serve/_/${id}.jpg");

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;