Imports System.Web
Imports System.IO
Imports iTunesLib
Imports System.Xml.Serialization

Public Class cXspfCreator

    Private mWhere As String
    Private mXmlTracks As List(Of cXmlTrack)
    Private mDisc As cInfoDisc = Nothing

    Public Sub New(ByVal where As String, ByVal lDisc As cInfoDisc)

        mWhere = where
        mDisc = lDisc
        mXmlTracks = New List(Of cXmlTrack)
        For Each track As IITFileOrCDTrack In lDisc.Tracks
            Try
                mXmlTracks.Add(New cXmlTrack(track, False))
            Catch ex As Exception
                ' oh well - errors like track deleted
            End Try
        Next

    End Sub

    Public Sub New(ByVal where As String, ByVal lTracks As List(Of cXmlTrack))

        Me.mWhere = where
        Me.mXmlTracks = lTracks

    End Sub

    Public Function fGetUri(ByVal loc As String) As String
        Dim u As String = New Uri(loc).ToString
        u = u.Replace(" ", "%20")
        u = u.Replace(Chr(34), "%22")
        Return u
    End Function

    Private Sub RenderPlaylistHeader(ByVal lDisc As cInfoDisc, ByVal playlist As cXspf)

        playlist.version = CStr(1)
        If lDisc Is Nothing Then
            playlist.title = Path.GetFileNameWithoutExtension(mWhere)
            playlist.info = New Uri(Path.GetDirectoryName(mWhere)).ToString().Replace(" ", "%20")
        Else
            playlist.title = lDisc.Name
            playlist.info = fGetUri(lDisc.GoogleSearchURL)
            If lDisc.ArtworkSource IsNot Nothing Then
                Dim folderImageFilePath As String = lDisc.ArtworkSource.ArtworkPath
                If File.Exists(folderImageFilePath) Then
                    playlist.image = fGetUri(folderImageFilePath)
                End If
            End If
        End If

        'playlist.annotation = "Playlist generated on the fly from local media files."
        playlist.creator = String.Format("{0} {1} XSPF Generator", Application.ProductName, Application.ProductVersion)
        playlist.location = New Uri(mWhere).ToString.Replace(" ", "%20")

        playlist.dateString = Date.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")

    End Sub

    Private Sub RenderPlaylistTracks(ByVal lTracks As List(Of cXmlTrack), ByVal playlist As cXspf)

        Dim trackList As New List(Of TrackType)
        For Each track As cXmlTrack In lTracks
            Try
                Dim trackInfo As New TrackType
                trackInfo.title = track.Name
                trackInfo.album = track.Album
                trackInfo.creator = track.Artist
                trackInfo.trackNum = CStr(track.TrackNumber)
                trackInfo.location = New String() {New Uri(track.Location).ToString.Replace(" ", "%20")}
                trackList.Add(trackInfo)
            Catch ex As Exception
                ' errors - unsure yet
            End Try
        Next
        playlist.trackList = trackList.ToArray

    End Sub

    Public Function Save() As Boolean

        Dim success As Boolean = False
        Try
            Dim pl As New cXspf
            RenderPlaylistHeader(mDisc, pl)
            RenderPlaylistTracks(mXmlTracks, pl)
            Using sw As New StreamWriter(mWhere)
                Dim ser As New XmlSerializer(pl.GetType)
                ser.Serialize(sw, pl)
            End Using
            success = True
        Catch ex As Exception
            success = False
        End Try

        Return success

    End Function

End Class
