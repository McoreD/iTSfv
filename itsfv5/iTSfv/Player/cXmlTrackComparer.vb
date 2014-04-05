Public Class cXmlTrackComparer
    Implements IComparer(Of cXmlTrack)

    Public Function Compare(ByVal x As cXmlTrack, ByVal y As cXmlTrack) As Integer Implements System.Collections.Generic.IComparer(Of cXmlTrack).Compare

        Return String.Compare(x.Location, y.Location)

    End Function

End Class

Public Module XmlTrackComparerMethods

    Public Function CompareByLocation(ByVal x As cXmlTrack, ByVal y As cXmlTrack) As Integer

        Return String.Compare(x.Location, y.Location)

    End Function

    Public Function CompareByTrackNumber(ByVal x As cXmlTrack, ByVal y As cXmlTrack) As Integer

        Return String.Compare(x.TrackNumber.ToString, y.TrackNumber.ToString)

    End Function

    Public Function CompareByArtworkSize(ByVal x As cXmlTrack, ByVal y As cXmlTrack) As Integer

        Return String.Compare(x.Artwork.Width.ToString("0000"), y.Artwork.Width.ToString("0000"))

    End Function

End Module