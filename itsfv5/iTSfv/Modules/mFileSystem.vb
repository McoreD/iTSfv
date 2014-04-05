Imports System.IO
Imports iTunesLib
Imports System.Configuration
Imports ICSharpCode.SharpZipLib
Imports System.ComponentModel
Imports iTSfv.cBwJob
Imports System.Collections.Specialized
Imports System.Net

Public Module mFileSystem

    ''* Module Responsibility for ALL functionality related to HDD input / output

    Public Const TEMP_TRACK_ARTWORK_NAME As String = "TrackArtwork"
    Public Const TEMP_CACHE_ARTWORK_NAME As String = "XmlArtwork"
    Public Const TEMP_FOLDER_ARTWORK_NAME As String = "FolderArtwork"
    Public Const TEMP_STORE_ARTWORK_NAME As String = "StoreArtwork"

    Public Const DLG_FILTER_URLTOCOVERART As String = "Vista Media Center Artwork Dictionary (UrlToCoverArt.dat)|UrlToCoverArt.dat"
    Public Const DLG_FILTER_STATS As String = "iTSfv Statistics Files (*-stats.cache)|*-stats.cache"
    Public Const DLG_FILTER_CSS As String = "Cascading Style Sheet (*.css)|*.css"
    Public Const DLG_BACKUP_RESTORE As String = "iTSfv Tags XML files (*.xml)|*.xml|iTSfv Tags Cache (*.tags-cache)|*.tags-cache"

    Friend mSbWarnings As New System.Text.StringBuilder ' global error writer
    Public mSbDebug As New System.Text.StringBuilder ' global debug writer
    Public mFilePathDebugLog As String = String.Format(mTempFolder + "debug-{0}-log.txt", Now.ToString("yyyyMMdd"))

    Private mCaptialWords As String() = {"EP", "DJ", "CDS", "OST", "CDM", "AC/DC", "*NSYNC", _
                                "II.", "III.", "IV.", "VI.", "VII.", "VIII"}
    Private mSimpleWords As String() = {"at", "by", "for", "from", "in", "into", "of", "off", _
                                        "on", "onto", "out", "over", "to", "up", "with", "and", _
                                        "but", "or", "nor", "a", "an", "the"}
    Private mSkipWords As String() = {"CDM", "CDS", "[Single]", "OST"}
    Private mIgnoreWords As String() = {"US", "UK", "(US", "(UK", "Y34RZ3R0R3M1X3D", "II", "III", "IV"}

    ' configure when initializing dirs
    Friend mTempFolder As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar
    Friend mFilePathArtworkConverted As String
    Friend mFilePathArtworkRes As String
    Friend mFilePathListArtworkRes As String
    Friend mFilePathFoldersNoAudio As String
    Friend mFilePathFoldersNoArtwork As String
    Friend mFilePathFoldersOneFile As String
    Friend mFilePathMusicFolderActivity As String = String.Format(mTempFolder + "music-folder-{0}-activity.txt", Now.ToString("yyyyMMdd"))
    Friend mFilePathWarningsLog As String = String.Format(mTempFolder + "warnings_{0}_log.txt", Now.ToString("yyyyMM"))
    Friend mFilePathNoLyicsTracks As String
    Friend mFilePathTracksEditedByAlbumBrowser As String
    Friend mFilePathArtworkAdded As String
    Friend mFilePathConfigBackup As String
    Friend mFilePathDuplicateTracks As String
    Friend mFilePathTrackMetatags As String
    Friend mFilePathLyricsAdded As String
    Friend mFilePathNoArtwork As String
    Friend mFilePathTracksNoBPM As String
    Friend mFilePathEmbeddedArtwork As String
    Friend mFilePathExcludedFiles As String
    Friend mFilePathMultipleArtwork As String
    Friend mFilePathAlbumsInconsecutiveTracks As String
    Friend mFilePathNonItsTracks As String
    Friend mFilePathErrorsLog As String
    Friend mFilePathRatingsBR As String
    Friend mFilePathRatingsAdjusted As String
    Friend mFilePathReport As String
    Friend mFilePathReplaceWords As String
    Friend mFilePathTagsRefreshed As String
    Friend mFilePathSimpleWords As String
    Friend mFilePathCapitalWords As String
    Friend mFilePathSkipAlbumWords As String
    Friend mFilePathIgnoreWords As String

    ' lists
    Private mListExcludeFiles As New List(Of String) ' list of files already added

    Friend mListFileNotFound As New List(Of String)
    Friend mListFoldersNoArtwork As New List(Of String) ' list of folders with no artwork
    Friend mListFoldersNoFolderJpg As New List(Of String) ' list of files folder.jpg was added
    Friend mListFoldersNoAudio As New List(Of String)
    Friend mListFoldersOneFile As New List(Of String)
    Friend mListTrackMetatags As New List(Of cXmlTrack)
    Friend mListTagsRefreshed As New List(Of String)
    Friend mListDuplicateTracks As New List(Of String)
    Friend mListTracksNoBPM As New List(Of String)
    Friend mListTracksNonMusicFolder As New List(Of String)
    Friend mListTracksToDelete As New List(Of cXmlTrack)
    Friend mListTracksArtworkConverted As New List(Of String)

    Public ReadOnly Property mFileExtAudioITunes() As String()

        Get
            Return My.Settings.FormatsITunes.Trim.Split(CChar(","))
        End Get

    End Property

    Public ReadOnly Property mFileExtAudioWMP() As String()
        Get
            Return My.Settings.FormatsWMP.Trim.Split(CChar(","))
        End Get
    End Property

    Public ReadOnly Property mFileExtOtherAudio() As String()
        Get
            Return My.Settings.FormatsOtherAudio.Trim.Split(CChar(","))
        End Get
    End Property

    Public ReadOnly Property mFileExtArtwork() As String()
        Get
            Return My.Settings.FormatsImages.Trim.Split(CChar(","))
        End Get
    End Property

    Public Function mfOpenFileOrDirPath(ByVal fileOrDirPath As String, _
                     ByVal sBarLeft As ToolStripStatusLabel, _
                     ByVal ttApp As ToolTip, _
                     ByVal sBar As StatusStrip) As Boolean

        If File.Exists(fileOrDirPath) Or Directory.Exists(fileOrDirPath) Then
            Process.Start(fileOrDirPath)
            mfUpdateStatusBarText("Opened " & fileOrDirPath, True)
            Return True
        Else

            Dim fName As String = Path.GetFileName(fileOrDirPath)

            If fName IsNot Nothing Then
                fName = fName.Replace(Now.ToString("MM"), String.Empty)
                fName = fName.Replace(Now.ToString("dd"), String.Empty)
                fName = fName.Replace(Now.ToString("yyyy"), Now.ToString("yyyy") + "*")

                ' we look at most recent html file in logs folder
                Dim f As String() = Directory.GetFiles(My.Settings.LogsDir, fName, SearchOption.TopDirectoryOnly)
                If f.Length > 0 Then
                    Array.Sort(f)
                    Array.Reverse(f)
                    If File.Exists(f(0)) Then
                        Process.Start(f(0))
                        mfUpdateStatusBarText(mfGetTruncatedText("Opened " & f(0), ttApp, sBar), True)
                        Return True
                    End If
                End If
            End If

            mfUpdateStatusBarText(mfGetTruncatedText("Failed to open " & fileOrDirPath, ttApp, sBar), True)
            Return False

        End If

    End Function

    Public Function mfGetArtworkForTrack(ByVal track As IITFileOrCDTrack, Optional ByVal scanAll As Boolean = True) As String

        Dim jpgCustom As String = "Cover.jpg"

        Dim temp As String = ""

        If My.Settings.DefaultExArtworkFolder Then
            temp = mfGetFilePathFromPattern(IO.Path.GetDirectoryName(track.Location), _
                                            My.Settings.ArtworkFileNameEx, _
                                            track, scanAll)
        Else
            temp = My.Settings.FolderPathExArtwork + Path.DirectorySeparatorChar + fGetFileNameFromPattern(My.Settings.ArtworkFileNamePatternEx, track)
        End If

        If String.IsNullOrEmpty(temp) = False Then
            jpgCustom = temp
        End If

        Return jpgCustom

    End Function

    Public Function mfGetNewFilesFromHDD(ByVal bwApp As BackgroundWorker, ByVal lExtensions As String()) As List(Of String)

        Dim lListTracksLocationHdd As New List(Of String)

        For Each folderPath As String In mpMusicFolderPaths

            msAppendDebug("Looking in " & folderPath)

            If Directory.Exists(folderPath) Then

                Dim oDirs() As String = Directory.GetDirectories(folderPath)
                Dim lDirs As New List(Of String)
                lDirs.AddRange(oDirs)

                mProgressDiscsMax = lDirs.Count

                For Each oDir As String In lDirs

                    mfUpdateStatusBarText("Scanning " & oDir, False)

                    If bwApp.CancellationPending Then
                        Exit For
                    End If

                    mProgressDiscsCurrent += 1

                    If bScanMusicFolder(oDir) Then
                        Try
                            For Each fileExt As String In lExtensions
                                lListTracksLocationHdd.AddRange(Directory.GetFiles(oDir, "*." & fileExt, SearchOption.AllDirectories))
                            Next
                        Catch ex As Exception
                            msAppendWarnings(ex.Message + " while finding all new files")
                        End Try
                    Else
                        msAppendDebug("Excluding folder: " & oDir)
                    End If

                Next

            End If

        Next

        Return lListTracksLocationHdd

    End Function

    Private Function bScanMusicFolder(ByVal folderPath As String) As Boolean

        ' Function to see if the music folder should be scanned

        If My.Settings.ScanOnlyRecentFolders Then

            Dim di As New DirectoryInfo(folderPath)
            Dim ts As TimeSpan = Now - di.LastWriteTime

            If ts.Days > My.Settings.ScanLimitDays Then
                Return False
            End If

        End If

        If folderPath.EndsWith(Path.DirectorySeparatorChar) = False Then
            folderPath += Path.DirectorySeparatorChar
        End If

        For Each lloc As String In My.Settings.ExcludeMusicFolders
            If folderPath.Contains(lloc) Then
                Return False
            End If
        Next

        Return True

    End Function

    Public Sub msConfigureDirsFiles()

        Try

            If My.Settings.MusicFolderLocations Is Nothing Then
                My.Settings.MusicFolderLocations = New StringCollection
            End If

            If My.Settings.ExcludeMusicFolders Is Nothing Then
                My.Settings.ExcludeMusicFolders = New StringCollection
            End If

            If My.Settings.WMP_MusicFolderPaths Is Nothing Then
                My.Settings.WMP_MusicFolderPaths = New StringCollection
            End If

            If String.Empty = My.Settings.ArtworkFileNamePatternEx Then
                My.Settings.ArtworkFileNamePatternEx = "%AlbumArtist% (%Year%) - %Album%"
            End If

            If String.Empty = My.Settings.ArtworkFileNamePatternIm Then
                My.Settings.ArtworkFileNamePatternIm = "%AlbumArtist% - %Album%"
            End If

            Dim lLogDirOld As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + "iTSfv Logs" + Path.DirectorySeparatorChar

            ' determine folder paths for the first time
            If My.Settings.LogsDir = String.Empty OrElse My.Settings.LogsDir = lLogDirOld Then
                Dim docs As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                Dim lLogDir As String
                If docs.EndsWith(Path.DirectorySeparatorChar) Then
                    lLogDir = docs + APP_ABBR_NAME_IT + Path.DirectorySeparatorChar + "Logs" + Path.DirectorySeparatorChar
                Else
                    lLogDir = docs + Path.DirectorySeparatorChar + APP_ABBR_NAME_IT + Path.DirectorySeparatorChar + "Logs" + Path.DirectorySeparatorChar
                End If
                My.Settings.LogsDir = lLogDir
            End If

            If My.Settings.SettingsDir = String.Empty Then
                Dim docs As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                Dim lSettingsDir As String
                If docs.EndsWith(Path.DirectorySeparatorChar) Then
                    lSettingsDir = docs + APP_ABBR_NAME_IT + Path.DirectorySeparatorChar + "Settings" + Path.DirectorySeparatorChar
                Else
                    lSettingsDir = docs + Path.DirectorySeparatorChar + APP_ABBR_NAME_IT + Path.DirectorySeparatorChar + "Settings" + Path.DirectorySeparatorChar
                End If
                My.Settings.SettingsDir = lSettingsDir
            End If

            ' cannot Change default Artwork folder to All Users\Pictures because there is no
            ' default way to find that folder

            If My.Settings.ArtworkDir = String.Empty Then
                Dim pics As String = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
                Dim lArtworkDir As String
                If pics.EndsWith(Path.DirectorySeparatorChar) Then
                    lArtworkDir = pics + APP_ABBR_NAME_IT + Path.DirectorySeparatorChar
                Else
                    lArtworkDir = pics + Path.DirectorySeparatorChar + APP_ABBR_NAME_IT + Path.DirectorySeparatorChar
                End If
                My.Settings.ArtworkDir = lArtworkDir
            End If

            If My.Settings.TempDir = String.Empty Then
                Dim temp As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                Dim lTempDir As String
                If temp.EndsWith(Path.DirectorySeparatorChar) Then
                    lTempDir = temp + APP_ABBR_NAME_IT + Path.DirectorySeparatorChar
                Else
                    lTempDir = temp + Path.DirectorySeparatorChar + APP_ABBR_NAME_IT + Path.DirectorySeparatorChar
                End If
                My.Settings.TempDir = lTempDir
            End If

            If My.Settings.FolderPathImArtwork = String.Empty Then
                Dim songsDir As String = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)
                Dim customArtworkDirPath As String = String.Empty
                Dim artworkDir As String = "iTunes\Album Artwork\"
                If songsDir.EndsWith(Path.DirectorySeparatorChar) = True Then
                    customArtworkDirPath = songsDir + artworkDir + APP_ABBR_NAME_IT
                Else
                    customArtworkDirPath = songsDir + Path.DirectorySeparatorChar + artworkDir + APP_ABBR_NAME_IT
                End If
                My.Settings.FolderPathImArtwork = customArtworkDirPath
                If My.Settings.FolderPathExArtwork = String.Empty Then
                    My.Settings.FolderPathExArtwork = customArtworkDirPath
                End If
            End If

            If My.Settings.LyricsFolderPathEx = String.Empty Then
                Dim songsDir As String = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)
                Dim customDirPath As String = String.Empty
                Dim artworkDir As String = "iTunes\Lyrics\"
                customDirPath = songsDir + Path.DirectorySeparatorChar + artworkDir
                My.Settings.LyricsFolderPathEx = customDirPath
            End If

            If My.Settings.LyricsFolderPathIm = String.Empty Then
                My.Settings.LyricsFolderPathIm = My.Settings.LyricsFolderPathEx
            End If

            If Directory.Exists(My.Settings.LogsDir) = False Then
                Directory.CreateDirectory(My.Settings.LogsDir)
            End If
            If Directory.Exists(My.Settings.SettingsDir) = False Then
                Directory.CreateDirectory(My.Settings.SettingsDir)
            End If
            If Directory.Exists(My.Settings.ArtworkDir) = False Then
                Directory.CreateDirectory(My.Settings.ArtworkDir)
            End If
            If Directory.Exists(My.Settings.TempDir) = False Then
                Directory.CreateDirectory(My.Settings.TempDir)
            End If

            If Directory.Exists(My.Settings.FolderPathExArtwork) = False Then
                Directory.CreateDirectory(My.Settings.FolderPathExArtwork)
            End If

            Dim err As New System.Text.StringBuilder
            If Directory.Exists(lLogDirOld) Then
                ' move "iTSfv Logs" it into iTSfv\Logs
                Try
                    My.Computer.FileSystem.MoveDirectory(lLogDirOld, My.Settings.LogsDir)
                Catch ex As Exception
                    err.AppendLine(ex.Message)
                    err.AppendLine(ex.StackTrace)
                    err.AppendLine(ex.Source)
                End Try
            End If


            ''**************************
            ''* WMPfv / UrlToCoverArt.dat
            ''***************************
            If My.Settings.UrlToCoverArtLocation = String.Empty Then
                Dim fUrlToCoverArt As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft\eHome\Art Cache\UrlToCoverArt.dat")
                My.Settings.UrlToCoverArtLocation = fUrlToCoverArt
            End If

            ''**************************
            ''* WMPfv / Music Folder Path
            ''***************************
            If My.Settings.WMP_MusicFolderPath = String.Empty Then
                Dim regKey As Microsoft.Win32.RegistryKey = _
           Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\Microsoft\MediaPlayer\Preferences", True)
                If regKey.GetValue("TrackFoldersDirectories0") IsNot Nothing Then
                    My.Settings.WMP_MusicFolderPath = CStr(regKey.GetValue("TrackFoldersDirectories0"))
                    If (Not String.IsNullOrEmpty(My.Settings.WMP_MusicFolderPath)) Then
                        If My.Settings.WMP_MusicFolderPath.EndsWith(Path.DirectorySeparatorChar) = False Then
                            My.Settings.WMP_MusicFolderPath += My.Settings.WMP_MusicFolderPath + Path.DirectorySeparatorChar
                        End If
                    End If
                End If
            End If

            ''*****************
            ''* Log File Paths
            ''****************

            mFilePathArtworkConverted = String.Format(My.Settings.LogsDir + "tracks-{0}-artwork-converted.txt", Now.ToString("yyyyMM"))
            mFilePathAlbumsInconsecutiveTracks = My.Settings.LogsDir + String.Format("incomplete-{0}-albums.txt", Now.ToString("yyyyMM"))
            mFilePathLyricsAdded = String.Format(My.Settings.LogsDir + "tracks-{0}-lyrics-added.txt", Now.ToString("yyyyMM"))
            mFilePathArtworkAdded = String.Format(My.Settings.LogsDir + "tracks-{0}-artwork-added.txt", Now.ToString("yyyyMM"))
            mFilePathDebugLog = String.Format(My.Settings.LogsDir + "debug-{0}-log.txt", Now.ToString("yyyyMMdd"))
            mFilePathDuplicateTracks = String.Format(My.Settings.LogsDir + "tracks-{0}-duplicate.txt", Now.ToString("yyyyMM"))
            mFilePathEmbeddedArtwork = String.Format(My.Settings.LogsDir + "tracks-{0}-non-embedded-artwork.txt", Now.ToString("yyyyMM"))
            mFilePathFoldersNoAudio = String.Format(My.Settings.LogsDir + "folders-{0}-no-audio.txt", Now.ToString("yyyyMM"))
            mFilePathFoldersOneFile = String.Format(My.Settings.LogsDir + "folders-{0}-one-file.txt", Now.ToString("yyyyMM"))
            mFilePathErrorsLog = String.Format(My.Settings.LogsDir + "errors_{0}_log.txt", Now.ToString("yyyyMM"))
            mFilePathListArtworkRes = String.Format(My.Settings.LogsDir + "artwork-{0}-low-res.txt", Now.ToString("yyyyMM"))
            mFilePathTrackMetatags = String.Format(My.Settings.LogsDir + "track_{0}_metatags.txt", Now.ToString("yyyyMMdd"))
            mFilePathMultipleArtwork = String.Format(My.Settings.LogsDir + "tracks-{0}-multiple-artwork.txt", Now.ToString("yyyyMM"))
            mFilePathMusicFolderActivity = String.Format(My.Settings.LogsDir + "music-folder-{0}-activity.txt", Now.ToString("yyyyMMdd"))
            mFilePathTracksNoBPM = String.Format(My.Settings.LogsDir + "tracks-{0}-no-bpm.txt", Now.ToString("yyyyMM"))
            mFilePathNoArtwork = String.Format(My.Settings.LogsDir + "tracks-{0}-no-artwork.txt", Now.ToString("yyyyMM"))
            mFilePathFoldersNoArtwork = String.Format(My.Settings.LogsDir + "folders-{0}-without-artwork.txt", Now.ToString("yyyyMM"))
            mFilePathNoLyicsTracks = String.Format(My.Settings.LogsDir + "tracks-{0}-no-lyrics.txt", Now.ToString("yyyyMM"))
            mFilePathNonItsTracks = String.Format(My.Settings.LogsDir + "non-iTS-{0}-tracks.txt", Now.ToString("yyyyMMdd"))
            mFilePathRatingsAdjusted = String.Format(My.Settings.LogsDir + "track-{0}-ratings.log", Now.ToString("yyyyMMdd"))
            mFilePathRatingsBR = String.Format(My.Settings.LogsDir + "iTunes-{0}-Ratings.xml", Now.ToString("yyyyMMdd"))
            mFilePathReport = String.Format(My.Settings.LogsDir + "tracks-{0}-report.html", Now.ToString("yyyyMMddTHH"))
            mFilePathTagsRefreshed = String.Format(My.Settings.LogsDir + "tracks-{0}-tags-refreshed.log", Now.ToString("yyyyMMdd"))
            mFilePathTracksEditedByAlbumBrowser = String.Format(My.Settings.LogsDir + "track-count-{0}-updated.txt", Now.ToString("yyyyMM"))
            mFilePathWarningsLog = String.Format(My.Settings.LogsDir + "warnings_{0}_log.txt", Now.ToString("yyyyMM"))

            ''*****************
            ''* Setting Files
            ''****************
            mFilePathIgnoreWords = My.Settings.SettingsDir + "ignore-words.txt"
            mFilePathSkipAlbumWords = My.Settings.SettingsDir + "skip-album-words.txt"
            mFilePathSimpleWords = My.Settings.SettingsDir + "simple-words.txt"
            mFilePathCapitalWords = My.Settings.SettingsDir + "capital-words.txt"
            mFilePathReplaceWords = My.Settings.SettingsDir + "replace-words.txt"
            mFilePathConfigBackup = My.Settings.SettingsDir + "user.config"
            mFilePathExcludedFiles = My.Settings.SettingsDir + "excluded-files.txt"

            ''''''''''''''''''
            ' export settings
            ''''''''''''''''''

            If File.Exists(mFilePathIgnoreWords) = False Then

                Using sw As New StreamWriter(mFilePathIgnoreWords)
                    For Each word As String In mIgnoreWords
                        sw.WriteLine(word)
                    Next
                End Using

            End If

            If File.Exists(mFilePathCapitalWords) = False Then

                Using sw As New StreamWriter(mFilePathCapitalWords)
                    For Each word As String In mCaptialWords
                        sw.WriteLine(word)
                    Next
                End Using

            End If

            If File.Exists(mFilePathSimpleWords) = False Then
                Using sw As New StreamWriter(mFilePathSimpleWords)
                    For Each word As String In mSimpleWords
                        sw.WriteLine(word)
                    Next
                End Using
            End If

            If File.Exists(mFilePathSkipAlbumWords) = False Then

                Using sw As New StreamWriter(mFilePathSkipAlbumWords)
                    For Each word As String In mSkipWords
                        sw.WriteLine(word)
                    Next
                End Using

            End If

            If File.Exists(mFilePathReplaceWords) = False Then
                Using sw As New StreamWriter(mFilePathReplaceWords)
                    sw.WriteLine(mfGetText("replace-words.txt"))
                End Using
            End If

            Dim tgAdapter As New TreeGUI.cAdapter
            Dim lReportCSS As String = My.Settings.SettingsDir + tgAdapter.GetConfig.CssFileName
            Dim cssReplace As Boolean = False
            If File.Exists(lReportCSS) = False Then
                cssReplace = True
            Else
                Dim fi As New FileInfo(lReportCSS)
                If fi.Length = 0 Then
                    cssReplace = True
                End If
            End If
            If cssReplace = True Then
                Using sw As New StreamWriter(lReportCSS)
                    Dim tgHtml As New TreeGUI.cHtml
                    sw.WriteLine(tgHtml.fGetText(tgAdapter.GetConfig.CssFileName))
                End Using
            End If

            ''''''''''''''
            ' Write Errors
            ''''''''''''''
            If err.Length > 0 Then
                err.AppendLine("while relocating iTSfv Logs directory.")
                err.AppendLine("Please remove the old iTSfv Logs directory manually.")
                msAppendWarnings(err.ToString)
            End If

            ''''''''''''''''''''''
            ' Restore user.config
            ''''''''''''''''''''''
            If My.Settings.ConfigAutoRestore = True Then
                If File.Exists(mfConfigFilePath) = False AndAlso File.Exists(mFilePathConfigBackup) = True Then
                    mfConfigRestore(mFilePathConfigBackup)
                End If
            End If

        Catch ex As Exception
            Console.WriteLine(ex.ToString())
        End Try

    End Sub

    Public Sub mfExcludeFileAdd(ByVal filePath As String)
        If mListExcludeFiles.Contains(filePath) = False Then
            mListExcludeFiles.Add(filePath)
        End If
    End Sub

    Public Function mfWriteExcludedFiles() As Boolean

        Dim suc As Boolean = True

        If mListExcludeFiles.Count > 0 Then
            Try
                Using sw As New StreamWriter(mFilePathExcludedFiles, append:=True)
                    For Each f As String In mListExcludeFiles
                        sw.WriteLine(f)
                    Next
                End Using
            Catch ex As Exception
                suc = False
            End Try
        End If

        Return suc

    End Function

    Public Function mfGetExcludedFiles() As List(Of String)

        If mListExcludeFiles.Count = 0 Then
            If File.Exists(mFilePathExcludedFiles) Then
                mListExcludeFiles.AddRange(IO.File.ReadAllLines(mFilePathExcludedFiles))
            End If
        End If

        Return mListExcludeFiles

    End Function

    Public Function mfGetStream(ByVal url As String, ByVal err As String) As Stream

        Dim request As HttpWebRequest = CType(HttpWebRequest.Create(url), HttpWebRequest)
        Dim response As WebResponse = Nothing
        Try
            request.UserAgent = Application.ProductName + " " + Application.ProductVersion
            request.Timeout = My.Settings.TimeoutITMS
            Return request.GetResponse().GetResponseStream()
        Catch ex As Exception
            msAppendWarnings(String.Format("{0} {1}", ex.Message, err))
        End Try

        Return Nothing

    End Function

    Public Function mfGetWordsList(ByVal filePath As String) As List(Of String)

        ' can use IO.File.ReadAllLines(filePath) as well

        If File.Exists(filePath) = True Then
            Using sr As New StreamReader(filePath)
                Dim words As New List(Of String)
                Dim lines As String() = sr.ReadToEnd.Split(CChar(vbCrLf))
                For Each l As String In lines
                    l = l.Trim
                    If l.Length > 0 Then
                        words.Add(l)
                    End If
                Next
                Return words
            End Using
        End If

        Return New List(Of String)

    End Function

    Public Function mfEncodeUrl(ByVal url As String) As String

        Return System.Web.HttpUtility.UrlEncode(url).Replace("&", "%26").Replace("?", "%3F")


    End Function

    Public Function mfGetArtworkCachePath(ByVal track As cXmlTrack) As String

        Dim artDir As String = My.Settings.ArtworkDir & mfGetLegalText(mGetAlbumArtist(track)) & Path.DirectorySeparatorChar & mfGetLegalText(fGetAlbum(track)) & Path.DirectorySeparatorChar
        Try
            If Directory.Exists(artDir) = False Then
                Directory.CreateDirectory(artDir)
            End If
        Catch ex As Exception
            msAppendWarnings("000003")
            msAppendWarnings(ex.Message)
        End Try

        Return artDir & "Artwork.jpg"

    End Function

    Public Function mfWriteArtwork(ByVal picture As TagLib.IPicture, ByVal oFilePath As String) As Boolean

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

    Public Function mfGetTempArworkDirForTrack(ByVal track As IITTrack) As String

        Return My.Settings.TempDir & mfGetLegalText(mGetAlbumArtist(track)) & Path.DirectorySeparatorChar & mfGetLegalText(fGetAlbum(track)) & Path.DirectorySeparatorChar
      
    End Function

    Public Function mfConfigureMp3tag() As Boolean

        Dim succ As Boolean = File.Exists(My.Settings.ExePathMp3tag)

        If succ = False Then

            Dim dlg As New OpenFileDialog
            dlg.Title = "Browse for Mp3tag location..."
            dlg.Filter = "Executable Files (*.exe)|*.exe"
            If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
                My.Settings.ExePathMp3tag = dlg.FileName
                succ = True
            End If

        End If

        Return succ

    End Function


    Public Function mfConfigureAAD() As Boolean

        Dim succ As Boolean = File.Exists(My.Settings.ExePathAAD)

        If succ = False Then

            Dim dlg As New OpenFileDialog
            dlg.Title = "Browse for Album Art Download XUI location..."
            dlg.Filter = "Executable Files (*.exe)|*.exe"
            If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
                My.Settings.ExePathAAD = dlg.FileName
                succ = True
            End If

        End If

        Return succ

    End Function

    Public Function mfGetTempArtworkCachePath(ByVal fName As String, ByVal track As IITTrack) As String

        Dim artDir As String = mfGetTempArworkDirForTrack(track)

        Try
            If Directory.Exists(artDir) = False Then
                Directory.CreateDirectory(artDir)
            End If
        Catch ex As Exception
            msAppendWarnings("000003")
            msAppendWarnings(ex.Message)
        End Try

        Return String.Format("{0}{1}-{2}{3}", artDir, track.TrackDatabaseID, fName, mfGetArtworkExtension(track.Artwork.Item(1)))

    End Function

    Public Function mfGetArtworkCachePath(ByVal track As IITTrack, ByVal fName As String) As String

        Dim artDir As String = My.Settings.ArtworkDir & mfGetLegalText(mGetAlbumArtist(track)) & Path.DirectorySeparatorChar & mfGetLegalText(fGetAlbum(track)) & Path.DirectorySeparatorChar
        Try
            If Directory.Exists(artDir) = False Then
                Directory.CreateDirectory(artDir)
            End If
        Catch ex As Exception
            msAppendWarnings("000003")
            msAppendWarnings(ex.Message)
        End Try

        Return artDir & fName & mfGetArtworkExtension(track.Artwork.Item(1))

    End Function

    Public Function mfGetReplaceWords() As List(Of String)

        Dim replaceWords As List(Of String) = mfGetWordsList(mFilePathReplaceWords)

        If replaceWords IsNot Nothing Then
            msAppendDebug(String.Format("Retrieved {0} words to replace from {1}", replaceWords.Count, mFilePathReplaceWords))
        End If

        Return replaceWords

    End Function

    Public Function mfGetIgnoreWords() As List(Of String)

        Dim ignoreWords As List(Of String) = mfGetWordsList(mFilePathIgnoreWords)

        If ignoreWords IsNot Nothing Then            
            msAppendDebug(String.Format("Retrieved {0} words to ignore from {1}", ignoreWords.Count, mFilePathIgnoreWords))
        End If

        Return ignoreWords

    End Function

    Public Function mfGetSkipAlbumWords() As List(Of String)

        Dim skipWords As List(Of String) = mfGetWordsList(mFilePathSkipAlbumWords)

        If skipWords IsNot Nothing Then
            msAppendDebug(String.Format("Retrieved {0} words to skip from {1}", skipWords.Count, mFilePathSkipAlbumWords))
        End If

        Return skipWords

    End Function

    Public Function mfGetCapitalWordsList() As List(Of String)

        Dim lCapitalWords As New List(Of String)

        Dim capitalWords As List(Of String) = mfGetWordsList(mFilePathCapitalWords)

        If capitalWords IsNot Nothing Then
            lCapitalWords.AddRange(capitalWords)
            msAppendDebug(String.Format("Retrieved {0} capital words from {1}", lCapitalWords.Count, mFilePathCapitalWords))
        End If

        Return lCapitalWords

    End Function

    Public Function mfGetSimpleWordsList() As List(Of String)

        Dim simpleWords As List(Of String) = mfGetWordsList(mFilePathSimpleWords)

        If simpleWords IsNot Nothing Then
            mSimpleWords = simpleWords.ToArray
            msAppendDebug(String.Format("Retrieved {0} simple words from {1}", simpleWords.Count, mFilePathSimpleWords))
        End If

        Return simpleWords

    End Function

    Public Sub msAppendWarnings(ByVal msg As String)

        If mSbWarnings.Length < mSbWarnings.MaxCapacity Then
            mSbWarnings.AppendLine(Now.ToString("yyyy-MM-ddTHH:mm:ss") + " " + msg)
        End If

    End Sub

    Public Sub msAppendDebug(ByVal msg As String)

        If mSbDebug.Length < mSbDebug.MaxCapacity Then
            mSbDebug.AppendLine(Now.ToString("yyyy-MM-ddTHH:mm:ss") + " " + msg)
        End If

        If mSbDebug.Length > 100000 Then
            msWriteDebugLog()
        End If

    End Sub

    Public Sub msWriteDebugLog()
        Try
            Using sw As New StreamWriter(mFilePathDebugLog, append:=True)
                sw.WriteLine(mSbDebug.ToString)
            End Using
            mSbDebug = New System.Text.StringBuilder() ' clear 
            GC.Collect()
        Catch ex As Exception
            msAppendWarnings(ex.Message)
        End Try
    End Sub

    Public Function mfExportIndex(ByVal lDisc As cXmlDisc) As Boolean

        Dim success As Boolean = False

        Try
            Dim tgApp As New TreeGUI.cAdapter
            tgApp.GetConfig.IndexFileName = fGetFileNameFromPattern(My.Settings.IndexFileNamePattern, lDisc.FirstTrack)
            tgApp.GetConfig.IndexFileExtension = My.Settings.IndexFileExt
            tgApp.GetConfig.FolderList.Add(lDisc.Location)
            tgApp.GetConfig.CssFilePath = My.Settings.IndexCSS
            tgApp.GetConfig.CollapseFolders = False

            Dim tnl As New TreeGUI.cTreeNetLib(tgApp)
            tnl.IndexNow(TreeGUI.cAdapter.IndexingMode.IN_EACH_DIRECTORY)

            msAppendDebug("Exported Index to " & tgApp.GetConfig.GetIndexFilePaths(0))
            success = True
        Catch ex As Exception

        End Try

        Return success


    End Function

    ''' <summary>
    ''' Function to safely read an image from file path. Returns nothing if fails.
    ''' </summary>
    ''' <param name="filePath"></param>
    ''' <returns>Image as Bitmap</returns>
    ''' <remarks></remarks>
    Public Function mfGetBitMapFromFilePath(ByVal filePath As String) As Bitmap

        Dim img As Bitmap = Nothing

        Try
            Dim temp As Image = Image.FromFile(filePath)
            img = New Bitmap(temp)
            temp.Dispose()
        Catch ex As Exception
            msAppendWarnings(ex.Message + " for " + filePath)
            'Console.Writeline(ex.Message)
        End Try

        Return img

    End Function

    Public Function mfGetFolderBrowser(ByVal title As String, ByVal txtBox As TextBox) As Boolean

        Dim dlg As New McoreSystem.FolderBrowser
        dlg.Title = title
        dlg.Flags = McoreSystem.BrowseFlags.BIF_NEWDIALOGSTYLE Or _
                    McoreSystem.BrowseFlags.BIF_STATUSTEXT Or _
                    McoreSystem.BrowseFlags.BIF_EDITBOX

        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            If dlg.DirectoryPath.Length > 0 Then
                Dim dir As String = dlg.DirectoryPath
                If dir.EndsWith(Path.DirectorySeparatorChar) = False Then
                    dir += Path.DirectorySeparatorChar
                End If
                txtBox.Text = dir
                Return True
            End If
        End If

        Return False

    End Function

    Friend Sub msWriteListToFile(ByVal log As LogData, Optional ByVal sort As Boolean = True)

        ' future method to overwrite file if file size over specified limit 

        If log.PathList IsNot Nothing AndAlso log.PathList.Count > 0 Then

            If sort = True Then
                log.PathList.Sort()
            End If

            Using sw As New StreamWriter(log.Destination, True)

                sw.WriteLine()
                sw.WriteLine("************************************")
                sw.WriteLine("Job Type: " & mCurrJobTypeMain.ToString)
                sw.WriteLine("Date and Time: " & Now.ToString("* yyyy-MM-dd HH:mm:ss"))
                sw.WriteLine("************************************")
                sw.WriteLine()

                For Each l As String In log.PathList
                    sw.WriteLine(l)
                Next

            End Using

            msAppendDebug("Log saved in " + log.Destination)
            log.PathList.Clear()

        ElseIf log.TrackList IsNot Nothing AndAlso log.TrackList.Count > 0 Then

            If sort = True Then
                log.TrackList.Sort(New cXmlTrackComparer)
            End If

            Using sw As New StreamWriter(log.Destination, True)

                sw.WriteLine()
                sw.WriteLine("*********************")
                sw.WriteLine(Now.ToString("* yyyy-MM-dd HH:mm:ss"))
                sw.WriteLine("*********************")
                sw.WriteLine()

                Select Case log.LogType

                    Case LogDataType.META_TAG_VERSIONS
                        For Each xt As cXmlTrack In log.TrackList
                            sw.WriteLine(String.Format("{0} - {1}", xt.Location, xt.MetaVersion))
                        Next

                    Case LogDataType.ARTWORK_RESOLUTIONS
                        For Each xt As cXmlTrack In log.TrackList
                            sw.WriteLine(String.Format("{0}x{1} for {2}", xt.Artwork.Width, xt.Artwork.Height, xt.Location, xt.MetaVersion))
                        Next

                End Select

            End Using

            log.TrackList.Clear()
            msAppendDebug("Log saved in " + log.Destination)

        End If


    End Sub

    Public Function mfSaveArtworkSafely(ByVal destFilePath As String, ByVal artworkSource As cArtworkSource) As Boolean

        Dim fiArtwork As FileInfo = Nothing
        Dim wasReadOnly As Boolean
        Dim bExported As Boolean = False

        If Directory.Exists(Path.GetDirectoryName(destFilePath)) = False Then
            Try
                Directory.CreateDirectory(Path.GetDirectoryName(destFilePath))
            Catch ex As Exception
                msAppendWarnings("000002")
                msAppendWarnings(ex.Message)
                Return False
            End Try
        End If

        If File.Exists(destFilePath) Then

            fiArtwork = New FileInfo(destFilePath)
            wasReadOnly = fiArtwork.IsReadOnly

            ' if read only then remove read only tag
            If fiArtwork.IsReadOnly Then
                fiArtwork.IsReadOnly = False
            End If

        End If

        Dim srcFilePath As String = artworkSource.ArtworkPath

        Select Case artworkSource.ArtworkType

            Case ArtworkSourceType.iTMS, ArtworkSourceType.File, ArtworkSourceType.AAD
                ' this way we know the iTMS was found
                ' copy the downloaded iTMS artwork
                If srcFilePath <> String.Empty Then
                    If srcFilePath <> destFilePath Then
                        If File.Exists(destFilePath) Then
                            File.Delete(destFilePath)
                        End If
                        My.Computer.FileSystem.CopyFile(srcFilePath, destFilePath, True)
                        bExported = True
                        msAppendDebug(String.Format("Saved {0} as {1}", srcFilePath, destFilePath))
                    End If
                Else
                    ' saving artwork from track's embedded artwork                            
                    bExported = mfSaveTrackArtworkJPEG(destFilePath, artworkSource.Track.Artwork)
                End If

            Case ArtworkSourceType.Track
                bExported = mfSaveTrackArtworkJPEG(destFilePath, artworkSource.Track.Artwork)

            Case Else
                If My.Settings.UseArtworkResize = True AndAlso _
                        srcFilePath <> destFilePath AndAlso srcFilePath <> String.Empty Then
                    ' saving resized artwork
                    If File.Exists(destFilePath) Then
                        File.Delete(destFilePath)
                    End If
                    If File.Exists(srcFilePath) Then
                        My.Computer.FileSystem.CopyFile(srcFilePath, destFilePath, True)
                        bExported = True
                        msAppendDebug(String.Format("Saved {0} as {1}", srcFilePath, destFilePath))
                    End If
                Else
                    ' saving artwork from track's embedded artwork            
                    bExported = mfSaveTrackArtworkJPEG(destFilePath, artworkSource.Track.Artwork)
                End If

        End Select

        ' undo the remove read only operation
        If fiArtwork IsNot Nothing AndAlso wasReadOnly Then
            fiArtwork.IsReadOnly = True
        End If

        Return bExported

    End Function

    Public Function mfSaveTrackArtworkJPEG(ByVal destFilePath As String, ByVal artwork As IITArtworkCollection) As Boolean

        Dim bExported As Boolean = False

        If artwork.Count > 0 Then

            Try
                Select Case artwork.Item(1).Format
                    Case ITArtworkFormat.ITArtworkFormatJPEG
                        ' if already jpg then no problems
                        artwork.Item(1).SaveArtworkToFile(destFilePath)
                        bExported = True
                    Case Else
                        ' otherwise we will convert to jpg     
                        Dim origRoot As String = CStr(IIf(My.Settings.ExportOrignalArtworkFormat, Path.GetDirectoryName(destFilePath), My.Settings.TempDir))
                        Dim origPath As String = Path.Combine(origRoot, Path.GetFileNameWithoutExtension(destFilePath) + mfGetArtworkExtension(artwork.Item(1)))
                        ' save image with proper extension and keep it
                        artwork.Item(1).SaveArtworkToFile(origPath)
                        Dim img As Image = Image.FromFile(origPath)
                        ' save the jpg convert
                        img.Save(destFilePath, System.Drawing.Imaging.ImageFormat.Jpeg)
                        bExported = True
                End Select
            Catch ex As Exception
                msAppendWarnings(String.Format("{0} while saving Artwork as {1}.", ex.Message, destFilePath))
                msAppendWarnings(ex.StackTrace)
            End Try

        End If

        If bExported = True Then
            msAppendDebug(String.Format("Saved {0} as {1}", "Track Artwork", destFilePath))
        End If

        Return bExported

    End Function

    Public Function mfAppIsLoadedAsStartup(ByVal ProductName As String, ByVal FilePath As String) As Boolean

        Dim succ As Boolean = False

        '' 5.35.3.1 Fixed possible crash on Application startup when registry access was denied to see if iTSfv loads on startup [John]

        Try
            Dim regKey As Microsoft.Win32.RegistryKey = _
                    Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run", False)
            If regKey IsNot Nothing Then
                If regKey.GetValue(ProductName) IsNot Nothing Then
                    succ = (regKey.GetValue(ProductName).ToString = FilePath)
                End If
            End If
        Catch ex As Exception
            ' oh well
        End Try

        Return succ

    End Function

    Public Function mfConfigBackup(ByVal filePath As String) As Boolean

        ' filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + "iTSfv.xml"
        Dim success As Boolean = True

        Try
            My.Settings.Save()

            Dim config As System.Configuration.Configuration = _
                  ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal)

            config.SaveAs(filePath)
        Catch ex As Exception
            success = False
        End Try

        Return success

    End Function

    Public Function mfConfigFilePath() As String

        Dim config As System.Configuration.Configuration = _
       ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal)
        Return config.FilePath

    End Function

    Public Function mfConfigRestore(ByVal filePath As String) As Boolean

        ' filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + "iTSfv.xml"

        Dim success As Boolean = True

        If File.Exists(filePath) Then

            Try
                Dim config As System.Configuration.Configuration = _
                      ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal)

                My.Computer.FileSystem.CopyFile(filePath, config.FilePath, True) ' jmcilhinney

                My.Settings.Reload()
                My.Forms.frmMain.sSettingsGet()
                My.Forms.frmOptions.sSettingsGet()
                My.Forms.frmValidator.sSettingsGet()

            Catch ex As Exception
                My.Settings.Reset()
                success = False
            End Try

        End If

        Return success

    End Function

    Public Function mfZipFile(ByVal inputFilePath As String, ByVal zipFilePath As String, Optional ByVal filesExtra As List(Of String) = Nothing) As Boolean

        Dim success As Boolean = True

        If File.Exists(inputFilePath) And IO.File.Exists(zipFilePath) = False Then

            Try

                If filesExtra Is Nothing Then
                    filesExtra = New List(Of String)
                End If
                If inputFilePath IsNot Nothing Then
                    filesExtra.Add(inputFilePath)
                End If

                Dim strmZipOutputStream As Zip.ZipOutputStream
                strmZipOutputStream = New Zip.ZipOutputStream(File.Create(zipFilePath))

                For Each filePath As String In filesExtra

                    If File.Exists(filePath) Then
                        Dim strmFile As FileStream = File.OpenRead(filePath)
                        Dim abyBuffer(CInt(strmFile.Length - 1)) As Byte
                        strmFile.Read(abyBuffer, 0, abyBuffer.Length)

                        Dim objZipEntry As Zip.ZipEntry = New Zip.ZipEntry(Path.GetFileName(filePath))
                        objZipEntry.DateTime = DateTime.Now
                        objZipEntry.Size = strmFile.Length
                        strmFile.Close()

                        strmZipOutputStream.PutNextEntry(objZipEntry)
                        strmZipOutputStream.Write(abyBuffer, 0, abyBuffer.Length)
                    End If

                Next

                ''''''''''''''''''''''''''''''''''''
                ' Finally Close strmZipOutputStream
                ''''''''''''''''''''''''''''''''''''
                strmZipOutputStream.Finish()
                strmZipOutputStream.Close()

                'If GetConfig.ZipAndDeleteFile = True Then
                '    My.Computer.FileSystem.DeleteFile(strFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                'End If

            Catch ex As System.UnauthorizedAccessException
                success = False
            End Try

        End If

        Return success

    End Function

End Module
