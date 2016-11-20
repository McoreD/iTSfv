import System
import System.Net
import System.Collections.Generic
import System.Web.Script.Serialization
import AlbumArtDownloader.Scripts
import util
import System.Text.RegularExpressions

class Musicbrainz(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "MusicBrainz"
	Author as string:
		get: return "Sebastian Hauser"
	Version as string:
		get: return "0.9"
	
	
	def Search(artist as string, album as string, results as IScriptResults):
		if(artist!= null and album!=null):			
			//striping isn't really necessary here, because musicbrainz handles those characters quite well
			//artist = StripCharacters("&.'\";:?!", artist)
			//album = StripCharacters("&.'\";:?!", album)
			
			//mbidUrl = GetMbidUrl("Portishead", "Dummy") //Test for album Portishead - Dummy
			mbidUrl = GetMbidUrl(artist, album)
			
			json = JavaScriptSerializer()
			
			try:
				mbidDoc = GetMusicBrainzPage(mbidUrl)
				mbidResult = json.DeserializeObject(mbidDoc) as Dictionary[of string, object]

				results.EstimatedCount = mbidResult["count"]
				
				//results are sorted by score. get first score and discard all results where the score is less then 60% of that score.
				scoreThreshold = System.Convert.ToInt32(mbidResult["releases"][0]["score"]) * 0.6
				
				for release as Dictionary[of string, object] in mbidResult["releases"]:
					mbid = release["id"]
					
					//join multiple artist credits
					mbidArtist = ""
					for artistCredit as Dictionary[of string, object] in release["artist-credit"]:
						mbidArtist += artistCredit["artist"]["name"]
						if artistCredit.ContainsKey("joinphrase"):
							mbidArtist += artistCredit["joinphrase"]
							
					mbidTitle = release["title"]
					mbidScore = System.Convert.ToInt32(release["score"])
					
					if mbidScore > scoreThreshold:

						try:
							picUrl = GetCaaUrl(mbid)
							
							picDoc = GetMusicBrainzPage(picUrl)
							picResult = json.DeserializeObject(picDoc) as Dictionary[of string, object]
							
							infoUrl = picResult["release"]

							for image as Dictionary[of string, object] in picResult["images"]:
								thumbnailUrl = image["thumbnails"]["small"]
								name = mbidArtist + " - " + mbidTitle
								pictureUrl = image["image"]
								if image["front"] == true:
									coverType = CoverType.Front
								elif image["back"] == true:
									coverType = CoverType.Back
								else:
									coverType = CoverType.Unknown
								results.Add(GetMusicBrainzStream(thumbnailUrl), name, infoUrl, -1, -1, pictureUrl, coverType)
						
						except e as System.Net.WebException:
							results.EstimatedCount--
					else:
						results.EstimatedCount--
			except e:
				return
		else:
			//both Parameter album and artist are necessary
			results.EstimatedCount = 0;
	
	
	def GetCaaUrl(mbid as string):
		caaBaseUrl = "http://coverartarchive.org/release/"
		return caaBaseUrl + mbid
	
	
	def GetMbidUrl(artist as string, album as string):
		//escape lucene search term
		regexpArtist = /[+!(){}\[\]^"~*?:\\\/-]|&&|\|\|/g.Replace(artist) do (m as Match): return "\\${m}"
		regexpAlbum = /[+!(){}\[\]^"~*?:\\\/-]|&&|\|\|/g.Replace(album) do (m as Match): return "\\${m}"
		
		encodedArtist = EncodeUrl(regexpArtist)
		encodedAlbum = EncodeUrl(regexpAlbum)
		
		mbidBaseUrl = "http://musicbrainz.org/ws/2/release/"
		if artist == "" and album == "":
			return "${mbidBaseUrl}?fmt=json&query="
		elif artist == "":
			//return "${mbidBaseUrl}?fmt=json&query=release:${encodedAlbum}"
			//temporary more fuzzy, because lucene is so strict
			return "${mbidBaseUrl}?fmt=json&query=release:(${encodedAlbum})"
		elif album == "":
			//return "${mbidBaseUrl}?fmt=json&query=artist:${encodedArtist}"
			//temporary more fuzzy, because lucene is so strict
			return "${mbidBaseUrl}?fmt=json&query=artist:(${encodedArtist})"
		else:
			//return "${mbidBaseUrl}?fmt=json&query=release:${encodedAlbum} AND artist:${encodedArtist}"
			//temporary more fuzzy, because lucene is so strict
			return "${mbidBaseUrl}?fmt=json&query=release:(${encodedAlbum}) AND artist:(${encodedArtist})"
	
	
	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return GetMusicBrainzStream(fullSizeCallbackParameter);

	
	def GetMusicBrainzPage(url as String):
		stream = GetMusicBrainzStream(url)
		try:
			return GetPage(stream)
		ensure:
			stream.Close()
	
	
	def GetMusicBrainzStream(url as String):
		request = WebRequest.Create(url) as HttpWebRequest
		request.UserAgent = "AAD:" + Name + "/" + Version + " ( https://github.com/yeeeargh/aad-scripts )"
				
		return request.GetResponse().GetResponseStream()
	
	