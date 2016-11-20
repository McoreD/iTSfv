import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class CDBaby(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "CD Baby"
	Version as string:
		get: return "0.2"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		//Retrieve the search results page
		search = album
		if String.IsNullOrEmpty(album):
			search = artist //Fall back on searching by artist if the album isn't there
			
		searchResultsHtml as string = GetPage("http://www.cdbaby.com/Search/" + EncodeUrl(Base64(search)) + "/0/cmVzdWx0VHlwZTo6QWxidW0%3d")
		
		matches = Regex("title=\"(?<title>(?<artist>[^:]+)[^\"]+)\" class=\"overlay-link\" href=\"/cd/(?<id>[^\"]+)\">", RegexOptions.IgnoreCase | RegexOptions.Singleline).Matches(searchResultsHtml)
		
		results.EstimatedCount = matches.Count
		
		checkArtistMatch = not String.IsNullOrEmpty(album) and not String.IsNullOrEmpty(artist)
		
		for match as Match in matches:
			matchArtist = match.Groups["artist"].Value.Replace("&amp;", "&")
			if not checkArtistMatch or matchArtist.Contains(artist):
				title = match.Groups["title"].Value.Replace("&amp;", "&")
				id = match.Groups["id"].Value
				
				results.Add("http://www.cdbaby.com/Images/Album/${id}_small.jpg", title, "http://www.cdbaby.com/cd/${id}", 200, 200, "http://www.cdbaby.com/Images/Album/${id}.jpg", CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;
		
	def Base64(value):
		return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value))