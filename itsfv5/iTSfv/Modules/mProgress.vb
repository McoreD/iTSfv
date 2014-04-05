Public Module mProgress

    Friend mProgressDiscsCurrent As Integer = 0
    Friend mProgressDiscsMax As Integer = 100
    Private mProgressDiscsMsg As String = String.Empty
    Friend mProgressTracksCurrent As Integer = 0
    Friend mProgressTracksMax As Integer = 100
    Private mProgressTracksMsg As String = String.Empty

    Public ReadOnly Property mpProgressDiscsMsg() As String
        Get
            Return mProgressDiscsMsg
        End Get
    End Property

    Public ReadOnly Property mpProgressTracksMsg() As String
        Get
            Return mProgressTracksMsg
        End Get
    End Property

    Public Function mfUpdateStatusBarText(ByVal msg As String, Optional ByVal secondary As Boolean = True) As String

        If secondary = True Then

            mProgressTracksMsg = msg

        Else

            If msg <> mProgressTracksMsg Then
                mProgressDiscsMsg = msg
            End If

        End If

        If My.Forms.frmMain.lbVerbose.Items.Contains(msg) = False Then
            My.Forms.frmMain.lbVerbose.Items.Add(msg)
            My.Forms.frmMain.lbVerbose.SelectedIndex = My.Forms.frmMain.lbVerbose.Items.Count - 1
        End If

        Return msg

    End Function

End Module
