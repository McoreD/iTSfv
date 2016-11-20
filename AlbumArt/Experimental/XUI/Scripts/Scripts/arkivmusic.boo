import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class ArkivMusik(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "ArkivMusik"
	Version as string:
		get: return "0.5"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.arkivmusic.com/classical/NameList?searching=1&role_wanted=0&search_term=" + EncodeUrl(artist + " " + album))
		
		matches = Regex("SRC=\"/graphics/covers/thumb/(?<image>[^\"]+)\".+?CLASS=\"listalbum\"><A HREF=\"(?<info>[^\"]+)\">(?<title>[^<]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			image = match.Groups["image"].Value;

			results.Add("http://www.arkivmusic.com/graphics/covers/thumb/" + image, match.Groups["title"].Value, "http://www.arkivmusic.com/classical/" + match.Groups["info"].Value, -1, -1, "http://www.arkivmusic.com/graphics/covers/full/" + image, CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;