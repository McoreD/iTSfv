# refs: System.Web.Extensions

import System.Text
import System.Text.RegularExpressions
import System.Web.Script.Serialization
import AlbumArtDownloader.Scripts
import util

//Inheritors should override the Suffix property to return a valid amazon suffix (like com, co.uk, de, etc...).
abstract class Amazon(AlbumArtDownloader.Scripts.IScript):
	virtual IncludeCustomerImages as bool:
		get: return true	// To avoid including customer images in the results, replace true with false here
	virtual IncludeOfficalImages as bool:
		get: return true	// To avoid including official images in the results, replace true with false here
	virtual Name as string:
		get: return "Amazon (.${Suffix})"
	Version as string:
		get: return "0.13s"
	Author as string:
		get: return "Alex Vallat, ZOOT"
	abstract protected Suffix as string:
		get: pass
	virtual protected CountryCode as string:
		get: return "01"
	virtual protected SearchIndex as string: //Deprectated, ignored.
		get: return "" 
	virtual protected def GetUrl(artist as string, album as string) as string:
		return "http://www.amazon.${Suffix}/gp/search?search-alias=popular&field-artist=${EncodeUrl(artist)}&field-title=${EncodeUrl(album)}&sort=relevancerank"
	
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)
		
		url = GetUrl(artist, album)
		resultsPage = GetPage(GetPageStream(url, null, true))

		resultsRegex = Regex("<a[^>]*title\\s*=\\s*\"(?<title>[^\"]+)\"[^>]*href\\s*=\\s*\"(?<url>[^\"]+?/dp/(?<id>[^/]+)/)[^>]+><h2.*?>(?<artist>[^<]+)(?:</a>)?</span></div></div>", RegexOptions.Singleline | RegexOptions.IgnoreCase)
		resultsMatches = resultsRegex.Matches(resultsPage)
		
		results.EstimatedCount = resultsMatches.Count
		
		if IncludeOfficalImages:
			// Add official images first
			for resultsMatch as Match in resultsMatches:
				id = resultsMatch.Groups["id"].Value
				url = resultsMatch.Groups["url"].Value
				title = System.Web.HttpUtility.HtmlDecode(resultsMatch.Groups["title"].Value)
				artist = System.Web.HttpUtility.HtmlDecode(resultsMatch.Groups["artist"].Value)
				imageBase = "http://ecx.images-amazon.com/images/P/${id}.${CountryCode}."

				thumbnail = TryGetImageStream(imageBase + "_THUMB_")

				results.Add(thumbnail, "${artist} - ${title}", url, -1, -1, imageBase, CoverType.Front)

		if IncludeCustomerImages:
			// Now add customer images
			json = JavaScriptSerializer()
			count = 0
			for resultsMatch as Match in resultsMatches:
				// We hit a page for each result.  Searches on Amazon should generally return the
				// item that was searched for quickly if it's going to be found at all, so don't
				// hammer the server.
				count++
				if count > 5:
						break
 
				id = resultsMatch.Groups["id"].Value
				url = resultsMatch.Groups["url"].Value
				title = System.Web.HttpUtility.HtmlDecode(resultsMatch.Groups["title"].Value)
				artist = System.Web.HttpUtility.HtmlDecode(resultsMatch.Groups["artist"].Value)
				imageBase = "http://ecx.images-amazon.com/images/P/${id}.${CountryCode}."
 
				images_url = "http://www.amazon.${Suffix}/gp/customer-media/product-gallery/${id}"
				imagesPage = GetPage(GetPageStream(images_url, null, true))
				jsonRegex = Regex('var state = (?<json>{[^;]*});', RegexOptions.Multiline)
				for jsonDataMatch as Match in jsonRegex.Matches(imagesPage):
						jsonData = jsonDataMatch.Groups["json"].Value
 
						// amazon.co.jp uses double-width backslashes when escaping JS strings.  No, really.
						jsonData = Regex("＼").Replace(jsonData, "\\")
 
						result = json.Deserialize[of ImageInfo](jsonData)
						if result.imageList != null:
							for image as ImageInfo.Image in result.imageList:
									thumbnail_url = image.url
									thumbnail_url = Regex("\\.jpg$").Replace(thumbnail_url, "._SX120_.jpg")
 
									results.Add(thumbnail_url, "${artist} - ${title}",
											images_url + "?currentImageID=${image.id}", image.width, image.height,
											image.url, CoverType.Front)

	def RetrieveFullSizeImage(imageBase):
		if imageBase.EndsWith(".jpg"): // Customer images never have larger sizes (and must end in .jpg)
			return TryGetImageStream(imageBase)

		imageStream = TryGetImageStream(imageBase + "_SCRM_")
		if imageStream != null:
			return imageStream

		//Fall back on Large size
		return TryGetImageStream(imageBase + "_SCL_")

	def TryGetImageStream(url):
		request as System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(url)
		try:
			response = request.GetResponse()
			if response.ContentLength > 43:
				return response.GetResponseStream()
			
			response.Close()
			return null
		except e as System.Net.WebException:
			return null

class ImageInfo:
		public pageUrl as string
		public imageList as List[Image]
		class Image:
				public url as string
				public id as string
				public width as int
				public height as int