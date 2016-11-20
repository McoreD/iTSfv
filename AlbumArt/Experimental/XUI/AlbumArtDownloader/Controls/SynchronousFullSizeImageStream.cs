using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace AlbumArtDownloader.Controls
{
	/// <summary>
	/// Stream that will read the full size image from an AlbumArt, but block until the full size image is available.
	/// </summary>
	internal class SynchronousFullSizeImageStream : Stream
	{
		private AlbumArt mAlbumArt;
		private AutoResetEvent mWaitForImage = new AutoResetEvent(false);
		private Stream mImageData;

		public SynchronousFullSizeImageStream(AlbumArt albumArt)
		{
			mAlbumArt = albumArt;
			mAlbumArt.RetrieveFullSizeImage(mWaitForImage);
		}
		
		private void WaitForImageData()
		{
			if (mImageData == null)
			{
				mWaitForImage.WaitOne(); //Wait for the full size image to download
				if (mImageData == null) //Something else might have already populated image data while waiting
				{
					mImageData = mAlbumArt.GetImageData();
				}
				mWaitForImage.Set(); //If there are any other waiting threads, signal them to continue too.
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			WaitForImageData();
			return mImageData.Read(buffer, offset, count);
		}

		public override long Length
		{
			get
			{
				WaitForImageData();
				return mImageData.Length;
			}
		}

		#region Stubs

		public override bool CanRead
		{
			get 
			{
				if (mImageData == null) return true;

				return mImageData.CanRead; 
			}
		}

		public override bool CanSeek
		{
			get 
			{
				if (mImageData == null) return false;

				return mImageData.CanSeek; 
			}
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override void Flush()
		{
			if (mImageData == null) throw new InvalidOperationException();

			mImageData.Flush();
		}

		public override long Position
		{
			get
			{
				if (mImageData == null) return 0;

				return mImageData.Position;
			}
			set
			{
				if (value != 0 && mImageData == null) throw new InvalidOperationException();

				mImageData.Position = value;
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (mImageData == null) throw new InvalidOperationException();

			return mImageData.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			if (mImageData == null) throw new InvalidOperationException();

			mImageData.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (mImageData == null) throw new InvalidOperationException();

			mImageData.Write(buffer, offset, count);
		}
		#endregion
	}
}
