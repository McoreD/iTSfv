Imports System.ComponentModel
Imports iTSfv.cBwJob
Imports WMPLib

Public Module mJobsWMP

    '' RESPONSIBLE FOR FUTURE JOBS FOR WMP 

    Public Sub msWMP_DeleteMissingTracks(ByVal oMainLibraryTracks As WMPLib.IWMPPlaylist, _
                                       ByVal oBwApp As BackgroundWorker, _
                                       ByVal bResume As Boolean, _
                                       ByVal intLastTrackID As Integer, _
                                       ByVal bDeleteTracksNotInHDD As Boolean, _
                                       ByVal bDeleteNonMusicFolderTracks As Boolean)

        msAppendDebug("Looking for tracks outside of music folders to remove")

        If bResume Then
            oBwApp.ReportProgress(ProgressType.UPDATE_TRACKS_PROGRESS_BAR_MAX, oMainLibraryTracks.count - intLastTrackID + 1)
        Else
            oBwApp.ReportProgress(ProgressType.UPDATE_TRACKS_PROGRESS_BAR_MAX, oMainLibraryTracks.count)
        End If

        If bDeleteTracksNotInHDD = True OrElse bDeleteNonMusicFolderTracks = True Then

            For i As Integer = intLastTrackID To oMainLibraryTracks.count - 1

                If oBwApp.CancellationPending Then
                    oBwApp.ReportProgress(ProgressType.READY)
                    Exit Sub
                End If

                Dim track As New WMPTrack(CType(oMainLibraryTracks.Item(i), IWMPMedia3))

                If track.Kind = MediaType.AUDIO Then

                    Dim booTrackDeleted As Boolean = False

                    If bDeleteNonMusicFolderTracks = True Then
                        oBwApp.ReportProgress(ProgressType.DELETE_TRACKS_DEAD_ALIEN, track.Album)
                    Else
                        oBwApp.ReportProgress(ProgressType.DELETE_TRACKS_DEAD, track.Album)
                    End If

                    If bDeleteNonMusicFolderTracks = True Then
                        '**************************
                        ' DELETE NON-MUSIC-FOLDER
                        '**************************
                        booTrackDeleted = fDeleteNonMusicFolderTrack(track)
                    End If

                    If bDeleteTracksNotInHDD = True Then
                        '**************************
                        ' DELETE NON-EXISTANT FILES
                        '**************************
                        If booTrackDeleted = False Then
                            booTrackDeleted = booTrackDeleted And fDeleteTrackNotInHDD(track)
                        End If
                    End If

                End If

            Next

            oBwApp.ReportProgress(ProgressType.UPDATE_DISCS_PROGRESS_BAR_MAX, oMainLibraryTracks.Count)

        End If

        Dim msg As String = String.Format("Removed {0} dead and {1} foreign tracks", mListFileNotFound.Count, mListTracksNonMusicFolder.Count)
        msAppendDebug(msg)
        oBwApp.ReportProgress(ProgressType.READY, msg)

    End Sub

    Private Function fDeleteNonMusicFolderTrack(ByVal track As WMPTrack) As Boolean

        Try
            If track.Location Is Nothing OrElse mfFileIsInMusicFolder(track.Location) = False Then
                mListTracksNonMusicFolder.Add(track.Location)
                msAppendDebug("Removing foreign track: " + track.Location)
                CType(mPlayer, cPlayerWMP).DeleteMedia(track)
                Return True
            End If
        Catch ex As Exception
            msAppendWarnings(ex.Message + " while removing foreign tracks.")
        End Try


        Return False

    End Function

    Private Function fDeleteTrackNotInHDD(ByVal track As WMPTrack) As Boolean

        Try

            If Not IO.File.Exists(track.Location) Then
                Dim tr As New cXmlTrack(track, False)
                mListTracksToDelete.Add(tr)
                Dim tInfo As String = track.Artist + " - " + track.Album + " - " + track.Name
                msAppendDebug("Removed dead track:" + tInfo)
                mListFileNotFound.Add(tInfo)
                CType(mPlayer, cPlayerWMP).DeleteMedia(track)
                Return True
            End If

        Catch ex As Exception
            msAppendWarnings(ex.Message + " while deleting dead tracks")
        End Try

        Return False

    End Function

End Module
