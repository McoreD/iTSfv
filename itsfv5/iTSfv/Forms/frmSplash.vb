Public NotInheritable Class frmSplash

    'TODO: This form can easily be set as the splash screen for the application by going to the "Application" tab
    '  of the Project Designer ("Properties" under the "Project" menu).


    Private Sub frmSplash_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Set up the dialog text at runtime according to the application's assembly information.  

        'TODO: Customize the application's assembly information in the "Application" pane of the project 
        '  properties dialog (under the "Project" menu).

        'Application title
        If My.Application.Info.Title <> "" Then
            ApplicationTitle.Text = My.Application.Info.Title
        Else
            'If the application title is missing, use the application name, without the extension
            ApplicationTitle.Text = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        End If

        If My.Settings.UpgradeSettings Then
            Try
                My.Settings.Upgrade()
                msAppendDebug("Application configuration was upgraded.")
                My.Settings.UpgradeSettings = False
            Catch ex As Exception
                msAppendDebug(ex.Message + " while upgrading user.config")
            End Try
        End If

        'Format the version information using the text set into the Version control at design time as the
        '  formatting string.  This allows for effective localization if desired.
        '  Build and revision information could be included by using the following code and changing the 
        '  Version control's designtime text to "Version {0}.{1:00}.{2}.{3}" or something similar.  See
        '  String.Format() in Help for more information.
        '
        '    Version.Text = System.String.Format(Version.Text, My.Application.Info.Version.Major, My.Application.Info.Version.Minor, My.Application.Info.Version.Build, My.Application.Info.Version.Revision)

        Version.Text = System.String.Format(Version.Text, Application.ProductVersion)

        'Copyright info
        Copyright.Text = My.Application.Info.Copyright

        If My.Settings.MainWindowHeightDefault = 0 Or My.Settings.MainWindowWidthDefault = 0 Then
            My.Settings.MainWindowWidthDefault = My.Forms.frmMain.Size.Width
            My.Settings.MainWindowHeightDefault = My.Forms.frmMain.Size.Height
        End If

        If My.Forms.frmLyricsViewer.Size.Width = 0 Or My.Forms.frmLyricsViewer.Size.Height = 0 Then
            My.Forms.frmLyricsViewer.Size = New Size(600, 600)
        End If
        If My.Settings.LyricsViewerSize.Width = 0 Or My.Settings.LyricsViewerSize.Height = 0 Then
            My.Settings.LyricsViewerSize = My.Forms.frmLyricsViewer.Size
        Else
            My.Forms.frmLyricsViewer.Size = My.Settings.LyricsViewerSize
        End If

        If My.Settings.LyricsViewerLocation.X < 0 Or My.Settings.LyricsViewerLocation.Y < 0 Then
            My.Settings.LyricsViewerLocation = My.Forms.frmLyricsViewer.Location
        Else
            My.Forms.frmLyricsViewer.Location = My.Settings.LyricsViewerLocation
        End If

        If My.Settings.MainWindowHeight = 0 Or My.Settings.MainWindowWidth = 0 Then
            My.Settings.MainWindowWidth = My.Forms.frmMain.Size.Width
            My.Settings.MainWindowHeight = My.Forms.frmMain.Size.Height
        Else
            My.Forms.frmMain.Size = New Size(My.Settings.MainWindowWidth, My.Settings.MainWindowHeight)
        End If

        bwSplash.RunWorkerAsync()

    End Sub


    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bwSplash.DoWork

        ''*************************
        ''* Configure Setting Files
        ''**************************
        Call msConfigureDirsFiles()

        ''*************************
        ''* Configure App Mode
        ''**************************
        Dim booiTunes As Boolean = mfGetItunes()
        Dim booWMP As Boolean = mfGetMediaCenter()

        If booiTunes = False And booWMP = True Then
            '' NO iTunes and YES Windows Vista Media Center
            If mpAppMode = eAppMode.AFV Then
                msAppModeSetWEafv() ' File Validator - if user requests
            Else
                msAppModeSetWMPfv() ' Windows Media Player - by default
            End If
        ElseIf booiTunes = False And booWMP = False Then
            '' NO iTunes and NO Windows Vista Media Center = File Validator
            msAppModeSetWEafv()     ' File Validator - by default
        End If

    End Sub

    Private Sub bwSplash_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles bwSplash.ProgressChanged



    End Sub

    Private Sub bwSplash_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bwSplash.RunWorkerCompleted


        '' If File Validator is requested then show File Validator 
        '' Else 
        '' If iTunes is present and requested iTSfv then show iTSfv 
        '' If Vista Media Center is present or WMPfv is requested then show WMPfv 
        '' In all other circumstances show File Validator

        If mpAppMode = eAppMode.AFV Then
            My.Forms.frmValidator.Show()
        Else
            If mpAppMode = eAppMode.ITSFV Then
                'MsgBox("Showed iTSfv")
                My.Forms.frmMain.Show()
            ElseIf mpAppMode = eAppMode.WMPFV Then
                'MsgBox("Showed WMPfv")
                My.Forms.frmWMPfv.Show()
            Else
                My.Forms.frmValidator.Show()
            End If
        End If

        ''*************************
        ''* Close Splash Screen
        ''**************************
        If Application.OpenForms.Count > 1 Then
            Me.Close()
        Else
            ' SOMETHING IS BLOODY BUGGY WITH .NET 
            ' WHEN IT FORM IS LOADED TO TRAY IT THINKS THE NUMBER OF OPEN FORMS IS 1    
            Me.Hide()
        End If


    End Sub
End Class
