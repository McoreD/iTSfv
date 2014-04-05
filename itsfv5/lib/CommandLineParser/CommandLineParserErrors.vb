Option Strict On
Imports System.Collections
Imports System.Collections.Specialized
Public Class CommandLineParserErrors
   Inherits CollectionBase
   Public ReadOnly Property Errors() As String()
      Get
         Dim msgs(Me.Count - 1) As String
         Me.List.CopyTo(msgs, 0)
         Return msgs
      End Get
   End Property
   Friend Sub Add(ByVal anErrorMessage As String)
      Me.List.Add(anErrorMessage)
   End Sub
   Friend Sub Remove(ByVal anErrorMessage As String)
      Me.List.Remove(anErrorMessage)
   End Sub
   Public ReadOnly Property Item(ByVal index As Integer) As String
      Get
         Return DirectCast(Me.List.Item(index), String)
      End Get
   End Property
End Class
