namespace CoverSources
import System
import System.Text
import System.Text.RegularExpressions
import util

class Coveralia:
	static SourceName as string:
		get: return "Coveralia"
	static SourceCreator as string:
		get: return "Alex Vallat"
	static SourceVersion as string:
		get: return "0.4"
	static def GetThumbs(coverart,artist,album):
		query as string = artist + " " + album
		query.Replace(' ','+')
		resultResults = GetPage(String.Format("http://www.coveralia.com/mostrar.php?bus={0}&bust=2", EncodeUrl(query)))
		
		//Get results
		resultRegex = Regex("<a href=\"(?<url>[^\"]+)\" class=\"texto9\">&nbsp;(?<name>[^<]+)<", RegexOptions.Multiline)
		resultMatches = resultRegex.Matches(resultResults)
		coverart.SetCountEstimate(resultMatches.Count * 3) //Estimate each result has front, back and CD images

		for resultMatch as Match in resultMatches:
			//Get the result page
			resultPage = GetPage(String.Format("http://www.coveralia.com/{0}", resultMatch.Groups["url"].Value))
			
			imagePageRegex = Regex("<a href=\"/caratula\\.php/(?<imageName>[^\"]+)\"><img src=\"/audio/thumbs/(?<thumbID>[^\"]+)\"")
			imagePageMatches = imagePageRegex.Matches(resultPage)
			for imagePageMatch as Match in imagePageMatches:
				//Find Full Size image
				fullSizeImagePage = GetPage(String.Format("http://www.coveralia.com/caratula.php/{0}", imagePageMatch.Groups["imageName"].Value))
				
				fullSizeImageRegex = Regex("<img width=\"(?<width>\\d+)\" height=\"(?<height>\\d+)\" alt=\"[^\"]+\" src=\"(?<url>http://www\\.coveralia\\.com/audio/[^\"]+)\"")
				fullSizeImageMatch = fullSizeImageRegex.Match(fullSizeImagePage) //Expecting only one match
				
				if fullSizeImageMatch.Success:
					coverart.AddThumb(String.Format("http://www.coveralia.com/audio/thumbs/{0}", imagePageMatch.Groups["thumbID"].Value), resultMatch.Groups["name"].Value, Int32.Parse(fullSizeImageMatch.Groups["width"].Value), Int32.Parse(fullSizeImageMatch.Groups["height"].Value), fullSizeImageMatch.Groups["url"].Value)
		
	static def GetResult(param):
		return param
