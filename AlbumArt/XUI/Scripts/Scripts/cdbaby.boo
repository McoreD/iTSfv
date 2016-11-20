import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class CDBaby(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "CD Baby"
	Version as string:
		get: return "0.4"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "Independent"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.cdbaby.com/Search/" + EncodeUrl(Base64("${artist} ${album}")) + "/0/cmVzdWx0VHlwZTo6QWxidW0%3d")
		
		matches = Regex("title=\"(?<title>[^\"]+)\" href=\"(?<url>/cd/[^\"]+)\"><img[^>]+?images\\.cdbaby\\.name(?<img>[^\"]+?)_small\\.jpg\"", RegexOptions.IgnoreCase | RegexOptions.Singleline).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			title = match.Groups["title"].Value.Replace("&amp;", "&")
			img = match.Groups["img"].Value
				
			results.Add("http://images.cdbaby.name${img}_small.jpg", title, "http://www.cdbaby.com/" + match.Groups["url"].Value, 200, 200, "http://images.cdbaby.name${img}.jpg", CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;
		
	def Base64(value):
		return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value))