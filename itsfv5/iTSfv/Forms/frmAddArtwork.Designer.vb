<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAddArtwork
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAddArtwork))
        Me.nudTop = New System.Windows.Forms.NumericUpDown
        Me.nudRight = New System.Windows.Forms.NumericUpDown
        Me.nudBottom = New System.Windows.Forms.NumericUpDown
        Me.nudLeft = New System.Windows.Forms.NumericUpDown
        Me.picArtwork = New System.Windows.Forms.PictureBox
        Me.btnYes = New System.Windows.Forms.Button
        Me.btnNo = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.btnPreview = New System.Windows.Forms.Button
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.lbTracks = New System.Windows.Forms.ListBox
        CType(Me.nudTop, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudRight, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudBottom, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudLeft, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picArtwork, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'nudTop
        '
        Me.nudTop.Location = New System.Drawing.Point(123, 21)
        Me.nudTop.Maximum = New Decimal(New Integer() {300, 0, 0, 0})
        Me.nudTop.Name = "nudTop"
        Me.nudTop.Size = New System.Drawing.Size(100, 20)
        Me.nudTop.TabIndex = 0
        Me.nudTop.Value = New Decimal(New Integer() {100, 0, 0, 0})
        '
        'nudRight
        '
        Me.nudRight.Location = New System.Drawing.Point(229, 88)
        Me.nudRight.Maximum = New Decimal(New Integer() {300, 0, 0, 0})
        Me.nudRight.Name = "nudRight"
        Me.nudRight.Size = New System.Drawing.Size(100, 20)
        Me.nudRight.TabIndex = 1
        Me.nudRight.Value = New Decimal(New Integer() {100, 0, 0, 0})
        '
        'nudBottom
        '
        Me.nudBottom.Location = New System.Drawing.Point(123, 153)
        Me.nudBottom.Maximum = New Decimal(New Integer() {300, 0, 0, 0})
        Me.nudBottom.Name = "nudBottom"
        Me.nudBottom.Size = New System.Drawing.Size(100, 20)
        Me.nudBottom.TabIndex = 2
        Me.nudBottom.Value = New Decimal(New Integer() {100, 0, 0, 0})
        '
        'nudLeft
        '
        Me.nudLeft.Location = New System.Drawing.Point(17, 88)
        Me.nudLeft.Maximum = New Decimal(New Integer() {300, 0, 0, 0})
        Me.nudLeft.Name = "nudLeft"
        Me.nudLeft.Size = New System.Drawing.Size(100, 20)
        Me.nudLeft.TabIndex = 3
        Me.nudLeft.Value = New Decimal(New Integer() {100, 0, 0, 0})
        '
        'picArtwork
        '
        Me.picArtwork.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picArtwork.Location = New System.Drawing.Point(123, 47)
        Me.picArtwork.Name = "picArtwork"
        Me.picArtwork.Size = New System.Drawing.Size(100, 100)
        Me.picArtwork.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picArtwork.TabIndex = 4
        Me.picArtwork.TabStop = False
        '
        'btnYes
        '
        Me.btnYes.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnYes.Location = New System.Drawing.Point(364, 225)
        Me.btnYes.Name = "btnYes"
        Me.btnYes.Size = New System.Drawing.Size(75, 23)
        Me.btnYes.TabIndex = 6
        Me.btnYes.Text = "&Yes"
        Me.btnYes.UseVisualStyleBackColor = True
        '
        'btnNo
        '
        Me.btnNo.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnNo.Location = New System.Drawing.Point(445, 225)
        Me.btnNo.Name = "btnNo"
        Me.btnNo.Size = New System.Drawing.Size(75, 23)
        Me.btnNo.TabIndex = 7
        Me.btnNo.Text = "&No"
        Me.btnNo.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCancel.Location = New System.Drawing.Point(526, 225)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 5
        Me.btnCancel.Text = "&Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.picArtwork)
        Me.GroupBox1.Controls.Add(Me.nudLeft)
        Me.GroupBox1.Controls.Add(Me.nudBottom)
        Me.GroupBox1.Controls.Add(Me.nudRight)
        Me.GroupBox1.Controls.Add(Me.nudTop)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(346, 197)
        Me.GroupBox1.TabIndex = 8
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Crop Settings"
        '
        'btnPreview
        '
        Me.btnPreview.Location = New System.Drawing.Point(12, 225)
        Me.btnPreview.Name = "btnPreview"
        Me.btnPreview.Size = New System.Drawing.Size(75, 23)
        Me.btnPreview.TabIndex = 9
        Me.btnPreview.Text = "&Preview"
        Me.btnPreview.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.lbTracks)
        Me.GroupBox2.Location = New System.Drawing.Point(364, 12)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(248, 197)
        Me.GroupBox2.TabIndex = 10
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Track List"
        '
        'lbTracks
        '
        Me.lbTracks.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lbTracks.FormattingEnabled = True
        Me.lbTracks.Location = New System.Drawing.Point(3, 16)
        Me.lbTracks.Name = "lbTracks"
        Me.lbTracks.Size = New System.Drawing.Size(242, 173)
        Me.lbTracks.TabIndex = 0
        '
        'frmAddArtwork
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(624, 270)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.btnPreview)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.btnYes)
        Me.Controls.Add(Me.btnNo)
        Me.Controls.Add(Me.btnCancel)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmAddArtwork"
        Me.Text = "frmAddArtwork"
        Me.TopMost = True
        CType(Me.nudTop, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudRight, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudBottom, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudLeft, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picArtwork, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents nudTop As System.Windows.Forms.NumericUpDown
    Friend WithEvents nudRight As System.Windows.Forms.NumericUpDown
    Friend WithEvents nudBottom As System.Windows.Forms.NumericUpDown
    Friend WithEvents nudLeft As System.Windows.Forms.NumericUpDown
    Friend WithEvents picArtwork As System.Windows.Forms.PictureBox
    Friend WithEvents btnYes As System.Windows.Forms.Button
    Friend WithEvents btnNo As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents btnPreview As System.Windows.Forms.Button
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents lbTracks As System.Windows.Forms.ListBox
End Class
