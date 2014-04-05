Imports System.IO
Imports System.Collections.Specialized

Public Class cFolderRemover

    Private mFolders As New List(Of String)
    Private mJunkFiles As New List(Of String)

    Public Sub New()
        ' nothing
    End Sub
    Public Sub New(ByVal folders As StringCollection)

        mJunkFiles.Add("desktop.ini")
        mJunkFiles.Add("thumbs.db")

        For Each f As String In folders
            If Directory.Exists(f) = True Then
                mFolders.Add(f)
            End If
        Next

    End Sub

    Public Sub RemoveEmptyFolders()

        mProgressDiscsMax = mFolders.Count
        mProgressDiscsCurrent = 0

        Dim foldersToDelete As New List(Of String)

        For Each d As String In mFolders

            Try

                Dim sd As String() = Directory.GetDirectories(d, "*.*", SearchOption.AllDirectories)
                mProgressTracksMax = sd.Length
                mProgressTracksCurrent = 0

                mfUpdateStatusBarText("Checking folders to delete...", secondary:=False)

                For Each p As String In sd

                    If Directory.Exists(p) = True Then

                        Dim di As New DirectoryInfo(p)
                        If di.Attributes.ToString.Contains(FileAttributes.System.ToString) = False Then

                            If fFolderSafeToDelete(p) = True Then

                                foldersToDelete.Add(p)
                                mfUpdateStatusBarText("Deleting folder " & p, secondary:=False)

                            End If

                        End If

                    End If

                    mProgressTracksCurrent += 1

                Next

            Catch ex As Exception
                msAppendWarnings(ex.Message + " while checking folders to delete")
            End Try

            mProgressDiscsCurrent += 1

        Next

        sIndexFolderListToFile(New LogData(mFilePathFoldersOneFile, mListFoldersOneFile))
        sIndexFolderListToFile(New LogData(mFilePathFoldersNoAudio, mListFoldersNoAudio))

        If foldersToDelete.Count > 0 Then

            mProgressTracksMax = foldersToDelete.Count
            mProgressTracksCurrent = 0

            For Each p As String In foldersToDelete

                Try
                    msAppendDebug("Deleting folder " & p)
                    My.Computer.FileSystem.DeleteDirectory(p, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
                    mfUpdateStatusBarText("Deleted folder " & p, secondary:=False)
                Catch ex As Exception
                    msAppendWarnings("Could not delete empty folder " & p)
                End Try

                mProgressTracksCurrent += 1

            Next

        End If

    End Sub

    Private Sub sIndexFolderListToFile(ByVal log As LogData)

        If log.PathList.Count > 0 Then

            Dim tgApp As New TreeGUI.cAdapter
            tgApp.GetConfig.SetSingleIndexPath(log.Destination)

            For Each d As String In log.PathList
                tgApp.GetConfig.FolderList.Add(d)
            Next

            Dim tnl As New TreeGUI.cTreeNetLib(tgApp)
            tnl.IndexNow(TreeGUI.cAdapter.IndexingMode.IN_ONE_FOLDER_MERGED)

        End If

    End Sub

    Public Function fFolderSafeToDelete(ByVal folderPath As String) As Boolean

        Dim safeToDelete As Boolean = True

        Dim files() As String = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)

        If files.Length = 0 Then

            safeToDelete = True

        Else

            Dim bFolderHasNoAudio As Boolean = fFolderHasNoAudioFiles(files)

            If bFolderHasNoAudio = True Then
                If mListFoldersNoAudio.Contains(folderPath) = False Then
                    mListFoldersNoAudio.Add(folderPath)
                End If
            End If

            If files.Length = 1 Then

                Dim d As String = Path.GetDirectoryName(files(0))
                If mListFoldersOneFile.Contains(d) = False Then
                    mListFoldersOneFile.Add(d)
                End If
            End If

            For Each f As String In files
                If mJunkFiles.Contains(Path.GetFileName(f).ToLower) = False Then
                    safeToDelete = False
                End If
            Next

            If files.Length < My.Settings.NonAudioFilesCount Then

                If bFolderHasNoAudio = True AndAlso My.Settings.DeleteNonAudioFolders = True Then
                    safeToDelete = True
                End If

            End If

        End If

        Return safeToDelete

    End Function

    Private Function fFolderHasNoAudioFiles(ByVal files() As String) As Boolean

        Dim audioExt As New List(Of String)
        audioExt.AddRange(mFileExtAudioITunes)
        audioExt.AddRange(mFileExtAudioWMP)
        audioExt.AddRange(mFileExtOtherAudio)

        For Each ext As String In audioExt
            For Each f As String In files
                If Path.GetExtension(f).ToLower.Contains(ext.ToLower) Then
                    Return False
                End If
            Next
        Next

        Return True

    End Function

End Class
