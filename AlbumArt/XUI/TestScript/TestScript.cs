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
using System.Threading;
using System.Configuration;

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

		/*
		private class TestWin : System.Windows.Window
		{
			public TestWin()
			{
				var a = new System.Windows.Media.Animation.ThicknessAnimation();
 				

			}
		}*/

		public void Search(string artist, string album, IScriptResults results)
		{
			/*
      var bitmap = new Bitmap(400, 600);
      var g = Graphics.FromImage(bitmap);
      g.Clear(SystemColors.Window);
      g.DrawString("hello\nthere", SystemFonts.DialogFont, SystemBrushes.WindowText, PointF.Empty);
      g.Dispose();

      results.Add(ConvertImageToStream(bitmap), "test", null);
      bitmap.Dispose();

      return;
			 * */
			
			int numberOfResults = 1;
			results.EstimatedCount = numberOfResults;
			Random rnd = new Random();

            var assembly = GetType().Assembly;

            try
            {
              for (int i = 0; i < numberOfResults; i++)
              {
                //results.Add(thumbnail, i.ToString(), "notauri", 1000 + rnd.Next(6) * 100, rnd.Next(1, 1600), fullSize, (CoverType)rnd.Next((int)CoverType.Unknown, (int)CoverType.CD + 1));
                results.Add(assembly.GetManifestResourceStream("TestScript.testThumbnail.jpg"), i.ToString(), "notauri", -1, -1, assembly.GetManifestResourceStream("TestScript.testBadImage.jpg"), (CoverType)rnd.Next((int)CoverType.Unknown, (int)CoverType.Booklet + 1), "gif");
                System.Threading.Thread.Sleep(1000);
              }
            }
            catch (ThreadAbortException)
            {
              System.Diagnostics.Debug.WriteLine("Exception");
            }
            finally
            {
              System.Diagnostics.Debug.WriteLine("finished");
            }
		}

    	private static Stream ConvertImageToStream(Image image)
      {
	
		var stream = new System.IO.MemoryStream();
		image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
		
		stream.Seek(0, SeekOrigin.Begin)		;
    return stream;
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
			//System.Threading.Thread.Sleep(3000);
			return fullSizeCallbackParameter;
		}

		/*private string GetPage(string uri)
		{
			return new System.Net.WebClient().DownloadString(uri);
		}*/
	}
}
