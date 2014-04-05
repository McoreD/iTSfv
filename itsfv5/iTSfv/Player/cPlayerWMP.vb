Imports WMPLib

Public Class cPlayerWMP
    Inherits cPlayer
    Implements iPlayer

    Private mWmpApp As WMPLib.WindowsMediaPlayer
    Private mMainLibraryCollection As WMPLib.IWMPMediaCollection

    Public Sub New()

        mWmpApp = New WMPLib.WindowsMediaPlayer
        mMainLibraryCollection = mWmpApp.mediaCollection
        mMainLibraryTracks = mMainLibraryCollection.getAll

    End Sub

    Public Function AddMedia(ByVal filePath As String) As Boolean
        Dim success As Boolean = True
        Try
            mMainLibraryCollection.add(filePath)
            'Console.Writeline("Added " & filePath)
        Catch ex As Exception
            success = False
        End Try
        Return success
    End Function

    Public Function DeleteMedia(ByVal track As WMPTrack) As Boolean
        Dim success As Boolean = True
        Try
            mMainLibraryCollection.remove(track.fGetWMPMedia3, True)
            'Console.Writeline("Deleted " & track.Location)
        Catch ex As Exception
            success = False
        End Try
        Return success
    End Function

    Public ReadOnly Property MusicFolderPath() As String

        Get
            Dim regKey As Microsoft.Win32.RegistryKey = _
            Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\Microsoft\MediaPlayer\Preferences", True)
            If regKey.GetValue("TrackFoldersDirectories0") IsNot Nothing Then
                Return regKey.GetValue("TrackFoldersDirectories0").ToString
            End If
            Return String.Empty
        End Get

    End Property

    Public Function LoadLibrary() As Boolean Implements iPlayer.LoadLibrary

    End Function

    Public Overloads ReadOnly Property MainLibraryTracks() As WMPLib.IWMPPlaylist
        Get
            Return CType(mMainLibraryTracks, IWMPPlaylist)
        End Get
    End Property

    Public Overloads ReadOnly Property Progress() As Integer Implements iPlayer.Progress
        Get
            Return Me.mProgress
        End Get
    End Property

    Public Function SelectedTracks() As System.Collections.Generic.List(Of cXmlTrack) Implements iPlayer.SelectedTracks

        Dim lXmlTracks As New List(Of cXmlTrack)

        Return lXmlTracks

    End Function



End Class
