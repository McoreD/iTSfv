import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class FourtyFiveCat(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "45cat"
	Version as string:
		get: return "0.7"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "LP Vinyl"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("\"", artist)
		album = StripCharacters("\"", album)
		query = EncodeUrl(artist + " " + album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.45cat.com/45_search.php?sq=${query}&sm=se")
		
		matches = Regex("href=\"/record/(?<record>[^\"]+)\">", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		for match as Match in matches:
			//Retrieve the details page
			detailsUrl = "http://www.45cat.com/record/" + match.Groups["record"].Value;
			detailsHtml as string = GetPage(detailsUrl)
			
			titleMatch = Regex("<b>A</b></td><td>(?<artist>[^<]+)</td>\\s*<td>(?<title>[^<]+)</td>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(detailsHtml)
			title = titleMatch.Groups["artist"].Value + " - " + titleMatch.Groups["title"].Value

			imageMatches = Regex("<img\\s[^>]*?onmouseover=\"Tip\\(\\s*'<b>Description:</b> (?<covertype>[^<]+)<.+?src=\"http://images\\.45cat\\.com/(?<image>.+?)-s\\.jpg\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(detailsHtml)
			for imageMatch as Match in imageMatches:
				image = imageMatch.Groups["image"].Value
				results.Add("http://images.45cat.com/${image}-s.jpg", title, detailsUrl, -1, -1, "http://images.45cat.com/${image}.jpg", GetCoverType(imageMatch.Groups["covertype"].Value))

	static def GetCoverType(typeString):
		typeString = typeString.ToLower()
		if(typeString.StartsWith("front")):
			return CoverType.Front;
		if(typeString.StartsWith("back")):
			return CoverType.Back;
		if(typeString.StartsWith("a ")):
			return CoverType.CD;
		if(typeString.StartsWith("b ")):
			return CoverType.CD;
		if(typeString.EndsWith("side")):
			return CoverType.CD;
		if(typeString.EndsWith(" vinyl")):
			return CoverType.CD;
		if(typeString.StartsWith("inner")):
			 return CoverType.Inside;
		if(typeString.Contains("poster")):
			return CoverType.Booklet;

		return CoverType.Unknown;	

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter