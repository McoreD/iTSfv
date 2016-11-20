using System;
using System.IO;
using System.Windows.Data;

namespace AlbumArtDownloader.Controls
{
	/// <summary>
	/// Returns a user-friendly string for filesize in bytes
	/// </summary>
	[ValueConversion(typeof(long), typeof(string))]
	public class FileSizeConverter : IValueConverter
	{
		private static readonly string[] sUnits = new string[] { "B", "KB", "MB", "GB", "TB" };

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is long)
			{
				long result = (long)value;

				//Find the most appropriate unit to display the result in
				int unitIndex = 0;
				while (result > 1024 && unitIndex < sUnits.Length)
				{
					result /= 1024;
					unitIndex++;
				}

				return String.Format("{0} {1}", result, sUnits[unitIndex]);
			}
			//Can only convert longs
			return System.Windows.DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			//Reverse conversion not supported
			return null;
		}
	}
}
