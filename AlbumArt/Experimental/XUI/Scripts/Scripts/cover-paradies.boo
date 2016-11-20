namespace CoverSources
import System
import System.Text
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class CoverParadies:
	static DescriptiveImageTitles = true //Set this to False to prevent the type of image (Front, Back, CD, etc.) from being appended to the image titles
	
	static SourceName as string:
		get: return "Cover-Paradies"
	static SourceCreator as string:
		get: return "Alex Vallat"
	static SourceVersion as string:
		get: return "0.13"
	static def GetThumbs(coverart,artist,album):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		query as string = artist + " " + album
		
		searchResults = GetPage(String.Format("http://www.cover-paradies.to/?Module=ExtendedSearch&StartSearch=true&PagePos=0&SearchString={0}&StringMode=Wild&DisplayStyle=Text&HideDetails=Yes&PageLimit=1000&SektionID-2=Yes", EncodeUrl(query)))
		
		//Get results
		resultsRegex = Regex(";ID=(?<ID>\\d+)\">(?!\\s*<img)", RegexOptions.Singleline)
		resultMatches = resultsRegex.Matches(searchResults)
		coverart.SetCountEstimate(resultMatches.Count * 3) //Estimate 3 covers per result. Results may vary.
		
		for resultMatch as Match in resultMatches:
			//Get the album page
			albumPageUri = String.Format("http://www.cover-paradies.to/?Module=ViewEntry&ID={0}", resultMatch.Groups["ID"].Value)
			albumPage = GetPage(albumPageUri)
			
			//Get the title for that album
			titleRegex = Regex("<title>Cover-Paradies - (?<title>[^<]+)<", RegexOptions.Singleline)
			title = titleRegex.Matches(albumPage)[0].Groups["title"].Value //Expecting only one match
			
			//Get all the images for the album
			imagesRegex = Regex("ID=(?<fullSizeID>\\d+)\"><img[^>]+?src=\"(?<thumb>[^\"]+)\" alt=\"(?<imageName>[^\"]+)\".+? (?<width>[\\d.]+) x (?<height>[\\d.]+)px", RegexOptions.Singleline)
			imageMatches = imagesRegex.Matches(albumPage)
			
			for imageMatch as Match in imageMatches:
				coverTypeString = imageMatch.Groups["imageName"].Value
				if(DescriptiveImageTitles and imageMatches.Count > 1):
					imageTitle = String.Format("{0} - {1}", title, coverTypeString)
				else:
					imageTitle = title
				
				de = System.Globalization.CultureInfo.GetCultureInfo("de-DE") //Numbers are in DE culture (. for thousands separater, not for decimal point)
				thousands = System.Globalization.NumberStyles.AllowThousands //Numbers have thousands separators.
				coverart.Add(GetPageStream(imageMatch.Groups["thumb"].Value, "http://www.cover-paradies.to"), imageTitle, albumPageUri, Int32.Parse(imageMatch.Groups["width"].Value, thousands, de), Int32.Parse(imageMatch.Groups["height"].Value, thousands, de), imageMatch.Groups["fullSizeID"].Value, string2coverType(coverTypeString))		

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

			
	static def GetResult(param):
		return String.Format("http://www.cover-paradies.to/res/exe/GetElement.php?ID={0}", param)
		
	static def string2coverType(typeString as string):
		if(string.Compare(typeString,"back",true)==0):
			return CoverType.Back;
		if(string.Compare(typeString,"cd",true)==0):
			return CoverType.CD;
		if(string.Compare(typeString,"front",true)==0):
			return CoverType.Front;
		if(string.Compare(typeString,"inlay",true)==0):
			return CoverType.Inlay;
		if(string.Compare(typeString,"inside",true)==0):
			return CoverType.Inlay;
		if(string.Compare(typeString,"booklet",true)==0):
			return CoverType.Unknown; //did not know where to sort it in.
		else:
			return CoverType.Unknown;

