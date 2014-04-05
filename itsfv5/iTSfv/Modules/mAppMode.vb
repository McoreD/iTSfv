Public Module mAppMode

    ' THIS MODULE IS RESPONSIBLE FOR ALL THE FUNCTIONS HANDLING APPLICATION MODE

    Public ReadOnly Property mpAppName() As String
        Get
            Select Case mpAppMode
                Case eAppMode.WMPFV
                    Return mAppInfo.GetApplicationTitle(APP_ABBR_NAME_WMP, Application.ProductVersion)
                Case Else
                    Return mAppInfo.GetApplicationTitle(APP_ABBR_NAME_IT, Application.ProductVersion)
            End Select
        End Get
    End Property

    Public Sub msAppModeGet()

        If My.Settings.AppModeWEafv Then
            My.Forms.frmOptions.rbAppModeWEafv.Checked = True
        ElseIf My.Settings.AppModeITsfv Then
            My.Forms.frmOptions.rbAppModeITsfv.Checked = True
        Else
            My.Forms.frmOptions.rbAppModeWMPfv.Checked = True
        End If

    End Sub

    Public Sub msAppModeSetITsfv()
        My.Settings.AppModeWEafv = False
        My.Settings.AppModeITsfv = True
        My.Settings.AppModeWMPfv = False
    End Sub

    Public Sub msAppModeSetWMPfv()
        My.Settings.AppModeITsfv = False
        My.Settings.AppModeWEafv = False
        My.Settings.AppModeWMPfv = True
    End Sub

    Public Sub msAppModeSetWEafv()
        My.Settings.AppModeWEafv = True
        My.Settings.AppModeWMPfv = False
        My.Settings.AppModeITsfv = False
    End Sub

    Public Sub msAppModeSet()
        If My.Forms.frmOptions.rbAppModeWMPfv.Checked Then
            msAppModeSetWMPfv()
        ElseIf My.Forms.frmOptions.rbAppModeITsfv.Checked Then
            msAppModeSetITsfv()
        Else
            msAppModeSetWEafv()
        End If
    End Sub

    Public ReadOnly Property mpAppMode() As eAppMode

        Get
            If My.Settings.AppModeITsfv Then
                Return eAppMode.ITSFV
            ElseIf My.Settings.AppModeWMPfv Then
                Return eAppMode.WMPFV
            Else
                Return eAppMode.AFV
            End If

        End Get

    End Property

End Module
