import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class SoundStation(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "Sound Station"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "LP Vinyl"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("\"", artist)
		album = StripCharacters("\"", album)
		query = EncodeUrl("\"" + artist + "\" \"" + album + "\"")

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.soundstation.dk/data/search.aspx?q=${query}&fi=all&fo=All+formats")
		
		matches = Regex("'/data/products/(?<id>\\d+)\\.aspx'\"><td>(?<artist>[^<]+)</td><td>(?<title>[^<]+)</td>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		for match as Match in matches:
			//Retrieve the details page
			title = match.Groups["artist"].Value + " - " + match.Groups["title"].Value
			id = match.Groups["id"].Value
			idPart = id.Substring(id.Length - 2, 2)

			thumbUrl = "http://www.soundstation.dk/images/products/small/${idPart}/${id}-"
			fullUrl = "http://www.soundstation.dk/images/products/large/${idPart}/${id}-"
			detailsUrl = "http://www.soundstation.dk/data/products/${id}.aspx"

			frontThumb = TryGetImageStream(thumbUrl + "a.jpg")
			if frontThumb != null:
				results.Add(frontThumb, title, detailsUrl, -1, -1, fullUrl + "a.jpg", CoverType.Front)
			
			backThumb = TryGetImageStream(thumbUrl + "b.jpg")
			if backThumb != null:
				results.Add(backThumb, title, detailsUrl, -1, -1, fullUrl + "b.jpg", CoverType.Back)

			cdThumb = TryGetImageStream(thumbUrl + "c.jpg")
			if cdThumb != null:
				results.Add(cdThumb, title, detailsUrl, -1, -1, fullUrl + "c.jpg", CoverType.CD)
	
	def TryGetImageStream(url):
		request as System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(url)
		try:
			response = request.GetResponse()
			return response.GetResponseStream()
		except e as System.Net.WebException:
			return null

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter	