Imports System.IO
Imports System.Text.RegularExpressions
Public Class FileSpecValidator
   Public Shared Function Validate(ByVal aFileSpec As String, ByVal mustContain As String, ByVal CaseSensitive As Boolean) As Boolean
        Dim aPattern As String = String.Empty
      Dim aPath As String
        Dim aDrive As String = String.Empty
      Dim aFilename As String
      Dim d As DirectoryInfo
      Dim files() As FileInfo

      If aFileSpec.Trim = "" Then
         Return False
      End If
      Try

         ' get the path
         aPath = getPath(aFileSpec)

         ' get the filename
         aFilename = getFilename(aFileSpec)

         ' if the path is empty, but the filename is not,
         ' the user entered a path such as "c:\*.*"
         If aPath Is String.Empty And (Not aFilename Is String.Empty) Then
            d = New DirectoryInfo(aFileSpec)
         Else
            ' is it valid?
            d = New DirectoryInfo(aPath)
         End If


         ' if the filename is valid, the GetFiles method will work
         files = d.GetFiles(aFilename)
      Catch ex As Exception
         Throw New ApplicationException("The file specification """ & aFileSpec & """ refers to an invalid path, or contains invalid characters.", ex)
      End Try

      ' the mustContain parameter contains the CompareValue set on the CommandLineEntry
      If mustContain Is Nothing OrElse mustContain Is String.Empty Then
         Return True
      Else
         ' look for at least one match
         If CaseSensitive Then
            ' match case search
            If aFileSpec.IndexOf(mustContain) > 0 Then
               Return True
            End If
         Else
            ' case-insensitive search
            If aFileSpec.ToLower.IndexOf(mustContain.ToLower) > 0 Then
               Return True
            End If
         End If
         Return False
         End If
   End Function
   Private Shared Function getPath(ByVal aFileSpec As String) As String
      Try
         ' get the path string
         Return Path.GetDirectoryName(aFileSpec)
      Catch ex As Exception
         Throw New ApplicationException("The path portion of the file specification """ & aFileSpec & """ is invalid, or contains invalid characters.", ex)
      End Try
   End Function

   Private Shared Function getFilename(ByVal aFileSpec As String) As String
      Try
         Return Path.GetFileName(aFileSpec)
      Catch ex As Exception
         Throw New ApplicationException("The file name portion of the file specification """ & aFileSpec & """ is invalid.", ex)
      End Try
   End Function


End Class
