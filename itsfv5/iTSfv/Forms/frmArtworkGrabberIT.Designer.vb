<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmArtworkGrabberIT
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmArtworkGrabberIT))
        Me.lblArtist = New System.Windows.Forms.Label
        Me.txtArtist = New System.Windows.Forms.TextBox
        Me.txtAlbum = New System.Windows.Forms.TextBox
        Me.lblAlbum = New System.Windows.Forms.Label
        Me.gbAlbumTags = New System.Windows.Forms.GroupBox
        Me.chkSaveAuto = New System.Windows.Forms.CheckBox
        Me.btnSearch = New System.Windows.Forms.Button
        Me.lblNote = New System.Windows.Forms.Label
        Me.bwApp = New System.ComponentModel.BackgroundWorker
        Me.tmrProgress = New System.Windows.Forms.Timer(Me.components)
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.tsStatus = New System.Windows.Forms.ToolStripStatusLabel
        Me.llbllFreeAlbum = New System.Windows.Forms.LinkLabel
        Me.gbAlbumTags.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblArtist
        '
        Me.lblArtist.AutoSize = True
        Me.lblArtist.Location = New System.Drawing.Point(19, 27)
        Me.lblArtist.Name = "lblArtist"
        Me.lblArtist.Size = New System.Drawing.Size(30, 13)
        Me.lblArtist.TabIndex = 0
        Me.lblArtist.Text = "Artist"
        '
        'txtArtist
        '
        Me.txtArtist.Location = New System.Drawing.Point(69, 24)
        Me.txtArtist.Name = "txtArtist"
        Me.txtArtist.Size = New System.Drawing.Size(285, 20)
        Me.txtArtist.TabIndex = 1
        '
        'txtAlbum
        '
        Me.txtAlbum.Location = New System.Drawing.Point(69, 50)
        Me.txtAlbum.Name = "txtAlbum"
        Me.txtAlbum.Size = New System.Drawing.Size(285, 20)
        Me.txtAlbum.TabIndex = 3
        '
        'lblAlbum
        '
        Me.lblAlbum.AutoSize = True
        Me.lblAlbum.Location = New System.Drawing.Point(19, 53)
        Me.lblAlbum.Name = "lblAlbum"
        Me.lblAlbum.Size = New System.Drawing.Size(36, 13)
        Me.lblAlbum.TabIndex = 2
        Me.lblAlbum.Text = "Album"
        '
        'gbAlbumTags
        '
        Me.gbAlbumTags.Controls.Add(Me.txtAlbum)
        Me.gbAlbumTags.Controls.Add(Me.lblAlbum)
        Me.gbAlbumTags.Controls.Add(Me.txtArtist)
        Me.gbAlbumTags.Controls.Add(Me.lblArtist)
        Me.gbAlbumTags.Location = New System.Drawing.Point(12, 12)
        Me.gbAlbumTags.Name = "gbAlbumTags"
        Me.gbAlbumTags.Size = New System.Drawing.Size(419, 90)
        Me.gbAlbumTags.TabIndex = 4
        Me.gbAlbumTags.TabStop = False
        Me.gbAlbumTags.Text = "Search in iTunes Store"
        '
        'chkSaveAuto
        '
        Me.chkSaveAuto.AutoSize = True
        Me.chkSaveAuto.Location = New System.Drawing.Point(35, 206)
        Me.chkSaveAuto.Name = "chkSaveAuto"
        Me.chkSaveAuto.Size = New System.Drawing.Size(196, 17)
        Me.chkSaveAuto.TabIndex = 5
        Me.chkSaveAuto.Text = "Automatically Save to Artwork folder"
        Me.chkSaveAuto.UseVisualStyleBackColor = True
        Me.chkSaveAuto.Visible = False
        '
        'btnSearch
        '
        Me.btnSearch.AutoSize = True
        Me.btnSearch.Location = New System.Drawing.Point(243, 202)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(188, 23)
        Me.btnSearch.TabIndex = 6
        Me.btnSearch.Text = "&Search and Export iTunes Artwork..."
        Me.btnSearch.UseVisualStyleBackColor = True
        '
        'lblNote
        '
        Me.lblNote.BackColor = System.Drawing.Color.Gainsboro
        Me.lblNote.Location = New System.Drawing.Point(15, 115)
        Me.lblNote.Name = "lblNote"
        Me.lblNote.Size = New System.Drawing.Size(416, 55)
        Me.lblNote.TabIndex = 7
        '
        'bwApp
        '
        Me.bwApp.WorkerReportsProgress = True
        '
        'tmrProgress
        '
        Me.tmrProgress.Enabled = True
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsStatus})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 242)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(444, 22)
        Me.StatusStrip1.SizingGrip = False
        Me.StatusStrip1.TabIndex = 8
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'tsStatus
        '
        Me.tsStatus.Image = CType(resources.GetObject("tsStatus.Image"), System.Drawing.Image)
        Me.tsStatus.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.tsStatus.Name = "tsStatus"
        Me.tsStatus.Size = New System.Drawing.Size(429, 17)
        Me.tsStatus.Spring = True
        Me.tsStatus.Text = "Ready"
        '
        'llbllFreeAlbum
        '
        Me.llbllFreeAlbum.AutoSize = True
        Me.llbllFreeAlbum.Location = New System.Drawing.Point(15, 179)
        Me.llbllFreeAlbum.Name = "llbllFreeAlbum"
        Me.llbllFreeAlbum.Size = New System.Drawing.Size(181, 13)
        Me.llbllFreeAlbum.TabIndex = 9
        Me.llbllFreeAlbum.TabStop = True
        Me.llbllFreeAlbum.Text = "http://www.tunecore.com/freealbum"
        '
        'frmArtworkGrabberIT
        '
        Me.AcceptButton = Me.btnSearch
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(444, 264)
        Me.Controls.Add(Me.llbllFreeAlbum)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.lblNote)
        Me.Controls.Add(Me.btnSearch)
        Me.Controls.Add(Me.chkSaveAuto)
        Me.Controls.Add(Me.gbAlbumTags)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmArtworkGrabberIT"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "frmArtworkGrabberIT"
        Me.gbAlbumTags.ResumeLayout(False)
        Me.gbAlbumTags.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblArtist As System.Windows.Forms.Label
    Friend WithEvents txtArtist As System.Windows.Forms.TextBox
    Friend WithEvents txtAlbum As System.Windows.Forms.TextBox
    Friend WithEvents lblAlbum As System.Windows.Forms.Label
    Friend WithEvents gbAlbumTags As System.Windows.Forms.GroupBox
    Friend WithEvents chkSaveAuto As System.Windows.Forms.CheckBox
    Friend WithEvents btnSearch As System.Windows.Forms.Button
    Friend WithEvents lblNote As System.Windows.Forms.Label
    Friend WithEvents bwApp As System.ComponentModel.BackgroundWorker
    Friend WithEvents tmrProgress As System.Windows.Forms.Timer
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents tsStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents llbllFreeAlbum As System.Windows.Forms.LinkLabel
End Class
