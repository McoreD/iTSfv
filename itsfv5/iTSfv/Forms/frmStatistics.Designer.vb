
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmStatistics
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
        Me.pbGenrePie = New System.Windows.Forms.PictureBox
        Me.pbGenrePieLegend = New System.Windows.Forms.PictureBox
        Me.TabControl1 = New System.Windows.Forms.TabControl
        Me.tpGenres = New System.Windows.Forms.TabPage
        Me.tpAlbumArtists = New System.Windows.Forms.TabPage
        Me.pbAlbumArtistsLegend = New System.Windows.Forms.PictureBox
        Me.pbAlbumArtistsPie = New System.Windows.Forms.PictureBox
        Me.tpArtists = New System.Windows.Forms.TabPage
        Me.pbArtistsLegend = New System.Windows.Forms.PictureBox
        Me.pbArtistsPie = New System.Windows.Forms.PictureBox
        Me.tpAlbums = New System.Windows.Forms.TabPage
        Me.pbAlbumsLegend = New System.Windows.Forms.PictureBox
        Me.pbAlbumsPie = New System.Windows.Forms.PictureBox
        Me.tpTracks = New System.Windows.Forms.TabPage
        Me.pbTracksLegend = New System.Windows.Forms.PictureBox
        Me.pbTracksPie = New System.Windows.Forms.PictureBox
        Me.tpSummary = New System.Windows.Forms.TabPage
        Me.txtSummary = New System.Windows.Forms.TextBox
        Me.cboViewBy = New System.Windows.Forms.ComboBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.cboChartType = New System.Windows.Forms.ComboBox
        Me.Label2 = New System.Windows.Forms.Label
        CType(Me.pbGenrePie, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pbGenrePieLegend, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabControl1.SuspendLayout()
        Me.tpGenres.SuspendLayout()
        Me.tpAlbumArtists.SuspendLayout()
        CType(Me.pbAlbumArtistsLegend, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pbAlbumArtistsPie, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpArtists.SuspendLayout()
        CType(Me.pbArtistsLegend, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pbArtistsPie, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpAlbums.SuspendLayout()
        CType(Me.pbAlbumsLegend, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pbAlbumsPie, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpTracks.SuspendLayout()
        CType(Me.pbTracksLegend, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pbTracksPie, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpSummary.SuspendLayout()
        Me.SuspendLayout()
        '
        'pbGenrePie
        '
        Me.pbGenrePie.BackColor = System.Drawing.Color.Transparent
        Me.pbGenrePie.Location = New System.Drawing.Point(6, 6)
        Me.pbGenrePie.Name = "pbGenrePie"
        Me.pbGenrePie.Size = New System.Drawing.Size(200, 200)
        Me.pbGenrePie.TabIndex = 0
        Me.pbGenrePie.TabStop = False
        '
        'pbGenrePieLegend
        '
        Me.pbGenrePieLegend.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pbGenrePieLegend.BackColor = System.Drawing.Color.White
        Me.pbGenrePieLegend.Location = New System.Drawing.Point(212, 6)
        Me.pbGenrePieLegend.Name = "pbGenrePieLegend"
        Me.pbGenrePieLegend.Size = New System.Drawing.Size(434, 235)
        Me.pbGenrePieLegend.TabIndex = 1
        Me.pbGenrePieLegend.TabStop = False
        '
        'TabControl1
        '
        Me.TabControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TabControl1.Controls.Add(Me.tpGenres)
        Me.TabControl1.Controls.Add(Me.tpAlbumArtists)
        Me.TabControl1.Controls.Add(Me.tpArtists)
        Me.TabControl1.Controls.Add(Me.tpAlbums)
        Me.TabControl1.Controls.Add(Me.tpTracks)
        Me.TabControl1.Controls.Add(Me.tpSummary)
        Me.TabControl1.Location = New System.Drawing.Point(12, 12)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(660, 273)
        Me.TabControl1.TabIndex = 3
        '
        'tpGenres
        '
        Me.tpGenres.BackColor = System.Drawing.Color.Transparent
        Me.tpGenres.Controls.Add(Me.pbGenrePieLegend)
        Me.tpGenres.Controls.Add(Me.pbGenrePie)
        Me.tpGenres.Location = New System.Drawing.Point(4, 22)
        Me.tpGenres.Name = "tpGenres"
        Me.tpGenres.Padding = New System.Windows.Forms.Padding(3)
        Me.tpGenres.Size = New System.Drawing.Size(652, 247)
        Me.tpGenres.TabIndex = 0
        Me.tpGenres.Text = "Top 5 Genres"
        Me.tpGenres.UseVisualStyleBackColor = True
        '
        'tpAlbumArtists
        '
        Me.tpAlbumArtists.Controls.Add(Me.pbAlbumArtistsLegend)
        Me.tpAlbumArtists.Controls.Add(Me.pbAlbumArtistsPie)
        Me.tpAlbumArtists.Location = New System.Drawing.Point(4, 22)
        Me.tpAlbumArtists.Name = "tpAlbumArtists"
        Me.tpAlbumArtists.Padding = New System.Windows.Forms.Padding(3)
        Me.tpAlbumArtists.Size = New System.Drawing.Size(652, 247)
        Me.tpAlbumArtists.TabIndex = 4
        Me.tpAlbumArtists.Text = "Top 5 Album Artists"
        Me.tpAlbumArtists.UseVisualStyleBackColor = True
        '
        'pbAlbumArtistsLegend
        '
        Me.pbAlbumArtistsLegend.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pbAlbumArtistsLegend.BackColor = System.Drawing.Color.White
        Me.pbAlbumArtistsLegend.Location = New System.Drawing.Point(212, 6)
        Me.pbAlbumArtistsLegend.Name = "pbAlbumArtistsLegend"
        Me.pbAlbumArtistsLegend.Size = New System.Drawing.Size(434, 235)
        Me.pbAlbumArtistsLegend.TabIndex = 5
        Me.pbAlbumArtistsLegend.TabStop = False
        '
        'pbAlbumArtistsPie
        '
        Me.pbAlbumArtistsPie.BackColor = System.Drawing.SystemColors.Control
        Me.pbAlbumArtistsPie.Location = New System.Drawing.Point(6, 6)
        Me.pbAlbumArtistsPie.Name = "pbAlbumArtistsPie"
        Me.pbAlbumArtistsPie.Size = New System.Drawing.Size(200, 200)
        Me.pbAlbumArtistsPie.TabIndex = 4
        Me.pbAlbumArtistsPie.TabStop = False
        '
        'tpArtists
        '
        Me.tpArtists.BackColor = System.Drawing.Color.Transparent
        Me.tpArtists.Controls.Add(Me.pbArtistsLegend)
        Me.tpArtists.Controls.Add(Me.pbArtistsPie)
        Me.tpArtists.Location = New System.Drawing.Point(4, 22)
        Me.tpArtists.Name = "tpArtists"
        Me.tpArtists.Padding = New System.Windows.Forms.Padding(3)
        Me.tpArtists.Size = New System.Drawing.Size(644, 247)
        Me.tpArtists.TabIndex = 1
        Me.tpArtists.Text = "Top 5 Artists"
        Me.tpArtists.UseVisualStyleBackColor = True
        '
        'pbArtistsLegend
        '
        Me.pbArtistsLegend.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pbArtistsLegend.BackColor = System.Drawing.Color.White
        Me.pbArtistsLegend.Location = New System.Drawing.Point(212, 6)
        Me.pbArtistsLegend.Name = "pbArtistsLegend"
        Me.pbArtistsLegend.Size = New System.Drawing.Size(426, 235)
        Me.pbArtistsLegend.TabIndex = 3
        Me.pbArtistsLegend.TabStop = False
        '
        'pbArtistsPie
        '
        Me.pbArtistsPie.BackColor = System.Drawing.SystemColors.Control
        Me.pbArtistsPie.Location = New System.Drawing.Point(6, 6)
        Me.pbArtistsPie.Name = "pbArtistsPie"
        Me.pbArtistsPie.Size = New System.Drawing.Size(200, 200)
        Me.pbArtistsPie.TabIndex = 2
        Me.pbArtistsPie.TabStop = False
        '
        'tpAlbums
        '
        Me.tpAlbums.Controls.Add(Me.pbAlbumsLegend)
        Me.tpAlbums.Controls.Add(Me.pbAlbumsPie)
        Me.tpAlbums.Location = New System.Drawing.Point(4, 22)
        Me.tpAlbums.Name = "tpAlbums"
        Me.tpAlbums.Padding = New System.Windows.Forms.Padding(3)
        Me.tpAlbums.Size = New System.Drawing.Size(644, 247)
        Me.tpAlbums.TabIndex = 3
        Me.tpAlbums.Text = "Top 5 Albums"
        Me.tpAlbums.UseVisualStyleBackColor = True
        '
        'pbAlbumsLegend
        '
        Me.pbAlbumsLegend.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pbAlbumsLegend.BackColor = System.Drawing.Color.White
        Me.pbAlbumsLegend.Location = New System.Drawing.Point(212, 6)
        Me.pbAlbumsLegend.Name = "pbAlbumsLegend"
        Me.pbAlbumsLegend.Size = New System.Drawing.Size(426, 235)
        Me.pbAlbumsLegend.TabIndex = 5
        Me.pbAlbumsLegend.TabStop = False
        '
        'pbAlbumsPie
        '
        Me.pbAlbumsPie.BackColor = System.Drawing.SystemColors.Control
        Me.pbAlbumsPie.Location = New System.Drawing.Point(6, 6)
        Me.pbAlbumsPie.Name = "pbAlbumsPie"
        Me.pbAlbumsPie.Size = New System.Drawing.Size(200, 200)
        Me.pbAlbumsPie.TabIndex = 4
        Me.pbAlbumsPie.TabStop = False
        '
        'tpTracks
        '
        Me.tpTracks.Controls.Add(Me.pbTracksLegend)
        Me.tpTracks.Controls.Add(Me.pbTracksPie)
        Me.tpTracks.Location = New System.Drawing.Point(4, 22)
        Me.tpTracks.Name = "tpTracks"
        Me.tpTracks.Padding = New System.Windows.Forms.Padding(3)
        Me.tpTracks.Size = New System.Drawing.Size(644, 247)
        Me.tpTracks.TabIndex = 5
        Me.tpTracks.Text = "Top 5 Tracks"
        Me.tpTracks.UseVisualStyleBackColor = True
        '
        'pbTracksLegend
        '
        Me.pbTracksLegend.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pbTracksLegend.BackColor = System.Drawing.Color.White
        Me.pbTracksLegend.Location = New System.Drawing.Point(212, 6)
        Me.pbTracksLegend.Name = "pbTracksLegend"
        Me.pbTracksLegend.Size = New System.Drawing.Size(426, 235)
        Me.pbTracksLegend.TabIndex = 7
        Me.pbTracksLegend.TabStop = False
        '
        'pbTracksPie
        '
        Me.pbTracksPie.BackColor = System.Drawing.SystemColors.Control
        Me.pbTracksPie.Location = New System.Drawing.Point(6, 6)
        Me.pbTracksPie.Name = "pbTracksPie"
        Me.pbTracksPie.Size = New System.Drawing.Size(200, 200)
        Me.pbTracksPie.TabIndex = 6
        Me.pbTracksPie.TabStop = False
        '
        'tpSummary
        '
        Me.tpSummary.BackColor = System.Drawing.Color.Transparent
        Me.tpSummary.Controls.Add(Me.txtSummary)
        Me.tpSummary.Location = New System.Drawing.Point(4, 22)
        Me.tpSummary.Name = "tpSummary"
        Me.tpSummary.Padding = New System.Windows.Forms.Padding(3)
        Me.tpSummary.Size = New System.Drawing.Size(644, 247)
        Me.tpSummary.TabIndex = 2
        Me.tpSummary.Text = "Summary"
        Me.tpSummary.UseVisualStyleBackColor = True
        '
        'txtSummary
        '
        Me.txtSummary.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtSummary.Location = New System.Drawing.Point(3, 3)
        Me.txtSummary.Multiline = True
        Me.txtSummary.Name = "txtSummary"
        Me.txtSummary.ReadOnly = True
        Me.txtSummary.Size = New System.Drawing.Size(638, 241)
        Me.txtSummary.TabIndex = 0
        '
        'cboViewBy
        '
        Me.cboViewBy.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cboViewBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboViewBy.FormattingEnabled = True
        Me.cboViewBy.Items.AddRange(New Object() {"All time favorite based on Played Count and Track Duration", "Most number of tracks based on Number of Tracks under Artist", "Recently favorite based on iTunes Store file validator Auto Rating"})
        Me.cboViewBy.Location = New System.Drawing.Point(71, 296)
        Me.cboViewBy.Name = "cboViewBy"
        Me.cboViewBy.Size = New System.Drawing.Size(393, 21)
        Me.cboViewBy.TabIndex = 6
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(21, 299)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(44, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "View by"
        '
        'cboChartType
        '
        Me.cboChartType.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboChartType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboChartType.FormattingEnabled = True
        Me.cboChartType.Items.AddRange(New Object() {"Pie", "Bar"})
        Me.cboChartType.Location = New System.Drawing.Point(541, 296)
        Me.cboChartType.Name = "cboChartType"
        Me.cboChartType.Size = New System.Drawing.Size(121, 21)
        Me.cboChartType.TabIndex = 7
        '
        'Label2
        '
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(476, 299)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(59, 13)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "Chart Type"
        '
        'frmStatistics
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(684, 329)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.cboChartType)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cboViewBy)
        Me.Controls.Add(Me.TabControl1)
        Me.MinimumSize = New System.Drawing.Size(700, 365)
        Me.Name = "frmStatistics"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "frmStatistics"
        CType(Me.pbGenrePie, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pbGenrePieLegend, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabControl1.ResumeLayout(False)
        Me.tpGenres.ResumeLayout(False)
        Me.tpAlbumArtists.ResumeLayout(False)
        CType(Me.pbAlbumArtistsLegend, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pbAlbumArtistsPie, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpArtists.ResumeLayout(False)
        CType(Me.pbArtistsLegend, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pbArtistsPie, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpAlbums.ResumeLayout(False)
        CType(Me.pbAlbumsLegend, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pbAlbumsPie, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpTracks.ResumeLayout(False)
        CType(Me.pbTracksLegend, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pbTracksPie, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpSummary.ResumeLayout(False)
        Me.tpSummary.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents pbGenrePie As System.Windows.Forms.PictureBox
    Friend WithEvents pbGenrePieLegend As System.Windows.Forms.PictureBox
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents tpGenres As System.Windows.Forms.TabPage
    Friend WithEvents tpArtists As System.Windows.Forms.TabPage
    Friend WithEvents cboViewBy As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents pbArtistsLegend As System.Windows.Forms.PictureBox
    Friend WithEvents pbArtistsPie As System.Windows.Forms.PictureBox
    Friend WithEvents tpSummary As System.Windows.Forms.TabPage
    Friend WithEvents txtSummary As System.Windows.Forms.TextBox
    Friend WithEvents tpAlbums As System.Windows.Forms.TabPage
    Friend WithEvents pbAlbumsLegend As System.Windows.Forms.PictureBox
    Friend WithEvents pbAlbumsPie As System.Windows.Forms.PictureBox
    Friend WithEvents cboChartType As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents tpAlbumArtists As System.Windows.Forms.TabPage
    Friend WithEvents pbAlbumArtistsLegend As System.Windows.Forms.PictureBox
    Friend WithEvents pbAlbumArtistsPie As System.Windows.Forms.PictureBox
    Friend WithEvents tpTracks As System.Windows.Forms.TabPage
    Friend WithEvents pbTracksLegend As System.Windows.Forms.PictureBox
    Friend WithEvents pbTracksPie As System.Windows.Forms.PictureBox
End Class

