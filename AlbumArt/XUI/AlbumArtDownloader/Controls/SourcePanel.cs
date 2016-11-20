using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AlbumArtDownloader.Controls
{
	[TemplatePart(Name = "PART_Options", Type = typeof(FrameworkElement))]
	public class SourcePanel : Control
	{
		static SourcePanel()
		{
			//This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
			//This style is defined in themes\generic.xaml
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SourcePanel), new FrameworkPropertyMetadata(typeof(SourcePanel)));
		}

		public SourcePanel()
		{
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Stop, new ExecutedRoutedEventHandler(StopCommandHandler)));
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (Options != null)
			{
				Options.IsVisibleChanged += BringIntoViewOnVisible;
			}
		}

		private void BringIntoViewOnVisible(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue == true)
				((FrameworkElement)sender).BringIntoView();
		}

		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Source), typeof(SourcePanel));
		public Source Source
		{
			get { return (Source)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		private void StopCommandHandler(object sender, ExecutedRoutedEventArgs e)
		{
			Source.AbortSearch();
		}

		protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Space)
			{
				Source.IsEnabled = !Source.IsEnabled;
			}
			base.OnKeyDown(e);
		}

		protected override void OnPreviewMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
		{
			base.OnPreviewMouseLeftButtonUp(e);
			if (e.OriginalSource is TextBlock) 
			{
				Focus();
			}
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonUp(e);

			Source.RaiseHighlightResults();
		}

		#region Elements
		private FrameworkElement mCachedOptions;
		protected FrameworkElement Options
		{
			get
			{
				if (mCachedOptions == null)
				{
					if (Template != null)
					{
						mCachedOptions = Template.FindName("PART_Options", this) as FrameworkElement;
					}
				}

				return mCachedOptions;
			}
		}
		#endregion
	}
}
