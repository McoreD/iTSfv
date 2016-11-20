# refs: System.Web
namespace util

import System.Text
import System.IO

def GetPageStream(url as string):
	return GetPageStream(url, null)
def GetPageStream(url as string, referer as string):
	return GetPageStream(url, null, false)
def GetPageStream(url as string, referer as string, useFirefoxHeaders as bool):
	request = System.Net.HttpWebRequest.Create(url) as System.Net.HttpWebRequest
	if not string.IsNullOrEmpty(referer):
		request.Referer = referer;

	if useFirefoxHeaders:
		request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"
		request.Headers.Add("KEEP_ALIVE","300")
		request.Headers.Add("ACCEPT_CHARSET","ISO-8859-1,utf-8;q=0.7,*;q=0.7")
		#request.Headers.Add("ACCEPT_ENCODING","gzip,deflate")
		request.Headers.Add("ACCEPT_LANGUAGE","en-us,en;q=0.5")
		request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.0.3) Gecko/2008092417 Firefox/3.0.3 "

	response = request.GetResponse()
	return response.GetResponseStream()	
	

def GetPage(url as string):
	return GetPage(GetPageStream(url))
def GetPage(pageStream as Stream):
	return GetPage(pageStream, Encoding.UTF8)
def GetPage(pageStream as Stream, encoding as Encoding):
	return StreamReader(pageStream, encoding).ReadToEnd()


def EncodeUrl(url as string):
	return EncodeUrl(url, Encoding.UTF8)

def EncodeUrl(url as string, encoding as Encoding):
	return System.Web.HttpUtility.UrlEncode(url, encoding)


/**
 * Does the same like util.UtilModule.GetPage,
 * but uses ISO/IEC 8859-1 (called iso-latin-1)
 * as encoding.
 * Many Pages uses this encoding for umlauts (äöüß),
 * when searching for "Die Ärzte" on such a site, using
 * this method is nessesacry
 */
def GetPageIsoLatin1(url as string):
	return GetPageIsoLatin1(url, false)

def GetPageIsoLatin1(url as string, useFirefoxHeaders as bool):
	return GetPage(GetPageStream(url, null, useFirefoxHeaders), Encoding.GetEncoding("iso-8859-1"))

/**
 * Does the same like util.UtilModule.EncodeUrl,
 * but uses ISO/IEC 8859-1 (called iso-latin-1)
 * as encoding.
 * EncodeUrl uses utf-8 per default. So encoding
 * "Die Ärzte" will result in "Die%20%c3%84rzte".
 * Some websites have a problem with that encoding.
 * These sites uses iso-latin-1 as encoding.
 * Using this methode to encode "Die Ärzte" will 
 * result in "Die%20%c4rzte"
 */
def EncodeUrlIsoLatin1(url as string):
	encoding as Encoding = Encoding.GetEncoding("iso-8859-1")#iso-latin-1
	return EncodeUrl(url, encoding)

def StripCharacters(charactersToStrip as string, stringToStrip as string):
	if(string.IsNullOrEmpty(stringToStrip)):
		return stringToStrip
	
	return Regex.Replace(stringToStrip, "[${charactersToStrip}]", "")