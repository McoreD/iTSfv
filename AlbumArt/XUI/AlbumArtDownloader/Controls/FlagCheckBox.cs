using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;

namespace AlbumArtDownloader.Controls
{
	#region Concrete Implementations (XAML can't handle generics)
	public class FlagCheckBoxAllowedCoverType : FlagCheckBox<AllowedCoverType>
	{
	}
	#endregion

	/// <summary>
	/// A checkbox which has a flag of an enum associated with it.
	/// </summary>
	public class FlagCheckBox<TEnum> : CheckBox
		where TEnum : struct
	{
		#region Flag
		public static readonly DependencyProperty FlagProperty = DependencyProperty.Register("Flag", typeof(TEnum), typeof(FlagCheckBox<TEnum>), new FrameworkPropertyMetadata(default(TEnum), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, UpdateCheckState));

		/// <summary>
		/// The flag (of the enum) that this checkbox is associated with.
		/// </summary>
		public TEnum Flag
		{
			get { return (TEnum)GetValue(FlagProperty); }
			set { SetValue(FlagProperty, value); }
		}

		private int FlagInternal
		{
			get { return Convert.ToInt32(Flag); }
			set { Flag = (TEnum)Enum.ToObject(typeof(TEnum), value); }
		}
		#endregion

		#region Value
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(TEnum), typeof(FlagCheckBox<TEnum>), new FrameworkPropertyMetadata(default(TEnum), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, UpdateCheckState));

		/// <summary>
		/// The current value of the enum property this checkbox is reflecting.
		/// </summary>
		public TEnum Value
		{
			get { return (TEnum)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		private int ValueInternal
		{
			get { return Convert.ToInt32(Value); }
			set { Value = (TEnum)Enum.ToObject(typeof(TEnum), value); }
		}
		#endregion

		private static void UpdateCheckState(object sender, DependencyPropertyChangedEventArgs e)
		{
			var flagCheckBox = (FlagCheckBox<TEnum>)sender;
			flagCheckBox.IsChecked = GetCheckState(flagCheckBox.FlagInternal, flagCheckBox.ValueInternal);
		}

		private static bool? GetCheckState(int flag, int value)
		{
			if (value == 0)
			{
				//Value contains no flags, so checkbox is unchecked
				return false;
			}
			else if ((flag & value) == flag)
			{
				//Value contains flag, so checkbox is checked
				return true;
			}
			else if ((value & flag) == value)
			{
				//Flag contains value, so checkbox is partial (indeterminate)
				return null;
			}
			else
			{
				//Flag and Value do not overlap at all, checkbox is unchecked
				return false;
			}
		}

		protected override void OnUnchecked(RoutedEventArgs e)
		{
			UpdateValueFromCheckState();
			base.OnUnchecked(e);
		}

		protected override void OnChecked(RoutedEventArgs e)
		{
			UpdateValueFromCheckState();
			base.OnChecked(e);
		}

		protected override void OnIndeterminate(RoutedEventArgs e)
		{
			UpdateValueFromCheckState();
			base.OnIndeterminate(e);
		}

		private void UpdateValueFromCheckState()
		{
			if (IsChecked.HasValue) //If the checkbox is indeterminate, then can't set the value at all
			{
				if (IsChecked.Value) //Checkbox is checked, so set the flag in the value
				{
					ValueInternal |= FlagInternal;
				}
				else //Checkbox is unchecked, so clear the flag in the value
				{
					ValueInternal &= ~FlagInternal;
				}
			}
		}
	}
}
