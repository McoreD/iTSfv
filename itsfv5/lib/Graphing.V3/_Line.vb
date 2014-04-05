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

Namespace Line
    'A chart is what you would normally work with,
    'it is a collection of pieces and the chart info itself.
    Public Class LineChart : Inherits Base.BaseGraph
        Private _Alignment As Base.b_LineTypes = Base.b_LineTypes.Horizontal
        Private _LineColor As System.Drawing.Color = System.Drawing.Color.Black

        Public LinePlotCollection As New Line.LinePlotCollection()
        Public Property Alignment() As Base.b_LineTypes
            Get
                Return _Alignment
            End Get
            Set(ByVal Value As Base.b_LineTypes)
                _Alignment = Value
            End Set
        End Property
        Public Property LineColor() As System.Drawing.Color
            Get
                Return _LineColor
            End Get
            Set(ByVal Value As System.Drawing.Color)
                _LineColor = Value
            End Set
        End Property
        Sub New()
            MyBase.new()
            MyBase.ChartType = Base.b_ChartType.Line
        End Sub
        Sub New(ByVal ImgSize As System.Drawing.Size)
            MyBase.new()
            MyBase.ChartType = Base.b_ChartType.Line
            ImageSize = ImgSize
        End Sub
        Sub New(ByVal ImgAutoSize As Boolean)
            MyBase.New()
            MyBase.ChartType = Base.b_ChartType.Line
            AutoSize = ImgAutoSize
        End Sub
        Sub New(ByVal ImgAutoSize As Boolean, ByVal LinePieceCollection As LinePlotCollection)
            MyBase.New()
            MyBase.ChartType = Base.b_ChartType.Line
            AutoSize = ImgAutoSize
            LinePieceCollection = LinePieceCollection
        End Sub
        Sub New(ByVal LinePlotCollection As LinePlotCollection)
            MyBase.new()
            MyBase.ChartType = Base.b_ChartType.Line
            LinePlotCollection = LinePlotCollection
        End Sub
    End Class

    'A LineSlice can be created alone or by just using the collection 
    'inside Chart
    Public Class LineSlice : Inherits Base.BaseChunk
        Friend Point As System.Drawing.Point = New System.Drawing.Point(0, 0)

        Sub New(ByVal decValue As Decimal, ByVal cColor As System.Drawing.Color, ByVal sKeyName As String)
            MyBase.new()
            Value = decValue
            Color = cColor
            KeyName = sKeyName
        End Sub
    End Class

    'A LinePlotCollection can be created all alone but it is a part of LineChart
    Public Class LinePlotCollection : Inherits Base.BaseChunkCollection
        Public Sub New()
            MyBase.New()
            List.Clear()
        End Sub
        Public Shadows Function Add(ByVal oPiePiece As LineSlice) As Integer
            Return MyBase.Add(oPiePiece)
        End Function
    End Class
End Namespace
