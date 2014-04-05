Public MustInherit Class cPlayer

    Protected mProgress As Integer
    Protected mMainLibraryTracks As Object

    Public ReadOnly Property Progress() As Integer
        Get
            Return mProgress
        End Get

    End Property

    Public Overloads ReadOnly Property MainLibraryTracks() As Object
        Get
            Return mMainLibraryTracks
        End Get
    End Property

End Class
