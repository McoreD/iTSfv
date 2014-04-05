'Feel free to use this as you wish it is free. With this said
'I hold no responsiblity for any damage mentally or physically, 
'in which you or your computer may experience. It is free from 
'defect as far as I can tell. I didn't comment as much as I wished
'So if you have any questions feel free to email me. I can be found
'on GotDotNet.com user besmith2. Peace out......Bruce Smith


'Most of the actual work is done here.
'I removed the key building ability 
'I might add it back, it just got to
'big and ugly. I might add a seperate 
'method to return a image with a key in it.

Namespace Bar
    'A chart is what you would normally work with,
    'it is a collection of pieces and the chart info itself.
    Public Class BarChart : Inherits Base.BaseGraph
        'This will hold the Bar pieces.
        Public BarSliceCollection As New Bar.BarPieceCollection()
        Private _Alignment As Base.b_BarTypes = Base.b_BarTypes.HorizontalLeft
        Public Property Alignment() As Base.b_BarTypes
            Get
                Return _Alignment
            End Get
            Set(ByVal Value As Base.b_BarTypes)
                _Alignment = Value
            End Set
        End Property
        'Private _ChartType As Base.b_ChartType = Base.b_ChartType.Bar

        Public Shadows ReadOnly Property ChartType() As Base.b_ChartType
            Get
                Return MyBase.ChartType
            End Get
        End Property

        Sub New()
            MyBase.new()
            MyBase.ChartType = Base.b_ChartType.Bar
        End Sub
        Sub New(ByVal BarPieceCollection As BarPieceCollection)
            MyBase.new()
            MyBase.ChartType = Base.b_ChartType.Bar
            BarSliceCollection = BarPieceCollection
        End Sub
    End Class
    'A BarSlice can be created alone or by just using the collection 
    'inside Chart
    Public Class BarSlice : Inherits Base.BaseChunk
        Sub New(ByVal decValue As Decimal, ByVal cColor As System.Drawing.Color, ByVal sKeyName As String)
            MyBase.new()
            Value = decValue
            Color = cColor
            KeyName = sKeyName
        End Sub
        'Private _Rectangle As New System.Drawing.Rectangle(0, 0, 0, 0)
        'Friend Property [Width]() As Integer
        '    Get
        '        Return _Rectangle.Width
        '    End Get
        '    Set(ByVal Value As Integer)
        '        _Rectangle.Width = Value
        '    End Set
        'End Property
        'Friend Property [Height]() As Integer
        '    Get
        '        Return _Rectangle.Height
        '    End Get
        '    Set(ByVal Value As Integer)
        '        _Rectangle.Height = Value
        '    End Set
        'End Property
        'Friend ReadOnly Property [Left]() As Integer
        '    Get
        '        Return _Rectangle.Left
        '    End Get
        'End Property
        'Friend ReadOnly Property [Top]() As Integer
        '    Get
        '        Return _Rectangle.Top
        '    End Get
        'End Property
        'Friend Property [Size]() As System.Drawing.Size
        '    Get
        '        Return _Rectangle.Size
        '    End Get
        '    Set(ByVal Value As System.Drawing.Size)
        '        _Rectangle.Size = Value
        '    End Set
        'End Property
        'Friend Property [Rectangle]() As System.Drawing.Rectangle
        '    Get
        '        Return _Rectangle
        '    End Get
        '    Set(ByVal Value As System.Drawing.Rectangle)
        '        _Rectangle = Value
        '    End Set
        'End Property
    End Class
    'A BarPieceCollection can be created all alone but it is a part of LineChart
    Public Class BarPieceCollection : Inherits Base.BaseChunkCollection
        Public Sub New()
            MyBase.New()
            List.Clear()
        End Sub
        Public Shadows Function Add(ByVal oBarPiece As BarSlice) As Integer
            Return MyBase.Add(oBarPiece)
        End Function

    End Class
End Namespace
