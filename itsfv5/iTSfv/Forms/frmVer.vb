Public Class frmVer

    Private Sub frmVer_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.Icon = My.Forms.frmMain.Icon

        Me.Text = String.Format("{0} {1} Version History", Application.ProductName, Application.ProductVersion)

        txtVer.Text = mfGetText("VersionHistory.txt")

    End Sub

End Class

