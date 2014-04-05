Imports System.IO
Imports iTSfv.cBwJob

Public Class frmValidator

    Private mValidator As cValidator = Nothing
    Private mFiles As New List(Of String)

    Private mCurrJob As cBwJob
    Private mCurrJobType As cBwJob.JobType = cBwJob.JobType.NEW_TASK

    ''*******************************************************************************
    ''* Conventions
    ''**************
    ''*
    ''* ffFunctionName - two ff in the front means it is executed by bwApp
    ''*
    ''* ffFunctionName - one f in the front means it is SAFE to execute without bwApp
    ''*******************************************************************************

    Private Sub frmValidator_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles Me.DragDrop

        Dim DropContents As String() = CType(e.Data.GetData(DataFormats.FileDrop, True), String())

        If DropContents.Length > 0 Then
            Dim listDropContents As New List(Of String)
            For Each fileOrDir As String In DropContents
                listDropContents.Add(fileOrDir)
            Next

            Dim listAddableFiles As New List(Of String)
            For Each dirOrFile As String In listDropContents
                listAddableFiles.AddRange(mfGetAddableFilesList(dirOrFile, mFileExtOtherAudio))
            Next

            sAddFiles(listAddableFiles)

            If 1 = DropContents.Length Then
                txtFolderPath.Text = DropContents(0)
            Else
                txtFolderPath.Text = Path.GetDirectoryName(DropContents(0))
            End If

        End If


    End Sub

    Private Sub sAddFiles(ByVal lList As List(Of String))

        mFiles.Clear()
        mFiles.AddRange(lList)
        lbDiscs.Items.Clear()

        mValidator = New cValidator(bwApp)

        btnValidate.Enabled = (mFiles.Count > 0)

    End Sub

    Private Sub frmValidator_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles Me.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.All
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Friend Sub sSettingsGet()

        If My.Settings.DefaultExArtworkFolder = True Then
            chkExportArtwork.Text = "Export Artwork to Album folder as " & My.Settings.ArtworkFileNameEx
        Else
            chkExportArtwork.Text = String.Format("Export Artwork to {0} as {1}", My.Settings.FolderPathExArtwork, My.Settings.ArtworkFileNamePatternEx)
        End If
        ttApp.SetToolTip(chkExportArtwork, chkExportArtwork.Text)

        'If My.Settings.LyricsFromAlbumFolder = True Then
        '    chkImportLyrics.Text = String.Format("Import Lyrics from Album folder as {0}{1}", My.Settings.LyricsFilenamePatternIm, My.Settings.LyricsFileExtIm)
        'Else
        '    chkImportLyrics.Text = String.Format("Import Lyrics from {0} as {1}{2}", My.Settings.LyricsFolderPathIm, My.Settings.LyricsFilenamePatternIm, My.Settings.LyricsFileExtIm)
        'End If

        'ttApp.SetToolTip(chkImportLyrics, chkImportLyrics.Text)

        chkEditCopyArtistToAlbumArtist.Text = CStr(If(My.Settings.OverwriteAlbumArtist, "Overwrite AlbumArtist using Artist tag", "Fill missing AlbumArtist using Artist tag"))
        chkEditCopyArtistToAlbumArtist.ForeColor = CType(IIf(My.Settings.OverwriteAlbumArtist, Color.Red, Color.Black), Color)

        If My.Settings.LyricsToAlbumFolder = True Then
            chkExportLyrics.Text = String.Format("Export Lyrics to Album folder as {0}{1}", My.Settings.LyricsFilenamePatternEx, My.Settings.LyricsFileExtEx)
        Else
            chkExportLyrics.Text = String.Format("Export Lyrics to {0} as {1}{2}", My.Settings.LyricsFolderPathEx, My.Settings.LyricsFilenamePatternEx, My.Settings.LyricsFileExtEx)
        End If
        ttApp.SetToolTip(chkExportLyrics, chkExportLyrics.Text)

        chkExportIndex.Text = String.Format("Export Index to Album folder as {0}{1}", My.Settings.IndexFileNamePattern, My.Settings.IndexFileExt)
        ttApp.SetToolTip(chkExportIndex, chkExportIndex.Text)

    End Sub

    Private Sub frmValidator_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        My.Forms.frmSplash.Close() '' close any hidden splash window
    End Sub


    Private Sub frmValidator_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        tcMain.ImageList = My.Forms.frmMain.ilTabs
        tpChecks.ImageKey = "tick.png"
        tpTracks.ImageKey = "music.png"
        tpDiscsBrowser.ImageKey = "cd.png"
        tpFileSystem.ImageKey = "folder_edit.png"


        WMPfvToolStripMenuItem.Enabled = (mAppInfo.ApplicationState = McoreSystem.AppInfo.SoftwareCycle.ALPHA)

        Me.Icon = My.Forms.frmMain.Icon
        Me.Text = mAppInfo.GetApplicationTitle(APP_ABBR_NAME_ITL, Application.ProductVersion)
        SwitchToITSfvToolStripMenuItem.Enabled = mfGetItunes()

        If mfGetItunes() = False Then

            msAppendDebug("iTSfv Lite Version: " & Application.ProductVersion)
            msAppendDebug("Logs Directory: " & My.Settings.LogsDir)
            msAppendDebug("iTMS Artwork Directory: " & My.Settings.ArtworkDir)
            msAppendDebug("Temporary Directory: " & My.Settings.TempDir)

        End If

        Call sSettingsGet()

    End Sub

    Private Sub btnValidate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnValidate.Click

        If bwApp.IsBusy = False Then
            Dim t As New cBwJob(cBwJob.JobType.VALIDATE_LIBRARY)
            bwApp.RunWorkerAsync(t)
        End If

    End Sub

    Private Sub sButtonBrowserDir()

        If mfGetFolderBrowser("Browse for the folder of audio files to validate...", txtFolderPath) = True Then

            sAddFiles(mfGetAddableFilesList(txtFolderPath.Text, mFileExtOtherAudio))

        End If

    End Sub

    Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click

        Call sButtonBrowserDir()

    End Sub


    Private Sub bwApp_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bwApp.DoWork

        ''******************************************
        ''* Step 1: Load Job, Job Type, Validator
        ''******************************************
        mCurrJob = CType(e.Argument, cBwJob)
        mCurrJobType = CType(e.Argument, cBwJob).Job

        '' start logging debug info
        'Try
        '    mBwAppDebugWriter = New StreamWriter(mFilePathDebugLog, True)
        'Catch ex As Exception

        'End Try

        msAppendDebug("Job Started: " & mCurrJobType.ToString)

        ''******************************************
        ''* Step 2: Load the tracks to Discs Browser
        ''******************************************
        bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, mFiles.Count)

        If lbDiscs.Items.Count = 0 Then

            For i As Integer = 0 To mFiles.Count - 1

                Dim filePath As String = CStr(mFiles(i))
                If File.Exists(filePath) Then
                    Dim xt As New cXmlTrack(filePath, False)
                    mValidator.sLoadTrackToArtist(xt)
                End If
                bwApp.ReportProgress(ProgressType.INCREMENT_DISC_PROGRESS)

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

        End If

        Dim success As Boolean = False

        Select Case mCurrJobType

            Case cBwJob.JobType.VALIDATE_DISC                
                success = mValidator.ValidateDisc(CType(mCurrJob.TaskData, cXmlDisc))

            Case cBwJob.JobType.VALIDATE_LIBRARY
                success = mValidator.ValidateLibrary()

        End Select

        e.Result = success


    End Sub

    Private Sub bwApp_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles bwApp.ProgressChanged

        Dim currTaskType As ProgressType = CType(e.ProgressPercentage, ProgressType)

        Select Case currTaskType

            Case ProgressType.ADD_DISC_TO_LISTBOX_DISCS
                If lbDiscs.Items.Contains(e.UserState.ToString) = False Then
                    lbDiscs.Items.Add(e.UserState.ToString)
                End If

            Case ProgressType.CLEAR_DISCS_LISTBOX
                lbDiscs.Items.Clear()

            Case ProgressType.FOUND_LYRICS_FOR
                pBarDiscs.Increment(1)
                sBarTrack.Text = String.Format("Found Lyrics for {0}", e.UserState.ToString)

            Case ProgressType.INCREMENT_DISC_PROGRESS
                pBarDiscs.Increment(1)

            Case ProgressType.INCREMENT_TRACK_PROGRESS
                pbarTracks.Increment(1)

            Case ProgressType.READY
                sBarTrack.Text = "Ready. " + e.UserState.ToString

            Case ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX
                pBarDiscs.Value = 0
                pBarDiscs.Maximum = CInt(e.UserState)

            Case ProgressType.UPDATE_TRACKS_PROGRESS_BAR_MAX
                pbarTracks.Value = 0
                pbarTracks.Maximum = CInt(e.UserState)

            Case Else
                pBarDiscs.Increment(1)
                sBarTrack.Text = String.Format("Validating {0}", Path.GetFileName(CStr(e.UserState)))

        End Select


    End Sub

    Private Sub bwApp_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bwApp.RunWorkerCompleted

        If chkStandardCheck.Checked Then
            Dim success As Boolean = CBool(e.Result)
            If success = True Then
                sBarTrack.Text = "All files confirm to iTunes Store Standard."
            Else
                sBarTrack.Text = "One or more files do not confirm to iTunes Store Standard."
                msWriteListToFile(New LogData(mFilePathNonItsTracks, mListTracksNonITSstandard))
                Process.Start(mFilePathNonItsTracks)
            End If
        End If

        msAppendDebug("Job Finished: " & mCurrJobType.ToString)

        msWriteDebugLog()

    End Sub

    Private Sub OptionsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OptionsToolStripMenuItem.Click
        msShowOptions()
    End Sub

    Private Function fGetDiscSelected() As cXmlDisc

        Dim lDisc As cXmlDisc = Nothing

        If lbDiscs.SelectedIndex <> -1 And mValidator IsNot Nothing Then

            lDisc = mValidator.fGetDisc(lbDiscs.SelectedItem.ToString)

        End If

        Return lDisc

    End Function

    Private Sub lbDiscs_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbDiscs.SelectedIndexChanged

        Dim lDisc As cXmlDisc = fGetDiscSelected()
        If lDisc IsNot Nothing Then
            pbArtwork.Image = mfGetBitMapFromFilePath(lDisc.ArtworkPath)
        End If

    End Sub

    Private Sub btnBrowsDisc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowsDisc.Click

        Dim lDisc As cXmlDisc = fGetDiscSelected()
        If lDisc IsNot Nothing Then
            Dim dir As String = lDisc.Location

            If Directory.Exists(dir) Then
                Process.Start(dir)
            End If
        End If

    End Sub

    Private Sub btnValidateDisc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnValidateDisc.Click

        Dim lDisc As cXmlDisc = fGetDiscSelected()
        If lDisc IsNot Nothing Then

            If bwApp.IsBusy = False Then
                Dim t As New cBwJob(cBwJob.JobType.VALIDATE_DISC)
                t.TaskData = lDisc '' We pack the selected disc 
                bwApp.RunWorkerAsync(t)
            End If

        End If

    End Sub

    Private Sub OpenDirectoryToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenDirectoryToolStripMenuItem.Click
        sButtonBrowserDir()
    End Sub


    Private Sub SwitchToITSfvToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SwitchToITSfvToolStripMenuItem.Click

        If My.Settings.AppModeITsfv = False Then
            msAppModeSetITsfv()
        End If
        My.Forms.frmMain.Show()
        Me.Close()

    End Sub

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        msShowAbout()
    End Sub

    Private Sub TrackReplaceAssistantToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackReplaceAssistantToolStripMenuItem.Click
        msShowTrackReplaceAssistant()
    End Sub

    Private Sub frmValidator_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown

        If mfGetItunes() = False Then
            msCheckUpdatesAuto(sBarTrack)
        End If

    End Sub

    Private Sub tmrUpdate_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrUpdate.Tick
        ttApp.SetToolTip(ssAppDisc, sBarTrack.Text)
    End Sub

    Private Sub VersionHistoryToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles VersionHistoryToolStripMenuItem.Click
        msShowVersionHistory()
    End Sub

    Private Sub WMPfvToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WMPfvToolStripMenuItem.Click
        msShowWMPfv()
    End Sub

    Private Sub DebugToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DebugToolStripMenuItem.Click
        msShowDebug(sBarTrack, ttApp, ssAppDisc)
    End Sub
End Class