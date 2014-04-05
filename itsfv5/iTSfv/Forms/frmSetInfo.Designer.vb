<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSetInfo
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
        Me.nudEpID = New System.Windows.Forms.NumericUpDown
        Me.chkEpID = New System.Windows.Forms.CheckBox
        Me.chkShow = New System.Windows.Forms.CheckBox
        Me.txtShow = New System.Windows.Forms.TextBox
        Me.btnOK = New System.Windows.Forms.Button
        Me.btnApply = New System.Windows.Forms.Button
        CType(Me.nudEpID, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'nudEpID
        '
        Me.nudEpID.Location = New System.Drawing.Point(135, 34)
        Me.nudEpID.Maximum = New Decimal(New Integer() {9999, 0, 0, 0})
        Me.nudEpID.Name = "nudEpID"
        Me.nudEpID.Size = New System.Drawing.Size(120, 20)
        Me.nudEpID.TabIndex = 0
        '
        'chkEpID
        '
        Me.chkEpID.AutoSize = True
        Me.chkEpID.Checked = True
        Me.chkEpID.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkEpID.Location = New System.Drawing.Point(42, 35)
        Me.chkEpID.Name = "chkEpID"
        Me.chkEpID.Size = New System.Drawing.Size(78, 17)
        Me.chkEpID.TabIndex = 1
        Me.chkEpID.Text = "Episode ID"
        Me.chkEpID.UseVisualStyleBackColor = True
        '
        'chkShow
        '
        Me.chkShow.AutoSize = True
        Me.chkShow.Checked = True
        Me.chkShow.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkShow.Location = New System.Drawing.Point(42, 71)
        Me.chkShow.Name = "chkShow"
        Me.chkShow.Size = New System.Drawing.Size(53, 17)
        Me.chkShow.TabIndex = 2
        Me.chkShow.Text = "Show"
        Me.chkShow.UseVisualStyleBackColor = True
        '
        'txtShow
        '
        Me.txtShow.Location = New System.Drawing.Point(135, 68)
        Me.txtShow.Name = "txtShow"
        Me.txtShow.Size = New System.Drawing.Size(211, 20)
        Me.txtShow.TabIndex = 3
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(306, 223)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 23)
        Me.btnOK.TabIndex = 4
        Me.btnOK.Text = "&OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnApply
        '
        Me.btnApply.Location = New System.Drawing.Point(387, 223)
        Me.btnApply.Name = "btnApply"
        Me.btnApply.Size = New System.Drawing.Size(75, 23)
        Me.btnApply.TabIndex = 5
        Me.btnApply.Text = "&Apply"
        Me.btnApply.UseVisualStyleBackColor = True
        '
        'frmSetInfo
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(509, 258)
        Me.Controls.Add(Me.btnApply)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.txtShow)
        Me.Controls.Add(Me.chkShow)
        Me.Controls.Add(Me.chkEpID)
        Me.Controls.Add(Me.nudEpID)
        Me.Name = "frmSetInfo"
        Me.Text = "frmSetInfo"
        CType(Me.nudEpID, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents nudEpID As System.Windows.Forms.NumericUpDown
    Friend WithEvents chkEpID As System.Windows.Forms.CheckBox
    Friend WithEvents chkShow As System.Windows.Forms.CheckBox
    Friend WithEvents txtShow As System.Windows.Forms.TextBox
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnApply As System.Windows.Forms.Button
End Class
