# refs: System.Web System.Windows.Forms
namespace CoverSources
import System.Text.RegularExpressions
import util
class CoverParadies:
	static SourceName as string:
		get: return "Cover Paradies"
	static SourceVersion as decimal:
		get: return 0.1
	static def GetThumbs(coverart,artist,album):
			url="http://www.cover-paradies.to/?Module=SimpleSearch"
			query as string=artist+" "+album
			query.Replace(' ','+')
			request = System.Net.HttpWebRequest.Create(url)
			content = "Page=0&SearchString="+EncodeUrl(query)+"&Sektion=2&B322=GO%21"
			request.Method="POST"
			request.ContentType="application/x-www-form-urlencoded"
			bytes=System.Text.UTF8Encoding().GetBytes(content)
			request.ContentLength=bytes.Length
			stream=request.GetRequestStream()
			stream.Write(bytes,0,bytes.Length)
			stream.Close()
			streamresponse=request.GetResponse().GetResponseStream()
			text=System.IO.StreamReader(streamresponse).ReadToEnd()
			i as int=text.IndexOf("Google Suche")
			if i == -1:
				return;
			text=text.Substring(i);
			reg=regex("""<img\salt="Uploader:[^"]*"\ssrc="http://193.138.231.156/~cover/Cover-Archiv/([0-9]+)/thumbs/([0-9]+).JPEG"\swidth=[^>]*>""",RegexOptions.Multiline)
			reg2=regex("""<p\salign="center"><font\ssize="3">([^<]*)</font><br><font\ssize="2">\(Uploaded.*?</font></p>.*?<img\ssrc="http://193.138.231.156/~cover/Archiv/Cover/Audio-CD/([A-Z\@])/Front/thumbs/([0-9]+).jpg""",RegexOptions.Singleline)
			matches=reg.Matches(text)
			if matches.Count==0:
				matches=reg2.Matches(text)
				coverart.SetCountEstimate(matches.Count)
				for match as Match in matches:
					coverart.AddThumb(string.Format("http://193.138.231.156/~cover/Archiv/Cover/Audio-CD/{0}/Front/thumbs/{1}.jpg",match.Groups[2].Value,match.Groups[3].Value),match.Groups[1].Value,0,0,[match.Groups[2].Value,match.Groups[3].Value])
			else:
				coverart.SetCountEstimate(matches.Count)
				for match as Match in matches:
					coverart.AddThumb(string.Format("http://193.138.231.156/~cover/Cover-Archiv/{0}/thumbs/{1}.JPEG",match.Groups[1].Value,match.Groups[2].Value),"Unknown",0,0,[match.Groups[1].Value,match.Groups[2].Value])
			

	static def GetResult(param):
		    return string.Format("http://193.138.231.156/~cover/Cover-Archiv/{0}/{1}.JPEG",param[0],param[1])
		
