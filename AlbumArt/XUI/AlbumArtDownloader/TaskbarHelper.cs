using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.Reflection;
using Microsoft.WindowsAPICodePack.Shell;
using System.Windows;
using System.Windows.Threading;

namespace AlbumArtDownloader
{
	internal static class TaskbarHelper
	{
		public static void CreateApplicationJumpList()
		{
			if (TaskbarManager.IsPlatformSupported)
			{
				try
				{
					var exePath = Assembly.GetEntryAssembly().Location;
					var iconReference = new IconReference(exePath, 0);

					var jumpList = JumpList.CreateJumpList();
					if (jumpList != null)
					{
						jumpList.AddUserTasks(new JumpListLink(exePath, "New Search") { Arguments = "/new", IconReference = iconReference },
											  new JumpListLink(exePath, "File Browser") { Arguments = "/fileBrowser", IconReference = iconReference },
											  new JumpListLink(exePath, "Foobar Browser") { Arguments = "/foobarBrowser", IconReference = iconReference });
					}

					jumpList.Refresh();
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.TraceError("Could not create jump list: " + ex.Message);
				}
			}
		}		

		public static void DelayedClearProgress(Window window)
		{
			var timer = new DispatcherTimer(DispatcherPriority.DataBind);
					timer.Interval = TimeSpan.FromMilliseconds(500);
					timer.Tick += delegate
						{
							timer.Stop();
							TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress, window);
						};
					timer.Start();
		}
	}
}
