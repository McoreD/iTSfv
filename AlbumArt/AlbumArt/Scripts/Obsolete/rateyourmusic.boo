namespace CoverSources
import System.Text.RegularExpressions
import util
class RateYourMusic:
	static SourceName as string:
		get: return "RateYourMusic"
	static SourceVersion as decimal:
		get: return 0.2
	static def GetThumbs(coverart,artist,album):
        params as string = 'searchterm='+ EncodeUrl(album) + "&type=l"
        text = GetPage("http://www.rateyourmusic.com/search?" + params)
        r = Regex("<img[^>]*?alt=\"([^\"]*)\"[^>]*?src=\"/album_images[0-9]*/s([0-9]+).jpg\">",RegexOptions.Multiline)
        iterator = r.Matches(text)
        coverart.SetCountEstimate(iterator.Count)
        for match as Match in iterator:
            coverart.AddThumb("http://www.rateyourmusic.com/album_images/s"+match.Groups[2].Value+".jpg",match.Groups[1].Value,0,0,match.Groups[2].Value)
    static def GetResult(param):
		return "http://www.rateyourmusic.com/album_images/o"+param+".jpg"
	
