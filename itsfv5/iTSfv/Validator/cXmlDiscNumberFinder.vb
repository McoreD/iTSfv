Imports System.IO

Public Class cXmlDiscNumberFinder

    Private mDisc As cXmlDisc = Nothing

    Public Sub New(ByVal lDisc As cXmlDisc)

        mDisc = lDisc

        Me.DiscNumber = fGetDiscNumber(lDisc.Location)

    End Sub

    Private mDiscNumber As Integer
    Public Property DiscNumber() As Integer
        Get
            Return mDiscNumber
        End Get
        Set(ByVal value As Integer)
            mDiscNumber = value
        End Set
    End Property


    Private Function fGetDiscNumber(ByVal filePath As String) As Integer

        ' tag sources are folder name, file name, album tag

        Dim lClues As New Dictionary(Of String, Integer)

        For i As Integer = 1 To 9
            lClues.Add(String.Format("Disc {0}", i.ToString("0")), i)
            'listDirs.Add(String.Format("Disk {0}", i.ToString("0")))
            lClues.Add(String.Format("CD{0}", i.ToString("0")), i)
        Next

        'For i As Integer = 1 To 12
        '    listDirs.Add(String.Format("Disc {0}", i.ToString("00")))
        '    listDirs.Add(String.Format("Disk {0}", i.ToString("00")))            
        'Next

        Dim dir As String = Path.GetFileName(filePath)
        Dim dn As Integer = fFindDiscNumber(lClues, dir)

        If dn = 0 Then
            ' search for tag
            Dim f As TagLib.File = TagLib.File.Create(mDisc.FirstTrack.Location)
            dn = fFindDiscNumber(lClues, f.Tag.Album)
        End If

        Return dn

    End Function

    Private Function fFindDiscNumber(ByVal lClues As Dictionary(Of String, Integer), ByVal str As String) As Integer

        Dim dn As Integer = 0

        For Each s As KeyValuePair(Of String, Integer) In lClues
            If str.Contains(s.Key) Then
                dn = s.Value
                Exit For
            End If
        Next

        Return dn

    End Function

End Class
