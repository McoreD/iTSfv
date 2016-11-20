import System
import System.Xml
import AlbumArtDownloader.Scripts
import util

class Beatport(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "Beatport"
	Version as string:
		get: return "0.2"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "Dance, Club, Electronic"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		if String.IsNullOrEmpty(artist) or String.IsNullOrEmpty(album):
			query = artist + album
		else:
			query = "\"" + artist + "\" \"" + album + "\""

		searchResults = GetPage("https://pro.beatport.com/search/releases?q="+ EncodeUrl(query) + "&_pjax=%23pjax-inner-wrapper")

		matches = Regex("<img class=\"release-artwork[^>]+?data-src=\"(?<thumbnail>[^\"]+?(?<id>\\d+)\\.jpg)\".*?<p class=\"release-title\"><a href=\"(?<url>[^\"]+)\">(?<title>[^<]+)<.*?<a href=\"/artist[^>]+>(?<artist>[^<]+)<", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResults)
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			url = match.Groups["url"].Value
			id = match.Groups["id"].Value
			title = System.Web.HttpUtility.HtmlDecode(match.Groups["title"].Value)
			artist = System.Web.HttpUtility.HtmlDecode(match.Groups["artist"].Value)
			thumbnail = match.Groups["thumbnail"].Value

			results.Add(thumbnail, title, url, -1, -1, id, CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		fullId = int.Parse(fullSizeCallbackParameter) + 1 // Add one to the ID to get a larger image (ref https://www.youtube.com/watch?v=MYvzxS1v3Ws)
		url = "https://geo-media.beatport.com/image/" + fullId.ToString() + ".jpg"
		if CheckResponse(url):
			return url;
		else:
			return "https://geo-media.beatport.com/image/" + fullSizeCallbackParameter + ".jpg"

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