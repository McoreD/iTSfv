using System;
using System.Windows.Data;

namespace AlbumArtDownloader.Controls
{
	/// <summary>
	/// Sums the values and the converter parameter (All should be of type Double)
	/// </summary>
	internal class SumConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double result = 0d;
			if(parameter is double)
				result += (double)parameter;

			foreach(object value in values)
				if(value is double)
					result += (double)value;

			return result;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			//Reverse conversion not supported
			return null;
		}
	}
}
