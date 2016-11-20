namespace CoverSources
import System
import System.Text
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class eCoverTo(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "CoverLib (Cover-Paradies)"
	Author as string:
		get: return "Alex Vallat"
	Version as string:
		get: return "0.19"
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		de = System.Globalization.CultureInfo.GetCultureInfo("de-DE") //Numbers are in DE culture (. for thousands separater, not for decimal point)
		thousands = System.Globalization.NumberStyles.AllowThousands //Numbers have thousands separators.

		resultMatches = Search(artist + " - " + album)

		if resultMatches.Count == 0:
			// Try looser search terms
			resultMatches = Search(artist + " " + album)

		results.EstimatedCount = resultMatches.Count * 3; //Estimate 3 covers per result. Results may vary.
		
		for resultMatch as Match in resultMatches:
			//Get the album page
			albumPageUri = "http://coverlib.com" + resultMatch.Groups["url"].Value
			albumPage = GetPage(albumPageUri)
			
			//Get all the images for the album
			imagesRegex = Regex("<a href=\"(?<full>[^\"]+)\"[^>]+><img alt=\"(?<title>[^\"]+?) (?<type>[^\" ]+?) Cover\" [^>]+?src=\"(?<thumbnail>[^\"]+)\".+?>Res:</strong> (?<width>[\\.\\d]+) x (?<height>[\\.\\d]+)px", RegexOptions.Singleline | RegexOptions.IgnoreCase)
			imageMatches = imagesRegex.Matches(albumPage)
			
			for imageMatch as Match in imageMatches:
				thumbnail = imageMatch.Groups["thumbnail"].Value;
				full = imageMatch.Groups["full"].Value;
				title = imageMatch.Groups["title"].Value;
				type = GetCoverType(imageMatch.Groups["type"].Value);
				width = Int32.Parse(imageMatch.Groups["width"].Value, thousands, de);
				height = Int32.Parse(imageMatch.Groups["height"].Value, thousands, de);
			
				results.Add("http://ecover.to" + thumbnail, title, albumPageUri, width, height, full, type)

	static def Search(query):
		searchResults = GetPage(String.Format("http://coverlib.com/search/?q={0}&Sektion=2", EncodeUrl(query)))
		
		//Get results
		resultsRegex = Regex("<p><a href=\"(?<url>/entry/[^\"]+)\"", RegexOptions.Singleline)
		return resultsRegex.Matches(searchResults)

	static def Post(url as String, content as String):
		request = System.Net.HttpWebRequest.Create(url)
		
		servicePoint = (request as System.Net.HttpWebRequest).ServicePoint;
		prevValue = servicePoint.Expect100Continue;
		servicePoint.Expect100Continue = false;

		try:
			request.Method="POST"
			request.ContentType = "application/x-www-form-urlencoded"
			bytes = System.Text.UTF8Encoding().GetBytes(content)
			request.ContentLength = bytes.Length
			stream = request.GetRequestStream()
			stream.Write(bytes,0,bytes.Length)
			stream.Close()
			streamresponse = request.GetResponse().GetResponseStream()
			return System.IO.StreamReader(streamresponse).ReadToEnd()
		ensure:
			servicePoint.Expect100Continue = prevValue;

	def RetrieveFullSizeImage(url):
		return "http://ecover.to" + url;
		
	static def GetCoverType(typeString):
		typeString = typeString.ToLower()
		if(typeString.Equals("front")):
			return CoverType.Front;
		if(typeString.Equals("back")):
			return CoverType.Back;
		if(typeString.StartsWith("cd")):
			return CoverType.CD;
		if(typeString.StartsWith("inside")):
			return CoverType.Inside;
		if(typeString.StartsWith("booklet")):
			return CoverType.Booklet;
		
		return CoverType.Unknown;