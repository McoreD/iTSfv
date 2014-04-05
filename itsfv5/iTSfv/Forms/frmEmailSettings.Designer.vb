<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEmailSettings
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEmailSettings))
        Me.Label15 = New System.Windows.Forms.Label
        Me.Label13 = New System.Windows.Forms.Label
        Me.Label12 = New System.Windows.Forms.Label
        Me.btnOK = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.ttApp = New System.Windows.Forms.ToolTip(Me.components)
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.lblEmailPass = New System.Windows.Forms.Label
        Me.txtEmailPassword = New System.Windows.Forms.TextBox
        Me.txtEmailAddress = New System.Windows.Forms.TextBox
        Me.nudSMTPPort = New System.Windows.Forms.NumericUpDown
        Me.txtSMTPHost = New System.Windows.Forms.TextBox
        Me.GroupBox1.SuspendLayout()
        CType(Me.nudSMTPPort, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(18, 26)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(76, 13)
        Me.Label15.TabIndex = 36
        Me.Label15.Text = "Email Address:"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(18, 103)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(62, 13)
        Me.Label13.TabIndex = 33
        Me.Label13.Text = "SMTP Port:"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(18, 77)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(74, 13)
        Me.Label12.TabIndex = 32
        Me.Label12.Text = "SMTP Server:"
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(250, 162)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 23)
        Me.btnOK.TabIndex = 3
        Me.btnOK.Text = "&OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(331, 162)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 4
        Me.btnCancel.Text = "&Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.txtEmailPassword)
        Me.GroupBox1.Controls.Add(Me.lblEmailPass)
        Me.GroupBox1.Controls.Add(Me.txtEmailAddress)
        Me.GroupBox1.Controls.Add(Me.Label15)
        Me.GroupBox1.Controls.Add(Me.nudSMTPPort)
        Me.GroupBox1.Controls.Add(Me.txtSMTPHost)
        Me.GroupBox1.Controls.Add(Me.Label13)
        Me.GroupBox1.Controls.Add(Me.Label12)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(411, 138)
        Me.GroupBox1.TabIndex = 40
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Please provide your Contact Details..."
        '
        'lblEmailPass
        '
        Me.lblEmailPass.AutoSize = True
        Me.lblEmailPass.Location = New System.Drawing.Point(18, 51)
        Me.lblEmailPass.Name = "lblEmailPass"
        Me.lblEmailPass.Size = New System.Drawing.Size(84, 13)
        Me.lblEmailPass.TabIndex = 38
        Me.lblEmailPass.Text = "Email Password:"
        '
        'txtEmailPassword
        '
        Me.txtEmailPassword.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.iTSfv.My.MySettings.Default, "EmailPassword", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.txtEmailPassword.Location = New System.Drawing.Point(124, 48)
        Me.txtEmailPassword.Name = "txtEmailPassword"
        Me.txtEmailPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtEmailPassword.Size = New System.Drawing.Size(270, 20)
        Me.txtEmailPassword.TabIndex = 37
        Me.txtEmailPassword.Text = Global.iTSfv.My.MySettings.Default.EmailPassword
        '
        'txtEmailAddress
        '
        Me.txtEmailAddress.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.iTSfv.My.MySettings.Default, "EmailAddress", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.txtEmailAddress.Location = New System.Drawing.Point(124, 22)
        Me.txtEmailAddress.Name = "txtEmailAddress"
        Me.txtEmailAddress.Size = New System.Drawing.Size(270, 20)
        Me.txtEmailAddress.TabIndex = 0
        Me.txtEmailAddress.Text = Global.iTSfv.My.MySettings.Default.EmailAddress
        Me.ttApp.SetToolTip(Me.txtEmailAddress, "For example:  mcored@ii.net")
        '
        'nudSMTPPort
        '
        Me.nudSMTPPort.DataBindings.Add(New System.Windows.Forms.Binding("Value", Global.iTSfv.My.MySettings.Default, "SMTPPort", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.nudSMTPPort.Location = New System.Drawing.Point(124, 100)
        Me.nudSMTPPort.Maximum = New Decimal(New Integer() {65536, 0, 0, 0})
        Me.nudSMTPPort.Name = "nudSMTPPort"
        Me.nudSMTPPort.Size = New System.Drawing.Size(120, 20)
        Me.nudSMTPPort.TabIndex = 2
        Me.nudSMTPPort.Value = Global.iTSfv.My.MySettings.Default.SMTPPort
        '
        'txtSMTPHost
        '
        Me.txtSMTPHost.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.iTSfv.My.MySettings.Default, "SMTPHost", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.txtSMTPHost.Location = New System.Drawing.Point(124, 74)
        Me.txtSMTPHost.Name = "txtSMTPHost"
        Me.txtSMTPHost.Size = New System.Drawing.Size(270, 20)
        Me.txtSMTPHost.TabIndex = 1
        Me.txtSMTPHost.Text = Global.iTSfv.My.MySettings.Default.SMTPHost
        Me.ttApp.SetToolTip(Me.txtSMTPHost, "For example: mail.ii.net")
        Me.ToolTip1.SetToolTip(Me.txtSMTPHost, "E.g. mail.ii.net")
        '
        'frmEmailSettings
        '
        Me.AcceptButton = Me.btnOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(432, 203)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmEmailSettings"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "frmEmailSettings"
        Me.TopMost = True
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.nudSMTPPort, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents txtEmailAddress As System.Windows.Forms.TextBox
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents nudSMTPPort As System.Windows.Forms.NumericUpDown
    Friend WithEvents txtSMTPHost As System.Windows.Forms.TextBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents ttApp As System.Windows.Forms.ToolTip
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents txtEmailPassword As System.Windows.Forms.TextBox
    Friend WithEvents lblEmailPass As System.Windows.Forms.Label
End Class
