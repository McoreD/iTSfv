Imports System.ComponentModel

Public Class cUpdateChecker

    Private sBarTrack As ToolStripStatusLabel
    Private WithEvents bwUpdate As New BackgroundWorker

    Private mcUpdateDownloadDir As String() = New String() {"http://itsfv.googlecode.com/files/", _
                                                            "http://superb-west.dl.sourceforge.net/sourceforge/itsfv/", _
                                                            "http://optusnet.dl.sourceforge.net/sourceforge/itsfv/"}

    Private mcUpdateCheckUrl As String() = New String() {"http://wmwiki.com/mcored/updates.txt", _
                                                       "http://itsfv.sourceforge.net/updates.txt"}
    Private mIsManualCheckUpdate As Boolean = False
    Private mAppIcon As Icon = Nothing
    Private mAppImage As Image = Nothing

    Public Sub New(ByVal appIcon As Icon, ByVal appImage As Image, _
                   ByVal sbar As ToolStripStatusLabel, ByVal manual As Boolean)

        Me.sBarTrack = sbar
        mIsManualCheckUpdate = manual

        mAppIcon = appIcon
        mAppImage = appImage

        bwUpdate.WorkerReportsProgress = True
        bwUpdate.WorkerSupportsCancellation = True

    End Sub

    Public Sub CheckUpdates()
        bwUpdate.RunWorkerAsync()
    End Sub

    Private Sub bwApp_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bwUpdate.DoWork

        bwUpdate.ReportProgress(0)

        Dim appInfo As New McoreSystem.AppInfo(Application.ProductName, _
                    Application.ProductVersion)    
        appInfo.AppIcon = mAppIcon
        appInfo.AppImage = mAppImage

        If My.Settings.AutoCheckUpdates AndAlso appInfo.isUpdated(mcUpdateCheckUrl) Then

            If appInfo.isUpdated(mcUpdateCheckUrl) Then
                appInfo.CheckUpdates(mcUpdateCheckUrl, mcUpdateDownloadDir, APP_ABBR_NAME_IT, McoreSystem.AppInfo.OutdatedMsgStyle.NewVersionOfAppAvailable)
            End If

        ElseIf mIsManualCheckUpdate Then

            appInfo.CheckUpdates(mcUpdateCheckUrl, mcUpdateDownloadDir, APP_ABBR_NAME_IT, McoreSystem.AppInfo.OutdatedMsgStyle.NewVersionOfAppAvailable)

        End If


    End Sub

    Private Sub bwApp_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles bwUpdate.ProgressChanged

        sBarTrack.Text = mfUpdateStatusBarText("Checking for updates...", True)

    End Sub

    Private Sub bwApp_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bwUpdate.RunWorkerCompleted

        sBarTrack.Text = mfUpdateStatusBarText("Done with Checking Updates...", True)

    End Sub
End Class
