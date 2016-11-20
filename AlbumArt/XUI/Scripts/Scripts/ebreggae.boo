import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class EBReggae(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "EB Reggae"
	Version as string:
		get: return "0.2"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "Reggae"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.ebreggae.com/Home.asp?X3QT=NewSearch&X3A=" + EncodeUrl(artist + " " + album))
		
		matches = Regex("'http://www\\.ebreggae\\.com/[^/]+/MB(?<id>\\d+)W[^\"]+'[^>]+>(?<title>[^>]+)</font>", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count * 2

		for match as Match in matches:
			id = match.Groups["id"].Value
			title = match.Groups["title"].Value
			thumbnailUrl = "http://www.ebreggae.com/i50/M${id}W50.jpg"

			request as System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(thumbnailUrl)
			response = request.GetResponse()
			if response.ContentType.StartsWith("image/"):
				results.Add(thumbnailUrl, title, "", 595, 520, "http://www.ebreggae.com/i595/M${id}W595.jpg", CoverType.Front)
				results.Add("http://www.ebreggae.com/i50/MB${id}W50.jpg", title, "", 595, 520, "http://www.ebreggae.com/i595/MB${id}W595.jpg", CoverType.Back)

			response.Close();

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter