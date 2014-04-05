Option Strict On
Imports System.Collections
Public Class UnmatchedTokens
   Inherits CollectionBase
   Public Sub Add(ByVal aToken As String)
      List.Add(aToken)
   End Sub
   Public Sub Remove(ByVal aToken As String)
      list.Remove(aToken)
   End Sub
   Public ReadOnly Property Item(ByVal index As Integer) As String
      Get
         Return DirectCast(List.Item(index), String)
      End Get
   End Property
End Class
