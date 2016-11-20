import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class Kalahari(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "Kalahari"
	Version as string:
		get: return "0.7"
	Author as string:
		get: return "Alex Vallat"
	Category as string:
		get: return "South African"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		query = EncodeUrl(album + " " + artist)

		searchResults = GetPage("http://www.kalahari.com/s;?Ntt=${query}&N=4294966903")

		//Find album info
		matches = Regex("<a\\s+href=\"(?<info>[^\"]+)\"[^>]*>\\s*<img\\salt=\"(?<title>[^\"]+)\"\\s+src=\"(?<image>[^\"]+?)0\\.jpg", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResults)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			image = match.Groups["image"].Value;

			results.Add(image + "1.jpg", match.Groups["title"].Value, "http://www.kalahari.com" + match.Groups["info"].Value, -1, -1, image + "2.jpg", CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;