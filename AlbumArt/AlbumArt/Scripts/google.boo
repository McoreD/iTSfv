namespace CoverSources
import System.Xml
import System.Drawing
import System.Text.RegularExpressions
import util

class GoogleImage:
	static def GetPageSecret(url as string):
		request as System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(url)
		request.UserAgent="Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727)"
		response = request.GetResponse()
		return response.GetResponseStream()
	static SourceName as string:
		get: return "GoogleImage"
	static SourceCreator as string:
		get: return "Unknown, Marc Landis"
	static SourceVersion as string:
		get: return "0.4"
	static def GetThumbs(coverart,artist,album):
		query = artist+" "+album
		params = EncodeUrl(query)
		params.Replace('%20','+')
		textstream = GetPageSecret("http://images.google.com/images?q="+params)
		text = System.IO.StreamReader(textstream).ReadToEnd()
		r = Regex("""dyn\.Img\("([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)","([^"]*)"\)""")
		iterator = r.Matches(text)
		coverart.SetCountEstimate(iterator.Count)
		for result as Match in iterator:
			name=Regex.Replace(Regex.Replace(result.Groups[7].Value,".x3cb.x3e",""),".x3c/b.x3e","")
			sizeString = result.Groups[10].Value
			sizeRegex = Regex("(?<width>\\d+) x (?<height>\\d+)")
			match = sizeRegex.Matches(sizeString)[0]
			width = System.Int32.Parse(match.Groups["width"].Value)
			height = System.Int32.Parse(match.Groups["height"].Value)
			coverart.AddThumb("http://images.google.com/images?q=tbn:"+result.Groups[3].Value+result.Groups[4].Value,name,width,height,result.Groups[4].Value)
	static def GetResult(param):
		return param
