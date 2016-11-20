using System;
using System.Windows;
using System.Windows.Controls;

namespace AlbumArtDownloader.Controls
{
	/// <summary>
	/// An editor for specifying limits, which consists of a checkbox for applying the limit
	/// or not, and a textbox for entering the (positive integer) limit.
	/// </summary>
	public class LimitBox : Control
	{
		static LimitBox()
		{
			//This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
			//This style is defined in themes\generic.xaml
			DefaultStyleKeyProperty.OverrideMetadata(typeof(LimitBox), new FrameworkPropertyMetadata(typeof(LimitBox)));
		}

		public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(LimitBox));
		public string Label
		{
			get { return (string)GetValue(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(LimitBox), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public int Value
		{
			get { return (int)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		public static readonly DependencyProperty HasValueProperty = DependencyProperty.Register("HasValue", typeof(bool), typeof(LimitBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public bool HasValue
		{
			get { return (bool)GetValue(HasValueProperty); }
			set { SetValue(HasValueProperty, HasValue); }
		}
	}
}
