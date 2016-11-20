namespace CoverSources
import System
import System.Text
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class CoverIsland:
	static SourceName as string:
		get: return "CoverIsland"
	static SourceCreator as string:
		get: return "Alex Vallat"
	static SourceVersion as string:
		get: return "0.7"
	static def GetThumbs(coverart, artist as string, album as string):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		if not String.IsNullOrEmpty(artist):
			firstLetter = artist.ToLower()[0]
		elif not String.IsNullOrEmpty(album):
			firstLetter = album.ToLower()[0]
		else:
			return //Nothing to search for
		
		listPage = GetPage("http://www.coverisland.com/copertine/cover.php?op=down&ty=1&let=" + firstLetter)
		
		resultsRegex = Regex(String.Format("<option value=\"(?<value>[^\"]+)\">(?<title>[^<]*{0}[^<]+{1}[^<]*)(?=<)", artist, album), RegexOptions.Multiline | RegexOptions.IgnoreCase)
		resultMatches = resultsRegex.Matches(listPage)
		
		coverart.SetCountEstimate(resultMatches.Count)
		
		for resultMatch as Match in resultMatches:
			matchTitle = resultMatch.Groups["title"].Value
			value = resultMatch.Groups["value"].Value
			if value.StartsWith("-"):
				type = int.Parse(value.Substring(1, 3))
				segno = 1
				title = value.Substring(3)
			else:
				type = int.Parse(value.Substring(0, 2))
				segno = 0
				title = value.Substring(2)
			
			imageTypes = List(6)
			if type & 0x1 == 0x1:
				imageTypes.Add("front")
			if type & 0x2 == 0x2:
				imageTypes.Add("back")
			if type & 0x4 == 0x4:
				imageTypes.Add("inside")
			if type & 0x8 == 0x8:
				imageTypes.Add("inlay")
			if type & 0x10 == 0x10:
				imageTypes.Add("cd")
			if type & 0x20 == 0x20:
				imageTypes.Add("cd2")
		
			for typeName in imageTypes:
				imageResult = Post("http://www.coverisland.com/copertine/cover.php?op=cvr&ty=1&let=" + firstLetter, String.Format("title={0}&type=-{1}&segno={2}", title, typeName, segno))
				imageRegex = Regex("\"(?<image>http\\://www\\.coverisland\\.com/view\\.php\\?[^\"]+)\"", RegexOptions.IgnoreCase)
				imageMatches = imageRegex.Matches(imageResult)
				for imageMatch as Match in imageMatches: //Only expecting one, really.
					url = imageMatch.Groups["image"].Value
					request as System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(url)
					request.Referer = "http://www.coverisland.com/copertine/cover.php?op=cvr&ty=1&let=" + firstLetter;
					response = request.GetResponse()
					if response.ContentType.StartsWith("image/"):
						coverart.Add(
							response.GetResponseStream(), 
							String.Format("{0} - {1}", matchTitle.Trim(), typeName),
							null,
							string2coverType(typeName)
							)

	static def Post(url as String, content as String):
		request = System.Net.HttpWebRequest.Create(url)
		request.Method="POST"
		request.ContentType = "application/x-www-form-urlencoded"
		bytes = System.Text.UTF8Encoding().GetBytes(content)
		request.ContentLength = bytes.Length
		stream = request.GetRequestStream()
		stream.Write(bytes,0,bytes.Length)
		stream.Close()
		streamresponse = request.GetResponse().GetResponseStream()
		return System.IO.StreamReader(streamresponse).ReadToEnd()
			
	static def GetResult(param):
		return param
		
	static def string2coverType(typeString as string):
		if(typeString.StartsWith("front")):
			return CoverType.Front;
		elif(typeString.StartsWith("back")):
			return CoverType.Back;
		elif(typeString.StartsWith("inlay")):
			return CoverType.Inlay;
		elif(typeString.StartsWith("cd")):
			return CoverType.CD;
		elif(typeString.StartsWith("inside")):
			return CoverType.Inlay;
		else:
			return CoverType.Unknown;
