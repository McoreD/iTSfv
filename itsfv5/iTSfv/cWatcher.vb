Imports System.IO
Imports System.ComponentModel

Public Class cWatcher

    Private mWatchers As New List(Of FileSystemWatcher)
    Private mWatchFolders As New List(Of String)
    Private bwApp As BackgroundWorker
    Private mFiles As New List(Of String)
    Private mWatching As Boolean
    Private mSbActivity As New System.Text.StringBuilder

    Public ReadOnly Property Watching() As Boolean
        Get
            Return mWatching
        End Get
    End Property

    Public ReadOnly Property Files() As List(Of String)
        Get
            Return mFiles
        End Get
    End Property

    Public Sub ClearList()
        mFiles.Clear()
    End Sub

    Public ReadOnly Property Activity() As String
        Get
            Return mSbActivity.ToString
        End Get
    End Property

    Public Sub StartWatchFolders()

        If mWatchFolders.Count <> My.Settings.MusicFolderLocations.Count Then
            For Each lDir As String In My.Settings.MusicFolderLocations
                If Directory.Exists(lDir) Then
                    '' 5.35.02.2 Prevented crash upon application startup when attempting to Folder Watch a non-existant folder [tucobenedito]
                    If mWatchFolders.Contains(lDir) = False Then
                        Dim lWatcher As New FileSystemWatcher(lDir)
                        AddHandler lWatcher.Changed, AddressOf mWatcher_Changed
                        AddHandler lWatcher.Created, AddressOf mWatcher_Created
                        AddHandler lWatcher.Deleted, AddressOf mWatcher_Deleted
                        AddHandler lWatcher.Renamed, AddressOf mWatcher_Renamed
                        mWatchFolders.Add(lDir)
                        lWatcher.IncludeSubdirectories = True
                        mWatchers.Add(lWatcher)
                    End If
                End If
            Next
        End If

        For Each fsw As FileSystemWatcher In mWatchers
            fsw.EnableRaisingEvents = True
            mWatching = True
        Next

    End Sub

    Public Sub StopWatchFolders()

        For Each fsw As FileSystemWatcher In mWatchers
            fsw.EnableRaisingEvents = False
            mWatching = False
        Next

        If mSbActivity.Length > 0 Then
            Using sw As New StreamWriter(mFilePathMusicFolderActivity, True)
                sw.WriteLine(mSbActivity.ToString)
            End Using
            mSbActivity = New System.Text.StringBuilder
        End If

    End Sub

    Private Sub sLogActivity(ByVal msg As String)
        mSbActivity.AppendLine(Now.ToString("yyyy-MM-ddTHH:mm:ss") & " " & msg)
    End Sub

    Private Sub mWatcher_Changed(ByVal sender As Object, ByVal e As System.IO.FileSystemEventArgs)

        sLogActivity("Modified: " & e.FullPath)

    End Sub

    Private Sub sUpdateList(ByVal filePath As String)

        Try
            Dim files As New List(Of String)
            files = mfGetAddableFilesList(filePath)

            For Each f As String In files
                sAddFileToListBoxTracks(f)
            Next
        Catch ex As Exception
            msAppendWarnings(ex.Message)
        End Try

    End Sub
    Private Sub mWatcher_Created(ByVal sender As Object, ByVal e As System.IO.FileSystemEventArgs)

        If bwApp.IsBusy = False Then

            sLogActivity(" Created: " & e.FullPath)

            sUpdateList(e.FullPath)

        End If

    End Sub

    Private Sub mWatcher_Deleted(ByVal sender As Object, ByVal e As System.IO.FileSystemEventArgs)

        sLogActivity(" Deleted: " & e.FullPath)

        If bwApp.IsBusy = False Then
            sRemoveFileFromList(e.FullPath)
        End If

    End Sub

    Public Sub New(ByVal lBwApp As BackgroundWorker)

        Me.bwApp = lBwApp

        For Each lDir As String In My.Settings.MusicFolderLocations
            If Directory.Exists(lDir) Then
                mWatchFolders.Add(lDir)
                Dim lWatcher As New FileSystemWatcher(lDir)
                AddHandler lWatcher.Changed, AddressOf mWatcher_Changed
                AddHandler lWatcher.Created, AddressOf mWatcher_Created
                AddHandler lWatcher.Deleted, AddressOf mWatcher_Deleted
                AddHandler lWatcher.Renamed, AddressOf mWatcher_Renamed
                lWatcher.IncludeSubdirectories = True
                mWatchers.Add(lWatcher)
            End If
        Next

    End Sub

    Private Sub sAddFileToListBoxTracks(ByVal filePath As String)

        ' 4.0.5.1 Prevented possible addition of duplicate entries to tracks ListBox in Explorer tab
        If mFiles.Contains(filePath) = False Then
            mFiles.Add(filePath)
        End If

    End Sub

    Private Sub sRemoveFileFromList(ByVal filePath As String)
        If mFiles.Contains(filePath) Then
            mFiles.Remove(filePath)
        End If
    End Sub

    Protected Overrides Sub Finalize()

        MyBase.Finalize()

    End Sub

    Private Sub mWatcher_Renamed(ByVal sender As Object, ByVal e As System.IO.RenamedEventArgs)

        sLogActivity("Renamed: " & e.OldFullPath & " to " & e.FullPath)

        sRemoveFileFromList(e.OldFullPath)
        sUpdateList(e.FullPath)

    End Sub

End Class
