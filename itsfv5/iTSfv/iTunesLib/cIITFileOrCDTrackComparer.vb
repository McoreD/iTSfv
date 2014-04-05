Imports iTunesLib
Imports System.Collections.Generic

Public Class cIITFileOrCDTrackComparer

    Implements IComparer(Of IITFileOrCDTrack)

    Public Function Compare(ByVal x As iTunesLib.IITFileOrCDTrack, ByVal y As iTunesLib.IITFileOrCDTrack) As Integer Implements System.Collections.Generic.IComparer(Of iTunesLib.IITFileOrCDTrack).Compare

        Return String.Compare(x.Location, y.Location)

    End Function

End Class
