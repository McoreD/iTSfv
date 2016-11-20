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
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.tabControlSettings = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.groupBoxGeneralSettings = new System.Windows.Forms.GroupBox();
            this.labelMaxResults = new System.Windows.Forms.Label();
            this.numericUpDownMaxResults = new System.Windows.Forms.NumericUpDown();
            this.checkBoxShowFolderPicturesRecursiv = new System.Windows.Forms.CheckBox();
            this.checkBoxBold = new System.Windows.Forms.CheckBox();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.checkBoxAutoDownloadFullImage = new System.Windows.Forms.CheckBox();
            this.checkBoxShowFolderPictures = new System.Windows.Forms.CheckBox();
            this.numericUpDownThumbnailWidth = new System.Windows.Forms.NumericUpDown();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.numericUpDownThumbnailHeight = new System.Windows.Forms.NumericUpDown();
            this.checkBoxClose = new System.Windows.Forms.CheckBox();
            this.checkBoxShowExistingArt = new System.Windows.Forms.CheckBox();
            this.groupBoxSizeOverlay = new System.Windows.Forms.GroupBox();
            this.panelSizeOverlaySettings = new System.Windows.Forms.Panel();
            this.pictureBoxPreviewSizeOverlayColor = new System.Windows.Forms.PictureBox();
            this.labelSizeOverlayColorForeground = new System.Windows.Forms.Label();
            this.labelSizeOverlayPreview = new System.Windows.Forms.Label();
            this.buttonSizeOverlayColorForeground = new System.Windows.Forms.Button();
            this.textBoxSizeOverlayColorForeground = new System.Windows.Forms.TextBox();
            this.checkBoxShowSizeOverlay = new System.Windows.Forms.CheckBox();
            this.tabPageScriptManager = new System.Windows.Forms.TabPage();
            this.splitContainerScriptManager = new System.Windows.Forms.SplitContainer();
            this.listScripts = new System.Windows.Forms.CheckedListBox();
            this.panelScriptUpDown = new System.Windows.Forms.Panel();
            this.buttonScriptDown = new System.Windows.Forms.Button();
            this.buttonScriptUp = new System.Windows.Forms.Button();
            this.panelScriptManager = new System.Windows.Forms.Panel();
            this.labelScriptCreator = new System.Windows.Forms.Label();
            this.labelScriptVersion = new System.Windows.Forms.Label();
            this.labelScriptName = new System.Windows.Forms.Label();
            this.tabPageSaveToolbar = new System.Windows.Forms.TabPage();
            this.listViewSaveButtons = new System.Windows.Forms.ListView();
            this.columnHeaderFolder = new System.Windows.Forms.ColumnHeader();
            this.panelSaveUpDown = new System.Windows.Forms.Panel();
            this.buttonSaveDown = new System.Windows.Forms.Button();
            this.buttonSaveUp = new System.Windows.Forms.Button();
            this.panelSaveEditButtons = new System.Windows.Forms.Panel();
            this.buttonSaveEdit = new System.Windows.Forms.Button();
            this.buttonSaveAdd = new System.Windows.Forms.Button();
            this.buttonSaveRemove = new System.Windows.Forms.Button();
            this.clhScriptName = new System.Windows.Forms.ColumnHeader();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.colorDialogSizeOverlay = new System.Windows.Forms.ColorDialog();
            this.tabControlSettings.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.groupBoxGeneralSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThumbnailWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThumbnailHeight)).BeginInit();
            this.groupBoxSizeOverlay.SuspendLayout();
            this.panelSizeOverlaySettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreviewSizeOverlayColor)).BeginInit();
            this.tabPageScriptManager.SuspendLayout();
            this.splitContainerScriptManager.Panel1.SuspendLayout();
            this.splitContainerScriptManager.Panel2.SuspendLayout();
            this.splitContainerScriptManager.SuspendLayout();
            this.panelScriptUpDown.SuspendLayout();
            this.panelScriptManager.SuspendLayout();
            this.tabPageSaveToolbar.SuspendLayout();
            this.panelSaveUpDown.SuspendLayout();
            this.panelSaveEditButtons.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 162);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Thumbnail Size";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(136, 162);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "x";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 136);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Use";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(88, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "threads";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 189);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(102, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Default Art Filename";
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.AutoSize = true;
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(272, 3);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(82, 27);
            this.buttonClose.TabIndex = 20;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // tabControlSettings
            // 
            this.tabControlSettings.Controls.Add(this.tabPageGeneral);
            this.tabControlSettings.Controls.Add(this.tabPageScriptManager);
            this.tabControlSettings.Controls.Add(this.tabPageSaveToolbar);
            this.tabControlSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlSettings.Location = new System.Drawing.Point(0, 0);
            this.tabControlSettings.Name = "tabControlSettings";
            this.tabControlSettings.SelectedIndex = 0;
            this.tabControlSettings.Size = new System.Drawing.Size(358, 346);
            this.tabControlSettings.TabIndex = 21;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.groupBoxGeneralSettings);
            this.tabPageGeneral.Controls.Add(this.groupBoxSizeOverlay);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGeneral.Size = new System.Drawing.Size(350, 320);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // groupBoxGeneralSettings
            // 
            this.groupBoxGeneralSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxGeneralSettings.Controls.Add(this.labelMaxResults);
            this.groupBoxGeneralSettings.Controls.Add(this.numericUpDownMaxResults);
            this.groupBoxGeneralSettings.Controls.Add(this.checkBoxShowFolderPicturesRecursiv);
            this.groupBoxGeneralSettings.Controls.Add(this.checkBoxBold);
            this.groupBoxGeneralSettings.Controls.Add(this.numericUpDown3);
            this.groupBoxGeneralSettings.Controls.Add(this.checkBoxAutoDownloadFullImage);
            this.groupBoxGeneralSettings.Controls.Add(this.label1);
            this.groupBoxGeneralSettings.Controls.Add(this.checkBoxShowFolderPictures);
            this.groupBoxGeneralSettings.Controls.Add(this.numericUpDownThumbnailWidth);
            this.groupBoxGeneralSettings.Controls.Add(this.textBox1);
            this.groupBoxGeneralSettings.Controls.Add(this.numericUpDownThumbnailHeight);
            this.groupBoxGeneralSettings.Controls.Add(this.checkBoxClose);
            this.groupBoxGeneralSettings.Controls.Add(this.label7);
            this.groupBoxGeneralSettings.Controls.Add(this.label2);
            this.groupBoxGeneralSettings.Controls.Add(this.label4);
            this.groupBoxGeneralSettings.Controls.Add(this.checkBoxShowExistingArt);
            this.groupBoxGeneralSettings.Controls.Add(this.label3);
            this.groupBoxGeneralSettings.Location = new System.Drawing.Point(6, 6);
            this.groupBoxGeneralSettings.Name = "groupBoxGeneralSettings";
            this.groupBoxGeneralSettings.Size = new System.Drawing.Size(338, 239);
            this.groupBoxGeneralSettings.TabIndex = 29;
            this.groupBoxGeneralSettings.TabStop = false;
            this.groupBoxGeneralSettings.Text = "General Settings";
            // 
            // labelMaxResults
            // 
            this.labelMaxResults.AutoSize = true;
            this.labelMaxResults.Location = new System.Drawing.Point(3, 214);
            this.labelMaxResults.Name = "labelMaxResults";
            this.labelMaxResults.Size = new System.Drawing.Size(89, 13);
            this.labelMaxResults.TabIndex = 22;
            this.labelMaxResults.Text = "Maximum Results";
            // 
            // numericUpDownMaxResults
            // 
            this.numericUpDownMaxResults.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::AlbumArtDownloader.Properties.Settings.Default, "MaxResults", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDownMaxResults.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownMaxResults.Location = new System.Drawing.Point(98, 212);
            this.numericUpDownMaxResults.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownMaxResults.Name = "numericUpDownMaxResults";
            this.numericUpDownMaxResults.Size = new System.Drawing.Size(47, 20);
            this.numericUpDownMaxResults.TabIndex = 23;
            this.numericUpDownMaxResults.Value = global::AlbumArtDownloader.Properties.Settings.Default.MaxResults;
            // 
            // checkBoxShowFolderPicturesRecursiv
            // 
            this.checkBoxShowFolderPicturesRecursiv.AutoSize = true;
            this.checkBoxShowFolderPicturesRecursiv.Checked = global::AlbumArtDownloader.Properties.Settings.Default.ShowFolderPicturesRecursiv;
            this.checkBoxShowFolderPicturesRecursiv.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowFolderPicturesRecursiv.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "ShowFolderPicturesRecursiv", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxShowFolderPicturesRecursiv.Location = new System.Drawing.Point(145, 65);
            this.checkBoxShowFolderPicturesRecursiv.Name = "checkBoxShowFolderPicturesRecursiv";
            this.checkBoxShowFolderPicturesRecursiv.Size = new System.Drawing.Size(74, 17);
            this.checkBoxShowFolderPicturesRecursiv.TabIndex = 21;
            this.checkBoxShowFolderPicturesRecursiv.Text = "Recursive";
            this.checkBoxShowFolderPicturesRecursiv.UseVisualStyleBackColor = true;
            // 
            // checkBoxBold
            // 
            this.checkBoxBold.AutoSize = true;
            this.checkBoxBold.Checked = global::AlbumArtDownloader.Properties.Settings.Default.ExactMatchBold;
            this.checkBoxBold.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "ExactMatchBold", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxBold.Location = new System.Drawing.Point(6, 19);
            this.checkBoxBold.Name = "checkBoxBold";
            this.checkBoxBold.Size = new System.Drawing.Size(160, 17);
            this.checkBoxBold.TabIndex = 13;
            this.checkBoxBold.Text = "Show exact matches in Bold";
            this.checkBoxBold.UseVisualStyleBackColor = true;
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::AlbumArtDownloader.Properties.Settings.Default, "ThreadCount", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDown3.Location = new System.Drawing.Point(35, 134);
            this.numericUpDown3.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericUpDown3.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(47, 20);
            this.numericUpDown3.TabIndex = 10;
            this.numericUpDown3.Value = global::AlbumArtDownloader.Properties.Settings.Default.ThreadCount;
            // 
            // checkBoxAutoDownloadFullImage
            // 
            this.checkBoxAutoDownloadFullImage.AutoSize = true;
            this.checkBoxAutoDownloadFullImage.Checked = global::AlbumArtDownloader.Properties.Settings.Default.AutoDownloadFullImage;
            this.checkBoxAutoDownloadFullImage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoDownloadFullImage.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "AutoDownloadFullImage", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxAutoDownloadFullImage.Location = new System.Drawing.Point(6, 88);
            this.checkBoxAutoDownloadFullImage.Name = "checkBoxAutoDownloadFullImage";
            this.checkBoxAutoDownloadFullImage.Size = new System.Drawing.Size(333, 17);
            this.checkBoxAutoDownloadFullImage.TabIndex = 20;
            this.checkBoxAutoDownloadFullImage.Text = "Automatically download the full image to determine size if required";
            this.checkBoxAutoDownloadFullImage.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowFolderPictures
            // 
            this.checkBoxShowFolderPictures.AutoSize = true;
            this.checkBoxShowFolderPictures.Checked = global::AlbumArtDownloader.Properties.Settings.Default.ShowFolderPictures;
            this.checkBoxShowFolderPictures.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowFolderPictures.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "ShowFolderPictures", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxShowFolderPictures.Location = new System.Drawing.Point(6, 65);
            this.checkBoxShowFolderPictures.Name = "checkBoxShowFolderPictures";
            this.checkBoxShowFolderPictures.Size = new System.Drawing.Size(133, 17);
            this.checkBoxShowFolderPictures.TabIndex = 18;
            this.checkBoxShowFolderPictures.Text = "Show pictures in folder";
            this.checkBoxShowFolderPictures.UseVisualStyleBackColor = true;
            // 
            // numericUpDownThumbnailWidth
            // 
            this.numericUpDownThumbnailWidth.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::AlbumArtDownloader.Properties.Settings.Default, "ThumbnailWidth", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDownThumbnailWidth.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownThumbnailWidth.Location = new System.Drawing.Point(83, 160);
            this.numericUpDownThumbnailWidth.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownThumbnailWidth.Name = "numericUpDownThumbnailWidth";
            this.numericUpDownThumbnailWidth.Size = new System.Drawing.Size(47, 20);
            this.numericUpDownThumbnailWidth.TabIndex = 5;
            this.numericUpDownThumbnailWidth.Value = global::AlbumArtDownloader.Properties.Settings.Default.ThumbnailWidth;
            // 
            // textBox1
            // 
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::AlbumArtDownloader.Properties.Settings.Default, "SaveFileName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox1.Location = new System.Drawing.Point(111, 186);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(127, 20);
            this.textBox1.TabIndex = 16;
            this.textBox1.Text = global::AlbumArtDownloader.Properties.Settings.Default.SaveFileName;
            // 
            // numericUpDownThumbnailHeight
            // 
            this.numericUpDownThumbnailHeight.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::AlbumArtDownloader.Properties.Settings.Default, "ThumbnailHeight", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDownThumbnailHeight.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownThumbnailHeight.Location = new System.Drawing.Point(154, 160);
            this.numericUpDownThumbnailHeight.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownThumbnailHeight.Name = "numericUpDownThumbnailHeight";
            this.numericUpDownThumbnailHeight.Size = new System.Drawing.Size(47, 20);
            this.numericUpDownThumbnailHeight.TabIndex = 7;
            this.numericUpDownThumbnailHeight.Value = global::AlbumArtDownloader.Properties.Settings.Default.ThumbnailHeight;
            // 
            // checkBoxClose
            // 
            this.checkBoxClose.AutoSize = true;
            this.checkBoxClose.Checked = global::AlbumArtDownloader.Properties.Settings.Default.CloseAfterSaving;
            this.checkBoxClose.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "CloseAfterSaving", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxClose.Location = new System.Drawing.Point(6, 111);
            this.checkBoxClose.Name = "checkBoxClose";
            this.checkBoxClose.Size = new System.Drawing.Size(125, 17);
            this.checkBoxClose.TabIndex = 8;
            this.checkBoxClose.Text = "Close after saving art";
            this.checkBoxClose.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowExistingArt
            // 
            this.checkBoxShowExistingArt.AutoSize = true;
            this.checkBoxShowExistingArt.Checked = global::AlbumArtDownloader.Properties.Settings.Default.PreviewSavedArt;
            this.checkBoxShowExistingArt.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowExistingArt.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "PreviewSavedArt", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxShowExistingArt.Location = new System.Drawing.Point(6, 42);
            this.checkBoxShowExistingArt.Name = "checkBoxShowExistingArt";
            this.checkBoxShowExistingArt.Size = new System.Drawing.Size(106, 17);
            this.checkBoxShowExistingArt.TabIndex = 3;
            this.checkBoxShowExistingArt.Text = "Show existing art";
            this.checkBoxShowExistingArt.UseVisualStyleBackColor = true;
            // 
            // groupBoxSizeOverlay
            // 
            this.groupBoxSizeOverlay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSizeOverlay.Controls.Add(this.panelSizeOverlaySettings);
            this.groupBoxSizeOverlay.Controls.Add(this.checkBoxShowSizeOverlay);
            this.groupBoxSizeOverlay.Location = new System.Drawing.Point(6, 251);
            this.groupBoxSizeOverlay.Name = "groupBoxSizeOverlay";
            this.groupBoxSizeOverlay.Size = new System.Drawing.Size(338, 64);
            this.groupBoxSizeOverlay.TabIndex = 28;
            this.groupBoxSizeOverlay.TabStop = false;
            this.groupBoxSizeOverlay.Text = "Size Overlay";
            // 
            // panelSizeOverlaySettings
            // 
            this.panelSizeOverlaySettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSizeOverlaySettings.Controls.Add(this.pictureBoxPreviewSizeOverlayColor);
            this.panelSizeOverlaySettings.Controls.Add(this.labelSizeOverlayColorForeground);
            this.panelSizeOverlaySettings.Controls.Add(this.labelSizeOverlayPreview);
            this.panelSizeOverlaySettings.Controls.Add(this.buttonSizeOverlayColorForeground);
            this.panelSizeOverlaySettings.Controls.Add(this.textBoxSizeOverlayColorForeground);
            this.panelSizeOverlaySettings.Location = new System.Drawing.Point(0, 38);
            this.panelSizeOverlaySettings.Name = "panelSizeOverlaySettings";
            this.panelSizeOverlaySettings.Size = new System.Drawing.Size(336, 21);
            this.panelSizeOverlaySettings.TabIndex = 30;
            // 
            // pictureBoxPreviewSizeOverlayColor
            // 
            this.pictureBoxPreviewSizeOverlayColor.BackColor = System.Drawing.Color.White;
            this.pictureBoxPreviewSizeOverlayColor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBoxPreviewSizeOverlayColor.Location = new System.Drawing.Point(234, 0);
            this.pictureBoxPreviewSizeOverlayColor.Name = "pictureBoxPreviewSizeOverlayColor";
            this.pictureBoxPreviewSizeOverlayColor.Size = new System.Drawing.Size(90, 21);
            this.pictureBoxPreviewSizeOverlayColor.TabIndex = 24;
            this.pictureBoxPreviewSizeOverlayColor.TabStop = false;
            this.pictureBoxPreviewSizeOverlayColor.Tag = "FFFFFF";
            // 
            // labelSizeOverlayColorForeground
            // 
            this.labelSizeOverlayColorForeground.AutoSize = true;
            this.labelSizeOverlayColorForeground.Location = new System.Drawing.Point(3, 4);
            this.labelSizeOverlayColorForeground.Name = "labelSizeOverlayColorForeground";
            this.labelSizeOverlayColorForeground.Size = new System.Drawing.Size(93, 13);
            this.labelSizeOverlayColorForeground.TabIndex = 21;
            this.labelSizeOverlayColorForeground.Text = "Size Overlay Color";
            // 
            // labelSizeOverlayPreview
            // 
            this.labelSizeOverlayPreview.AutoSize = true;
            this.labelSizeOverlayPreview.Location = new System.Drawing.Point(189, 4);
            this.labelSizeOverlayPreview.Name = "labelSizeOverlayPreview";
            this.labelSizeOverlayPreview.Size = new System.Drawing.Size(48, 13);
            this.labelSizeOverlayPreview.TabIndex = 29;
            this.labelSizeOverlayPreview.Text = "Preview:";
            // 
            // buttonSizeOverlayColorForeground
            // 
            this.buttonSizeOverlayColorForeground.Location = new System.Drawing.Point(154, -1);
            this.buttonSizeOverlayColorForeground.Name = "buttonSizeOverlayColorForeground";
            this.buttonSizeOverlayColorForeground.Size = new System.Drawing.Size(29, 23);
            this.buttonSizeOverlayColorForeground.TabIndex = 23;
            this.buttonSizeOverlayColorForeground.Text = "...";
            this.buttonSizeOverlayColorForeground.UseVisualStyleBackColor = true;
            this.buttonSizeOverlayColorForeground.Click += new System.EventHandler(this.buttonSizeOverlayColorForeground_Click);
            // 
            // textBoxSizeOverlayColorForeground
            // 
            this.textBoxSizeOverlayColorForeground.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::AlbumArtDownloader.Properties.Settings.Default, "SizeOverlayColorForeground", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxSizeOverlayColorForeground.Location = new System.Drawing.Point(103, 1);
            this.textBoxSizeOverlayColorForeground.Name = "textBoxSizeOverlayColorForeground";
            this.textBoxSizeOverlayColorForeground.Size = new System.Drawing.Size(45, 20);
            this.textBoxSizeOverlayColorForeground.TabIndex = 22;
            this.textBoxSizeOverlayColorForeground.Text = global::AlbumArtDownloader.Properties.Settings.Default.SizeOverlayColorForeground;
            this.textBoxSizeOverlayColorForeground.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxSizeOverlayColorForeground.TextChanged += new System.EventHandler(this.textBoxSizeOverlayColor_TextChanged);
            // 
            // checkBoxShowSizeOverlay
            // 
            this.checkBoxShowSizeOverlay.AutoSize = true;
            this.checkBoxShowSizeOverlay.Checked = global::AlbumArtDownloader.Properties.Settings.Default.ShowSizeOverlay;
            this.checkBoxShowSizeOverlay.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowSizeOverlay.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::AlbumArtDownloader.Properties.Settings.Default, "ShowSizeOverlay", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxShowSizeOverlay.Location = new System.Drawing.Point(6, 19);
            this.checkBoxShowSizeOverlay.Name = "checkBoxShowSizeOverlay";
            this.checkBoxShowSizeOverlay.Size = new System.Drawing.Size(115, 17);
            this.checkBoxShowSizeOverlay.TabIndex = 19;
            this.checkBoxShowSizeOverlay.Text = "Show Size Overlay";
            this.checkBoxShowSizeOverlay.UseVisualStyleBackColor = true;
            this.checkBoxShowSizeOverlay.CheckedChanged += new System.EventHandler(this.checkBoxShowSizeOverlay_CheckedChanged);
            // 
            // tabPageScriptManager
            // 
            this.tabPageScriptManager.Controls.Add(this.splitContainerScriptManager);
            this.tabPageScriptManager.Location = new System.Drawing.Point(4, 22);
            this.tabPageScriptManager.Name = "tabPageScriptManager";
            this.tabPageScriptManager.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageScriptManager.Size = new System.Drawing.Size(350, 320);
            this.tabPageScriptManager.TabIndex = 1;
            this.tabPageScriptManager.Text = "Script Manager";
            this.tabPageScriptManager.UseVisualStyleBackColor = true;
            // 
            // splitContainerScriptManager
            // 
            this.splitContainerScriptManager.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerScriptManager.Location = new System.Drawing.Point(3, 3);
            this.splitContainerScriptManager.Name = "splitContainerScriptManager";
            // 
            // splitContainerScriptManager.Panel1
            // 
            this.splitContainerScriptManager.Panel1.Controls.Add(this.listScripts);
            this.splitContainerScriptManager.Panel1.Controls.Add(this.panelScriptUpDown);
            // 
            // splitContainerScriptManager.Panel2
            // 
            this.splitContainerScriptManager.Panel2.Controls.Add(this.panelScriptManager);
            this.splitContainerScriptManager.Size = new System.Drawing.Size(344, 314);
            this.splitContainerScriptManager.SplitterDistance = 168;
            this.splitContainerScriptManager.TabIndex = 1;
            // 
            // listScripts
            // 
            this.listScripts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listScripts.Location = new System.Drawing.Point(0, 0);
            this.listScripts.Name = "listScripts";
            this.listScripts.Size = new System.Drawing.Size(133, 304);
            this.listScripts.TabIndex = 0;
            this.listScripts.SelectedIndexChanged += new System.EventHandler(this.listScripts_SelectedIndexChanged);
            this.listScripts.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listScripts_ItemCheck);
            this.listScripts.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listScripts_MouseDown);
            this.listScripts.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listScripts_KeyUp);
            this.listScripts.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listScripts_KeyDown);
            // 
            // panelScriptUpDown
            // 
            this.panelScriptUpDown.Controls.Add(this.buttonScriptDown);
            this.panelScriptUpDown.Controls.Add(this.buttonScriptUp);
            this.panelScriptUpDown.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelScriptUpDown.Location = new System.Drawing.Point(133, 0);
            this.panelScriptUpDown.Name = "panelScriptUpDown";
            this.panelScriptUpDown.Size = new System.Drawing.Size(35, 314);
            this.panelScriptUpDown.TabIndex = 6;
            // 
            // buttonScriptDown
            // 
            this.buttonScriptDown.Image = global::AlbumArtDownloader.Properties.Resources.arrMoveDown;
            this.buttonScriptDown.Location = new System.Drawing.Point(3, 74);
            this.buttonScriptDown.Name = "buttonScriptDown";
            this.buttonScriptDown.Size = new System.Drawing.Size(29, 65);
            this.buttonScriptDown.TabIndex = 1;
            this.buttonScriptDown.UseVisualStyleBackColor = true;
            this.buttonScriptDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // buttonScriptUp
            // 
            this.buttonScriptUp.Image = global::AlbumArtDownloader.Properties.Resources.arrMoveUp;
            this.buttonScriptUp.Location = new System.Drawing.Point(3, 3);
            this.buttonScriptUp.Name = "buttonScriptUp";
            this.buttonScriptUp.Size = new System.Drawing.Size(29, 65);
            this.buttonScriptUp.TabIndex = 0;
            this.buttonScriptUp.UseVisualStyleBackColor = true;
            this.buttonScriptUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // panelScriptManager
            // 
            this.panelScriptManager.Controls.Add(this.labelScriptCreator);
            this.panelScriptManager.Controls.Add(this.labelScriptVersion);
            this.panelScriptManager.Controls.Add(this.labelScriptName);
            this.panelScriptManager.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelScriptManager.Location = new System.Drawing.Point(0, 0);
            this.panelScriptManager.Name = "panelScriptManager";
            this.panelScriptManager.Size = new System.Drawing.Size(172, 314);
            this.panelScriptManager.TabIndex = 0;
            this.panelScriptManager.Visible = false;
            // 
            // labelScriptCreator
            // 
            this.labelScriptCreator.AutoSize = true;
            this.labelScriptCreator.Location = new System.Drawing.Point(3, 39);
            this.labelScriptCreator.Name = "labelScriptCreator";
            this.labelScriptCreator.Size = new System.Drawing.Size(44, 13);
            this.labelScriptCreator.TabIndex = 2;
            this.labelScriptCreator.Text = "Creator:";
            // 
            // labelScriptVersion
            // 
            this.labelScriptVersion.AutoSize = true;
            this.labelScriptVersion.Location = new System.Drawing.Point(3, 21);
            this.labelScriptVersion.Name = "labelScriptVersion";
            this.labelScriptVersion.Size = new System.Drawing.Size(45, 13);
            this.labelScriptVersion.TabIndex = 1;
            this.labelScriptVersion.Text = "Version:";
            // 
            // labelScriptName
            // 
            this.labelScriptName.AutoSize = true;
            this.labelScriptName.Location = new System.Drawing.Point(3, 3);
            this.labelScriptName.Name = "labelScriptName";
            this.labelScriptName.Size = new System.Drawing.Size(38, 13);
            this.labelScriptName.TabIndex = 0;
            this.labelScriptName.Text = "Name:";
            // 
            // tabPageSaveToolbar
            // 
            this.tabPageSaveToolbar.Controls.Add(this.listViewSaveButtons);
            this.tabPageSaveToolbar.Controls.Add(this.panelSaveUpDown);
            this.tabPageSaveToolbar.Controls.Add(this.panelSaveEditButtons);
            this.tabPageSaveToolbar.Location = new System.Drawing.Point(4, 22);
            this.tabPageSaveToolbar.Name = "tabPageSaveToolbar";
            this.tabPageSaveToolbar.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSaveToolbar.Size = new System.Drawing.Size(350, 320);
            this.tabPageSaveToolbar.TabIndex = 2;
            this.tabPageSaveToolbar.Text = "Save Toolbar";
            this.tabPageSaveToolbar.UseVisualStyleBackColor = true;
            // 
            // listViewSaveButtons
            // 
            this.listViewSaveButtons.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderFolder});
            this.listViewSaveButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewSaveButtons.FullRowSelect = true;
            this.listViewSaveButtons.GridLines = true;
            this.listViewSaveButtons.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewSaveButtons.HideSelection = false;
            this.listViewSaveButtons.Location = new System.Drawing.Point(3, 3);
            this.listViewSaveButtons.MultiSelect = false;
            this.listViewSaveButtons.Name = "listViewSaveButtons";
            this.listViewSaveButtons.Size = new System.Drawing.Size(309, 284);
            this.listViewSaveButtons.TabIndex = 7;
            this.listViewSaveButtons.UseCompatibleStateImageBehavior = false;
            this.listViewSaveButtons.View = System.Windows.Forms.View.Details;
            this.listViewSaveButtons.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listViewSaveButtons_AfterLabelEdit);
            // 
            // columnHeaderFolder
            // 
            this.columnHeaderFolder.Text = "Folder";
            this.columnHeaderFolder.Width = 305;
            // 
            // panelSaveUpDown
            // 
            this.panelSaveUpDown.Controls.Add(this.buttonSaveDown);
            this.panelSaveUpDown.Controls.Add(this.buttonSaveUp);
            this.panelSaveUpDown.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelSaveUpDown.Location = new System.Drawing.Point(312, 3);
            this.panelSaveUpDown.Name = "panelSaveUpDown";
            this.panelSaveUpDown.Size = new System.Drawing.Size(35, 284);
            this.panelSaveUpDown.TabIndex = 8;
            // 
            // buttonSaveDown
            // 
            this.buttonSaveDown.Image = global::AlbumArtDownloader.Properties.Resources.arrMoveDown;
            this.buttonSaveDown.Location = new System.Drawing.Point(3, 74);
            this.buttonSaveDown.Name = "buttonSaveDown";
            this.buttonSaveDown.Size = new System.Drawing.Size(29, 65);
            this.buttonSaveDown.TabIndex = 1;
            this.buttonSaveDown.UseVisualStyleBackColor = true;
            this.buttonSaveDown.Click += new System.EventHandler(this.buttonSaveDown_Click);
            // 
            // buttonSaveUp
            // 
            this.buttonSaveUp.Image = global::AlbumArtDownloader.Properties.Resources.arrMoveUp;
            this.buttonSaveUp.Location = new System.Drawing.Point(3, 3);
            this.buttonSaveUp.Name = "buttonSaveUp";
            this.buttonSaveUp.Size = new System.Drawing.Size(29, 65);
            this.buttonSaveUp.TabIndex = 0;
            this.buttonSaveUp.UseVisualStyleBackColor = true;
            this.buttonSaveUp.Click += new System.EventHandler(this.buttonSaveUp_Click);
            // 
            // panelSaveEditButtons
            // 
            this.panelSaveEditButtons.Controls.Add(this.buttonSaveEdit);
            this.panelSaveEditButtons.Controls.Add(this.buttonSaveAdd);
            this.panelSaveEditButtons.Controls.Add(this.buttonSaveRemove);
            this.panelSaveEditButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelSaveEditButtons.Location = new System.Drawing.Point(3, 287);
            this.panelSaveEditButtons.Name = "panelSaveEditButtons";
            this.panelSaveEditButtons.Size = new System.Drawing.Size(344, 30);
            this.panelSaveEditButtons.TabIndex = 6;
            // 
            // buttonSaveEdit
            // 
            this.buttonSaveEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveEdit.Location = new System.Drawing.Point(185, 3);
            this.buttonSaveEdit.Name = "buttonSaveEdit";
            this.buttonSaveEdit.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveEdit.TabIndex = 3;
            this.buttonSaveEdit.Text = "Edit";
            this.buttonSaveEdit.UseVisualStyleBackColor = true;
            this.buttonSaveEdit.Click += new System.EventHandler(this.buttonSaveEdit_Click);
            // 
            // buttonSaveAdd
            // 
            this.buttonSaveAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveAdd.Location = new System.Drawing.Point(104, 3);
            this.buttonSaveAdd.Name = "buttonSaveAdd";
            this.buttonSaveAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveAdd.TabIndex = 2;
            this.buttonSaveAdd.Text = "Add";
            this.buttonSaveAdd.UseVisualStyleBackColor = true;
            this.buttonSaveAdd.Click += new System.EventHandler(this.buttonSaveAdd_Click);
            // 
            // buttonSaveRemove
            // 
            this.buttonSaveRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveRemove.Location = new System.Drawing.Point(266, 3);
            this.buttonSaveRemove.Name = "buttonSaveRemove";
            this.buttonSaveRemove.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveRemove.TabIndex = 1;
            this.buttonSaveRemove.Text = "Remove";
            this.buttonSaveRemove.UseVisualStyleBackColor = true;
            this.buttonSaveRemove.Click += new System.EventHandler(this.buttonSaveRemove_Click);
            // 
            // clhScriptName
            // 
            this.clhScriptName.Text = "Name";
            this.clhScriptName.Width = 100;
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.buttonClose);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 346);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(358, 35);
            this.panelButtons.TabIndex = 22;
            // 
            // colorDialogSizeOverlay
            // 
            this.colorDialogSizeOverlay.AnyColor = true;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(358, 381);
            this.Controls.Add(this.tabControlSettings);
            this.Controls.Add(this.panelButtons);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.SizeChanged += new System.EventHandler(this.SettingsForm_SizeChanged);
            this.Validated += new System.EventHandler(this.SettingsDlg_Validated);
            this.tabControlSettings.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.groupBoxGeneralSettings.ResumeLayout(false);
            this.groupBoxGeneralSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThumbnailWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThumbnailHeight)).EndInit();
            this.groupBoxSizeOverlay.ResumeLayout(false);
            this.groupBoxSizeOverlay.PerformLayout();
            this.panelSizeOverlaySettings.ResumeLayout(false);
            this.panelSizeOverlaySettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreviewSizeOverlayColor)).EndInit();
            this.tabPageScriptManager.ResumeLayout(false);
            this.splitContainerScriptManager.Panel1.ResumeLayout(false);
            this.splitContainerScriptManager.Panel2.ResumeLayout(false);
            this.splitContainerScriptManager.ResumeLayout(false);
            this.panelScriptUpDown.ResumeLayout(false);
            this.panelScriptManager.ResumeLayout(false);
            this.panelScriptManager.PerformLayout();
            this.tabPageSaveToolbar.ResumeLayout(false);
            this.panelSaveUpDown.ResumeLayout(false);
            this.panelSaveEditButtons.ResumeLayout(false);
            this.panelButtons.ResumeLayout(false);
            this.panelButtons.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxShowExistingArt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownThumbnailWidth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownThumbnailHeight;
        private System.Windows.Forms.CheckBox checkBoxClose;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxBold;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.TabControl tabControlSettings;
        private System.Windows.Forms.TabPage tabPageGeneral;
        private System.Windows.Forms.TabPage tabPageScriptManager;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.CheckBox checkBoxShowFolderPictures;
        private System.Windows.Forms.CheckedListBox listScripts;
        private System.Windows.Forms.ColumnHeader clhScriptName;
        private System.Windows.Forms.SplitContainer splitContainerScriptManager;
        private System.Windows.Forms.Panel panelScriptManager;
        private System.Windows.Forms.Label labelScriptCreator;
        private System.Windows.Forms.Label labelScriptVersion;
        private System.Windows.Forms.Label labelScriptName;
        private System.Windows.Forms.CheckBox checkBoxShowSizeOverlay;
        private System.Windows.Forms.CheckBox checkBoxAutoDownloadFullImage;
        private System.Windows.Forms.Panel panelScriptUpDown;
        private System.Windows.Forms.Button buttonScriptDown;
		private System.Windows.Forms.Button buttonScriptUp;
        private System.Windows.Forms.ColorDialog colorDialogSizeOverlay;
        private System.Windows.Forms.Label labelSizeOverlayColorForeground;
        private System.Windows.Forms.Button buttonSizeOverlayColorForeground;
        private System.Windows.Forms.TextBox textBoxSizeOverlayColorForeground;
        private System.Windows.Forms.PictureBox pictureBoxPreviewSizeOverlayColor;
        private System.Windows.Forms.GroupBox groupBoxSizeOverlay;
        private System.Windows.Forms.GroupBox groupBoxGeneralSettings;
        private System.Windows.Forms.Label labelSizeOverlayPreview;
        private System.Windows.Forms.Panel panelSizeOverlaySettings;
        private System.Windows.Forms.CheckBox checkBoxShowFolderPicturesRecursiv;
        private System.Windows.Forms.TabPage tabPageSaveToolbar;
        private System.Windows.Forms.ListView listViewSaveButtons;
        private System.Windows.Forms.ColumnHeader columnHeaderFolder;
        private System.Windows.Forms.Panel panelSaveUpDown;
        private System.Windows.Forms.Button buttonSaveDown;
        private System.Windows.Forms.Button buttonSaveUp;
        private System.Windows.Forms.Panel panelSaveEditButtons;
        private System.Windows.Forms.Button buttonSaveEdit;
        private System.Windows.Forms.Button buttonSaveAdd;
        private System.Windows.Forms.Button buttonSaveRemove;
        private System.Windows.Forms.Label labelMaxResults;
        private System.Windows.Forms.NumericUpDown numericUpDownMaxResults;
    }
}