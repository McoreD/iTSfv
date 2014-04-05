Imports System.Net
Imports System.IO
Imports System.Xml
Imports System.ComponentModel

Public Class cLastFmPlayCounts

    Private mUserName As String = String.Empty
    Private mSongs As New Dictionary(Of String, cXmlTrack)
    Private mSongKeys As New List(Of String)

    Private Const mUrlChartsList As String = "http://ws.audioscrobbler.com/2.0/user/%UserName%/weeklychartlist.xml"
    Private Const mUrlChart As String = "http://ws.audioscrobbler.com/2.0/user/%UserName%/weeklytrackchart.xml?from=%From%&to=%To%"

    Private mBwApp As BackgroundWorker

    Public Sub New(ByVal lUserName As String, ByVal lBwApp As BackgroundWorker)

        mUserName = lUserName
        mBwApp = lBwApp

        mfUpdateStatusBarText(String.Format("Loading Last.fm Charts for {0}...", My.Settings.LastFmUserName), secondary:=False)

        Dim listCharts As New List(Of String)
        Dim gUrlChart As String = mUrlChart.Replace("%UserName%", lUserName)

        '' http://support.microsoft.com/kb/307643
        '' read all the chart URLs

        Dim strm As Stream = mfGetStream(mUrlChartsList.Replace("%UserName%", lUserName), "while accessing Last.fm Profile data")

        If strm IsNot Nothing Then

            ' Try

            Dim xtrCharts As New XmlTextReader(strm)

            While xtrCharts.Read

                If mBwApp.CancellationPending Then
                    Exit Sub
                End If

                mProgressTracksCurrent = -1

                If xtrCharts.NodeType = XmlNodeType.Element Then

                    If xtrCharts.Name.Equals("chart") Then
                        xtrCharts.MoveToNextAttribute()
                        Dim urlChart As String = gUrlChart.Replace("%From%", xtrCharts.Value)
                        xtrCharts.MoveToNextAttribute()
                        urlChart = urlChart.Replace("%To%", xtrCharts.Value)
                        'Console.Writeline(urlChart)
                        listCharts.Add(urlChart)
                    End If

                End If

            End While

            mProgressTracksMax = listCharts.Count
            mProgressTracksCurrent = 0

            For Each url As String In listCharts

                If mBwApp.CancellationPending Then
                    Exit Sub
                End If

                Dim xtrSongs As New XmlTextReader(url)

                Dim song As New cXmlTrack

                While xtrSongs.Read

                    If xtrSongs.NodeType = XmlNodeType.Element Then
                        If xtrSongs.Name.Equals("artist") Then
                            song.Artist = xtrSongs.ReadInnerXml
                        End If
                        If xtrSongs.Name.Equals("name") Then
                            song.Name = xtrSongs.ReadInnerXml
                        End If
                        If xtrSongs.Name.Equals("playcount") Then
                            Console.WriteLine(xtrSongs.ReadInnerXml)
                            Integer.TryParse(xtrSongs.ReadInnerXml, song.PlayedCount)
                        End If
                    End If

                    If String.Empty <> song.Artist AndAlso String.Empty <> song.Name Then

                        ''Console.Writeline(song.Artist + " - " + song.Name)
                        sAddUpdateSong(song)
                        song = New cXmlTrack

                    End If

                End While

                mProgressTracksCurrent += 1

            Next

            ' Catch ex As Exception

            ' msAppendWarnings(ex.Message + " while reading last.fm charts...")

            ' End Try

        End If ' stream is not nothing

    End Sub

    Private Sub sAddUpdateSong(ByVal song As cXmlTrack)

        Dim key As String = (song.Artist + " - " + song.Name).ToLower

        If mSongKeys.Contains(key) Then
            mSongs.Item(key).PlayedCount += song.PlayedCount
        Else
            mSongKeys.Add(key)
            mSongs.Add(key, song)
        End If

    End Sub

    Public ReadOnly Property Tracks() As Dictionary(Of String, cXmlTrack)
        Get
            Return mSongs
        End Get
    End Property

    Public Function fFindXmlTrack(ByVal lArtist As String, ByVal lSong As String) As cXmlTrack

        Dim key As String = (lArtist + " - " + lSong).ToLower

        If mSongKeys.Contains(key) Then
            Return mSongs(key)
        End If

        Return Nothing

    End Function

End Class
