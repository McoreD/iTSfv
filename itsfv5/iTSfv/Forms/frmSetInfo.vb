Imports iTunesLib

Public Class frmSetInfo

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click

        sEditTracks()
        Me.Close()

    End Sub

    Public Sub sEditTracks()

        If mItunesApp.BrowserWindow.SelectedTracks IsNot Nothing Then

            Dim tracks As IITTrackCollection = mItunesApp.BrowserWindow.SelectedTracks

            For Each track As IITTrack In tracks
                If track.Kind = ITTrackKind.ITTrackKindFile Then
                    Dim song As IITFileOrCDTrack = CType(track, IITFileOrCDTrack)
                    If chkEpID.Checked Then
                        song.EpisodeNumber = CInt(nudEpID.Value)
                    End If
                    If chkShow.Checked Then
                        Dim temp As String = mfGetStringFromPattern(txtShow.Text, New cXmlTrack(track, False))
                        song.Show = temp
                        song.EpisodeID = temp
                    End If
                End If
            Next

        End If

    End Sub

    Private Sub btnApply_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnApply.Click

        sEditTracks()

    End Sub

End Class