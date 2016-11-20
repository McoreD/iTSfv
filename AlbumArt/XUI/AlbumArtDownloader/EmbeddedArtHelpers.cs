using System;
using System.Globalization;

namespace AlbumArtDownloader
{
	internal static class EmbeddedArtHelpers
	{
		public static bool IsEmbeddedArtPath(string filePath)
		{
			return filePath.EndsWith(">");
		}

		/// <summary>
		/// Gets the non-embedded part and the embed index from an embedded file path. <paramref name="index"/> is
		/// the 0-based index of which embedded image is referred to, or -1 to mean a wildcard representing all embedded
		/// images (represented by * in the embedded file path string)
		/// </summary>
		public static void SplitToFilenameAndIndex(string embeddedFilePath, out string filePath, out int index)
		{
			int pos = embeddedFilePath.LastIndexOf('<');
			if (!IsEmbeddedArtPath(embeddedFilePath) || pos < 0)
			{
				throw new ArgumentException("Not a valid embedded art file path");
			}
			string indexString = embeddedFilePath.Substring(pos + 1, embeddedFilePath.Length - pos - 2);
			if(indexString == "*") //user-friendly representation of -1: more evocative of "any" than "-1" is.
			{
				index = -1;
			}
			else
			{
				index = Int32.Parse(indexString, CultureInfo.InvariantCulture);
			}
			filePath = embeddedFilePath.Substring(0, pos);
		}

		public static string GetEmbeddedFilePath(string filePath, int embedIndex)
		{
			if (IsEmbeddedArtPath(filePath))
			{
				throw new ArgumentException("filePath is already an embedded art path");
			}

			return String.Format(CultureInfo.InvariantCulture, "{0}<{1}>", filePath, embedIndex);
		}

		/// <summary>
		/// Peforms the same function as <see cref="System.IO.Path.GetFileName"/>, but aware of the embedded index suffix
		/// </summary>
		/// <param name="embeddedFilePath"></param>
		/// <returns></returns>
		public static string GetEmbeddedFileName(string embeddedFilePath)
		{
			int pos = embeddedFilePath.LastIndexOf('<');
			if (pos < 0)
			{
				return System.IO.Path.GetFileName(embeddedFilePath);
			}
			else
			{
				return System.IO.Path.GetFileName(embeddedFilePath.Substring(0, pos)) + embeddedFilePath.Substring(pos);
			}
		}

		/// <summary>
		/// Gets the embedded art referred to by <paramref name="embeddedFilePath"/>. Must not contain
		/// wildcards in the path, including the "-1" or "*" 'any embedded image' wildcard.
		/// </summary>
		/// <param name="embeddedFilePath"></param>
		/// <returns></returns>
		public static TagLib.IPicture GetEmbeddedArt(string embeddedFilePath)
		{
			if (!IsEmbeddedArtPath(embeddedFilePath))
			{
				throw new ArgumentException("Not a valid embedded art file path");
			}

			string filePath;
			int index;

			SplitToFilenameAndIndex(embeddedFilePath, out filePath, out index);

			//Read ID3 Tags
			TagLib.File fileTags = null;
			try
			{
				fileTags = TagLib.File.Create(filePath, TagLib.ReadStyle.None);
				
				var embeddedPictures = fileTags.Tag.Pictures;
				if (embeddedPictures.Length > index)
				{
					return embeddedPictures[index];
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.WriteLine("TagLib# could not read embedded artwork for file: " + filePath);
				System.Diagnostics.Trace.Indent();
				System.Diagnostics.Trace.WriteLine(e.Message);
				System.Diagnostics.Trace.Unindent();
				return null; //If this media file couldn't be read, just go on to the next one.				
			}
			finally
			{
				if (fileTags != null)
				{
					fileTags.Mode = TagLib.File.AccessMode.Closed;
				}
			}

			System.Diagnostics.Trace.WriteLine(String.Format("Embedded artwork not found at index [{0}] of file: {1}", index, filePath));
			return null;
		}

		/// <summary>
		/// Gets the index of the front cover embedded art image, if there is one.
		/// </summary>
		public static int? GetEmbeddedFrontCoverIndex(TagLib.File fileTags)
		{
			var embeddedPictures = fileTags.Tag.Pictures;
			if (embeddedPictures.Length > 0)
			{
				//There's an embedded picture
				//Check to see if there's a picture described as the front cover, to use in preference
				for (int i = 0; i < embeddedPictures.Length; i++)
				{
					if (embeddedPictures[i].Type == TagLib.PictureType.FrontCover)
					{
						return i;
					}
				}
				//None of the embedded pictures were tagged as "FrontCover", so just use the first picture
				return 0;
			}

			return null;
		}
	}
}
