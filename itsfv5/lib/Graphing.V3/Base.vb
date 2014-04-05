Imports System.Drawing
Imports System.Drawing.Imaging
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

Namespace Base
#Region "Base Classes"
    Public MustInherit Class BaseChunk : Inherits textStruct
        Private _Value As Decimal
        Private _KeyName As String

        Public Property KeyName() As String
            Get
                Return _KeyName
            End Get
            Set(ByVal value As String)
                _KeyName = value
            End Set
        End Property
        Public Property [Value]() As Decimal
            Get
                Return _Value
            End Get
            Set(ByVal Value1 As Decimal)
                _Value = Value1
            End Set
        End Property
    End Class
    Public MustInherit Class BaseGraph : Inherits textStruct
        Public ReadOnly Property version() As String
            Get
                Return "3.0"
            End Get
        End Property
        Private _KeyFontName As String = "Ariel"
        Private _KeyFontSize As Integer = 10
        Private _KeyFontStyle As System.Drawing.FontStyle = FontStyle.Regular

        Private _KeyBackColor As Color = Color.White
        'Private _KeyBackColor As Color =  System.Drawing.SystemColors.Control

        Private _KeyTitle As String = ""
        Private _KeyTitleColor As Color = Color.Black
        Private _KeyTitleFontName As String = "Ariel"
        Private _KeyTitleFontSize As Integer = 10
        Private _KeyTitleFontStyle As System.Drawing.FontStyle = FontStyle.Regular

        Private _ImageSize As Size = New Size(100, 100)
        Private _ValueType As b_ChartValueType = b_ChartValueType.ValueTotal
        Private _ChartType As b_ChartType = b_ChartType.Pie
        Private _GraphBorder As Boolean = True
        Private _GraphRect As Rectangle = New Rectangle(0, 0, 0, 0)
        Private _AutoSize As Boolean = False
        Private _GraphAlign As b_BarTypes = b_BarTypes.HorizontalLeft

        Public Property KeyFontName() As String
            Get
                Return _KeyFontName
            End Get
            Set(ByVal value As String)
                _KeyFontName = value
            End Set
        End Property
        Public Property KeyFontSize() As Integer
            Get
                Return _KeyFontSize
            End Get
            Set(ByVal value As Integer)
                _KeyFontSize = value
            End Set
        End Property
        Public Property KeyFontStyle() As System.Drawing.FontStyle
            Get
                Return _KeyFontStyle
            End Get
            Set(ByVal value As System.Drawing.FontStyle)
                _KeyFontStyle = value
            End Set
        End Property

        Public Property KeyBackColor() As Color
            Get
                Return _KeyBackColor
            End Get
            Set(ByVal value As Color)
                _KeyBackColor = value
            End Set
        End Property

        Public Property KeyTitle() As String
            Get
                Return _KeyTitle
            End Get
            Set(ByVal value As String)
                _KeyTitle = value
            End Set
        End Property
        Public Property KeyTitleColor() As Color
            Get
                Return _KeyTitleColor
            End Get
            Set(ByVal value As Color)
                _KeyTitleColor = value
            End Set
        End Property
        Public Property KeyTitleFontName() As String
            Get
                Return _KeyTitleFontName
            End Get
            Set(ByVal value As String)
                _KeyTitleFontName = value
            End Set
        End Property
        Public Property KeyTitleFontSize() As Integer
            Get
                Return _KeyTitleFontSize
            End Get
            Set(ByVal value As Integer)
                _KeyTitleFontSize = value
            End Set
        End Property
        Public Property KeyTitleFontStyle() As System.Drawing.FontStyle
            Get
                Return _KeyTitleFontStyle
            End Get
            Set(ByVal value As System.Drawing.FontStyle)
                _KeyTitleFontStyle = value
            End Set
        End Property

        Public Property AutoSize() As Boolean
            Get
                Return _AutoSize
            End Get
            Set(ByVal Value As Boolean)
                _AutoSize = Value
            End Set
        End Property
        Public Property ImageSize() As Size
            Get
                Return _ImageSize
            End Get
            Set(ByVal Value As Size)
                _ImageSize = Value
            End Set
        End Property
        Public Property ValueType() As b_ChartValueType
            Get
                Return _ValueType
            End Get
            Set(ByVal Value As b_ChartValueType)
                _ValueType = Value
            End Set
        End Property
        Public Property ChartType() As b_ChartType
            Get
                Return _ChartType
            End Get
            Set(ByVal Value As b_ChartType)
                _ChartType = Value
            End Set
        End Property
        Public Property GraphBorder() As Boolean
            Get
                Return _GraphBorder
            End Get
            Set(ByVal Value As Boolean)
                _GraphBorder = Value
            End Set
        End Property
        Protected Friend Property GraphRect() As Rectangle
            Get
                Return _GraphRect
            End Get
            Set(ByVal Value As Rectangle)
                _GraphRect = Value
            End Set
        End Property
        Public Property GraphAlign() As b_BarTypes
            Get
                Return _GraphAlign
            End Get
            Set(ByVal Value As b_BarTypes)
                _GraphAlign = Value
            End Set
        End Property
    End Class
    Public MustInherit Class textStruct
        Private _Color As Color = Color.Black
        Public Property [Color]() As Color
            Get
                Return _Color
            End Get
            Set(ByVal Value As Color)
                _Color = Value
            End Set
        End Property
    End Class

    Public Class BaseChunkCollection : Inherits CollectionBase
        Private _totalValue As Decimal = 0D

        Public Shadows Function Add(ByVal Chunk As Object) As Integer
            _totalValue += Chunk.Value
            Return List.Add(Chunk)
        End Function
        Public Shadows Function Remove(ByVal Index As Integer) As Boolean
            Try
                _totalValue -= List.Item(Index).value
                List.RemoveAt(Index)
            Catch ex As Exception
                'Log Error
            End Try
            Return True
        End Function
        Public ReadOnly Property MaxKeyNameLength() As Integer
            Get
                Dim _MaxValue As Integer

                For Each oBaseChunk As Base.BaseChunk In List

                    If _MaxValue < oBaseChunk.KeyName.Length Then
                        _MaxValue = oBaseChunk.KeyName.Length
                    End If
                Next
                Return _MaxValue

            End Get
        End Property
        Public ReadOnly Property MaxValue() As Decimal
            Get
                Dim oBaseChunk As Object
                Dim _MaxValue As Decimal = Decimal.MinValue
                For Each oBaseChunk In List
                    If _MaxValue < oBaseChunk.Value Then
                        _MaxValue = oBaseChunk.Value
                    End If
                Next
                Return _MaxValue
            End Get
        End Property
        Public ReadOnly Property MinValue() As Decimal
            Get
                Dim oBaseChunk As Object
                Dim _MinValue As Decimal = Decimal.MaxValue
                For Each oBaseChunk In List
                    If _MinValue > oBaseChunk.Value Then
                        _MinValue = oBaseChunk.Value
                    End If
                Next
                Return _MinValue
            End Get
        End Property
        Public ReadOnly Property TotalValue() As Decimal
            Get
                Return _totalValue
            End Get
        End Property
        'Private Function GetValue(ByVal ValueType As collValue)
        '    Dim oBaseChunk As Object
        '    Dim iHolder As Decimal = Decimal.Zero

        '    For Each oBaseChunk In list
        '        Select Case ValueType
        '            Case collValue.MaxValue
        '                If iHolder < oBaseChunk.Value Then
        '                    iHolder = oBaseChunk.Value
        '                End If
        '            Case collValue.MinValue
        '                If iHolder > oBaseChunk.Value Or iHolder = Decimal.MinValue Then
        '                    iHolder = oBaseChunk.Value
        '                End If
        '            Case collValue.TotalValue
        '                iHolder += oBaseChunk.Value

        '        End Select
        '    Next

        'End Function
        'Private Enum collValue
        'TotalValue = 0
        'MinValue = 1
        'MaxValue = 2
        'End Enum
    End Class
    Public Structure Padding
        Private _cellpadding As Integer
        Private _cellspacing As Integer
        Private _border As Integer
        Sub New(ByVal Padding As Integer, ByVal Spacing As Integer, ByVal CellBorder As Integer)
            _cellpadding = Padding
            _cellspacing = Spacing
            _border = CellBorder
        End Sub
        Public Property CellPadding() As Integer
            Get
                Return _cellpadding
            End Get
            Set(ByVal Value As Integer)
                _cellpadding = Value
            End Set
        End Property
        Public Property CellSpacing() As Integer
            Get
                Return _cellspacing
            End Get
            Set(ByVal Value As Integer)
                _cellspacing = Value
            End Set
        End Property
        Public Property Border() As Integer
            Get
                Return _border
            End Get
            Set(ByVal Value As Integer)
                _border = Value
            End Set
        End Property
    End Structure
    Public Structure MathInfo
        Private _Divisor As Integer
        Private _Multiplier As String
        Private _Max As Integer

        Public Property Divisor() As Long
            Get
                Return _Divisor
            End Get
            Set(ByVal value As Long)
                _Divisor = value
            End Set
        End Property

        Public Property Multiplier() As String
            Get
                Return _Multiplier
            End Get
            Set(ByVal value As String)
                _Multiplier = value
            End Set
        End Property

        Public Property Max() As Integer
            Get
                Return _Max
            End Get
            Set(ByVal value As Integer)
                _Max = value
            End Set
        End Property
    End Structure

#End Region
#Region "Base Enums"
    Public Enum b_BarTypes
        HorizontalLeft = 0
        HorizontalRight = 1
        VerticalTop = 2
        VerticalBottom = 3
    End Enum
    Public Enum b_LineTypes
        Horizontal = 1
        Vertical = 2
    End Enum
    Public Enum b_ChartValueType
        ValuePercent = 1
        ValueTotal = 2
    End Enum
    Public Enum b_ChartType
        Pie = 0
        Bar = 1
        Line = 2
    End Enum
#End Region
End Namespace



