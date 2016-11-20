import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

/**
 * Searches for covers on www.allcdcovers.com using the Regex 
 */
class AllCdCovers(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "AllCdCover"
	Version as string:
		get: return "0.3"
	Author as string:
		get: return "daju"
	
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		searchFor = EncodeUrl("${artist} ${album}".Trim())
		baseUrl = "http://www.allcdcovers.com/search/music/all/"
		allResultsPage = GetPage(GetPageStream("${baseUrl}${searchFor}", null, true))
		
		resultRegex = Regex("<a href=\"/show/(?<singleResultPage>[^\"]+)\"><img[^>]+><br />(?<typeName>[^<]+)</a>")
		resultMatches = resultRegex.Matches(allResultsPage)
		results.EstimatedCount = resultMatches.Count
		for resultMatch as Match in resultMatches:
			singleResultPageUrl = resultMatch.Groups["singleResultPage"].Value
			typeName = resultMatch.Groups["typeName"].Value
			
			singleResultPageUrl = "http://www.allcdcovers.com/show/${singleResultPageUrl}"
			singleResultPage = GetPage(GetPageStream(singleResultPageUrl, null, true))
			infoRegex =Regex("<dl class=\"tableLike\">\\s*<dt>Title:</dt>\\s*<dd>(?<name>[^<]+)</dd>\\s*<dt>Part:</dt>\\s*<dd>(?<typeNameTwo>[^<]+)</dd>\\s*<dt>Dimensions:</dt>\\s*<dd>(?<sizeX>[0-9]+) x (?<sizeY>[0-9]+) px</dd>\\s*<dt>Size:</dt>\\s*<dd>(?<sizeKB>[^<]+)</dd>")
			infoMatch = infoRegex.Match(singleResultPage)
			title = infoMatch.Groups["name"].Value
			sizeX = int.Parse(infoMatch.Groups["sizeX"].Value)
			sizeY = int.Parse(infoMatch.Groups["sizeY"].Value)
			sizeKB = infoMatch.Groups["sizeKB"].Value

			thumRegex = Regex("<div class=\"selectedCoverThumb\">\\s*<img alt=\"(?<typeNameThree>[^\"]+)\" class=\"coverThumb\" src=\"(?<thumbUrl>[^\"]+)\"\\s*[/]?>\\s*<br\\s*[/]?>");//\\s*<a href=\"(?<fullUrl>[^\"]+)\">
			thumMatch = thumRegex.Match(singleResultPage)
			thumbUrlPart = thumMatch.Groups["thumbUrl"].Value
			thumbUrl = "http://www.allcdcovers.com${thumbUrlPart}"
			
			fullRegex = Regex("<a\\s*href=\"/download/(?<fullUrl>[^\"]+)\"\\s*>")
			fullUrlMatch = fullRegex.Match(singleResultPage)
			fullUrlPart = fullUrlMatch.Groups["fullUrl"].Value
			fullUrl = "http://www.allcdcovers.com/download/${fullUrlPart}"
			
			coverName = "${title} - ${typeName} - ${sizeKB}"
			coverType as CoverType = string2coverType(typeName)
			useSmallVersion = false
			
			if(singleResultPage.Contains("captcha_image.php")): #This happens after a download of 4 covers.
				coverName = "${title} - ${typeName}"
				useSmallVersion = true
				fullUrl = thumbUrl
				sizeX = 65
				sizeY = 65
				if(coverType == CoverType.Front):
					midiRegex = Regex("<div class=\"productImage\">\\s*<img alt=\"(?<name>[^\"]+)\" width=\"(?<sizeX>\\d+)\" height=\"(?<sizeY>\\d+)\" src=\"/image_system/images/(?<midiUrl>[^\"]+)\"\\s*[/]?>")
					midiMatch = midiRegex.Match(singleResultPage)
					midiUrlPart = midiMatch.Groups["midiUrl"].Value
					if (not String.IsNullOrEmpty(midiUrlPart)):
						midiUrl = "http://www.allcdcovers.com/image_system/images/${midiUrlPart}"
						fullUrl = midiUrl
						sizeX =  int.Parse(midiMatch.Groups["sizeX"].Value)
						sizeY = int.Parse(midiMatch.Groups["sizeY"].Value)
					
				
			if((not String.IsNullOrEmpty(thumbUrlPart))and (not String.IsNullOrEmpty(fullUrlPart))or useSmallVersion):
				results.Add(
					GetPageStream(thumbUrl, null, true),
					coverName,
					singleResultPageUrl,
					sizeX,
					sizeY,
					fullUrl,
					coverType
					)
				
				
		
	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return GetPageStream(fullSizeCallbackParameter, null, true);
		
	def string2coverType(typeString as string):
		if(typeString.ToLower().StartsWith("front")):
			return CoverType.Front;
		elif(typeString.ToLower().StartsWith("back")):
			return CoverType.Back;
		elif(typeString.ToLower().StartsWith("inlay")):
			return CoverType.Inlay;
		elif(typeString.ToLower().StartsWith("cd")):
			return CoverType.CD;
		elif(typeString.ToLower().StartsWith("inside")):
			return CoverType.Inlay;
		else:
			return CoverType.Unknown;
