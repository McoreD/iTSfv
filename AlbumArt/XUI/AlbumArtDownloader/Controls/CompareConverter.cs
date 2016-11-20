using System;
using System.Windows.Data;

namespace AlbumArtDownloader.Controls
{
	/// <summary>
	/// Compares the values (of which there must be exactly two), and returns the result.
	/// </summary>
	internal class CompareConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (values.Length == 2 && values[0] is IComparable)
			{
				return ((IComparable)values[0]).CompareTo(values[1]);
			}
			return null;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			//Reverse conversion not supported
			return null;
		}
	}
}
