namespace CoverSources
import System
import System.Drawing
import System.Text
import System.Text.RegularExpressions
import util

class JunoRecords:

	static SourceName as string:
		get: return "Juno Records"
	static SourceVersion as string:
		get: return "0.1"
	static SourceCreator as string:
		get: return "Marc Landis"

	static def GetThumbs(coverart,artist,album):
		query as string = artist + " " + album
		query.Replace(' ','%20')
		
		searchResults = GetPage(String.Format("http://classic.juno.co.uk/search/?q={0}&precision=any&column=all&genre_id=0000&released=&sdate=&edate=", query))

		//Get obids
		resultsRegex = Regex("<a href=\"/products/(?<ID>.*?.htm)", RegexOptions.Singleline)
		resultMatches = resultsRegex.Matches(searchResults)
		coverart.SetCountEstimate(resultMatches.Count * 2) //Estimate 2 covers per result. Results may vary.

		for resultMatch as Match in resultMatches:
			//Get the album page
			albumPage = GetPage(String.Format("http://classic.juno.co.uk/products/{0}", resultMatch.Groups["ID"].Value))

			//Get the title for that album
			titleRegex = Regex("<h3>(?<title>.*?)</h3>", RegexOptions.Singleline)
			title = titleRegex.Matches(albumPage)[0].Groups["title"].Value //Expecting only one match

			//Get all the images for the album
			imagesRegex = Regex("150/CS(?<fullSizeID>.*?)\\.jpg.*?alt=\"(?<imageName>.*?)\"", RegexOptions.Singleline)
			imageMatches = imagesRegex.Matches(albumPage)
			
			for imageMatch as Match in imageMatches:
				if(imageMatches.Count > 1):
					imageTitle = String.Format("{0} - {1}", title, imageMatch.Groups["imageName"].Value)
				else:
					imageTitle = title

				coverart.AddThumb(String.Format("http://images.juno.co.uk/full/CS{0}-BIG.jpg", imageMatch.Groups["fullSizeID"].Value), imageTitle, -1, -1, null)		

	static def GetResult(param):
		return null

