using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace AlbumArtDownloader.Controls
{
	public class DpiIndependentZoomConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (values.Length == 2 && values[0] is double && values[1] is double)
			{

				var zoom = (double)values[0];
				var dpi = (double)values[1];

				return zoom * dpi / 96; //96 is WPF default dpi
			}

			return DependencyProperty.UnsetValue;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			//Reverse conversion not supported
			return null;
		}
	}
}
