<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmValidator
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmValidator))
        Me.ssAppDisc = New System.Windows.Forms.StatusStrip
        Me.sBarTrack = New System.Windows.Forms.ToolStripStatusLabel
        Me.pBarDiscs = New System.Windows.Forms.ToolStripProgressBar
        Me.pbarTracks = New System.Windows.Forms.ToolStripProgressBar
        Me.btnBrowse = New System.Windows.Forms.Button
        Me.btnValidate = New System.Windows.Forms.Button
        Me.txtFolderPath = New System.Windows.Forms.TextBox
        Me.bwApp = New System.ComponentModel.BackgroundWorker
        Me.tcMain = New System.Windows.Forms.TabControl
        Me.tpChecks = New System.Windows.Forms.TabPage
        Me.chkStandardCheck = New System.Windows.Forms.CheckBox
        Me.tpTracks = New System.Windows.Forms.TabPage
        Me.chkEditCopyArtistToAlbumArtist = New System.Windows.Forms.CheckBox
        Me.chkFillTrackCountEtc = New System.Windows.Forms.CheckBox
        Me.chkLyricsImport = New System.Windows.Forms.CheckBox
        Me.tpFileSystem = New System.Windows.Forms.TabPage
        Me.chkExportLyrics = New System.Windows.Forms.CheckBox
        Me.chkExportIndex = New System.Windows.Forms.CheckBox
        Me.chkExportArtwork = New System.Windows.Forms.CheckBox
        Me.tpDiscsBrowser = New System.Windows.Forms.TabPage
        Me.btnValidateDisc = New System.Windows.Forms.Button
        Me.btnBrowsDisc = New System.Windows.Forms.Button
        Me.pbArtwork = New System.Windows.Forms.PictureBox
        Me.lbDiscs = New System.Windows.Forms.ListBox
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.OpenDirectoryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.LogsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.DebugToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SwitchToITSfvToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.WMPfvToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.TrackReplaceAssistantToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.OptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.VersionHistoryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ttApp = New System.Windows.Forms.ToolTip(Me.components)
        Me.tmrUpdate = New System.Windows.Forms.Timer(Me.components)
        Me.ssAppDisc.SuspendLayout()
        Me.tcMain.SuspendLayout()
        Me.tpChecks.SuspendLayout()
        Me.tpTracks.SuspendLayout()
        Me.tpFileSystem.SuspendLayout()
        Me.tpDiscsBrowser.SuspendLayout()
        CType(Me.pbArtwork, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ssAppDisc
        '
        Me.ssAppDisc.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.sBarTrack, Me.pBarDiscs, Me.pbarTracks})
        Me.ssAppDisc.Location = New System.Drawing.Point(0, 312)
        Me.ssAppDisc.Name = "ssAppDisc"
        Me.ssAppDisc.Size = New System.Drawing.Size(645, 22)
        Me.ssAppDisc.SizingGrip = False
        Me.ssAppDisc.TabIndex = 1
        Me.ssAppDisc.Text = "StatusStrip1"
        '
        'sBarTrack
        '
        Me.sBarTrack.Image = CType(resources.GetObject("sBarTrack.Image"), System.Drawing.Image)
        Me.sBarTrack.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.sBarTrack.Name = "sBarTrack"
        Me.sBarTrack.Size = New System.Drawing.Size(426, 17)
        Me.sBarTrack.Spring = True
        Me.sBarTrack.Text = "Ready. Drag and Drop files or Browse for a folder and press Validate..."
        Me.sBarTrack.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pBarDiscs
        '
        Me.pBarDiscs.Name = "pBarDiscs"
        Me.pBarDiscs.Size = New System.Drawing.Size(100, 16)
        '
        'pbarTracks
        '
        Me.pbarTracks.Name = "pbarTracks"
        Me.pbarTracks.Size = New System.Drawing.Size(100, 16)
        '
        'btnBrowse
        '
        Me.btnBrowse.Location = New System.Drawing.Point(452, 38)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowse.TabIndex = 2
        Me.btnBrowse.Text = "&Browse..."
        Me.btnBrowse.UseVisualStyleBackColor = True
        '
        'btnValidate
        '
        Me.btnValidate.Enabled = False
        Me.btnValidate.Location = New System.Drawing.Point(533, 38)
        Me.btnValidate.Name = "btnValidate"
        Me.btnValidate.Size = New System.Drawing.Size(75, 23)
        Me.btnValidate.TabIndex = 3
        Me.btnValidate.Text = "&Validate"
        Me.btnValidate.UseVisualStyleBackColor = True
        '
        'txtFolderPath
        '
        Me.txtFolderPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.txtFolderPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories
        Me.txtFolderPath.Location = New System.Drawing.Point(10, 41)
        Me.txtFolderPath.Name = "txtFolderPath"
        Me.txtFolderPath.ReadOnly = True
        Me.txtFolderPath.Size = New System.Drawing.Size(436, 20)
        Me.txtFolderPath.TabIndex = 4
        '
        'bwApp
        '
        Me.bwApp.WorkerReportsProgress = True
        Me.bwApp.WorkerSupportsCancellation = True
        '
        'tcMain
        '
        Me.tcMain.Controls.Add(Me.tpChecks)
        Me.tcMain.Controls.Add(Me.tpTracks)
        Me.tcMain.Controls.Add(Me.tpFileSystem)
        Me.tcMain.Controls.Add(Me.tpDiscsBrowser)
        Me.tcMain.Location = New System.Drawing.Point(10, 67)
        Me.tcMain.Name = "tcMain"
        Me.tcMain.SelectedIndex = 0
        Me.tcMain.Size = New System.Drawing.Size(627, 237)
        Me.tcMain.TabIndex = 5
        '
        'tpChecks
        '
        Me.tpChecks.Controls.Add(Me.chkStandardCheck)
        Me.tpChecks.Location = New System.Drawing.Point(4, 22)
        Me.tpChecks.Name = "tpChecks"
        Me.tpChecks.Padding = New System.Windows.Forms.Padding(3)
        Me.tpChecks.Size = New System.Drawing.Size(619, 211)
        Me.tpChecks.TabIndex = 4
        Me.tpChecks.Text = "Checks"
        Me.tpChecks.UseVisualStyleBackColor = True
        '
        'chkStandardCheck
        '
        Me.chkStandardCheck.AutoSize = True
        Me.chkStandardCheck.Checked = Global.iTSfv.My.MySettings.Default.CheckStandard
        Me.chkStandardCheck.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "CheckStandard", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkStandardCheck.Location = New System.Drawing.Point(10, 10)
        Me.chkStandardCheck.Name = "chkStandardCheck"
        Me.chkStandardCheck.Size = New System.Drawing.Size(181, 17)
        Me.chkStandardCheck.TabIndex = 1
        Me.chkStandardCheck.Text = "Check for iTunes Store Standard"
        Me.chkStandardCheck.UseVisualStyleBackColor = True
        '
        'tpTracks
        '
        Me.tpTracks.Controls.Add(Me.chkEditCopyArtistToAlbumArtist)
        Me.tpTracks.Controls.Add(Me.chkFillTrackCountEtc)
        Me.tpTracks.Controls.Add(Me.chkLyricsImport)
        Me.tpTracks.Location = New System.Drawing.Point(4, 22)
        Me.tpTracks.Name = "tpTracks"
        Me.tpTracks.Size = New System.Drawing.Size(619, 211)
        Me.tpTracks.TabIndex = 2
        Me.tpTracks.Text = "Tracks"
        Me.tpTracks.UseVisualStyleBackColor = True
        '
        'chkEditCopyArtistToAlbumArtist
        '
        Me.chkEditCopyArtistToAlbumArtist.AutoSize = True
        Me.chkEditCopyArtistToAlbumArtist.Checked = Global.iTSfv.My.MySettings.Default.LibraryCopyAlbumArtist
        Me.chkEditCopyArtistToAlbumArtist.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkEditCopyArtistToAlbumArtist.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "LibraryCopyAlbumArtist", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkEditCopyArtistToAlbumArtist.Location = New System.Drawing.Point(10, 10)
        Me.chkEditCopyArtistToAlbumArtist.Name = "chkEditCopyArtistToAlbumArtist"
        Me.chkEditCopyArtistToAlbumArtist.Size = New System.Drawing.Size(202, 17)
        Me.chkEditCopyArtistToAlbumArtist.TabIndex = 15
        Me.chkEditCopyArtistToAlbumArtist.Text = "Fill missing AlbumArtist using Artist tag"
        Me.chkEditCopyArtistToAlbumArtist.UseVisualStyleBackColor = True
        '
        'chkFillTrackCountEtc
        '
        Me.chkFillTrackCountEtc.AutoSize = True
        Me.chkFillTrackCountEtc.Checked = Global.iTSfv.My.MySettings.Default.FillTrackCountEtc
        Me.chkFillTrackCountEtc.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkFillTrackCountEtc.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "FillTrackCountEtc", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkFillTrackCountEtc.Location = New System.Drawing.Point(10, 33)
        Me.chkFillTrackCountEtc.Name = "chkFillTrackCountEtc"
        Me.chkFillTrackCountEtc.Size = New System.Drawing.Size(243, 17)
        Me.chkFillTrackCountEtc.TabIndex = 14
        Me.chkFillTrackCountEtc.Text = "Fill Track Count, Disc Number and Disc Count"
        Me.chkFillTrackCountEtc.UseVisualStyleBackColor = True
        '
        'chkLyricsImport
        '
        Me.chkLyricsImport.AutoSize = True
        Me.chkLyricsImport.Checked = Global.iTSfv.My.MySettings.Default.LyricsImport
        Me.chkLyricsImport.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "LyricsImport", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkLyricsImport.Location = New System.Drawing.Point(10, 56)
        Me.chkLyricsImport.Name = "chkLyricsImport"
        Me.chkLyricsImport.Size = New System.Drawing.Size(192, 17)
        Me.chkLyricsImport.TabIndex = 0
        Me.chkLyricsImport.Text = "Import Missing &Lyrics from Lyricsfly"
        Me.chkLyricsImport.UseVisualStyleBackColor = True
        '
        'tpFileSystem
        '
        Me.tpFileSystem.Controls.Add(Me.chkExportLyrics)
        Me.tpFileSystem.Controls.Add(Me.chkExportIndex)
        Me.tpFileSystem.Controls.Add(Me.chkExportArtwork)
        Me.tpFileSystem.Location = New System.Drawing.Point(4, 22)
        Me.tpFileSystem.Name = "tpFileSystem"
        Me.tpFileSystem.Padding = New System.Windows.Forms.Padding(3)
        Me.tpFileSystem.Size = New System.Drawing.Size(619, 211)
        Me.tpFileSystem.TabIndex = 5
        Me.tpFileSystem.Text = "File System"
        Me.tpFileSystem.UseVisualStyleBackColor = True
        '
        'chkExportLyrics
        '
        Me.chkExportLyrics.AutoSize = True
        Me.chkExportLyrics.Checked = Global.iTSfv.My.MySettings.Default.ExportLyrics
        Me.chkExportLyrics.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "ExportLyrics", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkExportLyrics.Location = New System.Drawing.Point(10, 56)
        Me.chkExportLyrics.Name = "chkExportLyrics"
        Me.chkExportLyrics.Size = New System.Drawing.Size(159, 17)
        Me.chkExportLyrics.TabIndex = 16
        Me.chkExportLyrics.Text = "Export &Lyrics to Album folder"
        Me.chkExportLyrics.UseVisualStyleBackColor = True
        '
        'chkExportIndex
        '
        Me.chkExportIndex.AutoSize = True
        Me.chkExportIndex.Checked = Global.iTSfv.My.MySettings.Default.ExportIndex
        Me.chkExportIndex.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "ExportIndex", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkExportIndex.Location = New System.Drawing.Point(10, 33)
        Me.chkExportIndex.Name = "chkExportIndex"
        Me.chkExportIndex.Size = New System.Drawing.Size(222, 17)
        Me.chkExportIndex.TabIndex = 15
        Me.chkExportIndex.Text = "Export Index to Album folder as index.html"
        Me.chkExportIndex.UseVisualStyleBackColor = True
        '
        'chkExportArtwork
        '
        Me.chkExportArtwork.AutoSize = True
        Me.chkExportArtwork.Checked = Global.iTSfv.My.MySettings.Default.LibraryExportArtwork
        Me.chkExportArtwork.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkExportArtwork.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "LibraryExportArtwork", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkExportArtwork.Location = New System.Drawing.Point(10, 10)
        Me.chkExportArtwork.Name = "chkExportArtwork"
        Me.chkExportArtwork.Size = New System.Drawing.Size(231, 17)
        Me.chkExportArtwork.TabIndex = 7
        Me.chkExportArtwork.Text = "Export &Artwork to Album folder as Folder.jpg"
        Me.chkExportArtwork.UseVisualStyleBackColor = True
        '
        'tpDiscsBrowser
        '
        Me.tpDiscsBrowser.Controls.Add(Me.btnValidateDisc)
        Me.tpDiscsBrowser.Controls.Add(Me.btnBrowsDisc)
        Me.tpDiscsBrowser.Controls.Add(Me.pbArtwork)
        Me.tpDiscsBrowser.Controls.Add(Me.lbDiscs)
        Me.tpDiscsBrowser.Location = New System.Drawing.Point(4, 22)
        Me.tpDiscsBrowser.Name = "tpDiscsBrowser"
        Me.tpDiscsBrowser.Padding = New System.Windows.Forms.Padding(3)
        Me.tpDiscsBrowser.Size = New System.Drawing.Size(619, 211)
        Me.tpDiscsBrowser.TabIndex = 3
        Me.tpDiscsBrowser.Text = "Discs Browser"
        Me.tpDiscsBrowser.UseVisualStyleBackColor = True
        '
        'btnValidateDisc
        '
        Me.btnValidateDisc.Location = New System.Drawing.Point(6, 112)
        Me.btnValidateDisc.Name = "btnValidateDisc"
        Me.btnValidateDisc.Size = New System.Drawing.Size(100, 23)
        Me.btnValidateDisc.TabIndex = 3
        Me.btnValidateDisc.Text = "&Validate Disc"
        Me.btnValidateDisc.UseVisualStyleBackColor = True
        '
        'btnBrowsDisc
        '
        Me.btnBrowsDisc.Location = New System.Drawing.Point(6, 141)
        Me.btnBrowsDisc.Name = "btnBrowsDisc"
        Me.btnBrowsDisc.Size = New System.Drawing.Size(100, 23)
        Me.btnBrowsDisc.TabIndex = 2
        Me.btnBrowsDisc.Text = "&Browse Disc..."
        Me.btnBrowsDisc.UseVisualStyleBackColor = True
        '
        'pbArtwork
        '
        Me.pbArtwork.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pbArtwork.Location = New System.Drawing.Point(6, 6)
        Me.pbArtwork.Name = "pbArtwork"
        Me.pbArtwork.Size = New System.Drawing.Size(100, 100)
        Me.pbArtwork.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pbArtwork.TabIndex = 1
        Me.pbArtwork.TabStop = False
        '
        'lbDiscs
        '
        Me.lbDiscs.Dock = System.Windows.Forms.DockStyle.Right
        Me.lbDiscs.FormattingEnabled = True
        Me.lbDiscs.Location = New System.Drawing.Point(112, 3)
        Me.lbDiscs.Name = "lbDiscs"
        Me.lbDiscs.Size = New System.Drawing.Size(504, 199)
        Me.lbDiscs.Sorted = True
        Me.lbDiscs.TabIndex = 0
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.LogsToolStripMenuItem, Me.ToolsToolStripMenuItem, Me.HelpToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(645, 24)
        Me.MenuStrip1.TabIndex = 6
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenDirectoryToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "&File"
        '
        'OpenDirectoryToolStripMenuItem
        '
        Me.OpenDirectoryToolStripMenuItem.Name = "OpenDirectoryToolStripMenuItem"
        Me.OpenDirectoryToolStripMenuItem.Size = New System.Drawing.Size(163, 22)
        Me.OpenDirectoryToolStripMenuItem.Text = "&Open Directory..."
        '
        'LogsToolStripMenuItem
        '
        Me.LogsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DebugToolStripMenuItem})
        Me.LogsToolStripMenuItem.Name = "LogsToolStripMenuItem"
        Me.LogsToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.LogsToolStripMenuItem.Text = "&Logs"
        '
        'DebugToolStripMenuItem
        '
        Me.DebugToolStripMenuItem.Name = "DebugToolStripMenuItem"
        Me.DebugToolStripMenuItem.Size = New System.Drawing.Size(118, 22)
        Me.DebugToolStripMenuItem.Text = "&Debug..."
        '
        'ToolsToolStripMenuItem
        '
        Me.ToolsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SwitchToITSfvToolStripMenuItem, Me.WMPfvToolStripMenuItem, Me.TrackReplaceAssistantToolStripMenuItem, Me.ToolStripSeparator1, Me.OptionsToolStripMenuItem})
        Me.ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        Me.ToolsToolStripMenuItem.Size = New System.Drawing.Size(48, 20)
        Me.ToolsToolStripMenuItem.Text = "&Tools"
        '
        'SwitchToITSfvToolStripMenuItem
        '
        Me.SwitchToITSfvToolStripMenuItem.Name = "SwitchToITSfvToolStripMenuItem"
        Me.SwitchToITSfvToolStripMenuItem.Size = New System.Drawing.Size(206, 22)
        Me.SwitchToITSfvToolStripMenuItem.Text = "Switch to iTSfv"
        '
        'WMPfvToolStripMenuItem
        '
        Me.WMPfvToolStripMenuItem.Name = "WMPfvToolStripMenuItem"
        Me.WMPfvToolStripMenuItem.Size = New System.Drawing.Size(206, 22)
        Me.WMPfvToolStripMenuItem.Text = "&WMPfv..."
        '
        'TrackReplaceAssistantToolStripMenuItem
        '
        Me.TrackReplaceAssistantToolStripMenuItem.Name = "TrackReplaceAssistantToolStripMenuItem"
        Me.TrackReplaceAssistantToolStripMenuItem.Size = New System.Drawing.Size(206, 22)
        Me.TrackReplaceAssistantToolStripMenuItem.Text = "&Track Replace Assistant..."
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(203, 6)
        '
        'OptionsToolStripMenuItem
        '
        Me.OptionsToolStripMenuItem.Image = Global.iTSfv.My.Resources.Resources.application_edit
        Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
        Me.OptionsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2
        Me.OptionsToolStripMenuItem.Size = New System.Drawing.Size(206, 22)
        Me.OptionsToolStripMenuItem.Text = "&Options..."
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutToolStripMenuItem, Me.VersionHistoryToolStripMenuItem})
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.HelpToolStripMenuItem.Text = "&Help"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(163, 22)
        Me.AboutToolStripMenuItem.Text = "&About..."
        '
        'VersionHistoryToolStripMenuItem
        '
        Me.VersionHistoryToolStripMenuItem.Name = "VersionHistoryToolStripMenuItem"
        Me.VersionHistoryToolStripMenuItem.Size = New System.Drawing.Size(163, 22)
        Me.VersionHistoryToolStripMenuItem.Text = "&Version History..."
        '
        'tmrUpdate
        '
        Me.tmrUpdate.Enabled = True
        Me.tmrUpdate.Interval = 1000
        '
        'frmValidator
        '
        Me.AcceptButton = Me.btnBrowse
        Me.AllowDrop = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(645, 334)
        Me.Controls.Add(Me.tcMain)
        Me.Controls.Add(Me.txtFolderPath)
        Me.Controls.Add(Me.btnValidate)
        Me.Controls.Add(Me.btnBrowse)
        Me.Controls.Add(Me.ssAppDisc)
        Me.Controls.Add(Me.MenuStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.MaximizeBox = False
        Me.Name = "frmValidator"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "iTSfv Lite"
        Me.ssAppDisc.ResumeLayout(False)
        Me.ssAppDisc.PerformLayout()
        Me.tcMain.ResumeLayout(False)
        Me.tpChecks.ResumeLayout(False)
        Me.tpChecks.PerformLayout()
        Me.tpTracks.ResumeLayout(False)
        Me.tpTracks.PerformLayout()
        Me.tpFileSystem.ResumeLayout(False)
        Me.tpFileSystem.PerformLayout()
        Me.tpDiscsBrowser.ResumeLayout(False)
        CType(Me.pbArtwork, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ssAppDisc As System.Windows.Forms.StatusStrip
    Friend WithEvents sBarTrack As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents pBarDiscs As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents btnBrowse As System.Windows.Forms.Button
    Friend WithEvents btnValidate As System.Windows.Forms.Button
    Friend WithEvents txtFolderPath As System.Windows.Forms.TextBox
    Friend WithEvents bwApp As System.ComponentModel.BackgroundWorker
    Friend WithEvents tcMain As System.Windows.Forms.TabControl
    Friend WithEvents tpTracks As System.Windows.Forms.TabPage
    Friend WithEvents chkLyricsImport As System.Windows.Forms.CheckBox
    Friend WithEvents chkStandardCheck As System.Windows.Forms.CheckBox
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OptionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tpDiscsBrowser As System.Windows.Forms.TabPage
    Friend WithEvents lbDiscs As System.Windows.Forms.ListBox
    Friend WithEvents tpChecks As System.Windows.Forms.TabPage
    Friend WithEvents tpFileSystem As System.Windows.Forms.TabPage
    Friend WithEvents btnBrowsDisc As System.Windows.Forms.Button
    Friend WithEvents pbArtwork As System.Windows.Forms.PictureBox
    Friend WithEvents chkFillTrackCountEtc As System.Windows.Forms.CheckBox
    Friend WithEvents btnValidateDisc As System.Windows.Forms.Button
    Friend WithEvents OpenDirectoryToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents pbarTracks As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents SwitchToITSfvToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents chkExportArtwork As System.Windows.Forms.CheckBox
    Friend WithEvents HelpToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents chkExportIndex As System.Windows.Forms.CheckBox
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TrackReplaceAssistantToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents chkExportLyrics As System.Windows.Forms.CheckBox
    Friend WithEvents ttApp As System.Windows.Forms.ToolTip
    Friend WithEvents tmrUpdate As System.Windows.Forms.Timer
    Friend WithEvents VersionHistoryToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents WMPfvToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents chkEditCopyArtistToAlbumArtist As System.Windows.Forms.CheckBox
    Friend WithEvents LogsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DebugToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
