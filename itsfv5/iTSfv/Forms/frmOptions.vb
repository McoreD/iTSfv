Imports System.IO
Imports System.Collections.Specialized
Imports System.Text

Public Class frmOptions

    Private mBooEnableApply As Boolean = False
    Private mBooDiscsReload As Boolean = False
    Private mBooGuiIsReady As Boolean = False

    Private Sub sGuiMode()

        gpAppMode.Enabled = (mAppInfo.ApplicationState = McoreSystem.AppInfo.SoftwareCycle.ALPHA) Or _
                            (mAppInfo.ApplicationState = McoreSystem.AppInfo.SoftwareCycle.BETA)

        '' these settings are for iTSfv only 
        gbRulesAddNewFiles.Enabled = (mpAppMode = eAppMode.ITSFV)
        gbRulesExplorer.Enabled = (mpAppMode = eAppMode.ITSFV)
        If mpAppMode = eAppMode.AFV Then
            For Each ctl As Control In tpExplorer.Controls
                ctl.Enabled = False
            Next
        End If

        Me.Text = mpAppName + " Options"

    End Sub

    Private Sub frmOptions_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        sGuiMode()

        My.Forms.frmMain.sSettingsSave() ' save options set by user before accessing options

        Me.Icon = My.Forms.frmMain.Icon
        tcOptions.ImageList = My.Forms.frmMain.tcTabs.ImageList

        Dim i As Integer = 0
        For Each tab As TabPage In My.Forms.frmMain.tcTabs.TabPages
            tcOptions.TabPages(i).Text = tab.Text
            tcOptions.TabPages(i).ImageKey = tab.ImageKey
            i += 1
        Next

        If mpAppMode <> eAppMode.WMPFV Then
            tcFileSystem.TabPages.Remove(tpFSMediaCenter)
        End If

        tcValidate.ImageList = My.Forms.frmMain.tcValidate.ImageList
        For i = 0 To My.Forms.frmMain.tcValidate.TabCount - 1
            tcValidate.TabPages(i).Text = My.Forms.frmMain.tcValidate.TabPages(i).Text
            tcValidate.TabPages(i).ImageKey = My.Forms.frmMain.tcValidate.TabPages(i).ImageKey
        Next

        tcOneTouch.ImageList = My.Forms.frmMain.tcOneTouch.ImageList
        For i = 0 To My.Forms.frmMain.tcOneTouch.TabCount - 1
            tcOneTouch.TabPages(i).Text = My.Forms.frmMain.tcOneTouch.TabPages(i).Text
            tcOneTouch.TabPages(i).ImageKey = My.Forms.frmMain.tcOneTouch.TabPages(i).ImageKey
        Next

        sSettingsGet()

        gbMusicFolderLocations.Text = Environment.UserName & "'s Music Folder locations"

        ttApp.SetToolTip(rbImArtworkFolderDefault, rbImArtworkFolderDefault.Text)
        ttApp.SetToolTip(rbExArtworkFolderDefault, rbExArtworkFolderDefault.Text)
        ttApp.SetToolTip(cboExArtworkFilenamePatterns, mfGetText("TagsSupported.txt"))
        ttApp.SetToolTip(cboImArtworkFileName, mfGetText("TagsSupported.txt"))
        ttApp.SetToolTip(cboPlaylistFileNamePattern, mfGetText("TagsSupported.txt"))
        ttApp.SetToolTip(txtGoogleTrack, mfGetText("TagsSupported.txt"))

        Dim sbLossless As New StringBuilder
        sbLossless.AppendLine("This folder is used for:")
        sbLossless.AppendLine("Exporting iTunes Store Artwork using OneTouch > File System")
        lblLosslessMusic.Text = sbLossless.ToString
        lblLosslessMusic.AutoSize = True

        tcOptions.SelectedIndex = My.Forms.frmMain.tcTabs.SelectedIndex
        tcValidate.SelectedIndex = My.Forms.frmMain.tcValidate.SelectedIndex
        tcOneTouch.SelectedIndex = My.Forms.frmMain.tcOneTouch.SelectedIndex

        tcTracks.SelectedTab = tpTracksArtwork
        tcFileSystem.SelectedTab = tpFSArtwork

        mBooDiscsReload = False

        hpApp.SetHelpString(cboExArtworkFilenamePatterns, mfGetText("TagsSupported.txt"))
        hpApp.SetHelpString(cboPlaylistFileNamePattern, mfGetText("TagsSupported.txt"))

    End Sub

    Private Sub sSettingsGetTracks()

        '********
        ' General
        '********

        chkModifiedDateRetain.Checked = My.Settings.ModifiedDateRetain
        chkFillYear.Checked = My.Settings.FillYear
        chkResizeArtwork.Checked = My.Settings.ResizeArtwork

        cboSingleDiscSuffix.Items.Clear()
        For Each item As String In My.Settings.SingleDiscSuffixes
            If item <> String.Empty Then
                cboSingleDiscSuffix.Items.Add(item)
            End If
        Next

        '********
        ' lyrics 
        '********

        rbLyricsImportDirDefault.Checked = My.Settings.LyricsFromAlbumFolder
        rbLyricsImportDirCustom.Checked = Not rbLyricsImportDirDefault.Checked

        cboLyricsExtIm.Text = My.Settings.LyricsFileExtIm

    End Sub

    Private Sub sSettingsGetExplorer()

        txtMusicFolder.Text = mpMusicFolderPath
        cboAddFilesMode.Items.Clear()
        cboAddFilesMode.Items.Add(String.Format("Copy files to {0}", mpMusicFolderPath))
        cboAddFilesMode.Items.Add("Do Nothing or Let iTunes determine destination")
        cboAddFilesMode.Items.Add(String.Format("Move files to {0}", mpMusicFolderPath))
        cboAddFilesMode.SelectedIndex = My.Settings.AddFilesMode

        If My.Settings.LibraryXMLPath.Length > 0 Then
            chkDeleteTempFiles.Text = String.Format("Delete Temp Files when found in {0}", Path.GetDirectoryName(My.Settings.LibraryXMLPath))
        End If

        chkWarnNoTrackNumber.Checked = My.Settings.WarnNoTrackNumber

        lbMusicFolders.Items.Clear()
        If mpMusicFolderPaths IsNot Nothing Then
            For Each lloc As String In mpMusicFolderPaths
                If Not lbMusicFolders.Items.Contains(lloc) Then lbMusicFolders.Items.Add(lloc)
            Next
        End If

        If My.Settings.ExcludeMusicFolders IsNot Nothing Then
            For Each lloc As String In My.Settings.ExcludeMusicFolders
                If Not lbExcludeFolders.Items.Contains(lloc) Then lbExcludeFolders.Items.Add(lloc)
            Next
        End If

        cboMusicFolderStructure.Text = My.Settings.MusicFolderStructure
        For Each pattern As String In My.Settings.MusicFolderStructures
            If cboMusicFolderStructure.Items.Contains(pattern) = False Then
                cboMusicFolderStructure.Items.Add(pattern)
            End If
        Next

        lblAddMode.Text = CStr(IIf(chkSyncSilent.Checked, "Add Mode: Unattended", "Add Mode: Manual"))


    End Sub

    Private Sub sSettingsGetFileSystem()

        '*********
        ' Artwork 
        '*********

        cboImArtworkFileName.Items.Clear()
        cboExArtworkFileName.Items.Clear()

        For Each item As String In My.Settings.ArtworkFileNames
            If item <> "" Then
                If Not cboImArtworkFileName.Items.Contains(item) Then cboImArtworkFileName.Items.Add(item)
                If Not cboExArtworkFileName.Items.Contains(item) Then cboExArtworkFileName.Items.Add(item)
            End If
        Next
        For Each item As String In My.Settings.FileNamePatterns
            If item <> "" Then
                If Not cboImArtworkFileName.Items.Contains(item) Then cboImArtworkFileName.Items.Add(item)
                If Not cboExArtworkFileName.Items.Contains(item) Then cboExArtworkFileName.Items.Add(item)
            End If
        Next

        chkUseArtworkResize.Checked = My.Settings.UseArtworkResize
        chkDisableAlbumArtSmallJpg.Checked = My.Settings.DisableJpgAlbumArtSmall
        chkDisableFolderJpg.Checked = My.Settings.DisableJpgFolder
        chkDisableArtworkJpg.Checked = My.Settings.DisableJpgArtwork

        '********
        ' Lyrics 
        '********

        rbExLyricsFolderCustom.Checked = Not rbExLyricsFolderDefault.Checked

        cboLyricsFileNamePatternEx.Items.Clear()
        cboLyricsFilenamePatternIm.Items.Clear()

        rbLyricsPatternCustomIm.Checked = Not My.Settings.LyricsPatternFromFileIm
        rbLyricsPatternCustomEx.Checked = Not My.Settings.LyricsPatternFromFileEx

        For Each item As String In My.Settings.LyricsFilenamePatterns
            If item <> "" Then
                If Not cboLyricsFileNamePatternEx.Items.Contains(item) Then cboLyricsFileNamePatternEx.Items.Add(item)
                If Not cboLyricsFilenamePatternIm.Items.Contains(item) Then cboLyricsFilenamePatternIm.Items.Add(item)
            End If
        Next

        cboLyricsExtEx.Text = My.Settings.LyricsFileExtEx

        '*******************
        ' Playlists / Index 
        '*******************

        cboPlaylistFileNamePattern.Items.Clear()

        For Each item As String In My.Settings.GenericFileNamePatterns
            If item <> "" Then
                If Not cboPlaylistFileNamePattern.Items.Contains(item) Then cboPlaylistFileNamePattern.Items.Add(item)
                If Not cboIndexFileNamePattern.Items.Contains(item) Then cboIndexFileNamePattern.Items.Add(item)
            End If
        Next

        cboPlaylistFileNamePattern.Text = My.Settings.PlaylistFileNamePattern
        cboPlaylistType.Text = My.Settings.PlaylistExt

        '***************
        '* Media Center
        '***************
        Dim sb As New StringBuilder
        sb.AppendLine(String.Format("UrlToCover.dat will use {0} as the Artwork", IIf(My.Settings.DefaultExArtworkFolder, My.Settings.ArtworkFileNameEx, My.Settings.ArtworkFileNamePatternEx)))
        sb.AppendLine(String.Format("from {0}", CStr(IIf(My.Settings.DefaultExArtworkFolder, "Album folder", My.Settings.FolderPathExArtwork))))

        lblMediaCenterNotes.Text = sb.ToString

    End Sub

    Friend Sub sSettingsGet()

        '' this is the very first setting that should be loaded
        msAppModeGet()

        ' Library
        nudITScomplianceRate.Value = My.Settings.iTScomplianceRate
        chkMusicAudioOnly.Checked = My.Settings.MusicAudioOnly
        chkIncludePodcasts.Checked = My.Settings.IncludePodcasts
        chkEmptyTagsInclude.Checked = My.Settings.EmptyTagsInclude
        nudArtworkWidth.Value = My.Settings.LowResArtworkWidth
        nudArtworkHeight.Value = My.Settings.LowResArtworkHeight

        nudFolderJpgMinSize.Value = CType(My.Settings.FolderJpgMinSize / 1024, Decimal)

        sSettingsGetTracks()

        sSettingsGetAdvanced()

        '*******
        ' import 
        '*******
        chkImportAnyName.Checked = My.Settings.ImportAnySingleArtwork
        chkAlwaysHighResLibrary.Checked = My.Settings.AlwaysHighResLibrary
        'chkAlwaysHighResSelected.Checked = My.Settings.AlwaysHighResSelected
        chkAlwaysHighResLast100.Checked = My.Settings.AlwaysHighResLast100

        rbImArtworkFolderDefault.Checked = My.Settings.DefaultImArtworkFolder
        rbImportArtworkFolderOverride.Checked = Not rbImArtworkFolderDefault.Checked

        ' artwork
        txtImArtworkFolderPath.Text = My.Settings.FolderPathImArtwork
        txtExArtworkFolderPath.Text = My.Settings.FolderPathExArtwork

        chkOnlySquareArtwork.Checked = My.Settings.ArtworkSquaredPrefer

        '*******
        ' Export
        '*******
        ' lyrics
        sSettingsGetFileSystem()


        'chkDisableArtworkResized.Checked = My.Settings.DisableJpgArtworkResize
        rbExArtworkFolderDefault.Checked = My.Settings.DefaultExArtworkFolder
        rbExArtworkFolderCustom.Checked = Not rbExArtworkFolderDefault.Checked

        ' Album Browser
        chkTagBlankAlbum.Checked = My.Settings.TagBlankAlbum

        ' Explorer
        sSettingsGetExplorer()

        chkCapitalizeWordNewTrack.Checked = My.Settings.CapitalizeNewTrack
        ' General
        chkRemainingInsteadOfEToC.Checked = My.Settings.ShowEToC

        chkCheckUpdates.Checked = My.Settings.AutoCheckUpdates
        nudTimeoutSeconds.Value = My.Settings.TimeoutITMS
        nudPieNumber.Value = My.Settings.PieNumber


        cboImArtworkFileName.Text = My.Settings.ImArtworkFileName
        cboExArtworkFileName.SelectedIndex = My.Settings.ExArtworkFileSelectedIndex

        cboIndexFileNamePattern.Text = My.Settings.IndexFileNamePattern
        cboIndexFileExt.Text = My.Settings.IndexFileExt

        cboImArtworkFilenamePatterns.Items.Clear()
        cboExArtworkFilenamePatterns.Items.Clear()

        For Each item As String In My.Settings.FileNamePatterns
            If Not cboImArtworkFilenamePatterns.Items.Contains(item) Then cboImArtworkFilenamePatterns.Items.Add(item)
            If Not cboExArtworkFilenamePatterns.Items.Contains(item) Then cboExArtworkFilenamePatterns.Items.Add(item)
        Next

        cboImArtworkFilenamePatterns.Text = My.Settings.ArtworkFileNamePatternIm
        cboExArtworkFilenamePatterns.Text = My.Settings.ArtworkFileNamePatternEx

        ' order is important
        nudWeightPlayedCount.Value = My.Settings.WeightPlayedCount
        nudWeightSkippedCount.Value = My.Settings.WeightSkippedCount
        nudWeightLastPlayed.Value = My.Settings.WeightLastPlayed
        nudWeightDateAdded.Value = My.Settings.WeightDateAdded

        ' advanced / folders        
        txtArtworkDir.Text = My.Settings.ArtworkDir
        txtLogsDir.Text = My.Settings.LogsDir
        txtSettingsDir.Text = My.Settings.SettingsDir
        txtTempDir.Text = My.Settings.TempDir
        txtSettingsDir.Text = My.Settings.SettingsDir

        cboPowerOptions.SelectedIndex = My.Settings.PowerOption

        If My.Settings.ScheduleTime = "" OrElse _
            My.Settings.ScheduleTime = Now.ToString("HH:mm:ss") Then
            My.Settings.ScheduleTime = "03:00:00"
        End If

        dtpTime.Text = My.Settings.ScheduleTime

        chkMonday.Checked = My.Settings.OnMonday
        chkTuesday.Checked = My.Settings.OnTuesday
        chkWednesday.Checked = My.Settings.OnWednesday
        chkThursday.Checked = My.Settings.OnThursday
        chkFriday.Checked = My.Settings.OnFriday
        chkSaturday.Checked = My.Settings.OnSaturday
        chkSunday.Checked = My.Settings.OnSunday

        chkMinimizeToTray.Checked = My.Settings.MinimizeToTray

        Select Case My.Settings.AlbumBrowserMode
            Case Is = 0
                rbAbArtistGroupingDiscAlbum.Checked = True
            Case Is = 1
                rbAbArtistDiscAlbum.Checked = True
            Case Is = 2
                rbAbAlbumArtistDisc.Checked = True
        End Select

        chkOnlyHighResArtwork.Text = String.Format("Do not import if Artwork resolution is less than {0} x {1}", My.Settings.LowResArtworkWidth, My.Settings.LowResArtworkHeight)
        chkResizeArtwork.Text = String.Format("Resize large Artwork to {0} x {1} before importing", My.Settings.LowResArtworkWidth, My.Settings.LowResArtworkHeight)
        chkDisableArtworkResized.Text = String.Format("Artwork {0}x{1}.jpg", My.Settings.LowResArtworkWidth, My.Settings.LowResArtworkHeight)

        chkTrackArtworkShowAll.Enabled = chkArtworkChooseManual.Checked

    End Sub

    Private Function sAddFileNames(ByVal fName As String) As Boolean

        Dim success As Boolean = False

        If Not My.Settings.ArtworkFileNames.Contains(fName) Then
            My.Settings.ArtworkFileNames.Add(fName)
            success = True
        End If

        Return success

    End Function

    Private Function sAddFileNamePatterns(ByVal pattern As String) As Boolean

        Dim success As Boolean = False

        If Not My.Settings.FileNamePatterns.Contains(pattern) Then
            My.Settings.FileNamePatterns.Add(pattern)
            success = True
        End If

        Return success

    End Function

    Private Sub sSettingsSaveTracks()

        ' General
        My.Settings.ModifiedDateRetain = chkModifiedDateRetain.Checked
        My.Settings.FillYear = chkFillYear.Checked
        My.Settings.ResizeArtwork = chkResizeArtwork.Checked

        If My.Settings.SingleDiscSuffixes.Contains(cboSingleDiscSuffix.Text) = False Then
            My.Settings.SingleDiscSuffixes.Add(cboSingleDiscSuffix.Text)
        End If

        ' Lyrics
        My.Settings.LyricsFileExtIm = cboLyricsExtIm.Text
        My.Settings.LyricsFromAlbumFolder = rbLyricsImportDirDefault.Checked

    End Sub

    Private Sub sSettingsSaveFileSystem()

        My.Settings.LyricsFileExtEx = cboLyricsExtEx.Text

        If Not My.Settings.LyricsFilenamePatterns.Contains(cboLyricsFileNamePatternEx.Text) Then
            My.Settings.LyricsFilenamePatterns.Add(cboLyricsFileNamePatternEx.Text)
        End If

        My.Settings.PlaylistExt = cboPlaylistType.Text
        My.Settings.IndexFileExt = cboIndexFileExt.Text

        My.Settings.ArtworkFileNameEx = cboExArtworkFileName.Text
        My.Settings.ArtworkFileNameIm = cboImArtworkFileName.Text

        My.Settings.ImArtworkFileName = cboImArtworkFileName.Text
        My.Settings.ExArtworkFileSelectedIndex = Math.Max(cboExArtworkFileName.SelectedIndex, 0)

        My.Settings.ArtworkFileNamePatternIm = cboImArtworkFilenamePatterns.Text
        My.Settings.ArtworkFileNamePatternEx = cboExArtworkFilenamePatterns.Text

        My.Settings.PlaylistFileNamePattern = cboPlaylistFileNamePattern.Text

        sAddFileNamePatterns(cboImArtworkFilenamePatterns.Text)
        sAddFileNamePatterns(cboExArtworkFilenamePatterns.Text)

        My.Settings.ArtworkFileNamePatternEx = cboExArtworkFilenamePatterns.Text
        My.Settings.ArtworkFileNamePatternIm = cboImArtworkFilenamePatterns.Text

        ' Export
        My.Settings.UseArtworkResize = chkUseArtworkResize.Checked
        My.Settings.DisableJpgAlbumArtSmall = chkDisableAlbumArtSmallJpg.Checked
        My.Settings.DisableJpgFolder = chkDisableFolderJpg.Checked
        My.Settings.DisableJpgArtwork = chkDisableArtworkJpg.Checked
        'My.Settings.DisableJpgArtworkResize = chkDisableArtworkResized.Checked

    End Sub

    Private Sub sSettingsGetAdvanced()



    End Sub

    Private Sub sSettingsSaveAdvanced()

        Dim regKey As Microsoft.Win32.RegistryKey = _
       Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run", True)

        If regKey IsNot Nothing Then
            If My.Settings.LoadWithWindows Then
                regKey.SetValue(Application.ProductName, Application.ExecutablePath)
            Else
                regKey.DeleteValue(Application.ProductName, False)
            End If
        End If

        ' advanced - appearance
        My.Settings.PowerOption = cboPowerOptions.SelectedIndex

        ' advanced > files & folders

        My.Settings.ArtworkDir = txtArtworkDir.Text
        My.Settings.LogsDir = txtLogsDir.Text
        My.Settings.SettingsDir = txtSettingsDir.Text
        My.Settings.TempDir = txtTempDir.Text

        ' advanced > internet
        My.Settings.TimeoutITMS = CInt(nudTimeoutSeconds.Value)

        ' advanced > schedule
        My.Settings.OnMonday = chkMonday.Checked
        My.Settings.OnTuesday = chkTuesday.Checked
        My.Settings.OnWednesday = chkWednesday.Checked
        My.Settings.OnThursday = chkThursday.Checked
        My.Settings.OnFriday = chkFriday.Checked
        My.Settings.OnSaturday = chkSaturday.Checked
        My.Settings.OnSunday = chkSunday.Checked

        ' advanced > statistics
        My.Settings.MinimizeToTray = chkMinimizeToTray.Checked
        My.Settings.PieNumber = CInt(nudPieNumber.Value)

        If rbAbArtistGroupingDiscAlbum.Checked = True Then
            My.Settings.AlbumBrowserMode = 0
        ElseIf rbAbArtistDiscAlbum.Checked = True Then
            My.Settings.AlbumBrowserMode = 1
        ElseIf rbAbAlbumArtistDisc.Checked = True Then
            My.Settings.AlbumBrowserMode = 2
        End If


    End Sub

    Private Sub sSettingsSaveExplorer()

        My.Settings.MusicFolderStructure = cboMusicFolderStructure.Text
        If My.Settings.MusicFolderStructures.Contains(cboMusicFolderStructure.Text) = False Then
            My.Settings.MusicFolderStructures.Add(cboMusicFolderStructure.Text)
        End If

        My.Settings.AddFilesMode = CType(cboAddFilesMode.SelectedIndex, AddFilesType)

        mpMusicFolderPath = txtMusicFolder.Text

        If mpMusicFolderPaths IsNot Nothing Then

            mpMusicFolderPaths.Clear()

            For Each lloc As String In lbMusicFolders.Items

                If lloc.EndsWith(Path.DirectorySeparatorChar) = False Then
                    lloc += Path.DirectorySeparatorChar
                End If

                If mpMusicFolderPaths.Contains(lloc) = False Then
                    mpMusicFolderPaths.Add(lloc)
                End If

            Next

        End If

        If My.Settings.ExcludeMusicFolders IsNot Nothing Then

            For Each lloc As String In lbExcludeFolders.Items

                If lloc.EndsWith(Path.DirectorySeparatorChar) = False Then
                    lloc += Path.DirectorySeparatorChar
                End If

                If My.Settings.ExcludeMusicFolders.Contains(lloc) = False Then
                    My.Settings.ExcludeMusicFolders.Add(lloc)
                End If

            Next

        End If

    End Sub

    Private Sub sSettingsSave()

        sSettingsSaveTracks()
        sSettingsSaveAdvanced()

        ' Library 
        My.Settings.iTScomplianceRate = CType(nudITScomplianceRate.Value, Integer)

        My.Settings.MusicAudioOnly = chkMusicAudioOnly.Checked
        My.Settings.IncludePodcasts = chkIncludePodcasts.Checked
        My.Settings.EmptyTagsInclude = chkEmptyTagsInclude.Checked

        My.Settings.FolderPathImArtwork = txtImArtworkFolderPath.Text
        My.Settings.FolderPathExArtwork = txtExArtworkFolderPath.Text
        My.Settings.FolderJpgMinSize = nudFolderJpgMinSize.Value * 1024

        My.Settings.LowResArtworkWidth = CInt(nudArtworkWidth.Value)
        My.Settings.LowResArtworkHeight = CInt(nudArtworkHeight.Value)

        ' Explorer 

        My.Settings.WarnNoTrackNumber = chkWarnNoTrackNumber.Checked

        ' General
        My.Settings.ShowEToC = chkRemainingInsteadOfEToC.Checked
        My.Settings.DefaultImArtworkFolder = rbImArtworkFolderDefault.Checked
        My.Settings.DefaultExArtworkFolder = rbExArtworkFolderDefault.Checked
        My.Settings.CapitalizeNewTrack = chkCapitalizeWordNewTrack.Checked

        ' Import 
        My.Settings.ImportAnySingleArtwork = chkImportAnyName.Checked
        My.Settings.AlwaysHighResLibrary = chkAlwaysHighResLibrary.Checked
        'My.Settings.AlwaysHighResSelected = chkAlwaysHighResSelected.Checked
        My.Settings.AlwaysHighResLast100 = chkAlwaysHighResLast100.Checked

        My.Settings.ArtworkSquaredPrefer = chkOnlySquareArtwork.Checked

        ' Album Browser
        My.Settings.TagBlankAlbum = chkTagBlankAlbum.Checked

        My.Settings.AutoCheckUpdates = chkCheckUpdates.Checked

        sAddFileNames(cboExArtworkFileName.Text)
        sAddFileNames(cboImArtworkFileName.Text)

        If Not My.Settings.GenericFileNamePatterns.Contains(cboPlaylistFileNamePattern.Text) Then
            My.Settings.GenericFileNamePatterns.Add(cboPlaylistFileNamePattern.Text)
        End If
        If Not My.Settings.GenericFileNamePatterns.Contains(cboIndexFileNamePattern.Text) Then
            My.Settings.GenericFileNamePatterns.Add(cboIndexFileNamePattern.Text)
        End If

        sSettingsSaveExplorer()

        sSettingsSaveFileSystem()

        My.Settings.WeightDateAdded = CType(nudWeightDateAdded.Value, Integer)
        My.Settings.WeightLastPlayed = CType(nudWeightLastPlayed.Value, Integer)
        My.Settings.WeightPlayedCount = CType(nudWeightPlayedCount.Value, Integer)
        My.Settings.WeightSkippedCount = CType(nudWeightSkippedCount.Value, Integer)

        If (Now.ToString("HH:mm:ss") <> dtpTime.Text) Then
            ' otherwise there are rare cases where sometimes dtpTime gets now time
            My.Settings.ScheduleTime = dtpTime.Text
        End If

        '' this is the very last setting that should be saved
        msAppModeSet()

        ' sometimes it doesnt get written to user.config automtically so we do it manually
        My.Settings.Save()

    End Sub

    Private Sub btnBrowseMusicFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowseMusicFolder.Click

        ' 5.29.1.1 Directory seperator charactor was not appeneded to the music folder browsed from Options > Explorer
        If mfGetFolderBrowser("Browse for alternate Music folder location", txtMusicFolder) = True Then
            Dim dir As String = txtMusicFolder.Text
            If Not lbMusicFolders.Items.Contains(dir) Then
                lbMusicFolders.Items.Add(dir)
            End If
            btnApply.Enabled = True
        End If

    End Sub

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click

        sSettingsSave()
        Me.Close()

        If lblRestartApp.Visible = True Then

            msCloseForms()

        Else

            My.Forms.frmMain.sSettingsGet() ' update the main form gui based on options
            My.Forms.frmValidator.sSettingsGet()
            If mBooDiscsReload = True Then
                My.Forms.frmMain.sReloadDiscsToAlbumBrowser()
            End If

        End If

    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub


    Private Sub btnBrowseArtworkDir_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowseImArtworkDir.Click


        Dim dlg As New McoreSystem.FolderBrowser
        dlg.Title = "Browse for alternate Artwork folder location"
        dlg.Flags = McoreSystem.BrowseFlags.BIF_NEWDIALOGSTYLE Or _
                    McoreSystem.BrowseFlags.BIF_STATUSTEXT Or _
                    McoreSystem.BrowseFlags.BIF_EDITBOX

        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            If dlg.DirectoryPath.Length > 0 Then
                txtImArtworkFolderPath.Text = dlg.DirectoryPath
                btnApply.Enabled = True
            End If
        End If


    End Sub

    Private Sub rbArtworkFolderDefault_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbImArtworkFolderDefault.CheckedChanged

        txtImArtworkFolderPath.Enabled = Not rbImArtworkFolderDefault.Checked
        cboImArtworkFilenamePatterns.Enabled = Not rbImArtworkFolderDefault.Checked
        btnBrowseImArtworkDir.Enabled = Not rbImArtworkFolderDefault.Checked

    End Sub

    Private Sub btnBrowseExArtworkDir_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowseExArtworkDir.Click

        Dim dlg As New McoreSystem.FolderBrowser
        dlg.Title = "Browse for alternate Artwork folder location"
        dlg.Flags = McoreSystem.BrowseFlags.BIF_NEWDIALOGSTYLE Or _
                    McoreSystem.BrowseFlags.BIF_STATUSTEXT Or _
                    McoreSystem.BrowseFlags.BIF_EDITBOX

        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            If dlg.DirectoryPath.Length > 0 Then
                txtExArtworkFolderPath.Text = dlg.DirectoryPath
                btnApply.Enabled = True
            End If
        End If

    End Sub

    Private Sub rbExportArtworkFolderDefault_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbExArtworkFolderDefault.CheckedChanged

        txtExArtworkFolderPath.Enabled = Not rbExArtworkFolderDefault.Checked
        cboExArtworkFilenamePatterns.Enabled = Not rbExArtworkFolderDefault.Checked
        btnBrowseExArtworkDir.Enabled = Not rbExArtworkFolderDefault.Checked

    End Sub

    Private Sub nudWeightPlayedCount_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nudWeightPlayedCount.ValueChanged

        If mBooGuiIsReady Then sCalcRatio()

    End Sub

    Private Sub sCalcRatio()

        Dim intValue As Integer = 0


        intValue = CType(100 - nudWeightPlayedCount.Value - nudWeightSkippedCount.Value - nudWeightLastPlayed.Value, Integer)
        If intValue > 0 Then
            nudWeightDateAdded.Value = intValue ' date added is fixed
        End If

        intValue = CType(100 - nudWeightPlayedCount.Value - nudWeightSkippedCount.Value - nudWeightDateAdded.Value, Integer)
        If intValue > 0 Then
            nudWeightLastPlayed.Value = intValue ' last played is fixed
        End If

        intValue = CType(100 - nudWeightPlayedCount.Value - nudWeightLastPlayed.Value - nudWeightDateAdded.Value, Integer)
        If intValue > 0 Then
            nudWeightSkippedCount.Value = intValue ' skipped count is fixed
        End If

        intValue = CType(100 - nudWeightSkippedCount.Value - nudWeightLastPlayed.Value - nudWeightDateAdded.Value, Integer)
        If intValue > 0 Then
            nudWeightPlayedCount.Value = intValue ' played count is fixed
        End If

        btnApply.Enabled = True

    End Sub

    Private Sub nudWeightLastPlayed_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nudWeightLastPlayed.ValueChanged

        If mBooGuiIsReady Then sCalcRatio()

    End Sub

    Private Sub nudWeightDateAdded_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nudWeightDateAdded.ValueChanged

        If mBooGuiIsReady Then sCalcRatio()

    End Sub

    Private Sub chkSelectAll_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkScheduleSelectAll.CheckedChanged

        chkMonday.CheckState = chkScheduleSelectAll.CheckState
        chkTuesday.CheckState = chkScheduleSelectAll.CheckState
        chkWednesday.CheckState = chkScheduleSelectAll.CheckState
        chkThursday.CheckState = chkScheduleSelectAll.CheckState
        chkFriday.CheckState = chkScheduleSelectAll.CheckState
        chkSaturday.CheckState = chkScheduleSelectAll.CheckState
        chkSunday.CheckState = chkScheduleSelectAll.CheckState

    End Sub

    Private Sub nudWeightSkippedCount_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nudWeightSkippedCount.ValueChanged

        If mBooGuiIsReady Then sCalcRatio()

    End Sub


    Private Sub frmOptions_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown

        mBooGuiIsReady = True
        btnApply.Enabled = False

    End Sub

    Private Sub nudArtworkWidth_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nudArtworkWidth.ValueChanged
        nudArtworkHeight.Value = nudArtworkWidth.Value
    End Sub

    Private Sub sSettingsReload()

        sSettingsSave()
        sSettingsGet()

        Select Case mpAppMode
            Case eAppMode.ITSFV
                Me.Text = "iTSfv Options"
            Case eAppMode.WMPFV
                Me.Text = "WMPfv Options"
            Case eAppMode.AFV
                Me.Text = "File Validator Options"
        End Select

        My.Forms.frmMain.sSettingsGet()
        btnApply.Enabled = False

    End Sub

    Private Sub btnApply_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnApply.Click

        sSettingsReload()

    End Sub

    Private Sub rbAbAlbumArtistDisc_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbAbAlbumArtistDisc.CheckedChanged

        btnApply.Enabled = True
        mBooDiscsReload = True

    End Sub

    Private Sub rbAbArtistDiscAlbum_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbAbArtistDiscAlbum.CheckedChanged
        mBooDiscsReload = True
    End Sub

    Private Sub chkMusicAudioOnly_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkMusicAudioOnly.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkIncludePodcasts_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkIncludePodcasts.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub nudITScomplianceRate_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nudITScomplianceRate.ValueChanged
        btnApply.Enabled = True
    End Sub

    Private Sub nudArtworkHeight_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nudArtworkHeight.ValueChanged
        btnApply.Enabled = True
    End Sub

    Private Sub cboImArtworkFile_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboImArtworkFileName.SelectedIndexChanged
        btnApply.Enabled = True
    End Sub

    Private Sub cboImArtworkFilenamePattern_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboImArtworkFilenamePatterns.SelectedIndexChanged
        btnApply.Enabled = True
    End Sub

    Private Sub cboExArtworkFile_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboExArtworkFileName.SelectedIndexChanged
        btnApply.Enabled = True
    End Sub

    Private Sub cboExArtworkFilenamePattern_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboExArtworkFilenamePatterns.SelectedIndexChanged
        btnApply.Enabled = True
    End Sub

    Private Sub nudFolderJpgMinSize_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nudFolderJpgMinSize.ValueChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkDisableAlbumArtSmallJpg_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkDisableAlbumArtSmallJpg.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkDisableFolderJpg_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkDisableFolderJpg.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkDisableArtworkJpg_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkDisableArtworkJpg.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkCapitalizeWordNewTrack_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkCapitalizeWordNewTrack.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkRemainingInsteadOfEToC_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkRemainingInsteadOfEToC.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkMinimizeToTray_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkMinimizeToTray.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkCheckUpdates_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkCheckUpdates.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkMonday_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkMonday.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkTuesday_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkTuesday.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkSaturday_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkSaturday.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkWednesday_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkWednesday.CheckedChanged
        btnApply.Enabled = True
    End Sub


    Private Sub chkThursday_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkThursday.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkSunday_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkSunday.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkFriday_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkFriday.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub dtpTime_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtpTime.ValueChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkImportAnyName_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkImportAnyName.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub btnAddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddNew.Click

        Dim dlg As New McoreSystem.FolderBrowser
        dlg.Title = "Browse for alternate Music folder location"
        dlg.Flags = McoreSystem.BrowseFlags.BIF_NEWDIALOGSTYLE Or _
                    McoreSystem.BrowseFlags.BIF_STATUSTEXT Or _
                    McoreSystem.BrowseFlags.BIF_EDITBOX

        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            If dlg.DirectoryPath.Length > 0 Then
                Dim dirPath As String = dlg.DirectoryPath
                If dirPath.EndsWith(Path.DirectorySeparatorChar) = False Then
                    dirPath += Path.DirectorySeparatorChar
                End If
                If Not lbMusicFolders.Items.Contains(dirPath) Then
                    lbMusicFolders.Items.Add(dirPath)
                Else
                    MessageBox.Show(String.Format("{0} already exists in Music folder locations.", dirPath), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                btnApply.Enabled = True
            End If
        End If

    End Sub

    Private Sub btnRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemove.Click

        If lbMusicFolders.SelectedIndex <> -1 Then
            lbMusicFolders.Items.Remove(lbMusicFolders.SelectedItem)
            btnApply.Enabled = True
            btnRemove.Enabled = False
        End If

    End Sub

    Private Sub lbMusicFolders_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lbMusicFolders.MouseDoubleClick

        If lbMusicFolders.SelectedIndex > -1 Then
            If Directory.Exists(lbMusicFolders.SelectedItem.ToString) Then
                Process.Start(lbMusicFolders.SelectedItem.ToString)
            End If
        End If

    End Sub

    Private Sub lbMusicFolders_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbMusicFolders.SelectedIndexChanged

        If lbMusicFolders.SelectedItem IsNot Nothing Then
            btnRemove.Enabled = lbMusicFolders.SelectedIndex <> -1 And lbMusicFolders.SelectedItem.ToString <> txtMusicFolder.Text
        End If

    End Sub

    Private Sub nudPieNumber_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nudPieNumber.ValueChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkArtworkITMS_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkArtworkITMS.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub rbImportArtworkFolderOverride_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbImportArtworkFolderOverride.CheckedChanged
        cboImArtworkFileName.Enabled = Not rbImportArtworkFolderOverride.Checked
        btnApply.Enabled = True
    End Sub

    Private Sub rbExArtworkFolderCustom_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbExArtworkFolderCustom.CheckedChanged
        cboExArtworkFileName.Enabled = Not rbExArtworkFolderCustom.Checked
        btnApply.Enabled = True
    End Sub

    ' Public Function fBrowseFolderDlg(ByVal title As String, ByVal txtBox As TextBox) As Boolean is mFileSystem.vb

    Private Sub btnBrowseLogsPath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowseLogsPath.Click

        Dim oldDirPath As String = txtLogsDir.Text
        If mfGetFolderBrowser("Browse where iTSfv should save Log files...", txtLogsDir) = True Then
            btnApply.Enabled = True
            sAskMoveDir(oldDirPath, txtLogsDir.Text)
        End If

    End Sub

    Private Sub btnBrowseArtworkDir_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowseArtworkDir.Click

        Dim oldDirPath As String = txtArtworkDir.Text
        If mfGetFolderBrowser("Browse where iTSfv should save iTMS Artwork files...", txtArtworkDir) = True Then
            btnApply.Enabled = True
            sAskMoveDir(oldDirPath, txtArtworkDir.Text)
        End If

    End Sub

    Private Sub sAskMoveDir(ByVal oldPath As String, ByVal newPath As String)

        If (oldPath <> newPath) Then
            If MessageBox.Show("Do you want to move the files from the old directory?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then
                btnApply.Enabled = True
                If Directory.Exists(oldPath) Then
                    Try
                        My.Computer.FileSystem.MoveDirectory(oldPath, newPath, True)
                    Catch ex As Exception
                        ' some files are in user ' thanks Jojo
                    End Try
                End If
            End If
        End If

    End Sub

    Private Sub btnBrowseTempDir_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowseTempDir.Click

        If mfGetFolderBrowser("Browse where iTSfv should save Temporary files...", txtTempDir) = True Then
            btnApply.Enabled = True
        End If

    End Sub

    Private Sub chkTagBlackAlbum_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkTagBlankAlbum.CheckedChanged
        rbTagUnknownAlbum.Enabled = chkTagBlankAlbum.Checked
        rbTagTrackName.Enabled = chkTagBlankAlbum.Checked
        cboSingleDiscSuffix.Enabled = chkTagBlankAlbum.Checked
        btnApply.Enabled = True
    End Sub

    Private Sub chkAlwaysHighResLibrary_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkAlwaysHighResLibrary.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkAlwaysHighResSelected_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkAlwaysHighResSelected.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub cboExArtworkFilenamePattern_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboExArtworkFilenamePatterns.TextChanged
        btnApply.Enabled = True
    End Sub

    Private Sub nudTimeoutSeconds_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nudTimeoutSeconds.ValueChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkAlwaysOnTop_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkAlwaysOnTop.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub cboImArtworkFilenamePattern_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboImArtworkFilenamePatterns.TextChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkResizeArtwork_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkResizeArtwork.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkWarnNoTrackNumber_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkWarnNoTrackNumber.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub cboPlaylistType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboPlaylistType.SelectedIndexChanged
        btnApply.Enabled = True
    End Sub

    Private Sub cboExPlaylistFileName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboPlaylistFileNamePattern.SelectedIndexChanged
        btnApply.Enabled = True
    End Sub

    Private Sub cboExPlaylistFileName_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboPlaylistFileNamePattern.TextChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkUseArtworkResize_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkUseArtworkResize.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub cboIndexFileExt_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboIndexFileExt.SelectedIndexChanged
        btnApply.Enabled = True
        gbCSS.Enabled = (cboIndexFileExt.SelectedIndex = 0)
    End Sub

    Private Sub cboIndexFileNamePattern_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboIndexFileNamePattern.SelectedIndexChanged
        btnApply.Enabled = True
    End Sub

    Private Sub cboIndexFileNamePattern_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboIndexFileNamePattern.TextChanged
        btnApply.Enabled = True
    End Sub

    Private Sub btnImportCss_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnImportCss.Click

        Dim dlg As New OpenFileDialog
        dlg.InitialDirectory = My.Settings.SettingsDir
        dlg.Filter = DLG_FILTER_CSS

        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            txtCssFilePath.Text = dlg.FileName
            btnApply.Enabled = True
        End If

    End Sub

    Private Sub btnReportCSS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReportCSS.Click

        Dim dlg As New OpenFileDialog
        dlg.InitialDirectory = My.Settings.SettingsDir
        dlg.Filter = DLG_FILTER_CSS

        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            txtReportCSS.Text = dlg.FileName
            btnApply.Enabled = True
        End If

    End Sub

    Private Sub btnBrowseSettings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowseSettings.Click

        Dim oldDirPath As String = txtSettingsDir.Text
        If mfGetFolderBrowser("Browse where iTSfv should save Settings files...", txtSettingsDir) = True Then
            btnApply.Enabled = True
            sAskMoveDir(oldDirPath, txtSettingsDir.Text)
        End If

    End Sub

    Private Sub cboExArtworkFileName_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboExArtworkFileName.TextChanged
        btnApply.Enabled = True
    End Sub

    Private Sub chkFileSystemWatcher_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkFileSystemWatcher.CheckedChanged

        ' File System Watcher is disabled by default
        ' When the user enabled it for the first time iTSfv checks if 
        ' the Music folder path exists and if not then iTSfv will prompt for music folder path 
        ' Location is then saved in My.Settings 

        If chkFileSystemWatcher.Checked = True Then
            If Directory.Exists(My.Settings.MusicFolderPath) = False Then
                mfGetFolderBrowser("Browse for iTunes Music folder location (as configured in iTunes Options)...", txtMusicFolder)
                My.Settings.MusicFolderPath = txtMusicFolder.Text
            End If
        End If

        btnApply.Enabled = True

    End Sub


    Private Sub rbExLyricsFolderDefault_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbExLyricsFolderDefault.CheckedChanged

        txtLyricsFolderEx.Enabled = Not rbExLyricsFolderDefault.Checked
        btnBrowseLyricsFolderEx.Enabled = Not rbExLyricsFolderDefault.Checked

        btnApply.Enabled = True

    End Sub

    Private Sub btnBrowseLyricsFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowseLyricsFolderEx.Click

        If mfGetFolderBrowser("Browse for a folder location to export Lyrics...", txtLyricsFolderEx) = True Then
            btnApply.Enabled = True
        End If

    End Sub

    Private Sub rbImportLyricsLyricWiki_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkImportLyricsLyricWiki.CheckedChanged

    End Sub

    Private Sub rbLyricsImportDirDefault_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbLyricsImportDirDefault.CheckedChanged

        txtLyricsFolderIm.Enabled = Not rbLyricsImportDirDefault.Checked
        btnBrowseLyricsDirIm.Enabled = Not rbLyricsImportDirDefault.Checked

        btnApply.Enabled = True

    End Sub



    Private Sub rbTagUnknownAlbum_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbTagUnknownAlbum.CheckedChanged
        cboSingleDiscSuffix.Enabled = Not rbTagUnknownAlbum.Checked
    End Sub

    Private Sub chkStartWithWindows_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkStartWithWindows.CheckedChanged
        btnApply.Enabled = True
    End Sub

    Private Sub btnConfigRestore_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConfigRestore.Click

        Dim dlg As New OpenFileDialog
        dlg.Filter = "User Configuration Files (user.config)|user.config"
        dlg.Title = "Browse for the user.config backup and press Open to Restore..."
        dlg.InitialDirectory = My.Settings.SettingsDir
        dlg.FileName = mFilePathConfigBackup
        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            mfConfigRestore(dlg.FileName)
        End If

    End Sub

    Private Sub chkEmailAuto_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkEmailAuto.CheckedChanged
        txtSMTPHost.Enabled = chkEmailAuto.Checked
        nudSMTPPort.Enabled = chkEmailAuto.Checked
        txtEmailAddress.Enabled = chkEmailAuto.Checked
    End Sub

    Private Sub btnConfigOrig_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConfigOrig.Click
        Process.Start(Path.GetDirectoryName(mfConfigFilePath))
    End Sub

    Private Sub lblMusicFolderStructure_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkMusicFolderStructure.CheckedChanged
        cboMusicFolderStructure.Enabled = chkMusicFolderStructure.Checked
    End Sub

    Private Sub cboPatternFromFileEx_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboPatternFromFileEx.CheckedChanged

        cboLyricsFileNamePatternEx.Enabled = rbLyricsPatternCustomEx.Checked

    End Sub

    Private Sub rbLyricsPatternFromFileIm_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbLyricsPatternFromFileIm.CheckedChanged

        cboLyricsFilenamePatternIm.Enabled = rbLyricsPatternCustomIm.Checked

    End Sub

    Private Sub cboAppMode_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If mBooGuiIsReady Then
            sSettingsReload()
            'btnApply.Enabled = True
        End If
    End Sub

    Private Sub lblMediaCenterNotes_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblMediaCenterNotes.Click

    End Sub

    Private Sub chkRecoverRating_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkRecoverRating.CheckedChanged

    End Sub

    Private Sub chkRecoverBookmarkTime_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkRecoverBookmarkTime.CheckedChanged

    End Sub

    Private Sub rbAppModeITsfv_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbAppModeITsfv.CheckedChanged
        'btnRestart.Visible = mBooGuiIsReady
        lblRestartApp.Visible = mBooGuiIsReady
    End Sub

    Private Sub rbAppModeWMPfv_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbAppModeWMPfv.CheckedChanged
        'btnRestart.Visible = mBooGuiIsReady
        lblRestartApp.Visible = mBooGuiIsReady
    End Sub

    Private Sub rbAppModeWEafv_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbAppModeWEafv.CheckedChanged
        'btnRestart.Visible = mBooGuiIsReady
        lblRestartApp.Visible = mBooGuiIsReady
    End Sub

    Private Sub btnRestart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRestart.Click
        sSettingsSave()
        Application.Restart()
    End Sub

    Private Sub btnBrowseLyricsDirIm_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowseLyricsDirIm.Click

        If mfGetFolderBrowser("Browse for a folder location to import Lyrics...", txtLyricsFolderIm) = True Then
            btnApply.Enabled = True
        End If

    End Sub

    Private Sub chkCopyFilesExternal_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkCopyFilesExternal.CheckedChanged
        chkCopyFilesExternal.CheckState = CheckState.Indeterminate
    End Sub

    Private Sub chkWarnSelectAll_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkWarnSelectAll.CheckedChanged

        For Each ctl As Control In tpWarnings.Controls

            If TypeOf ctl Is CheckBox Then
                If CType(ctl, CheckBox).Name <> chkWarnSelectAll.Name Then
                    CType(ctl, CheckBox).Checked = chkWarnSelectAll.Checked
                End If
            End If

        Next

    End Sub

    Private Sub chkMostCommonArtistPerc_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkMostCommonArtistPerc.CheckedChanged
        nudMostCommonArtist.Enabled = chkMostCommonArtistPerc.Checked And chkMostCommonArtistPerc.Enabled
    End Sub

    Private Sub chkMostCommonAlbumArtist_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkMostCommonAlbumArtist.CheckedChanged
        chkMostCommonArtistPerc.Enabled = chkMostCommonAlbumArtist.Checked
    End Sub

    Private Sub lblRemoveDuplicates_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblRemoveDuplicates.CheckedChanged
        lblRemoveDuplicates.CheckState = CheckState.Indeterminate
    End Sub

    Private Sub lblFolderJpg_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblFolderJpg.CheckedChanged
        lblFolderJpg.CheckState = CheckState.Indeterminate
    End Sub

    Private Sub btnBrowseLossless_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowseLossless.Click

        If mfGetFolderBrowser("Browse for the Lossless Audio folder...", txtFolderLossless) = True Then
            btnApply.Enabled = True
        End If

    End Sub

    Private Sub btnBrowseAAD_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowseAAD.Click

        Dim dlg As New OpenFileDialog
        dlg.Title = "Browse for Album Art Downloader XUI location..."
        dlg.Filter = "Executable Files (*.exe)|*.exe"
        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            txtPathAAD.Text = dlg.FileName
        End If

    End Sub

    Private Sub btnBrowseMp3tag_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowseMp3tag.Click

        Dim dlg As New OpenFileDialog
        dlg.Title = "Browse for Mp3tag location..."
        dlg.Filter = "Executable Files (*.exe)|*.exe"
        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            txtPathMp3tag.Text = dlg.FileName
        End If

    End Sub

    Private Sub chkArtworkChooseManual_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkArtworkChooseManual.CheckedChanged
        chkTrackArtworkShowAll.Enabled = chkArtworkChooseManual.Checked    
    End Sub


    Private Sub btnAddExclude_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddExclude.Click

        Dim dlg As New McoreSystem.FolderBrowser
        dlg.Title = "Browse a folder to Exclude from Music folders..."
        dlg.Flags = McoreSystem.BrowseFlags.BIF_NEWDIALOGSTYLE Or _
                    McoreSystem.BrowseFlags.BIF_STATUSTEXT Or _
                    McoreSystem.BrowseFlags.BIF_EDITBOX

        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            If dlg.DirectoryPath.Length > 0 Then
                Dim dirPath As String = dlg.DirectoryPath
                If dirPath.EndsWith(Path.DirectorySeparatorChar) = False Then
                    dirPath += Path.DirectorySeparatorChar
                End If
                If Not lbExcludeFolders.Items.Contains(dirPath) Then
                    If Not lbMusicFolders.Items.Contains(dirPath) Then
                        lbExcludeFolders.Items.Add(dirPath)
                    Else
                        MessageBox.Show(String.Format("{0} already exists in Music folder locations.", dirPath), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                Else
                    MessageBox.Show(String.Format("{0} already exists in Exclude Music folder locations.", dirPath), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                btnApply.Enabled = True
            End If
        End If

    End Sub

    Private Sub btnRemoveExclude_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemoveExclude.Click
        If lbExcludeFolders.SelectedIndex <> -1 Then
            lbExcludeFolders.Items.Remove(lbExcludeFolders.SelectedItem)
            btnApply.Enabled = True
            btnRemove.Enabled = False
        End If
    End Sub

    Private Sub btnResetSettings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnResetSettings.Click

        If MessageBox.Show("Are you sure you want to reset all the settings?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
            My.Settings.Reset()
            sSettingsGet()
        End If

    End Sub

    Private Sub sButtonBrowseAADCLI()

        Dim dlg As New OpenFileDialog
        dlg.Title = "Browse for Album Art Downloader XUI Console location..."
        dlg.Filter = "Executable Files (*.exe)|*.exe"
        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            txtPathAADConsole.Text = dlg.FileName
        End If

    End Sub


    Private Sub btnAADConsole_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAADConsole.Click

        sButtonBrowseAADCLI()

    End Sub

    Private Sub chkArtworkfromAADCLI_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkArtworkfromAADCLI.CheckedChanged

        If mBooGuiIsReady Then
            If chkArtworkfromAADCLI.Checked Then
                If File.Exists(My.Settings.ExePathAADConsole) = False Then
                    sButtonBrowseAADCLI()
                End If
            End If
        End If

    End Sub

    Private Sub chkRatingTrackDuration_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkRatingTrackDuration.CheckedChanged
        nudScaleDuration.Enabled = chkRatingTrackDuration.Checked AndAlso chkScaleLongTracks.Checked
        chkScaleLongTracks.Enabled = chkRatingTrackDuration.Checked
        lblLongTrackDuration.Enabled = chkRatingTrackDuration.Checked
        nudLongSongLength.Enabled = chkRatingTrackDuration.Checked
    End Sub

    Private Sub lblLongTrackDuration_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblLongTrackDuration.CheckedChanged
        lblLongTrackDuration.CheckState = CheckState.Indeterminate
    End Sub

    Private Sub chkScaleLongTracks_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkScaleLongTracks.CheckedChanged
        nudScaleDuration.Enabled = chkScaleLongTracks.Checked
        lblLongTrackDuration.Enabled = chkScaleLongTracks.Checked
    End Sub

    Private Sub chkPrevRating_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkPrevRating.CheckedChanged
        chkPrevRating.CheckState = CheckState.Indeterminate
    End Sub

    Private Sub btnHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHelp.Click
        Dim h As New dlgHelp(mfGetText("TagsSupported.txt"))
        h.ShowDialog()
    End Sub
End Class
