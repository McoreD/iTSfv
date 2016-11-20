import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class medianet(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "medianet"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.mndigital.com/content-experience/searchpage/ajaxhelper.aspx?contenttype=albums&keyword=\"" + EncodeUrl(artist + "\" \"" + album + "\""))
		
		matches = Regex("<a href=\"(?<url>[^\"]+)\"><img[^>]+?alt=\"Cover Art: (?<title>[^\"]+)\"[^>]+?src=\"(?<image>[^\"]+?)m\\.jpeg\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			image = match.Groups["image"].Value;
			
			results.Add(image + "s.jpeg", match.Groups["title"].Value, match.Groups["url"].Value, 800, 800, image + "g.jpeg", CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;