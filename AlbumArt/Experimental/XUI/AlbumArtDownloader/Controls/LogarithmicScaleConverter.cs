using System;
using System.Windows.Data;

namespace AlbumArtDownloader.Controls
{
	/// <summary>
	/// Converts an value into the Log10 of that value.
	/// </summary>
	[ValueConversion(typeof(double), typeof(double))]
	public class LogarithmicScaleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double dValue = (double)value;
			if (dValue == 0)
				return Binding.DoNothing; //No meaningfull answer for 0.

			return Math.Log10(dValue);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return Math.Pow(10, (double)value);
		}
	}
}
