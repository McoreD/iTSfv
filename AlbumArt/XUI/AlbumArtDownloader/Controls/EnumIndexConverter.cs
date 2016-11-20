using System;
using System.Windows;
using System.Windows.Data;

namespace AlbumArtDownloader.Controls
{
	/// <summary>
	/// Casts the value of an enum to its index
	/// </summary>
	public class EnumIndexConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			System.Diagnostics.Debug.Assert(targetType == typeof(int), "Expecting to convert from enum to an int");
			return (int)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			System.Diagnostics.Debug.Assert(value is int, "Expecting to convert from int to enum");
			return Enum.ToObject(targetType, (int)value);
		}
	}
}
