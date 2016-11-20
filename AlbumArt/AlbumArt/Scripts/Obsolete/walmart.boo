namespace CoverSources
import System.Text.RegularExpressions
import util
class Walmart:
	static SourceName as string:
		get: return "Walmart"
	static SourceVersion as decimal:
		get: return 0.2
	static def GetThumbs(coverart,artist,album):
	        query = artist+" "+album
	        params = 'search_query=' + EncodeUrl(query)
	        text = GetPage("http://www.walmart.com/catalog/search-ng.gsp?Continue.x=0&Continue.y=0&Continue=Find&ics=20&ico=0&search_constraint=4104&" + params)
	        r = Regex("<a\\shref=\"/catalog/product\\.do\\?product_id=[0-9]+\"><img\\ssrc=\"([^\"]+)60X60.gif\"[^>]+alt=\"([^\"]*)\"[^>]*>",RegexOptions.Multiline)
	        r2 = Regex("""<a\shref="javascript:photo_opener\('(http://i.walmart.com/[a-zA-Z0-9/]+_)500X500.jpg""",RegexOptions.Multiline)
	        if r2.IsMatch(text):
	            result = r2.Match(text)
	            coverart.AddThumb(result.Groups[1].Value+"150X150.jpg","Product Page",500,500,result.Groups[1].Value)
	            return
	        iterator = r.Matches(text)
	        coverart.SetCountEstimate(iterator.Count)
	        for result as Match in iterator:
	            coverart.AddThumb(result.Groups[1].Value+"150X150.jpg",result.Groups[2].Value,500,500,result.Groups[1].Value)
	static def GetResult(param):
	        return param as string + "500X500.jpg"            
