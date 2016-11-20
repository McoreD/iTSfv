import System
import System.Net
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class Vgmdb(AlbumArtDownloader.Scripts.IScript):

	//******** For full sized images, put your VGMDB username and password here **************//
	VGMDB_UserName = ""
	VGMDB_Password = ""
	//****************************************************************************************//
	
	mCookies = CookieContainer()
	
	Name as string:
		get: return "VGMdb"
	Author as string:
		get: return "Alex Vallat"
	Version as string:
		get: return "0.6"
	
	def Search(artist as string, album as string, results as IScriptResults):
		if(artist.Equals("Various Artists", StringComparison.OrdinalIgnoreCase)):
			artist = ""; //Ignore Various Artists
		else:
			artist = StripCharacters("&.'\";:?!", artist)
		
		album = StripCharacters("&.'\";:?!", album)
		
		if(not String.IsNullOrEmpty(VGMDB_UserName) and not String.IsNullOrEmpty(VGMDB_Password)):
			//Try to log in
			GetPage("http://vgmdb.net/forums/login.php?do=login", "vb_login_username=${EncodeUrl(VGMDB_UserName)}&vb_login_password=${EncodeUrl(VGMDB_Password)}&s=&securitytoken=guest&do=login&vb_login_md5password=&vb_login_md5password_utf=", mCookies)
			//Fix for VGMDB returning cookies scoped to ".vgmdb.net"
			mCookies.SetCookies(Uri("http://vgmdb.net"), mCookies.GetCookieHeader(Uri("http://www.vgmdb.net")).Replace("; ", ","))
		
		resultsPage = GetPage("http://vgmdb.net/search?do=results", "action=advancedsearch&albumtitles=${EncodeUrl(album)}&catalognum=&eanupcjan=&dosearch=Search+Albums+Now&pubtype%5B0%5D=1&pubtype%5B1%5D=1&pubtype%5B2%5D=1&distype%5B0%5D=1&distype%5B1%5D=1&distype%5B2%5D=1&distype%5B3%5D=1&distype%5B4%5D=1&distype%5B5%5D=1&distype%5B6%5D=1&distype%5B7%5D=1&composer=${EncodeUrl(artist)}&arranger=&performer=&lyricist=&publisher=&game=&trackname=&caption=&notes=&anyfield=&releasedatemodifier=is&day=0&month=0&year=0&discsmodifier=is&discs=&pricemodifier=is&price_value=&tracklistmodifier=is&tracklists=&scanmodifier=is&scans=&albumadded=&albumlastedit=&scanupload=&tracklistadded=&tracklistlastedit=&sortby=albumtitle&orderby=ASC&childmodifier=0&src=aad", mCookies )
		
		//Get results
		resultsRegex = Regex("href=\"http://vgmdb\\.net/album/(?<id>\\d+)\" title='(?<title>[^']+)'", RegexOptions.IgnoreCase)
		resultMatches = resultsRegex.Matches(resultsPage)
		results.EstimatedCount = resultMatches.Count * 4 //Assume front, back, boocklet and disc
		
		for resultMatch as Match in resultMatches:
			id = resultMatch.Groups["id"].Value
			title = System.Web.HttpUtility.HtmlDecode(resultMatch.Groups["title"].Value)
			
			albumUrl = "http://vgmdb.net/album/${id}"
			albumPage = GetPage(albumUrl, mCookies)
			
			//Try to get full size images (will only work if logged in)
			imagesRegex = Regex("href=\"/db/covers-full\\.php\\?id=(?<fullId>\\d+)\".+?'/db/assets/covers-thumb/(?<img>[^']+)'.+?<h4 class=\"label\"[^>]+>(?<type>[^<]+)<", RegexOptions.IgnoreCase | RegexOptions.Singleline)
			imageMatches = imagesRegex.Matches(albumPage)
			
			if(imageMatches.Count == 0):
				//Get all the images for the album
				imagesRegex = Regex("\"/db/assets/covers-medium/(?<img>[^\"]+)\".+?<h4 class=\"label\"[^>]+>(?<type>[^<]+)<(?<fullId>)", RegexOptions.IgnoreCase | RegexOptions.Singleline)
				imageMatches = imagesRegex.Matches(albumPage)
			
			if(imageMatches.Count == 0):
				//Single Image Mode
				imagesRegex = Regex("'/db/assets/covers-medium/(?<img>[^']+)'(?<type>)(?<fullId>)", RegexOptions.IgnoreCase | RegexOptions.Singleline)
				imageMatches = imagesRegex.Matches(albumPage)
			
			for imageMatch as Match in imageMatches:
				img = imageMatch.Groups["img"].Value
				
				//Get the full sized image
				fullImg as String
				fullId = imageMatch.Groups["fullId"].Value
				if(not String.IsNullOrEmpty(fullId)):
					fullImg = "http://vgmdb.net/db/covers-full.php?id=" + fullId
				else:
					fullImg = "http://vgmdb.net/db/assets/covers-medium/" + img //Fall back on medium image, if not logged in
				
				coverTypeString = System.Web.HttpUtility.HtmlDecode(imageMatch.Groups["type"].Value)
				coverType = GetCoverType(coverTypeString)
				if(coverType == CoverType.Unknown and not String.IsNullOrEmpty(coverTypeString)):
					typedTitle = String.Concat(title, " - ", coverTypeString)
				else:
					typedTitle = title
			
				results.Add(
					"http://vgmdb.net/db/assets/covers-thumb/" + img, #thumbnail
					typedTitle, #name
					albumUrl, #infoUri
					-1, #fullSizeImageWidth
					-1, #fullSizeImageHeight
					fullImg, #fullSizeImageCallback
					coverType #coverType
					)

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return GetPageStream(fullSizeCallbackParameter, null, mCookies);
	
	static def GetPage(url as String, cookies as CookieContainer):
		return GetPage(url, null, cookies)
	
	static def GetPage(url as String, postData as String, cookies as CookieContainer):
		return System.IO.StreamReader(GetPageStream(url, postData, cookies)).ReadToEnd();
		
	static def GetPageStream(url as String, postData as String, cookies as CookieContainer):
		request = System.Net.HttpWebRequest.Create(url) as HttpWebRequest
		request.CookieContainer = cookies
				
		if(postData != null):
			request.Method="POST"
			request.ContentType = "application/x-www-form-urlencoded"
			bytes = System.Text.UTF8Encoding().GetBytes(postData)
			request.ContentLength = bytes.Length
			stream = request.GetRequestStream()
			stream.Write(bytes,0,bytes.Length)
			stream.Close()
		
		return request.GetResponse().GetResponseStream()
		
		
	static def GetCoverType(typeString as string):
		if(string.Compare(typeString,"back",true)==0):
			return CoverType.Back;
		if(string.Compare(typeString,"disc",true)==0):
			return CoverType.CD;
		if(string.Compare(typeString,"front",true)==0):
			return CoverType.Front;
		if(typeString.IndexOf("booklet", StringComparison.OrdinalIgnoreCase) != -1):
			return CoverType.Inside;
		
		return CoverType.Unknown;