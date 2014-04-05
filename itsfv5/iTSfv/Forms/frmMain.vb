'This file is part of iTSfv.

'iTSfv is free software; you can redistribute it and/or modify
'it under the terms of the GNU Lesser General Public License as 
'published by the Free Software Foundation; either version 2.1 
'of the License, or (at your option) any later version.

'iTSfv is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU Lesser General Public License for more details.

'You should have received a copy of the GNU General Public License
'along with this program.  If not, see <http://www.gnu.org/licenses/>.

Imports Graphing.V3
Imports iTunesLib
Imports System.IO
Imports McoreSystem
Imports System.Xml.Serialization
Imports System.Net.Mail
Imports iTSfv.cBwJob
Imports System.ComponentModel
Imports System.Text
Imports CommandLineParserLib
Imports System.Net
Imports System.Collections.Specialized

Public Class frmMain

    Private Const NUM_QUICK_VALIDATE As Integer = 100

    '* File Paths *'        
    ' moved to mFileSystem.vb

    Private mAlbumBrowserView As String = ""

    Private mMainLibraryTracks As IITTrackCollection

    Private mHtml As New TreeGUI.cHtml
    Private mAppTitle As String = mAppInfo.GetApplicationTitle
    Private mStringDecompilePattern As String = ""

    ' Lists/Logs
    Private mListNoArtwork As New List(Of String)
    Private mListEmbeddedArtwork As New List(Of String)
    Private mListAlbumsMissingArtwork As New List(Of String)
    Private mListTracksMultipleArtwork As New List(Of String)
    Private mListArtworkAdded As New List(Of String)
    Private mListLyricsAdded As New List(Of String)    
    Private mListReadOnlyTracks As New List(Of String)
    Private mListInfoPaths As New List(Of String)

    Private mListAlbumsMissingArtworkFull As New List(Of cXmlTrack)
    Private mListArtworkLowRes As New List(Of cXmlTrack)
    Private mListRatings As New List(Of String)

    ' Table Keys
    Private mListDiscKeys As New List(Of String)
    Private mListAlbumKeys As New List(Of String)

    ' Tables
    Private mTableDiscs As Hashtable = New Hashtable
    Private mTableAlbums As Hashtable = New Hashtable
    Private mTableArtworkRes As Hashtable ' null on purpose

    Private mLastCheckedTrackID As Integer = 1

    Private mCurrProgress As ProgressType = ProgressType.READY
    Private mCurrentAlbum As String

    Private mListLogList As New ArrayList

    Private mSecondsSoFar As Double = 0
    Private mEToCstr As String = ""
    Private mBooFinalTask As Boolean = True

    Private mWatcher As cWatcher
    Private mCurrJob As cBwJob

    Private mLast100Tracks As Boolean = False

    Private mFolderJpgMinSize As Double = My.Settings.FolderJpgMinSize
    Private mIncludePodcasts As Boolean = My.Settings.IncludePodcasts

    ' ComboBox Text which cannot be accessed by BackgroundWorker
    Private mTrimDirectionText As String = String.Empty
    Private mArtistDecompiled As String = String.Empty
    Private mFindText As String = String.Empty
    Private mReplaceText As String = String.Empty

    Private mValModes As New ValidatorModes(True, True, True, True)

    Private mBooResume As Boolean = False
    Private mBooTableArtworkResEdited As Boolean = False

    Private mTracksCount As Integer = 0
    Private mGuiReady As Boolean = False

    Public Function fGetLibraryParser() As cLibraryParser

        ' cannot move this function to a module because iTunesApp object is not initialized

        If mXmlLibParser Is Nothing Then

            Try
                mXmlLibParser = New cLibraryParser(bwApp, mItunesApp.LibraryXMLPath)

                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                '' 5.25.0.4 Customized iTunes Music folder location in Options > Explorer was always reset to default iTunes Music folder location
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                If My.Settings.MusicFolderPath = String.Empty Then
                    My.Settings.MusicFolderPath = mXmlLibParser.MusicFolder
                End If

                mStatsFilePath = My.Settings.LogsDir & mXmlLibParser.LibraryPersistantID & "-stats.cache"
                mFilePathArtworkRes = My.Settings.LogsDir & mXmlLibParser.LibraryPersistantID & "-tracks.cache"

            Catch ex As Exception
                '' handles errors 
                '' like Could not find file 'H:\Users\Manno\My Documents\My Music\iTunes\iTunes Music Library.xml'.
                msAppendWarnings(ex.Message)
            End Try

        End If

        Return mXmlLibParser

    End Function


    Public Function fGetIITTrackFromXmlTrack(ByVal xmlTrack As cXmlTrack) As IITTrack
        Return mMainLibraryTracks.Item(xmlTrack.Index)
    End Function

    Private Function fGetDiscSelected() As cInfoDisc

        Dim lDisc As cInfoDisc = Nothing

        If lbDiscs.SelectedIndex <> -1 And mTableDiscs IsNot Nothing Then

            Dim albumTitle As String = lbDiscs.SelectedItem.ToString
            lDisc = CType(mTableDiscs.Item(albumTitle), cInfoDisc)

        End If

        Return lDisc

    End Function


    Private Function fBwAppCropArtwork(ByVal pathArtwork As String) As Boolean

        Dim success As Boolean = False

        If mFileIsImage(pathArtwork) Then

            Try

                Dim tempFilePath As String = Path.Combine(My.Settings.TempDir, Path.GetFileName(pathArtwork))
                My.Computer.FileSystem.CopyFile(pathArtwork, tempFilePath, True)
                pathArtwork = tempFilePath

                'Console.Writeline("Found Artwork: " & pathArtwork)
                Dim lDisc As cInfoDisc = fGetDiscSelected()

                If lDisc IsNot Nothing Then

                    ' show the replace artwork wizard
                    Dim f As New frmAddArtwork(pathArtwork, lDisc)
                    f.ShowDialog()

                    If f.DialogResult <> Windows.Forms.DialogResult.Cancel Then

                        Dim src As New cArtworkSource(lDisc.Tracks.Item(0))
                        src.ArtworkPath = f.ArtworkPath
                        src.ReplaceArtwork = True
                        src.ArtworkType = ArtworkSourceType.File

                        Dim valOPt As New cValidatorOptions
                        valOPt.ARTWORK_SOURCE = src
                        valOPt.REMOVE_ARTWORK = True

                        Dim currJob As New cBwJob(cBwJob.JobType.VALIDATE_DISC_ADVANCED)
                        currJob.TaskData = valOPt

                        bwApp.RunWorkerAsync(currJob)
                        success = True

                    End If

                End If


            Catch ex As Exception
                ' oh well
            End Try

        End If

        Return success

    End Function

    Private Sub Form1_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles Me.DragDrop

        ' 5.12.1.3 Prevent running Add New Files job when a user drags new files to iTSfv while iTSfv is already adding new music to iTunes
        If bwApp.IsBusy = False Then

            'mBwAppDebugWriter = New StreamWriter(mFilePathDebugLog, True)
            'msAppendDebug(".")
            msAppendDebug("Initialized Adding Music via Drag&Drop")
            'mBwAppDebugWriter.Close()

            Dim filesOrDirs As String() = CType(e.Data.GetData(DataFormats.FileDrop, True), String())

            If filesOrDirs.Length = 1 Then
                If File.Exists(filesOrDirs(0)) Then
                    If fBwAppCropArtwork(filesOrDirs(0)) = True Then
                        Exit Sub
                    End If
                End If
            End If

            sQueueFilesFoldersToLibrary(filesOrDirs)

        End If

    End Sub

    Private Function sQueueFolderToLibrary(ByVal folderPath As String) As Boolean

        Return sQueueFilesFoldersToLibrary(New String() {folderPath})

    End Function

    Private Function sQueueFilesFoldersToLibrary(ByVal filesOrDirs As String()) As Boolean

        lbFiles.Items.Clear()
        mWatcher.ClearList()

        Dim listDropContents As New List(Of String)
        For Each fileOrDir As String In filesOrDirs
            If Directory.Exists(fileOrDir) Then
                mListInfoPaths.AddRange(fFindLogFiles(fileOrDir))
            End If
            listDropContents.Add(fileOrDir)
        Next

        Dim listAddableFiles As New List(Of String)
        For Each dirOrFile As String In listDropContents
            listAddableFiles.AddRange(mfGetAddableFilesList(dirOrFile))
        Next

        For Each filePath As String In listAddableFiles
            sQueueFileToListBoxTracks(filePath)
        Next

        If listAddableFiles.Count > 0 Then
            If bwApp.IsBusy Then
                If mCurrJobTypeMain = JobType.COMMAND_LINE Then
                    sBwAppAddNewFilesToLibrary()
                End If
            Else
                Dim taskAddNewFiles As New cBwJob(cBwJob.JobType.ADD_NEW_TRACKS)
                bwApp.RunWorkerAsync(taskAddNewFiles)
            End If
        End If

    End Function

    Private Function sButtonAddNewFiles() As Boolean

        Dim txtFolderPath As New TextBox
        If mfGetFolderBrowser("Add album folder to Library", txtFolderPath) = True Then
            sQueueFolderToLibrary(txtFolderPath.Text)
        End If

    End Function

    Private Sub sBwAppAddNewFilesToLibrary()

        bwApp.ReportProgress(ProgressType.SET_ACTIVE_TAB, tpExplorer) ' useful for drag n drop

        sAddFilesToLibrary()

    End Sub

    Private Sub sAddFilesToLibrary()

        ' prevent modifying the file add list
        'fswApp.EnableRaisingEvents = False

        ' 5.09.4.0 Support for dragging more music albums while iTSfv is already adding music to iTunes
        While lbFiles.Items.Count > 0
            ' repeat this while lbtracks items is greater than 0

            Dim filePaths As New List(Of String)

            Try
                For Each filePath As String In lbFiles.Items
                    filePaths.Add(filePath)
                Next
            Catch ex As Exception
                msAppendWarnings(ex.Message + " while reading Tracks ListBox for adding new music")
            End Try

            If filePaths.Count > 0 Then

                filePaths.Sort()
                filePaths.Reverse()

                Dim bSingleAlbum As Boolean = True

                Try
                    For i As Integer = 0 To filePaths.Count - 2
                        Dim f1 As TagLib.File = TagLib.File.Create(filePaths(i))
                        Dim f2 As TagLib.File = TagLib.File.Create(filePaths(i + 1))
                        TagLib.Id3v2.Tag.DefaultVersion = 3
                        TagLib.Id3v2.Tag.ForceDefaultVersion = True
                        bSingleAlbum = bSingleAlbum And (f1.Tag.Album.Trim.Equals(f2.Tag.Album.Trim))
                    Next
                Catch ex As Exception
                    bSingleAlbum = False
                    msAppendWarnings(String.Format("Error reading {0} while checking if files belong to a single album.", Path.GetDirectoryName(filePaths(0))))
                End Try

                Dim xd As New cXmlDisc(filePaths)
                Dim aaf As New cXmlDiscArtistFinder(xd)

                bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, filePaths.Count)

                Dim ans As DialogResult = Windows.Forms.DialogResult.No

                Dim lAddNewFilesDialogBox As New frmAddNewFiles(fileCount:=filePaths.Count)

                If mCurrJobTypeMain = cBwJob.JobType.SCHEDULE_DO Or _
                 mCurrJobTypeMain = JobType.COMMAND_LINE Or _
                   My.Settings.SyncSilent = True Then

                    ans = Windows.Forms.DialogResult.Yes

                    If bSingleAlbum = True Then
                        lAddNewFilesDialogBox.chkOverwriteTags.Checked = True
                        lAddNewFilesDialogBox.cboAlbumArtist.Text = aaf.AlbumArtist
                        lAddNewFilesDialogBox.chkAlbumArtist.Checked = True
                    End If

                Else

                    Try
                        Dim f As TagLib.File = TagLib.File.Create(filePaths(0))
                        TagLib.Id3v2.Tag.DefaultVersion = 3
                        TagLib.Id3v2.Tag.ForceDefaultVersion = True
                        lAddNewFilesDialogBox = New frmAddNewFiles(f, filePaths.Count)
                    Catch ex As Exception
                        lAddNewFilesDialogBox = New frmAddNewFiles(filePaths.Count)
                    End Try

                    'lAddNewFilesDialogBox.chkOverwriteTags.Enabled = booSingleAlbum

                    lAddNewFilesDialogBox.txtAdviceAlbumArtist.Text = aaf.AlbumArtist

                    If bSingleAlbum Then
                        lAddNewFilesDialogBox.AcceptButton = lAddNewFilesDialogBox.btnYes
                    Else
                        lAddNewFilesDialogBox.AcceptButton = lAddNewFilesDialogBox.btnNo
                    End If

                    lAddNewFilesDialogBox.ShowDialog()

                    ans = lAddNewFilesDialogBox.DialogResult

                End If ' of whether or not to show Add New Files dialog

                If ans = Windows.Forms.DialogResult.Yes Then

                    Dim lOldAlbumsCount As Integer = mTableDiscs.Count

                    Dim trackNum As Integer = filePaths.Count

                    For Each filepath As String In filePaths
                        If mListInfoPaths.Contains(filepath) Then
                            mListInfoPaths.Remove(filepath)
                        End If
                    Next

                    For Each filePath As String In filePaths

                        '' 5.35.02.1 iTSfv is now more error resilient when adding new files to library
                        Try
                            sPausePendingCheck()
                            If bwApp.CancellationPending = True Then
                                Exit Sub
                            End If

                            ' OVERWRITE TAGS ACCORDING TO ADD WINDOW DIALOG BOX
                            Dim xt As New cXmlTrack(filePath, False)

                            If lAddNewFilesDialogBox.chkOverwriteTags.Checked = True Then
                                If lAddNewFilesDialogBox.chkAlbumArtist.Checked = True Then
                                    xt.AlbumArtist = lAddNewFilesDialogBox.cboAlbumArtist.Text
                                End If
                                If lAddNewFilesDialogBox.chkAlbum.Checked = True Then
                                    xt.Album = lAddNewFilesDialogBox.txtAlbum.Text
                                End If
                                If lAddNewFilesDialogBox.chkGenre.Checked = True Then
                                    xt.Genre = lAddNewFilesDialogBox.cboGenre.Text
                                End If
                                If lAddNewFilesDialogBox.chkYear.Checked = True Then
                                    xt.Year = CInt(lAddNewFilesDialogBox.nudYear.Value)
                                End If
                            End If

                            Dim lStructure As String = If(My.Settings.OrganizeMusic, mfGetLegalDirectoryNameFromPattern(My.Settings.MusicFolderStructure, xt), "")
                            Dim lDir As String = Path.Combine(My.Settings.MusicFolderPath, lStructure)
                            Dim filePathNew As String = filePath

                            ' ORGANIZE MUSIC ONLY FOR EXTERNAL FILES
                            If mfFileIsInMusicFolder(filePath) = False Then

                                If My.Settings.AddFilesMode = AddFilesType.DO_NOTHING Then
                                    lDir = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(filePath)), lStructure)
                                    filePathNew = Path.Combine(lDir, Path.GetFileName(filePath))
                                Else
                                    '' 5.32.0.0 Music when added via iTSfv can be automatically organized in a customized folder strucutre                                                                        
                                    filePathNew = Path.Combine(lDir, Path.GetFileName(filePath))
                                End If ' organize music

                                If filePath <> filePathNew Then
                                    Select Case My.Settings.AddFilesMode
                                        Case AddFilesType.COPY
                                            My.Computer.FileSystem.CopyFile(filePath, filePathNew, overwrite:=True)
                                        Case AddFilesType.DO_NOTHING
                                            My.Computer.FileSystem.MoveFile(filePath, filePathNew, overwrite:=True)
                                        Case AddFilesType.MOVE
                                            My.Computer.FileSystem.MoveFile(filePath, filePathNew, overwrite:=True)
                                    End Select
                                End If

                            End If

                            ' reconstruct filePathNew
                            ' 5.60.6.3 Fixed any possibility where iTunes would refuse to add files copied by iTSfv
                            filePathNew = Path.Combine(Path.GetDirectoryName(filePathNew), Path.GetFileName(filePathNew))

                            fForceTags(lAddNewFilesDialogBox, filePathNew, trackNum)
                            trackNum -= 1

                            bwApp.ReportProgress(ProgressType.ADD_TRACKS_TO_LIBRARY, filePathNew)

                            Dim job As IITOperationStatus
                            job = mItunesApp.LibraryPlaylist.AddFile(filePathNew)

                            bwApp.ReportProgress(ProgressType.REMOVE_TRACK_FROM_LISTBOX, filePath)

                            Dim track As IITFileOrCDTrack

                            If job IsNot Nothing Then


                                'mfExcludeFileAdd(filePath)

                                If mfWarnNoTrackNumProceed(job.Tracks.Item(1).TrackNumber = 0) = False Then
                                    Process.Start("http://www.mp3tag.de/en/")
                                    bwApp.CancelAsync()
                                    job.Tracks.Item(1).Delete()
                                    bwApp.ReportProgress(ProgressType.READY)
                                    Exit Sub
                                End If

                                track = CType(job.Tracks.Item(1), IITFileOrCDTrack)

                                msAppendDebug(String.Format("Added {0} to {1}", filePath, track.Location))
                                ''Console.Writeline(track.Location)

                                ' COPY NFO/LOG files ( happens once because we are gonna clear the list after first successful file copy )

                                If mListInfoPaths.Count > 0 Then

                                    If bSingleAlbum = True Then

                                        msAppendDebug("Found single album. Starting to copy log/nfo files...")
                                        Dim dirAlbum As String = Path.GetDirectoryName(track.Location)

                                        Try

                                            For Each fpath As String In mListInfoPaths
                                                If File.Exists(fpath) Then

                                                    Dim destPath As String = Path.Combine(dirAlbum, Path.GetFileName(fpath))
                                                    If Not File.Exists(destPath) Then

                                                        Select Case My.Settings.AddFilesMode
                                                            Case AddFilesType.COPY
                                                                If mfFileIsInMusicFolder(filePath) = False Then
                                                                    My.Computer.FileSystem.CopyFile(fpath, destPath, overwrite:=False)
                                                                    msAppendDebug(String.Format("Copied {0} to {1}", fpath, destPath))
                                                                End If
                                                            Case AddFilesType.MOVE
                                                                My.Computer.FileSystem.MoveFile(fpath, destPath, overwrite:=False)
                                                                msAppendDebug(String.Format("Moved {0} to {1}", fpath, destPath))
                                                        End Select
                                                    End If

                                                End If
                                            Next

                                        Catch ex As Exception
                                            msAppendWarnings(ex.Message + " while copying log/nfo files.")
                                        Finally
                                            mListInfoPaths.Clear()
                                        End Try

                                    Else
                                        msAppendDebug("Found multiple albums. iTSfv will not copy log/nfo files...")
                                    End If ' single album

                                End If ' if nfo/log file list count is greater than zero

                                sLoadTrackToDiscsTable(track)

                            Else
                                msAppendDebug(String.Format("{0} was rejected by iTunes. Log off, log on and retry.", filePathNew))
                            End If ' job is not nothing

                        Catch ex As Exception

                            msAppendWarnings(ex.Message + String.Format(" while adding {0} to library", filePath))

                        End Try

                    Next ' for each file in file paths

                    If filePaths.Count > 0 Then
                        Try
                            Dim srcDir As String = Path.GetDirectoryName(filePaths(0))
                            Dim fr As New cFolderRemover()
                            If fr.fFolderSafeToDelete(srcDir) Then
                                My.Computer.FileSystem.DeleteDirectory(srcDir, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
                            End If
                        Catch ex As Exception
                            msAppendWarnings(ex.Message + " while attempting to remove empty directory")
                        End Try
                    End If

                    If chkValidate.Checked = True Then
                        ssValidateAlbums(lOldAlbumsCount)
                    End If

                    bwApp.ReportProgress(ProgressType.READY, String.Format("Found and added {0} new tracks from HDD.", filePaths.Count.ToString))

                ElseIf ans = Windows.Forms.DialogResult.No Then

                    bwApp.ReportProgress(ProgressType.CLEAR_TRACKS_LISTBOX)
                    bwApp.ReportProgress(ProgressType.READY, String.Format("Found {0} new tracks from HDD.", filePaths.Count.ToString))
                    Exit While 'or will crash

                Else

                    bwApp.ReportProgress(ProgressType.READY, String.Format("Found {0} new tracks from HDD.", filePaths.Count.ToString))
                    Exit While 'or will crash

                End If


            Else

                bwApp.ReportProgress(ProgressType.READY, String.Format("Found {0} new tracks from HDD.", filePaths.Count.ToString))


            End If

        End While

        If mWatcher IsNot Nothing Then
            mWatcher.ClearList()
        End If

        'mfWriteExcludedFiles()

    End Sub

    Private Function fFindLogFiles(ByVal dirPath As String) As List(Of String)

        Dim extFiles As New List(Of String)

        If My.Settings.AddFilesMode = AddFilesType.COPY Then

            Try
                extFiles.AddRange(Directory.GetFiles(dirPath, "*.nfo", SearchOption.AllDirectories))
                extFiles.AddRange(Directory.GetFiles(dirPath, "*.cue", SearchOption.AllDirectories))
                extFiles.AddRange(Directory.GetFiles(dirPath, "*.log", SearchOption.AllDirectories))
                extFiles.AddRange(Directory.GetFiles(dirPath, "*.txt", SearchOption.AllDirectories))
            Catch ex As Exception
                msAppendWarnings(ex.ToString)
            End Try

            ' add jpgs if there are more than one jpg
            Dim jpgFiles As New List(Of String)
            jpgFiles.AddRange(Directory.GetFiles(dirPath, "*.jpg", SearchOption.AllDirectories))
            jpgFiles.AddRange(Directory.GetFiles(dirPath, "*.png", SearchOption.AllDirectories))

            If jpgFiles.Count > 1 Or (1 = jpgFiles.Count AndAlso My.Settings.ImportAnySingleArtwork = False) Then
                msAppendDebug("Found one or more JPG files in source Album Folder...")
                extFiles.AddRange(jpgFiles)
            Else
                Try
                    Dim temp As Image = mfGetBitMapFromFilePath(jpgFiles(0))
                    If temp IsNot Nothing Then
                        If (temp.Width = temp.Height) AndAlso _
                        (temp.Width >= My.Settings.LowResArtworkWidth And _
                         temp.Height >= My.Settings.LowResArtworkHeight) Then
                            ' proper artwork
                            msAppendDebug("Found single JPG file that is appropriate for Artwork...")
                            extFiles.AddRange(jpgFiles)
                        End If
                    End If
                Catch ex As Exception
                    ' oh well
                End Try

            End If

        Else

            ' Add everything to move
            extFiles.AddRange(Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories))

        End If


        Return extFiles

    End Function

    Private Sub Form1_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles Me.DragEnter

        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.All
        Else
            e.Effect = DragDropEffects.None
        End If

    End Sub

    Private Sub sCleanFSTemp()

        'If bwApp.IsBusy Then
        '    bwApp.ReportProgress(ProgressType.CLEANING_TEMP_DIR)
        'End If

        If String.IsNullOrEmpty(My.Settings.TempDir) = False Then
            If My.Settings.CleanTempDir Then
                Try
                    Dim dirs() As String = Directory.GetDirectories(My.Settings.TempDir)
                    For Each d As String In dirs
                        Directory.Delete(d, True)
                    Next
                    Dim files() As String = Directory.GetFiles(My.Settings.TempDir, "*.*", SearchOption.AllDirectories)
                    For Each f As String In files
                        File.Delete(f)
                    Next
                Catch ex As Exception
                    Console.Write(ex.ToString())
                End Try
            End If
        End If

    End Sub

    Private Sub sCleanFS()

        Try

            Dim fi As New FileInfo(mFilePathMusicFolderActivity)
            If fi.Exists AndAlso fi.Length = 0 Then
                fi.Delete()
            End If

            Dim fiDebug As New FileInfo(mFilePathDebugLog)
            If fiDebug.Exists AndAlso fiDebug.Length = 0 Then
                fi.Delete()
            End If

            sCleanFSTemp()

        Catch ex As Exception
            Console.Write(ex.ToString())
        End Try

    End Sub

    Private Sub frmMain_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed

        If Me.WindowState <> FormWindowState.Maximized Then
            My.Settings.MainWindowWidth = Me.Size.Width
            My.Settings.MainWindowHeight = Me.Size.Height
        End If

        My.Forms.frmSplash.Close() '' close any hidden splash window
        Me.WindowState = FormWindowState.Minimized

        If mWatcher IsNot Nothing Then
            mWatcher.StopWatchFolders()
        End If

        sSettingsSave()

        sCleanFS()

        Try
            '' 5.34.5.3 Force releasing iTunes object before quitting iTSfv
            If mItunesApp IsNot Nothing Then
                System.Runtime.InteropServices.Marshal.ReleaseComObject(mItunesApp)
                GC.Collect()
                mItunesApp = Nothing
            End If
        Catch ex As Exception
            ' bugger it
        End Try

    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        ' e.Cancel = True means Form will not Close
        ' if fCancelJob = true then we need e.cancel = false

        If bwApp.IsBusy Then
            e.Cancel = Not fCancelJob()
            If e.Cancel = False Then
                tmrFormClose.Enabled = True
            End If
        End If

    End Sub

    Private Sub sUpdateGuiControls()

        'OptionsToolStripMenuItem.Enabled = Not bwApp.IsBusy

        For Each pg As TabPage In tcExplorer.TabPages
            For Each ctl As Control In pg.Controls
                If ctl.Name <> chkValidate.Name Then
                    ctl.Enabled = Not bwApp.IsBusy
                End If
            Next
        Next

        For Each pg As TabPage In Me.tcTabs.TabPages
            If Not pg.Name = tpSettings.Name And _
            Not pg.Name = tpExplorer.Name Then
                For Each ctl As Control In pg.Controls
                    ctl.Enabled = Not bwApp.IsBusy
                Next
            End If
        Next

        btnValidateTracksChecks.Enabled = Not bwApp.IsBusy
        btnValidateSelectedTracks.Enabled = Not bwApp.IsBusy
        btnValidateSelectedTracksLibrary.Enabled = Not bwApp.IsBusy
        btnValidateSelectedTracksFolder.Enabled = Not bwApp.IsBusy

        'miJobs.Enabled = Not bwApp.IsBusy
        For Each tti As ToolStripItem In miJobs.DropDownItems
            If tti.Name <> VerboseModeToolStripMenuItem.Name Then
                tti.Enabled = Not bwApp.IsBusy
            End If
        Next

        chkDiscComplete.Enabled = Not bwApp.IsBusy

        btnStatistics.Enabled = Not (bwApp.IsBusy And mStatsMaker Is Nothing)
        PlayFirstTrackToolStripMenuItem.Enabled = (lbDiscs.SelectedIndex <> -1)
        btnBrowseAlbum.Enabled = (lbDiscs.SelectedIndex <> -1) 'AndAlso Not bwApp.IsBusy
        btnCreatePlaylistAlbum.Enabled = (lbDiscs.SelectedIndex <> -1) 'AndAlso Not bwApp.IsBusy
        btnValidateAlbum.Enabled = (lbDiscs.SelectedIndex <> -1) AndAlso Not bwApp.IsBusy
        ValidateDiscToolStripMenuItem.Enabled = btnValidateAlbum.Enabled

        QuickValidationToolStripMenuItem.Enabled = Not bwApp.IsBusy
        ValidateSelectedTracksToolStripMenuItem.Enabled = Not bwApp.IsBusy

        chkResume.Enabled = Not bwApp.IsBusy
        chkItunesStoreStandard.Enabled = Not bwApp.IsBusy

        tsmiSelectedTracksValidate.Visible = Not bwApp.IsBusy

        btnValidateSelected.Enabled = Not bwApp.IsBusy
        btnValidateLibrary.Enabled = Not bwApp.IsBusy
        btnStop.Enabled = bwApp.IsBusy AndAlso mCurrJobTypeMain <> JobType.INITIALIZE_PLAYER

        cmsDiscs.Enabled = lbDiscs.Items.Count > 0
        cmsFiles.Enabled = lbFiles.Items.Count > 0

        ' menus
        tsmAddFolderToLib.Enabled = Not bwApp.IsBusy
        tsmItunesArtworkGrabber.Enabled = Not bwApp.IsBusy
        tsmTiunesStoreArtworkGrabSelected.Enabled = Not bwApp.IsBusy

        ' explorer tab
        lbDiscs.Enabled = True
        ttApp.SetToolTip(chkResume, "Last checked Track ID: " & My.Settings.LastCheckedTrackID)

    End Sub

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.MinimumSize = New Size(My.Settings.MainWindowWidthDefault, My.Settings.MainWindowHeightDefault)
        Me.sTrackProgress.Visible = False

        tsmLyricsViewer.Visible = mAppInfo.ApplicationState <> AppInfo.SoftwareCycle.FINAL
        tsmSetInfo.Visible = mAppInfo.ApplicationState <> AppInfo.SoftwareCycle.FINAL

        Me.Text = mAppTitle
        mfUpdateStatusBarText("Starting Timers...", True)

        bwTimers.RunWorkerAsync()

        My.Settings.LoadWithWindows = mfAppIsLoadedAsStartup(Application.ProductName, Application.ExecutablePath)

        If My.Settings.LoadToTray Then
            Me.WindowState = FormWindowState.Minimized
            Me.ShowInTaskbar = False
        End If

    End Sub

    Private Sub frmMain_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize

        If Me.WindowState = FormWindowState.Minimized Then
            Me.ShowInTaskbar = Not My.Settings.MinimizeToTray
        End If

    End Sub

    Private Sub Form1_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown

        OpenTracksReportToolStripMenuItem1.Text = Application.ProductName + " Report..."

        'mWatcher = New cWatcher(bwApp)
        bwWatcher.RunWorkerAsync()

        mListLogList.Add(mListAlbumsInconsecutiveTracks)
        mListLogList.Add(mListArtworkAdded)
        mListLogList.Add(mListFileNotFound)
        mListLogList.Add(mListFoldersNoFolderJpg)
        mListLogList.Add(mListTracksNoAlbumArtist)
        mListLogList.Add(mListNoArtwork)
        mListLogList.Add(mListTracksNoTrackNum)
        mListLogList.Add(mListReadOnlyTracks)
        mListLogList.Add(mListTracksNonITSstandard)

        For Each ss As String In My.Settings.ExportTrackPatterns
            cboExportFilePattern.Items.Add(ss)
        Next
        cboExportFilePattern.Text = My.Settings.ExportTrackPattern

        cboTrimDirection.SelectedIndex = 1
        cboAppendChar.SelectedIndex = 0

        Dim t As cBwJob
        Dim args() As String = Environment.GetCommandLineArgs
        If args.Length > 1 Then
            t = New cBwJob(cBwJob.JobType.COMMAND_LINE)
        Else
            t = New cBwJob(cBwJob.JobType.INITIALIZE_PLAYER)
        End If
        bwApp.RunWorkerAsync(t)

        ttApp.SetToolTip(chkReplaceWithNewKind, mfGetText("UpdateToIdenticalButDifferentKindFiles.txt"))
        ttApp.SetToolTip(btnReplaceTracks, mfGetText("UpdateToIdenticalButDifferentKindFiles.txt"))
        ttApp.SetToolTip(btnRecover, mfGetText("BrowsePreviousXMLlibrary.txt"))
        ttApp.SetToolTip(chkItunesStoreStandard, mfGetText("ItunesStoreStandard.txt"))
        ttApp.SetToolTip(chkImportArtwork, mfGetText("AddArtworkFromArtworkJpg.txt"))
        ttApp.SetToolTip(cboExportFilePattern, mfGetText("TagsSupported.txt"))
        ttApp.SetToolTip(cboExportFilePattern, mfGetText("TagsSupported.txt"))
        ttApp.SetToolTip(cboClipboardPattern, mfGetText("TagsSupported.txt"))

        btnAdjustRatings.Text = chkLibraryAdjustRatings.Text & " for all tracks in Library"
        btnReplaceTracks.Text = chkReplaceWithNewKind.Text & " (e.g. AAC with MP3 or MP3 with AAC)"
        chkSheduleAdjustRating.Text = btnAdjustRatings.Text
        chkScheduleFindNewFilesHDD.Text = btnFindNewFiles.Text & " and add to iTunes Music Library"

        For Each item As String In My.Settings.ArtistsDecompile
            If Not cboArtistsDecompiled.Items.Contains(item) Then cboArtistsDecompiled.Items.Add(item)
        Next

        msCheckUpdatesAuto(sBarTrack)

        If My.Settings.DeleteTempFiles Then
            bwFS.RunWorkerAsync()
        End If

        niTray.Visible = True

        'sUpdateSearchEngines()

    End Sub

    Private WithEvents tsmiEngine As System.Windows.Forms.ToolStripMenuItem

    Private Sub tsmiEngine_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles tsmiEngine.Click

        Dim tsmi As ToolStripMenuItem = CType(sender, ToolStripMenuItem)

        If mItunesApp IsNot Nothing AndAlso mItunesApp.BrowserWindow.SelectedTracks IsNot Nothing AndAlso _
mItunesApp.BrowserWindow.SelectedTracks.Count > 0 Then

            Dim track As IITTrack = mItunesApp.BrowserWindow.SelectedTracks.Item(1)
            Dim url As String = String.Concat(tsmi.Tag, mfEncodeUrl(mfGetStringFromPattern(My.Settings.GoogleTrack, track)))
            Process.Start(url)

        End If

    End Sub

    Private Sub sUpdateSearchEngines()

        If My.Settings.DiscSearch <> String.Empty Then

            tsmiSearch.DropDownItems.Clear()

            Dim sEngines() As String = My.Settings.DiscSearch.Split(CChar(vbCrLf))
            If sEngines.Length > 0 Then
                For Each line As String In sEngines
                    Dim spLine() As String = line.Split(CChar("|"))
                    If spLine.Length = 2 Then
                        Dim name As String = spLine(0)
                        Dim url As String = spLine(1)
                        tsmiEngine = New System.Windows.Forms.ToolStripMenuItem
                        tsmiEngine.Text = name.Trim
                        tsmiEngine.Tag = url
                        tsmiSearch.DropDownItems.Add(tsmiEngine)
                        AddHandler tsmiEngine.Click, AddressOf tsmiEngine_Click
                    Else
                        ' add blank line
                        tsmiSearch.DropDownItems.Add(New ToolStripSeparator)
                    End If
                Next

                ' Remove automatically added Handler
                RemoveHandler tsmiEngine.Click, AddressOf tsmiEngine_Click

            End If

        End If

    End Sub


    Private Function sGetXhtmlLine(ByVal msg As String, ByVal track As iTunesLib.IITFileOrCDTrack) As String

        ' mReport.WriteLine(String.Format("{0} for {1} - {2} - {3}", msg, track.Artist, track.Album, track.Name))
        'Dim line As String = mHtml.GetPara(String.Format("{0} for {1}", msg, track.Location))
        Dim line As String = mHtml.GetList(String.Format("{0} for {1}", msg, track.Location))
        'mReport.WriteLine(line)
        Return line

    End Function

    Private Sub sTracksAlbumArtist(ByVal track As iTunesLib.IITFileOrCDTrack, ByVal lDisc As cInfoDisc)

        Dim AlbumArtist As String = lDisc.Artist
        Dim countMissingAlbumArtist As Integer = 0
        Dim trackLoc As String = "dead track"

        Try
            trackLoc = track.Location
        Catch ex As Exception
            ' oh well
        End Try

        ' can have The track is not modifiable. errors so need try/catch
        Dim f As TagLib.File = TagLib.File.Create(track.Location)

        If f.Tag.FirstAlbumArtist Is Nothing Or My.Settings.OverwriteAlbumArtist = True Then

            Try
                track.AlbumArtist = If(String.IsNullOrEmpty(track.AlbumArtist), AlbumArtist, track.AlbumArtist)
                f.Tag.AlbumArtists = New String() {track.AlbumArtist}
                f.Save()
                If mListTracksNoAlbumArtist.Contains(track.Location) = False Then
                    mListTracksNoAlbumArtist.Add(track.Location)
                End If
            Catch ex As Exception
                msAppendWarnings(ex.Message & " while filling AlbumArtist for " & trackLoc)
                msAppendWarnings(ex.StackTrace)
            End Try

        Else

            If track.Compilation = True Then
                ' dont need to overwrite all the time
                If track.AlbumArtist <> VARIOUS_ARTISTS Then
                    Try
                        track.AlbumArtist = VARIOUS_ARTISTS
                    Catch ex As Exception
                        msAppendWarnings(ex.Message & " while setting track as Compilation for " & trackLoc)
                        msAppendWarnings(ex.StackTrace)
                    End Try
                End If
            ElseIf track.AlbumArtist.Equals("Various") Or track.AlbumArtist.Equals("VA") Then
                Try
                    track.AlbumArtist = VARIOUS_ARTISTS
                Catch ex As Exception
                    msAppendWarnings(ex.Message & " while filling AlbumArtist for " & trackLoc)
                End Try
            End If

        End If

        ' Fill Sort Artist

        If String.IsNullOrEmpty(track.SortArtist) Then
            Dim sortArtist As String = track.AlbumArtist
            If sortArtist.StartsWith("The ") Then
                sortArtist = sortArtist.Remove(0, 4)
            End If
            track.SortArtist = sortArtist
        End If

        If Not String.IsNullOrEmpty(track.AlbumArtist) And My.Settings.CopyAlbumArtistToSortArtist Then
            Try
                Dim sortArtist As String = track.AlbumArtist
                If sortArtist.StartsWith("The ") Then
                    sortArtist = sortArtist.Remove(0, 4)
                End If
                If track.SortArtist <> sortArtist Then
                    track.SortArtist = sortArtist
                End If
            Catch ex As Exception
                msAppendWarnings(ex.Message)
            End Try
        End If

        If Not String.IsNullOrEmpty(track.SortArtist) And My.Settings.FillSortAlbumArtist Then
            Try
                If String.IsNullOrEmpty(track.SortAlbumArtist) Then
                    track.SortAlbumArtist = track.SortArtist
                End If
            Catch ex As Exception
                msAppendWarnings(ex.Message)
            End Try
        End If

    End Sub


    Private Sub sCheckTrackLyrics(ByVal track As iTunesLib.IITFileOrCDTrack)

        Try
            If mfHasLyrics(track, My.Settings.LyricsCharMin) = False Then
                mListMissingLyrics.Add(track.Location)
            End If
        Catch ex As Exception
            msAppendWarnings(ex.Message + " while checking for track lyrics")
        End Try


    End Sub

    Private Sub sCheckTrackNumber(ByVal track As iTunesLib.IITFileOrCDTrack)

        If track.TrackNumber = Nothing AndAlso mListTracksNoTrackNum.Contains(track.Location) = False Then
            mListTracksNoTrackNum.Add(track.Location)
        End If

    End Sub

    Private Sub sCheckTrackITSstandard(ByVal track As iTunesLib.IITFileOrCDTrack)

        Dim xt As New cXmlTrack(track, False)

        If Not fIsItunesStoreStandard(xt) Then
            mListTracksNonITSstandard.Add(xt.Location + xt.Notes)
        End If

    End Sub

    Private Sub sCheckTrackEmbeddedArtwork(ByVal track As iTunesLib.IITFileOrCDTrack)

        Dim countMissingArtwork As Integer = 0

        Try
            Dim artWork As iTunesLib.IITArtworkCollection = track.Artwork

            If artWork.Count > 0 AndAlso artWork.Item(1).IsDownloadedArtwork = True Then

                countMissingArtwork += 1
                mListEmbeddedArtwork.Add(track.Location)

            End If

        Catch ex As Exception

            msAppendWarnings(ex.Message + " while checking for embedded artwork")

        End Try


    End Sub

    Private Sub sCheckTrackArtwork(ByVal track As iTunesLib.IITFileOrCDTrack)

        Dim countMissingArtwork As Integer = 0

        Dim artWork As iTunesLib.IITArtworkCollection = track.Artwork

        If artWork.Count = 0 Then

            countMissingArtwork += 1
            mListNoArtwork.Add(track.Location)

            If Not mListAlbumsMissingArtwork.Contains(track.Album) Then
                mListAlbumsMissingArtwork.Add(track.Album)
                Dim tr As New cXmlTrack(track, False)
                mListAlbumsMissingArtworkFull.Add(tr)

            End If

        End If

    End Sub

    Private Function ffReplaceFolderArtwork(ByVal folderArtworkPath As String, ByVal artSrcNew As cArtworkSource) As cArtworkSource

        Dim pathArtworkCompare As String = String.Empty
        Dim artSrcResult As New cArtworkSource(artSrcNew.Track)

        If artSrcNew.ArtworkPath <> String.Empty Then
            ' this is gonna get us valid itms art /embeded artwork name / art in folders
            pathArtworkCompare = artSrcNew.ArtworkPath
            artSrcResult.ArtworkType = artSrcNew.ArtworkType
        End If

        If pathArtworkCompare = String.Empty Or artSrcNew.ArtworkType = ArtworkSourceType.File Then
            ' we use track embedded artwork to compare if exists
            If artSrcNew.Track.Artwork.Count > 0 Then
                artSrcResult.ArtworkType = ArtworkSourceType.Track
                pathArtworkCompare = mfGetTempArtworkCachePath(TEMP_FOLDER_ARTWORK_NAME, artSrcNew.Track)
                artSrcNew.Track.Artwork(1).SaveArtworkToFile(pathArtworkCompare)
            End If
        End If

        If pathArtworkCompare <> String.Empty Then

            Dim s As Stream = File.Open(pathArtworkCompare, FileMode.Open, FileAccess.Read)
            Dim imgCompareArtwork As Image = Image.FromStream(s)
            s.Close()

            ' if ANY error in origArtworkPath

            If File.Exists(folderArtworkPath) Then

                Dim fiArtwork As New FileInfo(folderArtworkPath)
                Dim wasReadOnly As Boolean = fiArtwork.IsReadOnly

                ' if read only then remove read only tag
                If fiArtwork.IsReadOnly Then
                    fiArtwork.IsReadOnly = False
                End If

                Dim s1 As Stream = File.Open(folderArtworkPath, FileMode.Open, FileAccess.Read)
                Dim imgFolderArtwork As Image = Image.FromStream(s1)
                s1.Close()

                ' undo the remove read only operation
                If wasReadOnly Then
                    fiArtwork.IsReadOnly = True
                End If

                Dim booReplace As Boolean = (imgCompareArtwork.Width * imgCompareArtwork.Height) > (imgFolderArtwork.Width * imgFolderArtwork.Height)
                msAppendDebug(String.Format("Replace {0}x{1} with {3}x{4} for {2}? {5}", _
                        imgFolderArtwork.Width, imgFolderArtwork.Height, folderArtworkPath, _
                        imgCompareArtwork.Width, imgCompareArtwork.Height, booReplace))
                ' attempting to fix [ itsfv-Bugs-1790529 ] generic error occurred in GDI+
                imgCompareArtwork.Dispose()
                imgFolderArtwork.Dispose()

                artSrcResult.ReplaceArtwork = booReplace

                If booReplace = True Then
                    ' the track artwork path has a higher res
                    artSrcResult.ArtworkPath = pathArtworkCompare
                Else
                    ' the folder artwork path has a higher res
                    artSrcResult.ArtworkPath = folderArtworkPath
                End If

            Else ' if there is no valid alternative artwork filepath then dont have to replace

                artSrcResult.ReplaceArtwork = False
                artSrcResult.ArtworkPath = pathArtworkCompare

            End If ' if folderartwork exists

        Else

            artSrcResult.ReplaceArtwork = False
            artSrcResult.ArtworkPath = folderArtworkPath

        End If ' if compare artwork filepath is empty

        Return artSrcResult

    End Function

    Private Function ffReplaceTrackArtwork(ByVal track As iTunesLib.IITFileOrCDTrack, ByVal srcNew As cArtworkSource) As cArtworkSource

        If track.Artwork.Count > 0 Then

            Dim locTrack As String = "dead track"
            Try
                locTrack = track.Location
            Catch ex As Exception
            End Try

            Dim argReplaceArt As New cArtworkSource(track)

            ' returns boolean replace to replace track's artwork or not 
            ' returns url with largest artwork path

            Try

                If track.Artwork.Item(1).IsDownloadedArtwork Then

                    argReplaceArt.ReplaceArtwork = True ' NO EXCUSE   
                    argReplaceArt.ArtworkPath = srcNew.ArtworkPath
                    argReplaceArt.ArtworkType = srcNew.ArtworkType

                    msAppendDebug("Found non-embedded iTunes downloaded Artwork and set to embed..")

                Else

                    '' artwork is not itunes downloaded

                    If srcNew.ArtworkPath <> String.Empty Then
                        msAppendDebug("Compare Artwork Path: " & srcNew.ArtworkPath)
                    End If

                    Dim trackArtworkPath As String = mfGetTempArtworkCachePath(TEMP_TRACK_ARTWORK_NAME, track)

                    ' possible Catastrophic failure (Exception from HRESULT: 0x8000FFFF (E_UNEXPECTED))
                    ' generic gdi errors can happen here for some images
                    ' Dim trackArtwork As Bitmap = Nothing
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    '' 5.25.3.0 Track Artwork dimensions are retrieved from Cache while comparing Track Artwork to import the higher resolution artwork
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    Dim xt As cXmlTrack = fGetXmlTrackSyncCache(track)
                    argReplaceArt.Width = xt.Artwork.Width
                    argReplaceArt.Height = xt.Artwork.Height
                    msAppendDebug("Cached Track Artwork Path:   " & xt.Artwork.Location)
                    'track.Artwork(1).SaveArtworkToFile(trackArtworkPath)
                    'trackArtwork = fGetBitMapFromFilePath(trackArtworkPath)

                    If File.Exists(srcNew.ArtworkPath) Then

                        Dim fiArtwork As New FileInfo(srcNew.ArtworkPath)
                        Dim wasReadOnly As Boolean = fiArtwork.IsReadOnly

                        ' if read only then remove read only tag
                        If fiArtwork.IsReadOnly Then
                            fiArtwork.IsReadOnly = False
                        End If

                        Dim folderArtwork As Bitmap = mfGetBitMapFromFilePath(srcNew.ArtworkPath)

                        ' undo the remove read only operation
                        If wasReadOnly Then
                            fiArtwork.IsReadOnly = True
                        End If

                        ' check if the image is at least almost squared shape
                        Dim ar As Double = (folderArtwork.Width / folderArtwork.Height)
                        Dim booNewIsSquared As Boolean = (ar < 1.1) And (ar > 0.9)

                        If My.Settings.ArtworkSquaredPrefer = True AndAlso booNewIsSquared = False Then
                            Dim msg As String = String.Format("{0} with {1}x{2} will be ignored because it could cause White Borders.", srcNew.ArtworkPath, folderArtwork.Width, folderArtwork.Height)
                            mfUpdateStatusBarText(msg, True)
                            msAppendDebug(msg)
                        End If

                        Dim booOldIsLowRes As Boolean = argReplaceArt.Width < My.Settings.LowResArtworkWidth Or argReplaceArt.Height < My.Settings.LowResArtworkHeight

                        Dim bExtJpgBigger As Boolean = (folderArtwork.Height * folderArtwork.Width) > (argReplaceArt.Height * argReplaceArt.Width) AndAlso _
                        (My.Settings.ArtworkSquaredPrefer = booNewIsSquared)

                        argReplaceArt.ReplaceArtwork = bExtJpgBigger
                        argReplaceArt.ArtworkType = ArtworkSourceType.File ' Path.GetFileName(folderArtworkPath)

                        If bExtJpgBigger = True Then
                            If My.Settings.ArtworkSquaredPrefer = booNewIsSquared Or booOldIsLowRes = True Then
                                argReplaceArt.ArtworkPath = srcNew.ArtworkPath
                            Else
                                argReplaceArt.ArtworkPath = trackArtworkPath
                            End If
                        Else
                            argReplaceArt.ArtworkPath = trackArtworkPath
                        End If

                        msAppendDebug(String.Format("Track Artwork Size: {0}x{1}, Compare Artwork Size: {2}x{3}", argReplaceArt.Width, argReplaceArt.Height, folderArtwork.Width, folderArtwork.Height))

                        ' attempting to fix [ itsfv-Bugs-1790529 ] generic error occurred in GDI+
                        ' trackArtwork.Dispose()
                        folderArtwork.Dispose()

                        If argReplaceArt.ReplaceArtwork = True AndAlso argReplaceArt.ArtworkPath = trackArtworkPath Then

                            msAppendDebug("Track Artwork Path:   " & trackArtworkPath)

                            If File.Exists(trackArtworkPath) Then
                                File.Delete(trackArtworkPath)
                            End If

                            track.Artwork.Item(1).SaveArtworkToFile(trackArtworkPath)

                        End If

                        msAppendDebug(String.Format("Replace Artwork for {0}? {1}", Path.GetFileName(track.Location), argReplaceArt.ReplaceArtwork))

                    Else ' if there is no valid alternative artwork filepath then no competition

                        msAppendDebug(String.Format("Could not find any new Artwork to compare to {0}'s Artwork.", track.Location))

                        ' see if we can get itms artwork
                        If track.Artwork.Item(1).IsDownloadedArtwork = False Then
                            ' dont replace; keep the track artwork unchanged
                            argReplaceArt.ReplaceArtwork = False
                            argReplaceArt.ArtworkPath = srcNew.ArtworkPath
                        End If

                    End If '' srcNew artwork path exists

                End If '' artwork is not itunes downloaded

            Catch ex As Exception

                ' catch teh generic gdi error and ask to replace the artwork
                argReplaceArt.ReplaceArtwork = True
                argReplaceArt.ArtworkType = srcNew.ArtworkType ' Path.GetFileName(folderArtworkPath)
                argReplaceArt.ArtworkPath = srcNew.ArtworkPath
                msAppendWarnings(ex.Message & " for " & locTrack + " while comparing artwork to embed")
                msAppendWarnings(ex.StackTrace)
                msAppendDebug(String.Format("Replace Artwork for {0} {1}? {2}", track.TrackNumber.ToString("00"), mfGetNameToSearch(argReplaceArt.Track), argReplaceArt.ReplaceArtwork))

            End Try

            Return argReplaceArt

        Else

            Throw New Exception("Function called for non artwork track")

        End If


    End Function

    Private Function fSaveArtworkSafely2(ByVal destPath As String, ByVal compareSrc As cArtworkSource) As Boolean

        ' 5.10.1.1 Fixed iTSfv crashing during Saving Artwork caused by: Error HRESULT E_FAIL has been returned from a call to a COM component
        Dim bExported As Boolean = False

        If compareSrc IsNot Nothing Then

            Try
                Dim fiArtworkDestPath As New FileInfo(destPath)

                If Not fiArtworkDestPath.Exists Or My.Settings.ArtworkChooseManual Then
                    ' if there is no file exists then just save
                    bExported = mfSaveArtworkSafely(destPath, compareSrc)
                    String.Format("Saved {0}", Path.GetFileName(destPath))
                Else
                    ' now that there is exists a folder artwork we need to find if it needs replacing
                    If fiArtworkDestPath.Length < mFolderJpgMinSize Then
                        ' replace the artwork comparing size
                        bExported = mfSaveArtworkSafely(destPath, compareSrc)
                        String.Format("Replaced {0} by File Size", Path.GetFileName(destPath))
                    Else
                        ' if size test fails then try resolution test 
                        If mfCompareRes() = True Then
                            ' do the resolution test only if we are validating selected
                            Dim raTrack As cArtworkSource = ffReplaceFolderArtwork(destPath, compareSrc)
                            If raTrack.ReplaceArtwork = True Then
                                bExported = mfSaveArtworkSafely(destPath, raTrack)
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                msAppendWarnings(String.Format("{0} while saving {1} as {2}.", ex.Message, compareSrc.ArtworkPath, destPath))
                msAppendWarnings(ex.StackTrace)
            End Try

        End If

        Return bExported

    End Function

    Private Function fLyricsImport(ByVal track As IITFileOrCDTrack) As Boolean

        ' user interaction procedure
        ' 

        If track IsNot Nothing AndAlso track.Location IsNot Nothing Then

            If Not My.Settings.FormatsIgnore.Contains(Path.GetExtension(track.Location)) Then

                If mfHasLyrics(track, 0) = False Or My.Settings.LyricsOverwrite = True Then

                    Dim lyrics As String = String.Empty
                    Dim lyricsSrc As String = String.Empty

                    If My.Settings.LyricsFromLyricWiki Then

                        '******************
                        '* Download Lyrics
                        '******************
                        Dim lws As UploadersLib.OtherServices.Lyrics = mfGetLyrics(New cXmlTrack(track, False))
                        If lws IsNot Nothing Then
                            lyrics = lws.Text
                        End If

                        If lyrics.Length < My.Settings.MinCharLyricsWeb Then
                            lyrics = String.Empty
                        End If

                        If lyrics <> String.Empty Then
                            lyricsSrc = "Lyricsfly"
                            ' have to add this becaues the statusbar update from mfGetLyricsFromLyricWiki is too fast 
                            bwApp.ReportProgress(ProgressType.FOUND_LYRICS_FOR, String.Format("Artist: ""{0}"", Song: ""{1}""", track.Artist, track.Name))
                        End If

                    End If

                    If lyrics = String.Empty Or My.Settings.LyricsFromLyricWiki = False Then

                        '*********************
                        '* Read from local HDD
                        '*********************

                        Dim lyricsRootDir As String = String.Empty
                        Dim lyricsPath As String = String.Empty

                        If My.Settings.LyricsFromAlbumFolder Then
                            lyricsRootDir = Path.GetDirectoryName(track.Location)
                        Else
                            lyricsRootDir = My.Settings.LyricsFolderPathIm
                        End If

                        Dim lyricsNameWithSubdir As String = String.Empty
                        If My.Settings.LyricsPatternFromFileIm = True Then
                            lyricsNameWithSubdir = Path.GetFileNameWithoutExtension(track.Location) + My.Settings.LyricsFileExtEx
                        Else
                            lyricsNameWithSubdir = mfGetFileNameFromPattern(My.Settings.LyricsFilenamePatternEx, track) + My.Settings.LyricsFileExtEx
                        End If

                        Dim lyricsSubdir As String = mfGetLegalTextForDirectory(Path.GetDirectoryName(lyricsNameWithSubdir))
                        Dim lyricsName As String = Path.GetFileName(lyricsNameWithSubdir)

                        lyricsPath = Path.Combine(lyricsRootDir, Path.Combine(lyricsSubdir, lyricsName))

                        If File.Exists(lyricsPath) Then
                            Using sr As New StreamReader(lyricsPath)
                                lyrics = sr.ReadToEnd
                                lyricsSrc = Path.GetFileName(lyricsPath)
                            End Using
                        End If

                    End If

                    If lyrics <> String.Empty Then
                        track.Lyrics = mfGetFixedLyrics(lyrics)
                        Dim msg As String = String.Format("Added lyrics to ""{0}"" from {1}.", track.Name, lyricsSrc)
                        msAppendDebug(msg)
                        mfUpdateStatusBarText(msg, True)
                        mListLyricsAdded.Add(track.Location)
                    End If

                End If ' track has no lyrics or needs to overwrite lyrics

            End If ' if not file is an ignored 

        End If

    End Function

    Private Function fLyricsExport(ByVal track As IITFileOrCDTrack) As Boolean

        If track IsNot Nothing AndAlso track.Location IsNot Nothing Then

            Try
                Dim lyrics As String = track.Lyrics
                '' 5.34.5.7 iTSfv tried to fetch lyrics online for the songs that have no lyrics despite the "Import Lyrics from Lyricsfly" being unchecked. [Jojo]
                If My.Settings.LyricsFromLyricWiki AndAlso lyrics = String.Empty Then
                    Dim artist As String = mGetAlbumArtist(track)
                    Dim song As String = mfGetNameToSearch(track)
                    Dim lws As UploadersLib.OtherServices.Lyrics = mfGetLyrics(New cXmlTrack(track, False))
                    lyrics = lws.Text
                End If

                If lyrics <> String.Empty Then

                    Dim lyricsFolder As String = String.Empty
                    Dim lyricsNameDir As String = String.Empty
                    Dim lyricsPath As String = String.Empty

                    If My.Settings.LyricsToAlbumFolder = True Then
                        lyricsFolder = Path.GetDirectoryName(track.Location)
                    Else
                        lyricsFolder = My.Settings.LyricsFolderPathEx
                    End If

                    ' this can have sub folders with name
                    If My.Settings.LyricsPatternFromFileEx = True Then
                        lyricsNameDir = Path.GetFileNameWithoutExtension(track.Location) + My.Settings.LyricsFileExtEx
                    Else
                        lyricsNameDir = mfGetFileNameFromPattern(My.Settings.LyricsFilenamePatternEx, track) + My.Settings.LyricsFileExtEx
                    End If

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
                        Dim msg As String = String.Format("Exported Lyrics as {0}", lyricsPath)
                        msAppendDebug(msg)
                        mfUpdateStatusBarText(msg, True)
                    End Using

                End If

            Catch ex As Exception
                msAppendWarnings(ex.Message + " while exporting Lyrics")
            End Try

        End If

    End Function

    '' ffSaveTrackArtworkJPEG moved to FileSystem.vb

    '' ffSaveArtworkSafely moved to FileSystem.vb

    ''' <summary>
    ''' Copies Folder.jpg to Album Folder
    ''' </summary>
    ''' <param name="track"></param>
    ''' <remarks></remarks>
    Private Function fArtworkExport(ByVal track As iTunesLib.IITFileOrCDTrack, ByVal altSrc As cArtworkSource) As Boolean

        ' artwork sources contain availble artwork paths of itms or music folder
        ' it doesnt carry track's embedded artwork locations

        Dim bExported As Boolean = False

        If track.Location IsNot Nothing AndAlso altSrc IsNot Nothing _
                AndAlso chkWinExportArtwork.Checked Then

            If My.Settings.DefaultExArtworkFolder = True Then

                ' we save folder.jpg, artwork.jpg, albumartsmall.jpg, custom.jpg
                Dim jpgFolder As String = IO.Path.GetDirectoryName(track.Location) + "\Folder.jpg"
                If My.Settings.DisableJpgFolder = False Then
                    ' see if we are creating a new folder.jpg
                    Dim booWasThere As Boolean = File.Exists(jpgFolder)
                    bExported = fSaveArtworkSafely2(jpgFolder, altSrc)
                    If bExported = True Then
                        ' if we have succesffully saved folder.jpg
                        Try
                            If My.Settings.ReadOnlyFolderJpg AndAlso My.Settings.FolderJpgHidden AndAlso My.Settings.FolderJpgSystem Then
                                File.SetAttributes(jpgFolder, File.GetAttributes(jpgFolder) Or FileAttributes.ReadOnly Or FileAttributes.Hidden Or FileAttributes.System)
                            ElseIf My.Settings.ReadOnlyFolderJpg AndAlso My.Settings.FolderJpgHidden Then
                                File.SetAttributes(jpgFolder, File.GetAttributes(jpgFolder) Or FileAttributes.ReadOnly Or FileAttributes.Hidden)
                            ElseIf My.Settings.ReadOnlyFolderJpg Then
                                File.SetAttributes(jpgFolder, File.GetAttributes(jpgFolder) Or FileAttributes.ReadOnly)
                            End If
                        Catch ex As Exception
                            msAppendWarnings(ex.Message + " while attempting to set attributes to Folder.jpg")
                        End Try
                        ' if it wasnt there before then add to list
                        If booWasThere = False Then
                            mListFoldersNoFolderJpg.Add(jpgFolder)
                        End If
                    End If
                End If

                Dim jpgArtwork As String = IO.Path.GetDirectoryName(track.Location) + "\Artwork.jpg"
                If My.Settings.DisableJpgArtwork = False Then
                    ' Additionally copy Artwork.jpg
                    bExported = fSaveArtworkSafely2(jpgArtwork, altSrc)
                End If

                If My.Settings.DisableJpgArtworkResize = False Then
                    If File.Exists(altSrc.ArtworkPath) AndAlso File.Exists(altSrc.ArtworkPathResized) = False Then

                        ' we save resized jpg
                        Dim srcResized As New cArtworkSource(track)
                        srcResized.ArtworkType = altSrc.ArtworkType
                        srcResized.ArtworkPath = altSrc.ArtworkPath
                        srcResized = ffResizeArtwork(srcResized)

                        Dim jpgResized As String = IO.Path.GetDirectoryName(track.Location) + _
                            Path.DirectorySeparatorChar + Path.GetFileName(srcResized.ArtworkPath)
                        If File.Exists(jpgResized) = False Then
                            My.Computer.FileSystem.CopyFile(srcResized.ArtworkPath, jpgResized, True)
                            altSrc.ArtworkPathResized = jpgResized
                        End If

                        'If altSrc.ArtworkResized = True Then
                        '    Dim jpgResized As String = IO.Path.GetDirectoryName(track.Location) + _
                        '        Path.DirectorySeparatorChar + Path.GetFileName(altSrc.ArtworkPath)
                        '    If File.Exists(jpgResized) = False Then
                        '        My.Computer.FileSystem.CopyFile(altSrc.ArtworkPath, jpgResized, True)
                        '    End If
                        'End If

                    End If
                End If

                If My.Settings.DisableJpgAlbumArtSmall = False Then
                    ' Additionally copy AlbumArtSmall.jpg
                    Dim jpgAlbumArtSmall As String = IO.Path.GetDirectoryName(track.Location) + "\AlbumArtSmall.jpg"
                    'If File.Exists(jpgAlbumArtSmall) = False Then
                    ' we are not going to overwrite and stuff like that
                    ' yes we in 5.30.4.1 Overwrite AlbumArtSmall.jpg to update Media Center cache
                    bExported = fSaveArtworkSafely2(jpgAlbumArtSmall, altSrc)
                    'End If
                End If

                ' Additionally copy Custom jpg
                Dim jpgCustom As String = Path.Combine(IO.Path.GetDirectoryName(track.Location), mfGetFileNameFromPattern(My.Settings.ArtworkFileNameEx, track))
                If jpgArtwork <> jpgCustom AndAlso jpgFolder <> jpgCustom Then
                    bExported = fSaveArtworkSafely2(jpgCustom, altSrc)
                End If

            Else ' not DefaultExArtworkFolder

                ' we wont be exporting
                ' artwork.jpg, albumartsmall; folder.jpg will be a pattern
                Dim jpgCustom As String = My.Settings.FolderPathExArtwork + Path.DirectorySeparatorChar + mfGetFileNameFromPattern(My.Settings.ArtworkFileNamePatternEx, track)
                bExported = fSaveArtworkSafely2(jpgCustom, altSrc)

            End If

        End If

        Return bExported

    End Function

    Private Function fIsValidating() As Boolean

        Return mCurrJobTypeMain = cBwJob.JobType.VALIDATE_DISC OrElse _
        mCurrJobTypeMain = cBwJob.JobType.VALIDATE_LIBRARY OrElse _
        mCurrJobTypeMain = cBwJob.JobType.VALIDATE_TRACKS_SELECTED

    End Function

    Private Function ffGetArtworkSource(ByVal lDisc As cInfoDisc) As cArtworkSource

        Dim srcAll As New cArtworkSources(lDisc)
        Dim src As cArtworkSource = srcAll.BestArtworkSourceForFile
        ' resize if user asks to

        If src IsNot Nothing Then
            If src.ArtworkPath <> String.Empty AndAlso My.Settings.ResizeArtwork = True Then
                src = ffResizeArtwork(srcAll.BestArtworkSourceForFile)
            End If

        End If

        Return src

    End Function

    Private Function ffResizeArtwork(ByVal src As cArtworkSource) As cArtworkSource

        Try
            ' get bitmap image
            Dim bm_source As Bitmap = mfGetBitMapFromFilePath(src.ArtworkPath)
            If bm_source IsNot Nothing Then
                ' if artwork is large
                If bm_source.Width > My.Settings.LowResArtworkWidth AndAlso _
                bm_source.Height > My.Settings.LowResArtworkHeight Then
                    Dim scale_factor As Double = My.Settings.LowResArtworkWidth / bm_source.Width
                    Dim bm_dest As New Bitmap( _
                               CInt(bm_source.Width * scale_factor), _
                               CInt(bm_source.Height * scale_factor))
                    ' Make a Graphics object for the result Bitmap.
                    Dim gr_dest As Graphics = Graphics.FromImage(bm_dest)
                    ' Copy the source image into the destination bitmap.
                    gr_dest.DrawImage(bm_source, 0, 0, _
                        bm_dest.Width + 1, _
                        bm_dest.Height + 1)
                    Dim dest_path As String = Path.Combine(mfGetTempArworkDirForTrack(track:=src.Track), String.Format("{0} ({1}x{2}).jpg", Path.GetFileNameWithoutExtension(src.ArtworkPath), bm_dest.Width.ToString, bm_dest.Height.ToString))
                    bm_dest.Save(dest_path, Imaging.ImageFormat.Jpeg)
                    msAppendDebug(String.Format("{0} artwork {1}x{2} resized to {3}x{4} as {5}", src.ArtworkPath, bm_source.Width, bm_source.Height, bm_dest.Width, bm_dest.Height, Path.GetFileName(dest_path)))
                    src.ArtworkPath = dest_path
                    src.ArtworkIsResized = True

                End If
            End If

        Catch ex As Exception
            msAppendDebug(String.Format("Failed to resize artwork {0}", src.ArtworkPath))
        End Try

        Return src

    End Function

    Private Function ffAddArtworkFromFileSafely(ByVal track As iTunesLib.IITFileOrCDTrack, ByVal src As cArtworkSource) As Boolean

        Dim success As Boolean = False

        If Not My.Settings.FormatsIgnore.Contains(Path.GetExtension(track.Location)) Then

            Try
                ' add to list of added artwork
                mListArtworkAdded.Add(track.Location)
                ' report progress
                bwApp.ReportProgress(ProgressType.IMPORTING_ARTWORK, String.Format("Importing Artwork to {3}{0}{3} in {3}{1}{3} from {2}", track.Name, fGetAlbum(track), src.ArtworkName, Chr(34)))

                If track.Artwork.Count > 0 Then
                    ' delete existing artwork 1
                    track.Artwork.Item(1).SetArtworkFromFile(src.ArtworkPath)
                    ' write log
                    msAppendDebug(String.Format("Replaced Artwork in {0} from {1}", track.Location, src.ArtworkPath))
                Else
                    ' add artwork
                    track.AddArtworkFromFile(src.ArtworkPath)
                    ' write log
                    msAppendDebug(String.Format("Added Artwork to {0} from {1}", track.Location, src.ArtworkPath))
                End If

                success = True

            Catch ex As Exception

                msAppendWarnings(ex.Message + ex.StackTrace)

                Dim locTrack As String = "dead track"

                ' write writing using taglib
                Try
                    locTrack = track.Location
                    Dim f As TagLib.File = TagLib.File.Create(track.Location)
                    ' 5.8.3.0 iTSfv will attempt adding artwork using TagLib to tracks iTunes failed to AddArtworkFromFile
                    'f.Tag.Pictures = New TagLib.Picture() {TagLib.Picture.CreateFromPath(src.ArtworkPath)}
                    f.Tag.Pictures = New TagLib.Picture() {New TagLib.Picture(src.ArtworkPath)}
                    TagLib.Id3v2.Tag.DefaultVersion = 3
                    TagLib.Id3v2.Tag.ForceDefaultVersion = True
                    f.Save()
                    msAppendDebug(String.Format("Added Artwork to {0} from {1} using TagLib.dll", track.Location, src.ArtworkPath))
                    success = True
                Catch ex2 As Exception
                    ' [ itsfv-Bugs-1790522 ] HRESULT E_FAIL has been returned from a call to a COM componant
                    ' happens to some mp3 files
                    msAppendWarnings(ex2.Message & " for " & locTrack & Environment.NewLine & ex2.StackTrace)
                    msAppendDebug(String.Format("Failed adding Artwork to {0} from {1}", locTrack, src.ArtworkPath))
                End Try

            End Try

        End If

        Return success

    End Function

    Private Function sEditTrackImportArtwork(ByVal track As iTunesLib.IITFileOrCDTrack, ByVal artSrcNew As cArtworkSource) As Boolean

        Dim success As Boolean = False
        Dim artWorkColl As iTunesLib.IITArtworkCollection = track.Artwork

        If track.Location IsNot Nothing AndAlso artSrcNew IsNot Nothing Then

            If artWorkColl.Count > 0 Then
                ' 5.6.1.0 iTMS Artwork can now be imported without the presence of artwork saved in the file system
                ' if there exists is a higher resolution artwork in the file system 
                ' (only during validating selected tracks because of high hdd usage)
                ' replace track's artwork 
                ' otherwise if the track's artwork is downloaded from iTMS then 
                ' embed track's artwork

                If My.Settings.ArtworkChooseManual Then

                    If artSrcNew.ArtworkPath <> String.Empty Then
                        success = ffAddArtworkFromFileSafely(track, artSrcNew)
                    End If

                Else

                    If (My.Settings.DefaultImArtworkFolder = True AndAlso _
                                mfCompareRes() = True AndAlso _
                                artSrcNew.ArtworkPath <> String.Empty) OrElse _
                                (mfCompareRes() = True AndAlso track.Artwork(1).IsDownloadedArtwork = True) Then

                        Dim artSrc As cArtworkSource = ffReplaceTrackArtwork(track, artSrcNew)
                        If artSrc.ReplaceArtwork = True Then
                            success = ffAddArtworkFromFileSafely(track, artSrc)
                        End If

                    ElseIf track.Artwork(1).IsDownloadedArtwork = True Then

                        If artSrcNew.ArtworkPath <> String.Empty Then
                            success = ffAddArtworkFromFileSafely(track, artSrcNew)
                        End If

                    End If

                End If

            ElseIf artWorkColl.Count = 0 Then

                If artSrcNew.ArtworkPath <> String.Empty Then

                    If My.Settings.HighResArtworkOnly = False Or (My.Settings.HighResArtworkOnly = True AndAlso fFolderArtworkIsHighes(artSrcNew) = True) Then
                        success = ffAddArtworkFromFileSafely(track, artSrcNew)
                    ElseIf fFolderArtworkIsHighes(artSrcNew) = False Then
                        msAppendDebug(String.Format("Artwork was not added to {0} due to low resolution.", Path.GetFileName(track.Location)))
                    End If

                End If

            End If

        End If ' check if file exists

        Return success

    End Function

    Private Function fFolderArtworkIsHighes(ByVal src As cArtworkSource) As Boolean

        Dim succ As Boolean = False

        Try
            Dim folderArt As Bitmap = mfGetBitMapFromFilePath(src.ArtworkPath)
            succ = (folderArt.Height >= My.Settings.LowResArtworkHeight AndAlso _
                    folderArt.Width >= My.Settings.LowResArtworkWidth)
        Catch ex As Exception
            ' oh well
        End Try

        Return succ

    End Function

    Private Sub sMakeReadOnlyOrNot(ByVal track As iTunesLib.IITFileOrCDTrack)

        '***********************
        '* Criterion 
        ' 1/ track.TrackNumber 
        ' 2/ track.Name 
        ' 3/ track.Artist 
        ' 4/ track.Album  
        ' 5/ track.Genre 
        ' 6/ track.Artwork 
        ' 7/ track.TrackCount         
        ' 8/ track.DiscNumber 
        ' 9/ track.DiscCount 
        ' 10/ track.Year
        ' 11/ track.Lyrics

        Dim fi As New FileInfo(track.Location)

        If fIsItunesStoreStandard(New cXmlTrack(track, False)) And track.Lyrics IsNot Nothing Then

            If track.Lyrics.ToString.Length > 100 Then
                If Not fi.Attributes = FileAttributes.ReadOnly + FileAttributes.Archive Then
                    File.SetAttributes(track.Location, CType(FileAttributes.ReadOnly + FileAttributes.Archive, FileAttributes))
                    mListReadOnlyTracks.Add(track.Location)
                    ''Console.Writeline("Made Read-Only: " & track.Location)
                End If
            Else
                If Not fi.Attributes = FileAttributes.Normal Then
                    File.SetAttributes(track.Location, FileAttributes.Normal)
                    ''Console.Writeline("Made Normal: " & track.Location)
                End If

            End If

        End If

    End Sub

    ' moved fOKtoValidate to mAdapter.vb

    Private Function fGetXmlTrackSyncCache(ByVal track As IITFileOrCDTrack) As cXmlTrack

        ' returns NOTHING if the track has no artwork embedded

        Dim sbarText As String = sBarTrack.Text

        If mTableArtworkRes Is Nothing Then
            '' has not initialized before
            mTableArtworkRes = New Hashtable
            '' try to load cache if user allows
            If fBooUseCache() = True AndAlso File.Exists(mFilePathArtworkRes) Then
                bwApp.ReportProgress(ProgressType.LOAD_ARTWORK_DIMENSIONS)
                Dim o As Object = mfReadObjectFromFileBF(mFilePathArtworkRes)
                If o IsNot Nothing Then
                    '' no errors
                    mTableArtworkRes = CType(mfReadObjectFromFileBF(mFilePathArtworkRes), Hashtable)
                    bwApp.ReportProgress(ProgressType.RESTORE_STATUS_BAR_MESSAGE, sbarText)
                End If
            End If
        End If

        Dim xt As cXmlTrack = Nothing

        If track.Artwork.Count > 0 Then

            If mTableArtworkRes.ContainsKey(track.TrackDatabaseID) Then

                Dim editDate As Date = CType(mTableArtworkRes.Item(track.TrackDatabaseID), cXmlTrack).ModificationDate
                Dim diff As TimeSpan = track.ModificationDate - editDate

                If My.Settings.ModifiedDateRetain = False Then
                    ' this information is not relevant if track modified date was retained
                    msAppendDebug("Track Last Modified according to iTunes: " & track.ModificationDate.ToString())
                    msAppendDebug("Track Last Modified according to Cache: " & editDate.ToString())
                End If

                If Math.Abs(diff.TotalSeconds) > 0 Then
                    ' update hash table if modified date is new
                    xt = New cXmlTrack(track, True)
                    mTableArtworkRes.Item(track.TrackDatabaseID) = xt
                    ''Console.Writeline(diff.TotalSeconds)
                    mBooTableArtworkResEdited = True
                    msAppendDebug(String.Format("Updated Artwork dimensions Cache for {0}", xt.Location))

                Else
                    ' which is what we would like to see
                    xt = CType(mTableArtworkRes.Item(track.TrackDatabaseID), cXmlTrack)

                    If xt.Artwork.Width = 0 Then
                        ' 5.34.11.3 Update Artwork cache if the Artwork dimensions were zero
                        ' update hash table if modified date is new
                        xt = New cXmlTrack(track, True)
                        mTableArtworkRes.Item(track.TrackDatabaseID) = xt
                        ''Console.Writeline(diff.TotalSeconds)
                        mBooTableArtworkResEdited = True
                        msAppendDebug(String.Format("Updated Artwork dimensions Cache for {0}", xt.Location))
                    Else
                        msAppendDebug(String.Format("Retrieved {0} from Cache.", xt.Location))
                    End If

                End If

            Else

                xt = New cXmlTrack(track, True)
                mTableArtworkRes.Add(track.TrackDatabaseID, xt)
                mBooTableArtworkResEdited = fBooUseCache()
                msAppendDebug(String.Format("Cached Artwork dimensions for {0}", xt.Location))

            End If

        End If


        Return xt

    End Function

    Private Function fCheckTrackArtworkIsLowRes(ByVal track As IITFileOrCDTrack) As Boolean

        Dim lowres As Boolean = False

        If track.Artwork.Count > 0 Then

            Dim xt As cXmlTrack = fGetXmlTrackSyncCache(track)

            If xt IsNot Nothing AndAlso xt.Artwork IsNot Nothing Then

                Dim lWidth As Integer = xt.Artwork.Width
                Dim lHeight As Integer = xt.Artwork.Height

                msAppendDebug(String.Format("Retrieved Artwork dimensions from cache: {0}x{1}", lWidth, lHeight))

                If lWidth <> 0 AndAlso lHeight <> 0 Then

                    ' so we get rid of invalid artwork dim caused by gdi errors
                    If lWidth < My.Settings.LowResArtworkWidth Or lHeight < My.Settings.LowResArtworkHeight Then

                        'mListArtworkLowRes.Add(lWidth & "x" & lHeight & " for " & track.Location)
                        mListArtworkLowRes.Add(xt)
                        lowres = True

                    End If

                End If

            End If

        End If

        Return lowres

    End Function

    Private Sub sCheckTrackBPM(ByVal track As IITFileOrCDTrack)

        mfUpdateStatusBarText(String.Format("Checking BPM in ""{0}""", track.Name), secondary:=True)
        If 0 = track.BPM Then
            If mListTracksNoBPM.Contains(track.Location) = False Then
                mListTracksNoBPM.Add(track.Location)
            End If
        End If

    End Sub

    Private Sub sCheckTrackMetatag(ByVal track As IITFileOrCDTrack)

        Dim loc As String = String.Empty

        Try
            loc = track.Location
        Catch ex As Exception
            'oh
        End Try

        Try
            Dim f As TagLib.File = TagLib.File.Create(track.Location)

            Dim sbMeta As String = f.TagTypesOnDisk.ToString

            Dim id32_tag As TagLib.Id3v2.Tag = CType(f.GetTag(TagLib.TagTypes.Id3v2, True), TagLib.Id3v2.Tag)
            If id32_tag IsNot Nothing Then
                ' append which version if ID3v2 version
                sbMeta = sbMeta.Replace("Id3v2", String.Format("Id3v2 2.{0}", id32_tag.Version.ToString))
            End If

            Dim xt As New cXmlTrack(track, False)
            xt.MetaVersion = sbMeta
            'Dim msg As String = String.Format("{0} - {1}", track.Location, sbMeta)
            If mListTrackMetatags.Contains(xt) = False Then
                mListTrackMetatags.Add(xt)
            End If
        Catch ex As Exception
            msAppendWarnings(ex.Message + " while reading metatags for " + loc)
        End Try

    End Sub

    Private Sub sCheckTrack(ByVal track As IITFileOrCDTrack)

        ' Checks are done after editing the track

        '***************************
        ' CHECK FOR MISSING ARTWORK
        '***************************
        If chkCheckArtwork.CheckState = CheckState.Checked Then
            sCheckTrackArtwork(track)
        End If

        If chkCheckArtworkLowRes.Checked Then
            fCheckTrackArtworkIsLowRes(track)
        End If

        If chkCheckBPM.Checked Then
            sCheckTrackBPM(track)
        End If

        If chkCheckMetatag.Checked = True Then
            sCheckTrackMetatag(track)
        End If

        '***************************
        ' CHECK FOR ITUNES STANDARD
        '***************************
        If chkItunesStoreStandard.Checked Then
            sCheckTrackITSstandard(track)
        End If

        '***************************
        ' CHECK FOR MISSING TRACK #
        '***************************
        If chkCheckTrackNum.CheckState = CheckState.Checked Then
            sCheckTrackNumber(track)
        End If

        '***************************
        ' CHECK FOR MISSING LYRICS
        '***************************
        If chkCheckLyrics.CheckState = CheckState.Checked Then
            sCheckTrackLyrics(track)
        End If

    End Sub

    Public Function fGetStatsMaker() As cStatsMaker

        sUpdateSystemVariables()

        If mStatsMaker Is Nothing Then
            mStatsMaker = New cStatsMaker(bwApp, fGetLibraryParser)
            If bwApp.CancellationPending = False AndAlso mStatsMaker.MaxPlayedCount > 0 Then
                mfWriteObjectToFileBF(mStatsMaker, mStatsFilePath)
            End If
        End If

        Return mStatsMaker

    End Function

    Private Function fForceTags(ByVal frm As frmAddNewFiles, ByVal filePath As String, ByVal id As Integer) As Integer

        ' FILES WITH ID3V1 TAGS WILL BE OVERWRITTEN AS ID3V2 2.4 BY TAGLIB 2.0.2.0 
        ' THE GENRE WILL BE CONVERTED TO AN INDEX

        Try
            Dim f As TagLib.File = TagLib.File.Create(filePath)
            ' 5.13.0.1 Genre tag in ID3v1 would be overwritten with Genre index in ID3v2 2.4. Tags via TagLib# are saved in ID3v2 2.3 to overcome this problem.
            TagLib.Id3v2.Tag.DefaultVersion = 3
            TagLib.Id3v2.Tag.ForceDefaultVersion = True

            ''Console.Writeline(f.TagTypesOnDisk)

            If frm.chkOverwriteTags.Checked Then

                If frm.chkAlbumArtist.Checked = True AndAlso frm.cboAlbumArtist.Text <> String.Empty Then
                    f.Tag.AlbumArtists = New String() {frm.cboAlbumArtist.Text}
                End If

                If frm.chkArtist.Checked = True AndAlso frm.cboArtist.Text <> String.Empty Then
                    f.Tag.Performers = New String() {frm.cboArtist.Text}
                End If

                If frm.chkAlbum.Checked = True AndAlso frm.txtAlbum.Text <> String.Empty Then
                    f.Tag.Album = frm.txtAlbum.Text.Trim
                End If

                If frm.chkDisc.Checked Then
                    f.Tag.Disc = CUInt(frm.nudDiscNumber.Value)
                    f.Tag.DiscCount = CUInt(frm.nudDiscCount.Value)
                End If

                If frm.chkGenre.Checked Then
                    f.Tag.Genres = New String() {frm.cboGenre.Text}
                End If

                If frm.chkYear.Checked Then
                    f.Tag.Year = CUInt(frm.nudYear.Value)
                End If

            Else
                ' force them to id3 v2.3 anyway
                f.Tag.Album = f.Tag.Album
                'f.Tag.AlbumArtists = f.Tag.AlbumArtists

            End If

            If My.Settings.ForceTagsAddNew Then
                If f.Tag.Track = 0 Then
                    ' if no tag at all (id3v2 2.3, 2.4) are found then set it to id
                    f.Tag.Track = CUInt(id)
                Else
                    f.Tag.Track = f.Tag.Track ' http://www.hydrogenaudio.org/forums/index.php?s=&showtopic=51708&view=findpost&p=588265
                    f.Tag.Title = f.Tag.Title
                    f.Tag.Genres = f.Tag.Genres
                    f.Tag.Year = f.Tag.Year
                    f.Tag.Disc = f.Tag.Disc
                End If
            End If

            If f.Tag.Track = 1 AndAlso My.Settings.FilesAddArtworkClear Then
                If f.Tag.Pictures.Length > 0 Then
                    f.Tag.Pictures = Nothing
                    f.Save()
                End If
            End If

            f.Save()

        Catch ex As Exception

            'sWriteDebugLog(ex.Message + " for " + filePath)
            msAppendWarnings(ex.Message + " for " + filePath)

        End Try

        Return id

    End Function


    Private Sub sEditDeleteMultipleArtwork(ByVal track As IITFileOrCDTrack)

        Dim artwork As IITArtworkCollection = track.Artwork

        If artwork.Count > 1 Then

            mListTracksMultipleArtwork.Add(track.Location)

            Dim fiTrack As New FileInfo(track.Location)
            Dim wasReadOnly As Boolean = fiTrack.IsReadOnly
            If wasReadOnly Then fiTrack.IsReadOnly = False

            For i As Integer = 2 To artwork.Count
                If artwork.Item(i) IsNot Nothing Then
                    artwork.Item(i).Delete()
                End If
            Next

            If wasReadOnly = True Then fiTrack.IsReadOnly = True

        End If

    End Sub


    Private Sub ssAdjustTrackRating(ByVal track As IITFileOrCDTrack)

        Dim ra As New cRatingAdjuster(mRatingWeights, fGetStatsMaker.RatingParameters)
        Dim r As Integer = CInt(ra.fGetRating(New cXmlTrack(track, False)))

        track.Rating = r

        If r > 0 Then
            mListRatings.Add(String.Format("{0} for {1} - {2}", r.ToString("000"), track.Artist, track.Name))
            msAppendDebug(String.Format("Rating adjusted to {0} for {1} - {2}", track.Rating.ToString("000"), track.Artist, track.Name))
        End If

    End Sub

    Private Sub sEditLibrary(ByVal lDisc As cInfoDisc, ByVal track As IITFileOrCDTrack)

        If chkLibraryAdjustRatings.Checked = True Then
            '* Adjust Rating 
            ssAdjustTrackRating(track)
        End If

        If chkRatingsImportPOPM.Checked Or chkPlayedCountImportPCNT.Checked Then

            Dim popm As FramePOPM = mTagLibJobs.mfGetPOPM(track.Location)

            If chkRatingsImportPOPM.Checked Then
                '*Import Rating from POPM
                track.Rating = popm.Rating
            End If

            If chkPlayedCountImportPCNT.Checked Then
                track.PlayedCount = popm.PlayedCount
            End If

        End If

    End Sub

    Private Function fRemoveNullChar(ByVal track As IITFileOrCDTrack) As Boolean

        Dim success As Boolean = False

        Try

            Dim fPath As String = track.Location

            track.Name = track.Name + " "
            track.Name = track.Name.Trim.Replace("�", "")

            track.Artist = track.Artist + " "
            track.Artist = track.Artist.Trim.Replace("�", "")

            track.Album = track.Album + " "
            track.Album = track.Album.Trim.Replace("�", "")

            track.AlbumArtist = track.AlbumArtist + " "
            track.AlbumArtist = track.AlbumArtist.Trim.Replace("�", "")

            track.UpdateInfoFromFile()

            success = True

        Catch ex As Exception
            msAppendWarnings(ex.Message + " while removing null character")
        End Try

        Return success

    End Function

    Private Function fEditRemoveLowResArtwork(ByVal track As IITFileOrCDTrack) As Boolean

        Dim succ As Boolean = False

        Try

            If track.Artwork.Count > 0 Then
                If fCheckTrackArtworkIsLowRes(track) Then
                    track.Artwork.Item(1).Delete()
                    msAppendDebug("Removed Low Resolution artwork in " & track.Location)
                    succ = True
                End If

            End If

        Catch ex As Exception
            msAppendWarnings(ex.Message + " while removing low resolution artwork.")
        End Try

        Return succ

    End Function

    Private Sub sEditConvertToJPG(ByVal track As IITFileOrCDTrack)

        Try

            If track.Artwork.Count > 0 Then
                For i As Integer = 1 To track.Artwork.Count

                    Dim artwork As IITArtwork = track.Artwork.Item(i)
                    If artwork.Format <> ITArtworkFormat.ITArtworkFormatJPEG Then
                        Dim tempPath As String = mfGetTempArtworkCachePath("NonJPG" + i.ToString, track)
                        artwork.SaveArtworkToFile(tempPath)
                        Dim img As Image = Image.FromFile(tempPath)
                        ' save the jpg convert
                        Dim jpgPath As String = tempPath + ".jpg"
                        img.Save(jpgPath, System.Drawing.Imaging.ImageFormat.Jpeg)
                        mfUpdateStatusBarText(String.Format("Converting Artwork to JPG for ""{0}""", track.Name), True)
                        artwork.SetArtworkFromFile(jpgPath)
                        mListTracksArtworkConverted.Add(track.Location)
                    End If

                Next

                msAppendDebug("Converted Track Artwork to JPG for " & track.Location)

            End If

        Catch ex As Exception
            msAppendWarnings(ex.Message + " while converting Artwork to JPG")
        End Try


    End Sub

    Private Sub sEditTrackTagLib(ByVal track As IITFileOrCDTrack)

        Dim tl As New cTrackEditor

        If tl.UseFile(track.Location) Then

            Dim bEdited As Boolean = False

            If My.Settings.ArtworkSetFrontCover Then
                bEdited = tl.SetArtworkTypeFront()
            End If

            If My.Settings.WritePOPM Then
                bEdited = tl.SetFramePOPM(track.PlayedCount, track.Rating)
                ' mfUpdateStatusBarText(String.Format("Setting PlayedCount|Rating for {0} as {1}|{2}", track.Name, track.PlayedCount, track.Rating), True)
            End If

            If My.Settings.WritePCNT Then
                bEdited = tl.SetFramePCNT(track.PlayedCount)
            End If

            If bEdited Then
                tl.Save()
            End If
        End If

    End Sub

    Private Sub sEditTrack(ByVal lDisc As cInfoDisc, ByVal track As IITFileOrCDTrack)

        Dim locTrack As String = "dead track"

        Try

            locTrack = track.Location

            Dim fiTrack As New FileInfo(track.Location)
            Dim wasReadOnly As Boolean = fiTrack.IsReadOnly

            ''**********************************
            ''* Retain Modified Date
            ''**********************************
            Dim dMod As Date = fiTrack.LastWriteTime

            ''**********************************
            ''* Remove Read-Only
            ''**********************************
            If fiTrack.IsReadOnly Then
                fiTrack.IsReadOnly = False
                msAppendDebug(String.Format("Cleared {0} Read-Only flag", fiTrack.FullName))
            End If

            ''********************************************
            ''* Write TrackCount, DiscCount and DiscNumber
            ''********************************************
            If chkEditTrackCountEtc.Checked Then
                fWriteTrackCountEtc(track, lDisc)
            End If

            ''**********************************
            ''* Remove Double Spaces
            ''**********************************
            If My.Settings.RemoveDoubleSpaces = True Then
                If track.Artist IsNot Nothing Then
                    If track.Artist.Trim <> track.Artist Then
                        track.Artist = track.Artist.Trim
                    End If
                    track.Artist = mfGetFixedSpaces(track.Artist)
                End If
                If track.Album IsNot Nothing Then
                    If track.Album.Trim <> track.Album Then
                        track.Album = track.Album.Trim
                    End If
                    track.Album = mfGetFixedSpaces(track.Album)
                End If
                If track.Name IsNot Nothing Then
                    If track.Name.Trim <> track.Name Then
                        track.Name = track.Name.Trim
                    End If
                    track.Name = mfGetFixedSpaces(track.Name)
                End If
            End If

            '***************************
            ' COPY ARTIST TO ALBUMARTIST 
            '***************************
            If chkEditCopyArtistToAlbumArtist.Checked Then
                sTracksAlbumArtist(track, lDisc)
            End If

            '*********************************
            '* Add Artwork.jpg to Empty Tracks
            '*********************************
            If lDisc.ArtworkSource IsNot Nothing Then
                If chkImportArtwork.Checked Then
                    sEditTrackImportArtwork(track, lDisc.ArtworkSource)
                End If
            End If

            '*********************************
            '* Remove Low-res Artwork
            '*********************************
            If chkRemoveLowResArtwork.Checked Then
                fEditRemoveLowResArtwork(track)
            End If

            '*********************************
            '* Delete Multiple Artwork
            '*********************************
            If chkMultiArtworkRemove.Checked = True Then
                sEditDeleteMultipleArtwork(track)
            End If

            '************************
            '* Convert Artwork to JPG
            '************************
            If chkConvertArtworkJPG.Checked Then
                sEditConvertToJPG(track)
            End If

            '************************
            '* TagLib# Tasks
            '************************
            sEditTrackTagLib(track)

            '********************
            '* Update Track Genre
            '********************
            sTracksGenre(track, lDisc)

            '********************
            '* Update Track EQ
            '********************
            If track.EQ = String.Empty AndAlso chkEditEQbyGenre.Checked Then
                mLibraryTasks.sEditEQ(track)
            End If

            '****************************
            '* Match Year to Album Year
            '****************************
            If My.Settings.FillYear Then
                If track.Year = 0 AndAlso lDisc.Year <> 0 Then
                    track.Year = lDisc.Year
                End If
            End If

            If chkRemoveNull.Checked Then
                fRemoveNullChar(track)
            End If

            ''*******************************
            ''* Search and Add missing Lyrics
            ''*******************************
            If My.Settings.LyricsImport = True Then
                fLyricsImport(track)
            End If

            ''**********************************
            ''* Retain Modified Date
            ''**********************************
            If My.Settings.ModifiedDateRetain = True Then
                File.SetLastWriteTime(fiTrack.FullName, dMod)
            End If

            If wasReadOnly Then
                fiTrack.IsReadOnly = True
                msAppendDebug(String.Format("Undo clear {0} Read-Only flag", fiTrack.FullName))
            End If

            '***********************
            '* MAKE READ-ONLY OR NOT - do last
            '************************
            If chkWinMakeReadOnly.Checked Then
                sMakeReadOnlyOrNot(track)
            End If

        Catch ex As Exception
            msAppendDebug(String.Format("{0} while editing track {1}", ex.Message, locTrack))
        End Try

    End Sub

    Private Sub sTracksGenre(ByVal track As IITFileOrCDTrack, ByVal lDisc As cInfoDisc)

        Try
            Dim bEdited As Boolean = False
            If My.Settings.WriteGenre Then

                '****************************
                '* Add Genre from Last.fm
                '****************************
                If String.Empty = track.Genre Or My.Settings.OverwriteGenre = True Then
                    If String.IsNullOrEmpty(lDisc.Genre) = False Then
                        track.Genre = lDisc.Genre
                        bEdited = True
                    End If
                End If

            Else

                '****************************
                '* Match Genre to Album Genre
                '****************************
                If My.Settings.FillGenre Then
                    If track.Genre = String.Empty AndAlso lDisc.Genre IsNot Nothing Then
                        track.Genre = lDisc.Genre
                        bEdited = True
                    End If
                End If

            End If

            If bEdited Then
                mfUpdateStatusBarText(String.Format("Edited Genre tag in ""{0}""", track.Name), True)
            End If

        Catch ex As Exception
            msAppendWarnings(ex.Message + " while writing Genre")
        End Try

    End Sub

    Private Sub sEditFileSystem(ByVal lDisc As cInfoDisc)

        '*********************************
        '* Copy Folder.jpg to Album Folder
        '*********************************
        If chkWinExportArtwork.Checked = True Then
            For Each track As IITFileOrCDTrack In lDisc.Tracks
                If track.Artwork.Count > 0 Then
                    If fArtworkExport(track, lDisc.ArtworkSource) = True Then
                        msAppendDebug("Export Artwork Pass 1 Finished.")
                    End If
                    Exit For
                End If
            Next
        End If

        If chkWinExportPlaylist.Checked = True Then
            Dim pw As New cPlaylistWriter(lDisc)
            Dim where As String = ""
            Select Case My.Settings.PlaylistExt
                Case Is = ".m3u"
                    where = Path.Combine(lDisc.Location, mfGetFileNameFromPattern(My.Settings.PlaylistFileNamePattern, lDisc.FirstTrack)) + ".m3u"
                    pw.fCreatePlaylistM3U(where, True)
                Case Is = ".xspf"
                    where = Path.Combine(lDisc.Location, mfGetFileNameFromPattern(My.Settings.PlaylistFileNamePattern, lDisc.FirstTrack)) + ".xspf"
                    pw.fCreatePlaylisXSPF(where)
            End Select
            msAppendDebug(String.Format("Created Playlist as {0}", where))
        End If

        If chkExportIndex.Checked = True Then

            Dim tgApp As New TreeGUI.cAdapter
            tgApp.GetConfig.IndexFileName = mfGetFileNameFromPattern(My.Settings.IndexFileNamePattern, lDisc.FirstTrack)
            tgApp.GetConfig.IndexFileExtension = My.Settings.IndexFileExt
            tgApp.GetConfig.FolderList.Add(lDisc.Location)
            tgApp.GetConfig.CssFilePath = My.Settings.IndexCSS
            tgApp.GetConfig.CollapseFolders = False

            Dim tnl As New TreeGUI.cTreeNetLib(tgApp)
            tnl.IndexNow(TreeGUI.cAdapter.IndexingMode.IN_EACH_DIRECTORY)

            msAppendDebug("Exported Index to " & tgApp.GetConfig.GetIndexFilePaths(0))

        End If

    End Sub


    Private Function fAtLeastOneEditTrack() As Boolean

        Dim atleastOneEditTrack As Boolean = False

        For Each cb As Control In tpEditTracks.Controls
            If TypeOf cb Is CheckBox Then
                atleastOneEditTrack = atleastOneEditTrack Or CType(cb, CheckBox).Checked
                If atleastOneEditTrack = True Then Exit For
            End If
        Next

        ' Now check if TagLib# Tasks are turned on
        atleastOneEditTrack = atleastOneEditTrack Or My.Settings.ArtworkSetFrontCover

        Return atleastOneEditTrack

    End Function

    Private Function fAtLeastOneEditFileSystem() As Boolean

        Dim atleastOneEditFS As Boolean = False

        For Each cb As Control In tpFileSystem.Controls
            If TypeOf cb Is CheckBox Then
                atleastOneEditFS = atleastOneEditFS Or CType(cb, CheckBox).Checked
                If atleastOneEditFS = True Then Exit For
            End If
        Next

        Return atleastOneEditFS

    End Function

    Private Function fModeIsRemoveDeadForeignTracks() As Boolean

        Return (fAtLeastOneCheck() = False AndAlso _
        fAtLeastOneEditFileSystem() = False AndAlso _
        fAtLeastOneEditTrack() = False AndAlso _
        chkDeleteNonMusicFolderTracks.Checked = True) Or _
        (fAtLeastOneCheck() = False AndAlso _
        fAtLeastOneEditFileSystem() = False AndAlso _
        fAtLeastOneEditTrack() = False AndAlso _
        chkDeleteTracksNotInHDD.Checked = True)

    End Function

    Private Function fAtLeastOneCheck() As Boolean

        Dim atLeastOneCheck As Boolean = False

        For Each cb As Control In tpChecks.Controls
            If TypeOf cb Is CheckBox Then
                atLeastOneCheck = atLeastOneCheck Or CType(cb, CheckBox).Checked
                If atLeastOneCheck = True Then Exit For
            End If
        Next
        Return atLeastOneCheck

    End Function

    Private Function fAtLeastOneEditLibrary() As Boolean

        Dim atLeastOneEditLibrary As Boolean = False

        For Each cb As Control In tpEditLibrary.Controls
            If TypeOf cb Is CheckBox Then
                atLeastOneEditLibrary = atLeastOneEditLibrary Or CType(cb, CheckBox).Checked
                If atLeastOneEditLibrary = True Then Exit For
            End If
        Next

        Return atLeastOneEditLibrary

    End Function

    Private Sub ssValidateDisc(ByVal lDisc As cInfoDisc, Optional ByVal bValOpt As cValidatorOptions = Nothing)

        Try

            msAppendDebug(String.Format("Validating Disc: {0} in {1}", lDisc.Name, lDisc.Location))
            msAppendDebug(String.Format("Checks? {0}, Tracks? {1}, Library? {2}, File System? {3}", _
                                        mValModes.Checks, mValModes.Tracks, mValModes.Library, mValModes.FileSystem))

            If lDisc IsNot Nothing AndAlso String.IsNullOrEmpty(lDisc.Location) = False Then

                '''''''''''''''''''''''
                ' Initialize Settings
                '''''''''''''''''''''''

                '' Fill AlbumArtist for the Disc
                Dim aaf As New cDiscArtistFinder(lDisc)
                lDisc = aaf.UpdateDisc

                ''Console.Writeline(lDisc.Artist)

                '' Fill Genre from Last.fm
                If String.IsNullOrEmpty(lDisc.Genre) Or My.Settings.OverwriteGenre = True Then
                    If mValModes.Tracks = True AndAlso My.Settings.WriteGenre Then
                        Dim lastfmgenre As New cLastFmGenreTagger(lDisc, bwApp)
                        If String.IsNullOrEmpty(lastfmgenre.Genre) = False Then
                            lDisc.Genre = lastfmgenre.Genre
                        End If
                    End If
                End If

                '**********************
                '* Populate Track Count Etc
                '**********************
                If mValModes.Tracks = True AndAlso chkEditTrackCountEtc.Checked Then
                    fFillTrackCountEtc(lDisc)
                End If

                '' setup advanced disc validation
                Dim artworkSrc As cArtworkSource = Nothing

                bwApp.ReportProgress(ProgressType.UPDATE_TRACKS_PROGRESS_BAR_MAX, lDisc.Tracks.Count)

                If bValOpt IsNot Nothing Then
                    ' options are available
                    If bValOpt.REMOVE_ARTWORK = True Then
                        For Each track As IITFileOrCDTrack In lDisc.Tracks
                            If track.Artwork.Count > 0 Then
                                bwApp.ReportProgress(ProgressType.DELETE_ARTWORK, track.Name)
                                track.Artwork.Item(1).Delete()
                            End If
                        Next
                    End If
                    artworkSrc = bValOpt.ARTWORK_SOURCE
                End If

                Dim pBarMax As Integer = lDisc.Tracks.Count
                bwApp.ReportProgress(ProgressType.UPDATE_TRACKS_PROGRESS_BAR_MAX, pBarMax)

                Try
                    lDisc.Tracks.Sort(New cIITFileOrCDTrackComparer)
                    lDisc.Tracks.Reverse()
                Catch ex As Exception
                    ' 5.17.0.1 Fixed crash due to Failed to compare two elements in the array [Wayne]
                End Try

                If mValModes.Tracks = True Then
                    '***********************
                    '* UPDATE INFO FROM FILE - do this first
                    '************************
                    For Each track As IITFileOrCDTrack In lDisc.Tracks
                        If My.Settings.UpdateInfoFromFile = True Then
                            mfUpdateInfoFromFile(track)
                        End If
                    Next
                End If

                If artworkSrc Is Nothing Then
                    ' this is what will happen when advanced validation is not active
                    If (mValModes.FileSystem = True And chkWinExportArtwork.Checked = True) Or _
                       (mValModes.Tracks = True And chkImportArtwork.Checked = True) Then
                        artworkSrc = ffGetArtworkSource(lDisc)
                    End If
                End If
                lDisc.ArtworkSource = artworkSrc

                '''''''''''''
                ' Start Jobs
                '''''''''''''

                'Dim fTrack As IITFileOrCDTrack = lDisc.FirstTrack
                'Dim artworkPath As String = mfGetArtworkForTrack(fTrack)
                'If File.Exists(artworkPath) Then
                '    Dim xt As cXmlTrack = fGetLibraryParser.fFindXmlTrack(fTrack)
                '    If xt IsNot Nothing Then
                '        Dim itc As New cITCgen(fGetLibraryParser.LibraryPersistantID, artworkPath)
                '        itc.Save(xt.PersistantID)
                '    End If
                'End If

                '***************************
                '* Capitalize Words per Disc
                '***************************
                If (mCurrJobTypeMain <> cBwJob.JobType.ADJUST_RATINGS AndAlso My.Settings.CapitalizeNewTrack = True) Or _
                 (My.Settings.CapitalizeLowerCaseAuto = True AndAlso lDisc.FirstTrack.Name.ToLower = lDisc.FirstTrack.Name Or _
                  My.Settings.CapitalizeLowerCaseAuto = True AndAlso lDisc.FirstTrack.Name.ToUpper = lDisc.FirstTrack.Name) Then
                    ssCapitalizeWord(lDisc.Tracks)
                End If

                ''*******************************************
                ''* Edit File System per Album
                ''*******************************************
                If mValModes.FileSystem = True AndAlso fAtLeastOneEditFileSystem() = True Then
                    sEditFileSystem(lDisc)
                End If

                For Each track As IITFileOrCDTrack In lDisc.Tracks

                    mfUpdateStatusBarText(String.Format("Validating {0} ""{1}""", track.TrackNumber.ToString, track.Name), secondary:=True)

                    sPausePendingCheck()
                    If bwApp.CancellationPending Then
                        Exit For
                    End If

                    mTracksChecked += 1

                    If track.Location IsNot Nothing Then

                        ' This has to be called before editing tracks
                        ' because as soon as iTSfv detects an iTunes downloaded artwork 
                        ' it is going to embed that immediately 
                        If chkCheckEmbeddedArtwork.Checked Then
                            sCheckTrackEmbeddedArtwork(track)
                        End If

                        If mValModes.Tracks = True AndAlso fAtLeastOneEditTrack() = True Then
                            '*******************
                            '* Edit Track Tags
                            '*******************
                            Call sEditTrack(lDisc, track)
                        End If

                        If mValModes.Library = True AndAlso fAtLeastOneEditLibrary() = True Then
                            '*******************
                            '* Edit Library.xml
                            '*******************
                            sEditLibrary(lDisc, track)
                        End If

                        '****************************************
                        '* Edit File System per Track 
                        '****************************************
                        If mValModes.FileSystem = True AndAlso My.Settings.ExportLyrics Then
                            fLyricsExport(track)
                        End If

                        If mValModes.Checks = True AndAlso fAtLeastOneCheck() = True Then
                            '*******************
                            '* Check Tracks - AFTER editing tracks
                            '******************
                            Call sCheckTrack(track)
                        End If

                    End If

                    mProgressTracksCurrent += 1
                    'bwApp.ReportProgress(ProgressType.INCREMENT_TRACK_PROGRESS)

                Next

                '*************************************************************************
                '* Copy Folder.jpg to Album Folder (Pass 2) - why? after importing artwork
                '*************************************************************************
                If mValModes.FileSystem = True AndAlso lDisc.ArtworkSource IsNot Nothing Then
                    If chkImportArtwork.Checked AndAlso chkWinExportArtwork.Checked AndAlso lDisc.ArtworkSource.ArtworkPath <> String.Empty Then
                        If fArtworkExport(lDisc.Tracks.Item(0), lDisc.ArtworkSource) = True Then
                            msAppendDebug("Export Artwork Pass 2 Finished.")
                        End If
                    End If
                End If

                ''******************************************************************
                '' after all the exporting, fill the list of folders without artwork 
                ''******************************************************************
                If mValModes.Checks = True AndAlso chkCheckFoldersWithoutArtwork.Checked Then
                    Dim jpgCustom As String = mfGetArtworkForTrack(lDisc.FirstTrack, scanAll:=False)
                    If File.Exists(jpgCustom) = False Then
                        If mListFoldersNoArtwork.Contains(jpgCustom) = False Then
                            mListFoldersNoArtwork.Add(String.Format("{0} - No {1}", Path.GetDirectoryName(jpgCustom), Path.GetFileName(jpgCustom)))
                        End If
                    End If
                End If

                '**********************
                '* Fix Artist Thumbnail
                '**********************
                If mValModes.FileSystem = True Then
                    If (My.Settings.FixArtistThumbnailAlways = True AndAlso mCurrJobTypeMain = JobType.VALIDATE_TRACKS_SELECTED) Or _
                               (chkVistaThumbnailFix.Checked = True) Then
                        ssFixArtistThumbnail(lDisc)
                    End If
                End If

                Dim msg As String = String.Format("Validated {0}", lDisc.AlbumName)
                msAppendDebug(".") ' ready for the next album

                bwApp.ReportProgress(ProgressType.READY, msg)

            End If ' lDisc not nothing

        Catch ex As Exception
            msAppendDebug(ex.Message + " while validating disc")
        End Try

    End Sub

    Private Sub ssFixArtistThumbnail(ByVal lDisc As cInfoDisc)

        '' Fix Artist folder

        If String.IsNullOrEmpty(lDisc.Location) = False Then

            Dim dirArtist As String = Path.GetDirectoryName(lDisc.Location)

            If dirArtist IsNot Nothing Then

                If Directory.Exists(dirArtist) Then

                    Dim di As New DirectoryInfo(dirArtist)
                    Dim lastEdit As Date = di.LastWriteTime

                    Dim tempFile As String = dirArtist + "\itsfv.temp"
                    Using sw As New StreamWriter(tempFile, False)
                        sw.WriteLine("itsfv")
                        msAppendDebug(String.Format("Fixed folder icon in {0}", dirArtist))
                    End Using

                    bwApp.ReportProgress(ProgressType.UPDATE_THUMBNAIL_IN_ARTIST_FOLDER, dirArtist)

                    File.Delete(tempFile)

                    di = New DirectoryInfo(dirArtist)
                    di.LastWriteTime = lastEdit

                End If

            End If

        End If

    End Sub

    Dim mTracksChecked As Integer = 0

    Private Sub sUpdateSystemVariables()

        mRatingWeights.PlayedCount = CType(My.Settings.WeightPlayedCount, Short)
        mRatingWeights.SkippedCount = CType(My.Settings.WeightSkippedCount, Short)
        mRatingWeights.LastPlayed = CType(My.Settings.WeightLastPlayed, Short)
        mRatingWeights.DateAdded = CType(My.Settings.WeightDateAdded, Short)
        mRatingWeights.LongSongDuration = CInt(My.Settings.LongSongDuration) * 60
        mRatingWeights.ScaleDuration = My.Settings.ScaleDuration
        mRatingWeights.ReduceScaleLongTracks = My.Settings.ReduceScaleLongTracks

        mFolderJpgMinSize = My.Settings.FolderJpgMinSize
        mIncludePodcasts = My.Settings.IncludePodcasts

    End Sub

    Private Sub ssValidateAlbums(ByVal untilAlbum As Integer)

        sBwAppLoadDiscsToAlbumBrowser()

        mBooTableArtworkResEdited = False

        bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, mTableDiscs.Count - untilAlbum)

        mTracksChecked = 0

        sUpdateSystemVariables() 'update variables

        ' fill albums in reverse order i.e. last album first
        For i As Integer = mTableDiscs.Count - 1 To untilAlbum Step -1

            Dim albumTitle As String = mListDiscKeys.Item(i).ToString
            Dim lDisc As New cInfoDisc

            lDisc = CType(mTableDiscs.Item(albumTitle), cInfoDisc)
            lDisc.DiscID = mTableDiscs.Count - i

            mProgressDiscsMax = mTableDiscs.Count - untilAlbum
            mProgressDiscsCurrent = lDisc.DiscID
            bwApp.ReportProgress(ProgressType.VALIDATING_DISC_IN_ITUNES_LIBRARY, lDisc)

            ssValidateDisc(lDisc) ' validating albums

            sPausePendingCheck()
            If bwApp.CancellationPending Then
                Exit For
            End If

        Next

        fWriteArtworkCache()

    End Sub

    Private Function fWriteArtworkCache() As Boolean

        Dim succ As Boolean = False

        If fBooUseCache() = True Then
            If bwApp.CancellationPending = False And mBooTableArtworkResEdited = True Then
                bwApp.ReportProgress(ProgressType.WRITE_ARTWORK_CACHE)
                'sWriteTableToFile(mTableArtworkRes, mFilePathArtworkRes)
                succ = mfWriteObjectToFileBF(mTableArtworkRes, mFilePathArtworkRes)
            End If
        End If

        Return succ

    End Function

    Private Sub sWriteLogRatings()

        If mListRatings.Count > 0 Then

            mListRatings.Sort()
            mListRatings.Reverse()
            mListRatings.Add(String.Empty)
            mListRatings.Add(mRatingWeights.ToString)
            mListRatings.Add(mAdapter.mStatsMaker.RatingParameters.ToString)

            msWriteListToFile(New LogData(mFilePathRatingsAdjusted, mListRatings), sort:=False)

        End If

    End Sub

    Private Sub sWriteListToM3U(ByVal log As LogData)

        Dim pw As cPlaylistWriter = Nothing

        If log.PathList IsNot Nothing AndAlso log.PathList.Count > 0 Then

            If File.Exists(log.PathList(0)) Then
                pw = New cPlaylistWriter(log.PathList)
            End If

        ElseIf log.TrackList IsNot Nothing AndAlso log.TrackList.Count > 0 Then

            If log.LogType = LogDataType.GENERAL Then
                pw = New cPlaylistWriter(log.TrackList)
            End If

        End If

        If pw IsNot Nothing Then

            If pw.fCreatePlaylistM3U(log.Destination, False) = True Then
                msAppendDebug("M3U saved in " + log.Destination)
            End If

        End If

    End Sub

    Private Function sWritePlaylist(ByVal log As LogData) As Boolean

        Dim success As Boolean = True

        If log.PathList IsNot Nothing AndAlso log.PathList.Count > 0 Then

            If File.Exists(log.PathList.Item(0)) Then

                Dim uplName As String = Path.GetFileNameWithoutExtension(log.Destination)

                Try
                    Dim foundAppPlaylist As Boolean = False
                    Dim plApp As IITUserPlaylist = Nothing
                    For Each pl As IITPlaylist In mItunesApp.LibrarySource.Playlists
                        If pl.Name.Equals(APP_ABBR_NAME_IT) Then
                            plApp = CType(pl, IITUserPlaylist)
                            foundAppPlaylist = True
                            Exit For
                        End If
                    Next
                    If foundAppPlaylist = False Then
                        plApp = CType(mItunesApp.CreateFolder(APP_ABBR_NAME_IT), IITUserPlaylist)
                    End If

                    Dim upl As IITUserPlaylist = Nothing
                    Dim foundUpl As Boolean = False
                    For Each pl As IITPlaylist In mItunesApp.LibrarySource.Playlists
                        If pl.Name.Equals(uplName) Then
                            upl = CType(pl, IITUserPlaylist)
                            upl.Delete()
                            'foundUpl = True
                            Exit For
                        End If
                    Next

                    If foundUpl = False Then
                        upl = CType(plApp.CreatePlaylist(uplName), IITUserPlaylist)
                    End If

                    upl.AddFiles(log.PathList.ToArray)

                    msAppendDebug(String.Format("Playlist saved as {0} in iTunes", upl.Name))

                Catch ex As Exception

                    success = False
                    msAppendWarnings(ex.Message + " while creating iTunes Playlist for " + uplName)

                End Try

            End If

            Return success

        ElseIf log.TrackList IsNot Nothing AndAlso log.TrackList.Count > 0 Then

            Select Case log.LogType
                Case LogDataType.META_TAG_VERSIONS
                    ' TODO: Create iTunes Playlists based on Metatag Versions

                Case LogDataType.ARTWORK_RESOLUTIONS

                    log.PathList = New List(Of String)
                    For Each xt As cXmlTrack In log.TrackList
                        log.PathList.Add(xt.Location)
                    Next
                    sWritePlaylist(log)

            End Select

        End If

    End Function

    Private Sub sWriteLogFiles()

        sWriteLogRatings()

        Dim logs As New List(Of LogData)
        logs.Add(New LogData(mFilePathMultipleArtwork, mListTracksMultipleArtwork))
        logs.Add(New LogData(mFilePathTracksEditedByAlbumBrowser, mListTracksCountUpdated))
        logs.Add(New LogData(mFilePathArtworkAdded, mListArtworkAdded))
        logs.Add(New LogData(mFilePathArtworkConverted, mListTracksArtworkConverted))
        logs.Add(New LogData(mFilePathEmbeddedArtwork, mListEmbeddedArtwork))
        logs.Add(New LogData(mFilePathTrackMetatags, mListTrackMetatags, LogDataType.META_TAG_VERSIONS))
        logs.Add(New LogData(mFilePathListArtworkRes, mListArtworkLowRes, LogDataType.ARTWORK_RESOLUTIONS))
        logs.Add(New LogData(mFilePathLyricsAdded, mListLyricsAdded))
        logs.Add(New LogData(mFilePathNoArtwork, mListNoArtwork))
        logs.Add(New LogData(mFilePathTracksNoBPM, mListTracksNoBPM))
        logs.Add(New LogData(mFilePathNoLyicsTracks, mListMissingLyrics))
        logs.Add(New LogData(mFilePathNonItsTracks, mListTracksNonITSstandard))
        logs.Add(New LogData(mFilePathTagsRefreshed, mListTagsRefreshed))
        logs.Add(New LogData(mFilePathDuplicateTracks, mListDuplicateTracks))

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' Log to Text LAST because the lists are cleared after logging
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If My.Settings.LogToM3u Then

            For Each log As LogData In logs
                sWriteListToM3U(log)
            Next

        End If

        ''''''''''''''''''''''''''''''''''''''''''''''''''''
        '' Create Validation Results in iTunes Playlists
        '''''''''''''''''''''''''''''''''''''''''''''''''''
        If My.Settings.CreateValidationPlaylists Then

            bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, logs.Count)
            For Each log As LogData In logs
                bwApp.ReportProgress(ProgressType.SAVE_PLAYLIST, Path.GetFileNameWithoutExtension(log.Destination))
                sWritePlaylist(log)
            Next
            bwApp.ReportProgress(ProgressType.READY)

        End If

        If My.Settings.LogToText Then

            For Each log As LogData In logs
                msWriteListToFile(log)
            Next

            '' These logs are TXT only
            msWriteListToFile(New LogData(mFilePathFoldersNoArtwork, mListFoldersNoArtwork))
            msWriteListToFile(New LogData(mFilePathFoldersOneFile, mListFoldersOneFile))
            msWriteListToFile(New LogData(mFilePathFoldersNoAudio, mListFoldersNoAudio))

            msWriteListToFile(New LogData(mFilePathAlbumsInconsecutiveTracks, mListAlbumsInconsecutiveTracks))

        End If

    End Sub

    Private Sub sBwAppValidateDisc()

        Dim albumTitle As String = mCurrentAlbum
        Dim lDisc As New cInfoDisc
        lDisc = CType(mTableDiscs.Item(albumTitle), cInfoDisc)
        ssValidateDisc(lDisc) ' validating album

    End Sub

    Private Sub sBwAppValidateDiscAdv()

        Dim valOpt As cValidatorOptions = CType(mCurrJob.TaskData, cValidatorOptions)

        ' we need to replace artwork 
        Dim albumTitle As String = mCurrentAlbum
        Dim lDisc As New cInfoDisc
        lDisc = CType(mTableDiscs.Item(albumTitle), cInfoDisc)

        ssValidateDisc(lDisc, bValOpt:=valOpt)

    End Sub

    Private Sub sBwAppAdjustRatings()

        'For j As Integer = 0 To mXmlLibParser.TrackCollection.Count - 1
        '    Dim xt As cXmlTrack = mXmlLibParser.TrackCollection(j)
        '    If xt.TrackType = TrackTypeXML.FILE Then
        '        Dim track As IITTrack = mMainLibraryTracks.Item(xt.Index)
        '        If track.Kind = ITTrackKind.ITTrackKindFile Then
        '            If xt.Location <> CType(track, IITFileOrCDTrack).Location Then
        '                Console.WriteLine(xt.Location + Environment.NewLine + CType(track, IITFileOrCDTrack).Location)
        '            End If
        '        End If
        '    End If

        'Next

        bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, mMainLibraryTracks.Count)

        Dim count As Integer = 1

        For i As Integer = mMainLibraryTracks.Count To 1 Step -1

            count += 1

            Dim track As IITTrack = mMainLibraryTracks.Item(i)

            If track.Kind = ITTrackKind.ITTrackKindFile Then

                sPausePendingCheck()
                If bwApp.CancellationPending = True Then
                    Exit For
                End If

                bwApp.ReportProgress(ProgressType.ADJUSTING_RATING, track)

                Dim song As IITFileOrCDTrack = CType(track, IITFileOrCDTrack)
                ssAdjustTrackRating(song)

                ' optionally write PCNT and POPM
                If My.Settings.AdjustRatingPOPM OrElse My.Settings.AdjustRatingPCNT Then
                    Dim tl As New cTrackEditor
                    If tl.UseFile(song.Location) Then
                        Dim bEdited As Boolean = False
                        If My.Settings.AdjustRatingPOPM Then
                            bEdited = tl.SetFramePOPM(track.PlayedCount, track.Rating)
                        End If
                        If My.Settings.AdjustRatingPCNT Then
                            bEdited = tl.SetFramePCNT(track.PlayedCount)
                        End If
                        If bEdited Then
                            tl.Save()
                        End If
                    End If
                End If

                mProgressDiscsCurrent = count
                mProgressDiscsMax = mMainLibraryTracks.Count

            End If

        Next

        bwApp.ReportProgress(ProgressType.READY)

    End Sub

    Private Function sLoadAlbumsTableSafely() As Boolean

        Dim success As Boolean = False

        success = fBwAppRemoveDeadForeignTracks()

        sLoadAlbumsTable()

        Return success

    End Function

    Private Function fBwAppRemoveDeadForeignTracks() As Boolean

        Dim success As Boolean = False

        If chkDeleteNonMusicFolderTracks.Checked Then
            success = ffSafelyDeleteDeadForeignTracks()
        Else
            ssBwAppDeleteDeadTracks(chkDeleteTracksNotInHDD.Checked, chkDeleteNonMusicFolderTracks.Checked)
        End If

        Return success

    End Function

    Private Sub sBwAppValidateLibrary()

        ' Tracks start from 1 instead of conventional 0 for an Array

        If fModeIsRemoveDeadForeignTracks() = True Then

            ' 5.24.1.2 iTSfv continued to load Discs Browser even if the user only had the 
            ' two checkBoxes: remove Dead and Foreign tracks checked
            fBwAppRemoveDeadForeignTracks()

        Else

            mBooFinalTask = False
            sLoadAlbumsTableSafely()
            mBooFinalTask = True

            If bwApp.CancellationPending = False Then

                ssValidateAlbums(0)

                If Not bwApp.CancellationPending Then

                    If chkResume.Checked Then
                        Dim sw As New StreamWriter(mFilePathAlbumsInconsecutiveTracks, True)
                        sw.WriteLine()
                        sw.WriteLine("Only new Albums since last session are loaded since 'resume from last Track' checkBox was checked (warning: partially completed albums from previous session would count as an incomplete album.)")
                        sw.Close()
                    End If

                    sLastCheckedTrackIdSave()

                End If

            End If

        End If

        bwApp.ReportProgress(ProgressType.READY)

    End Sub

    Private Sub sLastCheckedTrackIdSave()
        My.Settings.LastCheckedTrackID += mTracksChecked - 1
        If My.Settings.LastCheckedTrackID > mMainLibraryTracks.Count Then
            My.Settings.LastCheckedTrackID = mTracksChecked
        End If
    End Sub


    ' sWriteListToFile moved to mFileSystem.vb

    Private Function fStuffReportList() As Boolean

        Dim booReport As Boolean = False

        For Each l As List(Of String) In mListLogList
            If l.Count > 0 Then
                booReport = True
            End If
        Next

        Return booReport

    End Function

    Private Function fBooUseCache() As Boolean

        Return mCurrJobTypeMain = JobType.VALIDATE_LIBRARY AndAlso My.Settings.CacheLibrary Or _
        mCurrJobTypeMain = JobType.VALIDATE_TRACKS_SELECTED AndAlso My.Settings.CacheSelectedTracks Or _
        mCurrJobTypeMain = JobType.VALIDATE_DISC AndAlso My.Settings.CacheSelectedTracks Or _
        mCurrJobTypeMain = JobType.VALIDATE_LIBRARY AndAlso My.Settings.CacheLast100 Or _
        mCurrJobTypeMain = JobType.ADD_NEW_TRACKS AndAlso My.Settings.CacheAddNew

    End Function

    Private Function fBooReport() As Boolean

        Return mCurrJobTypeMain = JobType.VALIDATE_LIBRARY AndAlso My.Settings.ReportValidateLibrary Or _
        mCurrJobTypeMain = JobType.VALIDATE_TRACKS_SELECTED AndAlso My.Settings.ReportValidateSelected Or _
        mCurrJobTypeMain = JobType.VALIDATE_DISC AndAlso My.Settings.ReportValidateSelected Or _
        mCurrJobTypeMain = JobType.VALIDATE_LIBRARY AndAlso My.Settings.ReportValidateLast100 Or _
        mCurrJobTypeMain = JobType.ADD_NEW_TRACKS AndAlso My.Settings.ReportAddNewFiles

    End Function


    Private Function fWriteReportToXhtml() As Boolean

        Dim succ As Boolean = True

        Try
            Dim numValidated As Integer = 0
            If mBooResume = True Then
                numValidated = mTracksChecked
            Else
                numValidated = mMainLibraryTracks.Count
            End If

            Dim perc As Decimal = CDec(0.0)
            If numValidated > 0 Then
                perc = CType((numValidated - mListTracksNonITSstandard.Count) * 100 / numValidated, Decimal)
            End If

            ' START NEW INSTANCE OF REPORT WRITING
            Using lReport As New StreamWriter(mFilePathReport, False)

                lReport.WriteLine(mHtml.GetDocType)
                lReport.WriteLine(mHtml.GetCssStyle(My.Settings.ReportCSS))
                lReport.WriteLine(mHtml.GetTitle(String.Format(mAppTitle & " Tracks Report ({0})", Now.ToString("yyyyMMddTHHmm"))))
                lReport.WriteLine(mHtml.CloseHead)

                'If fStuffReportList() Then

                lReport.WriteLine(mHtml.GetHeading(Application.ProductName & " Tracks Report", 1))

                mListTracksNoTrackNum.Sort()
                sWriteListToXhtml("Missing Track Number", mListTracksNoTrackNum, lReport)
                ' mListMissingArtwork.Sort() ' sorted already after validation
                sWriteListToXhtml("Missing Artwork", mListNoArtwork, lReport)
                mListMissingLyrics.Sort()
                'sWriteListToXhtml("Missing Lyrics", mListMissingLyrics)

                mListTracksNoAlbumArtist.Sort()
                sWriteListToXhtml("Missing AlbumArtist", mListTracksNoAlbumArtist, lReport)
                mListFileNotFound.Sort()
                sWriteListToXhtml("Missing Tracks", mListFileNotFound, lReport, False)
                mListFoldersNoFolderJpg.Sort()
                sWriteListToXhtml("Missing Folder.jpg", mListFoldersNoFolderJpg, lReport)

                Dim lListArtworkLowRes As New List(Of String)
                For Each xt As cXmlTrack In mListArtworkLowRes
                    lListArtworkLowRes.Add(String.Format("{0}x{1} for {2}", xt.Artwork.Width, xt.Artwork.Height, xt.Location, xt.MetaVersion))
                Next
                lListArtworkLowRes.Sort()
                sWriteListToXhtml("Tracks with Artwork resolution less than " & _
                                  My.Settings.LowResArtworkWidth & "x" & My.Settings.LowResArtworkHeight, _
                                  lListArtworkLowRes, lReport, True, TreeGUI.cHtml.ListType.Bulletted)

                mListReadOnlyTracks.Sort()
                sWriteListToXhtml("Tracks set to Read-Only", mListReadOnlyTracks, lReport)

                If perc > My.Settings.iTScomplianceRate Then
                    mListTracksNonITSstandard.Sort()
                    sWriteListToXhtml("Tracks not iTunes Store standard compliant", mListTracksNonITSstandard, lReport)
                End If

                Try
                    sWriteGoogleArtwork(lReport)
                Catch ex As Exception
                    msAppendWarnings(ex.Message)
                    msAppendWarnings(ex.StackTrace)
                End Try

                ' End If ' stuff to report

                lReport.WriteLine(mHtml.GetHeading("Statistics", 1))
                lReport.WriteLine(mHtml.OpenBulletedList)
                If chkItunesStoreStandard.Checked Then
                    lReport.WriteLine(mHtml.GetList("iTunes Store Standard compliance: " & perc.ToString("0.00") & "%"))
                    lReport.WriteLine(mHtml.GetList("Total Number of Tracks meet iTunes Store Standard: " & numValidated - mListTracksNonITSstandard.Count))
                End If
                lReport.WriteLine(mHtml.GetList("Number of Tracks validated: " & numValidated & " at " & (numValidated / mSecondsSoFar).ToString("0.00") & " Tracks/Second"))

                If mStatsMaker IsNot Nothing Then
                    lReport.WriteLine(mHtml.GetList(String.Format("Average Track Length: {0}", mStatsMaker.fGetAverageTrackDuration)))
                End If

                lReport.WriteLine(mHtml.GetList("Total Number of Tracks in Library: " & mMainLibraryTracks.Count))
                lReport.WriteLine(mHtml.CloseBulletedList)

                lReport.WriteLine(mHtml.GetHeading("Summary", 1))

                lReport.WriteLine(mHtml.OpenBulletedList)
                lReport.WriteLine(mHtml.GetList("Found Tracks without Artwork: " & mListNoArtwork.Count))
                lReport.WriteLine(mHtml.GetList("Found Tracks without Track Number: " & mListTracksNoTrackNum.Count))
                lReport.WriteLine(mHtml.GetList("Found Tracks without Lyrics: " & mListMissingLyrics.Count))
                lReport.WriteLine(mHtml.GetList("Tracks added Artwork from Artwork.jpg: " & mListArtworkAdded.Count))
                lReport.WriteLine(mHtml.GetList("Fixed Tracks without Album Artist: " & mListTracksNoAlbumArtist.Count))
                lReport.WriteLine(mHtml.GetList("Deleted non Music Folder Tracks: " & mListTracksNonMusicFolder.Count))
                lReport.WriteLine(mHtml.GetList("Deleted Missing Tracks: " & mListFileNotFound.Count))
                lReport.WriteLine(mHtml.GetList("Copied Folder.jpg to Album Folders: " & mListFoldersNoFolderJpg.Count))
                lReport.WriteLine(mHtml.GetList("Tracks made Read-Only: " & mListReadOnlyTracks.Count))
                lReport.WriteLine(mHtml.CloseBulletedList)

                lReport.WriteLine(mHtml.CloseBody)

                ' CLOSE REPORT, READY TO VIEW
                My.Settings.LastTrackReportPath = mFilePathReport

            End Using

        Catch ex As Exception
            msAppendWarnings(ex.Message + " while creating XHTML Report")
            succ = False
        End Try

        Return succ

    End Function

    Private Sub sClearLists()
        ' Clear them after writing
        mListAlbumsMissingArtwork.Clear()
        mListAlbumsMissingArtworkFull.Clear()
        mListArtworkAdded.Clear()
        mListArtworkLowRes.Clear()
        mListFileNotFound.Clear()
        mListFoldersNoFolderJpg.Clear()
        mListTracksNoAlbumArtist.Clear()
        mListTracksNoTrackNum.Clear()
        mListNoArtwork.Clear()
        mListRatings.Clear()
        mListReadOnlyTracks.Clear()
        mListTracksCountUpdated.Clear()
        mListTracksMultipleArtwork.Clear()
        mListTracksNonITSstandard.Clear()
        mListTracksNonMusicFolder.Clear()
        mListTracksToDelete.Clear()
    End Sub

    Private Sub sWriteGoogleArtwork(ByVal sw As StreamWriter)

        If mListAlbumsMissingArtwork.Count > 0 Then

            sw.WriteLine(mHtml.GetHeading("Google search for missing Artwork", 2))

            sw.WriteLine(mHtml.OpenNumberedList)
            'mListAlbumsMissingArtworkFull.Sort(New Comparison(Of cXmlTrack)(AddressOf CompareByTrackNumber))
            mListAlbumsMissingArtworkFull.Sort(New cXmlTrackComparer)
            For Each trInfo As cXmlTrack In mListAlbumsMissingArtworkFull
                ' <a href="downloads/Blurred-10-seconds-WMV9.wmv">Blurred</a>
                sw.WriteLine(mHtml.GetList(trInfo.GoogleSearchURL))
                ''Console.Writeline(trInfo.Track.location)
            Next
            sw.WriteLine(mHtml.CloseNumberedList)

        End If

    End Sub

    Private Function fGetArtistAlbumFromFilePath(ByVal filePath As String) As String

        Dim elemFilePath() As String = filePath.Split(Path.DirectorySeparatorChar)
        Dim str As String = ""
        If elemFilePath.Length > 2 Then
            str = String.Format("{0} - {1}", elemFilePath(elemFilePath.Length - 3), elemFilePath(elemFilePath.Length - 2))
        Else
            str = elemFilePath(elemFilePath.Length - 2) ' mp3 is in root drive
        End If

        Return str
    End Function

    Private Sub sWriteListToXhtml(ByVal title As String, _
                                ByVal myList As List(Of String), _
                                ByVal sw As StreamWriter, _
                                Optional ByVal subHeading As Boolean = True, _
                                Optional ByVal myListType As TreeGUI.cHtml.ListType = TreeGUI.cHtml.ListType.Numbered)

        If subHeading = True Then

            If myList.Count > 0 Then

                sw.WriteLine(mHtml.GetHeading(title, 2))

                Dim lastHeading3 As String = fGetArtistAlbumFromFilePath(myList(0))
                sw.WriteLine(mHtml.GetHeading(lastHeading3, 3))

                sw.WriteLine(mHtml.OpenList(myListType))

                For Each line As String In myList

                    If lastHeading3 <> fGetArtistAlbumFromFilePath(line) Then
                        lastHeading3 = fGetArtistAlbumFromFilePath(line)
                        sw.WriteLine(mHtml.CloseList(myListType))
                        sw.WriteLine(mHtml.GetHeading(lastHeading3, 3))
                        sw.WriteLine(mHtml.OpenList(myListType))
                    End If

                    sw.WriteLine(mHtml.GetList(line))

                Next

                sw.WriteLine(mHtml.CloseList(myListType))

            End If

        Else

            ' FOR LISTS NOT COMPATIBLE WITH SUB-HEADING
            ' THIS MEANS LISTS THAT DO NOT HAVE FILE PATHS

            If myList.Count > 0 Then

                sw.WriteLine(mHtml.GetHeading(title, 2))

                sw.WriteLine(mHtml.OpenList(myListType))
                For Each l As String In myList
                    sw.WriteLine(mHtml.GetList(l))
                Next
                sw.WriteLine(mHtml.CloseList(myListType))

            End If

        End If

    End Sub

    Private Function fGetLastCheckTrackID() As Integer

        Dim lastID As Integer = 0

        If mLast100Tracks Then
            lastID = mMainLibraryTracks.Count - (NUM_QUICK_VALIDATE - 1)
        ElseIf mBooResume = True AndAlso mCurrJobTypeMain = JobType.VALIDATE_LIBRARY Then ' we need to resume
            lastID = My.Settings.LastCheckedTrackID
            If lastID <= 0 Or mMainLibraryTracks.Count <= lastID Then
                lastID = mMainLibraryTracks.Count - (NUM_QUICK_VALIDATE - 1)
            End If
        Else 'full
            lastID = 1
        End If

        If lastID <= 0 Or mMainLibraryTracks.Count <= lastID Then
            lastID = 1
        End If

        Return lastID

    End Function

    Private Sub sLogSettings()

        For Each pg As TabPage In Me.tcTabs.TabPages

            msAppendDebug(pg.Text)
            For Each ctl As Control In pg.Controls

                If TypeOf ctl Is TabControl Then
                    msAppendDebug(ctl.Text)
                    For Each tp As TabPage In CType(ctl, TabControl).TabPages
                        For Each ctl2 As Control In pg.Controls
                            If TypeOf ctl2 Is CheckBox Then
                                msAppendDebug(ctl2.Text & "?: " & CType(ctl2, CheckBox).Checked)
                            End If
                        Next
                    Next
                End If

                If TypeOf ctl Is CheckBox Then
                    msAppendDebug(ctl.Text & "?: " & CType(ctl, CheckBox).Checked)
                End If

            Next

        Next


    End Sub

    Private Sub sButtonValidate()

        If Not bwApp.IsBusy And mfWarnLowResArtworkProceed(chkRemoveLowResArtwork.Checked) = True Then

            Me.TopMost = My.Settings.AlwaysOnTop
            If chkResume.Checked Then
                lbDiscs.Items.Clear()
            End If

            sSettingsSave()
            'OptionsToolStripMenuItem.Enabled = Not bwApp.IsBusy
            'sLogSettings()

            Dim taskValidate As New cBwJob(cBwJob.JobType.VALIDATE_LIBRARY)
            bwApp.RunWorkerAsync(taskValidate)

        End If

    End Sub

    Private Sub sButtonValidateLibrary()
        sButtonValidate()
    End Sub
    Private Sub btnValidate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnValidateLibrary.Click
        sButtonValidateLibrary()
    End Sub

    Private mBooLoadItunesError As Boolean = False

    Private Sub sBwAppLoadPlayer()

        Try
            bwApp.ReportProgress(ProgressType.INITIALIZE_PLAYER_LIBRARY_START)
            mItunesApp = New iTunesLib.iTunesAppClass
            mMainLibraryTracks = mItunesApp.LibraryPlaylist.Tracks
            ITUNES_VERSION = mItunesApp.Version
            msAppendDebug("iTunes Version: " & ITUNES_VERSION)
            mTracksCount = mMainLibraryTracks.Count
            msAppendDebug("Total tracks in iTunes: " & mTracksCount)

            bwApp.ReportProgress(ProgressType.INITIALIZE_ITUNES_XML_DATABASE_START)
            My.Settings.LibraryXMLPath = mItunesApp.LibraryXMLPath
            msAppendDebug("iTunes Library XML Path: " & My.Settings.LibraryXMLPath)
            msAppendDebug("iTunes Music folder: " & fGetLibraryParser.MusicFolder)
            For i As Integer = 0 To My.Settings.MusicFolderLocations.Count - 1
                msAppendDebug(String.Format("{0}'s Music Folder {1}: {2}", Environment.UserName, i + 1, My.Settings.MusicFolderLocations(i)))
            Next

            bwApp.ReportProgress(ProgressType.INITIALIZE_PLAYER_FINISH)

        Catch ex As Exception

            msAppendWarnings("000001")
            msAppendWarnings(ex.Message)
            msAppendWarnings(ex.StackTrace)
            bwApp.ReportProgress(ProgressType.INITIALIZE_ITUNES_ERROR, ex)
            sRegServerItunes()
            mBooLoadItunesError = True

        End Try



    End Sub

    Private Sub sBwAppCommandLineJobs()

        sBwAppLoadPlayer()

        Dim cli As New cCommandLine

        If cli.AddFolder And Directory.Exists(cli.AddFolderPath) Then
            sQueueFolderToLibrary(cli.AddFolderPath)
        End If

        If cli.Synchroclean Then
            sBwAppFindNewTracksFromHDD()
            ssBwAppDeleteDeadTracks(bDeleteTracksNotInHDD:=True, bDeleteNonMusicFolderTracks:=False)
        Else

            If cli.RemoveDeadForeignFiles Then
                ssBwAppDeleteDeadTracks(True, True)
            ElseIf cli.RemoveDeadFiles Then
                ssBwAppDeleteDeadTracks(True, False)
            ElseIf cli.RemoveForeignFiles Then
                ssBwAppDeleteDeadTracks(False, True)
            End If

            If cli.AddFiles Then
                sBwAppFindNewTracksFromHDD()
            End If

        End If


        If cli.AdjustRatings Then
            sBwAppAdjustRatings()
        End If



        If cli.ReverseScrobble Then
            sBwAppReverseScrobble()
        End If

        If cli.ValidateLibrary Then
            sBwAppValidateLibrary()
        End If

    End Sub

    Private Sub sRegServerItunes()

        Dim p As New Process
        Dim psi As New ProcessStartInfo("itunes.exe")
        ''Console.Writeline(psi.FileName)
        psi.Arguments = "regserver"
        p.StartInfo = psi
        p.Start()
        ''Console.Writeline("Registered COM for " & p.ProcessName)

    End Sub

    Private Sub OpenTracksReportToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenTracksReportToolStripMenuItem.Click
        sOpenTracksReport()
    End Sub

    Private Sub sOpenTracksReport()

        ' 2.7.0.1 Crashing while attempting to open non-existant tracks report
        ' we look at most recent html file in logs folder
        Dim f As String() = Directory.GetFiles(My.Settings.LogsDir, "*report.html", SearchOption.TopDirectoryOnly)
        If f.Length > 0 Then
            Array.Sort(f)
            Array.Reverse(f)
            My.Settings.LastTrackReportPath = f(0)
        End If

        If mfOpenFileOrDirPath(My.Settings.LastTrackReportPath, sBarTrack, ttApp, ssAppDisc) = False Then
            mfOpenFileOrDirPath(mFilePathReport, sBarTrack, ttApp, ssAppDisc)
        End If

    End Sub

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        msShowAbout()
    End Sub

    Private Sub VersionHistoryToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles VersionHistoryToolStripMenuItem.Click
        msShowVersionHistory()
    End Sub

    Private Function fCancelJob() As Boolean

        If bwApp.IsBusy Then

            msAppendDebug("iTSfv was paused...")

            If (MessageBox.Show("Are you sure?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes) Then
                bwApp.CancelAsync()
                mCurrJob.JobCancelled = True
                If mCurrJobTypeMain = cBwJob.JobType.VALIDATE_LIBRARY Then
                    lbDiscs.Items.Clear()
                End If
                btnStop.Enabled = False
                msAppendDebug("Operation was stopped by the user...")
                Return True
            Else
                mJobPaused = False
            End If

        End If

        Return False

    End Function

    Private Sub btnStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStop.Click

        mJobPaused = True
        fCancelJob()

    End Sub

    Private Sub tmrFormClose_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrFormClose.Tick

        If Not bwApp.IsBusy Then
            Me.Close()
        End If

    End Sub

    Private Sub ssDecompileAlbum(ByVal track As IITFileOrCDTrack)

        If mArtistDecompiled <> "" Then

            bwApp.ReportProgress(ProgressType.DECOMPILE_TRACKS, track.Name)

            If track.Location IsNot Nothing Then

                If track.Artist <> String.Empty AndAlso track.Artist <> mArtistDecompiled.Trim Then
                    track.Name = mfGetStringFromPattern(mStringDecompilePattern, track)
                End If
                track.Artist = mArtistDecompiled.Trim
                track.AlbumArtist = track.Artist
                track.Compilation = False

            End If

        End If

    End Sub

    Private Function ffGetFixedCapitalString(ByVal tag As String, ByVal wl As WordLists) As String

        tag = mfGetFixedCase(tag, wl)

        If chkStrict.Checked Then
            Return ffGetFixedCapitalString2(tag, wl)
        Else
            Return ffGetFixedCapitalString1(tag)
        End If

    End Function

    Private Function ffGetFixedCapitalString1(ByVal tag As String) As String

        ' never replace different words here or iTunes would't find them
        tag = mfGetFixedString(tag)

        Return tag

    End Function

    Private Function ffGetFixedCapitalString2(ByVal tag As String, ByVal wl As WordLists) As String

        Dim oldTag As String = tag

        For Each simpleWord As String In wl.simpleWords

            Dim cWord As String = StrConv(simpleWord, VbStrConv.ProperCase)
            tag = tag.Replace(" " & cWord & " ", " " & simpleWord & " ")
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '' 5.25.3.1 Some fixes to prevent decapitalizing "Finale: Up" to "Final: up" etc. 
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            tag = tag.Replace(": " + simpleWord, ": " + cWord)
            tag = tag.Replace(". " + simpleWord, ". " + cWord)

        Next

        ' never replace different words here or iTunes would't find them
        tag = mfGetFixedString(tag)

        If oldTag <> tag Then
            msAppendDebug(String.Format("""{0}"" is now ""{1}""", oldTag, tag))
        End If

        Return tag

    End Function

    Private Function fBooRegroupSafe(ByVal song As IITFileOrCDTrack) As Boolean

        Dim bSafe As Boolean = song.Playlist.Name = "Music" OrElse _
               song.Playlist.Name = "Recently Added" OrElse _
               song.Playlist.Name = "Recently Played" OrElse _
               song.Playlist.Name = "Recently Modified"

        If bSafe = False Then
            msAppendDebug("Capitalizing AlbumArtist tag is disabled under playlist: " + song.Playlist.Name)
        End If

        Return bSafe

    End Function

    Private Sub ssCapitalizeWord(ByVal songs As List(Of IITFileOrCDTrack), Optional ByVal bRenameFile As Boolean = True)

        If songs IsNot Nothing AndAlso songs.Count > 0 Then

            Dim wl As New WordLists
            wl.simpleWords = mfGetSimpleWordsList()
            wl.capitalWords = mfGetCapitalWordsList()

            bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, songs.Count)

            If fBooRegroupSafe(songs(0)) Then

                If chkAlbumArtist.Checked Or chkArtist.Checked Then
                    mfUpdateStatusBarText("Regrouping tracks in album...", True)
                End If

                Dim wasReadOnly(songs.Count) As Boolean

                For i As Integer = 0 To songs.Count - 1

                    Dim song As IITFileOrCDTrack = songs(i)
                    Dim fiTrack As New FileInfo(song.Location)
                    wasReadOnly(i) = fiTrack.IsReadOnly
                    If wasReadOnly(i) Then fiTrack.IsReadOnly = False

                    If chkAlbumArtist.Checked And String.IsNullOrEmpty(song.AlbumArtist) = False Then
                        song.AlbumArtist = ffGetFixedCapitalString(song.AlbumArtist, wl) + "1"
                    End If

                    If chkArtist.Checked And String.IsNullOrEmpty(song.Artist) = False Then
                        song.Artist = ffGetFixedCapitalString(song.Artist, wl) + "1"
                    End If

                Next

                For i As Integer = 0 To songs.Count - 1

                    Dim song As IITFileOrCDTrack = songs(i)
                    Dim fiTrack As New FileInfo(song.Location)

                    If chkAlbumArtist.Checked And String.IsNullOrEmpty(song.AlbumArtist) = False Then
                        song.AlbumArtist = song.AlbumArtist.Remove(song.AlbumArtist.Length - 1, 1)
                    End If

                    If chkArtist.Checked And String.IsNullOrEmpty(song.Artist) = False Then
                        song.Artist = song.Artist.Remove(song.Artist.Length - 1, 1)
                    End If

                    If wasReadOnly(i) Then fiTrack.IsReadOnly = True

                Next

            End If

            For Each song As IITFileOrCDTrack In songs

                sPausePendingCheck()
                If bwApp.CancellationPending Then
                    Exit Sub
                End If

                If File.Exists(song.Location) Then

                    Dim locTrack As String = "dead track"

                    Try
                        locTrack = song.Location
                    Catch ex As Exception

                    End Try

                    bwApp.ReportProgress(ProgressType.CAPITALIZE_FIRST_LETTER, song.Name)

                    Dim fiTrack As New FileInfo(song.Location)
                    Dim wasReadOnly As Boolean = fiTrack.IsReadOnly
                    If wasReadOnly Then fiTrack.IsReadOnly = False

                    Try

                        '' modify the tags first
                        If chkArtist.Checked Then
                            song.Artist = ffGetFixedCapitalString(song.Artist, wl)
                        End If

                        If chkAlbum.Checked Then
                            song.Album = ffGetFixedCapitalString(song.Album, wl)
                        End If

                        If chkName.Checked Then
                            song.Name = ffGetFixedCapitalString(song.Name, wl)
                        End If

                        If chkGenre.Checked Then
                            song.Genre = ffGetFixedCapitalString(song.Genre, wl)
                        End If

                        '' rename files 
                        If bRenameFile = True Then

                            Dim strOldFileName As String = Path.GetFileName(song.Location)
                            Dim strNewFileName As String = ffGetFixedCapitalString1(Path.GetFileNameWithoutExtension(song.Location)) + Path.GetExtension(song.Location)

                            '' this ensures user does not accidentally replaces words in a filename so that iTunes cannot find
                            If strOldFileName.ToLower = strNewFileName.ToLower Then

                                Dim strOldFilePath As String = song.Location
                                Dim strFolderPath As String = IO.Path.GetDirectoryName(strOldFilePath)

                                Dim temp As String = ".temp"

                                Try
                                    locTrack = song.Location
                                    My.Computer.FileSystem.RenameFile(strOldFilePath, strNewFileName & temp)
                                    My.Computer.FileSystem.RenameFile(strFolderPath & "\" & strNewFileName & temp, strNewFileName)
                                Catch ex As Exception
                                    msAppendWarnings(ex.Message & " for " & locTrack)
                                    msAppendWarnings(ex.StackTrace)
                                    bwApp.ReportProgress(ProgressType.READY, ex.Message)
                                End Try

                            End If

                        End If ' rename file

                    Catch ex As Exception
                        msAppendWarnings(ex.Message & " for " & locTrack)
                        msAppendWarnings(ex.StackTrace)
                    End Try

                    If wasReadOnly Then fiTrack.IsReadOnly = True

                End If

            Next

        End If

    End Sub

    Private Sub sBwAppFindNewTracksFromHDD()

        msAppendDebug("Finding new tracks in specified music locations...")

        ' find all tracks in music folders
        Dim lListTracksLocationHdd As New List(Of String)
        lListTracksLocationHdd = mfGetNewFilesFromHDD(bwApp, mFileExtAudioITunes)

        Dim lListTracksLocationsPlayer As New List(Of String)

        If My.Settings.SyncQuick = True Then

            ' 5.14.0.4 Synchrocleaning more than once still showed up and attemped to add the new tracks detected from the first synchroclean
            bwApp.ReportProgress(ProgressType.INITIALIZE_ITUNES_XML_DATABASE_START)
            Dim lXmlLibParser As New cLibraryParser(bwApp, mItunesApp.LibraryXMLPath)
            ' quick: we use XML search
            Dim jumpNum As Integer = 100
            bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, lXmlLibParser.TrackCollection.Count / jumpNum)

            Dim jumpCheck As Integer = 0

            For Each xmlTrack As cXmlTrack In lXmlLibParser.TrackCollection

                jumpCheck += 1 ' xml search is so fast that we gotta slow down reporting progress

                sPausePendingCheck()
                If bwApp.CancellationPending Then
                    Exit Sub
                End If

                If jumpCheck Mod jumpNum = 0 Then
                    bwApp.ReportProgress(ProgressType.SCANNING_TRACK_IN_ITUNES_XML_DATABASE, String.Format("{0} - {1}", xmlTrack.Album, xmlTrack.Artist))
                End If

                If File.Exists(xmlTrack.Location) Then
                    lListTracksLocationsPlayer.Add(CType(xmlTrack.Location, String).ToLower)
                End If

            Next

        Else

            bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, mMainLibraryTracks.Count)

            ' slow: we use ITL file search
            Dim listMissingTracks As New List(Of IITFileOrCDTrack)

            For Each track As iTunesLib.IITTrack In mMainLibraryTracks

                sPausePendingCheck()
                If bwApp.CancellationPending Then
                    Exit Sub
                End If

                If track.Kind = ITTrackKind.ITTrackKindFile Then

                    Dim song As IITFileOrCDTrack = CType(track, IITFileOrCDTrack)
                    sLoadTrackToDiscsTable(song) ' HELP PREFETECH LOADING ALBUM BROWSER
                    If File.Exists(song.Location) Then
                        lListTracksLocationsPlayer.Add(song.Location.ToLower)
                    Else
                        listMissingTracks.Add(CType(track, IITFileOrCDTrack))
                    End If

                    bwApp.ReportProgress(ProgressType.SCANNING_TRACK_IN_PLAYER_LIBRARY, mLibraryTasks.fGetDiscName(track))

                End If

            Next

            If chkReplaceWithNewKind.Checked Then
                lListTracksLocationsPlayer.AddRange(ssReplaceTracks(listMissingTracks))
            End If ' if replace tracks with different kind check box is checked

        End If

        lListTracksLocationsPlayer.Sort()
        lListTracksLocationHdd.Sort()

        Dim filesNew As Integer = lListTracksLocationHdd.Count - lListTracksLocationsPlayer.Count
        If filesNew > 0 Then
            bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, filesNew)
            ' bwApp.ReportProgress(TaskType.SET_ACTIVE_TAB, tpExplorer) ' doesnt really work
        End If

        '* FINALLY FIND NEW MUSIC

        Dim listAddableFiles As New List(Of String)

        For Each filePathFromHdd As String In lListTracksLocationHdd

            Dim result As Integer = lListTracksLocationsPlayer.BinarySearch(filePathFromHdd.ToLower)
            'Dim xmlResult As Integer = xmlTracksLocationItunes.BinarySearch(filePathFromHdd.ToLower)

            If result < 0 Then
                ' Track is not found then list it
                listAddableFiles.Add(filePathFromHdd)
                bwApp.ReportProgress(ProgressType.ADD_TRACKS_TO_LISTBOX_TRACKS, filePathFromHdd)
            End If

        Next

        msAppendDebug("Finding new tracks in specified music locations... Done.")

        Dim msg As String = String.Empty
        If My.Settings.ScanOnlyRecentFolders Then
            msg = String.Format("{0} of {1} recently added music files were in iTunes Library.", lListTracksLocationHdd.Count - listAddableFiles.Count, lListTracksLocationHdd.Count)
        Else
            msg = String.Format("{0} of {1} music files were in iTunes Library.", lListTracksLocationsPlayer.Count, lListTracksLocationHdd.Count)
        End If


        If chkAddFile.Checked Then
            sAddFilesToLibrary()
        Else
            MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

        msAppendDebug(msg)
        bwApp.ReportProgress(ProgressType.READY, msg)

        'fswApp.EnableRaisingEvents = My.Settings.WatchFileSystem

    End Sub


    Private Function ssReplaceTracks(ByVal listMissingTracks As List(Of iTunesLib.IITFileOrCDTrack)) As List(Of String)

        Dim lListNewTrackLocations As New List(Of String)

        ' set the progress bar value to 0 and maximum to missing tracks count
        bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, listMissingTracks.Count)

        For Each trackOld As iTunesLib.IITFileOrCDTrack In listMissingTracks

            ' the track that is being searched in xml library

            Dim xmlTrack As cXmlTrack = fGetLibraryParser.fFindXmlTrack(trackOld)

            If xmlTrack IsNot Nothing Then

                For Each fileExt As String In mFileExtAudioITunes

                    Dim fileNewPath As String = Path.ChangeExtension(xmlTrack.Location, fileExt)

                    If xmlTrack.Location <> fileNewPath Then

                        'MsgBox("Missing Track: " & xmlTrack.Location)

                        If Not File.Exists(fileNewPath) Then

                            'MsgBox("Didn't find NewPath: " & fileNewPath)

                            ' try looking in music library 
                            fileNewPath = My.Settings.MusicFolderPath + "\" + _
                               xmlTrack.Artist + "\" + _
                               xmlTrack.Album + "\" + Path.GetFileName(fileNewPath)

                            'MsgBox("Try NewPath: " & fileNewPath)

                        End If

                        If File.Exists(fileNewPath) Then

                            'Console.Writeline("Found " & fileNewPath & " PlayCount: " & trackOld.PlayedCount)
                            bwApp.ReportProgress(ProgressType.REPLACING_TRACKS, xmlTrack.Location)

                            Dim job As IITOperationStatus
                            job = mItunesApp.LibraryPlaylist.AddFile(fileNewPath)

                            If job IsNot Nothing Then

                                Try
                                    Dim trackNew As IITTrack = job.Tracks.Item(1)
                                    lListNewTrackLocations.Add(fileNewPath)

                                    trackNew.PlayedCount += trackOld.PlayedCount
                                    trackNew.Rating = trackOld.Rating
                                    trackNew.PlayedDate = trackOld.PlayedDate

                                Catch ex As Exception
                                    msAppendWarnings(ex.Message + " while migrating tags to new track during track replace.")
                                End Try

                            End If

                        End If

                        Exit For ' no need to go through other extensions

                    End If ' duplicate file check

                Next ' check for all extensions loop

            End If ' if found xml track is not nothing

        Next ' all tracks loop

        '' 20071230T071453 The playlist is not modifiable. while deleting dead track during track replace.
        'For Each trackOld As iTunesLib.IITFileOrCDTrack In listMissingTracks
        '    Try
        '        trackOld.Delete()
        '    Catch ex As Exception
        '        msAppendWarnings(ex.Message + " while deleting dead track during track replace.")
        '    End Try
        'Next

        Return lListNewTrackLocations

    End Function

    Private Function ffSafelyDeleteDeadForeignTracks() As Boolean

        msAppendDebug("Safety checks initiated before removing tracks outside of music folders.")
        Dim success As Boolean = False
        Dim guessedFolder As String = ffGetMusicFolderGuessed(100)

        Dim folderMatches As Boolean = False

        For Each musicFolder As String In My.Settings.MusicFolderLocations

            If musicFolder.EndsWith(Path.DirectorySeparatorChar) = False Then
                musicFolder += Path.DirectorySeparatorChar
            End If

            If musicFolder.Equals(guessedFolder) Then
                folderMatches = True
                Exit For
            End If

        Next

        If folderMatches = False Then

            If MessageBox.Show(Application.ProductName & " has found that some or all of your tracks are not in any of the iTunes Music folder locations so iTSfv will not remove tracks out side of music folders." & Environment.NewLine & _
             Environment.NewLine & "Most of your music are in:" & Environment.NewLine & guessedFolder & _
                Environment.NewLine & String.Format("Your iTunes Music folder location is:" & _
                Environment.NewLine & fGetLibraryParser.MusicFolder & Environment.NewLine & Environment.NewLine & "Set iTunes Music folder location to where you primarily store music ({0}) and retry to remove tracks outside of music folder or add another location using iTSfv Options. iTSfv will continue to remove tracks that do not exist.", guessedFolder), _
                Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = Windows.Forms.DialogResult.OK Then

                ssBwAppDeleteDeadTracks(chkDeleteTracksNotInHDD.Checked, chkDeleteNonMusicFolderTracks.Checked)
                success = True

            End If

        Else
            ssBwAppDeleteDeadTracks(chkDeleteTracksNotInHDD.Checked, True)
            success = True
        End If

        Return success

    End Function

    Private Sub sBwAppSynchroclean()

        If ffSafelyDeleteDeadForeignTracks() = True Then
            If Not bwApp.CancellationPending Then
                sBwAppFindNewTracksFromHDD()
            End If
        Else
            bwApp.ReportProgress(ProgressType.READY)
        End If

    End Sub

    Private Sub sBwAppReplaceTracks()

        Dim tracks As IITTrackCollection
        tracks = mItunesApp.BrowserWindow.SelectedTracks

        If tracks IsNot Nothing Then

            bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, tracks.Count)

            Dim listMissingTracks As New List(Of IITFileOrCDTrack)

            For Each track As IITTrack In tracks
                If track.Kind = ITTrackKind.ITTrackKindFile Then
                    listMissingTracks.Add(CType(track, IITFileOrCDTrack))
                End If
            Next

            ssReplaceTracks(listMissingTracks)

        End If

        bwApp.ReportProgress(ProgressType.READY)

    End Sub

    Private Function fTracksAreStandard(ByVal tracks As List(Of IITFileOrCDTrack)) As String

        Dim msg As String = "Ready. All tracks conform to iTunes Store standard."

        Dim standard As Boolean = True

        Dim ugly As Integer = 0

        Dim tracksMax As Integer = Math.Min(tracks.Count, 25)

        For i As Integer = 0 To tracksMax - 1

            Dim track As iTunesLib.IITTrack = tracks(i)
            If track.Kind = ITTrackKind.ITTrackKindFile Then
                Dim checkTrack As Boolean = fIsItunesStoreStandard(New cXmlTrack(CType(track, IITFileOrCDTrack), False))
                If checkTrack = False Then
                    ugly += 1
                End If
                standard = standard And checkTrack
            End If

        Next

        If standard = False Then
            msg = String.Format("Ready. {0} of {1} Tracks checked do not conform to iTunes Store standard.", ugly, tracksMax)
        End If

        Return msg

    End Function

    Private Sub sBwAppValidateTracksSelected()

        Dim tracks As IITTrackCollection
        tracks = mItunesApp.BrowserWindow.SelectedTracks

        'bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, tracks.Count)

        If tracks IsNot Nothing Then

            ssBwAppDeleteDeadTracks(chkDeleteTracksNotInHDD.Checked, chkDeleteNonMusicFolderTracks.Checked, tracks)

            ' we need to recreate hash table
            mTableDiscs = New Hashtable
            mListDiscKeys.Clear()

            mProgressTracksMax = tracks.Count

            For Each track As iTunesLib.IITTrack In tracks

                sPausePendingCheck()
                If bwApp.CancellationPending Then
                    Exit For
                End If

                If track.Kind = ITTrackKind.ITTrackKindFile Then

                    sLoadTrackToDiscsTable(CType(track, IITFileOrCDTrack))
                    mProgressTracksCurrent += 1

                End If

            Next

            ssValidateAlbums(0)

        End If

        bwApp.ReportProgress(ProgressType.READY)

    End Sub

    Private Sub sBwAppExportTracks()

        ' 5.10.0.0 Support exporting selected tracks to another directory specified

        Dim tracks As IITTrackCollection
        tracks = mItunesApp.BrowserWindow.SelectedTracks

        bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, tracks.Count)

        If tracks IsNot Nothing Then

            For Each track As iTunesLib.IITTrack In tracks

                sPausePendingCheck()
                If bwApp.CancellationPending Then
                    Exit For
                End If

                Dim srcPath As String = ""
                Dim destPath As String = ""

                If track.Kind = ITTrackKind.ITTrackKindFile AndAlso File.Exists(CType(track, IITFileOrCDTrack).Location) Then

                    Try

                        srcPath = CType(track, IITFileOrCDTrack).Location
                        Dim destFileName As String = mfGetFileNameFromPattern(mExportFilePattern, CType(track, IITFileOrCDTrack)) + Path.GetExtension(CType(track, IITFileOrCDTrack).Location)
                        destPath = Path.Combine(mCurrJob.mMessage, destFileName)
                        ''Console.Writeline(destPath)
                        My.Computer.FileSystem.CopyFile(srcPath, destPath, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs)

                        ' optionally export Artwork 
                        If My.Settings.ExportTracksArtwork AndAlso track.Artwork.Count > 0 Then
                            Dim artworkPath As String = Path.Combine(mCurrJob.mMessage, Path.GetFileNameWithoutExtension(destPath) + mfGetArtworkExtension(track.Artwork(1)))
                            track.Artwork(1).SaveArtworkToFile(artworkPath)
                        End If

                        bwApp.ReportProgress(ProgressType.EXPORT_TRACKS, track.Name)

                    Catch ex As Exception

                        Try

                            destPath = Path.Combine(mCurrJob.mMessage, Path.GetFileName(CType(track, IITFileOrCDTrack).Location))
                            My.Computer.FileSystem.CopyFile(srcPath, destPath, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs)

                            bwApp.ReportProgress(ProgressType.EXPORT_TRACKS, track.Name)

                        Catch ex2 As Exception

                            ' Do Nothing

                        End Try

                        msAppendWarnings("Source File Path: " & srcPath)
                        msAppendWarnings("Destination Path: " & destPath)
                        msAppendWarnings(ex.Message + " while exporting selected tracks")

                    End Try

                End If ' file exists

            Next

            bwApp.ReportProgress(ProgressType.READY)

        End If

    End Sub

    Private Function fBooEditTracks() As Boolean

        Return chkTagRemove.Checked Or _
        chkReplaceTextInTags.Checked Or _
        chkDecompile.Checked Or _
        chkTrimChar.Checked Or _
        chkAppendChar.Checked

    End Function

    Private Sub sBwAppEditSelectedTracks()

        Dim tracks As IITTrackCollection
        tracks = mItunesApp.BrowserWindow.SelectedTracks

        If tracks IsNot Nothing Then

            If fBooEditTracks() Then

                bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, tracks.Count)

                If mReplaceText.Equals("<Empty>") Then
                    mReplaceText = String.Empty
                End If

                For Each track As iTunesLib.IITTrack In tracks

                    sPausePendingCheck()
                    If bwApp.CancellationPending Then
                        Exit For
                    End If

                    If track.Kind = ITTrackKind.ITTrackKindFile Then

                        Try

                            Dim fiTrack As New FileInfo(CType(track, IITFileOrCDTrack).Location)
                            Dim wasReadOnly As Boolean = fiTrack.IsReadOnly

                            ''**********************************
                            ''* Retain Modified Date
                            ''**********************************
                            Dim dBefore As Date = fiTrack.LastWriteTime

                            ''**********************************
                            ''* Remove Read-Only
                            ''**********************************
                            If fiTrack.IsReadOnly Then
                                fiTrack.IsReadOnly = False
                                msAppendDebug(String.Format("Cleared {0} Read-Only flag", fiTrack.FullName))
                            End If

                            ''**********************************
                            ''* Remove Lyrics
                            ''**********************************
                            If chkTagRemove.Checked Then
                                ssRemoveTags(CType(track, IITFileOrCDTrack))
                            End If

                            ''**********************************
                            ''* Replace Text in Tags
                            ''**********************************
                            If chkReplaceTextInTags.Checked Then
                                ssReplaceTextInTags(CType(track, IITFileOrCDTrack))
                            End If

                            ''**********************************
                            ''* Decompile Track
                            ''**********************************
                            If chkDecompile.Checked Then
                                ssDecompileAlbum(CType(track, IITFileOrCDTrack))
                            End If

                            ''**********************************
                            ''* Trim Characters
                            ''**********************************
                            If chkTrimChar.Checked Then
                                ssTrimChar(CType(track, IITFileOrCDTrack))
                            End If

                            ''**********************************
                            ''* Append/Prepend Characters
                            ''**********************************
                            If chkAppendChar.Checked Then
                                ssAppendPrependChar(CType(track, IITFileOrCDTrack))
                            End If

                            ''**********************************
                            ''* Retain Modified Date
                            ''**********************************
                            If My.Settings.ModifiedDateRetain = True Then
                                File.SetLastWriteTime(fiTrack.FullName, dBefore)
                            End If

                            ''**********************************
                            ''* Restore Read-Only
                            ''**********************************
                            If wasReadOnly Then
                                fiTrack.IsReadOnly = True
                                msAppendDebug(String.Format("Undo clear {0} Read-Only flag", fiTrack.FullName))
                            End If

                            bwApp.ReportProgress(ProgressType.INCREMENT_DISC_PROGRESS)

                        Catch ex As Exception
                            msAppendWarnings(ex.Message + " while editing tracks")
                        End Try

                    End If

                Next ' for each track in disc

            End If ' edit tracks other than Captitalize First Letter

            ''**********************************
            ''* Captitalize First Letter
            ''**********************************
            If chkCapitalizeFirstLetter.Checked Then

                Dim songs As New List(Of IITFileOrCDTrack)
                For Each track As iTunesLib.IITTrack In tracks
                    If track.Kind = ITTrackKind.ITTrackKindFile Then
                        Dim song As IITFileOrCDTrack = CType(track, IITFileOrCDTrack)
                        If File.Exists(song.Location) Then
                            songs.Add(song)
                        End If
                    End If
                Next

                ssCapitalizeWord(songs, chkRenameFile.Checked)

            End If

        End If


        bwApp.ReportProgress(ProgressType.READY)


    End Sub

    Private Function fGetTrimText(ByVal oTxt As String) As String

        Dim lTxt As String = String.Empty

        Dim trimChar As Integer = CInt(nudTrimChar.Value)

        If oTxt.Length - trimChar > 0 Then
            If mTrimDirectionText = "Left" Then
                lTxt = oTxt.Substring(trimChar).Trim
            Else
                lTxt = oTxt.Substring(0, oTxt.Length - trimChar).Trim
            End If
        End If

        msAppendDebug(String.Format("""{0}"" was trimmed to ""{1}""", oTxt, lTxt))

        Return lTxt

    End Function

    Private Sub ssAppendPrependChar(ByVal track As IITFileOrCDTrack)

        bwApp.ReportProgress(ProgressType.APPEND_PREPEND_CHAR, track.Name)

        If mAppendPrepend = "append" Then
            ' append
            If chkName.Checked Then
                track.Name = String.Concat(track.Name, txtAppend.Text)
            End If

            If chkAlbum.Checked Then
                track.Album = String.Concat(track.Album, txtAppend.Text)
            End If

            If chkArtist.Checked Then
                track.Artist = String.Concat(track.Artist, txtAppend.Text)
            End If

            If chkGenre.Checked Then
                track.Genre = String.Concat(track.Genre, txtAppend.Text)
            End If

            If track.Kind = ITTrackKind.ITTrackKindFile Then
                Dim song As IITFileOrCDTrack = track
                If chkAlbumArtist.Checked Then
                    song.AlbumArtist = String.Concat(song.AlbumArtist, txtAppend.Text)
                End If
            End If

        ElseIf mAppendPrepend = "prepend" Then

            ' prepend
            If chkName.Checked Then
                track.Name = String.Concat(txtAppend.Text, track.Name)
            End If

            If chkAlbum.Checked Then
                track.Album = String.Concat(txtAppend.Text, track.Album)
            End If

            If chkArtist.Checked Then
                track.Artist = String.Concat(txtAppend.Text, track.Artist)
            End If

            If chkGenre.Checked Then
                track.Genre = String.Concat(txtAppend.Text, track.Genre)
            End If

            If chkAlbumArtist.Checked Then
                track.AlbumArtist = String.Concat(txtAppend.Text, track.AlbumArtist)
            End If

        End If

    End Sub

    Private Sub ssTrimChar(ByVal track As IITFileOrCDTrack)

        bwApp.ReportProgress(ProgressType.TRIMMING_CHAR, track.Name)

        If chkName.Checked Then
            track.Name = fGetTrimText(track.Name)
        End If

        If chkAlbum.Checked Then
            track.Album = fGetTrimText(track.Album)
        End If

        If chkArtist.Checked Then
            track.Artist = fGetTrimText(track.Artist)
        End If

        If chkGenre.Checked Then
            track.Genre = fGetTrimText(track.Genre)
        End If

        If track.Kind = ITTrackKind.ITTrackKindFile Then

            If chkAlbumArtist.Checked Then
                CType(track, IITFileOrCDTrack).AlbumArtist = fGetTrimText(CType(track, IITFileOrCDTrack).AlbumArtist)
            End If

        End If

    End Sub


    Private Sub ssRemoveTags(ByVal track As IITFileOrCDTrack)

        Try
            If track.Kind = ITTrackKind.ITTrackKindFile Then
                Dim msg As String = String.Empty
                Dim tags As String = String.Empty
                If My.Settings.RemoveLyrics Then
                    msg = String.Format("Removing {0} in ""{1}""", "Lyrics", track.Name)
                    tags = "Lyrics"
                    If CType(track, IITFileOrCDTrack).Lyrics <> String.Empty Then
                        CType(track, IITFileOrCDTrack).Lyrics = ""
                    End If
                    msAppendDebug(msg)
                End If
                If My.Settings.RemoveComments Then
                    If My.Settings.RemoveLyrics Then
                        tags = "Lyrics and Comments"
                    Else
                        tags = "Comments"
                    End If
                    msg = String.Format("Removing {0} in ""{1}""", "Comments", track.Name)
                    If CType(track, IITFileOrCDTrack).Comment <> String.Empty Then
                        CType(track, IITFileOrCDTrack).Comment = ""
                    End If
                End If
                bwApp.ReportProgress(ProgressType.REMOVING_TAGS, String.Format("Removing {0} in ""{1}""", tags, track.Name))
            End If
        Catch ex As Exception
            msAppendWarnings(ex.Message + " while removing tags")
        End Try

    End Sub

    Private Sub sBwAppOverrideTags()

        Dim tracks As IITTrackCollection
        tracks = mItunesApp.BrowserWindow.SelectedTracks

        bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, tracks.Count)

        If tracks IsNot Nothing Then

            Dim dt As New DateTime

            Dim totalDuration As Integer = 0

            For Each track As iTunesLib.IITTrack In tracks

                If chkPlayedDateOverride.Checked Then
                    totalDuration += track.Duration
                    dt = dtpPlayedDate.Value.AddSeconds(totalDuration)
                    track.PlayedDate = dt
                End If

                If chkPlayedCountOverride.Checked Then
                    bwApp.ReportProgress(ProgressType.UPDATE_STATUSBAR_TEXT, String.Format("Incrementing PlayedCount for {0} by {1}", track.Name, nudPlayedCountOverride.Value))
                    track.PlayedCount = CType(track.PlayedCount + nudPlayedCountOverride.Value, Integer)
                End If

                If chkRatingOverride.Checked Then
                    bwApp.ReportProgress(ProgressType.UPDATE_STATUSBAR_TEXT, String.Format("Incrementing Rating for {0} by {1}", track.Name, nudRatingOverride.Value))
                    track.Rating = CType(track.Rating + nudRatingOverride.Value, Integer)
                End If

            Next

            bwApp.ReportProgress(ProgressType.READY, String.Format(" Overrode {0} tracks.", tracks.Count))

        Else

            bwApp.ReportProgress(ProgressType.READY)

        End If

    End Sub

    Private Sub sBwAppScheduleTask()

        If chkSheduleAdjustRating.Checked Then
            sBwAppAdjustRatings()
        End If

        If chkScheduleFindNewFilesHDD.Checked Then
            sBwAppFindNewTracksFromHDD()
        End If

        If chkSchValidateLibrary.Checked Then
            sBwAppValidateLibrary()
        End If

    End Sub


    Private Sub sBwAppStatistics()

        Dim f As New frmStatistics(fGetStatsMaker)
        If bwApp.CancellationPending = False Then
            bwApp.ReportProgress(ProgressType.SHOW_STATISTICS_WINDOW, f)
        End If
        bwApp.ReportProgress(ProgressType.READY)

    End Sub

    Private Sub ssShowStatistics(ByVal lStats As cStatsMaker)

        If Not bwApp.CancellationPending AndAlso lStats IsNot Nothing Then

            Dim f As New frmStatistics(lStats)
            f.Show()

        End If

    End Sub

    Private Sub sBwAppImportPOPM()
        mJobsIT.msBwAppImportPOPM(bwApp, mMainLibraryTracks)
    End Sub

    Private Sub bwApp_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bwApp.DoWork

        'mfUpdateStatusBarText("", False)

        ' start fresh
        mJobPaused = False
        mSecondsSoFar = 0
        mCurrJob = CType(e.Argument, cBwJob)
        mCurrJobTypeMain = CType(e.Argument, cBwJob).Job

        If mCurrJobTypeMain = JobType.COMMAND_LINE Then
            bwApp.ReportProgress(ProgressType.SEND_APP_TO_TRAY)
        End If

        mProgressDiscsCurrent = 0
        mProgressDiscsMax = 0
        mProgressTracksCurrent = 0
        mProgressTracksMax = 0

        If mBooLoadItunesError Then
            sBwAppLoadPlayer()
            mBooLoadItunesError = False
        End If

        If mCurrJobTypeMain <> JobType.INITIALIZE_PLAYER Then
            bwApp.ReportProgress(ProgressType.WRITE_APPLICATION_SETTINGS)
            If mItunesApp IsNot Nothing Then
                mTracksCount = mMainLibraryTracks.Count
            End If
        End If

        sClearLists()

        'If mBwAppDebugWriter IsNot Nothing Then
        '    mBwAppDebugWriter.WriteLine()
        'End If
        msAppendDebug("Job Started: " & mCurrJobTypeMain.ToString)

        Select Case mCurrJobTypeMain

            Case cBwJob.JobType.ADD_NEW_TRACKS
                sExecuteJob(AddressOf sBwAppAddNewFilesToLibrary)

            Case cBwJob.JobType.ADJUST_RATINGS
                mfUpdateStatusBarText("Adjusting track ratings...", False)
                sExecuteJob(AddressOf sBwAppAdjustRatings)

            Case JobType.COMMAND_LINE
                mfUpdateStatusBarText("Processing Command Line jobs...", False)
                sExecuteJob(AddressOf sBwAppCommandLineJobs, True)

            Case JobType.EDIT_SELECTED_TRACKS
                sExecuteJob(AddressOf sBwAppEditSelectedTracks)

            Case cBwJob.JobType.DELETE_DEAD_FOREIGN_TRACKS
                mfUpdateStatusBarText("Deleting dead or foreign tracks...", False)
                sExecuteJob(AddressOf fBwAppRemoveDeadForeignTracks)

            Case JobType.DELETE_EMPTY_FOLDERS
                mfUpdateStatusBarText("Deleting empty folders...", False)
                sExecuteJob(AddressOf fBwAppDeleteEmptyFolders)

            Case JobType.EXPORT_ARTWORK_BATCH
                sExecuteJob(AddressOf sBwAppExportArtwork)

            Case JobType.EXPORT_ARTWORK_MANUAL
                sExecuteJob(AddressOf sBwAppExportArtworkSelected)

            Case cBwJob.JobType.EXPORT_TRACKS
                sExecuteJob(AddressOf sBwAppExportTracks)

            Case cBwJob.JobType.FIND_NEW_TRACKS_FROM_HDD
                mfUpdateStatusBarText("Finding new tracks in HDD...", False)
                sExecuteJob(AddressOf sBwAppFindNewTracksFromHDD)

            Case JobType.IMPORT_PLAYEDCOUNT_LASTFM
                sExecuteJob(AddressOf sBwAppReverseScrobble)

            Case JobType.IMPORT_POPM_PCNT
                sExecuteJob(AddressOf Me.sBwAppImportPOPM)

            Case cBwJob.JobType.RELOAD_DISCS_TO_ALBUM_BROWSER
                sExecuteJob(AddressOf ssBwAppReloadTableDiscs)

            Case cBwJob.JobType.OFFSET_TRACKNUMBER
                sExecuteJob(AddressOf sBwAppOffsetTrackNumber)

            Case cBwJob.JobType.OVERRIDE_TAGS
                sExecuteJob(AddressOf sBwAppOverrideTags)

            Case cBwJob.JobType.RATINGS_BACKUP
                sExecuteJob(AddressOf sBwAppLibraryBackup)

            Case cBwJob.JobType.RATINGS_RESTORE
                sExecuteJob(AddressOf sBwAppLibraryRestore)

            Case cBwJob.JobType.RECOVER_TAGS
                sExecuteJob(AddressOf sBwAppRecoverTags)

            Case JobType.REMOVE_DUPLICATE_TRACKS
                sExecuteJob(AddressOf sBwAppRemoveDuplicateSongs)

            Case cBwJob.JobType.REPLACE_TRACKS
                sExecuteJob(AddressOf sBwAppReplaceTracks)

            Case cBwJob.JobType.SCHEDULE_DO
                sExecuteJob(AddressOf sBwAppScheduleTask)

            Case cBwJob.JobType.STATISTICS_DO
                mfUpdateStatusBarText("Gathering iTunes Music Library Statistics...", False)
                sExecuteJob(AddressOf sBwAppStatistics)

            Case cBwJob.JobType.SYNCHROCLEAN
                sExecuteJob(AddressOf sBwAppSynchroclean)

            Case cBwJob.JobType.VALIDATE_TRACKS_SELECTED
                mfUpdateStatusBarText("Validating Selected Tracks...", False)
                sExecuteJob(AddressOf sBwAppValidateTracksSelected)

            Case cBwJob.JobType.VALIDATE_DISC
                sExecuteJob(AddressOf sBwAppValidateDisc)

            Case cBwJob.JobType.VALIDATE_DISC_ADVANCED
                sExecuteJob(AddressOf sBwAppValidateDiscAdv)

            Case cBwJob.JobType.VALIDATE_LIBRARY
                mfUpdateStatusBarText("Validating Library...", False)
                sExecuteJob(AddressOf sBwAppValidateLibrary)

            Case JobType.WRITE_POPM_PCNT
                sExecuteJob(AddressOf sBwAppWritePOPM)

            Case cBwJob.JobType.INITIALIZE_PLAYER
                msAppendDebug("iTSfv Version: " & Application.ProductVersion)
                msAppendDebug("Logs Directory: " & My.Settings.LogsDir)
                msAppendDebug("Settings Directory: " & My.Settings.SettingsDir)
                msAppendDebug("iTMS Artwork Directory: " & My.Settings.ArtworkDir)
                msAppendDebug("Temporary Directory: " & My.Settings.TempDir)
                sBwAppLoadPlayer()

        End Select

        e.Result = mCurrJob

    End Sub

    Private Sub sBwAppReverseScrobble()

        If bwApp.CancellationPending Then
            Exit Sub
        End If

        If String.Empty = My.Settings.LastFmUserName Then
            My.Settings.LastFmUserName = InputBox("Please enter your Last.fm Profile Name", Application.ProductName)
        End If

        If String.Empty <> My.Settings.LastFmUserName Then

            Dim lastfm As New cLastFmPlayCounts(My.Settings.LastFmUserName, bwApp)

            mProgressTracksMax = mMainLibraryTracks.Count

            mfUpdateStatusBarText("Parsing iTunes Music Library...", secondary:=False)

            For i As Integer = 1 To mMainLibraryTracks.Count

                Dim track As IITTrack = mMainLibraryTracks.Item(i)

                If track.Kind = ITTrackKind.ITTrackKindFile Then

                    sPausePendingCheck()
                    If bwApp.CancellationPending = True Then
                        Exit For
                    End If

                    Dim song As IITFileOrCDTrack = CType(track, IITFileOrCDTrack)
                    Dim xt As cXmlTrack = lastfm.fFindXmlTrack(song.Artist, song.Name)

                    If xt IsNot Nothing Then

                        Dim playedCountOld As Integer = song.PlayedCount

                        If song.PlayedCount < xt.PlayedCount Or My.Settings.PlayedCountUpdateOnlyIfHigher = False Then

                            Try
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
                                    msAppendDebug(String.Format("Cleared {0} Read-Only flag", fiTrack.FullName))
                                End If

                                ''*********************
                                ''* Editing PlayedCount
                                ''*********************
                                If My.Settings.PlayedCountUpdateOnlyIfHigher = False Then

                                    If My.Settings.PlayedCountAccumilate = True Then
                                        song.PlayedCount += xt.PlayedCount
                                    Else
                                        song.PlayedCount = xt.PlayedCount
                                    End If

                                Else

                                    If song.PlayedCount < xt.PlayedCount Then

                                        If My.Settings.PlayedCountAccumilate = True Then
                                            song.PlayedCount += xt.PlayedCount
                                        Else
                                            song.PlayedCount = xt.PlayedCount
                                        End If

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
                                    msAppendDebug(String.Format("Undo clear {0} Read-Only flag", fiTrack.FullName))
                                End If

                                Dim songName As String = String.Format("""{0} - {1}""", song.Artist, song.Name)
                                mfUpdateStatusBarText("Updated PlayedCount for " + songName, secondary:=True)
                                msAppendDebug(String.Format("PlayedCount for {0} updated from {1} to {2}", songName, playedCountOld, song.PlayedCount))

                            Catch ex As Exception

                                msAppendDebug(ex.Message + " while updating PlayedCount using Last.fm")

                            End Try

                        End If

                    End If

                End If

                mProgressTracksCurrent = i

            Next

        End If

        bwApp.ReportProgress(ProgressType.READY)

    End Sub

    Private Sub sWriteErrorsLog(ByVal mySb As System.Text.StringBuilder)

        Dim lErrLog As New System.IO.StreamWriter(mFilePathErrorsLog, True)
        lErrLog.WriteLine(mySb.ToString)
        lErrLog.Close()

        mSbWarnings = New System.Text.StringBuilder ' empty the errors cache

    End Sub

    Private Sub sWriteWarningsLog(ByVal mySb As System.Text.StringBuilder)

        Dim lErrLog As New System.IO.StreamWriter(mFilePathWarningsLog, True)
        lErrLog.WriteLine(mySb.ToString)
        lErrLog.Close()

        mSbWarnings = New System.Text.StringBuilder ' empty the errors cache

    End Sub

    Private Function fJobShouldStart() As Boolean

        Return mCurrJobTypeMain = JobType.ADD_NEW_TRACKS OrElse _
        mCurrJobTypeMain = JobType.FIND_NEW_TRACKS_FROM_HDD OrElse _
        mCurrJobTypeMain = JobType.SYNCHROCLEAN OrElse _
        mCurrJobTypeMain = JobType.EXPORT_ARTWORK_MANUAL

    End Function

    Private Sub sExecuteJob(ByVal mySub As MethodInvoker, Optional ByVal cli As Boolean = False)

        Dim succ As Boolean = cli

        If mItunesApp IsNot Nothing Then
            '' 5.31.6.3 Was not possible to add new files to iTunes if the iTunes music library was empty
            If fJobShouldStart() Or mItunesApp.LibraryPlaylist.Tracks.Count > 0 Then
                succ = True
            End If
        End If

        If succ Then

            Dim startTime As Date = Now

            If mAppInfo.ApplicationState = AppInfo.SoftwareCycle.ALPHA Then

                mySub.Invoke()

            Else
                ' always report/log bug for beta/final
                Try
                    mySub.Invoke()

                    '' Dim emailCheck As Integer = CInt(9999 ^ mMainLibraryTracks.Count) '' BugReport check

                Catch ex As Exception
                    sLastCheckedTrackIdSave()
                    Dim sbErr As New System.Text.StringBuilder
                    sbErr.AppendLine("Culture: " & Threading.Thread.CurrentThread.CurrentCulture.Name)
                    sbErr.AppendLine(Environment.NewLine)
                    sbErr.AppendLine("Date and Time: " & vbTab & Now.ToString("yyyy-MM-ddTHH:mm:ss"))
                    sbErr.AppendLine("iTunes version: " & mItunesApp.Version)
                    sbErr.AppendLine(APP_ABBR_NAME_IT & " version: " & vbTab & Application.ProductVersion)
                    sbErr.AppendLine("Error caused by: ")
                    sbErr.AppendLine(String.Format("Job: {0}", mCurrJobTypeMain.ToString) & Environment.NewLine)
                    sbErr.AppendLine(String.Format("{0}", ex.Message) & Environment.NewLine)
                    sbErr.AppendLine("Error path: ")
                    sbErr.AppendLine(String.Format("{0}", ex.StackTrace) & Environment.NewLine)
                    sWriteErrorsLog(sbErr)
                    ' ask to email the bug report
                    ssReportBug(sbErr.ToString)
                End Try

            End If '' app is ALPHA

            '* AFTER VALIDATING IS COMPLETE NOW START WRITING REPORT
            If fBooReport() AndAlso bwApp.CancellationPending = False Then
                msAppendDebug("Generating XHTML Report in " + mFilePathReport)
                If fWriteReportToXhtml() = True Then
                    If My.Settings.ReportOpen = True AndAlso File.Exists(mFilePathReport) Then
                        Process.Start(mFilePathReport)
                    End If
                End If
            End If

            ' WRITE LOG FILES AFTER REPORT IS DONE
            If mCurrJobTypeMain <> cBwJob.JobType.INITIALIZE_PLAYER Then
                sWriteLogFiles()
            End If

            If mSbWarnings.Length > 0 Then
                sWriteWarningsLog(mSbWarnings)
            End If

            Dim finishTime As Date = Now
            Dim diff As TimeSpan = finishTime - startTime
            msAppendDebug(String.Format("Duration: {0}", fGetHMStoString(diff.TotalSeconds)))

            bwApp.ReportProgress(ProgressType.CLEANING_TEMP_DIR)
            msAppendDebug(String.Format("Cleaning Temporary Files... in {0}", My.Settings.TempDir))
            sCleanFSTemp()

        End If

    End Sub

    Public Sub ssReportBug(ByVal bug As String)

        If MessageBox.Show("Something unexpected happened and " + _
        Application.ProductName + " crashed. Would you like to report the bug?", _
        Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) _
        = DialogResult.Yes Then
            sEmail(bug)
        Else
            Process.Start(mFilePathErrorsLog)
        End If

    End Sub

    Public Sub sEmail(ByVal msg As String)

        If My.Settings.EmailAuto Then
            ' we use built-in email
            sEmailAuto(msg)
        Else
            sEmailManual(msg)
        End If

    End Sub

    Public Sub sEmailAuto(ByVal msg As String, Optional ByVal retry As Boolean = False)

        Dim bProceed As Boolean = True
        Dim frmEmail As New frmEmailSettings
        frmEmail.txtEmailAddress.Text = My.Settings.EmailAddress
        frmEmail.txtEmailPassword.Text = My.Settings.EmailPassword
        frmEmail.txtSMTPHost.Text = My.Settings.SMTPHost
        frmEmail.nudSMTPPort.Value = My.Settings.SMTPPort

        If retry = True Or My.Settings.EmailAddress = String.Empty Or _
            (frmEmail.EmailTypeIsWeb = True AndAlso My.Settings.EmailPassword = String.Empty) Then

            '' 5.34.6.1 iTSfv will ask for SMTP Host to automatically send bug reports
            frmEmail.ShowDialog()
            bProceed = (frmEmail.DialogResult = Windows.Forms.DialogResult.OK)

        End If

        If (String.Empty <> My.Settings.EmailAddress) And bProceed Then

            Dim message As New MailMessage(My.Settings.EmailAddress, EMAIL_ADDRESS)

            Dim client As New SmtpClient

            '' support for gmail 
            If My.Settings.EmailAddress.Contains("@gmail.com") Then

                My.Settings.SMTPHost = "smtp.gmail.com"
                My.Settings.SMTPPort = 587
                client.DeliveryMethod = SmtpDeliveryMethod.Network
                client.Credentials = New NetworkCredential(My.Settings.EmailAddress, My.Settings.EmailPassword)
                client.EnableSsl = True

            End If

            client.Host = My.Settings.SMTPHost
            client.Port = CInt(My.Settings.SMTPPort)

            Dim strSubject As String = mAppInfo.GetApplicationTitleFull
            message.Subject = strSubject
            message.BodyEncoding = System.Text.Encoding.Unicode
            message.Body = msg

            Dim zipFilePath As String = Path.ChangeExtension(mFilePathDebugLog, String.Concat(Now.ToString("mmss"), ".zip"))

            Dim temp As New List(Of String)
            temp.Add(mFilePathErrorsLog)
            temp.Add(mFilePathWarningsLog)

            'mBwAppDebugWriter.Close()

            mfUpdateStatusBarText("Zipping " + Path.GetFileName(mFilePathDebugLog), True)

            If mfZipFile(mFilePathDebugLog, zipFilePath, temp) Then

                sBarTrack.Text = mfUpdateStatusBarText("Emailing Bug Report...", True)

                message.Attachments.Add(New Attachment(zipFilePath))

                Try

                    client.Send(message)

                Catch ex As Exception

                    msAppendWarnings(ex.Message + " while emailing bug report. Retrying...")
                    sEmailAuto(msg, retry:=True)

                End Try

            End If

        End If

    End Sub

    Public Sub sEmailManual(ByVal msg As String)

        Dim strSubject As String = mAppInfo.GetApplicationTitleFull

        Try
            System.Diagnostics.Process.Start("mailto:" & EMAIL_ADDRESS & "?subject=" & strSubject + "&body=" + msg)
        Catch ex As Exception
            Dim msgBuilder As New System.Text.StringBuilder
            msgBuilder.Append("mailto:" & EMAIL_ADDRESS)
            msgBuilder.Append("&subject=" & strSubject)
            msgBuilder.AppendLine("&body=" & "Please attach " & mFilePathErrorsLog)
            msgBuilder.Append("&attach=" + mFilePathErrorsLog)
            Process.Start(msgBuilder.ToString)
        End Try

    End Sub


    Private Function fWasFileReadOnly(ByVal filePath As String) As Boolean

        Dim fi As New FileInfo(filePath)
        Dim isReadOnly As Boolean = (fi.Attributes = FileAttributes.ReadOnly Or _
               fi.Attributes = FileAttributes.Archive + FileAttributes.ReadOnly)

        If isReadOnly Then
            File.SetAttributes(filePath, FileAttributes.Normal)
        End If

        Return isReadOnly

    End Function

    Private Sub sBwAppLibraryRestore1(ByVal filePath As String)

        Dim configFile As New Xml.XmlDocument
        configFile.Load(filePath)

        Dim Node_list As Xml.XmlNodeList = configFile.GetElementsByTagName("Track")

        bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, Node_list.Count)

        For Each nd As Xml.XmlNode In Node_list

            Dim tArtist As String = nd.SelectSingleNode("Artist").InnerText
            Dim tAlbum As String = nd.SelectSingleNode("Album").InnerText
            Dim tName As String = nd.SelectSingleNode("Name").InnerText

            Dim tRating As Integer
            Integer.TryParse(nd.SelectSingleNode("Rating").InnerText, tRating)

            Dim tPlayedCount As Integer
            Integer.TryParse(nd.SelectSingleNode("PlayedCount").InnerText, tPlayedCount)

            Dim tPlayedDate As Date
            Date.TryParse(nd.SelectSingleNode("PlayedDate").InnerText, tPlayedDate)

            Dim tEQ As String = nd.SelectSingleNode("EQ").InnerText
            Dim tEnabled As Boolean = CType(nd.SelectSingleNode("Enabled").InnerText, Boolean)
            Dim tStart As Integer = CType(nd.SelectSingleNode("Start").InnerText, Integer)
            Dim tFinish As Integer = CType(nd.SelectSingleNode("Finish").InnerText, Integer)

            ' file or cd tracks
            Dim tExcludeFromShuffle As Boolean = False
            If nd.SelectSingleNode("ExcludeFromShuffle") IsNot Nothing Then
                tExcludeFromShuffle = CType(nd.SelectSingleNode("ExcludeFromShuffle").InnerText, Boolean)
            End If
            Dim tRememberBookmark As Boolean = False
            If nd.SelectSingleNode("RememberBookmark") IsNot Nothing Then
                tRememberBookmark = CType(nd.SelectSingleNode("RememberBookmark").InnerText, Boolean)
            End If
            Dim tBookmarkTime As Integer = 0
            If nd.SelectSingleNode("BookmarkTime") IsNot Nothing Then
                tBookmarkTime = CType(nd.SelectSingleNode("BookmarkTime").InnerText, Integer)
            End If

            Dim tSkippedCount As Integer = 0
            If nd.SelectSingleNode("SkippedCount") IsNot Nothing Then
                tSkippedCount = CType(nd.SelectSingleNode("SkippedCount").InnerText, Integer)
            End If

            Dim tSkippedDate As Date
            If nd.SelectSingleNode("SkippedDate") IsNot Nothing Then
                Date.TryParse(nd.SelectSingleNode("SkippedDate").InnerText, tSkippedDate)
            End If

            Dim tracksFound As IITTrackCollection

            tracksFound = mItunesApp.LibraryPlaylist.Search(tName, ITPlaylistSearchField.ITPlaylistSearchFieldSongNames)

            If tracksFound IsNot Nothing AndAlso tracksFound.Count > 0 Then

                For Each track As IITTrack In tracksFound

                    If track IsNot Nothing Then

                        If track.Album = tAlbum And track.Artist = tArtist Then

                            Try

                                Dim wasReadOnly As Boolean = False
                                Dim fiTrack As FileInfo = Nothing

                                If track.Kind = ITTrackKind.ITTrackKindFile AndAlso File.Exists(CType(track, IITFileOrCDTrack).Location) Then

                                    fiTrack = New FileInfo(CType(track, IITFileOrCDTrack).Location)
                                    wasReadOnly = fiTrack.IsReadOnly
                                    If wasReadOnly Then fiTrack.IsReadOnly = False

                                End If

                                '* 01 - RATING
                                If My.Settings.RecRating = True Then
                                    track.Rating = tRating
                                End If

                                '* 02 - PLAYED COUNT
                                If My.Settings.RecPlayedCount Then
                                    If track.PlayedCount < tPlayedCount Then track.PlayedCount = tPlayedCount
                                End If

                                '* 03 - PLAYED DATE
                                If My.Settings.RecPlayedDate Then
                                    track.PlayedDate = tPlayedDate
                                End If

                                '* 04 - EQ
                                If My.Settings.RecEQ Then
                                    track.EQ = tEQ
                                End If

                                '* 05 - Start/Finish
                                If My.Settings.RecStartFinish Then
                                    'Console.Writeline(track.Name)
                                    track.Start = tStart
                                    track.Finish = tFinish
                                End If

                                '* 06 - Enabled / 5.32.0.2 Enabled tag was backed up but was not restored
                                If My.Settings.RecEnabled Then
                                    track.Enabled = tEnabled
                                End If

                                If track.Kind = ITTrackKind.ITTrackKindFile Then

                                    ' 07 - Exclude from Shuffle
                                    If My.Settings.RecExcludeFromShuffle Then
                                        CType(track, IITFileOrCDTrack).ExcludeFromShuffle = tExcludeFromShuffle
                                    End If

                                    ' 08 - Bookmark Time
                                    If My.Settings.RecBookmarkTime Then
                                        CType(track, IITFileOrCDTrack).RememberBookmark = tRememberBookmark
                                        If tRememberBookmark = True Then
                                            CType(track, IITFileOrCDTrack).BookmarkTime = tBookmarkTime
                                        End If
                                    End If

                                    ' 09 - Skipped Count
                                    If My.Settings.RecSkippedCount Then
                                        CType(track, IITFileOrCDTrack).SkippedCount = tSkippedCount
                                    End If

                                    ' 10 - Skipped Date
                                    If My.Settings.RecSkippedDate Then
                                        CType(track, IITFileOrCDTrack).SkippedDate = tSkippedDate
                                    End If

                                End If

                                If wasReadOnly Then fiTrack.IsReadOnly = True

                                msAppendDebug(String.Format("Restored tags in ""{0}"" - ""{1}""", track.Artist, track.Name))

                                Exit For

                            Catch ex As Exception

                                msAppendWarnings(String.Format("{0} while restoring tags for {1} - {2}", ex.Message, track.Artist, track.Name))

                            End Try

                        End If ' album and artist matches

                    End If ' track is not nothing 

                Next

            End If

            bwApp.ReportProgress(ProgressType.RESTORING_RATINGS, tAlbum & " - " & tArtist)

            sPausePendingCheck()
            If bwApp.CancellationPending Then
                Exit For
            End If

        Next


    End Sub

    Private Sub sBwAppLibraryRestore2(ByVal filePath As String)

        Dim lXmlTracks As List(Of cXmlTrack) = CType(mfReadObjectFromFileBF(filePath), Global.System.Collections.Generic.List(Of Global.iTSfv.cXmlTrack))

        If lXmlTracks IsNot Nothing Then

            bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, lXmlTracks.Count)

            For Each xt As cXmlTrack In lXmlTracks

                Dim tracksFound As IITTrackCollection

                tracksFound = mItunesApp.LibraryPlaylist.Search(xt.Name, ITPlaylistSearchField.ITPlaylistSearchFieldSongNames)

                If tracksFound IsNot Nothing AndAlso tracksFound.Count > 0 Then

                    Try

                        For Each track As IITTrack In tracksFound

                            If track.Album = xt.Album And track.Artist = xt.Artist Then

                                Dim wasReadOnly As Boolean = False
                                Dim fiTrack As FileInfo = Nothing

                                If track.Kind = ITTrackKind.ITTrackKindFile AndAlso File.Exists(CType(track, IITFileOrCDTrack).Location) Then

                                    fiTrack = New FileInfo(CType(track, IITFileOrCDTrack).Location)
                                    wasReadOnly = fiTrack.IsReadOnly
                                    If wasReadOnly Then fiTrack.IsReadOnly = False

                                End If

                                '* 01 - RATING
                                If My.Settings.RecRating Then
                                    track.Rating = xt.Rating
                                End If

                                '* 02 - PLAYED COUNT
                                If My.Settings.RecPlayedCount Then
                                    If track.PlayedCount < xt.PlayedCount Then track.PlayedCount = xt.PlayedCount
                                End If

                                '* 03 - PLAYED DATE
                                If My.Settings.RecPlayedDate Then
                                    track.PlayedDate = xt.PlayedDate
                                End If

                                '* 04 - EQ
                                If My.Settings.RecEQ Then
                                    If xt.EQ IsNot Nothing Then
                                        track.EQ = xt.EQ
                                    End If
                                End If

                                '* 05 - Start/Finish
                                If My.Settings.RecStartFinish Then
                                    track.Start = xt.Start
                                    track.Finish = xt.Finish
                                End If

                                '* 06 - Enabled / 5.32.0.2 Enabled tag was backed up but was not restored
                                If My.Settings.RecEnabled Then
                                    track.Enabled = xt.Enabled
                                End If

                                If track.Kind = ITTrackKind.ITTrackKindFile Then

                                    ' 07 - Exclude from Shuffle
                                    If My.Settings.RecExcludeFromShuffle Then
                                        CType(track, IITFileOrCDTrack).ExcludeFromShuffle = xt.ExcludeFromShuffle
                                    End If

                                    ' 08 - Bookmark Time
                                    If My.Settings.RecBookmarkTime Then
                                        If xt.RememberBookmark = True Then
                                            CType(track, IITFileOrCDTrack).BookmarkTime = xt.BookmarkTime
                                        End If
                                    End If

                                    ' 09 - Skipped Count
                                    If My.Settings.RecSkippedCount Then
                                        CType(track, IITFileOrCDTrack).SkippedCount = xt.SkippedCount
                                    End If

                                    ' 10 - Skipped Date
                                    If My.Settings.RecSkippedDate Then
                                        CType(track, IITFileOrCDTrack).SkippedDate = xt.SkippedDate
                                    End If

                                End If


                                If wasReadOnly Then fiTrack.IsReadOnly = True
                                Exit For

                            End If

                        Next

                    Catch ex As Exception

                        msAppendWarnings(String.Format("{0} while restoring tags for {1} - {2}", ex.Message, xt.Artist, xt.Name))

                    End Try

                End If

                bwApp.ReportProgress(ProgressType.RESTORING_RATINGS, xt.Album & " - " & xt.Artist)

                sPausePendingCheck()
                If bwApp.CancellationPending Then
                    Exit For
                End If

            Next

        End If

    End Sub

    Private Sub sBwAppLibraryRestore()

        If mCurrJob.TaskData IsNot Nothing Then

            Dim files As String() = CType(mCurrJob.TaskData, String())

            For Each fPath As String In files

                If mfValidRestoreFile(fPath) AndAlso File.Exists(fPath) Then

                    Select Case Path.GetExtension(fPath)

                        Case ".xml"
                            sBwAppLibraryRestore1(fPath)
                        Case ".tags-cache"
                            sBwAppLibraryRestore2(fPath)

                    End Select

                End If

            Next

        End If

        bwApp.ReportProgress(ProgressType.READY)

    End Sub

    Private Sub sBwAppLibraryBackup1(ByVal myTracks As IITTrackCollection)

        Dim configFile As New Xml.XmlDocument

        Dim Node_root As Xml.XmlNode = configFile.AppendChild(configFile.CreateElement("LibraryPlaylist"))

        For Each track As IITTrack In myTracks

            '' 5.34.1.3 iTSfv backed up tracks only if they were played at least once or rating greater than zero [Jojo]
            'If track.Rating > 0 Or track.PlayedCount > 0 Then

            '' 5.34.2.0 Option to update database refreshing tags before backing up tags
            If My.Settings.UpdateDatabaseBeforeBackup Then
                If track.Kind = ITTrackKind.ITTrackKindFile Then
                    mfUpdateInfoFromFile(CType(track, IITFileOrCDTrack))
                End If
            End If

            Dim Node_Track As Xml.XmlNode = configFile.CreateElement("Track")
            Node_root.AppendChild(Node_Track)

            Dim Node_artist As Xml.XmlElement
            Node_artist = configFile.CreateElement("Artist")
            Node_artist.InnerText = track.Artist
            Node_Track.AppendChild(Node_artist)

            Dim Node_album As Xml.XmlElement
            Node_album = configFile.CreateElement("Album")
            Node_album.InnerText = track.Album
            Node_Track.AppendChild(Node_album)

            Dim Node_name As Xml.XmlElement
            Node_name = configFile.CreateElement("Name")
            Node_name.InnerText = track.Name
            Node_Track.AppendChild(Node_name)

            Dim Node_Enabled As Xml.XmlElement
            Node_Enabled = configFile.CreateElement("Enabled")
            Node_Enabled.InnerText = track.Enabled.ToString
            Node_Track.AppendChild(Node_Enabled)

            If track.Kind = ITTrackKind.ITTrackKindFile Then

                Dim Node_RememberBookmark As Xml.XmlElement
                Node_RememberBookmark = configFile.CreateElement("RememberBookmark")
                Node_RememberBookmark.InnerText = CType(track, IITFileOrCDTrack).RememberBookmark.ToString
                Node_Track.AppendChild(Node_RememberBookmark)

                Dim Node_Bookmark As Xml.XmlElement
                Node_Bookmark = configFile.CreateElement("BookmarkTime")
                Node_Bookmark.InnerText = CType(track, IITFileOrCDTrack).BookmarkTime.ToString
                Node_Track.AppendChild(Node_Bookmark)

                Dim Node_ShuffleExclude As Xml.XmlElement
                Node_ShuffleExclude = configFile.CreateElement("ExcludeFromShuffle")
                Node_ShuffleExclude.InnerText = CType(track, IITFileOrCDTrack).ExcludeFromShuffle.ToString
                Node_Track.AppendChild(Node_ShuffleExclude)

                Dim Node_SkippedCount As Xml.XmlElement
                Node_SkippedCount = configFile.CreateElement("SkippedCount")
                Node_SkippedCount.InnerText = CType(track, IITFileOrCDTrack).SkippedCount.ToString
                Node_Track.AppendChild(Node_SkippedCount)

                Dim Node_SkippedDate As Xml.XmlElement
                Node_SkippedDate = configFile.CreateElement("SkippedDate")
                Node_SkippedDate.InnerText = CType(track, IITFileOrCDTrack).SkippedDate.ToString
                Node_Track.AppendChild(Node_SkippedDate)

            End If

            Dim Node_EQ As Xml.XmlElement
            Node_EQ = configFile.CreateElement("EQ")
            Node_EQ.InnerText = track.EQ
            Node_Track.AppendChild(Node_EQ)

            Dim Node_rating As Xml.XmlElement
            Node_rating = configFile.CreateElement("Rating")
            Node_rating.InnerText = track.Rating.ToString
            Node_Track.AppendChild(Node_rating)

            Dim Node_PlayedCount As Xml.XmlElement
            Node_PlayedCount = configFile.CreateElement("PlayedCount")
            Node_PlayedCount.InnerText = track.PlayedCount.ToString
            Node_Track.AppendChild(Node_PlayedCount)

            Dim Node_PlayedDate As Xml.XmlElement
            Node_PlayedDate = configFile.CreateElement("PlayedDate")
            Node_PlayedDate.InnerText = track.PlayedDate.ToString
            Node_Track.AppendChild(Node_PlayedDate)

            Dim Node_Start As Xml.XmlElement
            Node_Start = configFile.CreateElement("Start")
            Node_Start.InnerText = track.Start.ToString
            Node_Track.AppendChild(Node_Start)

            Dim Node_Finish As Xml.XmlElement
            Node_Finish = configFile.CreateElement("Finish")
            Node_Finish.InnerText = track.Finish.ToString
            Node_Track.AppendChild(Node_Finish)

            ' End If

            bwApp.ReportProgress(ProgressType.BACKINGUP_RATINGS, mLibraryTasks.fGetDiscName(track))

            sPausePendingCheck()
            If bwApp.CancellationPending Then
                Exit For
            End If

        Next

        Try
            Dim myWriter As Xml.XmlTextWriter
            myWriter = New Xml.XmlTextWriter(mFilePathRatingsBR, System.Text.Encoding.UTF8)
            myWriter.Formatting = Xml.Formatting.Indented
            configFile.Save(myWriter)
            myWriter.Close()
            msAppendDebug(String.Format("Saved tag information of {0} tracks to {1}", myTracks.Count, mFilePathRatingsBR))
        Catch ex As Exception
            msAppendWarnings(ex.Message + " while backing1 up tags")
        End Try

        bwApp.ReportProgress(ProgressType.READY)

    End Sub

    Private Function sBwAppLibraryBackup2(ByVal myTracks As IITTrackCollection) As Boolean

        Dim succ As Boolean = False

        If myTracks IsNot Nothing Then

            Dim lXmlTracks As New List(Of cXmlTrack)

            For Each track As IITTrack In myTracks
                If track IsNot Nothing Then

                    '' 5.34.2.0 Option to update database refreshing tags before backing up tags
                    If My.Settings.UpdateDatabaseBeforeBackup Then
                        If track.Kind = ITTrackKind.ITTrackKindFile Then
                            mfUpdateInfoFromFile(CType(track, IITFileOrCDTrack))
                        End If
                    End If

                    If track.Kind = ITTrackKind.ITTrackKindFile Then
                        lXmlTracks.Add(New cXmlTrack(CType(track, IITFileOrCDTrack), False))
                    Else
                        lXmlTracks.Add(New cXmlTrack(track, False))
                    End If

                    succ = True
                    bwApp.ReportProgress(ProgressType.BACKINGUP_RATINGS, mLibraryTasks.fGetDiscName(track))

                    sPausePendingCheck()
                    If bwApp.CancellationPending Then
                        Exit For
                    End If

                End If
            Next

            msAppendDebug(String.Format("Saving tag information of {0} tracks to {1}", myTracks.Count, mFilePathRatingsBR))
            mfWriteObjectToFileBF(lXmlTracks, mFilePathRatingsBR)

            Return succ

        End If

    End Function

    Private Sub sBwAppLibraryBackup(ByVal myTracks As IITTrackCollection)

        If myTracks IsNot Nothing Then
            bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, myTracks.Count)
        End If

        Select Case Path.GetExtension(mFilePathRatingsBR)

            Case ".xml"
                sBwAppLibraryBackup1(myTracks)
            Case ".tags-cache"
                sBwAppLibraryBackup2(myTracks)

        End Select

    End Sub

    Private Sub sBwAppLibraryBackup()

        If rbLibrary.Checked Then

            sBwAppLibraryBackup(mMainLibraryTracks)

        ElseIf mItunesApp.BrowserWindow.SelectedTracks IsNot Nothing Then

            sBwAppLibraryBackup(mItunesApp.BrowserWindow.SelectedTracks)

        End If

    End Sub

    Private Function fWriteTrackCountEtc(ByVal track As IITFileOrCDTrack, ByVal lDisc As cInfoDisc) As Boolean

        Dim booTrackEdited As Boolean = False

        Dim lDiscTitle As String = lDisc.Name
        Dim lTracksCount As Integer = lDisc.Tracks.Count
        Dim lTrackCountMax As Integer = lDisc.TrackCount
        Dim lAlbumIsComplete As Boolean = lDisc.IsComplete
        Dim fiTrack As New FileInfo(track.Location)
        Dim wasReadOnly As Boolean = fiTrack.IsReadOnly
        If wasReadOnly Then fiTrack.IsReadOnly = False

        ' track count is not set
        ' album is incomplete even if fully scanned 
        ' track count is less than maximum known track count
        ' album is complete and track count is high than album track count

        Dim f As TagLib.File = TagLib.File.Create(track.Location)

        If Not f.Tag.TrackCount > 0 Or _
            (lAlbumIsComplete = False AndAlso chkResume.Checked = False) Or _
             f.Tag.TrackCount < lTrackCountMax Then
            track.TrackCount = lTrackCountMax
            f.Tag.TrackCount = CUInt(lTrackCountMax)
            booTrackEdited = True
        ElseIf fIsConsequetiveTracksAlbum(lDisc) AndAlso track.TrackCount <> lTracksCount Then
            Dim lTrackNumber As Integer = lTracksCount - (track.TrackCount - track.TrackNumber)
            If track.TrackNumber > lTracksCount Then
                track.TrackNumber = CType(lTrackNumber, Integer)
                booTrackEdited = True
            End If
            If lAlbumIsComplete Then
                track.TrackCount = lTracksCount
                booTrackEdited = True
            End If
        End If

        If track.DiscCount <> lDisc.DiscCount Or track.DiscNumber <> lDisc.DiscNumber Then
            bwApp.ReportProgress(ProgressType.WRITING_TAGS_TO_TRACKS, track.Name)
        End If

        If (My.Settings.DisableGroupedDiscCountUpdate = True AndAlso track.Grouping = "") Or lAlbumIsComplete = True Then

            If f.Tag.DiscCount <> lDisc.DiscCount Then
                track.DiscCount = lDisc.DiscCount
                booTrackEdited = True
            End If

        End If

        If f.Tag.Disc <> lDisc.DiscNumber Then
            track.DiscNumber = lDisc.DiscNumber
            booTrackEdited = True
        End If

        If booTrackEdited Then
            ' save the taglib way as well
            f.Tag.TrackCount = CUInt(track.TrackCount) '' very strange bug where TrackCount would be cleared otherwise
            f.Tag.Track = CUInt(track.TrackNumber) '' very strange bug where TrackNumber would be cleared otherwise
            f.Tag.Disc = CUInt(lDisc.DiscNumber)
            f.Tag.DiscCount = CUInt(lDisc.DiscCount)
            f.Save()
            mListTracksCountUpdated.Add(track.Location)
        End If

        If wasReadOnly Then fiTrack.IsReadOnly = True

        If Not lAlbumIsComplete Then
            If Not mListAlbumsInconsecutiveTracks.Contains(lDiscTitle) Then
                mListAlbumsInconsecutiveTracks.Add(lDiscTitle)
            End If
        End If

    End Function

    Private Function fFillTrackCountEtc(ByRef lDisc As cInfoDisc) As Boolean

        Dim succ As Boolean = False

        Try

            Dim lAlbumTitle As String = lDisc.Name
            Dim titleAlbum As String = mLibraryTasks.fGetAlbumName(lDisc.FirstTrack)

            If lDisc.IsComplete = False Then
                msAppendDebug("Warning: Album is flagged as Incomplete.")
            End If

            Dim lTracksCount As Integer = lDisc.Tracks.Count
            Dim lTrackCountMax As Integer = lDisc.Tracks.Count
            Dim lDiscNumber As Integer = 1
            Dim lDiscCountMax As Integer = 1
            If mListAlbumKeys.Contains(lAlbumTitle) Then
                lDiscCountMax = CType(mTableAlbums.Item(titleAlbum), cInfoAlbum).Discs.Count
            End If

            For Each track As IITFileOrCDTrack In lDisc.Tracks
                ' save max TrackCount
                lTrackCountMax = Math.Max(Math.Max(lTracksCount, track.TrackNumber), track.TrackCount)
                ' save max disc number
                lDiscNumber = Math.Max(lDiscNumber, track.DiscNumber)
                ' save max DiscCount from previous DiscCount, tracksDiscNumber and track.DiscCount
                lDiscCountMax = Math.Max(Math.Max(lDiscCountMax, track.DiscNumber), track.DiscCount)
            Next

            ' TRACK COUNT
            lDisc.TrackCount = lTrackCountMax
            ' DISC NUMBER
            lDisc.DiscNumber = lDiscNumber
            ' DISC COUNT
            lDisc.DiscCount = lDiscCountMax

            'bwApp.ReportProgress(ProgressType.UPDATE_TRACKS_PROGRESS_BAR_MAX, lDisc.Tracks.Count)

        Catch ex As Exception
            msAppendWarnings(String.Format("{0} while populating TrackCount, DiscNumber and DiscCount for {1}", ex.Message, lDisc.DiscName))
        End Try

    End Function

    Private Sub sLoadDiscToAlbumsTable(ByVal disc As cInfoDisc)

        Dim lAlbum As New cInfoAlbum(disc.AlbumName)

        Dim albumTitle As String = disc.AlbumName

        ' update album titles
        If Not mListAlbumKeys.Contains(albumTitle) Then
            mListAlbumKeys.Add(albumTitle)
        End If

        ' check if album already exists
        If (mTableAlbums.ContainsKey(albumTitle)) Then
            lAlbum = CType(mTableAlbums.Item(albumTitle), cInfoAlbum)
        Else
            mTableAlbums.Add(albumTitle, lAlbum)
        End If

        If Not (lAlbum.HasDisc(disc)) Then
            lAlbum.Discs.Add(disc)
        End If

    End Sub

    Private Function sTagBlankAlbum(ByVal track As IITFileOrCDTrack) As Boolean

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

    Private Function fBooForceSingleDisc() As Boolean

        Return chkDiscComplete.Checked AndAlso mCurrJobTypeMain = JobType.VALIDATE_TRACKS_SELECTED

    End Function

    Private Sub sLoadTrackToDiscsTable(ByVal track As IITFileOrCDTrack)

        Dim locTrack As String = "dead track"
        Try
            locTrack = track.Location
        Catch ex As Exception
        End Try

        Try

            If mfOKtoValidate(track) Then

                Dim lDisc As New cInfoDisc

                If track.Album <> String.Empty Then

                    If fBooForceSingleDisc() AndAlso mListAlbumKeys.Count = 1 Then
                        lDisc.AlbumName = mListAlbumKeys(0)
                    Else
                        ' 5.6.1.2 DiscCount was wrong for album names shared by different artists [ffs]
                        lDisc.AlbumName = mLibraryTasks.fGetAlbumName(track)
                    End If

                Else
                    If My.Settings.TagBlankAlbum Then
                        ' 5.7.4.5 Tagging "Unknown Album" to songs where the album tag is blank is now optional [Bluenote]
                        sTagBlankAlbum(track)
                    End If
                End If

                If fBooForceSingleDisc() AndAlso mListDiscKeys.Count = 1 Then
                    lDisc.Name = mListDiscKeys(0)
                Else
                    lDisc.Name = mLibraryTasks.fGetDiscName(track) ' not the same as track.album
                End If

                If lDisc.Name <> String.Empty Then

                    Dim discTitle As String = lDisc.Name

                    If Not mListDiscKeys.Contains(discTitle) Then
                        mListDiscKeys.Add(discTitle)
                    End If

                    ' check if album already exists
                    If (mTableDiscs.ContainsKey(discTitle)) Then
                        ' album is in the album list 
                        lDisc = CType(mTableDiscs.Item(discTitle), cInfoDisc)
                    Else
                        ' create new album and add to album list  
                        mTableDiscs.Add(discTitle, lDisc)
                        bwApp.ReportProgress(ProgressType.READ_TRACKS_FROM_DISCS, mLibraryTasks.fGetDiscName(track))
                    End If

                    ' check if track already is there 
                    If Not lDisc.HasTrack(track) Then
                        lDisc.Tracks.Add(track)
                    End If

                End If

                If lDisc.Genre = String.Empty Then
                    lDisc.Genre = track.Genre
                End If

                If lDisc.Year = 0 Then
                    lDisc.Year = track.Year
                End If

                ' load the disc to album
                sLoadDiscToAlbumsTable(lDisc)

            End If

        Catch ex As Exception

            msAppendDebug(String.Format("Error occured while reading {0}, {1}", locTrack, ex.Message))

        End Try

    End Sub

    Private Sub sBwAppLoadDiscsToAlbumBrowser()

        ' adding tracks to listbox *after* reading *all* the tracks makes it possible to 
        ' browse the album folder while bwapp is busy

        bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, mTableDiscs.Count)

        For i As Integer = 0 To mTableDiscs.Count - 1
            Dim lDisc As cInfoDisc = CType(mTableDiscs.Item(mListDiscKeys(i)), cInfoDisc)
            ' fill extra info because now reading tracks is done
            lDisc.IsComplete = fIsCompleteAlbum(lDisc)
            bwApp.ReportProgress(ProgressType.ADD_DISC_TO_LISTBOX_DISCS, lDisc.Name)
        Next

        bwApp.ReportProgress(ProgressType.READY, String.Format("Loaded {0} Discs.", mTableDiscs.Count))

    End Sub

    Public Sub sReloadDiscsToAlbumBrowser()

        ' set public for accessing via options
        ' reloading will never initiate by the bwapp
        ' or while bwapp is busy because it will stuff up 
        ' all disc count etc 

        If Not bwApp.IsBusy Then
            If lbDiscs.Items.Count > 0 Then
                lbDiscs.Items.Clear()
                Dim task As New cBwJob(cBwJob.JobType.RELOAD_DISCS_TO_ALBUM_BROWSER)
                bwApp.RunWorkerAsync(task)
            End If
        End If

    End Sub

    Private Sub ssBwAppReloadTableDiscs()

        Dim lTableDiscs As New Hashtable
        Dim lListDiscKeys As New List(Of String)

        bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, mTableDiscs.Count)

        For i As Integer = 0 To mTableDiscs.Count - 1

            ' grab old disc
            Dim disc As cInfoDisc = CType(mTableDiscs.Item(mListDiscKeys(i)), cInfoDisc)

            ' change disc name 
            Dim bFound As Boolean = False
            Dim tracksClone As New List(Of IITFileOrCDTrack)
            tracksClone.AddRange(disc.Tracks)
            For Each track As IITFileOrCDTrack In tracksClone
                Try
                    If File.Exists(track.Location) Then
                        disc.Name = mLibraryTasks.fGetDiscName(track)
                        bFound = True
                        Exit For
                    End If
                Catch ex As System.Runtime.InteropServices.COMException
                    disc.Tracks.Remove(track)
                End Try
            Next
            ' add disc only if at least one track is found
            If bFound = True Then
                ' add new keys                
                lListDiscKeys.Add(disc.Name)
                ' add changed new name
                If lListDiscKeys.Count > i Then
                    ' 5.23.1.1 Fixed possible crash due to Index was out of range error if tracks were deleted while reloading albums 
                    If lTableDiscs.Contains(lListDiscKeys(i)) = False Then
                        ' 5.37.1.3 Fixed crash during reloading Discs Browser after tags are changed: Item has already been added. 
                        lTableDiscs.Add(lListDiscKeys(i), disc)
                    Else
                        lTableDiscs(lListDiscKeys(i)) = disc
                    End If
                End If
                ' increment progress bar
                bwApp.ReportProgress(ProgressType.READ_TRACKS_FROM_DISCS, disc.Name)
            End If

        Next

        mTableDiscs = lTableDiscs
        mListDiscKeys = lListDiscKeys

        ' after reloading tracks to discs, load the discs to listbox
        sBwAppLoadDiscsToAlbumBrowser()

    End Sub

    Private Function fIsCompleteAlbum(ByVal lAlbum As cInfoDisc) As Boolean

        Dim booComplete As Boolean = False

        Dim trackNumbers As New List(Of Integer)
        For Each track As IITFileOrCDTrack In lAlbum.Tracks
            Try ' to prevent track deleted error 
                If track.Location IsNot Nothing Then trackNumbers.Add(track.TrackNumber)
            Catch ex As Exception
                msAppendWarnings(ex.Message)
            End Try
        Next

        trackNumbers.Sort()

        If trackNumbers.Count > 0 Then
            booComplete = (1 = trackNumbers(0)) AndAlso fIsConsequetiveTracksAlbum(lAlbum)
        End If

        Return booComplete

    End Function

    Private Function fIsConsequetiveTracksAlbum(ByVal lAlbum As cInfoDisc) As Boolean

        Dim booAlbumFull As Boolean = False

        Try
            Dim trackNumbers As New List(Of Integer)
            For Each track As IITFileOrCDTrack In lAlbum.Tracks
                If track.Location IsNot Nothing Then trackNumbers.Add(track.TrackNumber)
            Next
            trackNumbers.Sort()

            ' 4.0.9.1 Albums with Consecutive track numbers but missing first track was regarded as a complete album
            ' Dim booAlbumFull As Boolean = True
            booAlbumFull = (1 = trackNumbers(0))

            For i As Integer = 0 To trackNumbers.Count - 2
                booAlbumFull = booAlbumFull And (trackNumbers(i + 1) - trackNumbers(i) = 1)
            Next
        Catch ex As Exception
            ' we will implement an error handled IITFileOrCDTrack by iTSfv 6.0
        End Try

        Return booAlbumFull

    End Function

    Private Function ssBwAppDeleteDeadTracks(ByVal bDeleteTracksNotInHDD As Boolean, _
                                             ByVal bDeleteNonMusicFolderTracks As Boolean, _
                                             Optional ByVal lTracks As IITTrackCollection = Nothing) As Boolean

        Dim success As Boolean = True

        For Each musicFolder As String In My.Settings.MusicFolderLocations

            If musicFolder.EndsWith(Path.DirectorySeparatorChar) = False Then
                musicFolder += Path.DirectorySeparatorChar
            End If

            If Directory.Exists(musicFolder) = False Then
                msAppendDebug("One or more music folders are inaccessible. Quitting removal of missing tracks...")
                success = False
                Exit For
            End If

        Next

        If success Then

            If lTracks Is Nothing Then
                lTracks = mMainLibraryTracks
            End If

            If bDeleteTracksNotInHDD = True OrElse bDeleteNonMusicFolderTracks = True Then

                msAppendDebug("Looking for tracks outside of music folders to remove")

                If chkResume.Checked Then
                    bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, mMainLibraryTracks.Count - mLastCheckedTrackID + 1)
                Else
                    bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, mMainLibraryTracks.Count)
                End If

                For i As Integer = lTracks.Count To 1 Step -1

                    sPausePendingCheck()
                    If bwApp.CancellationPending Then
                        bwApp.ReportProgress(ProgressType.READY)
                        Exit Function
                    End If

                    Dim track As IITTrack = lTracks.Item(i)

                    If track.Kind = ITTrackKind.ITTrackKindFile Then

                        Dim booTrackDeleted As Boolean = False

                        If bDeleteNonMusicFolderTracks = True Then
                            bwApp.ReportProgress(ProgressType.DELETE_TRACKS_DEAD_ALIEN, track.Album)
                        Else
                            bwApp.ReportProgress(ProgressType.DELETE_TRACKS_DEAD, track.Album)
                        End If

                        If bDeleteNonMusicFolderTracks = True AndAlso bDeleteNonMusicFolderTracks = True Then
                            '**************************
                            ' DELETE NON-MUSIC-FOLDER
                            '**************************
                            booTrackDeleted = mfDeleteNonMusicFolderTrack(CType(track, IITFileOrCDTrack))
                        End If

                        If bDeleteTracksNotInHDD = True Then
                            '**************************
                            ' DELETE NON-EXISTANT FILES
                            '**************************
                            If booTrackDeleted = False Then
                                booTrackDeleted = booTrackDeleted And mfDeleteTrackNotInHDD(CType(track, IITFileOrCDTrack))
                            End If
                        End If

                        mProgressDiscsCurrent += 1

                    End If

                Next

                bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, mMainLibraryTracks.Count)

                Dim msg As String = String.Format("Removed {0} dead and {1} foreign tracks", mListFileNotFound.Count, mListTracksNonMusicFolder.Count)
                msAppendDebug(msg)

                bwApp.ReportProgress(ProgressType.READY, msg)

            End If

        End If


    End Function



    Public Function fGetRandomNumber(ByVal MaxNumber As Integer, _
    Optional ByVal MinNumber As Integer = 0) As Integer

        'initialize random number generator
        Dim r As New Random(System.DateTime.Now.Millisecond)

        'if passed incorrect arguments, swap them
        'can also throw exception or return 0

        If MinNumber > MaxNumber Then
            Dim t As Integer = MinNumber
            MinNumber = MaxNumber
            MaxNumber = t
        End If

        Return r.Next(MinNumber, MaxNumber)

    End Function

    Private Sub sPausePendingCheck()

        '' 5.34.14.1 Pressing Stop button did not pause the currently active job [Jojo]

        If mJobPaused And bwApp.CancellationPending = False Then
            Threading.Thread.Sleep(2000)
            Call sPausePendingCheck()
        End If

    End Sub

    Private Sub sLoadAlbumsTable()

        Dim tracksInHashTable As Integer = fGetNumberOfTracksInHashTable()

        '* DELETE UNWANTED TRACKS FIRST

        If bwApp.CancellationPending Then
            Exit Sub
        End If

        bwApp.ReportProgress(ProgressType.CLEAR_DISCS_LISTBOX)

        mLastCheckedTrackID = fGetLastCheckTrackID()

        ' add from first track to last track. we reverse in albums. 

        bwApp.ReportProgress(ProgressType.UPDATE_TRACKS_PROGRESS_BAR_MAX, mMainLibraryTracks.Count - mLastCheckedTrackID)

        For i As Integer = mLastCheckedTrackID To mMainLibraryTracks.Count

            sPausePendingCheck()
            ' now when the sub is cancelled at this point the album is gonna be incomplete
            If bwApp.CancellationPending Then
                bwApp.ReportProgress(ProgressType.READY)
                Exit Sub
            End If

            Dim track As iTunesLib.IITTrack = mMainLibraryTracks(i)

            If track.Kind = ITTrackKind.ITTrackKindFile Then

                ' this means every song in the album browser are IITFileOrCDTrack
                sLoadTrackToDiscsTable(CType(track, IITFileOrCDTrack))

            End If

            bwApp.ReportProgress(ProgressType.INCREMENT_TRACK_PROGRESS)

        Next

        bwApp.ReportProgress(ProgressType.READY)

    End Sub



    Private Function fGetNumberOfTracksInHashTable() As Integer

        Dim countTracks As Integer = 0

        If mTableDiscs.Count > 0 And mListDiscKeys.Count > 0 Then

            Dim lAlbum As New cInfoDisc
            For i As Integer = 0 To mTableDiscs.Count - 1
                lAlbum = CType(mTableDiscs(mListDiscKeys(i)), cInfoDisc)
                ' If lAlbum IsNot Nothing Then
                countTracks += lAlbum.Tracks.Count
                'End If
            Next

        End If

        Return countTracks

    End Function

    Private Sub sBwAppOffsetTrackNumber()

        Dim tracks As IITTrackCollection
        tracks = mItunesApp.BrowserWindow.SelectedTracks

        bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, tracks.Count)

        If tracks IsNot Nothing Then

            For Each track As iTunesLib.IITFileOrCDTrack In tracks

                track.TrackNumber = CType(track.TrackNumber + nudOffsetTrackNum.Value, Integer)

                bwApp.ReportProgress(ProgressType.OFFSET_TRACKNUMBER, track.Name)

            Next

            bwApp.ReportProgress(ProgressType.READY, String.Format(" Offsetted {0} tracks.", tracks.Count))

        Else

            bwApp.ReportProgress(ProgressType.READY)

        End If


    End Sub

    Private Sub ssReplaceTextInTags(ByVal track As IITFileOrCDTrack)

        If mFindText.Length > 0 Then

            If chkName.Checked Then
                Dim o As String = track.Name
                Dim n As String = track.Name.Replace(mFindText, mReplaceText).Trim
                track.Name = n
                If o <> n Then
                    msAppendDebug(String.Format("""{0}"" is now ""{1}""", o, n))
                End If
            End If

            If chkArtist.Checked Then
                track.Artist = track.Artist.Replace(mFindText, mReplaceText).Trim
            End If

            If chkAlbum.Checked Then
                track.Album = track.Album.Replace(mFindText, mReplaceText).Trim
            End If

            If chkGenre.Checked Then
                track.Genre = track.Genre.Replace(mFindText, mReplaceText).Trim
            End If

            bwApp.ReportProgress(ProgressType.REPLACING_TAGS, track.Name)

        End If

    End Sub

    Private Sub bwApp_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles bwApp.ProgressChanged

        Try

            mCurrProgress = CType(e.ProgressPercentage, ProgressType)
            Me.Cursor = Cursors.AppStarting

            Dim userStateString As String = ""
            If e.UserState IsNot Nothing AndAlso TypeOf (e.UserState) Is String Then
                userStateString = e.UserState.ToString
            End If

            If mSbDebug.Length > 0 Then
                My.Forms.frmDebug.txtDebug.Text = mSbDebug.ToString
            End If

            ''Console.Writeline(mStatus.ToString)

            Select Case mCurrProgress

                Case ProgressType.ADD_DISC_TO_LISTBOX_DISCS
                    chkCheckArtworkLowRes.Enabled = False
                    'pBarDiscs.Style = ProgressBarStyle.Continuous
                    'pBarDiscs.Increment(1)
                    mProgressDiscsCurrent += 1
                    Dim lDiscTitle As String = e.UserState.ToString
                    If Not lbDiscs.Items.Contains(lDiscTitle) Then
                        mfUpdateStatusBarText("Loading " & lDiscTitle & " to Album Browser...", True)
                        lbDiscs.Items.Add(lDiscTitle)
                    End If

                Case ProgressType.ADD_TRACKS_TO_LISTBOX_TRACKS
                    sQueueFileToListBoxTracks(userStateString) 'filePath

                Case ProgressType.ADD_TRACKS_TO_LIBRARY
                    'pBarDiscs.Style = ProgressBarStyle.Continuous
                    'pBarDiscs.Increment(1)
                    mProgressDiscsCurrent += 1
                    Dim filePath As String = e.UserState.ToString
                    If lbFiles.Items.Contains(filePath) Then lbFiles.Items.Remove(filePath)
                    mfUpdateStatusBarText(String.Format("Adding {0} to iTunes Library...", userStateString), secondary:=False)

                Case ProgressType.ADJUSTING_RATING
                    Dim track As IITFileOrCDTrack = CType(e.UserState, IITFileOrCDTrack)
                    mfUpdateStatusBarText(String.Format("Adjusting My Rating: {0} - {1}. ", mGetAlbumArtist(track), track.Name), False)

                Case ProgressType.ANALYSING_ALBUM
                    'pBarDiscs.Style = ProgressBarStyle.Continuous
                    'pBarDiscs.Increment(1)
                    mProgressDiscsCurrent += 1
                    mfUpdateStatusBarText(String.Format("Analysing album: {0}", userStateString), False)

                Case ProgressType.APPEND_PREPEND_CHAR
                    Dim sap As String = If(cboAppendChar.SelectedIndex = 0, "Appending", "Prepending")
                    mfUpdateStatusBarText(String.Format("{0} ""{1}"" to ""{2}""", sap, txtAppend.Text, userStateString), True)

                Case ProgressType.BACKINGUP_RATINGS
                    'pBarDiscs.Style = ProgressBarStyle.Continuous
                    'pBarDiscs.Increment(1)
                    mProgressDiscsCurrent += 1
                    mfUpdateStatusBarText(String.Format("Saving ratings for tracks in album: {0}", userStateString), False)

                Case ProgressType.CAPITALIZE_FIRST_LETTER
                    mProgressDiscsCurrent += 1
                    mfUpdateStatusBarText("Capitalizing " & userStateString, False)

                Case ProgressType.CLEANING_TEMP_DIR
                    pBarDiscs.Style = ProgressBarStyle.Marquee
                    pBarDiscs.ToolTipText = "Cleaning Temporary Files..."

                Case ProgressType.CLEAR_DISCS_LISTBOX
                    lbDiscs.Items.Clear()

                Case ProgressType.CLEAR_TRACKS_LISTBOX
                    lbFiles.Items.Clear()

                Case ProgressType.DECOMPILE_TRACKS
                    mfUpdateStatusBarText(String.Format("Decompiling track: {0}", userStateString), True)

                Case ProgressType.DELETE_ARTWORK
                    mProgressTracksCurrent += 1
                    mfUpdateStatusBarText(String.Format("Deleting existing Artwork in {0}", userStateString), True)

                Case ProgressType.DELETE_TRACKS_DEAD_ALIEN
                    mfUpdateStatusBarText(String.Format("Checking dead or foreign tracks to delete in: {0}", userStateString), True)

                Case ProgressType.DELETE_TRACKS_DEAD
                    mfUpdateStatusBarText(String.Format("Checking dead tracks to delete in: {0}", userStateString), True)

                Case ProgressType.DETERINE_WHERE_MOST_MUSIC_IS
                    'pBarDiscs.Style = ProgressBarStyle.Continuous
                    'pBarDiscs.Increment(1)
                    mfUpdateStatusBarText(String.Format("Determining where most of the music is... {0}", e.UserState.ToString), False)

                Case ProgressType.EDITING_SELECTED_TRACKS
                    'pbarTracks.Style = ProgressBarStyle.Continuous
                    'pbarTracks.Increment(1)
                    mProgressTracksCurrent += 1

                Case ProgressType.EMAIL_SENDING
                    pBarDiscs.Style = ProgressBarStyle.Marquee
                    mfUpdateStatusBarText("Emailing " + userStateString, False)

                Case ProgressType.EXPORT_TRACKS
                    'pBarDiscs.Style = ProgressBarStyle.Continuous
                    'pBarDiscs.Increment(1)
                    mProgressDiscsCurrent += 1
                    mfUpdateStatusBarText(String.Format("Exporting {0} to {1}", e.UserState.ToString, mCurrJob.mMessage), False)

                Case ProgressType.FOUND_LYRICS_FOR
                    mfUpdateStatusBarText(String.Format("Found Lyrics for {0}", e.UserState.ToString), False)

                Case ProgressType.GETTING_TRACK_INFO
                    pbarTrack.Style = ProgressBarStyle.Marquee
                    mfUpdateStatusBarText("Getting " & userStateString & " info", False)

                Case ProgressType.IMPORTING_ARTWORK
                    If TypeOf (e.UserState) Is String Then
                        mfUpdateStatusBarText(userStateString, True)
                    End If

                Case ProgressType.INCREMENT_DISC_PROGRESS
                    mProgressDiscsCurrent += 1

                Case ProgressType.INCREMENT_TRACK_PROGRESS
                    mProgressTracksCurrent += 1

                Case ProgressType.INITIALIZE_PLAYER_LIBRARY_START
                    pBarDiscs.Style = ProgressBarStyle.Marquee
                    mfUpdateStatusBarText(String.Format("Initializing {0} Library.", mpCurrentPlayer), False)

                Case ProgressType.INITIALIZE_ITUNES_XML_DATABASE_START
                    pBarDiscs.Style = ProgressBarStyle.Marquee
                    mfUpdateStatusBarText("Initializing iTunes XML Database.", False)

                Case ProgressType.INITIALIZE_ITUNES_ERROR
                    Dim ex As Exception = CType(e.UserState, Exception)
                    mfUpdateStatusBarText(ex.Message, False)

                Case ProgressType.INITIALIZE_PLAYER_FINISH
                    mfUpdateStatusBarText("Ready. Found " & mMainLibraryTracks.Count & " Tracks.", False)

                    My.Forms.frmOptions.txtMusicFolder.Text = mpMusicFolderPath

                    If Directory.Exists(mpMusicFolderPath) Then
                        If Not My.Forms.frmOptions.lbMusicFolders.Items.Contains(mpMusicFolderPath) Then
                            My.Forms.frmOptions.lbMusicFolders.Items.Add(mpMusicFolderPath)
                        End If
                        If mpMusicFolderPaths.Contains(mpMusicFolderPath) = False Then
                            mpMusicFolderPaths.Add(mpMusicFolderPath)
                        End If
                    End If

                    Dim sb As New System.Text.StringBuilder
                    sb.AppendLine("Delete tracks not in the following folders: ")
                    For Each loc As String In mpMusicFolderPaths
                        sb.AppendLine(loc)
                    Next
                    ttApp.SetToolTip(chkDeleteNonMusicFolderTracks, sb.ToString)

                Case ProgressType.LOAD_ARTWORK_DIMENSIONS
                    pBarDiscs.Style = ProgressBarStyle.Marquee
                    mfUpdateStatusBarText("Loading Artwork dimensions cache...", False)

                Case ProgressType.OFFSET_TRACKNUMBER
                    mProgressDiscsCurrent += 1
                    mfUpdateStatusBarText("Offsetting " & userStateString, False)

                Case ProgressType.UPDATE_STATUSBAR_TEXT
                    mProgressDiscsCurrent += 1
                    mfUpdateStatusBarText("Overriding " & userStateString, False)

                Case ProgressType.PARSING_ITUNES_LIBRARY
                    'pBarDiscs.Style = ProgressBarStyle.Marquee
                    mProgressTracksCurrent += 1
                    mfUpdateStatusBarText("Parsing iTunes Library...", True)

                Case ProgressType.PARSING_ITUNES_XML_DATABASE
                    pBarDiscs.Style = ProgressBarStyle.Marquee
                    mfUpdateStatusBarText("Parsing iTunes XML Database...", False)

                Case ProgressType.READ_TRACKS_FROM_DISCS
                    chkDeleteNonMusicFolderTracks.Enabled = False
                    chkDeleteTracksNotInHDD.Enabled = False
                    mfUpdateStatusBarText(String.Format("Reading tracks from album: {0}", userStateString), secondary:=True)

                Case ProgressType.READY
                    If userStateString.Length > 0 Then
                        mfUpdateStatusBarText("Ready. " & userStateString, False)
                    ElseIf mCurrJobTypeMain = cBwJob.JobType.VALIDATE_TRACKS_SELECTED Then
                        Dim tracks As IITTrackCollection
                        tracks = mItunesApp.BrowserWindow.SelectedTracks
                        If tracks IsNot Nothing Then
                            Dim lst As New List(Of IITFileOrCDTrack)
                            Dim tracksMax As Integer = Math.Min(25, tracks.Count)
                            For i As Integer = 1 To tracksMax
                                Dim t As IITTrack = tracks(i)
                                If t.Kind = ITTrackKind.ITTrackKindFile Then
                                    lst.Add(CType(t, IITFileOrCDTrack))
                                End If
                            Next
                            If lst.Count > 0 Then
                                mfUpdateStatusBarText(fTracksAreStandard(lst), False)
                            Else
                                mfUpdateStatusBarText("Ready. None of the tracks are valid tracks.", False)
                            End If
                        End If
                    Else
                        mfUpdateStatusBarText(String.Format("Ready. Found {0} Tracks.", mMainLibraryTracks.Count), False)
                    End If

                Case ProgressType.RECOVERING_TRACKS
                    'pBarDiscs.Style = ProgressBarStyle.Continuous
                    'pBarDiscs.Increment(1)
                    mProgressDiscsCurrent += 1
                    mfUpdateStatusBarText(String.Format("... Recovering {0}", userStateString), True)

                Case ProgressType.REMOVING_TAGS
                    mfUpdateStatusBarText(userStateString, False)

                Case ProgressType.REMOVE_TRACK_FROM_LISTBOX
                    lbFiles.Items.Remove(userStateString)

                Case ProgressType.REPLACING_TAGS
                    mfUpdateStatusBarText("Replacing " & cboFind.Text & " in " & userStateString, False)

                Case ProgressType.REPLACING_TRACKS
                    'pBarDiscs.Style = ProgressBarStyle.Continuous
                    'pBarDiscs.Increment(1)
                    mProgressDiscsCurrent += 1
                    mfUpdateStatusBarText("Replacing " & userStateString, False)

                Case ProgressType.RESTORE_STATUS_BAR_MESSAGE
                    mfUpdateStatusBarText(userStateString, False)

                Case ProgressType.RESTORING_RATINGS
                    'pBarDiscs.Style = ProgressBarStyle.Continuous
                    'pBarDiscs.Increment(1)
                    mProgressDiscsCurrent += 1
                    mfUpdateStatusBarText(String.Format("Restoring ratings for tracks in album: {0}", userStateString), secondary:=False)

                Case ProgressType.SAVE_PLAYLIST
                    'pBarDiscs.Style = ProgressBarStyle.Continuous
                    'pBarDiscs.Increment(1)
                    mProgressDiscsCurrent += 1
                    mfUpdateStatusBarText(String.Format("Saving Playlist: {0}", userStateString), False)

                Case ProgressType.SCANNING_FILE_IN_HDD
                    pBarDiscs.Style = ProgressBarStyle.Marquee
                    If String.Empty <> userStateString Then
                        mfUpdateStatusBarText("Scanning " + Path.GetDirectoryName(userStateString), False)
                    End If

                Case ProgressType.SCANNING_TRACK_IN_PLAYER_LIBRARY
                    'pBarDiscs.Increment(1)
                    mProgressDiscsCurrent += 1
                    'sBarLeft.Text = fGetTruncatedText("Scanning tracks in album: " + userStateString)

                Case ProgressType.SCANNING_TRACK_IN_ITUNES_XML_DATABASE
                    'pBarDiscs.Increment(1)
                    mProgressDiscsCurrent += 1
                    mfUpdateStatusBarText("Scanning album: " + userStateString + " in XML database", False)

                Case ProgressType.SEARCHING_ITMS_ARTWORK
                    pbarTrack.Style = ProgressBarStyle.Marquee
                    mfUpdateStatusBarText(e.UserState.ToString, True)

                Case ProgressType.SEND_APP_TO_TRAY
                    Me.WindowState = FormWindowState.Minimized

                Case ProgressType.SET_ACTIVE_TAB
                    Me.tcTabs.SelectedTab = CType(e.UserState, TabPage)

                Case ProgressType.SET_PROGRESS_BAR_CONTINUOUS
                    pBarDiscs.Style = ProgressBarStyle.Continuous

                Case ProgressType.SET_PROGRESS_BAR_MARQUEE
                    pBarDiscs.Style = ProgressBarStyle.Marquee

                Case ProgressType.SHOW_STATISTICS_WINDOW
                    If fGetStatsMaker.MaxPlayedCount > 0 Then
                        Dim f As frmStatistics = CType(e.UserState, frmStatistics)
                        f.Show()
                    Else
                        MessageBox.Show("You need to listen to music first.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
                    End If

                Case ProgressType.TRIMMING_CHAR
                    mfUpdateStatusBarText("Trimming Characters in " & userStateString, False)

                Case ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX
                    mProgressDiscsMax = CType(e.UserState, Integer)
                    mProgressDiscsCurrent = 0
                    'pBarDiscs.Style = ProgressBarStyle.Continuous
                    'pBarDiscs.Value = 0
                    'pBarDiscs.Maximum = CType(e.UserState, Integer)

                Case ProgressType.UPDATE_THUMBNAIL_IN_ARTIST_FOLDER
                    mfUpdateStatusBarText("Forcing Thumbnail for " + userStateString, False)

                Case ProgressType.UPDATE_TRACKS_PROGRESS_BAR_MAX
                    mProgressTracksMax = CType(e.UserState, Integer)
                    mProgressTracksCurrent = 0

                Case ProgressType.VALIDATING_DISC_IN_ITUNES_LIBRARY
                    Dim lDisc As cInfoDisc = CType(e.UserState, cInfoDisc)
                    pbArtwork.Image = lDisc.fGetArtwork
                    Dim prefix As String = "Validating disc: "
                    Select Case mCurrJobTypeMain
                        Case cBwJob.JobType.ADJUST_RATINGS
                            prefix = "Adjusting My Rating: "
                    End Select
                    mfUpdateStatusBarText(prefix + lDisc.Name, False)

                Case ProgressType.WRITE_APPLICATION_SETTINGS
                    sSettingsSave()

                Case ProgressType.WRITE_ARTWORK_CACHE
                    pBarDiscs.Style = ProgressBarStyle.Marquee
                    mfUpdateStatusBarText("Writing Artwork dimensions cache...", False)

                Case ProgressType.UPDATING_TRACK
                    mfUpdateStatusBarText(String.Format("Updating {0}...", userStateString))

                Case ProgressType.WRITING_TAGS_TO_TRACKS
                    mfUpdateStatusBarText(String.Format("Writing Track Count, Disc Number and Disc Count to track: {0}", userStateString), True)

                Case ProgressType.ZIPPING_FILES
                    mfUpdateStatusBarText("Zipping " + userStateString, False)

                Case Else
                    MessageBox.Show(Application.ProductName & " was not designed to handle this progress.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)

            End Select

            'sUpdateProgress()

        Catch ex As Exception
            msAppendWarnings(ex.Message + " while reporting progress")
        End Try

    End Sub

    Private Sub sUpdateProgress()

        Dim suffix As String = String.Empty

        Select Case mCurrJobTypeMain
            Case cBwJob.JobType.ADJUST_RATINGS
                suffix = " - Adjusting My Rating"
            Case cBwJob.JobType.FIND_NEW_TRACKS_FROM_HDD
                suffix = " - Finding new Tracks in HDD"
            Case cBwJob.JobType.SYNCHROCLEAN
                suffix = " - Synchrocleaning"
            Case cBwJob.JobType.ADD_NEW_TRACKS
                suffix = " - Adding New Music"
            Case JobType.IMPORT_PLAYEDCOUNT_LASTFM
                suffix = " - Updating PlayedCount using Last.fm"
            Case Else
                suffix = String.Empty
        End Select

        ' 3.0.13.1 Prevented possible arithmetic overflow in progress bar
        If mProgressDiscsMax > 0 AndAlso mEToCstr.Length > 0 Then

            Dim multi As Double = 100 / mProgressDiscsMax
            Dim perc As Double = mProgressDiscsCurrent * multi
            If mProgressTracksMax > 0 AndAlso mProgressTracksCurrent > 0 Then
                perc = perc + (mProgressTracksCurrent / mProgressTracksMax) * multi
            End If

            If perc > 0 AndAlso perc <= 100 Then

                pBarDiscs.Style = ProgressBarStyle.Continuous
                pBarDiscs.Maximum = 100
                If mProgressDiscsCurrent <= mProgressDiscsMax Then
                    pBarDiscs.Value = CInt(perc)
                Else
                    pBarDiscs.Value = pBarDiscs.Maximum
                End If

                If mBooFinalTask = True Then
                    Me.Text = String.Format("[{0}%] {1} [{2}]", perc.ToString("0.0"), mAppTitle, mEToCstr) + suffix
                Else
                    Me.Text = String.Format("[{0}%] {1}", perc.ToString("0.0"), mAppTitle) + suffix
                End If

                niTray.Text = String.Format("[{0}%] {1}", perc.ToString("0.0"), APP_ABBR_NAME_IT) + suffix

            End If

        Else

            pBarDiscs.Style = ProgressBarStyle.Marquee

        End If

        ' End If

    End Sub

    Private Sub bwApp_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bwApp.RunWorkerCompleted

        '*******************************
        ' all the clearing up tasks here
        '*******************************

        Dim job As cBwJob = CType(e.Result, cBwJob)

        Me.TopMost = False
        Me.Text = mAppInfo.GetApplicationTitle
        mValModes = New ValidatorModes(True, True, True, True)

        If mSbWarnings.Length > 0 Then
            mfUpdateStatusBarText("View Logs > Errors...", secondary:=True)
        End If

        mLast100Tracks = False

        pBarDiscs.Style = ProgressBarStyle.Continuous
        pbarTrack.Style = ProgressBarStyle.Continuous

        If mCurrJobTypeMain <> JobType.INITIALIZE_PLAYER Then

            If pBarDiscs.Value < pBarDiscs.Maximum Then
                pBarDiscs.Value = pBarDiscs.Maximum
            End If

            If pbarTrack.Value < pbarTrack.Maximum Then
                pbarTrack.Value = pbarTrack.Maximum
            End If

        End If

        ' begin: these settings are not remembered by iTSfv        
        chkWinMakeReadOnly.Checked = False
        ' end: once these settings are remembered, will be gone to sSettingsLoad

        sSettingsGet()
        sUpdateGuiControls()

        Me.Cursor = Cursors.Default

        ' begin: because it is extremely annoying if these were enabled for next diff job
        chkDiscComplete.Checked = False
        ' end: so we disable these

        ' enable these which were disabled during reading tracks
        chkCheckArtworkLowRes.Enabled = True
        chkDeleteNonMusicFolderTracks.Enabled = True
        chkDeleteTracksNotInHDD.Enabled = True

        If mCurrJobTypeMain = JobType.EXPORT_ARTWORK_MANUAL Then
            If mArtworkJobStatus IsNot Nothing Then
                mfExportArtworkIT(mArtworkJobStatus, dirPath:=String.Empty)
            End If
        End If

        msAppendDebug("Job Finished: " & mCurrJobTypeMain.ToString)
        msWriteDebugLog()

        If job.JobCancelled = False Then
            If mfOKtoPowerMgmt() Then
                Dim po As New dlgPowerOptions
                po.ShowDialog()
            End If
        End If

        If mCurrJobTypeMain = JobType.COMMAND_LINE AndAlso My.Settings.ExitAfterCLI Then
            msCloseForms()
        End If

    End Sub

    Public Sub sSettingsGet()

        Me.Text = mpAppName()
        niTray.Text = Me.Text

        If String.Empty = My.Settings.PlaylistFileNamePattern Then
            ' nfi why this happens sometimes
            My.Settings.PlaylistFileNamePattern = "%AlbumArtist% - (%Year%) %Album%"
        End If
        If String.Empty = My.Settings.IndexFileExt Then
            ' nfi why this happens sometimes
            My.Settings.IndexFileExt = ".html"
        End If

        sUpdateSystemVariables()
        sUpdateSearchEngines()

        chkEditCopyArtistToAlbumArtist.Text = CStr(IIf(My.Settings.OverwriteAlbumArtist, "Overwrite AlbumArtist using Artist tag", "Fill missing AlbumArtist using Artist tag"))
        chkEditCopyArtistToAlbumArtist.ForeColor = CType(IIf(My.Settings.OverwriteAlbumArtist, Color.Red, Color.Black), Color)

        chkWriteGenre.Text = CStr(IIf(My.Settings.OverwriteGenre, "Overwrite Genre tag using Last.fm", "Fill missing Genre tag using Last.fm"))

        chkWinExportArtwork.Checked = My.Settings.LibraryExportArtwork
        chkWinExportPlaylist.Checked = My.Settings.ExportPlaylist
        chkImportArtwork.Checked = My.Settings.LibraryImportArtwork
        chkEditTrackCountEtc.Checked = My.Settings.FillTrackCountEtc
        chkEditCopyArtistToAlbumArtist.Checked = My.Settings.LibraryCopyAlbumArtist
        chkDeleteTracksNotInHDD.Checked = My.Settings.LibraryDeleteTracksNotInHDD
        chkDeleteNonMusicFolderTracks.Checked = My.Settings.LibraryDeleteNonMusicFolderTracks
        chkSheduleAdjustRating.Checked = My.Settings.ScheduleAdjustRatings
        chkScheduleFindNewFilesHDD.Checked = My.Settings.ScheduleFindNewFilesHDD

        chkResume.Checked = My.Settings.ResumeValidation

        If My.Settings.DefaultImArtworkFolder = True Then
            chkImportArtwork.Text = "Embed Artwork to track from iTunes or " & My.Settings.ArtworkFileNameIm
        Else
            chkImportArtwork.Text = String.Format("Embed Artwork in {0} from iTunes or {1}", My.Settings.FolderPathImArtwork, My.Settings.ArtworkFileNamePatternIm)
        End If
        ttApp.SetToolTip(chkImportArtwork, chkImportArtwork.Text)

        If My.Settings.DefaultExArtworkFolder = True Then
            chkWinExportArtwork.Text = "Export Artwork to Album folder as " & My.Settings.ArtworkFileNameEx
        Else
            chkWinExportArtwork.Text = String.Format("Export Artwork to {0} as {1}", My.Settings.FolderPathExArtwork, My.Settings.ArtworkFileNamePatternEx)
        End If
        ttApp.SetToolTip(chkWinExportArtwork, chkWinExportArtwork.Text)

        Dim srcDirLyricsIm As String = CStr(IIf(My.Settings.LyricsFromAlbumFolder, "Album folder", My.Settings.LyricsFolderPathIm))
        Dim srcFilePatternIm As String = If(My.Settings.LyricsFromLyricWiki, "Lyricsfly", If(My.Settings.LyricsPatternFromFileIm, "%FileName%", My.Settings.LyricsFilenamePatternIm))
        chkImportLyrics.Text = String.Format("Import Lyrics from {0} using {1}", srcDirLyricsIm, If(My.Settings.LyricsFromLyricWiki, srcFilePatternIm, srcFilePatternIm + My.Settings.LyricsFileExtIm))
        ttApp.SetToolTip(chkImportLyrics, chkImportLyrics.Text)
        Dim srcDirLyricsEx As String = CStr(IIf(My.Settings.LyricsToAlbumFolder, "Album folder", My.Settings.LyricsFolderPathEx))
        Dim srcFilePatternEx As String = If(My.Settings.LyricsPatternFromFileEx, "%FileName%", My.Settings.LyricsFilenamePatternEx)
        chkExportLyrics.Text = String.Format("Export Lyrics to {0} as {1}", srcDirLyricsEx, srcFilePatternEx)
        ttApp.SetToolTip(chkExportLyrics, chkExportLyrics.Text)

        chkWinExportPlaylist.Text = String.Format("Export Playlist to Album folder as {0}{1}", My.Settings.PlaylistFileNamePattern, My.Settings.PlaylistExt)
        ttApp.SetToolTip(chkWinExportPlaylist, chkWinExportPlaylist.Text)

        chkExportIndex.Text = String.Format("Export Index to Album folder as {0}{1}", My.Settings.IndexFileNamePattern, My.Settings.IndexFileExt)
        ttApp.SetToolTip(chkExportIndex, chkExportIndex.Text)

        chkCheckFoldersWithoutArtwork.Text = String.Format("Check for album folders without {0}", My.Settings.ArtworkFileNameEx)

        cboClipboardPattern.Items.Clear()
        For Each pattern As String In My.Settings.TagInfoPatterns
            cboClipboardPattern.Items.Add(pattern)
        Next
        cboClipboardPattern.Text = My.Settings.ClipboardPattern

        cboFind.Items.Clear()
        For Each txt As String In My.Settings.FindTextColl
            cboFind.Items.Add(txt)
        Next

        cboReplace.Items.Clear()
        For Each txt As String In My.Settings.ReplaceTextColl
            cboReplace.Items.Add(txt)
        Next

        cboDecompileOptions.SelectedIndex = 0
        cboFind.SelectedIndex = 0
        cboReplace.SelectedIndex = 0

        mGuiReady = True

    End Sub

    Public Sub sSettingsSave()

        If cboArtistsDecompiled.Text <> String.Empty AndAlso My.Settings.ArtistsDecompile.Contains(cboArtistsDecompiled.Text) = False Then
            My.Settings.ArtistsDecompile.Add(cboArtistsDecompiled.Text)
        End If

        If cboFind.Text <> String.Empty AndAlso My.Settings.FindTextColl.Contains(cboFind.Text) = False Then
            My.Settings.FindTextColl.Add(cboFind.Text)
        End If

        If cboReplace.Text <> String.Empty AndAlso My.Settings.ReplaceTextColl.Contains(cboReplace.Text) = False Then
            My.Settings.ReplaceTextColl.Add(cboReplace.Text)
        End If

        ' clipboard patterns are saved when user clicks "Copy to Clipboard"

        My.Settings.LibraryExportArtwork = chkWinExportArtwork.Checked
        My.Settings.ExportPlaylist = chkWinExportPlaylist.Checked
        My.Settings.LibraryImportArtwork = chkImportArtwork.Checked
        My.Settings.FillTrackCountEtc = chkEditTrackCountEtc.Checked
        My.Settings.LibraryCopyAlbumArtist = chkEditCopyArtistToAlbumArtist.Checked
        My.Settings.LibraryDeleteTracksNotInHDD = chkDeleteTracksNotInHDD.Checked
        My.Settings.LibraryDeleteNonMusicFolderTracks = chkDeleteNonMusicFolderTracks.Checked
        My.Settings.ScheduleAdjustRatings = chkSheduleAdjustRating.Checked
        My.Settings.ScheduleFindNewFilesHDD = chkScheduleFindNewFilesHDD.Checked

        My.Settings.ResumeValidation = chkResume.Checked

        If Not String.IsNullOrEmpty(cboExportFilePattern.Text) Then
            My.Settings.ExportTrackPattern = cboExportFilePattern.Text
        End If

        If My.Settings.BackupConfig = True Then
            mfConfigBackup(mFilePathConfigBackup)
        End If

        My.Settings.Save()

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRecover.Click

        Dim dlg As New OpenFileDialog
        dlg.Filter = "XML files (*.xml)|*.xml"
        dlg.InitialDirectory = Path.GetDirectoryName(mItunesApp.LibraryXMLPath)

        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then

            If mItunesApp.LibraryXMLPath = dlg.FileName Then
                MessageBox.Show("To recover lost tags you need an inactive iTunes Music Library.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Else
                txtXmlLibPath.Text = dlg.FileName
            End If

            If txtXmlLibPath.Text.Length > 0 Then

                Dim t As New cBwJob(cBwJob.JobType.RECOVER_TAGS)
                bwApp.RunWorkerAsync(t)

            End If

        End If

    End Sub


    Private Sub sBwAppRecoverTags()

        bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, mMainLibraryTracks.Count)

        Dim libParser As New cLibraryParser(bwApp, txtXmlLibPath.Text)

        ' you might this its faster to use the active XML to iterate through tracks 
        ' but you need to eventually write tags also, so this is the ok way

        For Each track As iTunesLib.IITTrack In mMainLibraryTracks

            If track.Kind = ITTrackKind.ITTrackKindFile Then

                Dim song As IITFileOrCDTrack = CType(track, IITFileOrCDTrack)

                If File.Exists(song.Location) Then

                    Dim xmlTrack As cXmlTrack = libParser.fFindXmlTrack(song)

                    If xmlTrack IsNot Nothing Then

                        Try
                            '' 01 - Rating
                            If My.Settings.RecRating Then
                                song.Rating = xmlTrack.Rating
                            End If
                            '' 02 - Played Date
                            If My.Settings.RecPlayedDate Then
                                song.PlayedDate = xmlTrack.PlayedDate
                            End If

                            '' 03 - Played Count
                            If My.Settings.RecPlayedCount Then
                                If song.PlayedCount < xmlTrack.PlayedCount Then
                                    song.PlayedCount += xmlTrack.PlayedCount
                                End If
                            End If

                            '' 04 - Skipped Date
                            If My.Settings.RecSkippedDate Then
                                song.SkippedDate = xmlTrack.SkippedDate
                            End If
                            '' 05 - Skipped Count
                            If My.Settings.RecSkippedCount Then
                                song.SkippedCount = xmlTrack.SkippedCount
                            End If

                            '' 06 - Track Number
                            If My.Settings.RecTrackNum And song.TrackNumber = 0 Then
                                song.TrackNumber = xmlTrack.TrackNumber
                            End If

                            '' 07 - Track Count
                            If My.Settings.RecTrackCount And song.TrackCount = 0 Then
                                song.TrackCount = xmlTrack.TrackCount
                            End If

                            '' 08 - Disc Number
                            If My.Settings.RecDiscNum And song.DiscNumber = 0 Then
                                song.DiscNumber = xmlTrack.DiscNumber
                            End If

                            '' 09 - Disc Count
                            If My.Settings.RecDiscCount And song.DiscCount = 0 Then
                                song.DiscCount = xmlTrack.DiscCount
                            End If

                            '' 10 - Equalizer
                            If My.Settings.RecEQ Then
                                song.EQ = xmlTrack.EQ
                            End If

                            '' 11 - Enabled
                            If My.Settings.RecEnabled Then
                                song.Enabled = xmlTrack.Enabled
                            End If

                            '' 12 - Bookmark Time 
                            If My.Settings.RecBookmarkTime And song.RememberBookmark = True Then
                                song.BookmarkTime = xmlTrack.BookmarkTime
                            End If

                            msAppendDebug("Recovered tags in " & song.Location)

                        Catch ex As Exception
                            msAppendWarnings(ex.Message + " while recovering tags")
                        End Try

                    End If ' track is a file

                End If ' end of xmltrack is nothing check

            End If ' end of file exists check

            bwApp.ReportProgress(ProgressType.RECOVERING_TRACKS, track.Artist & " - " & track.Album)

            sPausePendingCheck()
            If bwApp.CancellationPending Then
                Exit Sub
            End If

        Next

        bwApp.ReportProgress(ProgressType.READY)

    End Sub

    Private Sub sButtonFindNewTracks()

        If bwApp.IsBusy = False Then
            Me.TopMost = My.Settings.AlwaysOnTop
            lbDiscs.Items.Clear()

            If lbFiles.Items.Count > 0 Then

                'Dim listAddFiles As New List(Of String)
                'For Each filePath As String In lbFiles.Items
                '    listAddFiles.Add(filePath)
                'Next

                Dim taskAddNewFiles As New cBwJob(cBwJob.JobType.ADD_NEW_TRACKS)
                bwApp.RunWorkerAsync(taskAddNewFiles)

            Else

                Dim taskFindNewTracks As New cBwJob(cBwJob.JobType.FIND_NEW_TRACKS_FROM_HDD)
                bwApp.RunWorkerAsync(taskFindNewTracks)

            End If
        End If

    End Sub

    Private Sub btnFindTrackUnaddable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFindNewFiles.Click

        sButtonFindNewTracks()

    End Sub

    Private Sub sButtonShowFileInExplorer()

        If lbFiles.SelectedIndices.Count > 0 Then
            For Each f As String In lbFiles.SelectedItems
                If File.Exists(f) Then
                    Process.Start(Path.GetDirectoryName(f))
                End If
            Next
        End If

    End Sub

    Private Sub lbFiles_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lbFiles.MouseDoubleClick

        sButtonShowFileInExplorer()

    End Sub

    Private Sub sButtonCreatePlaylistDisc()

        If lbDiscs.SelectedIndex <> -1 And mTableDiscs IsNot Nothing Then

            Dim albumTitle As String = lbDiscs.SelectedItem.ToString
            Dim lDisc As New cInfoDisc
            lDisc = CType(mTableDiscs.Item(albumTitle), cInfoDisc)

            If lDisc IsNot Nothing Then
                sCreatePlaylistDialog(lDisc.Tracks, mfGetFileNameFromPattern(My.Settings.PlaylistFileNamePattern, lDisc.Tracks(0)))
            End If

        End If

    End Sub

    Private Sub btnAlbumsLoad_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreatePlaylistAlbum.Click
        sButtonCreatePlaylistDisc()
    End Sub

    Private Sub listBoxAlbums_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbDiscs.DoubleClick
        sPlayDisc()
    End Sub

    Private Sub lbDiscs_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lbDiscs.MouseDown
        Dim indexOver As Integer = lbDiscs.IndexFromPoint(e.X, e.Y)
        If indexOver >= 0 AndAlso indexOver < lbDiscs.Items.Count Then
            lbDiscs.SelectedIndex = indexOver
        End If
        lbDiscs.Refresh()
    End Sub

    Private Sub lbAlbums_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbDiscs.SelectedIndexChanged

        btnValidateAlbum.Enabled = (lbDiscs.SelectedIndex <> -1) AndAlso Not bwApp.IsBusy
        PlayFirstTrackToolStripMenuItem.Enabled = (lbDiscs.SelectedIndex <> -1)
        btnCreatePlaylistAlbum.Enabled = lbDiscs.SelectedIndex <> -1
        btnBrowseAlbum.Enabled = (lbDiscs.SelectedIndex <> -1) 'AndAlso Not bwApp.IsBusy

        If lbDiscs.SelectedItem IsNot Nothing Then
            mCurrentAlbum = lbDiscs.SelectedItem.ToString
        End If

        If lbDiscs.SelectedIndex <> -1 And mTableDiscs IsNot Nothing Then

            If bwDiscsBrowserInfo.IsBusy = False Then
                Dim lDisc As cInfoDisc = fGetDiscSelected()
                If lDisc IsNot Nothing Then
                    bwDiscsBrowserInfo.RunWorkerAsync(lDisc)
                End If
            End If

        End If

    End Sub

    Private Sub btnRatingsRestore_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRatingsRestore.Click

        If Not bwApp.IsBusy Then

            Dim dlg As New OpenFileDialog
            dlg.Multiselect = True
            dlg.Filter = DLG_BACKUP_RESTORE

            If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then

                If mfValidRestoreFile(dlg.FileName) Then

                    txtRatingsRestorePath.Text = dlg.FileName
                    pBarDiscs.Value = 0
                    pBarDiscs.Maximum = mMainLibraryTracks.Count

                    Dim t As New cBwJob(cBwJob.JobType.RATINGS_RESTORE)
                    t.TaskData = dlg.FileNames
                    bwApp.RunWorkerAsync(t)

                Else

                    '' 5.34.6.4 Advice user to use Recover Tags if they try to restore tags from iTunes Music Library.xml

                    Dim sb As New StringBuilder
                    sb.AppendLine("You attempted to restore tags from a iTunes Music Library.xml.")
                    sb.AppendLine("To restore tags from a previous iTunes Music Library XML file")
                    sb.AppendLine("you need to use the Recover Tags tab.")
                    sb.AppendLine()
                    sb.AppendLine("This Restore tags function works only for XML files backed up by iTSfv.")
                    MessageBox.Show(sb.ToString, mpAppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

                End If


            End If

        End If

    End Sub


    Private Sub btnRatingsBackupBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRatingsBackup.Click

        If Not bwApp.IsBusy Then

            Dim dlg As New SaveFileDialog
            dlg.Filter = DLG_BACKUP_RESTORE
            dlg.FileName = Path.GetFileNameWithoutExtension(mFilePathRatingsBR)
            dlg.AddExtension = True

            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)

            If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then

                ' 5.09.3.3 Backup Ratings XML file save path defaulted to Logs Directory
                mFilePathRatingsBR = dlg.FileName
                txtRatingsBackupPath.Text = mFilePathRatingsBR

                pBarDiscs.Value = 0
                pBarDiscs.Maximum = mMainLibraryTracks.Count

                Dim t As New cBwJob(cBwJob.JobType.RATINGS_BACKUP)
                bwApp.RunWorkerAsync(t)

            End If

        End If


    End Sub

    Private Sub sPlayDisc()

        If lbDiscs.SelectedIndex <> -1 Then

            Dim lDisc As New cInfoDisc
            lDisc = CType(mTableDiscs.Item(lbDiscs.SelectedItem.ToString), cInfoDisc)
            If lDisc IsNot Nothing Then
                If File.Exists(lDisc.FirstTrack.Location) Then
                    mItunesApp.PlayFile(lDisc.FirstTrack.Location)
                End If
            End If

        End If

    End Sub

    Private Sub SToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SToolStripMenuItem.Click
        sShowErrors()
    End Sub

    Private Sub OpenNoniTSStandardTrackListwToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenNoniTSStandardTrackListwToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathNonItsTracks, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub OpenWarningsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenWarningsToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathWarningsLog, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub OpenAlbumsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenAlbumsToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathAlbumsInconsecutiveTracks, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub TrackCountUpdatedTracksToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackCountUpdatedTracksToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathTracksEditedByAlbumBrowser, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub sUpdateButtonControls()

        If tcTabs.SelectedTab Is tpSelectedTracks AndAlso tcSelectedTracks.SelectedTab Is tpEditor Then
            btnValidateSelected.Text = "&Edit Selected Tracks"
            'ValidateToolStripMenuItem1.Text = "&Edit"
            chkDiscComplete.Enabled = False
        Else
            btnValidateSelected.Text = "&Validate Selected Tracks"
            'ValidateToolStripMenuItem1.Text = "&Validate"
            chkDiscComplete.Enabled = True
        End If

        tsmiSelectedTracksValidate.Visible = chkDiscComplete.Enabled

        'CheckToolStripMenuItem.Visible = chkDiscComplete.Enabled
        'EditTracksToolStripMenuItem.Visible = chkDiscComplete.Enabled
        'LibraryToolStripMenuItem1.Visible = chkDiscComplete.Enabled
        'tsmSelectedTracksValidateFS.Visible = chkDiscComplete.Enabled

    End Sub

    Private Sub tcTabs_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tcTabs.SelectedIndexChanged

        sUpdateButtonControls()
        ttApp.SetToolTip(lbDiscs, "")

    End Sub

    Private Sub TracksWithNoLyricsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TracksWithNoLyricsToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathNoLyicsTracks, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub CheckForUpdatesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckForUpdatesToolStripMenuItem.Click
        msShowUpdatesManual(sBarTrack)
    End Sub

    Private Function fScheduledNow() As Boolean

        If My.Settings.OnMonday And Now.DayOfWeek.ToString = "Monday" Or _
           My.Settings.OnTuesday And Now.DayOfWeek.ToString = "Tuesday" Or _
           My.Settings.OnWednesday And Now.DayOfWeek.ToString = "Wednesday" Or _
           My.Settings.OnThursday And Now.DayOfWeek.ToString = "Thursday" Or _
           My.Settings.OnFriday And Now.DayOfWeek.ToString = "Friday" Or _
           My.Settings.OnSaturday And Now.DayOfWeek.ToString = "Saturday" Or _
           My.Settings.OnSunday And Now.DayOfWeek.ToString = "Sunday" Then

            If My.Settings.ScheduleTime = Now.ToString("HH:mm:ss") Then
                Return True
            End If

        End If

        Return False

    End Function

    Private Sub sButtonRunSchedule()

        If chkScheduleFindNewFilesHDD.Checked Then lbFiles.Items.Clear()
        Dim t As New cBwJob(cBwJob.JobType.SCHEDULE_DO)
        bwApp.RunWorkerAsync(t)

    End Sub

    Private Sub tmrTaskDuration_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrSecond.Tick

        'fswApp.EnableRaisingEvents = My.Settings.WatchFileSystem AndAlso Not bwApp.IsBusy

        btnClearFilesListBox.Enabled = lbFiles.Items.Count > 0

        If lbDiscs.Items.Count > 0 Then
            tpDiscsBrowser.Text = String.Format("Discs Browser ({0})", lbDiscs.Items.Count)
        Else
            tpDiscsBrowser.Text = String.Format("Discs Browser")
        End If

        If mWatcher IsNot Nothing Then

            tmrAddMusicAuto.Interval = CInt(My.Settings.AddMusicAutoFreq * 60 * 1000)
            mSecondsLeftToAddMusic = mSecondsLeftToAddMusic - 1

            If My.Settings.WatchFileSystem AndAlso Not bwApp.IsBusy Then
                mWatcher.StartWatchFolders()
            Else
                mWatcher.StopWatchFolders()
            End If

            If mWatcher.Watching = True Then
                tpExplorerFiles.Text = String.Format("Folder Watcher ({0})", lbFiles.Items.Count)
                If My.Settings.AddMusicAuto = True Then
                    If lbFiles.Items.Count > 0 Then
                        Dim booPrompt As String = "Prompt to Add Music in"
                        If My.Settings.SyncSilent = True Then
                            booPrompt = "Automatically Add Music in"
                        End If
                        tpExplorerFiles.Text = String.Format("Folder Watcher ({0}) - {1} {2}", lbFiles.Items.Count, booPrompt, fGetHMStoString2(mSecondsLeftToAddMusic))
                    End If
                End If
                If mWatcher.Files.Count <> lbFiles.Items.Count Then
                    lbFiles.Items.Clear()
                    lbFiles.Items.AddRange(mWatcher.Files.ToArray)
                End If
                If mWatcher.Activity.Length > 0 Then
                    txtActivity.Text = mWatcher.Activity
                End If
            Else
                tpExplorerFiles.Text = String.Format("Files ({0})", lbFiles.Items.Count)
            End If
        End If

        If bwApp.IsBusy Or mWebClient.IsBusy Then

            sUpdateGuiControls()
            mSecondsSoFar += 1

            If lbVerbose.Items.Count > 100 Then
                lbVerbose.Items.Clear()
            End If

            If mProgressTracksMax > 0 AndAlso mProgressTracksCurrent > 0 Then
                Dim perc As Double = 100 * mProgressTracksCurrent / mProgressTracksMax
                sTrackProgress.Text = String.Format("{0}%", perc.ToString("0.0"))
                sTrackProgress.Visible = True
            Else
                sTrackProgress.Visible = False
            End If

            If mProgressDiscsMax > 0 Then

                Dim multi As Double = 100 / mProgressDiscsMax
                Dim perc As Double = mProgressDiscsCurrent * multi
                If mProgressTracksMax > 0 AndAlso mProgressTracksCurrent > 0 Then
                    perc = perc + (mProgressTracksCurrent / mProgressTracksMax) * multi
                End If

                ''Console.Writeline("% done: " & percDone)
                If perc > 0 Then
                    Dim secsForComplete As Double = mSecondsSoFar / (perc / 100)

                    Dim secsToGo As Double = (secsForComplete - mSecondsSoFar)

                    If My.Settings.ShowEToC Then
                        mEToCstr = "EToC is " + Now.AddSeconds(secsToGo).ToString("HH:mm:ss")
                    Else
                        Dim ts As TimeSpan = TimeSpan.FromSeconds(secsToGo)
                        If ts.TotalSeconds < 120 Then
                            mEToCstr = ts.TotalSeconds.ToString("0") & " seconds remaining"
                        Else
                            mEToCstr = ts.TotalMinutes.ToString("0") & " minutes remaining"
                        End If

                    End If

                End If

            End If

        ElseIf fScheduledNow() Then
            sButtonRunSchedule()
        Else
            sTrackProgress.Visible = False
        End If

    End Sub

    Private Sub chkResume_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkResume.CheckedChanged

        '* Prevent automatically selecting settings that modify track or iTunes Library data in anyway [chip]
        mBooResume = chkResume.Checked

    End Sub

    Private Sub TracksThatArtworkWasAddedToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TracksThatArtworkWasAddedToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathArtworkAdded, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub OptionsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OptionsToolStripMenuItem.Click
        msShowOptions()
    End Sub

    Private Sub sQueueFileToListBoxTracks(ByVal filePath As String)

        ' 4.0.5.1 Prevented possible addition of duplicate entries to tracks ListBox in Explorer tab
        If Not lbFiles.Items.Contains(filePath) Then
            lbFiles.Items.Add(filePath)
        End If

    End Sub

    Private Sub MusicFolderActivityToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MusicFolderActivityToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathMusicFolderActivity, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub sOpenMusicFolder()
        mfOpenFileOrDirPath(My.Settings.MusicFolderPath, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub OpenMusicFolderToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenMusicFolderToolStripMenuItem.Click
        sOpenMusicFolder()
    End Sub

    Private Sub cboArtistsDecompiled_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles cboArtistsDecompiled.KeyUp
        sAutoCompleteCombo_KeyUp(cboArtistsDecompiled, e)
    End Sub

    Private Sub cboArtistsDecompiled_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboArtistsDecompiled.Leave
        sAutoCompleteCombo_Leave(cboArtistsDecompiled)
    End Sub

    Private Sub sButtonAdjustRatings()

        If bwApp.IsBusy = False Then
            mStatsMaker = Nothing
            Dim task As New cBwJob(cBwJob.JobType.ADJUST_RATINGS)
            bwApp.RunWorkerAsync(task)
        End If

    End Sub

    Private Sub btnAdjustRatings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdjustRatings.Click

        sButtonAdjustRatings()

    End Sub


    Private Sub btnOffsetTrackNum_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOffsetTrackNum.Click


        Me.TopMost = My.Settings.AlwaysOnTop
        Dim task As New cBwJob(cBwJob.JobType.OFFSET_TRACKNUMBER)
        bwApp.RunWorkerAsync(task)


    End Sub

    Private Sub sButtonValidateDisc()

        If lbDiscs.SelectedIndex <> -1 Then

            Me.TopMost = My.Settings.AlwaysOnTop
            Dim task As New cBwJob(cBwJob.JobType.VALIDATE_DISC)
            bwApp.RunWorkerAsync(task)

        End If
    End Sub

    Private Sub btnValidateAlbum_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnValidateAlbum.Click
        sButtonValidateDisc()
    End Sub

    Private Sub sButtonCopyTagInfo()

        Clipboard.SetData(DataFormats.StringFormat, fClipboardPreview(bAll:=True))

    End Sub


    Private Sub btnClipboard_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClipboard.Click

        My.Settings.ClipboardPattern = cboClipboardPattern.Text
        If cboClipboardPattern.Text <> String.Empty AndAlso My.Settings.TagInfoPatterns.Contains(cboClipboardPattern.Text) = False Then
            My.Settings.TagInfoPatterns.Add(cboClipboardPattern.Text)
        End If

        sButtonCopyTagInfo()

    End Sub

    Private Function ffGetMusicFolderGuessed(ByVal numTracks As Integer) As String

        Try
            ' get random num tracks and save location in a list 
            Dim listTracks As New List(Of String)

            bwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, numTracks)

            For i As Integer = 1 To numTracks
                Dim t As IITTrack = mMainLibraryTracks.Item(fGetRandomNumber(mMainLibraryTracks.Count, 1))
                If t.Kind = ITTrackKind.ITTrackKindFile Then
                    Dim loc As String = CType(t, IITFileOrCDTrack).Location
                    If File.Exists(loc) Then
                        listTracks.Add(Path.GetDirectoryName(loc))
                        ''Console.Writeline(t.TrackDatabaseID & " " & CType(t, IITFileOrCDTrack).Location)
                        bwApp.ReportProgress(ProgressType.DETERINE_WHERE_MOST_MUSIC_IS, Path.GetDirectoryName(loc))
                    End If
                End If
            Next

            Dim booSame As Boolean = True

            ' we put 10 here; in reality who would put music 10 folders deep?
            For deep As Integer = 10 To 0 Step -1
                booSame = True
                Dim i As Integer = 0
                For i = 0 To listTracks.Count - 2
                    booSame = booSame And fEqualsRoot(listTracks.Item(i), listTracks.Item(i + 1), deep)
                    Console.WriteLine(String.Format("Trying {0} deep: {1}, {2}th time: {3}", deep, fGetRootFolderPath(listTracks.Item(i), deep), i + 1, booSame))
                    If booSame = False Then Exit For
                Next

                If booSame Then

                    Dim mDiscArtists As New Dictionary(Of String, Integer)

                    For i = 0 To listTracks.Count - 1

                        Dim p As String = fGetRootFolderPath(listTracks(i), deep)
                        If mDiscArtists.ContainsKey(p) Then
                            mDiscArtists.Item(p) += 1
                        Else
                            mDiscArtists.Add(p, 1)
                        End If

                    Next

                    Dim topHit As Integer = 0
                    Dim topArtist As String = VARIOUS_ARTISTS

                    Dim et As IEnumerator = mDiscArtists.GetEnumerator
                    Dim de As System.Collections.Generic.KeyValuePair(Of String, Integer)

                    While et.MoveNext
                        de = CType(et.Current, KeyValuePair(Of String, Integer))
                        If String.IsNullOrEmpty(de.Key) = False AndAlso CInt(de.Value) > topHit Then
                            topHit = CInt(de.Value)
                            topArtist = CStr(de.Key)
                        End If
                    End While

                    Return topArtist

                End If
            Next

        Catch ex As Exception

            msAppendWarnings(ex.Message + " while guessing music folder path.")
            msAppendWarnings(ex.StackTrace)

        End Try

        Return Nothing

    End Function

    Private Function fEqualsRoot(ByVal loc1 As String, ByVal loc2 As String, ByVal deep As Integer) As Boolean

        Dim okPair As Boolean = fGetRootFolderPath(loc1, deep).Equals(fGetRootFolderPath(loc2, deep))
        If deep = 0 AndAlso okPair = False Then
            okPair = mfFileIsInMusicFolder(loc1) AndAlso mfFileIsInMusicFolder(loc2)
        End If

        Return okPair

    End Function

    Private Function fGetRootFolderPath(ByVal loc As String, ByVal deep As Integer) As String

        Dim str() As String = loc.Split(Path.DirectorySeparatorChar)
        Dim fp As String = ""

        Dim i As Integer = 0
        Dim max As Boolean = False
        While max = False
            fp += str(i) + Path.DirectorySeparatorChar
            max = (deep = i) Or (str.Length - 1 = i)
            i += 1
        End While

        Return fp

    End Function

    Private Sub sSynchroclean()

        sSettingsSave()

        Dim locs As New System.Text.StringBuilder
        For Each loc As String In My.Settings.MusicFolderLocations
            locs.AppendLine(loc)
        Next

        Dim msg As String = "Please review the following information before continuing. Ignoring the message could possibly result in removing all the tracks in your library. " & Environment.NewLine & _
              Environment.NewLine & "Music folder Paths:" & Environment.NewLine & locs.ToString & Environment.NewLine & _
             "iTunes Music Library.xml Path:" & Environment.NewLine & mItunesApp.LibraryXMLPath & Environment.NewLine & _
             Environment.NewLine & _
             "Do you wish to continue?"

        If MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then

            chkResume.Checked = False

            '' disable all the rest
            'For Each tp As TabPage In tcValidate.TabPages
            '    For Each ctl As Control In tp.Controls
            '        If TypeOf ctl Is CheckBox Then
            '            CType(ctl, CheckBox).Checked = False
            '        End If
            '    Next
            'Next
            ' enable these two 
            chkDeleteNonMusicFolderTracks.Checked = True
            chkDeleteTracksNotInHDD.Checked = True
            chkResume.Checked = False

            If Not bwApp.IsBusy Then

                Me.TopMost = My.Settings.AlwaysOnTop

                Dim myTask As New cBwJob(cBwJob.JobType.SYNCHROCLEAN)
                bwApp.RunWorkerAsync(myTask)

            End If

        End If

    End Sub

    Private Sub btnSynchroclean_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSynchroclean.Click

        sSynchroclean()

    End Sub

    Private Sub btnOverride_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOverride.Click

        Me.TopMost = My.Settings.AlwaysOnTop
        Dim task As New cBwJob(cBwJob.JobType.OVERRIDE_TAGS)
        bwApp.RunWorkerAsync(task)

    End Sub


    Private Sub tpLibrary_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tpSettings.Click

    End Sub

    Private Sub sShowApp()
        Select Case Me.WindowState
            Case FormWindowState.Minimized
                Me.ShowInTaskbar = True
                Me.WindowState = FormWindowState.Normal
                SendToSystemTrayToolStripMenuItem.Enabled = True
            Case FormWindowState.Normal
                Me.WindowState = FormWindowState.Minimized
        End Select
        tsmShowApp.Enabled = (Me.WindowState = FormWindowState.Minimized)
    End Sub

    Private Sub niTray_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles niTray.MouseDoubleClick
        sShowApp()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        msCloseForms()
    End Sub

    Private Sub TracksWithMultipleArtworkToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TracksWithMultipleArtworkToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathMultipleArtwork, sBarTrack, ttApp, ssAppDisc)
    End Sub


    Private Sub AdjustRatingsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AdjustRatingsToolStripMenuItem.Click

        sButtonAdjustRatings()

    End Sub

    Private Sub CopyTrackInfoToClipboardToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyTrackInfoToClipboardToolStripMenuItem.Click

        sButtonCopyTagInfo()

    End Sub

    Private Sub sButtonValidateLast100Tracks()

        mLast100Tracks = True

        chkItunesStoreStandard.Checked = True
        sButtonValidate()

    End Sub

    Private Sub QuickValidationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QuickValidationToolStripMenuItem.Click
        sButtonValidateLast100Tracks()
    End Sub

    Private Sub btnClearListBox_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClearFilesListBox.Click
        If bwApp.IsBusy = False Then
            lbFiles.Items.Clear()
            mWatcher.ClearList()
        End If
    End Sub


    Private Sub sButtonValidateOrEditSelectedTracks()

        ' 5.30.4.2 Double clicking Validate Selected Tracks button could have crashed iTSfv because BackgroundWorker is busy
        If bwApp.IsBusy = False And mfWarnLowResArtworkProceed(chkRemoveLowResArtwork.Checked) = True Then

            If mItunesApp IsNot Nothing AndAlso mItunesApp.BrowserWindow.SelectedTracks IsNot Nothing AndAlso mItunesApp.BrowserWindow.SelectedTracks.Count > 0 Then

                sSettingsSave()
                'sLogSettings()

                Me.TopMost = My.Settings.AlwaysOnTop
                Dim task As cBwJob

                If tcTabs.SelectedTab Is tpSelectedTracks AndAlso _
                    tcSelectedTracks.SelectedTab.Text = tpEditor.Text Then

                    task = New cBwJob(cBwJob.JobType.EDIT_SELECTED_TRACKS)

                Else

                    lbDiscs.Items.Clear() ' we need this to display stuff clearly
                    task = New cBwJob(cBwJob.JobType.VALIDATE_TRACKS_SELECTED)

                End If

                btnValidateSelected.Enabled = False
                bwApp.RunWorkerAsync(task)


            End If

        End If

    End Sub

    Private Sub btnSelectedValidateAlbum_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnValidateSelected.Click
        sButtonValidateOrEditSelectedTracks()
    End Sub

    Private Sub btnReplaceTracks_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReplaceTracks.Click

        Me.TopMost = My.Settings.AlwaysOnTop

        Dim t As New cBwJob(cBwJob.JobType.REPLACE_TRACKS)
        bwApp.RunWorkerAsync(t)

    End Sub

    Private Sub sButtonBrowseDisc()
        If lbDiscs.SelectedIndex <> -1 Then

            Dim lDisc As New cInfoDisc
            lDisc = CType(mTableDiscs.Item(lbDiscs.SelectedItem.ToString), cInfoDisc)
            If lDisc IsNot Nothing Then
                Dim folderPath As String = lDisc.Location
                If Directory.Exists(folderPath) Then Process.Start(folderPath)
            End If

        End If
    End Sub

    Private Sub btnBrowseAlbum_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowseAlbum.Click
        sButtonBrowseDisc()
    End Sub

    Private Sub sSendToTray()

        Me.WindowState = FormWindowState.Minimized
        Me.ShowInTaskbar = False
        SendToSystemTrayToolStripMenuItem.Enabled = False
        tsmShowApp.Enabled = True

    End Sub

    Private Sub SendToSystemTrayToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SendToSystemTrayToolStripMenuItem.Click
        sSendToTray()
    End Sub

    Private Sub ssGetStatistics(ByVal lStats As cStatsMaker)

        If lStats Is Nothing AndAlso Not bwApp.IsBusy Then
            Dim task As New cBwJob(cBwJob.JobType.STATISTICS_DO)
            bwApp.RunWorkerAsync(task)
        Else
            ssShowStatistics(lStats)
        End If

    End Sub

    Private Sub sOpenStatistics(ByVal filePath As String, ByVal userOpened As Boolean)

        ' WORKING - DO NOT MODIFY

        Try
            If userOpened = True Then
                ' external statistics
                Dim lStats As cStatsMaker = CType(mfReadObjectFromFileBF(filePath), cStatsMaker)
                lStats.Name = Path.GetFileName(filePath)
                lStats.DateStats = New FileInfo(filePath).LastWriteTime.Date
                sShowStatistics(lStats)
            Else
                ' internal statistics 
                mStatsMaker = CType(mfReadObjectFromFileBF(filePath), cStatsMaker)
                mStatsMaker.Name = Path.GetFileName(filePath)
                mStatsMaker.DateStats = New FileInfo(filePath).LastWriteTime.Date
                If bwApp.IsBusy OrElse MessageBox.Show("Would you like to load statistics from cache?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
                    ssShowStatistics(mStatsMaker)
                Else
                    mStatsMaker = Nothing
                    ssGetStatistics(mStatsMaker)
                End If
            End If
        Catch ex As Exception
            msAppendWarnings(ex.Message + " while opening statistics")
            mStatsMaker = Nothing
            'mAdapter.UserStatistics = Nothing
            ssGetStatistics(mStatsMaker)
        End Try

    End Sub
    Private Sub sShowStatistics(ByVal lStats As cStatsMaker)

        If lStats Is Nothing Then
            sOpenStatistics(mStatsFilePath, False)
        Else
            ssGetStatistics(lStats)
        End If

    End Sub

    Private Sub btnStatistics_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStatistics.Click

        If bwApp.IsBusy = False Then
            ' only empty statsmaker if bwApp is not doing anything
            ' otherwise everytime the user presses this and the adjusting rating 
            ' is working then the bwApp has to create statsmaker
            mStatsMaker = Nothing ' really need this to work nicely
        End If

        sShowStatistics(mStatsMaker)

    End Sub

    Private Sub TracksWithLowResolutionArtworkToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TracksWithLowResolutionArtworkToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathListArtworkRes, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub TracksWithNoArtworkToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TracksWithNoArtworkToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathNoArtwork, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub TracksThatRatingWasAdjustedToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TracksThatRatingWasAdjustedToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathRatingsAdjusted, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub PlayFirstTrackToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PlayFirstTrackToolStripMenuItem.Click
        sPlayDisc()
    End Sub

    Private Sub AboutToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles miAbout.Click
        msShowAbout()
    End Sub

    Private Sub VersionHistoryToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles VersionHistoryToolStripMenuItem1.Click
        msShowVersionHistory()
    End Sub

    Private Sub OpenTracksReportToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenTracksReportToolStripMenuItem1.Click
        sOpenTracksReport()
    End Sub

    Private Sub CheckForUpdatesToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckForUpdatesToolStripMenuItem1.Click
        msShowUpdatesManual(sBarTrack)
    End Sub

    Private Sub HydrogenAudioToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HydrogenAudioToolStripMenuItem.Click
        Process.Start("http://www.hydrogenaudio.org/forums/index.php?showtopic=51708")
    End Sub

    Private Sub miSynchroclean_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles miSynchroclean.Click
        sSynchroclean()
    End Sub

    Private Sub ExitToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem1.Click
        msCloseForms()
    End Sub

    Private Sub miSendToTray_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles miSendToTray.Click
        sSendToTray()
    End Sub

    Private Sub sOpenManual()

        Dim files As String() = Directory.GetFiles(Application.StartupPath, "*.pdf", SearchOption.AllDirectories)
        If files.Length > 0 Then
            mfOpenFileOrDirPath(files(0), sBarTrack, ttApp, ssAppDisc)
        Else
            Process.Start("http://sourceforge.net/project/showfiles.php?group_id=204248&package_id=248537")
        End If

    End Sub
    Private Sub miManual_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles miManual.Click
        sOpenManual()
    End Sub

    Private Sub miAdjustRatings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles miAdjustRatings.Click
        sButtonAdjustRatings()
    End Sub

    Private Sub miStatistics_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles miStatistics.Click
        sShowStatistics(mStatsMaker)
    End Sub

    Private Sub BrowseMusicFolderToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowseMusicFolderToolStripMenuItem.Click
        sOpenMusicFolder()
    End Sub

    Private Sub OpenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenToolStripMenuItem.Click

        Dim dlg As New OpenFileDialog
        dlg.Title = String.Format("Open {0} Statistics file", Application.ProductName)
        dlg.InitialDirectory = My.Settings.LogsDir
        dlg.Filter = DLG_FILTER_STATS
        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            sOpenStatistics(dlg.FileName, True)
        End If

    End Sub

    Private Sub ILoungeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ILoungeToolStripMenuItem.Click
        Process.Start("http://forums.ilounge.com/showthread.php?t=192735")
    End Sub

    Private Sub SourceForgeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SourceForgeToolStripMenuItem.Click
        Process.Start("http://sourceforge.net/forum/?group_id=204248")
    End Sub

    Private Sub AlbumsWithInconsecutiveTrackNumbersToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AlbumsWithInconsecutiveTrackNumbersToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathAlbumsInconsecutiveTracks, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub TracksThatRatingWasAdjustedToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TracksThatRatingWasAdjustedToolStripMenuItem1.Click
        mfOpenFileOrDirPath(mFilePathRatingsAdjusted, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub TracksThatTrackCountWasUpdatedToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TracksThatTrackCountWasUpdatedToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathTracksEditedByAlbumBrowser, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub TracksNotITunesStoreCompliantToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TracksNotITunesStoreCompliantToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathNonItsTracks, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub sShowErrors()
        If mSbWarnings.Length > 0 Then
            Dim f As New frmErrors(mSbWarnings.ToString)
            f.Show()
            f.Focus()
        Else
            mfOpenFileOrDirPath(mFilePathErrorsLog, sBarTrack, ttApp, ssAppDisc)
        End If
    End Sub

    Private Sub ErrorsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ErrorsToolStripMenuItem.Click
        sShowErrors()
    End Sub

    Private Sub WarningsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WarningsToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathWarningsLog, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub BrowseLogsFolderToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowseLogsFolderToolStripMenuItem.Click
        mfOpenFileOrDirPath(My.Settings.LogsDir, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub ValidateITunesMusicLibraryToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ValidateITunesMusicLibraryToolStripMenuItem.Click
        sButtonValidateLibrary()
    End Sub

    Private Sub miValidateSelected_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles miValidateSelected.Click
        sButtonValidateOrEditSelectedTracks()
    End Sub

    Private Sub ValidateLast100TracksToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ValidateLast100TracksToolStripMenuItem.Click
        sButtonValidateLast100Tracks()
    End Sub

    Private Sub btnCopyTo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCopyTo.Click

        Dim dlg As New McoreSystem.FolderBrowser
        dlg.Title = "Browse for alternate Music folder location"
        dlg.Flags = McoreSystem.BrowseFlags.BIF_NEWDIALOGSTYLE Or _
                    McoreSystem.BrowseFlags.BIF_STATUSTEXT Or _
                    McoreSystem.BrowseFlags.BIF_EDITBOX

        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            If dlg.DirectoryPath.Length > 0 Then
                Dim task As New cBwJob(cBwJob.JobType.EXPORT_TRACKS)
                My.Settings.ExportTracksLastFolder = dlg.DirectoryPath
                task.mMessage = dlg.DirectoryPath
                bwApp.RunWorkerAsync(task)
            End If
        End If

        If My.Settings.ExportTrackPatterns.Contains(cboExportFilePattern.Text) = False Then
            My.Settings.ExportTrackPatterns.Add(cboExportFilePattern.Text)
            cboExportFilePattern.Items.Add(cboExportFilePattern.Text)
        End If

    End Sub

    Private mExportFilePattern As String

    Private Sub cboExportFilePattern_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles cboExportFilePattern.KeyDown
        If e.KeyCode = Keys.Delete Then
            cboExportFilePattern.Items.Remove(cboExportFilePattern.SelectedText)
            My.Settings.ExportTrackPatterns.Remove(cboExportFilePattern.SelectedText)
        End If
    End Sub

    ' need this cos bwapp cant access cbo text
    Private Sub cboExportFilePattern_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboExportFilePattern.SelectedIndexChanged
        mExportFilePattern = cboExportFilePattern.Text        
    End Sub

    Private Sub AddNewFilesNotInITunesMusicLibraryToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNewFilesNotInITunesMusicLibraryToolStripMenuItem.Click
        sButtonFindNewTracks()
    End Sub

    Private Sub BrowseITMSArtworksFolderToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowseITMSArtworksFolderToolStripMenuItem.Click

        mfOpenFileOrDirPath(My.Settings.ArtworkDir, sBarTrack, ttApp, ssAppDisc)

    End Sub

    Private Sub DebugToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DebugToolStripMenuItem.Click
        msShowDebug(sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub sCreatePlaylistDialog(ByVal tracks As List(Of IITFileOrCDTrack), Optional ByVal fileName As String = "")

        Dim dlg As New SaveFileDialog
        dlg.Filter = "M3U Playlist (*.m3u)|*.m3u|XML Shareable Playlist Format (*.xspf)|*.xspf"
        If fileName <> "" Then
            dlg.FileName = fileName
        End If
        dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)
        dlg.AddExtension = True

        If tracks IsNot Nothing Then

            If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then

                Dim pw As New cPlaylistWriter(tracks)
                Dim ext As String = Path.GetExtension(dlg.FileName)

                Select Case ext
                    Case ".m3u"
                        pw.fCreatePlaylistM3U(dlg.FileName, False)
                    Case ".xspf"
                        pw.fCreatePlaylisXSPF(dlg.FileName)
                End Select

            End If

        End If

    End Sub

    Private Sub sCreatePlaylist(ByVal tracks As IITTrackCollection)

        Dim temp As New List(Of IITFileOrCDTrack)
        For Each track As IITTrack In tracks
            If track.Kind = ITTrackKind.ITTrackKindFile Then
                temp.Add(CType(track, IITFileOrCDTrack))
            End If
        Next

        sCreatePlaylistDialog(temp)

    End Sub

    Private Sub sButtonCreatePlaylistSelected()
        If mItunesApp IsNot Nothing AndAlso mItunesApp.BrowserWindow.SelectedTracks IsNot Nothing Then
            sCreatePlaylist(mItunesApp.BrowserWindow.SelectedTracks)
        End If

    End Sub

    Private Sub CreatePlaylistOfSelectedTracksToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CreatePlaylistOfSelectedTracksToolStripMenuItem.Click
        sButtonCreatePlaylistSelected()
    End Sub

    Private Sub beDiscsBrowserInfo_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bwDiscsBrowserInfo.DoWork

        Dim lDisc As cInfoDisc = CType(e.Argument, cInfoDisc)

        If lDisc IsNot Nothing Then

            ' avoid fancy stuff while bwapp is busy
            If bwApp.IsBusy = False Then
                lDisc.StandardText = fTracksAreStandard(lDisc.Tracks)
                ' using disc's new toString method
                lDisc.TrackList = lDisc.ToString
            End If

            lDisc.ArtworkImage = lDisc.fGetArtwork

        End If

        e.Result = lDisc

    End Sub

    Private Sub beDiscsBrowserInfo_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bwDiscsBrowserInfo.RunWorkerCompleted

        Dim ldisc As cInfoDisc = CType(e.Result, cInfoDisc)

        If ldisc IsNot Nothing Then

            If ldisc.StandardText IsNot Nothing Then
                mfUpdateStatusBarText(ldisc.StandardText, False)
            End If
            If ldisc.TrackList IsNot Nothing Then
                ttApp.SetToolTip(lbDiscs, ldisc.TrackList)
            End If

            pbArtwork.Image = ldisc.ArtworkImage
            ldisc.ArtworkImage = Nothing '' clear from memory

        End If

    End Sub

    Private Sub TrackReplaceAssistantToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackReplaceAssistantToolStripMenuItem.Click

        msShowTrackReplaceAssistant()

    End Sub

    Private Sub ToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles miToolsOptions.Click
        msShowOptions()
    End Sub

    Private Sub BrowseTemporaryFiToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowseTemporaryFiToolStripMenuItem.Click
        mfOpenFileOrDirPath(My.Settings.TempDir, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub ValidateDiscToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ValidateDiscToolStripMenuItem.Click
        sButtonValidateDisc()
    End Sub

    Private Sub CreatePlaylistToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CreatePlaylistToolStripMenuItem.Click
        sButtonCreatePlaylistDisc()
    End Sub

    Private Sub ShowDiscInWindowsExplroerToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowDiscInWindowsExplroerToolStripMenuItem.Click
        sButtonBrowseDisc()
    End Sub

    Private Sub tsmCopyTracklist_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmCopyTracklist.Click

        If lbDiscs.SelectedIndex <> -1 And mTableDiscs IsNot Nothing Then

            Dim albumTitle As String = lbDiscs.SelectedItem.ToString
            Dim lDisc As cInfoDisc = CType(mTableDiscs.Item(albumTitle), cInfoDisc)

            If lDisc IsNot Nothing Then
                Dim sb As New System.Text.StringBuilder
                sb.AppendLine(String.Format(lDisc.AlbumName))
                sb.AppendLine()
                sb.AppendLine(lDisc.ToString(True, True))
                Clipboard.SetData(DataFormats.StringFormat, sb.ToString)
            End If

        End If

    End Sub

    Private Sub PlayDiscInITunesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PlayDiscInITunesToolStripMenuItem.Click
        sPlayDisc()
    End Sub


    Private Sub GoogleSearchToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GoogleSearchToolStripMenuItem.Click

        If lbDiscs.SelectedIndex <> -1 And mTableDiscs IsNot Nothing Then

            Dim albumTitle As String = lbDiscs.SelectedItem.ToString
            Dim lDisc As cInfoDisc = CType(mTableDiscs.Item(albumTitle), cInfoDisc)
            If lDisc IsNot Nothing Then
                Process.Start(lDisc.GoogleSearchURL)
            End If

        End If

    End Sub

    Private Sub picArtwork_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles pbArtwork.DoubleClick

        If lbDiscs.SelectedIndex <> -1 And mTableDiscs IsNot Nothing Then

            Dim albumTitle As String = lbDiscs.SelectedItem.ToString
            Dim lDisc As New cInfoDisc
            lDisc = CType(mTableDiscs.Item(albumTitle), cInfoDisc)

            If lDisc IsNot Nothing Then
                If lDisc.ArtworkSource IsNot Nothing Then
                    If File.Exists(lDisc.ArtworkSource.ArtworkPath) Then
                        Process.Start(lDisc.ArtworkSource.ArtworkPath)
                    End If
                End If
            End If

        End If


    End Sub

    Private Sub FileValidatorToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileValidatorToolStripMenuItem.Click
        My.Forms.frmValidator.Show()
    End Sub

    Private Sub ValidateToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ValidateToolStripMenuItem.Click
        sButtonValidateOrEditSelectedTracks()
    End Sub

    Private Function fClipboardPreview(ByVal bAll As Boolean) As String

        Dim sb As New System.Text.StringBuilder

        If mGuiReady Then

            Try

                Dim songs As New List(Of String)

                If mItunesApp IsNot Nothing AndAlso mItunesApp.BrowserWindow.SelectedTracks IsNot Nothing Then

                    If bAll Then
                        For Each track As IITTrack In mItunesApp.BrowserWindow.SelectedTracks
                            songs.Add(mfGetStringFromPattern(cboClipboardPattern.Text, track))
                        Next
                    Else
                        If mItunesApp.BrowserWindow.SelectedTracks.Count > 0 Then
                            songs.Add(mfGetStringFromPattern(cboClipboardPattern.Text, mItunesApp.BrowserWindow.SelectedTracks(1)))
                        End If
                    End If

                    If chkClipboardSort.Checked Then
                        songs.Sort()
                    End If

                    For Each song As String In songs
                        sb.AppendLine(song)
                    Next

                    lblClipboard.Text = sb.ToString

                Else

                    lblClipboard.Text = String.Empty

                End If

            Catch ex As Exception
                msAppendWarnings(ex.Message + " while attempting to preview clipboard")
            End Try

        End If

        Return sb.ToString

    End Function

    Private Sub cboClipboardPattern_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboClipboardPattern.SelectedIndexChanged
        If mGuiReady = True Then
            fClipboardPreview(bAll:=True)
        End If
    End Sub

    Private Sub cboClipboardPattern_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboClipboardPattern.TextChanged
        If mGuiReady = True Then
            fClipboardPreview(bAll:=False)
        End If
    End Sub

    Private Sub bwFS_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bwFS.DoWork

        If My.Settings.DeleteTempFiles Then

            If File.Exists(My.Settings.LibraryXMLPath) Then

                Dim tempFiles As New List(Of String)
                tempFiles.AddRange(Directory.GetFiles(Path.GetDirectoryName(My.Settings.LibraryXMLPath), "Temp File*", SearchOption.TopDirectoryOnly))
                tempFiles.AddRange(Directory.GetFiles(Path.GetDirectoryName(My.Settings.LibraryXMLPath), "iT*.tmp", SearchOption.TopDirectoryOnly))

                For Each tempFile As String In tempFiles
                    Try
                        File.Delete(tempFile)
                        msAppendDebug("Deleted " + tempFile)
                    Catch ex As Exception
                        msAppendWarnings(ex.Message + " while deleting iTunes temp file")
                    End Try
                Next
            End If

        End If

    End Sub

    Private Sub AddFolderToLibraryToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmAddFolderToLib.Click
        sButtonAddNewFiles()
    End Sub

    Private Sub chkValidationPlaylists_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkValidationPlaylists.CheckedChanged

    End Sub

    Private Sub DeleteDeadOrForeignTracksToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteDeadOrForeignTracksToolStripMenuItem.Click

        If bwApp.IsBusy = False Then

            chkDeleteTracksNotInHDD.Checked = True

            Dim t As New cBwJob(cBwJob.JobType.DELETE_DEAD_FOREIGN_TRACKS)
            bwApp.RunWorkerAsync(t)

        End If

    End Sub


    Private Sub TracksThatArtworkIsLowResolutionToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TracksThatArtworkIsLowResolutionToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathListArtworkRes, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub TracksThatArtworkWasAddedToolStripMenuItem1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TracksThatArtworkWasAddedToolStripMenuItem1.Click
        mfOpenFileOrDirPath(mFilePathArtworkAdded, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub TracksWithmultipleArtworkToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TracksWithmultipleArtworkToolStripMenuItem1.Click
        mfOpenFileOrDirPath(mFilePathMultipleArtwork, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub TracksWithoutArtworkToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TracksWithoutArtworkToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathNoArtwork, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub TracksWithoutLyricsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TracksWithoutLyricsToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathNoLyicsTracks, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub TracksLyricsWereAddedToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TracksLyricsWereAddedToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathLyricsAdded, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub SimpleWordsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SimpleWordsToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathSimpleWords, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub ReplaceWordsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReplaceWordsToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathReplaceWords, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub SkipWordsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SkipWordsToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathSkipAlbumWords, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub SettingsFolderToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SettingsFolderToolStripMenuItem.Click
        mfOpenFileOrDirPath(My.Settings.SettingsDir, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub btnSchRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSchRun.Click

        sButtonRunSchedule()

    End Sub

    Private Sub tmrAddMusicAuto_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrAddMusicAuto.Tick

        If bwApp.IsBusy = False Then
            If lbFiles.Items.Count > 0 Then

                Dim taskAddNewFiles As New cBwJob(cBwJob.JobType.ADD_NEW_TRACKS)
                bwApp.RunWorkerAsync(taskAddNewFiles)

                mSecondsLeftToAddMusic = CInt(tmrAddMusicAuto.Interval / 1000)

            End If
        End If

    End Sub

    Private Sub AlwaysOnTopToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AlwaysOnTopToolStripMenuItem.Click
        AlwaysOnTopToolStripMenuItem.Checked = Not AlwaysOnTopToolStripMenuItem.Checked
        Me.TopMost = AlwaysOnTopToolStripMenuItem.Checked
    End Sub

    Private Sub gbBackupTags_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles gbBackupTags.Enter

    End Sub


    Private Sub tcSelectedTracks_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tcSelectedTracks.SelectedIndexChanged
        sUpdateButtonControls()
    End Sub

    Private Sub chkCapitalizeFirstLetter_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkCapitalizeFirstLetter.CheckedChanged
        chkRenameFile.Enabled = chkCapitalizeFirstLetter.Checked
        chkStrict.Enabled = chkCapitalizeFirstLetter.Checked
    End Sub

    Private Sub chkReplaceTextInTags_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkReplaceTextInTags.CheckedChanged
        cboFind.Enabled = chkReplaceTextInTags.Checked
        cboReplace.Enabled = chkReplaceTextInTags.Checked
    End Sub

    Private Sub chkDecompile_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkDecompile.CheckedChanged
        cboDecompileOptions.Enabled = chkDecompile.Checked
        cboArtistsDecompiled.Enabled = chkDecompile.Checked
    End Sub

    Private Sub chkTrimChar_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkTrimChar.CheckedChanged
        nudTrimChar.Enabled = chkTrimChar.Checked
        cboTrimDirection.Enabled = chkTrimChar.Checked
    End Sub

    Private Sub cboTrimDirection_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboTrimDirection.SelectedIndexChanged
        mTrimDirectionText = cboTrimDirection.Text
    End Sub

    Private Sub txtFind_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboFind.SelectedIndexChanged
        mFindText = cboFind.Text
    End Sub

    Private Sub cboFind_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboFind.TextChanged
        mFindText = cboFind.Text
    End Sub

    Private Sub cboReplace_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboReplace.SelectedIndexChanged
        mReplaceText = cboReplace.Text
    End Sub

    Private Sub cboReplace_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboReplace.TextChanged
        mReplaceText = cboReplace.Text
    End Sub

    Private Sub pbarTracks_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles pbarTrack.MouseEnter
        If pbarTrack.Value > 0 Then
            pbarTrack.ToolTipText = String.Format("{0} of {1} done...", pbarTrack.Value, pbarTrack.Maximum)
        End If
    End Sub

    Private Sub lbFiles_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lbFiles.MouseDown

        '' ENABLES RIGHT CLICKED SELECTED INDEX 
        Dim indexOver As Integer = lbFiles.IndexFromPoint(e.X, e.Y)
        If indexOver >= 0 AndAlso indexOver < lbFiles.Items.Count Then
            lbFiles.SelectedIndex = indexOver
        End If
        lbFiles.Refresh()

    End Sub

    Private Sub lbFiles_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbFiles.MouseEnter
        cmsFiles.Enabled = lbFiles.Items.Count > 0
    End Sub

    Private Sub lbFiles_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbFiles.SelectedIndexChanged

    End Sub

    Private Sub ShowInWindowsExplorerToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowInWindowsExplorerToolStripMenuItem.Click
        sButtonShowFileInExplorer()
    End Sub

    Private Sub RemoveFromListToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RemoveFromListToolStripMenuItem.Click

        Dim temp As New List(Of String)
        For Each f As String In lbFiles.SelectedItems
            temp.Add(f)
        Next

        For Each f As String In temp
            lbFiles.Items.Remove(f)
            '' 5.35.02.4 Files > Context Menu > Remove File did not remove the file reference in Watched files
            mWatcher.Files.Remove(f)
        Next

    End Sub

    Private Sub bwWatchers_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bwWatcher.DoWork

        '' 5.35.02.3 The GUI was lagging when iTSfv was configuring watch folders in the network on application start up
        mWatcher = New cWatcher(bwApp)

    End Sub

    Private Sub tsmShowApp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmShowApp.Click
        sShowApp()
    End Sub

    Private Sub CapitalWordsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CapitalWordsToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathCapitalWords, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub BrowseSettingsFolderToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowseSettingsFolderToolStripMenuItem.Click
        mfOpenFileOrDirPath(My.Settings.SettingsDir, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub BrowseLogsFolderToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowseLogsFolderToolStripMenuItem1.Click
        mfOpenFileOrDirPath(My.Settings.LogsDir, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub tsmiDonate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmiDonate.Click

        Process.Start("http://sourceforge.net/project/project_donations.php?group_id=204248")

    End Sub

    Private Sub sButtonUpdatePlayedCount()

        If bwApp.IsBusy = False Then

            Dim t As New cBwJob(JobType.IMPORT_PLAYEDCOUNT_LASTFM)
            bwApp.RunWorkerAsync(t)

        End If

    End Sub

    Private Sub btnSyncLastFM_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSyncLastFM.Click

        sButtonUpdatePlayedCount()

    End Sub

    Private Sub tsmUpdatePlayedCount_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmUpdatePlayedCount.Click

        sButtonUpdatePlayedCount()

    End Sub

    Private Sub pbarTracks_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbarTrack.Click
        ' nothing
    End Sub

    Private Sub AlbumArtDownloaderToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AlbumArtDownloaderToolStripMenuItem.Click

        If mfConfigureAAD() = True Then
            Process.Start(My.Settings.ExePathAAD)
        End If

    End Sub

    Private Sub sBwAppRemoveDuplicateSongs()

        Dim dr As New cDupRemover(mMainLibraryTracks, bwApp)
        dr.RemoveDuplicates()

        mfUpdateStatusBarText("Ready.", False)
        bwApp.ReportProgress(ProgressType.READY)

    End Sub

    Private Sub btnRemoveDuplicates_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemoveDuplicates.Click

        If bwApp.IsBusy = False Then

            Dim t As New cBwJob(JobType.REMOVE_DUPLICATE_TRACKS)
            bwApp.RunWorkerAsync(t)

        End If

    End Sub

    Private Sub DuplicateTracksToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DuplicateTracksToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathDuplicateTracks, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub tsmTrackTagsBPM_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmTrackTagsBPM.Click
        mfOpenFileOrDirPath(mFilePathTracksNoBPM, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub tsmTrackTagsRefreshed_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmTrackTagsRefreshed.Click
        mfOpenFileOrDirPath(mFilePathTagsRefreshed, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub tsmMusicFolderActivity_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmMusicFolderActivity.Click
        mfOpenFileOrDirPath(mFilePathMusicFolderActivity, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub FoldersWithoutArtworkToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FoldersWithoutArtworkToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathFoldersNoArtwork, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub SavePlaylistOfSelectedTracksAsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SavePlaylistOfSelectedTracksAsToolStripMenuItem.Click
        sButtonCreatePlaylistSelected()
    End Sub

    Private Sub ValidateToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmiSelectedTracksValidate.Click
        ' it is in double click
    End Sub

    Private Sub CopyInfoToClipboardToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyInfoToClipboardToolStripMenuItem.Click
        sButtonCopyTagInfo()
    End Sub

    Private Sub sArtworkSearch(ByVal artist As String, ByVal album As String)

        If String.Empty <> artist AndAlso String.Empty <> album Then

            If mfConfigureAAD() = True Then

                Dim proc As New Process
                Dim psi As New ProcessStartInfo(My.Settings.ExePathAAD)
                psi.Arguments = String.Format("/artist ""{0}"" /album ""{1}"" /sort size-", artist, album)
                proc.StartInfo = psi
                proc.Start()

            End If

        End If

    End Sub

    Private Sub SearchArtworkToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchArtworkToolStripMenuItem.Click

        If mItunesApp IsNot Nothing AndAlso mItunesApp.BrowserWindow.SelectedTracks IsNot Nothing AndAlso _
        mItunesApp.BrowserWindow.SelectedTracks.Count > 0 Then

            Dim track As IITTrack = mItunesApp.BrowserWindow.SelectedTracks.Item(1)
            Dim ar As String = String.Empty
            If track.Kind = ITTrackKind.ITTrackKindFile Then
                If CType(track, IITFileOrCDTrack).AlbumArtist <> String.Empty Then
                    ar = CType(track, IITFileOrCDTrack).AlbumArtist
                Else
                    ar = track.Artist
                End If

            Else
                ar = track.Artist
            End If

            sArtworkSearch(ar, fGetAlbumToSearch(track))

        End If

    End Sub

    Private Sub ArtworkSearchToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ArtworkSearchToolStripMenuItem.Click

        Dim lDisc As cInfoDisc = fGetDiscSelected()
        If lDisc IsNot Nothing And lDisc.FirstTrack IsNot Nothing Then
            sArtworkSearch(lDisc.FirstTrack.AlbumArtist, lDisc.FirstTrack.Album)
        End If

    End Sub

    Private Sub Mp3tagToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Mp3tagToolStripMenuItem.Click

        If mfConfigureMp3tag() = True Then
            Dim proc As New Process
            Dim psi As New ProcessStartInfo(My.Settings.ExePathMp3tag)
            proc.StartInfo = psi
            proc.Start()
        End If

    End Sub

    Private Sub TrackMetatagVersionsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackMetatagVersionsToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathTrackMetatags, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub Mp3tagToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Mp3tagSelectedTracks.Click

        If mItunesApp IsNot Nothing AndAlso mItunesApp.BrowserWindow.SelectedTracks IsNot Nothing AndAlso _
 mItunesApp.BrowserWindow.SelectedTracks.Count > 0 Then

            Dim track As IITTrack = mItunesApp.BrowserWindow.SelectedTracks.Item(1)
            If track.Kind = ITTrackKind.ITTrackKindFile Then
                fMp3TagDirectory(Path.GetDirectoryName(CType(track, IITFileOrCDTrack).Location))
            End If

        End If

    End Sub

    Private Sub Mp3tagToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Mp3tagSelectedDisc.Click

        Dim lDisc As cInfoDisc = fGetDiscSelected()
        If lDisc IsNot Nothing Then
            fMp3TagDirectory(lDisc.Location)
        End If

    End Sub

    Private Function fMp3TagDirectory(ByVal folder As String) As Boolean

        If mfConfigureMp3tag() = True Then

            Dim proc As New Process
            Dim psi As New ProcessStartInfo(My.Settings.ExePathMp3tag)
            If Directory.Exists(folder) Then
                psi.Arguments = String.Format("/fp: ""{0}""", folder)
            End If
            proc.StartInfo = psi
            proc.Start()

        End If

    End Function

    Private Sub LyricViewerToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmLyricsViewer.Click
        sShowLyricViewer()
    End Sub

    Private Sub sShowLyricViewer()

        If mItunesApp IsNot Nothing AndAlso mItunesApp.BrowserWindow.SelectedTracks IsNot Nothing AndAlso mItunesApp.BrowserWindow.SelectedTracks.Count > 0 Then
            If mItunesApp.BrowserWindow.SelectedTracks(1).Kind = ITTrackKind.ITTrackKindFile Then
                Dim lv As New frmLyricsViewer(CType(mItunesApp.BrowserWindow.SelectedTracks(1), IITFileOrCDTrack))
                lv.Show()
            End If
        Else
            Dim lv As New frmLyricsViewer()
            lv.Show()
        End If

    End Sub


    Private Sub sButtonValidateSelectedTracksChecks()
        mValModes = New ValidatorModes(lChecks:=True, lTracks:=False, lLibrary:=False, lFileSystem:=False)
        sButtonValidateOrEditSelectedTracks()
    End Sub

    Private Sub sButtonValidateSelectedTracksTags()
        mValModes = New ValidatorModes(lChecks:=False, lTracks:=True, lLibrary:=False, lFileSystem:=False)
        sButtonValidateOrEditSelectedTracks()
    End Sub

    Private Sub sButtonValidateSelectedTracksLibrary()
        mValModes = New ValidatorModes(lChecks:=False, lTracks:=False, lLibrary:=True, lFileSystem:=False)
        sButtonValidateOrEditSelectedTracks()
    End Sub

    Private Sub sButtonValidateSelectedTracksFileSystem()
        mValModes = New ValidatorModes(lChecks:=False, lTracks:=False, lLibrary:=False, lFileSystem:=True)
        sButtonValidateOrEditSelectedTracks()
    End Sub

    Private Sub FileSystemToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmSelectedTracksValidateFS.Click
        sButtonValidateSelectedTracksFileSystem()
    End Sub

    Private Sub LibraryToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LibraryToolStripMenuItem1.Click
        sButtonValidateSelectedTracksLibrary()
    End Sub

    Private Sub EditTracksToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EditTracksToolStripMenuItem.Click
        sButtonValidateSelectedTracksTags()
    End Sub

    Private Sub CheckToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckToolStripMenuItem.Click
        sButtonValidateSelectedTracksChecks()
    End Sub

    Private Sub btnValidateSelectedTracksFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnValidateSelectedTracksFolder.Click
        sButtonValidateSelectedTracksFileSystem()
    End Sub

    Private Sub btnValidateSelectedTracksLibrary_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnValidateSelectedTracksLibrary.Click
        sButtonValidateSelectedTracksLibrary()
    End Sub

    Private Sub btnValidateSelectedTracks_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnValidateSelectedTracks.Click
        sButtonValidateSelectedTracksTags()
    End Sub

    Private Sub btnValidateTracksChecks_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnValidateTracksChecks.Click
        sButtonValidateSelectedTracksChecks()
    End Sub

    Private Sub SubmitDebugReportToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SubmitDebugReportToolStripMenuItem.Click
        sEmailAuto("Debug reports are attached...")
    End Sub

    Private Sub btnAdvDeleteEmptyFolders_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdvDeleteEmptyFolders.Click

        If bwApp.IsBusy = False Then

            Dim t As New cBwJob(cBwJob.JobType.DELETE_EMPTY_FOLDERS)
            bwApp.RunWorkerAsync(t)

        End If

    End Sub

    Private Sub fBwAppDeleteEmptyFolders()

        Dim fr As New cFolderRemover(mpMusicFolderPaths)
        fr.RemoveEmptyFolders()

    End Sub

    Private Sub FoldersWithOneFileToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FoldersWithOneFileToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathFoldersOneFile, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub FoldersWithoutAudioToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FoldersWithoutAudioToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathFoldersNoAudio, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub pBarDiscs_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pBarDiscs.Click
        ' nothing
    End Sub

    Private Sub pBarDiscs_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles pBarDiscs.MouseEnter

        If pBarDiscs.Value > 0 Then
            pBarDiscs.ToolTipText = String.Format("{0} of {1} done...", pBarDiscs.Value, pBarDiscs.Maximum)
        End If

    End Sub

    Private Sub ITunesArtworkGrabberToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmItunesArtworkGrabber.Click

        My.Forms.frmArtworkGrabberIT.Show()
        My.Forms.frmArtworkGrabberIT.Focus()

    End Sub

    Private Sub sButtonExportArtwork()

        If File.Exists(mArtworkGrabberFilePathMp3) = False Then
            Dim wc As New cFileDownloader(bwApp)
            wc.fDownloadArtwork()
            ' webclient will take care of it
        Else
            If bwApp.IsBusy = False Then
                Dim t As New cBwJob(cBwJob.JobType.EXPORT_ARTWORK_MANUAL)
                bwApp.RunWorkerAsync(t)
            End If
        End If

    End Sub

    Private Sub btnArtworkExport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnArtworkExport.Click

        sButtonExportArtwork()

    End Sub

    Private mArtworkJobStatus As IITFileOrCDTrack = Nothing

    Private Sub sBwAppExportArtworkSelected()

        If mItunesApp IsNot Nothing AndAlso mItunesApp.BrowserWindow.SelectedTracks IsNot Nothing Then

            If mItunesApp.BrowserWindow.SelectedTracks.Count = 1 Then

                Dim track As IITTrack = mItunesApp.BrowserWindow.SelectedTracks.Item(1)
                Dim ag As New cArtworkGrabberIT(track)
                mArtworkJobStatus = ag.fDownloadArtwork()

            ElseIf mItunesApp.BrowserWindow.SelectedTracks.Count > 1 Then

                mProgressTracksMax = mItunesApp.BrowserWindow.SelectedTracks.Count

                For Each track As IITTrack In mItunesApp.BrowserWindow.SelectedTracks

                    If track.Kind = ITTrackKind.ITTrackKindFile Then

                        Dim ag As New cArtworkGrabberIT(CType(track, IITFileOrCDTrack))
                        Dim song As IITFileOrCDTrack = ag.fDownloadArtwork()
                        If song IsNot Nothing Then
                            mfExportArtworkIT(song, My.Settings.FolderPathExArtwork)
                        Else
                            mfUpdateStatusBarText("Could not find Artwork from iTunes Store...", True)
                        End If

                    End If

                    mProgressTracksCurrent += 1

                Next

            End If

            mfUpdateStatusBarText("Exported iTunes Store Artwork...", True)

        End If

    End Sub

    Private Sub bwTimers_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bwTimers.DoWork

        While tmrSecond.Enabled
            If Me.IsDisposed = False Then
                bwTimers.ReportProgress(0)
                Threading.Thread.Sleep(1000)
            End If
        End While

    End Sub

    Private Sub bwTimers_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles bwTimers.ProgressChanged

        '***************************
        '* Set Tool Tips
        '***************************

        ttApp.SetToolTip(ssAppDisc, mpProgressDiscsMsg)
        ttApp.SetToolTip(ssAppTrack, mpProgressTracksMsg)

        If bwApp.IsBusy Or mWebClient.IsBusy Then

            '***************************
            '* Progress - Primary 
            '***************************

            If String.Empty <> mpProgressDiscsMsg Then
                sBarDisc.Text = mpProgressDiscsMsg
            End If

            '***************************
            '* Progress - Secondary
            '***************************

            If mProgressTracksCurrent > 0 Then
                pbarTrack.Style = ProgressBarStyle.Continuous
                pbarTrack.Maximum = mProgressTracksMax
                If mProgressTracksCurrent <= pbarTrack.Maximum Then
                    pbarTrack.Value = mProgressTracksCurrent
                Else
                    pbarTrack.Value = pbarTrack.Maximum
                End If
            Else
                pbarTrack.Style = ProgressBarStyle.Marquee
            End If

            If mSbDebug.Length > 0 And mSbDebug.Length < mSbDebug.MaxCapacity Then
                My.Forms.frmDebug.txtDebug.Text = mSbDebug.ToString
            End If

            If String.Empty <> mpProgressTracksMsg Then
                sBarTrack.Text = mpProgressTracksMsg
            End If

            sUpdateProgress()

        Else

            '***************************
            '* Progress - Primary
            '***************************

            If sBarDisc.Text.Contains("Ready") = False Then
                sBarDisc.Text = (String.Format("Ready. Found {0} Tracks.", mTracksCount))
            ElseIf mpProgressDiscsMsg.Contains("Ready") Then
                sBarDisc.Text = mpProgressDiscsMsg
            End If

            '***************************
            '* Progress - Secondary
            '***************************

            If String.Empty <> mpProgressTracksMsg Then
                sBarTrack.Text = "Idle. Last Action: " + mpProgressTracksMsg
            ElseIf sBarTrack.Text.Contains("Idle") Then
                sBarTrack.Text = mpProgressTracksMsg
            End If

        End If


    End Sub

    Private Sub GrabITunesStoreTrackArtworkToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmTiunesStoreArtworkGrabSelected.Click

        sButtonExportArtwork()

    End Sub

    Private Sub sBwAppExportArtwork()

        If Directory.Exists(My.Settings.LosslessMusicFolder) Then

            mfUpdateStatusBarText("Processing " + My.Settings.LosslessMusicFolder, False)
            Dim folders As String() = Directory.GetDirectories(My.Settings.LosslessMusicFolder, "*.*", SearchOption.AllDirectories)

            mProgressDiscsMax = folders.Length

            For Each folder As String In folders

                sPausePendingCheck()
                If bwApp.CancellationPending Then
                    Exit For
                End If

                mfUpdateStatusBarText("Scanning folder: " & folder, False)

                Dim filePaths As New List(Of String)
                For Each ext As String In mFileExtOtherAudio
                    filePaths.AddRange(Directory.GetFiles(folder, "*." + ext, SearchOption.TopDirectoryOnly))
                Next

                If filePaths.Count > 0 Then

                    ' is a flac dir 
                    Dim ag As New cArtworkGrabberIT(filePaths(0))
                    Dim song As IITFileOrCDTrack = ag.fDownloadArtwork()
                    If song IsNot Nothing Then
                        mfExportArtworkIT(song, folder)
                        My.Settings.ITunesArtworkLastExportFolder = folder
                    Else
                        mfUpdateStatusBarText("Could not find Artwork from iTunes Store...", True)
                    End If

                End If

                mProgressDiscsCurrent += 1

            Next

        End If

    End Sub

    Private Sub sButtonExportArtworkBatch()

        If Directory.Exists(My.Settings.LosslessMusicFolder) = False Then
            If mfGetFolderBrowser("Browse for the Lossless Audio folder...", My.Forms.frmOptions.txtFolderLossless) Then
                My.Settings.LosslessMusicFolder = My.Forms.frmOptions.txtFolderLossless.Text
            End If
        End If

        If Directory.Exists(My.Settings.LosslessMusicFolder) Then
            If bwApp.IsBusy = False Then
                Dim t As New cBwJob(cBwJob.JobType.EXPORT_ARTWORK_BATCH)
                bwApp.RunWorkerAsync(t)
            End If
        End If

    End Sub

    Private Sub btnBatchArtworkGrab_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBatchArtworkGrab.Click
        sButtonExportArtworkBatch()
    End Sub

    Private Sub tsmiArtworkConverted_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmiArtworkConverted.Click
        mfOpenFileOrDirPath(mFilePathArtworkConverted, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub sBarDisc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles sBarDisc.Click
        ' nothing
    End Sub

    Private mAppendPrepend As String = "append"
    Private Sub cboAppendChar_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboAppendChar.SelectedIndexChanged
        mAppendPrepend = cboAppendChar.Text
    End Sub

    Private Sub chkAddFile_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkAddFile.CheckedChanged

        If chkAddFile.Checked Then
            btnFindNewFiles.Image = My.Resources.folder_find
        Else
            btnFindNewFiles.Image = My.Resources.find
        End If

    End Sub

    Private Sub WakoopaToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WakoopaToolStripMenuItem.Click
        Process.Start("http://wakoopa.com/software/itunes-store-file-validator")
    End Sub

    Private Sub BetaVersionsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BetaVersionsToolStripMenuItem.Click
        Process.Start("http://sourceforge.net/project/showfiles.php?group_id=204248&package_id=243989")
    End Sub

    Private Sub VerboseModeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles VerboseModeToolStripMenuItem.Click

        VerboseModeToolStripMenuItem.Checked = Not VerboseModeToolStripMenuItem.Checked
        lbVerbose.Visible = VerboseModeToolStripMenuItem.Checked

        If VerboseModeToolStripMenuItem.Checked Then
            tlpMain.RowStyles(2).Height = 25.0!
        Else
            tlpMain.RowStyles(2).Height = 0.0!
        End If


    End Sub

    Private Sub btnBatchArtworkGrab_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBatchArtworkGrab.MouseEnter

        If String.Empty <> My.Settings.ITunesArtworkLastExportFolder Then
            ttApp.SetToolTip(btnBatchArtworkGrab, "Last folder Artwork exported: " + My.Settings.ITunesArtworkLastExportFolder)
        End If

    End Sub

    Private Sub OhlohToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OhlohToolStripMenuItem.Click
        Process.Start("http://www.ohloh.net/projects/12169")
    End Sub

    Private Sub SVNRepositoryToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SVNRepositoryToolStripMenuItem.Click
        Process.Start("http://code.google.com/p/itsfv/source/checkout")
    End Sub

    Private Sub tsmiSelectedTracksValidate_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tsmiSelectedTracksValidate.DoubleClick
        sButtonValidateOrEditSelectedTracks()
    End Sub

    Private Sub tsmiSearch_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tsmiSearch.DoubleClick
        If mItunesApp IsNot Nothing AndAlso mItunesApp.BrowserWindow.SelectedTracks IsNot Nothing AndAlso _
mItunesApp.BrowserWindow.SelectedTracks.Count > 0 Then

            Dim track As IITTrack = mItunesApp.BrowserWindow.SelectedTracks.Item(1)
            Dim url As String = "http://www.google.com/search?q=" + mfEncodeUrl(mfGetStringFromPattern(My.Settings.GoogleTrack, track))
            Process.Start(url)

        End If
    End Sub

    Private Sub DiggToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DiggToolStripMenuItem.Click
        Process.Start("http://digg.com/software/iTSfv_Hands_down_the_best_iTunes_addon_you_could_ever_ask")
    End Sub

    Private Sub bwQueueFiles_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bwQueueFiles.DoWork

        Dim fp As List(Of String) = CType(e.Argument, Global.System.Collections.Generic.List(Of String))
        bwQueueFiles.RunWorkerAsync(fp)

    End Sub

    Private Sub bwQueueFiles_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles bwQueueFiles.ProgressChanged

        mCurrProgress = CType(e.ProgressPercentage, ProgressType)

        Dim userStateString As String = ""
        If e.UserState IsNot Nothing AndAlso TypeOf (e.UserState) Is String Then
            userStateString = e.UserState.ToString
        End If

        Select Case mCurrProgress

            Case ProgressType.ADD_TRACKS_TO_LISTBOX_TRACKS
                sQueueFileToListBoxTracks(userStateString)

            Case ProgressType.REMOVE_TRACK_FROM_LISTBOX
                lbFiles.Items.Remove(userStateString)

        End Select


    End Sub

    Private Sub tsmiSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmiSearch.Click
        'nothing?
    End Sub

    Private Sub sButtonWritePOPM()

        If bwApp.IsBusy = False Then
            Dim t As New cBwJob(cBwJob.JobType.WRITE_POPM_PCNT)
            bwApp.RunWorkerAsync(t)
        End If

    End Sub

    Private Sub sBwAppWritePOPM()
        mJobsIT.msBwAppExportPOPM(bwApp, mMainLibraryTracks)
    End Sub

    Private Sub btnWritePOPM_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWritePOPM.Click
        sButtonWritePOPM()
    End Sub

    Private Sub sButtonImportPOPM()
        If bwApp.IsBusy = False Then
            Dim t As New cBwJob(cBwJob.JobType.IMPORT_POPM_PCNT)
            bwApp.RunWorkerAsync(t)
        End If
    End Sub

    Private Sub btnImportPOPM_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnImportPOPM.Click
        sButtonImportPOPM()
    End Sub

    Private Sub cboExportFilePattern_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboExportFilePattern.TextChanged
        mExportFilePattern = cboExportFilePattern.Text
    End Sub

    Private Sub IgnoreWordsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles IgnoreWordsToolStripMenuItem.Click
        mfOpenFileOrDirPath(mFilePathIgnoreWords, sBarTrack, ttApp, ssAppDisc)
    End Sub

    Private Sub tsmSetInfo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmSetInfo.Click

        Dim f As New frmSetInfo()
        f.Show()

    End Sub

    Private Sub GoogleGroupsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GoogleGroupsToolStripMenuItem.Click
        Process.Start("http://groups.google.com/group/itsfv")
    End Sub

    Private Sub ProjectHomeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProjectHomeToolStripMenuItem.Click
        Process.Start("http://itsfv.googlecode.com")
    End Sub

    Private Sub SaveStatisticsFileAsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveStatisticsFileAsToolStripMenuItem.Click
        SaveStatisticsFile()
    End Sub

    Private Sub SaveStatisticsFile()

        If mStatsMaker Is Nothing Then
            sShowStatistics(mStatsMaker)
        End If

        If mStatsMaker IsNot Nothing Then

            Dim dlg As New SaveFileDialog
            dlg.InitialDirectory = My.Settings.LogsDir
            dlg.Filter = DLG_FILTER_STATS
            dlg.FileName = String.Format("{0}-{1}-stats.cache", mXmlLibParser.LibraryPersistantID, Now.ToString("yyyyMMdd"))
            If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
                mfWriteObjectToFileBF(mStatsMaker, dlg.FileName)
            End If

        End If

    End Sub

    Private Sub chkImportLyrics_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkImportLyrics.CheckedChanged

    End Sub
End Class



