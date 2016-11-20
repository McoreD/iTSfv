namespace CoverSources
import System.Xml
import System.Drawing
import util

class Amazon:
	static SourceName as string:
		get: return "Amazon"
	static SourceVersion as string:
		get: return "0.3"
	static SourceCreator as string:
		get: return "Unknown"
	static def GetThumbs(coverart,artist,album):
		x=System.Xml.XmlDocument()
		x.Load("http://xml.amazon.com/onca/xml3?f=xml&t=webservices-20&dev-t=1MV23E34ARMVYMBDZB02&type=lite&page=1&mode=music&KeywordSearch="+EncodeUrl(artist+" "+album))
		results=x.GetElementsByTagName("Details")
		coverart.SetCountEstimate(results.Count)
		for node in results:
			coverart.AddThumb(node["ImageUrlMedium"].InnerText,node["ProductName"].InnerText,-1,-1,node["ImageUrlLarge"].InnerText)
	static def GetResult(param):
		return param

