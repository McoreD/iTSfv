using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AlbumArtDownloader
{
	internal static class BitmapHelpers
	{
		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		private static extern bool DeleteObject(IntPtr hObject);

		/// <summary>
		/// Synchronously downloads or converts a bitmap from an object which may be any of:
		/// <para>System.Drawing.Bitmap</para>
		/// <para>Uri</para>
		/// <para>String (containing a url)</para>
		/// <para>Stream</para>
		/// </summary>
		/// <param name="from"></param>
		/// <returns></returns>
		public static Bitmap GetBitmap(object from)
		{
			Bitmap bitmap = null;

			try
			{
				if (from is Bitmap)
				{
					bitmap = (Bitmap)from;
				}
				else
				{
					Stream stream = null;
					if (from is Stream)
					{
						stream = (Stream)from;
					}
					else
					{
						Uri uri = null;
						if (from is string)
						{
							uri = new Uri((string)from, UriKind.Absolute);
						}
						else if (from is Uri)
						{
							uri = (Uri)from;
						}
						if (uri != null)
						{
							WebRequest request = HttpWebRequest.Create(uri);
							stream = request.GetResponse().GetResponseStream();
						}
					}
					if (stream != null)
					{
						bitmap = (Bitmap)Bitmap.FromStream(stream);
					}
				}
			}
			catch (Exception)
			{
				System.Diagnostics.Trace.Write("Could not get a bitmap for: ");
				System.Diagnostics.Trace.WriteLine(from);
			}
			return bitmap;
		}

		/// <summary>
		/// Converts a System.Drawing.Bitmap to a System.Windows.Media.Imaging.BitmapSource
		/// </summary>
		public static BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
		{
			BitmapSource bitmapSource = null;
			if (bitmap != null)
			{
				IntPtr hBitmap = bitmap.GetHbitmap();
				try
				{
					bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
					   hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
					bitmapSource.Freeze();
				}
				finally
				{
					DeleteObject(hBitmap);
				}
			}
			return bitmapSource;
		}
	}
}
