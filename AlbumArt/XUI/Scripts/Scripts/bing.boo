import System
import System.Text.RegularExpressions

import util

class Bing(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Bing Images"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)
	
		searchResults = GetPage("http://www.bing.com/images/search?q="+ EncodeUrl("\"" + artist + "\" \"" + album + "\""))

		matches = Regex("<a href=\"#\"[^>]+? m=\"[^\"]+?surl:&quot;(?<url>[^\"]+?)&quot;[^\"]+?imgurl:&quot;(?<image>[^\"]+?)&quot;[^\"]+\" [^>]+? t1=\"(?<title>[^\"]+)\" t2=\"(?<width>\\d+) x (?<height>\\d+)[^>]+>\\s*<img[^>]+? src.?=\"(?<thumbnail>[^\"]+)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(searchResults)
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			url = match.Groups["url"].Value
			image = match.Groups["image"].Value
			title = match.Groups["title"].Value
			thumbnail = match.Groups["thumbnail"].Value
			height = System.Int32.Parse(match.Groups["height"].Value)
			width = System.Int32.Parse(match.Groups["width"].Value)
			
			results.Add(thumbnail, title, url, width, height, image, CoverType.Unknown);


	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter