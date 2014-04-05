<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmVer
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.txtVer = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'txtVer
        '
        Me.txtVer.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtVer.Location = New System.Drawing.Point(12, 12)
        Me.txtVer.Multiline = True
        Me.txtVer.Name = "txtVer"
        Me.txtVer.ReadOnly = True
        Me.txtVer.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtVer.Size = New System.Drawing.Size(608, 422)
        Me.txtVer.TabIndex = 0
        Me.txtVer.TabStop = False
        Me.txtVer.WordWrap = False
        '
        'frmVer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(632, 446)
        Me.Controls.Add(Me.txtVer)
        Me.MinimumSize = New System.Drawing.Size(480, 400)
        Me.Name = "frmVer"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "frmVer"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtVer As System.Windows.Forms.TextBox
End Class
