using System;
using System.Windows;
using System.Windows.Input;

namespace AlbumArtDownloader
{
	/// <summary>
	/// An attached property that can be used to enable full window drag behaviour.
	/// To use, set FullWindowDrag.IsDraggable="True" on a window.
	/// </summary>
	public class FullWindowDrag
	{
		public static readonly DependencyProperty IsDraggableProperty
		   = DependencyProperty.RegisterAttached("IsDraggable", typeof(bool), typeof(FullWindowDrag),
				new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsDraggableChanged)));

		public static void SetIsDraggable(DependencyObject dependencyObject, bool enabled)
		{
			dependencyObject.SetValue(IsDraggableProperty, enabled);
		}

		private static void OnIsDraggableChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			Window window = dependencyObject as Window;
			if (window != null)
			{
				if ((bool)e.NewValue)
				{
					//Attach behaviour to the window
					new FullWindowDrag(window);
				}
				else
				{
					//TODO: Does this need to ever be detached?
					System.Diagnostics.Debug.Fail("Detaching draggable behaviour not implemented");
				}
			}
		}

		#region Attached behaviour
		private Window mAttachedWindow;
		private Point? mMouseDown;

		public FullWindowDrag(Window window)
		{
			mAttachedWindow = window;
			mAttachedWindow.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
			mAttachedWindow.MouseMove += new MouseEventHandler(OnMouseMove);
			mAttachedWindow.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(OnPreviewMouseLeftButtonUp);
		}

		private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			mMouseDown = e.GetPosition(mAttachedWindow);
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && mMouseDown.HasValue)
			{
				Vector offset = e.GetPosition(mAttachedWindow) - mMouseDown.Value;
				mAttachedWindow.Left += offset.X;
				mAttachedWindow.Top += offset.Y;
			}
		}

		private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			mMouseDown = null;
		}
		#endregion
	}
}
