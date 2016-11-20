using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

using AlbumArtDownloader.Scripts;
using System.Windows.Media.Imaging;

namespace AlbumArtDownloader
{
	internal class ScriptResult
	{
		private IScript mScript;
		private object mThumbnail;
		private string mName;
		private int mWidth, mHeight;
		private object mFullSizeImageCallbackParameter;
		private byte[] mImageData;
        private string mExtension;
		private bool mImageDownloaded;
		private CoverType mCoverType;
		
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

                mImageData = BitmapHelpers.GetBitmapData(fullSizeImage);
                if (mImageData == null)
                {
                    //just use the thumbnail image
                    mImageData = BitmapHelpers.GetBitmapData(mThumbnail);
                }

                if (mImageData != null)
                {
                    var decoder = BitmapDecoder.Create(new MemoryStream(mImageData), BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                    var frame = decoder.Frames[0];
                    mWidth = frame.PixelWidth;
                    mHeight = frame.PixelHeight;

                    mExtension = decoder.CodecInfo.FileExtensions.Split(',')[0].Substring(1);

					//Hack for consistency with previous versions - use .jpg by default for jpeg files, not .jpeg
					if ("jpeg".Equals(mExtension, StringComparison.OrdinalIgnoreCase))
					{
						mExtension = "jpg";
					}
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

			if (mImageData == null)
				return false; //No image to save

			Console.WriteLine(); //New line after all the source searching

			//Construct the file path
			string path = pathPattern.Replace("%name%", Program.MakeSafeForPath(mName))
									 .Replace("%source%", Program.MakeSafeForPath(mScript.Name))
									 .Replace("%size%", String.Format("{0} x {1}", mWidth, mHeight))
									 .Replace("%extension%", mExtension)
									 .Replace("%sequence%", sequence.ToString());

			path = ReplaceCustomTypeString(path, mCoverType);

			//Ensure path is absolute, if relative
			path = Path.GetFullPath(path);

			try
			{
				DirectoryInfo folder = new DirectoryInfo(Path.GetDirectoryName(path));
				if (!folder.Exists)
					folder.Create();

				File.WriteAllBytes(path, mImageData);
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
	}
}
