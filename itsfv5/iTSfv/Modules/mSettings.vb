Imports System.Collections.Specialized

Public Module mSettings

    Public Property mpMusicFolderPath() As String

        Get
            Dim mfp As String = String.Empty
            Select Case mpAppMode
                Case eAppMode.WMPFV
                    mfp = My.Settings.WMP_MusicFolderPath
                Case Else
                    mfp = My.Settings.MusicFolderPath
            End Select
            If mfp.EndsWith(IO.Path.DirectorySeparatorChar) = False Then
                mfp += IO.Path.DirectorySeparatorChar
            End If
            Return mfp
        End Get

        Set(ByVal value As String)

            If value.EndsWith(IO.Path.DirectorySeparatorChar) = False Then
                value += IO.Path.DirectorySeparatorChar
            End If

            Select Case mpAppMode
                Case eAppMode.WMPFV
                    My.Settings.WMP_MusicFolderPath = value
                Case Else
                    My.Settings.MusicFolderPath = value
            End Select

        End Set

    End Property

    Public Property mpMusicFolderPaths() As StringCollection

        Get
            Select Case mpAppMode
                Case eAppMode.WMPFV
                    Return My.Settings.WMP_MusicFolderPaths
                Case Else
                    Return My.Settings.MusicFolderLocations
            End Select
        End Get

        Set(ByVal value As StringCollection)

            Select Case mpAppMode
                Case eAppMode.WMPFV
                    My.Settings.WMP_MusicFolderPaths = value
                Case Else
                    My.Settings.MusicFolderLocations = value
            End Select

        End Set

    End Property

    Public ReadOnly Property mpCurrentPlayer() As String

        Get
            Select Case mpAppMode
                Case eAppMode.ITSFV
                    Return "iTunes"
                Case eAppMode.WMPFV
                    Return "Windows Media Player"
                Case Else
                    Return "Directory"
            End Select
            Return String.Empty
        End Get

    End Property

End Module
