Imports System.Collections.Specialized

Public Class cUrlToCoverParser

    Private mFilePath As String

    Private mDictionary As StringDictionary
    Private mKeys As New List(Of String)
    Private mValues As New List(Of String)
    Private mFilePathUrlToCoverArt As String

    Public Sub New(ByVal src As String)

        mFilePathUrlToCoverArt = src
        Try
            mDictionary = CType(mfReadObjectFromFileBF(src), Specialized.StringDictionary)
        Catch ex As Exception
            ' do nothing
        End Try

        If mDictionary Is Nothing Then
            mDictionary = New StringDictionary
        End If

        Dim et As IEnumerator = mDictionary.GetEnumerator

        Dim de As System.Collections.DictionaryEntry
        While et.MoveNext
            de = CType(et.Current, DictionaryEntry)
            mKeys.Add(CStr(de.Key))
            mValues.Add(CStr(de.Value))
        End While

    End Sub

    Public Sub SyncEntry(ByVal trackPath As String, ByVal artworkPath As String)

        If mDictionary.ContainsKey(trackPath) = False Then
            mDictionary.Add(trackPath, artworkPath)
        Else
            mDictionary(trackPath) = artworkPath
        End If
    End Sub

    Public Sub ReplaceEntries(ByVal tracksList As List(Of String), ByVal artworkList As List(Of String))

        mDictionary.Clear()
        SyncEntries(tracksList, artworkList)

    End Sub

    Public Sub SyncEntries(ByVal tracksList As List(Of String), ByVal artworkList As List(Of String))

        mDictionary.Clear()
        If tracksList.Count = artworkList.Count Then
            For i As Integer = 0 To tracksList.Count - 1
                SyncEntry(tracksList(i), artworkList(i))
            Next
        End If

    End Sub

    Public Sub SaveDictionary()

        If My.Settings.UrlToCoverArtBackup Then
            Dim ext As String = String.Format("{0}.bak", Now.ToString("yyyyMMdd"))
            My.Computer.FileSystem.CopyFile(mFilePathUrlToCoverArt, _
            IO.Path.ChangeExtension(mFilePathUrlToCoverArt, ext), True)
        End If

        mfWriteObjectToFileBF(mDictionary, mFilePathUrlToCoverArt)

    End Sub

    Public ReadOnly Property Dictionary() As StringDictionary
        Get
            Return mDictionary
        End Get
    End Property

    Public Property Keys() As List(Of String)
        Get
            Return mKeys
        End Get
        Set(ByVal value As List(Of String))
            mKeys = value
        End Set
    End Property

    Public Property Values() As List(Of String)
        Get
            Return mValues
        End Get
        Set(ByVal value As List(Of String))
            mValues = value
        End Set
    End Property

End Class
