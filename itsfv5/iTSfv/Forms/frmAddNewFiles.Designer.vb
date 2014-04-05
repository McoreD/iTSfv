<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAddNewFiles
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAddNewFiles))
        Me.btnCancel = New System.Windows.Forms.Button
        Me.btnNo = New System.Windows.Forms.Button
        Me.btnYes = New System.Windows.Forms.Button
        Me.cboAlbumArtist = New System.Windows.Forms.ComboBox
        Me.gbAlbumTags = New System.Windows.Forms.GroupBox
        Me.cboArtist = New System.Windows.Forms.ComboBox
        Me.chkArtist = New System.Windows.Forms.CheckBox
        Me.cboGenre = New System.Windows.Forms.ComboBox
        Me.nudYear = New System.Windows.Forms.NumericUpDown
        Me.chkYear = New System.Windows.Forms.CheckBox
        Me.chkGenre = New System.Windows.Forms.CheckBox
        Me.chkAlbumArtist = New System.Windows.Forms.CheckBox
        Me.lblOf = New System.Windows.Forms.Label
        Me.chkDisc = New System.Windows.Forms.CheckBox
        Me.nudDiscCount = New System.Windows.Forms.NumericUpDown
        Me.nudDiscNumber = New System.Windows.Forms.NumericUpDown
        Me.chkAlbum = New System.Windows.Forms.CheckBox
        Me.txtAlbum = New System.Windows.Forms.TextBox
        Me.chkOverwriteTags = New System.Windows.Forms.CheckBox
        Me.txtAdviceAlbumArtist = New System.Windows.Forms.TextBox
        Me.lblAdvice = New System.Windows.Forms.Label
        Me.btnAutofill = New System.Windows.Forms.Button
        Me.gbAlbumTags.SuspendLayout()
        CType(Me.nudYear, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudDiscCount, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudDiscNumber, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnCancel
        '
        Me.btnCancel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCancel.Location = New System.Drawing.Point(275, 250)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 13
        Me.btnCancel.Text = "&Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnNo
        '
        Me.btnNo.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnNo.Location = New System.Drawing.Point(194, 250)
        Me.btnNo.Name = "btnNo"
        Me.btnNo.Size = New System.Drawing.Size(75, 23)
        Me.btnNo.TabIndex = 12
        Me.btnNo.Text = "&No"
        Me.btnNo.UseVisualStyleBackColor = True
        '
        'btnYes
        '
        Me.btnYes.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnYes.Location = New System.Drawing.Point(113, 250)
        Me.btnYes.Name = "btnYes"
        Me.btnYes.Size = New System.Drawing.Size(75, 23)
        Me.btnYes.TabIndex = 11
        Me.btnYes.Text = "&Yes"
        Me.btnYes.UseVisualStyleBackColor = True
        '
        'cboAlbumArtist
        '
        Me.cboAlbumArtist.FormattingEnabled = True
        Me.cboAlbumArtist.ItemHeight = 13
        Me.cboAlbumArtist.Location = New System.Drawing.Point(105, 19)
        Me.cboAlbumArtist.Name = "cboAlbumArtist"
        Me.cboAlbumArtist.Size = New System.Drawing.Size(223, 21)
        Me.cboAlbumArtist.Sorted = True
        Me.cboAlbumArtist.TabIndex = 1
        '
        'gbAlbumTags
        '
        Me.gbAlbumTags.Controls.Add(Me.cboArtist)
        Me.gbAlbumTags.Controls.Add(Me.chkArtist)
        Me.gbAlbumTags.Controls.Add(Me.cboGenre)
        Me.gbAlbumTags.Controls.Add(Me.nudYear)
        Me.gbAlbumTags.Controls.Add(Me.chkYear)
        Me.gbAlbumTags.Controls.Add(Me.chkGenre)
        Me.gbAlbumTags.Controls.Add(Me.cboAlbumArtist)
        Me.gbAlbumTags.Controls.Add(Me.chkAlbumArtist)
        Me.gbAlbumTags.Controls.Add(Me.lblOf)
        Me.gbAlbumTags.Controls.Add(Me.chkDisc)
        Me.gbAlbumTags.Controls.Add(Me.nudDiscCount)
        Me.gbAlbumTags.Controls.Add(Me.nudDiscNumber)
        Me.gbAlbumTags.Controls.Add(Me.chkAlbum)
        Me.gbAlbumTags.Controls.Add(Me.txtAlbum)
        Me.gbAlbumTags.Enabled = False
        Me.gbAlbumTags.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gbAlbumTags.Location = New System.Drawing.Point(22, 46)
        Me.gbAlbumTags.Name = "gbAlbumTags"
        Me.gbAlbumTags.Size = New System.Drawing.Size(340, 188)
        Me.gbAlbumTags.TabIndex = 14
        Me.gbAlbumTags.TabStop = False
        Me.gbAlbumTags.Text = "Disc Tags"
        '
        'cboArtist
        '
        Me.cboArtist.FormattingEnabled = True
        Me.cboArtist.ItemHeight = 13
        Me.cboArtist.Location = New System.Drawing.Point(105, 46)
        Me.cboArtist.Name = "cboArtist"
        Me.cboArtist.Size = New System.Drawing.Size(223, 21)
        Me.cboArtist.Sorted = True
        Me.cboArtist.TabIndex = 3
        '
        'chkArtist
        '
        Me.chkArtist.AutoSize = True
        Me.chkArtist.Location = New System.Drawing.Point(9, 48)
        Me.chkArtist.Name = "chkArtist"
        Me.chkArtist.Size = New System.Drawing.Size(49, 17)
        Me.chkArtist.TabIndex = 2
        Me.chkArtist.Text = "Artist"
        Me.chkArtist.UseVisualStyleBackColor = True
        '
        'cboGenre
        '
        Me.cboGenre.FormattingEnabled = True
        Me.cboGenre.Location = New System.Drawing.Point(105, 125)
        Me.cboGenre.Name = "cboGenre"
        Me.cboGenre.Size = New System.Drawing.Size(225, 21)
        Me.cboGenre.TabIndex = 9
        '
        'nudYear
        '
        Me.nudYear.Location = New System.Drawing.Point(105, 99)
        Me.nudYear.Maximum = New Decimal(New Integer() {9999, 0, 0, 0})
        Me.nudYear.Name = "nudYear"
        Me.nudYear.Size = New System.Drawing.Size(148, 20)
        Me.nudYear.TabIndex = 7
        Me.nudYear.Value = New Decimal(New Integer() {2008, 0, 0, 0})
        '
        'chkYear
        '
        Me.chkYear.AutoSize = True
        Me.chkYear.Location = New System.Drawing.Point(10, 100)
        Me.chkYear.Name = "chkYear"
        Me.chkYear.Size = New System.Drawing.Size(48, 17)
        Me.chkYear.TabIndex = 6
        Me.chkYear.Text = "Year"
        Me.chkYear.UseVisualStyleBackColor = True
        '
        'chkGenre
        '
        Me.chkGenre.AutoSize = True
        Me.chkGenre.Location = New System.Drawing.Point(10, 127)
        Me.chkGenre.Name = "chkGenre"
        Me.chkGenre.Size = New System.Drawing.Size(55, 17)
        Me.chkGenre.TabIndex = 8
        Me.chkGenre.Text = "Genre"
        Me.chkGenre.UseVisualStyleBackColor = True
        '
        'chkAlbumArtist
        '
        Me.chkAlbumArtist.AutoSize = True
        Me.chkAlbumArtist.Location = New System.Drawing.Point(9, 21)
        Me.chkAlbumArtist.Name = "chkAlbumArtist"
        Me.chkAlbumArtist.Size = New System.Drawing.Size(81, 17)
        Me.chkAlbumArtist.TabIndex = 0
        Me.chkAlbumArtist.Text = "Album Artist"
        Me.chkAlbumArtist.UseVisualStyleBackColor = True
        '
        'lblOf
        '
        Me.lblOf.AutoSize = True
        Me.lblOf.Location = New System.Drawing.Point(171, 155)
        Me.lblOf.Name = "lblOf"
        Me.lblOf.Size = New System.Drawing.Size(16, 13)
        Me.lblOf.TabIndex = 14
        Me.lblOf.Text = "of"
        '
        'chkDisc
        '
        Me.chkDisc.AutoSize = True
        Me.chkDisc.Location = New System.Drawing.Point(9, 153)
        Me.chkDisc.Name = "chkDisc"
        Me.chkDisc.Size = New System.Drawing.Size(47, 17)
        Me.chkDisc.TabIndex = 10
        Me.chkDisc.Text = "Disc"
        Me.chkDisc.UseVisualStyleBackColor = True
        '
        'nudDiscCount
        '
        Me.nudDiscCount.Location = New System.Drawing.Point(193, 152)
        Me.nudDiscCount.Name = "nudDiscCount"
        Me.nudDiscCount.ReadOnly = True
        Me.nudDiscCount.Size = New System.Drawing.Size(60, 20)
        Me.nudDiscCount.TabIndex = 12
        Me.nudDiscCount.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'nudDiscNumber
        '
        Me.nudDiscNumber.Location = New System.Drawing.Point(105, 152)
        Me.nudDiscNumber.Name = "nudDiscNumber"
        Me.nudDiscNumber.ReadOnly = True
        Me.nudDiscNumber.Size = New System.Drawing.Size(60, 20)
        Me.nudDiscNumber.TabIndex = 11
        Me.nudDiscNumber.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'chkAlbum
        '
        Me.chkAlbum.AutoSize = True
        Me.chkAlbum.Location = New System.Drawing.Point(9, 75)
        Me.chkAlbum.Name = "chkAlbum"
        Me.chkAlbum.Size = New System.Drawing.Size(55, 17)
        Me.chkAlbum.TabIndex = 4
        Me.chkAlbum.Text = "Album"
        Me.chkAlbum.UseVisualStyleBackColor = True
        '
        'txtAlbum
        '
        Me.txtAlbum.Location = New System.Drawing.Point(105, 73)
        Me.txtAlbum.Name = "txtAlbum"
        Me.txtAlbum.Size = New System.Drawing.Size(223, 20)
        Me.txtAlbum.TabIndex = 5
        '
        'chkOverwriteTags
        '
        Me.chkOverwriteTags.AutoSize = True
        Me.chkOverwriteTags.ForeColor = System.Drawing.Color.Red
        Me.chkOverwriteTags.Location = New System.Drawing.Point(22, 21)
        Me.chkOverwriteTags.Name = "chkOverwriteTags"
        Me.chkOverwriteTags.Size = New System.Drawing.Size(301, 17)
        Me.chkOverwriteTags.TabIndex = 14
        Me.chkOverwriteTags.Text = "Overwrite &Tags ( Warning: Only add ONE album at a time )"
        Me.chkOverwriteTags.UseVisualStyleBackColor = True
        '
        'txtAdviceAlbumArtist
        '
        Me.txtAdviceAlbumArtist.BackColor = System.Drawing.SystemColors.Info
        Me.txtAdviceAlbumArtist.Location = New System.Drawing.Point(175, 285)
        Me.txtAdviceAlbumArtist.Name = "txtAdviceAlbumArtist"
        Me.txtAdviceAlbumArtist.ReadOnly = True
        Me.txtAdviceAlbumArtist.Size = New System.Drawing.Size(175, 20)
        Me.txtAdviceAlbumArtist.TabIndex = 15
        '
        'lblAdvice
        '
        Me.lblAdvice.AutoSize = True
        Me.lblAdvice.Location = New System.Drawing.Point(29, 288)
        Me.lblAdvice.Name = "lblAdvice"
        Me.lblAdvice.Size = New System.Drawing.Size(140, 13)
        Me.lblAdvice.TabIndex = 16
        Me.lblAdvice.Text = "Recommended Album Artist:"
        '
        'btnAutofill
        '
        Me.btnAutofill.Location = New System.Drawing.Point(31, 250)
        Me.btnAutofill.Name = "btnAutofill"
        Me.btnAutofill.Size = New System.Drawing.Size(75, 23)
        Me.btnAutofill.TabIndex = 17
        Me.btnAutofill.Text = "&Autofill"
        Me.btnAutofill.UseVisualStyleBackColor = True
        '
        'frmAddNewFiles
        '
        Me.AcceptButton = Me.btnAutofill
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(378, 318)
        Me.Controls.Add(Me.btnAutofill)
        Me.Controls.Add(Me.lblAdvice)
        Me.Controls.Add(Me.txtAdviceAlbumArtist)
        Me.Controls.Add(Me.chkOverwriteTags)
        Me.Controls.Add(Me.gbAlbumTags)
        Me.Controls.Add(Me.btnYes)
        Me.Controls.Add(Me.btnNo)
        Me.Controls.Add(Me.btnCancel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmAddNewFiles"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "fAddNewFiles"
        Me.TopMost = True
        Me.gbAlbumTags.ResumeLayout(False)
        Me.gbAlbumTags.PerformLayout()
        CType(Me.nudYear, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudDiscCount, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudDiscNumber, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnNo As System.Windows.Forms.Button
    Friend WithEvents btnYes As System.Windows.Forms.Button
    Friend WithEvents cboAlbumArtist As System.Windows.Forms.ComboBox
    Friend WithEvents gbAlbumTags As System.Windows.Forms.GroupBox
    Friend WithEvents chkAlbum As System.Windows.Forms.CheckBox
    Friend WithEvents txtAlbum As System.Windows.Forms.TextBox
    Friend WithEvents chkOverwriteTags As System.Windows.Forms.CheckBox
    Friend WithEvents chkDisc As System.Windows.Forms.CheckBox
    Friend WithEvents nudDiscCount As System.Windows.Forms.NumericUpDown
    Friend WithEvents nudDiscNumber As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblOf As System.Windows.Forms.Label
    Friend WithEvents chkAlbumArtist As System.Windows.Forms.CheckBox
    Friend WithEvents chkGenre As System.Windows.Forms.CheckBox
    Friend WithEvents nudYear As System.Windows.Forms.NumericUpDown
    Friend WithEvents chkYear As System.Windows.Forms.CheckBox
    Friend WithEvents cboGenre As System.Windows.Forms.ComboBox
    Friend WithEvents cboArtist As System.Windows.Forms.ComboBox
    Friend WithEvents chkArtist As System.Windows.Forms.CheckBox
    Friend WithEvents txtAdviceAlbumArtist As System.Windows.Forms.TextBox
    Friend WithEvents lblAdvice As System.Windows.Forms.Label
    Friend WithEvents btnAutofill As System.Windows.Forms.Button
End Class
