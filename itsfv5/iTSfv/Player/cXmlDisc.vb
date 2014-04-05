Imports System.Globalization

<Serializable()> Public Class cXmlDisc

    Private mDiscID As UInteger
    Public Property DiscID() As UInteger
        Get
            Return mDiscID
        End Get
        Set(ByVal value As UInteger)
            mDiscID = value
        End Set
    End Property

    Public Function HasTrack(ByVal track As cXmlTrack) As Boolean
        '' 5.32.0.4 iTSfv showed duplicated tracklists if the same album was added to iTunes multiple times
        For Each oTrack As cXmlTrack In mTracks
            If track.TrackNumber = oTrack.TrackNumber AndAlso _
            track.Name.Equals(oTrack.Name) Then
                Return True
            End If
        Next
        Return False
    End Function

    Private mAlbumArtist As String = VARIOUS_ARTISTS
    Public Property Artist() As String
        Get
            Return mAlbumArtist
        End Get
        Set(ByVal value As String)
            If value <> String.Empty Then
                mAlbumArtist = value
            End If
        End Set
    End Property

    Private mAlbumName As String = UNKNOWN_ALBUM
    Public Property AlbumName() As String
        Get
            Return mAlbumName
        End Get
        Set(ByVal value As String)
            If value <> String.Empty Then
                mAlbumName = value
            End If
        End Set
    End Property

    Private mArtworkSources As cArtworkSource = Nothing
    Public Property ArtworkSource() As cArtworkSource
        Get
            Return mArtworkSources
        End Get
        Set(ByVal value As cArtworkSource)
            mArtworkSources = value
        End Set
    End Property

    Private mDiscName As String = "Unknown Disc"
    Public Property Name() As String
        Get
            Return mDiscName
        End Get
        Set(ByVal value As String)
            If value <> String.Empty Then
                mDiscName = value
            End If
        End Set
    End Property

    Private mGenre As String
    Public Property Genre() As String
        Get
            Return mGenre
        End Get
        Set(ByVal value As String)
            mGenre = value
        End Set
    End Property

    Private mYear As UInteger
    Public Property Year() As UInteger
        Get
            Return mYear
        End Get
        Set(ByVal value As UInteger)
            mYear = value
        End Set
    End Property

    Private mHighestTrackNum As UInteger
    Public Property HighestTrackNumber() As UInteger
        Get
            Return mHighestTrackNum
        End Get
        Set(ByVal value As UInteger)
            mHighestTrackNum = value
        End Set
    End Property

    Public ReadOnly Property FirstTrack() As cXmlTrack
        Get
            Return mTracks(0)
        End Get
    End Property

    Private mArtworkCachePath As String = String.Empty
    Public ReadOnly Property ArtworkPath() As String
        Get
            If String.Empty = mArtworkCachePath Then
                mArtworkCachePath = mfGetArtworkCachePath(Tracks(0))
            End If
            Return mArtworkCachePath
        End Get
    End Property

    Public Function AddTrack(ByVal track As cXmlTrack) As Boolean
        Dim success As Boolean = False
        If HasTrack(track) = False Then
            mTracks.Add(track)
            success = True
        End If
        Return success
    End Function

    Private mTracks As New List(Of cXmlTrack)
    Public Property Tracks() As List(Of cXmlTrack)
        Get
            Return mTracks
        End Get
        Set(ByVal value As List(Of cXmlTrack))
            mTracks = value
        End Set
    End Property

    Private mIsComplete As Boolean
    Public Property IsComplete() As Boolean
        Get
            Return mIsComplete
        End Get
        Set(ByVal value As Boolean)
            mIsComplete = value
        End Set
    End Property

    Public Function fGetAlbumName() As String

        For Each track As cXmlTrack In Tracks
            If track.Album <> "" Then
                Return Tracks(0).Album
            End If
        Next

        Return "Unknown Album"

    End Function

    Public Function fGetArtistName() As String

        For Each track As cXmlTrack In Tracks
            If track.Compilation = True Then
                Return "Compilations"
            End If
        Next

        Return Tracks.Item(0).AlbumArtist

    End Function

    Private mStandardText As String
    Public Property StandardText() As String
        Get
            Return mStandardText
        End Get
        Set(ByVal value As String)
            mStandardText = value
        End Set
    End Property

    Private mTrackList As String
    Public Property TrackList() As String
        Get
            Return mTrackList
        End Get
        Set(ByVal value As String)
            mTrackList = value
        End Set
    End Property

    Public Overloads Function ToString(ByVal bBitRate As Boolean, ByVal bSize As Boolean) As String

        Dim tracks As New List(Of String)

        For Each track As cXmlTrack In mTracks

            Dim l As New System.Text.StringBuilder

            l.Append(track.TrackNumber.ToString("00") + " " + track.Name)

            If bBitRate = True Then
                l.Append(String.Format(" [{0} Kibit/s]", track.BitRate))
            End If

            If bSize = True Then
                Dim sz As Decimal = CDec(track.Size / (1024))
                l.Append(String.Format(" [{0} KiB]", sz.ToString("N0", CultureInfo.CurrentCulture)))
            End If

            tracks.Add(l.ToString)

        Next

        tracks.Sort()

        Dim sb As New System.Text.StringBuilder
        For Each l As String In tracks
            sb.AppendLine(l)
        Next

        Return sb.ToString

    End Function

    Public ReadOnly Property Location() As String
        Get
            Return IO.Path.GetDirectoryName(mTracks(0).Location)
        End Get
    End Property

    Public ReadOnly Property GoogleSearchURL() As String

        Get

            ' generate google artwork search string
            Dim url As String = ""
            Dim track As cXmlTrack = mTracks(0)

            If track.AlbumArtist <> String.Empty Then
                url = String.Format("http://www.google.com/search?q={0}+%22{1}%22", track.Album, track.AlbumArtist)
            Else
                url = String.Format("http://www.google.com/search?q=%22{0}%22+%22{1}%22", track.Album, track.Artist)
            End If

            Return url

        End Get
    End Property


    Public Overrides Function ToString() As String
        Return ToString(False, False)
    End Function

    Public Sub New(ByVal filePaths As List(Of String))

        For Each p As String In filePaths

            Try
                Dim xt As New cXmlTrack(p, False)
                Me.Tracks.Add(xt)
            Catch ex As Exception
                ' we will see 
            End Try

        Next

        If Me.Tracks.Count > 0 Then
            Me.Name = mGetDiscName(Me.FirstTrack)
            Me.AlbumName = mGetAlbumName(Me.FirstTrack)
        End If

    End Sub

    Public Sub New(ByVal lTrack As cXmlTrack)

        Me.Name = mGetDiscName(lTrack)
        Me.AlbumName = mGetAlbumName(lTrack)

    End Sub
End Class
