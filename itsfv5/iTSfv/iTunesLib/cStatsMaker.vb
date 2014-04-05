Imports System.ComponentModel
Imports iTSfv.cBwJob

<Serializable()> Public Class cStatsMaker

    ' Statistics
    Private mGenreTable As Hashtable
    Private mGenreKeys As List(Of String)
    Private mArtistsTable As Hashtable
    Private mArtistsKeys As List(Of String)
    Private mAlbumArtistsTable As Hashtable
    Private mAlbumArtistsKeys As List(Of String)
    Private mAlbumsTable As Hashtable
    Private mAlbumsKeys As List(Of String)
    Private mTracksTable As Hashtable
    Private mTracksKeys As List(Of String)

    ' Rating parameters - for the initial round we do some guesses
    Private mMaxPlayedCount As Integer = 0
    Private mMaxTrackDuration As Integer = 0
    Private mAvgTrackDuration As Double = 0

    Private mLongestTrack As New cXmlTrack()

    Dim mCountFileTracks As Integer = 0

    Private mTopGenre As List(Of String)
    Private mTopAlbumArtists As List(Of String)
    Private mTopArtists As List(Of String)
    Private mTopAlbums As List(Of String)
    Private mTopTracks As List(Of String)

    Private mRatingAdjuster As cRatingAdjuster
    Private mRatingParams As RatingParams

    Private mTotalPlayTime As UInt64 = 0
    Public TOP_FIVE As UInt16 = 10

    Public ReadOnly Property MaxPlayedCount() As Double
        Get
            Return mMaxPlayedCount
        End Get
    End Property

    Public ReadOnly Property MaxTrackDuration() As Double
        Get
            Return mMaxTrackDuration
        End Get
    End Property

    Public ReadOnly Property AvgTrackDuration() As Double
        Get
            Return mAvgTrackDuration
        End Get
    End Property

    Private mName As String = IO.Path.GetFileName(mStatsFilePath)
    Public Property Name() As String
        Get
            Return mName
        End Get
        Set(ByVal value As String)
            mName = value
        End Set
    End Property

    Private mDateStats As Date = Today
    Public Property DateStats() As Date
        Get
            Return mDateStats
        End Get
        Set(ByVal value As Date)
            mDateStats = value
        End Set
    End Property

    Private Function IsValidTrack(ByVal xt As cXmlTrack) As Boolean

        Dim bValid As Boolean = xt.TrackType = TrackTypeXML.FILE ' if tracks is a mp3 file
        bValid = bValid And mfTagsComplete(track:=xt) ' if track is a properly tagged file

        Return bValid

    End Function


    Public Sub New(ByVal bwApp As BackgroundWorker, ByVal xmlParser As cLibraryParser)

        TOP_FIVE = CUShort(My.Settings.PieNumber)

        mGenreTable = New Hashtable
        mGenreKeys = New List(Of String)
        mAlbumArtistsTable = New Hashtable
        mAlbumArtistsKeys = New List(Of String)
        mArtistsTable = New Hashtable
        mArtistsKeys = New List(Of String)
        mAlbumsTable = New Hashtable
        mAlbumsKeys = New List(Of String)
        mTracksTable = New Hashtable
        mTracksKeys = New List(Of String)

        mRatingParams.AvgTrackDuration = My.Settings.AvgTrackDuration
        mRatingParams.MaxPlayedCount = My.Settings.MaxPlayedCount
        mRatingParams.MaxTrackDuration = My.Settings.MaxTrackDuration

        'mRatingWeights.PlayedCount = CType(My.Settings.WeightPlayedCount, Short)
        'mRatingWeights.SkippedCount = CType(My.Settings.WeightSkippedCount, Short)
        'mRatingWeights.LastPlayed = CType(My.Settings.WeightLastPlayed, Short)
        'mRatingWeights.DateAdded = CType(My.Settings.WeightDateAdded, Short)

        mRatingAdjuster = New cRatingAdjuster(mRatingWeights, mRatingParams)

        If bwApp.IsBusy Then
            bwApp.ReportProgress(ProgressType.UPDATE_TRACKS_PROGRESS_BAR_MAX, xmlParser.TrackCollection.Count)
        End If

        ' Fixed [ itsfv-Bugs-1790526 ] Arithmetic operation resulted in an overflow while adjust ratings [Andre]
        ' integer to double
        Dim lTotalTrackDuration As Double = 0

        mProgressDiscsMax = 0
        mProgressDiscsCurrent = 0

        For Each track As cXmlTrack In xmlParser.TrackCollection

            If IsValidTrack(xt:=track) Then

                sLoadTrackToTracksTable(CType(track, cXmlTrack))

                mCountFileTracks += 1

                ' need for adjusting rating
                mMaxPlayedCount = Math.Max(mMaxPlayedCount, track.PlayedCount)
                If track.Duration > mMaxTrackDuration Then

                    mMaxTrackDuration = track.Duration
                    mLongestTrack.Name = track.Name
                    mLongestTrack.Artist = track.Artist
                    mLongestTrack.Album = track.Album

                End If

                lTotalTrackDuration += track.Duration
                mTotalPlayTime = CULng(mTotalPlayTime + (track.Duration * track.PlayedCount))

                ' for statistics, the subs take ONLY fully tagged tags
                sLoadTrackToGenreTable(CType(track, cXmlTrack))
                sLoadTrackToArtistsTable(CType(track, cXmlTrack))
                sLoadTrackToAlbumArtistsTable(CType(track, cXmlTrack))
                sLoadTrackToAlbumsTable(CType(track, cXmlTrack))

            End If

            If bwApp.CancellationPending Then
                Exit Sub
            End If

            If bwApp.IsBusy Then
                bwApp.ReportProgress(ProgressType.PARSING_ITUNES_LIBRARY)
            End If

        Next

        mfUpdateStatusBarText("Gathered Statistics.", True)

        If mCountFileTracks > 0 Then
            mAvgTrackDuration = CType(lTotalTrackDuration / mCountFileTracks, Double)
        End If

        mRatingParams.AvgTrackDuration = mAvgTrackDuration
        mRatingParams.MaxPlayedCount = mMaxPlayedCount
        mRatingParams.MaxTrackDuration = mMaxTrackDuration

        ' update config file with new stats
        My.Settings.MaxPlayedCount = mMaxPlayedCount
        My.Settings.MaxTrackDuration = mMaxTrackDuration
        My.Settings.AvgTrackDuration = mAvgTrackDuration

    End Sub

    Public ReadOnly Property RatingParameters() As RatingParams
        Get
            Return mRatingParams
        End Get
    End Property

    Private Function fRatingParametersReady() As Boolean

        Return mRatingParams.AvgTrackDuration > 0 And mRatingParams.MaxPlayedCount > 0 And mRatingParams.MaxTrackDuration > 0

    End Function

    Private Function fGetTopValues(ByVal howMany As Integer, ByVal myView As GraphViewType, ByVal statsTable As Hashtable, ByVal statsKeys As List(Of String)) As List(Of String)

        Dim listSortedValues As New List(Of Double)
        For i As Integer = 0 To statsKeys.Count - 1

            Select Case myView

                Case GraphViewType.PREFERENCE
                    listSortedValues.Add(CType(statsTable.Item(statsKeys(i)), cStatValues).Score)
                Case GraphViewType.STORAGE
                    listSortedValues.Add(CType(statsTable.Item(statsKeys(i)), cStatValues).Count)
                Case GraphViewType.RATING
                    listSortedValues.Add(CType(statsTable.Item(statsKeys(i)), cStatValues).Rating)
            End Select

        Next

        listSortedValues.Sort()
        listSortedValues.Reverse()

        howMany = Math.Min(howMany, listSortedValues.Count)

        ' howMany will be 5 with 0 to 4 as index

        Dim topFiveKeys As New List(Of String)


        For i As Integer = 0 To howMany - 1

            For j As Integer = 0 To statsTable.Count - 1

                Dim statValue As Double

                Select Case myView

                    Case GraphViewType.PREFERENCE
                        statValue = CType(statsTable.Item(statsKeys(j)), cStatValues).Score
                    Case GraphViewType.STORAGE
                        statValue = CType(statsTable.Item(statsKeys(j)), cStatValues).Count
                    Case GraphViewType.RATING
                        statValue = CType(statsTable.Item(statsKeys(j)), cStatValues).Rating

                End Select

                If listSortedValues(i) = statValue AndAlso Not topFiveKeys.Contains(statsKeys(j)) Then
                    topFiveKeys.Add(statsKeys(j))
                    Exit For
                End If

            Next

        Next

        Return topFiveKeys


    End Function


    Public Sub sDrawPieGraphGenre(ByVal myView As GraphViewType, ByVal graphBox As PictureBox, ByVal legendBox As PictureBox)

        mTopGenre = fGetTopValues(TOP_FIVE, myView, mGenreTable, mGenreKeys)

        sDrawPieGraph(myView, mGenreTable, mTopGenre, graphBox, legendBox)

    End Sub

    Public Sub sDrawPieGraphAlbumArtists(ByVal myView As GraphViewType, ByVal graphBox As PictureBox, ByVal legendBox As PictureBox)

        mTopAlbumArtists = fGetTopValues(TOP_FIVE, myView, mAlbumArtistsTable, mAlbumArtistsKeys)

        sDrawPieGraph(myView, mAlbumArtistsTable, mTopAlbumArtists, graphBox, legendBox)

    End Sub


    Public Sub sDrawPieGraphArtists(ByVal myView As GraphViewType, ByVal graphBox As PictureBox, ByVal legendBox As PictureBox)

        mTopArtists = fGetTopValues(TOP_FIVE, myView, mArtistsTable, mArtistsKeys)

        sDrawPieGraph(myView, mArtistsTable, mTopArtists, graphBox, legendBox)

    End Sub

    Public Sub sDrawPieGraphAlbums(ByVal myView As GraphViewType, ByVal graphBox As PictureBox, ByVal legendBox As PictureBox)

        mTopAlbums = fGetTopValues(TOP_FIVE, myView, mAlbumsTable, mAlbumsKeys)

        sDrawPieGraph(myView, mAlbumsTable, mTopAlbums, graphBox, legendBox)

    End Sub
    Public Sub sDrawPieGraphTracks(ByVal myView As GraphViewType, ByVal graphBox As PictureBox, ByVal legendBox As PictureBox)

        mTopTracks = fGetTopValues(TOP_FIVE, myView, mTracksTable, mTracksKeys)

        sDrawPieGraph(myView, mTracksTable, mTopTracks, graphBox, legendBox)

    End Sub

    Public Function fGetTopGenres() As List(Of String)
        Return mTopGenre
    End Function

    Public Function fGetTopArtists() As List(Of String)
        Return mTopArtists
    End Function

    Public Function fGetTopAlbumArtists() As List(Of String)
        Return mTopAlbumArtists
    End Function

    Public Function fGetTopTracks() As List(Of String)
        Return mTopTracks
    End Function

    Private m_Rnd As Random
    ' Return a random RGB color.
    Public Function RandomRGBColor(ByVal seed As Integer) As Color

        m_Rnd = New Random(seed)

        Return Color.FromArgb(255, _
            m_Rnd.Next(0, 255), _
            m_Rnd.Next(0, 255), _
            m_Rnd.Next(0, 255))
    End Function

    Private Sub sDrawPieGraph(ByVal myView As GraphViewType, ByVal statsTable As Hashtable, ByVal topKeys As List(Of String), ByVal graphBox As PictureBox, ByVal legendBox As PictureBox)

        Dim lPieGraph As New Graphing.V3.Pie.PieChart
        Dim lBarGraph As New Graphing.V3.Bar.BarChart()
        Dim lWidth As Integer = 200
        Dim lHeight As Integer = lWidth

        'Adding pieces is fairly straight forward.

        Dim lValueTotal As Double

        Select Case myView
            Case GraphViewType.PREFERENCE
                For i As Integer = 0 To topKeys.Count - 1
                    lValueTotal += CType(statsTable(topKeys(i)), cStatValues).Score
                Next
            Case GraphViewType.STORAGE
                For i As Integer = 0 To topKeys.Count - 1
                    lValueTotal += CType(statsTable(topKeys(i)), cStatValues).Count
                Next
            Case GraphViewType.RATING
                For i As Integer = 0 To topKeys.Count - 1
                    lValueTotal = CInt(lValueTotal + CType(statsTable(topKeys(i)), cStatValues).Rating)
                Next

        End Select

        Dim max As Integer = Math.Min(TOP_FIVE, topKeys.Count)

        For i As Integer = 0 To max - 1

            Dim myColor As Color

            Select Case i
                Case 0
                    myColor = Color.Red
                Case 1
                    myColor = Color.Green
                Case 2
                    myColor = Color.Blue
                Case 3
                    myColor = Color.Pink
                Case 4
                    myColor = Color.Yellow
                Case Else
                    myColor = RandomRGBColor(i)
            End Select

            Dim lValue As Integer

            Select Case myView

                Case GraphViewType.PREFERENCE
                    lValue = CType(statsTable(topKeys(i)), cStatValues).Score

                Case GraphViewType.STORAGE
                    lValue = CType(statsTable(topKeys(i)), cStatValues).Count

                Case GraphViewType.RATING
                    lValue = CType(statsTable(topKeys(i)), cStatValues).Rating

            End Select

            Dim perc As Double = mfGetPercentage(lValue, lValueTotal)

            If perc > 0 Then
                Select Case mChartType
                    Case ChartType.BAR
                        Dim lLegend As String = String.Format("{0} ({1})", topKeys(i), lValue)
                        lBarGraph.BarSliceCollection.Add(New Graphing.V3.Bar.BarSlice(lValue, myColor, lLegend))
                    Case ChartType.PIE
                        Dim lLegend As String = String.Format("{0} ({1}%)", topKeys(i), perc)
                        lPieGraph.PieSliceCollection.Add(New Graphing.V3.Pie.PieSlice(lValue, myColor, lLegend))
                End Select
            End If

        Next

        Dim renderer As New Graphing.V3.Render

        Select Case mChartType

            Case ChartType.BAR

                lBarGraph.KeyTitle = "Legend"
                lBarGraph.KeyTitleFontStyle = FontStyle.Bold

                lBarGraph.Alignment = Graphing.V3.Base.b_BarTypes.VerticalBottom
                'Background color
                lBarGraph.Color = Color.White
                'Really does nothing in a barchart.
                lBarGraph.GraphBorder = False
                'Will autosize the chart
                lBarGraph.AutoSize = False
                lBarGraph.ImageSize = New Size(lWidth, lHeight)

                graphBox.Image = renderer.DrawChart(lBarGraph)
                legendBox.Image = renderer.DrawKey(lBarGraph)

            Case ChartType.PIE

                'Dim lWidth As Double = 200, lHeight As Double = 200

                'Specify the exact size
                '.KeyTitle = "Rating parameters were collected. " & Environment.NewLine & _
                '            "Graphs will be ready when you analyse next time."
                lPieGraph.KeyTitle = "Legend"
                lPieGraph.KeyTitleFontStyle = FontStyle.Bold
                '.ImageSize = New Size(lWidth, lHeight)
                'Or Auto size
                lPieGraph.AutoSize = True
                lPieGraph.Diameter = 160
                lPieGraph.Thickness = 5
                'Background color for the chart
                lPieGraph.Color = Color.White ' System.Drawing.SystemColors.Control
                'Adds a border around the pie.
                lPieGraph.GraphBorder = True

                graphBox.Image = renderer.DrawChart(lPieGraph)
                legendBox.Image = renderer.DrawKey(lPieGraph)

        End Select


    End Sub


    Private Function fGetWDHMS(ByVal sec As Double) As String

        Dim wdhms() As Double = fGetDurationInWeeksDHMS(sec)
        Dim secLeft As Double = sec - wdhms(0) * 7 * 24 * 3600
        If secLeft < 0 Then secLeft = sec

        Return String.Format("{0} Weeks {1}", wdhms(0), fGetDHMS(secLeft))

    End Function


    Private Function fGetDHMS(ByVal sec As Double) As String

        Dim dhms() As Double = fGetDurationsInDaysHMS(sec)
        Dim secLeft As Double = sec - dhms(0) * 3600 * 24
        If secLeft < 0 Then secLeft = sec

        Return String.Format("{0} Days {1}", dhms(0), fGetHMStoString(secLeft))

    End Function


    Public Function fGetTotalPlayedTime() As String

        Return fGetWDHMS(mTotalPlayTime) '& " which is " & mTotalPlayTime.ToString

    End Function

    Public Function fGetLongestTrack() As cXmlTrack
        Return mLongestTrack
    End Function

    Public Function fGetMaxTrackDuration() As String

        Return fGetHMStoString(mMaxTrackDuration)

    End Function

    Public Function fGetAverageTrackDuration() As String

        Return fGetHMStoString(mAvgTrackDuration)

    End Function

    Private Sub sLoadTrackToAlbumsTable(ByVal track As cXmlTrack)

        Dim myKey As String = mGetDiscName(track)

        ssLoadStatsToTable(mAlbumsTable, mAlbumsKeys, myKey, track)

    End Sub

    Private Sub sLoadTrackToAlbumArtistsTable(ByVal track As cXmlTrack)

        Dim myKey As String = "Unknown Artist"

        If Not track.AlbumArtist = "" Then
            myKey = track.AlbumArtist
        ElseIf Not track.Artist = "" Then
            myKey = track.Artist
        End If

        ssLoadStatsToTable(mAlbumArtistsTable, mAlbumArtistsKeys, myKey, track)

    End Sub

    Private Sub sLoadTrackToArtistsTable(ByVal track As cXmlTrack)

        Dim myKey As String = "Unknown Artist"

        If Not track.Artist = "" Then
            myKey = track.Artist
        ElseIf Not track.AlbumArtist = "" Then
            myKey = track.AlbumArtist
        End If

        ssLoadStatsToTable(mArtistsTable, mArtistsKeys, myKey, track)

    End Sub

    Private Sub sLoadTrackToTracksTable(ByVal track As cXmlTrack)

        ' this sub gets partially tags so we have to take care of it
        Dim myKey As String = "Unknown Artist" + " - " + track.Name

        If Not track.Artist = "" Then
            myKey = track.Artist + " - " + track.Name
        ElseIf Not track.AlbumArtist = "" Then
            myKey = track.AlbumArtist + " - " + track.Name
        End If

        ssLoadStatsToTable(mTracksTable, mTracksKeys, myKey, track)

    End Sub

    Public ReadOnly Property TracksCount() As Integer
        Get
            Return mTracksTable.Count
        End Get
    End Property

    Public ReadOnly Property AlbumsCount() As Integer
        Get
            Return mAlbumsTable.Count
        End Get
    End Property

    Public ReadOnly Property AlbumArtistsCount() As Integer
        Get
            Return mAlbumArtistsTable.Count
        End Get
    End Property

    Public ReadOnly Property GenreCount() As Integer
        Get
            Return mGenreTable.Count
        End Get
    End Property

    Public ReadOnly Property ArtistsCount() As Integer
        Get
            Return mArtistsTable.Count
        End Get
    End Property

    Private Sub sLoadTrackToGenreTable(ByVal track As cXmlTrack)

        ' key = genre
        ' value = stats of tracks of that genre

        Dim myKey As String = "Unknown Genre"

        If Not track.Genre Is Nothing Then
            myKey = track.Genre
        End If

        ssLoadStatsToTable(mGenreTable, mGenreKeys, myKey, track)

    End Sub

    Private Sub ssLoadStatsToTable(ByVal myTable As Hashtable, ByVal myKeys As List(Of String), ByVal lKey As String, ByVal track As cXmlTrack)

        If Not myKeys.Contains(lKey) Then
            myKeys.Add(lKey)
        End If

        If Not myTable.Contains(lKey) Then

            Dim lStat As New cStatValues
            lStat.Count = 1
            lStat.Score = (track.PlayedCount * track.Duration)

            If fRatingParametersReady() Then
                'lStat.Rating = CInt(mLibraryTasks.fGetRatingScore(track, mRatingWeights, mRatingParams))
                lStat.Rating = CInt(mRatingAdjuster.fGetRating(track))
            End If

            myTable.Add(lKey, lStat)

        Else

            CType(myTable.Item(lKey), cStatValues).Count += 1
            CType(myTable.Item(lKey), cStatValues).Score += (track.PlayedCount * track.Duration)
            If fRatingParametersReady() Then
                CType(myTable.Item(lKey), cStatValues).Rating = CInt(CType(myTable.Item(lKey), cStatValues).Rating + mRatingAdjuster.fGetRating(track))
            End If

        End If


    End Sub

End Class
