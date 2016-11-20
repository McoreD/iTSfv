using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace AlbumArtDownloader
{
	public partial class QueueManager : System.Windows.Window
	{
		public QueueManager()
		{
			InitializeComponent();

			//ApplicationCommands.Delete cancels a single (selected) search.
			//ApplicationCommands.Stop cancels all searches (clears the queue).
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, new ExecutedRoutedEventHandler(DeleteExec), new CanExecuteRoutedEventHandler(DeleteCanExec)));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Stop, new ExecutedRoutedEventHandler(StopExec), new CanExecuteRoutedEventHandler(StopCanExec)));

			mQueueDisplay.MouseDoubleClick += new MouseButtonEventHandler(OnQueueDoubleClick);
		}

		private void DeleteExec(object sender, ExecutedRoutedEventArgs e)
		{
			if (e.Parameter is ArtSearchWindow)
			{
				//Remove the specified item
				RemoveFromQueue((ArtSearchWindow)e.Parameter);
			}
			else
			{
				//Remove all the selected items
				foreach (ArtSearchWindow searchWindow in new System.Collections.ArrayList(mQueueDisplay.SelectedItems))
				{
					RemoveFromQueue(searchWindow);
				}
			}
		}

		private void DeleteCanExec(object sender, CanExecuteRoutedEventArgs e)
		{
			if (e.Parameter is ArtSearchWindow || mQueueDisplay.SelectedItems.Count > 0)
				e.CanExecute = true; //Can execute if there is a specific item to delete, or if there is a selection to delete.
			else
				e.CanExecute = false;
		}

		private void StopExec(object sender, ExecutedRoutedEventArgs e)
		{
			IList<ArtSearchWindow> searchQueue = ((App)Application.Current).SearchQueue.Queue;
			while (searchQueue.Count > 0)
			{
				RemoveFromQueue(searchQueue[0]);
			}
		}

		private void StopCanExec(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ((App)Application.Current).SearchQueue.Queue.Count > 0;
		}

		private void RemoveFromQueue(ArtSearchWindow searchWindow)
		{
			((App)Application.Current).SearchQueue.CancelSearchWindow(searchWindow);
		}

		private void OnQueueDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FrameworkElement source = e.OriginalSource as FrameworkElement;
			if (source != null)
			{
				//If the source is deep in the visual tree, go up to its top level parent first
				while (source.Parent is FrameworkElement)
				{
					source = (FrameworkElement)source.Parent;
				}
				//Find the top level templated parent
				while (source.TemplatedParent is FrameworkElement)
				{
					source = (FrameworkElement)source.TemplatedParent;
				}
				ArtSearchWindow searchWindow = mQueueDisplay.ItemContainerGenerator.ItemFromContainer(source) as ArtSearchWindow;
				if (searchWindow != null)
				{
					((App)Application.Current).SearchQueue.ForceSearchWindow(searchWindow);
				}
			}
		}

		private void OnAutomaticDownloadClick(object sender, RoutedEventArgs e)
		{
			var searchQueue = ((App)Application.Current).SearchQueue.Queue;
			if (searchQueue.Count > 0)
			{
				var autoDownloader = new AutoDownloader();

				while (searchQueue.Count > 0)
				{
					var window = searchQueue[0];
					bool ignored;
					Album album = new Album(null, window.Artist, window.Album);
					album.ArtFile = window.GetDefaultSaveFolderPattern(out ignored);
					autoDownloader.Add(album);

					RemoveFromQueue(window);
				}

				autoDownloader.Show();
			}
		}
	}
}