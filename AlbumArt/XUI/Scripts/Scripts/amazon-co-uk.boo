class AmazonCoUk(Amazon):
	override protected Suffix as string:
		get: return "co.uk"
	override protected CountryCode as string:
		get: return "02"
