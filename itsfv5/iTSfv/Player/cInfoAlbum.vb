Public Class cInfoAlbum

    ' this class should hold discs

    Private mDiscs As New List(Of cInfoDisc)

    Private mAlbumName As String = "Unknown Album"
    Public Property Name() As String
        Get
            Return mAlbumName
        End Get
        Set(ByVal value As String)
            mAlbumName = value
        End Set
    End Property

    Public Property Discs() As List(Of cInfoDisc)
        Get
            Return mDiscs
        End Get
        Set(ByVal value As List(Of cInfoDisc))
            mDiscs = value
        End Set
    End Property

    Public Function HasDisc(ByVal disc As cInfoDisc) As Boolean

        For Each myDisc As cInfoDisc In Discs
            If myDisc.Name.Equals(disc.Name) Then
                Return True
            End If
        Next

        Return False

    End Function

    Public Sub New(ByVal lAlbumName As String)

        Me.mAlbumName = lAlbumName

    End Sub
End Class
