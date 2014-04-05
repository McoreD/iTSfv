

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.btnValidateLibrary = New System.Windows.Forms.Button()
        Me.chkEditCopyArtistToAlbumArtist = New System.Windows.Forms.CheckBox()
        Me.ssAppTrack = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel2 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.sBarTrack = New System.Windows.Forms.ToolStripStatusLabel()
        Me.sTrackProgress = New System.Windows.Forms.ToolStripStatusLabel()
        Me.pbarTrack = New System.Windows.Forms.ToolStripProgressBar()
        Me.chkDeleteTracksNotInHDD = New System.Windows.Forms.CheckBox()
        Me.cmsApp = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.tsmShowApp = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator18 = New System.Windows.Forms.ToolStripSeparator()
        Me.OpenTracksReportToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenMusicFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LogFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenAlbumsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenNoniTSStandardTrackListwToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TracksThatArtworkWasAddedToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TracksThatRatingWasAdjustedToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TrackCountUpdatedTracksToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TracksWithLowResolutionArtworkToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TracksWithMultipleArtworkToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TracksWithNoArtworkToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TracksWithNoLyricsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MusicFolderActivityToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.SToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenWarningsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TaskToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AdjustRatingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopyTrackInfoToClipboardToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ValidateSelectedTracksToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ValidateToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.QuickValidationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.VersionHistoryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CheckForUpdatesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.SendToSystemTrayToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.chkWinExportArtwork = New System.Windows.Forms.CheckBox()
        Me.chkResume = New System.Windows.Forms.CheckBox()
        Me.tmrFormClose = New System.Windows.Forms.Timer(Me.components)
        Me.tcTabs = New System.Windows.Forms.TabControl()
        Me.tpSettings = New System.Windows.Forms.TabPage()
        Me.tcValidate = New System.Windows.Forms.TabControl()
        Me.tpChecks = New System.Windows.Forms.TabPage()
        Me.btnValidateTracksChecks = New System.Windows.Forms.Button()
        Me.chkCheckMetatag = New System.Windows.Forms.CheckBox()
        Me.chkCheckBPM = New System.Windows.Forms.CheckBox()
        Me.chkCheckFoldersWithoutArtwork = New System.Windows.Forms.CheckBox()
        Me.chkCheckEmbeddedArtwork = New System.Windows.Forms.CheckBox()
        Me.chkCheckArtworkLowRes = New System.Windows.Forms.CheckBox()
        Me.chkCheckLyrics = New System.Windows.Forms.CheckBox()
        Me.chkItunesStoreStandard = New System.Windows.Forms.CheckBox()
        Me.chkCheckTrackNum = New System.Windows.Forms.CheckBox()
        Me.chkCheckArtwork = New System.Windows.Forms.CheckBox()
        Me.tpEditTracks = New System.Windows.Forms.TabPage()
        Me.chkEditCopyAlbumArtistToSortArtist = New System.Windows.Forms.CheckBox()
        Me.chkConvertArtworkJPG = New System.Windows.Forms.CheckBox()
        Me.btnValidateSelectedTracks = New System.Windows.Forms.Button()
        Me.chkMultiArtworkRemove = New System.Windows.Forms.CheckBox()
        Me.chkImportArtwork = New System.Windows.Forms.CheckBox()
        Me.chkEditTrackCountEtc = New System.Windows.Forms.CheckBox()
        Me.chkEditEQbyGenre = New System.Windows.Forms.CheckBox()
        Me.chkWriteGenre = New System.Windows.Forms.CheckBox()
        Me.chkRemoveLowResArtwork = New System.Windows.Forms.CheckBox()
        Me.chkUpdateInfoFromFile = New System.Windows.Forms.CheckBox()
        Me.chkImportLyrics = New System.Windows.Forms.CheckBox()
        Me.chkRemoveNull = New System.Windows.Forms.CheckBox()
        Me.tpEditLibrary = New System.Windows.Forms.TabPage()
        Me.chkPlayedCountImportPCNT = New System.Windows.Forms.CheckBox()
        Me.chkRatingsImportPOPM = New System.Windows.Forms.CheckBox()
        Me.btnValidateSelectedTracksLibrary = New System.Windows.Forms.Button()
        Me.chkDeleteNonMusicFolderTracks = New System.Windows.Forms.CheckBox()
        Me.chkLibraryAdjustRatings = New System.Windows.Forms.CheckBox()
        Me.chkValidationPlaylists = New System.Windows.Forms.CheckBox()
        Me.tpFileSystem = New System.Windows.Forms.TabPage()
        Me.btnValidateSelectedTracksFolder = New System.Windows.Forms.Button()
        Me.chkWinExportPlaylist = New System.Windows.Forms.CheckBox()
        Me.chkWinMakeReadOnly = New System.Windows.Forms.CheckBox()
        Me.chkExportLyrics = New System.Windows.Forms.CheckBox()
        Me.chkExportIndex = New System.Windows.Forms.CheckBox()
        Me.chkVistaThumbnailFix = New System.Windows.Forms.CheckBox()
        Me.ilTabs = New System.Windows.Forms.ImageList(Me.components)
        Me.tpSelectedTracks = New System.Windows.Forms.TabPage()
        Me.tcSelectedTracks = New System.Windows.Forms.TabControl()
        Me.tpEditor = New System.Windows.Forms.TabPage()
        Me.chkRemoveComments = New System.Windows.Forms.CheckBox()
        Me.chkRemoveLyrics = New System.Windows.Forms.CheckBox()
        Me.txtAppend = New System.Windows.Forms.TextBox()
        Me.cboAppendChar = New System.Windows.Forms.ComboBox()
        Me.chkAppendChar = New System.Windows.Forms.CheckBox()
        Me.cboReplace = New System.Windows.Forms.ComboBox()
        Me.cboFind = New System.Windows.Forms.ComboBox()
        Me.cboTrimDirection = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.nudTrimChar = New System.Windows.Forms.NumericUpDown()
        Me.chkTrimChar = New System.Windows.Forms.CheckBox()
        Me.chkTagRemove = New System.Windows.Forms.CheckBox()
        Me.chkDecompile = New System.Windows.Forms.CheckBox()
        Me.chkReplaceTextInTags = New System.Windows.Forms.CheckBox()
        Me.chkCapitalizeFirstLetter = New System.Windows.Forms.CheckBox()
        Me.chkStrict = New System.Windows.Forms.CheckBox()
        Me.gbWriteTags = New System.Windows.Forms.GroupBox()
        Me.chkGenre = New System.Windows.Forms.CheckBox()
        Me.chkAlbumArtist = New System.Windows.Forms.CheckBox()
        Me.chkArtist = New System.Windows.Forms.CheckBox()
        Me.chkName = New System.Windows.Forms.CheckBox()
        Me.chkAlbum = New System.Windows.Forms.CheckBox()
        Me.cboArtistsDecompiled = New System.Windows.Forms.ComboBox()
        Me.chkRenameFile = New System.Windows.Forms.CheckBox()
        Me.cboDecompileOptions = New System.Windows.Forms.ComboBox()
        Me.lblWith = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.tpSTClipboard = New System.Windows.Forms.TabPage()
        Me.chkClipboardSort = New System.Windows.Forms.CheckBox()
        Me.gbClipBoardTags = New System.Windows.Forms.GroupBox()
        Me.lblClipboard = New System.Windows.Forms.TextBox()
        Me.cboClipboardPattern = New System.Windows.Forms.ComboBox()
        Me.btnClipboard = New System.Windows.Forms.Button()
        Me.tpSTCheat = New System.Windows.Forms.TabPage()
        Me.gbOffset = New System.Windows.Forms.GroupBox()
        Me.nudOffsetTrackNum = New System.Windows.Forms.NumericUpDown()
        Me.btnOffsetTrackNum = New System.Windows.Forms.Button()
        Me.gbOverride = New System.Windows.Forms.GroupBox()
        Me.nudRatingOverride = New System.Windows.Forms.NumericUpDown()
        Me.chkRatingOverride = New System.Windows.Forms.CheckBox()
        Me.nudPlayedCountOverride = New System.Windows.Forms.NumericUpDown()
        Me.chkPlayedCountOverride = New System.Windows.Forms.CheckBox()
        Me.btnOverride = New System.Windows.Forms.Button()
        Me.dtpPlayedDate = New System.Windows.Forms.DateTimePicker()
        Me.chkPlayedDateOverride = New System.Windows.Forms.CheckBox()
        Me.tpSTExport = New System.Windows.Forms.TabPage()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnArtworkExport = New System.Windows.Forms.Button()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.chkExportArtwork = New System.Windows.Forms.CheckBox()
        Me.cboExportFilePattern = New System.Windows.Forms.ComboBox()
        Me.btnCopyTo = New System.Windows.Forms.Button()
        Me.tpExplorer = New System.Windows.Forms.TabPage()
        Me.tcExplorer = New System.Windows.Forms.TabControl()
        Me.tpExplorerFiles = New System.Windows.Forms.TabPage()
        Me.chkReplaceWithNewKind = New System.Windows.Forms.CheckBox()
        Me.chkValidate = New System.Windows.Forms.CheckBox()
        Me.lbFiles = New System.Windows.Forms.ListBox()
        Me.cmsFiles = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ShowInWindowsExplorerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RemoveFromListToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.chkAddFile = New System.Windows.Forms.CheckBox()
        Me.btnFindNewFiles = New System.Windows.Forms.Button()
        Me.btnClearFilesListBox = New System.Windows.Forms.Button()
        Me.tpExplorerActivity = New System.Windows.Forms.TabPage()
        Me.txtActivity = New System.Windows.Forms.TextBox()
        Me.tpDiscsBrowser = New System.Windows.Forms.TabPage()
        Me.btnBrowseAlbum = New System.Windows.Forms.Button()
        Me.btnValidateAlbum = New System.Windows.Forms.Button()
        Me.btnCreatePlaylistAlbum = New System.Windows.Forms.Button()
        Me.lbDiscs = New System.Windows.Forms.ListBox()
        Me.cmsDiscs = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.PlayDiscInITunesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ValidateDiscToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator16 = New System.Windows.Forms.ToolStripSeparator()
        Me.CreatePlaylistToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowDiscInWindowsExplroerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmCopyTracklist = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator24 = New System.Windows.Forms.ToolStripSeparator()
        Me.ArtworkSearchToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GoogleSearchToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Mp3tagSelectedDisc = New System.Windows.Forms.ToolStripMenuItem()
        Me.pbArtwork = New System.Windows.Forms.PictureBox()
        Me.tpBackupRestore = New System.Windows.Forms.TabPage()
        Me.tcTags = New System.Windows.Forms.TabControl()
        Me.tpTagsBackupRestore = New System.Windows.Forms.TabPage()
        Me.gbBackupTags = New System.Windows.Forms.GroupBox()
        Me.rbSelectedTracks = New System.Windows.Forms.RadioButton()
        Me.btnRatingsBackup = New System.Windows.Forms.Button()
        Me.rbLibrary = New System.Windows.Forms.RadioButton()
        Me.txtRatingsBackupPath = New System.Windows.Forms.TextBox()
        Me.gbRestoreTags = New System.Windows.Forms.GroupBox()
        Me.txtRatingsRestorePath = New System.Windows.Forms.TextBox()
        Me.btnRatingsRestore = New System.Windows.Forms.Button()
        Me.tpTagsRecover = New System.Windows.Forms.TabPage()
        Me.gbBrowsePrevLib = New System.Windows.Forms.GroupBox()
        Me.txtXmlLibPath = New System.Windows.Forms.TextBox()
        Me.btnRecover = New System.Windows.Forms.Button()
        Me.tpOneTouch = New System.Windows.Forms.TabPage()
        Me.tcOneTouch = New System.Windows.Forms.TabControl()
        Me.tpAdvGeneral = New System.Windows.Forms.TabPage()
        Me.btnSynchroclean = New System.Windows.Forms.Button()
        Me.tpAdvTracks = New System.Windows.Forms.TabPage()
        Me.btnWritePOPM = New System.Windows.Forms.Button()
        Me.btnReplaceTracks = New System.Windows.Forms.Button()
        Me.tpAdvLibrary = New System.Windows.Forms.TabPage()
        Me.btnImportPOPM = New System.Windows.Forms.Button()
        Me.btnAdjustRatings = New System.Windows.Forms.Button()
        Me.btnRemoveDuplicates = New System.Windows.Forms.Button()
        Me.btnSyncLastFM = New System.Windows.Forms.Button()
        Me.tpAdvFilesystem = New System.Windows.Forms.TabPage()
        Me.btnBatchArtworkGrab = New System.Windows.Forms.Button()
        Me.btnAdvDeleteEmptyFolders = New System.Windows.Forms.Button()
        Me.tpSchedule = New System.Windows.Forms.TabPage()
        Me.btnSchRun = New System.Windows.Forms.Button()
        Me.chkScheduleFindNewFilesHDD = New System.Windows.Forms.CheckBox()
        Me.chkSheduleAdjustRating = New System.Windows.Forms.CheckBox()
        Me.chkSchValidateLibrary = New System.Windows.Forms.CheckBox()
        Me.btnValidateSelected = New System.Windows.Forms.Button()
        Me.bwApp = New System.ComponentModel.BackgroundWorker()
        Me.ttApp = New System.Windows.Forms.ToolTip(Me.components)
        Me.tmrSecond = New System.Windows.Forms.Timer(Me.components)
        Me.niTray = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.msApp = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmAddFolderToLib = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveStatisticsFileAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CreatePlaylistOfSelectedTracksToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator()
        Me.miSendToTray = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExitToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.miJobs = New System.Windows.Forms.ToolStripMenuItem()
        Me.miAdjustRatings = New System.Windows.Forms.ToolStripMenuItem()
        Me.miStatistics = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmUpdatePlayedCount = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator17 = New System.Windows.Forms.ToolStripSeparator()
        Me.DeleteDeadOrForeignTracksToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AddNewFilesNotInITunesMusicLibraryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.miSynchroclean = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
        Me.PlayFirstTrackToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.miValidateSelected = New System.Windows.Forms.ToolStripMenuItem()
        Me.ValidateLast100TracksToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ValidateITunesMusicLibraryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator26 = New System.Windows.Forms.ToolStripSeparator()
        Me.VerboseModeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LogsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenTracksReportToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator14 = New System.Windows.Forms.ToolStripSeparator()
        Me.ArtworkToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TracksThatArtworkWasAddedToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.TracksWithmultipleArtworkToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.TracksWithoutArtworkToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TracksThatArtworkIsLowResolutionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiArtworkConverted = New System.Windows.Forms.ToolStripMenuItem()
        Me.LyricsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TracksWithoutLyricsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TracksLyricsWereAddedToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmTrackTags = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmTrackTagsBPM = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmTrackTagsRefreshed = New System.Windows.Forms.ToolStripMenuItem()
        Me.TrackMetatagVersionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator11 = New System.Windows.Forms.ToolStripSeparator()
        Me.TracksThatRatingWasAdjustedToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.TracksNotITunesStoreCompliantToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TracksThatTrackCountWasUpdatedToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator12 = New System.Windows.Forms.ToolStripSeparator()
        Me.LibraryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DuplicateTracksToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FileSystemToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FoldersWithOneFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FoldersWithoutArtworkToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FoldersWithoutAudioToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator25 = New System.Windows.Forms.ToolStripSeparator()
        Me.tsmMusicFolderActivity = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator22 = New System.Windows.Forms.ToolStripSeparator()
        Me.AlbumsWithInconsecutiveTrackNumbersToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator13 = New System.Windows.Forms.ToolStripSeparator()
        Me.DebugToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ErrorsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.WarningsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator20 = New System.Windows.Forms.ToolStripSeparator()
        Me.BrowseLogsFolderToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.SettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.IgnoreWordsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CapitalWordsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SimpleWordsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ReplaceWordsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SkipWordsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator19 = New System.Windows.Forms.ToolStripSeparator()
        Me.BrowseSettingsFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FoldersToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BrowseMusicFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator15 = New System.Windows.Forms.ToolStripSeparator()
        Me.BrowseITMSArtworksFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BrowseLogsFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SettingsFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BrowseTemporaryFiToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SelectedTracksToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SavePlaylistOfSelectedTracksAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiSelectedTracksValidate = New System.Windows.Forms.ToolStripMenuItem()
        Me.CheckToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditTracksToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LibraryToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmSelectedTracksValidateFS = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator23 = New System.Windows.Forms.ToolStripSeparator()
        Me.CopyInfoToClipboardToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Mp3tagSelectedTracks = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmTiunesStoreArtworkGrabSelected = New System.Windows.Forms.ToolStripMenuItem()
        Me.SearchArtworkToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmiSearch = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AlbumArtDownloaderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmItunesArtworkGrabber = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmLyricsViewer = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmSetInfo = New System.Windows.Forms.ToolStripMenuItem()
        Me.Mp3tagToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FileValidatorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TrackReplaceAssistantToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator10 = New System.Windows.Forms.ToolStripSeparator()
        Me.AlwaysOnTopToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.miToolsOptions = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.miManual = New System.Windows.Forms.ToolStripMenuItem()
        Me.SupportForumsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GoogleGroupsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HydrogenAudioToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ILoungeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SourceForgeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator()
        Me.BetaVersionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SVNRepositoryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator27 = New System.Windows.Forms.ToolStripSeparator()
        Me.DiggToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OhlohToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.WakoopaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ProjectHomeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.VersionHistoryToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator9 = New System.Windows.Forms.ToolStripSeparator()
        Me.tsmiDonate = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator21 = New System.Windows.Forms.ToolStripSeparator()
        Me.SubmitDebugReportToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CheckForUpdatesToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.miAbout = New System.Windows.Forms.ToolStripMenuItem()
        Me.bwDiscsBrowserInfo = New System.ComponentModel.BackgroundWorker()
        Me.bwFS = New System.ComponentModel.BackgroundWorker()
        Me.tmrAddMusicAuto = New System.Windows.Forms.Timer(Me.components)
        Me.chkStartFinish = New System.Windows.Forms.CheckBox()
        Me.chkDiscNumber = New System.Windows.Forms.CheckBox()
        Me.bwWatcher = New System.ComponentModel.BackgroundWorker()
        Me.chkDiscComplete = New System.Windows.Forms.CheckBox()
        Me.ssAppDisc = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.sBarDisc = New System.Windows.Forms.ToolStripStatusLabel()
        Me.pBarDiscs = New System.Windows.Forms.ToolStripProgressBar()
        Me.bwTimers = New System.ComponentModel.BackgroundWorker()
        Me.lbVerbose = New System.Windows.Forms.ListBox()
        Me.bwQueueFiles = New System.ComponentModel.BackgroundWorker()
        Me.tlpMain = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.btnStatistics = New System.Windows.Forms.Button()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.ssAppTrack.SuspendLayout()
        Me.cmsApp.SuspendLayout()
        Me.tcTabs.SuspendLayout()
        Me.tpSettings.SuspendLayout()
        Me.tcValidate.SuspendLayout()
        Me.tpChecks.SuspendLayout()
        Me.tpEditTracks.SuspendLayout()
        Me.tpEditLibrary.SuspendLayout()
        Me.tpFileSystem.SuspendLayout()
        Me.tpSelectedTracks.SuspendLayout()
        Me.tcSelectedTracks.SuspendLayout()
        Me.tpEditor.SuspendLayout()
        CType(Me.nudTrimChar, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gbWriteTags.SuspendLayout()
        Me.tpSTClipboard.SuspendLayout()
        Me.gbClipBoardTags.SuspendLayout()
        Me.tpSTCheat.SuspendLayout()
        Me.gbOffset.SuspendLayout()
        CType(Me.nudOffsetTrackNum, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gbOverride.SuspendLayout()
        CType(Me.nudRatingOverride, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudPlayedCountOverride, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpSTExport.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.tpExplorer.SuspendLayout()
        Me.tcExplorer.SuspendLayout()
        Me.tpExplorerFiles.SuspendLayout()
        Me.cmsFiles.SuspendLayout()
        Me.tpExplorerActivity.SuspendLayout()
        Me.tpDiscsBrowser.SuspendLayout()
        Me.cmsDiscs.SuspendLayout()
        CType(Me.pbArtwork, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpBackupRestore.SuspendLayout()
        Me.tcTags.SuspendLayout()
        Me.tpTagsBackupRestore.SuspendLayout()
        Me.gbBackupTags.SuspendLayout()
        Me.gbRestoreTags.SuspendLayout()
        Me.tpTagsRecover.SuspendLayout()
        Me.gbBrowsePrevLib.SuspendLayout()
        Me.tpOneTouch.SuspendLayout()
        Me.tcOneTouch.SuspendLayout()
        Me.tpAdvGeneral.SuspendLayout()
        Me.tpAdvTracks.SuspendLayout()
        Me.tpAdvLibrary.SuspendLayout()
        Me.tpAdvFilesystem.SuspendLayout()
        Me.tpSchedule.SuspendLayout()
        Me.msApp.SuspendLayout()
        Me.ssAppDisc.SuspendLayout()
        Me.tlpMain.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnValidateLibrary
        '
        Me.btnValidateLibrary.AutoSize = True
        Me.btnValidateLibrary.Cursor = System.Windows.Forms.Cursors.Default
        Me.btnValidateLibrary.Enabled = False
        Me.btnValidateLibrary.Location = New System.Drawing.Point(10, 37)
        Me.btnValidateLibrary.Name = "btnValidateLibrary"
        Me.btnValidateLibrary.Size = New System.Drawing.Size(330, 23)
        Me.btnValidateLibrary.TabIndex = 1
        Me.btnValidateLibrary.Text = "&Validate iTunes Music Library"
        Me.btnValidateLibrary.UseVisualStyleBackColor = True
        '
        'chkEditCopyArtistToAlbumArtist
        '
        Me.chkEditCopyArtistToAlbumArtist.AutoSize = True
        Me.chkEditCopyArtistToAlbumArtist.Checked = True
        Me.chkEditCopyArtistToAlbumArtist.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkEditCopyArtistToAlbumArtist.Location = New System.Drawing.Point(8, 8)
        Me.chkEditCopyArtistToAlbumArtist.Name = "chkEditCopyArtistToAlbumArtist"
        Me.chkEditCopyArtistToAlbumArtist.Size = New System.Drawing.Size(271, 17)
        Me.chkEditCopyArtistToAlbumArtist.TabIndex = 2
        Me.chkEditCopyArtistToAlbumArtist.Text = "Fill missing AlbumArtist and Sort Artist using Artist tag"
        Me.chkEditCopyArtistToAlbumArtist.UseVisualStyleBackColor = True
        '
        'ssAppTrack
        '
        Me.ssAppTrack.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel2, Me.sBarTrack, Me.sTrackProgress, Me.pbarTrack})
        Me.ssAppTrack.Location = New System.Drawing.Point(0, 540)
        Me.ssAppTrack.Name = "ssAppTrack"
        Me.ssAppTrack.ShowItemToolTips = True
        Me.ssAppTrack.Size = New System.Drawing.Size(784, 22)
        Me.ssAppTrack.TabIndex = 4
        Me.ssAppTrack.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel2
        '
        Me.ToolStripStatusLabel2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripStatusLabel2.Image = Global.iTSfv.My.Resources.Resources.info
        Me.ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        Me.ToolStripStatusLabel2.Size = New System.Drawing.Size(16, 17)
        Me.ToolStripStatusLabel2.Text = "ToolStripStatusLabel2"
        '
        'sBarTrack
        '
        Me.sBarTrack.AutoSize = False
        Me.sBarTrack.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.sBarTrack.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.sBarTrack.Name = "sBarTrack"
        Me.sBarTrack.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.sBarTrack.Size = New System.Drawing.Size(617, 17)
        Me.sBarTrack.Spring = True
        Me.sBarTrack.Text = "Idle."
        Me.sBarTrack.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'sTrackProgress
        '
        Me.sTrackProgress.BackColor = System.Drawing.SystemColors.Info
        Me.sTrackProgress.Name = "sTrackProgress"
        Me.sTrackProgress.Size = New System.Drawing.Size(34, 17)
        Me.sTrackProgress.Text = "0.0%"
        '
        'pbarTrack
        '
        Me.pbarTrack.Name = "pbarTrack"
        Me.pbarTrack.Size = New System.Drawing.Size(100, 16)
        '
        'chkDeleteTracksNotInHDD
        '
        Me.chkDeleteTracksNotInHDD.AutoSize = True
        Me.chkDeleteTracksNotInHDD.Location = New System.Drawing.Point(10, 10)
        Me.chkDeleteTracksNotInHDD.Name = "chkDeleteTracksNotInHDD"
        Me.chkDeleteTracksNotInHDD.Size = New System.Drawing.Size(167, 17)
        Me.chkDeleteTracksNotInHDD.TabIndex = 5
        Me.chkDeleteTracksNotInHDD.Text = "Delete tracks that do not exist"
        Me.chkDeleteTracksNotInHDD.UseVisualStyleBackColor = True
        '
        'cmsApp
        '
        Me.cmsApp.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmShowApp, Me.ToolStripSeparator18, Me.OpenTracksReportToolStripMenuItem, Me.OpenMusicFolderToolStripMenuItem, Me.LogFilesToolStripMenuItem, Me.TaskToolStripMenuItem, Me.ValidateSelectedTracksToolStripMenuItem, Me.QuickValidationToolStripMenuItem, Me.ToolStripSeparator1, Me.AboutToolStripMenuItem, Me.OptionsToolStripMenuItem, Me.VersionHistoryToolStripMenuItem, Me.CheckForUpdatesToolStripMenuItem, Me.ToolStripSeparator3, Me.SendToSystemTrayToolStripMenuItem, Me.ExitToolStripMenuItem})
        Me.cmsApp.Name = "cMenu"
        Me.cmsApp.Size = New System.Drawing.Size(215, 308)
        '
        'tsmShowApp
        '
        Me.tsmShowApp.Enabled = False
        Me.tsmShowApp.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.tsmShowApp.Name = "tsmShowApp"
        Me.tsmShowApp.Size = New System.Drawing.Size(214, 22)
        Me.tsmShowApp.Text = "&Show iTSfv..."
        '
        'ToolStripSeparator18
        '
        Me.ToolStripSeparator18.Name = "ToolStripSeparator18"
        Me.ToolStripSeparator18.Size = New System.Drawing.Size(211, 6)
        '
        'OpenTracksReportToolStripMenuItem
        '
        Me.OpenTracksReportToolStripMenuItem.Image = Global.iTSfv.My.Resources.Resources.report
        Me.OpenTracksReportToolStripMenuItem.Name = "OpenTracksReportToolStripMenuItem"
        Me.OpenTracksReportToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.OpenTracksReportToolStripMenuItem.Text = "Open &Tracks Report..."
        '
        'OpenMusicFolderToolStripMenuItem
        '
        Me.OpenMusicFolderToolStripMenuItem.Image = Global.iTSfv.My.Resources.Resources.musicfolder
        Me.OpenMusicFolderToolStripMenuItem.Name = "OpenMusicFolderToolStripMenuItem"
        Me.OpenMusicFolderToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.OpenMusicFolderToolStripMenuItem.Text = "Open &Music Folder..."
        '
        'LogFilesToolStripMenuItem
        '
        Me.LogFilesToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenAlbumsToolStripMenuItem, Me.OpenNoniTSStandardTrackListwToolStripMenuItem, Me.TracksThatArtworkWasAddedToolStripMenuItem, Me.TracksThatRatingWasAdjustedToolStripMenuItem, Me.TrackCountUpdatedTracksToolStripMenuItem, Me.TracksWithLowResolutionArtworkToolStripMenuItem, Me.TracksWithMultipleArtworkToolStripMenuItem, Me.TracksWithNoArtworkToolStripMenuItem, Me.TracksWithNoLyricsToolStripMenuItem, Me.MusicFolderActivityToolStripMenuItem, Me.ToolStripSeparator2, Me.SToolStripMenuItem, Me.OpenWarningsToolStripMenuItem})
        Me.LogFilesToolStripMenuItem.Name = "LogFilesToolStripMenuItem"
        Me.LogFilesToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.LogFilesToolStripMenuItem.Text = "Open &Log Files"
        '
        'OpenAlbumsToolStripMenuItem
        '
        Me.OpenAlbumsToolStripMenuItem.Name = "OpenAlbumsToolStripMenuItem"
        Me.OpenAlbumsToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.OpenAlbumsToolStripMenuItem.Text = "&Albums with Inconsecutive Track Numbers..."
        '
        'OpenNoniTSStandardTrackListwToolStripMenuItem
        '
        Me.OpenNoniTSStandardTrackListwToolStripMenuItem.Name = "OpenNoniTSStandardTrackListwToolStripMenuItem"
        Me.OpenNoniTSStandardTrackListwToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.OpenNoniTSStandardTrackListwToolStripMenuItem.Text = "Tracks not iTunes Store compliant..."
        '
        'TracksThatArtworkWasAddedToolStripMenuItem
        '
        Me.TracksThatArtworkWasAddedToolStripMenuItem.Name = "TracksThatArtworkWasAddedToolStripMenuItem"
        Me.TracksThatArtworkWasAddedToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.TracksThatArtworkWasAddedToolStripMenuItem.Text = "&Tracks that Artwork was added..."
        '
        'TracksThatRatingWasAdjustedToolStripMenuItem
        '
        Me.TracksThatRatingWasAdjustedToolStripMenuItem.Name = "TracksThatRatingWasAdjustedToolStripMenuItem"
        Me.TracksThatRatingWasAdjustedToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.TracksThatRatingWasAdjustedToolStripMenuItem.Text = "Tracks that Rating was adjusted..."
        '
        'TrackCountUpdatedTracksToolStripMenuItem
        '
        Me.TrackCountUpdatedTracksToolStripMenuItem.Name = "TrackCountUpdatedTracksToolStripMenuItem"
        Me.TrackCountUpdatedTracksToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.TrackCountUpdatedTracksToolStripMenuItem.Text = "Tracks that Track Count was updated..."
        '
        'TracksWithLowResolutionArtworkToolStripMenuItem
        '
        Me.TracksWithLowResolutionArtworkToolStripMenuItem.Name = "TracksWithLowResolutionArtworkToolStripMenuItem"
        Me.TracksWithLowResolutionArtworkToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.TracksWithLowResolutionArtworkToolStripMenuItem.Text = "Tracks with low resolution Artwork..."
        '
        'TracksWithMultipleArtworkToolStripMenuItem
        '
        Me.TracksWithMultipleArtworkToolStripMenuItem.Name = "TracksWithMultipleArtworkToolStripMenuItem"
        Me.TracksWithMultipleArtworkToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.TracksWithMultipleArtworkToolStripMenuItem.Text = "Tracks with multiple Artwork..."
        '
        'TracksWithNoArtworkToolStripMenuItem
        '
        Me.TracksWithNoArtworkToolStripMenuItem.Name = "TracksWithNoArtworkToolStripMenuItem"
        Me.TracksWithNoArtworkToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.TracksWithNoArtworkToolStripMenuItem.Text = "&Tracks without Artwork..."
        '
        'TracksWithNoLyricsToolStripMenuItem
        '
        Me.TracksWithNoLyricsToolStripMenuItem.Name = "TracksWithNoLyricsToolStripMenuItem"
        Me.TracksWithNoLyricsToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.TracksWithNoLyricsToolStripMenuItem.Text = "Tracks without Lyrics..."
        '
        'MusicFolderActivityToolStripMenuItem
        '
        Me.MusicFolderActivityToolStripMenuItem.Name = "MusicFolderActivityToolStripMenuItem"
        Me.MusicFolderActivityToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.MusicFolderActivityToolStripMenuItem.Text = "Music Folder Activity..."
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(295, 6)
        '
        'SToolStripMenuItem
        '
        Me.SToolStripMenuItem.Name = "SToolStripMenuItem"
        Me.SToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.SToolStripMenuItem.Text = "&Errors..."
        '
        'OpenWarningsToolStripMenuItem
        '
        Me.OpenWarningsToolStripMenuItem.Name = "OpenWarningsToolStripMenuItem"
        Me.OpenWarningsToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.OpenWarningsToolStripMenuItem.Text = "&Warnings..."
        '
        'TaskToolStripMenuItem
        '
        Me.TaskToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AdjustRatingsToolStripMenuItem, Me.CopyTrackInfoToClipboardToolStripMenuItem})
        Me.TaskToolStripMenuItem.Name = "TaskToolStripMenuItem"
        Me.TaskToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.TaskToolStripMenuItem.Text = "&Tasks"
        '
        'AdjustRatingsToolStripMenuItem
        '
        Me.AdjustRatingsToolStripMenuItem.Name = "AdjustRatingsToolStripMenuItem"
        Me.AdjustRatingsToolStripMenuItem.Size = New System.Drawing.Size(223, 22)
        Me.AdjustRatingsToolStripMenuItem.Text = "&Adjust Ratings"
        '
        'CopyTrackInfoToClipboardToolStripMenuItem
        '
        Me.CopyTrackInfoToClipboardToolStripMenuItem.Name = "CopyTrackInfoToClipboardToolStripMenuItem"
        Me.CopyTrackInfoToClipboardToolStripMenuItem.Size = New System.Drawing.Size(223, 22)
        Me.CopyTrackInfoToClipboardToolStripMenuItem.Text = "&Copy Track Info to Clipboard"
        '
        'ValidateSelectedTracksToolStripMenuItem
        '
        Me.ValidateSelectedTracksToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ValidateToolStripMenuItem})
        Me.ValidateSelectedTracksToolStripMenuItem.Name = "ValidateSelectedTracksToolStripMenuItem"
        Me.ValidateSelectedTracksToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.ValidateSelectedTracksToolStripMenuItem.Text = "&Selected Tracks"
        '
        'ValidateToolStripMenuItem
        '
        Me.ValidateToolStripMenuItem.Name = "ValidateToolStripMenuItem"
        Me.ValidateToolStripMenuItem.Size = New System.Drawing.Size(123, 22)
        Me.ValidateToolStripMenuItem.Text = "&Validate"
        '
        'QuickValidationToolStripMenuItem
        '
        Me.QuickValidationToolStripMenuItem.Name = "QuickValidationToolStripMenuItem"
        Me.QuickValidationToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5
        Me.QuickValidationToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.QuickValidationToolStripMenuItem.Text = "&Validate last 100 Tracks"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(211, 6)
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.AboutToolStripMenuItem.Text = "&About..."
        '
        'OptionsToolStripMenuItem
        '
        Me.OptionsToolStripMenuItem.Image = Global.iTSfv.My.Resources.Resources.application_edit
        Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
        Me.OptionsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2
        Me.OptionsToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.OptionsToolStripMenuItem.Text = "&Options..."
        '
        'VersionHistoryToolStripMenuItem
        '
        Me.VersionHistoryToolStripMenuItem.Name = "VersionHistoryToolStripMenuItem"
        Me.VersionHistoryToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.VersionHistoryToolStripMenuItem.Text = "&Version History..."
        '
        'CheckForUpdatesToolStripMenuItem
        '
        Me.CheckForUpdatesToolStripMenuItem.Image = Global.iTSfv.My.Resources.Resources.world
        Me.CheckForUpdatesToolStripMenuItem.Name = "CheckForUpdatesToolStripMenuItem"
        Me.CheckForUpdatesToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.CheckForUpdatesToolStripMenuItem.Text = "&Check for Updates..."
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(211, 6)
        '
        'SendToSystemTrayToolStripMenuItem
        '
        Me.SendToSystemTrayToolStripMenuItem.Name = "SendToSystemTrayToolStripMenuItem"
        Me.SendToSystemTrayToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F12
        Me.SendToSystemTrayToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.SendToSystemTrayToolStripMenuItem.Text = "&Send to System Tray"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Image = Global.iTSfv.My.Resources.Resources._exit
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(214, 22)
        Me.ExitToolStripMenuItem.Text = "E&xit"
        '
        'chkWinExportArtwork
        '
        Me.chkWinExportArtwork.AutoSize = True
        Me.chkWinExportArtwork.Checked = True
        Me.chkWinExportArtwork.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkWinExportArtwork.Location = New System.Drawing.Point(10, 10)
        Me.chkWinExportArtwork.Name = "chkWinExportArtwork"
        Me.chkWinExportArtwork.Size = New System.Drawing.Size(231, 17)
        Me.chkWinExportArtwork.TabIndex = 6
        Me.chkWinExportArtwork.Text = "Export &Artwork to Album folder as Folder.jpg"
        Me.chkWinExportArtwork.UseVisualStyleBackColor = True
        '
        'chkResume
        '
        Me.chkResume.AutoSize = True
        Me.chkResume.Checked = True
        Me.chkResume.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkResume.Location = New System.Drawing.Point(351, 41)
        Me.chkResume.Name = "chkResume"
        Me.chkResume.Size = New System.Drawing.Size(183, 17)
        Me.chkResume.TabIndex = 7
        Me.chkResume.Text = "Resume from last checked album"
        Me.chkResume.UseVisualStyleBackColor = True
        '
        'tmrFormClose
        '
        '
        'tcTabs
        '
        Me.tcTabs.Controls.Add(Me.tpSettings)
        Me.tcTabs.Controls.Add(Me.tpSelectedTracks)
        Me.tcTabs.Controls.Add(Me.tpExplorer)
        Me.tcTabs.Controls.Add(Me.tpDiscsBrowser)
        Me.tcTabs.Controls.Add(Me.tpBackupRestore)
        Me.tcTabs.Controls.Add(Me.tpOneTouch)
        Me.tcTabs.Controls.Add(Me.tpSchedule)
        Me.tcTabs.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tcTabs.ImageList = Me.ilTabs
        Me.tcTabs.Location = New System.Drawing.Point(3, 3)
        Me.tcTabs.Multiline = True
        Me.tcTabs.Name = "tcTabs"
        Me.tcTabs.SelectedIndex = 0
        Me.tcTabs.Size = New System.Drawing.Size(761, 326)
        Me.tcTabs.TabIndex = 9
        '
        'tpSettings
        '
        Me.tpSettings.Controls.Add(Me.tcValidate)
        Me.tpSettings.ImageKey = "(none)"
        Me.tpSettings.Location = New System.Drawing.Point(4, 23)
        Me.tpSettings.Name = "tpSettings"
        Me.tpSettings.Padding = New System.Windows.Forms.Padding(3)
        Me.tpSettings.Size = New System.Drawing.Size(753, 299)
        Me.tpSettings.TabIndex = 0
        Me.tpSettings.Tag = "Main"
        Me.tpSettings.Text = "Validate"
        Me.tpSettings.UseVisualStyleBackColor = True
        '
        'tcValidate
        '
        Me.tcValidate.Controls.Add(Me.tpChecks)
        Me.tcValidate.Controls.Add(Me.tpEditTracks)
        Me.tcValidate.Controls.Add(Me.tpEditLibrary)
        Me.tcValidate.Controls.Add(Me.tpFileSystem)
        Me.tcValidate.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tcValidate.ImageList = Me.ilTabs
        Me.tcValidate.Location = New System.Drawing.Point(3, 3)
        Me.tcValidate.Name = "tcValidate"
        Me.tcValidate.SelectedIndex = 0
        Me.tcValidate.Size = New System.Drawing.Size(747, 293)
        Me.tcValidate.TabIndex = 17
        '
        'tpChecks
        '
        Me.tpChecks.Controls.Add(Me.btnValidateTracksChecks)
        Me.tpChecks.Controls.Add(Me.chkCheckMetatag)
        Me.tpChecks.Controls.Add(Me.chkCheckBPM)
        Me.tpChecks.Controls.Add(Me.chkCheckFoldersWithoutArtwork)
        Me.tpChecks.Controls.Add(Me.chkCheckEmbeddedArtwork)
        Me.tpChecks.Controls.Add(Me.chkCheckArtworkLowRes)
        Me.tpChecks.Controls.Add(Me.chkCheckLyrics)
        Me.tpChecks.Controls.Add(Me.chkItunesStoreStandard)
        Me.tpChecks.Controls.Add(Me.chkCheckTrackNum)
        Me.tpChecks.Controls.Add(Me.chkCheckArtwork)
        Me.tpChecks.ImageKey = "tick.png"
        Me.tpChecks.Location = New System.Drawing.Point(4, 23)
        Me.tpChecks.Name = "tpChecks"
        Me.tpChecks.Padding = New System.Windows.Forms.Padding(3)
        Me.tpChecks.Size = New System.Drawing.Size(739, 266)
        Me.tpChecks.TabIndex = 0
        Me.tpChecks.Text = "Checks"
        Me.ttApp.SetToolTip(Me.tpChecks, "Checks in this tab do not modify tracks or library or file system any way.")
        Me.tpChecks.UseVisualStyleBackColor = True
        '
        'btnValidateTracksChecks
        '
        Me.btnValidateTracksChecks.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnValidateTracksChecks.AutoSize = True
        Me.btnValidateTracksChecks.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnValidateTracksChecks.Location = New System.Drawing.Point(531, 226)
        Me.btnValidateTracksChecks.Name = "btnValidateTracksChecks"
        Me.btnValidateTracksChecks.Size = New System.Drawing.Size(188, 25)
        Me.btnValidateTracksChecks.TabIndex = 18
        Me.btnValidateTracksChecks.Text = "Check Standard in Selected Tracks"
        Me.btnValidateTracksChecks.UseVisualStyleBackColor = True
        '
        'chkCheckMetatag
        '
        Me.chkCheckMetatag.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkCheckMetatag.AutoSize = True
        Me.chkCheckMetatag.Checked = Global.iTSfv.My.MySettings.Default.CheckMetatag
        Me.chkCheckMetatag.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "CheckMetatag", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkCheckMetatag.Location = New System.Drawing.Point(450, 33)
        Me.chkCheckMetatag.Name = "chkCheckMetatag"
        Me.chkCheckMetatag.Size = New System.Drawing.Size(156, 17)
        Me.chkCheckMetatag.TabIndex = 17
        Me.chkCheckMetatag.Text = "Check for Metatag versions"
        Me.chkCheckMetatag.UseVisualStyleBackColor = True
        '
        'chkCheckBPM
        '
        Me.chkCheckBPM.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkCheckBPM.AutoSize = True
        Me.chkCheckBPM.Checked = Global.iTSfv.My.MySettings.Default.CheckBPM
        Me.chkCheckBPM.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "CheckBPM", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkCheckBPM.Location = New System.Drawing.Point(450, 10)
        Me.chkCheckBPM.Name = "chkCheckBPM"
        Me.chkCheckBPM.Size = New System.Drawing.Size(167, 17)
        Me.chkCheckBPM.TabIndex = 16
        Me.chkCheckBPM.Text = "Check for tracks without BPM"
        Me.chkCheckBPM.UseVisualStyleBackColor = True
        '
        'chkCheckFoldersWithoutArtwork
        '
        Me.chkCheckFoldersWithoutArtwork.AutoSize = True
        Me.chkCheckFoldersWithoutArtwork.Checked = Global.iTSfv.My.MySettings.Default.CheckFoldersWithoutArtwork
        Me.chkCheckFoldersWithoutArtwork.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "CheckFoldersWithoutArtwork", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkCheckFoldersWithoutArtwork.Location = New System.Drawing.Point(10, 148)
        Me.chkCheckFoldersWithoutArtwork.Name = "chkCheckFoldersWithoutArtwork"
        Me.chkCheckFoldersWithoutArtwork.Size = New System.Drawing.Size(223, 17)
        Me.chkCheckFoldersWithoutArtwork.TabIndex = 15
        Me.chkCheckFoldersWithoutArtwork.Text = "Check for album folders without Folder.jpg"
        Me.chkCheckFoldersWithoutArtwork.UseVisualStyleBackColor = True
        '
        'chkCheckEmbeddedArtwork
        '
        Me.chkCheckEmbeddedArtwork.AutoSize = True
        Me.chkCheckEmbeddedArtwork.Checked = Global.iTSfv.My.MySettings.Default.CheckDownloadedArtwork
        Me.chkCheckEmbeddedArtwork.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "CheckDownloadedArtwork", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkCheckEmbeddedArtwork.Location = New System.Drawing.Point(10, 56)
        Me.chkCheckEmbeddedArtwork.Name = "chkCheckEmbeddedArtwork"
        Me.chkCheckEmbeddedArtwork.Size = New System.Drawing.Size(261, 17)
        Me.chkCheckEmbeddedArtwork.TabIndex = 14
        Me.chkCheckEmbeddedArtwork.Text = "Check for tracks with iTunes downloaded Artwork"
        Me.chkCheckEmbeddedArtwork.UseVisualStyleBackColor = True
        '
        'chkCheckArtworkLowRes
        '
        Me.chkCheckArtworkLowRes.AutoSize = True
        Me.chkCheckArtworkLowRes.Checked = Global.iTSfv.My.MySettings.Default.CheckArtworkLowRes
        Me.chkCheckArtworkLowRes.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "CheckArtworkLowRes", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkCheckArtworkLowRes.Location = New System.Drawing.Point(10, 102)
        Me.chkCheckArtworkLowRes.Name = "chkCheckArtworkLowRes"
        Me.chkCheckArtworkLowRes.Size = New System.Drawing.Size(178, 17)
        Me.chkCheckArtworkLowRes.TabIndex = 13
        Me.chkCheckArtworkLowRes.Text = "Check for low resolution Artwork"
        Me.ttApp.SetToolTip(Me.chkCheckArtworkLowRes, "To specifiy low resolution dimensions, press F2")
        Me.chkCheckArtworkLowRes.UseVisualStyleBackColor = True
        '
        'chkCheckLyrics
        '
        Me.chkCheckLyrics.AutoSize = True
        Me.chkCheckLyrics.Checked = Global.iTSfv.My.MySettings.Default.CheckLyrics
        Me.chkCheckLyrics.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "CheckLyrics", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkCheckLyrics.Location = New System.Drawing.Point(10, 125)
        Me.chkCheckLyrics.Name = "chkCheckLyrics"
        Me.chkCheckLyrics.Size = New System.Drawing.Size(171, 17)
        Me.chkCheckLyrics.TabIndex = 12
        Me.chkCheckLyrics.Text = "Check for tracks without Lyrics"
        Me.chkCheckLyrics.UseVisualStyleBackColor = True
        '
        'chkItunesStoreStandard
        '
        Me.chkItunesStoreStandard.AutoSize = True
        Me.chkItunesStoreStandard.Checked = Global.iTSfv.My.MySettings.Default.CheckStandard
        Me.chkItunesStoreStandard.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "CheckStandard", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkItunesStoreStandard.Location = New System.Drawing.Point(10, 10)
        Me.chkItunesStoreStandard.Name = "chkItunesStoreStandard"
        Me.chkItunesStoreStandard.Size = New System.Drawing.Size(179, 17)
        Me.chkItunesStoreStandard.TabIndex = 10
        Me.chkItunesStoreStandard.Text = "Check for iTunes Store standard"
        Me.chkItunesStoreStandard.UseVisualStyleBackColor = True
        '
        'chkCheckTrackNum
        '
        Me.chkCheckTrackNum.AutoSize = True
        Me.chkCheckTrackNum.Checked = Global.iTSfv.My.MySettings.Default.CheckTrackCount
        Me.chkCheckTrackNum.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkCheckTrackNum.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "CheckTrackCount", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkCheckTrackNum.Location = New System.Drawing.Point(10, 79)
        Me.chkCheckTrackNum.Name = "chkCheckTrackNum"
        Me.chkCheckTrackNum.Size = New System.Drawing.Size(212, 17)
        Me.chkCheckTrackNum.TabIndex = 9
        Me.chkCheckTrackNum.Text = "Check for tracks without Track Number"
        Me.chkCheckTrackNum.UseVisualStyleBackColor = True
        '
        'chkCheckArtwork
        '
        Me.chkCheckArtwork.AutoSize = True
        Me.chkCheckArtwork.Checked = Global.iTSfv.My.MySettings.Default.CheckArtwork
        Me.chkCheckArtwork.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkCheckArtwork.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "CheckArtwork", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkCheckArtwork.Location = New System.Drawing.Point(10, 33)
        Me.chkCheckArtwork.Name = "chkCheckArtwork"
        Me.chkCheckArtwork.Size = New System.Drawing.Size(180, 17)
        Me.chkCheckArtwork.TabIndex = 3
        Me.chkCheckArtwork.Text = "Check for tracks without Artwork"
        Me.chkCheckArtwork.UseVisualStyleBackColor = True
        '
        'tpEditTracks
        '
        Me.tpEditTracks.Controls.Add(Me.chkEditCopyAlbumArtistToSortArtist)
        Me.tpEditTracks.Controls.Add(Me.chkConvertArtworkJPG)
        Me.tpEditTracks.Controls.Add(Me.btnValidateSelectedTracks)
        Me.tpEditTracks.Controls.Add(Me.chkMultiArtworkRemove)
        Me.tpEditTracks.Controls.Add(Me.chkImportArtwork)
        Me.tpEditTracks.Controls.Add(Me.chkEditTrackCountEtc)
        Me.tpEditTracks.Controls.Add(Me.chkEditCopyArtistToAlbumArtist)
        Me.tpEditTracks.Controls.Add(Me.chkEditEQbyGenre)
        Me.tpEditTracks.Controls.Add(Me.chkWriteGenre)
        Me.tpEditTracks.Controls.Add(Me.chkRemoveLowResArtwork)
        Me.tpEditTracks.Controls.Add(Me.chkUpdateInfoFromFile)
        Me.tpEditTracks.Controls.Add(Me.chkImportLyrics)
        Me.tpEditTracks.Controls.Add(Me.chkRemoveNull)
        Me.tpEditTracks.ImageIndex = 3
        Me.tpEditTracks.Location = New System.Drawing.Point(4, 23)
        Me.tpEditTracks.Name = "tpEditTracks"
        Me.tpEditTracks.Padding = New System.Windows.Forms.Padding(3)
        Me.tpEditTracks.Size = New System.Drawing.Size(739, 266)
        Me.tpEditTracks.TabIndex = 1
        Me.tpEditTracks.Text = "Tracks"
        Me.ttApp.SetToolTip(Me.tpEditTracks, "This tab consists of operations that modify iTunes track tags ")
        Me.tpEditTracks.UseVisualStyleBackColor = True
        '
        'chkEditCopyAlbumArtistToSortArtist
        '
        Me.chkEditCopyAlbumArtistToSortArtist.AutoSize = True
        Me.chkEditCopyAlbumArtistToSortArtist.Checked = Global.iTSfv.My.MySettings.Default.CopyAlbumArtistToSortArtist
        Me.chkEditCopyAlbumArtistToSortArtist.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "CopyAlbumArtistToSortArtist", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkEditCopyAlbumArtistToSortArtist.Location = New System.Drawing.Point(8, 32)
        Me.chkEditCopyAlbumArtistToSortArtist.Name = "chkEditCopyAlbumArtistToSortArtist"
        Me.chkEditCopyAlbumArtistToSortArtist.Size = New System.Drawing.Size(246, 17)
        Me.chkEditCopyAlbumArtistToSortArtist.TabIndex = 23
        Me.chkEditCopyAlbumArtistToSortArtist.Text = "Ensure Sort Artist and Album Artist are identical"
        Me.chkEditCopyAlbumArtistToSortArtist.UseVisualStyleBackColor = True
        '
        'chkConvertArtworkJPG
        '
        Me.chkConvertArtworkJPG.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkConvertArtworkJPG.AutoSize = True
        Me.chkConvertArtworkJPG.Checked = Global.iTSfv.My.MySettings.Default.ConvertArtworkJPG
        Me.chkConvertArtworkJPG.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "ConvertArtworkJPG", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkConvertArtworkJPG.Location = New System.Drawing.Point(448, 80)
        Me.chkConvertArtworkJPG.Name = "chkConvertArtworkJPG"
        Me.chkConvertArtworkJPG.Size = New System.Drawing.Size(176, 17)
        Me.chkConvertArtworkJPG.TabIndex = 22
        Me.chkConvertArtworkJPG.Text = "Convert Artwork to JPEG format"
        Me.chkConvertArtworkJPG.UseVisualStyleBackColor = True
        '
        'btnValidateSelectedTracks
        '
        Me.btnValidateSelectedTracks.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnValidateSelectedTracks.AutoSize = True
        Me.btnValidateSelectedTracks.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnValidateSelectedTracks.Location = New System.Drawing.Point(531, 226)
        Me.btnValidateSelectedTracks.Name = "btnValidateSelectedTracks"
        Me.btnValidateSelectedTracks.Size = New System.Drawing.Size(188, 25)
        Me.btnValidateSelectedTracks.TabIndex = 21
        Me.btnValidateSelectedTracks.Text = "Validate Tags in Selected Tracks"
        Me.btnValidateSelectedTracks.UseVisualStyleBackColor = True
        '
        'chkMultiArtworkRemove
        '
        Me.chkMultiArtworkRemove.AutoSize = True
        Me.chkMultiArtworkRemove.Location = New System.Drawing.Point(8, 152)
        Me.chkMultiArtworkRemove.Name = "chkMultiArtworkRemove"
        Me.chkMultiArtworkRemove.Size = New System.Drawing.Size(207, 17)
        Me.chkMultiArtworkRemove.TabIndex = 17
        Me.chkMultiArtworkRemove.Text = "&Delete Multiple Artwork except the first"
        Me.chkMultiArtworkRemove.UseVisualStyleBackColor = True
        '
        'chkImportArtwork
        '
        Me.chkImportArtwork.AutoSize = True
        Me.chkImportArtwork.Checked = True
        Me.chkImportArtwork.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkImportArtwork.Location = New System.Drawing.Point(8, 56)
        Me.chkImportArtwork.Name = "chkImportArtwork"
        Me.chkImportArtwork.Size = New System.Drawing.Size(212, 17)
        Me.chkImportArtwork.TabIndex = 10
        Me.chkImportArtwork.Text = "Import &Artwork to track from Artwork.jpg"
        Me.chkImportArtwork.UseVisualStyleBackColor = True
        '
        'chkEditTrackCountEtc
        '
        Me.chkEditTrackCountEtc.AutoSize = True
        Me.chkEditTrackCountEtc.Checked = True
        Me.chkEditTrackCountEtc.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkEditTrackCountEtc.Location = New System.Drawing.Point(8, 104)
        Me.chkEditTrackCountEtc.Name = "chkEditTrackCountEtc"
        Me.chkEditTrackCountEtc.Size = New System.Drawing.Size(243, 17)
        Me.chkEditTrackCountEtc.TabIndex = 13
        Me.chkEditTrackCountEtc.Text = "Fill Track Count, Disc Number and Disc Count"
        Me.chkEditTrackCountEtc.UseVisualStyleBackColor = True
        '
        'chkEditEQbyGenre
        '
        Me.chkEditEQbyGenre.AutoSize = True
        Me.chkEditEQbyGenre.Location = New System.Drawing.Point(8, 128)
        Me.chkEditEQbyGenre.Name = "chkEditEQbyGenre"
        Me.chkEditEQbyGenre.Size = New System.Drawing.Size(170, 17)
        Me.chkEditEQbyGenre.TabIndex = 16
        Me.chkEditEQbyGenre.Text = "Set Track EQ based on Genre"
        Me.chkEditEQbyGenre.UseVisualStyleBackColor = True
        '
        'chkWriteGenre
        '
        Me.chkWriteGenre.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkWriteGenre.AutoSize = True
        Me.chkWriteGenre.Checked = Global.iTSfv.My.MySettings.Default.WriteGenre
        Me.chkWriteGenre.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "WriteGenre", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkWriteGenre.Location = New System.Drawing.Point(448, 8)
        Me.chkWriteGenre.Name = "chkWriteGenre"
        Me.chkWriteGenre.Size = New System.Drawing.Size(190, 17)
        Me.chkWriteGenre.TabIndex = 20
        Me.chkWriteGenre.Text = "Fill missing Genre tag using Last.fm"
        Me.chkWriteGenre.UseVisualStyleBackColor = True
        '
        'chkRemoveLowResArtwork
        '
        Me.chkRemoveLowResArtwork.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkRemoveLowResArtwork.AutoSize = True
        Me.chkRemoveLowResArtwork.Checked = Global.iTSfv.My.MySettings.Default.RemoveLowResArtwork
        Me.chkRemoveLowResArtwork.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "RemoveLowResArtwork", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkRemoveLowResArtwork.ForeColor = System.Drawing.Color.Red
        Me.chkRemoveLowResArtwork.Location = New System.Drawing.Point(448, 56)
        Me.chkRemoveLowResArtwork.Name = "chkRemoveLowResArtwork"
        Me.chkRemoveLowResArtwork.Size = New System.Drawing.Size(171, 17)
        Me.chkRemoveLowResArtwork.TabIndex = 15
        Me.chkRemoveLowResArtwork.Text = "&Remove low resolution artwork"
        Me.chkRemoveLowResArtwork.UseVisualStyleBackColor = True
        '
        'chkUpdateInfoFromFile
        '
        Me.chkUpdateInfoFromFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkUpdateInfoFromFile.AutoSize = True
        Me.chkUpdateInfoFromFile.Checked = Global.iTSfv.My.MySettings.Default.UpdateInfoFromFile
        Me.chkUpdateInfoFromFile.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "UpdateInfoFromFile", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkUpdateInfoFromFile.Location = New System.Drawing.Point(448, 32)
        Me.chkUpdateInfoFromFile.Name = "chkUpdateInfoFromFile"
        Me.chkUpdateInfoFromFile.Size = New System.Drawing.Size(210, 17)
        Me.chkUpdateInfoFromFile.TabIndex = 19
        Me.chkUpdateInfoFromFile.Text = "Update Database refreshingTags in file"
        Me.ttApp.SetToolTip(Me.chkUpdateInfoFromFile, "Validating iTunes Music Library with this setting checked will be extremely slow")
        Me.chkUpdateInfoFromFile.UseVisualStyleBackColor = True
        '
        'chkImportLyrics
        '
        Me.chkImportLyrics.AutoSize = True
        Me.chkImportLyrics.Checked = Global.iTSfv.My.MySettings.Default.LyricsImport
        Me.chkImportLyrics.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkImportLyrics.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "LyricsImport", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkImportLyrics.Location = New System.Drawing.Point(8, 80)
        Me.chkImportLyrics.Name = "chkImportLyrics"
        Me.chkImportLyrics.Size = New System.Drawing.Size(211, 17)
        Me.chkImportLyrics.TabIndex = 19
        Me.chkImportLyrics.Text = "Import &Lyrics  to track from Album folder"
        Me.chkImportLyrics.UseVisualStyleBackColor = True
        '
        'chkRemoveNull
        '
        Me.chkRemoveNull.AutoSize = True
        Me.chkRemoveNull.Checked = Global.iTSfv.My.MySettings.Default.RemoveNullChar
        Me.chkRemoveNull.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "RemoveNullChar", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkRemoveNull.Location = New System.Drawing.Point(8, 176)
        Me.chkRemoveNull.Name = "chkRemoveNull"
        Me.chkRemoveNull.Size = New System.Drawing.Size(230, 17)
        Me.chkRemoveNull.TabIndex = 18
        Me.chkRemoveNull.Text = "&Remove Null characters at the end of Tags"
        Me.ttApp.SetToolTip(Me.chkRemoveNull, "Validating iTunes Music Library with this setting checked will be slow")
        Me.chkRemoveNull.UseVisualStyleBackColor = True
        '
        'tpEditLibrary
        '
        Me.tpEditLibrary.Controls.Add(Me.chkPlayedCountImportPCNT)
        Me.tpEditLibrary.Controls.Add(Me.chkRatingsImportPOPM)
        Me.tpEditLibrary.Controls.Add(Me.btnValidateSelectedTracksLibrary)
        Me.tpEditLibrary.Controls.Add(Me.chkDeleteNonMusicFolderTracks)
        Me.tpEditLibrary.Controls.Add(Me.chkDeleteTracksNotInHDD)
        Me.tpEditLibrary.Controls.Add(Me.chkLibraryAdjustRatings)
        Me.tpEditLibrary.Controls.Add(Me.chkValidationPlaylists)
        Me.tpEditLibrary.ImageKey = "database_edit.png"
        Me.tpEditLibrary.Location = New System.Drawing.Point(4, 23)
        Me.tpEditLibrary.Name = "tpEditLibrary"
        Me.tpEditLibrary.Size = New System.Drawing.Size(739, 266)
        Me.tpEditLibrary.TabIndex = 2
        Me.tpEditLibrary.Text = "Library"
        Me.ttApp.SetToolTip(Me.tpEditLibrary, "This tab consists of operations that modifies the iTunes Library playlist. " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "This" & _
                " tab does not modify tags in Tracks.")
        Me.tpEditLibrary.UseVisualStyleBackColor = True
        '
        'chkPlayedCountImportPCNT
        '
        Me.chkPlayedCountImportPCNT.AutoSize = True
        Me.chkPlayedCountImportPCNT.Checked = Global.iTSfv.My.MySettings.Default.PlayedCountImportPCNT
        Me.chkPlayedCountImportPCNT.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "PlayedCountImportPCNT", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkPlayedCountImportPCNT.Location = New System.Drawing.Point(10, 102)
        Me.chkPlayedCountImportPCNT.Name = "chkPlayedCountImportPCNT"
        Me.chkPlayedCountImportPCNT.Size = New System.Drawing.Size(251, 17)
        Me.chkPlayedCountImportPCNT.TabIndex = 19
        Me.chkPlayedCountImportPCNT.Text = "Import PlayedCount from POPM or PCNT Frame"
        Me.chkPlayedCountImportPCNT.UseVisualStyleBackColor = True
        '
        'chkRatingsImportPOPM
        '
        Me.chkRatingsImportPOPM.AutoSize = True
        Me.chkRatingsImportPOPM.Checked = Global.iTSfv.My.MySettings.Default.RatingImportPOPM
        Me.chkRatingsImportPOPM.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "RatingImportPOPM", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkRatingsImportPOPM.Location = New System.Drawing.Point(10, 79)
        Me.chkRatingsImportPOPM.Name = "chkRatingsImportPOPM"
        Me.chkRatingsImportPOPM.Size = New System.Drawing.Size(178, 17)
        Me.chkRatingsImportPOPM.TabIndex = 18
        Me.chkRatingsImportPOPM.Text = "Import &Rating from POPM Frame"
        Me.chkRatingsImportPOPM.UseVisualStyleBackColor = True
        '
        'btnValidateSelectedTracksLibrary
        '
        Me.btnValidateSelectedTracksLibrary.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnValidateSelectedTracksLibrary.AutoSize = True
        Me.btnValidateSelectedTracksLibrary.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnValidateSelectedTracksLibrary.Location = New System.Drawing.Point(531, 226)
        Me.btnValidateSelectedTracksLibrary.Name = "btnValidateSelectedTracksLibrary"
        Me.btnValidateSelectedTracksLibrary.Size = New System.Drawing.Size(188, 25)
        Me.btnValidateSelectedTracksLibrary.TabIndex = 17
        Me.btnValidateSelectedTracksLibrary.Text = "Validate Library for Selected Tracks"
        Me.btnValidateSelectedTracksLibrary.UseVisualStyleBackColor = True
        '
        'chkDeleteNonMusicFolderTracks
        '
        Me.chkDeleteNonMusicFolderTracks.AutoSize = True
        Me.chkDeleteNonMusicFolderTracks.Location = New System.Drawing.Point(10, 33)
        Me.chkDeleteNonMusicFolderTracks.Name = "chkDeleteNonMusicFolderTracks"
        Me.chkDeleteNonMusicFolderTracks.Size = New System.Drawing.Size(186, 17)
        Me.chkDeleteNonMusicFolderTracks.TabIndex = 14
        Me.chkDeleteNonMusicFolderTracks.Text = "Delete tracks not in Music Folders"
        Me.chkDeleteNonMusicFolderTracks.UseVisualStyleBackColor = True
        '
        'chkLibraryAdjustRatings
        '
        Me.chkLibraryAdjustRatings.AutoSize = True
        Me.chkLibraryAdjustRatings.Checked = Global.iTSfv.My.MySettings.Default.LibraryAdjustRatings
        Me.chkLibraryAdjustRatings.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "LibraryAdjustRatings", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkLibraryAdjustRatings.Location = New System.Drawing.Point(10, 56)
        Me.chkLibraryAdjustRatings.Name = "chkLibraryAdjustRatings"
        Me.chkLibraryAdjustRatings.Size = New System.Drawing.Size(227, 17)
        Me.chkLibraryAdjustRatings.TabIndex = 15
        Me.chkLibraryAdjustRatings.Text = "Adjust the Rating according to play pattern"
        Me.chkLibraryAdjustRatings.UseVisualStyleBackColor = True
        '
        'chkValidationPlaylists
        '
        Me.chkValidationPlaylists.AutoSize = True
        Me.chkValidationPlaylists.Checked = Global.iTSfv.My.MySettings.Default.CreateValidationPlaylists
        Me.chkValidationPlaylists.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "CreateValidationPlaylists", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkValidationPlaylists.Location = New System.Drawing.Point(10, 125)
        Me.chkValidationPlaylists.Name = "chkValidationPlaylists"
        Me.chkValidationPlaylists.Size = New System.Drawing.Size(224, 17)
        Me.chkValidationPlaylists.TabIndex = 16
        Me.chkValidationPlaylists.Text = "&Save Validation Results in iTunes Playlists"
        Me.chkValidationPlaylists.UseVisualStyleBackColor = True
        '
        'tpFileSystem
        '
        Me.tpFileSystem.Controls.Add(Me.btnValidateSelectedTracksFolder)
        Me.tpFileSystem.Controls.Add(Me.chkWinExportPlaylist)
        Me.tpFileSystem.Controls.Add(Me.chkWinExportArtwork)
        Me.tpFileSystem.Controls.Add(Me.chkWinMakeReadOnly)
        Me.tpFileSystem.Controls.Add(Me.chkExportLyrics)
        Me.tpFileSystem.Controls.Add(Me.chkExportIndex)
        Me.tpFileSystem.Controls.Add(Me.chkVistaThumbnailFix)
        Me.tpFileSystem.ImageKey = "folder_edit.png"
        Me.tpFileSystem.Location = New System.Drawing.Point(4, 23)
        Me.tpFileSystem.Name = "tpFileSystem"
        Me.tpFileSystem.Size = New System.Drawing.Size(739, 266)
        Me.tpFileSystem.TabIndex = 3
        Me.tpFileSystem.Text = "File System"
        Me.ttApp.SetToolTip(Me.tpFileSystem, "This tab consists of operations that interacts with the Windows file system. " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Th" & _
                "is tab does not modify Tracks or Library Playlist.")
        Me.tpFileSystem.UseVisualStyleBackColor = True
        '
        'btnValidateSelectedTracksFolder
        '
        Me.btnValidateSelectedTracksFolder.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnValidateSelectedTracksFolder.AutoSize = True
        Me.btnValidateSelectedTracksFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnValidateSelectedTracksFolder.Location = New System.Drawing.Point(531, 226)
        Me.btnValidateSelectedTracksFolder.Name = "btnValidateSelectedTracksFolder"
        Me.btnValidateSelectedTracksFolder.Size = New System.Drawing.Size(188, 25)
        Me.btnValidateSelectedTracksFolder.TabIndex = 16
        Me.btnValidateSelectedTracksFolder.Text = "Validate Folders of Selected Tracks"
        Me.btnValidateSelectedTracksFolder.UseVisualStyleBackColor = True
        '
        'chkWinExportPlaylist
        '
        Me.chkWinExportPlaylist.AutoSize = True
        Me.chkWinExportPlaylist.Location = New System.Drawing.Point(10, 33)
        Me.chkWinExportPlaylist.Name = "chkWinExportPlaylist"
        Me.chkWinExportPlaylist.Size = New System.Drawing.Size(164, 17)
        Me.chkWinExportPlaylist.TabIndex = 13
        Me.chkWinExportPlaylist.Text = "Export &Playlist to Album folder"
        Me.chkWinExportPlaylist.UseVisualStyleBackColor = True
        '
        'chkWinMakeReadOnly
        '
        Me.chkWinMakeReadOnly.AutoSize = True
        Me.chkWinMakeReadOnly.Location = New System.Drawing.Point(10, 102)
        Me.chkWinMakeReadOnly.Name = "chkWinMakeReadOnly"
        Me.chkWinMakeReadOnly.Size = New System.Drawing.Size(244, 17)
        Me.chkWinMakeReadOnly.TabIndex = 11
        Me.chkWinMakeReadOnly.Text = "&Set Read-Only attribute to tag complete tracks"
        Me.chkWinMakeReadOnly.UseVisualStyleBackColor = True
        '
        'chkExportLyrics
        '
        Me.chkExportLyrics.AutoSize = True
        Me.chkExportLyrics.Checked = Global.iTSfv.My.MySettings.Default.ExportLyrics
        Me.chkExportLyrics.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "ExportLyrics", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkExportLyrics.Location = New System.Drawing.Point(10, 79)
        Me.chkExportLyrics.Name = "chkExportLyrics"
        Me.chkExportLyrics.Size = New System.Drawing.Size(159, 17)
        Me.chkExportLyrics.TabIndex = 15
        Me.chkExportLyrics.Text = "Export &Lyrics to Album folder"
        Me.chkExportLyrics.UseVisualStyleBackColor = True
        '
        'chkExportIndex
        '
        Me.chkExportIndex.AutoSize = True
        Me.chkExportIndex.Checked = Global.iTSfv.My.MySettings.Default.ExportIndex
        Me.chkExportIndex.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "ExportIndex", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkExportIndex.Location = New System.Drawing.Point(10, 56)
        Me.chkExportIndex.Name = "chkExportIndex"
        Me.chkExportIndex.Size = New System.Drawing.Size(222, 17)
        Me.chkExportIndex.TabIndex = 14
        Me.chkExportIndex.Text = "Export Index to Album folder as index.html"
        Me.chkExportIndex.UseVisualStyleBackColor = True
        '
        'chkVistaThumbnailFix
        '
        Me.chkVistaThumbnailFix.AutoSize = True
        Me.chkVistaThumbnailFix.Checked = Global.iTSfv.My.MySettings.Default.FixFolderThumbnail
        Me.chkVistaThumbnailFix.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "FixFolderThumbnail", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkVistaThumbnailFix.Location = New System.Drawing.Point(10, 125)
        Me.chkVistaThumbnailFix.Name = "chkVistaThumbnailFix"
        Me.chkVistaThumbnailFix.Size = New System.Drawing.Size(192, 17)
        Me.chkVistaThumbnailFix.TabIndex = 12
        Me.chkVistaThumbnailFix.Text = "Fix Folder Thumbnail in Artist Folder"
        Me.chkVistaThumbnailFix.UseVisualStyleBackColor = True
        '
        'ilTabs
        '
        Me.ilTabs.ImageStream = CType(resources.GetObject("ilTabs.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ilTabs.TransparentColor = System.Drawing.Color.Transparent
        Me.ilTabs.Images.SetKeyName(0, "clock.png")
        Me.ilTabs.Images.SetKeyName(1, "folder_edit.png")
        Me.ilTabs.Images.SetKeyName(2, "database_edit.png")
        Me.ilTabs.Images.SetKeyName(3, "cd_edit.png")
        Me.ilTabs.Images.SetKeyName(4, "tick.png")
        Me.ilTabs.Images.SetKeyName(5, "folder_explore.png")
        Me.ilTabs.Images.SetKeyName(6, "cd.png")
        Me.ilTabs.Images.SetKeyName(7, "tag_blue.png")
        Me.ilTabs.Images.SetKeyName(8, "chart_bar.png")
        Me.ilTabs.Images.SetKeyName(9, "wand.png")
        Me.ilTabs.Images.SetKeyName(10, "tag_blue_edit.png")
        Me.ilTabs.Images.SetKeyName(11, "page_copy.png")
        Me.ilTabs.Images.SetKeyName(12, "folder_go.png")
        Me.ilTabs.Images.SetKeyName(13, "bomb.png")
        Me.ilTabs.Images.SetKeyName(14, "database_edit.png")
        Me.ilTabs.Images.SetKeyName(15, "check-vista.png")
        Me.ilTabs.Images.SetKeyName(16, "heart_add.png")
        Me.ilTabs.Images.SetKeyName(17, "music.png")
        Me.ilTabs.Images.SetKeyName(18, "folder_image.png")
        '
        'tpSelectedTracks
        '
        Me.tpSelectedTracks.Controls.Add(Me.tcSelectedTracks)
        Me.tpSelectedTracks.Location = New System.Drawing.Point(4, 23)
        Me.tpSelectedTracks.Name = "tpSelectedTracks"
        Me.tpSelectedTracks.Padding = New System.Windows.Forms.Padding(3)
        Me.tpSelectedTracks.Size = New System.Drawing.Size(753, 299)
        Me.tpSelectedTracks.TabIndex = 1
        Me.tpSelectedTracks.Text = "Selected Tracks"
        Me.tpSelectedTracks.UseVisualStyleBackColor = True
        '
        'tcSelectedTracks
        '
        Me.tcSelectedTracks.Controls.Add(Me.tpEditor)
        Me.tcSelectedTracks.Controls.Add(Me.tpSTClipboard)
        Me.tcSelectedTracks.Controls.Add(Me.tpSTCheat)
        Me.tcSelectedTracks.Controls.Add(Me.tpSTExport)
        Me.tcSelectedTracks.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tcSelectedTracks.ImageList = Me.ilTabs
        Me.tcSelectedTracks.Location = New System.Drawing.Point(3, 3)
        Me.tcSelectedTracks.Name = "tcSelectedTracks"
        Me.tcSelectedTracks.SelectedIndex = 0
        Me.tcSelectedTracks.Size = New System.Drawing.Size(747, 293)
        Me.tcSelectedTracks.TabIndex = 15
        '
        'tpEditor
        '
        Me.tpEditor.Controls.Add(Me.chkRemoveComments)
        Me.tpEditor.Controls.Add(Me.chkRemoveLyrics)
        Me.tpEditor.Controls.Add(Me.txtAppend)
        Me.tpEditor.Controls.Add(Me.cboAppendChar)
        Me.tpEditor.Controls.Add(Me.chkAppendChar)
        Me.tpEditor.Controls.Add(Me.cboReplace)
        Me.tpEditor.Controls.Add(Me.cboFind)
        Me.tpEditor.Controls.Add(Me.cboTrimDirection)
        Me.tpEditor.Controls.Add(Me.Label2)
        Me.tpEditor.Controls.Add(Me.nudTrimChar)
        Me.tpEditor.Controls.Add(Me.chkTrimChar)
        Me.tpEditor.Controls.Add(Me.chkTagRemove)
        Me.tpEditor.Controls.Add(Me.chkDecompile)
        Me.tpEditor.Controls.Add(Me.chkReplaceTextInTags)
        Me.tpEditor.Controls.Add(Me.chkCapitalizeFirstLetter)
        Me.tpEditor.Controls.Add(Me.chkStrict)
        Me.tpEditor.Controls.Add(Me.gbWriteTags)
        Me.tpEditor.Controls.Add(Me.cboArtistsDecompiled)
        Me.tpEditor.Controls.Add(Me.chkRenameFile)
        Me.tpEditor.Controls.Add(Me.cboDecompileOptions)
        Me.tpEditor.Controls.Add(Me.lblWith)
        Me.tpEditor.Controls.Add(Me.Label1)
        Me.tpEditor.ImageKey = "tag_blue_edit.png"
        Me.tpEditor.Location = New System.Drawing.Point(4, 23)
        Me.tpEditor.Name = "tpEditor"
        Me.tpEditor.Padding = New System.Windows.Forms.Padding(3)
        Me.tpEditor.Size = New System.Drawing.Size(739, 266)
        Me.tpEditor.TabIndex = 1
        Me.tpEditor.Text = "Editor"
        Me.tpEditor.UseVisualStyleBackColor = True
        '
        'chkRemoveComments
        '
        Me.chkRemoveComments.AutoSize = True
        Me.chkRemoveComments.Checked = Global.iTSfv.My.MySettings.Default.RemoveComments
        Me.chkRemoveComments.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "RemoveComments", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkRemoveComments.Location = New System.Drawing.Point(96, 120)
        Me.chkRemoveComments.Name = "chkRemoveComments"
        Me.chkRemoveComments.Size = New System.Drawing.Size(75, 17)
        Me.chkRemoveComments.TabIndex = 33
        Me.chkRemoveComments.Text = "Comments"
        Me.chkRemoveComments.UseVisualStyleBackColor = True
        '
        'chkRemoveLyrics
        '
        Me.chkRemoveLyrics.AutoSize = True
        Me.chkRemoveLyrics.Checked = Global.iTSfv.My.MySettings.Default.RemoveLyrics
        Me.chkRemoveLyrics.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "RemoveLyrics", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkRemoveLyrics.Location = New System.Drawing.Point(40, 120)
        Me.chkRemoveLyrics.Name = "chkRemoveLyrics"
        Me.chkRemoveLyrics.Size = New System.Drawing.Size(53, 17)
        Me.chkRemoveLyrics.TabIndex = 32
        Me.chkRemoveLyrics.Text = "Lyrics"
        Me.chkRemoveLyrics.UseVisualStyleBackColor = True
        '
        'txtAppend
        '
        Me.txtAppend.Location = New System.Drawing.Point(173, 176)
        Me.txtAppend.Name = "txtAppend"
        Me.txtAppend.Size = New System.Drawing.Size(317, 20)
        Me.txtAppend.TabIndex = 30
        '
        'cboAppendChar
        '
        Me.cboAppendChar.AutoCompleteCustomSource.AddRange(New String() {"append", "prepend"})
        Me.cboAppendChar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboAppendChar.FormattingEnabled = True
        Me.cboAppendChar.Items.AddRange(New Object() {"append", "prepend"})
        Me.cboAppendChar.Location = New System.Drawing.Point(76, 176)
        Me.cboAppendChar.Name = "cboAppendChar"
        Me.cboAppendChar.Size = New System.Drawing.Size(91, 21)
        Me.cboAppendChar.TabIndex = 29
        '
        'chkAppendChar
        '
        Me.chkAppendChar.AutoSize = True
        Me.chkAppendChar.Location = New System.Drawing.Point(17, 178)
        Me.chkAppendChar.Name = "chkAppendChar"
        Me.chkAppendChar.Size = New System.Drawing.Size(53, 17)
        Me.chkAppendChar.TabIndex = 28
        Me.chkAppendChar.Text = "String"
        Me.chkAppendChar.UseVisualStyleBackColor = True
        '
        'cboReplace
        '
        Me.cboReplace.Enabled = False
        Me.cboReplace.FormattingEnabled = True
        Me.cboReplace.Location = New System.Drawing.Point(249, 36)
        Me.cboReplace.Name = "cboReplace"
        Me.cboReplace.Size = New System.Drawing.Size(121, 21)
        Me.cboReplace.TabIndex = 27
        '
        'cboFind
        '
        Me.cboFind.Enabled = False
        Me.cboFind.FormattingEnabled = True
        Me.cboFind.Location = New System.Drawing.Point(89, 36)
        Me.cboFind.Name = "cboFind"
        Me.cboFind.Size = New System.Drawing.Size(121, 21)
        Me.cboFind.TabIndex = 26
        '
        'cboTrimDirection
        '
        Me.cboTrimDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboTrimDirection.Enabled = False
        Me.cboTrimDirection.ForeColor = System.Drawing.Color.Red
        Me.cboTrimDirection.FormattingEnabled = True
        Me.cboTrimDirection.Items.AddRange(New Object() {"Left", "Right"})
        Me.cboTrimDirection.Location = New System.Drawing.Point(220, 148)
        Me.cboTrimDirection.Name = "cboTrimDirection"
        Me.cboTrimDirection.Size = New System.Drawing.Size(93, 21)
        Me.cboTrimDirection.TabIndex = 25
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.ForeColor = System.Drawing.Color.Red
        Me.Label2.Location = New System.Drawing.Point(134, 151)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(80, 13)
        Me.Label2.TabIndex = 24
        Me.Label2.Text = "characters from"
        '
        'nudTrimChar
        '
        Me.nudTrimChar.DataBindings.Add(New System.Windows.Forms.Binding("Value", Global.iTSfv.My.MySettings.Default, "TrimChar", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.nudTrimChar.Enabled = False
        Me.nudTrimChar.ForeColor = System.Drawing.Color.Red
        Me.nudTrimChar.Location = New System.Drawing.Point(69, 149)
        Me.nudTrimChar.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudTrimChar.Name = "nudTrimChar"
        Me.nudTrimChar.ReadOnly = True
        Me.nudTrimChar.Size = New System.Drawing.Size(59, 20)
        Me.nudTrimChar.TabIndex = 23
        Me.nudTrimChar.Value = Global.iTSfv.My.MySettings.Default.TrimChar
        '
        'chkTrimChar
        '
        Me.chkTrimChar.AutoSize = True
        Me.chkTrimChar.ForeColor = System.Drawing.Color.Red
        Me.chkTrimChar.Location = New System.Drawing.Point(17, 150)
        Me.chkTrimChar.Name = "chkTrimChar"
        Me.chkTrimChar.Size = New System.Drawing.Size(46, 17)
        Me.chkTrimChar.TabIndex = 22
        Me.chkTrimChar.Text = "&Trim"
        Me.chkTrimChar.UseVisualStyleBackColor = True
        '
        'chkTagRemove
        '
        Me.chkTagRemove.AutoSize = True
        Me.chkTagRemove.Checked = Global.iTSfv.My.MySettings.Default.EditRemoveLyrics
        Me.chkTagRemove.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "EditRemoveLyrics", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkTagRemove.Location = New System.Drawing.Point(17, 94)
        Me.chkTagRemove.Name = "chkTagRemove"
        Me.chkTagRemove.Size = New System.Drawing.Size(203, 17)
        Me.chkTagRemove.TabIndex = 21
        Me.chkTagRemove.Text = "Remove the following tag from Track:"
        Me.chkTagRemove.UseVisualStyleBackColor = True
        '
        'chkDecompile
        '
        Me.chkDecompile.AutoSize = True
        Me.chkDecompile.Checked = Global.iTSfv.My.MySettings.Default.EditDecompileTracks
        Me.chkDecompile.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "EditDecompileTracks", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkDecompile.Location = New System.Drawing.Point(17, 67)
        Me.chkDecompile.Name = "chkDecompile"
        Me.chkDecompile.Size = New System.Drawing.Size(120, 17)
        Me.chkDecompile.TabIndex = 20
        Me.chkDecompile.Text = "&Decompile tracks to"
        Me.chkDecompile.UseVisualStyleBackColor = True
        '
        'chkReplaceTextInTags
        '
        Me.chkReplaceTextInTags.AutoSize = True
        Me.chkReplaceTextInTags.Checked = Global.iTSfv.My.MySettings.Default.EditReplaceText
        Me.chkReplaceTextInTags.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "EditReplaceText", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkReplaceTextInTags.Location = New System.Drawing.Point(17, 40)
        Me.chkReplaceTextInTags.Name = "chkReplaceTextInTags"
        Me.chkReplaceTextInTags.Size = New System.Drawing.Size(66, 17)
        Me.chkReplaceTextInTags.TabIndex = 19
        Me.chkReplaceTextInTags.Text = "&Replace"
        Me.chkReplaceTextInTags.UseVisualStyleBackColor = True
        '
        'chkCapitalizeFirstLetter
        '
        Me.chkCapitalizeFirstLetter.AutoSize = True
        Me.chkCapitalizeFirstLetter.Checked = Global.iTSfv.My.MySettings.Default.EditCapitalizeWords
        Me.chkCapitalizeFirstLetter.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkCapitalizeFirstLetter.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "EditCapitalizeWords", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkCapitalizeFirstLetter.Location = New System.Drawing.Point(17, 13)
        Me.chkCapitalizeFirstLetter.Name = "chkCapitalizeFirstLetter"
        Me.chkCapitalizeFirstLetter.Size = New System.Drawing.Size(181, 17)
        Me.chkCapitalizeFirstLetter.TabIndex = 18
        Me.chkCapitalizeFirstLetter.Text = "Capitalize first letter of each word"
        Me.chkCapitalizeFirstLetter.UseVisualStyleBackColor = True
        '
        'chkStrict
        '
        Me.chkStrict.AutoSize = True
        Me.chkStrict.Checked = Global.iTSfv.My.MySettings.Default.EditCapitalizeStrict
        Me.chkStrict.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "EditCapitalizeStrict", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkStrict.Location = New System.Drawing.Point(417, 13)
        Me.chkStrict.Name = "chkStrict"
        Me.chkStrict.Size = New System.Drawing.Size(80, 17)
        Me.chkStrict.TabIndex = 16
        Me.chkStrict.Text = "&Strict Mode"
        Me.chkStrict.UseVisualStyleBackColor = True
        '
        'gbWriteTags
        '
        Me.gbWriteTags.Controls.Add(Me.chkGenre)
        Me.gbWriteTags.Controls.Add(Me.chkAlbumArtist)
        Me.gbWriteTags.Controls.Add(Me.chkArtist)
        Me.gbWriteTags.Controls.Add(Me.chkName)
        Me.gbWriteTags.Controls.Add(Me.chkAlbum)
        Me.gbWriteTags.Location = New System.Drawing.Point(616, 6)
        Me.gbWriteTags.Name = "gbWriteTags"
        Me.gbWriteTags.Size = New System.Drawing.Size(112, 153)
        Me.gbWriteTags.TabIndex = 4
        Me.gbWriteTags.TabStop = False
        Me.gbWriteTags.Text = "Write tags to"
        '
        'chkGenre
        '
        Me.chkGenre.AutoSize = True
        Me.chkGenre.Checked = Global.iTSfv.My.MySettings.Default.EditGenre
        Me.chkGenre.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "EditGenre", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkGenre.Location = New System.Drawing.Point(12, 111)
        Me.chkGenre.Name = "chkGenre"
        Me.chkGenre.Size = New System.Drawing.Size(55, 17)
        Me.chkGenre.TabIndex = 5
        Me.chkGenre.Text = "Genre"
        Me.chkGenre.UseVisualStyleBackColor = True
        '
        'chkAlbumArtist
        '
        Me.chkAlbumArtist.AutoSize = True
        Me.chkAlbumArtist.Checked = Global.iTSfv.My.MySettings.Default.EditAlbumArtist
        Me.chkAlbumArtist.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "EditAlbumArtist", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkAlbumArtist.Location = New System.Drawing.Point(12, 88)
        Me.chkAlbumArtist.Name = "chkAlbumArtist"
        Me.chkAlbumArtist.Size = New System.Drawing.Size(81, 17)
        Me.chkAlbumArtist.TabIndex = 4
        Me.chkAlbumArtist.Text = "Album Artist"
        Me.chkAlbumArtist.UseVisualStyleBackColor = True
        '
        'chkArtist
        '
        Me.chkArtist.AutoSize = True
        Me.chkArtist.Checked = Global.iTSfv.My.MySettings.Default.EditArtist
        Me.chkArtist.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "EditArtist", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkArtist.Location = New System.Drawing.Point(12, 19)
        Me.chkArtist.Name = "chkArtist"
        Me.chkArtist.Size = New System.Drawing.Size(49, 17)
        Me.chkArtist.TabIndex = 1
        Me.chkArtist.Text = "Artist"
        Me.chkArtist.UseVisualStyleBackColor = True
        '
        'chkName
        '
        Me.chkName.AutoSize = True
        Me.chkName.Checked = Global.iTSfv.My.MySettings.Default.EditName
        Me.chkName.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkName.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "EditName", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkName.Location = New System.Drawing.Point(12, 65)
        Me.chkName.Name = "chkName"
        Me.chkName.Size = New System.Drawing.Size(54, 17)
        Me.chkName.TabIndex = 3
        Me.chkName.Text = "Name"
        Me.chkName.UseVisualStyleBackColor = True
        '
        'chkAlbum
        '
        Me.chkAlbum.AutoSize = True
        Me.chkAlbum.Checked = Global.iTSfv.My.MySettings.Default.EditAlbum
        Me.chkAlbum.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "EditAlbum", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkAlbum.Location = New System.Drawing.Point(12, 42)
        Me.chkAlbum.Name = "chkAlbum"
        Me.chkAlbum.Size = New System.Drawing.Size(55, 17)
        Me.chkAlbum.TabIndex = 2
        Me.chkAlbum.Text = "Album"
        Me.chkAlbum.UseVisualStyleBackColor = True
        '
        'cboArtistsDecompiled
        '
        Me.cboArtistsDecompiled.Enabled = False
        Me.cboArtistsDecompiled.FormattingEnabled = True
        Me.cboArtistsDecompiled.Location = New System.Drawing.Point(369, 64)
        Me.cboArtistsDecompiled.Name = "cboArtistsDecompiled"
        Me.cboArtistsDecompiled.Size = New System.Drawing.Size(132, 21)
        Me.cboArtistsDecompiled.TabIndex = 15
        '
        'chkRenameFile
        '
        Me.chkRenameFile.AutoSize = True
        Me.chkRenameFile.Checked = True
        Me.chkRenameFile.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkRenameFile.Location = New System.Drawing.Point(204, 13)
        Me.chkRenameFile.Name = "chkRenameFile"
        Me.chkRenameFile.Size = New System.Drawing.Size(207, 17)
        Me.chkRenameFile.TabIndex = 9
        Me.chkRenameFile.Text = "Also &rename the file to match the Case"
        Me.chkRenameFile.UseVisualStyleBackColor = True
        '
        'cboDecompileOptions
        '
        Me.cboDecompileOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboDecompileOptions.Enabled = False
        Me.cboDecompileOptions.FormattingEnabled = True
        Me.cboDecompileOptions.Items.AddRange(New Object() {"%Name% - %Artist%", "%Name% [%Artist%]", "%Name% (%Artist%)", "%Name% (feat. %Artist%)"})
        Me.cboDecompileOptions.Location = New System.Drawing.Point(143, 64)
        Me.cboDecompileOptions.Name = "cboDecompileOptions"
        Me.cboDecompileOptions.Size = New System.Drawing.Size(148, 21)
        Me.cboDecompileOptions.TabIndex = 11
        '
        'lblWith
        '
        Me.lblWith.AutoSize = True
        Me.lblWith.Enabled = False
        Me.lblWith.Location = New System.Drawing.Point(217, 39)
        Me.lblWith.Name = "lblWith"
        Me.lblWith.Size = New System.Drawing.Size(26, 13)
        Me.lblWith.TabIndex = 7
        Me.lblWith.Text = "with"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(297, 67)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(66, 13)
        Me.Label1.TabIndex = 12
        Me.Label1.Text = "with Artist as"
        '
        'tpSTClipboard
        '
        Me.tpSTClipboard.Controls.Add(Me.chkClipboardSort)
        Me.tpSTClipboard.Controls.Add(Me.gbClipBoardTags)
        Me.tpSTClipboard.Controls.Add(Me.btnClipboard)
        Me.tpSTClipboard.ImageKey = "page_copy.png"
        Me.tpSTClipboard.Location = New System.Drawing.Point(4, 23)
        Me.tpSTClipboard.Name = "tpSTClipboard"
        Me.tpSTClipboard.Padding = New System.Windows.Forms.Padding(3)
        Me.tpSTClipboard.Size = New System.Drawing.Size(739, 266)
        Me.tpSTClipboard.TabIndex = 4
        Me.tpSTClipboard.Text = "Clipboard"
        Me.tpSTClipboard.UseVisualStyleBackColor = True
        '
        'chkClipboardSort
        '
        Me.chkClipboardSort.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.chkClipboardSort.AutoSize = True
        Me.chkClipboardSort.Checked = True
        Me.chkClipboardSort.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkClipboardSort.Location = New System.Drawing.Point(16, 234)
        Me.chkClipboardSort.Name = "chkClipboardSort"
        Me.chkClipboardSort.Size = New System.Drawing.Size(113, 17)
        Me.chkClipboardSort.TabIndex = 2
        Me.chkClipboardSort.Text = "&Sort Alphabetically"
        Me.chkClipboardSort.UseVisualStyleBackColor = True
        '
        'gbClipBoardTags
        '
        Me.gbClipBoardTags.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.gbClipBoardTags.Controls.Add(Me.lblClipboard)
        Me.gbClipBoardTags.Controls.Add(Me.cboClipboardPattern)
        Me.gbClipBoardTags.Location = New System.Drawing.Point(10, 10)
        Me.gbClipBoardTags.Name = "gbClipBoardTags"
        Me.gbClipBoardTags.Size = New System.Drawing.Size(706, 214)
        Me.gbClipBoardTags.TabIndex = 1
        Me.gbClipBoardTags.TabStop = False
        Me.gbClipBoardTags.Text = "Use the following pattern"
        '
        'lblClipboard
        '
        Me.lblClipboard.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblClipboard.Location = New System.Drawing.Point(6, 46)
        Me.lblClipboard.Multiline = True
        Me.lblClipboard.Name = "lblClipboard"
        Me.lblClipboard.ReadOnly = True
        Me.lblClipboard.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.lblClipboard.Size = New System.Drawing.Size(694, 162)
        Me.lblClipboard.TabIndex = 3
        '
        'cboClipboardPattern
        '
        Me.cboClipboardPattern.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboClipboardPattern.FormattingEnabled = True
        Me.cboClipboardPattern.Location = New System.Drawing.Point(6, 19)
        Me.cboClipboardPattern.Name = "cboClipboardPattern"
        Me.cboClipboardPattern.Size = New System.Drawing.Size(694, 21)
        Me.cboClipboardPattern.Sorted = True
        Me.cboClipboardPattern.TabIndex = 2
        '
        'btnClipboard
        '
        Me.btnClipboard.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClipboard.Location = New System.Drawing.Point(531, 230)
        Me.btnClipboard.Name = "btnClipboard"
        Me.btnClipboard.Size = New System.Drawing.Size(179, 23)
        Me.btnClipboard.TabIndex = 0
        Me.btnClipboard.Text = "Copy to Clipboard"
        Me.btnClipboard.UseVisualStyleBackColor = True
        '
        'tpSTCheat
        '
        Me.tpSTCheat.Controls.Add(Me.gbOffset)
        Me.tpSTCheat.Controls.Add(Me.gbOverride)
        Me.tpSTCheat.ImageKey = "bomb.png"
        Me.tpSTCheat.Location = New System.Drawing.Point(4, 23)
        Me.tpSTCheat.Name = "tpSTCheat"
        Me.tpSTCheat.Padding = New System.Windows.Forms.Padding(3)
        Me.tpSTCheat.Size = New System.Drawing.Size(739, 266)
        Me.tpSTCheat.TabIndex = 5
        Me.tpSTCheat.Text = "Cheat"
        Me.tpSTCheat.UseVisualStyleBackColor = True
        '
        'gbOffset
        '
        Me.gbOffset.Controls.Add(Me.nudOffsetTrackNum)
        Me.gbOffset.Controls.Add(Me.btnOffsetTrackNum)
        Me.gbOffset.Location = New System.Drawing.Point(11, 124)
        Me.gbOffset.Name = "gbOffset"
        Me.gbOffset.Size = New System.Drawing.Size(630, 56)
        Me.gbOffset.TabIndex = 6
        Me.gbOffset.TabStop = False
        Me.gbOffset.Text = "Offset"
        '
        'nudOffsetTrackNum
        '
        Me.nudOffsetTrackNum.Location = New System.Drawing.Point(149, 22)
        Me.nudOffsetTrackNum.Minimum = New Decimal(New Integer() {100, 0, 0, -2147483648})
        Me.nudOffsetTrackNum.Name = "nudOffsetTrackNum"
        Me.nudOffsetTrackNum.Size = New System.Drawing.Size(120, 20)
        Me.nudOffsetTrackNum.TabIndex = 1
        '
        'btnOffsetTrackNum
        '
        Me.btnOffsetTrackNum.Location = New System.Drawing.Point(9, 19)
        Me.btnOffsetTrackNum.Name = "btnOffsetTrackNum"
        Me.btnOffsetTrackNum.Size = New System.Drawing.Size(134, 23)
        Me.btnOffsetTrackNum.TabIndex = 0
        Me.btnOffsetTrackNum.Text = "&Offset Track Number by"
        Me.btnOffsetTrackNum.UseVisualStyleBackColor = True
        '
        'gbOverride
        '
        Me.gbOverride.Controls.Add(Me.nudRatingOverride)
        Me.gbOverride.Controls.Add(Me.chkRatingOverride)
        Me.gbOverride.Controls.Add(Me.nudPlayedCountOverride)
        Me.gbOverride.Controls.Add(Me.chkPlayedCountOverride)
        Me.gbOverride.Controls.Add(Me.btnOverride)
        Me.gbOverride.Controls.Add(Me.dtpPlayedDate)
        Me.gbOverride.Controls.Add(Me.chkPlayedDateOverride)
        Me.gbOverride.Location = New System.Drawing.Point(11, 13)
        Me.gbOverride.Name = "gbOverride"
        Me.gbOverride.Size = New System.Drawing.Size(630, 105)
        Me.gbOverride.TabIndex = 5
        Me.gbOverride.TabStop = False
        Me.gbOverride.Text = "Override"
        '
        'nudRatingOverride
        '
        Me.nudRatingOverride.Location = New System.Drawing.Point(182, 68)
        Me.nudRatingOverride.Minimum = New Decimal(New Integer() {100, 0, 0, -2147483648})
        Me.nudRatingOverride.Name = "nudRatingOverride"
        Me.nudRatingOverride.Size = New System.Drawing.Size(131, 20)
        Me.nudRatingOverride.TabIndex = 6
        '
        'chkRatingOverride
        '
        Me.chkRatingOverride.AutoSize = True
        Me.chkRatingOverride.Location = New System.Drawing.Point(9, 68)
        Me.chkRatingOverride.Name = "chkRatingOverride"
        Me.chkRatingOverride.Size = New System.Drawing.Size(121, 17)
        Me.chkRatingOverride.TabIndex = 5
        Me.chkRatingOverride.Text = "Increment Rating by"
        Me.chkRatingOverride.UseVisualStyleBackColor = True
        '
        'nudPlayedCountOverride
        '
        Me.nudPlayedCountOverride.Location = New System.Drawing.Point(182, 42)
        Me.nudPlayedCountOverride.Minimum = New Decimal(New Integer() {100, 0, 0, -2147483648})
        Me.nudPlayedCountOverride.Name = "nudPlayedCountOverride"
        Me.nudPlayedCountOverride.Size = New System.Drawing.Size(131, 20)
        Me.nudPlayedCountOverride.TabIndex = 4
        '
        'chkPlayedCountOverride
        '
        Me.chkPlayedCountOverride.AutoSize = True
        Me.chkPlayedCountOverride.Location = New System.Drawing.Point(9, 42)
        Me.chkPlayedCountOverride.Name = "chkPlayedCountOverride"
        Me.chkPlayedCountOverride.Size = New System.Drawing.Size(153, 17)
        Me.chkPlayedCountOverride.TabIndex = 3
        Me.chkPlayedCountOverride.Text = "Increment Played Count by"
        Me.chkPlayedCountOverride.UseVisualStyleBackColor = True
        '
        'btnOverride
        '
        Me.btnOverride.Location = New System.Drawing.Point(549, 76)
        Me.btnOverride.Name = "btnOverride"
        Me.btnOverride.Size = New System.Drawing.Size(75, 23)
        Me.btnOverride.TabIndex = 2
        Me.btnOverride.Text = "Override"
        Me.btnOverride.UseVisualStyleBackColor = True
        '
        'dtpPlayedDate
        '
        Me.dtpPlayedDate.CustomFormat = "yyyy-MM-dd HH:mm:ss"
        Me.dtpPlayedDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpPlayedDate.Location = New System.Drawing.Point(182, 16)
        Me.dtpPlayedDate.Name = "dtpPlayedDate"
        Me.dtpPlayedDate.ShowUpDown = True
        Me.dtpPlayedDate.Size = New System.Drawing.Size(131, 20)
        Me.dtpPlayedDate.TabIndex = 1
        Me.ttApp.SetToolTip(Me.dtpPlayedDate, "Date and Time you started playing the first track")
        '
        'chkPlayedDateOverride
        '
        Me.chkPlayedDateOverride.AutoSize = True
        Me.chkPlayedDateOverride.Location = New System.Drawing.Point(9, 19)
        Me.chkPlayedDateOverride.Name = "chkPlayedDateOverride"
        Me.chkPlayedDateOverride.Size = New System.Drawing.Size(167, 17)
        Me.chkPlayedDateOverride.TabIndex = 0
        Me.chkPlayedDateOverride.Text = "Time you played the first track"
        Me.chkPlayedDateOverride.UseVisualStyleBackColor = True
        '
        'tpSTExport
        '
        Me.tpSTExport.Controls.Add(Me.GroupBox1)
        Me.tpSTExport.Controls.Add(Me.GroupBox2)
        Me.tpSTExport.ImageKey = "folder_go.png"
        Me.tpSTExport.Location = New System.Drawing.Point(4, 23)
        Me.tpSTExport.Name = "tpSTExport"
        Me.tpSTExport.Padding = New System.Windows.Forms.Padding(3)
        Me.tpSTExport.Size = New System.Drawing.Size(739, 266)
        Me.tpSTExport.TabIndex = 6
        Me.tpSTExport.Text = "Export"
        Me.tpSTExport.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.btnArtworkExport)
        Me.GroupBox1.Location = New System.Drawing.Point(10, 100)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(690, 55)
        Me.GroupBox1.TabIndex = 4
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "iTunes Album Art (requires an iTunes Store account)"
        '
        'btnArtworkExport
        '
        Me.btnArtworkExport.AutoSize = True
        Me.btnArtworkExport.Location = New System.Drawing.Point(15, 19)
        Me.btnArtworkExport.Name = "btnArtworkExport"
        Me.btnArtworkExport.Size = New System.Drawing.Size(262, 23)
        Me.btnArtworkExport.TabIndex = 3
        Me.btnArtworkExport.Text = "&Export Artwork of Selected Track in iTunes Store..."
        Me.btnArtworkExport.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox2.Controls.Add(Me.chkExportArtwork)
        Me.GroupBox2.Controls.Add(Me.cboExportFilePattern)
        Me.GroupBox2.Controls.Add(Me.btnCopyTo)
        Me.GroupBox2.Location = New System.Drawing.Point(10, 10)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(690, 84)
        Me.GroupBox2.TabIndex = 2
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Export Tracks with the following File Pattern"
        '
        'chkExportArtwork
        '
        Me.chkExportArtwork.AutoSize = True
        Me.chkExportArtwork.Checked = Global.iTSfv.My.MySettings.Default.ExportTracksArtwork
        Me.chkExportArtwork.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "ExportTracksArtwork", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkExportArtwork.Location = New System.Drawing.Point(292, 53)
        Me.chkExportArtwork.Name = "chkExportArtwork"
        Me.chkExportArtwork.Size = New System.Drawing.Size(84, 17)
        Me.chkExportArtwork.TabIndex = 2
        Me.chkExportArtwork.Text = "with Artwork"
        Me.chkExportArtwork.UseVisualStyleBackColor = True
        '
        'cboExportFilePattern
        '
        Me.cboExportFilePattern.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboExportFilePattern.FormattingEnabled = True
        Me.cboExportFilePattern.Location = New System.Drawing.Point(15, 22)
        Me.cboExportFilePattern.Name = "cboExportFilePattern"
        Me.cboExportFilePattern.Size = New System.Drawing.Size(644, 21)
        Me.cboExportFilePattern.Sorted = True
        Me.cboExportFilePattern.TabIndex = 1
        '
        'btnCopyTo
        '
        Me.btnCopyTo.AutoSize = True
        Me.btnCopyTo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.btnCopyTo.Location = New System.Drawing.Point(15, 49)
        Me.btnCopyTo.Name = "btnCopyTo"
        Me.btnCopyTo.Size = New System.Drawing.Size(269, 23)
        Me.btnCopyTo.TabIndex = 0
        Me.btnCopyTo.Text = "&Browse Destination Folder to Copy Selected Tracks..."
        Me.btnCopyTo.UseVisualStyleBackColor = True
        '
        'tpExplorer
        '
        Me.tpExplorer.Controls.Add(Me.tcExplorer)
        Me.tpExplorer.ImageKey = "folder_explore.png"
        Me.tpExplorer.Location = New System.Drawing.Point(4, 23)
        Me.tpExplorer.Name = "tpExplorer"
        Me.tpExplorer.Padding = New System.Windows.Forms.Padding(3)
        Me.tpExplorer.Size = New System.Drawing.Size(753, 299)
        Me.tpExplorer.TabIndex = 3
        Me.tpExplorer.Text = "Explorer"
        Me.tpExplorer.UseVisualStyleBackColor = True
        '
        'tcExplorer
        '
        Me.tcExplorer.Controls.Add(Me.tpExplorerFiles)
        Me.tcExplorer.Controls.Add(Me.tpExplorerActivity)
        Me.tcExplorer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tcExplorer.Location = New System.Drawing.Point(3, 3)
        Me.tcExplorer.Name = "tcExplorer"
        Me.tcExplorer.SelectedIndex = 0
        Me.tcExplorer.Size = New System.Drawing.Size(747, 293)
        Me.tcExplorer.TabIndex = 18
        '
        'tpExplorerFiles
        '
        Me.tpExplorerFiles.Controls.Add(Me.chkReplaceWithNewKind)
        Me.tpExplorerFiles.Controls.Add(Me.chkValidate)
        Me.tpExplorerFiles.Controls.Add(Me.lbFiles)
        Me.tpExplorerFiles.Controls.Add(Me.chkAddFile)
        Me.tpExplorerFiles.Controls.Add(Me.btnFindNewFiles)
        Me.tpExplorerFiles.Controls.Add(Me.btnClearFilesListBox)
        Me.tpExplorerFiles.Location = New System.Drawing.Point(4, 22)
        Me.tpExplorerFiles.Name = "tpExplorerFiles"
        Me.tpExplorerFiles.Padding = New System.Windows.Forms.Padding(3)
        Me.tpExplorerFiles.Size = New System.Drawing.Size(739, 267)
        Me.tpExplorerFiles.TabIndex = 0
        Me.tpExplorerFiles.Text = "Files"
        Me.tpExplorerFiles.UseVisualStyleBackColor = True
        '
        'chkReplaceWithNewKind
        '
        Me.chkReplaceWithNewKind.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkReplaceWithNewKind.AutoSize = True
        Me.chkReplaceWithNewKind.Location = New System.Drawing.Point(472, 6)
        Me.chkReplaceWithNewKind.Name = "chkReplaceWithNewKind"
        Me.chkReplaceWithNewKind.Size = New System.Drawing.Size(255, 17)
        Me.chkReplaceWithNewKind.TabIndex = 2
        Me.chkReplaceWithNewKind.Text = "Replace track with identical but different kind file"
        Me.chkReplaceWithNewKind.UseVisualStyleBackColor = True
        '
        'chkValidate
        '
        Me.chkValidate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkValidate.AutoSize = True
        Me.chkValidate.Checked = Global.iTSfv.My.MySettings.Default.ValidateAfterAdding
        Me.chkValidate.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkValidate.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "ValidateAfterAdding", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkValidate.Location = New System.Drawing.Point(472, 52)
        Me.chkValidate.Name = "chkValidate"
        Me.chkValidate.Size = New System.Drawing.Size(236, 17)
        Me.chkValidate.TabIndex = 5
        Me.chkValidate.Text = "&Validate tracks after adding to iTunes Library"
        Me.chkValidate.UseVisualStyleBackColor = True
        '
        'lbFiles
        '
        Me.lbFiles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbFiles.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.lbFiles.ContextMenuStrip = Me.cmsFiles
        Me.lbFiles.FormattingEnabled = True
        Me.lbFiles.HorizontalScrollbar = True
        Me.lbFiles.Location = New System.Drawing.Point(6, 6)
        Me.lbFiles.Name = "lbFiles"
        Me.lbFiles.ScrollAlwaysVisible = True
        Me.lbFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lbFiles.Size = New System.Drawing.Size(451, 247)
        Me.lbFiles.Sorted = True
        Me.lbFiles.TabIndex = 0
        '
        'cmsFiles
        '
        Me.cmsFiles.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ShowInWindowsExplorerToolStripMenuItem, Me.RemoveFromListToolStripMenuItem})
        Me.cmsFiles.Name = "cmsFiles"
        Me.cmsFiles.Size = New System.Drawing.Size(223, 48)
        '
        'ShowInWindowsExplorerToolStripMenuItem
        '
        Me.ShowInWindowsExplorerToolStripMenuItem.Name = "ShowInWindowsExplorerToolStripMenuItem"
        Me.ShowInWindowsExplorerToolStripMenuItem.Size = New System.Drawing.Size(222, 22)
        Me.ShowInWindowsExplorerToolStripMenuItem.Text = "&Show in Windows Explorer..."
        '
        'RemoveFromListToolStripMenuItem
        '
        Me.RemoveFromListToolStripMenuItem.Name = "RemoveFromListToolStripMenuItem"
        Me.RemoveFromListToolStripMenuItem.Size = New System.Drawing.Size(222, 22)
        Me.RemoveFromListToolStripMenuItem.Text = "&Remove from List"
        '
        'chkAddFile
        '
        Me.chkAddFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkAddFile.AutoSize = True
        Me.chkAddFile.Checked = Global.iTSfv.My.MySettings.Default.AddNewFilesAfterScan
        Me.chkAddFile.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkAddFile.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "AddNewFilesAfterScan", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkAddFile.Location = New System.Drawing.Point(472, 29)
        Me.chkAddFile.Name = "chkAddFile"
        Me.chkAddFile.Size = New System.Drawing.Size(253, 17)
        Me.chkAddFile.TabIndex = 3
        Me.chkAddFile.Text = "&Add new tracks to iTunes after scan is complete"
        Me.ttApp.SetToolTip(Me.chkAddFile, "Add new tracks to iTunes after scan is complete")
        Me.chkAddFile.UseVisualStyleBackColor = True
        '
        'btnFindNewFiles
        '
        Me.btnFindNewFiles.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnFindNewFiles.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.btnFindNewFiles.Image = Global.iTSfv.My.Resources.Resources.folder_find
        Me.btnFindNewFiles.Location = New System.Drawing.Point(472, 109)
        Me.btnFindNewFiles.Name = "btnFindNewFiles"
        Me.btnFindNewFiles.Size = New System.Drawing.Size(238, 23)
        Me.btnFindNewFiles.TabIndex = 1
        Me.btnFindNewFiles.Text = "Find New Tracks not in iTunes Library"
        Me.btnFindNewFiles.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.btnFindNewFiles.UseVisualStyleBackColor = True
        '
        'btnClearFilesListBox
        '
        Me.btnClearFilesListBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClearFilesListBox.Location = New System.Drawing.Point(472, 138)
        Me.btnClearFilesListBox.Name = "btnClearFilesListBox"
        Me.btnClearFilesListBox.Size = New System.Drawing.Size(238, 23)
        Me.btnClearFilesListBox.TabIndex = 4
        Me.btnClearFilesListBox.Text = "&Clear Found / Watched Files List"
        Me.btnClearFilesListBox.UseVisualStyleBackColor = True
        '
        'tpExplorerActivity
        '
        Me.tpExplorerActivity.Controls.Add(Me.txtActivity)
        Me.tpExplorerActivity.Location = New System.Drawing.Point(4, 22)
        Me.tpExplorerActivity.Name = "tpExplorerActivity"
        Me.tpExplorerActivity.Padding = New System.Windows.Forms.Padding(3)
        Me.tpExplorerActivity.Size = New System.Drawing.Size(739, 267)
        Me.tpExplorerActivity.TabIndex = 1
        Me.tpExplorerActivity.Text = "Activity"
        Me.tpExplorerActivity.UseVisualStyleBackColor = True
        '
        'txtActivity
        '
        Me.txtActivity.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtActivity.Location = New System.Drawing.Point(3, 3)
        Me.txtActivity.Multiline = True
        Me.txtActivity.Name = "txtActivity"
        Me.txtActivity.ReadOnly = True
        Me.txtActivity.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtActivity.Size = New System.Drawing.Size(733, 261)
        Me.txtActivity.TabIndex = 0
        '
        'tpDiscsBrowser
        '
        Me.tpDiscsBrowser.Controls.Add(Me.btnBrowseAlbum)
        Me.tpDiscsBrowser.Controls.Add(Me.btnValidateAlbum)
        Me.tpDiscsBrowser.Controls.Add(Me.btnCreatePlaylistAlbum)
        Me.tpDiscsBrowser.Controls.Add(Me.lbDiscs)
        Me.tpDiscsBrowser.Controls.Add(Me.pbArtwork)
        Me.tpDiscsBrowser.ImageKey = "cd.png"
        Me.tpDiscsBrowser.Location = New System.Drawing.Point(4, 23)
        Me.tpDiscsBrowser.Name = "tpDiscsBrowser"
        Me.tpDiscsBrowser.Padding = New System.Windows.Forms.Padding(3)
        Me.tpDiscsBrowser.Size = New System.Drawing.Size(753, 299)
        Me.tpDiscsBrowser.TabIndex = 4
        Me.tpDiscsBrowser.Text = "Discs Browser"
        Me.tpDiscsBrowser.UseVisualStyleBackColor = True
        '
        'btnBrowseAlbum
        '
        Me.btnBrowseAlbum.Location = New System.Drawing.Point(6, 141)
        Me.btnBrowseAlbum.Name = "btnBrowseAlbum"
        Me.btnBrowseAlbum.Size = New System.Drawing.Size(100, 23)
        Me.btnBrowseAlbum.TabIndex = 6
        Me.btnBrowseAlbum.Text = "&Browse Disc..."
        Me.btnBrowseAlbum.UseVisualStyleBackColor = True
        '
        'btnValidateAlbum
        '
        Me.btnValidateAlbum.Location = New System.Drawing.Point(6, 112)
        Me.btnValidateAlbum.Name = "btnValidateAlbum"
        Me.btnValidateAlbum.Size = New System.Drawing.Size(100, 23)
        Me.btnValidateAlbum.TabIndex = 5
        Me.btnValidateAlbum.Text = "&Validate Disc"
        Me.btnValidateAlbum.UseVisualStyleBackColor = True
        '
        'btnCreatePlaylistAlbum
        '
        Me.btnCreatePlaylistAlbum.Location = New System.Drawing.Point(6, 169)
        Me.btnCreatePlaylistAlbum.Name = "btnCreatePlaylistAlbum"
        Me.btnCreatePlaylistAlbum.Size = New System.Drawing.Size(100, 23)
        Me.btnCreatePlaylistAlbum.TabIndex = 1
        Me.btnCreatePlaylistAlbum.Text = "&Create Playlist..."
        Me.btnCreatePlaylistAlbum.UseVisualStyleBackColor = True
        '
        'lbDiscs
        '
        Me.lbDiscs.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbDiscs.ContextMenuStrip = Me.cmsDiscs
        Me.lbDiscs.FormattingEnabled = True
        Me.lbDiscs.HorizontalScrollbar = True
        Me.lbDiscs.Location = New System.Drawing.Point(112, 6)
        Me.lbDiscs.Name = "lbDiscs"
        Me.lbDiscs.Size = New System.Drawing.Size(632, 277)
        Me.lbDiscs.Sorted = True
        Me.lbDiscs.TabIndex = 0
        '
        'cmsDiscs
        '
        Me.cmsDiscs.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PlayDiscInITunesToolStripMenuItem, Me.ValidateDiscToolStripMenuItem, Me.ToolStripSeparator16, Me.CreatePlaylistToolStripMenuItem, Me.ShowDiscInWindowsExplroerToolStripMenuItem, Me.tsmCopyTracklist, Me.ToolStripSeparator24, Me.ArtworkSearchToolStripMenuItem, Me.GoogleSearchToolStripMenuItem, Me.Mp3tagSelectedDisc})
        Me.cmsDiscs.Name = "cDiscs"
        Me.cmsDiscs.Size = New System.Drawing.Size(223, 192)
        '
        'PlayDiscInITunesToolStripMenuItem
        '
        Me.PlayDiscInITunesToolStripMenuItem.Image = Global.iTSfv.My.Resources.Resources.control_play
        Me.PlayDiscInITunesToolStripMenuItem.Name = "PlayDiscInITunesToolStripMenuItem"
        Me.PlayDiscInITunesToolStripMenuItem.Size = New System.Drawing.Size(222, 22)
        Me.PlayDiscInITunesToolStripMenuItem.Text = "&Play Disc in iTunes"
        '
        'ValidateDiscToolStripMenuItem
        '
        Me.ValidateDiscToolStripMenuItem.Name = "ValidateDiscToolStripMenuItem"
        Me.ValidateDiscToolStripMenuItem.Size = New System.Drawing.Size(222, 22)
        Me.ValidateDiscToolStripMenuItem.Text = "&Validate Disc"
        '
        'ToolStripSeparator16
        '
        Me.ToolStripSeparator16.Name = "ToolStripSeparator16"
        Me.ToolStripSeparator16.Size = New System.Drawing.Size(219, 6)
        '
        'CreatePlaylistToolStripMenuItem
        '
        Me.CreatePlaylistToolStripMenuItem.Name = "CreatePlaylistToolStripMenuItem"
        Me.CreatePlaylistToolStripMenuItem.Size = New System.Drawing.Size(222, 22)
        Me.CreatePlaylistToolStripMenuItem.Text = "&Create Playlist..."
        '
        'ShowDiscInWindowsExplroerToolStripMenuItem
        '
        Me.ShowDiscInWindowsExplroerToolStripMenuItem.Image = Global.iTSfv.My.Resources.Resources.folder_explore
        Me.ShowDiscInWindowsExplroerToolStripMenuItem.Name = "ShowDiscInWindowsExplroerToolStripMenuItem"
        Me.ShowDiscInWindowsExplroerToolStripMenuItem.Size = New System.Drawing.Size(222, 22)
        Me.ShowDiscInWindowsExplroerToolStripMenuItem.Text = "&Show in Windows Explorer..."
        '
        'tsmCopyTracklist
        '
        Me.tsmCopyTracklist.Image = Global.iTSfv.My.Resources.Resources.page_copy
        Me.tsmCopyTracklist.Name = "tsmCopyTracklist"
        Me.tsmCopyTracklist.Size = New System.Drawing.Size(222, 22)
        Me.tsmCopyTracklist.Text = "Copy &Tracklist to Clipboard"
        '
        'ToolStripSeparator24
        '
        Me.ToolStripSeparator24.Name = "ToolStripSeparator24"
        Me.ToolStripSeparator24.Size = New System.Drawing.Size(219, 6)
        '
        'ArtworkSearchToolStripMenuItem
        '
        Me.ArtworkSearchToolStripMenuItem.Name = "ArtworkSearchToolStripMenuItem"
        Me.ArtworkSearchToolStripMenuItem.Size = New System.Drawing.Size(222, 22)
        Me.ArtworkSearchToolStripMenuItem.Text = "&Artwork Search using AAD..."
        '
        'GoogleSearchToolStripMenuItem
        '
        Me.GoogleSearchToolStripMenuItem.Name = "GoogleSearchToolStripMenuItem"
        Me.GoogleSearchToolStripMenuItem.Size = New System.Drawing.Size(222, 22)
        Me.GoogleSearchToolStripMenuItem.Text = "&Google this Disc..."
        '
        'Mp3tagSelectedDisc
        '
        Me.Mp3tagSelectedDisc.Image = Global.iTSfv.My.Resources.Resources.mp3tag
        Me.Mp3tagSelectedDisc.Name = "Mp3tagSelectedDisc"
        Me.Mp3tagSelectedDisc.Size = New System.Drawing.Size(222, 22)
        Me.Mp3tagSelectedDisc.Text = "Mp3tag..."
        '
        'pbArtwork
        '
        Me.pbArtwork.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pbArtwork.Location = New System.Drawing.Point(6, 6)
        Me.pbArtwork.Name = "pbArtwork"
        Me.pbArtwork.Size = New System.Drawing.Size(100, 100)
        Me.pbArtwork.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pbArtwork.TabIndex = 2
        Me.pbArtwork.TabStop = False
        '
        'tpBackupRestore
        '
        Me.tpBackupRestore.Controls.Add(Me.tcTags)
        Me.tpBackupRestore.ImageKey = "tag_blue.png"
        Me.tpBackupRestore.Location = New System.Drawing.Point(4, 23)
        Me.tpBackupRestore.Name = "tpBackupRestore"
        Me.tpBackupRestore.Padding = New System.Windows.Forms.Padding(3)
        Me.tpBackupRestore.Size = New System.Drawing.Size(753, 299)
        Me.tpBackupRestore.TabIndex = 5
        Me.tpBackupRestore.Text = "Tags"
        Me.tpBackupRestore.UseVisualStyleBackColor = True
        '
        'tcTags
        '
        Me.tcTags.Controls.Add(Me.tpTagsBackupRestore)
        Me.tcTags.Controls.Add(Me.tpTagsRecover)
        Me.tcTags.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tcTags.Location = New System.Drawing.Point(3, 3)
        Me.tcTags.Name = "tcTags"
        Me.tcTags.SelectedIndex = 0
        Me.tcTags.Size = New System.Drawing.Size(747, 293)
        Me.tcTags.TabIndex = 18
        '
        'tpTagsBackupRestore
        '
        Me.tpTagsBackupRestore.Controls.Add(Me.gbBackupTags)
        Me.tpTagsBackupRestore.Controls.Add(Me.gbRestoreTags)
        Me.tpTagsBackupRestore.Location = New System.Drawing.Point(4, 22)
        Me.tpTagsBackupRestore.Name = "tpTagsBackupRestore"
        Me.tpTagsBackupRestore.Padding = New System.Windows.Forms.Padding(3)
        Me.tpTagsBackupRestore.Size = New System.Drawing.Size(739, 267)
        Me.tpTagsBackupRestore.TabIndex = 0
        Me.tpTagsBackupRestore.Text = "Backup/Restore"
        Me.tpTagsBackupRestore.UseVisualStyleBackColor = True
        '
        'gbBackupTags
        '
        Me.gbBackupTags.Controls.Add(Me.rbSelectedTracks)
        Me.gbBackupTags.Controls.Add(Me.btnRatingsBackup)
        Me.gbBackupTags.Controls.Add(Me.rbLibrary)
        Me.gbBackupTags.Controls.Add(Me.txtRatingsBackupPath)
        Me.gbBackupTags.Location = New System.Drawing.Point(10, 10)
        Me.gbBackupTags.Name = "gbBackupTags"
        Me.gbBackupTags.Size = New System.Drawing.Size(568, 74)
        Me.gbBackupTags.TabIndex = 3
        Me.gbBackupTags.TabStop = False
        Me.gbBackupTags.Text = "Backup Tags : Browse for a XML file to backup tags"
        '
        'rbSelectedTracks
        '
        Me.rbSelectedTracks.AutoSize = True
        Me.rbSelectedTracks.Checked = True
        Me.rbSelectedTracks.Location = New System.Drawing.Point(165, 21)
        Me.rbSelectedTracks.Name = "rbSelectedTracks"
        Me.rbSelectedTracks.Size = New System.Drawing.Size(179, 17)
        Me.rbSelectedTracks.TabIndex = 1
        Me.rbSelectedTracks.TabStop = True
        Me.rbSelectedTracks.Text = "Selected Tracks in Music Library"
        Me.rbSelectedTracks.UseVisualStyleBackColor = True
        '
        'btnRatingsBackup
        '
        Me.btnRatingsBackup.Location = New System.Drawing.Point(474, 41)
        Me.btnRatingsBackup.Name = "btnRatingsBackup"
        Me.btnRatingsBackup.Size = New System.Drawing.Size(75, 23)
        Me.btnRatingsBackup.TabIndex = 5
        Me.btnRatingsBackup.Text = "&Backup..."
        Me.btnRatingsBackup.UseVisualStyleBackColor = True
        '
        'rbLibrary
        '
        Me.rbLibrary.AutoSize = True
        Me.rbLibrary.Location = New System.Drawing.Point(6, 21)
        Me.rbLibrary.Name = "rbLibrary"
        Me.rbLibrary.Size = New System.Drawing.Size(87, 17)
        Me.rbLibrary.TabIndex = 0
        Me.rbLibrary.Text = "Music Library"
        Me.rbLibrary.UseVisualStyleBackColor = True
        '
        'txtRatingsBackupPath
        '
        Me.txtRatingsBackupPath.Location = New System.Drawing.Point(6, 44)
        Me.txtRatingsBackupPath.Name = "txtRatingsBackupPath"
        Me.txtRatingsBackupPath.ReadOnly = True
        Me.txtRatingsBackupPath.Size = New System.Drawing.Size(462, 20)
        Me.txtRatingsBackupPath.TabIndex = 4
        '
        'gbRestoreTags
        '
        Me.gbRestoreTags.Controls.Add(Me.txtRatingsRestorePath)
        Me.gbRestoreTags.Controls.Add(Me.btnRatingsRestore)
        Me.gbRestoreTags.Location = New System.Drawing.Point(10, 90)
        Me.gbRestoreTags.Name = "gbRestoreTags"
        Me.gbRestoreTags.Size = New System.Drawing.Size(568, 58)
        Me.gbRestoreTags.TabIndex = 2
        Me.gbRestoreTags.TabStop = False
        Me.gbRestoreTags.Text = "Restore Tags : Browse for a XML file to restore tags"
        '
        'txtRatingsRestorePath
        '
        Me.txtRatingsRestorePath.Location = New System.Drawing.Point(6, 21)
        Me.txtRatingsRestorePath.Name = "txtRatingsRestorePath"
        Me.txtRatingsRestorePath.ReadOnly = True
        Me.txtRatingsRestorePath.Size = New System.Drawing.Size(462, 20)
        Me.txtRatingsRestorePath.TabIndex = 2
        '
        'btnRatingsRestore
        '
        Me.btnRatingsRestore.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.btnRatingsRestore.Location = New System.Drawing.Point(474, 18)
        Me.btnRatingsRestore.Name = "btnRatingsRestore"
        Me.btnRatingsRestore.Size = New System.Drawing.Size(75, 23)
        Me.btnRatingsRestore.TabIndex = 1
        Me.btnRatingsRestore.Text = "&Restore..."
        Me.btnRatingsRestore.UseVisualStyleBackColor = True
        '
        'tpTagsRecover
        '
        Me.tpTagsRecover.Controls.Add(Me.gbBrowsePrevLib)
        Me.tpTagsRecover.Location = New System.Drawing.Point(4, 22)
        Me.tpTagsRecover.Name = "tpTagsRecover"
        Me.tpTagsRecover.Padding = New System.Windows.Forms.Padding(3)
        Me.tpTagsRecover.Size = New System.Drawing.Size(739, 267)
        Me.tpTagsRecover.TabIndex = 1
        Me.tpTagsRecover.Text = "Recover"
        Me.tpTagsRecover.UseVisualStyleBackColor = True
        '
        'gbBrowsePrevLib
        '
        Me.gbBrowsePrevLib.Controls.Add(Me.txtXmlLibPath)
        Me.gbBrowsePrevLib.Controls.Add(Me.btnRecover)
        Me.gbBrowsePrevLib.Location = New System.Drawing.Point(10, 10)
        Me.gbBrowsePrevLib.Name = "gbBrowsePrevLib"
        Me.gbBrowsePrevLib.Size = New System.Drawing.Size(597, 49)
        Me.gbBrowsePrevLib.TabIndex = 2
        Me.gbBrowsePrevLib.TabStop = False
        Me.gbBrowsePrevLib.Text = "Recover Tags : Browse for a previous iTunes Music Library.xml to recover tags"
        '
        'txtXmlLibPath
        '
        Me.txtXmlLibPath.Location = New System.Drawing.Point(6, 19)
        Me.txtXmlLibPath.Name = "txtXmlLibPath"
        Me.txtXmlLibPath.ReadOnly = True
        Me.txtXmlLibPath.Size = New System.Drawing.Size(494, 20)
        Me.txtXmlLibPath.TabIndex = 1
        '
        'btnRecover
        '
        Me.btnRecover.Location = New System.Drawing.Point(506, 16)
        Me.btnRecover.Name = "btnRecover"
        Me.btnRecover.Size = New System.Drawing.Size(75, 23)
        Me.btnRecover.TabIndex = 9
        Me.btnRecover.Text = "&Recover..."
        Me.btnRecover.UseVisualStyleBackColor = True
        '
        'tpOneTouch
        '
        Me.tpOneTouch.Controls.Add(Me.tcOneTouch)
        Me.tpOneTouch.ImageKey = "wand.png"
        Me.tpOneTouch.Location = New System.Drawing.Point(4, 23)
        Me.tpOneTouch.Name = "tpOneTouch"
        Me.tpOneTouch.Padding = New System.Windows.Forms.Padding(3)
        Me.tpOneTouch.Size = New System.Drawing.Size(753, 299)
        Me.tpOneTouch.TabIndex = 6
        Me.tpOneTouch.Text = "OneTouch"
        Me.tpOneTouch.UseVisualStyleBackColor = True
        '
        'tcOneTouch
        '
        Me.tcOneTouch.Controls.Add(Me.tpAdvGeneral)
        Me.tcOneTouch.Controls.Add(Me.tpAdvTracks)
        Me.tcOneTouch.Controls.Add(Me.tpAdvLibrary)
        Me.tcOneTouch.Controls.Add(Me.tpAdvFilesystem)
        Me.tcOneTouch.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tcOneTouch.ImageList = Me.ilTabs
        Me.tcOneTouch.Location = New System.Drawing.Point(3, 3)
        Me.tcOneTouch.Name = "tcOneTouch"
        Me.tcOneTouch.SelectedIndex = 0
        Me.tcOneTouch.Size = New System.Drawing.Size(747, 293)
        Me.tcOneTouch.TabIndex = 11
        '
        'tpAdvGeneral
        '
        Me.tpAdvGeneral.Controls.Add(Me.chkResume)
        Me.tpAdvGeneral.Controls.Add(Me.btnValidateLibrary)
        Me.tpAdvGeneral.Controls.Add(Me.btnSynchroclean)
        Me.tpAdvGeneral.Location = New System.Drawing.Point(4, 23)
        Me.tpAdvGeneral.Name = "tpAdvGeneral"
        Me.tpAdvGeneral.Padding = New System.Windows.Forms.Padding(3)
        Me.tpAdvGeneral.Size = New System.Drawing.Size(739, 266)
        Me.tpAdvGeneral.TabIndex = 0
        Me.tpAdvGeneral.Text = "General"
        Me.tpAdvGeneral.UseVisualStyleBackColor = True
        '
        'btnSynchroclean
        '
        Me.btnSynchroclean.Image = Global.iTSfv.My.Resources.Resources.database_refresh
        Me.btnSynchroclean.Location = New System.Drawing.Point(10, 10)
        Me.btnSynchroclean.Name = "btnSynchroclean"
        Me.btnSynchroclean.Size = New System.Drawing.Size(519, 23)
        Me.btnSynchroclean.TabIndex = 5
        Me.btnSynchroclean.Text = "Synchroclean® - clean Music library from invalid tracks and synchronize with Musi" & _
            "c folder"
        Me.btnSynchroclean.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.btnSynchroclean.UseVisualStyleBackColor = True
        '
        'tpAdvTracks
        '
        Me.tpAdvTracks.Controls.Add(Me.btnWritePOPM)
        Me.tpAdvTracks.Controls.Add(Me.btnReplaceTracks)
        Me.tpAdvTracks.ImageIndex = 3
        Me.tpAdvTracks.Location = New System.Drawing.Point(4, 23)
        Me.tpAdvTracks.Name = "tpAdvTracks"
        Me.tpAdvTracks.Padding = New System.Windows.Forms.Padding(3)
        Me.tpAdvTracks.Size = New System.Drawing.Size(739, 266)
        Me.tpAdvTracks.TabIndex = 3
        Me.tpAdvTracks.Text = "Tracks"
        Me.tpAdvTracks.UseVisualStyleBackColor = True
        '
        'btnWritePOPM
        '
        Me.btnWritePOPM.AutoSize = True
        Me.btnWritePOPM.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnWritePOPM.ImageIndex = 16
        Me.btnWritePOPM.ImageList = Me.ilTabs
        Me.btnWritePOPM.Location = New System.Drawing.Point(10, 39)
        Me.btnWritePOPM.Name = "btnWritePOPM"
        Me.btnWritePOPM.Size = New System.Drawing.Size(366, 23)
        Me.btnWritePOPM.TabIndex = 10
        Me.btnWritePOPM.Text = "Write PlayedCount and Rating data to PCNT and POPM Frames"
        Me.btnWritePOPM.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnWritePOPM.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.btnWritePOPM.UseVisualStyleBackColor = True
        '
        'btnReplaceTracks
        '
        Me.btnReplaceTracks.AutoSize = True
        Me.btnReplaceTracks.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnReplaceTracks.ImageIndex = 17
        Me.btnReplaceTracks.ImageList = Me.ilTabs
        Me.btnReplaceTracks.Location = New System.Drawing.Point(10, 10)
        Me.btnReplaceTracks.Name = "btnReplaceTracks"
        Me.btnReplaceTracks.Size = New System.Drawing.Size(519, 23)
        Me.btnReplaceTracks.TabIndex = 9
        Me.btnReplaceTracks.Text = "Replace Tracks"
        Me.btnReplaceTracks.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnReplaceTracks.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.btnReplaceTracks.UseVisualStyleBackColor = True
        '
        'tpAdvLibrary
        '
        Me.tpAdvLibrary.Controls.Add(Me.btnImportPOPM)
        Me.tpAdvLibrary.Controls.Add(Me.btnAdjustRatings)
        Me.tpAdvLibrary.Controls.Add(Me.btnRemoveDuplicates)
        Me.tpAdvLibrary.Controls.Add(Me.btnSyncLastFM)
        Me.tpAdvLibrary.ImageKey = "database_edit.png"
        Me.tpAdvLibrary.Location = New System.Drawing.Point(4, 23)
        Me.tpAdvLibrary.Name = "tpAdvLibrary"
        Me.tpAdvLibrary.Padding = New System.Windows.Forms.Padding(3)
        Me.tpAdvLibrary.Size = New System.Drawing.Size(739, 266)
        Me.tpAdvLibrary.TabIndex = 1
        Me.tpAdvLibrary.Text = "Library"
        Me.tpAdvLibrary.UseVisualStyleBackColor = True
        '
        'btnImportPOPM
        '
        Me.btnImportPOPM.AutoSize = True
        Me.btnImportPOPM.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnImportPOPM.ImageKey = "heart_add.png"
        Me.btnImportPOPM.ImageList = Me.ilTabs
        Me.btnImportPOPM.Location = New System.Drawing.Point(10, 97)
        Me.btnImportPOPM.Name = "btnImportPOPM"
        Me.btnImportPOPM.Size = New System.Drawing.Size(476, 23)
        Me.btnImportPOPM.TabIndex = 11
        Me.btnImportPOPM.Text = "Import Rating and PlayedCount data from POPM and PCNT Frames"
        Me.btnImportPOPM.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnImportPOPM.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.btnImportPOPM.UseVisualStyleBackColor = True
        '
        'btnAdjustRatings
        '
        Me.btnAdjustRatings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.btnAdjustRatings.Image = Global.iTSfv.My.Resources.Resources.star
        Me.btnAdjustRatings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnAdjustRatings.Location = New System.Drawing.Point(10, 10)
        Me.btnAdjustRatings.Name = "btnAdjustRatings"
        Me.btnAdjustRatings.Size = New System.Drawing.Size(476, 23)
        Me.btnAdjustRatings.TabIndex = 4
        Me.btnAdjustRatings.Text = "Adjust Ratings"
        Me.btnAdjustRatings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.btnAdjustRatings.UseVisualStyleBackColor = True
        '
        'btnRemoveDuplicates
        '
        Me.btnRemoveDuplicates.AutoSize = True
        Me.btnRemoveDuplicates.Image = Global.iTSfv.My.Resources.Resources.database_delete
        Me.btnRemoveDuplicates.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnRemoveDuplicates.Location = New System.Drawing.Point(10, 39)
        Me.btnRemoveDuplicates.Name = "btnRemoveDuplicates"
        Me.btnRemoveDuplicates.Size = New System.Drawing.Size(476, 23)
        Me.btnRemoveDuplicates.TabIndex = 0
        Me.btnRemoveDuplicates.Text = "&Remove Duplicate Songs from Library (press F2 for more options)"
        Me.btnRemoveDuplicates.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.btnRemoveDuplicates.UseVisualStyleBackColor = True
        '
        'btnSyncLastFM
        '
        Me.btnSyncLastFM.Image = Global.iTSfv.My.Resources.Resources.LastFM
        Me.btnSyncLastFM.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSyncLastFM.Location = New System.Drawing.Point(10, 68)
        Me.btnSyncLastFM.Name = "btnSyncLastFM"
        Me.btnSyncLastFM.Size = New System.Drawing.Size(476, 23)
        Me.btnSyncLastFM.TabIndex = 10
        Me.btnSyncLastFM.Text = "ReverseScrobble® - Update Track PlayedCount in iTunes Music Library using Last.fm" & _
            " Profile"
        Me.btnSyncLastFM.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.btnSyncLastFM.UseVisualStyleBackColor = True
        '
        'tpAdvFilesystem
        '
        Me.tpAdvFilesystem.Controls.Add(Me.btnBatchArtworkGrab)
        Me.tpAdvFilesystem.Controls.Add(Me.btnAdvDeleteEmptyFolders)
        Me.tpAdvFilesystem.ImageKey = "folder_edit.png"
        Me.tpAdvFilesystem.Location = New System.Drawing.Point(4, 23)
        Me.tpAdvFilesystem.Name = "tpAdvFilesystem"
        Me.tpAdvFilesystem.Padding = New System.Windows.Forms.Padding(3)
        Me.tpAdvFilesystem.Size = New System.Drawing.Size(739, 266)
        Me.tpAdvFilesystem.TabIndex = 4
        Me.tpAdvFilesystem.Text = "File System"
        Me.tpAdvFilesystem.UseVisualStyleBackColor = True
        '
        'btnBatchArtworkGrab
        '
        Me.btnBatchArtworkGrab.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnBatchArtworkGrab.ImageIndex = 18
        Me.btnBatchArtworkGrab.ImageList = Me.ilTabs
        Me.btnBatchArtworkGrab.Location = New System.Drawing.Point(10, 39)
        Me.btnBatchArtworkGrab.Name = "btnBatchArtworkGrab"
        Me.btnBatchArtworkGrab.Size = New System.Drawing.Size(368, 23)
        Me.btnBatchArtworkGrab.TabIndex = 1
        Me.btnBatchArtworkGrab.Text = "&Export iTunes Store Artwork using an External Music Folder..."
        Me.btnBatchArtworkGrab.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.btnBatchArtworkGrab.UseVisualStyleBackColor = True
        '
        'btnAdvDeleteEmptyFolders
        '
        Me.btnAdvDeleteEmptyFolders.AutoSize = True
        Me.btnAdvDeleteEmptyFolders.Image = Global.iTSfv.My.Resources.Resources.folder_delete
        Me.btnAdvDeleteEmptyFolders.Location = New System.Drawing.Point(10, 10)
        Me.btnAdvDeleteEmptyFolders.Name = "btnAdvDeleteEmptyFolders"
        Me.btnAdvDeleteEmptyFolders.Size = New System.Drawing.Size(368, 23)
        Me.btnAdvDeleteEmptyFolders.TabIndex = 0
        Me.btnAdvDeleteEmptyFolders.Text = "&Delete Empty folders inside Music folders... (press F2 for more options)"
        Me.btnAdvDeleteEmptyFolders.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.btnAdvDeleteEmptyFolders.UseVisualStyleBackColor = True
        '
        'tpSchedule
        '
        Me.tpSchedule.Controls.Add(Me.btnSchRun)
        Me.tpSchedule.Controls.Add(Me.chkScheduleFindNewFilesHDD)
        Me.tpSchedule.Controls.Add(Me.chkSheduleAdjustRating)
        Me.tpSchedule.Controls.Add(Me.chkSchValidateLibrary)
        Me.tpSchedule.ImageKey = "clock.png"
        Me.tpSchedule.Location = New System.Drawing.Point(4, 23)
        Me.tpSchedule.Name = "tpSchedule"
        Me.tpSchedule.Padding = New System.Windows.Forms.Padding(3)
        Me.tpSchedule.Size = New System.Drawing.Size(753, 299)
        Me.tpSchedule.TabIndex = 7
        Me.tpSchedule.Text = "Scheduled Tasks"
        Me.tpSchedule.UseVisualStyleBackColor = True
        '
        'btnSchRun
        '
        Me.btnSchRun.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSchRun.AutoSize = True
        Me.btnSchRun.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.btnSchRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSchRun.Location = New System.Drawing.Point(589, 258)
        Me.btnSchRun.Name = "btnSchRun"
        Me.btnSchRun.Size = New System.Drawing.Size(150, 25)
        Me.btnSchRun.TabIndex = 3
        Me.btnSchRun.Text = "&Run Scheduled Tasks Now"
        Me.btnSchRun.UseVisualStyleBackColor = True
        '
        'chkScheduleFindNewFilesHDD
        '
        Me.chkScheduleFindNewFilesHDD.AutoSize = True
        Me.chkScheduleFindNewFilesHDD.Location = New System.Drawing.Point(15, 38)
        Me.chkScheduleFindNewFilesHDD.Name = "chkScheduleFindNewFilesHDD"
        Me.chkScheduleFindNewFilesHDD.Size = New System.Drawing.Size(140, 17)
        Me.chkScheduleFindNewFilesHDD.TabIndex = 1
        Me.chkScheduleFindNewFilesHDD.Text = "Find new files from HDD"
        Me.chkScheduleFindNewFilesHDD.UseVisualStyleBackColor = True
        '
        'chkSheduleAdjustRating
        '
        Me.chkSheduleAdjustRating.AutoSize = True
        Me.chkSheduleAdjustRating.Location = New System.Drawing.Point(15, 15)
        Me.chkSheduleAdjustRating.Name = "chkSheduleAdjustRating"
        Me.chkSheduleAdjustRating.Size = New System.Drawing.Size(89, 17)
        Me.chkSheduleAdjustRating.TabIndex = 0
        Me.chkSheduleAdjustRating.Text = "Adjust Rating"
        Me.chkSheduleAdjustRating.UseVisualStyleBackColor = True
        '
        'chkSchValidateLibrary
        '
        Me.chkSchValidateLibrary.AutoSize = True
        Me.chkSchValidateLibrary.Checked = Global.iTSfv.My.MySettings.Default.SchValidateLibrary
        Me.chkSchValidateLibrary.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.iTSfv.My.MySettings.Default, "SchValidateLibrary", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.chkSchValidateLibrary.Location = New System.Drawing.Point(15, 61)
        Me.chkSchValidateLibrary.Name = "chkSchValidateLibrary"
        Me.chkSchValidateLibrary.Size = New System.Drawing.Size(164, 17)
        Me.chkSchValidateLibrary.TabIndex = 2
        Me.chkSchValidateLibrary.Text = "&Validate iTunes Music Library"
        Me.chkSchValidateLibrary.UseVisualStyleBackColor = True
        '
        'btnValidateSelected
        '
        Me.btnValidateSelected.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnValidateSelected.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.btnValidateSelected.Cursor = System.Windows.Forms.Cursors.Default
        Me.btnValidateSelected.Enabled = False
        Me.btnValidateSelected.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.btnValidateSelected.Location = New System.Drawing.Point(393, 6)
        Me.btnValidateSelected.Name = "btnValidateSelected"
        Me.btnValidateSelected.Size = New System.Drawing.Size(140, 22)
        Me.btnValidateSelected.TabIndex = 0
        Me.btnValidateSelected.Text = "Validate Selected Tracks"
        Me.ttApp.SetToolTip(Me.btnValidateSelected, "Ideally you should select all the tracks that consist the whole album")
        Me.btnValidateSelected.UseVisualStyleBackColor = True
        '
        'bwApp
        '
        Me.bwApp.WorkerReportsProgress = True
        Me.bwApp.WorkerSupportsCancellation = True
        '
        'ttApp
        '
        Me.ttApp.AutomaticDelay = 200
        Me.ttApp.AutoPopDelay = 5000
        Me.ttApp.InitialDelay = 200
        Me.ttApp.ReshowDelay = 40
        '
        'tmrSecond
        '
        Me.tmrSecond.Enabled = True
        Me.tmrSecond.Interval = 1000
        '
        'niTray
        '
        Me.niTray.ContextMenuStrip = Me.cmsApp
        Me.niTray.Icon = CType(resources.GetObject("niTray.Icon"), System.Drawing.Icon)
        Me.niTray.Text = "iTSfv"
        '
        'msApp
        '
        Me.msApp.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.miJobs, Me.LogsToolStripMenuItem, Me.SettingsToolStripMenuItem, Me.FoldersToolStripMenuItem, Me.SelectedTracksToolStripMenuItem, Me.ToolsToolStripMenuItem, Me.HelpToolStripMenuItem})
        Me.msApp.Location = New System.Drawing.Point(0, 0)
        Me.msApp.Name = "msApp"
        Me.msApp.Size = New System.Drawing.Size(784, 24)
        Me.msApp.TabIndex = 17
        Me.msApp.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmAddFolderToLib, Me.OpenToolStripMenuItem, Me.SaveStatisticsFileAsToolStripMenuItem, Me.CreatePlaylistOfSelectedTracksToolStripMenuItem, Me.ToolStripSeparator7, Me.miSendToTray, Me.ToolStripSeparator4, Me.ExitToolStripMenuItem1})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(35, 20)
        Me.FileToolStripMenuItem.Text = "&File"
        '
        'tsmAddFolderToLib
        '
        Me.tsmAddFolderToLib.Image = Global.iTSfv.My.Resources.Resources.folder_add
        Me.tsmAddFolderToLib.Name = "tsmAddFolderToLib"
        Me.tsmAddFolderToLib.Size = New System.Drawing.Size(263, 22)
        Me.tsmAddFolderToLib.Text = "&Add folder to Library..."
        '
        'OpenToolStripMenuItem
        '
        Me.OpenToolStripMenuItem.Image = CType(resources.GetObject("OpenToolStripMenuItem.Image"), System.Drawing.Image)
        Me.OpenToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem"
        Me.OpenToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
        Me.OpenToolStripMenuItem.Size = New System.Drawing.Size(263, 22)
        Me.OpenToolStripMenuItem.Text = "&Open Statistics File..."
        '
        'SaveStatisticsFileAsToolStripMenuItem
        '
        Me.SaveStatisticsFileAsToolStripMenuItem.Name = "SaveStatisticsFileAsToolStripMenuItem"
        Me.SaveStatisticsFileAsToolStripMenuItem.Size = New System.Drawing.Size(263, 22)
        Me.SaveStatisticsFileAsToolStripMenuItem.Text = "Save S&tatistics File As..."
        '
        'CreatePlaylistOfSelectedTracksToolStripMenuItem
        '
        Me.CreatePlaylistOfSelectedTracksToolStripMenuItem.Name = "CreatePlaylistOfSelectedTracksToolStripMenuItem"
        Me.CreatePlaylistOfSelectedTracksToolStripMenuItem.Size = New System.Drawing.Size(263, 22)
        Me.CreatePlaylistOfSelectedTracksToolStripMenuItem.Text = "&Save Playlist of Selected Tracks As..."
        '
        'ToolStripSeparator7
        '
        Me.ToolStripSeparator7.Name = "ToolStripSeparator7"
        Me.ToolStripSeparator7.Size = New System.Drawing.Size(260, 6)
        '
        'miSendToTray
        '
        Me.miSendToTray.Name = "miSendToTray"
        Me.miSendToTray.ShortcutKeys = System.Windows.Forms.Keys.F12
        Me.miSendToTray.Size = New System.Drawing.Size(263, 22)
        Me.miSendToTray.Text = "Send to System Tray"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(260, 6)
        '
        'ExitToolStripMenuItem1
        '
        Me.ExitToolStripMenuItem1.Image = Global.iTSfv.My.Resources.Resources._exit
        Me.ExitToolStripMenuItem1.Name = "ExitToolStripMenuItem1"
        Me.ExitToolStripMenuItem1.Size = New System.Drawing.Size(263, 22)
        Me.ExitToolStripMenuItem1.Text = "E&xit"
        '
        'miJobs
        '
        Me.miJobs.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.miAdjustRatings, Me.miStatistics, Me.tsmUpdatePlayedCount, Me.ToolStripSeparator17, Me.DeleteDeadOrForeignTracksToolStripMenuItem, Me.AddNewFilesNotInITunesMusicLibraryToolStripMenuItem, Me.miSynchroclean, Me.ToolStripSeparator6, Me.PlayFirstTrackToolStripMenuItem, Me.ToolStripSeparator5, Me.miValidateSelected, Me.ValidateLast100TracksToolStripMenuItem, Me.ValidateITunesMusicLibraryToolStripMenuItem, Me.ToolStripSeparator26, Me.VerboseModeToolStripMenuItem})
        Me.miJobs.Name = "miJobs"
        Me.miJobs.Size = New System.Drawing.Size(41, 20)
        Me.miJobs.Text = "&Jobs"
        '
        'miAdjustRatings
        '
        Me.miAdjustRatings.Image = Global.iTSfv.My.Resources.Resources.star
        Me.miAdjustRatings.Name = "miAdjustRatings"
        Me.miAdjustRatings.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.R), System.Windows.Forms.Keys)
        Me.miAdjustRatings.Size = New System.Drawing.Size(358, 22)
        Me.miAdjustRatings.Text = "Adjust track &Ratings according to play pattern"
        '
        'miStatistics
        '
        Me.miStatistics.Image = Global.iTSfv.My.Resources.Resources.chart_bar
        Me.miStatistics.Name = "miStatistics"
        Me.miStatistics.Size = New System.Drawing.Size(358, 22)
        Me.miStatistics.Text = "Display iTunes Music Library Statistics..."
        '
        'tsmUpdatePlayedCount
        '
        Me.tsmUpdatePlayedCount.Image = Global.iTSfv.My.Resources.Resources.LastFM
        Me.tsmUpdatePlayedCount.Name = "tsmUpdatePlayedCount"
        Me.tsmUpdatePlayedCount.Size = New System.Drawing.Size(358, 22)
        Me.tsmUpdatePlayedCount.Text = "Update Track PlayedCount in iTunes using Last.fm Profile"
        '
        'ToolStripSeparator17
        '
        Me.ToolStripSeparator17.Name = "ToolStripSeparator17"
        Me.ToolStripSeparator17.Size = New System.Drawing.Size(355, 6)
        '
        'DeleteDeadOrForeignTracksToolStripMenuItem
        '
        Me.DeleteDeadOrForeignTracksToolStripMenuItem.Image = Global.iTSfv.My.Resources.Resources.database_delete
        Me.DeleteDeadOrForeignTracksToolStripMenuItem.Name = "DeleteDeadOrForeignTracksToolStripMenuItem"
        Me.DeleteDeadOrForeignTracksToolStripMenuItem.Size = New System.Drawing.Size(358, 22)
        Me.DeleteDeadOrForeignTracksToolStripMenuItem.Text = "&Remove dead or foreign tracks from Library..."
        '
        'AddNewFilesNotInITunesMusicLibraryToolStripMenuItem
        '
        Me.AddNewFilesNotInITunesMusicLibraryToolStripMenuItem.Image = Global.iTSfv.My.Resources.Resources.folder_find
        Me.AddNewFilesNotInITunesMusicLibraryToolStripMenuItem.Name = "AddNewFilesNotInITunesMusicLibraryToolStripMenuItem"
        Me.AddNewFilesNotInITunesMusicLibraryToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.F), System.Windows.Forms.Keys)
        Me.AddNewFilesNotInITunesMusicLibraryToolStripMenuItem.Size = New System.Drawing.Size(358, 22)
        Me.AddNewFilesNotInITunesMusicLibraryToolStripMenuItem.Text = "&Find New Tracks not in iTunes Music Library..."
        '
        'miSynchroclean
        '
        Me.miSynchroclean.Image = Global.iTSfv.My.Resources.Resources.database_refresh
        Me.miSynchroclean.Name = "miSynchroclean"
        Me.miSynchroclean.Size = New System.Drawing.Size(358, 22)
        Me.miSynchroclean.Text = "Synchroclean® iTunes Music Library..."
        '
        'ToolStripSeparator6
        '
        Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
        Me.ToolStripSeparator6.Size = New System.Drawing.Size(355, 6)
        '
        'PlayFirstTrackToolStripMenuItem
        '
        Me.PlayFirstTrackToolStripMenuItem.Image = Global.iTSfv.My.Resources.Resources.control_play
        Me.PlayFirstTrackToolStripMenuItem.Name = "PlayFirstTrackToolStripMenuItem"
        Me.PlayFirstTrackToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.P), System.Windows.Forms.Keys)
        Me.PlayFirstTrackToolStripMenuItem.Size = New System.Drawing.Size(358, 22)
        Me.PlayFirstTrackToolStripMenuItem.Text = "&Play Album"
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(355, 6)
        '
        'miValidateSelected
        '
        Me.miValidateSelected.Name = "miValidateSelected"
        Me.miValidateSelected.Size = New System.Drawing.Size(358, 22)
        Me.miValidateSelected.Text = "Validate &Selected Tracks"
        '
        'ValidateLast100TracksToolStripMenuItem
        '
        Me.ValidateLast100TracksToolStripMenuItem.Name = "ValidateLast100TracksToolStripMenuItem"
        Me.ValidateLast100TracksToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5
        Me.ValidateLast100TracksToolStripMenuItem.Size = New System.Drawing.Size(358, 22)
        Me.ValidateLast100TracksToolStripMenuItem.Text = "&Validate Last 100 Tracks"
        '
        'ValidateITunesMusicLibraryToolStripMenuItem
        '
        Me.ValidateITunesMusicLibraryToolStripMenuItem.Name = "ValidateITunesMusicLibraryToolStripMenuItem"
        Me.ValidateITunesMusicLibraryToolStripMenuItem.Size = New System.Drawing.Size(358, 22)
        Me.ValidateITunesMusicLibraryToolStripMenuItem.Text = "&Validate iTunes Music Library"
        '
        'ToolStripSeparator26
        '
        Me.ToolStripSeparator26.Name = "ToolStripSeparator26"
        Me.ToolStripSeparator26.Size = New System.Drawing.Size(355, 6)
        '
        'VerboseModeToolStripMenuItem
        '
        Me.VerboseModeToolStripMenuItem.Checked = True
        Me.VerboseModeToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.VerboseModeToolStripMenuItem.Name = "VerboseModeToolStripMenuItem"
        Me.VerboseModeToolStripMenuItem.Size = New System.Drawing.Size(358, 22)
        Me.VerboseModeToolStripMenuItem.Text = "Show Verbose Mode"
        '
        'LogsToolStripMenuItem
        '
        Me.LogsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenTracksReportToolStripMenuItem1, Me.ToolStripSeparator14, Me.ArtworkToolStripMenuItem, Me.LyricsToolStripMenuItem, Me.tsmTrackTags, Me.ToolStripSeparator11, Me.TracksThatRatingWasAdjustedToolStripMenuItem1, Me.TracksNotITunesStoreCompliantToolStripMenuItem, Me.TracksThatTrackCountWasUpdatedToolStripMenuItem, Me.ToolStripSeparator12, Me.LibraryToolStripMenuItem, Me.FileSystemToolStripMenuItem, Me.ToolStripSeparator22, Me.AlbumsWithInconsecutiveTrackNumbersToolStripMenuItem, Me.ToolStripSeparator13, Me.DebugToolStripMenuItem, Me.ErrorsToolStripMenuItem, Me.WarningsToolStripMenuItem, Me.ToolStripSeparator20, Me.BrowseLogsFolderToolStripMenuItem1})
        Me.LogsToolStripMenuItem.Name = "LogsToolStripMenuItem"
        Me.LogsToolStripMenuItem.Size = New System.Drawing.Size(41, 20)
        Me.LogsToolStripMenuItem.Text = "&Logs"
        '
        'OpenTracksReportToolStripMenuItem1
        '
        Me.OpenTracksReportToolStripMenuItem1.Image = Global.iTSfv.My.Resources.Resources.report
        Me.OpenTracksReportToolStripMenuItem1.Name = "OpenTracksReportToolStripMenuItem1"
        Me.OpenTracksReportToolStripMenuItem1.Size = New System.Drawing.Size(298, 22)
        Me.OpenTracksReportToolStripMenuItem1.Text = "Validation &Report..."
        '
        'ToolStripSeparator14
        '
        Me.ToolStripSeparator14.Name = "ToolStripSeparator14"
        Me.ToolStripSeparator14.Size = New System.Drawing.Size(295, 6)
        '
        'ArtworkToolStripMenuItem
        '
        Me.ArtworkToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TracksThatArtworkWasAddedToolStripMenuItem1, Me.TracksWithmultipleArtworkToolStripMenuItem1, Me.TracksWithoutArtworkToolStripMenuItem, Me.TracksThatArtworkIsLowResolutionToolStripMenuItem, Me.tsmiArtworkConverted})
        Me.ArtworkToolStripMenuItem.Name = "ArtworkToolStripMenuItem"
        Me.ArtworkToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.ArtworkToolStripMenuItem.Text = "Track &Artwork"
        '
        'TracksThatArtworkWasAddedToolStripMenuItem1
        '
        Me.TracksThatArtworkWasAddedToolStripMenuItem1.Name = "TracksThatArtworkWasAddedToolStripMenuItem1"
        Me.TracksThatArtworkWasAddedToolStripMenuItem1.Size = New System.Drawing.Size(343, 22)
        Me.TracksThatArtworkWasAddedToolStripMenuItem1.Text = "Tracks that &Artwork was added..."
        '
        'TracksWithmultipleArtworkToolStripMenuItem1
        '
        Me.TracksWithmultipleArtworkToolStripMenuItem1.Name = "TracksWithmultipleArtworkToolStripMenuItem1"
        Me.TracksWithmultipleArtworkToolStripMenuItem1.Size = New System.Drawing.Size(343, 22)
        Me.TracksWithmultipleArtworkToolStripMenuItem1.Text = "Tracks with &Multiple Artwork..."
        '
        'TracksWithoutArtworkToolStripMenuItem
        '
        Me.TracksWithoutArtworkToolStripMenuItem.Name = "TracksWithoutArtworkToolStripMenuItem"
        Me.TracksWithoutArtworkToolStripMenuItem.Size = New System.Drawing.Size(343, 22)
        Me.TracksWithoutArtworkToolStripMenuItem.Text = "Tracks without Artwork..."
        '
        'TracksThatArtworkIsLowResolutionToolStripMenuItem
        '
        Me.TracksThatArtworkIsLowResolutionToolStripMenuItem.Name = "TracksThatArtworkIsLowResolutionToolStripMenuItem"
        Me.TracksThatArtworkIsLowResolutionToolStripMenuItem.Size = New System.Drawing.Size(343, 22)
        Me.TracksThatArtworkIsLowResolutionToolStripMenuItem.Text = "Tracks that Artwork is &Low Resolution..."
        '
        'tsmiArtworkConverted
        '
        Me.tsmiArtworkConverted.Name = "tsmiArtworkConverted"
        Me.tsmiArtworkConverted.Size = New System.Drawing.Size(343, 22)
        Me.tsmiArtworkConverted.Text = "Tracks that Artwork was Converted to JPEG format..."
        '
        'LyricsToolStripMenuItem
        '
        Me.LyricsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TracksWithoutLyricsToolStripMenuItem, Me.TracksLyricsWereAddedToolStripMenuItem})
        Me.LyricsToolStripMenuItem.Name = "LyricsToolStripMenuItem"
        Me.LyricsToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.LyricsToolStripMenuItem.Text = "Track &Lyrics"
        '
        'TracksWithoutLyricsToolStripMenuItem
        '
        Me.TracksWithoutLyricsToolStripMenuItem.Name = "TracksWithoutLyricsToolStripMenuItem"
        Me.TracksWithoutLyricsToolStripMenuItem.Size = New System.Drawing.Size(242, 22)
        Me.TracksWithoutLyricsToolStripMenuItem.Text = "Tracks without &Lyrics..."
        '
        'TracksLyricsWereAddedToolStripMenuItem
        '
        Me.TracksLyricsWereAddedToolStripMenuItem.Name = "TracksLyricsWereAddedToolStripMenuItem"
        Me.TracksLyricsWereAddedToolStripMenuItem.Size = New System.Drawing.Size(242, 22)
        Me.TracksLyricsWereAddedToolStripMenuItem.Text = "Tracks that Lyrics were Added..."
        '
        'tsmTrackTags
        '
        Me.tsmTrackTags.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmTrackTagsBPM, Me.tsmTrackTagsRefreshed, Me.TrackMetatagVersionsToolStripMenuItem})
        Me.tsmTrackTags.Name = "tsmTrackTags"
        Me.tsmTrackTags.Size = New System.Drawing.Size(298, 22)
        Me.tsmTrackTags.Text = "Track &Tags"
        '
        'tsmTrackTagsBPM
        '
        Me.tsmTrackTagsBPM.Name = "tsmTrackTagsBPM"
        Me.tsmTrackTagsBPM.Size = New System.Drawing.Size(254, 22)
        Me.tsmTrackTagsBPM.Text = "Tracks without BPM..."
        '
        'tsmTrackTagsRefreshed
        '
        Me.tsmTrackTagsRefreshed.Name = "tsmTrackTagsRefreshed"
        Me.tsmTrackTagsRefreshed.Size = New System.Drawing.Size(254, 22)
        Me.tsmTrackTagsRefreshed.Text = "Tracks that Tags were refreshed..."
        '
        'TrackMetatagVersionsToolStripMenuItem
        '
        Me.TrackMetatagVersionsToolStripMenuItem.Name = "TrackMetatagVersionsToolStripMenuItem"
        Me.TrackMetatagVersionsToolStripMenuItem.Size = New System.Drawing.Size(254, 22)
        Me.TrackMetatagVersionsToolStripMenuItem.Text = "Track &Metatag versions..."
        '
        'ToolStripSeparator11
        '
        Me.ToolStripSeparator11.Name = "ToolStripSeparator11"
        Me.ToolStripSeparator11.Size = New System.Drawing.Size(295, 6)
        '
        'TracksThatRatingWasAdjustedToolStripMenuItem1
        '
        Me.TracksThatRatingWasAdjustedToolStripMenuItem1.Name = "TracksThatRatingWasAdjustedToolStripMenuItem1"
        Me.TracksThatRatingWasAdjustedToolStripMenuItem1.Size = New System.Drawing.Size(298, 22)
        Me.TracksThatRatingWasAdjustedToolStripMenuItem1.Text = "Tracks that &Rating was adjusted..."
        '
        'TracksNotITunesStoreCompliantToolStripMenuItem
        '
        Me.TracksNotITunesStoreCompliantToolStripMenuItem.Name = "TracksNotITunesStoreCompliantToolStripMenuItem"
        Me.TracksNotITunesStoreCompliantToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.TracksNotITunesStoreCompliantToolStripMenuItem.Text = "Tracks not iTunes Store compliant..."
        '
        'TracksThatTrackCountWasUpdatedToolStripMenuItem
        '
        Me.TracksThatTrackCountWasUpdatedToolStripMenuItem.Name = "TracksThatTrackCountWasUpdatedToolStripMenuItem"
        Me.TracksThatTrackCountWasUpdatedToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.TracksThatTrackCountWasUpdatedToolStripMenuItem.Text = "Tracks that Track &Count was updated..."
        '
        'ToolStripSeparator12
        '
        Me.ToolStripSeparator12.Name = "ToolStripSeparator12"
        Me.ToolStripSeparator12.Size = New System.Drawing.Size(295, 6)
        '
        'LibraryToolStripMenuItem
        '
        Me.LibraryToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DuplicateTracksToolStripMenuItem})
        Me.LibraryToolStripMenuItem.Name = "LibraryToolStripMenuItem"
        Me.LibraryToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.LibraryToolStripMenuItem.Text = "&Library"
        '
        'DuplicateTracksToolStripMenuItem
        '
        Me.DuplicateTracksToolStripMenuItem.Name = "DuplicateTracksToolStripMenuItem"
        Me.DuplicateTracksToolStripMenuItem.Size = New System.Drawing.Size(175, 22)
        Me.DuplicateTracksToolStripMenuItem.Text = "&Duplicate Tracks..."
        '
        'FileSystemToolStripMenuItem
        '
        Me.FileSystemToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FoldersWithOneFileToolStripMenuItem, Me.FoldersWithoutArtworkToolStripMenuItem, Me.FoldersWithoutAudioToolStripMenuItem, Me.ToolStripSeparator25, Me.tsmMusicFolderActivity})
        Me.FileSystemToolStripMenuItem.Name = "FileSystemToolStripMenuItem"
        Me.FileSystemToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.FileSystemToolStripMenuItem.Text = "&File System"
        '
        'FoldersWithOneFileToolStripMenuItem
        '
        Me.FoldersWithOneFileToolStripMenuItem.Name = "FoldersWithOneFileToolStripMenuItem"
        Me.FoldersWithOneFileToolStripMenuItem.Size = New System.Drawing.Size(212, 22)
        Me.FoldersWithOneFileToolStripMenuItem.Text = "Folders with &one File..."
        '
        'FoldersWithoutArtworkToolStripMenuItem
        '
        Me.FoldersWithoutArtworkToolStripMenuItem.Name = "FoldersWithoutArtworkToolStripMenuItem"
        Me.FoldersWithoutArtworkToolStripMenuItem.Size = New System.Drawing.Size(212, 22)
        Me.FoldersWithoutArtworkToolStripMenuItem.Text = "&Folders without Artwork..."
        '
        'FoldersWithoutAudioToolStripMenuItem
        '
        Me.FoldersWithoutAudioToolStripMenuItem.Name = "FoldersWithoutAudioToolStripMenuItem"
        Me.FoldersWithoutAudioToolStripMenuItem.Size = New System.Drawing.Size(212, 22)
        Me.FoldersWithoutAudioToolStripMenuItem.Text = "Folders without Audio..."
        '
        'ToolStripSeparator25
        '
        Me.ToolStripSeparator25.Name = "ToolStripSeparator25"
        Me.ToolStripSeparator25.Size = New System.Drawing.Size(209, 6)
        '
        'tsmMusicFolderActivity
        '
        Me.tsmMusicFolderActivity.Name = "tsmMusicFolderActivity"
        Me.tsmMusicFolderActivity.Size = New System.Drawing.Size(212, 22)
        Me.tsmMusicFolderActivity.Text = "&Music Folder Activity..."
        '
        'ToolStripSeparator22
        '
        Me.ToolStripSeparator22.Name = "ToolStripSeparator22"
        Me.ToolStripSeparator22.Size = New System.Drawing.Size(295, 6)
        '
        'AlbumsWithInconsecutiveTrackNumbersToolStripMenuItem
        '
        Me.AlbumsWithInconsecutiveTrackNumbersToolStripMenuItem.Name = "AlbumsWithInconsecutiveTrackNumbersToolStripMenuItem"
        Me.AlbumsWithInconsecutiveTrackNumbersToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.AlbumsWithInconsecutiveTrackNumbersToolStripMenuItem.Text = "&Albums with Inconsecutive Track Numbers..."
        '
        'ToolStripSeparator13
        '
        Me.ToolStripSeparator13.Name = "ToolStripSeparator13"
        Me.ToolStripSeparator13.Size = New System.Drawing.Size(295, 6)
        '
        'DebugToolStripMenuItem
        '
        Me.DebugToolStripMenuItem.Name = "DebugToolStripMenuItem"
        Me.DebugToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.DebugToolStripMenuItem.Text = "&Debug..."
        '
        'ErrorsToolStripMenuItem
        '
        Me.ErrorsToolStripMenuItem.Name = "ErrorsToolStripMenuItem"
        Me.ErrorsToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.ErrorsToolStripMenuItem.Text = "&Errors..."
        '
        'WarningsToolStripMenuItem
        '
        Me.WarningsToolStripMenuItem.Name = "WarningsToolStripMenuItem"
        Me.WarningsToolStripMenuItem.Size = New System.Drawing.Size(298, 22)
        Me.WarningsToolStripMenuItem.Text = "&Warnings..."
        '
        'ToolStripSeparator20
        '
        Me.ToolStripSeparator20.Name = "ToolStripSeparator20"
        Me.ToolStripSeparator20.Size = New System.Drawing.Size(295, 6)
        '
        'BrowseLogsFolderToolStripMenuItem1
        '
        Me.BrowseLogsFolderToolStripMenuItem1.Image = Global.iTSfv.My.Resources.Resources.folder_explore
        Me.BrowseLogsFolderToolStripMenuItem1.Name = "BrowseLogsFolderToolStripMenuItem1"
        Me.BrowseLogsFolderToolStripMenuItem1.Size = New System.Drawing.Size(298, 22)
        Me.BrowseLogsFolderToolStripMenuItem1.Text = "&Browse Logs Folder..."
        '
        'SettingsToolStripMenuItem
        '
        Me.SettingsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.IgnoreWordsToolStripMenuItem, Me.CapitalWordsToolStripMenuItem, Me.SimpleWordsToolStripMenuItem, Me.ReplaceWordsToolStripMenuItem, Me.SkipWordsToolStripMenuItem, Me.ToolStripSeparator19, Me.BrowseSettingsFolderToolStripMenuItem})
        Me.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem"
        Me.SettingsToolStripMenuItem.Size = New System.Drawing.Size(58, 20)
        Me.SettingsToolStripMenuItem.Text = "&Settings"
        '
        'IgnoreWordsToolStripMenuItem
        '
        Me.IgnoreWordsToolStripMenuItem.Name = "IgnoreWordsToolStripMenuItem"
        Me.IgnoreWordsToolStripMenuItem.Size = New System.Drawing.Size(207, 22)
        Me.IgnoreWordsToolStripMenuItem.Text = "&Ignore Words..."
        '
        'CapitalWordsToolStripMenuItem
        '
        Me.CapitalWordsToolStripMenuItem.Name = "CapitalWordsToolStripMenuItem"
        Me.CapitalWordsToolStripMenuItem.Size = New System.Drawing.Size(207, 22)
        Me.CapitalWordsToolStripMenuItem.Text = "&Capital Words..."
        '
        'SimpleWordsToolStripMenuItem
        '
        Me.SimpleWordsToolStripMenuItem.Name = "SimpleWordsToolStripMenuItem"
        Me.SimpleWordsToolStripMenuItem.Size = New System.Drawing.Size(207, 22)
        Me.SimpleWordsToolStripMenuItem.Text = "Simple Words..."
        '
        'ReplaceWordsToolStripMenuItem
        '
        Me.ReplaceWordsToolStripMenuItem.Name = "ReplaceWordsToolStripMenuItem"
        Me.ReplaceWordsToolStripMenuItem.Size = New System.Drawing.Size(207, 22)
        Me.ReplaceWordsToolStripMenuItem.Text = "Replace Words..."
        '
        'SkipWordsToolStripMenuItem
        '
        Me.SkipWordsToolStripMenuItem.Name = "SkipWordsToolStripMenuItem"
        Me.SkipWordsToolStripMenuItem.Size = New System.Drawing.Size(207, 22)
        Me.SkipWordsToolStripMenuItem.Text = "Skip Words in Album..."
        '
        'ToolStripSeparator19
        '
        Me.ToolStripSeparator19.Name = "ToolStripSeparator19"
        Me.ToolStripSeparator19.Size = New System.Drawing.Size(204, 6)
        '
        'BrowseSettingsFolderToolStripMenuItem
        '
        Me.BrowseSettingsFolderToolStripMenuItem.Image = Global.iTSfv.My.Resources.Resources.folder_explore
        Me.BrowseSettingsFolderToolStripMenuItem.Name = "BrowseSettingsFolderToolStripMenuItem"
        Me.BrowseSettingsFolderToolStripMenuItem.Size = New System.Drawing.Size(207, 22)
        Me.BrowseSettingsFolderToolStripMenuItem.Text = "&Browse Settings Folder..."
        '
        'FoldersToolStripMenuItem
        '
        Me.FoldersToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BrowseMusicFolderToolStripMenuItem, Me.ToolStripSeparator15, Me.BrowseITMSArtworksFolderToolStripMenuItem, Me.BrowseLogsFolderToolStripMenuItem, Me.SettingsFolderToolStripMenuItem, Me.BrowseTemporaryFiToolStripMenuItem})
        Me.FoldersToolStripMenuItem.Name = "FoldersToolStripMenuItem"
        Me.FoldersToolStripMenuItem.Size = New System.Drawing.Size(54, 20)
        Me.FoldersToolStripMenuItem.Text = "&Folders"
        '
        'BrowseMusicFolderToolStripMenuItem
        '
        Me.BrowseMusicFolderToolStripMenuItem.Image = Global.iTSfv.My.Resources.Resources.musicfolder
        Me.BrowseMusicFolderToolStripMenuItem.Name = "BrowseMusicFolderToolStripMenuItem"
        Me.BrowseMusicFolderToolStripMenuItem.Size = New System.Drawing.Size(182, 22)
        Me.BrowseMusicFolderToolStripMenuItem.Text = "&Music Folder..."
        '
        'ToolStripSeparator15
        '
        Me.ToolStripSeparator15.Name = "ToolStripSeparator15"
        Me.ToolStripSeparator15.Size = New System.Drawing.Size(179, 6)
        '
        'BrowseITMSArtworksFolderToolStripMenuItem
        '
        Me.BrowseITMSArtworksFolderToolStripMenuItem.Name = "BrowseITMSArtworksFolderToolStripMenuItem"
        Me.BrowseITMSArtworksFolderToolStripMenuItem.Size = New System.Drawing.Size(182, 22)
        Me.BrowseITMSArtworksFolderToolStripMenuItem.Text = "&Artwork Folder..."
        '
        'BrowseLogsFolderToolStripMenuItem
        '
        Me.BrowseLogsFolderToolStripMenuItem.Name = "BrowseLogsFolderToolStripMenuItem"
        Me.BrowseLogsFolderToolStripMenuItem.Size = New System.Drawing.Size(182, 22)
        Me.BrowseLogsFolderToolStripMenuItem.Text = "&Logs Folder..."
        '
        'SettingsFolderToolStripMenuItem
        '
        Me.SettingsFolderToolStripMenuItem.Name = "SettingsFolderToolStripMenuItem"
        Me.SettingsFolderToolStripMenuItem.Size = New System.Drawing.Size(182, 22)
        Me.SettingsFolderToolStripMenuItem.Text = "&Settings Folder..."
        '
        'BrowseTemporaryFiToolStripMenuItem
        '
        Me.BrowseTemporaryFiToolStripMenuItem.Name = "BrowseTemporaryFiToolStripMenuItem"
        Me.BrowseTemporaryFiToolStripMenuItem.Size = New System.Drawing.Size(182, 22)
        Me.BrowseTemporaryFiToolStripMenuItem.Text = "&Temporary Folder..."
        '
        'SelectedTracksToolStripMenuItem
        '
        Me.SelectedTracksToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SavePlaylistOfSelectedTracksAsToolStripMenuItem, Me.tsmiSelectedTracksValidate, Me.ToolStripSeparator23, Me.CopyInfoToClipboardToolStripMenuItem, Me.Mp3tagSelectedTracks, Me.tsmTiunesStoreArtworkGrabSelected, Me.SearchArtworkToolStripMenuItem, Me.tsmiSearch})
        Me.SelectedTracksToolStripMenuItem.Name = "SelectedTracksToolStripMenuItem"
        Me.SelectedTracksToolStripMenuItem.Size = New System.Drawing.Size(94, 20)
        Me.SelectedTracksToolStripMenuItem.Text = "&Selected Tracks"
        '
        'SavePlaylistOfSelectedTracksAsToolStripMenuItem
        '
        Me.SavePlaylistOfSelectedTracksAsToolStripMenuItem.Name = "SavePlaylistOfSelectedTracksAsToolStripMenuItem"
        Me.SavePlaylistOfSelectedTracksAsToolStripMenuItem.Size = New System.Drawing.Size(319, 22)
        Me.SavePlaylistOfSelectedTracksAsToolStripMenuItem.Text = "&Save Playlist As..."
        '
        'tsmiSelectedTracksValidate
        '
        Me.tsmiSelectedTracksValidate.DoubleClickEnabled = True
        Me.tsmiSelectedTracksValidate.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CheckToolStripMenuItem, Me.EditTracksToolStripMenuItem, Me.LibraryToolStripMenuItem1, Me.tsmSelectedTracksValidateFS})
        Me.tsmiSelectedTracksValidate.Name = "tsmiSelectedTracksValidate"
        Me.tsmiSelectedTracksValidate.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.D0), System.Windows.Forms.Keys)
        Me.tsmiSelectedTracksValidate.Size = New System.Drawing.Size(319, 22)
        Me.tsmiSelectedTracksValidate.Text = "&Validate"
        '
        'CheckToolStripMenuItem
        '
        Me.CheckToolStripMenuItem.Name = "CheckToolStripMenuItem"
        Me.CheckToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.D1), System.Windows.Forms.Keys)
        Me.CheckToolStripMenuItem.Size = New System.Drawing.Size(177, 22)
        Me.CheckToolStripMenuItem.Text = "&Check"
        '
        'EditTracksToolStripMenuItem
        '
        Me.EditTracksToolStripMenuItem.Name = "EditTracksToolStripMenuItem"
        Me.EditTracksToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.D2), System.Windows.Forms.Keys)
        Me.EditTracksToolStripMenuItem.Size = New System.Drawing.Size(177, 22)
        Me.EditTracksToolStripMenuItem.Text = "&Tracks"
        '
        'LibraryToolStripMenuItem1
        '
        Me.LibraryToolStripMenuItem1.Name = "LibraryToolStripMenuItem1"
        Me.LibraryToolStripMenuItem1.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.D3), System.Windows.Forms.Keys)
        Me.LibraryToolStripMenuItem1.Size = New System.Drawing.Size(177, 22)
        Me.LibraryToolStripMenuItem1.Text = "&Library"
        '
        'tsmSelectedTracksValidateFS
        '
        Me.tsmSelectedTracksValidateFS.Name = "tsmSelectedTracksValidateFS"
        Me.tsmSelectedTracksValidateFS.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.D4), System.Windows.Forms.Keys)
        Me.tsmSelectedTracksValidateFS.Size = New System.Drawing.Size(177, 22)
        Me.tsmSelectedTracksValidateFS.Text = "&File System"
        '
        'ToolStripSeparator23
        '
        Me.ToolStripSeparator23.Name = "ToolStripSeparator23"
        Me.ToolStripSeparator23.Size = New System.Drawing.Size(316, 6)
        '
        'CopyInfoToClipboardToolStripMenuItem
        '
        Me.CopyInfoToClipboardToolStripMenuItem.Image = Global.iTSfv.My.Resources.Resources.page_copy
        Me.CopyInfoToClipboardToolStripMenuItem.Name = "CopyInfoToClipboardToolStripMenuItem"
        Me.CopyInfoToClipboardToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
        Me.CopyInfoToClipboardToolStripMenuItem.Size = New System.Drawing.Size(319, 22)
        Me.CopyInfoToClipboardToolStripMenuItem.Text = "&Copy Info to Clipboard"
        '
        'Mp3tagSelectedTracks
        '
        Me.Mp3tagSelectedTracks.Image = Global.iTSfv.My.Resources.Resources.mp3tag
        Me.Mp3tagSelectedTracks.Name = "Mp3tagSelectedTracks"
        Me.Mp3tagSelectedTracks.Size = New System.Drawing.Size(319, 22)
        Me.Mp3tagSelectedTracks.Text = "Edit using Mp3tag..."
        '
        'tsmTiunesStoreArtworkGrabSelected
        '
        Me.tsmTiunesStoreArtworkGrabSelected.Image = Global.iTSfv.My.Resources.Resources.folder_image
        Me.tsmTiunesStoreArtworkGrabSelected.Name = "tsmTiunesStoreArtworkGrabSelected"
        Me.tsmTiunesStoreArtworkGrabSelected.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Down), System.Windows.Forms.Keys)
        Me.tsmTiunesStoreArtworkGrabSelected.Size = New System.Drawing.Size(319, 22)
        Me.tsmTiunesStoreArtworkGrabSelected.Text = "&Export iTunes Store track &Artwork..."
        Me.tsmTiunesStoreArtworkGrabSelected.ToolTipText = "Select a track in iTunes Store and click me"
        '
        'SearchArtworkToolStripMenuItem
        '
        Me.SearchArtworkToolStripMenuItem.Name = "SearchArtworkToolStripMenuItem"
        Me.SearchArtworkToolStripMenuItem.Size = New System.Drawing.Size(319, 22)
        Me.SearchArtworkToolStripMenuItem.Text = "&Search for Artwork using AAD..."
        '
        'tsmiSearch
        '
        Me.tsmiSearch.DoubleClickEnabled = True
        Me.tsmiSearch.Image = Global.iTSfv.My.Resources.Resources.world
        Me.tsmiSearch.Name = "tsmiSearch"
        Me.tsmiSearch.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.G), System.Windows.Forms.Keys)
        Me.tsmiSearch.Size = New System.Drawing.Size(319, 22)
        Me.tsmiSearch.Text = "&Search using Google..."
        '
        'ToolsToolStripMenuItem
        '
        Me.ToolsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AlbumArtDownloaderToolStripMenuItem, Me.tsmItunesArtworkGrabber, Me.tsmLyricsViewer, Me.tsmSetInfo, Me.Mp3tagToolStripMenuItem, Me.FileValidatorToolStripMenuItem, Me.TrackReplaceAssistantToolStripMenuItem, Me.ToolStripSeparator10, Me.AlwaysOnTopToolStripMenuItem, Me.miToolsOptions})
        Me.ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        Me.ToolsToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.ToolsToolStripMenuItem.Text = "&Tools"
        '
        'AlbumArtDownloaderToolStripMenuItem
        '
        Me.AlbumArtDownloaderToolStripMenuItem.Name = "AlbumArtDownloaderToolStripMenuItem"
        Me.AlbumArtDownloaderToolStripMenuItem.Size = New System.Drawing.Size(240, 22)
        Me.AlbumArtDownloaderToolStripMenuItem.Text = "Album Art Downloader XUI..."
        '
        'tsmItunesArtworkGrabber
        '
        Me.tsmItunesArtworkGrabber.Name = "tsmItunesArtworkGrabber"
        Me.tsmItunesArtworkGrabber.Size = New System.Drawing.Size(240, 22)
        Me.tsmItunesArtworkGrabber.Text = "iTunes Store Artwork &Grabber..."
        '
        'tsmLyricsViewer
        '
        Me.tsmLyricsViewer.Name = "tsmLyricsViewer"
        Me.tsmLyricsViewer.Size = New System.Drawing.Size(240, 22)
        Me.tsmLyricsViewer.Text = "&Lyric Viewer..."
        '
        'tsmSetInfo
        '
        Me.tsmSetInfo.Name = "tsmSetInfo"
        Me.tsmSetInfo.Size = New System.Drawing.Size(240, 22)
        Me.tsmSetInfo.Text = "&Set Info..."
        '
        'Mp3tagToolStripMenuItem
        '
        Me.Mp3tagToolStripMenuItem.Image = Global.iTSfv.My.Resources.Resources.mp3tag
        Me.Mp3tagToolStripMenuItem.Name = "Mp3tagToolStripMenuItem"
        Me.Mp3tagToolStripMenuItem.Size = New System.Drawing.Size(240, 22)
        Me.Mp3tagToolStripMenuItem.Text = "&Mp3tag..."
        '
        'FileValidatorToolStripMenuItem
        '
        Me.FileValidatorToolStripMenuItem.Name = "FileValidatorToolStripMenuItem"
        Me.FileValidatorToolStripMenuItem.Size = New System.Drawing.Size(240, 22)
        Me.FileValidatorToolStripMenuItem.Text = "TagLib# Audio &File Validator..."
        '
        'TrackReplaceAssistantToolStripMenuItem
        '
        Me.TrackReplaceAssistantToolStripMenuItem.Name = "TrackReplaceAssistantToolStripMenuItem"
        Me.TrackReplaceAssistantToolStripMenuItem.Size = New System.Drawing.Size(240, 22)
        Me.TrackReplaceAssistantToolStripMenuItem.Text = "&Track Replace Assistant..."
        Me.TrackReplaceAssistantToolStripMenuItem.ToolTipText = "Track Replace Assistant... (for similar kind files e.g. 128 Kib/s MP3 by 192 Kib/" & _
            "s VBR MP3 etc.)"
        '
        'ToolStripSeparator10
        '
        Me.ToolStripSeparator10.Name = "ToolStripSeparator10"
        Me.ToolStripSeparator10.Size = New System.Drawing.Size(237, 6)
        '
        'AlwaysOnTopToolStripMenuItem
        '
        Me.AlwaysOnTopToolStripMenuItem.Name = "AlwaysOnTopToolStripMenuItem"
        Me.AlwaysOnTopToolStripMenuItem.Size = New System.Drawing.Size(240, 22)
        Me.AlwaysOnTopToolStripMenuItem.Text = "&Always On Top"
        '
        'miToolsOptions
        '
        Me.miToolsOptions.Image = Global.iTSfv.My.Resources.Resources.application_edit
        Me.miToolsOptions.Name = "miToolsOptions"
        Me.miToolsOptions.ShortcutKeys = System.Windows.Forms.Keys.F2
        Me.miToolsOptions.Size = New System.Drawing.Size(240, 22)
        Me.miToolsOptions.Text = "&Options..."
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.miManual, Me.SupportForumsToolStripMenuItem, Me.ProjectHomeToolStripMenuItem, Me.VersionHistoryToolStripMenuItem1, Me.ToolStripSeparator9, Me.tsmiDonate, Me.ToolStripSeparator21, Me.SubmitDebugReportToolStripMenuItem, Me.CheckForUpdatesToolStripMenuItem1, Me.miAbout})
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(40, 20)
        Me.HelpToolStripMenuItem.Text = "&Help"
        '
        'miManual
        '
        Me.miManual.Image = Global.iTSfv.My.Resources.Resources.page_white_acrobat
        Me.miManual.Name = "miManual"
        Me.miManual.Size = New System.Drawing.Size(186, 22)
        Me.miManual.Text = "&Manual..."
        '
        'SupportForumsToolStripMenuItem
        '
        Me.SupportForumsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GoogleGroupsToolStripMenuItem, Me.HydrogenAudioToolStripMenuItem, Me.ILoungeToolStripMenuItem, Me.SourceForgeToolStripMenuItem, Me.ToolStripSeparator8, Me.BetaVersionsToolStripMenuItem, Me.SVNRepositoryToolStripMenuItem, Me.ToolStripSeparator27, Me.DiggToolStripMenuItem, Me.OhlohToolStripMenuItem, Me.WakoopaToolStripMenuItem})
        Me.SupportForumsToolStripMenuItem.Name = "SupportForumsToolStripMenuItem"
        Me.SupportForumsToolStripMenuItem.Size = New System.Drawing.Size(186, 22)
        Me.SupportForumsToolStripMenuItem.Text = "&Forums / Links"
        '
        'GoogleGroupsToolStripMenuItem
        '
        Me.GoogleGroupsToolStripMenuItem.Name = "GoogleGroupsToolStripMenuItem"
        Me.GoogleGroupsToolStripMenuItem.Size = New System.Drawing.Size(171, 22)
        Me.GoogleGroupsToolStripMenuItem.Text = "Google &Groups.."
        '
        'HydrogenAudioToolStripMenuItem
        '
        Me.HydrogenAudioToolStripMenuItem.Name = "HydrogenAudioToolStripMenuItem"
        Me.HydrogenAudioToolStripMenuItem.Size = New System.Drawing.Size(171, 22)
        Me.HydrogenAudioToolStripMenuItem.Text = "&HydrogenAudio..."
        '
        'ILoungeToolStripMenuItem
        '
        Me.ILoungeToolStripMenuItem.Name = "ILoungeToolStripMenuItem"
        Me.ILoungeToolStripMenuItem.Size = New System.Drawing.Size(171, 22)
        Me.ILoungeToolStripMenuItem.Text = "&iLounge..."
        '
        'SourceForgeToolStripMenuItem
        '
        Me.SourceForgeToolStripMenuItem.Name = "SourceForgeToolStripMenuItem"
        Me.SourceForgeToolStripMenuItem.Size = New System.Drawing.Size(171, 22)
        Me.SourceForgeToolStripMenuItem.Text = "&SourceForge..."
        '
        'ToolStripSeparator8
        '
        Me.ToolStripSeparator8.Name = "ToolStripSeparator8"
        Me.ToolStripSeparator8.Size = New System.Drawing.Size(168, 6)
        '
        'BetaVersionsToolStripMenuItem
        '
        Me.BetaVersionsToolStripMenuItem.Name = "BetaVersionsToolStripMenuItem"
        Me.BetaVersionsToolStripMenuItem.Size = New System.Drawing.Size(171, 22)
        Me.BetaVersionsToolStripMenuItem.Text = "&Beta Versions..."
        '
        'SVNRepositoryToolStripMenuItem
        '
        Me.SVNRepositoryToolStripMenuItem.Name = "SVNRepositoryToolStripMenuItem"
        Me.SVNRepositoryToolStripMenuItem.Size = New System.Drawing.Size(171, 22)
        Me.SVNRepositoryToolStripMenuItem.Text = "S&VN Repository..."
        '
        'ToolStripSeparator27
        '
        Me.ToolStripSeparator27.Name = "ToolStripSeparator27"
        Me.ToolStripSeparator27.Size = New System.Drawing.Size(168, 6)
        '
        'DiggToolStripMenuItem
        '
        Me.DiggToolStripMenuItem.Name = "DiggToolStripMenuItem"
        Me.DiggToolStripMenuItem.Size = New System.Drawing.Size(171, 22)
        Me.DiggToolStripMenuItem.Text = "&Digg..."
        '
        'OhlohToolStripMenuItem
        '
        Me.OhlohToolStripMenuItem.Name = "OhlohToolStripMenuItem"
        Me.OhlohToolStripMenuItem.Size = New System.Drawing.Size(171, 22)
        Me.OhlohToolStripMenuItem.Text = "&Ohloh..."
        '
        'WakoopaToolStripMenuItem
        '
        Me.WakoopaToolStripMenuItem.Name = "WakoopaToolStripMenuItem"
        Me.WakoopaToolStripMenuItem.Size = New System.Drawing.Size(171, 22)
        Me.WakoopaToolStripMenuItem.Text = "&Wakoopa..."
        '
        'ProjectHomeToolStripMenuItem
        '
        Me.ProjectHomeToolStripMenuItem.Name = "ProjectHomeToolStripMenuItem"
        Me.ProjectHomeToolStripMenuItem.Size = New System.Drawing.Size(186, 22)
        Me.ProjectHomeToolStripMenuItem.Text = "Project &Home..."
        '
        'VersionHistoryToolStripMenuItem1
        '
        Me.VersionHistoryToolStripMenuItem1.Name = "VersionHistoryToolStripMenuItem1"
        Me.VersionHistoryToolStripMenuItem1.Size = New System.Drawing.Size(186, 22)
        Me.VersionHistoryToolStripMenuItem1.Text = "&Version History..."
        '
        'ToolStripSeparator9
        '
        Me.ToolStripSeparator9.Name = "ToolStripSeparator9"
        Me.ToolStripSeparator9.Size = New System.Drawing.Size(183, 6)
        '
        'tsmiDonate
        '
        Me.tsmiDonate.Name = "tsmiDonate"
        Me.tsmiDonate.Size = New System.Drawing.Size(186, 22)
        Me.tsmiDonate.Text = "&Donate..."
        '
        'ToolStripSeparator21
        '
        Me.ToolStripSeparator21.Name = "ToolStripSeparator21"
        Me.ToolStripSeparator21.Size = New System.Drawing.Size(183, 6)
        '
        'SubmitDebugReportToolStripMenuItem
        '
        Me.SubmitDebugReportToolStripMenuItem.Image = Global.iTSfv.My.Resources.Resources.email
        Me.SubmitDebugReportToolStripMenuItem.Name = "SubmitDebugReportToolStripMenuItem"
        Me.SubmitDebugReportToolStripMenuItem.Size = New System.Drawing.Size(186, 22)
        Me.SubmitDebugReportToolStripMenuItem.Text = "Submit &Bug Report"
        '
        'CheckForUpdatesToolStripMenuItem1
        '
        Me.CheckForUpdatesToolStripMenuItem1.Image = Global.iTSfv.My.Resources.Resources.world
        Me.CheckForUpdatesToolStripMenuItem1.Name = "CheckForUpdatesToolStripMenuItem1"
        Me.CheckForUpdatesToolStripMenuItem1.Size = New System.Drawing.Size(186, 22)
        Me.CheckForUpdatesToolStripMenuItem1.Text = "&Check for Updates..."
        '
        'miAbout
        '
        Me.miAbout.Name = "miAbout"
        Me.miAbout.Size = New System.Drawing.Size(186, 22)
        Me.miAbout.Text = "&About..."
        '
        'bwDiscsBrowserInfo
        '
        '
        'bwFS
        '
        '
        'tmrAddMusicAuto
        '
        Me.tmrAddMusicAuto.Enabled = True
        Me.tmrAddMusicAuto.Interval = 900000
        '
        'chkStartFinish
        '
        Me.chkStartFinish.AutoSize = True
        Me.chkStartFinish.Location = New System.Drawing.Point(452, 28)
        Me.chkStartFinish.Name = "chkStartFinish"
        Me.chkStartFinish.Size = New System.Drawing.Size(106, 17)
        Me.chkStartFinish.TabIndex = 12
        Me.chkStartFinish.Text = "Start/Finish Time"
        Me.chkStartFinish.UseVisualStyleBackColor = True
        '
        'chkDiscNumber
        '
        Me.chkDiscNumber.Location = New System.Drawing.Point(17, 74)
        Me.chkDiscNumber.Name = "chkDiscNumber"
        Me.chkDiscNumber.Size = New System.Drawing.Size(94, 17)
        Me.chkDiscNumber.TabIndex = 1
        Me.chkDiscNumber.Text = "Disk Number"
        Me.chkDiscNumber.UseVisualStyleBackColor = True
        '
        'bwWatcher
        '
        '
        'chkDiscComplete
        '
        Me.chkDiscComplete.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkDiscComplete.AutoSize = True
        Me.chkDiscComplete.Location = New System.Drawing.Point(167, 11)
        Me.chkDiscComplete.Name = "chkDiscComplete"
        Me.chkDiscComplete.Size = New System.Drawing.Size(213, 17)
        Me.chkDiscComplete.TabIndex = 18
        Me.chkDiscComplete.Text = "Group Selected Tracks as a single Disc"
        Me.chkDiscComplete.UseVisualStyleBackColor = True
        '
        'ssAppDisc
        '
        Me.ssAppDisc.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1, Me.sBarDisc, Me.pBarDiscs})
        Me.ssAppDisc.Location = New System.Drawing.Point(0, 518)
        Me.ssAppDisc.Name = "ssAppDisc"
        Me.ssAppDisc.Size = New System.Drawing.Size(784, 22)
        Me.ssAppDisc.SizingGrip = False
        Me.ssAppDisc.TabIndex = 19
        Me.ssAppDisc.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripStatusLabel1.Image = Global.iTSfv.My.Resources.Resources.info
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(16, 17)
        Me.ToolStripStatusLabel1.Text = "ToolStripStatusLabel1"
        '
        'sBarDisc
        '
        Me.sBarDisc.AutoSize = False
        Me.sBarDisc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.sBarDisc.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.sBarDisc.Name = "sBarDisc"
        Me.sBarDisc.Size = New System.Drawing.Size(651, 17)
        Me.sBarDisc.Spring = True
        Me.sBarDisc.Text = "Ready."
        Me.sBarDisc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pBarDiscs
        '
        Me.pBarDiscs.Name = "pBarDiscs"
        Me.pBarDiscs.Size = New System.Drawing.Size(100, 16)
        '
        'bwTimers
        '
        Me.bwTimers.WorkerReportsProgress = True
        '
        'lbVerbose
        '
        Me.lbVerbose.BackColor = System.Drawing.SystemColors.Control
        Me.lbVerbose.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lbVerbose.FormattingEnabled = True
        Me.lbVerbose.Location = New System.Drawing.Point(3, 372)
        Me.lbVerbose.Name = "lbVerbose"
        Me.lbVerbose.ScrollAlwaysVisible = True
        Me.lbVerbose.Size = New System.Drawing.Size(761, 105)
        Me.lbVerbose.TabIndex = 20
        '
        'bwQueueFiles
        '
        Me.bwQueueFiles.WorkerReportsProgress = True
        '
        'tlpMain
        '
        Me.tlpMain.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tlpMain.ColumnCount = 1
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpMain.Controls.Add(Me.TableLayoutPanel2, 0, 1)
        Me.tlpMain.Controls.Add(Me.lbVerbose, 0, 2)
        Me.tlpMain.Controls.Add(Me.tcTabs, 0, 0)
        Me.tlpMain.Location = New System.Drawing.Point(9, 27)
        Me.tlpMain.Name = "tlpMain"
        Me.tlpMain.RowCount = 3
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.tlpMain.Size = New System.Drawing.Size(767, 480)
        Me.tlpMain.TabIndex = 21
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 4
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 226.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 153.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 225.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.chkDiscComplete, 1, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.btnValidateSelected, 2, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.btnStatistics, 3, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.btnStop, 0, 0)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(3, 335)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 1
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(761, 31)
        Me.TableLayoutPanel2.TabIndex = 0
        '
        'btnStatistics
        '
        Me.btnStatistics.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnStatistics.AutoSize = True
        Me.btnStatistics.ImageKey = "chart_bar.png"
        Me.btnStatistics.ImageList = Me.ilTabs
        Me.btnStatistics.Location = New System.Drawing.Point(539, 5)
        Me.btnStatistics.Name = "btnStatistics"
        Me.btnStatistics.Size = New System.Drawing.Size(219, 23)
        Me.btnStatistics.TabIndex = 10
        Me.btnStatistics.Text = "Display iTunes Music Library Statistics..."
        Me.btnStatistics.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.btnStatistics.UseVisualStyleBackColor = True
        '
        'btnStop
        '
        Me.btnStop.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnStop.AutoSize = True
        Me.btnStop.Enabled = False
        Me.btnStop.Image = Global.iTSfv.My.Resources.Resources._stop
        Me.btnStop.Location = New System.Drawing.Point(3, 5)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.Size = New System.Drawing.Size(64, 23)
        Me.btnStop.TabIndex = 8
        Me.btnStop.Text = "&Stop..."
        Me.btnStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.btnStop.UseVisualStyleBackColor = True
        '
        'frmMain
        '
        Me.AcceptButton = Me.btnValidateSelected
        Me.AllowDrop = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.ContextMenuStrip = Me.cmsApp
        Me.Controls.Add(Me.ssAppDisc)
        Me.Controls.Add(Me.ssAppTrack)
        Me.Controls.Add(Me.msApp)
        Me.Controls.Add(Me.tlpMain)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.msApp
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "iTSfv"
        Me.ssAppTrack.ResumeLayout(False)
        Me.ssAppTrack.PerformLayout()
        Me.cmsApp.ResumeLayout(False)
        Me.tcTabs.ResumeLayout(False)
        Me.tpSettings.ResumeLayout(False)
        Me.tcValidate.ResumeLayout(False)
        Me.tpChecks.ResumeLayout(False)
        Me.tpChecks.PerformLayout()
        Me.tpEditTracks.ResumeLayout(False)
        Me.tpEditTracks.PerformLayout()
        Me.tpEditLibrary.ResumeLayout(False)
        Me.tpEditLibrary.PerformLayout()
        Me.tpFileSystem.ResumeLayout(False)
        Me.tpFileSystem.PerformLayout()
        Me.tpSelectedTracks.ResumeLayout(False)
        Me.tcSelectedTracks.ResumeLayout(False)
        Me.tpEditor.ResumeLayout(False)
        Me.tpEditor.PerformLayout()
        CType(Me.nudTrimChar, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gbWriteTags.ResumeLayout(False)
        Me.gbWriteTags.PerformLayout()
        Me.tpSTClipboard.ResumeLayout(False)
        Me.tpSTClipboard.PerformLayout()
        Me.gbClipBoardTags.ResumeLayout(False)
        Me.gbClipBoardTags.PerformLayout()
        Me.tpSTCheat.ResumeLayout(False)
        Me.gbOffset.ResumeLayout(False)
        CType(Me.nudOffsetTrackNum, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gbOverride.ResumeLayout(False)
        Me.gbOverride.PerformLayout()
        CType(Me.nudRatingOverride, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudPlayedCountOverride, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpSTExport.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.tpExplorer.ResumeLayout(False)
        Me.tcExplorer.ResumeLayout(False)
        Me.tpExplorerFiles.ResumeLayout(False)
        Me.tpExplorerFiles.PerformLayout()
        Me.cmsFiles.ResumeLayout(False)
        Me.tpExplorerActivity.ResumeLayout(False)
        Me.tpExplorerActivity.PerformLayout()
        Me.tpDiscsBrowser.ResumeLayout(False)
        Me.cmsDiscs.ResumeLayout(False)
        CType(Me.pbArtwork, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpBackupRestore.ResumeLayout(False)
        Me.tcTags.ResumeLayout(False)
        Me.tpTagsBackupRestore.ResumeLayout(False)
        Me.gbBackupTags.ResumeLayout(False)
        Me.gbBackupTags.PerformLayout()
        Me.gbRestoreTags.ResumeLayout(False)
        Me.gbRestoreTags.PerformLayout()
        Me.tpTagsRecover.ResumeLayout(False)
        Me.gbBrowsePrevLib.ResumeLayout(False)
        Me.gbBrowsePrevLib.PerformLayout()
        Me.tpOneTouch.ResumeLayout(False)
        Me.tcOneTouch.ResumeLayout(False)
        Me.tpAdvGeneral.ResumeLayout(False)
        Me.tpAdvGeneral.PerformLayout()
        Me.tpAdvTracks.ResumeLayout(False)
        Me.tpAdvTracks.PerformLayout()
        Me.tpAdvLibrary.ResumeLayout(False)
        Me.tpAdvLibrary.PerformLayout()
        Me.tpAdvFilesystem.ResumeLayout(False)
        Me.tpAdvFilesystem.PerformLayout()
        Me.tpSchedule.ResumeLayout(False)
        Me.tpSchedule.PerformLayout()
        Me.msApp.ResumeLayout(False)
        Me.msApp.PerformLayout()
        Me.ssAppDisc.ResumeLayout(False)
        Me.ssAppDisc.PerformLayout()
        Me.tlpMain.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnValidateLibrary As System.Windows.Forms.Button
    Friend WithEvents chkEditCopyArtistToAlbumArtist As System.Windows.Forms.CheckBox
    Friend WithEvents chkCheckArtwork As System.Windows.Forms.CheckBox
    Friend WithEvents ssAppTrack As System.Windows.Forms.StatusStrip
    Friend WithEvents sBarTrack As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents chkDeleteTracksNotInHDD As System.Windows.Forms.CheckBox
    Friend WithEvents cmsApp As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents OpenTracksReportToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents VersionHistoryToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents chkWinExportArtwork As System.Windows.Forms.CheckBox
    Friend WithEvents chkResume As System.Windows.Forms.CheckBox
    Friend WithEvents btnStop As System.Windows.Forms.Button
    Friend WithEvents tmrFormClose As System.Windows.Forms.Timer
    Friend WithEvents tcTabs As System.Windows.Forms.TabControl
    Friend WithEvents tpSettings As System.Windows.Forms.TabPage
    Friend WithEvents tpSelectedTracks As System.Windows.Forms.TabPage
    Friend WithEvents chkName As System.Windows.Forms.CheckBox
    Friend WithEvents chkAlbum As System.Windows.Forms.CheckBox
    Friend WithEvents chkArtist As System.Windows.Forms.CheckBox
    Friend WithEvents gbWriteTags As System.Windows.Forms.GroupBox
    Friend WithEvents chkCheckTrackNum As System.Windows.Forms.CheckBox
    Friend WithEvents lblWith As System.Windows.Forms.Label
    Friend WithEvents bwApp As System.ComponentModel.BackgroundWorker
    Friend WithEvents chkImportArtwork As System.Windows.Forms.CheckBox
    Friend WithEvents chkWinMakeReadOnly As System.Windows.Forms.CheckBox
    Friend WithEvents ttApp As System.Windows.Forms.ToolTip
    Friend WithEvents btnRecover As System.Windows.Forms.Button
    Friend WithEvents gbBrowsePrevLib As System.Windows.Forms.GroupBox
    Friend WithEvents txtXmlLibPath As System.Windows.Forms.TextBox
    Friend WithEvents tpExplorer As System.Windows.Forms.TabPage
    Friend WithEvents btnFindNewFiles As System.Windows.Forms.Button
    Friend WithEvents lbFiles As System.Windows.Forms.ListBox
    Friend WithEvents chkReplaceWithNewKind As System.Windows.Forms.CheckBox
    Friend WithEvents chkItunesStoreStandard As System.Windows.Forms.CheckBox
    Friend WithEvents tpDiscsBrowser As System.Windows.Forms.TabPage
    Friend WithEvents btnCreatePlaylistAlbum As System.Windows.Forms.Button
    Friend WithEvents lbDiscs As System.Windows.Forms.ListBox
    Friend WithEvents pbArtwork As System.Windows.Forms.PictureBox
    Friend WithEvents tpBackupRestore As System.Windows.Forms.TabPage
    Friend WithEvents btnRatingsRestore As System.Windows.Forms.Button
    Friend WithEvents gbRestoreTags As System.Windows.Forms.GroupBox
    Friend WithEvents txtRatingsRestorePath As System.Windows.Forms.TextBox
    Friend WithEvents gbBackupTags As System.Windows.Forms.GroupBox
    Friend WithEvents btnRatingsBackup As System.Windows.Forms.Button
    Friend WithEvents txtRatingsBackupPath As System.Windows.Forms.TextBox
    Friend WithEvents LogFilesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenNoniTSStandardTrackListwToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenWarningsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenAlbumsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TrackCountUpdatedTracksToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents chkCheckLyrics As System.Windows.Forms.CheckBox
    Friend WithEvents TracksWithNoLyricsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CheckForUpdatesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tmrSecond As System.Windows.Forms.Timer
    Friend WithEvents chkRenameFile As System.Windows.Forms.CheckBox
    Friend WithEvents TracksThatArtworkWasAddedToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OptionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MusicFolderActivityToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenMusicFolderToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents chkEditTrackCountEtc As System.Windows.Forms.CheckBox
    Friend WithEvents cboDecompileOptions As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents tcSelectedTracks As System.Windows.Forms.TabControl
    Friend WithEvents tpEditor As System.Windows.Forms.TabPage
    Friend WithEvents cboArtistsDecompiled As System.Windows.Forms.ComboBox
    Friend WithEvents chkDeleteNonMusicFolderTracks As System.Windows.Forms.CheckBox
    Friend WithEvents chkLibraryAdjustRatings As System.Windows.Forms.CheckBox
    Friend WithEvents btnAdjustRatings As System.Windows.Forms.Button
    Friend WithEvents nudOffsetTrackNum As System.Windows.Forms.NumericUpDown
    Friend WithEvents btnOffsetTrackNum As System.Windows.Forms.Button
    Friend WithEvents btnValidateAlbum As System.Windows.Forms.Button
    Friend WithEvents pbarTrack As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents chkEditEQbyGenre As System.Windows.Forms.CheckBox
    Friend WithEvents tpSTClipboard As System.Windows.Forms.TabPage
    Friend WithEvents gbClipBoardTags As System.Windows.Forms.GroupBox
    Friend WithEvents btnClipboard As System.Windows.Forms.Button
    Friend WithEvents tpOneTouch As System.Windows.Forms.TabPage
    Friend WithEvents btnSynchroclean As System.Windows.Forms.Button
    Friend WithEvents tpSTCheat As System.Windows.Forms.TabPage
    Friend WithEvents chkPlayedDateOverride As System.Windows.Forms.CheckBox
    Friend WithEvents nudPlayedCountOverride As System.Windows.Forms.NumericUpDown
    Friend WithEvents chkPlayedCountOverride As System.Windows.Forms.CheckBox
    Friend WithEvents btnOverride As System.Windows.Forms.Button
    Friend WithEvents dtpPlayedDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents niTray As System.Windows.Forms.NotifyIcon
    Friend WithEvents tpSchedule As System.Windows.Forms.TabPage
    Friend WithEvents chkSheduleAdjustRating As System.Windows.Forms.CheckBox
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TracksWithMultipleArtworkToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents gbOffset As System.Windows.Forms.GroupBox
    Friend WithEvents gbOverride As System.Windows.Forms.GroupBox
    Friend WithEvents chkScheduleFindNewFilesHDD As System.Windows.Forms.CheckBox
    Friend WithEvents TaskToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AdjustRatingsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CopyTrackInfoToClipboardToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents QuickValidationToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnValidateSelected As System.Windows.Forms.Button
    Friend WithEvents btnClearFilesListBox As System.Windows.Forms.Button
    Friend WithEvents rbSelectedTracks As System.Windows.Forms.RadioButton
    Friend WithEvents rbLibrary As System.Windows.Forms.RadioButton
    Friend WithEvents btnReplaceTracks As System.Windows.Forms.Button
    Friend WithEvents chkAlbumArtist As System.Windows.Forms.CheckBox
    Friend WithEvents chkValidate As System.Windows.Forms.CheckBox
    Friend WithEvents btnBrowseAlbum As System.Windows.Forms.Button
    Friend WithEvents SendToSystemTrayToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnStatistics As System.Windows.Forms.Button
    Friend WithEvents chkVistaThumbnailFix As System.Windows.Forms.CheckBox
    Friend WithEvents tcValidate As System.Windows.Forms.TabControl
    Friend WithEvents tpChecks As System.Windows.Forms.TabPage
    Friend WithEvents tpEditTracks As System.Windows.Forms.TabPage
    Friend WithEvents tpEditLibrary As System.Windows.Forms.TabPage
    Friend WithEvents tpFileSystem As System.Windows.Forms.TabPage
    Friend WithEvents chkCheckArtworkLowRes As System.Windows.Forms.CheckBox
    Friend WithEvents TracksWithLowResolutionArtworkToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TracksWithNoArtworkToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents chkMultiArtworkRemove As System.Windows.Forms.CheckBox
    Friend WithEvents TracksThatRatingWasAdjustedToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Friend WithEvents msApp As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FoldersToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LogsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents miAbout As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents miJobs As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PlayFirstTrackToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents VersionHistoryToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ValidateLast100TracksToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenTracksReportToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CheckForUpdatesToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SupportForumsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HydrogenAudioToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ILoungeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SourceForgeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator8 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator9 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents miSynchroclean As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents miSendToTray As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ValidateITunesMusicLibraryToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents miManual As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents miAdjustRatings As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator6 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents miStatistics As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents BrowseMusicFolderToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents miValidateSelected As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator7 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents AlbumsWithInconsecutiveTrackNumbersToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TracksThatRatingWasAdjustedToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TracksThatTrackCountWasUpdatedToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator14 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator13 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator12 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ErrorsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents WarningsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TracksNotITunesStoreCompliantToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents BrowseLogsFolderToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tpSTExport As System.Windows.Forms.TabPage
    Friend WithEvents cboExportFilePattern As System.Windows.Forms.ComboBox
    Friend WithEvents btnCopyTo As System.Windows.Forms.Button
    Friend WithEvents tcExplorer As System.Windows.Forms.TabControl
    Friend WithEvents tpExplorerFiles As System.Windows.Forms.TabPage
    Friend WithEvents AddNewFilesNotInITunesMusicLibraryToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents BrowseITMSArtworksFolderToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DebugToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents CreatePlaylistOfSelectedTracksToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents chkStrict As System.Windows.Forms.CheckBox
    Friend WithEvents bwDiscsBrowserInfo As System.ComponentModel.BackgroundWorker
    Friend WithEvents ToolsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TrackReplaceAssistantToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator10 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents miToolsOptions As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator15 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BrowseTemporaryFiToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents cmsDiscs As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ValidateDiscToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ShowDiscInWindowsExplroerToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CreatePlaylistToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmCopyTracklist As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PlayDiscInITunesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator16 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents GoogleSearchToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ValidateSelectedTracksToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FileValidatorToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents cboClipboardPattern As System.Windows.Forms.ComboBox
    Friend WithEvents chkWinExportPlaylist As System.Windows.Forms.CheckBox
    Friend WithEvents chkCheckEmbeddedArtwork As System.Windows.Forms.CheckBox
    Friend WithEvents ValidateToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents chkExportIndex As System.Windows.Forms.CheckBox
    Friend WithEvents bwFS As System.ComponentModel.BackgroundWorker
    Friend WithEvents tsmAddFolderToLib As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents chkRemoveNull As System.Windows.Forms.CheckBox
    Friend WithEvents chkValidationPlaylists As System.Windows.Forms.CheckBox
    Friend WithEvents DeleteDeadOrForeignTracksToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator17 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents chkUpdateInfoFromFile As System.Windows.Forms.CheckBox
    Friend WithEvents chkImportLyrics As System.Windows.Forms.CheckBox
    Friend WithEvents chkExportLyrics As System.Windows.Forms.CheckBox
    Friend WithEvents ArtworkToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TracksThatArtworkIsLowResolutionToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TracksThatArtworkWasAddedToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator11 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents TracksWithmultipleArtworkToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TracksWithoutArtworkToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LyricsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TracksWithoutLyricsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TracksLyricsWereAddedToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SettingsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SimpleWordsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ReplaceWordsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SkipWordsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SettingsFolderToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents chkSchValidateLibrary As System.Windows.Forms.CheckBox
    Friend WithEvents btnSchRun As System.Windows.Forms.Button
    Friend WithEvents chkAddFile As System.Windows.Forms.CheckBox
    Friend WithEvents tpExplorerActivity As System.Windows.Forms.TabPage
    Friend WithEvents txtActivity As System.Windows.Forms.TextBox
    Friend WithEvents tmrAddMusicAuto As System.Windows.Forms.Timer
    Friend WithEvents AlwaysOnTopToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents chkTagRemove As System.Windows.Forms.CheckBox
    Friend WithEvents chkDecompile As System.Windows.Forms.CheckBox
    Friend WithEvents chkReplaceTextInTags As System.Windows.Forms.CheckBox
    Friend WithEvents chkCapitalizeFirstLetter As System.Windows.Forms.CheckBox
    Friend WithEvents cboTrimDirection As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents nudTrimChar As System.Windows.Forms.NumericUpDown
    Friend WithEvents chkTrimChar As System.Windows.Forms.CheckBox
    Friend WithEvents chkGenre As System.Windows.Forms.CheckBox
    Friend WithEvents cboReplace As System.Windows.Forms.ComboBox
    Friend WithEvents cboFind As System.Windows.Forms.ComboBox
    'Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
    Friend WithEvents chkStartFinish As System.Windows.Forms.CheckBox
    'Friend WithEvents CheckBox3 As System.Windows.Forms.CheckBox
    'Friend WithEvents CheckBox4 As System.Windows.Forms.CheckBox
    'Friend WithEvents CheckBox5 As System.Windows.Forms.CheckBox
    'Friend WithEvents CheckBox6 As System.Windows.Forms.CheckBox
    'Friend WithEvents CheckBox7 As System.Windows.Forms.CheckBox
    'Friend WithEvents CheckBox8 As System.Windows.Forms.CheckBox
    'Friend WithEvents CheckBox9 As System.Windows.Forms.CheckBox
    'Friend WithEvents CheckBox10 As System.Windows.Forms.CheckBox
    'Friend WithEvents CheckBox11 As System.Windows.Forms.CheckBox
    'Friend WithEvents CheckBox12 As System.Windows.Forms.CheckBox
    Friend WithEvents chkDiscNumber As System.Windows.Forms.CheckBox
    Friend WithEvents tcTags As System.Windows.Forms.TabControl
    Friend WithEvents tpTagsBackupRestore As System.Windows.Forms.TabPage
    Friend WithEvents tpTagsRecover As System.Windows.Forms.TabPage
    Friend WithEvents cmsFiles As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ShowInWindowsExplorerToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RemoveFromListToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents chkRemoveLowResArtwork As System.Windows.Forms.CheckBox
    Friend WithEvents bwWatcher As System.ComponentModel.BackgroundWorker
    Friend WithEvents chkCheckFoldersWithoutArtwork As System.Windows.Forms.CheckBox
    Friend WithEvents tsmShowApp As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator18 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents chkDiscComplete As System.Windows.Forms.CheckBox
    Friend WithEvents CapitalWordsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator19 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BrowseSettingsFolderToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator20 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BrowseLogsFolderToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmiDonate As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator21 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents btnSyncLastFM As System.Windows.Forms.Button
    Friend WithEvents chkWriteGenre As System.Windows.Forms.CheckBox
    Friend WithEvents tsmUpdatePlayedCount As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AlbumArtDownloaderToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents chkCheckBPM As System.Windows.Forms.CheckBox
    Friend WithEvents tcOneTouch As System.Windows.Forms.TabControl
    Friend WithEvents tpAdvGeneral As System.Windows.Forms.TabPage
    Friend WithEvents tpAdvLibrary As System.Windows.Forms.TabPage
    Friend WithEvents tpAdvTracks As System.Windows.Forms.TabPage
    Friend WithEvents btnRemoveDuplicates As System.Windows.Forms.Button
    Friend WithEvents LibraryToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DuplicateTracksToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmTrackTags As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmTrackTagsBPM As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmTrackTagsRefreshed As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FileSystemToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmMusicFolderActivity As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator22 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents FoldersWithoutArtworkToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ArtworkSearchToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SelectedTracksToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SavePlaylistOfSelectedTracksAsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmiSelectedTracksValidate As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator23 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents SearchArtworkToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CopyInfoToClipboardToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator24 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents chkCheckMetatag As System.Windows.Forms.CheckBox
    Friend WithEvents Mp3tagToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TrackMetatagVersionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Mp3tagSelectedTracks As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Mp3tagSelectedDisc As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmLyricsViewer As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CheckToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EditTracksToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LibraryToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmSelectedTracksValidateFS As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnValidateTracksChecks As System.Windows.Forms.Button
    Friend WithEvents btnValidateSelectedTracks As System.Windows.Forms.Button
    Friend WithEvents btnValidateSelectedTracksLibrary As System.Windows.Forms.Button
    Friend WithEvents btnValidateSelectedTracksFolder As System.Windows.Forms.Button
    Friend WithEvents SubmitDebugReportToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tpAdvFilesystem As System.Windows.Forms.TabPage
    Friend WithEvents btnAdvDeleteEmptyFolders As System.Windows.Forms.Button
    Friend WithEvents FoldersWithOneFileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FoldersWithoutAudioToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator25 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ssAppDisc As System.Windows.Forms.StatusStrip
    Friend WithEvents sBarDisc As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents pBarDiscs As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents tsmItunesArtworkGrabber As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents btnArtworkExport As System.Windows.Forms.Button
    Friend WithEvents bwTimers As System.ComponentModel.BackgroundWorker
    Friend WithEvents tsmTiunesStoreArtworkGrabSelected As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents chkClipboardSort As System.Windows.Forms.CheckBox
    Friend WithEvents btnBatchArtworkGrab As System.Windows.Forms.Button
    Friend WithEvents chkConvertArtworkJPG As System.Windows.Forms.CheckBox
    Friend WithEvents tsmiArtworkConverted As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lblClipboard As System.Windows.Forms.TextBox
    Friend WithEvents chkAppendChar As System.Windows.Forms.CheckBox
    Friend WithEvents cboAppendChar As System.Windows.Forms.ComboBox
    Friend WithEvents txtAppend As System.Windows.Forms.TextBox
    Friend WithEvents ilTabs As System.Windows.Forms.ImageList
    Friend WithEvents WakoopaToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents BetaVersionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmiSearch As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator26 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents VerboseModeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lbVerbose As System.Windows.Forms.ListBox
    Friend WithEvents OhlohToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SVNRepositoryToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator27 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents DiggToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents bwQueueFiles As System.ComponentModel.BackgroundWorker
    Friend WithEvents ToolStripStatusLabel2 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents sTrackProgress As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents chkRatingsImportPOPM As System.Windows.Forms.CheckBox
    Friend WithEvents tlpMain As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnWritePOPM As System.Windows.Forms.Button
    Friend WithEvents btnImportPOPM As System.Windows.Forms.Button
    Friend WithEvents chkExportArtwork As System.Windows.Forms.CheckBox
    Friend WithEvents IgnoreWordsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmSetInfo As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents chkPlayedCountImportPCNT As System.Windows.Forms.CheckBox
    Friend WithEvents GoogleGroupsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ProjectHomeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents nudRatingOverride As System.Windows.Forms.NumericUpDown
    Friend WithEvents chkRatingOverride As System.Windows.Forms.CheckBox
    Friend WithEvents SaveStatisticsFileAsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents chkEditCopyAlbumArtistToSortArtist As System.Windows.Forms.CheckBox
    Friend WithEvents chkRemoveComments As System.Windows.Forms.CheckBox
    Friend WithEvents chkRemoveLyrics As System.Windows.Forms.CheckBox
End Class

