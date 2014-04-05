Imports System.Windows.Forms

Public Class dlgArtistsList

    Private mArtist As String = ""
    Public ReadOnly Property Artist() As String
        Get
            Return mArtist
        End Get
    End Property

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        mArtist = lbArtists.SelectedItem.ToString
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Public Sub New(ByVal myList As List(Of String))

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        For Each s As String In myList
            lbArtists.Items.Add(s)
        Next

    End Sub

    Private Sub dlgArtistsList_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Icon = My.Forms.frmMain.Icon
    End Sub
End Class
