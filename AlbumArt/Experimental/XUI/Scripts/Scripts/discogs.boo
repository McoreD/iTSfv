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
		get: return "0.9"
	static def GetThumbs(coverart,artist,album):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		query as string = artist + " " + album
		
		obidResults = GetPage(String.Format("http://www.discogs.com/search?type=all&q={0}", EncodeUrl(query)))
			
		//Get obids
		obidRegex = Regex("<img src=\"[^\"]+?/image/R-50-(?<obid>\\d+)-[^\"]+\".+?<a href=\"(?<url>[^\"]+)\"><em>(?:</?em>|(?>(?<name>[^<]+)))+</a>", RegexOptions.Singleline | RegexOptions.IgnoreCase)
		obidMatches = obidRegex.Matches(obidResults)
		coverart.EstimatedCount = obidMatches.Count //Probably more than this, as some releases might have multiple images

		for obidMatch as Match in obidMatches:
			//Construct the release name by joining up all the captures of the "name" group
			releaseNameBuilder = StringBuilder()
			for namePart in obidMatch.Groups["name"].Captures:
				releaseNameBuilder.Append(namePart)
			
			releaseName = releaseNameBuilder.ToString()
			releaseUrl = "http://www.discogs.com" + obidMatch.Groups["url"].Value
			
			//Get the image results
			imageResults = GetPage(String.Format("http://www.discogs.com/viewimages?release={0}", obidMatch.Groups["obid"].Value))
			
			imageRegex = Regex("<img src=\"(?<url1>[^\"]+?/image/R-)(?<url2>\\d+-\\d+.(?:jpe?g|gif|png))\" width=\"(?<width>\\d+)\" height=\"(?<height>\\d+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase)
			imageMatches = imageRegex.Matches(imageResults)
			
			coverart.EstimatedCount += imageMatches.Count - 1 //Adjust count by how many images for this release
			
			for imageMatch as Match in imageMatches:
				coverart.Add(imageMatch.Groups["url1"].Value + "150-" + imageMatch.Groups["url2"].Value, releaseName, releaseUrl, Int32.Parse(imageMatch.Groups["width"].Value), Int32.Parse(imageMatch.Groups["height"].Value), imageMatch.Groups["url1"].Value + imageMatch.Groups["url2"].Value)
		
	static def GetResult(param):
		return param
