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
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AlbumArtDownloader
{
    public partial class SettingsForm : Form
    {
        MainForm theparent;
        static char[] hexDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        ListViewItem listViewItemEdit;
        bool isEdit;

        public SettingsForm(MainForm parent)
        {
            theparent = parent;
            InitializeComponent();

            lock (theparent.a.scripts)
            {
				mAllowCheck = true;
                foreach (Script s in theparent.a.scripts)
                {
                    listScripts.Items.Add(s, s.Enabled);
                }
				mAllowCheck = false;
            }

			listScripts_SelectedIndexChanged(null, EventArgs.Empty); //Set up panel visibility and button enabling appropriately

            foreach ( ToolStripButton tsb in theparent.toolStripSave.Items )
            {
                listViewSaveButtons.Items.Add((string)tsb.Tag);
            }

            drawSizeOverlayPreview();
        }
        
        private void SettingsDlg_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void listScripts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listScripts.SelectedItems.Count > 0)
            {
                panelScriptManager.Visible = true;
                labelScriptName.Text = String.Format("Name: {0}", ((Script)listScripts.SelectedItem).Name);
                labelScriptVersion.Text = String.Format("Name: {0}", ((Script)listScripts.SelectedItem).Version);
                labelScriptCreator.Text = String.Format("Name: {0}", ((Script)listScripts.SelectedItem).Creator);
                
				int selIdx = listScripts.SelectedIndex;
				buttonScriptUp.Enabled = selIdx > 0;
				buttonScriptDown.Enabled = selIdx < listScripts.Items.Count - 1;
            }
            else
            {
                panelScriptManager.Visible = false;
				buttonScriptUp.Enabled = false;
				buttonScriptDown.Enabled = false;
            }
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            if (listScripts.SelectedIndices.Count > 0)
            {
                MoveListViewItem(ref listScripts, true);
            }
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            if (listScripts.SelectedIndices.Count > 0)
            {
                MoveListViewItem(ref listScripts, false);
            }
        }

        private void MoveListViewItem(ref CheckedListBox lv, bool moveUp)
        {
            int selIdx;
			int newIdx;

			selIdx = lv.SelectedIndex;
            if (moveUp)
            {
                // ignore moveup of row(0)
                if (selIdx == 0)
                    return;

				newIdx = selIdx - 1;

            }
            else
            {
                // ignore movedown of last item
                if (selIdx == lv.Items.Count - 1)
                    return;

				newIdx = selIdx + 1;
            }

			Script s = (Script)lv.SelectedItem;
			lv.Items.RemoveAt(selIdx);
			lv.Items.Insert(newIdx, s);
			mAllowCheck = true;
			lv.SetItemChecked(newIdx, s.Enabled);
			mAllowCheck = false;
			lv.SelectedIndex = newIdx;

			lv.Refresh();
			lv.Focus();
        }

        private void MoveListViewItem(ref ListView lv, bool moveUp)
        {
            string cache;
            int selIdx;

            selIdx = lv.SelectedItems[0].Index;
            if (moveUp)
            {
                if (selIdx == 0)
                    return;

                for (int i = 0; i < lv.Items[selIdx].SubItems.Count; i++)
                {
                    cache = lv.Items[selIdx - 1].SubItems[i].Text;
                    lv.Items[selIdx - 1].SubItems[i].Text =
                      lv.Items[selIdx].SubItems[i].Text;
                    lv.Items[selIdx].SubItems[i].Text = cache;
                }
                lv.Items[selIdx - 1].Selected = true;
                lv.Refresh();
                lv.Focus();
            }
            else
            {
                if (selIdx == lv.Items.Count - 1)
                    return;

                for (int i = 0; i < lv.Items[selIdx].SubItems.Count; i++)
                {
                    cache = lv.Items[selIdx + 1].SubItems[i].Text;
                    lv.Items[selIdx + 1].SubItems[i].Text =
                      lv.Items[selIdx].SubItems[i].Text;
                    lv.Items[selIdx].SubItems[i].Text = cache;
                }
                lv.Items[selIdx + 1].Selected = true;
                lv.Refresh();
                lv.Focus();
            }
        }


        private void buttonClose_Click(object sender, EventArgs e)
        {
            lock (theparent.a.scripts)
            {
                for (int i = 0; i < listScripts.Items.Count; i++)
                {
                    ((Script)listScripts.Items[i]).SortPosition = i;
                }

                theparent.UpdateScriptOrder(theparent.a.scripts);
            }

            string[] buttons = new string[listViewSaveButtons.Items.Count];

            for (int i = 0; i < listViewSaveButtons.Items.Count; i++)
            {
                buttons[i] = listViewSaveButtons.Items[i].Text;
            }

            theparent.AddSaveButtonsFromString(string.Join("|", buttons), theparent.toolStripSave);

        }

		private bool mAllowCheck; //Only allow check state to change programatically.
		private void listScripts_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (!mAllowCheck)
			{
				//Prevent the check change
				e.NewValue = e.CurrentValue;
				return;
			}
			lock (theparent.a.scripts)
			{
				((Script)listScripts.Items[e.Index]).Enabled = e.NewValue == CheckState.Checked;
			}
		}

		private void listScripts_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.X < (SystemInformation.MenuCheckSize.Width + SystemInformation.Border3DSize.Width))
			{
				//They clicked directly on the checkbox, so check it.
				int idx = listScripts.IndexFromPoint(e.Location);
				if (idx >= 0)
				{
					mAllowCheck = true;
					listScripts.SetItemChecked(idx, !listScripts.GetItemChecked(idx));
					mAllowCheck = false;
				}
			}
		}

		private void listScripts_KeyDown(object sender, KeyEventArgs e)
		{
			mAllowCheck = true; //Allow checking by keypress (space, generally)
		}

		private void listScripts_KeyUp(object sender, KeyEventArgs e)
		{
			mAllowCheck = false; //Reset allow checking after keypress
		}

        private Color HexStringToColor(string hexColor)
        {
            string hc = ExtractHexDigits(hexColor);
            if (hc.Length != 6)
            {
                // you can choose whether to throw an exception
                //throw new ArgumentException("hexColor is not exactly 6 digits.");
                return Color.Empty;
            }
            string r = hc.Substring(0, 2);
            string g = hc.Substring(2, 2);
            string b = hc.Substring(4, 2);
            Color color = Color.Empty;
            try
            {
                int ri
                   = Int32.Parse(r, System.Globalization.NumberStyles.HexNumber);
                int gi
                   = Int32.Parse(g, System.Globalization.NumberStyles.HexNumber);
                int bi
                   = Int32.Parse(b, System.Globalization.NumberStyles.HexNumber);
                color = Color.FromArgb(ri, gi, bi);
            }
            catch
            {
                // you can choose whether to throw an exception
                //throw new ArgumentException("Conversion failed.");
                return Color.Empty;
            }
            return color;
        }

        private string ColorToHexString(Color color)
        {
            byte[] bytes = new byte[3];
            bytes[0] = color.R;
            bytes[1] = color.G;
            bytes[2] = color.B;
            char[] chars = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int b = bytes[i];
                chars[i * 2] = hexDigits[b >> 4];
                chars[i * 2 + 1] = hexDigits[b & 0xF];
            }
            return new string(chars);
        }

        private string ExtractHexDigits(string input)
        {
            // remove any characters that are not digits (like #)
            Regex isHexDigit
               = new Regex("[abcdefABCDEF\\d]+", RegexOptions.Compiled);
            string newnum = "";
            foreach (char c in input)
            {
                if (isHexDigit.IsMatch(c.ToString()))
                    newnum += c.ToString();
            }
            return newnum;
        }

        private void buttonSizeOverlayColorForeground_Click(object sender, EventArgs e)
        {
            try
            {
                colorDialogSizeOverlay.Color = HexStringToColor(textBoxSizeOverlayColorForeground.Text);
            }
            catch
            {
            }

            colorDialogSizeOverlay.ShowDialog(this);

            textBoxSizeOverlayColorForeground.Text = ColorToHexString(colorDialogSizeOverlay.Color);
        }

        private void drawSizeOverlayPreview()
        {
            Bitmap b = new Bitmap(pictureBoxPreviewSizeOverlayColor.Width, pictureBoxPreviewSizeOverlayColor.Height);

            Graphics g = Graphics.FromImage(b);
            g.Clear(Color.White);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            Font f = new Font(SystemFonts.DefaultFont, FontStyle.Bold);
            g.DrawString("500 x 500", f, new SolidBrush(HexStringToColor(textBoxSizeOverlayColorForeground.Text)), 1, 1);

            g.Flush();
            
            pictureBoxPreviewSizeOverlayColor.Image = b;

            f.Dispose();
            g.Dispose();
        }

        private void textBoxSizeOverlayColor_TextChanged(object sender, EventArgs e)
        {
            drawSizeOverlayPreview();
        }
        private void checkBoxUseSizeOverlayColor2_CheckedChanged(object sender, EventArgs e)
        {
            drawSizeOverlayPreview();
        }

        private void checkBoxShowSizeOverlay_CheckedChanged(object sender, EventArgs e)
        {
            panelSizeOverlaySettings.Enabled = checkBoxShowSizeOverlay.Checked;
        }

        private void buttonSaveUp_Click(object sender, EventArgs e)
        {
            if (listViewSaveButtons.SelectedIndices.Count > 0)
            {
                MoveListViewItem(ref listViewSaveButtons, true);
            }
        }

        private void buttonSaveDown_Click(object sender, EventArgs e)
        {
            MoveListViewItem(ref listViewSaveButtons, false);
        }

        private void buttonSaveAdd_Click(object sender, EventArgs e)
        {

            isEdit = false;

            listViewItemEdit = listViewSaveButtons.Items.Add("");

            listViewItemEdit.Selected = true;

            listViewSaveButtons.LabelEdit = true;

            listViewItemEdit.BeginEdit();
        }

        private void listViewSaveButtons_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            listViewSaveButtons.LabelEdit = false;

            if ((e.Label == "" || e.Label == null) && isEdit == false)
                listViewSaveButtons.Items.RemoveAt(e.Item);
        }

        private void buttonSaveEdit_Click(object sender, EventArgs e)
        {
            if (listViewSaveButtons.SelectedIndices.Count > 0)
            {
                isEdit = true;
                listViewSaveButtons.LabelEdit = true;
                listViewSaveButtons.SelectedItems[0].BeginEdit();
            }
        }

        private void buttonSaveRemove_Click(object sender, EventArgs e)
        {
            if (listViewSaveButtons.SelectedIndices.Count > 0)
                listViewSaveButtons.Items.RemoveAt(listViewSaveButtons.SelectedIndices[0]);

        }

        private void SettingsForm_SizeChanged(object sender, EventArgs e)
        {
            columnHeaderFolder.Width = -2;
        }

    }
}