import util

class AmazonJp(Amazon):
	override protected Suffix as string:
		get: return "jp"
	override protected def GetUrl(artist as string, album as string) as string:
		return "http://www.amazon.${Suffix}/gp/search/ref=sr_adv_m_pop/?search-alias=popular&__mk_ja_JP=%83J%83%5E%83J%83i&field-artist=${EncodeUrl(artist, PageEncoding)}&field-title=${EncodeUrl(album, PageEncoding)}&sort=relevancerank"
	virtual protected PageEncoding as Encoding:
		get: return System.Text.Encoding.GetEncoding(932)
	