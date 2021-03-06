import System
import System.Xml
import AlbumArtDownloader.Scripts
import util

class maniadb(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "maniadb"
	Version as string:
		get: return "0.3"
	Author as string:
		get: return "Alex Vallat"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		x = XmlDocument()

		maxResults = 20
		x.Load("http://www.maniadb.com/api/search.asp?query=${EncodeUrl(album)}&option2=artist&query2=${EncodeUrl(artist)}&display=${maxResults}")

		//results.EstimatedCount = Int32.Parse(x.SelectSingleNode("rss/channel/display").InnerText)

		resultNodes=x.SelectNodes("rss/channel/item")
		results.EstimatedCount = resultNodes.Count

		for node in resultNodes:
			imagePart = node.SelectSingleNode("thumnail").InnerText.Substring("http://image.maniadb.com/images/album_t/".Length)
			thumbnail = "http://image.maniadb.com/images/album_t/" + imagePart
			full = "http://image.maniadb.com/images/album/" + imagePart
			title = node.SelectSingleNode("title").InnerText
			page = node.SelectSingleNode("link").InnerText
			
			results.Add(thumbnail, title, page, -1, -1, full, CoverType.Front);


	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;