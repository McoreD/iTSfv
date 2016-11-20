import AlbumArtDownloader.Scripts
import util

class GoogleImage(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "GoogleImage"
	Version as string:
		get: return "0.10"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		imagesHtml = GetPageIsoLatin1("http://images.google.com/images?gbv=1&q=" + EncodeUrl(artist + " " + album))

		imageMatches = @/(?i)\/imgres\?imgurl=(?<fullSize>[^&]+)&amp;imgrefurl=(?<infoUri>[^&]+)[^>]+?&amp;h=(?<height>\d+)&amp;w=(?<width>\d+)[^>]+?&amp;tbnid=(?<tbnid>[^&]+).+?<\/a><br>(?<title>.+?)<br>/.Matches(imagesHtml)
		
		results.EstimatedCount = imageMatches.Count
		
		for imageMatch as Match in imageMatches:
			title = System.Web.HttpUtility.HtmlDecode(/<\/?b>/.Replace(imageMatch.Groups["title"].Value, ""))
			fullSize = imageMatch.Groups["fullSize"].Value
			infoUri = imageMatch.Groups["infoUri"].Value
			height = System.Int32.Parse(imageMatch.Groups["height"].Value)
			width = System.Int32.Parse(imageMatch.Groups["width"].Value)
			tbnid = imageMatch.Groups["tbnid"].Value
			
			results.Add("http://tbn0.google.com/images?q=tbn:${tbnid}", title, infoUri, width, height, fullSize);


	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter

