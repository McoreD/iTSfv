namespace CoverSources
import System
import System.Text.RegularExpressions
import System.Collections
import AlbumArtDownloader.Scripts
import util

class RateYourMusic:
	static SourceName as string:
		get: return "Rate Your Music"
	static SourceCreator as string:
		get: return "Alex Vallat"
	static SourceVersion as string:
		get: return "0.3"
	static def GetThumbs(coverart,artist,album):
		if(String.IsNullOrEmpty(album)):
			return //Can't search without the album title to search for
		
		resultResults = GetPage(String.Format("http://rateyourmusic.com/go/search?searchterm={0}&searchtype=l", EncodeUrl(album)))
		
		//Get results
		resultRegex = Regex("<tr id=\"infobox(?<id>\\d+)\".+?<b>(?<artist>[^<]+)</b>.+?<em>(?<album>[^<]+)</em>", RegexOptions.Singleline)
		resultMatches = resultRegex.Matches(resultResults)
		
		//Filter results that match both artist name too
		fullMatches = ArrayList(resultMatches.Count)
		for resultMatch as Match in resultMatches:
			foundArtist = resultMatch.Groups["artist"].Value
			//Add this result if the artist name as found contains the artist name being searched for
			if (String.IsNullOrEmpty(artist) or foundArtist.IndexOf(artist, StringComparison.OrdinalIgnoreCase) >= 0):
				fullMatches.Add(resultMatch)
		
		coverart.EstimatedCount = fullMatches.Count
	
		//Process the filtered results
		for resultMatch as Match in fullMatches:
			name = String.Format("{0} - {1}", resultMatch.Groups["artist"].Value, resultMatch.Groups["album"].Value)
			id = resultMatch.Groups["id"].Value
			coverart.Add(
				GetStreamWithUserAgent(String.Format("http://static.rateyourmusic.com/album_images/s{0}.jpg", id)), 
				name,
				String.Format("http://static.rateyourmusic.com/album_images/o{0}.jpg", id),
				CoverType.Front
				)
			
	static def GetResult(param):
		return GetStreamWithUserAgent(param)
		
	static def GetStreamWithUserAgent(url as string):
		request = System.Net.HttpWebRequest.Create(url) as System.Net.HttpWebRequest
		request.UserAgent = "Mozilla/5.0"
		response = request.GetResponse()
		return response.GetResponseStream()
