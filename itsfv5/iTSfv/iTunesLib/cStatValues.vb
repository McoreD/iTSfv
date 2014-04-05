<Serializable()> Public Class cStatValues

    Private mCount As Integer
    Public Property Count() As Integer
        Get
            Return mCount
        End Get
        Set(ByVal value As Integer)
            mCount = value
        End Set
    End Property

    Private mScore As Integer
    Public Property Score() As Integer
        Get
            Return mScore
        End Get
        Set(ByVal value As Integer)
            mScore = value
        End Set
    End Property

    Private mRating As Integer
    Public Property Rating() As Integer
        Get
            Return mRating
        End Get
        Set(ByVal value As Integer)
            mRating = value
        End Set
    End Property

End Class
