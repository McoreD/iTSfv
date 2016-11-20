using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AlbumArtDownloader
{
	[Serializable]
	public class Preset : DependencyObject
	{
		public Preset()
		{
		}

		public Preset(string name, string value)
		{
			Name = name;
			Value = value;
		}

		#region Name
		[NonSerialized]
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Preset));
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		#endregion

		#region Value
		[NonSerialized]
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(Preset));

		public string Value
		{
			get { return (string)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		#endregion
	}
}
