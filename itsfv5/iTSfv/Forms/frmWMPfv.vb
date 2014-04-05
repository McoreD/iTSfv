Imports System.Collections
Imports iTSfv.cBwJob
Imports System.IO

Public Class frmWMPfv

    Private mCurrJob As cBwJob
    Private mCurrJobType As cBwJob.JobType = cBwJob.JobType.NEW_TASK

    Private Sub frmWMPfv_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        My.Forms.frmSplash.Close() '' close any hidden splash window
    End Sub

    Private Sub frmWMPfv_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.Text = mpAppName() + " Preview"
        SwitchToITSfvToolStripMenuItem.Enabled = mfGetItunes()

    End Sub


    Private Sub btnSync_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSync.Click

        If bwApp.IsBusy = False Then
            Dim t As New cBwJob(cBwJob.JobType.SYNCHROCLEAN)
            bwApp.RunWorkerAsync(t)
        End If

    End Sub

    Private Sub sBwAppSynchroclean()

        sBwAppDeleteMissingTracks()
        sBwAppFindNewTracksFromHDD()

        If My.Settings.UpdateUrlToCoverArt Then
            sBwAppRewriteUrlToCover()
        End If

    End Sub

    Private Sub bwApp_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bwApp.DoWork

        mCurrJob = CType(e.Argument, cBwJob)
        mCurrJobType = mCurrJob.Job

        msAppendDebug("Job Started: " & mCurrJobType.ToString)

        Select Case mCurrJobType

            Case JobType.DELETE_DEAD_FOREIGN_TRACKS
                sBwAppDeleteMissingTracks()

            Case JobType.FIND_NEW_TRACKS_FROM_HDD
                sBwAppFindNewTracksFromHDD()

            Case JobType.INITIALIZE_PLAYER
                sBwAppLoadPlayer()

            Case JobType.SYNC_MEDIA_CENTER_CACHE
                sBwAppRewriteUrlToCover()

            Case JobType.SYNCHROCLEAN
                sBwAppSynchroclean()

            Case JobType.VALIDATE_LIBRARY
                sBwAppValidateLibrary()

        End Select

    End Sub

    Private Sub sBwAppValidateLibrary()

        Dim jumpNum As Integer = 100
        Dim lLibraryPlaylist As WMPLib.IWMPPlaylist = CType(mPlayer, cPlayerWMP).MainLibraryTracks
        bwApp.ReportProgress(ProgressType.UPDATE_TRACKS_PROGRESS_BAR_MAX, lLibraryPlaylist.count / jumpNum)
        Dim jumpCheck As Integer = 0

        Dim mValidator As New cValidator(bwApp)

        For i As Integer = 0 To lLibraryPlaylist.count - 1

            jumpCheck += 1
            Dim track As New WMPTrack(CType(lLibraryPlaylist.Item(i), WMPLib.IWMPMedia3))

            If jumpCheck Mod jumpNum = 0 Then
                Dim msg As String = "other media..."
                If track.Album <> String.Empty And track.Artist <> String.Empty Then
                    msg = String.Format("{0} - {1}", track.Album, track.Artist)
                End If
                bwApp.ReportProgress(ProgressType.SCANNING_TRACK_IN_PLAYER_LIBRARY, msg)
            End If

            If track.Location <> String.Empty Then
                mValidator.sLoadTrackToArtist(New cXmlTrack(track, False))
            End If

            If bwApp.CancellationPending Then
                Exit Sub
            End If

        Next

        ''******************************************
        '** Step 3: Load Albums/Discs to List Box
        ''******************************************
        'bwApp.ReportProgress(TaskType.CLEAR_DISCS_LISTBOX)
        For Each lArtistKey As String In mValidator.AlbumArtistKeys
            Dim lArtist As cXmlAlbumArtist = CType(mValidator.AlbumArtists(lArtistKey), cXmlAlbumArtist)
            For Each lAlbumKey As String In lArtist.AlbumKeys
                Dim lAlbum As cXmlAlbum = CType(lArtist.Albums(lAlbumKey), cXmlAlbum)
                For Each lDiscKey As String In lAlbum.DiscKeys
                    Dim lDisc As cXmlDisc = CType(lAlbum.Discs(lDiscKey), cXmlDisc)
                    lDisc.IsComplete = mDiscIsComplete(lDisc)
                    bwApp.ReportProgress(ProgressType.ADD_DISC_TO_LISTBOX_DISCS, lDiscKey)
                Next
            Next
        Next

        mValidator.ValidateLibrary()

    End Sub

    Private Sub sBwAppLoadPlayer()

        Try
            bwApp.ReportProgress(ProgressType.INITIALIZE_PLAYER_LIBRARY_START)
            mPlayer = New cPlayerWMP
            msAppendDebug("WMPfv Version: " & Application.ProductVersion)
            msAppendDebug("Logs Directory: " & My.Settings.LogsDir)
            msAppendDebug("Artwork Directory: " & My.Settings.ArtworkDir)
            msAppendDebug("Temporary Directory: " & My.Settings.TempDir)
            msAppendDebug("WMP Rip folder: " & mpMusicFolderPath)
            For i As Integer = 0 To mpMusicFolderPaths.Count - 1
                msAppendDebug(String.Format("{0}'s Music Folder {1}: {2}", Environment.UserName, i + 1, mpMusicFolderPaths(i)))
            Next
            msAppendDebug("Number of Media in WMP: " & CType(mPlayer, cPlayerWMP).MainLibraryTracks.count)

            bwApp.ReportProgress(ProgressType.INITIALIZE_PLAYER_FINISH)

        Catch ex As Exception

        End Try



    End Sub


    Private Sub sBwAppDeleteMissingTracks()

        msWMP_DeleteMissingTracks(oMainLibraryTracks:=CType(mPlayer, cPlayerWMP).MainLibraryTracks, _
                                  oBwApp:=bwApp, bResume:=My.Settings.ResumeValidation, _
                                  intLastTrackID:=0, bDeleteTracksNotInHDD:=My.Settings.LibraryDeleteTracksNotInHDD, _
                                  bDeleteNonMusicFolderTracks:=My.Settings.LibraryDeleteNonMusicFolderTracks)


    End Sub


    Private Sub sBwAppFindNewTracksFromHDD()

        msAppendDebug("Finding new tracks in specified music locations...")

        ' find all tracks in music folders
        Dim lListTracksLocationHdd As New List(Of String)
        lListTracksLocationHdd = mfGetNewFilesFromHDD(bwApp, mFileExtAudioWMP)
        'Console.Writeline(lListTracksLocationHdd.Count)

        Dim lListTracksLocationsPlayer As New List(Of String)

        Dim jumpNum As Integer = 100
        Dim lLibraryPlaylist As WMPLib.IWMPPlaylist = CType(mPlayer, cPlayerWMP).MainLibraryTracks
        bwApp.ReportProgress(ProgressType.UPDATE_TRACKS_PROGRESS_BAR_MAX, lLibraryPlaylist.count / jumpNum)
        Dim jumpCheck As Integer = 0

        For i As Integer = 0 To lLibraryPlaylist.count - 1

            ''Console.Writeline(i.ToString)

            jumpCheck += 1
            Dim track As New WMPTrack(CType(lLibraryPlaylist.Item(i), WMPLib.IWMPMedia3))

            If jumpCheck Mod jumpNum = 0 Then
                Dim msg As String = "other media..."
                If track.Album <> String.Empty And track.Artist <> String.Empty Then
                    msg = String.Format("{0} - {1}", track.Album, track.Artist)
                End If
                bwApp.ReportProgress(ProgressType.SCANNING_TRACK_IN_PLAYER_LIBRARY, msg)
            End If

            If track.Location <> String.Empty Then
                lListTracksLocationsPlayer.Add(track.Location.ToLower)
            End If

            If bwApp.CancellationPending Then
                Exit Sub
            End If

        Next

        lListTracksLocationsPlayer.Sort()
        lListTracksLocationHdd.Sort()

        Dim filesNew As Integer = lListTracksLocationHdd.Count - lListTracksLocationsPlayer.Count


        '* FINALLY FIND NEW MUSIC

        Dim listAddableFiles As New List(Of String)

        For Each filePathFromHdd As String In lListTracksLocationHdd

            Dim result As Integer = lListTracksLocationsPlayer.BinarySearch(filePathFromHdd.ToLower)

            If result < 0 Then
                ' Track is not found then list it
                listAddableFiles.Add(filePathFromHdd)
                bwApp.ReportProgress(ProgressType.ADD_TRACKS_TO_LISTBOX_TRACKS, filePathFromHdd)
            End If

            If bwApp.CancellationPending Then
                Exit Sub
            End If

        Next

        msAppendDebug("Finding new tracks in specified music locations... Done.")

        If listAddableFiles.Count > 0 Then

            bwApp.ReportProgress(ProgressType.UPDATE_TRACKS_PROGRESS_BAR_MAX, listAddableFiles.Count)
            For Each f As String In listAddableFiles
                If IO.File.Exists(f) Then
                    bwApp.ReportProgress(ProgressType.ADD_TRACKS_TO_LIBRARY, Path.GetFileName(f))
                    CType(mPlayer, cPlayerWMP).AddMedia(f)
                    msAppendDebug("Added " & f)
                End If

                If bwApp.CancellationPending Then
                    Exit Sub
                End If

            Next
        End If

        bwApp.ReportProgress(ProgressType.READY)

    End Sub


    Private Sub btnFindNewTracks_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFindNewTracks.Click

        If bwApp.IsBusy = False Then
            Dim t As New cBwJob(cBwJob.JobType.FIND_NEW_TRACKS_FROM_HDD)
            bwApp.RunWorkerAsync(t)
        End If

    End Sub

    Private Sub sAddFileToListBoxTracks(ByVal filePath As String)

        ' 4.0.5.1 Prevented possible addition of duplicate entries to tracks ListBox in Explorer tab
        If Not lbFiles.Items.Contains(filePath) Then
            lbFiles.Items.Add(filePath)
        End If

    End Sub


    Private Sub bwApp_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles bwApp.ProgressChanged

        Dim userStateString As String = String.Empty
        If TypeOf e.UserState Is String Then
            userStateString = e.UserState.ToString
        End If

        Dim currTaskType As ProgressType = CType(e.ProgressPercentage, ProgressType)

        Select Case currTaskType

            Case ProgressType.ADD_DISC_TO_LISTBOX_DISCS
                If lbDiscs.Items.Contains(e.UserState.ToString) = False Then
                    lbDiscs.Items.Add(e.UserState.ToString)
                End If

            Case ProgressType.ADD_TRACKS_TO_LIBRARY
                pbarTracks.Style = ProgressBarStyle.Continuous
                pbarTracks.Increment(1)
                sBarLeft.Text = String.Format("Adding file {0}", userStateString)

            Case ProgressType.ADD_TRACKS_TO_LISTBOX_TRACKS
                sAddFileToListBoxTracks(userStateString) 'filePath

            Case ProgressType.CLEAR_DISCS_LISTBOX
                lbDiscs.Items.Clear()

            Case ProgressType.DELETE_TRACKS_DEAD
                pbarTracks.Style = ProgressBarStyle.Continuous
                pbarTracks.Increment(1)
                sBarLeft.Text = String.Format("Checking dead tracks to delete in: {0}", userStateString)

            Case ProgressType.DELETE_TRACKS_DEAD_ALIEN
                pbarTracks.Style = ProgressBarStyle.Continuous
                pbarTracks.Increment(1)
                sBarLeft.Text = String.Format("Checking dead or foreign tracks to delete in: {0}", userStateString)

            Case ProgressType.FOUND_LYRICS_FOR
                pBarDiscs.Style = ProgressBarStyle.Continuous
                pBarDiscs.Increment(1)
                sBarLeft.Text = String.Format("Found Lyrics for {0}", e.UserState.ToString)

            Case ProgressType.INCREMENT_DISC_PROGRESS
                pBarDiscs.Style = ProgressBarStyle.Continuous
                pBarDiscs.Increment(1)

            Case ProgressType.INCREMENT_TRACK_PROGRESS
                pbarTracks.Increment(1)

            Case ProgressType.INITIALIZE_PLAYER_LIBRARY_START
                pBarDiscs.Style = ProgressBarStyle.Marquee
                sBarLeft.Text = String.Format("Initializing {0} Library.", mpCurrentPlayer)

            Case ProgressType.INITIALIZE_PLAYER_FINISH
                pBarDiscs.Style = ProgressBarStyle.Continuous
                sBarLeft.Text = mfGetTruncatedText("Ready. Found " & CType(mPlayer, cPlayerWMP).MainLibraryTracks.count & " Media.", ttApp, ssApp)
                My.Forms.frmOptions.txtMusicFolder.Text = mpMusicFolderPath
                If Directory.Exists(mpMusicFolderPath) Then
                    If Not My.Forms.frmOptions.lbMusicFolders.Items.Contains(mpMusicFolderPath) Then
                        My.Forms.frmOptions.lbMusicFolders.Items.Add(mpMusicFolderPath)
                    End If
                    If mpMusicFolderPaths.Contains(mpMusicFolderPath) = False Then
                        mpMusicFolderPaths.Add(mpMusicFolderPath)
                    End If
                End If

            Case ProgressType.READY
                sBarLeft.Text = "Ready. " + userStateString

            Case ProgressType.SCANNING_FILE_IN_HDD
                pBarDiscs.Style = ProgressBarStyle.Marquee
                If String.Empty <> userStateString Then
                    sBarLeft.Text = mfGetTruncatedText("Scanning " + Path.GetDirectoryName(userStateString), _
                                                             ttApp, ssApp)
                End If

            Case ProgressType.SCANNING_TRACK_IN_PLAYER_LIBRARY
                pbarTracks.Style = ProgressBarStyle.Continuous
                pbarTracks.Increment(1)
                sBarLeft.Text = "Scanning " & userStateString

            Case ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX
                pBarDiscs.Value = 0
                pBarDiscs.Maximum = CInt(e.UserState)

            Case ProgressType.UPDATE_TRACKS_PROGRESS_BAR_MAX
                pbarTracks.Value = 0
                pbarTracks.Maximum = CInt(e.UserState)

            Case Else

                Console.WriteLine("Not yet handled: " & currTaskType.ToString)

        End Select


    End Sub

    Private Sub bwApp_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bwApp.RunWorkerCompleted

        msAppendDebug("Job Finished: " & mCurrJobType.ToString)

        msWriteDebugLog()

    End Sub

    Private Sub SwitchToITSfvToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SwitchToITSfvToolStripMenuItem.Click

        If My.Settings.AppModeITsfv = False Then
            msAppModeSetITsfv()
        End If
        My.Forms.frmMain.Show()
        Me.Close()

    End Sub

    Private Sub OptionsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OptionsToolStripMenuItem.Click
        msShowOptions()
    End Sub

    Private Sub sBwAppRewriteUrlToCover()

        Dim jumpNum As Integer = 50
        Dim lLibraryPlaylist As WMPLib.IWMPPlaylist = CType(mPlayer, cPlayerWMP).MainLibraryTracks
        bwApp.ReportProgress(ProgressType.UPDATE_TRACKS_PROGRESS_BAR_MAX, lLibraryPlaylist.count / jumpNum)
        Dim jumpCheck As Integer = 0

        Dim trackList As New List(Of String)
        Dim artworkList As New List(Of String)

        For i As Integer = 0 To lLibraryPlaylist.count - 1

            jumpCheck += 1

            Dim track As New WMPTrack(CType(lLibraryPlaylist.Item(i), WMPLib.IWMPMedia3))

            If jumpCheck Mod jumpNum = 0 Then
                bwApp.ReportProgress(ProgressType.SCANNING_TRACK_IN_PLAYER_LIBRARY, String.Format("{0} - {1}", track.Album, track.Artist))
            End If

            If track.Kind = MediaType.AUDIO Then

                If track.Location <> String.Empty Then

                    Dim xt As New cXmlTrack(track, False)
                    Dim jpgCustom As String = String.Empty
                    If My.Settings.DefaultExArtworkFolder Then
                        jpgCustom = mfGetArtworkFilePathFromPattern(IO.Path.GetDirectoryName(track.Location), My.Settings.ArtworkFileNameEx, xt)
                    Else
                        jpgCustom = My.Settings.FolderPathExArtwork + Path.DirectorySeparatorChar + fGetFileNameFromPattern(My.Settings.ArtworkFileNamePatternEx, xt)
                    End If

                    If File.Exists(jpgCustom) Then
                        'Console.Writeline(jpgCustom)
                        trackList.Add(track.Location)
                        artworkList.Add(jpgCustom)
                    End If

                End If

            End If

        Next

        If File.Exists(My.Settings.UrlToCoverArtLocation) Then
            Dim oArtParser As New cUrlToCoverParser(My.Settings.UrlToCoverArtLocation)
            oArtParser.ReplaceEntries(trackList, artworkList)
            oArtParser.SaveDictionary()
        End If

        bwApp.ReportProgress(ProgressType.READY)


    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUrlToCoverArtRewrite.Click

        If File.Exists(My.Settings.UrlToCoverArtLocation) = False Then
            Dim dlg As New OpenFileDialog
            dlg.InitialDirectory = Path.GetDirectoryName(My.Settings.UrlToCoverArtLocation)
            dlg.Filter = DLG_FILTER_URLTOCOVERART
            dlg.FileName = My.Settings.UrlToCoverArtLocation

            If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
                My.Settings.UrlToCoverArtLocation = dlg.FileName
            End If
        End If

        If File.Exists(My.Settings.UrlToCoverArtLocation) Then

            If bwApp.IsBusy = False Then
                Dim t As New cBwJob(cBwJob.JobType.SYNC_MEDIA_CENTER_CACHE)
                bwApp.RunWorkerAsync(t)
            End If

        End If

    End Sub

    Private Sub frmWMPfv_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown

        Dim t As New cBwJob(cBwJob.JobType.INITIALIZE_PLAYER)
        bwApp.RunWorkerAsync(t)

    End Sub

    Private Sub ssApp_ItemClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles ssApp.ItemClicked

    End Sub

    Private Sub VersionHistoryToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles VersionHistoryToolStripMenuItem.Click
        msShowVersionHistory()
    End Sub

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        msShowAbout()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        msCloseForms()
    End Sub

    Private Sub DebugToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DebugToolStripMenuItem.Click

        msShowDebug(sBarLeft, ttApp, ssApp)

    End Sub

    Private Sub sUpdateGuiControls()

        'For Each pg As TabPage In tcExplorer.TabPages
        '    For Each ctl As Control In pg.Controls
        '        If ctl.Name <> chkValidate.Name Then
        '            ctl.Enabled = Not bwApp.IsBusy
        '        End If
        '    Next
        'Next

        For Each pg As TabPage In Me.tcTabs.TabPages
            If Not pg.Name = tpValidate.Name And _
            Not pg.Name = tpExplorer.Name Then
                For Each ctl As Control In pg.Controls
                    ctl.Enabled = Not bwApp.IsBusy
                Next
            End If
        Next

        btnStop.Enabled = bwApp.IsBusy
        SwitchToITSfvToolStripMenuItem.Enabled = Not bwApp.IsBusy

    End Sub

    Private Sub tmrApp_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        sUpdateGuiControls()
    End Sub

    Private Sub btnStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStop.Click
        mfCancelJob()
    End Sub

    Private Function mfCancelJob() As Boolean

        If bwApp.IsBusy Then
            If (MessageBox.Show("Are you sure?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes) Then
                bwApp.CancelAsync()
                If mCurrJobType = cBwJob.JobType.VALIDATE_LIBRARY Then
                    lbDiscs.Items.Clear()
                End If
                btnStop.Enabled = False
                Return True
            End If
        End If

        Return False

    End Function

    Private Sub btnValidateLibrary_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnValidateLibrary.Click
        If bwApp.IsBusy = False Then
            Dim t As New cBwJob(cBwJob.JobType.VALIDATE_LIBRARY)
            bwApp.RunWorkerAsync(t)
        End If
    End Sub

    Private Sub llblEmail_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles llblEmail.LinkClicked
        Process.Start("mailto:mcored@gmail.com?subject=WMPfv Development")
    End Sub
End Class