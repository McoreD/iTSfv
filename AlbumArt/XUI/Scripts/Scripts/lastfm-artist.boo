import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class LastFmArtist(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "LastFM Artist"
	Version as string:
		get: return "0.6"
	Author as string:
		get: return "Alex Vallat, pochaboo"
	Category as string:
		get: return "Artist Images"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)

		encodedArtist = EncodeUrl(artist)
		imagesHtml = GetPage("http://www.last.fm/music/${encodedArtist}/+images")

		imageIdMatches = Regex("<a\\s+href=\"/music/[^/]+/\\+images/(?<id>[^\"]+)\"[^<]+<img src=\"(?<imageUrlBase>[^\"]+?)(?<thumbnail>\\d+x\\d+)/").Matches(imagesHtml)
		
		results.EstimatedCount = imageIdMatches.Count
		
		for imageIdMatch as Match in imageIdMatches:
			id = imageIdMatch.Groups["id"].Value
			imageUrlBase = imageIdMatch.Groups["imageUrlBase"].Value
			thumbnail = imageIdMatch.Groups["thumbnail"].Value
			results.Add(imageUrlBase + thumbnail + "/" + id, artist, "http://www.last.fm/music/${encodedArtist}/+images/${id}", -1, -1, imageUrlBase + "ar0/" + id);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;