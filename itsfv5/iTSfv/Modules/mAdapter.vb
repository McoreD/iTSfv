Imports System.IO
Imports iTunesLib
Imports System.Collections
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Soap
Imports McoreSystem
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Net

Public Module mAdapter

    Friend mAppInfo As New McoreSystem.AppInfo(Application.ProductName, _
                    Application.ProductVersion, _
                    AppInfo.SoftwareCycle.BETA)

    Friend WithEvents mItunesApp As IiTunes
    Friend mWebClient As New WebClient

    Friend mPlayer As cPlayer
    Friend mItunesWindow As IITWindowCollection
    Friend mXmlLibParser As cLibraryParser

    Friend mJobPaused As Boolean = False

    Public mChartType As ChartType = ChartType.PIE

    Public mStatsMaker As cStatsMaker
    Friend mRatingWeights As RatingWeights

    Public mStatsFilePath As String ' configure when initializing itunes

    Public mLibraryTasks As New cLibraryTasks

    Public Const SBAR_MAX_CHAR As Integer = 100

    Public Const EMAIL_ADDRESS As String = "mcored@gmail.com"

    Public Const APP_ABBR_NAME_IT As String = "iTSfv"
    Public Const APP_ABBR_NAME_ITL As String = "iTSfv Lite"
    Public Const APP_ABBR_NAME_WMP As String = "WMPfv"

    Public Const VARIOUS_ARTISTS As String = "Various Artists"
    Public Const UNKNOWN_ALBUM As String = "Unknown Album"
    Public Const UNKNOWN_ARTIST As String = "Unknown Artist"
    Public Const UNKNOWN_GENRE As String = "Unknown Genre"
    Public Const UNKNOWN_YEAR As Integer = 0
    ' filters and extensions are moved to mFileSystem.vb

    Public ITUNES_VERSION As String = "0.0.0.0" ' set in form load
    Public mCurrJobTypeMain As cBwJob.JobType = cBwJob.JobType.NEW_TASK

    ' List/Logs
    Friend mListTracksNoTrackNum As New List(Of String)
    Friend mListTracksNonITSstandard As New List(Of String)
    Friend mListMissingLyrics As New List(Of String)

    Friend mListTracksNoAlbumArtist As New List(Of String)
    Friend mArtworkGrabberUrlPathMp3 As String = "http://wmwiki.com/mcored/itunes-artwork-grabber.mp3"
    Friend mArtworkGrabberFilePathMp3 As String = Path.Combine(Application.StartupPath, "itunes-artwork-grabber.mp3")

    Public Function mfOKtoPowerMgmt() As Boolean
        Return My.Settings.PowerOption > 0 AndAlso (My.Settings.PowerSynchroclean AndAlso mCurrJobTypeMain = cBwJob.JobType.SYNCHROCLEAN Or _
        My.Settings.PowerValidateLibrary AndAlso mCurrJobTypeMain = cBwJob.JobType.VALIDATE_LIBRARY)
    End Function

    Public Sub msCheckUpdatesAuto(ByVal sBarLeft As ToolStripStatusLabel)

        'If My.Settings.AutoCheckUpdates Then
        '    Dim uc As New cUpdateChecker(My.Forms.frmMain.Icon, My.Resources.itsfv2, _
        '                                 sBarLeft, manual:=False)
        '    uc.CheckUpdates()
        'End If

        CheckUpdates(manual:=False)

    End Sub

    Private Sub CheckUpdates(ByVal manual As Boolean)
        Dim uc As New McoreSystem.UpdateChecker("http://code.google.com/p/itsfv/downloads/list")
        uc.Manual = manual
        uc.CheckUpdates()
    End Sub

    Public Sub msShowUpdatesManual(ByVal sBarLeft As ToolStripStatusLabel)

        'Dim uc As New cUpdateChecker(My.Forms.frmMain.Icon, My.Resources.itsfv2, _
        '                            sBarLeft, manual:=True)
        'uc.CheckUpdates()

        CheckUpdates(manual:=True)
        sBarLeft.Text = String.Empty

    End Sub

    Public Function mfValidRestoreFile(ByVal filePath As String) As Boolean

        Dim fileWrong As Boolean = Path.GetFileName(filePath).Equals("iTunes Music Library.xml") Or _
        Path.GetFileName(filePath).Equals("iTunes Library.xml")

        Return Not fileWrong

    End Function

    Public Function mfOKtoValidate(ByVal track As IITFileOrCDTrack) As Boolean

        If track.Location Is Nothing Then
            Return False
        End If

        Dim booCheckTrack As Boolean = True

        If My.Settings.MusicAudioOnly Then
            ' if true then file is a file and is audio only
            booCheckTrack = (track.VideoKind = ITVideoKind.ITVideoKindNone)
        End If

        If Not My.Settings.IncludePodcasts Then
            ' this means do not include podcasts
            booCheckTrack = booCheckTrack AndAlso Not track.Podcast
        End If

        booCheckTrack = booCheckTrack And fTaggedForValidation(track)

        Return booCheckTrack

    End Function

    Public Function mfCompareRes() As Boolean

        Return (mCurrJobTypeMain = cBwJob.JobType.VALIDATE_TRACKS_SELECTED AndAlso My.Settings.AlwaysHighResSelected = True) Or _
               (mCurrJobTypeMain = cBwJob.JobType.VALIDATE_DISC AndAlso My.Settings.AlwaysHighResSelected = True) Or _
               (mCurrJobTypeMain = cBwJob.JobType.ADD_NEW_TRACKS AndAlso My.Settings.AlwaysHighResNewlyAdded = True) Or _
               (mCurrJobTypeMain = cBwJob.JobType.VALIDATE_LIBRARY AndAlso My.Settings.AlwaysHighResLibrary = True) Or _
               (mCurrJobTypeMain = cBwJob.JobType.VALIDATE_LIBRARY AndAlso My.Settings.AlwaysHighResLast100 = True)

    End Function

    Public Function mfGetLegalDirectoryNameFromPattern(ByVal pattern As String, ByVal xt As cXmlTrack) As String

        Return mfGetLegalTextForDirectory(mfGetStringFromSyntax(pattern, xt))

    End Function

    Public Function mfGetLegalTextForDirectory(ByVal txt As String) As String

        txt = txt.Replace("/", "_")
        txt = txt.Replace(":", "_")
        txt = txt.Replace("*", "_")
        txt = txt.Replace("?", "_")
        txt = txt.Replace(Chr(34), "_")
        txt = txt.Replace("<", "_")
        txt = txt.Replace(">", "_")
        txt = txt.Replace("|", "_")

        Return txt

    End Function

    Public Function mfGetLegalText(ByVal txt As String) As String

        txt = mfGetLegalTextForDirectory(txt)
        txt = txt.Replace("\", "_")

        Return txt

    End Function

    Public Function mfGetPercentage(ByVal num As Integer, ByVal total As Double) As Double
        Return Double.Parse((100 * num / total).ToString("00.0"))
    End Function

    Public Function mfGetText(ByVal strName As String) As String

        Try
            ' get the current assembly
            Dim oAsm As System.Reflection.Assembly = _
            System.Reflection.Assembly.GetExecutingAssembly()
            Dim oStrm As IO.Stream = _
            oAsm.GetManifestResourceStream(oAsm.GetName.Name + "." + strName)
            ' read contents of embedded file
            Dim oRdr As IO.StreamReader = New IO.StreamReader(oStrm)
            Return oRdr.ReadToEnd

        Catch ex As Exception
            Throw ex

        End Try

    End Function

    Public Function fTaggedForValidation(ByVal track As IITTrack) As Boolean

        Return My.Settings.EmptyTagsInclude Or track.Artist IsNot Nothing AndAlso _
        track.Album IsNot Nothing AndAlso _
        track.Name IsNot Nothing

    End Function

    Public Function mfTagsComplete(ByVal track As cXmlTrack) As Boolean

        Return track.Artist IsNot Nothing AndAlso _
        track.Album IsNot Nothing AndAlso _
        track.Name IsNot Nothing

    End Function

    Public Function mfGetAddableFilesList(ByVal filePath As String, Optional ByVal lExt() As String = Nothing) As List(Of String)

        ' files from outside of music folder gets added
        ' but it doesnt matter because a seperate check is done before adding to listbox

        Dim listAddableFiles As New List(Of String)

        Dim lAllExt As New List(Of String)
        lAllExt.AddRange(mFileExtAudioITunes)
        If lExt IsNot Nothing Then
            Dim lOtherExt As New List(Of String)
            lOtherExt.AddRange(lExt)
            For Each s As String In lOtherExt
                If lAllExt.Contains(s) = False Then
                    lAllExt.Add(s)
                End If
            Next
        End If

        If Directory.Exists(filePath.Trim) Then

            For Each fileExt As String In lAllExt
                listAddableFiles.AddRange(Directory.GetFiles(filePath, "*." & fileExt, SearchOption.AllDirectories))
            Next

        ElseIf File.Exists(filePath) Then

            For Each fileExt As String In lAllExt
                If "." & fileExt = Path.GetExtension(filePath).ToLower Then
                    listAddableFiles.Add(filePath)
                End If
            Next

        End If

        Return listAddableFiles

    End Function

    Public Function fGetDurationInWeeksDHMS(ByVal seconds As Double) As Double()

        Dim arrayDaysHMS() As Double = fGetDurationsInDaysHMS(seconds)

        Dim arrayWDHMS(5) As Double
        arrayWDHMS(0) = CInt(arrayDaysHMS(0) / 7)
        arrayWDHMS(1) = CInt(arrayDaysHMS(0) Mod 7)
        arrayWDHMS(2) = arrayDaysHMS(1)
        arrayWDHMS(3) = arrayDaysHMS(2)
        arrayWDHMS(4) = arrayDaysHMS(3)

        Return arrayWDHMS

    End Function

    Public Function fGetDurationsInDaysHMS(ByVal seconds As Double) As Double()

        Dim arrayHoursMinutesSeconds() As Double = fGetDurationInHoursMS(seconds)

        Dim arrayDHMS(4) As Double
        arrayDHMS(0) = CInt(arrayHoursMinutesSeconds(0) / 24)
        arrayDHMS(1) = CInt(arrayHoursMinutesSeconds(0) Mod 24)
        arrayDHMS(2) = arrayHoursMinutesSeconds(1)
        arrayDHMS(3) = arrayHoursMinutesSeconds(2)

        Return arrayDHMS

    End Function

    Public Function mFileIsImage(ByVal filePath As String) As Boolean

        For Each ext As String In mFileExtArtwork
            If Path.GetExtension(filePath).Contains(ext) Then
                Return True
            End If
        Next

    End Function

    Public Function mfGetArtworkExtension(ByVal artwork As IITArtwork) As String

        Try
            Select Case artwork.Format
                Case ITArtworkFormat.ITArtworkFormatJPEG
                    Return ".jpg"
                Case ITArtworkFormat.ITArtworkFormatBMP
                    Return ".bmp"
                Case ITArtworkFormat.ITArtworkFormatPNG
                    Return ".png"
                Case Else
                    Return ".jpg"
            End Select
        Catch ex As Exception
            Return ".jpg"
        End Try

    End Function

    Public Function fGetHMStoString(ByVal sec As Double) As String

        Dim hms() As Double = fGetDurationInHoursMS(sec)
        Return String.Format("{0} Hours {1} Minutes {2} Seconds", hms(0), hms(1).ToString("00"), hms(2))

    End Function

    Public Function fGetHMStoString2(ByVal sec As Double) As String

        Dim hms() As Double = fGetDurationInHoursMS(sec)
        Return String.Format("{0}:{1}:{2}", hms(0).ToString("00"), hms(1).ToString("00"), hms(2).ToString("00"))

    End Function

    Public Function fGetDurationInHoursMS(ByVal seconds As Double) As Double()

        Dim arrayHoursMinutesSeconds(3) As Double
        Dim SecondsLeft As Double = seconds
        Dim hours As Integer = 0
        Dim minutes As Integer = 0

        While SecondsLeft >= 3600
            SecondsLeft -= 3600
            hours += 1
        End While

        arrayHoursMinutesSeconds(0) = hours

        While SecondsLeft >= 60
            SecondsLeft -= 60
            minutes += 1
        End While

        arrayHoursMinutesSeconds(1) = minutes
        arrayHoursMinutesSeconds(2) = SecondsLeft

        Return arrayHoursMinutesSeconds

    End Function

    Public Function mfReadObjectFromFileBF(ByVal filePath As String) As Object

        Dim myObject As Object = Nothing

        If IO.File.Exists(filePath) Then

            Dim fs As New IO.FileStream(filePath, IO.FileMode.Open, IO.FileAccess.Read)

            Try
                Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
                myObject = DirectCast(bf.Deserialize(fs), Object)
                Return myObject
            Catch ex As Exception
                fs.Close()
            Finally
                fs.Close()
            End Try

        End If

        Return myObject

    End Function

    Public Function mfReadObjectFromFileXML(ByVal filePath As String) As Object

        Dim myObject As New Object

        Try
            Using fs As New IO.FileStream(filePath, IO.FileMode.Open, IO.FileAccess.Read)
                Dim xs As New System.Xml.Serialization.XmlSerializer(myObject.GetType)
                myObject = TryCast(xs.Deserialize(fs), Object)
                fs.Close()
            End Using
        Catch ex As Exception
            Return False
        End Try

        Return myObject

    End Function

    Public Function mfWriteObjectToFileBF(ByVal myObject As Object, ByVal filePath As String) As Boolean

        Dim ow As New cObjectWriter(myObject, filePath)
        Dim t As New Threading.Thread(AddressOf ow.WriteObjectToBF)
        t.Start()

        Return True

    End Function

    Public Sub msWriteObjectToFileXML(ByVal myObject As Object, ByVal filePath As String)

        Using fs As New IO.FileStream(filePath, IO.FileMode.Create)
            Dim xs As New System.Xml.Serialization.XmlSerializer(myObject.GetType)
            xs.Serialize(fs, myObject)
        End Using

    End Sub

    Sub sWriteTableToFile(ByVal addresses As Hashtable, ByVal filePath As String)

        ' Create a hashtable of values that will eventually be serialized.
        ' To serialize the hashtable (and its key/value pairs),
        ' you must first open a stream for writing.
        ' Use a file stream here.
        Dim fs As New FileStream(filePath, FileMode.Create)

        ' Construct a SoapFormatter and use it
        ' to serialize the data to the stream.
        Dim formatter As New SoapFormatter
        Try
            formatter.Serialize(fs, addresses)
        Catch e As SerializationException
            'Console.Writeline("Failed to serialize. Reason: " & e.Message)
            Throw
        Finally
            fs.Close()
        End Try

    End Sub

    Public Enum TypeAddFiles
        NORMAL
        SILENT
    End Enum

    Public Enum TypeSerialize

        BINARY_FORMATTER
        XML_FORMATTER
        SOAP_FORMATTER

    End Enum

    Public Function fReadTableFromFile(ByVal filePath As String) As Hashtable

        ' Declare the hashtable reference.
        Dim addresses As New Hashtable

        ' Open the file containing the data that you want to deserialize.
        Dim fs As New FileStream(filePath, FileMode.Open)
        Try
            Dim formatter As New SoapFormatter

            ' Deserialize the hashtable from the file and
            ' assign the reference to the local variable.
            addresses = DirectCast(formatter.Deserialize(fs), Hashtable)
        Catch e As SerializationException
            'Console.Writeline("Failed to deserialize. Reason: " & e.Message)
        Finally
            fs.Close()
        End Try

        ' To prove that the table deserialized correctly,
        ' display the key/value pairs to the console.
        'Dim de As DictionaryEntry
        'For Each de In addresses
        '    'Console.Writeline("{0} lives at {1}.", de.Key, de.Value)
        'Next

        Return addresses

    End Function

    ' mfGetStringFromPattern is moved mLibrary

    Public Function fGetFileNameFromPattern(ByVal pattern As String, ByVal track As IITFileOrCDTrack) As String

        '' 5.31.1.1 Illegal characters in path while getting pattern [Raphael]

        pattern = mfGetStringFromPattern(pattern, track)
        Dim dir As String = Path.GetDirectoryName(mfGetLegalTextForDirectory(pattern))
        Dim f As String = Path.GetFileName(mfGetLegalText(pattern))

        Return Path.Combine(dir, f)

    End Function

    Public Function fGetFileNameFromPattern(ByVal pattern As String, ByVal track As cXmlTrack) As String

        '' 5.31.1.1 Illegal characters in path while getting pattern [Raphael]

        pattern = mfGetStringFromSyntax(pattern, track)
        Dim dir As String = Path.GetDirectoryName(mfGetLegalTextForDirectory(pattern))
        Dim f As String = Path.GetFileName(mfGetLegalText(pattern))

        Return Path.Combine(dir, f)

    End Function

    Public Function mfGetFilePathFromPattern(ByVal baseDir As String, ByVal artworkName As String, _
                                             ByVal firstTrack As IITFileOrCDTrack, ByVal scanAll As Boolean) As String

        Dim artworkPath As String = ""

        Try
            artworkPath = Path.Combine(baseDir, mfGetFileNameFromPattern(artworkName, firstTrack))
            If scanAll Then
                If Not File.Exists(artworkPath) Then
                    For Each fileName As String In My.Settings.ArtworkFileNames
                        artworkPath = baseDir + "\" + mfGetFileNameFromPattern(fileName, firstTrack)
                        If IO.File.Exists(artworkPath) Then
                            Return artworkPath
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            msAppendWarnings(ex.Message + " while getting file path from pattern")
        End Try

        Return artworkPath

    End Function

    Public Function mfGetArtworkFilePathFromPattern(ByVal baseDir As String, ByVal artworkName As String, ByVal firstTrack As cXmlTrack) As String

        Dim artworkPath As String = Path.Combine(baseDir, mfGetFileNameFromPattern(artworkName, firstTrack))

        If Not File.Exists(artworkPath) Then
            For Each fileName As String In My.Settings.ArtworkFileNames
                artworkPath = baseDir + "\" + mfGetFileNameFromPattern(fileName, firstTrack)
                If IO.File.Exists(artworkPath) Then
                    Return artworkPath
                End If
            Next
        End If

        Return artworkPath

    End Function

    Public Function mfGetFileNameFromPattern(ByVal pattern As String, ByVal location As String) As String

        ''************************************************************
        ''* 5.26.4.0 Support creating sub-directories for patterns
        ''* e.g. %AlbumArtist%\(%Year%) %Album%\%TrackNumber% - %Name%
        ''************************************************************

        Dim temp As String = "Artwork.jpg"

        Try
            Dim dir As String = Path.GetDirectoryName(mfGetLegalTextForDirectory(pattern))
            Dim f As String = Path.GetFileName(mfGetLegalText(pattern))
            temp = Path.Combine(dir, f)
        Catch ex As Exception
            msAppendWarnings(String.Format("File Path: {0}", location))
            msAppendWarnings(ex.Message + " while getting file name from pattern")
        End Try

        Return temp

    End Function

    Public Function mfGetFileNameFromPattern(ByVal pattern As String, ByVal track As cXmlTrack) As String

        pattern = mfGetStringFromSyntax(pattern, track)
        Return mfGetFileNameFromPattern(pattern, track.Location)

    End Function

    Public Function mfGetFileNameFromPattern(ByVal pattern As String, ByVal track As IITFileOrCDTrack) As String

        pattern = mfGetStringFromPattern(pattern, track)
        Return mfGetFileNameFromPattern(pattern, track.Location)

    End Function

    Public Sub sAutoCompleteCombo_KeyUp(ByVal cbo As ComboBox, ByVal e As KeyEventArgs)

        Dim sTypedText As String
        Dim iFoundIndex As Integer
        Dim oFoundItem As Object
        Dim sFoundText As String
        Dim sAppendText As String

        'Allow select keys without Autocompleting
        Select Case e.KeyCode
            Case Keys.Back, Keys.Left, Keys.Right, Keys.Up, Keys.Delete, Keys.Down
                Return
        End Select

        'Get the Typed Text and Find it in the list
        sTypedText = cbo.Text
        iFoundIndex = cbo.FindString(sTypedText)

        'If we found the Typed Text in the list then Autocomplete
        If iFoundIndex >= 0 Then

            'Get the Item from the list (Return Type depends if Datasource was bound
            ' or List Created)
            oFoundItem = cbo.Items(iFoundIndex)

            'Use the ListControl.GetItemText to resolve the Name in case the Combo
            ' was Data bound
            sFoundText = cbo.GetItemText(oFoundItem)

            'Append then found text to the typed text to preserve case
            sAppendText = sFoundText.Substring(sTypedText.Length)
            cbo.Text = sTypedText & sAppendText

            'Select the Appended Text
            cbo.SelectionStart = sTypedText.Length
            cbo.SelectionLength = sAppendText.Length

        End If

    End Sub

    Public Sub sAutoCompleteCombo_Leave(ByVal cbo As ComboBox)
        Dim iFoundIndex As Integer
        iFoundIndex = cbo.FindStringExact(cbo.Text)
        cbo.SelectedIndex = iFoundIndex
    End Sub

    Public Function mfGetFixedLyrics(ByVal str As String) As String

        'Dim lines As String() = str.Split(CChar(Environment.NewLine))
        'Dim lyrics As New stringbuilder
        'If lines.Length > 0 Then
        '    For Each line As String In lines
        '        If line.Length > 0 Then
        '            Dim chr As Char() = line.ToCharArray
        '            chr(0) = CChar(chr(0).ToString.ToUpper)
        '            lyrics.AppendLine(chr.ToString)
        '        End If
        '    Next
        'End If

        If String.IsNullOrEmpty(str) = False Then

            str = mfGetFixedString(str)
            str = mfStripHTMLTags(str)
            str = str.Replace("''", "")
            str = str.Replace("'''", "")

            'If str.IndexOf(vbCrlf) = -1 Then
            '    Console.WriteLine(str)
            '    ' split lines
            '    str = str.Replace(". ", ",")
            '    Dim lyrics() As String = str.Split(CChar(","))
            '    Dim sbLyrics As New StringBuilder
            '    For Each l As String In lyrics
            '        Console.WriteLine(l)
            '        If l.Length < 20 Then
            '            sbLyrics.Append(l)
            '            sbLyrics.Append(", ")

            '        Else
            '            sbLyrics.AppendLine(l.Trim)
            '        End If
            '    Next
            '    str = sbLyrics.ToString
            'End If

        End If

        Return str

    End Function

    Public Function mfStripHTMLTags(ByVal HTMLToStrip As String) As String

        Dim stripped As String = String.Empty

        If HTMLToStrip <> "" Then
            stripped = Regex.Replace(HTMLToStrip, "<(.|\n)+?>", String.Empty)
        End If

        Return stripped

    End Function

    Public Function mfGetFixedCase(ByVal tag As String, ByVal wl As WordLists) As String

        Dim sb As New StringBuilder

        tag = mfGetFixedSpaces(tag)

        Dim tagSp As String() = tag.Split(CChar(" "))
        Dim ignoreWords As List(Of String) = mfGetIgnoreWords()

        For Each word As String In tagSp

            If String.IsNullOrEmpty(word) = False Then

                If ignoreWords.Contains(word) = False Then

                    If word.Length > 0 AndAlso IsNumeric(word(0)) = False Then

                        Dim w As String = word.Replace("(", "").Replace("{", "").Replace("[", "")
                        w = w.Replace(")", "").Replace("}", "").Replace("}", "")

                        If wl.capitalWords.Contains(w.ToUpper) Then
                            sb.Append(word.ToUpper)
                        Else
                            sb.Append(StrConv(word, VbStrConv.ProperCase))
                        End If

                    Else
                        sb.Append(word.ToLower)
                    End If

                Else

                    sb.Append(word) '' without any modification

                End If

                sb.Append(" ")

            End If

        Next

        tag = sb.ToString.Trim

        Return tag

    End Function

    Public Function mfGetFixedString(ByVal sss As String) As String

        If (Not String.IsNullOrEmpty(sss)) Then

            Dim replaceWords As List(Of String) = mfGetReplaceWords()

            For Each line In replaceWords
                Dim word() As String = System.Text.RegularExpressions.Regex.Split(line, ",,,,")
                If word.Length = 2 Then
                    sss = sss.Replace(word(0), word(1))
                ElseIf word.Length = 4 Then
                    sss = sss.Replace(word(1), word(2))
                End If
            Next

            sss = sss.Trim

        End If

        Return sss

    End Function

    Public Function mfGetMediaCenter() As Boolean
        Return File.Exists(My.Settings.UrlToCoverArtLocation)
    End Function

    Public Function mfGetTruncatedText(ByVal oString As String, _
                                                 ByVal oToolTip As ToolTip, _
                                                 ByVal oStatusStrip As StatusStrip) As String

        oToolTip.SetToolTip(oStatusStrip, oString)

        If oString.Length > SBAR_MAX_CHAR Then
            oString = oString.Substring(0, SBAR_MAX_CHAR)
        End If

        Return oString

    End Function

    Public Function mfGetItunes() As Boolean

        ' Uses ITDetector to determine if iTunes is installed, seems to be new in v7 iTunes.
        '
        Dim success As Boolean = False

        Dim iDetect As ITDETECTORLib.iTunesDetector = Nothing

        Try
            iDetect = New ITDETECTORLib.iTunesDetector
        Catch ex As Exception
            '
            ' Not bothered if iDetect not present, could be old version
            '
        End Try

        '
        ' If iDetect inialise did not fail use it to test for iTunes
        '
        If Not iDetect Is Nothing Then
            success = iDetect.IsiTunesAvailable
        Else
            ' we our registry check
            Dim regKey As Microsoft.Win32.RegistryKey = _
                 Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\Apple Computer, Inc.\iTunes", False)

            If regKey IsNot Nothing Then
                success = True
            End If
        End If

        Return success

    End Function

    Public ReadOnly Property mpRestartOptions() As McoreSystem.WindowsController.RestartOptions

        Get

            Select Case My.Settings.PowerOption
                Case 0
                    Return Nothing
                Case 1
                    Return WindowsController.RestartOptions.Suspend
                Case 2
                    Return WindowsController.RestartOptions.Hibernate
                Case 3
                    Return WindowsController.RestartOptions.ShutDown
                Case Else
                    Return Nothing
            End Select

        End Get

    End Property

End Module

#Region "Enumerations"

Public Enum AddFilesType

    COPY
    DO_NOTHING
    MOVE

End Enum

Public Enum ChartType

    PIE
    BAR

End Enum

Public Enum LogDataType

    GENERAL
    META_TAG_VERSIONS
    ARTWORK_RESOLUTIONS

End Enum

Public Enum TrackTypeXML

    FILE
    URL

End Enum

Public Enum GraphViewType

    PREFERENCE
    STORAGE
    RATING

End Enum

Public Enum ArtworkSourceType

    AAD
    File
    iTMS
    iTunes
    Track

End Enum

Public Enum eAppMode
    AFV = 0
    ITSFV = 1
    WMPFV = 2
End Enum

#End Region

#Region "Structures"

Public Structure ValidatorModes

    Public Sub New(ByVal lChecks As Boolean, ByVal lTracks As Boolean, _
                   ByVal lLibrary As Boolean, ByVal lFileSystem As Boolean)

        Checks = lChecks
        Tracks = lTracks
        Library = lLibrary
        FileSystem = lFileSystem

    End Sub

    Dim Checks As Boolean
    Dim Tracks As Boolean
    Dim Library As Boolean
    Dim FileSystem As Boolean

End Structure

Public Structure WordLists

    Dim simpleWords As List(Of String)
    Dim capitalWords As List(Of String)
    Dim replaceWords As String()

End Structure

Public Structure ClosestMatch

    Dim Letter As String
    Dim Aliases As List(Of String)

End Structure

Public Structure TrackData

    Public Sub New(ByVal artist As String, ByVal name As String)

        Me.Artist = artist
        Me.Name = name

    End Sub
    Dim Artist As String
    Dim Name As String

End Structure

Public Structure LogData

    Dim Destination As String
    Dim PathList As List(Of String)
    Dim TrackList As List(Of cXmlTrack)
    Dim LogType As LogDataType

    Public Sub New(ByVal dest As String, ByVal l As List(Of cXmlTrack), ByVal lType As LogDataType)
        Me.Destination = dest
        Me.TrackList = l
        Me.LogType = lType
    End Sub

    Public Sub New(ByVal dest As String, ByVal l As List(Of IITFileOrCDTrack), ByVal lType As LogDataType)
        Me.Destination = dest
        For Each t As IITFileOrCDTrack In l
            Me.TrackList.Add(New cXmlTrack(t, False))
        Next
        Me.LogType = lType
    End Sub

    Public Sub New(ByVal dest As String, ByVal l As List(Of String))
        Me.Destination = dest
        Me.PathList = l
        Me.LogType = LogDataType.GENERAL
    End Sub

End Structure

Public Structure FramePOPM

    Dim PlayedCount As Integer
    Dim Rating As Integer

    Public Overrides Function ToString() As String
        Return String.Format("PlayedCount: {0}, Rating: {1}", PlayedCount, Rating)
    End Function

End Structure

<Serializable()> Public Structure RatingWeights

    Dim PlayedCount As Int16
    Dim SkippedCount As Int16
    Dim LastPlayed As Int16
    Dim DateAdded As Int16
    Dim LongSongDuration As Integer
    Dim ScaleDuration As Double
    Dim ReduceScaleLongTracks As Boolean

    Public Overrides Function ToString() As String
        Return String.Format("PlayedCount: {0}%, SkippedCount: {1}%, LastPlayed: {2}%, DateAdded: {3}%", PlayedCount, SkippedCount, LastPlayed, DateAdded)
    End Function

End Structure

<Serializable()> Public Structure RatingParams

    Dim MaxPlayedCount As Double
    Dim MaxTrackDuration As Double
    Dim AvgTrackDuration As Double

    Public Overrides Function ToString() As String
        Return String.Format("Maximum Played Count: {0}, Maximum Track Duration: {1}, Average Track Duration: {2}", MaxPlayedCount, MaxTrackDuration, AvgTrackDuration)
    End Function

End Structure

#End Region