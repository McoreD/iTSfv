namespace CoverSources
import System
import System.Text
import System.Text.RegularExpressions
import System.Collections
import util

class Freecovers:
	static SourceName as string:
		get: return "Freecovers"
	static SourceCreator as string:
		get: return "Alex Vallat"
	static SourceVersion as string:
		get: return "0.1"
	static def GetThumbs(coverart,artist,album):
		query as string = artist + " " + album
		query.Replace(' ','+')
		resultResults = GetPage(String.Format("http://www.freecovers.net/search.php?search={0}&cat=4", EncodeUrl(query)))
		
		//Get results
		resultRegex = Regex("<a class=\"versionName\"[^>]+>(?<name>[^<]+)</a>[^\\[]+(?:\\[<a href=http://www.freecovers.net/view/(?<urlBase>[^ >]+?)/(?<urlTitle>[^ >/]+?)-(?<urlPart>[^\\.]+)\\.html[^<]+</a>\\] )+", RegexOptions.Multiline)
		resultMatches = resultRegex.Matches(resultResults)
		
		//Filter results that match both artist and album
		fullMatches = ArrayList(resultMatches.Count)
		for resultMatch as Match in resultMatches:
		  name = resultMatch.Groups["name"].Value
		  if (String.IsNullOrEmpty(artist) or name.IndexOf(artist, StringComparison.OrdinalIgnoreCase) >= 0) and (String.IsNullOrEmpty(album) or name.IndexOf(album, StringComparison.OrdinalIgnoreCase) >= 0):
		    fullMatches.Add(resultMatch)
		
		coverart.SetCountEstimate(fullMatches.Count)
		
		//Process the filtered results
		for resultMatch as Match in fullMatches:
			name = resultMatch.Groups["name"].Value
			
			for i in range(resultMatch.Groups["urlBase"].Captures.Count):
				urlBase = resultMatch.Groups["urlBase"].Captures[i].Value
				urlTitle = resultMatch.Groups["urlTitle"].Captures[i].Value
				urlPart = resultMatch.Groups["urlPart"].Captures[i].Value
				if (urlBase != "" and urlTitle != "" and urlPart != ""):
					//coverart.AddThumb(String.Format("http://www.freecovers.net/download/{0}/{1}-%5B{2}%5D-%5Bwww.FreeCovers.net%5D.jpg", urlBase, urlTitle, urlPart), String.Format("{0} - {1}", name, urlPart), -1, -1, null)
					coverart.AddThumb(String.Format("http://www.freecovers.net/thumb/{0}/preview.jpg", urlBase), String.Format("{0} - {1}", name, urlPart), -1, -1, String.Format("http://www.freecovers.net/download/{0}/{1}-%5B{2}%5D-%5Bwww.FreeCovers.net%5D.jpg", urlBase, urlTitle, urlPart))
		
	static def GetResult(param):
		return param
