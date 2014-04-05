Public Class frmErrors

    Private Sub frmErrors_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.Icon = My.Forms.frmMain.Icon
        Me.Text = Application.ProductName + " Errors Log"

    End Sub

    Public Sub New(ByVal log As String)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        txtErrors.Text = log

    End Sub
End Class

