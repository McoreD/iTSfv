Public Class cXmlDiscArtistFinder

    Private mDiscArtists As New Dictionary(Of String, Integer)
    Private mDiscArtist As String = VARIOUS_ARTISTS
    Private mDisc As cXmlDisc = Nothing
    Private mConfidence As Double = 0.0

    Public ReadOnly Property AlbumArtist() As String

        Get
            Return mDiscArtist
        End Get

    End Property

    Public Sub New(ByVal lDisc As cXmlDisc)

        Me.mDisc = lDisc

        If My.Settings.MostCommonAlbumArtist Then

            For i As Integer = 0 To lDisc.Tracks.Count - 1

                Dim oAlbumArtist As String = VARIOUS_ARTISTS

                If String.Empty <> lDisc.Tracks(i).Artist Then
                    oAlbumArtist = lDisc.Tracks(i).Artist
                End If

                sAddArtist(oAlbumArtist)

            Next

            mDiscArtist = fGetTopArtist()

        Else

            Dim bArtistIsSame As Boolean = True
            Dim oAlbumArtist As String = lDisc.FirstTrack.Artist

            For i As Integer = 0 To lDisc.Tracks.Count - 2

                Dim artist1 As String = lDisc.Tracks(i).Artist
                Dim artist2 As String = lDisc.Tracks(i + 1).Artist
                If String.Empty <> artist1 AndAlso String.Empty <> artist2 Then
                    bArtistIsSame = bArtistIsSame And artist1.Equals(artist2)
                End If

            Next

            If bArtistIsSame = True Then
                mDiscArtist = oAlbumArtist ' this will not get assigned if strAlbumArtist is empty
            Else
                mDiscArtist = VARIOUS_ARTISTS                
            End If

        End If

        msAppendDebug(String.Format("Chosen Most Common Artist: ""{0}"" with {1}% confidence", mDiscArtist, mConfidence.ToString("0.00")))
        'msUpdateStatusBarText(String.Format("AlbumArtist: ""{0}""", mDiscArtist), append:=False)

    End Sub

    Private Function fGetTopArtist() As String

        Dim topHit As Integer = 0
        Dim topArtist As String = VARIOUS_ARTISTS

        If mDisc.Tracks.Count > 0 And mDiscArtists.Count > 0 Then

            Dim et As IEnumerator = mDiscArtists.GetEnumerator

            Dim de As System.Collections.Generic.KeyValuePair(Of String, Integer)

            While et.MoveNext
                de = CType(et.Current, KeyValuePair(Of String, Integer))
                If String.IsNullOrEmpty(de.Key) = False AndAlso CInt(de.Value) > topHit Then
                    topHit = CInt(de.Value)
                    topArtist = CStr(de.Key)
                End If
            End While

            mConfidence = 100 * mDiscArtists(topArtist) / mDisc.Tracks.Count

            If My.Settings.MostCommonArtistRatioActive = True Then
                ' work out if top Artist has lost the election                 
                If mConfidence < My.Settings.MostCommonArtistPerc Then
                    topArtist = VARIOUS_ARTISTS
                End If

            End If

        End If


        Return topArtist

    End Function

    Private Sub sAddArtist(ByVal artist As String)

        If mDiscArtists.ContainsKey(artist) Then
            mDiscArtists.Item(artist) += 1
        Else
            mDiscArtists.Add(artist, 1)
        End If

    End Sub



End Class
