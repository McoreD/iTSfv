using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections.Generic;

namespace AlbumArtDownloader.Controls
{
	/// <summary>
	/// Converter to determine whether or not to show a tooltip, based on whether it overflows or not.
	/// Usage (in the style of a TextBlock):
	/// <code>
	/// <Setter Property="TextTrimming" Value="CharacterEllipsis"/>
	/// <Setter Property="ToolTip" Value="{Binding RelativeSource={RelativeSource Self}, Path=Text}"/>
	/// <Setter Property="ToolTipService.IsEnabled">
	/// 	<Setter.Value>
	/// 		<MultiBinding Converter="{StaticResource mTooltipEnabledOnOverflowConverter}" Mode="OneWay">
	/// 			<Binding RelativeSource="{RelativeSource Self}" Mode="OneWay"/>
	/// 			<Binding Path="ActualWidth" RelativeSource="{RelativeSource Self}" Mode="OneWay"/>
	/// 			<Binding Path="ToolTip" RelativeSource="{RelativeSource Self}" Mode="OneWay"/>
	/// 		</MultiBinding>
	/// 	</Setter.Value>
	/// </Setter>
	/// </code>
	/// </summary>
	public class TooltipEnabledOnOverflowConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (values.Length == 3 && values[0] is TextBlock && values[1] is double && values[2] is string)
			{
				TextBlock textBlock = (TextBlock)values[0];
				double actualWidth = (double)values[1];
				string text = (string)values[2];

				//Check the size
				return Utilities.GetTextWidth(text, textBlock, culture) > actualWidth;
			}
			return false;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			//Reverse conversion not supported
			return null;
		}
	}
}
