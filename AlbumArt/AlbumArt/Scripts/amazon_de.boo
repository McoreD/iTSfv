namespace CoverSources
import System
import System.Drawing
import System.Text
import System.Text.RegularExpressions
import util

class AmazonDe:

	static SourceName as string:
		get: return "Amazon.de"
	static SourceVersion as string:
		get: return "0.1"
	static SourceCreator as string:
		get: return "Marc Landis"

	static def GetThumbs(coverart,artist,album):
		query as string = artist + " " + album
		query.Replace(' ','+')
		
		searchResults = GetPage(String.Format("http://www.amazon.de/s/ref=nb_ss_w/302-1749690-8236014?__mk_de_DE=%C5M%C5Z%D5%D1&url=search-alias%3Dpopular&field-keywords={0}", query))

		//Get obids
		resultsRegex = Regex("<a href=\".*?/dp/(?<ID>.*?)/.*?\"><span class=\"srTitle\">.*?</span></a>", RegexOptions.Multiline)
		resultMatches = resultsRegex.Matches(searchResults)
		
		coverart.SetCountEstimate(resultMatches.Count)

		for resultMatch as Match in resultMatches:
			//Get the album page
			albumPage = GetPage(String.Format("http://www.amazon.de/gp/product/images/{0}",resultMatch.Groups["ID"].Value))

			//Get the title for that album
			titleRegex = Regex("<strong>(?<title>.*?)</strong>", RegexOptions.Multiline)
			imageMatches = titleRegex.Matches(albumPage)
			
			for titleMatch as Match in imageMatches:
				title = titleMatch.Groups["title"].Value

			//Get all the images for the album
			imagesRegex = Regex("'<div id=\"imageViewerDiv\"><img src=\"(?<fullSizeID>.*?)\".*?</div>", RegexOptions.Singleline)
			imageMatches = imagesRegex.Matches(albumPage)
			
			for imageMatch as Match in imageMatches:
				coverart.AddThumb(String.Format("{0}", imageMatch.Groups["fullSizeID"].Value), title, -1, -1, null)		

	static def GetResult(param):
		return null

