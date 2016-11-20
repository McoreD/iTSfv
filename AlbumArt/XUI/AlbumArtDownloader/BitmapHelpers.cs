using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;

namespace AlbumArtDownloader
{
	internal static class BitmapHelpers
	{
		/// <summary>
		/// Synchronously downloads bitmap data from an object which may be any of:
		/// <para>Uri</para>
		/// <para>String (containing a url)</para>
		/// <para>Stream (will be disposed by this method)</para>
        /// NOTE: Not a bitmap, any more. That functionality has been removed as the original source data of the bitmap would not be available.
		/// </summary>
		/// <param name="from"></param>
		/// <returns></returns>
		public static byte[] GetBitmapData(object from)
		{
            if (from == null)
            {
                return null;
            }

            if (from.GetType().FullName == "System.Drawing.Bitmap")
            {
                throw new NotSupportedException("Bitmaps can not be used directly, pass the bitmap source data instead");
            }

            try
            {
			    Stream stream = null;
                long length = -1;
			    if (from is Stream)
			    {
				    stream = (Stream)from;
                    try
                    {
                        length = stream.Length;
                    }
                    catch (NotSupportedException)
                    { 
                    }
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
                        var response = request.GetResponse();
					    stream = response.GetResponseStream();

                        try
                        {
                            length = response.ContentLength;
                        }
                        catch (NotSupportedException)
                        {
                        }
				    }
			    }
			    if (stream != null)
			    {
				    // Copy the stream data and return it
                    var bytes = ReadFully(stream, length);
                    stream.Dispose();
                    return bytes;
			    }
			}
			catch (Exception)
			{
				System.Diagnostics.Trace.Write("Could not get a bitmap data for: ");
				System.Diagnostics.Trace.WriteLine(from);
			}

			return null;
		}

        //Taken from http://www.yoda.arachsys.com/csharp/readbinary.html
        /// <summary>
        /// Reads data from a stream until the end is reached. The
        /// data is returned as a byte array. An IOException is
        /// thrown if any of the underlying IO calls fail.
        /// </summary>
        /// <param name="stream">The stream to read data from</param>
        /// <param name="initialLength">The initial buffer length</param>
        private static byte[] ReadFully(Stream stream, long initialLength)
        {
            // If we've been passed an unhelpful initial length, just
            // use 32K.
            if (initialLength < 1)
            {
                initialLength = 32768;
            }

            byte[] buffer = new byte[initialLength];
            int read = 0;

            int chunk;
            while ((chunk = stream.Read(buffer, read, buffer.Length - read)) > 0)
            {
                read += chunk;

                // If we've reached the end of our buffer, check to see if there's
                // any more information
                if (read == buffer.Length)
                {
                    int nextByte = stream.ReadByte();

                    // End of stream? If so, we're done
                    if (nextByte == -1)
                    {
                        return buffer;
                    }

                    // Nope. Resize the buffer, put in the byte we've just
                    // read, and continue
                    byte[] newBuffer = new byte[buffer.Length * 2];
                    Array.Copy(buffer, newBuffer, buffer.Length);
                    newBuffer[read] = (byte)nextByte;
                    buffer = newBuffer;
                    read++;
                }
            }
            // Buffer is now too big. Shrink it.
            byte[] ret = new byte[read];
            Array.Copy(buffer, ret, read);
            return ret;
        }
	}
}
