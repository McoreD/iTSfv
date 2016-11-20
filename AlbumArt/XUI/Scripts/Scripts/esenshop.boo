import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class Esenshop(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "esenshop"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "Turkish"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		encoding = System.Text.Encoding.GetEncoding(1254) // Turkish
		query = EncodeUrl(artist + " " + album, encoding)

		//Retrieve the search results page
		searchResultsHtml as string = Post("http://www.esenshop.com/search.aspx", "ddAraKat=5&txtAra=${query}&txtAraGor=${query}", encoding)
		
		matches = Regex("upload/big/(?<img>\\d+)\\.jpg\"[^>]+>\\s*</td>\\s*</tr>\\s*<tr>\\s*<td[^>]*>\\s*<span id=\"dlUrun[^>]*><a class=\"menu\" href=\"(?<url>[^\"]+)\"><b>(?<artist>[^<]+?)</b><br>(?<album>[^<]+?)</a>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count

		for match as Match in matches:
			img = match.Groups["img"].Value
			title = match.Groups["artist"].Value + " " + match.Groups["album"].Value

			thumbnail = TryGetImageStream("http://www.esenshop.com/ThumbJpeg.ashx?VFilePath=upload/big/${img}.jpg")

			if thumbnail != null:
				results.Add(thumbnail, System.Web.HttpUtility.HtmlDecode(title), "http://www.esenshop.com/" + match.Groups["url"].Value, -1, -1, "http://www.esenshop.com/upload/big/${img}.jpg", CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter

	static def TryGetImageStream(url):
		request as System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(url)
		try:
			response = request.GetResponse()
			if response.ContentLength > 0:
				return response.GetResponseStream()
			
			response.Close()
			return null
		except e as System.Net.WebException:
			return null

	static def Post(url as String, content as String, encoding as System.Text.Encoding):
		request = System.Net.HttpWebRequest.Create(url)
		request.Method="POST"
		request.ContentType = "application/x-www-form-urlencoded"
		bytes = System.Text.UTF8Encoding().GetBytes(content)
		request.ContentLength = bytes.Length
		stream = request.GetRequestStream()
		stream.Write(bytes,0,bytes.Length)
		stream.Close()
		streamresponse = request.GetResponse().GetResponseStream()

		return System.IO.StreamReader(streamresponse, encoding).ReadToEnd()