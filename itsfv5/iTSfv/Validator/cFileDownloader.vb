Imports System.Net
Imports System.Text
Imports System.IO
Imports System.ComponentModel

Public Class cFileDownloader

    Private mBw As BackgroundWorker = Nothing
    Private WithEvents mWebClient As WebClient = mAdapter.mWebClient

    Public Enum JobType
        DOWNLOAD_ARTWORK_GRABBER
    End Enum

    Friend Function fDownloadArtwork() As Boolean

        Dim succ As Boolean = False

        If File.Exists(mArtworkGrabberFilePathMp3) = False Then

            Try
                mWebClient.DownloadFileAsync(New System.Uri(mArtworkGrabberUrlPathMp3), mArtworkGrabberFilePathMp3)
            Catch ex As Exception
                msAppendWarnings(ex.Message + " while downloading " + mArtworkGrabberUrlPathMp3)
                Dim sb As New StringBuilder
                sb.AppendLine("Please copy an mp3 file called " + Path.GetFileName(mArtworkGrabberFilePathMp3))
                sb.AppendLine("to the same folder where iTSfv.exe is and retry.")
                MessageBox.Show(sb.ToString, "Error downloading " + Path.GetFileName(mArtworkGrabberFilePathMp3), MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End Try

            succ = True

        End If

        Return succ

    End Function

    Private Sub mWebClient_DownloadFileCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs) Handles mWebClient.DownloadFileCompleted

        If File.Exists(mArtworkGrabberFilePathMp3) Then
            If mBw.IsBusy = False Then
                Dim t As New cBwJob(cBwJob.JobType.EXPORT_ARTWORK_MANUAL)
                mBw.RunWorkerAsync(t)
            End If
        End If

    End Sub

    Private Sub mWebClient_DownloadProgressChanged(ByVal sender As Object, ByVal e As System.Net.DownloadProgressChangedEventArgs) Handles mWebClient.DownloadProgressChanged

        mfUpdateStatusBarText(String.Format("Downloading {0}...", Path.GetFileName(mArtworkGrabberFilePathMp3)), True)
        mProgressTracksMax = CInt(e.TotalBytesToReceive)
        mProgressTracksCurrent = CInt(e.BytesReceived)

    End Sub

    Public Sub New(ByVal lBwApp As BackgroundWorker)
        Me.mBw = lBwApp
    End Sub

End Class
