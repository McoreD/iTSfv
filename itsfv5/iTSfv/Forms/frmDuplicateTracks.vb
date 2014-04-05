Imports iTunesLib

Public Class frmDuplicateTracks

    Private mResult As Windows.Forms.DialogResult = Windows.Forms.DialogResult.No
    Private mTracks As List(Of IITTrack)
    Dim listDdv As New List(Of sDgvTrack)

    Public Overloads ReadOnly Property DialogResult() As Windows.Forms.DialogResult
        Get
            Return mResult
        End Get
    End Property

    Private Sub btnYes_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnYes.Click

        mResult = Windows.Forms.DialogResult.Yes
        Me.Close()

    End Sub

    Private Sub btnNo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNo.Click

        mResult = Windows.Forms.DialogResult.No
        Me.Close()

    End Sub

    Public Sub New(ByRef tracks As List(Of IITTrack))

        mTracks = tracks

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        For Each track As IITTrack In tracks
            listDdv.Add(New sDgvTrack(track))
        Next

        dgvTracks.DataSource = listDdv

    End Sub

    Private Structure sDgvTrack

        Private mName As String
        Private mArtist As String
        Private mAlbum As String
        Private mLocation As String
        Private mTrack As IITTrack

        Sub New(ByVal lTrack As IITTrack)

            mArtist = lTrack.Artist
            mAlbum = lTrack.Album
            mName = lTrack.Name
            mTrack = lTrack

            If lTrack.Kind = ITTrackKind.ITTrackKindFile Then
                mLocation = CType(lTrack, IITFileOrCDTrack).Location
            End If

        End Sub

        Public Property Artist() As String
            Get
                Return mArtist
            End Get
            Set(ByVal value As String)
                mArtist = value
            End Set
        End Property

        Public Property Album() As String
            Get
                Return mAlbum
            End Get
            Set(ByVal value As String)
                mAlbum = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return mName
            End Get
            Set(ByVal value As String)
                mName = value
            End Set
        End Property

        Public Property Location() As String
            Get
                Return mLocation
            End Get
            Set(ByVal value As String)
                mLocation = value
            End Set
        End Property

    End Structure

    Private Sub btnRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemove.Click

        Dim rowsList As New List(Of DataGridViewRow)

        For i As Integer = 0 To dgvTracks.SelectedRows.Count - 1
            rowsList.Add(dgvTracks.SelectedRows(i))
        Next

        For Each row As DataGridViewRow In rowsList
            dgvTracks.Rows.Remove(row)
        Next

    End Sub

End Class