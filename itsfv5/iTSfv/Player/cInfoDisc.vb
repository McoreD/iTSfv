Imports iTunesLib
Imports System.Globalization
Imports System.Reflection

' not serializable

Public Class cInfoDisc

    ' NOTE: DO NOT HAVE A PROPERTY FOR IMAGE AND STORE THE IMAGE AS IT COULD CAUSE
    ' MASSIVE MEMORY LEAKS. AN IMAGE CAN BE AS LARGE 1 MiB SO STORING AN IMAGE FOR EACH 
    ' ALBUM MEANS THAT YOU ARE GOING TO CONSUME ABOUT 1 GiB of RAM for iTSfv WHEN YOU 
    ' VALIDATE ITUNES MUSIC LIBRARY

    Private mDiscID As Integer
    Public Property DiscID() As Integer
        Get
            Return mDiscID
        End Get
        Set(ByVal value As Integer)
            mDiscID = value
        End Set
    End Property

    Private mTrackCount As Integer = 0
    Public Property TrackCount() As Integer
        Get
            Return mTrackCount
        End Get
        Set(ByVal value As Integer)
            mTrackCount = value
        End Set
    End Property

    Private mDiscCount As Integer = 0
    Public Property DiscCount() As Integer
        Get
            Return mDiscCount
        End Get
        Set(ByVal value As Integer)
            mDiscCount = value
        End Set
    End Property

    Private mDiscNum As Integer = 0
    Public Property DiscNumber() As Integer
        Get
            Return mDiscNum
        End Get
        Set(ByVal value As Integer)
            mDiscNum = value
        End Set
    End Property

    Public Function HasTrack(ByVal track As IITFileOrCDTrack) As Boolean
        '' 5.32.0.4 iTSfv showed duplicated tracklists if the same album was added to iTunes multiple times
        For Each oTrack As IITFileOrCDTrack In mTracks
            If track.TrackNumber = oTrack.TrackNumber AndAlso _
            track.Name.Equals(oTrack.Name) Then
                Return True
            End If
        Next
        Return False
    End Function

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

    Private mArtworkSources As cArtworkSource = Nothing
    Public Property ArtworkSource() As cArtworkSource
        Get
            Return mArtworkSources
        End Get
        Set(ByVal value As cArtworkSource)
            mArtworkSources = value
        End Set
    End Property

    Private mDiscName As String = String.Empty
    Public ReadOnly Property DiscName() As String
        Get
            If String.Empty = mDiscName Then
                mDiscName = fGetAlbumName()
            End If
            Return mDiscName
        End Get
    End Property

    Private mName As String = "Unknown Album"
    Public Property Name() As String
        Get
            Return mName
        End Get
        Set(ByVal value As String)
            If value <> String.Empty Then
                mName = value
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

    Private mYear As Integer
    Public Property Year() As Integer
        Get
            Return mYear
        End Get
        Set(ByVal value As Integer)
            mYear = value
        End Set
    End Property

    Dim SortedTracks As New List(Of IITFileOrCDTrack)

    Public ReadOnly Property FirstTrack() As IITFileOrCDTrack

        Get
            If SortedTracks.Count = 0 Then
                SortedTracks.AddRange(mTracks)
                SortedTracks.Sort(New cIITFileOrCDTrackComparer)
            End If
            Return SortedTracks(0)
        End Get

    End Property

    Private mTracks As New List(Of iTunesLib.IITFileOrCDTrack)
    Public Property Tracks() As List(Of iTunesLib.IITFileOrCDTrack)
        Get
            Return mTracks
        End Get
        Set(ByVal value As List(Of iTunesLib.IITFileOrCDTrack))
            mTracks = value
        End Set
    End Property

    Private mTracksXml As New List(Of cXmlTrack)
    Public Property TracksXML() As List(Of cXmlTrack)
        Get
            Return mTracksXml
        End Get
        Set(ByVal value As List(Of cXmlTrack))
            mTracksXml = value
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

        For Each track As IITFileOrCDTrack In Tracks
            If track.Album <> "" Then
                Return Tracks(0).Album
            End If
        Next

        Return "Unknown Album"

    End Function

    Public Function fGetArtistName() As String

        For Each track As IITFileOrCDTrack In Tracks
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

    Private mArtworkImage As Image = Nothing
    ''' <summary>
    ''' Property to set a Artwork Image. Be sure to clear this after use to avoid memory leaks.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ArtworkImage() As Image
        Get
            Return mArtworkImage
        End Get
        Set(ByVal value As Image)
            mArtworkImage = value
        End Set
    End Property

    Public Property Confidence() As Double
        Get

        End Get
        Set(ByVal value As Double)

        End Set
    End Property

    Public Function fGetArtwork() As Image

        Try
            For Each track As IITFileOrCDTrack In Tracks
                Dim f As TagLib.File = TagLib.File.Create(track.Location)
                If f.Tag.Pictures.Length > 0 Then
                    ' storing this in mArtworkImage will create Memory leaks
                    Return mTagLibJobs.mfPictureToImage(f.Tag.Pictures(0))
                End If
            Next
        Catch ex As Exception
            msAppendDebug(ex.Message + " while getting artwork for disc")
        End Try

        Return Nothing

    End Function

    Public Overloads Function ToString(ByVal bBitRate As Boolean, ByVal bSize As Boolean) As String

        Dim tracks As New List(Of String)

        For Each track As IITFileOrCDTrack In mTracks

            Dim l As New System.Text.StringBuilder

            l.Append(track.TrackNumber.ToString("00") + " " + track.Name)

            If bBitRate = True Then
                l.Append(String.Format(" [{0} Kb/s]", track.BitRate))
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

    Private mLocation As String = String.Empty
    Public ReadOnly Property Location() As String
        Get
            If String.Empty = mLocation Then
                Try
                    mLocation = IO.Path.GetDirectoryName(mTracks(0).Location)
                Catch ex As Exception
                    msAppendWarnings(Me.Name)
                    msAppendWarnings(ex.Message + " while getting Location for track.")
                    msAppendWarnings("Try relocating files into a shorter file path.")
                End Try
            End If
            Return mLocation
        End Get
    End Property

    Public ReadOnly Property GoogleSearchURL() As String

        Get

            ' generate google artwork search string

            Dim track As IITFileOrCDTrack = mTracks(0)
            Dim artist As String = CStr(IIf(String.Empty <> track.AlbumArtist, track.AlbumArtist, track.Artist))
            Dim url As String = "http://www.google.com/search?q=" + mfEncodeUrl(track.Album + " " + artist)

            Return url

        End Get
    End Property


    Public Overrides Function ToString() As String
        Return ToString(False, False)
    End Function



End Class
