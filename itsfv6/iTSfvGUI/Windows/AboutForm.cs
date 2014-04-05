#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2012 ShareX Developers

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using HelpersLib;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using UploadersLib;

namespace iTSfvGUI
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            Text = Program.Title;
            lblProductName.Text = Application.ProductName + " " + Program.AssemblyVersion;
            lblCopyright.Text = AssemblyCopyright;

            AppendBoldLine("Thanks to:");
            AppendLine("Berk (Jaex) for major contributions in HelpersLib and UploadersLib");
            AppendLine();

            if (Program.LibNames != null)
            {
                AppendBoldLine("Referenced assemblies:");
                foreach (string dll in Program.LibNames)
                {
                    AppendLine(dll);
                }
            }

            CheckUpdate();
        }

        public UpdateChecker CheckUpdate()
        {
            UpdateChecker updateChecker = new GitHubUpdateChecker("ShareX", "ShareX");
            updateChecker.CurrentVersion = Program.AssemblyVersion;
            updateChecker.Proxy = ProxyInfo.Current.GetWebProxy();
            updateChecker.CheckUpdate();

            // Fallback if GitHub API fails
            if (updateChecker.UpdateInfo == null || updateChecker.UpdateInfo.Status == UpdateStatus.UpdateCheckFailed)
            {
                updateChecker = new XMLUpdateChecker("http://getsharex.com/Update.xml", "ShareX");
                updateChecker.CurrentVersion = Program.AssemblyVersion;
                updateChecker.Proxy = ProxyInfo.Current.GetWebProxy();
                updateChecker.CheckUpdate();
            }

            return updateChecker;
        }

        private void AppendLine(string text = "")
        {
            txtDetails.AppendText(text + Environment.NewLine);
        }

        private void AppendBoldLine(string text)
        {
            txtDetails.SelectionFont = new Font(txtDetails.SelectionFont, FontStyle.Bold);
            txtDetails.AppendText(text + Environment.NewLine);
            txtDetails.SelectionFont = new Font(txtDetails.SelectionFont, FontStyle.Regular);
        }

        private void AboutForm_Shown(object sender, EventArgs e)
        {
            this.BringToFront();
            this.Activate();
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        private void lblWebsite_Click(object sender, EventArgs e)
        {
            Helpers.LoadBrowserAsync(Program.URL_WEBSITE);
        }

        private void lblBugs_Click(object sender, EventArgs e)
        {
            Helpers.LoadBrowserAsync(Program.URL_ISSUES);
        }

        private void pbBerkURL_Click(object sender, EventArgs e)
        {
            Helpers.LoadBrowserAsync(Links.URL_BERK);
        }

        private void pbMikeURL_Click(object sender, EventArgs e)
        {
            Helpers.LoadBrowserAsync(Links.URL_MIKE);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtDetails_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }
    }
}