Imports iTunesLib

Public Class frmStatistics

    Private mStats As cStatsMaker

    Public Sub New(ByVal lStats As cStatsMaker)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        mStats = lStats

        Me.tpGenres.Text = String.Format("Top {0} Genre ({1})", lStats.TOP_FIVE, lStats.GenreCount)
        Me.tpAlbumArtists.Text = String.Format("Top {0} Album Artists ({1})", lStats.TOP_FIVE, lStats.AlbumArtistsCount)
        Me.tpArtists.Text = String.Format("Top {0} Artists ({1})", lStats.TOP_FIVE, lStats.ArtistsCount)
        Me.tpAlbums.Text = String.Format("Top {0} Albums ({1})", lStats.TOP_FIVE, lStats.AlbumsCount)
        Me.tpTracks.Text = String.Format("Top {0} Tracks ({1})", lStats.TOP_FIVE, lStats.TracksCount)

    End Sub

    Private Sub frmStatistics_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        cboViewBy.SelectedIndex = 2
        If mStats.TOP_FIVE > 5 Then
            cboChartType.SelectedIndex = ChartType.BAR
        Else
            cboChartType.SelectedIndex = ChartType.PIE
        End If

        Me.Icon = My.Forms.frmMain.Icon
        Me.Text = String.Format("{0} Statistics ({1}) as of {2}", Application.ProductName, mStats.Name, mStats.DateStats.ToString("yyyy-MM-dd"))

    End Sub

    Private Sub sDrawGraphs()

        mStats.sDrawPieGraphGenre(CType(cboViewBy.SelectedIndex, iTSfv.GraphViewType), pbGenrePie, pbGenrePieLegend)
        mStats.sDrawPieGraphAlbumArtists(CType(cboViewBy.SelectedIndex, iTSfv.GraphViewType), pbAlbumArtistsPie, pbAlbumArtistsLegend)
        mStats.sDrawPieGraphArtists(CType(cboViewBy.SelectedIndex, iTSfv.GraphViewType), pbArtistsPie, pbArtistsLegend)
        mStats.sDrawPieGraphAlbums(CType(cboViewBy.SelectedIndex, iTSfv.GraphViewType), pbAlbumsPie, pbAlbumsLegend)
        mStats.sDrawPieGraphTracks(CType(cboViewBy.SelectedIndex, iTSfv.GraphViewType), pbTracksPie, pbTracksLegend)

        Dim sb As New System.Text.StringBuilder

        sb.Append("I have played a lot of songs by " & mStats.fGetTopArtists(0))
        If mStats.fGetTopArtists.Count > 1 Then
            sb.Append(" followed by " & mStats.fGetTopArtists(1))
            If mStats.fGetTopArtists.Count > 2 Then
                sb.Append(" and " & mStats.fGetTopArtists(2) & ".")
            End If
        End If

        sb.Append(Environment.NewLine)

        sb.Append("I have been listening to a lot of " & mStats.fGetTopGenres(0))
        If mStats.fGetTopGenres.Count > 1 Then
            sb.Append(" followed by " & mStats.fGetTopGenres(1))
            If mStats.fGetTopGenres.Count > 2 Then
                sb.AppendLine(" and " & mStats.fGetTopGenres(2) & ".")
            End If
        End If

        Select Case CType(cboViewBy.SelectedIndex, iTSfv.GraphViewType)
            Case GraphViewType.STORAGE
                sb.Replace(" played", "")
                sb.Replace(" been listening to", "")
            Case GraphViewType.RATING
                sb.Replace("played", "recently played")
                sb.Replace("been listening", "recently been listening")
        End Select

        sb.Append(Environment.NewLine)

        sb.AppendLine("Average Length of a track in my library is " & mStats.fGetAverageTrackDuration)
        sb.Append("The longest track in my library is " & mStats.fGetMaxTrackDuration)
        Dim track As cXmlTrack = mStats.fGetLongestTrack
        If track IsNot Nothing Then
            sb.AppendLine(" by " & track.Artist & " in " & track.Album & " which is " & track.Name)
        End If
        sb.Append(Environment.NewLine)
        sb.AppendLine("Time spent listening to music is " & mStats.fGetTotalPlayedTime)

        txtSummary.Text = sb.ToString

    End Sub

    Private Sub cboViewBy_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboViewBy.SelectedIndexChanged

        sDrawGraphs()

    End Sub


    Private Sub cboChartType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboChartType.SelectedIndexChanged

        mChartType = CType(cboChartType.SelectedIndex, ChartType)

        sDrawGraphs()

    End Sub

End Class

