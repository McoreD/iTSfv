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
Namespace Pie
    'A chart is what you would normally work with,
    'it is a collection of pieces and the chart info itself.
    Public Class PieChart : Inherits Base.BaseGraph
        'This will hold the pie pieces.
        Public PieSliceCollection As New Pie.PiePieceCollection()
        Public Diameter As Single
        Public Thickness As Single
        Sub New()
            MyBase.new()
            MyBase.ChartType = Base.b_ChartType.Pie
        End Sub
        Sub New(ByVal ImgSize As System.Drawing.Size)
            MyBase.new()
            MyBase.ChartType = Base.b_ChartType.Pie
            ImageSize = ImgSize
        End Sub
        Sub New(ByVal ImgAutoSize As Boolean)
            MyBase.New()
            MyBase.ChartType = Base.b_ChartType.Pie
            AutoSize = ImgAutoSize
        End Sub
        Sub New(ByVal ImgAutoSize As Boolean, ByVal PiePieceCollection As PiePieceCollection)
            MyBase.New()
            MyBase.ChartType = Base.b_ChartType.Pie
            AutoSize = ImgAutoSize
            PieSliceCollection = PiePieceCollection
        End Sub
        Sub New(ByVal ImgSize As System.Drawing.Size, ByVal PiePieceCollection As PiePieceCollection)
            MyBase.new()
            MyBase.ChartType = Base.b_ChartType.Pie
            PieSliceCollection = PiePieceCollection
        End Sub
    End Class
    'A pie slice can be created alone or by just using the collection 
    'inside PieChart
    Public Class PieSlice : Inherits Base.BaseChunk
        Private _sweepAngle As Decimal = 0D
        Private _PiecePercent As Decimal = 0D

        Sub New(ByVal decValue As Decimal, ByVal cColor As System.Drawing.Color, ByVal sKeyName As String)
            MyBase.new()

            Value = decValue
            Color = cColor
            KeyName = sKeyName
        End Sub

        Friend Property PiecePercent() As Decimal
            Get
                Return _PiecePercent
            End Get
            Set(ByVal Value As Decimal)
                _PiecePercent = Value
            End Set
        End Property
        Friend Property SweepAngle() As Decimal
            Get
                Return (360 * (_PiecePercent / 100))
            End Get
            Set(ByVal Value As Decimal)
                _sweepAngle = Value
            End Set
        End Property
    End Class
    'A pie collection can be created all alone but it is a part of PieChart
    Public Class PiePieceCollection : Inherits Base.BaseChunkCollection
        Public Sub New()
            MyBase.New()
            List.Clear()
        End Sub

        Public Shadows Function Add(ByVal oPiePiece As PieSlice) As Integer
            Return MyBase.Add(oPiePiece)
        End Function
        Friend Sub CalcPercent()
            Dim oBaseChunk As PieSlice
            Dim lTvalue As Decimal = TotalValue
            For Each oBaseChunk In List
                Try
                    oBaseChunk.PiecePercent = Decimal.Round(((oBaseChunk.Value / lTvalue) * 100), 2)
                Catch

                End Try
            Next
        End Sub
        Friend Sub CalcPercent(ByVal CalcIsPercent As Boolean)
            Dim oBaseChunk As PieSlice
            Dim lTvalue As Decimal = TotalValue
            For Each oBaseChunk In List
                Try
                    oBaseChunk.PiecePercent = oBaseChunk.Value
                Catch

                End Try
            Next
        End Sub
    End Class
End Namespace
