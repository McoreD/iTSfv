using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace AlbumArtDownloader.Controls
{
	public class HighlightResultAdorner : Adorner
	{
		public HighlightResultAdorner(UIElement adornedElement)
			: base(adornedElement)
		{ }

		protected override void OnRender(DrawingContext drawingContext)
		{
			drawingContext.DrawRoundedRectangle(null, new Pen(SystemColors.HighlightBrush, 3),
				new Rect(2, 2, ActualWidth - 7, ActualHeight - 4), 3, 3);
		}
	}
}
