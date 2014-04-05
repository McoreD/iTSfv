
Public Class frmTrackReplace

    Private Sub ListBox1_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles lbTracksNew.DragDrop

        lbTracksNew.Items.Clear()

        Dim DropContents As String() = CType(e.Data.GetData(DataFormats.FileDrop, True), String())

        Dim listDropContents As New List(Of String)
        For Each fileOrDir As String In DropContents
            listDropContents.Add(fileOrDir)
        Next

        Dim listAddableFiles As New List(Of String)
        For Each dirOrFile As String In listDropContents
            listAddableFiles.AddRange(mfGetAddableFilesList(dirOrFile, mFileExtOtherAudio))
        Next

        listAddableFiles.Sort()

        For Each filePath As String In listAddableFiles
            lbTracksNew.Items.Add(filePath)
        Next

    End Sub

    Private Sub lbTracksNew_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles lbTracksNew.DragEnter

        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.All
        Else
            e.Effect = DragDropEffects.None
        End If

    End Sub


    Private Sub lbTracksOld_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles lbTracksOld.DragDrop

        lbTracksOld.Items.Clear()

        Dim DropContents As String() = CType(e.Data.GetData(DataFormats.FileDrop, True), String())

        Dim listDropContents As New List(Of String)
        For Each fileOrDir As String In DropContents
            listDropContents.Add(fileOrDir)
        Next

        Dim listAddableFiles As New List(Of String)
        For Each dirOrFile As String In listDropContents
            listAddableFiles.AddRange(mfGetAddableFilesList(dirOrFile, mFileExtOtherAudio))
        Next

        listAddableFiles.Sort()

        For Each filePath As String In listAddableFiles
            lbTracksOld.Items.Add(filePath)
        Next

    End Sub

    Private Sub lbTracksOld_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles lbTracksOld.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.All
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub lbTracksOld_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbTracksOld.SelectedIndexChanged

    End Sub

    Private Sub btnRename_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRename.Click

        If lbTracksNew.Items.Count = lbTracksOld.Items.Count Then

            For i As Integer = 0 To lbTracksNew.Items.Count - 1

                ''Console.Writeline(lbTracksNew.Items.Item(i).ToString)
                ''Console.Writeline(IO.Path.GetFileNameWithoutExtension(lbTracksOld.Items.Item(i).ToString) + IO.Path.GetExtension(lbTracksNew.Items.Item(i).ToString))
                Dim filePathOld As String = lbTracksNew.Items.Item(i).ToString
                Dim fileNameNew As String = IO.Path.GetFileNameWithoutExtension(lbTracksOld.Items.Item(i).ToString) + IO.Path.GetExtension(lbTracksNew.Items.Item(i).ToString)
                If IO.Path.GetFileName(filePathOld).ToLower <> fileNameNew.ToLower Then
                    My.Computer.FileSystem.RenameFile(filePathOld, fileNameNew)
                End If
            Next

            lbTracksNew.Items.Clear()
            lbTracksOld.Items.Clear()

        End If

    End Sub

    Private Sub frmTrackReplace_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed

        lbTracksNew.Items.Clear()
        lbTracksOld.Items.Clear()

    End Sub

    Private Sub frmTrackReplace_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.Icon = My.Forms.frmMain.Icon
        Me.Text = Application.ProductName + " - Track Replace Assistant"

    End Sub
End Class

