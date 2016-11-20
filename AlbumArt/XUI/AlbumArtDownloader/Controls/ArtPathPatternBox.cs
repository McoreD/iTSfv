using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Win32;

namespace AlbumArtDownloader.Controls
{
	[TemplatePart(Name = "PART_PathEditorHistory", Type = typeof(ComboBox))]
	[TemplatePart(Name = "PART_PathEditor", Type = typeof(TextBox))]
	[TemplatePart(Name = "PART_MenuButton", Type = typeof(ToggleButton))]
	[TemplatePart(Name = "PART_BrowseMenuItem", Type = typeof(MenuItem))]
	public class ArtPathPatternBox : Control
	{
		private static readonly int sMaxHistoryLength = 8;
		public static class Commands
		{
			public static RoutedUICommand Browse = new RoutedUICommand("Browse", "Browse", typeof(Commands));
			public static RoutedUICommand InsertPlaceholder = new RoutedUICommand("Insert Placeholder", "InsertPlaceholder", typeof(Commands));
		}
		static ArtPathPatternBox()
		{
			//This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
			//This style is defined in themes\generic.xaml
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ArtPathPatternBox), new FrameworkPropertyMetadata(typeof(ArtPathPatternBox)));
			//The control itself shouldn't be a tab stop (it's inner controls are)
			IsTabStopProperty.OverrideMetadata(typeof(ArtPathPatternBox), new FrameworkPropertyMetadata(false));
			KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(ArtPathPatternBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.Local));
			KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(ArtPathPatternBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
			KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(ArtPathPatternBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
		}

		public ArtPathPatternBox()
		{
			AdditionalPlaceholders.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnAdditionalPlaceholdersChanged);

			CommandBindings.Add(new CommandBinding(Commands.Browse, new ExecutedRoutedEventHandler(BrowseExec), new CanExecuteRoutedEventHandler(CanBrowse)));
			CommandBindings.Add(new CommandBinding(Commands.InsertPlaceholder, new ExecutedRoutedEventHandler(InsertPlaceholderExec)));
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (MenuButton != null)
			{
				RecreatePlaceholdersMenu();

				MenuButton.ContextMenuOpening += new ContextMenuEventHandler(MenuButtonContextMenuOpening);
				MenuButton.ContextMenu.Placement = PlacementMode.Bottom;
				MenuButton.ContextMenu.PlacementTarget = MenuButton;
			}
			if (PathEditor != null)
			{
				//Make it mimic the editable area of a combo box
				PathEditor.Margin = new Thickness(1, 1, SystemParameters.VerticalScrollBarWidth, 1);
				PathEditor.KeyDown += new KeyEventHandler(OnPathEditorKeyDown);
				PathEditor.PreviewKeyDown += new KeyEventHandler(OnPathEditorPreviewKeyDown);
				if (PathEditorHistory != null)
				{
					PathEditorHistory.SelectionChanged += new SelectionChangedEventHandler(OnPathEditorHistorySelectionChanged);
					PathEditorHistory.DropDownClosed += new EventHandler(OnPathEditorHistoryDropDownClosed);
					PathEditorHistory.CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, new ExecutedRoutedEventHandler(OnHistoryDeleteExec)));
				}
			}
		}

		private void CanBrowse(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = BrowseDialog != null;
		}
		private void BrowseExec(object sender, ExecutedRoutedEventArgs e)
		{
			BrowseDialog.FileName = PathPattern;
			if (BrowseDialog.ShowDialog().GetValueOrDefault(false))
			{
				PathPattern = BrowseDialog.FileName;
			}
			if(PathEditor != null)
				PathEditor.Focus();
		}

		private void InsertPlaceholderExec(object sender, ExecutedRoutedEventArgs e)
		{
			if (PathEditor == null)
			{
				PathPattern += (string)e.Parameter; //Fallback position, just append the placeholder to the path text
			}
			else
			{
				//If there is a path editor text box, put the placeholder at the insertion point
				PathEditor.SelectedText = (string)e.Parameter;
				PathEditor.Select(PathEditor.SelectionStart + PathEditor.SelectionLength, 0);
				PathEditor.Focus();
			}
		}

		private void OnHistoryDeleteExec(object sender, ExecutedRoutedEventArgs e)
		{
			ComboBox history = (ComboBox)sender;
			string highlightedEntry = Utilities.GetComboBoxHighlightedItem(history) as string;
			if (highlightedEntry != null)
			{
				History.Remove(highlightedEntry);
			}
		}

		private void MenuButtonContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			//Cancel context menu opening
			e.Handled = true;
		}

		#region Combo box mimicry for PathEditor
		private void OnPathEditorPreviewKeyDown(object sender, KeyEventArgs e)
		{
			//Handle keys that would otherwise be handled by the path editor
			switch (e.Key)
			{
				case Key.Up:
				case Key.Down:
					if (PathEditorHistory.IsDropDownOpen)
					{
						if (PathEditorHistory.SelectedIndex == -1)
						{
							PathEditorHistory.SelectedIndex = 0;
						}
						PathEditorHistory.Focus();
					}
					e.Handled = true;
					break;
			}
		}
		private void OnPathEditorKeyDown(object sender, KeyEventArgs e)
		{
			//Mimic combo box behaviour
			Key systemKey = e.Key;
			if (systemKey == Key.System)
			{
				systemKey = e.SystemKey;
			}
			switch (systemKey)
			{
				case Key.F4:
					if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) != ModifierKeys.Alt)
					{
						PathEditorHistory.IsDropDownOpen = !PathEditorHistory.IsDropDownOpen;
						e.Handled = true;
					}
					return;

				case Key.Return:
				case Key.Escape:
					if (PathEditorHistory.IsDropDownOpen)
					{
						PathEditorHistory.IsDropDownOpen = false;
						e.Handled = true;
					}
					return;

				case Key.Up:
				case Key.Down:
					if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
					{
						PathEditorHistory.IsDropDownOpen = !PathEditorHistory.IsDropDownOpen;
					}
					e.Handled = true;
					return;
			}
		}

		private void OnPathEditorHistorySelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string value = PathEditorHistory.SelectedValue as string;
			if (value != null)
			{
				PathEditor.Text = value;
			}
		}
		private void OnPathEditorHistoryGotFocus(object sender, RoutedEventArgs e)
		{
			PathEditor.Focus();
		}
		private void OnPathEditorHistoryDropDownClosed(object sender, EventArgs e)
		{
			PathEditor.Focus();
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);

			if ((!e.Handled) && (PathEditor != null))
			{
				if (e.OriginalSource == this)
				{
					PathEditor.Focus();
					e.Handled = true;
				}
			}
		}
		#endregion

		private FileDialog mBrowseDialog;
		public FileDialog BrowseDialog
		{
			get 
			{
				return mBrowseDialog; 
			}
			set 
			{
				mBrowseDialog = value;
			}
		}

		private bool mIncludeArtistAndAlbumPlaceholders = true;
		public bool IncludeArtistAndAlbumPlaceholders 
		{
			get
			{
				return mIncludeArtistAndAlbumPlaceholders;
			}
			set
			{
				if (value != mIncludeArtistAndAlbumPlaceholders)
				{
					mIncludeArtistAndAlbumPlaceholders = value;

					//TODO: Recreate the placeholder menu?
				}
			}
		}

		public event DependencyPropertyChangedEventHandler PathPatternChanged;
		public static readonly DependencyProperty PathPatternProperty = DependencyProperty.Register("PathPattern", typeof(string), typeof(ArtPathPatternBox), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnPathPatternChanged)));
		public string PathPattern
		{
			get { return (string)GetValue(PathPatternProperty); }
			set { SetValue(PathPatternProperty, value); }
		}
		private static void OnPathPatternChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			((ArtPathPatternBox)sender).OnPathPatternChanged(e);
		}
		protected virtual void OnPathPatternChanged(DependencyPropertyChangedEventArgs e)
		{
			DependencyPropertyChangedEventHandler temp = PathPatternChanged;
			if (temp != null)
			{
				temp(this, e);
			}
		}

		private ObservableCollection<PatternPlaceholder> mAdditionalPlaceholders = new ObservableCollection<PatternPlaceholder>();
		public ObservableCollection<PatternPlaceholder> AdditionalPlaceholders
		{
			get { return mAdditionalPlaceholders; }
		}

		private void OnAdditionalPlaceholdersChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (Template != null) //If the template has not been assigned yet, then don't bother updating, it will be updated when the template is applied anyway
			{
				if (e.Action == NotifyCollectionChangedAction.Add)
				{
					//Just add the item
					if (AdditionalPlaceholders.Count == 1)
					{
						//This is the first item, so add a separator
						PlaceholdersMenu.Add(new Separator());
					}
					foreach (PatternPlaceholder placeholder in e.NewItems)
					{
						PlaceholdersMenu.Add(CreatePlaceholderMenuItem(placeholder));
					}
				}
				else
				{
					RecreatePlaceholdersMenu();
				}
			}
		}

		private ObservableCollection<Control> mPlaceholdersMenu = new ObservableCollection<Control>();
		public ObservableCollection<Control> PlaceholdersMenu
		{
			get { return mPlaceholdersMenu; }
		}
		private void RecreatePlaceholdersMenu()
		{
			mPlaceholdersMenu.Clear();
			MenuItem browse = MenuButton.TryFindResource("PART_BrowseMenuItem") as MenuItem;
			if (browse != null && Commands.Browse.CanExecute(null, this))
			{
				PlaceholdersMenu.Add(browse);
				PlaceholdersMenu.Add(new Separator());
			}

			if (IncludeArtistAndAlbumPlaceholders)
			{
				PlaceholdersMenu.Add(CreatePlaceholderMenuItem(new PatternPlaceholder("A_rtist", "The artist searched for", "%artist%")));
				PlaceholdersMenu.Add(CreatePlaceholderMenuItem(new PatternPlaceholder("A_lbum", "The album searched for", "%album%")));
			}

			if (AdditionalPlaceholders.Count > 0)
			{
				if (IncludeArtistAndAlbumPlaceholders)
				{
					PlaceholdersMenu.Add(new Separator());
				}

				foreach (PatternPlaceholder placeholder in AdditionalPlaceholders)
				{
					PlaceholdersMenu.Add(CreatePlaceholderMenuItem(placeholder));
				}
			}
		}

		private MenuItem CreatePlaceholderMenuItem(PatternPlaceholder placeholder)
		{
			MenuItem menuItem = new MenuItem();
			menuItem.Header = placeholder.MenuItemHeader;
			menuItem.ToolTip = placeholder.ToolTip;
			menuItem.CommandParameter = placeholder.Placeholder;
			menuItem.Command = Commands.InsertPlaceholder;
			return menuItem;
		}

		private ObservableCollection<String> mHistory = new ObservableCollection<string>();
		public ObservableCollection<String> History
		{
			get { return mHistory; }
		}

		/// <summary>
		/// Adds the current <see cref="PathPattern"/> to the <see cref="History"/>, or brings it
		/// to the top of the history list if it is already there.
		/// </summary>
		public void AddPatternToHistory()
		{
			//Make sure the changing of the history doesn't affect the text
			string pathPattern = PathPattern;
			if (!String.IsNullOrEmpty(pathPattern)) //Don't add a blank entry
			{
				int index = History.IndexOf(pathPattern);
				if (index == -1)
				{
					//Not yet in history, so add it.
					History.Insert(0, pathPattern);
					//If the history list is too long, trim it
					if (History.Count > sMaxHistoryLength)
					{
						History.RemoveAt(History.Count - 1);
					}
				}
				else if(index != 0)
				{
					//Already in the history, so bring it to the top
					History.Move(index, 0);
				}
			}
		}

		#region Elements
		private ToggleButton mCachedMenuButton;
		protected ToggleButton MenuButton
		{
			get
			{
				if (mCachedMenuButton == null)
				{
					if (Template != null)
					{
						mCachedMenuButton = Template.FindName("PART_MenuButton", this) as ToggleButton;
					}
				}

				return mCachedMenuButton;
			}
		}
		private ComboBox mCachedPathEditorHistory;
		protected ComboBox PathEditorHistory
		{
			get
			{
				if (mCachedPathEditorHistory == null)
				{
					if (Template != null)
					{
						mCachedPathEditorHistory = Template.FindName("PART_PathEditorHistory", this) as ComboBox;
					}
				}

				return mCachedPathEditorHistory;
			}
		}

		private TextBox mCachedPathEditor;
		protected TextBox PathEditor
		{
			get
			{
				if (mCachedPathEditor == null)
				{
					if (Template != null)
					{
						mCachedPathEditor = Template.FindName("PART_PathEditor", this) as TextBox;
					}
				}

				return mCachedPathEditor;
			}
		}
		#endregion
	}
}
