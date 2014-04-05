Public Class frmDebug

    Private Sub frmDebug_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.Icon = My.Forms.frmMain.Icon
        Me.Text = Application.ProductName + " Debug Log"

    End Sub

    'Public Sub New(ByVal log As String)

    '    ' This call is required by the Windows Form Designer.
    '    InitializeComponent()

    '    ' Add any initialization after the InitializeComponent() call.
    '    txtDebug.Text = log

    'End Sub
End Class