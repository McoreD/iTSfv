using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace AlbumArtDownloader.Controls
{
	internal class ScrollOrigin : Adorner
	{
		private static readonly BitmapSource sScrollOrigin;
		private static readonly Size sImageSize;

		static ScrollOrigin()
		{
			sScrollOrigin = new BitmapImage(new Uri("pack://application:,,,/Controls/scrollorigin.png"));
			sScrollOrigin.Freeze();
			sImageSize = new Size(sScrollOrigin.Width, sScrollOrigin.Height);
		}

		private Point mLocation;
		
		public ScrollOrigin(UIElement adornedElement, Point location) : base(adornedElement)
		{
			mLocation = location;
			//Center the image on that point
			mLocation.Offset(-sImageSize.Width / 2D + 1, -sImageSize.Height / 2D + 1);
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			drawingContext.DrawImage(sScrollOrigin, new Rect(mLocation, sImageSize));
		}
	}
}
