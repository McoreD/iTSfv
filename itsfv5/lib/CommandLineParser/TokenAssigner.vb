Option Strict On
Public Class TokenAssigner
   Private mParser As CommandLineParser
   Public Sub New(ByVal aParser As CommandLineParser)
      mParser = aParser
   End Sub
   Public Function AssignTokens(ByVal tokens As TokenCollection, ByVal entries As CommandLineEntryCollection) As UnmatchedTokens
      tokens = tokens
      Dim i As Integer
      Dim tokenIndex As Integer
      Dim aToken As String
      Dim anEntry As CommandLineEntry
      Dim unmatched As New UnmatchedTokens()
      Dim assigned As Boolean
      For Each aToken In tokens
         If ((aToken.Substring(0, 1) = "-") OrElse (aToken.Substring(0, 1) = "/")) Then
            ' strip the flag character
            aToken = aToken.Substring(1)

            ' loop through the token characters trying to match each 
            ' with an entry that hasn't yet been assigned.
            assigned = False
            For Each anEntry In entries.UnassignedEntries
               If anEntry.CommandType = CommandTypeEnum.Flag Then
                  If anEntry.IsCaseSensitive Then
                     If anEntry.CompareValue = aToken Then
                        anEntry.Value = aToken
                        assigned = True
                     End If
                  Else
                     If anEntry.CompareValue.ToLower = aToken.ToLower Then
                        anEntry.Value = aToken
                        assigned = True
                     End If
                  End If
               End If
            Next
            ' try to make partial matches against the flag entries with this token
            If Not assigned Then
               tokenIndex = 0
               Do While tokenIndex < aToken.Length
                  assigned = False
                  For Each anEntry In entries.UnassignedEntries
                     If anEntry.HasValue = False Then
                        If anEntry.CommandType = CommandTypeEnum.Flag Then
                           If aToken.Substring(tokenIndex).Length >= anEntry.CompareValue.Length Then
                              If anEntry.IsCaseSensitive Then
                                 If aToken.Substring(tokenIndex, anEntry.CompareValue.Length) = anEntry.CompareValue Then
                                    assigned = True
                                    tokenIndex += anEntry.CompareValue.Length
                                    Exit For
                                 End If
                              Else
                                 If aToken.Substring(tokenIndex, anEntry.CompareValue.Length).ToLower() = anEntry.CompareValue.ToLower() Then
                                    anEntry.Value = aToken.Substring(tokenIndex, anEntry.CompareValue.Length)
                                    assigned = True
                                    tokenIndex += anEntry.CompareValue.Length
                                    Exit For
                                 End If
                              End If
                           End If
                        End If
                     End If
                  Next
                  If Not assigned Then
                     unmatched.Add(aToken.Substring(tokenIndex, 1))
                     tokenIndex += 1
                  End If
               Loop
            End If
         Else
            ' for non-flag type tokens, try to assign a value to one of the
            ' commandType entries
            assigned = False
            For i = 0 To entries.Count - 1
               anEntry = entries.Item(i)
               If Not anEntry.HasValue Then
                  Try
                     anEntry.Value = aToken
                     assigned = True
                     Exit For
                  Catch ex As Exception
                     If anEntry.Required = True Then
                        mParser.Errors.Add(ex.Message)
                        mParser.SetValid(False)
                     End If
                  End Try
               End If
            Next
            If Not assigned Then
               unmatched.Add(aToken)
            End If
         End If
      Next
      Return unmatched
   End Function

End Class
