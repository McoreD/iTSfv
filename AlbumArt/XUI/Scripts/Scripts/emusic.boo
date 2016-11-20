import AlbumArtDownloader.Scripts
import util

class eMusic(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "eMusic"
	Version as string:
		get: return "0.5"
	Author as string:
		get: return "Alex Vallat, DRata"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		searchResultsHtml as string = GetPage("http://www.google.com/search?q=site%3Aemusic.com+inurl:album" + EncodeUrl(" inurl:\"" + artist + "\" inurl:\"" + album + "\""))
		
		matches = Regex("(?:href=\"|url\\?q=)(?<url>http://[^/]*emusic.com/album/(?<title>[^\"]+)/(?<id>(?<idPart>\\d{3}){2}\\d+)[^\"&]*)[&\"]", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			url = match.Groups["url"].Value
			id = match.Groups["id"].Value
			idParts = match.Groups["idPart"].Captures
			title = match.Groups["title"].Value
			title = title.Replace('-MP3-Download', '') // Remove -MP3-Download if found in URL
			title = title.Replace('-', ' ')

			imageUrlBase = "http://images.emusic.com/music/images/album/0/${idParts[0]}/${idParts[1]}/${id}/"
			
			results.Add(imageUrlBase + "300x300.jpg", title, url, -1, -1, imageUrlBase, CoverType.Front);


	def RetrieveFullSizeImage(imageUrlBase):
		imageStream = TryGetImageStream(imageUrlBase + "1400x1400.jpg")

		if imageStream != null:
			return imageStream
		else:
			// Couldn't find 1400x1400, try 600x600
			return TryGetImageStream(imageUrlBase + "600x600.jpg")

	def TryGetImageStream(url):
		request as System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(url)
		try:
			response = request.GetResponse()
			return response.GetResponseStream()
		except e as System.Net.WebException:
			return null