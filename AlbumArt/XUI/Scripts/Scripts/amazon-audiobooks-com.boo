# refs: System.Web.Extensions

import AlbumArtDownloader.Scripts
import util

class AmazonAudiobooksCom(Amazon, ICategorised):
	override Name as string:
		get: return "Amazon Audiobooks (.com)"
	Category as string:
		get: return "Audiobooks"
	override protected Suffix as string:
		get: return "com"
	override protected def GetUrl(artist as string, album as string) as string:
		return "http://www.amazon.com/gp/search?search-alias=stripbooks&field-author=${EncodeUrl(artist)}&field-title=${EncodeUrl(album)}&sort=relevancerank&field-feature_browse-bin=618075011"