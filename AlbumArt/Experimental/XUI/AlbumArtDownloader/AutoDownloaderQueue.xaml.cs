using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using AlbumArtDownloader.Controls;
using System.Collections.ObjectModel;

namespace AlbumArtDownloader
{
	public partial class AutoDownloaderQueue : SortableListView
	{
		public static class Commands
		{
			public static RoutedUICommand ShowInResults = new RoutedUICommand("Show in Results", "ShowInResults", typeof(Commands));
		}

		private readonly ObservableCollection<Album> mAlbums = new ObservableCollection<Album>();

		public AutoDownloaderQueue()
		{
			InitializeComponent();

			ItemsSource = mAlbums;
		}

		internal ObservableCollection<Album> Albums
		{
			get
			{
				return mAlbums;
			}
		}

		internal Album GetNextAlbum()
		{
			if (Items.Count > 0)
			{
				var current = (Album)SelectedItem;
				if (current == null)
				{
					current = (Album)Items[0];
				}
				var startingPoint = current;
				while (current.ArtFileStatus != ArtFileStatus.Queued)
				{
					int nextIndex = Items.IndexOf(current) + 1;
					if(nextIndex >= Items.Count)
					{
						nextIndex = 0; //Loop back round to the beginning
					}
					current = (Album)Items[nextIndex];

					if (current == startingPoint)
					{
						//Looked at all items, there are no more queued items anywhere in the list
						return null;
					}
				}
				//Found the next queued item
				SelectedItem = current;
				return current;
			}
			return null;
		}

		#region Autoscroll prevention
		/// <summary>
		/// If false, then the control may have the attention of the user, and should not be autoscrolled.
		/// </summary>
		internal bool ShouldAutoscroll
		{
			get
			{
				if (IsMouseOver)
				{
					return false;
				}
				//Check if it has the focus (even if the window itself doesn't)
				DependencyObject focusedElement = FocusManager.GetFocusedElement(FocusManager.GetFocusScope(this)) as DependencyObject;
				if (focusedElement != null && FindCommonVisualAncestor(focusedElement) == this)
				{
					//The focused element is a child of this element
					return false;
				}
				return true;
			}
		}
		#endregion

		#region Command Handlers
		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			base.OnMouseDoubleClick(e);
			
			DependencyObject parent = e.OriginalSource as DependencyObject;
			while (parent != null)
			{
				if (parent is ListViewItem)
				{
					//A list item was double clicked on, so find it in the results
					e.Handled = true;
					var listViewItem = (ListViewItem)parent;
					Commands.ShowInResults.Execute(listViewItem.Content as Album, listViewItem);
					return;
				}
				else if (parent == this)
				{
					//A list item was not double clicked on, something else was
					break;
				}
				parent = VisualTreeHelper.GetParent(parent);
			}
			//Do nothing for double click happening elsewhere.
		}
		#endregion
	}	
}
