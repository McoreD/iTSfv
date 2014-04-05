Public Module mFormControls

    Public Sub msShowOptions()

        My.Forms.frmOptions.Show()
        My.Forms.frmOptions.Focus()

    End Sub

    Public Sub msShowTrackReplaceAssistant()

        My.Forms.frmTrackReplace.Show()
        My.Forms.frmTrackReplace.Focus()

    End Sub

    Public Sub msShowVersionHistory()
        My.Forms.frmVer.Show()
        My.Forms.frmVer.Focus()
    End Sub

    Public Sub msShowWMPfv()
        My.Forms.frmWMPfv.Show()
        My.Forms.frmWMPfv.Focus()
    End Sub

    Public Sub msShowDebug(ByVal oToolStripStatusLabel As ToolStripStatusLabel, _
                           ByVal oToolTip As ToolTip, _
                           ByVal oStatusStrip As StatusStrip)

        If mSbDebug.Length > 0 Then            
            My.Forms.frmDebug.Show()
            My.Forms.frmDebug.Focus()
        Else
            mfOpenFileOrDirPath(mFilePathDebugLog, oToolStripStatusLabel, oToolTip, oStatusStrip)
        End If

    End Sub

    Public Sub msCloseForms()

        Try
            Dim lForms As New List(Of Form)
            For Each frm As Form In Application.OpenForms
                lForms.Add(frm)
            Next
            For Each frm As Form In lForms
                frm.Close()
            Next
        Catch ex As Exception
            ' oh well
        End Try

        My.Forms.frmSplash.Close()
        My.Forms.frmTrackReplace.Close()
        My.Forms.frmValidator.Close()
        My.Forms.frmVer.Close()
        My.Forms.frmOptions.Close()
        My.Forms.frmMain.Close()
        My.Forms.frmAbout.Close()

        If mItunesApp IsNot Nothing Then
            System.Runtime.InteropServices.Marshal.ReleaseComObject(mItunesApp)
            GC.Collect()
            mItunesApp = Nothing
        End If
        
    End Sub

    Public Sub msShowAbout()

        My.Forms.frmAbout.ShowDialog()

    End Sub

End Module
