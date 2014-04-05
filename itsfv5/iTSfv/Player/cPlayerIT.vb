Imports iTunesLib

Public Class cPlayerIT
    Inherits cPlayer
    Implements iPlayer

    Public Function LoadLibrary() As Boolean Implements iPlayer.LoadLibrary


    End Function

    Public Overloads ReadOnly Property Progress() As Integer Implements iPlayer.Progress

        Get
            Return mProgress
        End Get
    End Property

    Public Function SelectedTracks() As System.Collections.Generic.List(Of cXmlTrack) Implements iPlayer.SelectedTracks

        Dim lXmlTracks As New List(Of cXmlTrack)

        Return lXmlTracks

    End Function


End Class
