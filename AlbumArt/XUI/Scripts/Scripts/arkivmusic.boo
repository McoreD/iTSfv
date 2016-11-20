import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class ArkivMusik(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "ArkivMusik"
	Version as string:
		get: return "0.8"
	Author as string:
		get: return "Alex Vallat, DRata"
	Category as string:
		get: return "Classical"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Get the searchingPage
		searchPageHtml as string = GetPage("http://www.arkivmusic.com/classical/Search?all_search=1")
		searchingPage = Regex("name=\"searchingPage\" value=\"(?<searchingPage>[^\"]+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(searchPageHtml).Groups["searchingPage"].Value

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.arkivmusic.com/classical/NameList?searching=1&searchingPage=${searchingPage}&role_wanted=0&search_term=" + EncodeUrl(artist + " " + album))
		
		matches = Regex("<img\\s+class=\"albumchunk-listingimage\"\\s+alt=\"(?<title>[^\"]+)\"\\s+src=\"(?<image>[^\"]+)\".+?album_id=(?<id>\\d+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			image = match.Groups["image"].Value;
			id = match.Groups["id"].Value;
			results.Add(image, match.Groups["title"].Value, "http://www.arkivmusic.com/classical/album.jsp?album_id=" + id, -1, -1, null, CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;