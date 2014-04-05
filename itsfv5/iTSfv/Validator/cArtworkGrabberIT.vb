Imports iTunesLib
Imports System.IO
Imports System.Net
Imports System.Text

Public Class cArtworkGrabberIT


    Private mAlbum As String = ""
    Private mArtist As String = ""
    Private mName As String = ""
    Private mYear As Integer = 0

    Public Sub New(ByVal artist As String, ByVal album As String)

        Me.mArtist = artist
        Me.mAlbum = album

    End Sub

    Public Function fSplitArtists(ByVal lArtist As String) As List(Of String)

        Dim artists As New List(Of String)
        Dim subArtists As String() = lArtist.Split(CChar("&"))
        ' Academy of Ancient Music, Christopher Hogwood & Jaap Schröder
        For Each s As String In subArtists
            artists.Add(s.Trim)
        Next

        Dim temp1 As New List(Of String)
        For Each s As String In subArtists
            If s.Contains(",") Then
                Dim sp1 As String() = s.Split(CChar(","))
                For Each a As String In sp1
                    temp1.Add(a.Trim)
                Next
            End If
        Next
        For Each s As String In temp1
            artists.Add(s)
        Next

        artists.Sort()

        Return artists

    End Function

    Public Function fDownloadArtwork() As IITFileOrCDTrack

        If File.Exists(mArtworkGrabberFilePathMp3) = False Then
            My.Computer.Network.DownloadFile(New Uri(mArtworkGrabberUrlPathMp3), _
                                             mArtworkGrabberFilePathMp3, "", "", _
                                             True, My.Settings.TimeoutITMS, True, FileIO.UICancelOption.DoNothing)
        End If

        Dim song As IITFileOrCDTrack = Nothing

        If mItunesApp IsNot Nothing Then
            If mArtist <> String.Empty AndAlso mAlbum <> String.Empty Then
                If File.Exists(mArtworkGrabberFilePathMp3) Then

                    ' first attempt
                    song = fDownloadArtwork2(mArtist)

                    If song Is Nothing Then

                        If mArtist.Contains("&") Then

                            Dim lArtists As List(Of String) = fSplitArtists(mArtist)

                            If My.Settings.MultiArtistPromptArtwork = True Then

                                Dim dlg As New dlgArtistsList(lArtists)
                                dlg.ShowDialog()
                                dlg.Focus()
                                If dlg.DialogResult = DialogResult.OK Then
                                    song = fDownloadArtwork2(dlg.Artist)
                                Else
                                    Return Nothing
                                End If

                            Else

                                For Each lArtist As String In lArtists
                                    mfUpdateStatusBarText(String.Format("Changed Artist to ""{0}""...", lArtist), True)
                                    song = fDownloadArtwork2(lArtist)
                                    If song IsNot Nothing Then Exit For
                                Next

                            End If

                        End If

                        If song Is Nothing Then
                            '' 2nd retry
                            mfUpdateStatusBarText(String.Format("Changed Artist to ""{0}""...", VARIOUS_ARTISTS), True)
                            song = fDownloadArtwork2(VARIOUS_ARTISTS)

                            If song Is Nothing Then
                                '' 3rd retry
                                mfUpdateStatusBarText(String.Format("Changed Artist to ""{0}""...", "Compilation"), True)
                                song = fDownloadArtwork2("Compilation")
                            End If

                        End If

                    End If

                End If
            End If
        End If

        Dim succ As Boolean = song IsNot Nothing

        Return If(succ, song, Nothing)

    End Function

    Private Function fDownloadArtwork2(ByVal lArtist As String) As IITFileOrCDTrack

        Dim job As IITOperationStatus = Nothing
        Dim song As IITFileOrCDTrack = Nothing
        Dim succ As Boolean = False

        Try

            If mItunesApp IsNot Nothing Then

                If lArtist <> String.Empty AndAlso mAlbum <> String.Empty Then
                    If File.Exists(mArtworkGrabberFilePathMp3) Then

                        mfUpdateStatusBarText("Copying dummy mp3 file to Temporary folder...", True)
                        Dim destPath As String = Path.Combine(My.Settings.TempDir, Path.GetFileName(mArtworkGrabberFilePathMp3))
                        My.Computer.FileSystem.CopyFile(mArtworkGrabberFilePathMp3, destPath, True)

                        mfUpdateStatusBarText("Writing tags to dummy file...", True)
                        Dim f As TagLib.File = TagLib.File.Create(destPath)
                        f.Tag.Performers = New String() {lArtist}
                        f.Tag.Album = mAlbum
                        If mName <> String.Empty Then f.Tag.Title = mName
                        If mYear <> 0 Then f.Tag.Year = CUInt(mYear)
                        f.Save()

                        mfUpdateStatusBarText("Adding dummy mp3 file to iTunes...", True)
                        job = mItunesApp.LibraryPlaylist.AddFile(destPath)

                        mfUpdateStatusBarText("Waiting for Artwork download to complete...", True)
                        System.Threading.Thread.Sleep(5000)

                        If job IsNot Nothing Then
                            If job.Tracks IsNot Nothing Then
                                If job.Tracks.Count > 0 Then
                                    song = CType(job.Tracks(1), IITFileOrCDTrack)
                                    If song IsNot Nothing Then
                                        msAppendDebug(String.Format("Temporarily added dummy file with Artist: ""{0}"", Album: ""{1}""", song.Artist, song.Album))
                                        succ = song.Artwork.Count > 0 ' success if artwork found
                                        If succ = False Then
                                            If IO.File.Exists(song.Location) Then
                                                My.Computer.FileSystem.DeleteFile(song.Location)
                                            End If
                                            song.Delete()
                                        End If
                                    End If
                                End If
                            End If
                        End If

                    End If
                End If
            End If

        Catch ex As Exception
            msAppendWarnings(ex.Message + " while grabbing Artwork using Artwork Grabber.")
        End Try

        Return If(succ, song, Nothing)

    End Function

    Public Sub New(ByVal filePath As String)

        If File.Exists(filePath) Then

            Dim xt As New cXmlTrack(filePath, False)
            zFillTags(xt)

        End If

    End Sub

    Private Sub zFillTags(ByVal xt As cXmlTrack)

        If xt.Compilation = True Then
            mArtist = VARIOUS_ARTISTS
        Else
            mArtist = xt.Artist
        End If

        mAlbum = xt.Album
        mName = xt.Name
        mYear = xt.Year

    End Sub

    Public Sub New(ByVal song As IITTrack)

        Dim xt As New cXmlTrack(song, False)
        zFillTags(xt)

    End Sub


End Class
