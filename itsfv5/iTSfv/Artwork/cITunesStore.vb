Public Class cITunesStore

    Private mCountry As String
    Private mID As String

    Public Sub New(ByVal country As String, ByVal id As UInteger)

        Me.mCountry = country
        Me.mID = id.ToString

    End Sub

    Public ReadOnly Property Country() As String
        Get
            Return mCountry
        End Get
    End Property

    Public ReadOnly Property ID() As String
        Get
            Return mID
        End Get
    End Property

End Class
