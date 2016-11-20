namespace CoverSources
import System.Text.RegularExpressions
import util
class Buy:
	static SourceName as string:
		get: return "Buy.com"
	static SourceVersion as decimal:
		get: return 0.2
	static def GetThumbs(coverart,artist,album):
	        query = artist+" "+album
	        params = 'qu=' + EncodeUrl(query) + "&search_store=6&querytype=music&loc=109&dclksa=1"
	        text = GetPage("http://www.buy.com/retail/searchresults.asp?" + params)
	        r = Regex("<img[^>]*src=\"http://ak.buy.com/db_assets/ad_images/([0-9]+)/([0-9]+).gif\">.*?<a[^>]*class=\"medBlueText\"><b>([^>]*)</b>",RegexOptions.Multiline)
	        r2 = Regex("""javascript:largeIM\('http://ak.buy.com/db_assets/large_images/([0-9]+)/([0-9]+).jpg'\)[^>]+><img[^>]*src="http://ak.buy.com/db_assets/prod_images/\\1/\\2.jpg"[^>]*alt="([^">]+)""",RegexOptions.Multiline)
	        if r2.IsMatch(text):
	            result = r2.Match(text)
	            coverart.AddThumb("http://ak.buy.com/db_assets/prod_images/"+result.Groups[1].Value+"/"+result.Groups[2].Value+".jpg",result.Groups[3].Value,0,0,[result.Groups[1].Value,result.Groups[2].Value])
	            return
	        if text.Contains("Here is our best estimate for your search"):
	            return
	        iterator = r.Matches(text)
	        coverart.SetCountEstimate(iterator.Count)
	        for result as Match in iterator:
	            print result.Groups
	            coverart.AddThumb("http://ak.buy.com/db_assets/prod_images/"+result.Groups[1].Value+"/"+result.Groups[2].Value+".jpg",result.Groups[3].Value,0,0,[result.Groups[1].Value,result.Groups[2].Value])
	
	static def GetResult(param):
		    return "http://ak.buy.com/db_assets/large_images/"+param[0]+"/"+param[1]+".jpg"
		
