using System;
using System.Windows;
using System.Windows.Controls;

namespace AlbumArtDownloader.Controls
{
	/// <summary>
	/// A control which wraps another control (the content), but restrains it
	/// from having a desired size larger than the available size.
	/// </summary>
	public class SizeRestrainer : ContentControl
	{
		static SizeRestrainer()
		{
			//Size restrainers should not be focusable (at least by default)
			FocusableProperty.OverrideMetadata(typeof(SizeRestrainer), new FrameworkPropertyMetadata(false));
		}

		protected override Size MeasureOverride(Size constraint)
		{
			return Size.Empty; //Don't need any size larger than the available size
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			return base.ArrangeOverride(arrangeBounds);
		}
	}
}
