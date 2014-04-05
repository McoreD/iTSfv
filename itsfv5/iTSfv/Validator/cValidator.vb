Imports System.ComponentModel
Imports System.IO
Imports iTSfv.cBwJob
Imports UploadersLib.OtherServices

Public Class cValidator

    ' Class responsible for creating a new validator instance 
    ' Validate Library 
    ' Validate Selected Tracks
    ' Validate Last 100 Tracks
    ' Validate Disc

    Private bwApp As BackgroundWorker
    Private mListAlbumKeys As New List(Of String)
    Private mTableAlbums As New Hashtable
    Private mDiscKeys As New List(Of String)
    Private mTableDiscs As New Hashtable

    Private mArtistKeys As New List(Of String)
    Private mAlbumArtists As New Hashtable

    Public ReadOnly Property DiscKeys() As List(Of String)
        Get
            Return mDiscKeys
        End Get
    End Property

    Public Sub New(ByVal bw As BackgroundWorker)
        Me.bwApp = bw
    End Sub

    Public ReadOnly Property AlbumArtistKeys() As List(Of String)
        Get
            Return mArtistKeys
        End Get
    End Property

    Public ReadOnly Property AlbumArtists() As Hashtable
        Get
            Return mAlbumArtists
        End Get
    End Property

    Public Function ValidateLibrary() As Boolean

        Dim success As Boolean = True

        bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, DiscKeys.Count)

        For Each lArtistKey As String In AlbumArtistKeys

            Dim lArtist As cXmlAlbumArtist = CType(AlbumArtists(lArtistKey), cXmlAlbumArtist)

            For Each lAlbumKey As String In lArtist.AlbumKeys

                Dim lAlbum As cXmlAlbum = CType(lArtist.Albums(lAlbumKey), cXmlAlbum)

                For Each lDiscKey As String In lAlbum.DiscKeys

                    bwApp.ReportProgress(ProgressType.VALIDATING_DISC_IN_ITUNES_LIBRARY, lDiscKey)

                    Dim lDisc As cXmlDisc = CType(lAlbum.Discs(lDiscKey), cXmlDisc)
                    success = success And ValidateDisc(lDisc)

                Next

            Next

        Next

        bwApp.ReportProgress(ProgressType.READY, String.Format("Validated {0} Discs", DiscKeys.Count))

        Return success

    End Function

    Public Function fWriteArtwork(ByVal picture As TagLib.IPicture, ByVal oFilePath As String) As Boolean

        Dim success As Boolean = True

        Try
            Dim lExt As String = picture.MimeType.Substring(picture.MimeType.IndexOf("/") + 1)
            lExt = lExt.Replace("jpeg", "jpg")

            Dim lFilePath As String = Path.Combine(Path.GetDirectoryName(oFilePath), String.Format("{0}.{1}", Path.GetFileNameWithoutExtension(oFilePath), lExt))

            If File.Exists(lFilePath) Then
                Dim fiArtwork As New FileInfo(lFilePath)
                fiArtwork.Attributes = FileAttributes.Normal
            End If

            Dim lStream As System.IO.Stream = IO.File.Open(lFilePath, IO.FileMode.Create)

            Dim lData() As Byte = picture.Data.Data
            lStream.Write(lData, 0, lData.Length)
            lStream.Close()
        Catch ex As Exception
            msAppendWarnings(ex.Message + " while exporting artwork using TagLib")
            success = False
        End Try

        Return success

    End Function

    Private Function fExportArtwork(ByVal lDisc As cXmlDisc) As Boolean

        Dim bExported As Boolean = True

        For Each track As cXmlTrack In lDisc.Tracks

            If track.Location IsNot Nothing Then

                Dim f As TagLib.File = TagLib.File.Create(track.Location)

                If f.Tag.Pictures.Length > 0 Then
                    ' we have a track with artwork

                    If My.Settings.DefaultExArtworkFolder = True Then

                        '**********************
                        '* Folder.jpg
                        '**********************
                        Dim jpgFolder As String = IO.Path.GetDirectoryName(track.Location) + "\Folder"
                        If My.Settings.DisableJpgFolder = False Then

                            ' see if we are creating a new folder.jpg
                            Dim booWasThere As Boolean = File.Exists(jpgFolder)

                            ' write artwork to file
                            bExported = bExported And fWriteArtwork(f.Tag.Pictures(0), jpgFolder)

                            ' if we have succesffully saved folder.jpg
                            If My.Settings.ReadOnlyFolderJpg Then
                                Try
                                    File.SetAttributes(jpgFolder, FileAttributes.ReadOnly)
                                Catch ex As Exception
                                    msAppendWarnings(ex.Message + " while attempting to set Read-Only attribute to Folder.jpg")
                                End Try
                            End If
                            ' if it wasnt there before then add to list
                            If booWasThere = False Then
                                mListFoldersNoFolderJpg.Add(jpgFolder)
                            End If

                        End If

                        '***********************
                        '* Artwork.jpg
                        '***********************
                        Dim jpgArtwork As String = IO.Path.GetDirectoryName(track.Location) + "\Artwork"
                        If My.Settings.DisableJpgArtwork = False Then
                            ' Additionally copy Artwork.jpg
                            bExported = bExported And fWriteArtwork(f.Tag.Pictures(0), jpgArtwork)
                        End If

                        '***********************
                        '* AlbumArtSmall.jpg
                        '***********************
                        If My.Settings.DisableJpgAlbumArtSmall = False Then
                            ' Additionally copy AlbumArtSmall.jpg
                            Dim jpgAlbumArtSmall As String = IO.Path.GetDirectoryName(track.Location) + "\AlbumArtSmall"
                            bExported = bExported And fWriteArtwork(f.Tag.Pictures(0), jpgAlbumArtSmall)
                            'End If
                        End If

                        '*******************************************
                        '* %AlbumArtist% - (%Year%) %Album%.jpg etc.
                        '*******************************************
                        Dim jpgCustom As String = IO.Path.GetDirectoryName(track.Location) + "\" + fGetFileNameFromPattern(My.Settings.ArtworkFileNameEx, track)
                        If jpgArtwork <> jpgCustom AndAlso jpgFolder <> jpgCustom Then
                            bExported = bExported And fWriteArtwork(f.Tag.Pictures(0), jpgCustom)
                        End If

                    End If

                End If

            End If

        Next

        Return False

    End Function

    Public Function ValidateDisc(ByVal lDisc As cXmlDisc) As Boolean

        Dim success As Boolean = True

        bwApp.ReportProgress(ProgressType.UPDATE_TRACKS_PROGRESS_BAR_MAX, lDisc.Tracks.Count)

        Dim xbf As New cXmlDiscArtistFinder(lDisc)
        lDisc.Artist = xbf.AlbumArtist

        For Each xt As cXmlTrack In lDisc.Tracks

            '***************************
            ' COPY ARTIST TO ALBUMARTIST 
            '***************************
            If My.Settings.LibraryCopyAlbumArtist Then
                sTracksAlbumArtist(xt, lDisc.Artist)
            End If

            ''*********************************
            ''* Import Missing Lyrics
            ''*********************************
            If My.Settings.LyricsImport Then
                success = success And fImportLyrics(xt)
            End If

            ''**********************************
            ''* Check for iTunes Store Standard
            ''**********************************

            If My.Settings.CheckStandard = True Then
                success = success And fCheckStandard(xt)
            End If

            ''*********************************
            ''* Export Lyrics
            ''*********************************
            If My.Settings.ExportLyrics = True Then
                success = success And fExportLyrics(xt)
            End If

            ''**********************************
            ''* Finally Report Track Progress
            ''**********************************
            bwApp.ReportProgress(ProgressType.INCREMENT_TRACK_PROGRESS)

        Next

        ''*********************************
        ''* Fill Track Count Etc.
        ''*********************************
        If My.Settings.FillTrackCountEtc = True Then
            success = success And fFillTrackCountEtc(lDisc)
        End If

        ''*********************************
        ''* Export Artwork
        ''*********************************
        If My.Settings.LibraryExportArtwork = True Then
            success = success And fExportArtwork(lDisc)
        End If

        ''*********************************
        ''* Export Index    
        ''*********************************
        If My.Settings.ExportIndex = True Then
            success = success And fExportIndex(lDisc)
        End If

        bwApp.ReportProgress(ProgressType.READY, "Validated " + lDisc.Name)

        Return success

    End Function

    Private Function fExportLyrics(ByVal xt As cXmlTrack) As Boolean

        If xt IsNot Nothing AndAlso xt.Location IsNot Nothing Then

            Dim track As TagLib.File = TagLib.File.Create(xt.Location)

            Dim lyrics As String = track.Tag.Lyrics
            If lyrics = String.Empty Then
                Dim lws As UploadersLib.OtherServices.Lyrics = mfGetLyrics(xt)
                lyrics = lws.Text
                If lyrics <> String.Empty Then
                    bwApp.ReportProgress(ProgressType.FOUND_LYRICS_FOR, Chr(34) + xt.Name + Chr(34))
                End If
            End If

            If lyrics <> String.Empty Then

                Try
                    Dim lyricsFolder As String = String.Empty
                    Dim lyricsNameDir As String = String.Empty
                    Dim lyricsPath As String = String.Empty

                    If My.Settings.LyricsToAlbumFolder = True Then
                        lyricsFolder = Path.GetDirectoryName(xt.Location)
                    Else
                        lyricsFolder = My.Settings.LyricsFolderPathEx
                    End If

                    ' this can have sub folders with name
                    lyricsNameDir = fGetFileNameFromPattern(My.Settings.LyricsFilenamePatternEx, xt) + My.Settings.LyricsFileExtEx
                    Dim lyricsDir As String = mfGetLegalTextForDirectory(Path.GetDirectoryName(lyricsNameDir))
                    Dim lyricsName As String = Path.GetFileName(lyricsNameDir)


                    lyricsPath = Path.Combine(lyricsFolder, Path.Combine(lyricsDir, lyricsName))

                    Dim dir As String = Path.GetDirectoryName(lyricsPath)

                    If Directory.Exists(dir) = False Then
                        Directory.CreateDirectory(dir)
                    End If

                    'TODO: Change extension from LRC to TXT if no timestamp

                    Using sw As New StreamWriter(lyricsPath, False)
                        sw.WriteLine(mfGetFixedLyrics(lyrics))
                        msAppendDebug(String.Format("Exported Lyrics as {0}", lyricsPath))
                    End Using

                Catch ex As Exception
                    msAppendWarnings(ex.Message + " while exporting Lyrics")
                End Try

            End If

        End If

    End Function

    Private Function fExportIndex(ByVal lDisc As cXmlDisc) As Boolean

        If lDisc IsNot Nothing Then
            Return mfExportIndex(lDisc)
        End If

    End Function

    Private Function fCheckStandard(ByVal xt As cXmlTrack) As Boolean

        Dim success As Boolean = True

        Dim filePath As String = xt.Location
        If File.Exists(filePath) Then
            success = success And fCheckFileStandard(filePath)
        End If

        Return success

    End Function


    Private Function fCheckFileStandard(ByVal filePath As String) As Boolean

        Dim xt As cXmlTrack = New cXmlTrack(filePath, False)

        Dim success As Boolean = fIsItunesStoreStandard(xt)

        If success = False Then
            mListTracksNonITSstandard.Add(xt.Location + xt.Notes)
            'Console.Writeline(xt.Location + xt.Notes)
        End If

        Return success

    End Function

    Private Function fImportLyrics(ByVal xt As cXmlTrack) As Boolean

        Dim success As Boolean = False

        Dim filePath As String = xt.Location

        If File.Exists(filePath) Then

            Try
                '' Create TabLib reference
                Dim track As TagLib.File = TagLib.File.Create(filePath)

                If track.Tag.Lyrics Is Nothing Then

                    Dim artist As String = fGetAlbumArtist(track)
                    Dim song As String = mfGetNameToSearch(track)
                    Dim lws As Lyrics = mfGetLyrics(xt)
                    Dim lyrics As String = lws.Text

                    If Not String.IsNullOrEmpty(lyrics) Then
                        track.Tag.Lyrics = lyrics
                        bwApp.ReportProgress(ProgressType.FOUND_LYRICS_FOR, Chr(34) + xt.Name + Chr(34))
                        TagLib.Id3v2.Tag.DefaultVersion = 3
                        TagLib.Id3v2.Tag.ForceDefaultVersion = True
                        track.Save()
                        success = True
                    End If

                End If

            Catch ex As Exception
                msAppendWarnings(ex.Message + " while adding lyrics using File Validator")
            End Try

        End If

        Return success

    End Function

    Private Sub sTracksAlbumArtist(ByVal xt As cXmlTrack, ByVal AlbumArtist As String)

        Dim countMissingAlbumArtist As Integer = 0
        Dim trackLoc As String = "dead track"

        Dim track As TagLib.File = TagLib.File.Create(xt.Location)
        TagLib.Id3v2.Tag.DefaultVersion = 3
        TagLib.Id3v2.Tag.ForceDefaultVersion = True

        Try
            trackLoc = xt.Location
        Catch ex As Exception
            ' oh well
        End Try

        xt.AlbumArtist = track.Tag.FirstAlbumArtist

        ' can have The track is not modifiable. errors so need try/catch
        If xt.AlbumArtist Is Nothing Or My.Settings.OverwriteAlbumArtist = True Then

            Try
                xt.AlbumArtist = AlbumArtist
            Catch ex As Exception
                msAppendWarnings(ex.Message & " while filling AlbumArtist for " & trackLoc)
                msAppendWarnings(ex.StackTrace)
            End Try

        Else

            If xt.Compilation = True Then
                ' dont need to overwrite all the time
                If xt.AlbumArtist <> VARIOUS_ARTISTS Then
                    Try
                        xt.AlbumArtist = VARIOUS_ARTISTS
                    Catch ex As Exception
                        msAppendWarnings(ex.Message & " while setting track as Compilation for " & trackLoc)
                        msAppendWarnings(ex.StackTrace)
                    End Try
                End If
            ElseIf xt.AlbumArtist.Equals("Various") Or xt.AlbumArtist.Equals("VA") Then
                Try
                    xt.AlbumArtist = VARIOUS_ARTISTS
                Catch ex As Exception
                    msAppendWarnings(ex.Message & " while filling AlbumArtist for " & trackLoc)
                End Try
            End If

        End If

        If File.Exists(xt.Location) Then

            Try
                track.Tag.AlbumArtists = New String() {xt.AlbumArtist}
                track.Save()
            Catch ex As Exception
                msAppendWarnings(ex.Message & " while filling AlbumArtist for " & trackLoc)
            End Try

        End If

    End Sub

    Private Function fFillTrackCountEtc(ByVal lDisc As cXmlDisc) As Boolean

        Dim lAlbumTitle As String = lDisc.Name

        Dim lAlbumIsComplete As Boolean = lDisc.IsComplete

        Dim lTracksCount As UInteger = CUInt(lDisc.Tracks.Count)
        Dim lTrackCountMax As UInteger = CUInt(lDisc.Tracks.Count)
        For Each track As cXmlTrack In lDisc.Tracks
            lTrackCountMax = CUInt(Math.Max(lTracksCount, track.TrackNumber))
            lTrackCountMax = CUInt(Math.Max(lTracksCount, track.TrackCount))
        Next
        lDisc.HighestTrackNumber = lTrackCountMax

        bwApp.ReportProgress(ProgressType.UPDATE_TRACKS_PROGRESS_BAR_MAX, lDisc.Tracks.Count)

        For Each xt As cXmlTrack In lDisc.Tracks

            If bwApp.CancellationPending Then
                Exit Function
            End If

            If File.Exists(xt.Location) Then

                Dim track As TagLib.File = TagLib.File.Create(xt.Location)
                TagLib.Id3v2.Tag.DefaultVersion = 3
                TagLib.Id3v2.Tag.ForceDefaultVersion = True

                Dim booTrackEdited As Boolean = False

                If Not track.Tag.TrackCount > 0 Or (lAlbumIsComplete = False) Or _
     track.Tag.TrackCount < lTrackCountMax Then
                    track.Tag.TrackCount = CUInt(lTrackCountMax)
                    booTrackEdited = True
                ElseIf mDiscHasConsecutiveTracks(lDisc) AndAlso track.Tag.TrackCount <> lTracksCount Then
                    Dim lTrackNumber As UInteger = Math.Max(lTracksCount - (track.Tag.TrackCount - track.Tag.Track), track.Tag.Track)
                    If track.Tag.Track > lTracksCount Then
                        track.Tag.Track = CType(lTrackNumber, UInteger)
                    End If
                    If lAlbumIsComplete = False Then
                        track.Tag.TrackCount = lTracksCount
                    End If
                    booTrackEdited = True
                End If

                If (My.Settings.DisableGroupedDiscCountUpdate = True AndAlso xt.Grouping = "") AndAlso lAlbumIsComplete = True Then

                    Dim titleAlbum As String = mLibraryTasks.fGetAlbumName(track)
                    For Each lArtistKey As String In mArtistKeys
                        Dim lArtist As cXmlAlbumArtist = CType(AlbumArtists(lArtistKey), cXmlAlbumArtist)
                        If lArtist.AlbumKeys.Contains(titleAlbum) Then
                            Dim lAlbum As cXmlAlbum = CType(lArtist.Albums(titleAlbum), cXmlAlbum)
                            Dim dscCount As Integer = lAlbum.Discs.Count
                            If track.Tag.DiscCount <> dscCount Then
                                track.Tag.DiscCount = CUInt(Math.Max(Math.Max(dscCount, track.Tag.Disc), track.Tag.DiscCount))
                                booTrackEdited = True
                            End If
                            Exit For
                        End If

                    Next

                End If

                If track.Tag.Disc = 0 Then
                    track.Tag.Disc = 1
                    booTrackEdited = True
                End If

                If booTrackEdited Then
                    bwApp.ReportProgress(ProgressType.WRITING_TAGS_TO_TRACKS, track.Name)
                    mListTracksCountUpdated.Add(xt.Location)
                End If

                If Not lAlbumIsComplete Then
                    If Not mListAlbumsInconsecutiveTracks.Contains(lAlbumTitle) Then
                        mListAlbumsInconsecutiveTracks.Add(lAlbumTitle)
                    End If
                End If

                If booTrackEdited Then
                    track.Save()
                End If

            End If

        Next


    End Function

    Public Function fGetArtist(ByVal lArtistKey As String) As cXmlAlbumArtist

        If mArtistKeys.Contains(lArtistKey) Then
            Return CType(AlbumArtists(lArtistKey), cXmlAlbumArtist)
        End If

        Return Nothing

    End Function

    Public Function fGetAlbum(ByVal lAlbumKey As String) As cXmlAlbum

        For Each lArtistKey As String In AlbumArtistKeys

            Dim lArtist As cXmlAlbumArtist = CType(AlbumArtists(lArtistKey), cXmlAlbumArtist)

            If lArtist.AlbumKeys.Contains(lAlbumKey) Then
                Return CType(lArtist.Albums(lAlbumKey), cXmlAlbum)
            End If

        Next

        Return Nothing

    End Function

    Public Function fGetDisc(ByVal lDiscKey As String) As cXmlDisc

        For Each lArtistKey As String In AlbumArtistKeys
            Dim lArtist As cXmlAlbumArtist = CType(AlbumArtists(lArtistKey), cXmlAlbumArtist)
            For Each lAlbumKey As String In lArtist.AlbumKeys
                Dim lAlbum As cXmlAlbum = CType(lArtist.Albums(lAlbumKey), cXmlAlbum)
                If lAlbum.DiscKeys.Contains(lDiscKey) Then
                    Return CType(lAlbum.Discs(lDiscKey), cXmlDisc)
                End If
            Next
        Next

        Return Nothing

    End Function

    Public Sub sLoadTrackToArtist(ByVal track As cXmlTrack)

        Dim lArtist As New cXmlAlbumArtist(track.AlbumArtist)

        If mArtistKeys.Contains(track.AlbumArtist) = False Then
            mArtistKeys.Add(track.AlbumArtist)
        End If

        If mAlbumArtists.ContainsKey(track.AlbumArtist) Then
            ' we already have the artist 
            lArtist = CType(mAlbumArtists(track.AlbumArtist), cXmlAlbumArtist)
        Else
            mAlbumArtists.Add(track.AlbumArtist, lArtist)
        End If

        Dim lDisc As New cXmlDisc(track)

        '* Add a Disc Key to Master List
        If mDiscKeys.Contains(lDisc.Name) = False Then
            mDiscKeys.Add(lDisc.Name)
        End If

        Dim lAlbum As New cXmlAlbum(lDisc.AlbumName)

        ' if album exists uses that 
        If lArtist.Albums.ContainsKey(lDisc.AlbumName) Then
            lAlbum = CType(lArtist.Albums(lAlbum.Name), cXmlAlbum)
        Else
            lArtist.AddAlbum(lAlbum.Name, lAlbum)
        End If


        If lAlbum.Discs.ContainsKey(lDisc.Name) Then
            lDisc = CType(lAlbum.Discs(lDisc.Name), cXmlDisc)
        Else
            lAlbum.AddDisc(lDisc.Name, lDisc)
        End If

        lDisc.AddTrack(track)

    End Sub

    Private Sub sLoadDiscToAlbumsTable(ByVal disc As cXmlDisc)

        Dim lAlbum As New cXmlAlbum(disc.AlbumName)

        Dim albumTitle As String = disc.AlbumName

        ' update album titles
        If Not mListAlbumKeys.Contains(albumTitle) Then
            mListAlbumKeys.Add(albumTitle)
        End If

        ' check if album already exists
        If (mTableAlbums.ContainsKey(albumTitle)) Then
            lAlbum = CType(mTableAlbums.Item(albumTitle), cXmlAlbum)
        Else
            mTableAlbums.Add(albumTitle, lAlbum)
        End If

        If Not (lAlbum.HasDisc(disc)) Then
            lAlbum.Discs.Add(disc.AlbumName, disc)
        End If

    End Sub

    Public Function sTagBlankAlbum(ByVal track As TagLib.File) As Boolean

        Dim success As Boolean = False
        Try
            If My.Settings.TagUnknownAlbum Then
                track.Tag.Album = UNKNOWN_ALBUM
            Else
                track.Tag.Album = mfGetNameToSearch(track) + " " + My.Settings.SingleDiscSuffix
            End If
        Catch ex As Exception
            msAppendWarnings(ex.Message + " tagging blank album")
        End Try

    End Function

    Public Function sTagBlankAlbum(ByVal track As cXmlTrack) As Boolean

        Dim success As Boolean = False
        Try
            If My.Settings.TagUnknownAlbum Then
                track.Album = UNKNOWN_ALBUM
            Else
                track.Album = mfGetNameToSearch(track) + " " + My.Settings.SingleDiscSuffix
            End If
        Catch ex As Exception
            msAppendWarnings(ex.Message + " tagging blank album")
        End Try

    End Function

    Private Sub sLoadTrackToDiscsTable(ByVal track As cXmlTrack)

        Dim locTrack As String = "dead track"
        Try
            locTrack = track.Location  'track.Location
        Catch ex As Exception
        End Try

        Try

            'TODO: HOW TO SEE IF OK TO VALIDATE WITH XMLTRACK??
            ' If fOKtoValidate(track) Then

            Dim lDisc As New cXmlDisc(track)

            If track.Album <> String.Empty Then
                ' 5.6.1.2 DiscCount was wrong for album names shared by different artists [ffs]
                lDisc.AlbumName = mGetAlbumName(track)
                ' 5.7.4.5 Tagging "Unknown Album" to songs where the album tag is blank is now optional [Bluenote]
            Else
                If My.Settings.TagBlankAlbum Then
                    sTagBlankAlbum(track)
                End If
            End If

            If lDisc.Name <> String.Empty Then

                Dim discTitle As String = lDisc.Name

                If Not mDiscKeys.Contains(discTitle) Then
                    mDiscKeys.Add(discTitle)
                End If

                ' check if album already exists
                If (mTableDiscs.ContainsKey(discTitle)) Then
                    ' album is in the album list 
                    lDisc = CType(mTableDiscs.Item(discTitle), cXmlDisc)
                Else
                    ' create new album and add to album list  
                    mTableDiscs.Add(discTitle, lDisc)
                    bwApp.ReportProgress(ProgressType.READ_TRACKS_FROM_DISCS, mGetDiscName(track))
                End If

                ' check if track already is there 
                If Not lDisc.Tracks.Contains(track) Then
                    lDisc.Tracks.Add(track)
                End If

            End If

            If lDisc.Genre = String.Empty Then
                lDisc.Genre = track.Genre
            End If

            If lDisc.Year = 0 Then
                lDisc.Year = CUInt(track.Year)
            End If

            ' load the disc to album
            sLoadDiscToAlbumsTable(lDisc)

            '  End If

        Catch ex As Exception

            msAppendDebug(String.Format("Error occured while reading {0}, {1}", locTrack, ex.Message))

        End Try

    End Sub


End Class
