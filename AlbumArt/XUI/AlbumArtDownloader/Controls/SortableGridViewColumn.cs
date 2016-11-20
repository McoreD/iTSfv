//AV (27/05/07):
//This file taken from http://blogs.interknowlogy.com/joelrumerman/archive/2007/04/03/12497.aspx
//(no licensing info was available there)
//AV (05/07/08):
//Added IsVisible property and functionality
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AlbumArtDownloader.Controls //AV (27/05/07): changed namespace to match other controls
{
    public class SortableGridViewColumn : GridViewColumn
    {
		static SortableGridViewColumn()
		{
			WidthProperty.OverrideMetadata(typeof(SortableGridViewColumn), new FrameworkPropertyMetadata(null, new CoerceValueCallback(CoerceWidth)));
		}

        public string SortPropertyName
        {
            get { return (string)GetValue(SortPropertyNameProperty); }
            set { SetValue(SortPropertyNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SortPropertyName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortPropertyNameProperty =
            DependencyProperty.Register("SortPropertyName", typeof(string), typeof(SortableGridViewColumn), new UIPropertyMetadata(""));


        public bool IsDefaultSortColumn
        {
            get { return (bool)GetValue(IsDefaultSortColumnProperty); }
            set { SetValue(IsDefaultSortColumnProperty, value); }
        }

        public static readonly DependencyProperty IsDefaultSortColumnProperty =
            DependencyProperty.Register("IsDefaultSortColumn", typeof(bool), typeof(SortableGridViewColumn), new UIPropertyMetadata(false));


		public static readonly DependencyProperty IsVisibleProperty =
			DependencyProperty.Register("IsVisible", typeof(bool), typeof(SortableGridViewColumn), new UIPropertyMetadata(true, OnIsVisiblePropertyChanged));
		public bool IsVisible
		{
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}

		private static void OnIsVisiblePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			sender.CoerceValue(WidthProperty);
		}

		private static object CoerceWidth(DependencyObject sender, object newValue)
		{
			//If the column is hidden, coerce its width to be zero
			if (!((SortableGridViewColumn)sender).IsVisible)
			{
				return 0D;
			}
			return newValue;
		}
    }
}
