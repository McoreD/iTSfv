import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class Qobuz(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Qobuz"
	Version as string:
		get: return "0.2"
	Author as string:
		get: return "Alex Vallat, thomian"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.qobuz.com/recherche?i=boutique&q=" + EncodeUrl(artist + " " + album))
		
		matches = Regex("<a href=\"(?<url>[^\"]+)\"[^>]+>\\s+<img alt=\"(?<title>[^\"]+)\"[^>]*rel=\"(?<id>(?<idPrefix>[^\"]{4})[^\"]*(?<idPrefix2>[^\"]{2})(?<idPrefix1>[^\"]{2}))\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count

		for match as Match in matches:
			infoUrl = match.Groups["url"].Value
			title = match.Groups["title"].Value
			id = match.Groups["id"].Value
			//idPrefix = match.Groups["idPrefix"].Value
			idPrefix1 = match.Groups["idPrefix1"].Value
			idPrefix2 = match.Groups["idPrefix2"].Value
			urlBase = "http://static.qobuz.com/images/covers/${idPrefix1}/${idPrefix2}/${id}"
			// in previous script versions the image was reached using URL-Base:
			// urlBase = "http://static.qobuz.com/images/jaquettes/${idPrefix}/${id}"
			// this link still works, but not for max size images, only for 600px images

			// See if max size jpg is available
			if CheckResponse(urlBase + "_max.jpg"):
				fullSizeImageUrl = urlBase + "_max.jpg"
			else:
				// fall back on 600x600
				fullSizeImageUrl = urlBase + "_600.jpg"
			
			results.Add(urlBase + "_100.jpg", title, "http://www.qobuz.com" + infoUrl, -1, -1, fullSizeImageUrl, CoverType.Front)

	def CheckResponse(url):
		checkRequest as System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(url)
		checkRequest.Method = "HEAD"
		checkRequest.AllowAutoRedirect = false
		try:
			response = checkRequest.GetResponse() as System.Net.HttpWebResponse
			return response.StatusCode == System.Net.HttpStatusCode.OK
		except e as System.Net.WebException:
			return false;
		ensure:
			if response != null:
				response.Close()

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter