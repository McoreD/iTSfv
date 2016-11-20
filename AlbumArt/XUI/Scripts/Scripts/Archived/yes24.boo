namespace CoverSources
import System
import System.Text
import System.Text.RegularExpressions
import System.Web

class Yes24:
	static SourceName as string:
		get: return "Yes24"
	static SourceCreator as string:
		get: return "Alex Vallat"
	static SourceVersion as string:
		get: return "0.7"
	static SourceCategory as string:
		get: return "Eastern"
	static def GetThumbs(coverart,artist,album):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		encoding = Encoding.GetEncoding("euc-kr")
		url = String.Format("http://www.yes24.com/searchCenter/searchDetailResult.aspx?qtitle={0}&qauthor={1}", HttpUtility.UrlEncode(album, encoding), HttpUtility.UrlEncode(artist, encoding))
		
		goodsNoResults = GetPage(url, encoding)
		
		//Get goodsNo
		goodsNoRegex = Regex("/goods/(?<goodsNo>\\d+)'><b>(?<title>[^<]+)</b></a>", RegexOptions.Multiline)
		goodsNoMatches = goodsNoRegex.Matches(goodsNoResults)
		coverart.SetCountEstimate(goodsNoMatches.Count)

		for goodsNoMatch as Match in goodsNoMatches:
			title = goodsNoMatch.Groups["title"].Value
			
			imageUri = "http://image.yes24.com/goods/" + goodsNoMatch.Groups["goodsNo"].Value
			coverart.AddThumb(imageUri + "/M", title, -1, -1, imageUri + "/L")
	
	static def GetPage(url as string, encoding as Encoding):
			request = System.Net.HttpWebRequest.Create(url)
			response = request.GetResponse()
			s = System.IO.StreamReader(response.GetResponseStream(), encoding)
			return s.ReadToEnd()
	
	static def GetResult(param):
		return param
