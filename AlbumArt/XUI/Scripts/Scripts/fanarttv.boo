import System.Text.RegularExpressions
import System.Threading
import System.Collections.Generic
import System.Web.Script.Serialization
import AlbumArtDownloader.Scripts

import util

class fanartTV(AlbumArtDownloader.Scripts.IScript):
	
	gImageReferer as string
	searchString as string

	Name as string:
		get: return "fanart.tv"
	Version as string:
		get: return "0.3"
	Author as string:
		get: return "Mordred"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!()", artist).Replace('/','+')
		album = StripCharacters("&.'\";:?!()", album).Replace('/','+')
		// sample query: http://musicbrainz.org/ws/2/release-group/?limit=100&query=artist:(Metallica)%20AND%20release:(Master of Puppets)
		mbidBaseUrl = "http://search.musicbrainz.org/ws/2/release-group/"
		if(album.Length==0):
			mbidUrl = "${mbidBaseUrl}?fmt=json&query=artist:("+ artist +")"
		elif (artist.Length==0):
			mbidUrl = "${mbidBaseUrl}?fmt=json&query=release:("+ album +")"
		else:
			mbidUrl = "${mbidBaseUrl}?fmt=json&query=artist:("+ artist +") AND release:("+ album +")"	 // AND status:Official")

		scoreThreshold = 70
		
		json = JavaScriptSerializer()
		
		try:
			mbidDoc = GetPage(mbidUrl)
			mbidResult = json.DeserializeObject(mbidDoc) as Dictionary[of string, object]

			results.EstimatedCount = mbidResult["release-group-list"]["count"]

			for releaseGroup as Dictionary[of string, object] in mbidResult["release-group-list"]["release-group"]:
				mbid = releaseGroup["id"]
				mbidArtist = releaseGroup["artist-credit"]["name-credit"][0]["artist"]["name"]
				mbidTitle = releaseGroup["title"]
				mbidScore = System.Convert.ToInt32(releaseGroup["score"])

				if mbidScore > scoreThreshold:
					try:
						val = "e0b3b908a83ae178fa15f7e06fbef29a"
						fanartURL = "http://api.fanart.tv/webservice/album/${val}/${mbid}/json/all/1/2/"
						fanartJson = GetPage(fanartURL)
						if string.Compare(fanartJson, "null"):
							fanartResult = json.DeserializeObject(fanartJson) as Dictionary[of string, object]
							for result in fanartResult.Values:
    							fanartArtistResult = result["albums"]["${mbid}"]
								if fanartArtistResult.ContainsKey("albumcover"):
									for image as Dictionary[of string, object] in fanartArtistResult["albumcover"]:
										pictureUrl = image["url"]
										thumbnailUrl = pictureUrl + "/preview"
										name = mbidArtist + " - " + mbidTitle
										coverType = CoverType.Front
										results.Add(thumbnailUrl, name, "", 1000, 1000, pictureUrl, coverType)
								if fanartArtistResult.ContainsKey("cdart"):
									for image as Dictionary[of string, object] in fanartArtistResult["cdart"]:
										pictureUrl = image["url"]
										thumbnailUrl = pictureUrl + "/preview"
										name = mbidArtist + " - " + mbidTitle
										coverType = CoverType.CD
										results.Add(thumbnailUrl, name, "", 1000, 1000, pictureUrl, coverType)
							
						Thread.Sleep(100)	// to avoid API hammering

					except e as System.Net.WebException:
						results.EstimatedCount--

		except e:
			return

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;