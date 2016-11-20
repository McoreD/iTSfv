import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class MusicMight(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "MusicMight"
	Version as string:
		get: return "0.3"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		if String.IsNullOrEmpty(album):
			return //Only searching on album is supported

		album = StripCharacters("&.'\";:?!", album)
			
		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.musicmight.com/search?t=recording&q=" + EncodeUrl(album))
		
		matches = Regex("<a\\s[^>]*?href\\s*=\\s*'(?<url>[^']+)'[^>]*?>\\s*<img\\s[^>]+?src\\s*=\\s*'http://s3\\.amazonaws\\.com//mmimagesm/(?<img>[^']+)'[^>]*?>(?<title>.*?)</td>", RegexOptions.IgnoreCase | RegexOptions.Singleline).Matches(searchResultsHtml)
		
		if matches.Count == 0: //Try single page result
			matches = Regex("\"http://s3\\.amazonaws\\.com//mmimagelg/(?<img>[^\"]+)\"[^>]*?>.*?<h4><span id=\"artistTitle\">(?:(?:<[^>4]+>)(?<title>[^<]+)?)+</h4>", RegexOptions.IgnoreCase | RegexOptions.Singleline).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			img = match.Groups["img"].Value
			title = ""
			for capture as Capture in match.Groups["title"].Captures:
				title += capture.Value
			title = title.Replace("<br />", "").Replace("</a>", "").Replace("&bull;", "-");

			url as string = null
			if match.Groups["url"].Success:
				url = "http://www.musicmight.com" + match.Groups["url"].Value
			
			results.Add("http://s3.amazonaws.com//mmimagesm/" + img, title, url, -1, -1, "http://s3.amazonaws.com//mmimagelg/" + img, CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;