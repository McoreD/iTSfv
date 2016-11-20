# refs: System.Web.Extensions

import System
import System.Text.RegularExpressions
import System.Web.Script.Serialization

import util

class Deezer(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Deezer"
	Author as string:
		get: return "Alex Vallat"
	Version as string:
		get: return "0.6"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		jsonSearchResults = GetPage("http://api.deezer.com/2.0/search/album?q=" + EncodeUrl(artist + " " + album))
			
		json = JavaScriptSerializer()
		searchResults = json.Deserialize[of SearchResults](jsonSearchResults)

		if searchResults.data.Length > 0:
			results.EstimatedCount = searchResults.data.Length

			imageIdRegex = Regex("cover/(?<id>[^/]+)/", RegexOptions.Singleline | RegexOptions.IgnoreCase)
		
			for album in searchResults.data:
				title = album.artist.name + " - " + album.title

				// Fetch image, to determine ID
				checkRequest = System.Net.HttpWebRequest.Create(album.cover) as System.Net.HttpWebRequest
				checkRequest.Method = "HEAD"
				checkRequest.AllowAutoRedirect = false
				response = checkRequest.GetResponse()
				thumbnail = response.Headers["Location"]
				response.Close()

				match = imageIdRegex.Match(thumbnail)
				
				results.Add(thumbnail, title, album.link or String.Empty, 1400, 1400, match.Groups["id"].Value, CoverType.Front, "png")

	def RetrieveFullSizeImage(id):
		for size in range(1400, 200, -200):
			imageStream = TryGetImageStream("http://cdn-images.deezer.com/images/cover/${id}/${size}x${size}-000000-0-0-0.png")
			if imageStream != null:
				return imageStream
		
		return null

	def TryGetImageStream(url):
		request as System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(url)
		try:
			return request.GetResponse().GetResponseStream()
		except e as System.Net.WebException:
			return null

	class SearchResults:
		public data as (Album)
		
		class Album:
			public id as String
			public title as String
			public link as String
			public cover as String
			public artist as Artist
				
			class Artist:
				public name as String