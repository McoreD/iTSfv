namespace CoverSources
import System
import System.Text
import System.Text.RegularExpressions
import util

class Discogs:
	static SourceName as string:
		get: return "Discogs"
	static SourceCreator as string:
		get: return "Alex Vallat"
	static SourceVersion as string:
		get: return "0.3"
	static def GetThumbs(coverart,artist,album):
		query as string = artist + " " + album
		query.Replace(' ','+')
		obidResults = GetPage(String.Format("http://www.discogs.com/search?type=all&q={0}", EncodeUrl(query)))
			
		//Get obids
		obidRegex = Regex("<a href=\"/release/(?<obid>\\d+)\"><span[^>]+>(?:</?em>|(?<name>[^<]+))+</span>", RegexOptions.Multiline)
		obidMatches = obidRegex.Matches(obidResults)
		coverart.SetCountEstimate(obidMatches.Count) //Probably more than this, as some releases might have multiple images

		for obidMatch as Match in obidMatches:
			//Construct the release name by joining up all the captures of the "name" group
			releaseNameBuilder = StringBuilder()
			for namePart in obidMatch.Groups["name"].Captures:
				releaseNameBuilder.Append(namePart)
			
			releaseName = releaseNameBuilder.ToString()
			
			//Get the image results
			imageResults = GetPage(String.Format("http://www.discogs.com/viewimages?what=R&obid={0}&showpending=1", obidMatch.Groups["obid"].Value))
			
			imageRegex = Regex("<img src=\"(?<url>http://www\\.discogs\\.com/image/R-\\d+-\\d+.(?:jpe?g|gif|png))\" width=\"(?<width>\\d+)\" height=\"(?<height>\\d+)\"")
			imageMatches = imageRegex.Matches(imageResults)
			for imageMatch as Match in imageMatches:
				coverart.AddThumb(imageMatch.Groups["url"].Value, releaseName, Int32.Parse(imageMatch.Groups["width"].Value), Int32.Parse(imageMatch.Groups["height"].Value), imageMatch.Groups["url"].Value)
		
	static def GetResult(param):
		return param
