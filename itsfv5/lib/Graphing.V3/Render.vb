Imports System.IO
Imports System.Threading
Imports System.drawing
Imports System.Drawing.Imaging
Imports System.Drawing.Drawing2D
Imports Graphing.V3.Pie
Imports Graphing.V3.Bar
Imports Graphing.V3.Line

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

Public Class Render
    Public GraphPadding As New Base.Padding(4, 4, 10)
    Public bMap As Bitmap
    '
    Public Overloads Function DrawChart(ByVal graph As Base.BaseGraph) As System.Drawing.Image
        Select Case graph.ChartType
            Case Base.b_ChartType.Bar
                Return DrawBarChart(CType(graph, BarChart))
            Case Base.b_ChartType.Line
                Return DrawLineChart(CType(graph, LineChart))
            Case Base.b_ChartType.Pie
                Return DrawPieChart(CType(graph, PieChart))

            Case Else
                Return Nothing
        End Select
    End Function
    Public Overloads Sub DrawChart(ByVal graph As Base.BaseGraph, ByVal retStream As Stream)
        Select Case graph.ChartType
            Case Base.b_ChartType.Bar
                DrawBarChart(CType(graph, BarChart), retStream)
            Case Base.b_ChartType.Line
                DrawLineChart(CType(graph, LineChart), retStream)
            Case Base.b_ChartType.Pie
                DrawPieChart(CType(graph, PieChart), retStream)
        End Select
    End Sub
    Public Overloads Function DrawKey(ByVal Graph As Base.BaseGraph) As System.Drawing.Image
        Dim kMap As Bitmap
        Dim oBaseChunkCollection As Base.BaseChunkCollection = Nothing
        Dim gBrush As New SolidBrush(Graph.KeyTitleColor)
        Dim bmSize As New System.Drawing.Size
        Dim minWidth As Integer = 0

        Select Case Graph.ChartType
            Case Base.b_ChartType.Bar
                oBaseChunkCollection = CType(Graph, BarChart).BarSliceCollection
            Case Base.b_ChartType.Line
                oBaseChunkCollection = CType(Graph, LineChart).LinePlotCollection
            Case Base.b_ChartType.Pie
                oBaseChunkCollection = CType(Graph, PieChart).PieSliceCollection
        End Select

        If Graph.KeyTitle.Length > oBaseChunkCollection.MaxKeyNameLength Then
            minWidth = Graph.KeyTitle.Length * Graph.KeyFontSize
        Else
            minWidth = oBaseChunkCollection.MaxKeyNameLength * Graph.KeyFontSize
        End If

        bmSize.Width = (minWidth)
        bmSize.Height = ((oBaseChunkCollection.Count * 20) + 25)

        kMap = GetBitmap(bmSize)
        Dim g As Graphics = Graphics.FromImage(kMap)
        g.SmoothingMode = SmoothingMode.AntiAlias ' antialias objects  
        g.Clear(Graph.KeyBackColor)  ' blank the image  

        If Graph.KeyTitle > "" Then
            Dim titleRectF As New RectangleF(0, 0, bmSize.Width + 12, 25)
            Dim titleFormat As New StringFormat()
            Dim titleFont As New Font(Graph.KeyTitleFontName, Graph.KeyTitleFontSize, Graph.KeyTitleFontStyle)
            gBrush.Color = Graph.KeyTitleColor
            titleFormat.Alignment = StringAlignment.Center
            titleFormat.LineAlignment = StringAlignment.Center
            g.DrawString(Graph.KeyTitle, titleFont, gBrush, titleRectF, titleFormat)
            titleRectF = Nothing
            titleFormat.Dispose()
            titleFont.Dispose()
        End If

        'DrawString(Text, Graphics, Alignment, Color)
        Dim drawFont As Font
        Dim approximateWidth As Integer = 0
        Dim gPen As New Pen(Color.Black, 2)
        Dim keyCount As Integer = oBaseChunkCollection.Count

        For Each bChunk As Base.BaseChunk In oBaseChunkCollection
            drawFont = New Font(Graph.KeyFontName, Graph.KeyFontSize, Graph.KeyFontStyle)
            If Len(bChunk.KeyName) > approximateWidth Then approximateWidth = Len(bChunk.KeyName)

            gPen.Width = 1
            gBrush.Color = bChunk.Color
            g.FillRectangle(gBrush, 5, keyCount + 24, 10, 10)
            gPen.Color = Color.Black
            g.DrawRectangle(gPen, 5, keyCount + 24, 10, 10)
            gBrush.Color = Color.Black
            g.DrawString(bChunk.KeyName, drawFont, gBrush, 17, keyCount + 21)
            keyCount += 18 '----This is the spacing between the Keys
        Next
        Return kMap
    End Function
    Public Overloads Sub DrawKey(ByVal Graph As Base.BaseGraph, ByVal retStream As Stream)
        Dim b As System.Drawing.Bitmap = DrawChart(Graph)
        b.Save(retStream, ImageFormat.Jpeg)
        b.Dispose()
    End Sub

