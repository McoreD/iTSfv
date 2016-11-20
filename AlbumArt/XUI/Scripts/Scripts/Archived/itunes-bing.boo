import AlbumArtDownloader.Scripts
import util

class iTunesBing(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "iTunes/Bing"
	Version as string:
		get: return "0.7"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		searchResultsHtml as string = GetPage("http://www.bing.com/search?q=site%3Aitunes.apple.com%2F+" + EncodeUrl("intitle:\"${artist}\" intitle:\"${album}\""))
		
		matches = Regex("<a href=\"(?<url>http://itunes\\.apple\\.com/[^/]+/album/[^\"]+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			url = match.Groups["url"].Value
			
			// Now fetch the iTunes page
			albumResultHtml as string = GetPage(url)
			albumMatch = Regex("<img [^>]+?alt=\"(?<title>[^>]+?)\" class=\"artwork\" src=\"(?<image>[^\"]+?)170x170-75\\.jpg\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(albumResultHtml)

			imageUrlBase = albumMatch.Groups["image"].Value
			
			results.Add(imageUrlBase + "170x170-75.jpg", albumMatch.Groups["title"].Value, url, -1, -1, imageUrlBase, CoverType.Front);

	def RetrieveFullSizeImage(imageUrlBase):
		imageStream = TryGetImageStream(imageUrlBase + "jpg")

		if imageStream != null:
			return imageStream
		else:
			// Couldn't find full size .jpg, try .tif
			imageStream = TryGetImageStream(imageUrlBase + "tif")

			if imageStream != null:
				return imageStream
			else:
				// Couldn't find full size .jpg or .tif, fall back on 600x600
				return TryGetImageStream(imageUrlBase + "600x600-75.jpg")

	def TryGetImageStream(url):
		request as System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(url)
		try:
			response = request.GetResponse()
			return response.GetResponseStream()
		except e as System.Net.WebException:
			return null