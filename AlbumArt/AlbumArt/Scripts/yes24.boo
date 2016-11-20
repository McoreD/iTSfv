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
		get: return "0.2"
	static def GetThumbs(coverart,artist,album):
		encoding = Encoding.GetEncoding("euc-kr")
		url = String.Format("http://www.yes24.com/searchCenter/searchresultDetail.aspx?qtitle={0}&qauthor={1}", HttpUtility.UrlEncode(album, encoding), HttpUtility.UrlEncode(artist, encoding))
		
		goodsNoResults = GetPage(url, encoding)
		
		//Get goodsNo
		goodsNoRegex = Regex("goodsNo=(?<goodsNo>\\d+)'><b>(?<title>[^<]+)</b></a>", RegexOptions.Multiline)
		goodsNoMatches = goodsNoRegex.Matches(goodsNoResults)
		coverart.SetCountEstimate(goodsNoMatches.Count)

		for goodsNoMatch as Match in goodsNoMatches:
			title = goodsNoMatch.Groups["title"].Value
			
			//Get the image results
			imageResults = GetPage(String.Format("http://www.yes24.com/Goods/FTGoodsView.aspx?goodsNo={0}", goodsNoMatch.Groups["goodsNo"].Value), encoding)
			thumbnailRegex = Regex("<img src='(?<thumbnail>[^']+)' .*? name='imageMedium'>")
			thumbnail = thumbnailRegex.Matches(imageResults)[0].Groups["thumbnail"].Value
			
			fullImageRegex = Regex("&ImgUrl=(?<fullImage>[^\"]+)\"\\)")
			fullImageMatches = fullImageRegex.Matches(imageResults)
			if fullImageMatches.Count > 0:
				fullImage = String.Format("http://image.yes24.com/momo{0}", fullImageMatches[0].Groups["fullImage"].Value)
			else:
				fullImage = thumbnail
			
			coverart.AddThumb(thumbnail, title, -1, -1, fullImage)
	
	static def GetPage(url as string, encoding as Encoding):
			request = System.Net.HttpWebRequest.Create(url)
			response = request.GetResponse()
			s = System.IO.StreamReader(response.GetResponseStream(), encoding)
			return s.ReadToEnd()
	
	static def GetResult(param):
		return param
