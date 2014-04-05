Imports iTunesLib
Imports System.Text
Imports System.IO
Imports System.Text.RegularExpressions
Imports UploadersLib.OtherServices

Public Module mLibrary

    Friend mListTracksCountUpdated As New List(Of String)
    Friend mListAlbumsInconsecutiveTracks As New List(Of String)

    Friend mSecondsLeftToAddMusic As Integer = CInt(My.Settings.AddMusicAutoFreq * 60)

    Public Function fGetArtist(ByVal track As cXmlTrack) As String

        If track.Artist IsNot Nothing Then
            Return track.Artist
        Else
            Return track.AlbumArtist
        End If

        Return UNKNOWN_ARTIST

    End Function

    Public Function fGetArtist(ByVal track As IITTrack) As String

        If track.Artist IsNot Nothing Then
            Return track.Artist
        Else
            If track.Kind = ITTrackKind.ITTrackKindFile Then
                If CType(track, IITFileOrCDTrack).AlbumArtist IsNot Nothing Then
                    Return CType(track, IITFileOrCDTrack).AlbumArtist
                End If
            End If
        End If

        Return UNKNOWN_ARTIST

    End Function

    Public Function mfHasLyrics(ByVal track As iTunesLib.IITFileOrCDTrack, ByVal minChar As Decimal) As Boolean

        Dim boo As Boolean = True

        Dim locTrack As String = "dead track"
        Try
            locTrack = track.Location
        Catch ex As Exception
        End Try

        '' 5.34.5.1 Minimum number of characters in Lyrics tag before track is considered to have no lyrics is now optional in Options > Checks [Jojo]
        Try
            If track.Lyrics Is Nothing OrElse track.Lyrics.ToString.Length <= minChar Then
                boo = False
            End If
        Catch ex As Exception
            msAppendWarnings(ex.Message + " while checking for lyrics in " + locTrack)
        End Try

        Return boo

    End Function

    Private Function checkSongExists(ByVal artist As String, ByVal song As String) As Lyrics

        Dim lyrics As UploadersLib.OtherServices.Lyrics

        Dim lyricsWiki As New Lyricsfly(Application.ProductName, Application.ProductName)
        lyrics = lyricsWiki.SearchLyrics(artist, song)

        Return lyrics

    End Function

    Public Function mfGetLyrics(ByVal track As cXmlTrack) As Lyrics

        Dim artist As String = ""
        Dim song As String = ""

        Dim lws As New Lyrics

        Try
            artist = track.Artist
            song = track.Name
            mfUpdateStatusBarText(String.Format("Looking up for Lyrics for Artist: ""{0}"", Song: ""{1}""", artist, song), True)
            msAppendDebug(String.Format("Looking up for Lyrics for Artist: ""{0}"", Song: ""{1}""", artist, song))
            lws = checkSongExists(track.Artist, track.Name)

            If lws IsNot Nothing AndAlso Not String.IsNullOrEmpty(lws.Text) Then

                mfUpdateStatusBarText(String.Format("Found Lyrics for Artist: ""{0}"", Song: ""{1}""", artist, song), True)
                msAppendDebug(String.Format("Found Lyrics for Artist: ""{0}"", Song: ""{1}""", artist, song))

            ElseIf (artist <> track.AlbumArtist) Then

                artist = track.AlbumArtist
                song = track.Name
                mfUpdateStatusBarText(String.Format("Looking up for Lyrics for Album Artist: ""{0}"", Song: ""{1}""", artist, song), True)
                msAppendDebug(String.Format("Looking up for Lyrics for Album Artist: ""{0}"", Song: ""{1}""", artist, song))
                lws = checkSongExists(track.AlbumArtist, track.Name)

                If lws IsNot Nothing AndAlso Not String.IsNullOrEmpty(lws.Text) Then

                    mfUpdateStatusBarText(String.Format("Found Lyrics for Artist: ""{0}"", Song: ""{1}""", artist, song), True)
                    msAppendDebug(String.Format("Found Lyrics for Artist: ""{0}"", Song: ""{1}""", artist, song))

                ElseIf (song <> mfGetNameToSearch(track)) Then

                    artist = track.Artist
                    song = mfGetNameToSearch(track)
                    mfUpdateStatusBarText(String.Format("Looking up for Lyrics for Artist: ""{0}"", Song: ""{1}""", artist, song), True)
                    msAppendDebug(String.Format("Looking up for Lyrics for Artist: ""{0}"", Song: ""{1}""", artist, song))
                    lws = checkSongExists(track.Artist, mfGetNameToSearch(track))

                    If lws IsNot Nothing AndAlso Not String.IsNullOrEmpty(lws.Text) Then
                        mfUpdateStatusBarText(String.Format("Found Lyrics for Artist: ""{0}"", Song: ""{1}""", artist, song), True)
                        msAppendDebug(String.Format("Found Lyrics for Artist: ""{0}"", Song: ""{1}""", artist, song))

                    ElseIf (artist <> track.AlbumArtist And song <> track.Name) Then

                        artist = track.AlbumArtist
                        song = mfGetNameToSearch(track)
                        mfUpdateStatusBarText(String.Format("Looking up for Lyrics for Album Artist: ""{0}"", Song: ""{1}""", artist, song), True)
                        msAppendDebug(String.Format("Looking up for Lyrics for Album Artist: ""{0}"", Song: ""{1}""", artist, song))
                        lws = checkSongExists(track.AlbumArtist, mfGetNameToSearch(track))

                        If lws IsNot Nothing AndAlso Not String.IsNullOrEmpty(lws.Text) Then
                            mfUpdateStatusBarText(String.Format("Found Lyrics for Artist: ""{0}"", Song: ""{1}""", artist, song), True)
                            msAppendDebug(String.Format("Found Lyrics for Artist: ""{0}"", Song: ""{1}""", artist, song))
                        End If

                    End If

                End If

            End If

        Catch ex As Exception
            msAppendWarnings(ex.Message)
        End Try

        Dim iso8859 As Encoding = Encoding.GetEncoding("ISO-8859-1")

        If lws IsNot Nothing AndAlso Not String.IsNullOrEmpty(lws.Text) Then
            lws.Text = Encoding.UTF8.GetString(iso8859.GetBytes(lws.Text))
        End If

        Return lws

    End Function

    Public Function mfFileIsInMusicFolder(ByVal filepath As String) As Boolean

        For Each loc As String In mpMusicFolderPaths
            If filepath.IndexOf(loc) <> -1 Then
                Return True
            End If
        Next

        Return False

    End Function

    Public Function fGetAlbumArtist(ByVal f As TagLib.File) As String
        If f.Tag.AlbumArtists.Length > 0 Then
            Return f.Tag.AlbumArtists(0)
        Else
            Return VARIOUS_ARTISTS
        End If
    End Function

    Public Function mDiscIsComplete(ByVal lDisc As cXmlDisc) As Boolean

        Dim booComplete As Boolean = False

        Dim trackNumbers As New List(Of Integer)
        For Each track As cXmlTrack In lDisc.Tracks
            Try ' to prevent track deleted error 
                If track.Location IsNot Nothing Then trackNumbers.Add(track.TrackNumber)
            Catch ex As Exception
                msAppendWarnings(ex.Message)
            End Try
        Next

        trackNumbers.Sort()

        If trackNumbers.Count > 0 Then
            booComplete = (1 = trackNumbers(0)) AndAlso mDiscHasConsecutiveTracks(lDisc)
        End If

        Return booComplete

    End Function

    Public Function mDiscHasConsecutiveTracks(ByVal lDisc As cXmlDisc) As Boolean

        Dim booAlbumFull As Boolean = False

        Try
            Dim trackNumbers As New List(Of Integer)
            For Each track As cXmlTrack In lDisc.Tracks
                If track.Location IsNot Nothing Then trackNumbers.Add(track.TrackNumber)
            Next
            trackNumbers.Sort()

            booAlbumFull = (1 = trackNumbers(0))

            For i As Integer = 0 To trackNumbers.Count - 2
                booAlbumFull = booAlbumFull And (trackNumbers(i + 1) - trackNumbers(i) = 1)
            Next
        Catch ex As Exception
            ' we will implement an error handled IITFileOrCDTrack by iTSfv 6.0
        End Try

        Return booAlbumFull

    End Function

    Public Function mGetAlbumArtist(ByVal track As cXmlTrack) As String

        If track.AlbumArtist IsNot Nothing Then
            Return track.AlbumArtist
        ElseIf track.Artist IsNot Nothing Then
            Return track.Artist
        End If

        Return VARIOUS_ARTISTS

    End Function

    Public Function mGetAlbumArtist(ByVal track As IITTrack) As String

        If track.Kind = ITTrackKind.ITTrackKindFile Then
            If CType(track, IITFileOrCDTrack).AlbumArtist IsNot Nothing Then
                Return CType(track, IITFileOrCDTrack).AlbumArtist
            ElseIf track.Artist IsNot Nothing Then
                Return track.Artist
            End If
        End If

        Return VARIOUS_ARTISTS

    End Function

    ''' <summary>
    ''' Replaces CDM with Empty String
    ''' </summary>
    ''' <remarks></remarks>
    Public Function fGetAlbumToSearch(ByVal track As IITTrack) As String

        Dim temp As String = fGetAlbum(track)
        Dim skipWords As List(Of String) = mfGetSkipAlbumWords()

        If skipWords IsNot Nothing Then
            For Each s As String In skipWords
                If temp.Contains(s) Then
                    temp = temp.Replace(s, String.Empty).Trim
                End If
            Next
        End If

        Return temp

    End Function

    Public Function mfTagLibAlbumArtist(ByVal lAlbumArtists As String()) As String

        Dim sb As New StringBuilder

        If lAlbumArtists.Length > 0 Then

            If lAlbumArtists.Length = 2 Then
                '' fix for AC/DC
                If lAlbumArtists(0).Equals("AC") AndAlso lAlbumArtists(1).Equals("DC") Then
                    sb.Append(lAlbumArtists(0))
                    sb.Append("/")
                    sb.Append(lAlbumArtists(1))
                Else
                    '' otherwise
                    sb.Append(lAlbumArtists(0))
                End If

            Else
                '' in all other cases
                sb.Append(lAlbumArtists(0))
            End If

        End If

        Return sb.ToString

    End Function


    Public Function mGetAlbumName(ByVal track As cXmlTrack) As String

        Dim strArtist As String = String.Empty
        Dim strAlbum As String = fGetAlbum(track).Trim

        If track.Compilation = True Then

            strArtist = VARIOUS_ARTISTS

        Else

            If track.AlbumArtist <> "" Then
                strArtist = track.AlbumArtist.Trim
            ElseIf track.Compilation = False AndAlso track.Artist <> "" Then
                strArtist = track.Artist.Trim
            Else
                strArtist = VARIOUS_ARTISTS
            End If

        End If

        Return strArtist + " - " + strAlbum

    End Function

    Public Function mGetDiscName(ByVal track As cXmlTrack) As String

        Dim albArtist As String = String.Empty
        Dim strAlbum As String = UNKNOWN_ALBUM
        If track.Album <> String.Empty Then
            strAlbum = track.Album.Trim
        End If

        If track.Compilation = True Then

            albArtist = VARIOUS_ARTISTS

        Else

            If track.TrackType = TrackTypeXML.FILE AndAlso track.AlbumArtist <> "" Then
                albArtist = track.AlbumArtist.Trim
            ElseIf track.Compilation = False AndAlso track.Artist <> "" Then
                albArtist = track.Artist.Trim
            Else
                albArtist = VARIOUS_ARTISTS
            End If

        End If

        '' 5.35.2.8 First disc of a box set did not follow the list by Grouping tag in Discs Browser
        If track.DiscNumber > 1 Or (track.DiscNumber >= 1 AndAlso track.DiscCount > 1) Then
            ' Multi Disk View Settings
            Dim strDisCnt As String = String.Empty
            If track.DiscCount >= 1 Then
                strDisCnt = "/" + track.DiscCount.ToString("00")
            End If

            Select Case My.Settings.AlbumBrowserMode
                Case Is = 0
                    albArtist = String.Format("{0} - {1} Disk {2} - {3}", albArtist, track.Grouping, track.DiscNumber.ToString("00"), strAlbum)
                Case Is = 1
                    albArtist = String.Format("{0} - {1} (Disk {2})", albArtist, strAlbum, track.DiscNumber.ToString("00"))
                Case Is = 2
                    albArtist = String.Format("{0} - {1} (Disk {2})", strAlbum, albArtist, track.DiscNumber.ToString("00"))
            End Select

        Else
            ' Single Disks
            Select Case My.Settings.AlbumBrowserMode
                Case Is = 2
                    albArtist = String.Format("{0} - {1}", strAlbum, albArtist)
                Case Else
                    albArtist = String.Format("{0} - {1}", albArtist, strAlbum)
            End Select
        End If

        'Console.Writeline(albArtist)

        Return albArtist

    End Function


    Public Function fGetAlbum(ByVal track As TagLib.File) As String
        If track.Tag.Album <> String.Empty Then
            Return track.Tag.Album
        ElseIf track.Name <> String.Empty Then
            Return mfGetNameToSearch(track)
        Else
            Return UNKNOWN_ALBUM
        End If
    End Function
    Public Function fGetAlbum(ByVal track As cXmlTrack) As String
        If track.Album <> String.Empty Then
            Return track.Album
        ElseIf track.Name <> String.Empty Then
            Return mfGetNameToSearch(track)
        Else
            Return UNKNOWN_ALBUM
        End If
    End Function

    Public Function fGetAlbum(ByVal track As IITTrack) As String
        If track.Album <> String.Empty Then
            Return track.Album
        ElseIf track.Name <> String.Empty Then
            Return mfGetNameToSearch(track)
        Else
            Return UNKNOWN_ALBUM
        End If
    End Function

    Public Function mfGetNameToSearch(ByVal track As cXmlTrack) As String

        Dim last As Integer = 0

        If track.Name IsNot String.Empty Then
            last = track.Name.IndexOf("(")
            If last = -1 Then
                last = track.Name.IndexOf("[")
                If last = -1 Then
                    last = track.Name.IndexOf("{")
                End If
            End If
        End If

        If last > 0 Then
            Return track.Name.Substring(0, last).Trim
        End If

        Return track.Name

    End Function

    Public Function mfGetNameToSearch(ByVal track As TagLib.File) As String

        Dim last As Integer = 0

        If track.Tag.Title IsNot String.Empty Then
            last = track.Tag.Title.IndexOf("(")
            If last = -1 Then
                last = track.Tag.Title.IndexOf("[")
                If last = -1 Then
                    last = track.Tag.Title.IndexOf("{")
                End If
            End If
        End If

        If last > 0 Then
            Return track.Tag.Title.Substring(0, last).Trim
        End If

        Return track.Tag.Title

    End Function

    Public Function mfGetNameToSearch(ByVal track As IITTrack) As String

        Dim last As Integer = 0

        If track.Name IsNot String.Empty Then
            last = track.Name.IndexOf("(")
            If last = -1 Then
                last = track.Name.IndexOf("[")
                If last = -1 Then
                    last = track.Name.IndexOf("{")
                End If
            End If
        End If

        If last > 0 Then
            Return track.Name.Substring(0, last).Trim
        End If

        Return track.Name

    End Function

    ' moved fGetFixedString to mAdapter.vb

    Friend Function fIsItunesStoreStandard(ByVal lTrack As cXmlTrack) As Boolean

        lTrack.Notes = " -- "

        If lTrack.TrackNumber = 0 Then
            If mListTracksNoTrackNum.Contains(lTrack.Location) = False Then
                mListTracksNoTrackNum.Add(lTrack.Location)
            End If
            lTrack.Notes += "Track Number; "
        End If

        If lTrack.Genre Is Nothing Then
            lTrack.Notes += "Genre; "
        End If

        If lTrack.Artwork Is Nothing Then
            lTrack.Notes += "Artwork; "
            If mListFoldersNoArtwork.Contains(lTrack.Location) = False Then
                mListFoldersNoArtwork.Add(lTrack.Location)
            End If
        End If

        If lTrack.TrackCount = 0 Then
            lTrack.Notes += "TrackCount; "
        End If

        If lTrack.DiscNumber = 0 Then
            lTrack.Notes += "DiscNumber; "
        End If

        If lTrack.DiscCount = 0 Then
            lTrack.Notes += "DiscCount; "
        End If

        If Not lTrack.Year > 0 Then
            lTrack.Notes += "Year; "
        End If

        Return lTrack.TagsComplete

    End Function

    Public Function mfUpdateInfoFromFile(ByVal track As IITFileOrCDTrack) As Boolean

        Dim success As Boolean = False

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

            Dim f As TagLib.File = TagLib.File.Create(track.Location)

            mfUpdateStatusBarText(String.Format("Refreshing tags in ""{0}""", track.Name), False)

            'Dim apple_tag As TagLib.Mpeg4.AppleTag = CType(f.GetTag(TagLib.TagTypes.Apple, True), TagLib.Mpeg4.AppleTag)
            'If apple_tag IsNot Nothing Then
            '    apple_tag.IsCompilation = True ' TEST ONLY
            'End If

            Dim id32_tag As TagLib.Id3v2.Tag = CType(f.GetTag(TagLib.TagTypes.Id3v2, True), TagLib.Id3v2.Tag)

            If id32_tag IsNot Nothing Then

                '' id32_tag.IsCompilation = True ' TEST ONLY

                TagLib.Id3v2.Tag.DefaultVersion = 3
                TagLib.Id3v2.Tag.ForceDefaultVersion = True

            End If

            If f.Tag.Track <> 0 Then
                track.TrackNumber = CInt(f.Tag.Track)
            ElseIf track.TrackNumber > 0 Then
                f.Tag.Track = CUInt(track.TrackNumber)
            End If

            If f.Tag.TrackCount <> 0 Then
                track.TrackCount = CInt(f.Tag.TrackCount)
            ElseIf track.TrackCount > 0 Then
                f.Tag.TrackCount = CUInt(track.TrackCount)
            End If

            If f.Tag.Disc <> 0 Then
                track.DiscNumber = CInt(f.Tag.Disc)
            Else
                f.Tag.Disc = CUInt(track.DiscNumber)
            End If

            If f.Tag.DiscCount <> 0 Then
                track.DiscCount = CInt(f.Tag.DiscCount)
            Else
                f.Tag.DiscCount = CUInt(track.DiscCount)
            End If

            If f.Tag.Year <> 0 Then
                track.Year = CInt(f.Tag.Year)
            ElseIf track.Year > 0 Then
                f.Tag.Year = CUInt(track.Year)
            End If

            If f.Tag.Title IsNot Nothing Then
                track.Name = f.Tag.Title.Trim
            ElseIf String.IsNullOrEmpty(track.Name) = False Then
                f.Tag.Title = track.Name
            End If

            If f.Tag.Album IsNot Nothing Then
                track.Album = f.Tag.Album.Trim
            ElseIf track.Album IsNot Nothing Then
                f.Tag.Album = track.Album
            End If

            If f.Tag.FirstPerformer IsNot Nothing Then
                track.Artist = f.Tag.FirstPerformer.Trim
            ElseIf String.IsNullOrEmpty(track.Artist) = False Then
                f.Tag.Performers = New String() {track.Artist}
            End If

            If f.Tag.FirstAlbumArtist IsNot Nothing Then
                track.AlbumArtist = mfTagLibAlbumArtist(f.Tag.AlbumArtists).Trim
            ElseIf String.IsNullOrEmpty(track.AlbumArtist) = False Then
                f.Tag.AlbumArtists = New String() {track.AlbumArtist}
            End If

            If f.Tag.FirstComposer IsNot Nothing Then
                track.Composer = f.Tag.FirstComposer.Trim
            End If

            If f.Tag.FirstGenre IsNot Nothing Then
                track.Genre = f.Tag.FirstGenre.Trim
            Else
                f.Tag.Genres = New String() {track.Genre}
            End If

            If mfCommentIsJunk(f.Tag.Comment) Or mfCommentIsJunk(track.Comment) Then
                f.Tag.Comment = ""
                track.Comment = ""
            Else
                track.Comment = f.Tag.Comment
            End If

            f.Save()
            track.UpdateInfoFromFile()

            Dim dAfter As Date = track.ModificationDate

            If dAfter <> dBefore Then
                mListTagsRefreshed.Add(track.Location)
            End If

            ''***************************************************
            ''* Retain Modified Date - last thing 1 after editing
            ''***************************************************
            If My.Settings.ModifiedDateRetain = True Then
                File.SetLastWriteTime(fiTrack.FullName, dBefore)
            End If

            ''************************************************
            ''* Restore Read-Only - last thing 2 after editing
            ''************************************************
            If wasReadOnly Then
                fiTrack.IsReadOnly = True
                msAppendDebug(String.Format("Undo clear {0} Read-Only flag", fiTrack.FullName))
            End If

            success = True

        Catch ex As Exception
            msAppendWarnings(ex.Message + " while updating tags from file")
        End Try

        Return success

    End Function

    Public Function mfCommentIsJunk(ByVal comment As String) As Boolean

        ' test for shit like  0000079F 000007D8 00001C6D 000018D1 0002950F 0002953D 00006700 00008000 0001CEDB 000110BD
        Dim success As Boolean = True

        If Not String.IsNullOrEmpty(comment) Then
            comment = comment.Trim
            Dim groups As String() = comment.Split(CChar(" "))
            'success = (groups.Length = 10)
            For Each word In groups
                success = success And (word.Length = 8)
            Next
            'If success = True Then

            'End If

        End If

        Return success

    End Function

    Public Function mfGetFixedSpaces(ByVal str As String) As String

        While str.Contains("  ")
            str = str.Replace("  ", " ")
        End While

        Return str

    End Function

    Public Function mfGetStringFromPattern(ByVal pattern As String, ByVal track As cXmlTrack) As String

        If File.Exists(track.Location) Then
            pattern = pattern.Replace("%FileName%", Path.GetFileNameWithoutExtension(track.Location))
            pattern = pattern.Replace("%Folder%", Path.GetDirectoryName(track.Location))
            pattern = pattern.Replace("%Location%", track.Location)
        End If

        pattern = pattern.Replace("\n", Environment.NewLine)

        If track.Name IsNot Nothing Then
            pattern = Regex.Replace(pattern, "%Name%", track.Name, RegexOptions.IgnoreCase)
        End If

        pattern = Regex.Replace(pattern, "%Artist%", fGetArtist(track).Trim, RegexOptions.IgnoreCase)
        pattern = Regex.Replace(pattern, "%Album%", fGetAlbum(track).Trim, RegexOptions.IgnoreCase)
        pattern = pattern.Replace("%Grouping%", track.Grouping)
        pattern = Regex.Replace(pattern, "%AlbumArtist%", mGetAlbumArtist(track).Trim, RegexOptions.IgnoreCase)
        pattern = Regex.Replace(pattern, "%album artist%", mGetAlbumArtist(track).Trim, RegexOptions.IgnoreCase)
        pattern = pattern.Replace("%Composer%", track.Composer)
        pattern = pattern.Replace("%Conductor%", track.Conductor)

        pattern = Regex.Replace(pattern, "%BitRate%", track.BitRate.ToString, RegexOptions.IgnoreCase)

        pattern = pattern.Replace("%TrackNumber%", track.TrackNumber.ToString("00"))
        pattern = pattern.Replace("%TrackCount%", track.TrackCount.ToString("00"))

        If track.DiscNumber > 0 Then
            If track.DiscCount > 9 Then
                pattern = pattern.Replace("%DiscNumber%", track.DiscNumber.ToString("00"))
            Else
                pattern = pattern.Replace("%DiscNumber%", track.DiscNumber.ToString)
            End If
        Else
            pattern = pattern.Replace("(%DiscNumber%)", String.Empty)
            pattern = pattern.Replace("%DiscNumber%", String.Empty)
        End If

        pattern = pattern.Replace("%DiscCount%", track.DiscCount.ToString)

        If track.Year > 0 Then
            pattern = Regex.Replace(pattern, "%Year%", track.Year.ToString, RegexOptions.IgnoreCase)
        Else
            pattern = Regex.Replace(pattern, "\(%Year%\)", "", RegexOptions.IgnoreCase)
            pattern = Regex.Replace(pattern, "%Year%", "", RegexOptions.IgnoreCase)
        End If

        If String.IsNullOrEmpty(track.Genre) Then
            pattern = pattern.Replace("%Genre%", UNKNOWN_GENRE)
        Else
            pattern = pattern.Replace("%Genre%", track.Genre)
        End If

        pattern = pattern.Replace("%PlayedCount%", track.PlayedCount.ToString)
        pattern = pattern.Replace("%Rating%", track.Rating.ToString)

        Return pattern

    End Function

    Public Function mfGetStringFromSyntax(ByVal pattern As String, ByVal track As cXmlTrack) As String

        pattern = mfGetStringFromScript(pattern, track)
        pattern = mfGetStringFromPattern(pattern, track)
        pattern = mfGetFixedSpaces(pattern)

        Return pattern

    End Function

    Public Function mfGetStringFromPattern(ByVal pattern As String, ByVal track As IITTrack) As String

        Dim xt As New cXmlTrack(track, False)
        Return mfGetStringFromSyntax(pattern, xt)

    End Function

End Module
