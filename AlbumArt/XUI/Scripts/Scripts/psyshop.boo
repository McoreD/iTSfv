namespace CoverSources
import System
import System.Text
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class Psyshop:
	static SourceName as string:
		get: return "Psyshop"
	static SourceCreator as string:
		get: return "Alex Vallat"
	static SourceVersion as string:
		get: return "0.7"
	static SourceCategory as string:
		get: return "Dance, Club, Electronic"
	static def GetThumbs(coverart,artist,album):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		query as string = artist + " " + album
		
		resultsPage = Post("http://www.psyshop.com/psyfctn/psysrch", String.Format("boolean=AND&case=INSENSITIVE&cd=TRUE&dw=TRUE&other=TRUE&terms={0}", EncodeUrl(query)))
		
		//Get results
		resultsRegex = Regex("<DIV CLASS=\"n\"><A HREF=\"(?<url>/shop/CDs/[^/]+/(?<id>[^\\.]+).html)\">(?<title>[^<]+)<", RegexOptions.Multiline | RegexOptions.IgnoreCase)
		resultMatches = resultsRegex.Matches(resultsPage)
		coverart.SetCountEstimate(resultMatches.Count)
		
		for resultMatch as Match in resultMatches:
			id = resultMatch.Groups["id"].Value
			coverType = CoverType.Front #Assume that the image is always the front cover
			albumUrl = "http://www.psyshop.com" + resultMatch.Groups["url"].Value
			coverart.Add(
					"http://87.106.17.252/pic/${id}_s.jpg", #thumbnail
					resultMatch.Groups["title"].Value, #name
					albumUrl, #infoUri
					512, #fullSizeImageWidth
					512, #fullSizeImageHeight
					"http://www.psyshop.com/a/${id}_b.jpg", #fullSizeImageCallback
					coverType #coverType
					)

	static def Post(url as String, content as String):
		request = System.Net.HttpWebRequest.Create(url) as System.Net.HttpWebRequest
		
		servicePoint = request.ServicePoint;
		prevValue = servicePoint.Expect100Continue;
		servicePoint.Expect100Continue = false;

		try:
			request.Headers.Add("Accept-Language","en-us,en;q=0.5")
			request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"
			request.Headers.Add("Accept-Encoding","gzip,deflate")
			request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.0.3) Gecko/2008092417 Firefox/3.0.3"
			request.Referer = "http://www.psyshop.com/"

			request.Method="POST"
			request.ContentType = "application/x-www-form-urlencoded"
			bytes = System.Text.UTF8Encoding().GetBytes(content)
			request.ContentLength = bytes.Length
			stream = request.GetRequestStream()
			stream.Write(bytes,0,bytes.Length)
			stream.Close()
			streamresponse = request.GetResponse().GetResponseStream()
			return System.IO.StreamReader(streamresponse).ReadToEnd()
		ensure:
			servicePoint.Expect100Continue = prevValue;

	static def GetResult(param):
		return param
