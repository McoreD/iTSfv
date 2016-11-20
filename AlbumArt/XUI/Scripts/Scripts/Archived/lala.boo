import AlbumArtDownloader.Scripts
import util

class Lala(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Lala"
	Version as string:
		get: return "0.1"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)
		searchFor = EncodeUrl(artist + " " + album)

		apiResults = GetPage("http://www.lala.com/api/SearchPage/runBucketedSearch/v24.4.0-25?albumsQ=${searchFor}%20%20rank%3Aalbum&albumsCount=10&songsQ=[]%20&songsCount=0&artistsQ=[]%20&artistsCount=0&sortKey=Relevance&sortDir=Desc")

		matches = @/"id": "(?<id>[^"]+)"[^}]+"title": "(?<title>[^"]+)[^}]+"artist": "(?<artist>[^"]+)/.Matches(apiResults)
		
		results.EstimatedCount = matches.Count
		
		for match as Match in matches:
			title = match.Groups["artist"].Value + " - " + match.Groups["title"].Value
			id = match.Groups["id"].Value
			
			results.Add("http://album-images.pplala.com/servlet/ArtWorkServlet/${id}/xs", title, "http://www.lala.com/#album/${id}", -1, -1, "http://album-images.pplala.com/servlet/ArtWorkServlet/${id}/xl", CoverType.Front)

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter

