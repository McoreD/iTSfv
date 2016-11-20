import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class sevendigital(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "7digital"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://7digital.com/search/products?searchDisplay=albums&page=1&search=" + EncodeUrl(artist + " " + album))
		
		matches = Regex("<a href=\"(?<info>[^\"]+)\"(?:.(?!</a))+<img src=\"(?<image>http://cdn.7static.com/static/img/sleeveart/[^_]+)_50.jpg\" alt=\"(?<title>[^\"]+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			image = match.Groups["image"].Value;
			fullSize as string;
			size as int;

			//Detect if 800x800 size is available
			checkFor800 = System.Net.HttpWebRequest.Create(image + "_800.jpg") as System.Net.HttpWebRequest
			checkFor800.Method = "HEAD";

			response = checkFor800.GetResponse() as System.Net.HttpWebResponse
			if response.StatusCode == System.Net.HttpStatusCode.OK:
				fullSize = image + "_800.jpg"
				size = 800;
			else:
				//fall back on 350x350 image
				fullSize = image + "_350.jpg"
				size = 350;

			response.Close();

			results.Add(image + "_50.jpg", System.Web.HttpUtility.HtmlDecode(match.Groups["title"].Value), "http://7digital.com" + match.Groups["info"].Value, size, size, fullSize, CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;
