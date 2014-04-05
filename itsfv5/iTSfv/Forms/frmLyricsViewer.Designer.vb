<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmLyricsViewer
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
        Me.cmsOptions = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.cmsTopMost = New System.Windows.Forms.ToolStripMenuItem
        Me.tmrSecond = New System.Windows.Forms.Timer(Me.components)
        Me.tlpDesc = New System.Windows.Forms.TableLayoutPanel
        Me.pbArtwork = New System.Windows.Forms.PictureBox
        Me.pDesc = New System.Windows.Forms.Panel
        Me.btnEditLyrics = New System.Windows.Forms.Button
        Me.txtGenre = New System.Windows.Forms.Label
        Me.txtYear = New System.Windows.Forms.Label
        Me.txtAlbumArtist = New System.Windows.Forms.Label
        Me.txtAlbum = New System.Windows.Forms.Label
        Me.txtArtist = New System.Windows.Forms.Label
        Me.txtName = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.tlpMain = New System.Windows.Forms.TableLayoutPanel
        Me.panelLyics = New System.Windows.Forms.Panel
        Me.txtLyrics = New System.Windows.Forms.RichTextBox
        Me.cmsOptions.SuspendLayout()
        Me.tlpDesc.SuspendLayout()
        CType(Me.pbArtwork, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pDesc.SuspendLayout()
        Me.tlpMain.SuspendLayout()
        Me.panelLyics.SuspendLayout()
        Me.SuspendLayout()
        '
        'cmsOptions
        '
        Me.cmsOptions.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.cmsTopMost})
        Me.cmsOptions.Name = "cmsOptions"
        Me.cmsOptions.Size = New System.Drawing.Size(155, 26)
        '
        'cmsTopMost
        '
        Me.cmsTopMost.Checked = Global.iTSfv.My.MySettings.Default.LyricsViewerTopMost
        Me.cmsTopMost.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cmsTopMost.Name = "cmsTopMost"
        Me.cmsTopMost.Size = New System.Drawing.Size(154, 22)
        Me.cmsTopMost.Text = "&Always On Top"
        '
        'tmrSecond
        '
        Me.tmrSecond.Enabled = True
        Me.tmrSecond.Interval = 1000
        '
        'tlpDesc
        '
        Me.tlpDesc.ColumnCount = 2
        Me.tlpDesc.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 206.0!))
        Me.tlpDesc.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpDesc.Controls.Add(Me.pbArtwork, 0, 0)
        Me.tlpDesc.Controls.Add(Me.pDesc, 1, 0)
        Me.tlpDesc.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpDesc.Location = New System.Drawing.Point(3, 3)
        Me.tlpDesc.Name = "tlpDesc"
        Me.tlpDesc.RowCount = 1
        Me.tlpDesc.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 206.0!))
        Me.tlpDesc.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 206.0!))
        Me.tlpDesc.Size = New System.Drawing.Size(578, 204)
        Me.tlpDesc.TabIndex = 0
        '
        'pbArtwork
        '
        Me.pbArtwork.ContextMenuStrip = Me.cmsOptions
        Me.pbArtwork.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pbArtwork.Location = New System.Drawing.Point(3, 3)
        Me.pbArtwork.Name = "pbArtwork"
        Me.pbArtwork.Size = New System.Drawing.Size(200, 200)
        Me.pbArtwork.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pbArtwork.TabIndex = 3
        Me.pbArtwork.TabStop = False
        '
        'pDesc
        '
        Me.pDesc.Controls.Add(Me.btnEditLyrics)
        Me.pDesc.Controls.Add(Me.txtGenre)
        Me.pDesc.Controls.Add(Me.txtYear)
        Me.pDesc.Controls.Add(Me.txtAlbumArtist)
        Me.pDesc.Controls.Add(Me.txtAlbum)
        Me.pDesc.Controls.Add(Me.txtArtist)
        Me.pDesc.Controls.Add(Me.txtName)
        Me.pDesc.Controls.Add(Me.Label6)
        Me.pDesc.Controls.Add(Me.Label5)
        Me.pDesc.Controls.Add(Me.Label4)
        Me.pDesc.Controls.Add(Me.Label3)
        Me.pDesc.Controls.Add(Me.Label2)
        Me.pDesc.Controls.Add(Me.Label1)
        Me.pDesc.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pDesc.Location = New System.Drawing.Point(209, 3)
        Me.pDesc.Name = "pDesc"
        Me.pDesc.Size = New System.Drawing.Size(366, 200)
        Me.pDesc.TabIndex = 4
        '
        'btnEditLyrics
        '
        Me.btnEditLyrics.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnEditLyrics.Enabled = False
        Me.btnEditLyrics.Location = New System.Drawing.Point(248, 168)
        Me.btnEditLyrics.Name = "btnEditLyrics"
        Me.btnEditLyrics.Size = New System.Drawing.Size(107, 23)
        Me.btnEditLyrics.TabIndex = 12
        Me.btnEditLyrics.Text = "&Submit Correction..."
        Me.btnEditLyrics.UseVisualStyleBackColor = True
        Me.btnEditLyrics.Visible = False
        '
        'txtGenre
        '
        Me.txtGenre.AutoSize = True
        Me.txtGenre.Font = New System.Drawing.Font("Calibri", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtGenre.Location = New System.Drawing.Point(112, 155)
        Me.txtGenre.Name = "txtGenre"
        Me.txtGenre.Size = New System.Drawing.Size(71, 18)
        Me.txtGenre.TabIndex = 11
        Me.txtGenre.Text = "Pending..."
        '
        'txtYear
        '
        Me.txtYear.AutoSize = True
        Me.txtYear.Font = New System.Drawing.Font("Calibri", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtYear.Location = New System.Drawing.Point(112, 127)
        Me.txtYear.Name = "txtYear"
        Me.txtYear.Size = New System.Drawing.Size(71, 18)
        Me.txtYear.TabIndex = 10
        Me.txtYear.Text = "Pending..."
        '
        'txtAlbumArtist
        '
        Me.txtAlbumArtist.AutoSize = True
        Me.txtAlbumArtist.Font = New System.Drawing.Font("Calibri", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtAlbumArtist.Location = New System.Drawing.Point(112, 99)
        Me.txtAlbumArtist.Name = "txtAlbumArtist"
        Me.txtAlbumArtist.Size = New System.Drawing.Size(71, 18)
        Me.txtAlbumArtist.TabIndex = 9
        Me.txtAlbumArtist.Text = "Pending..."
        '
        'txtAlbum
        '
        Me.txtAlbum.AutoSize = True
        Me.txtAlbum.Font = New System.Drawing.Font("Calibri", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtAlbum.Location = New System.Drawing.Point(112, 71)
        Me.txtAlbum.Name = "txtAlbum"
        Me.txtAlbum.Size = New System.Drawing.Size(71, 18)
        Me.txtAlbum.TabIndex = 8
        Me.txtAlbum.Text = "Pending..."
        '
        'txtArtist
        '
        Me.txtArtist.AutoSize = True
        Me.txtArtist.Font = New System.Drawing.Font("Calibri", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtArtist.Location = New System.Drawing.Point(112, 43)
        Me.txtArtist.Name = "txtArtist"
        Me.txtArtist.Size = New System.Drawing.Size(71, 18)
        Me.txtArtist.TabIndex = 7
        Me.txtArtist.Text = "Pending..."
        '
        'txtName
        '
        Me.txtName.AutoSize = True
        Me.txtName.Font = New System.Drawing.Font("Calibri", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtName.Location = New System.Drawing.Point(112, 15)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(71, 18)
        Me.txtName.TabIndex = 6
        Me.txtName.Text = "Pending..."
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Calibri", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(16, 155)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(51, 18)
        Me.Label6.TabIndex = 5
        Me.Label6.Text = "Genre:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Calibri", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(16, 127)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(39, 18)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "Year:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Calibri", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(16, 99)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(90, 18)
        Me.Label4.TabIndex = 3
        Me.Label4.Text = "Album Artist:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Calibri", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(16, 71)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(53, 18)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Album:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Calibri", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(16, 43)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(46, 18)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Artist:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Calibri", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(16, 15)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(45, 18)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Name"
        '
        'tlpMain
        '
        Me.tlpMain.ColumnCount = 1
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpMain.Controls.Add(Me.tlpDesc, 0, 0)
        Me.tlpMain.Controls.Add(Me.panelLyics, 0, 1)
        Me.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpMain.Location = New System.Drawing.Point(0, 0)
        Me.tlpMain.Name = "tlpMain"
        Me.tlpMain.RowCount = 2
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 210.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.tlpMain.Size = New System.Drawing.Size(584, 566)
        Me.tlpMain.TabIndex = 4
        '
        'panelLyics
        '
        Me.panelLyics.Controls.Add(Me.txtLyrics)
        Me.panelLyics.Dock = System.Windows.Forms.DockStyle.Fill
        Me.panelLyics.Location = New System.Drawing.Point(3, 213)
        Me.panelLyics.Name = "panelLyics"
        Me.panelLyics.Size = New System.Drawing.Size(578, 350)
        Me.panelLyics.TabIndex = 1
        '
        'txtLyrics
        '
        Me.txtLyrics.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtLyrics.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtLyrics.Location = New System.Drawing.Point(0, 0)
        Me.txtLyrics.Name = "txtLyrics"
        Me.txtLyrics.Size = New System.Drawing.Size(578, 350)
        Me.txtLyrics.TabIndex = 0
        Me.txtLyrics.Text = ""
        '
        'frmLyricsViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(584, 566)
        Me.Controls.Add(Me.tlpMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Name = "frmLyricsViewer"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "frmLyricsViewer"
        Me.TopMost = True
        Me.cmsOptions.ResumeLayout(False)
        Me.tlpDesc.ResumeLayout(False)
        CType(Me.pbArtwork, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pDesc.ResumeLayout(False)
        Me.pDesc.PerformLayout()
        Me.tlpMain.ResumeLayout(False)
        Me.panelLyics.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tmrSecond As System.Windows.Forms.Timer
    Friend WithEvents cmsOptions As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents cmsTopMost As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tlpDesc As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents pbArtwork As System.Windows.Forms.PictureBox
    Friend WithEvents pDesc As System.Windows.Forms.Panel
    Friend WithEvents txtGenre As System.Windows.Forms.Label
    Friend WithEvents txtYear As System.Windows.Forms.Label
    Friend WithEvents txtAlbumArtist As System.Windows.Forms.Label
    Friend WithEvents txtAlbum As System.Windows.Forms.Label
    Friend WithEvents txtArtist As System.Windows.Forms.Label
    Friend WithEvents txtName As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents tlpMain As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents panelLyics As System.Windows.Forms.Panel
    Friend WithEvents btnEditLyrics As System.Windows.Forms.Button
    Friend WithEvents txtLyrics As System.Windows.Forms.RichTextBox
End Class
