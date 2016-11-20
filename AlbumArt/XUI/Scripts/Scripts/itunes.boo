# refs: System.Web.Extensions
import System.Collections.Generic
import System.Web.Script.Serialization
import AlbumArtDownloader.Scripts
import util

class iTunes(AlbumArtDownloader.Scripts.IScript):
	virtual Name as string:
		get: 
			name = "iTunes"
			if not System.String.IsNullOrEmpty(CountryName):
				name += " (${CountryName})"
			return name
	Version as string:
		get: return "0.5"
	Author as string:
		get: return "Alex Vallat"
	virtual protected CountryName as string:
		get: return null
	virtual protected CountryCode as string:
		get: return "US"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)
		
		searchResultsJson as string = GetPage("http://itunes.apple.com/search?entity=album&country=${CountryCode}&term=" + EncodeUrl("\"" + artist + "\" \"" + album + "\""))

		json = JavaScriptSerializer()
		searchResults = json.DeserializeObject(searchResultsJson) as Dictionary[of string, object]
		
		results.EstimatedCount = searchResults["resultCount"]
		
		for result as Dictionary[of string, object] in searchResults["results"]:
			title = result["artistName"] + " - " + result["collectionName"]
			url = result["collectionViewUrl"]
			imageUrlBase = result["artworkUrl100"]
			// Remove size from image to get base
			sizeMatch = Regex("^(?<imageUrlBase>.+)100x100.+\\.jpg$", RegexOptions.IgnoreCase).Match(imageUrlBase) //example: "100x100-75.jpg" or "100x100bb.jpg"
			imageUrlBase = sizeMatch.Groups["imageUrlBase"].Value
				
			// See if full size jpg is available
			if CheckResponse(imageUrlBase + "jpg"):
				fullSizeImageUrl = imageUrlBase + "jpg"
				extension = "jpg"
			elif CheckResponse(imageUrlBase + "5000x5000-100.jpg"):	//we can often get full size image!
				fullSizeImageUrl = imageUrlBase + "5000x5000-100.jpg"
				extension = "jpg"
			elif CheckResponse(imageUrlBase + "5000x5000-75.jpg"):
				fullSizeImageUrl = imageUrlBase + "5000x5000-75.jpg"
				extension = "jpg"
			elif CheckResponse(imageUrlBase + "tif"): // Couldn't find full size .jpg, try .tif
				fullSizeImageUrl = imageUrlBase + "tif"
				extension = "tiff"
			elif CheckResponse(imageUrlBase + "1200x1200-75.jpg"): // Couldn't find full size .jpg or .tif, fall back on 1200x1200
				fullSizeImageUrl = imageUrlBase + "1200x1200-75.jpg"
				extension = "jpg"
			else:
				// Final fall back on 600x600
				fullSizeImageUrl = imageUrlBase + "600x600-75.jpg"
				extension = "jpg"
			
			results.Add(imageUrlBase + "170x170-75.jpg", title, url, -1, -1, fullSizeImageUrl, CoverType.Front, extension);

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
