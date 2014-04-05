Public Class cInfoTrack

    ' not serializable

    Public IsArtworkMissing As Boolean = False
    Public IsAlbumArtistMissing As Boolean = False
    Public IsFileMissing As Boolean = False

    Private mTrack As iTunesLib.IITFileOrCDTrack
    Public Property Track() As iTunesLib.IITFileOrCDTrack
        Get
            Return mTrack
        End Get
        Set(ByVal value As iTunesLib.IITFileOrCDTrack)
            mTrack = value
        End Set
    End Property

    Private mGoogleSearchArtwork As String
    Public Property GoogleSearchArtwork() As String
        Get
            Return mGoogleSearchArtwork
        End Get
        Set(ByVal value As String)
            mGoogleSearchArtwork = value
        End Set
    End Property

    Private mNotes As String
    Public Property Notes() As String
        Get
            Return mNotes
        End Get
        Set(ByVal value As String)
            mNotes = value
        End Set
    End Property
    Public Function fGetGoogleSearchURL() As String

        Dim msg As String = ""

        If Track.AlbumArtist <> String.Empty Then
            msg = String.Format("<a href={0}{1}{0}>{2} - {3}</a>", Chr(34), GoogleSearchArtwork, Track.AlbumArtist, Track.Album)
        Else
            msg = String.Format("<a href={0}{1}{0}>{2} - {3}</a>", Chr(34), GoogleSearchArtwork, Track.Artist, Track.Album)
        End If

        Return msg

    End Function

End Class
