import System.Text
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

//Inheritors should override the Suffix property to return a valid amazon suffix (like com, co.uk, de, etc...).
abstract class Amazon(AlbumArtDownloader.Scripts.IScript):
	virtual Name as string:
		get: return "Amazon (.${Suffix})"
	Version as string:
		get: return "0.6s"
	Author as string:
		get: return "Alex Vallat"
	abstract protected Suffix as string:
		get: pass
	virtual protected SearchIndex as string: //Deprectated, ignored.
		get: return "" 
	virtual protected def GetUrl(artist as string, album as string) as string:
		return "http://www.amazon.${Suffix}/gp/search/ref=sr_adv_m_pop/?search-alias=popular&field-artist=${EncodeUrlIsoLatin1(artist)}&field-title=${EncodeUrlIsoLatin1(album)}&sort=relevancerank"
	virtual protected PageEncoding as Encoding:
		get: return Encoding.GetEncoding("iso-8859-1")
	
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)
		
		url = GetUrl(artist, album)
		resultsPage = GetPage(GetPageStream(url, null, true), PageEncoding)
		
		resultsRegex = Regex("<div\\s[^>]*class\\s*=\\s*\"image\"[^>]*>(?>.*?\\ssrc\\s*=\\s*\")(?<image>http://ecx.*?\\._)(?<thumb>(?:[^_]+_))(?<ext>\\.[^\"]+)\".*?<div\\s[^>]*class\\s*=\\s*\"title\"[^>]*>\\s*<a\\s[^>]*href\\s*=\\s*\"(?<url>[^\"]+)[^>]+>\\s*(?<title>.*?)</a>(?:\\s*<span\\s[^>]*class=\"ptBrand\"[^>]*>(?:[^<]*<a\\s[^>]*>)?\\s*(?:by |von |de )?(?<artist>[^<]+))?", RegexOptions.Singleline | RegexOptions.IgnoreCase)
		resultsMatches = resultsRegex.Matches(resultsPage)
		
		results.EstimatedCount = resultsMatches.Count
		
		for resultsMatch as Match in resultsMatches:
  			image = resultsMatch.Groups["image"].Value
  			thumb = resultsMatch.Groups["thumb"].Value
  			ext = resultsMatch.Groups["ext"].Value
  			url = resultsMatch.Groups["url"].Value
  			title = System.Web.HttpUtility.HtmlDecode(resultsMatch.Groups["title"].Value)
  			artist = System.Web.HttpUtility.HtmlDecode(resultsMatch.Groups["artist"].Value)
  			
  			results.Add(image + thumb + ext, "${artist} - ${title}", url, -1, -1, image + "SL500_" + ext, CoverType.Front)

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter

