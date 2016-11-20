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
namespace AlbumArtDownloader
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.textBoxArtist = new System.Windows.Forms.TextBox();
            this.textBoxAlbum = new System.Windows.Forms.TextBox();
            this.groupBoxSearch = new System.Windows.Forms.GroupBox();
            this.labelAlbum = new System.Windows.Forms.Label();
            this.labelArtist = new System.Windows.Forms.Label();
            this.buttonAddTask = new System.Windows.Forms.Button();
            this.textBoxFileSave = new System.Windows.Forms.TextBox();
            this.labelSaveTo = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.MainStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.MainProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.groupBoxOutput = new System.Windows.Forms.GroupBox();
            this.buttonSaveToBrowse = new System.Windows.Forms.Button();
            this.contextMenuStripPicture = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPreview = new System.Windows.Forms.ToolStripMenuItem();
            this.imageListTile = new System.Windows.Forms.ImageList(this.components);
            this.timerClose = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStripQueue = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItemView = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemCancel = new System.Windows.Forms.ToolStripMenuItem();
            this.QueueImageList = new System.Windows.Forms.ImageList(this.components);
            this.splitContainerTileQueue_Browser = new System.Windows.Forms.SplitContainer();
            this.splitContainerTile_Queue = new System.Windows.Forms.SplitContainer();
            this.megaListTiles = new AlbumArtDownloader.MegaList();
            this.clhTiles = new System.Windows.Forms.ColumnHeader();
            this.listViewQueue = new System.Windows.Forms.ListView();
            this.clhQueueArtist = new System.Windows.Forms.ColumnHeader();
            this.clhQueueAlbum = new System.Windows.Forms.ColumnHeader();
            this.tabControlBrowser = new System.Windows.Forms.TabControl();
            this.tabPageBrowseFolder = new System.Windows.Forms.TabPage();
            this.megaListBrowserPath = new AlbumArtDownloader.MegaList();
            this.clhBrowserPathArtist = new System.Windows.Forms.ColumnHeader();
            this.clhBrowserPathAlbum = new System.Windows.Forms.ColumnHeader();
            this.clhBrowserPathArt = new System.Windows.Forms.ColumnHeader();
            this.clhBrowserPathSize = new System.Windows.Forms.ColumnHeader();
            this.imageListHasAlbumArt = new System.Windows.Forms.ImageList(this.components);
            this.toolStripBrowserPath = new System.Windows.Forms.ToolStrip();
            this.BrowserPathRefresh = new System.Windows.Forms.ToolStripButton();
            this.BrowserPathCancel = new System.Windows.Forms.ToolStripButton();
            this.BrowserPathAddArtless = new System.Windows.Forms.ToolStripButton();
            this.BrowserPathProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.tabPageCOMServer = new System.Windows.Forms.TabPage();
            this.megaListBrowserCOM = new AlbumArtDownloader.MegaList();
            this.clhBrowserCOMArtist = new System.Windows.Forms.ColumnHeader();
            this.clhBrowserCOMAlbum = new System.Windows.Forms.ColumnHeader();
            this.clhBrowserCOMArt = new System.Windows.Forms.ColumnHeader();
            this.clhBrowserCOMSize = new System.Windows.Forms.ColumnHeader();
            this.toolStripBrowserCOM = new System.Windows.Forms.ToolStrip();
            this.BrowserCOMRefresh = new System.Windows.Forms.ToolStripButton();
            this.BrowserCOMCancel = new System.Windows.Forms.ToolStripButton();
            this.BrowserCOMAddArtless = new System.Windows.Forms.ToolStripButton();
            this.BrowserCOMProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.BrowserWorkerCOM = new System.ComponentModel.BackgroundWorker();
            this.BrowserWorkerPath = new System.ComponentModel.BackgroundWorker();
            this.folderBrowserDialogFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.queueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.browserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelSearchOutput = new System.Windows.Forms.Panel();
            this.toolStripContainerMain = new System.Windows.Forms.ToolStripContainer();
            this.toolStripSave = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonSaveAs = new System.Windows.Forms.ToolStripButton();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.folderBrowserDialogSaveTo = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBoxSearch.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.groupBoxOutput.SuspendLayout();
            this.contextMenuStripPicture.SuspendLayout();
            this.contextMenuStripQueue.SuspendLayout();
            this.splitContainerTileQueue_Browser.Panel1.SuspendLayout();
            this.splitContainerTileQueue_Browser.Panel2.SuspendLayout();
            this.splitContainerTileQueue_Browser.SuspendLayout();
            this.splitContainerTile_Queue.Panel1.SuspendLayout();
            this.splitContainerTile_Queue.Panel2.SuspendLayout();
            this.splitContainerTile_Queue.SuspendLayout();
            this.tabControlBrowser.SuspendLayout();
            this.tabPageBrowseFolder.SuspendLayout();
            this.toolStripBrowserPath.SuspendLayout();
            this.tabPageCOMServer.SuspendLayout();
            this.toolStripBrowserCOM.SuspendLayout();
            this.menuStripMain.SuspendLayout();
            this.panelSearchOutput.SuspendLayout();
            this.toolStripContainerMain.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainerMain.ContentPanel.SuspendLayout();
            this.toolStripContainerMain.TopToolStripPanel.SuspendLayout();
            this.toolStripContainerMain.SuspendLayout();
            this.toolStripSave.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxArtist
            // 
            this.textBoxArtist.Location = new System.Drawing.Point(45, 13);
            this.textBoxArtist.Name = "textBoxArtist";
            this.textBoxArtist.Size = new System.Drawing.Size(144, 20);
            this.textBoxArtist.TabIndex = 0;
            this.textBoxArtist.Enter += new System.EventHandler(this.artist_Enter);
            // 
            // textBoxAlbum
            // 
            this.textBoxAlbum.Location = new System.Drawing.Point(45, 39);
            this.textBoxAlbum.Name = "textBoxAlbum";
            this.textBoxAlbum.Size = new System.Drawing.Size(144, 20);
            this.textBoxAlbum.TabIndex = 1;
            this.textBoxAlbum.Enter += new System.EventHandler(this.album_Enter);
            // 
            // groupBoxSearch
            // 
            this.groupBoxSearch.Controls.Add(this.labelAlbum);
            this.groupBoxSearch.Controls.Add(this.labelArtist);
            this.groupBoxSearch.Controls.Add(this.buttonAddTask);
            this.groupBoxSearch.Controls.Add(this.textBoxAlbum);
            this.groupBoxSearch.Controls.Add(this.textBoxArtist);
            this.groupBoxSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBoxSearch.Location = new System.Drawing.Point(0, 0);
            this.groupBoxSearch.Name = "groupBoxSearch";
            this.groupBoxSearch.Size = new System.Drawing.Size(284, 68);
            this.groupBoxSearch.TabIndex = 3;
            this.groupBoxSearch.TabStop = false;
            this.groupBoxSearch.Text = "Search";
            // 
            // labelAlbum
            // 
            this.labelAlbum.AutoSize = true;
            this.labelAlbum.Location = new System.Drawing.Point(6, 42);
            this.labelAlbum.Name = "labelAlbum";
            this.labelAlbum.Size = new System.Drawing.Size(39, 13);
            this.labelAlbum.TabIndex = 10;
            this.labelAlbum.Text = "Album:";
            // 
            // labelArtist
            // 
            this.labelArtist.AutoSize = true;
            this.labelArtist.Location = new System.Drawing.Point(6, 16);
            this.labelArtist.Name = "labelArtist";
            this.labelArtist.Size = new System.Drawing.Size(33, 13);
            this.labelArtist.TabIndex = 9;
            this.labelArtist.Text = "Artist:";
            // 
            // buttonAddTask
            // 
            this.buttonAddTask.Image = global::AlbumArtDownloader.Properties.Resources.test3;
            this.buttonAddTask.Location = new System.Drawing.Point(195, 21);
            this.buttonAddTask.Name = "buttonAddTask";
            this.buttonAddTask.Size = new System.Drawing.Size(83, 31);
            this.buttonAddTask.TabIndex = 2;
            this.buttonAddTask.Text = "Add Task";
            this.buttonAddTask.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonAddTask.UseVisualStyleBackColor = true;
            this.buttonAddTask.Click += new System.EventHandler(this.buttonAddTask_Click);
            // 
            // textBoxFileSave
            // 
            this.textBoxFileSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFileSave.Location = new System.Drawing.Point(63, 27);
            this.textBoxFileSave.Name = "textBoxFileSave";
            this.textBoxFileSave.Size = new System.Drawing.Size(397, 20);
            this.textBoxFileSave.TabIndex = 5;
            this.textBoxFileSave.TextChanged += new System.EventHandler(this.testBoxFileSave_TextChanged);
            // 
            // labelSaveTo
            // 
            this.labelSaveTo.AutoSize = true;
            this.labelSaveTo.Location = new System.Drawing.Point(6, 30);
            this.labelSaveTo.Name = "labelSaveTo";
            this.labelSaveTo.Size = new System.Drawing.Size(51, 13);
            this.labelSaveTo.TabIndex = 6;
            this.labelSaveTo.Text = "Save To:";
            // 
            // statusStrip
            // 
            this.statusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainStatus,
            this.MainProgress});
            this.statusStrip.Location = new System.Drawing.Point(0, 0);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(788, 22);
            this.statusStrip.TabIndex = 7;
            this.statusStrip.Text = "statusStrip1";
            // 
            // MainStatus
            // 
            this.MainStatus.Name = "MainStatus";
            this.MainStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // MainProgress
            // 
            this.MainProgress.Name = "MainProgress";
            this.MainProgress.Size = new System.Drawing.Size(100, 16);
            this.MainProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.MainProgress.Visible = false;
            // 
            // groupBoxOutput
            // 
            this.groupBoxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxOutput.Controls.Add(this.buttonSaveToBrowse);
            this.groupBoxOutput.Controls.Add(this.labelSaveTo);
            this.groupBoxOutput.Controls.Add(this.textBoxFileSave);
            this.groupBoxOutput.Location = new System.Drawing.Point(290, 0);
            this.groupBoxOutput.Name = "groupBoxOutput";
            this.groupBoxOutput.Size = new System.Drawing.Size(498, 68);
            this.groupBoxOutput.TabIndex = 8;
            this.groupBoxOutput.TabStop = false;
            this.groupBoxOutput.Text = "Output";
            // 
            // buttonSaveToBrowse
            // 
            this.buttonSaveToBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveToBrowse.Location = new System.Drawing.Point(466, 21);
            this.buttonSaveToBrowse.Name = "buttonSaveToBrowse";
            this.buttonSaveToBrowse.Size = new System.Drawing.Size(25, 31);
            this.buttonSaveToBrowse.TabIndex = 7;
            this.buttonSaveToBrowse.Text = "...";
            this.buttonSaveToBrowse.UseVisualStyleBackColor = true;
            this.buttonSaveToBrowse.Click += new System.EventHandler(this.buttonSaveToBrowse_Click);
            // 
            // contextMenuStripPicture
            // 
            this.contextMenuStripPicture.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemSaveAs,
            this.toolStripMenuItemPreview});
            this.contextMenuStripPicture.Name = "contextMenuStrip1";
            this.contextMenuStripPicture.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStripPicture.Size = new System.Drawing.Size(126, 48);
            // 
            // toolStripMenuItemSaveAs
            // 
            this.toolStripMenuItemSaveAs.Name = "toolStripMenuItemSaveAs";
            this.toolStripMenuItemSaveAs.Size = new System.Drawing.Size(125, 22);
            this.toolStripMenuItemSaveAs.Text = "&Save As...";
            this.toolStripMenuItemSaveAs.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripMenuItemPreview
            // 
            this.toolStripMenuItemPreview.Name = "toolStripMenuItemPreview";
            this.toolStripMenuItemPreview.Size = new System.Drawing.Size(125, 22);
            this.toolStripMenuItemPreview.Text = "&Preview...";
            this.toolStripMenuItemPreview.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // imageListTile
            // 
            this.imageListTile.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageListTile.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListTile.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // timerClose
            // 
            this.timerClose.Tick += new System.EventHandler(this.timerClose_Tick);
            // 
            // contextMenuStripQueue
            // 
            this.contextMenuStripQueue.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemView,
            this.ToolStripMenuItemCancel});
            this.contextMenuStripQueue.Name = "contextMenuStrip2";
            this.contextMenuStripQueue.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStripQueue.Size = new System.Drawing.Size(107, 48);
            // 
            // ToolStripMenuItemView
            // 
            this.ToolStripMenuItemView.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ToolStripMenuItemView.Name = "ToolStripMenuItemView";
            this.ToolStripMenuItemView.Size = new System.Drawing.Size(106, 22);
            this.ToolStripMenuItemView.Text = "&View";
            this.ToolStripMenuItemView.Click += new System.EventHandler(this.cancelToolStripMenuItem_Click);
            // 
            // ToolStripMenuItemCancel
            // 
            this.ToolStripMenuItemCancel.Name = "ToolStripMenuItemCancel";
            this.ToolStripMenuItemCancel.Size = new System.Drawing.Size(106, 22);
            this.ToolStripMenuItemCancel.Text = "&Cancel";
            this.ToolStripMenuItemCancel.Click += new System.EventHandler(this.cancelToolStripMenuItem1_Click);
            // 
            // QueueImageList
            // 
            this.QueueImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("QueueImageList.ImageStream")));
            this.QueueImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.QueueImageList.Images.SetKeyName(0, "test.ico");
            this.QueueImageList.Images.SetKeyName(1, "test3.ico");
            this.QueueImageList.Images.SetKeyName(2, "test2.ico");
            // 
            // splitContainerTileQueue_Browser
            // 
            this.splitContainerTileQueue_Browser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerTileQueue_Browser.Location = new System.Drawing.Point(0, 73);
            this.splitContainerTileQueue_Browser.Name = "splitContainerTileQueue_Browser";
            // 
            // splitContainerTileQueue_Browser.Panel1
            // 
            this.splitContainerTileQueue_Browser.Panel1.Controls.Add(this.splitContainerTile_Queue);
            // 
            // splitContainerTileQueue_Browser.Panel2
            // 
            this.splitContainerTileQueue_Browser.Panel2.Controls.Add(this.tabControlBrowser);
            this.splitContainerTileQueue_Browser.Size = new System.Drawing.Size(788, 414);
            this.splitContainerTileQueue_Browser.SplitterDistance = 440;
            this.splitContainerTileQueue_Browser.TabIndex = 0;
            // 
            // splitContainerTile_Queue
            // 
            this.splitContainerTile_Queue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerTile_Queue.Location = new System.Drawing.Point(0, 0);
            this.splitContainerTile_Queue.Name = "splitContainerTile_Queue";
            this.splitContainerTile_Queue.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerTile_Queue.Panel1
            // 
            this.splitContainerTile_Queue.Panel1.Controls.Add(this.megaListTiles);
            // 
            // splitContainerTile_Queue.Panel2
            // 
            this.splitContainerTile_Queue.Panel2.Controls.Add(this.listViewQueue);
            this.splitContainerTile_Queue.Size = new System.Drawing.Size(440, 414);
            this.splitContainerTile_Queue.SplitterDistance = 227;
            this.splitContainerTile_Queue.TabIndex = 0;
            // 
            // megaListTiles
            // 
            this.megaListTiles.CausesValidation = false;
            this.megaListTiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhTiles});
            this.megaListTiles.ContextMenuStrip = this.contextMenuStripPicture;
            this.megaListTiles.Cursor = System.Windows.Forms.Cursors.Default;
            this.megaListTiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.megaListTiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.megaListTiles.HideSelection = false;
            this.megaListTiles.LargeImageList = this.imageListTile;
            this.megaListTiles.Location = new System.Drawing.Point(0, 0);
            this.megaListTiles.MultiSelect = false;
            this.megaListTiles.Name = "megaListTiles";
            this.megaListTiles.ShowItemToolTips = true;
            this.megaListTiles.Size = new System.Drawing.Size(440, 227);
            this.megaListTiles.SmallImageList = this.imageListTile;
            this.megaListTiles.TabIndex = 0;
            this.toolTip.SetToolTip(this.megaListTiles, "test");
            this.megaListTiles.UseCompatibleStateImageBehavior = false;
            this.megaListTiles.ItemActivate += new System.EventHandler(this.megaListTiles_ItemActivate);
            this.megaListTiles.MouseDown += new System.Windows.Forms.MouseEventHandler(this.megaListTiles_MouseDown);
            // 
            // listViewQueue
            // 
            this.listViewQueue.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhQueueArtist,
            this.clhQueueAlbum});
            this.listViewQueue.ContextMenuStrip = this.contextMenuStripQueue;
            this.listViewQueue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewQueue.FullRowSelect = true;
            this.listViewQueue.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewQueue.Location = new System.Drawing.Point(0, 0);
            this.listViewQueue.Name = "listViewQueue";
            this.listViewQueue.Size = new System.Drawing.Size(440, 183);
            this.listViewQueue.SmallImageList = this.QueueImageList;
            this.listViewQueue.TabIndex = 1;
            this.listViewQueue.UseCompatibleStateImageBehavior = false;
            this.listViewQueue.View = System.Windows.Forms.View.Details;
            this.listViewQueue.ItemActivate += new System.EventHandler(this.listViewQueue_ItemActivate);
            this.listViewQueue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewQueue_KeyDown);
            // 
            // clhQueueArtist
            // 
            this.clhQueueArtist.Text = "Artist";
            this.clhQueueArtist.Width = 85;
            // 
            // clhQueueAlbum
            // 
            this.clhQueueAlbum.Text = "Album";
            this.clhQueueAlbum.Width = 83;
            // 
            // tabControlBrowser
            // 
            this.tabControlBrowser.Controls.Add(this.tabPageBrowseFolder);
            this.tabControlBrowser.Controls.Add(this.tabPageCOMServer);
            this.tabControlBrowser.DataBindings.Add(new System.Windows.Forms.Binding("SelectedIndex", global::AlbumArtDownloader.Properties.Settings.Default, "SelectedBrowserTab", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tabControlBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlBrowser.Location = new System.Drawing.Point(0, 0);
            this.tabControlBrowser.Name = "tabControlBrowser";
            this.tabControlBrowser.SelectedIndex = global::AlbumArtDownloader.Properties.Settings.Default.SelectedBrowserTab;
            this.tabControlBrowser.Size = new System.Drawing.Size(344, 414);
            this.tabControlBrowser.TabIndex = 2;
            // 
            // tabPageBrowseFolder
            // 
            this.tabPageBrowseFolder.Controls.Add(this.megaListBrowserPath);
            this.tabPageBrowseFolder.Controls.Add(this.toolStripBrowserPath);
            this.tabPageBrowseFolder.Location = new System.Drawing.Point(4, 22);
            this.tabPageBrowseFolder.Name = "tabPageBrowseFolder";
            this.tabPageBrowseFolder.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBrowseFolder.Size = new System.Drawing.Size(336, 388);
            this.tabPageBrowseFolder.TabIndex = 1;
            this.tabPageBrowseFolder.Text = "Browse Folder";
            this.tabPageBrowseFolder.UseVisualStyleBackColor = true;
            // 
            // megaListBrowserPath
            // 
            this.megaListBrowserPath.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhBrowserPathArtist,
            this.clhBrowserPathAlbum,
            this.clhBrowserPathArt,
            this.clhBrowserPathSize});
            this.megaListBrowserPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.megaListBrowserPath.FullRowSelect = true;
            this.megaListBrowserPath.Location = new System.Drawing.Point(3, 28);
            this.megaListBrowserPath.Name = "megaListBrowserPath";
            this.megaListBrowserPath.Size = new System.Drawing.Size(330, 357);
            this.megaListBrowserPath.SmallImageList = this.imageListHasAlbumArt;
            this.megaListBrowserPath.TabIndex = 2;
            this.megaListBrowserPath.UseCompatibleStateImageBehavior = false;
            this.megaListBrowserPath.View = System.Windows.Forms.View.Details;
            this.megaListBrowserPath.ItemActivate += new System.EventHandler(this.megaListBrowserPath_ItemActivate);
            this.megaListBrowserPath.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.megaListBrowserPath_ColumnClick);
            // 
            // clhBrowserPathArtist
            // 
            this.clhBrowserPathArtist.Text = "Artist";
            // 
            // clhBrowserPathAlbum
            // 
            this.clhBrowserPathAlbum.Text = "Album";
            // 
            // clhBrowserPathArt
            // 
            this.clhBrowserPathArt.Text = "Art";
            // 
            // clhBrowserPathSize
            // 
            this.clhBrowserPathSize.Text = "Size";
            // 
            // imageListHasAlbumArt
            // 
            this.imageListHasAlbumArt.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListHasAlbumArt.ImageStream")));
            this.imageListHasAlbumArt.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListHasAlbumArt.Images.SetKeyName(0, "OK.bmp");
            this.imageListHasAlbumArt.Images.SetKeyName(1, "error.ico");
            // 
            // toolStripBrowserPath
            // 
            this.toolStripBrowserPath.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripBrowserPath.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BrowserPathRefresh,
            this.BrowserPathCancel,
            this.BrowserPathAddArtless,
            this.BrowserPathProgress});
            this.toolStripBrowserPath.Location = new System.Drawing.Point(3, 3);
            this.toolStripBrowserPath.Name = "toolStripBrowserPath";
            this.toolStripBrowserPath.Size = new System.Drawing.Size(330, 25);
            this.toolStripBrowserPath.TabIndex = 3;
            this.toolStripBrowserPath.Text = "toolStrip1";
            // 
            // BrowserPathRefresh
            // 
            this.BrowserPathRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BrowserPathRefresh.Image = global::AlbumArtDownloader.Properties.Resources.test21;
            this.BrowserPathRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BrowserPathRefresh.Name = "BrowserPathRefresh";
            this.BrowserPathRefresh.Size = new System.Drawing.Size(23, 22);
            this.BrowserPathRefresh.Text = "Refresh";
            this.BrowserPathRefresh.Click += new System.EventHandler(this.BrowserPathRefresh_Click);
            // 
            // BrowserPathCancel
            // 
            this.BrowserPathCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BrowserPathCancel.Enabled = false;
            this.BrowserPathCancel.Image = global::AlbumArtDownloader.Properties.Resources.test2;
            this.BrowserPathCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BrowserPathCancel.Name = "BrowserPathCancel";
            this.BrowserPathCancel.Size = new System.Drawing.Size(23, 22);
            this.BrowserPathCancel.Text = "Cancel";
            this.BrowserPathCancel.Click += new System.EventHandler(this.BrowserPathCancel_Click);
            // 
            // BrowserPathAddArtless
            // 
            this.BrowserPathAddArtless.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BrowserPathAddArtless.Enabled = false;
            this.BrowserPathAddArtless.Image = global::AlbumArtDownloader.Properties.Resources.test4;
            this.BrowserPathAddArtless.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BrowserPathAddArtless.Name = "BrowserPathAddArtless";
            this.BrowserPathAddArtless.Size = new System.Drawing.Size(23, 22);
            this.BrowserPathAddArtless.Text = "Add Artless Items to Queue";
            this.BrowserPathAddArtless.Click += new System.EventHandler(this.BrowserPathAddArtless_Click);
            // 
            // BrowserPathProgress
            // 
            this.BrowserPathProgress.Name = "BrowserPathProgress";
            this.BrowserPathProgress.Size = new System.Drawing.Size(100, 22);
            this.BrowserPathProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.BrowserPathProgress.Visible = false;
            // 
            // tabPageCOMServer
            // 
            this.tabPageCOMServer.Controls.Add(this.megaListBrowserCOM);
            this.tabPageCOMServer.Controls.Add(this.toolStripBrowserCOM);
            this.tabPageCOMServer.Location = new System.Drawing.Point(4, 22);
            this.tabPageCOMServer.Name = "tabPageCOMServer";
            this.tabPageCOMServer.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCOMServer.Size = new System.Drawing.Size(336, 388);
            this.tabPageCOMServer.TabIndex = 0;
            this.tabPageCOMServer.Text = "Foobar2000 COM Server";
            this.tabPageCOMServer.UseVisualStyleBackColor = true;
            // 
            // megaListBrowserCOM
            // 
            this.megaListBrowserCOM.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhBrowserCOMArtist,
            this.clhBrowserCOMAlbum,
            this.clhBrowserCOMArt,
            this.clhBrowserCOMSize});
            this.megaListBrowserCOM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.megaListBrowserCOM.FullRowSelect = true;
            this.megaListBrowserCOM.Location = new System.Drawing.Point(3, 28);
            this.megaListBrowserCOM.Name = "megaListBrowserCOM";
            this.megaListBrowserCOM.Size = new System.Drawing.Size(330, 357);
            this.megaListBrowserCOM.SmallImageList = this.imageListHasAlbumArt;
            this.megaListBrowserCOM.TabIndex = 0;
            this.megaListBrowserCOM.UseCompatibleStateImageBehavior = false;
            this.megaListBrowserCOM.View = System.Windows.Forms.View.Details;
            this.megaListBrowserCOM.ItemActivate += new System.EventHandler(this.megaListBrowserCOM_ItemActivate);
            this.megaListBrowserCOM.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.megaListBrowserCOM_ColumnClick);
            // 
            // clhBrowserCOMArtist
            // 
            this.clhBrowserCOMArtist.Text = "Artist";
            // 
            // clhBrowserCOMAlbum
            // 
            this.clhBrowserCOMAlbum.Text = "Album";
            // 
            // clhBrowserCOMArt
            // 
            this.clhBrowserCOMArt.Text = "Art";
            // 
            // clhBrowserCOMSize
            // 
            this.clhBrowserCOMSize.Text = "Size";
            // 
            // toolStripBrowserCOM
            // 
            this.toolStripBrowserCOM.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripBrowserCOM.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BrowserCOMRefresh,
            this.BrowserCOMCancel,
            this.BrowserCOMAddArtless,
            this.BrowserCOMProgress});
            this.toolStripBrowserCOM.Location = new System.Drawing.Point(3, 3);
            this.toolStripBrowserCOM.Name = "toolStripBrowserCOM";
            this.toolStripBrowserCOM.Size = new System.Drawing.Size(330, 25);
            this.toolStripBrowserCOM.TabIndex = 1;
            this.toolStripBrowserCOM.Text = "toolStrip1";
            // 
            // BrowserCOMRefresh
            // 
            this.BrowserCOMRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BrowserCOMRefresh.Image = global::AlbumArtDownloader.Properties.Resources.test21;
            this.BrowserCOMRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BrowserCOMRefresh.Name = "BrowserCOMRefresh";
            this.BrowserCOMRefresh.Size = new System.Drawing.Size(23, 22);
            this.BrowserCOMRefresh.Text = "Refresh";
            this.BrowserCOMRefresh.Click += new System.EventHandler(this.BrowserCOMRefresh_Click);
            // 
            // BrowserCOMCancel
            // 
            this.BrowserCOMCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BrowserCOMCancel.Enabled = false;
            this.BrowserCOMCancel.Image = global::AlbumArtDownloader.Properties.Resources.test2;
            this.BrowserCOMCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BrowserCOMCancel.Name = "BrowserCOMCancel";
            this.BrowserCOMCancel.Size = new System.Drawing.Size(23, 22);
            this.BrowserCOMCancel.Text = "Cancel";
            this.BrowserCOMCancel.Click += new System.EventHandler(this.BrowserCOMCancel_Click);
            // 
            // BrowserCOMAddArtless
            // 
            this.BrowserCOMAddArtless.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BrowserCOMAddArtless.Enabled = false;
            this.BrowserCOMAddArtless.Image = global::AlbumArtDownloader.Properties.Resources.test4;
            this.BrowserCOMAddArtless.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BrowserCOMAddArtless.Name = "BrowserCOMAddArtless";
            this.BrowserCOMAddArtless.Size = new System.Drawing.Size(23, 22);
            this.BrowserCOMAddArtless.Text = "Add Artless Items to Queue";
            this.BrowserCOMAddArtless.Click += new System.EventHandler(this.BrowserCOMAddArtless_Click);
            // 
            // BrowserCOMProgress
            // 
            this.BrowserCOMProgress.Name = "BrowserCOMProgress";
            this.BrowserCOMProgress.Size = new System.Drawing.Size(100, 22);
            this.BrowserCOMProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.BrowserCOMProgress.Visible = false;
            // 
            // BrowserWorkerCOM
            // 
            this.BrowserWorkerCOM.WorkerReportsProgress = true;
            this.BrowserWorkerCOM.WorkerSupportsCancellation = true;
            this.BrowserWorkerCOM.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BrowserWorkerCOM_DoWork);
            this.BrowserWorkerCOM.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BrowserWorkerCOM_RunWorkerCompleted);
            this.BrowserWorkerCOM.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BrowserWorkerCOM_ProgressChanged);
            // 
            // BrowserWorkerPath
            // 
            this.BrowserWorkerPath.WorkerReportsProgress = true;
            this.BrowserWorkerPath.WorkerSupportsCancellation = true;
            this.BrowserWorkerPath.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BrowserWorkerPath_DoWork);
            this.BrowserWorkerPath.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BrowserWorkerPath_RunWorkerCompleted);
            this.BrowserWorkerPath.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BrowserWorkerPath_ProgressChanged);
            // 
            // menuStripMain
            // 
            this.menuStripMain.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStripMain.Size = new System.Drawing.Size(788, 24);
            this.menuStripMain.TabIndex = 9;
            this.menuStripMain.Text = "menuStripMain";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Image = global::AlbumArtDownloader.Properties.Resources.test;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.queueToolStripMenuItem,
            this.browserToolStripMenuItem,
            this.saveToolbarToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // queueToolStripMenuItem
            // 
            this.queueToolStripMenuItem.CheckOnClick = true;
            this.queueToolStripMenuItem.Name = "queueToolStripMenuItem";
            this.queueToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.queueToolStripMenuItem.Text = "Queue";
            this.queueToolStripMenuItem.CheckedChanged += new System.EventHandler(this.queueToolStripMenuItem_CheckedChanged);
            // 
            // browserToolStripMenuItem
            // 
            this.browserToolStripMenuItem.CheckOnClick = true;
            this.browserToolStripMenuItem.Name = "browserToolStripMenuItem";
            this.browserToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.browserToolStripMenuItem.Text = "Browser";
            this.browserToolStripMenuItem.CheckedChanged += new System.EventHandler(this.browserToolStripMenuItem_CheckedChanged);
            // 
            // saveToolbarToolStripMenuItem
            // 
            this.saveToolbarToolStripMenuItem.CheckOnClick = true;
            this.saveToolbarToolStripMenuItem.Name = "saveToolbarToolStripMenuItem";
            this.saveToolbarToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.saveToolbarToolStripMenuItem.Text = "Save Toolbar";
            this.saveToolbarToolStripMenuItem.CheckedChanged += new System.EventHandler(this.saveToolbarToolStripMenuItem_CheckedChanged);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // panelSearchOutput
            // 
            this.panelSearchOutput.Controls.Add(this.groupBoxOutput);
            this.panelSearchOutput.Controls.Add(this.groupBoxSearch);
            this.panelSearchOutput.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSearchOutput.Location = new System.Drawing.Point(0, 0);
            this.panelSearchOutput.Name = "panelSearchOutput";
            this.panelSearchOutput.Size = new System.Drawing.Size(788, 73);
            this.panelSearchOutput.TabIndex = 10;
            // 
            // toolStripContainerMain
            // 
            // 
            // toolStripContainerMain.BottomToolStripPanel
            // 
            this.toolStripContainerMain.BottomToolStripPanel.Controls.Add(this.statusStrip);
            // 
            // toolStripContainerMain.ContentPanel
            // 
            this.toolStripContainerMain.ContentPanel.AutoScroll = true;
            this.toolStripContainerMain.ContentPanel.Controls.Add(this.splitContainerTileQueue_Browser);
            this.toolStripContainerMain.ContentPanel.Controls.Add(this.panelSearchOutput);
            this.toolStripContainerMain.ContentPanel.Size = new System.Drawing.Size(788, 487);
            this.toolStripContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainerMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainerMain.Name = "toolStripContainerMain";
            this.toolStripContainerMain.Size = new System.Drawing.Size(788, 569);
            this.toolStripContainerMain.TabIndex = 11;
            this.toolStripContainerMain.Text = "toolStripContainer1";
            // 
            // toolStripContainerMain.TopToolStripPanel
            // 
            this.toolStripContainerMain.TopToolStripPanel.Controls.Add(this.menuStripMain);
            this.toolStripContainerMain.TopToolStripPanel.Controls.Add(this.toolStripSave);
            // 
            // toolStripSave
            // 
            this.toolStripSave.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStripSave.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripSave.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonSaveAs});
            this.toolStripSave.Location = new System.Drawing.Point(0, 24);
            this.toolStripSave.Name = "toolStripSave";
            this.toolStripSave.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripSave.Size = new System.Drawing.Size(788, 36);
            this.toolStripSave.Stretch = true;
            this.toolStripSave.TabIndex = 0;
            // 
            // toolStripButtonSaveAs
            // 
            this.toolStripButtonSaveAs.Image = global::AlbumArtDownloader.Properties.Resources.saveHS1;
            this.toolStripButtonSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSaveAs.Name = "toolStripButtonSaveAs";
            this.toolStripButtonSaveAs.Size = new System.Drawing.Size(64, 33);
            this.toolStripButtonSaveAs.Text = "Save as ...";
            this.toolStripButtonSaveAs.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButtonSaveAs.Click += new System.EventHandler(this.toolStripButtonSaveAs_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "JPEG Files|*.jpg;*.jpeg";
            // 
            // MainForm
            // 
            this.AcceptButton = this.buttonAddTask;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(788, 569);
            this.Controls.Add(this.toolStripContainerMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripMain;
            this.Name = "MainForm";
            this.Text = "Album Art Downloader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBoxSearch.ResumeLayout(false);
            this.groupBoxSearch.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.groupBoxOutput.ResumeLayout(false);
            this.groupBoxOutput.PerformLayout();
            this.contextMenuStripPicture.ResumeLayout(false);
            this.contextMenuStripQueue.ResumeLayout(false);
            this.splitContainerTileQueue_Browser.Panel1.ResumeLayout(false);
            this.splitContainerTileQueue_Browser.Panel2.ResumeLayout(false);
            this.splitContainerTileQueue_Browser.ResumeLayout(false);
            this.splitContainerTile_Queue.Panel1.ResumeLayout(false);
            this.splitContainerTile_Queue.Panel2.ResumeLayout(false);
            this.splitContainerTile_Queue.ResumeLayout(false);
            this.tabControlBrowser.ResumeLayout(false);
            this.tabPageBrowseFolder.ResumeLayout(false);
            this.tabPageBrowseFolder.PerformLayout();
            this.toolStripBrowserPath.ResumeLayout(false);
            this.toolStripBrowserPath.PerformLayout();
            this.tabPageCOMServer.ResumeLayout(false);
            this.tabPageCOMServer.PerformLayout();
            this.toolStripBrowserCOM.ResumeLayout(false);
            this.toolStripBrowserCOM.PerformLayout();
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.panelSearchOutput.ResumeLayout(false);
            this.toolStripContainerMain.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainerMain.BottomToolStripPanel.PerformLayout();
            this.toolStripContainerMain.ContentPanel.ResumeLayout(false);
            this.toolStripContainerMain.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainerMain.TopToolStripPanel.PerformLayout();
            this.toolStripContainerMain.ResumeLayout(false);
            this.toolStripContainerMain.PerformLayout();
            this.toolStripSave.ResumeLayout(false);
            this.toolStripSave.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxArtist;
        private System.Windows.Forms.TextBox textBoxAlbum;
        private System.Windows.Forms.Button buttonAddTask;
        private System.Windows.Forms.GroupBox groupBoxSearch;
        private System.Windows.Forms.Label labelSaveTo;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Label labelArtist;
        private System.Windows.Forms.Label labelAlbum;
        private System.Windows.Forms.GroupBox groupBoxOutput;
        private System.Windows.Forms.Button buttonSaveToBrowse;
        private System.Windows.Forms.Timer timerClose;
        private System.Windows.Forms.ImageList imageListTile;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripPicture;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSaveAs;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPreview;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripQueue;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemView;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemCancel;
        private MegaList megaListTiles;
        private System.Windows.Forms.ColumnHeader clhTiles;
        private System.Windows.Forms.ColumnHeader clhQueueArtist;
        private System.Windows.Forms.ColumnHeader clhQueueAlbum;
        private System.Windows.Forms.SplitContainer splitContainerTileQueue_Browser;
        private System.Windows.Forms.SplitContainer splitContainerTile_Queue;
        private MegaList megaListBrowserCOM;
        private System.Windows.Forms.ColumnHeader clhBrowserCOMArtist;
        private System.Windows.Forms.ColumnHeader clhBrowserCOMAlbum;
        private System.Windows.Forms.ColumnHeader clhBrowserCOMArt;
        private System.Windows.Forms.ImageList imageListHasAlbumArt;
        private System.ComponentModel.BackgroundWorker BrowserWorkerCOM;
        private System.Windows.Forms.ToolStrip toolStripBrowserCOM;
        private System.Windows.Forms.ToolStripButton BrowserCOMRefresh;
        private System.Windows.Forms.ToolStripButton BrowserCOMCancel;
        private System.Windows.Forms.ToolStripButton BrowserCOMAddArtless;
        private System.Windows.Forms.ToolStripProgressBar BrowserCOMProgress;
        private System.Windows.Forms.ToolStripStatusLabel MainStatus;
        private System.Windows.Forms.ToolStripProgressBar MainProgress;
        private System.Windows.Forms.ImageList QueueImageList;
        private System.ComponentModel.BackgroundWorker BrowserWorkerPath;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogFolder;
        private System.Windows.Forms.TabControl tabControlBrowser;
        private System.Windows.Forms.TabPage tabPageCOMServer;
        private System.Windows.Forms.TabPage tabPageBrowseFolder;
        private MegaList megaListBrowserPath;
        private System.Windows.Forms.ColumnHeader clhBrowserPathArtist;
        private System.Windows.Forms.ColumnHeader clhBrowserPathAlbum;
        private System.Windows.Forms.ColumnHeader clhBrowserPathArt;
        private System.Windows.Forms.ToolStrip toolStripBrowserPath;
        private System.Windows.Forms.ToolStripButton BrowserPathRefresh;
        private System.Windows.Forms.ToolStripButton BrowserPathCancel;
        private System.Windows.Forms.ToolStripButton BrowserPathAddArtless;
        private System.Windows.Forms.ToolStripProgressBar BrowserPathProgress;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem queueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem browserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Panel panelSearchOutput;
        private System.Windows.Forms.ListView listViewQueue;
        private System.Windows.Forms.ColumnHeader clhBrowserPathSize;
        private System.Windows.Forms.ColumnHeader clhBrowserCOMSize;
        private System.Windows.Forms.ToolStripContainer toolStripContainerMain;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveAs;
        private System.Windows.Forms.ToolStripMenuItem saveToolbarToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        internal System.Windows.Forms.ToolStrip toolStripSave;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogSaveTo;
        internal System.Windows.Forms.TextBox textBoxFileSave;
    }
}

