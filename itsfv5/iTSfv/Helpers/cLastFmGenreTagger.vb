Imports System.Xml
Imports iTunesLib
Imports System.IO
Imports System.ComponentModel

Public Class cLastFmGenreTagger

    Private Const mTagsUrl As String = "http://ws.audioscrobbler.com/1.0/track/%Artist%/%Name%/toptags.xml"

    Private mBwApp As BackgroundWorker
    Private mDisc As cInfoDisc = Nothing
    Private mDiscGenres As New Dictionary(Of String, Integer)
    Private mGenre As String = String.Empty
    Private mWordLists As New WordLists

    Public Sub New(ByVal lDisc As cInfoDisc, ByVal lBwApp As BackgroundWorker)

        mDisc = lDisc
        mBwApp = lBwApp

        mWordLists.simpleWords = mfGetSimpleWordsList()
        mWordLists.capitalWords = mfGetCapitalWordsList()

        If My.Settings.GenreScanAll = True Then

            mProgressTracksMax = lDisc.Tracks.Count
            mProgressTracksCurrent = 0

            For Each track As IITFileOrCDTrack In lDisc.Tracks

                sPausePendingCheck()
                If mBwApp.CancellationPending Then
                    Exit For
                End If

                mProgressTracksCurrent += 1

                sAddGenre(Me.fFindGenre(track.Artist, track.Name))

            Next

            mGenre = fGetTopGenre()
            mfUpdateStatusBarText("Choosing most common Genre: " + mGenre, True)

        Else

            Dim i As Integer = 0
            While String.Empty = mGenre AndAlso i < lDisc.Tracks.Count
                mGenre = fFindGenre(lDisc.Tracks(i).Artist, lDisc.Tracks(i).Name)
                i += 1
            End While

        End If

        msAppendDebug(String.Format("Choosing Genre: ""{0}"" from Last.fm", mGenre))

    End Sub

    Private Sub sPausePendingCheck()

        '' 5.34.14.1 Pressing Stop button did not pause the currently active job [Jojo]

        If mJobPaused And mBwApp.CancellationPending = False Then
            Threading.Thread.Sleep(2000)
            Call sPausePendingCheck()
        End If

    End Sub

    Private Function fGetTopGenre() As String

        Dim topHit As Integer = 0
        Dim topGenre As String = mGenre

        If mDisc.Tracks.Count > 0 And mDiscGenres.Count > 0 Then

            Dim et As IEnumerator = mDiscGenres.GetEnumerator

            Dim de As System.Collections.Generic.KeyValuePair(Of String, Integer)

            While et.MoveNext
                de = CType(et.Current, KeyValuePair(Of String, Integer))
                If String.IsNullOrEmpty(de.Key) = False AndAlso CInt(de.Value) > topHit Then
                    topHit = CInt(de.Value)
                    topGenre = CStr(de.Key)
                End If
            End While

        End If

        Return topGenre

    End Function

    Private Sub sAddGenre(ByVal genre As String)

        If mDiscGenres.ContainsKey(genre) Then
            mDiscGenres.Item(genre) += 1
        Else
            mDiscGenres.Add(genre, 1)
        End If

    End Sub

    Public Sub New(ByVal lArtist As String, ByVal lName As String)
        mGenre = fFindGenre(lArtist, lName)
    End Sub

    Private Function fFindGenre(ByVal lArtist As String, ByVal lName As String) As String

        Dim lGenre As String = String.Empty

        Try
            Dim lTagsUrl As String = mTagsUrl
            lTagsUrl = lTagsUrl.Replace("%Artist%", mfEncodeUrl(mfEncodeUrl(lArtist)))
            lTagsUrl = lTagsUrl.Replace("%Name%", mfEncodeUrl(mfEncodeUrl(lName)))

            'lTagsUrl = lTagsUrl.Replace("%2f", "%252F")
            ' Console.WriteLine(lTagsUrl)

            mfUpdateStatusBarText(String.Format("Looking up Genre for ""{0}"" using Last.fm...", lName), True)

            Dim strm As Stream = mfGetStream(lTagsUrl, "while accessing Last.fm Web Services")
            Threading.Thread.Sleep(1000) ' last.fm needs a 1 second break

            If strm IsNot Nothing Then

                Dim xtr As New XmlTextReader(strm)

                While xtr.Read

                    If xtr.NodeType = XmlNodeType.Element Then

                        If xtr.Name.Equals("name") Then
                            lGenre = mfGetFixedCase(mfGetFixedString(xtr.ReadInnerXml), mWordLists)
                            'Console.WriteLine(lGenre)
                            Exit While
                        End If

                    End If

                End While

            End If

        Catch ex As Exception
            msAppendWarnings(ex.Message + " while finding genre using Last.fm")
        End Try

        'Console.Writeline("Found: " & lGenre)

        Return lGenre

    End Function


    Public ReadOnly Property Genre() As String
        Get
            Return mGenre
        End Get
    End Property


End Class
