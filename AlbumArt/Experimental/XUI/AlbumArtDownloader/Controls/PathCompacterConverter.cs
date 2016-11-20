using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace AlbumArtDownloader.Controls
{

	/// <summary>
	/// Compacts a file path to fit the size available
	/// </summary>
	public class PathCompacterConverter : IMultiValueConverter
	{
		[DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
		static extern bool PathCompactPathEx([Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags);

		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			TextBlock textBlock = (TextBlock)values[0];
			double actualWidth = (double)values[1]; //Included seperately so that updates are triggered when the width changes
			string path = values[2] as string;

			if (actualWidth == 0 || String.IsNullOrEmpty(path))
				return String.Empty;

			StringBuilder result = new StringBuilder(path);
			int maxChars = path.Length; //Start at full length, and shrink to fit
			do
			{
				//Check the size
				double textWidth = Utilities.GetTextWidth(result.ToString(), textBlock, culture);
				if (textWidth <= actualWidth)
				{
					if (maxChars == path.Length || (actualWidth - textWidth < textBlock.FontSize))
					{
						//Close enough.
						return result.ToString();
					}
					else
					{
						//Shrunk too far, so grow back a bit
						maxChars++;
					}
				}
				else
				{
					//Not small enough. Attempt a guess at a better size, by making the string the same proportion smaller as the width is.
					int previousMaxChars = maxChars;
					maxChars = (int)(maxChars / (textWidth / actualWidth));
					if (previousMaxChars - maxChars <= 1)
					{
						//The reduction is less than or equal to one character, so reduce by one and return the result
						maxChars = previousMaxChars - 1;

						PathCompactPathEx(result, path, maxChars, 0);
						return result.ToString();
					}
				}

				PathCompactPathEx(result, path, maxChars, 0);

			} while (maxChars > 0);
			
			return result.ToString();
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			//Reverse conversion not supported
			return null;
		}
	}
}
