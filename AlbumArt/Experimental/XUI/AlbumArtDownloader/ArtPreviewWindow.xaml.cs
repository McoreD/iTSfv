using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AlbumArtDownloader.Controls;

namespace AlbumArtDownloader
{
	public partial class ArtPreviewWindow : System.Windows.Window, IAppWindow
	{
		/// <summary>This value is how close to 1:1 the zoom has to be before being snapped to 1:1</summary>
		private static readonly double sZoomSnapping = 1.2;
		/// <summary>The factor by which the zoom is changed by the zoom in or out buttons</summary>
		private static readonly double sZoomButtonFactor = 1.5; //NOTE: Must be greater than sZoomSnapping
		/// <summary>The factor by which the zoom is changed by the mouse wheel with Ctrl held down</summary>
		private static readonly double sZoomWheelFactor = 1.3; //NOTE: Must be greater than sZoomSnapping
		
		public ArtPreviewWindow()
		{
			InitializeComponent();

			CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, new ExecutedRoutedEventHandler(CopyExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, new ExecutedRoutedEventHandler(SaveExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, new ExecutedRoutedEventHandler(SaveAsExec)));
			CommandBindings.Add(new CommandBinding(NavigationCommands.IncreaseZoom, new ExecutedRoutedEventHandler(IncreaseZoomExec)));
			CommandBindings.Add(new CommandBinding(NavigationCommands.DecreaseZoom, new ExecutedRoutedEventHandler(DecreaseZoomExec)));

			mImageScroller.PreviewMouseWheel += new MouseWheelEventHandler(OnMouseWheel);
			mImageScroller.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnMouseDown);
			mImageScroller.MouseMove += new MouseEventHandler(OnMouseMove);
			mImageScroller.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(OnMouseUp);

			mFilePathDisplay.TextInput += new TextCompositionEventHandler(FilePathDisplay_TextInput);
			mFilePathDisplay.MouseLeftButtonDown += new MouseButtonEventHandler(FilePathDisplay_MouseLeftButtonDown);
			mFilePathTextBox.PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(FilePathTextBox_LostKeyboardFocus);
			mFilePathBrowse.PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(FilePathBrowse_LostKeyboardFocus);
			mFilePathTextBox.KeyDown += new KeyEventHandler(FilePathTextBox_KeyDown);
			mFilePathBrowse.Click += new RoutedEventHandler(FilePathBrowse_Click);

			Properties.Settings.Default.PropertyChanged += OnPropertyChanged;
		}

		protected override void OnClosed(EventArgs e)
		{
			Properties.Settings.Default.PropertyChanged -= OnPropertyChanged;
			base.OnClosed(e);
		}

