import System.IO
import System.Net
import AlbumArtDownloader.Scripts
import util

class GoogleImage(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "GoogleImage"
	Version as string:
		get: return "0.18"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		url = "https://www.google.com/search?q=" + EncodeUrl(artist + " " + album) + "&gbv=2&tbm=isch"

		request = System.Net.HttpWebRequest.Create(url) as System.Net.HttpWebRequest
		request.Accept = "text/html, application/xhtml+xml, */*"
		request.AutomaticDecompression = DecompressionMethods.GZip
		request.Headers.Add("Accept-Language","en-GB")
		request.UserAgent = "Mozilla/5.0 Firefox/25.0"

		imagesHtml = StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd()

		imageMatches = Regex("/imgres\\?(?<params>[^\"]+).+?\"s\":\"(?<title>[^\"]+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(imagesHtml)
		
		results.EstimatedCount = imageMatches.Count
		
		paramsMatcher = Regex("(?<name>[^=]+)=(?<value>.+?)(&amp;|$)")
		for imageMatch as Match in imageMatches:
			paramsMatches = paramsMatcher.Matches(imageMatch.Groups["params"].Value);
			params = {}
			for paramsMatch as Match in paramsMatches:
				params[paramsMatch.Groups["name"].Value] = paramsMatch.Groups["value"].Value

			title = System.Web.HttpUtility.HtmlDecode(imageMatch.Groups["title"].Value.Replace("\\u0026","&"))
			fullSize = params["imgurl"]
			infoUri = params["imgrefurl"]
			height = System.Int32.Parse(params["h"])
			width = System.Int32.Parse(params["w"])
			tbnid = params["tbnid"]
			
			results.Add("http://tbn0.google.com/images?q=tbn:${tbnid}", title, infoUri, width, height, fullSize);


	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter

