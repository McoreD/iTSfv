Imports iTunesLib

Public Class cLibraryTasks

    Public Sub New()
        ' Blank Constructor
    End Sub


    Public Function sEditEQ(ByVal track As iTunesLib.IITFileOrCDTrack) As String

        Dim strGenre As String = track.Genre

        If strGenre = "Bass" Then

            track.EQ = "Bass Booster"

        Else

            Dim strEQ() As String = {"R&B", "Rock", "Dance", _
                                          "Hip-Hop", "Classical", _
                                          "Deep", "Electronic", _
                                          "Jazz", _
                                          "Latin", "Lounge", _
                                          "Pop", "Acoustic"}

            For Each myEQ As String In strEQ
                If track.Genre = myEQ Then
                    track.EQ = myEQ
                    Return myEQ
                End If
            Next

        End If


        Return Nothing

    End Function

    Private Function fGetRatingScore(ByVal track As cXmlTrack, ByVal myWeights As RatingWeights, ByVal myRatingParams As RatingParams) As Double

        '' moved to cRatingsAdjuster.vb
        '' Delete this function after next CVS commit

    End Function


    Public Function fGetAlbumName(ByVal track As TagLib.File) As String

        Dim strArtist As String = String.Empty
        Dim strAlbum As String = fGetAlbum(track).Trim

        If track.Tag.FirstAlbumArtist = VARIOUS_ARTISTS Then

            strArtist = VARIOUS_ARTISTS

        Else

            If track.Tag.FirstAlbumArtist <> "" Then
                strArtist = track.Tag.FirstAlbumArtist.Trim
            ElseIf track.Tag.FirstAlbumArtist <> VARIOUS_ARTISTS AndAlso track.Tag.FirstPerformer <> "" Then
                strArtist = track.Tag.FirstPerformer
            Else
                strArtist = VARIOUS_ARTISTS
            End If

        End If

        Return strArtist + " - " + strAlbum

    End Function

    Public Function fGetAlbumName(ByVal track As IITFileOrCDTrack) As String

        Dim strArtist As String = String.Empty
        Dim strAlbum As String = fGetAlbum(track).Trim

        If track.Compilation = True Then

            strArtist = VARIOUS_ARTISTS

        Else

            If track.Kind = ITTrackKind.ITTrackKindFile AndAlso CType(track, IITFileOrCDTrack).AlbumArtist <> "" Then
                strArtist = CType(track, IITFileOrCDTrack).AlbumArtist.Trim
            ElseIf track.Compilation = False AndAlso track.Artist <> "" Then
                strArtist = track.Artist.Trim
            Else
                strArtist = VARIOUS_ARTISTS
            End If

        End If

        ''Console.Writeline(strArtist + " - " + strAlbum)

        Return strArtist + " - " + strAlbum

    End Function

    Public Function fGetDiscName(ByVal track As IITTrack) As String

        '* Changed from IITFileOrCDTrack to IITTrack 
        ' - why? for backing up ratings 

        Dim strDiscName As String = String.Empty

        If track.Kind = ITTrackKind.ITTrackKindFile Then
            Dim temp As New cXmlTrack(CType(track, IITFileOrCDTrack), False)
            strDiscName = mLibrary.mGetDiscName(temp)
        Else
            strDiscName = track.Album            
        End If

        Return strDiscName

    End Function

End Class

