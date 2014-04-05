<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDuplicateTracks
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmDuplicateTracks))
        Me.btnYes = New System.Windows.Forms.Button
        Me.btnNo = New System.Windows.Forms.Button
        Me.dgvTracks = New System.Windows.Forms.DataGridView
        Me.btnRemove = New System.Windows.Forms.Button
        CType(Me.dgvTracks, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnYes
        '
        Me.btnYes.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnYes.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnYes.Location = New System.Drawing.Point(367, 317)
        Me.btnYes.Name = "btnYes"
        Me.btnYes.Size = New System.Drawing.Size(75, 23)
        Me.btnYes.TabIndex = 8
        Me.btnYes.Text = "&Yes"
        Me.btnYes.UseVisualStyleBackColor = True
        '
        'btnNo
        '
        Me.btnNo.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnNo.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnNo.Location = New System.Drawing.Point(448, 317)
        Me.btnNo.Name = "btnNo"
        Me.btnNo.Size = New System.Drawing.Size(75, 23)
        Me.btnNo.TabIndex = 9
        Me.btnNo.Text = "&No"
        Me.btnNo.UseVisualStyleBackColor = True
        '
        'dgvTracks
        '
        Me.dgvTracks.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvTracks.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvTracks.Location = New System.Drawing.Point(12, 12)
        Me.dgvTracks.Name = "dgvTracks"
        Me.dgvTracks.ReadOnly = True
        Me.dgvTracks.Size = New System.Drawing.Size(521, 297)
        Me.dgvTracks.TabIndex = 10
        '
        'btnRemove
        '
        Me.btnRemove.AutoSize = True
        Me.btnRemove.Location = New System.Drawing.Point(12, 317)
        Me.btnRemove.Name = "btnRemove"
        Me.btnRemove.Size = New System.Drawing.Size(128, 23)
        Me.btnRemove.TabIndex = 11
        Me.btnRemove.Text = "&Ignore Selected Tracks"
        Me.btnRemove.UseVisualStyleBackColor = True
        Me.btnRemove.Visible = False
        '
        'frmDuplicateTracks
        '
        Me.AcceptButton = Me.btnNo
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(545, 350)
        Me.Controls.Add(Me.btnRemove)
        Me.Controls.Add(Me.dgvTracks)
        Me.Controls.Add(Me.btnYes)
        Me.Controls.Add(Me.btnNo)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimizeBox = False
        Me.Name = "frmDuplicateTracks"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Are you sure you want remove the following tracks from Library?"
        Me.TopMost = True
        CType(Me.dgvTracks, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnYes As System.Windows.Forms.Button
    Friend WithEvents btnNo As System.Windows.Forms.Button
    Friend WithEvents dgvTracks As System.Windows.Forms.DataGridView
    Friend WithEvents btnRemove As System.Windows.Forms.Button
End Class
