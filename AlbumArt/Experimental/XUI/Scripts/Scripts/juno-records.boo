namespace CoverSources
import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class JunoRecords:

	static SourceName as string:
		get: return "Juno Records"
	static SourceVersion as string:
		get: return "0.7"
	static SourceCreator as string:
		get: return "Marc Landis"

	static def GetThumbs(coverart,artist,album):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		query as string = artist + " " + album
		query = EncodeUrl(query)
		
		searchResults = GetPage("http://www.juno.co.uk/search/?as=1&q=${query}&s_search_precision=any&s_search_type=all&s_search_music=1&s_show_out_of_stock=1&s_music_product_type=all&s_media_type=all-media&s_genre_id=0000&s_released=&s_start_date=&s_end_date=")
		
		//Get obids
		resultsRegex = Regex("<a class=\"productimage\" href=\"/products/(?<ID>[^?]+/)\\?", RegexOptions.Singleline)
		resultMatches = resultsRegex.Matches(searchResults)
		coverart.SetCountEstimate(resultMatches.Count * 2) //Estimate 2 covers per result. Results may vary.

		for resultMatch as Match in resultMatches:
			//Get the album page
			albumPageUrl = String.Format("http://www.juno.co.uk/products/{0}", resultMatch.Groups["ID"].Value)
			albumPage = GetPage(albumPageUrl)

			//Get the title for that album
			titleRegex = Regex("<h1>(?<title>.*?)</h1>", RegexOptions.Singleline)
			title = titleRegex.Matches(albumPage)[0].Groups["title"].Value //Expecting only one match

			//Get all the images for the album
			imagesRegex = Regex("150/CS(?<fullSizeID>.*?)\\.jpg.*?alt=\"(?<imageName>.*?)\"", RegexOptions.Singleline)
			imageMatches = imagesRegex.Matches(albumPage)
			
			for imageMatch as Match in imageMatches:
				if(imageMatches.Count > 1):
					altText = imageMatch.Groups["imageName"].Value
					coverType = string2coverType(altText);
					imageTitle = "${title} - ${altText}"
				else:
					imageTitle = title
				fullSizeID = imageMatch.Groups["fullSizeID"].Value
				coverart.Add(
					"http://cdn.images.juno.co.uk/150/CS${fullSizeID}.jpg", #thumbnail
					imageTitle, #name
					albumPageUrl, #infoUri
					-1, #fullSizeImageWidth
					-1, #fullSizeImageHeight
					"http://cdn.images.juno.co.uk/full/CS${fullSizeID}-BIG.jpg", #fullSizeImageCallback
					coverType #coverType
					)
				
	static def GetResult(param):
		return param
		
	static def string2coverType(typeString as string):
		if(typeString.ToLower().Contains("front")):
			return CoverType.Front;
		elif(typeString.ToLower().Contains("back")):
			return CoverType.Back;
		elif(typeString.ToLower().Contains("inlay")):
			return CoverType.Inlay;
		elif(typeString.ToLower().Contains("cd")):
			return CoverType.CD;
		elif(typeString.ToLower().Contains("inside")):
			return CoverType.Inlay;
		else:
			return CoverType.Unknown;

