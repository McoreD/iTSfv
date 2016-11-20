import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class AbsolutePunk(AlbumArtDownloader.Scripts.IScript, ICategorised):

	Name as string:
		get: return "absolutepunk.net"
	Version as string:
		get: return "0.2"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "Punk, Metal, Rock"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)
		query = EncodeUrl(artist + " - " + album)

		//Retrieve the search results

		searchResultsHtml as string = Post("http://www.absolutepunk.net/gallery/search.php", "s=&do=searchresults&string=${query}&dosearch=Search&fields%5B%5D3=title&catids%5B%5D=7");

		matches = Regex("i=(?<id>\\d+)&[^>]+>\\s*<img [^>]*?src=\"(?<thumb>[^\"]+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			id = match.Groups["id"].Value;
			thumb = match.Groups["thumb"].Value;
			
			fullSizePage = GetPage("http://www.absolutepunk.net/gallery/showimage.php?i=${id}&original=1")
			imageMatch = Regex("<img alt=\"(?<title>[^\"]+)\"[^>]+?height=\"(?<height>\\d+)\"[^>]+?src=\"(?<full>[^\"]+)\"[^>]+?width=\"(?<width>\\d+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(fullSizePage)
			title = imageMatch.Groups["title"].Value;
			full = imageMatch.Groups["full"].Value;
			height = System.Int32.Parse(imageMatch.Groups["height"].Value)
			width = System.Int32.Parse(imageMatch.Groups["width"].Value)

			results.Add(thumb, title, "http://www.absolutepunk.net/gallery/showimage.php?i=${id}", width, height, full, CoverType.Front)

	def RetrieveFullSizeImage(url):
		return url

	def Post(url as String, content as String):
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