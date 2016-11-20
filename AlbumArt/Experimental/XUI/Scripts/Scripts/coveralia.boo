namespace CoverSources
import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class Coveralia:
	static SourceName as string:
		get: return "Coveralia"
	static SourceCreator as string:
		get: return "Alex Vallat"
	static SourceVersion as string:
		get: return "0.11"
	static def GetThumbs(coverart,artist,album):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		query as string = artist + " " + album
		
		resultResults = GetPageIsoLatin1(String.Format("http://www.coveralia.com/mostrar.php?bus={0}&bust=2", EncodeUrlIsoLatin1(query)))
		
		//Get results
		resultRegex = Regex("<a href=\"(?<url>/discos/[^\"]+\\.php)\">", RegexOptions.Multiline)
		resultMatches = resultRegex.Matches(resultResults)
		coverart.EstimatedCount = resultMatches.Count * 3 //Estimate each result has front, back and CD images

		for resultMatch as Match in resultMatches:
			//Get the result page
			resultPage = GetPageIsoLatin1(String.Format("http://www.coveralia.com{0}", resultMatch.Groups["url"].Value))
			
			labelRegex = Regex("<title>(?<title>[^<]+?)\\s(?:-\\s*)?caratulas", RegexOptions.IgnoreCase)
			labelMatch = labelRegex.Match(resultPage) //Expecting one match
			
			imagePageRegex = Regex("<a href=\"/caratulas/(?<imageName>[^\"]+)\"[^>]*><img src=\"http://images.coveralia.com/audio/thumbs/(?<thumbID>[^\"]+)\"")
			imagePageMatches = imagePageRegex.Matches(resultPage)
			
			coverart.EstimatedCount += imagePageMatches.Count - 3 //Adjust estimated count based on number of matches found here
			
			for imagePageMatch as Match in imagePageMatches:
				//Find Full Size image
				fullSizeImageUrlEnd = imagePageMatch.Groups["imageName"].Value
				urlParts = fullSizeImageUrlEnd.Split(".-".ToCharArray(),42);
				if(urlParts.Length>=3):
					coverTypeString = urlParts[urlParts.Length-2]
					if(coverTypeString=="Frontal" or coverTypeString=="Trasera"):
						#Specialcase: Could be "Interior-Frontal" or "Interior-Trasera"
						if(urlParts[urlParts.Length-3]=="Interior"):
							coverTypeString="Interior ${coverTypeString}"
				
				fullSizeImagePageUrl =String.Format("http://www.coveralia.com/caratulas/{0}", imagePageMatch.Groups["imageName"].Value)
				fullSizeImagePage = GetPage(fullSizeImagePageUrl)
				
				//Width and Height in the html are not the actual width and height of the image, they are always around 500, so ignore them.
				fullSizeImageRegex = Regex("src=\"(?<url>http://images\\.coveralia\\.com/audio/[^\"]+)\"[^>]*class=\"caratula\"")
				fullSizeImageMatch = fullSizeImageRegex.Match(fullSizeImagePage) //Expecting only one match
				
				if fullSizeImageMatch.Success:
					coverart.Add(
						String.Format("http://images.coveralia.com/audio/thumbs/{0}", imagePageMatch.Groups["thumbID"].Value),
						labelMatch.Groups["title"].Value + " - " + coverTypeString,
						fullSizeImagePageUrl,
						-1,
						-1,
						fullSizeImageMatch.Groups["url"].Value,
						string2coverType(coverTypeString))
			
	static def GetResult(param):
		return param
		
	static def string2coverType(typeString as string):
		if(string.Compare(typeString,"Trasera",true)==0):
			return CoverType.Back;
		if(typeString.ToLower().StartsWith("cd")):
			return CoverType.CD;
		if(typeString.ToLower().StartsWith("interior")):
			return CoverType.Inlay;
		if(string.Compare(typeString,"Frontal",true)==0):
			return CoverType.Front;
		else:
			return CoverType.Unknown;
