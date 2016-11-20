using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AlbumArtDownloader
{
	/// <summary>
	/// Interaction logic for Menu.xaml
	/// </summary>
	public partial class Menu : System.Windows.Controls.Border
	{
		public static class Commands
		{
			private static InputGestureCollection FileInputGestures()
			{
				InputGestureCollection gestures = new InputGestureCollection();
                gestures.Add(new KeyGesture(Key.B, ModifierKeys.Control));
                return gestures;
			}

            private static InputGestureCollection FileInputGesturesDetails()
            {
                InputGestureCollection gestures = new InputGestureCollection();
                gestures.Add(new KeyGesture(Key.D, ModifierKeys.Control));
                return gestures;
            }

            public static RoutedUICommand NewFileBrowser = new RoutedUICommand("New File Browser...", "NewFileBrowser", typeof(Commands), FileInputGestures());
            public static RoutedUICommand NewFileBrowserDetail = new RoutedUICommand("New File Browser Detail...", "NewFileBrowserDetail", typeof(Commands), FileInputGesturesDetails());

			public static RoutedUICommand NewFoobarBrowser = new RoutedUICommand("New Foobar Browser...", "NewFoobarBrowser", typeof(Commands));
			public static RoutedUICommand ShowQueueManager = new RoutedUICommand("Queue Manager", "ShowQueueManager", typeof(Commands));
			public static RoutedUICommand About = new RoutedUICommand("About...", "About", typeof(Commands));
			public static RoutedUICommand Exit = new RoutedUICommand("E_xit", "Exit", typeof(Commands));
			public static RoutedUICommand CommandLineReference = new RoutedUICommand("Command Line _Reference", "CommandLineReference", typeof(Commands));
			public static RoutedUICommand CheckForUpdates = new RoutedUICommand("Check for _Updtes", "CheckForUpdates", typeof(Commands));
		}

		static Menu()
		{
            CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(ApplicationCommands.New, new ExecutedRoutedEventHandler(NewSearchWindowExec)));
            CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(Commands.NewFileBrowser, new ExecutedRoutedEventHandler(NewFileBrowserExec)));
            CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(Commands.NewFileBrowserDetail, new ExecutedRoutedEventHandler(NewFileBrowserDetailExec)));
			CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(Commands.NewFoobarBrowser, new ExecutedRoutedEventHandler(NewFoobarBrowserExec)));
			CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(Commands.ShowQueueManager, new ExecutedRoutedEventHandler(ShowQueueManagerExec)));
			CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(ApplicationCommands.Close, new ExecutedRoutedEventHandler(CloseExec)));
			CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(Commands.Exit, new ExecutedRoutedEventHandler(ExitExec)));
			CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(Commands.About, new ExecutedRoutedEventHandler(AboutExec)));
			CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(Commands.CommandLineReference, new ExecutedRoutedEventHandler(CommandLineReferenceExec)));
			CommandManager.RegisterClassCommandBinding(typeof(Window), new CommandBinding(Commands.CheckForUpdates, new ExecutedRoutedEventHandler(CheckForUpdatesExec)));
		}

		public Menu()
		{
			InitializeComponent();

#if EPHEMERAL_SETTINGS
			this.Background = System.Windows.Media.Brushes.Red;
#endif
		}

		private void OnWindowListOpened(object sender, RoutedEventArgs e)
		{
			//Populate the list with all the windows
			MenuItem windowList = sender as MenuItem;
			if (windowList != null)
			{
				windowList.Items.Clear();

				//Add the Queue Manager window back in
				windowList.Items.Add(mQueueManager);
				windowList.Items.Add(new Separator());
				
				int iWindow = 0;
				Window thisWindow = Window.GetWindow(windowList);
				IAppWindow thisAppWindow = thisWindow as IAppWindow;

				if (thisAppWindow != null)
				{
					//The queue manager is not the active window
					mQueueManager.IsChecked = false;

					//Add this window as the first in the list.
					MenuItem thisWindowMenuItem = CreateWindowMenuItem(thisAppWindow, ++iWindow);
					thisWindowMenuItem.IsChecked = true; //This is the active window
					windowList.Items.Add(thisWindowMenuItem);
				}
				else if(thisWindow is QueueManager)
				{
					mQueueManager.IsChecked = true;
				}

				foreach (Window window in Application.Current.Windows)
				{
					if (window.IsVisible && window is IAppWindow && window != thisAppWindow) //Don't add this window again, it's already added.
					{
						windowList.Items.Add(CreateWindowMenuItem((IAppWindow)window, ++iWindow));
					}
				}
			}
		}

		private MenuItem CreateWindowMenuItem(IAppWindow window, int windowIndex)
		{
			MenuItem windowMenuItem = new MenuItem();
			windowMenuItem.Header = String.Format("_{0} {1}", windowIndex, window.Description);
			windowMenuItem.IsCheckable = true;
			windowMenuItem.Command = NavigationCommands.GoToPage;
			windowMenuItem.CommandParameter = window;
			return windowMenuItem;
		}

		#region Standard Handlers
		private static void NewSearchWindowExec(object sender, ExecutedRoutedEventArgs e)
		{
			Common.NewSearchWindow(sender as IAppWindow, true);
		}
		private static void NewFileBrowserExec(object sender, ExecutedRoutedEventArgs e)
		{
			Common.NewFileBrowser(sender as IAppWindow);
		}
        private static void NewFileBrowserDetailExec(object sender, ExecutedRoutedEventArgs e)
		{
			Common.NewFileBrowserDetail(sender as IAppWindow);
		}
		private static void NewFoobarBrowserExec(object sender, ExecutedRoutedEventArgs e)
		{
			Common.NewFoobarBrowser(sender as IAppWindow);
		}
		private static void CloseExec(object sender, ExecutedRoutedEventArgs e)
		{
			Window window = sender as Window;
			if(window != null)
				window.Close();
		}
		private static void ExitExec(object sender, ExecutedRoutedEventArgs e)
		{
			((App)Application.Current).SearchQueue.Queue.Clear(); //The queued windows won't open during a shutdown anyway, but this makes things crystal clear.
			Application.Current.Shutdown();
		}
		private static void ShowQueueManagerExec(object sender, ExecutedRoutedEventArgs e)
		{
			((App)Application.Current).SearchQueue.ShowManagerWindow();
		}
		private static void AboutExec(object sender, ExecutedRoutedEventArgs e)
		{
			About about = new About();
			about.Owner = sender as Window;
			about.ShowDialog();
		}
		private static void CommandLineReferenceExec(object sender, ExecutedRoutedEventArgs e)
		{
			new CommandArgsHelp().ShowDialog(null);
		}
		private static void CheckForUpdatesExec(object sender, ExecutedRoutedEventArgs e)
		{
			//Force a check for updates now
			Updates.CheckForUpdates(true);
		}
		#endregion

		private void HideMenuItemExec(object sender, ExecutedRoutedEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			if (item != null)
			{
				item.Visibility = Visibility.Collapsed;
			}
		}
	}
}
