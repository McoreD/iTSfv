# refs: ICSharpCode.SharpZipLib.dll System.Windows.Forms
namespace CoverSources
import System.Xml
import System.Drawing
import util
import ICSharpCode.SharpZipLib

class iTMS:
	static def GetPageSecret(url as string):
		request as System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(url)
		request.UserAgent="iTunes/7.0 (Macintosh; U; PPC Mac OS X 10.4.7)"
		request.Headers.Add("X-Apple-Tz","7200")
		request.Headers.Add("X-Apple-Store-Front","143457")
		request.Headers.Add("Accept-Language","en-us, en;q=0.50")
		request.Headers.Add("Accept-Encoding","gzip, x-aes-cbc")
		response = request.GetResponse()
		return response.GetResponseStream()
	static SourceName as string:
		get: return "iTunes Music Store"
	static SourceVersion as decimal:
		get: return 0.1
	static def GetThumbs(coverart,artist,album):
		x=System.Xml.XmlDocument()
		t=GetPageSecret("http://ax.phobos.apple.com.edgesuite.net/WebObjects/MZSearch.woa/wa/coverArtMatch?an="+EncodeUrl(artist)+"&pn="+EncodeUrl(album))
		x.Load(ICSharpCode.SharpZipLib.GZip.GZipInputStream(t))
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
			coverart.SetCountEstimate(1)
			coverart.AddThumb(GetPageSecret(url),albumname,600,600,null)
	static def GetResult(param):
		return null
