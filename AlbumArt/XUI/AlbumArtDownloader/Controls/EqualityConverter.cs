using System;
using System.Windows;
using System.Windows.Data;

namespace AlbumArtDownloader.Controls
{
	/// <summary>
	/// Checks if the value is equal to the parameter, and returns true if they are.
	/// When set, if the value is true, returns the parameter. Otherwise returns Unset.
	/// </summary>
	public class EqualityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value.Equals(parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if(value is bool && (bool)value)
				return parameter;

			return DependencyProperty.UnsetValue;
		}
	}
}
