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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace AlbumArtDownloader
{
    public partial class PreviewForm : Form
    {
        Image b;
        MainForm mainForm;
        AlbumArtDownloader.ArtDownloader.ThumbRes thumbres;
        public PreviewForm(MainForm mainForm, Image show, AlbumArtDownloader.ArtDownloader.ThumbRes thethumb)
        {
            InitializeComponent();
            pictureBoxPreview.ClientSize = show.Size;
            pictureBoxPreview.Image = b = show;
            thumbres = thethumb;
            this.mainForm = mainForm;
        }

        private void Preview_Load(object sender, EventArgs e)
        {
            if (thumbres.ScriptOwner == null)
            {
                FileInfo fileInfo = new FileInfo(thumbres.Name);
                this.Text = fileInfo.Name;
            }
            else
            {
                this.Text = thumbres.Name;
            }
            Rectangle r = Screen.GetWorkingArea(this);
            if (r.Width < this.Size.Width || r.Height < this.Size.Height)
            {
                Size s = new Size(r.Width, r.Height);
                this.AutoSize = false;
                this.Size = s;
                pictureBoxPreview.SizeMode = PictureBoxSizeMode.Zoom;
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void Preview_FormClosed(object sender, FormClosedEventArgs e)
        {
            b.Dispose();
        }

        private void Preview_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (thumbres != null)
                mainForm.ThumbClicked(thumbres, mainForm.textBoxFileSave.Text + "\\" + Properties.Settings.Default.SaveFileName);
        }

    }
}