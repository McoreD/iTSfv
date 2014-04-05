Imports System.ComponentModel
Imports iTSfv.cBwJob
Imports iTunesLib
Imports System.IO

Public Module mJobsIT

    '' RESPONSIBLE FOR FUTURE JOBS FOR ITUNES 

    Private Sub sPausePendingCheck(ByVal bwApp As BackgroundWorker)

        '' 5.34.14.1 Pressing Stop button did not pause the currently active job [Jojo]

        If mJobPaused And bwApp.CancellationPending = False Then
            Threading.Thread.Sleep(2000)
            Call sPausePendingCheck(bwApp)
        End If

    End Sub

    Private Sub sBwAppDeleteMissingTracks(ByVal lMainLibraryTracks As IITTrackCollection, _
                                           ByVal bwApp As BackgroundWorker, _
                                           ByVal chkResume As Boolean, _
                                           ByVal fGetLastCheckTrackID As Integer, _
                                           ByVal chkDeleteTracksNotInHDD As Boolean, _
                                           ByVal chkDeleteNonMusicFolderTracks As Boolean)

        msAppendDebug("Looking for tracks outside of music folders to remove")

        If chkResume Then
            bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, lMainLibraryTracks.Count - fGetLastCheckTrackID + 1)
        Else
            bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, lMainLibraryTracks.Count)
        End If

        If chkDeleteTracksNotInHDD = True OrElse chkDeleteNonMusicFolderTracks = True Then

            For i As Integer = lMainLibraryTracks.Count To fGetLastCheckTrackID Step -1

                If bwApp.CancellationPending Then
                    bwApp.ReportProgress(ProgressType.READY)
                    Exit Sub
                End If

                Dim track As IITTrack = lMainLibraryTracks.Item(i)

                If track.Kind = ITTrackKind.ITTrackKindFile Then

                    Dim booTrackDeleted As Boolean = False

                    If chkDeleteNonMusicFolderTracks = True Then
                        bwApp.ReportProgress(ProgressType.DELETE_TRACKS_DEAD_ALIEN, track.Album)
                    Else
                        bwApp.ReportProgress(ProgressType.DELETE_TRACKS_DEAD, track.Album)
                    End If

                    If chkDeleteNonMusicFolderTracks = True Then
                        '**************************
                        ' DELETE NON-MUSIC-FOLDER
                        '**************************
                        booTrackDeleted = mfDeleteNonMusicFolderTrack(CType(track, IITFileOrCDTrack))
                    End If

                    If chkDeleteTracksNotInHDD = True Then
                        '**************************
                        ' DELETE NON-EXISTANT FILES
                        '**************************
                        If booTrackDeleted = False Then
                            booTrackDeleted = booTrackDeleted And mfDeleteTrackNotInHDD(CType(track, IITFileOrCDTrack))
                        End If
                    End If

                End If

            Next

            bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, lMainLibraryTracks.Count)

        End If

        Dim msg As String = String.Format("Removed {0} dead and {1} foreign tracks", mListFileNotFound.Count, mListTracksNonMusicFolder.Count)
        msAppendDebug(msg)
        bwApp.ReportProgress(ProgressType.READY, msg)

    End Sub

    Public Function mfDeleteNonMusicFolderTrack(ByVal track As IITFileOrCDTrack) As Boolean

        Try
            If String.IsNullOrEmpty(track.Location) = False AndAlso mfFileIsInMusicFolder(track.Location) = False Then
                mListTracksNonMusicFolder.Add(track.Location)
                msAppendDebug("Removing foreign track: " + track.Location)
                track.Delete()
                Return True
            End If
        Catch ex As Exception
            msAppendWarnings(ex.Message + " while removing foreign tracks.")
        End Try

        Return False

    End Function

    Public Function mfDeleteTrackNotInHDD(ByVal track As iTunesLib.IITFileOrCDTrack) As Boolean

        Try

            If Not IO.File.Exists(track.Location) Then

                Dim tr As New cXmlTrack(track, False)
                mListTracksToDelete.Add(tr)
                Dim tInfo As String = track.Artist + " - " + track.Album + " - " + track.Name
                msAppendDebug("Removed dead track: " + tInfo)
                mListFileNotFound.Add(tInfo)
                track.Delete()
                Return True

            End If

        Catch ex As Exception
            msAppendWarnings(ex.Message + " while deleting dead tracks")
        End Try

        Return False

    End Function

    Private Sub sExportArtwork(ByVal song As IITFileOrCDTrack, ByVal dirPath As String)

        If song.Artwork.Count > 0 Then

            Dim artSrc As New cArtworkSource(song, bLoadArtwork:=True)

            mfUpdateStatusBarText("Found Artwork from iTunes Store...", True)

            Dim jpgCustom As String = My.Settings.FolderPathExArtwork + Path.DirectorySeparatorChar + mfGetFileNameFromPattern(My.Settings.ArtworkFileNamePatternEx, song)
            If jpgCustom.EndsWith(".jpg") = False Then
                jpgCustom = String.Concat(jpgCustom, ".jpg")
            End If

            If String.Empty <> dirPath Then

                If My.Settings.StoreArtworkChoose Then

                    Dim listArtSrc As New List(Of cArtworkSource)

                    Dim listArtwork As New List(Of String)
                    For Each ext As String In mFileExtArtwork
                        listArtwork.AddRange(Directory.GetFiles(dirPath, "*." + ext, SearchOption.AllDirectories))
                    Next
                    For Each art As String In listArtwork
                        listArtSrc.Add(New cArtworkSource(art))
                    Next

                    If artSrc.ArtworkPath <> String.Empty Then
                        listArtSrc.Add(artSrc)
                    End If

                    If listArtSrc.Count > 1 Then
                        Dim dlg As New dlgArtworkList(listArtSrc)
                        dlg.ShowDialog()
                        dlg.Focus()
                        If dlg.DialogResult = DialogResult.OK Then
                            artSrc = dlg.Artwork
                        Else
                            artSrc = Nothing ' dont save artwork
                        End If
                    End If

                End If

                If artSrc IsNot Nothing Then

                    If My.Settings.DefaultExArtworkFolder = True Then
                        Dim fileName As String = mfGetFileNameFromPattern(My.Settings.ArtworkFileNamePatternEx, song)
                        If Directory.Exists(dirPath) = True Then
                            Dim destPath As String = Path.Combine(dirPath, fileName)
                            mfSaveArtworkSafely(destPath, artSrc)
                        End If
                    Else
                        mfSaveArtworkSafely(jpgCustom, artSrc)
                    End If

                End If

            Else

                Dim dlg As New SaveFileDialog
                dlg.Title = "Browse where you want to save the iTunes Store track Artwork..."
                dlg.Filter = "JPG files (*.jpg)|*.jpg"
                dlg.FileName = Path.GetFileName(jpgCustom)
                dlg.DefaultExt = ".jpg"

                If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
                    mfSaveArtworkSafely(dlg.FileName, artSrc)
                End If

            End If

        End If

    End Sub

    Public Function mfExportArtworkIT(ByVal song As IITFileOrCDTrack, ByVal dirPath As String) As Boolean

        Dim succ As Boolean = True
        Dim songLoc As String = "Unknown Track Location"

        Try
            If song IsNot Nothing Then

                songLoc = song.Location
                Dim songArtist As String = song.Artist

                succ = song.Artwork.Count > 0

                If succ Then
                    sExportArtwork(song, dirPath)
                End If

                If IO.File.Exists(songLoc) Then
                    My.Computer.FileSystem.DeleteFile(songLoc)
                End If

                song.Delete()

                If succ = False Then
                    mfUpdateStatusBarText("Could not find Artwork from iTunes Store...", True)
                End If

            End If

            ' - type artist, album, first track title and press search 
            '- search copies a dummy mp3 from program dir to temp, adds the tags to file, adds teh file to itunes 
            '- if job.tracks.count > 0 then if  track.artwork.count > 0 then prompt where to save the artwork or auto save to Artwork folder
            '- delete the track from itunes, delete track from temp
        Catch ex As Exception
            msAppendWarnings(ex.Message + " while exporting iTunes Store Artwork for " + songLoc)
        End Try

        Return succ


    End Function

    Public Sub msBwAppExportPOPM(ByVal bwApp As BackgroundWorker, ByVal mMainLibraryTracks As IITTrackCollection)

        mfUpdateStatusBarText("Writing PlayedCount and Rating information to POPM and PCNT Frames...", False)

        mProgressDiscsMax = mMainLibraryTracks.Count

        For i As Integer = 1 To mMainLibraryTracks.Count

            Dim track As IITTrack = mMainLibraryTracks.Item(i)

            If track.Kind = ITTrackKind.ITTrackKindFile Then

                sPausePendingCheck(bwApp)
                If bwApp.CancellationPending = True Then
                    Exit For
                End If

                Dim song As IITFileOrCDTrack = CType(track, IITFileOrCDTrack)
                Dim te As New cTrackEditor()

                If te.UseFile(song.Location) Then

                    bwApp.ReportProgress(ProgressType.UPDATING_TRACK, String.Format("""{0}"" - ""{1}""", song.Artist, song.Name))

                    Dim bEdit As Boolean = song.PlayedCount > 0 Or song.Rating > 0

                    If bEdit Then
                        te.SetFramePCNT(song.PlayedCount)
                        te.SetFramePOPM(song.PlayedCount, song.Rating)
                        te.Save()
                    End If

                End If

                mProgressDiscsCurrent = i

            End If

        Next

        bwApp.ReportProgress(ProgressType.READY)

    End Sub

    Public Sub msBwAppImportPOPM(ByVal bwApp As BackgroundWorker, ByVal mMainLibraryTracks As IITTrackCollection)

        mfUpdateStatusBarText("Importing PlayedCount and Rating information from POPM and PCNT Frames...", False)

        mProgressDiscsMax = mMainLibraryTracks.Count

        For i As Integer = 1 To mMainLibraryTracks.Count

            Dim track As IITTrack = mMainLibraryTracks.Item(i)

            If track.Kind = ITTrackKind.ITTrackKindFile Then

                sPausePendingCheck(bwApp)
                If bwApp.CancellationPending = True Then
                    Exit For
                End If

                Dim song As IITFileOrCDTrack = CType(track, IITFileOrCDTrack)
                Dim te As New cTrackEditor()

                If te.UseFile(song.Location) Then

                    bwApp.ReportProgress(ProgressType.UPDATING_TRACK, String.Format("""{0}"" - ""{1}""", song.Artist, song.Name))

                    Dim popm As FramePOPM = mTagLibJobs.mfGetPOPM(song.Location)

                    ''*********************
                    ''* Pre-Edit Steps
                    ''*********************
                    Dim fiTrack As New FileInfo(song.Location)
                    Dim wasReadOnly As Boolean = fiTrack.IsReadOnly
                    Dim dMod As Date
                    If My.Settings.ModifiedDateRetain = True Then
                        dMod = fiTrack.LastWriteTime
                    End If
                    If fiTrack.IsReadOnly Then
                        fiTrack.IsReadOnly = False
                    End If

                    ''*********************
                    ''* Editing PlayedCount
                    ''*********************
                    If My.Settings.PlayedCountUpdateOnlyIfHigher = False Then

                        If My.Settings.PlayedCountAccumilate = True Then
                            song.PlayedCount += popm.PlayedCount
                        Else
                            song.PlayedCount = popm.PlayedCount
                        End If

                    Else

                        If song.PlayedCount < popm.PlayedCount Then

                            If My.Settings.PlayedCountAccumilate = True Then
                                song.PlayedCount += popm.PlayedCount
                            Else
                                song.PlayedCount = popm.PlayedCount
                            End If

                        End If

                    End If

                    ''*********************
                    ''* Editing Rating
                    ''*********************
                    If popm.Rating > track.Rating Or My.Settings.RatingUpdateOnlyIfHigher = False Then
                        If track.Rating <> popm.Rating Then
                            track.Rating = popm.Rating
                        End If
                    End If

                    ''*********************
                    ''* Post-Edit Steps
                    ''*********************
                    If My.Settings.ModifiedDateRetain = True Then
                        File.SetLastWriteTime(fiTrack.FullName, dMod)
                    End If
                    If wasReadOnly Then
                        fiTrack.IsReadOnly = True
                    End If

                End If

                mProgressDiscsCurrent = i

            End If

        Next

        bwApp.ReportProgress(ProgressType.READY)

    End Sub



End Module
