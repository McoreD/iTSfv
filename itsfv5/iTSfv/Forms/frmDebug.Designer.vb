<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDebug
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
        Me.txtDebug = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'txtDebug
        '
        Me.txtDebug.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtDebug.Font = New System.Drawing.Font("Arial Unicode MS", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDebug.Location = New System.Drawing.Point(12, 12)
        Me.txtDebug.Multiline = True
        Me.txtDebug.Name = "txtDebug"
        Me.txtDebug.ReadOnly = True
        Me.txtDebug.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtDebug.Size = New System.Drawing.Size(608, 422)
        Me.txtDebug.TabIndex = 1
        Me.txtDebug.TabStop = False
        Me.txtDebug.WordWrap = False
        '
        'frmDebug
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(632, 446)
        Me.Controls.Add(Me.txtDebug)
        Me.Name = "frmDebug"
        Me.Text = "frmDebug"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtDebug As System.Windows.Forms.TextBox
End Class
