# refs: ICSharpCode.SharpZipLib.dll
namespace CoverSources
import System.Xml
import util

class iTMS:
	static def GetPageSecret(url as string):
		request as System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(url)
		request.UserAgent = "iTunes/7.4 (Macintosh; U; PPC Mac OS X 10.4.7)"
		request.Headers.Add("X-Apple-Tz","-21600")
		request.Headers.Add("X-Apple-Store-Front","143441")
		request.Headers.Add("Accept-Language","en-us, en;q=0.50")
		request.Headers.Add("Accept-Encoding","gzip, x-aes-cbc")
		response = request.GetResponse()
		return response.GetResponseStream()
	static SourceName as string:
		get: return "iTunes Music Store"
	static SourceVersion as string:
		get: return "0.4"
	static SourceCreator as string:
		get: return "david_dl, Alex Vallat"
	static def GetThumbs(coverart,artist,album):
		x=System.Xml.XmlDocument()
		t=GetPageSecret("http://ax.phobos.apple.com.edgesuite.net/WebObjects/MZSearch.woa/wa/coverArtMatch?an="+EncodeUrl(artist)+"&pn="+EncodeUrl(album))
		try:
			x.Load(ICSharpCode.SharpZipLib.GZip.GZipInputStream(t))
		except e as ICSharpCode.SharpZipLib.GZip.GZipException:
			return //Wasn't a zip, so art wasn't found
		
		tags=x.GetElementsByTagName("dict")
		if tags.Count==0:
			return
		url=""
		albumname=album
		for tag in tags[0].ChildNodes:
			if tag.InnerText=="cover-art-url":
				url=tag.NextSibling.InnerText
			if tag.InnerText=="playlistName":
				albumname=tag.NextSibling.InnerText
		if url.Length>0:
			url = System.Text.RegularExpressions.Regex("\\.enc\\.jpg\\?.*").Replace(url, "")
			coverart.SetCountEstimate(1)
			coverart.AddThumb(url + ".170x170-75.jpg", albumname, -1, -1, url)
	static def GetResult(url):
		try:
			return GetPageSecret(url + ".jpg");
		except e:
			return GetPageSecret(url + ".tif");
