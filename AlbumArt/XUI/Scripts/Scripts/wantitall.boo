import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class WantItAll(AlbumArtDownloader.Scripts.IScript, ICategorised):
	Name as string:
		get: return "WantItAll"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "alsaan"
	Category as string:
		get: return "South African"
		
	def Search(artist as string, album as string, results as IScriptResults):
	
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)
		
		searchString = artist + " " + album
		searchString = searchString.Trim().ToLower().Replace(" ", "-")
		
		searchUrl = String.Format("http://www.wantitall.co.za/{0}/Music/p1", searchString)
		html = GetPage(searchUrl)
		
		re = Regex("<li class=\"aproduct\"><a href=\"(?<productPageUrl>/Music[^\"]+)\">.+?<img src=\"(?<thumbnailUrl>[^\"]+)\" alt=\"(?<name>[^\"]+)\"")
		matches = re.Matches(html)
		
		results.EstimatedCount = matches.Count
		
		for match in matches:
			name = match.Groups["name"].Value
			productPageUrl = "http://www.wantitall.co.za" + match.Groups["productPageUrl"].Value
			thumbnailUrl = match.Groups["thumbnailUrl"].Value
			
			if thumbnailUrl != "http://www.wantitall.co.za//images/no_image_small.jpg":
				results.Add(thumbnailUrl, name, productPageUrl, -1, -1, thumbnailUrl, CoverType.Front)

	def RetrieveFullSizeImage(thumbnailUrl): 
	
		return thumbnailUrl.Replace("._SL75_","")