#Region "        Private methods"
    Private Overloads Function DrawPieChart(ByVal PieChart As PieChart) As System.Drawing.Image
        If PieChart.AutoSize Then
            PieChart.ImageSize = AutoSize(PieChart, PieChart.PieSliceCollection)
        End If

        bMap = GetBitmap(PieChart.ImageSize)
        PieChart.GraphRect = CalcGraph(PieChart)
        'Change to percent if needed.
        If PieChart.ValueType = Base.b_ChartValueType.ValueTotal Then
            PieChart.PieSliceCollection.CalcPercent()
        Else
            PieChart.PieSliceCollection.CalcPercent(True)
        End If

        Dim lBorder As Integer = GraphPadding.Border
        Dim totalBorder As Integer = (lBorder * 3)
        PieChart.GraphRect = New Rectangle(lBorder, lBorder, _
                                           bMap.Width - totalBorder, bMap.Height - totalBorder)
        '   So I have a bitmap on which I can draw, and I have the pie percents now 
        'Its time to draw some stuff.

        Dim g As Graphics = Graphics.FromImage(bMap)
        Dim startAngle As Single = 0.0F
        Dim sweepAngle As Single = 0.0F
        Dim gBrush As New SolidBrush(PieChart.Color)

        g.SmoothingMode = SmoothingMode.AntiAlias ' antialias objects  
        g.Clear(PieChart.Color)  ' blank the image  

        Dim oPiePiece As PieSlice
        Dim gPen As New Pen(Color.Black, 1)
        Dim Diameter As Integer = PieChart.Diameter
        Dim thickness As Integer = PieChart.Thickness
        Dim pieRect As Rectangle

        For j As Integer = CInt(Diameter * thickness * 0.01F) To 0 Step -1
            For Each oPiePiece In PieChart.PieSliceCollection
                'Flip through the pieces Build the pie
                sweepAngle = 360 * (oPiePiece.PiecePercent / 100)
                pieRect = New Rectangle(PieChart.GraphRect.X, _
                                    CInt(PieChart.GraphRect.Y + CSng(j)), _
                                    Diameter, _
                                    Diameter)

                Dim hBrush As New HatchBrush(HatchStyle.Percent50, oPiePiece.Color)
                g.FillPie(hBrush, pieRect, startAngle, sweepAngle)
                startAngle += sweepAngle
            Next
        Next

        'Top Layer of Circle
        For j As Integer = CInt(Diameter * 0.01F) To 0 Step -1
            For Each oPiePiece In PieChart.PieSliceCollection
                'Flip through the pieces Build the pie
                sweepAngle = 360 * (oPiePiece.PiecePercent / 100)
                pieRect = New Rectangle(PieChart.GraphRect.X, _
                                    CInt(PieChart.GraphRect.Y + CSng(j)), _
                                    Diameter, _
                                    Diameter)

                gBrush.Color = oPiePiece.Color

                g.FillPie(gBrush, pieRect, startAngle, sweepAngle)
                startAngle += sweepAngle
            Next
        Next


        g.Dispose()
        gBrush.Dispose()
        Return bMap
    End Function
    Private Overloads Sub DrawPieChart(ByVal PieChart As PieChart, ByVal retStream As Stream)
        Dim b As System.Drawing.Bitmap = DrawChart(PieChart)
        b.Save(retStream, ImageFormat.Jpeg)
        b.Dispose()
    End Sub

    Private Overloads Function DrawBarChart(ByVal BarChart As BarChart) As System.Drawing.Image

        If BarChart.AutoSize Then
            BarChart.ImageSize = AutoSize(BarChart, BarChart.BarSliceCollection)
        End If

        bMap = GetBitmap(BarChart.ImageSize)
        Dim g As Graphics = Graphics.FromImage(bMap)
        g.SmoothingMode = SmoothingMode.HighSpeed ' antialias objects  
        g.Clear(BarChart.Color)  ' blank the image  

        BarChart.GraphRect = CalcGraph(BarChart)

        DrawGrid(BarChart, g)
        g.SmoothingMode = SmoothingMode.AntiAlias ' antialias objects  

        DrawVerticalBar(BarChart, g)

        Select Case BarChart.Alignment
            Case Base.b_BarTypes.HorizontalLeft
                bMap.RotateFlip(RotateFlipType.Rotate270FlipXY)
            Case Base.b_BarTypes.HorizontalRight
                bMap.RotateFlip(RotateFlipType.Rotate90FlipX)
            Case Base.b_BarTypes.VerticalBottom
            Case Base.b_BarTypes.VerticalTop
                bMap.RotateFlip(RotateFlipType.Rotate180FlipX)
        End Select

        Return bMap
    End Function
    Private Overloads Function DrawBarChart(ByVal BarChart As BarChart, ByVal retStream As Stream) As Object
        Dim b As System.Drawing.Bitmap = DrawChart(BarChart)
        b.Save(retStream, ImageFormat.Jpeg)
        b.Dispose()
        Return Nothing
    End Function

    Private Overloads Function DrawLineChart(ByVal LineChart As LineChart) As System.Drawing.Image
        If LineChart.AutoSize Then
            LineChart.ImageSize = AutoSize(LineChart, LineChart.LinePlotCollection)
        End If

        bMap = GetBitmap(LineChart.ImageSize)
        Dim g As Graphics = Graphics.FromImage(bMap)
        g.SmoothingMode = SmoothingMode.HighQuality ' antialias objects  
        g.Clear(LineChart.Color)  ' blank the image  

        LineChart.GraphRect = CalcGraph(LineChart)

        DrawVerticalPlots(LineChart, g)

        If LineChart.Alignment = Base.b_LineTypes.Horizontal Then
            bMap.RotateFlip(RotateFlipType.Rotate270FlipNone)
        End If


        Return bMap
    End Function
    Private Overloads Function DrawLineChart(ByVal LineChart As LineChart, ByVal retStream As Stream) As Object
        Dim b As System.Drawing.Bitmap = DrawChart(LineChart)
        b.Save(retStream, ImageFormat.Jpeg)
        b.Dispose()
        Return Nothing
    End Function

    Private Sub DrawGrid(ByVal chart As Base.BaseGraph, ByRef g As Graphics)
        Dim gBrush As New SolidBrush(Color.LightGray)
        Dim gRect As Rectangle = CalcGridRect(chart)
        Dim gpen As New Pen(Color.LightGray)
        Dim mInfo As Base.MathInfo = Nothing
        Dim topValue As Long
        Dim steps As Single = 0.0
        Dim tStep As Integer = 15

        g.DrawRectangle(gpen, gRect)
        If chart.ChartType = Base.b_ChartType.Bar Then
            topValue = CType(chart, BarChart).BarSliceCollection.MaxValue
            mInfo = getMathInfo(topValue)
        Else
            topValue = CType(chart, LineChart).LinePlotCollection.MaxValue
            mInfo = getMathInfo(topValue)
        End If


        steps = (gRect.Height / tStep)

        Dim aX As Single = gRect.X 'Starting Vertical
        Dim aY As Single = gRect.Y 'Starting Horizontal
        Dim bX As Single = gRect.Right 'Ending Vertical
        Dim bY As Single = 0 'Ending Horizontal
        'Dim sLineValue As String
        Dim Line_Value As Long = (topValue / tStep)
        Dim iCount As Long

        For iChart As Double = aY To (gRect.Height + aY) Step steps

            aY = iChart
            bY = iChart

            If iCount Mod 2 = 0 Then
                gpen = New Pen(Color.LightGray, 1)
            Else
                gpen = New Pen(Color.LightGray, 1)
            End If
            'sLineValue = Math.Round(topValue / mInfo.Divisor).ToString

            'g.DrawString(sLineValue, New Font("ariel", 5, FontStyle.Regular), gBrush, gRect.Left - 20, aY - 5)

            g.DrawLine(gpen, aX, aY, bX, bY)
            iCount += 1
            topValue -= Line_Value
            If iCount > 5000 Then Exit For
        Next
        'gBrush.Color = Color.Red
        'Dim strFormat As New StringFormat(StringFormatFlags.DirectionVertical)


        'g.DrawString("Scale (x" & mInfo.Multiplier & ")", New Font("Tahoma", 8, FontStyle.Regular), gBrush, gRect.Right - 2, gRect.Bottom - 70, strFormat)




    End Sub
    Private Sub DrawValues(ByVal chart As Base.BaseGraph, ByRef g As Graphics)
        Select Case chart.GraphAlign
            Case Base.b_BarTypes.HorizontalLeft

            Case Base.b_BarTypes.HorizontalRight

            Case Base.b_BarTypes.VerticalBottom

            Case Base.b_BarTypes.VerticalTop

        End Select
    End Sub
    Private Sub DrawVerticalBar(ByVal BarChart As BarChart, ByRef g As Graphics)
        Dim bSlice As BarSlice
        'Dim gPen As New Pen(Color.DimGray, 1)
        'LinearGradientBrush(rPrimary, getLightColor(ColorPrimary, 55), getDarkColor(ColorPrimary, 55), LinearGradientMode.ForwardDiagonal)
        Dim gBrush As New SolidBrush(BarChart.Color)
        Dim totalSpacing As Integer = BarChart.GraphRect.Width - (GraphPadding.CellSpacing * (BarChart.BarSliceCollection.Count - 1))
        totalSpacing -= (GraphPadding.Border * 2)
        Dim Width As Integer = (totalSpacing / BarChart.BarSliceCollection.Count)
        Dim Height As Integer
        Dim bX As Single = BarChart.GraphRect.Left + (GraphPadding.Border)
        Dim bY As Single = BarChart.GraphRect.Bottom
        Dim lbarRectangle As Rectangle
        Dim rbarRectangle As Rectangle
        Dim barRectangle As Rectangle
        Dim shadowRect As Rectangle

        For Each bSlice In BarChart.BarSliceCollection
            Height = (BarChart.GraphRect.Height * (bSlice.Value / BarChart.BarSliceCollection.MaxValue))
            bY = ((BarChart.GraphRect.Bottom) - Height)

            barRectangle = New Rectangle(bX, bY, Width - 2, Height)
            lbarRectangle = New Rectangle(bX, bY, (Width / 2), Height)
            rbarRectangle = New Rectangle(bX + (Width / 2) - 2, bY, Width / 2, Height)

            Dim lgBrush As New LinearGradientBrush(lbarRectangle, getLightColor(bSlice.Color, 80), getDarkColor(bSlice.Color, 55), LinearGradientMode.BackwardDiagonal)
            Dim rgBrush As New LinearGradientBrush(rbarRectangle, getLightColor(bSlice.Color, 80), getDarkColor(bSlice.Color, 55), LinearGradientMode.ForwardDiagonal)
            'Shadow Rectangle , could be a switch
            shadowRect = New Rectangle(bX + 2, bY - 3, Width, Height + 3)
            'Shadow color
            gBrush.Color = Color.LightGray
            'Draw Shadow
            g.FillRectangle(gBrush, shadowRect)
            'Kill the rect
            shadowRect = Nothing
            'Get the bar color
            'Draw the bar.
            g.FillRectangle(lgBrush, lbarRectangle)
            g.FillRectangle(rgBrush, rbarRectangle)
            'Outline the bar, could be a switch.
            Dim gPen As New Pen(getDarkColor(bSlice.Color, 1), 1)

            g.DrawRectangle(gPen, barRectangle)
            'Get the next x coord.
            bX += (Width + GraphPadding.CellPadding)
        Next
        gBrush.Dispose()
    End Sub
    Private Sub DrawVerticalPlots(ByVal LineChart As LineChart, ByRef g As Graphics)
        Dim LinePlot As Line.LineSlice
        Dim LinePointCurr As Point = Nothing
        Dim LinePointLast As Point = Nothing
        Dim LastDotColor As Color

        Dim WorkingAreaHeight As Integer
        Dim WorkingAreaWidth As Integer
        Dim bX As Integer = 0
        Dim bY As Integer = GraphPadding.Border
        Dim gBrush As New SolidBrush(Color.Blue)
        Dim gPen As New Pen(LineChart.LineColor, 1)

        Dim PlotDotSize As Integer = 8
        WorkingAreaHeight = (LineChart.GraphRect.Height - (GraphPadding.Border * 2)) - PlotDotSize
        WorkingAreaWidth = (LineChart.GraphRect.Width - (GraphPadding.Border * 2)) - PlotDotSize

        Dim vertSpacing As Integer = WorkingAreaHeight / LineChart.LinePlotCollection.Count
        Dim firstPass As Boolean = True
        bY = (vertSpacing / 2) + GraphPadding.Border


        For Each LinePlot In LineChart.LinePlotCollection
            bX = WorkingAreaWidth * Math.Round((LinePlot.Value / LineChart.LinePlotCollection.MaxValue), 2)
            'Draw the colored dot...
            LinePointCurr = New Point(bX, bY)

            If firstPass Then
                firstPass = False
            Else
                'Draw a line from the last plot.
                gBrush.Color = LastDotColor
                g.DrawLine(gPen, LinePointAdd(LinePointLast, PlotDotSize), LinePointAdd(LinePointCurr, PlotDotSize))
                g.FillEllipse(gBrush, LinePointLast.X, LinePointLast.Y, PlotDotSize, PlotDotSize)
            End If
            LinePointLast = LinePointCurr
            gBrush.Color = LinePlot.Color
            g.FillEllipse(gBrush, LinePointLast.X, LinePointLast.Y, PlotDotSize, PlotDotSize)
            'Highlight the dot
            g.DrawEllipse(gPen, LinePointLast.X, LinePointLast.Y, PlotDotSize, PlotDotSize)
            LastDotColor = LinePlot.Color

            bY += vertSpacing
        Next


    End Sub

    Private Function LinePointAdd(ByVal LinePoint As Point, ByVal PlotDotSize As Integer) As Point
        Return New Point(LinePoint.X + (PlotDotSize / 2), LinePoint.Y + (PlotDotSize / 2))
    End Function

    Private Function GetBitmap(ByVal bmSize As Size) As System.Drawing.Bitmap
        Return New System.Drawing.Bitmap(bmSize.Width, bmSize.Height, PixelFormat.Format24bppRgb)
    End Function
    Private Function AutoSize(ByVal Graph As Base.BaseGraph, ByVal ChunkCollection As Base.BaseChunkCollection) As Size
        Dim lheight, lwidth As Integer

        Select Case Graph.ChartType
            Case Base.b_ChartType.Pie
                Return New Size(300, 300)
            Case Base.b_ChartType.Bar, Base.b_ChartType.Line
                If Graph.GraphAlign = Base.b_BarTypes.HorizontalLeft Or Graph.GraphAlign = Base.b_BarTypes.HorizontalRight Then
                    lwidth = ChunkCollection.Count * 55
                    lheight = (lwidth * 0.75)
                Else
                    lheight = ChunkCollection.Count * 55
                    lwidth = (lheight * 0.75)
                End If
        End Select
    End Function
    Private Function CalcGraph(ByVal Graph As Base.BaseGraph) As Rectangle

        'Calculation code
        'Return New Rectangle(1, 10, 250, 200)
        Dim lBorder As Integer = GraphPadding.Border * 2.8
        Dim retRect As New Rectangle()
        retRect.X = lBorder
        retRect.Y = lBorder
        retRect.Width = Graph.ImageSize.Width - (lBorder * 2)
        retRect.Height = Graph.ImageSize.Height - (lBorder * 2)

        Return retRect
    End Function
    Private Function CalcGridRect(ByVal Graph As Base.BaseGraph) As Rectangle
        Dim oRect As Rectangle = CalcGraph(Graph)
        Return New Rectangle(oRect.X, oRect.Y, oRect.Width + 3, oRect.Height)
    End Function

    Private Function getDarkColor(ByVal c As Color, ByVal d As Byte) As Color
        Dim r As Byte = 0
        Dim g As Byte = 0
        Dim b As Byte = 0

        If (c.R > d) Then r = (c.R - d)
        If (c.G > d) Then g = (c.G - d)
        If (c.B > d) Then b = (c.B - d)

        Dim c1 As Color = Color.FromArgb(r, g, b)
        Return c1
    End Function
    Private Function getLightColor(ByVal c As Color, ByVal d As Byte) As Color
        Dim r As Byte = 255
        Dim g As Byte = 255
        Dim b As Byte = 255

        If (CInt(c.R) + CInt(d) <= 255) Then r = (c.R + d)
        If (CInt(c.G) + CInt(d) <= 255) Then g = (c.G + d)
        If (CInt(c.B) + CInt(d) <= 255) Then b = (c.B + d)

        Dim c2 As Color = Color.FromArgb(r, g, b)
        Return c2
    End Function

    Protected Function getMathInfo(ByRef maxValue As Long) As Base.MathInfo
        Dim retInfo As New Base.MathInfo

        With retInfo
            Select Case maxValue
                Case 0 To 99
                    .Divisor = 1
                    .Multiplier = "1"
                    .Max += 5
                    .Max = Math.Round(.Max / 10) * 10
                Case 100 To 999
                    .Divisor = 10
                    .Multiplier = "10"
                    .Max += 50
                    .Max = Math.Round(.Max / 100) * 100
                Case 1000 To 9999
                    .Divisor = 100
                    .Multiplier = "100"
                    .Max += 500
                    .Max = Math.Round(.Max / 1000) * 1000
                Case 10000 To 99999
                    .Divisor = 1000
                    .Multiplier = "1K"
                    .Max += 5000
                    .Max = Math.Round(.Max / 10000) * 10000
                Case 100000 To 999999
                    .Divisor = 10000
                    .Multiplier = "10K"
                    .Max += 50000
                    .Max = Math.Round(.Max / 100000) * 100000
                Case 1000000 To 9999999
                    .Divisor = 100000
                    .Multiplier = "100K"
                    .Max += 500000
                    .Max = Math.Round(.Max / 1000000) * 1000000
                Case 10000000 To 99999999
                    .Divisor = 1000000
                    .Multiplier = "1M"
                    .Max += 5000000
                    .Max = Math.Round(.Max / 10000000) * 10000000
                Case 100000000 To 999999999
                    .Divisor = 10000000
                    .Multiplier = "10M"
                    .Max += 50000000
                    .Max = Math.Round(.Max / 100000000) * 100000000
                Case 1000000000 To 9999999999
                    .Divisor = 100000000
                    .Multiplier = "100M"
                    .Max += 500000000
                    .Max = Math.Round(.Max / 1000000000) * 1000000000
                Case 10000000000 To 99999999999
                    .Divisor = 1000000000
                    .Multiplier = "1B"
                    .Max += 5000000000
                    .Max = Math.Round(.Max / 10000000000) * 10000000000
                Case 100000000000 To 999999999999
                    .Divisor = 10000000000
                    .Multiplier = "10B"
                    .Max += 50000000000
                    .Max = Math.Round(.Max / 100000000000) * 100000000000
                Case 1000000000000 To 9999999999999
                    .Divisor = 100000000000
                    .Multiplier = "100B"
                    .Max += 500000000000
                    .Max = Math.Round(.Max / 1000000000000) * 1000000000000
            End Select
        End With
        Return retInfo
    End Function

#End Region

End Class
