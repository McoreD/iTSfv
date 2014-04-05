Option Strict On
Imports System.Collections
Public Class TokenCollection
   Inherits CollectionBase
   Public Sub Add(ByVal aToken As String)
      Me.List.Add(aToken)
   End Sub
   Public ReadOnly Property Item(ByVal index As Integer) As String
      Get
         Return DirectCast(Me.List.Item(index), String)
      End Get
   End Property
   Public Sub Remove(ByVal aToken As String)
      Me.List.Remove(aToken)
   End Sub
End Class
