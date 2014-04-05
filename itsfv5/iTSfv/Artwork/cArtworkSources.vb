Imports System.IO
Imports iTunesLib

Public Class cArtworkSources

    Private mArtworkSoures As New List(Of cArtworkSource)
    Private mArtworkSourceBestForFile As cArtworkSource = Nothing  ' to be saved as folder.jpg etc


    Public Sub New(ByVal lDisc As cInfoDisc)

        ' once a disc is imported to artwork sources
        ' try to get itms artwork 

        If My.Settings.ArtworkSrcITMS Then
            fGetArtworkSourceFromWeb(lDisc)
        End If
        If My.Settings.ArtworkSrcItunes Then
            fGetArtworkSourceFromStore(lDisc)
        End If
        If My.Settings.ArtworkSrcCache Then
            fGetArtworkSourceFromCache(lDisc)
        End If
        If My.Settings.ArtworkSrcFolder Then
            fGetArtworkSourceFromFolder(lDisc)
        End If
        If My.Settings.ArtworkSrcTrack Then
            fGetArtworkSourceFromTrack(lDisc)
        End If
        If My.Settings.ArtworkSrcAAD Then
            fGetArtworkSourceFromAAD(lDisc)
        End If

        If My.Settings.ArtworkSrcAADCLI Then
            If (My.Settings.AADCLIOnlyIfNoArtwork = True AndAlso lDisc.FirstTrack.Artwork.Count = 0) Or _
            My.Settings.AADCLIOnlyIfNoArtwork = False Then
                fGetArtworkSourceFromAADConsole(lDisc)
            End If
        End If

        msAppendDebug(String.Format("Found {0} sources of Artwork", mArtworkSoures.Count))

        If mArtworkSoures.Count > 0 Then

            For Each src As cArtworkSource In mArtworkSoures
                Dim srcArtworkLoc As String = If(src.ArtworkPath <> String.Empty, src.ArtworkPath, "Memory")
                msAppendDebug(String.Format("Artwork from {0} as {1} with {2}x{3}", src.ArtworkType.ToString, src.ArtworkPath, src.Width, src.Height))
            Next

            fBestArtworkSourceForFileSet()
            msAppendDebug(String.Format("Choosing {0} as the best Artwork", If(mArtworkSourceBestForFile Is Nothing, "nothing", mArtworkSourceBestForFile.ArtworkPath)))

        End If

    End Sub

    Private Function fBestArtworkSourceForFileSet() As cArtworkSource

        If mArtworkSoures.Count > 0 Then

            If mArtworkSoures.Count > 1 AndAlso My.Settings.ArtworkChooseManual = True Then

                msAppendDebug("Choosing Artwork manually...")
                mfUpdateStatusBarText("Waiting for Artwork to be chosen...", secondary:=True)

                Dim dlg As New dlgArtworkList(mArtworkSoures)
                dlg.ShowDialog()
                dlg.Focus()
                If dlg.DialogResult = DialogResult.OK Then
                    mArtworkSourceBestForFile = dlg.Artwork
                End If

            Else

                mArtworkSourceBestForFile = mArtworkSoures(0)

                For i As Integer = 0 To mArtworkSoures.Count - 2
                    If mArtworkSoures(i).Width * mArtworkSoures(i).Height < _
                    mArtworkSoures(i + 1).Width * mArtworkSoures(i + 1).Height Then
                        mArtworkSourceBestForFile = mArtworkSoures(i + 1)
                    End If
                Next

            End If

        End If

        Return mArtworkSourceBestForFile

    End Function

    Private Sub sAddArtworkSourceToList(ByVal src As cArtworkSource)

        Try
            If src IsNot Nothing AndAlso mArtworkSoures.Contains(src) = False Then
                If IO.File.Exists(src.ArtworkPath) Then
                    src.sLoadDimensions()
                    mArtworkSoures.Add(src)
                ElseIf src.ArtworkImage IsNot Nothing Then
                    src.Height = src.ArtworkImage.Height
                    src.Width = src.ArtworkImage.Width
                End If
            End If
        Catch ex As Exception
            msAppendWarnings(ex.Message + " while adding Artwork source to Artwork Sources")
        End Try

    End Sub

    Private Function fGetArtworkSourceFromStore(ByVal lDisc As cInfoDisc) As cArtworkSource

        Dim firstTrack As IITFileOrCDTrack = lDisc.Tracks.Item(0)
        Dim src As cArtworkSource = New cArtworkSource(firstTrack)

        If firstTrack.Artwork.Count = 0 Or _
        firstTrack.Artwork.Count > 0 AndAlso firstTrack.Artwork(1).IsDownloadedArtwork = False Then

            Try

                Dim ag As New cArtworkGrabberIT(firstTrack)
                Dim trackAgent As IITFileOrCDTrack = ag.fDownloadArtwork()
                If trackAgent IsNot Nothing Then
                    If trackAgent.Artwork.Count > 0 Then
                        'TEMP_STORE_ARTWORK_NAME
                        Dim trackArtworkPath As String = mfGetTempArtworkCachePath(TEMP_STORE_ARTWORK_NAME, firstTrack)
                        trackAgent.Artwork(1).SaveArtworkToFile(trackArtworkPath)
                        src.ArtworkType = ArtworkSourceType.iTunes
                        src.ArtworkPath = trackArtworkPath
                        sAddArtworkSourceToList(src)
                    End If
                End If

                If File.Exists(trackAgent.Location) Then
                    File.Delete(trackAgent.Location)
                End If

                trackAgent.Delete()
            Catch ex As Exception
                msAppendWarnings(ex.Message + " while adding iTunes Artwork source to Artwork Sources")
            End Try

        End If

        Return src

    End Function

    Private Function fGetArtworkSourceFromWeb(ByVal lDisc As cInfoDisc) As cArtworkSource

        Dim firstTrack As IITFileOrCDTrack = lDisc.Tracks.Item(0)

        Dim src As cArtworkSource = New cArtworkSource(firstTrack)

        ' first check if user asks to download from iTMS
        Dim p As String = ffGetITMSArtworkPath(firstTrack)
        If p <> String.Empty Then
            src.ArtworkType = ArtworkSourceType.iTMS
            src.ArtworkPath = p
            sAddArtworkSourceToList(src)
        End If

        Return src

    End Function

    Private Function fGetArtworkSourceFromTrack(ByVal lDisc As cInfoDisc) As cArtworkSource

        Dim src As cArtworkSource = Nothing

        ' see if one of the tracks has itunes downloaded artwork if yes then return a filepath
        Try
            For Each track As IITFileOrCDTrack In lDisc.Tracks
                If track.Artwork.Count > 0 Then
                    If track.Artwork.Item(1).IsDownloadedArtwork = False Then
                        src = New cArtworkSource(track)
                        Dim trackArtworkPath As String = mfGetTempArtworkCachePath(TEMP_TRACK_ARTWORK_NAME, track)                        
                        track.Artwork(1).SaveArtworkToFile(trackArtworkPath)
                        src.ArtworkType = ArtworkSourceType.Track
                        src.ArtworkPath = trackArtworkPath
                        sAddArtworkSourceToList(src)
                        If Not (My.Settings.ArtworkChooseManual And My.Settings.TrackArtworkShowAll) Then
                            Exit For
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            '' 5.34.1.2 Added stability for Access is denied while attempting to save track Artwork [Julie]
            msAppendDebug(ex.Message + " while saving artwork from track")
        End Try

        Return src

    End Function

    Private Function fGetArtworkSourceFromCache(ByVal lDisc As cInfoDisc) As cArtworkSource

        Dim src As cArtworkSource = Nothing
        Dim success As Boolean = False

        ' secondly check if track has an itunes downloaded artwork
        For Each track As IITFileOrCDTrack In lDisc.Tracks
            Try
                If track.Artwork.Count > 0 AndAlso track.Artwork.Item(1).IsDownloadedArtwork = True Then
                    src = New cArtworkSource(track)
                    src.ArtworkType = ArtworkSourceType.iTunes
                    src.ArtworkPath = mfGetArtworkCachePath(track, "Artwork")
                    If mfSaveArtworkSafely(src.ArtworkPath, src) Then
                        success = True
                        msAppendDebug("Found non-embedded iTunes Artwork")
                        Exit For
                    End If                    
                End If
            Catch ex As Exception
                msAppendWarnings(ex.Message)
            End Try
        Next ' looping to get the first track that has embedded

        sAddArtworkSourceToList(src)

        Return src

    End Function

    Private Function fGetArtworkSourceFromAADConsole(ByVal ldisc As cInfoDisc) As cArtworkSource

        Dim firstTrack As IITFileOrCDTrack = ldisc.Tracks(0)
        Dim src As New cArtworkSource(firstTrack)
        Dim pathArtwork As String = mfGetArtworkCachePath(firstTrack, "AAD")
        Dim success As Boolean = False

        If File.Exists(pathArtwork) Then

            success = True

        ElseIf File.Exists(My.Settings.ExePathAADConsole) Then

            Dim p As New Process
            Dim psi As New ProcessStartInfo(My.Settings.ExePathAADConsole)

            psi.WindowStyle = ProcessWindowStyle.Minimized

            If Path.GetFileName(My.Settings.ExePathAADConsole).ToLower = "aad.exe" Then
                psi.Arguments = String.Format("/artist ""{0}"" /album ""{1}"" /minSize {2} /minAspect {3} /path ""{4}""", _
                                              mLibrary.fGetArtist(firstTrack), mLibrary.fGetAlbum(firstTrack), _
                                              My.Settings.LowResArtworkWidth, 0.9, pathArtwork)
            Else
                psi.Arguments = String.Format("/artist ""{0}"" /album ""{1}"" /minSize {2} /path ""{3}""", _
                              mLibrary.fGetArtist(firstTrack), mLibrary.fGetAlbum(firstTrack), _
                              My.Settings.LowResArtworkWidth, pathArtwork)
            End If

            p.StartInfo = psi
            p.Start()

            mfUpdateStatusBarText("Searching for Artwork using AAD...", True)
            p.WaitForExit()

            If File.Exists(pathArtwork) Then
                success = True
            End If

        End If

        If success Then
            src.ArtworkPath = pathArtwork
            src.ArtworkType = ArtworkSourceType.AAD
            sAddArtworkSourceToList(src)
        End If

        Return src

    End Function

    Private Function fGetArtworkSourceFromAAD(ByVal ldisc As cInfoDisc) As cArtworkSource

        Dim firstTrack As IITFileOrCDTrack = ldisc.Tracks(0)
        Dim src As cArtworkSource = New cArtworkSource(firstTrack)
        Dim success As Boolean = False

        Try
            src.ArtworkType = ArtworkSourceType.AAD

            Dim dirAAD As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Album Art" + Path.DirectorySeparatorChar + mfGetLegalTextForDirectory(firstTrack.AlbumArtist) + Path.DirectorySeparatorChar + mfGetLegalTextForDirectory(firstTrack.Album))

            If Directory.Exists(dirAAD) Then

                For Each pattern As String In My.Settings.ArtworkFileNames

                    pattern = mfGetFileNameFromPattern(pattern, firstTrack)

                    Dim artWorkPath As String = dirAAD + Path.DirectorySeparatorChar + pattern
                    If File.Exists(artWorkPath) Then
                        src.ArtworkPath = artWorkPath
                        success = True
                        Exit For
                    End If

                Next

            End If

            If success Then
                sAddArtworkSourceToList(src)
            End If

        Catch ex As Exception
            msAppendWarnings(ex.Message + " while getting artwork source from AAD")
        End Try

        Return src

    End Function

    Private Function fGetArtworkSourceFromFolder(ByVal ldisc As cInfoDisc) As cArtworkSource

        Dim firstTrack As IITFileOrCDTrack = ldisc.Tracks(0)
        Dim src As cArtworkSource = New cArtworkSource(firstTrack)
        Dim success As Boolean = False

        Try
            ' from here onwards source is a file
            src.ArtworkType = ArtworkSourceType.File

            ' next check for the user preferred filepath to import from 
            If My.Settings.DefaultImArtworkFolder = True Then
                ' if in default folder
                Dim albumDir As String = IO.Path.GetDirectoryName(firstTrack.Location)
                Dim artworkPath As String = albumDir + Path.DirectorySeparatorChar + mfGetFileNameFromPattern(My.Settings.ArtworkFileNameIm, firstTrack)

                Dim artworkFiles As New List(Of String)
                Try
                    artworkFiles.AddRange(Directory.GetFiles(albumDir, "*.jpg", SearchOption.TopDirectoryOnly))
                    artworkFiles.AddRange(Directory.GetFiles(albumDir, "*.png", SearchOption.TopDirectoryOnly))
                Catch ex As Exception
                    msAppendWarnings("000004")
                    msAppendWarnings(ex.Message & " for " & albumDir)
                    msAppendDebug(ex.Message & " for " & albumDir)
                End Try

                If IO.File.Exists(artworkPath) Then
                    src.ArtworkPath = artworkPath
                    success = True
                ElseIf artworkFiles.Count = 1 AndAlso My.Settings.ImportAnySingleArtwork = True Then
                    ' show the replace artwork wizard
                    If mCurrJobTypeMain = cBwJob.JobType.VALIDATE_TRACKS_SELECTED Then
                        ' only prompt when user validates selected
                        Dim f As New frmAddArtwork(artworkFiles(0), ldisc)
                        f.ShowDialog()
                    End If
                    src.ArtworkPath = artworkFiles(0)
                    success = True
                Else
                    For Each fileName As String In My.Settings.ArtworkFileNames
                        artworkPath = albumDir + "\" + mfGetFileNameFromPattern(fileName, firstTrack)
                        If IO.File.Exists(artworkPath) Then
                            src.ArtworkPath = artworkPath
                            success = True
                            Exit For
                        End If
                    Next
                End If

            Else ' not DefaultImArtworkFolder

                For Each pattern As String In My.Settings.FileNamePatterns
                    pattern = mfGetFileNameFromPattern(pattern, firstTrack)
                    Dim artWorkPath As String = My.Settings.FolderPathImArtwork + Path.DirectorySeparatorChar + pattern
                    If File.Exists(artWorkPath) Then
                        src.ArtworkPath = artWorkPath
                        success = True
                        Exit For
                    End If
                Next

            End If

            If success Then
                sAddArtworkSourceToList(src)
            End If


        Catch ex As Exception
            msAppendWarnings(ex.Message + " while getting artwork source from file")
        End Try

        Return src

    End Function

    Private Function ffGetITMSArtworkPath(ByVal track As iTunesLib.IITFileOrCDTrack) As String

        If mfCompareRes() = True Then
            Dim artist As String = mGetAlbumArtist(track)
            Dim album As String = fGetAlbumToSearch(track)
            Dim art As New cArtworkITMS(artist, album)
            'Dim art As cArtworkSearch = New cArtworkAmazon(artist, album)
            'bwApp.ReportProgress(TaskType.SEARCHING_ITMS_ARTWORK, "Searching iTMS Artwork and ")
            Return art.GetArtworkPath
        End If

        Return String.Empty

    End Function

    Public ReadOnly Property ArtworkSources() As List(Of cArtworkSource)
        Get
            Return mArtworkSoures
        End Get
    End Property

    Public ReadOnly Property BestArtworkSourceForFile() As cArtworkSource
        Get
            Return mArtworkSourceBestForFile
        End Get
    End Property

End Class
