Imports iTunesLib

<Serializable()> Public Class cRatingAdjuster

    Private myWeights As RatingWeights
    Private myRatingParams As RatingParams

    Public Sub New(ByVal lWeights As RatingWeights, ByVal lRatingParams As RatingParams)

        Me.myWeights = lWeights
        Me.myRatingParams = lRatingParams

    End Sub

    Public Function fGetRating(ByVal track As cXmlTrack) As Double

        Return fGetRating_McoreD(track)

    End Function

    Private Function fGetRating_McoreD(ByVal track As cXmlTrack) As Double

        Dim myRating As Integer = 0

        If track.PlayedCount > 0 Then

            Dim ratingScore As Double = 0.0

            Dim factoredPlayedCount As Double = track.Duration / myRatingParams.AvgTrackDuration * track.PlayedCount

            If mRatingWeights.ReduceScaleLongTracks Then
                If track.Duration > mRatingWeights.LongSongDuration AndAlso myWeights.ScaleDuration > 0 Then
                    ' this is a long song so scaling applies
                    factoredPlayedCount = (myWeights.ScaleDuration / 100.0) * factoredPlayedCount
                End If
            End If

            Dim playedCount As Double = If(My.Settings.RatingsTrackDurationUse, factoredPlayedCount, track.PlayedCount)
            Dim skippedCount As Integer = track.SkippedCount
            Dim daysSinceLastPlayed As Double = Now.Subtract(track.PlayedDate).TotalDays
            Dim daysSinceAdded As Double = Now.Subtract(track.DateAdded).TotalDays

            Dim scorePlayedCount As Double = playedCount / Math.Max(myRatingParams.MaxPlayedCount, 1)
            Dim scoreSkippedCount As Double = playedCount / (playedCount + skippedCount)
            Dim scoreLastPlayed As Double = Math.Max((playedCount - daysSinceLastPlayed) / playedCount, 0)
            Dim scoreAdded As Double = Math.Max((daysSinceAdded - daysSinceLastPlayed) / daysSinceAdded, 0)

            ratingScore = myWeights.PlayedCount * scorePlayedCount + _
                                        myWeights.SkippedCount * scoreSkippedCount + _
                                        myWeights.LastPlayed * scoreLastPlayed + _
                                        myWeights.DateAdded * scoreAdded

            If ratingScore > 0.0 Then
                myRating = CType(Math.Round(ratingScore, 0), Integer)
            End If

            myRating = CInt((track.Rating * My.Settings.PrevRatingWeight / 100.0 + myRating * (100.0 - My.Settings.PrevRatingWeight) / 100.0))

        End If

        Return myRating

    End Function

    Private Function fGetRating_BigBernie(ByVal track As cXmlTrack) As Double

        Dim daysSinceLastPlayed As Double = Now.Subtract(track.PlayedDate).TotalDays
        Dim daysSinceAdded As Double = Now.Subtract(track.DateAdded).TotalDays

        Dim r As Double = 500000000000 + 10000000000 * (Math.Log10(track.PlayedCount + 1.01) / 1.001 ^ (daysSinceLastPlayed) - (Math.Log10(track.SkippedCount + 1.01)) ^ 1.7) / daysSinceAdded ^ 0.125

        Return r

    End Function

End Class
