Public NotInheritable Class frmAbout

    Private Sub frmAbout_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Set the title of the form.
        Dim ApplicationTitle As String
        If My.Application.Info.Title <> "" Then
            ApplicationTitle = My.Application.Info.Title
        Else
            ApplicationTitle = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        End If

        Dim msg As New System.Text.StringBuilder
        msg.AppendLine("Special thanks and credits go to:")
        msg.AppendLine()
        msg.AppendLine("Jose Hidalgo, Jojo, NearlyGod, urlwolf, Bluenote,")
        msg.AppendLine("Leif, trw, everybody who emailed bug reports,")
        msg.AppendLine("and the rest of HydrogenAudio members,")
        msg.AppendLine("")
        msg.AppendLine("Chris Daniels for the iTSfv icon,")
        msg.AppendLine("Mark James for the Silk icon set 1.3.")
        msg.AppendLine("Senthil Kumar for iLyrics,")
        msg.AppendLine()
        msg.AppendLine("Running from: " & Application.ExecutablePath)
        msg.AppendLine()
        msg.AppendLine("Settings file: " & mfConfigFilePath())

        Me.Text = String.Format("About {0}", My.Application.Info.ProductName)
        ' Initialize all of the text displayed on the About Box.
        ' TODO: Customize the application's assembly information in the "Application" pane of the project 
        '    properties dialog (under the "Project" menu).
        Me.LabelProductName.Text = ApplicationTitle
        Me.LabelVersion.Text = String.Format("Version {0}", Application.ProductVersion)
        Me.LabelCopyright.Text = My.Application.Info.Copyright
        Me.LabelCompanyName.Text = My.Application.Info.CompanyName
        Me.TextBoxDescription.Text = msg.ToString
    End Sub

    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
        Me.Close()
    End Sub

End Class
