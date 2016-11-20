# refs: System.Web
namespace util

def GetPageStream(url as string):
            request = System.Net.HttpWebRequest.Create(url)
            response = request.GetResponse()
            return response.GetResponseStream()
def GetPage(url as string):
            s=System.IO.StreamReader(GetPageStream(url))
            return s.ReadToEnd()
def EncodeUrl(url as string):
	return System.Web.HttpUtility.UrlEncode(url.Replace("&","%26").Replace("?","%3F"));
