import System
import System.Xml
import AlbumArtDownloader.Scripts
import util

/**
 * Searches for covers on freecover using the API 
 */
class freecoversApi(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Freecovers API"
	Version as string:
		get: return "0.2"
	Author as string:
		get: return "daju"
	
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		searchFor = EncodeUrlIsoLatin1("${artist} ${album}")
		
		baseUrl = "http://www.freecovers.net/api/search/"
		url = "${baseUrl}${searchFor}/Music+CD"
		/**
		 * To support umlauts, a ugly hack is necessary.
		 * freecovers xml result contains as first row:
		 * <?xml version="1.0" encoding="utf-8" ?>
		 * but actually the encoding is not utf-8,
		 * but iso-latin-1. 
		 */
		xmlResult as string = GetPageIsoLatin1(url)
		reader as XmlReader = XmlReader.Create(System.IO.StringReader(xmlResult))
		
		x = System.Xml.XmlDocument()
		x.Load(reader)
		titleNodes = x.SelectNodes("rsp[@stat='ok']/title")
//		allCoverNodes = x.SelectNodes("rsp[@stat='ok']/title/covers/cover")
		for titleNode as XmlNode in titleNodes:
			albumName = titleNode.SelectSingleNode("name").InnerText
			if (String.IsNullOrEmpty(artist) or albumName.IndexOf(artist, StringComparison.OrdinalIgnoreCase) >= 0) and (String.IsNullOrEmpty(album) or albumName.IndexOf(album, StringComparison.OrdinalIgnoreCase) >= 0):
				coverNodes = titleNode.SelectNodes("covers/cover")
				results.EstimatedCount += coverNodes.Count
				for coverNode as XmlNode in coverNodes:
					typeString = coverNode.SelectSingleNode("type").InnerText
					buyUrl = coverNode.SelectSingleNode("url").InnerText
					thumbnailUrl = coverNode.SelectSingleNode("thumbnail").InnerText
					previewUrl = coverNode.SelectSingleNode("preview").InnerText
					
					results.Add(thumbnailUrl, albumName, buyUrl, 400, 400, previewUrl, string2coverType(typeString));
		
	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;

	def string2coverType(typeString as string):
		if(typeString.ToLower().StartsWith("front")):
			return CoverType.Front;
		elif(typeString.ToLower().StartsWith("back")):
			return CoverType.Back;
		elif(typeString.ToLower().StartsWith("inlay")):
			return CoverType.Inlay;
		elif(typeString.ToLower().StartsWith("cd")):
			return CoverType.CD;
		elif(typeString.ToLower().StartsWith("inside")):
			return CoverType.Inlay;
		else:
			return CoverType.Unknown;