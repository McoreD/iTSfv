import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class Take2(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "Take2"
	Version as string:
		get: return "0.5"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "South African"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		query = ""

		if (artist.Length!=0):
			query += EncodeUrl("\"${artist}\" ")
		if (album.Length!=0):
			query += EncodeUrl("\"${album}\"")

		//Retrieve the search results page
		searchResultsHtml as string = GetPage(GetPageStream("http://www.takealot.com/music/all?qsearch=" + query, null, true));
		
		matches = Regex("<a href=\"(?<info>[^\"]+)\"[^>]*>\\s*<noscript>\\s*<img [^>]+?src=\"(?<image>http://media\\d?\\.takealot\\.com/covers/[^\"-]+)-fixedserp\\.jpg\" alt=\"(?<title>[^\"]+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			image = match.Groups["image"].Value
			results.Add(image + "-preview.jpg", System.Web.HttpUtility.HtmlDecode(match.Groups["title"].Value), "http://www.takealot.com" + match.Groups["info"].Value, -1, -1, image + "-full.jpg", CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;
