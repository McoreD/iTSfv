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
using System.Text;
using System.Runtime.InteropServices;

namespace AlbumArtDownloader
{
    #region LVS_EX
    public enum LVS_EX
    {
        LVS_EX_GRIDLINES = 0x00000001,
        LVS_EX_SUBITEMIMAGES = 0x00000002,
        LVS_EX_CHECKBOXES = 0x00000004,
        LVS_EX_TRACKSELECT = 0x00000008,
        LVS_EX_HEADERDRAGDROP = 0x00000010,
        LVS_EX_FULLROWSELECT = 0x00000020,
        LVS_EX_ONECLICKACTIVATE = 0x00000040,
        LVS_EX_TWOCLICKACTIVATE = 0x00000080,
        LVS_EX_FLATSB = 0x00000100,
        LVS_EX_REGIONAL = 0x00000200,
        LVS_EX_INFOTIP = 0x00000400,
        LVS_EX_UNDERLINEHOT = 0x00000800,
        LVS_EX_UNDERLINECOLD = 0x00001000,
        LVS_EX_MULTIWORKAREAS = 0x00002000,
        LVS_EX_LABELTIP = 0x00004000,
        LVS_EX_BORDERSELECT = 0x00008000,
        LVS_EX_DOUBLEBUFFER = 0x00010000,
        LVS_EX_HIDELABELS = 0x00020000,
        LVS_EX_SINGLEROW = 0x00040000,
        LVS_EX_SNAPTOGRID = 0x00080000,
        LVS_EX_SIMPLESELECT = 0x00100000
    }
    #endregion

    #region LVM
    public enum LVM
    {
        LVM_FIRST = 0x1000,
        LVM_SETEXTENDEDLISTVIEWSTYLE = (LVM_FIRST + 54),
        LVM_GETEXTENDEDLISTVIEWSTYLE = (LVM_FIRST + 55),
    }
    #endregion


    class MegaList : System.Windows.Forms.ListView
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr handle, int messg, int wparam, int lparam);
        public void SetExStyle()
        {
            this.DoubleBuffered = true;
            LVS_EX styles = (LVS_EX)SendMessage(this.Handle,
            (int)LVM.LVM_GETEXTENDEDLISTVIEWSTYLE, 0, 0);
            styles |= LVS_EX.LVS_EX_BORDERSELECT;
            SendMessage(this.Handle,
                (int)LVM.LVM_SETEXTENDEDLISTVIEWSTYLE, 0, (int)styles);
        }
        public void SetExStyleNoBorderSelect()
        {
            this.DoubleBuffered = true;
        }
    }
}
