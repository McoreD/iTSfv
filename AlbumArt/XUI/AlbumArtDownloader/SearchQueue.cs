using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace AlbumArtDownloader
{
	/// <summary>
	/// This class holds a queue of search windows that have yet to be displayed or searched
	/// </summary>
	public class SearchQueue: INotifyPropertyChanged
	{
		#region WinAPI
		[DllImport("user32.dll")]
		private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		private const UInt32 SWP_NOSIZE = 0x0001;
		private const UInt32 SWP_NOMOVE = 0x0002;
		private const UInt32 SWP_NOREDRAW = 0x0008;
		private const UInt32 SWP_NOACTIVATE = 0x0010;
		private const UInt32 SWP_SHOWWINDOW = 0x0040;

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, int lParam);
		private delegate bool EnumWindowsProc(IntPtr hwnd, int lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
		private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

		private const UInt32 WM_NCPAINT = 0x0085;
		#endregion

		private QueueManager mManagerWindow;
		private ObservableCollection<ArtSearchWindow> mQueue = new ObservableCollection<ArtSearchWindow>();
		private HashSet<ArtSearchWindow> mNoLoadFromSettingsOnShow = new HashSet<ArtSearchWindow>();

		public SearchQueue()
		{
			SimulataneousWindowsAllowed = Properties.Settings.Default.NumberOfWindowsForQueue;
		}

		/// <summary>
		/// Enqueues the specified search window. This may mean that the search
		/// window is shown immediately, if there are less than <see cref="SimultaneousWindowsAllowed"/>
		/// windows open already.
		/// </summary>
		/// <param name="loadFromSettingsOnShow">If true, the window will load the current settings when it is shown.</param>
		public void EnqueueSearchWindow(ArtSearchWindow searchWindow, bool loadFromSettingsOnShow)
		{
			if (!loadFromSettingsOnShow) //The usual case is to load on show, but if not doing it, flag up this state so that ShowSearchWindow doesn't call LoadSettings for it
			{
				mNoLoadFromSettingsOnShow.Add(searchWindow);
			}
			if (NumberOfOpenSearchWindows < SimulataneousWindowsAllowed)
			{
				//Show the window immediately
				ShowSearchWindow(searchWindow);
			}
			else
			{
				//Enqueue it
				mQueue.Add(searchWindow);
			}
		}

		/// <summary>
		/// If the specified search window is in the queue, remove it from the queue without showing it.
		/// Disposes of the search window, if it is removed.
		/// </summary>
		public void CancelSearchWindow(ArtSearchWindow searchWindow)
		{
			if (mQueue.Remove(searchWindow))
			{
				searchWindow.Close();
			}
		}

		/// <summary>
		/// If the specified search window is in the queue, force it to be shown immediately.
		/// </summary>
		public void ForceSearchWindow(ArtSearchWindow searchWindow)
		{
			int index = mQueue.IndexOf(searchWindow);
			if(index >= 0)
			{
				mQueue.RemoveAt(index);
				ShowSearchWindow(searchWindow);
			}
		}

		/// <summary>
		/// Checks to see if a search window is ready for dequeuing, and if so, dequeues it
		/// </summary>
		private void DequeueNextSearchWindow()
		{
			if (Queue.Count > 0)
			{
				while (NumberOfOpenSearchWindows < SimulataneousWindowsAllowed) //Keep dequeueing as long as more simultaneous windows are allowed.
				{
					//Dequeue and show the next window
					ArtSearchWindow searchWindow = Queue[0];
					mQueue.RemoveAt(0);
					if (NumberOfOpenSearchWindows == 0)
					{
						//Just show normally
						ShowSearchWindow(searchWindow);
					}
					else
					{
						//Show behind existing windows, unactivated
						if (!App.UsePreSP1Compatibility)
						{
							//ShowActivated not supported pre SP1.
							SetShowActivated(searchWindow, false);
						}
						ShowSearchWindow(searchWindow);

						//Send it behind the lowest existing window.
						IntPtr hWnd = ((HwndSource)HwndSource.FromVisual(searchWindow)).Handle;

						//Find the lowest existing window
						IntPtr bottomWindow = IntPtr.Zero;
						
						EnumWindowsProc enumWindowsProc = new EnumWindowsProc(delegate(IntPtr enumHWnd, int enumLParam)
						{
							var hwndSource = HwndSource.FromHwnd(enumHWnd);
							if (hwndSource != null && hwndSource.RootVisual is ArtSearchWindow)
							{
								bottomWindow = enumHWnd;
							}
							return true;
						});
						EnumWindows(enumWindowsProc, 0);
						GC.KeepAlive(enumWindowsProc);
						
						//Send it behind that one
						if (bottomWindow != IntPtr.Zero)
						{
							SetWindowPos(hWnd, bottomWindow, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOREDRAW | SWP_NOSIZE);

							//HACK: Repaint all the other windows Non-Client area (as WPF screws this up)
							foreach (Window window in Application.Current.Windows)
							{
								if (window.IsVisible)
								{
									SendMessage(((HwndSource)HwndSource.FromVisual(window)).Handle, WM_NCPAINT, 0, 0);
								}
							}
						}
					}
				}
				if (Queue.Count == 0)
				{
					//Close the manager, if it is open
					if (mManagerWindow != null)
						mManagerWindow.Close();
				}
			}
		}

		//This method exists to isolate the ShowActivated member, which is unsupported under SP1.
		//This method may not be called if App.UsePreSP1Compatiblity is set false, as it will crash (before even executing the method)
		private void SetShowActivated(ArtSearchWindow window, bool showActivated)
		{
			window.ShowActivated = false;
		}

		/// <summary>
		/// Shows the specified search window, and starts it searching.
		/// The window should have been dequeued before calling this method.
		/// </summary>
		private void ShowSearchWindow(ArtSearchWindow searchWindow)
		{
			if (!mNoLoadFromSettingsOnShow.Remove(searchWindow)) //Don't update from settings if specifically requested not to.
			{
				searchWindow.LoadSettings(); //Ensure the settings are brought up to date
			}
			searchWindow.Closed += new EventHandler(OnSearchWindowClosed);

			searchWindow.Show();
			
			NumberOfOpenSearchWindows++;
		}

		private void OnSearchWindowClosed(object sender, EventArgs e)
		{
			NumberOfOpenSearchWindows--;
		}

		private int mNumberOfOpenSearchWindows;
		/// <summary>
		/// The number of search windows that are currently open.
		/// Note that this only includes search windows opened through
		/// <see cref="ShowSearchWindow"/>, not opened by other means.
		/// </summary>
		public int NumberOfOpenSearchWindows
		{
			get { return mNumberOfOpenSearchWindows; }
			set 
			{
				if (value != mNumberOfOpenSearchWindows)
				{
					mNumberOfOpenSearchWindows = value;

					//Check to see if a new window can be opened
					DequeueNextSearchWindow();
				}
			}
		}

		public ObservableCollection<ArtSearchWindow> Queue
		{
			get
			{
				return mQueue;
			}
		}

		/// <summary>
		/// Displays the Queue Manager window to view and manipulate the queue
		/// </summary>
		public void ShowManagerWindow()
		{
			if (mManagerWindow == null)
			{
				mManagerWindow = new QueueManager();
				mManagerWindow.Closed += new EventHandler(OnManagerWindowClosed);
				mManagerWindow.Show();
			}
			else
			{
				//There is already a queue manager window, so just bring it to the front.
				mManagerWindow.Activate();
			}
		}

		private void OnManagerWindowClosed(object sender, EventArgs e)
		{
			mManagerWindow = null; //Reset the manager window variable so that a new window is created next time.
		}

		#region SimulataneousWindowsAllowed
		private int mSimulataneousWindowsAllowed = 1;
		/// <summary>
		/// The number of windows that can be open before further windows are added to the queue rather than shown.
		/// </summary>
		public int SimulataneousWindowsAllowed
		{
			get { return mSimulataneousWindowsAllowed; }
			set
			{
				if (value != mSimulataneousWindowsAllowed)
				{
					if (value < 1)
						throw new ArgumentOutOfRangeException(); //Must allow at least 1 simultaneous window

					mSimulataneousWindowsAllowed = value;

					DequeueNextSearchWindow();
					Properties.Settings.Default.NumberOfWindowsForQueue = mSimulataneousWindowsAllowed;

					NotifyPropertyChanged("SimulataneousWindowsAllowed");
				}
			}
		}
		#endregion

		#region Property Notification
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler temp = PropertyChanged;
			if (temp != null)
			{
				temp(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}
}
