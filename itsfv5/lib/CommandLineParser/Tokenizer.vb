Option Strict On
Imports System.Text
Public Class Tokenizer
   Private Const dash As Char = "-"c
   Private Const slash As Char = "/"c
   Private Const backslash As Char = "\"c
   Public Function Tokenize(ByVal s As String) As TokenCollection
      Dim tokens As New TokenCollection()
        Dim aToken As String = String.Empty
      Dim i As Integer
      Dim inQuote As Boolean = False
      Dim sb As New StringBuilder()
      Dim aChar As String
      Dim chars() As Char
        Dim lastChar As String = String.Empty
        Dim quoteChar As String = String.Empty
      s = s.Trim()
      If s <> String.Empty Then
         chars = s.ToCharArray
         For i = 0 To chars.Length - 1
            aChar = chars(i)
            Select Case aChar
               Case """"c, "'"c
                  If quoteChar = "" Then
                     quoteChar = aChar
                     inQuote = Not inQuote
                  ElseIf quoteChar = aChar Then
                     inQuote = Not inQuote
                     quoteChar = ""
                     If sb.Length > 0 Then
                        tokens.Add(sb.ToString)
                        sb = New StringBuilder()
                     End If
                  Else
                     sb.Append(aChar)
                  End If
               Case " "c, Chr(9)
                  If Not inQuote Then
                     If sb.Length > 0 Then
                        tokens.Add(sb.ToString)
                        sb = New StringBuilder()
                     End If
                  Else
                     sb.Append(aChar)
                  End If
               Case Else
                  sb.Append(aChar)
            End Select
         Next
      End If
      If sb.Length > 0 Then
         tokens.Add(sb.ToString)
      End If
      Return tokens
   End Function
End Class
