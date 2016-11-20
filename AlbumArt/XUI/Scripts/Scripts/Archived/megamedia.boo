import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class MegaMedia(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "Mega Media"
	Version as string:
		get: return "0.2"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "Netherlands"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResults = GetPage("http://www.mega-media.nl/zoeken.php?artiest=${EncodeUrl(artist)}&how=and&term=${EncodeUrl(album)}&submit=Zoek")

		matches = Regex("info\\.php\\?lid=(?<lid>\\d+)\">(?<artist>[^>]+)</a></td></tr><tr><td title=\"(?<album>[^\"]+)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline).Matches(searchResults)
		
		// Hopelessley inaccurate count, as their search returns a *lot* of dupes
		results.EstimatedCount = matches.Count
		
		uniqueLids = System.Collections.Generic.Dictionary[of string, object]()

		for match as Match in matches:
			lid = match.Groups["lid"].Value

			if not uniqueLids.ContainsKey(lid):
				uniqueLids.Add(lid, null)
				title = match.Groups["artist"].Value + " - " + match.Groups["album"].Value
				imageUrlBase = "http://download.aim4music.com/shopsupport/download_process/bundle$artwork.aspx?bundle_id=" + lid
				
				thumbnail = TryGetImageStream(imageUrlBase + "&width=120&height=120")
				if thumbnail != null:
					results.Add(thumbnail, title, "http://www.mega-media.nl/info.php?lid=" + lid, -1, -1, imageUrlBase + "&height=2000&width=2000", CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;

	def TryGetImageStream(url):
		request as System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(url)
		try:
			response = request.GetResponse()
			if response.ContentLength > 0:
				return response.GetResponseStream()
			
			response.Close()
			return null
		except e as System.Net.WebException:
			return null