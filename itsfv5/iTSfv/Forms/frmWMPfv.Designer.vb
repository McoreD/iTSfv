<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmWMPfv
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
        Dim tmrApp As System.Windows.Forms.Timer
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmWMPfv))
        Me.tcValidate = New System.Windows.Forms.TabControl
        Me.tpValidateChecks = New System.Windows.Forms.TabPage
        Me.tpValidateLibrary = New System.Windows.Forms.TabPage
        Me.tpValidateTracks = New System.Windows.Forms.TabPage
        Me.tpValidateFileSystem = New System.Windows.Forms.TabPage
        Me.tpTags = New System.Windows.Forms.TabPage
        Me.btnUrlToCoverArtRewrite = New System.Windows.Forms.Button
        Me.tcTabs = New System.Windows.Forms.TabControl
        Me.tpValidate = New System.Windows.Forms.TabPage
        Me.tpExplorer = New System.Windows.Forms.TabPage
        Me.chkAdd = New System.Windows.Forms.CheckBox
        Me.lbFiles = New System.Windows.Forms.ListBox
        Me.btnFindNewTracks = New System.Windows.Forms.Button
        Me.tpDiscsBrowser = New System.Windows.Forms.TabPage
        Me.lbDiscs = New System.Windows.Forms.ListBox
        Me.tpAdvanced = New System.Windows.Forms.TabPage
        Me.btnValidateLibrary = New System.Windows.Forms.Button
        Me.btnSync = New System.Windows.Forms.Button
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.LogsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.DebugToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SwitchToITSfvToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.OptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.VersionHistoryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ssApp = New System.Windows.Forms.StatusStrip
        Me.sBarLeft = New System.Windows.Forms.ToolStripStatusLabel
        Me.pBarDiscs = New System.Windows.Forms.ToolStripProgressBar
        Me.pbarTracks = New System.Windows.Forms.ToolStripProgressBar
        Me.bwApp = New System.ComponentModel.BackgroundWorker
        Me.ttApp = New System.Windows.Forms.ToolTip(Me.components)
        Me.btnStop = New System.Windows.Forms.Button
        Me.lblDev = New System.Windows.Forms.Label
        Me.llblEmail = New System.Windows.Forms.LinkLabel
        tmrApp = New System.Windows.Forms.Timer(Me.components)
        Me.tcValidate.SuspendLayout()
        Me.tcTabs.SuspendLayout()
        Me.tpValidate.SuspendLayout()
        Me.tpExplorer.SuspendLayout()
        Me.tpDiscsBrowser.SuspendLayout()
        Me.tpAdvanced.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.ssApp.SuspendLayout()
        Me.SuspendLayout()
        '
        'tmrApp
        '
        tmrApp.Enabled = True
        tmrApp.Interval = 1000
        AddHandler tmrApp.Tick, AddressOf Me.tmrApp_Tick
        '
        'tcValidate
        '
        Me.tcValidate.Controls.Add(Me.tpValidateChecks)
        Me.tcValidate.Controls.Add(Me.tpValidateLibrary)
        Me.tcValidate.Controls.Add(Me.tpValidateTracks)
        Me.tcValidate.Controls.Add(Me.tpValidateFileSystem)
        Me.tcValidate.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tcValidate.Location = New System.Drawing.Point(3, 3)
        Me.tcValidate.Name = "tcValidate"
        Me.tcValidate.SelectedIndex = 0
        Me.tcValidate.Size = New System.Drawing.Size(652, 197)
        Me.tcValidate.TabIndex = 2
        '
        'tpValidateChecks
        '
        Me.tpValidateChecks.Location = New System.Drawing.Point(4, 22)
        Me.tpValidateChecks.Name = "tpValidateChecks"
        Me.tpValidateChecks.Padding = New System.Windows.Forms.Padding(3)
        Me.tpValidateChecks.Size = New System.Drawing.Size(644, 171)
        Me.tpValidateChecks.TabIndex = 4
        Me.tpValidateChecks.Text = "Checks"
        Me.tpValidateChecks.UseVisualStyleBackColor = True
        '
        'tpValidateLibrary
        '
        Me.tpValidateLibrary.Location = New System.Drawing.Point(4, 22)
        Me.tpValidateLibrary.Name = "tpValidateLibrary"
        Me.tpValidateLibrary.Padding = New System.Windows.Forms.Padding(3)
        Me.tpValidateLibrary.Size = New System.Drawing.Size(644, 171)
        Me.tpValidateLibrary.TabIndex = 2
        Me.tpValidateLibrary.Text = "Library"
        Me.tpValidateLibrary.UseVisualStyleBackColor = True
        '
        'tpValidateTracks
        '
        Me.tpValidateTracks.Location = New System.Drawing.Point(4, 22)
        Me.tpValidateTracks.Name = "tpValidateTracks"
        Me.tpValidateTracks.Padding = New System.Windows.Forms.Padding(3)
        Me.tpValidateTracks.Size = New System.Drawing.Size(644, 171)
        Me.tpValidateTracks.TabIndex = 1
        Me.tpValidateTracks.Text = "Tracks"
        Me.tpValidateTracks.UseVisualStyleBackColor = True
        '
        'tpValidateFileSystem
        '
        Me.tpValidateFileSystem.Location = New System.Drawing.Point(4, 22)
        Me.tpValidateFileSystem.Name = "tpValidateFileSystem"
        Me.tpValidateFileSystem.Padding = New System.Windows.Forms.Padding(3)
        Me.tpValidateFileSystem.Size = New System.Drawing.Size(644, 171)
        Me.tpValidateFileSystem.TabIndex = 3
        Me.tpValidateFileSystem.Text = "File System"
        Me.tpValidateFileSystem.UseVisualStyleBackColor = True
        '
        'tpTags
        '
        Me.tpTags.Location = New System.Drawing.Point(4, 22)
        Me.tpTags.Name = "tpTags"
        Me.tpTags.Padding = New System.Windows.Forms.Padding(3)
        Me.tpTags.Size = New System.Drawing.Size(658, 203)
        Me.tpTags.TabIndex = 4
        Me.tpTags.Text = "Tags"
        Me.tpTags.UseVisualStyleBackColor = True
        '
        'btnUrlToCoverArtRewrite
        '
        Me.btnUrlToCoverArtRewrite.AutoSize = True
        Me.btnUrlToCoverArtRewrite.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.btnUrlToCoverArtRewrite.Location = New System.Drawing.Point(17, 44)
        Me.btnUrlToCoverArtRewrite.Name = "btnUrlToCoverArtRewrite"
        Me.btnUrlToCoverArtRewrite.Size = New System.Drawing.Size(402, 23)
        Me.btnUrlToCoverArtRewrite.TabIndex = 0
        Me.btnUrlToCoverArtRewrite.Text = "Rewrite UrlToCoverArt.dat to use external Artwork in Windows Vista Media Center"
        Me.btnUrlToCoverArtRewrite.UseVisualStyleBackColor = True
        '
        'tcTabs
        '
        Me.tcTabs.Controls.Add(Me.tpValidate)
        Me.tcTabs.Controls.Add(Me.tpExplorer)
        Me.tcTabs.Controls.Add(Me.tpDiscsBrowser)
        Me.tcTabs.Controls.Add(Me.tpTags)
        Me.tcTabs.Controls.Add(Me.tpAdvanced)
        Me.tcTabs.Location = New System.Drawing.Point(12, 34)
        Me.tcTabs.Name = "tcTabs"
        Me.tcTabs.SelectedIndex = 0
        Me.tcTabs.Size = New System.Drawing.Size(666, 229)
        Me.tcTabs.TabIndex = 0
        '
        'tpValidate
        '
        Me.tpValidate.Controls.Add(Me.tcValidate)
        Me.tpValidate.Location = New System.Drawing.Point(4, 22)
        Me.tpValidate.Name = "tpValidate"
        Me.tpValidate.Padding = New System.Windows.Forms.Padding(3)
        Me.tpValidate.Size = New System.Drawing.Size(658, 203)
        Me.tpValidate.TabIndex = 0
        Me.tpValidate.Text = "Validate"
        Me.tpValidate.UseVisualStyleBackColor = True
        '
        'tpExplorer
        '
        Me.tpExplorer.Controls.Add(Me.chkAdd)
        Me.tpExplorer.Controls.Add(Me.lbFiles)
        Me.tpExplorer.Controls.Add(Me.btnFindNewTracks)
        Me.tpExplorer.Location = New System.Drawing.Point(4, 22)
        Me.tpExplorer.Name = "tpExplorer"
        Me.tpExplorer.Padding = New System.Windows.Forms.Padding(3)
        Me.tpExplorer.Size = New System.Drawing.Size(658, 203)
        Me.tpExplorer.TabIndex = 1
        Me.tpExplorer.Text = "Explorer"
        Me.tpExplorer.UseVisualStyleBackColor = True
        '
        'chkAdd
        '
        Me.chkAdd.AutoSize = True
        Me.chkAdd.Location = New System.Drawing.Point(376, 18)
        Me.chkAdd.Name = "chkAdd"
        Me.chkAdd.Size = New System.Drawing.Size(248, 17)
        Me.chkAdd.TabIndex = 2
        Me.chkAdd.Text = "&Add new tracks to WMP after scan is complete"
        Me.chkAdd.UseVisualStyleBackColor = True
        '
        'lbFiles
        '
        Me.lbFiles.FormattingEnabled = True
        Me.lbFiles.Location = New System.Drawing.Point(6, 6)
        Me.lbFiles.Name = "lbFiles"
        Me.lbFiles.Size = New System.Drawing.Size(364, 186)
        Me.lbFiles.TabIndex = 1
        '
        'btnFindNewTracks
        '
        Me.btnFindNewTracks.AutoSize = True
        Me.btnFindNewTracks.Location = New System.Drawing.Point(376, 169)
        Me.btnFindNewTracks.Name = "btnFindNewTracks"
        Me.btnFindNewTracks.Size = New System.Drawing.Size(248, 23)
        Me.btnFindNewTracks.TabIndex = 0
        Me.btnFindNewTracks.Text = "&Find New Tracks not in WMP Library"
        Me.btnFindNewTracks.UseVisualStyleBackColor = True
        '
        'tpDiscsBrowser
        '
        Me.tpDiscsBrowser.Controls.Add(Me.lbDiscs)
        Me.tpDiscsBrowser.Location = New System.Drawing.Point(4, 22)
        Me.tpDiscsBrowser.Name = "tpDiscsBrowser"
        Me.tpDiscsBrowser.Padding = New System.Windows.Forms.Padding(3)
        Me.tpDiscsBrowser.Size = New System.Drawing.Size(658, 203)
        Me.tpDiscsBrowser.TabIndex = 6
        Me.tpDiscsBrowser.Text = "Discs Browser"
        Me.tpDiscsBrowser.UseVisualStyleBackColor = True
        '
        'lbDiscs
        '
        Me.lbDiscs.FormattingEnabled = True
        Me.lbDiscs.Location = New System.Drawing.Point(6, 6)
        Me.lbDiscs.Name = "lbDiscs"
        Me.lbDiscs.Size = New System.Drawing.Size(632, 225)
        Me.lbDiscs.TabIndex = 0
        '
        'tpAdvanced
        '
        Me.tpAdvanced.Controls.Add(Me.btnValidateLibrary)
        Me.tpAdvanced.Controls.Add(Me.btnUrlToCoverArtRewrite)
        Me.tpAdvanced.Controls.Add(Me.btnSync)
        Me.tpAdvanced.Location = New System.Drawing.Point(4, 22)
        Me.tpAdvanced.Name = "tpAdvanced"
        Me.tpAdvanced.Padding = New System.Windows.Forms.Padding(3)
        Me.tpAdvanced.Size = New System.Drawing.Size(658, 203)
        Me.tpAdvanced.TabIndex = 5
        Me.tpAdvanced.Text = "Advanced"
        Me.tpAdvanced.UseVisualStyleBackColor = True
        '
        'btnValidateLibrary
        '
        Me.btnValidateLibrary.AutoSize = True
        Me.btnValidateLibrary.Location = New System.Drawing.Point(17, 73)
        Me.btnValidateLibrary.Name = "btnValidateLibrary"
        Me.btnValidateLibrary.Size = New System.Drawing.Size(144, 23)
        Me.btnValidateLibrary.TabIndex = 4
        Me.btnValidateLibrary.Text = "&Validate WMP Library"
        Me.btnValidateLibrary.UseVisualStyleBackColor = True
        Me.btnValidateLibrary.Visible = False
        '
        'btnSync
        '
        Me.btnSync.AutoSize = True
        Me.btnSync.Location = New System.Drawing.Point(17, 15)
        Me.btnSync.Name = "btnSync"
        Me.btnSync.Size = New System.Drawing.Size(436, 23)
        Me.btnSync.TabIndex = 0
        Me.btnSync.Text = "Synchroclean® - clean Music library from invalid tracks and synchronize with Musi" & _
            "c folder"
        Me.btnSync.UseVisualStyleBackColor = True
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.LogsToolStripMenuItem, Me.ToolsToolStripMenuItem, Me.HelpToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(690, 24)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "&File"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(92, 22)
        Me.ExitToolStripMenuItem.Text = "&Exit"
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
        Me.ToolsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SwitchToITSfvToolStripMenuItem, Me.ToolStripSeparator1, Me.OptionsToolStripMenuItem})
        Me.ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        Me.ToolsToolStripMenuItem.Size = New System.Drawing.Size(48, 20)
        Me.ToolsToolStripMenuItem.Text = "&Tools"
        '
        'SwitchToITSfvToolStripMenuItem
        '
        Me.SwitchToITSfvToolStripMenuItem.Name = "SwitchToITSfvToolStripMenuItem"
        Me.SwitchToITSfvToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.SwitchToITSfvToolStripMenuItem.Text = "Switch to iTSfv"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(149, 6)
        '
        'OptionsToolStripMenuItem
        '
        Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
        Me.OptionsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2
        Me.OptionsToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.OptionsToolStripMenuItem.Text = "&Options..."
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.VersionHistoryToolStripMenuItem, Me.AboutToolStripMenuItem})
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.HelpToolStripMenuItem.Text = "&Help"
        '
        'VersionHistoryToolStripMenuItem
        '
        Me.VersionHistoryToolStripMenuItem.Name = "VersionHistoryToolStripMenuItem"
        Me.VersionHistoryToolStripMenuItem.Size = New System.Drawing.Size(163, 22)
        Me.VersionHistoryToolStripMenuItem.Text = "&Version History..."
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(163, 22)
        Me.AboutToolStripMenuItem.Text = "&About..."
        '
        'ssApp
        '
        Me.ssApp.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.sBarLeft, Me.pBarDiscs, Me.pbarTracks})
        Me.ssApp.Location = New System.Drawing.Point(0, 304)
        Me.ssApp.Name = "ssApp"
        Me.ssApp.Size = New System.Drawing.Size(690, 22)
        Me.ssApp.SizingGrip = False
        Me.ssApp.TabIndex = 2
        Me.ssApp.Text = "StatusStrip1"
        '
        'sBarLeft
        '
        Me.sBarLeft.AutoSize = False
        Me.sBarLeft.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.sBarLeft.Name = "sBarLeft"
        Me.sBarLeft.Size = New System.Drawing.Size(471, 17)
        Me.sBarLeft.Spring = True
        Me.sBarLeft.Text = "Ready."
        Me.sBarLeft.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
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
        'bwApp
        '
        Me.bwApp.WorkerReportsProgress = True
        Me.bwApp.WorkerSupportsCancellation = True
        '
        'btnStop
        '
        Me.btnStop.Location = New System.Drawing.Point(16, 269)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.Size = New System.Drawing.Size(75, 23)
        Me.btnStop.TabIndex = 3
        Me.btnStop.Text = "&Stop..."
        Me.btnStop.UseVisualStyleBackColor = True
        '
        'lblDev
        '
        Me.lblDev.AutoSize = True
        Me.lblDev.BackColor = System.Drawing.SystemColors.Info
        Me.lblDev.Location = New System.Drawing.Point(97, 274)
        Me.lblDev.Name = "lblDev"
        Me.lblDev.Size = New System.Drawing.Size(378, 13)
        Me.lblDev.TabIndex = 0
        Me.lblDev.Text = "If you would like to join the development of WMPfv please send me an email to"
        '
        'llblEmail
        '
        Me.llblEmail.AutoSize = True
        Me.llblEmail.BackColor = System.Drawing.SystemColors.Control
        Me.llblEmail.Location = New System.Drawing.Point(481, 274)
        Me.llblEmail.Name = "llblEmail"
        Me.llblEmail.Size = New System.Drawing.Size(100, 13)
        Me.llblEmail.TabIndex = 4
        Me.llblEmail.TabStop = True
        Me.llblEmail.Text = "mcored@gmail.com"
        '
        'frmWMPfv
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(690, 326)
        Me.Controls.Add(Me.llblEmail)
        Me.Controls.Add(Me.lblDev)
        Me.Controls.Add(Me.btnStop)
        Me.Controls.Add(Me.ssApp)
        Me.Controls.Add(Me.tcTabs)
        Me.Controls.Add(Me.MenuStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.MaximizeBox = False
        Me.Name = "frmWMPfv"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "frmWMfv"
        Me.tcValidate.ResumeLayout(False)
        Me.tcTabs.ResumeLayout(False)
        Me.tpValidate.ResumeLayout(False)
        Me.tpExplorer.ResumeLayout(False)
        Me.tpExplorer.PerformLayout()
        Me.tpDiscsBrowser.ResumeLayout(False)
        Me.tpAdvanced.ResumeLayout(False)
        Me.tpAdvanced.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ssApp.ResumeLayout(False)
        Me.ssApp.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents tcValidate As System.Windows.Forms.TabControl
    Friend WithEvents tpValidateTracks As System.Windows.Forms.TabPage
    Friend WithEvents tpValidateLibrary As System.Windows.Forms.TabPage
    Friend WithEvents tpValidateFileSystem As System.Windows.Forms.TabPage
    Friend WithEvents tpTags As System.Windows.Forms.TabPage
    Friend WithEvents tcTabs As System.Windows.Forms.TabControl
    Friend WithEvents tpExplorer As System.Windows.Forms.TabPage
    Friend WithEvents tpValidate As System.Windows.Forms.TabPage
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ssApp As System.Windows.Forms.StatusStrip
    Friend WithEvents sBarLeft As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents pBarDiscs As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents pbarTracks As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents tpAdvanced As System.Windows.Forms.TabPage
    Friend WithEvents btnSync As System.Windows.Forms.Button
    Friend WithEvents btnFindNewTracks As System.Windows.Forms.Button
    Friend WithEvents bwApp As System.ComponentModel.BackgroundWorker
    Friend WithEvents tpDiscsBrowser As System.Windows.Forms.TabPage
    Friend WithEvents lbDiscs As System.Windows.Forms.ListBox
    Friend WithEvents SwitchToITSfvToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OptionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lbFiles As System.Windows.Forms.ListBox
    Friend WithEvents chkAdd As System.Windows.Forms.CheckBox
    Friend WithEvents btnUrlToCoverArtRewrite As System.Windows.Forms.Button
    Friend WithEvents tpValidateChecks As System.Windows.Forms.TabPage
    Friend WithEvents VersionHistoryToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LogsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DebugToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ttApp As System.Windows.Forms.ToolTip
    Friend WithEvents btnStop As System.Windows.Forms.Button
    Friend WithEvents btnValidateLibrary As System.Windows.Forms.Button
    Friend WithEvents lblDev As System.Windows.Forms.Label
    Friend WithEvents llblEmail As System.Windows.Forms.LinkLabel
End Class
