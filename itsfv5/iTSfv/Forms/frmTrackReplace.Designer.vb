

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTrackReplace
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
        Me.lbTracksNew = New System.Windows.Forms.ListBox
        Me.lbTracksOld = New System.Windows.Forms.ListBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.btnRename = New System.Windows.Forms.Button
        Me.Label3 = New System.Windows.Forms.Label
        Me.tlpMain = New System.Windows.Forms.TableLayoutPanel
        Me.tlpButtons = New System.Windows.Forms.TableLayoutPanel
        Me.tlpMain.SuspendLayout()
        Me.tlpButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'lbTracksNew
        '
        Me.lbTracksNew.AllowDrop = True
        Me.lbTracksNew.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lbTracksNew.FormattingEnabled = True
        Me.lbTracksNew.Location = New System.Drawing.Point(3, 23)
        Me.lbTracksNew.Name = "lbTracksNew"
        Me.lbTracksNew.ScrollAlwaysVisible = True
        Me.lbTracksNew.Size = New System.Drawing.Size(552, 212)
        Me.lbTracksNew.TabIndex = 0
        '
        'lbTracksOld
        '
        Me.lbTracksOld.AllowDrop = True
        Me.lbTracksOld.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lbTracksOld.FormattingEnabled = True
        Me.lbTracksOld.Location = New System.Drawing.Point(3, 272)
        Me.lbTracksOld.Name = "lbTracksOld"
        Me.lbTracksOld.ScrollAlwaysVisible = True
        Me.lbTracksOld.Size = New System.Drawing.Size(552, 212)
        Me.lbTracksOld.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(3, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(464, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Rename the followings files e.g. files in Eminem - The Marshall Mathers LP [2000/" & _
            "MP3/V0 (VBR)]"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(3, 248)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(380, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "to look like this e.g. files in Eminem - The Marshall Mathers LP [2000/MP3/128]"
        '
        'btnRename
        '
        Me.btnRename.Location = New System.Drawing.Point(448, 3)
        Me.btnRename.Name = "btnRename"
        Me.btnRename.Size = New System.Drawing.Size(101, 23)
        Me.btnRename.TabIndex = 4
        Me.btnRename.Text = "&Rename"
        Me.btnRename.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(3, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(411, 13)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "After renaming, overwrite the files in Music Folder and make sure to validate the" & _
            " album"
        '
        'tlpMain
        '
        Me.tlpMain.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tlpMain.ColumnCount = 1
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpMain.Controls.Add(Me.Label1, 0, 0)
        Me.tlpMain.Controls.Add(Me.lbTracksNew, 0, 1)
        Me.tlpMain.Controls.Add(Me.Label2, 0, 2)
        Me.tlpMain.Controls.Add(Me.lbTracksOld, 0, 3)
        Me.tlpMain.Controls.Add(Me.tlpButtons, 0, 4)
        Me.tlpMain.Location = New System.Drawing.Point(12, 12)
        Me.tlpMain.Name = "tlpMain"
        Me.tlpMain.RowCount = 5
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41.0!))
        Me.tlpMain.Size = New System.Drawing.Size(558, 538)
        Me.tlpMain.TabIndex = 6
        '
        'tlpButtons
        '
        Me.tlpButtons.ColumnCount = 2
        Me.tlpButtons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpButtons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 107.0!))
        Me.tlpButtons.Controls.Add(Me.btnRename, 1, 0)
        Me.tlpButtons.Controls.Add(Me.Label3, 0, 0)
        Me.tlpButtons.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpButtons.Location = New System.Drawing.Point(3, 500)
        Me.tlpButtons.Name = "tlpButtons"
        Me.tlpButtons.RowCount = 1
        Me.tlpButtons.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpButtons.Size = New System.Drawing.Size(552, 35)
        Me.tlpButtons.TabIndex = 4
        '
        'frmTrackReplace
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(584, 564)
        Me.Controls.Add(Me.tlpMain)
        Me.MinimumSize = New System.Drawing.Size(600, 600)
        Me.Name = "frmTrackReplace"
        Me.Text = "frmTrackReplace"
        Me.tlpMain.ResumeLayout(False)
        Me.tlpMain.PerformLayout()
        Me.tlpButtons.ResumeLayout(False)
        Me.tlpButtons.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lbTracksNew As System.Windows.Forms.ListBox
    Friend WithEvents lbTracksOld As System.Windows.Forms.ListBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btnRename As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents tlpMain As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents tlpButtons As System.Windows.Forms.TableLayoutPanel
End Class
