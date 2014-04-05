Public Class cXmlAlbumArtist

    Private mAlbums As New Hashtable
    Private mAlbumKeys As New List(Of String)
    Private mDiscKeys As New List(Of String)

    Private mAlbumArtist As String = UNKNOWN_ARTIST

    Public ReadOnly Property AlbumKeys() As List(Of String)
        Get
            Return mAlbumKeys
        End Get
    End Property

    Public ReadOnly Property DiscKeys() As List(Of String)
        Get
            Return mDiscKeys
        End Get
    End Property

    Public Property Albums() As Hashtable
        Get
            Return mAlbums
        End Get
        Set(ByVal value As Hashtable)
            mAlbums = value
        End Set
    End Property

    Public Function AddAlbum(ByVal lAlbumName As String, ByVal lAlbum As cXmlAlbum) As Boolean

        Dim success As Boolean = False
        If mAlbumKeys.Contains(lAlbumName) = False Then
            ' new Album
            mAlbumKeys.Add(lAlbumName)
            mAlbums.Add(lAlbumName, lAlbum)
            For Each lDisc As cXmlDisc In lAlbum.Discs
                mDiscKeys.Add(lDisc.Name)
            Next
        Else
            ' replace current album with updated discs info
            For Each lDisc As cXmlDisc In lAlbum.Discs
                CType(mAlbums(lAlbumName), cXmlAlbum).AddDisc(lDisc.Name, lDisc)
            Next
        End If
        Return success

    End Function

    Public Sub New(ByVal lAlbumArtist As String)
        Me.mAlbumArtist = lAlbumArtist
    End Sub

    Public ReadOnly Property AlbumArtist() As String
        Get
            Return mAlbumArtist
        End Get
    End Property

End Class
