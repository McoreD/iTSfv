import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class HMVCanada(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "HMV Canada"
	Version as string:
		get: return "0.2"
	Author as string:
		get: return "Sebastien Leclerc"

	def Search(artist as string, album as string, results as IScriptResults):
		if string.IsNullOrEmpty(album):
			return //Only searching on album is supported

		album = StripCharacters("&.'\";:?!", album)

		resultsPage = GetPage("http://www.hmv.ca/Search.aspx?keyword=${EncodeUrl(album)}&filter=music")
		
		resultsRegex = Regex("div\\s*[^>]*class='chartItemImage'[^>]*>.*?\\ssrc\\s*=\\s*'(?<path>(/(?!sm)\\w+)+/)small/(?<filename>[^']*)(?:[^>]*>){1,5}[^\\s]*\\s*href\\s*='[^']*?(?<url>/Products/Detail/[^']*)[^>]+>(?<title>[^<]*)[^?]+[^>]+>(?<artist>[^<]*)", RegexOptions.Singleline | RegexOptions.IgnoreCase)
		resultsMatches = resultsRegex.Matches(resultsPage)
	
		results.EstimatedCount = resultsMatches.Count;

		for resultsMatch as Match in resultsMatches:
  			loc= "http://www.hmv.ca"+resultsMatch.Groups["path"].Value;
  			filename = resultsMatch.Groups["filename"].Value;
  			title = resultsMatch.Groups["title"].Value;
  			url = "http://www.hmv.ca"+resultsMatch.Groups["url"].Value;
  			artistname = resultsMatch.Groups["artist"].Value;
  			
  			results.Add("${loc}large/${filename}", "${artistname} - ${title}", url, -1, -1, "${loc}large/${filename}" , CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter
