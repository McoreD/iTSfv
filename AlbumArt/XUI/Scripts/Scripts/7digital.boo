import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class sevendigital(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "7digital"
	Version as string:
		get: return "0.4"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.7digital.com/releasesearchresult/index?pagesize=30&IsPagedRequest=true&search-type=Releases&page=0&search-term=" + EncodeUrl(artist + " " + album))
		
		matches = Regex("<img src=\"(?<image>http://cdn.7static.com/static/img/sleeveart/[^_]+)_50\\.jpg\"(?:.(?!title))+ title=\"(?<title>[^\"]+)\" href=\"(?<info>[^\"]+)\".+?\"popularity-value\"[^>]+>\\s*(?<score>[\\d\\.]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		cutOffScore as Single;

		for match as Match in matches:
			image = match.Groups["image"].Value;
			score = Single.Parse(match.Groups["score"].Value, System.Globalization.CultureInfo.InvariantCulture);
			if cutOffScore == 0:
				cutOffScore = score / 3 * 2;

			if score < cutOffScore:
				return; // Don't bother with items who score less than 2/3 of the best score. Results are sorted by score

			fullSize as string;
			size as int;

			//Detect if 800x800 size is available
			if CheckResponse(image, "800"):
				fullSize = image + "_800.jpg"
				size = 800;
			elif CheckResponse(image, "500"):
				//fall back on 500x500
				fullSize = image + "_500.jpg"
				size = 500;
			else:
				//fall back on 350x350 image
				fullSize = image + "_350.jpg"
				size = 350;

			results.Add(image + "_50.jpg", System.Web.HttpUtility.HtmlDecode(match.Groups["title"].Value), "http://7digital.com" + match.Groups["info"].Value, size, size, fullSize, CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;

	def CheckResponse(image, size):
		checkRequest = System.Net.HttpWebRequest.Create(image + "_" + size + ".jpg") as System.Net.HttpWebRequest
		checkRequest.Method = "HEAD"
		checkRequest.AllowAutoRedirect = false
		try:
			response = checkRequest.GetResponse() as System.Net.HttpWebResponse
			return response.StatusCode == System.Net.HttpStatusCode.OK
		except e as System.Net.WebException:
			return false;
		ensure:
			if response != null:
				response.Close()
		