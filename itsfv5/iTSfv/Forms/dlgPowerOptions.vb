Imports System.Windows.Forms

Public Class dlgPowerOptions

    Private mPowerOption As Integer = 0

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        sPowerMgmt()
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private mCountDown As Integer = CInt(My.Settings.PowerDelaySeconds)

    Private Sub tmrPower_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrPower.Tick

        Dim op As String = "Do Nothing to"
        Select Case mPowerOption
            Case 1
                op = "Suspending"
            Case 2
                op = "Hibernating"
            Case 3
                op = "Shutting down"
        End Select

        Dim warn As String = String.Format("{0} the computer in {1} seconds...", op, mCountDown)
        lblWarning.Text = warn
        mCountDown = mCountDown - 1

        If mCountDown = 0 Then
            sPowerMgmt()        
        End If

    End Sub

    Private Sub sPowerMgmt()

        If mPowerOption > 0 Then

            Dim wc As New McoreSystem.WindowsController.WindowsController
            Dim ro As McoreSystem.WindowsController.RestartOptions = mpRestartOptions
            If ro <> Nothing Then
                McoreSystem.WindowsController.WindowsController.ExitWindows(ro, force:=True)
                msCloseForms()
            Else
                Me.Close()
            End If

        Else

            Me.Close()

        End If

    End Sub

    Private Sub dlgPowerOptions_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        mPowerOption = My.Settings.PowerOption
        cboPowerOption.SelectedIndex = mPowerOption

        Me.Text = Application.ProductName + " Power Management"
        Me.Icon = My.Forms.frmMain.Icon

    End Sub

    Private Sub cboPowerOption_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboPowerOption.SelectedIndexChanged
        mPowerOption = cboPowerOption.SelectedIndex
    End Sub

End Class
