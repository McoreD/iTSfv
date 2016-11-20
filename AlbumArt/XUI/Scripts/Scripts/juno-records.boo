namespace CoverSources
import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class JunoRecords:

	static SourceName as string:
		get: return "Juno Records"
	static SourceVersion as string:
		get: return "0.8"
	static SourceCreator as string:
		get: return "Marc Landis, Alex Vallat"
	static SourceCategory as string:
		get: return "Dance, Club, Electronic"

	static def GetThumbs(coverart,artist,album):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		query as string = artist + " " + album
		query = EncodeUrl(query)
		
		searchResults = GetPage("http://www.juno.co.uk/search/?q[all][0]=${query}&submit-search=SEARCH&show_out_of_stock=1")
		
		//Get pages
		resultsRegex = Regex("<div class=\"pl-img\"><a href=\"(?<url>[^\"]+)\"", RegexOptions.Singleline)
		resultMatches = resultsRegex.Matches(searchResults)
		coverart.SetCountEstimate(resultMatches.Count * 2) //Estimate 2 covers per result. Results may vary.

		for resultMatch as Match in resultMatches:
			//Get the album page
			albumPageUrl = "http://www.juno.co.uk" + resultMatch.Groups["url"].Value
			albumPage = GetPage(albumPageUrl)

			//Get all the images for the album
			imagesRegex = Regex("<img alt=\"(?<imageName>.*?)\" src=\"http://images\\.junostatic\\.com/300/(?<ID>.*?)-MED\\.jpg\"", RegexOptions.Singleline)
			imageMatches = imagesRegex.Matches(albumPage)
			
			for imageMatch as Match in imageMatches:
				id = imageMatch.Groups["ID"].Value
				coverart.Add(
					"http://images.junostatic.com/75/${id}-TN.jpg", #thumbnail
					imageMatch.Groups["imageName"].Value, #name
					albumPageUrl, #infoUri
					-1, #fullSizeImageWidth
					-1, #fullSizeImageHeight
					"http://images.junostatic.com/full/${id}-BIG.jpg", #fullSizeImageCallback
					CoverType.Unknown
					)
				
	static def GetResult(param):
		return param

