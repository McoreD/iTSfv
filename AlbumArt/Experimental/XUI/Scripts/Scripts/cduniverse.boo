namespace CoverSources
import System
import System.Text.RegularExpressions
import System.Collections
import util

class CdUniverse:
	static SourceName as string:
		get: return "CD Universe"
	static SourceCreator as string:
		get: return "Alex Vallat"		
	static SourceVersion as string:
		get: return "0.4"
	static def GetThumbs(coverart,artist,album):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		albumResults = GetPage(String.Format("http://www.cduniverse.com/sresult.asp?HT_Search_Info={0}&HT_Search=TITLE", EncodeUrl(album)))

		//Get results
		albumRegex = Regex("<a class=\"artitle\"[^>]+>\\s*(?<artist>[^<]+)\\s*</a>[^?]+\\?pid=(?<pid>\\d+)[&\"][^>]*>\\s*(?<title>[^<]+)\\s*</a>", RegexOptions.Singleline)
		albumMatches = albumRegex.Matches(albumResults)
		
		//Filter by artist
		pids = ArrayList(albumMatches.Count)
		for albumMatch as Match in albumMatches:
			if String.IsNullOrEmpty(artist) or albumMatch.Groups["artist"].Value.IndexOf(artist, StringComparison.OrdinalIgnoreCase) >= 0:
				pids.Add(albumMatch.Groups["pid"].Value)
		
		coverart.SetCountEstimate(pids.Count)

		for pid in pids:
			//Get the image page (if not 404)
			try:
				imagePage = GetPage(String.Format("http://www.cduniverse.com/images.asp?pid={0}&image=front", pid))
			except e as System.Net.WebException:
				continue //Skip this page
			
			//Get the title
			titleRegex = Regex("<div class=\"label\">(?<title>[^<]+)<", RegexOptions.Multiline | RegexOptions.IgnoreCase)
			title = titleRegex.Matches(imagePage)[0].Groups["title"].Value //Expecting only one match
			
			//Get the image(s?)
			imageRegex = Regex("<img src=\"(?<url>[^\"]+/Large/[^\"]+)\"", RegexOptions.Multiline | RegexOptions.IgnoreCase)
			imageMatches = imageRegex.Matches(imagePage)
			for imageMatch as Match in imageMatches:
				url = imageMatch.Groups["url"].Value
				coverart.AddThumb(url, title, -1, -1, null)
		

	static def GetResult(param):
		return param
