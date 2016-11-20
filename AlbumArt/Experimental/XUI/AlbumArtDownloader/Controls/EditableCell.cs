using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace AlbumArtDownloader.Controls
{
	public class EditableCell : Control
	{
		public static class Commands
		{
			public static RoutedUICommand Edit = new RoutedUICommand("Edit", "Edit", typeof(Commands), new InputGestureCollection() { new KeyGesture(Key.F2) });
		}

		private ListViewItem mParentListViewItem;
		private Editor mEditor;

		static EditableCell()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(EditableCell),
				new FrameworkPropertyMetadata(typeof(EditableCell)));
		}

		public EditableCell()
		{
			CommandBindings.Add(new CommandBinding(Commands.Edit, EditExec));
		}

		private void EditExec(object sender, ExecutedRoutedEventArgs e)
		{
			IsEditing = true;
		}
		
		private bool mEnterEditOnMouseUp = false;
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			if (mParentListViewItem != null && mParentListViewItem.IsSelected)
			{
				mEnterEditOnMouseUp = true;
			}
		}
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			mEnterEditOnMouseUp = false;
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (mEnterEditOnMouseUp)
			{
				mEnterEditOnMouseUp = false;

				IsEditing = true;
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			mParentListViewItem = GetParentListViewItem();
		}

		private ListViewItem GetParentListViewItem()
		{
			//TODO: Is there a better way of doing this=
			DependencyObject parent = this;
			while (parent != null)
			{
				if (parent is ListViewItem)
				{
					return (ListViewItem)parent;
				}
				else
				{
					parent = VisualTreeHelper.GetParent(parent);
				}
			}

			return null;
		}

		protected override Size MeasureOverride(Size constraint)
		{
			Size baseResult = base.MeasureOverride(constraint);
			Size result = constraint; //Take the space offered
			//But if Infinite space is offered, take just the base size
			if (Double.IsPositiveInfinity(result.Width))
			{
				result.Width = baseResult.Width;
			}
			if (Double.IsPositiveInfinity(result.Height))
			{
				result.Height = baseResult.Height;
			}
			return result;
		}

		#region Value
		public static readonly DependencyProperty ValueProperty =
				DependencyProperty.Register(
						"Value",
						typeof(object),
						typeof(EditableCell),
						new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		/// <summary>
		/// Gets or sets the value of the EditableCell
		/// </summary>
		public object Value
		{
			get { return GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		#endregion

		#region IsEditing
		public event DependencyPropertyChangedEventHandler IsEditingChanged;
		
		public static DependencyProperty IsEditingProperty =
				DependencyProperty.Register(
						"IsEditing",
						typeof(bool),
						typeof(EditableCell),
						new FrameworkPropertyMetadata(false, OnIsEditingChanged));

		/// <summary>
		/// Returns true if the EditBox control is in editing mode.
		/// </summary>
		public bool IsEditing
		{
			get { return (bool)GetValue(IsEditingProperty); }
			set { SetValue(IsEditingProperty, value); }
		}

		private static void OnIsEditingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			((EditableCell)sender).OnIsEditingChanged(e);
		}

		protected virtual void OnIsEditingChanged(DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				if (mEditor == null)
				{
					mEditor = new Editor(this);
					AdornerLayer.GetAdornerLayer(this).Add(mEditor);
				}
				mEditor.BeginEdit();
			}
			else
			{
				if (mEditor != null)
				{
					mEditor.EndEdit(false);
				}
			}

			DependencyPropertyChangedEventHandler temp = IsEditingChanged;
			if (temp != null)
			{
				temp(this, e);
			}
		}

		#endregion

		private class Editor : Adorner
		{
			private readonly VisualCollection mVisualChildren;
			private readonly TextBox mTextBox;
			private readonly BindingExpressionBase mBinding;

			public Editor(EditableCell editableCell)
				: base(editableCell)
			{
				mTextBox = new TextBox();
				mTextBox.Padding = new Thickness(0);
				mTextBox.KeyDown += OnTextBoxKeyDown;
				mTextBox.LostKeyboardFocus += OnTextBoxLostKeyboardFocus;

				mVisualChildren = new VisualCollection(this);
				mVisualChildren.Add(mTextBox);

				mBinding = mTextBox.SetBinding(TextBox.TextProperty, new Binding()
				{
					Path = new PropertyPath(EditableCell.ValueProperty),
					Mode = BindingMode.TwoWay,
					UpdateSourceTrigger = UpdateSourceTrigger.Explicit,
					Source = editableCell
				});

				BindTextBoxToEditableCell(FrameworkElement.HorizontalAlignmentProperty);
				BindTextBoxToEditableCell(FrameworkElement.VerticalAlignmentProperty);
				BindTextBoxToEditableCell(Control.FontFamilyProperty);
				BindTextBoxToEditableCell(Control.FontSizeProperty);
				BindTextBoxToEditableCell(Control.FontStretchProperty);
				BindTextBoxToEditableCell(Control.FontStyleProperty);
				BindTextBoxToEditableCell(Control.FontWeightProperty);
			}

			private void BindTextBoxToEditableCell(DependencyProperty dependencyProperty)
			{
				mTextBox.SetBinding(dependencyProperty, new Binding()
				{
					Path = new PropertyPath(dependencyProperty),
					Source = AdornedElement,
					Mode = BindingMode.OneWay
				});
			}

			public void BeginEdit()
			{
				Visibility = Visibility.Visible;

				mTextBox.SelectAll();
				mTextBox.Focus();
			}

			public void EndEdit(bool confirm)
			{
				if (AdornedElement.IsEditing)
				{
					if (confirm)
					{
						mBinding.UpdateSource();
					}
					AdornedElement.IsEditing = false;
				}
				Visibility = Visibility.Collapsed;
			}

			private new EditableCell AdornedElement
			{
				get { return (EditableCell)base.AdornedElement; }
			}

			private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
			{
				switch (e.Key)
				{
					case Key.Enter:
						//Confirm the edit
						EndEdit(true);
						e.Handled = true;
						break;
					case Key.Escape:
						//Cancel the edit
						EndEdit(false);
						e.Handled = true;
						break;
					case Key.Tab:
						AdornedElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
						EditableCell nextCell = Keyboard.FocusedElement as EditableCell;
						if (nextCell != null)
						{
							nextCell.IsEditing = true;
						}
						e.Handled = true;
						break;
				}
			}

			private void OnTextBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
			{
				//Losing focus results in confirming the edit
				EndEdit(true);
			}

			protected override Size MeasureOverride(Size constraint)
			{
				return new Size(AdornedElement.ActualWidth, AdornedElement.ActualHeight);
			}

			protected override Size ArrangeOverride(Size finalSize)
			{
				mTextBox.Arrange(new Rect(new Point(0, 0), finalSize));
				return finalSize;
			}

			#region VisualChildren
			protected override int VisualChildrenCount
			{
				get { return mVisualChildren.Count; }
			}
			protected override Visual GetVisualChild(int index)
			{
				return mVisualChildren[index];
			}
			#endregion
		}
	}
}