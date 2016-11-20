using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace AlbumArtDownloader.Controls
{
	[TemplatePart(Name = "PART_SortField", Type = typeof(ComboBox))]
	[TemplatePart(Name = "PART_SortDirection", Type = typeof(ToggleButton))]
	public class ArtPanelListSortPicker : Control
	{
		private static readonly SortFieldItem[] sSortFields = new SortFieldItem[] { 
												new SortFieldItem("None", ""), 
												new SortFieldItem("Name", "ResultName"), 
												new SortFieldItem("Size", "ImageWidth"), 
												new SortFieldItem("Area", "ImageArea"), 
												new SortFieldItem("Source", "SourceName"),
												new SortFieldItem("Type", "CoverType")};

		static ArtPanelListSortPicker()
		{
			//This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
			//This style is defined in themes\generic.xaml
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ArtPanelListSortPicker), new FrameworkPropertyMetadata(typeof(ArtPanelListSortPicker)));
		}

		/// <summary>
		/// Used to prevent the source description being updated when the UI is being set from code.
		/// </summary>
		private bool mSuspendUpdateSortDescription = false;

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			
			if (SortField != null)
			{
				SortField.ItemsSource = sSortFields;
				SortField.SelectionChanged += UpdateToSortDescription;
			}
			if (SortDirection != null)
			{
				SortDirection.Checked += UpdateToSortDescription;
				SortDirection.Unchecked += UpdateToSortDescription;
			}

			UpdateFromSortDescription();
		}

		private void UpdateToSortDescription(object sender, EventArgs e)
		{
			if (!mSuspendUpdateSortDescription)
			{
				SortDescription = new SortDescription(
					((SortFieldItem)SortField.SelectedItem).FieldName,
					SortDirection.IsChecked.GetValueOrDefault() ? ListSortDirection.Descending : ListSortDirection.Ascending);
			}
		}

		public static readonly DependencyProperty SortDescriptionProperty = DependencyProperty.Register("SortDescription", typeof(SortDescription), typeof(ArtPanelListSortPicker), 
					new FrameworkPropertyMetadata(new SortDescription("ResultName", ListSortDirection.Descending),
					FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					new PropertyChangedCallback(OnSortDescriptionChanged)));
		public SortDescription SortDescription
		{
			get { return (SortDescription)GetValue(SortDescriptionProperty); }
			set { SetValue(SortDescriptionProperty, value); }
		}
		private static void OnSortDescriptionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			((ArtPanelListSortPicker)sender).UpdateFromSortDescription();
		}

		private void UpdateFromSortDescription()
		{
			try
			{
				mSuspendUpdateSortDescription = true;

				if (SortField != null)
				{
					//Find the index of the sort field, or 0 if not found
					string fieldName = SortDescription.PropertyName;
					int index = Math.Max(0, Array.FindIndex<SortFieldItem>(sSortFields, new Predicate<SortFieldItem>(delegate(SortFieldItem item) { return item.FieldName == fieldName; })));
					SortField.SelectedIndex = index;
					if(index == 0)
					{
						SortDirection.Visibility = Visibility.Hidden;
					}
					else
					{
						SortDirection.Visibility = Visibility.Visible;
						SortDirection.IsChecked = SortDescription.Direction == ListSortDirection.Descending;
					}
				}
			}
			finally
			{
				mSuspendUpdateSortDescription = false;
			}
		}

		#region Elements
		private ComboBox mCachedSortField;
		protected ComboBox SortField
		{
			get
			{
				if (mCachedSortField == null)
				{
					if (Template != null)
					{
						mCachedSortField = Template.FindName("PART_SortField", this) as ComboBox;
					}
				}

				return mCachedSortField;
			}
		}
		private ToggleButton mCachedSortDirection;
		protected ToggleButton SortDirection
		{
			get
			{
				if (mCachedSortDirection == null)
				{
					if (Template != null)
					{
						mCachedSortDirection = Template.FindName("PART_SortDirection", this) as ToggleButton;
					}
				}

				return mCachedSortDirection;
			}
		}
		#endregion

		private struct SortFieldItem
		{
			private string mDisplayName;
			private string mFieldName;

			public SortFieldItem(string displayName, string fieldName)
			{
				mDisplayName = displayName;
				mFieldName = fieldName;
			}

			public string DisplayName
			{
				get { return mDisplayName; }
			}
			public string FieldName
			{
				get { return mFieldName; }
			}

			public override string ToString()
			{
				return DisplayName;
			}
		}
	}
}
