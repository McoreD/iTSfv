import util

class AmazonJp(Amazon):
	override protected Suffix as string:
		get: return "jp"
	override protected def GetUrl(artist as string, album as string) as string:
		return "http://www.amazon.${Suffix}/gp/search?search-alias=popular&__mk_ja_JP=%83J%83%5E%83J%83i&field-artist=${EncodeUrl(artist, ShiftJIS)}&field-title=${EncodeUrl(album, ShiftJIS)}&sort=relevancerank"
	private ShiftJIS as Encoding:
		get: return System.Text.Encoding.GetEncoding(932)
	override protected CountryCode as string:
		get: return "09"	