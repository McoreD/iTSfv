Imports System.Text.RegularExpressions
Imports System.Text

Public Module mScripts

    Public Structure TagFunction

        Dim OrigString As String
        Dim TagString As String
        Dim CommandString As String
        Dim ArgString As String

    End Structure


    Public Function mfGetStringFromScript(ByVal pattern As String, ByVal track As cXmlTrack) As String

        Dim re1 As String = "(\$(?:[a-z]+))\((%(?:[A-Z][a-z]+)%),.*?([-+]?\d+)\)"

        Dim r As New Regex(re1, RegexOptions.IgnoreCase Or RegexOptions.Singleline)
        Dim mc As MatchCollection = r.Matches(pattern)

        Dim lFns As New List(Of TagFunction)
        For Each m As Match In mc
            If (m.Success) Then
                Dim fn As New TagFunction
                fn.OrigString = m.Groups(0).Value
                fn.CommandString = m.Groups(1).Value
                fn.TagString = m.Groups(2).Value
                fn.ArgString = m.Groups(3).Value
                lFns.Add(fn)
            End If
        Next

        For Each fn As TagFunction In lFns

            If (Not String.IsNullOrEmpty(fn.OrigString) AndAlso _
                Not String.IsNullOrEmpty(fn.TagString) AndAlso _
                Not String.IsNullOrEmpty(fn.ArgString)) Then

                If fn.CommandString.Equals("$cut") Then
                    Dim arg As Integer = CInt(fn.ArgString)
                    Dim tag As String = mfGetStringFromPattern(fn.TagString, track)
                    tag = tag.Substring(0, Math.Min(tag.Length, arg))
                    pattern = pattern.Replace(fn.OrigString, tag)
                End If

            End If

        Next

        Return pattern

    End Function

    Private Function fGetArg(ByVal fn As String) As Integer

        fn = mfGetFixedSpaces(fn)
        fn = fn.Replace(" ", "")

        Dim sp As String() = fn.Split(CChar(","))
        Dim n As Integer = 0

        If sp.Length > 1 Then
            n = CInt(sp(1).Replace(")", ""))
        End If

        Return n

    End Function

    Private Function fGetFunctionName(ByVal fn As String) As String

        Return fn.Split(CChar("("))(0)

    End Function

    Private Function fGetSyntaxName(ByVal s As String) As String

        Return s.Substring(s.IndexOf("%"), s.LastIndexOf("%") - s.IndexOf("%") + 1)

    End Function

    Private Function fGetPadding(ByVal s As String) As String

        Dim sb As New StringBuilder
        Dim n As Integer = fGetArg(s)
        For i = 1 To n
            sb.Append("0")
        Next
        Return sb.ToString

    End Function

End Module
