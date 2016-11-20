//AV (27/05/07):
//This file taken from http://blogs.interknowlogy.com/joelrumerman/archive/2007/04/03/12497.aspx
//(no licensing info was available there)
//AV (05/07/08):
//Added context menu for showing and hiding columns
//Added serialisable settings
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Media;

namespace AlbumArtDownloader.Controls //AV (27/05/07): changed namespace to match other controls
{

    // if the GridView exposed any methods at all that allowed for overriding at a control level, I would be
    // able to do all of this work inside it rather than the ListView. However, b/c it doesn't, I have to do the 
    // work inside the ListView.

    // The GridView has access to the ItemSource on the ListView through the dependency property mechanism.

    public class SortableListView : ListView
    {
        SortableGridViewColumn lastSortedOnColumn = null;
        ListSortDirection lastDirection = ListSortDirection.Ascending;


        #region New Dependency Properties

        public string ColumnHeaderSortedAscendingTemplate
        {
            get { return (string)GetValue(ColumnHeaderSortedAscendingTemplateProperty); }
            set { SetValue(ColumnHeaderSortedAscendingTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnHeaderSortedAscendingTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnHeaderSortedAscendingTemplateProperty =
            DependencyProperty.Register("ColumnHeaderSortedAscendingTemplate", typeof(string), typeof(SortableListView), new UIPropertyMetadata(""));


        public string ColumnHeaderSortedDescendingTemplate
        {
            get { return (string)GetValue(ColumnHeaderSortedDescendingTemplateProperty); }
            set { SetValue(ColumnHeaderSortedDescendingTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnHeaderSortedDescendingTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnHeaderSortedDescendingTemplateProperty =
            DependencyProperty.Register("ColumnHeaderSortedDescendingTemplate", typeof(string), typeof(SortableListView), new UIPropertyMetadata(""));


        public string ColumnHeaderNotSortedTemplate
        {
            get { return (string)GetValue(ColumnHeaderNotSortedTemplateProperty); }
            set { SetValue(ColumnHeaderNotSortedTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnHeaderNotSortedTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnHeaderNotSortedTemplateProperty =
            DependencyProperty.Register("ColumnHeaderNotSortedTemplate", typeof(string), typeof(SortableListView), new UIPropertyMetadata(""));

        #endregion

        /// <summary>
        /// Executes when the control is initialized completely the first time through. Runs only once.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            // add the event handler to the GridViewColumnHeader. This strongly ties this ListView to a GridView.
            this.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(GridViewColumnHeaderClickedHandler));

            // cast the ListView's View to a GridView
            GridView gridView = this.View as GridView;
            if (gridView != null)
            {
                // determine which column is marked as IsDefaultSortColumn. Stops on the first column marked this way.
                SortableGridViewColumn sortableGridViewColumn = null;
                foreach (GridViewColumn gridViewColumn in gridView.Columns)
                {
                    sortableGridViewColumn = gridViewColumn as SortableGridViewColumn;
                    if (sortableGridViewColumn != null)
                    {
                        if (sortableGridViewColumn.IsDefaultSortColumn)
                        {
                            break;
                        }
                        sortableGridViewColumn = null;
                    }
                }

                // if the default sort column is defined, sort the data and then update the templates as necessary.
                if (sortableGridViewColumn != null)
                {
                    lastSortedOnColumn = sortableGridViewColumn;
                    Sort(sortableGridViewColumn.SortPropertyName, ListSortDirection.Ascending);

                    if (!String.IsNullOrEmpty(this.ColumnHeaderSortedAscendingTemplate))
                    {
                        sortableGridViewColumn.HeaderTemplate = this.TryFindResource(ColumnHeaderSortedAscendingTemplate) as DataTemplate;
                    }

                    this.SelectedIndex = 0;
                }
            }

            base.OnInitialized(e);
        }

        /// <summary>
        /// Event Handler for the ColumnHeader Click Event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;

            // ensure that we clicked on the column header and not the padding that's added to fill the space.
            if (headerClicked != null && headerClicked.Role != GridViewColumnHeaderRole.Padding)
            {
                // attempt to cast to the sortableGridViewColumn object.
                SortableGridViewColumn sortableGridViewColumn = (headerClicked.Column) as SortableGridViewColumn;

                // ensure that the column header is the correct type and a sort property has been set.
                if (sortableGridViewColumn != null && !String.IsNullOrEmpty(sortableGridViewColumn.SortPropertyName))
                {
					ListSortDirection direction;

					if (lastSortedOnColumn == null
						|| String.IsNullOrEmpty(lastSortedOnColumn.SortPropertyName)
						|| !String.Equals(sortableGridViewColumn.SortPropertyName, lastSortedOnColumn.SortPropertyName, StringComparison.InvariantCultureIgnoreCase))
					{
						//This is a new sort, start with Ascending
						direction = ListSortDirection.Ascending;
					}
					else
					{
						if (lastDirection == ListSortDirection.Ascending)
						{
							direction = ListSortDirection.Descending;
						}
						else
						{
							direction = ListSortDirection.Ascending;
						}
					}
					SortByColumn(sortableGridViewColumn, direction);
                }
            }
        }

		public void SortByColumn(SortableGridViewColumn sortableGridViewColumn, ListSortDirection direction)
		{
			bool newSortColumn = false;

			// determine if this is a new sort, or a switch in sort direction.
			if (lastSortedOnColumn == null
				|| String.IsNullOrEmpty(lastSortedOnColumn.SortPropertyName)
				|| !String.Equals(sortableGridViewColumn.SortPropertyName, lastSortedOnColumn.SortPropertyName, StringComparison.InvariantCultureIgnoreCase))
			{
				newSortColumn = true;
			}

			// get the sort property name from the column's information.
			string sortPropertyName = sortableGridViewColumn.SortPropertyName;

			// Sort the data.
			Sort(sortPropertyName, direction);

			if (direction == ListSortDirection.Ascending)
			{
				if (!String.IsNullOrEmpty(this.ColumnHeaderSortedAscendingTemplate))
				{
					sortableGridViewColumn.HeaderTemplate = this.TryFindResource(ColumnHeaderSortedAscendingTemplate) as DataTemplate;
				}
				else
				{
					sortableGridViewColumn.HeaderTemplate = null;
				}
			}
			else
			{
				if (!String.IsNullOrEmpty(this.ColumnHeaderSortedDescendingTemplate))
				{
					sortableGridViewColumn.HeaderTemplate = this.TryFindResource(ColumnHeaderSortedDescendingTemplate) as DataTemplate;
				}
				else
				{
					sortableGridViewColumn.HeaderTemplate = null;
				}
			}

			// Remove arrow from previously sorted header
			if (newSortColumn && lastSortedOnColumn != null)
			{
				if (!String.IsNullOrEmpty(this.ColumnHeaderNotSortedTemplate))
				{
					lastSortedOnColumn.HeaderTemplate = this.TryFindResource(ColumnHeaderNotSortedTemplate) as DataTemplate;
				}
				else
				{
					lastSortedOnColumn.HeaderTemplate = null;
				}
			}
			lastSortedOnColumn = sortableGridViewColumn;
		}

        /// <summary>
        /// Helper method that sorts the data.
        /// </summary>
        /// <param name="sortBy"></param>
        /// <param name="direction"></param>
        private void Sort(string sortBy, ListSortDirection direction)
        {
            lastDirection = direction;
            ICollectionView dataView = CollectionViewSource.GetDefaultView(this.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

		protected override void OnContextMenuOpening(ContextMenuEventArgs e)
		{
			base.OnContextMenuOpening(e);

			if (!e.Handled)
			{
				//Show a context menu to allow hiding and showing of (sortable) columns
				GridView gridView = this.View as GridView;
				if (gridView != null)
				{
					ContextMenu menu = new ContextMenu();
					foreach (SortableGridViewColumn column in gridView.Columns.OfType<SortableGridViewColumn>())
					{
						MenuItem menuItem = new MenuItem();
						menuItem.Header = column.Header.ToString();
						//Bind the check to whether the column is visible or not
						menuItem.IsCheckable = true;
						menuItem.SetBinding(MenuItem.IsCheckedProperty, new Binding()
						{
							Source = column,
							Path = new PropertyPath(SortableGridViewColumn.IsVisibleProperty),
							Mode = BindingMode.TwoWay
						});
						menu.Items.Add(menuItem);
					}
					this.ContextMenu = menu;
				}
			}
		}

		#region Settings

		public GridSettings GetSettings()
		{
			GridSettings settings = new GridSettings();
			GridView gridView = this.View as GridView;
			if (gridView != null)
			{
				var columnSettings = new List<GridSettings.ColumnSetting>(gridView.Columns.Count);
				foreach (SortableGridViewColumn column in gridView.Columns.OfType<SortableGridViewColumn>())
				{
					columnSettings.Add(new GridSettings.ColumnSetting()
					{
						Name = column.SortPropertyName,
						Width = (double)column.ReadLocalValue(SortableGridViewColumn.WidthProperty), //Read local value to avoid IsVisible coercion to 0
						IsVisible = column.IsVisible
					});
				}

				settings.ColumnSettings = columnSettings.ToArray();
				settings.LastSortedOnColumnIndex = gridView.Columns.IndexOf(this.lastSortedOnColumn);
				settings.LastDirection = this.lastDirection;
			}
			return settings;
		}

		public void ApplySettings(GridSettings settings)
		{
			GridView gridView = this.View as GridView;
			if (gridView != null)
			{
				if (settings.ColumnSettings != null)
				{
					int firstIndex = gridView.Columns.IndexOf(gridView.Columns.OfType<SortableGridViewColumn>().First());
					var columns = gridView.Columns.OfType<SortableGridViewColumn>().ToLookup(column => column.SortPropertyName);
					//First, apply position and size of all columns in the settings
					for (int i = 0; i < settings.ColumnSettings.Length; i++)
					{
						GridSettings.ColumnSetting columnSetting = settings.ColumnSettings[i];
						SortableGridViewColumn column = columns[columnSetting.Name].FirstOrDefault();
						if(column != null)
						{
							//There is a column to which these settings apply.
							column.Width = columnSetting.Width;
							column.IsVisible = columnSetting.IsVisible;
							//Move the column to the right index
							gridView.Columns.Move(gridView.Columns.IndexOf(column), firstIndex + i);
						}
					}

					if (settings.LastSortedOnColumnIndex > -1 && settings.LastSortedOnColumnIndex < gridView.Columns.Count)
					{
						SortableGridViewColumn lastSortedColumn = gridView.Columns[settings.LastSortedOnColumnIndex] as SortableGridViewColumn;
						if (lastSortedColumn != null)
						{
							SortByColumn(lastSortedColumn, settings.LastDirection);
						}
					}
				}
			}
		}

		[Serializable]
		public struct GridSettings
		{
			//These must be public so that the XmlSerializer can serialise them.
			public ColumnSetting[] ColumnSettings; //Use an array rather than a List or ArrayList, as XmlSerialiser can't serialise these
			public int LastSortedOnColumnIndex;
			public ListSortDirection LastDirection;

			[Serializable]
			public struct ColumnSetting
			{
				public string Name;
				public double Width;
				public bool IsVisible;
			}
		}
		#endregion
    }
}
