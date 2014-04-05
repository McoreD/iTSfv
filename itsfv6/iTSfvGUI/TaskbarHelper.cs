using Microsoft.WindowsAPICodePack.Taskbar;
using MS.WindowsAPICodePack.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace iTSfvGUI
{
    public static class TaskbarHelper
    {
        #region Windows 7 Taskbar

        private static readonly string appId = Application.ProductName;  // need for Windows 7 Taskbar
        private static readonly string progId = Application.ProductName; // need for Windows 7 Taskbar

        private static IntPtr WindowHandle = IntPtr.Zero;
        private static TaskbarManager WindowsTaskbar;

        #endregion Windows 7 Taskbar

        public static void Init(Form form)
        {
            if (TaskbarManager.IsPlatformSupported)
            {
                WindowsTaskbar = TaskbarManager.Instance;
                WindowsTaskbar.ApplicationId = TaskbarHelper.appId;
                WindowHandle = form.Handle;
            }
        }

        public static void TaskbarSetProgressState(TaskbarProgressBarState tbps)
        {
            TaskbarSetProgressState(Program.MainForm, tbps);
        }

        private static void TaskbarSetProgressState(Form form, TaskbarProgressBarState tbps)
        {
            if (form != null && WindowHandle != IntPtr.Zero && TaskbarManager.IsPlatformSupported && WindowsTaskbar != null)
            {
                WindowsTaskbar.SetProgressState(tbps, windowHandle: WindowHandle);
            }
        }

        public static void TaskbarSetProgressValue(int progress)
        {
            TaskbarSetProgressValue(Program.MainForm, progress);
        }

        private static void TaskbarSetProgressValue(Form form, int progress)
        {
            if (form != null && WindowHandle != IntPtr.Zero && TaskbarManager.IsPlatformSupported && WindowsTaskbar != null)
            {
                WindowsTaskbar.SetProgressValue(progress, 100, windowHandle: WindowHandle);
            }
        }
    }
}