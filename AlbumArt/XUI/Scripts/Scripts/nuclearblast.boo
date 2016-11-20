import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class NuclearBlast(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "Nuclear Blast"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "Punk, Metal, Rock"		
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.nuclearblast.de/en/shop/artikel/gruppen/51000.1.no-label.html?records_per_page=90&article_group_sort_type_handle=rank&custom_keywords=" + EncodeUrl(artist + " " + album))
		
		matches = Regex("<a class=\"article-image\" href=\"(?<url>[^\"]+)\">\\s+<img src=\"(?<image>[^?]+)\\?[^\"]+\" alt=\"(?<title>[^\"]+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			image = match.Groups["image"].Value;
			
			results.Add(image + "?x=155&amp;y=155", System.Web.HttpUtility.HtmlDecode(match.Groups["title"].Value), match.Groups["url"].Value, -1, -1, image, CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;
		