		#region Drag panning
		private Point? mPreviousMousePosition;
		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			mImageScroller.Focus();
			if (e.OriginalSource is Image)
			{
				mPreviousMousePosition = e.GetPosition(mImageScroller);
				mImageScroller.CaptureMouse();
				e.Handled = true;
			}
		}
		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && mPreviousMousePosition.HasValue)
			{
				Point mousePosition = e.GetPosition(mImageScroller);
				Vector offset = mousePosition - mPreviousMousePosition.Value;
				mPreviousMousePosition = mousePosition;

				mImageScroller.ScrollToHorizontalOffset(mImageScroller.HorizontalOffset - offset.X);
				mImageScroller.ScrollToVerticalOffset(mImageScroller.VerticalOffset - offset.Y);
			}
		}
		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			mPreviousMousePosition = null;
			mImageScroller.ReleaseMouseCapture();
		}
		#endregion

		#region Command Handlers
		private void CopyExec(object sender, ExecutedRoutedEventArgs e)
		{
			AlbumArt albumArt = (AlbumArt)AlbumArt;
			if (albumArt != null)
			{
				albumArt.CopyToClipboard();
			}
		}
		private void SaveExec(object sender, ExecutedRoutedEventArgs e)
		{
			AlbumArt albumArt = (AlbumArt)AlbumArt;
			if (albumArt != null)
			{
				string preset = e.Parameter as String;
				if (preset != null)
				{
					albumArt.Preset = preset;
				}
				else if (Properties.Settings.Default.Presets.Length > 0)
				{
					albumArt.Preset = Properties.Settings.Default.Presets[0].Value;
				}
				else
				{
					albumArt.Preset = null;
				}

				albumArt.PropertyChanged += AutoCloseOnSave;

				albumArt.Save();
			}
		}
		private void SaveAsExec(object sender, ExecutedRoutedEventArgs e)
		{
			AlbumArt albumArt = (AlbumArt)AlbumArt;
			if (albumArt != null)
			{
				albumArt.PropertyChanged += AutoCloseOnSave; //No auto-close for SaveAs operation.
				albumArt.SaveAs();
			}
		}
		private void AutoCloseOnSave(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsSaved" && ((AlbumArt)sender).IsSaved)
			{
				Close();
			}
		}

		private void IncreaseZoomExec(object sender, ExecutedRoutedEventArgs e)
		{
			Zoom *= sZoomButtonFactor;
		}
		private void DecreaseZoomExec(object sender, ExecutedRoutedEventArgs e)
		{
			Zoom /= sZoomButtonFactor;
		}
		#endregion

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ShowPixelsWhenZoomed")
			{
				UpdateBitmapScalingMode();
			}
		}

		private void UpdateBitmapScalingMode()
		{
			if (!App.UsePreSP1Compatibility) //NearestNeighbor scaling was introduced with SP1
			{
				if (Zoom <= 1D || !Properties.Settings.Default.ShowPixelsWhenZoomed)
				{
					//Zoomed out, or opted out of accurate pixel display - use default high quality resizing
					RenderOptions.SetBitmapScalingMode(mImageScroller, BitmapScalingMode.Unspecified);
				}
				else
				{
					//Zoomed in, use accurate pixel resizing
					RenderOptions.SetBitmapScalingMode(mImageScroller, BitmapScalingMode.NearestNeighbor);
				}
			}
		}


		private void OnMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
			{
				//Zoom
				if(e.Delta > 0)
					Zoom *= sZoomWheelFactor;
				else if(e.Delta < 0)
					Zoom /= sZoomWheelFactor;

				e.Handled = true;
			}
			else if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
			{
				//The handling for vertical scrolling is to scroll 3 lines up or down.
				//Do the equivalent for left and right here.
				if (e.Delta > 0)
				{
					mImageScroller.LineLeft();
					mImageScroller.LineLeft();
					mImageScroller.LineLeft();
				}
				else if (e.Delta < 0)
				{
					mImageScroller.LineRight();
					mImageScroller.LineRight();
					mImageScroller.LineRight();
				}

				e.Handled = true;
			}
		}

		#region Properties
		public static readonly DependencyProperty AlbumArtProperty = DependencyProperty.Register("AlbumArt", typeof(AlbumArt), typeof(ArtPreviewWindow));
		/// <summary>The AlbumArt to preview</summary>
		public AlbumArt AlbumArt
		{
			get { return (AlbumArt)GetValue(AlbumArtProperty); }
			set { SetValue(AlbumArtProperty, value); }
		}

		public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register("Zoom", typeof(double), typeof(ArtPreviewWindow), new FrameworkPropertyMetadata(1D, new PropertyChangedCallback(OnZoomChanged), new CoerceValueCallback(CoerceZoom)));
		/// <summary>The Zoom to preview</summary>
		public double Zoom
		{
			get { return (double)GetValue(ZoomProperty); }
			set { SetValue(ZoomProperty, value); }
		}
		private static object CoerceZoom(DependencyObject sender, object baseValue)
		{
			//Snap to zoom 1:1, if within sZoomSnapping
			double zoomRatio = (double)baseValue;
			//Do the equivalent of Math.Abs, for a ratio
			if (zoomRatio < 1)
				zoomRatio = 1 / zoomRatio;

			if (zoomRatio < sZoomSnapping)
				return 1D;

			return baseValue;
		}
		private static void OnZoomChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var artPreviewWindow = (ArtPreviewWindow)sender;
			ScrollViewer scrollViewer = artPreviewWindow.mImageScroller;

			double deltaZoom = (double)e.NewValue / (double)e.OldValue;
	
			double halfViewportWidth = scrollViewer.ViewportWidth / 2;
			scrollViewer.ScrollToHorizontalOffset((scrollViewer.HorizontalOffset + halfViewportWidth) * deltaZoom - halfViewportWidth);

			double halfViewportHeight = scrollViewer.ViewportHeight / 2;
			scrollViewer.ScrollToVerticalOffset((scrollViewer.VerticalOffset + halfViewportHeight) * deltaZoom - halfViewportHeight);

			artPreviewWindow.UpdateBitmapScalingMode();
		}

		public static readonly DependencyProperty PresetsContextMenuProperty = ArtPanel.PresetsContextMenuProperty.AddOwner(typeof(ArtPreviewWindow));
		/// <summary>The menu to display when the Save button dropper is clicked</summary>
		public ContextMenu PresetsContextMenu
		{
			get { return (ContextMenu)GetValue(PresetsContextMenuProperty); }
			set { SetValue(PresetsContextMenuProperty, value); }
		}
		#endregion

		private void ZoomToFit(object sender, EventArgs e)
		{
			if (AlbumArt != null && AlbumArt.Image != null && !Double.IsNaN(AlbumArt.Image.Width) && !Double.IsNaN(AlbumArt.Image.Height))
			{
				double fitHorizontal = mImageScroller.ViewportWidth / AlbumArt.Image.Width;
				double fitVertical = mImageScroller.ViewportHeight / AlbumArt.Image.Height;
				Zoom = Math.Min(fitHorizontal, fitVertical);
			}
		}

		#region File path editing
		
		private void FilePathDisplay_TextInput(object sender, TextCompositionEventArgs e)
		{
			ShowFilePathEdit();

			if (e.Text != " " && e.Text != "\r")
			{
				mFilePathTextBox.Text = e.Text;
				mFilePathTextBox.CaretIndex = e.Text.Length;
			}
			e.Handled = true;
		}

		private void FilePathDisplay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			ShowFilePathEdit();
		}
		private void FilePathTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (e.NewFocus != null && e.NewFocus == mFilePathBrowse)
			{
				//The only other thing that can be focused without closing the editor
				return;
			}
			HideFilePathEdit(true);
		}
		private void FilePathBrowse_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (e.NewFocus != null && e.NewFocus == mFilePathTextBox)
			{
				//The only other thing that can be focused without closing the editor
				return;
			}
			HideFilePathEdit(true);
		}
		private void FilePathTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				//Cancel the edit
				HideFilePathEdit(false);
				e.Handled = true;
			}
			else if (e.Key == Key.Enter)
			{
				//Confirm the edit
				HideFilePathEdit(true);
				e.Handled = true;
			}
		}

		private void ShowFilePathEdit()
		{
			mFilePathDisplay.Visibility = Visibility.Hidden;
			mFilePathEditor.Visibility = Visibility.Visible;
			mFilePathTextBox.Text = AlbumArt.FilePath;
			mFilePathTextBox.SelectAll();
			mFilePathTextBox.Focus();
		}

		private void HideFilePathEdit(bool confirm)
		{
			if (confirm)
				AlbumArt.FilePath = mFilePathTextBox.Text;

			mFilePathDisplay.Visibility = Visibility.Visible;
			mFilePathEditor.Visibility = Visibility.Hidden;
		}

		private void FilePathBrowse_Click(object sender, RoutedEventArgs e)
		{
			HideFilePathEdit(true);
			ApplicationCommands.SaveAs.Execute(null, this);
		}
		#endregion


		#region IAppWindow Members
		//No settings to load
		void IAppWindow.SaveSettings() {}
		void IAppWindow.LoadSettings() {}

		string IAppWindow.Description
		{
			get 
			{
				string description = "Preview";
				if(AlbumArt != null)
					description += ": " + AlbumArt.ResultName;

				return description;
			}
		}
		#endregion
	}
}