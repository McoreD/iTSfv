import System
import System.IO
import System.Net
import System.Drawing
import System.Drawing.Imaging
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class BuyDotCom(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Buy.com"
	Version as string:
		get: return "0.8.5"
	Author as string:
		get: return "alsaan, DRata, Alex Vallat"
		
	def Search(artist as string, album as string, results as IScriptResults):
	
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)
	  	
		searchParameter = EncodeUrl(artist + " " + album)
		  	  
		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.rakuten.com/sr/srajax.aspx?qu=${searchParameter}&from=6&sid=6&page=1&pv=1")
		
		//Check whether we actually got any relevant result or not
		if(searchResultsHtml.IndexOf("did not return an exact match") > -1):
			return

		//Remove "Results ... in All Categories"
		allCategories = searchResultsHtml.IndexOf(" in all categories ")
		if(allCategories > -1):
			searchResultsHtml = searchResultsHtml.Substring(0, allCategories)

		//Remove "Similar Products in "
		similar = searchResultsHtml.IndexOf(">Similar Products in ")
		if(similar > -1):
			searchResultsHtml = searchResultsHtml.Substring(0, similar)
		
		//Extract all the thumbnails and the links to product pages
		itemsRegex = Regex("<div class=\"product-image-container\">\\s*<a href=\"(?<productPageUrl>[^\"]*/(?<sku>[^\"]*)\\.html)\"[^>]*>\\s*<img[^>]*src=\"(?<thumbnailUrl>[^\"]*)\"[^>]*title=\"(?<title>[^\"]*)\"[^>]")
		itemsMatches = itemsRegex.Matches(searchResultsHtml)
		
		//Set the estimated number of covers available (approx. 1 cover per product page)
		results.EstimatedCount = itemsMatches.Count
		
		for itemMatch in itemsMatches:
			AddThumbnail(results, itemMatch)
									
	def AddThumbnail(results, itemMatch as Match):
		
		title = itemMatch.Groups["title"].Value
		productPageUrl = "http://www.buy.com" + itemMatch.Groups["productPageUrl"].Value
		productNumber = itemMatch.Groups["sku"].Value
		thumbnailUrl = itemMatch.Groups["thumbnailUrl"].Value
		
		if not thumbnailUrl.EndsWith(".gif"):
					
			imageUrl = String.Format("http://ak.buy.com/PI/0/125/{0}.jpg", productNumber)			
			results.Add(imageUrl, title, productPageUrl, -1, -1, productNumber, CoverType.Front)
			
	def RetrieveFullSizeImage(productNumber): 
	
		imageUrl = String.Format("http://ak.buy.com/PI/0/500/{0}.jpg", productNumber)
		
		request = System.Net.HttpWebRequest.Create(imageUrl) as System.Net.HttpWebRequest
		imageStream = request.GetResponse().GetResponseStream()
		imageStream = ConvertToMemoryStream(imageStream)
						
		bitmap = Bitmap(imageStream)
		frameSize = GetFrameSize(bitmap)	
	
		if frameSize > 125:
			croppedBitmap = CropBitmap(bitmap, 125)
			imageStream.Dispose()
			imageStream = ConvertImageToStream(croppedBitmap)
			croppedBitmap.Dispose()
		elif frameSize > 0:
			croppedBitmap = CropBitmap(bitmap, 250)
			imageStream.Dispose()
			imageStream = ConvertImageToStream(croppedBitmap)
			croppedBitmap.Dispose()	
			
		bitmap.Dispose()

		imageStream.Seek(0, SeekOrigin.Begin)
		return imageStream		
		
	def GetFrameSize(bitmap):
	
		imageSize = bitmap.Width
		imageMiddle as int = imageSize / 2
		frameSize = 0
		whiteArgb = Color.White.ToArgb()
		
		for i in range(0, 130):
		
			frameSize = i
			
			color = bitmap.GetPixel(i, imageMiddle)
			if color.ToArgb() != whiteArgb:
				break
				
			color = bitmap.GetPixel(imageMiddle,i)
			if color.ToArgb() != whiteArgb:
				break
				
			color = bitmap.GetPixel(imageSize - i - 1,imageMiddle)
			if color.ToArgb() != whiteArgb:
				break
				
			color = bitmap.GetPixel(imageMiddle, imageSize - i - 1)
			if color.ToArgb() != whiteArgb:
				break

		return frameSize
		
	def CropBitmap(bitmap, size):
		
		x as int = ((bitmap.Width / 2) - (size / 2))
		y as int = ((bitmap.Width / 2) - (size / 2))
		w = size
		h = size
	
		croppedBitmap = bitmap.Clone(Rectangle(x, y, w, h), bitmap.PixelFormat)
		bitmap.Dispose()
		
		return croppedBitmap
		
	def ConvertImageToStream(image):
	
		stream = System.IO.MemoryStream()
		image.Save(stream, ImageFormat.Png)
		
		stream.Seek(0, SeekOrigin.Begin)		
		return stream
		
	def ConvertToMemoryStream(sourceStream as Stream) as Stream:	
	
		buffer = array(byte, 4096)
		readBytes = 1
		memoryStream = MemoryStream()
		
		while readBytes > 0:
			readBytes = sourceStream.Read(buffer, 0, buffer.Length - 1)
			memoryStream.Write(buffer, 0, readBytes)
			
		sourceStream.Dispose()
		sourceStream = memoryStream
			
		memoryStream.Seek(0, SeekOrigin.Begin)		
		return memoryStream