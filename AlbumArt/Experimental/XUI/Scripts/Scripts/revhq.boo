namespace CoverSources
import System
import System.Text.RegularExpressions
import System.Collections
import AlbumArtDownloader.Scripts
import util

class RevHQ:
	static SourceName as string:
		get: return "RevHQ"
	static SourceCreator as string:
		get: return "Alex Vallat"
	static SourceVersion as string:
		get: return "0.4"
	static def GetThumbs(coverart,artist,album):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		if(String.IsNullOrEmpty(album)):
			return //Can't search without the album title to search for
		
		resultsPage = GetPage(String.Format("http://revhq.com/store.revhq?Page=search&Keywords={0}&Category=Title", EncodeUrl(album)))
		
		//Get results
		resultRegex = Regex("<B>(?<artist>[^<]+)</B>\\: <A HREF=\"/store\\.revhq\\?Page=search&Id=(?<id>[^\"]+)\">", RegexOptions.Singleline)
		resultMatches = resultRegex.Matches(resultsPage)
		
		if (resultMatches.Count == 0):
		  //Only one match was found by RevHQ, and it's showing it right now
		  
		  artistRegex = Regex("<A HREF=\"/store\\.revhq\\?Page=search&BandId=[^\"]+\">(?<artist>[^<]+)</A>", RegexOptions.Singleline)
		  artistMatches = artistRegex.Matches(resultsPage)
		  if(artistMatches.Count > 0):
		    coverart.EstimatedCount = 1
		    foundArtist = artistMatches[0].Groups["artist"].Value //Expecting only one match
  		  
		    if (String.IsNullOrEmpty(artist) or foundArtist.IndexOf(artist, StringComparison.OrdinalIgnoreCase) >= 0):
			    //Artist matches too (or no artist specified)
			    ProcessResultPage(resultsPage, foundArtist, coverart)
			  
		else:
		  //Multiple matches found
		  
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
		    resultPage = GetPage(String.Format("http://revhq.com/store.revhq?Page=search&Id={0}", resultMatch.Groups["id"].Value))
		    ProcessResultPage(resultPage, resultMatch.Groups["artist"].Value, coverart)
			
	static def ProcessResultPage(resultPage, artist, coverart):
	  imageRegex = Regex("SRC=\"/images/covers/(?<full>(?<size>\\d+)/(?<id>[^.]+)[^\"]+)\"", RegexOptions.Singleline)
	  imageMatch = imageRegex.Matches(resultPage)[0] //Expecting only one match
	  id = imageMatch.Groups["id"].Value
	  size = int.Parse(imageMatch.Groups["size"].Value)
	  full = imageMatch.Groups["full"].Value
	  
	  titleRegex = Regex("<TD CLASS=base><B>(?<title>[^<]+)</B>", RegexOptions.Singleline)
	  title = titleRegex.Matches(resultPage)[0].Groups["title"].Value //Expecting only one match
	  coverType = CoverType.Front #Assume that the image is always the front cover
	  
	  coverart.Add(String.Format("http://revhq.com/images/covers/50/{0}.gif", id), artist + " - " + title, size, size, String.Format("http://revhq.com/images/covers/{0}", full, coverType))

	static def GetResult(param):
		return param
