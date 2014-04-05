Option Strict On
Imports System.Collections
Public Class CommandLineEntryCollection
   Inherits CollectionBase
   Private mEntries As New ArrayList()
   Private mParser As CommandLineParser
   Friend Sub New(ByVal aParser As CommandLineParser)
      mParser = aParser
   End Sub
   Public Function Add(ByVal anEntry As CommandLineEntry) As CommandLineEntry
      Me.List.Add(anEntry)
      Return anEntry
   End Function
   Public Property Parser() As CommandLineParser
      Get
         Return mParser
      End Get
      Set(ByVal Value As CommandLineParser)
         mParser = Value
      End Set
   End Property
   Public Sub Remove(ByVal anEntry As CommandLineEntry)
      Me.List.Remove(anEntry)
   End Sub
   Public ReadOnly Property Item(ByVal index As Integer) As CommandLineEntry
      Get
         Return DirectCast(Me.List.Item(index), CommandLineEntry)
      End Get
   End Property
   Public ReadOnly Property IndexOf(ByVal anEntry As CommandLineEntry) As Integer
      Get
         Return Me.List.IndexOf(anEntry)
      End Get
   End Property
   Public ReadOnly Property UnassignedEntries() As CommandLineEntryCollection
      Get
         Dim anEntry As CommandLineEntry
         Dim unassigned As New CommandLineEntryCollection(mParser)
         For Each anEntry In Me.List
            If anEntry.HasValue = False Then
               unassigned.Add(anEntry)
            End If
         Next
         Return unassigned
      End Get
   End Property
End Class
