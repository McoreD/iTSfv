Imports iTunesLib
Imports System.Text
Imports System.ComponentModel

Public Class cDupRemover

    Private mTracks As IITTrackCollection
    Private mTrackKeys As New List(Of String)
    Private mTrackDups As New List(Of IITTrack)

    Private mBwApp As BackgroundWorker

    Public Sub New(ByVal tracks As IITTrackCollection, ByRef lBwApp As BackgroundWorker)

        mTracks = tracks
        mBwApp = lBwApp

        mTrackDups = mfGetDuplicateTracks()

    End Sub

    Public ReadOnly Property DuplicateTracks() As List(Of IITTrack)
        Get
            Return mTrackDups
        End Get
    End Property

    Public Function RemoveDuplicates() As Boolean

        Dim succ As Boolean = True

        ' Debugging
        'For i As Integer = 1 To 10
        '    mTrackDups.Add(mTracks.Item(i))
        'Next

        If mTrackDups.Count > 0 Then

            Dim bDelete As DialogResult = DialogResult.Yes

            If My.Settings.ConfirmRemoveDuplicates = True Then
                Dim frmDt As New frmDuplicateTracks(mTrackDups)
                frmDt.ShowDialog()
                bDelete = frmDt.DialogResult
            End If

            If bDelete = DialogResult.Yes Then

                For Each track As IITTrack In mTrackDups
                    Try
                        Dim msg As String = String.Format("Removing duplicate song: ""{0} - {1}""", track.Artist, track.Name)
                        msAppendDebug(msg)
                        mfUpdateStatusBarText(msg, secondary:=True)
                        track.Delete()
                    Catch ex As Exception
                        msAppendWarnings(ex.Message + " while removing duplicate song")
                        succ = False
                    End Try
                Next

            End If

        End If

        Return succ

    End Function

    Private Function fGetTrackKey(ByVal track As IITTrack) As String

        Dim sb As New StringBuilder

        If My.Settings.DupTrackNumber Then
            sb.Append(track.TrackNumber)
        End If

        If My.Settings.DupDiscNumber Then
            sb.Append(track.DiscNumber)
        End If

        If My.Settings.DupDuration Then
            sb.Append(track.Duration)
        End If

        sb.Append(track.Artist)
        sb.Append(track.Album)
        sb.Append(track.Name)

        Return sb.ToString

    End Function

    Private Sub sPausePendingCheck()

        '' 5.34.14.1 Pressing Stop button did not pause the currently active job [Jojo]

        If mJobPaused And mBwApp.CancellationPending = False Then
            Threading.Thread.Sleep(2000)
            Call sPausePendingCheck()
        End If

    End Sub

    Public Function mfGetDuplicateTracks() As List(Of IITTrack)

        Dim dupsList As New List(Of IITTrack)

        mProgressTracksMax = mTracks.Count
        mProgressTracksCurrent = 0

        mfUpdateStatusBarText("Checking for Duplicate Tracks", secondary:=False)

        For Each track As IITTrack In mTracks

            sPausePendingCheck()
            If mBwApp.CancellationPending Then
                Exit For
            End If

            Dim key As String = fGetTrackKey(track)

            If mTrackKeys.Contains(key) Then

                dupsList.Add(track)

                If track.Kind = ITTrackKind.ITTrackKindFile Then
                    mListDuplicateTracks.Add(CType(track, IITFileOrCDTrack).Location)
                End If

            Else
                mTrackKeys.Add(key)
            End If

            mProgressTracksCurrent += 1

        Next

        Return dupsList

    End Function

End Class
