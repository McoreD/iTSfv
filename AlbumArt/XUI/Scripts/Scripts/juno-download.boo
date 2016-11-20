namespace CoverSources
import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class JunoDownload:

	static SourceName as string:
		get: return "Juno Download"
	static SourceVersion as string:
		get: return "0.1"
	static SourceCreator as string:
		get: return "Alex Vallat"
	static SourceCategory as string:
		get: return "Dance, Club, Electronic"

	static def GetThumbs(coverart,artist,album):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		query as string = artist + " " + album
		query = EncodeUrl(query)
		
		searchResults = GetPage("http://www.junodownload.co.uk/search/?q[all][]=${query}")
		
		//Get pages
		resultsRegex = Regex("<div class=\"productlist_widget_product_image productimage\">\\s*<a href=\"(?<url>[^\"]+)\">\\s*<img src=\"(?<imgThumb>[^\"]+?/(?<imgId>[^\"./]+)\\.jpg)\" alt=\"(?<album>[^\"]+)\".+?<span class=\"jq_highlight pwrtext\">(?:<[^>]+>(?<artist>[^<]+))+", RegexOptions.Singleline)
		resultMatches = resultsRegex.Matches(searchResults)
		coverart.SetCountEstimate(resultMatches.Count)

		for match as Match in resultMatches:
			imgId = match.Groups["imgId"].Value
			title = ""
			for artist in match.Groups["artist"].Captures:
				title += artist.Value + " "
			title += "- " + match.Groups["album"].Value
			
			coverart.Add(
				match.Groups["imgThumb"].Value, #thumbnail
				title,
				"http://www.junodownload.co.uk" + match.Groups["url"].Value,
				-1, #fullSizeImageWidth
				-1, #fullSizeImageHeight
				"http://images.junostatic.com/full/${imgId}-BIG.jpg", #fullSizeImageCallback
				CoverType.Front
				)
				
	static def GetResult(param):
		return param

