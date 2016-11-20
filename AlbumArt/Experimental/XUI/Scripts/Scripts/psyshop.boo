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
		get: return "0.3"
	static def GetThumbs(coverart,artist,album):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		query as string = artist + " " + album
		
		resultsPage = Post("http://217.160.136.176/cgi-bin/search.cgi", String.Format("boolean=AND&case=INSENSITIVE&cd=TRUE&terms={0}", EncodeUrl(query)))
		
		//Get results
		resultsRegex = Regex("<A HREF=\"http://www.psyshop.com/shop/CDs/[^/]+/(?<id>[^\\.]+).html\"><DIV CLASS=\"n\">(?<title>[^<]+)</DIV>", RegexOptions.Multiline | RegexOptions.IgnoreCase)
		resultMatches = resultsRegex.Matches(resultsPage)
		coverart.SetCountEstimate(resultMatches.Count)
		
		for resultMatch as Match in resultMatches:
			id = resultMatch.Groups["id"].Value
			coverType = CoverType.Front #Assume that the image is always the front cover
			albumUrl = "http://www.psyshop.com/shop/CDs/${id}.html"
			coverart.Add(
					"http://217.160.164.51/pic/${id}_s.jpg", #thumbnail
					resultMatch.Groups["title"].Value, #name
					albumUrl, #infoUri
					512, #fullSizeImageWidth
					512, #fullSizeImageHeight
					"http://217.160.138.169/pic_b/${id}_b.jpg", #fullSizeImageCallback
					coverType #coverType
					)

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
			
	static def GetResult(param):
		return param
