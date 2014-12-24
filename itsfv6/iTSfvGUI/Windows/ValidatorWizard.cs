using iTSfvGUI.Properties;
using iTSfvLib;
using Microsoft.WindowsAPICodePack.Dialogs;
using ShareX.HelpersLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace iTSfvGUI
{
    public partial class ValidatorWizard : Form
    {
        private BackgroundWorker AddFilesWorker = new BackgroundWorker() { WorkerReportsProgress = true };

        public ValidatorWizard()
        {
            InitializeComponent();
        }

        private void LoadDictionaryCheckBoxes(Dictionary<string, CheckBox> dicCheckBoxes, FlowLayoutPanel flp)
        {
            foreach (Control ctl in flp.Controls)
            {
                if (ctl is CheckBox)
                    dicCheckBoxes.Add(ctl.Name, ctl as CheckBox);
            }
        }

        public void SettingsReader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            AutoCheckUpdate();

            Dictionary<string, CheckBox> dicCheckBoxes = GetDictionaryCheckBoxes();

            PropertyInfo[] properties = typeof(UserConfig).GetProperties();
            foreach (PropertyInfo pi in properties)
            {
                string checkBoxName = "chk" + pi.Name;
                if (pi.PropertyType == typeof(Boolean) && dicCheckBoxes.ContainsKey(checkBoxName))
                {
                    CheckBox chk = dicCheckBoxes[checkBoxName];
                    chk.Checked = (bool)pi.GetValue(Program.Config.UI, null);
                }
            }

            Program.Library = new XmlLibrary(Program.Config);
            Program.LogViewer.Show();
        }

        private Dictionary<string, CheckBox> GetDictionaryCheckBoxes()
        {
            Dictionary<string, CheckBox> dicCheckBoxes = new Dictionary<string, CheckBox>();
            LoadDictionaryCheckBoxes(dicCheckBoxes, flpChecks);
            LoadDictionaryCheckBoxes(dicCheckBoxes, flpTracks);
            LoadDictionaryCheckBoxes(dicCheckBoxes, flpFileSystem);
            return dicCheckBoxes;
        }

        /// <summary>
        /// Returns a UserConfig object based on the checkbox configuration
        /// </summary>
        /// <param name="userConfig"></param>
        /// <returns></returns>
        private UserConfig SaveUserConfig(UserConfig userConfig = null)
        {
            if (userConfig == null)
                userConfig = new UserConfig();

            IEnumerator e = GetDictionaryCheckBoxes().GetEnumerator();
            KeyValuePair<string, CheckBox> kvp = new KeyValuePair<string, CheckBox>();

            while (e.MoveNext())
            {
                kvp = (KeyValuePair<string, CheckBox>)e.Current;
                CheckBox chk = kvp.Value as CheckBox;
                PropertyInfo[] properties = typeof(UserConfig).GetProperties();
                foreach (PropertyInfo pi in properties)
                {
                    string propName = chk.Name.Remove(0, 3);
                    if (pi.PropertyType == typeof(Boolean) && pi.Name.Equals(propName))
                    {
                        pi.SetValue(userConfig, chk.Checked, null);
                        break;
                    }
                }
            }

            return userConfig;
        }

        private void ValidatorWizard_Load(object sender, EventArgs e)
        {
            this.Text = Program.Title;
        }

        private void ValidatorWizard_Shown(object sender, EventArgs e)
        {
            TaskbarHelper.Init(this);
        }

        private void AutoCheckUpdate()
        {
            if (Program.Config.AutoCheckUpdate)
            {
                Thread updateThread = new Thread(() =>
                {
                    UpdateChecker updateChecker = AboutForm.CheckUpdate();

                    if (updateChecker != null && updateChecker.Status == ShareX.HelpersLib.UpdateStatus.UpdateAvailable &&
                        MessageBox.Show(Resources.MainWindow_CheckUpdate_,
                            string.Format("{0} {1} is available", Application.ProductName, updateChecker.LatestVersion),
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        using (DownloaderForm updaterForm = new DownloaderForm(updateChecker))
                        {
                            updaterForm.ShowDialog();

                            if (updaterForm.Status == DownloaderFormStatus.InstallStarted)
                            {
                                Application.Exit();
                            }
                        }
                    }
                });
                updateThread.IsBackground = true;
                updateThread.Start();
            }
        }

        private void ValidatorWizard_Move(object sender, EventArgs e)
        {
            UpdateLogViewPos();
        }

        private void UpdateLogViewPos()
        {
            if (null != Program.LogViewer)
            {
                Program.LogViewer.Width = this.Width;
                Program.LogViewer.Location = new Point(this.Location.X, this.Location.Y + this.Height);
            }
        }

        private void miTasksAddFiles_Click(object sender, EventArgs e)
        {
            AddFilesByFolderBrowser(Program.Config.ShowAddFilesWizard);
        }

        private void AddFilesByFolderBrowser(bool showWizard = false)
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog("Add files or a folder...")
            {
                Multiselect = true,
                IsFolderPicker = true
            };

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (showWizard)
                    ShowAddFilesWizard(dlg.FileNames.ToArray());
                else
                    AddFilesFolders(dlg.FileNames.ToArray());
            }
        }

        private void lbDiscs_DragDrop(object sender, DragEventArgs e)
        {
            var pathsFilesFolders = (string[])e.Data.GetData(DataFormats.FileDrop, true);

            if (Program.Config.ShowAddFilesWizard)
                ShowAddFilesWizard(pathsFilesFolders);
            else
                AddFilesFolders(pathsFilesFolders);
        }

        private void AddFilesFolders(string[] filesDirs)
        {
            AddFilesWorker.DoWork += AddFilesWorker_DoWork;
            AddFilesWorker.ProgressChanged += Program.LogViewer.AddFilesWorker_ProgressChanged;
            AddFilesWorker.RunWorkerCompleted += AddFilesWorker_RunWorkerCompleted;

            AddFilesWorker.RunWorkerAsync(filesDirs);
        }

        private void AddTracks(List<XmlTrack> tracks)
        {
            Program.Library.AddTracks(tracks);
            UpdateAlbumArtistTree();
        }

        private void AddFilesWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UpdateAlbumArtistTree();
        }

        private void UpdateAlbumArtistTree()
        {
            tvLibrary.Nodes.Clear();

            foreach (XmlAlbumArtist band in Program.Library.AlbumArtists)
            {
                TreeNode tnAlbumArtist = new TreeNode(band.Name) { Tag = band };

                IEnumerator i = band.Albums.GetEnumerator();
                KeyValuePair<string, XmlAlbum> kvpAlbum = new KeyValuePair<string, XmlAlbum>();

                while (i.MoveNext())
                {
                    kvpAlbum = (KeyValuePair<string, XmlAlbum>)i.Current;
                    tnAlbumArtist.Nodes.Add(new TreeNode(kvpAlbum.Value.GetAlbumName()) { Tag = kvpAlbum.Value });
                }

                tvLibrary.Nodes.Add(tnAlbumArtist);
            }

            Program.LogViewer.AddFilesWorker_RunWorkerCompleted();
            if (Program.Config.ValidateAfterAddingTracks)
                RunTasks();
        }

        private void AddFilesWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            AddFilesWorker.ReportProgress(0);
            Program.Library.AddFilesOrFolders(e.Argument as string[]);
        }

        private void lbDiscs_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void lbDiscs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbDiscs.SelectedIndex > -1)
            {
                XmlDisc tempDisc = lbDiscs.SelectedItem as XmlDisc;
                ttApp.SetToolTip(lbDiscs, tempDisc.ToTracklistString());
            }
        }

        #region Helpers

        public void ShowAddFilesWizard(string[] filesDirs)
        {
            AddFilesWizard afw = new AddFilesWizard(filesDirs);
            if (afw.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Program.Config.CopyMusicToLibrary = afw.CopyMusicToLibrary;

                AddTracks(afw.Tracks);
            }
        }

        #endregion Helpers

        private void tsmiOptions_Click(object sender, EventArgs e)
        {
            OptionsWindow dlg = new OptionsWindow(Program.Config);
            dlg.ShowDialog();
        }

        private void lbDiscs_MouseDown(object sender, MouseEventArgs e)
        {
            lbDiscs.SelectedIndex = lbDiscs.IndexFromPoint(e.X, e.Y);

            if (lbDiscs.SelectedIndex > -1)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    cmsDisc.Show(lbDiscs, e.X, e.Y);
                }
            }
        }

        private void tvLibrary_Click(object sender, EventArgs e)
        {
            // nothing
        }

        private void tvLibrary_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode tn = e.Node;
            if (tn.Tag is XmlAlbum)
            {
                XmlAlbum album = tn.Tag as XmlAlbum;
                lbDiscs.Items.Clear();

                IEnumerator iDisc = album.Discs.GetEnumerator();
                KeyValuePair<string, XmlDisc> kvpDisc = new KeyValuePair<string, XmlDisc>();

                while (iDisc.MoveNext())
                {
                    kvpDisc = (KeyValuePair<string, XmlDisc>)iDisc.Current;
                    lbDiscs.Items.Add(kvpDisc.Value);
                }
            }
        }

        private void ValidatorWizard_Resize(object sender, EventArgs e)
        {
            UpdateLogViewPos();
        }

        private void tsmiTasksValidate_Click(object sender, EventArgs e)
        {
            RunTasks();
        }

        private void RunTasks()
        {
            Program.TreadUI = SynchronizationContext.Current;

            UserConfig userConfig = SaveUserConfig();

            if (UserConfig.IsConfigured(userConfig))
            {
                XmlLibrary selectedLibrary = new XmlLibrary(Program.Config);

                if (lbDiscs.SelectedItem != null)
                {
                    if (lbDiscs.SelectedItem is XmlDisc)
                    {
                        XmlDisc disc = lbDiscs.SelectedItem as XmlDisc;
                        selectedLibrary.AddTracks(disc.Tracks);
                    }
                }
                else if (tvLibrary.SelectedNode != null)
                {
                    if (tvLibrary.SelectedNode.Tag is XmlAlbumArtist)
                    {
                        XmlAlbumArtist band = tvLibrary.SelectedNode.Tag as XmlAlbumArtist;
                        selectedLibrary.AddTracks(band.GetTracks());
                    }
                    else if (tvLibrary.SelectedNode.Tag is XmlAlbum)
                    {
                        XmlAlbum album = tvLibrary.SelectedNode.Tag as XmlAlbum;
                        selectedLibrary.AddTracks(album.GetTracks());
                    }
                }
                else
                {
                    selectedLibrary = Program.Library; // if nothing is selected then validate entire library
                }

                Program.LogViewer.BindWorker(selectedLibrary.Worker);
                selectedLibrary.RunTasks();
            }
        }

        private void ValidatorWizard_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveUserConfig(Program.Config.UI);
            Program.LogViewer.Close();
        }

        private void tsmiFile_AddFilesWithStructure_Click(object sender, EventArgs e)
        {
            AddFilesByFolderBrowser(showWizard: true);
        }

        private void tsmiFoldersLogs_Click(object sender, EventArgs e)
        {
            ShareX.HelpersLib.Helpers.OpenFolder(Program.LogsFolderPath);
        }

        private void tvLibrary_DragDrop(object sender, DragEventArgs e)
        {
            lbDiscs_DragDrop(sender, e);
        }

        private void tvLibrary_DragEnter(object sender, DragEventArgs e)
        {
            lbDiscs_DragEnter(sender, e);
        }

        private void ValidatorWizard_DragEnter(object sender, DragEventArgs e)
        {
            lbDiscs_DragEnter(sender, e);
        }

        private void ValidatorWizard_DragDrop(object sender, DragEventArgs e)
        {
            lbDiscs_DragDrop(sender, e);
        }

        private void tsmiSearchInGoogle_Click(object sender, EventArgs e)
        {
            if (lbDiscs.SelectedIndex > -1)
            {
                XmlDisc disc = lbDiscs.SelectedItem as XmlDisc;
                URLHelpers.OpenURL(disc.GoogleSearchURL);
            }
        }

        private void tsmiShowInExplorer_Click(object sender, EventArgs e)
        {
            if (lbDiscs.SelectedIndex > -1)
            {
                XmlDisc disc = lbDiscs.SelectedItem as XmlDisc;
                Helpers.OpenFolder(disc.Location);
            }
        }

        private void tsmiAbout_Click(object sender, EventArgs e)
        {
            AboutForm frm = new AboutForm() { Icon = this.Icon };
            frm.Show();
        }

        private void tsmiHelpLogViewer_Click(object sender, EventArgs e)
        {
            if (Program.LogViewer.IsDisposed || Program.LogViewer == null)
                Program.LogViewer = new LogViewer();

            Program.LogViewer.Show();
            Program.LogViewer.Focus();
        }
    }
}