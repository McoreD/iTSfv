<Serializable()> Public Class cXmlAlbum

    Private mDiscs As New Hashtable
    Private mDiscKeys As New List(Of String)

    Private mAlbumName As String = UNKNOWN_ARTIST + " - " + UNKNOWN_ALBUM

    Public ReadOnly Property DiscKeys() As List(Of String)
        Get
            Return mDiscKeys
        End Get
    End Property

    Public Property Name() As String
        Get
            Return mAlbumName
        End Get
        Set(ByVal value As String)
            mAlbumName = value
        End Set
    End Property

    Public Function AddDisc(ByVal lDiscName As String, ByVal lDisc As cXmlDisc) As Boolean

        Dim success As Boolean = False
        If mDiscKeys.Contains(lDiscName) = False Then
            ' new Disc
            mDiscKeys.Add(lDiscName)
            mDiscs.Add(lDiscName, lDisc)
        Else
            ' Disc is present
            For Each track As cXmlTrack In lDisc.Tracks
                ' Add new tracks
                CType(mDiscs(lDiscName), cXmlDisc).AddTrack(track)
            Next
        End If

    End Function

    Public ReadOnly Property Discs() As Hashtable
        Get
            Return mDiscs
        End Get
    End Property

    Public Function HasDisc(ByVal disc As cXmlDisc) As Boolean

        For Each myDisc As cXmlDisc In Discs
            If myDisc.Name.Equals(disc.Name) Then
                Return True
            End If
        Next

        Return False

    End Function

    Public Sub New(ByVal lAlbumName As String)

        mAlbumName = lAlbumName

    End Sub


End Class
