using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using AlbumArtDownloader.Scripts;
using System.IO;
using System.Net;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace AlbumArtDownloader
{
	internal class ScriptResult
	{
		private IScript mScript;
		private object mThumbnail;
		private string mName;
		private int mWidth, mHeight;
		private object mFullSizeImageCallbackParameter;
		private Bitmap mImage;
		private bool mImageDownloaded;
		private CoverType mCoverType;
		
		[System.Obsolete("set coverType")]
		public ScriptResult(IScript script, object thumbnail, string name, int width, int height, object fullSizeImageCallbackParameter)
			:this(script, thumbnail, name, width, height, fullSizeImageCallbackParameter, CoverType.Unknown)
		{}
		
		public ScriptResult(IScript script, object thumbnail, string name, int width, int height, object fullSizeImageCallbackParameter, CoverType coverType)
		{
			mScript = script;
			mThumbnail = thumbnail;
			mName = name;
			mWidth = width;
			mHeight = height;
			mFullSizeImageCallbackParameter = fullSizeImageCallbackParameter;
			mCoverType = coverType;
		}

		public CoverType CoverType { get { return mCoverType; } }

		private void DownloadImage()
		{
			if (!mImageDownloaded)
			{
				object fullSizeImage = null;
				try
				{
					fullSizeImage = mScript.RetrieveFullSizeImage(mFullSizeImageCallbackParameter);
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.Fail(String.Format("Script {0} threw an exception while retreiving full sized image: {1}", mScript.Name, e.Message));
				}
				mImage = GetBitmap(fullSizeImage);

				if (mImage == null) //If it is null, just use the thumbnail image
				{
					mImage = GetBitmap(mThumbnail);
				}

				if (mImage != null)
				{
					mWidth = mImage.Width;
					mHeight = mImage.Height;
				}
				mImageDownloaded = true;
			}
		}

		public int GetMinImageDimension(bool forceDownload)
		{
			if (mWidth < 0 || mHeight < 0 || forceDownload)
				DownloadImage(); //Must download the image to determine the width and height;

			return Math.Min(mWidth, mHeight);
		}

		public float GetImageAspectRatio(bool forceDownload)
		{
			if (mWidth < 0 || mHeight < 0 || forceDownload)
				DownloadImage(); //Must download the image to determine the width and height;

			if (mWidth == 0 || mHeight == 0) //No meaningful answer for 0-sized image, so return 0
				return 0;

			return (float)Math.Min(mWidth, mHeight) / (float)Math.Max(mWidth, mHeight);
		}

		public Orientation GetImageOrientation(bool forceDownload)
		{
			if (mWidth < 0 || mHeight < 0 || forceDownload)
				DownloadImage(); //Must download the image to determine the width and height;

			if (mWidth == 0 || mHeight == 0) //No meaningful answer for 0-sized image, so return None
				return Orientation.None;

			if (mWidth > mHeight)
			{
				return Orientation.Landscape;
			}
			else if (mHeight > mWidth)
			{
				return Orientation.Portrait;
			}
			else
			{
				return Orientation.Square;
			}
		}

		public bool Save(string pathPattern, int sequence)
		{
			DownloadImage(); //Ensure image is downloaded

			if (mImage == null)
				return false; //No image to save

			Console.WriteLine(); //New line after all the source searching

			//Find the image file format extension
			string extension;
			//Find the codec
			Guid bitmapFormatGuid = mImage.RawFormat.Guid;
			ImageCodecInfo info = ImageCodecInfo.GetImageEncoders().FirstOrDefault(i => i.FormatID == bitmapFormatGuid);
			if (info != null)
			{
				//Use the first filename extension of the codec, with *. removed from it, in lower case
				extension = info.FilenameExtension.Split(';')[0].Substring(2).ToLower();
			}
			else
			{
				System.Diagnostics.Trace.WriteLine("Could not determine image file format for: " + mName);
				//Use .bmp as a general image file format indicator
				extension = "bmp";
			}

			//Construct the file path
			string path = pathPattern.Replace("%name%", Program.MakeSafeForPath(mName))
									 .Replace("%source%", Program.MakeSafeForPath(mScript.Name))
									 .Replace("%size%", String.Format("{0} x {1}", mWidth, mHeight))
									 .Replace("%extension%", extension)
									 .Replace("%sequence%", sequence.ToString());

			path = ReplaceCustomTypeString(path, mCoverType);

			//Ensure path is absolute, if relative
			path = Path.GetFullPath(path);

			try
			{
				DirectoryInfo folder = new DirectoryInfo(Path.GetDirectoryName(path));
				if (!folder.Exists)
					folder.Create();

				//Image.Save has rubbish error reporting, so detect any errors pre-emptively by creating a file
				File.Create(path, 1, FileOptions.DeleteOnClose).Close();

				mImage.Save(path); //If an exception is thrown, let it pass back up.
			}
			catch (Exception)
			{
				Console.WriteLine("Could not save image to: \"{0}\"", path);
				throw; //Let the exception bubble up to be reported as an unexpected faliure
			}

			Console.WriteLine("Saved \"{0}\" from {1} to: \"{2}\"", mName, mScript.Name, path);
			return true;
		}

		private static string ReplaceCustomTypeString(string path, CoverType coverType)
		{
			return Regex.Replace(path, @"%type(?:\((?<names>[^)]*)\))?%",
				new MatchEvaluator(delegate(Match match)
				{
					string name;
					string[] names = match.Groups["names"].Value.Split(',');
					if (names.Length > (int)coverType)
					{
						name = names[(int)coverType];
					}
					else
					{
						//No custom name provided
						name = coverType.ToString();
					}
					return name;
				}),
				RegexOptions.IgnoreCase);
		}
		
		#region Copied from AlbumArtDownloader/BitmapHelpers.cs
		//TODO: Refactor this out into a common library? Or at least shared file?

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
		#endregion
	}
}
