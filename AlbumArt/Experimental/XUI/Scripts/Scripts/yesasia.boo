import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class YesAsia(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "YesAsia"
	Version as string:
		get: return "0.3"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		searchResultsHtml as string = GetPage("http://www.yesasia.com/global/search-music/0-0-0-bpt.48_bt.48_cioos.true_q." + EncodeUrl(artist + " " + album) + "_ss.101-en/list.html")
		
		matches = Regex("<span class=\"cover\"><img alt=\"(?<name>[^\"]+)\"[^>]+?\\ssrc=\"(?<img1>[^\"]+?)s(?<img2>_[^\"]+)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			name = match.Groups["name"].Value
			img1 = match.Groups["img1"].Value
			img2 = match.Groups["img2"].Value
			
			results.Add(img1 + "s" + img2, name, null, -1, -1, img1 + "l" + img2, CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;