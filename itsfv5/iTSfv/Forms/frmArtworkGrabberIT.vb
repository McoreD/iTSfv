Imports iTunesLib
Imports System.Text
Imports System.IO

Public Class frmArtworkGrabberIT

    Private mMP3filePath As String = Path.Combine(Application.StartupPath, "itunes-artwork-grabber.mp3")

    Private Sub frmArtworkGrabberIT_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.Text = Application.ProductName + " - iTunes Store Artwork Grabber"
        Me.Icon = My.Forms.frmMain.Icon

        Dim sbNote As New StringBuilder
        sbNote.AppendLine("Make sure in iTunes > Options > General you have")
        sbNote.AppendLine("Automatically Download missing album artwork checked")
        sbNote.AppendLine()
        sbNote.AppendLine("You can create an iTunes Account without a Credit Card using")

        lblNote.Text = sbNote.ToString

    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click

        msAppendDebug("Job Started: EXPORT_ARTWORK_MANUAL using Artwork Grabber GUI")
        btnSearch.Enabled = False

        bwApp.RunWorkerAsync()

    End Sub

    Private Sub bwApp_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bwApp.DoWork

        Dim ag As New cArtworkGrabberIT(txtArtist.Text, txtAlbum.Text)
        Dim song As IITFileOrCDTrack = ag.fDownloadArtwork()
        e.Result = song

    End Sub

    Private Sub bwApp_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bwApp.RunWorkerCompleted

        If e.Result IsNot Nothing Then

            Dim song As IITFileOrCDTrack = CType(e.Result, IITFileOrCDTrack)
            If song IsNot Nothing Then
                mfExportArtworkIT(song, dirPath:=String.Empty)
            Else
                mfUpdateStatusBarText("Could not find Artwork from iTunes Store...", True)
            End If

        End If

        msAppendDebug("Job Finished: EXPORT_ARTWORK_MANUAL using Artwork Grabber GUI")
        msWriteDebugLog()

        btnSearch.Enabled = True

    End Sub

    Private Sub tmrProgress_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrProgress.Tick

        If String.Empty <> mpProgressTracksMsg Then

            If bwApp.IsBusy = True Then
                tsStatus.Text = mpProgressTracksMsg
            Else
                tsStatus.Text = "Ready. " + mpProgressTracksMsg
            End If

        End If

    End Sub

    Private Sub llbllFreeAlbum_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles llbllFreeAlbum.LinkClicked
        Process.Start(llbllFreeAlbum.Text)
    End Sub
End Class