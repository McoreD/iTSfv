class AmazonFr(Amazon):
	override protected Suffix as string:
		get: return "fr"
	override protected CountryCode as string:
		get: return "08"