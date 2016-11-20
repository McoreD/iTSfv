import System
import System.Text.RegularExpressions
import AlbumArtDownloader.Scripts
import util

/**
 * Searches for covers on darktown using Regex to 
 * navigate througth the sites.
 */
class darktown(AlbumArtDownloader.Scripts.IScript):
	Name as string:
		get: return "Darktown"
	Version as string:
		get: return "0.5"
	Author as string:
		get: return "daju"
	
	def Search(artist as string, album as string, results as IScriptResults):
		artist = StripCharacters("&.'\";:?!", artist)
		album = StripCharacters("&.'\";:?!", album)

		toSearchFor = "${artist} ${album}"
		toSearchFor = toSearchFor.Trim() #delete unnessary whitespaces
		toSearchFor = EncodeUrlIsoLatin1(toSearchFor)# iso-latin-1 encoding is nessecary for searching  for "Die Ärzte"
		myQuery = "http://www.darktown.ws/search.php?action=search&what=${toSearchFor}&category=audio"
		firstResultPage  = GetPageIsoLatin1(myQuery, true)
		
		resultRegex = Regex("'/coverdownload.php[^']*'", RegexOptions.Multiline)
		resultMatches = resultRegex.Matches(firstResultPage)
		results.EstimatedCount = resultMatches.Count
		for resultMatch as Match in resultMatches:
			currentRes = resultMatch.ToString().Replace('\'',' ').Trim()
			nextQuery = "http://www.darktown.to${currentRes}"
			secondResultPage  = GetPageIsoLatin1(nextQuery, true)
			
			imgRegex = Regex("\"http://img\\.darktown\\.[^/]+/getcover.php[^\"]+\"", RegexOptions.Multiline)
			imgMatches = imgRegex.Matches(secondResultPage)
			if (imgMatches.Count==1):
				imgUrl = imgMatches[0].ToString().Replace('"',' ').Trim()
						
				thumbRegex = Regex("\"http://img\\.darktown\\.[^/]+/thumbnail\\.php[^\"]+\"", RegexOptions.Multiline)
				thumbMatches = thumbRegex.Matches(secondResultPage)
				if (thumbMatches.Count==1):
					thumbUrl = 	thumbMatches[0].ToString().Replace('"',' ').Trim()
				else:
					thumbUrl = imgUrl
				
				nameRegex = Regex("<font size=4>[^<]*</font></b><br><b>[^<]*</b>", RegexOptions.Multiline)
				nameMatches = nameRegex.Matches(secondResultPage)
				if (nameMatches.Count==1):
					name = nameMatches[0].ToString().Remove(0,13).Replace("</font></b><br><b>"," - ").Replace("</b>","").Trim()
				
				typeRegex = Regex("<b>Typ des Covers:</b>[^<]*", RegexOptions.Multiline)
				typeMatches = typeRegex.Matches(secondResultPage)
				coverType = CoverType.Unknown;
				if (typeMatches.Count==1):
					type = typeMatches[0].ToString().Remove(0,22).Trim()
					coverType = string2coverType(type);
				
				kbRegex = Regex("<b>Dateigr[^:]*:</b>[^<]*", RegexOptions.Multiline)
				kbMatches = kbRegex.Matches(secondResultPage)
				if (kbMatches.Count==1):
					kb = kbMatches[0].ToString()
					kb = kb.Remove(0,kb.LastIndexOf('>')+1).Trim()
					
				results.Add(thumbUrl, "${name} - ${type} - ${kb}", nextQuery, -1, -1, imgUrl, coverType);
				
		
		
	def RetrieveFullSizeImage(fullSizeCallbackParameter):
		return fullSizeCallbackParameter;

		
	def string2coverType(typeString as string):
		if(string.Compare(typeString,"back",true)==0):
			return CoverType.Back;
		if(string.Compare(typeString,"cd",true)==0):
			return CoverType.CD;
		if(string.Compare(typeString,"front",true)==0):
			return CoverType.Front;
		if(string.Compare(typeString,"inlay",true)==0):
			return CoverType.Inlay;
		else:
			return CoverType.Unknown;
