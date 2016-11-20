using System;
using System.Windows.Data;

namespace AlbumArtDownloader.Controls
{
	/// <summary>
	/// Formats the values according to the string formatter specified as the parameter
	/// </summary>
	public class StringFormatterConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return String.Format((string)parameter, values);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			//Reverse conversion not supported
			return null;
		}
	}
}
