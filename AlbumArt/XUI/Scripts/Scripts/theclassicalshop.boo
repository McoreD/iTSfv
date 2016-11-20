import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class TheClassicalShop(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "The Classical Shop"
	Version as string:
		get: return "0.2"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "Classical"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)
		query = artist + " " + album;

		//Retrieve the search results
		searchResultsHtml as string = GetPage("http://www.theclassicalshop.net/SearchResults2.aspx?zoom_query=${EncodeUrl(query)}&zoom_per_page=24")
		
		matches = Regex("src=\"(?<thumb>[^\"]+?mages/(?<id>[^\"]+?).jpeg)\"\\s[^>]+?class=\"result_image\".+?class=\"result_title\".+?\\shref=\"(?<url>[^\"]+)\"[^>]+>(?<title>[^<]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			
			results.Add(match.Groups["thumb"].Value, match.Groups["title"].Value, "http://www.theclassicalshop.net/SearchResults2.aspx/" + match.Groups["url"].Value, -1, -1, match.Groups["id"].Value, CoverType.Front)

	def RetrieveFullSizeImage(id):
		return "http://www.theclassicalshop.net/download_file.aspx?file=${EncodeUrl(id)}.jpg";