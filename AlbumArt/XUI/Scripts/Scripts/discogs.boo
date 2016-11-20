# refs: System.Web.Extensions

import System
import System.Text.RegularExpressions
import System.Web.Script.Serialization

import util

class Discogs(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Discogs"
	Author as string:
		get: return "Alex Vallat"
	Version as string:
		get: return "0.16"
	def Search(artist as string, album as string, results as IScriptResults):
		//artist = StripCharacters("&.'\";:?!", artist)
		//album = StripCharacters("&.'\";:?!", album)

		json = JavaScriptSerializer()

		resultsInfoJson = GetDiscogsPage("http://www.discogs.com/search/ac?searchType=release&q=" + EncodeUrl("\"${artist}\" \"${album}\""))
		resultsInfo = json.Deserialize[of (Result)](resultsInfoJson)
		
		results.EstimatedCount = resultsInfo.Length;
		
		for result in resultsInfo:
			// Get the release info from api
			title = result.artist[0] + " - " + result.title[0];
			url = result.uri[0]
			//id = url.Substring(url.LastIndexOf('/'));

			releasePage = GetDiscogsPage("http://www.discogs.com" + url)
			releasePageImagesMatch = Regex("data-images='(?<json>[^']+)'", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(releasePage)
			if releasePageImagesMatch.Success:
				imagesJson = releasePageImagesMatch.Groups["json"].Value
				images = json.Deserialize[of (Image)](imagesJson)
			
				results.EstimatedCount += images.Length - 1
				for image in images:
					results.Add(GetPageStream(image.thumb, null, true), title, "http://www.discogs.com" + url, image.width, image.height, image.full, CoverType.Unknown)

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return GetPageStream(fullSizeCallbackParameter, null, true);

	def GetDiscogsPage(url):
		stream = GetPageStream(url, null, true)
		try:
			return GetPage(stream)
		ensure:
			stream.Close()
	
	class Result:
		public artist as (String)
		public title as (String)
		public uri as (String)

	class Image:
		public thumb as String
		public full as string
		public width as int
		public height as int
