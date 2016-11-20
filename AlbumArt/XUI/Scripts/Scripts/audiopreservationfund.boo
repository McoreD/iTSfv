import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class AudioPreservationFund(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "Audio Preservation Fund"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "LP Vinyl"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		if(String.IsNullOrEmpty(album)):
			return //Can't search without the album title to search for
		
		// Search archives 7 (University of Texas at Austin Historical Music Recordings) and 8 (Bowling Green State University - Sound Recordings Archives) only, as they are the only ones with cover images
		searchResultsHtml as string = Post("http://audiopreservationfund.org/archivessearch.php", "keyword=${EncodeUrl(album)}&keywordtype=title&archive_id=7")
		searchResultsHtml += Post("http://audiopreservationfund.org/archivessearch.php", "keyword=${EncodeUrl(album)}&keywordtype=title&archive_id=8")
		
		matches = Regex("<tr [^>]+>\\s*<td class=\"dblistcell\">(?<artist>[^<]+)</td>\\s*<td class=\"dblistcell\">(?<album>[^<]+)</td>.*?href=\"(?<detail>archivesdetail\\.php[^\"]+)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline).Matches(searchResultsHtml)
		
		for match as Match in matches:
			//Filter results that match both artist name too
			foundArtist = match.Groups["artist"].Value
			foundAlbum = match.Groups["album"].Value
			
			//Process this result if the artist name as found contains the artist name being searched for
			if (String.IsNullOrEmpty(artist) or foundArtist.IndexOf(artist, StringComparison.OrdinalIgnoreCase) >= 0):
				
				//Fetch the details page
				detailsPageUrl = "http://audiopreservationfund.org/" + match.Groups["detail"].Value
				detailsPageHtml as string = GetPage(detailsPageUrl)
				coverMatches = Regex("href=\\\"(?<urlstart>graphics/archives/./(?<covertype>[^/]+))/Big/(?<image>[^\"]+)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline).Matches(detailsPageHtml)

				for coverMatch as Match in coverMatches:
					urlStart = "http://audiopreservationfund.org/" + coverMatch.Groups["urlstart"].Value
					image  = coverMatch.Groups["image"].Value
					results.Add(urlStart + "/Small/" + image, foundArtist + " " + foundAlbum, detailsPageUrl, -1, -1, urlStart + "/Big/" + image, GetCoverType(coverMatch.Groups["covertype"].Value));

	static def GetCoverType(typeString):
		if(string.Compare(typeString,"Back%20Covers",true)==0):
			return CoverType.Back;
		if(string.Compare(typeString,"Vinyl",true)==0):
			return CoverType.CD;
		if(string.Compare(typeString,"Front%20Covers",true)==0):
			return CoverType.Front;
		if(string.Compare(typeString,"inlay",true)==0):
			return CoverType.Inlay;
		if(string.Compare(typeString,"inside",true)==0):
			return CoverType.Inside;
		if(string.Compare(typeString,"booklet",true)==0):
			return CoverType.Booklet;
		else:
			return CoverType.Unknown;	

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;
		
	static def Post(url as String, content as String):
		request = System.Net.HttpWebRequest.Create(url)
		request.Method="POST"
		request.ContentType = "application/x-www-form-urlencoded"
		bytes = System.Text.UTF8Encoding().GetBytes(content)
		request.ContentLength = bytes.Length
		stream = request.GetRequestStream()
		stream.Write(bytes,0,bytes.Length)
		stream.Close()
		streamresponse = request.GetResponse().GetResponseStream()
		return System.IO.StreamReader(streamresponse).ReadToEnd()