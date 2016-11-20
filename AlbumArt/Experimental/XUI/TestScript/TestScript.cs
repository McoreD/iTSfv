using System;
using System.Collections.Generic;
using System.Text;
using AlbumArtDownloader.Scripts;
using System.Drawing;
using System.Xml;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Net.Cache;

namespace TestScript
{
	public class TestScript : IScript
	{
		public string Name
		{
			get { return "Test Script"; }
		}

		public string Author
		{
			get { return "Alex Vallat"; }
		}

		public string Version
		{
			get { return typeof(TestScript).Assembly.GetName().Version.ToString(); }
		}

		public static string GetPage(Stream pageStream, Encoding encoding)
		{
			return new StreamReader(pageStream, encoding).ReadToEnd();
		}

		public static string GetPage(string url)
		{
			return GetPage(GetPageStream(url, null, false), Encoding.UTF8);
		}

		public static Stream GetPageStream(string url, string referer, bool useFirefoxHeaders)
		{
			HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
			if (!string.IsNullOrEmpty(referer))
			{
				request.Referer = referer;
			}
			if (useFirefoxHeaders)
			{
				request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
				request.Headers.Add("KEEP_ALIVE", "300");
				request.Headers.Add("ACCEPT_CHARSET", "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
				request.Headers.Add("ACCEPT_LANGUAGE", "en-us,en;q=0.5");
				request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.0.3) Gecko/2008092417 Firefox/3.0.3 ";
			}
			return request.GetResponse().GetResponseStream();
		}



		public static string GetPage(String url, String postData, CookieContainer cookies)
		{
			HttpWebRequest request = System.Net.HttpWebRequest.Create(url) as HttpWebRequest;

			if (postData != null)
			{
				request.Method = "POST";
				request.ContentType = "application/x-www-form-urlencoded";
				var bytes = new System.Text.UTF8Encoding().GetBytes(postData);
				request.ContentLength = bytes.Length;
				var stream = request.GetRequestStream();
				stream.Write(bytes, 0, bytes.Length);
				stream.Close();
			}

			request.CookieContainer = cookies;

			var response = request.GetResponse();
			var streamresponse = response.GetResponseStream();
			return new System.IO.StreamReader(streamresponse).ReadToEnd();
		}




		public void Search(string artist, string album, IScriptResults results)
		{
			int numberOfResults = 50;
			results.EstimatedCount = numberOfResults;
			Random rnd = new Random();
			for (int i = 0; i < numberOfResults; i++)
			{
				Bitmap thumbnail = new Bitmap(typeof(TestScript), "testThumbnail.jpg");
				Bitmap fullSize = new Bitmap(typeof(TestScript), "testFullsize.jpg");
				//results.Add(thumbnail, i.ToString(), "notauri", 1000 + rnd.Next(6) * 100, rnd.Next(1, 1600), fullSize, (CoverType)rnd.Next((int)CoverType.Unknown, (int)CoverType.CD + 1));
				results.Add(thumbnail, i.ToString(), "notauri", -1, -1, fullSize, (CoverType)rnd.Next((int)CoverType.Unknown, (int)CoverType.CD + 1));
				//System.Threading.Thread.Sleep(1000);
			}
		}

		public static string Post(string url, string content)
		{
			WebRequest request = WebRequest.Create(url);

			var servicePoint = ((HttpWebRequest)request).ServicePoint;

			bool prevValue = servicePoint.Expect100Continue;
			servicePoint.Expect100Continue = false;

			try
			{
				request.Method = "POST";
				request.ContentType = "application/x-www-form-urlencoded";
				byte[] bytes = new UTF8Encoding().GetBytes(content);
				request.ContentLength = bytes.Length;
				Stream requestStream = request.GetRequestStream();
				requestStream.Write(bytes, 0, bytes.Length);
				requestStream.Close();
			
				return new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();

			}
			finally
			{
				servicePoint.Expect100Continue = prevValue;
			}
		}

 


		public object RetrieveFullSizeImage(object fullSizeCallbackParameter)
		{
			return fullSizeCallbackParameter;
		}

		/*private string GetPage(string uri)
		{
			return new System.Net.WebClient().DownloadString(uri);
		}*/
	}
}
