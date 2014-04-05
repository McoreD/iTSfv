Public Class frmEmailSettings

    Private mResult As Windows.Forms.DialogResult = Windows.Forms.DialogResult.Cancel

    Public Overloads ReadOnly Property DialogResult() As Windows.Forms.DialogResult
        Get
            Return mResult
        End Get
    End Property

    Public Overloads ReadOnly Property EmailTypeIsWeb() As Boolean
        Get
            Return fBooWebMail()
        End Get
    End Property

    Private Sub frmEmailSettings_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

    End Sub

    Private Sub frmEmailSettings_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Text = Application.ProductName + " - Bug Report Wizard"
    End Sub

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        mResult = Windows.Forms.DialogResult.OK
        My.Settings.Save()
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        mResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub frmEmailSettings_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        txtEmailAddress.Focus()
    End Sub

    Private Sub txtEmailAddress_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtEmailAddress.TextChanged
        txtSMTPHost.Enabled = Not fBooWebMail()
        nudSMTPPort.Enabled = Not fBooWebMail()
        'txtEmailPassword.Enabled = fBooWebMail()
    End Sub

    Private Function fBooWebMail() As Boolean

        Dim webMail As String() = New String() {"gmail.com"}
        For Each s As String In webMail
            If txtEmailAddress.Text.Contains(s) Then
                Return True
            End If
        Next

        Return False

    End Function

End Class