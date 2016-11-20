/*  This file is part of Album Art Downloader.
 *  CoverDownloader is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  CoverDownloader is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CoverDownloader; if not, write to the Free Software             
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA  */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace AlbumArtDownloader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        ///         
        [DllImport("USER32.DLL", SetLastError = true)]
        private static extern uint SetForegroundWindow(uint hwnd);
        [DllImport("USER32.DLL", SetLastError = true)]
        private static extern uint FindWindow(string lpClassName, string lpWindowName);
        [Flags]
        enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0000,
            SMTO_BLOCK = 0x0001,
            SMTO_ABORTIFHUNG = 0x0002,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x0008
        }
        [DllImport("USER32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        unsafe static extern IntPtr SendMessageTimeout(uint hWnd, uint Msg,
           UIntPtr wParam, COPYDATASTRUCT* lParam, SendMessageTimeoutFlags fuFlags,
           uint uTimeout, out UIntPtr lpdwResult);
        public unsafe struct COPYDATASTRUCT
        {
            public int dwData;
            public int cbData;
            public int lpData;
        }
        static Mutex coolmutex;
        unsafe static void SendArgs(uint wnd, string[] args)
        {

            COPYDATASTRUCT data;
            data.dwData = 7;
            string buffer = string.Join("\0", args);
            IntPtr ptrData = Marshal.AllocCoTaskMem(buffer.Length * sizeof(char));
            char[] chars = buffer.ToCharArray();
            Marshal.Copy(chars, 0, ptrData, chars.Length);
            UIntPtr result;
            data.lpData = (int)ptrData;
            data.cbData = chars.Length * sizeof(char);
            COPYDATASTRUCT* pointer = (&data);
            SendMessageTimeout(wnd, 74, (UIntPtr)0, pointer, SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 5000, out result);
            Marshal.FreeCoTaskMem(ptrData);

        }
        [STAThread]
        static void Main(string[] args)
        {
            coolmutex = new System.Threading.Mutex(false, "AlbumArtSingleInstance");
            if (coolmutex.WaitOne(1, true))
            {

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //  try
                //  {
                Application.Run(new MainForm(args));
                //  }
                //  catch (Exception e)
                //   {
                //    System.Windows.Forms.MessageBox.Show(e.Message);
                //   }
                coolmutex.Close();
            }
            else
            {

                uint hwndInstance = FindWindow(null, "Album Art Downloader");
                if (hwndInstance != 0)
                {
                    if (args.Length == 0)
                    {
                        SetForegroundWindow(hwndInstance);
                    }
                    else SendArgs(hwndInstance, args);
                }
            }
        }
    }
}