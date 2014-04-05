Imports System.Text

Public Module mWarnings

    Private mDisableWarning As String = "To disable this warning go to Options > Advanced > Warnings"

    Public Function mfWarnLowResArtworkProceed(ByVal check As Boolean) As Boolean

        Dim proc As Boolean = True

        If My.Settings.WarnRemoveLowResArtworkChecked Then

            Dim sb As New StringBuilder
            sb.AppendLine("Remove Low Resolution Artwork checkbox is checked.")
            sb.AppendLine("Do you want to proceed?")
            sb.AppendLine()
            sb.AppendLine(mDisableWarning)
            If check = True Then
                If MessageBox.Show(sb.ToString, Application.ProductName, _
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No Then
                    proc = False
                End If
            End If

        End If

        Return proc

    End Function

    Public Function mfWarnNoTrackNumProceed(ByVal check As Boolean) As Boolean

        Dim proc As Boolean = True

        If My.Settings.WarnNoTrackNumber Then

            Dim sb As New StringBuilder
            sb.AppendLine("The last added track does not have Track Number.")
            sb.AppendLine("Use Mp3tag to extract Track Number from the track file name.")
            sb.AppendLine("Do you wish to continue?")

            If check = True Then
                If MessageBox.Show(sb.ToString, Application.ProductName, MessageBoxButtons.YesNo, _
                                   MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) = DialogResult.No Then
                    proc = False
                End If
            End If

        End If

        Return proc

    End Function

End Module
