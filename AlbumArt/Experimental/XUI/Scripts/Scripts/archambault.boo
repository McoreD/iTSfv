import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

class Archambault(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Archambault"
	Version as string:
		get: return "0.4"
	Author as string:
		get: return "Sebastien Leclerc"

	def Search(artist as string, album as string, results as IScriptResults):
		artist = EncodeUrlIsoLatin1(StripCharacters("&.'\";:?!", artist))
		album = EncodeUrlIsoLatin1(StripCharacters("&.'\";:?!", album))

		//Archambault doesn't like terms starting with "the"
		artist = Regex.Replace(artist, "^the", "", RegexOptions.IgnoreCase)
		album = Regex.Replace(album, "^the", "", RegexOptions.IgnoreCase)

		resultsPage = GetPage("http://www.archambault.ca/qmi/navigation/search/ExtendedSearchResults.jsp?erpId=ACH&searchMode=advanced&searchType=MUSIC&searchArtist=${artist}&searchAlbum=${album}&searchFormat=DC")
		
		resultsRegex = Regex("<a href=\\\"(?<url>[^\\\"]+)[^>]*title=\\\"(?<title>[^\\\"]+)\\\"(?:.(?!</a>))*?<img class=\"preview\" [^>]+ src=\"(?<image>http://storage[^=]*=?)(?<picsize>[1-9]*x[^&]+)(?<imageparm>[^\"]*?)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase)
		resultsMatches = resultsRegex.Matches(resultsPage)
	
		results.EstimatedCount = resultsMatches.Count;
		
		for resultsMatch as Match in resultsMatches:
  			image = resultsMatch.Groups["image"].Value;
  			//picsize = resultsMatch.Groups["picsize"].Value;
  			imageparm = resultsMatch.Groups["imageparm"].Value;
  			url = "http://www.archambault.ca"+resultsMatch.Groups["url"].Value;
  			title = System.Web.HttpUtility.HtmlDecode(resultsMatch.Groups["title"].Value);
  			results.Add(image+"125x125"+imageparm, title, url, -1, -1, image+"1500x1500"+imageparm, CoverType.Front);

	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter