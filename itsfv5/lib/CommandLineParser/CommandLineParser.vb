Option Strict On
Public Enum CommandTypeEnum
    ' a file specification, such as "*.txt"
    Filespec = 1

    ' a short date string, e.g. 08/12/2002
    ShortDateString = 2

    ' a long date string, e.g. 08/12/2002 12:00:01 AM 
    LongDateString = 3

    ' any string value
    Value = 4

    ' text validated with a regular expression
    RegExpression = 5

    ' a value treated as an single or 
    ' multiple character option that must 
    ' be preceded by "/" or "-"
    Flag = 6

    ' a file that must already exist
    ExistingFile = 7

    ' a folder that must already exist
    ExistingFolder = 8
End Enum
Public Class CommandLineParser
    Private mCommandLine As String = String.Empty
    Private mEntries As CommandLineEntryCollection
    Private mBadTokens As UnmatchedTokens = _
       New UnmatchedTokens()
    Private mIsValid As Boolean = False
    Private mErrors As CommandLineParserErrors = _
       New CommandLineParserErrors()

    Public Sub New()
        ' allow creation with no parameters
        mEntries = New CommandLineEntryCollection(Me)
    End Sub

    Public Sub New(ByVal commandline As String)
        ' create parser with existing command line
        mEntries = New CommandLineEntryCollection(Me)
        Me.CommandLine = commandline
    End Sub

    Public Property Entries() As  _
       CommandLineEntryCollection
        Get
            Return mEntries
        End Get
        Set(ByVal Value As CommandLineEntryCollection)
            mEntries = Value
        End Set
    End Property

    Public Function CreateEntry(ByVal EntryType As CommandTypeEnum) As CommandLineEntry
        Return New CommandLineEntry(EntryType, Me)
    End Function

    Public Function CreateEntry(ByVal EntryType As CommandTypeEnum, ByVal aCompareValue As String) As CommandLineEntry
        Return New CommandLineEntry(EntryType, Me, aCompareValue)
    End Function

    Public ReadOnly Property Errors() As  _
       CommandLineParserErrors
        Get
            Return mErrors
        End Get
    End Property

    Public ReadOnly Property IsValid() As Boolean
        Get
            Return mIsValid
        End Get
    End Property

    Friend Sub SetValid(ByVal b As Boolean)
        mIsValid = b
    End Sub

    Public Property CommandLine() As String
        Get
            Return mCommandLine
        End Get
        Set(ByVal Value As String)
            mCommandLine = Value
        End Set
    End Property

    Public ReadOnly Property UnmatchedTokens() _
       As UnmatchedTokens
        Get
            Return mBadTokens
        End Get
    End Property


    Private Sub SetUnmatchedTokens(ByVal badtokens _
       As UnmatchedTokens)
        ' implemented as a method rather than a property 
        ' because of the lack of scope differential in 
        ' set/get properties
        mBadTokens = badtokens
    End Sub

    Private Function getFirstIndexOfCommandType( _
       ByVal aCommandType As CommandTypeEnum) _
       As Integer
        Dim anEntry As CommandLineEntry
        Dim i As Integer
        For i = 0 To mEntries.Count - 1
            anEntry = mEntries.Item(i)
            If anEntry.CommandType = aCommandType Then
                Return i
            End If
        Next
    End Function

    Private Function getLastIndexOfCommandType( _
       ByVal aCommandType As CommandTypeEnum) _
       As Integer
        Dim anEntry As CommandLineEntry
        Dim i As Integer
        For i = mEntries.Count - 1 To 0 Step -1
            anEntry = mEntries.Item(i)
            If anEntry.CommandType = aCommandType Then
                Return i
            End If
        Next
    End Function

    Public Sub PerformFinalValidation()
        Dim lastEntry As CommandLineEntry = Nothing
        Dim nextEntry As CommandLineEntry = Nothing
        Dim anEntry As CommandLineEntry = Nothing
        Dim i As Integer = 0
        Try
            For Each anEntry In mEntries
                i += 1
                If i < mEntries.Count - 1 Then
                    nextEntry = mEntries.Item(i + 1)
                End If
                If anEntry.Required And _
                   (anEntry.HasValue = False) Then
                    anEntry.setValid(False)
                    Me.Errors.Add("A required entry (" & mEntries.IndexOf(anEntry) & ", " & anEntry.CommandType.ToString & ") has no matching value.")
                    Me.SetValid(False)
                End If
                If anEntry.RequiredPosition > 0 Then
                    If anEntry.RequiredPosition <> i Then
                        anEntry.setValid(False)
                        Me.Errors.Add("The entry (" & mEntries.IndexOf(anEntry) & " with the RequiredPosition property " & anEntry.RequiredPosition.ToString & " is not in the correct position.")
                        Me.SetValid(False)
                    End If
                End If
                If anEntry.MustFollow > 0 And _
                   (Not lastEntry Is Nothing) Then
                    If anEntry.MustFollow <> _
                       lastEntry.CommandType Then
                        anEntry.setValid(False)
                        Me.Errors.Add("The entry (" & mEntries.IndexOf(anEntry) & " marked MustFollow, does not follow the correct type.")
                        Me.SetValid(False)
                    End If
                End If
                If Not anEntry.MustFollowEntry Is _
                   Nothing And (Not lastEntry Is _
                   Nothing) Then
                    If Not anEntry.MustFollowEntry Is _
                       lastEntry Then
                        anEntry.setValid(False)
                        Me.Errors.Add("The entry (" & mEntries.IndexOf(anEntry) & " marked MustFollowEntry, does not follow the specified entry.")
                        Me.SetValid(False)
                    End If
                End If
                If anEntry.MustPrecede > 0 And _
                   (Not nextEntry Is Nothing) Then
                    If anEntry.MustPrecede <> _
                       nextEntry.CommandType Then
                        anEntry.setValid(False)
                        Me.Errors.Add("The entry (" & mEntries.IndexOf(anEntry) & " marked MustPrecede, does not precede the correct type.")
                        Me.SetValid(False)
                    End If
                End If
                If Not anEntry.MustPrecedeEntry Is _
                   Nothing And (Not nextEntry Is _
                   Nothing) Then
                    If Not anEntry.MustPrecedeEntry _
                       Is nextEntry Then
                        anEntry.setValid(False)
                        Me.Errors.Add("The entry (" & mEntries.IndexOf(anEntry) & " marked MustPrecedeEntry, does not precede the specified entry.")
                        Me.SetValid(False)
                    End If
                End If
                If anEntry.MustAppearAfter > 0 Then
                    If mEntries.IndexOf(anEntry) < _
                       getFirstIndexOfCommandType _
                      (anEntry.MustAppearAfter) Then
                        anEntry.setValid(False)
                        Me.Errors.Add("The entry (" & mEntries.IndexOf(anEntry) & " marked MustAppearAfter, does not appear after the specified type.")
                        Me.SetValid(False)
                    End If
                End If
                If anEntry.MustAppearBefore > 0 Then
                    If mEntries.IndexOf(anEntry) > _
                       getLastIndexOfCommandType _
                       (anEntry.MustAppearAfter) Then
                        anEntry.setValid(False)
                        Me.Errors.Add("The entry (" & mEntries.IndexOf(anEntry) & " marked MustAppearBefore, does not appear before the specified type.")
                        Me.SetValid(False)
                    End If
                End If
                lastEntry = anEntry
            Next
        Catch ex As Exception
            Me.SetValid(False)
            Throw New ApplicationException(ex.Message + _
               "  Error in PerformFinalValidation.")
        End Try
        ' the parse is valid if no errors have occurred
        If Me.Errors.Count = 0 Then
            Me.SetValid(True)
        End If
    End Sub
    Public Function Parse() As Boolean
        Dim tk As New Tokenizer()
        Dim tkAssigner As New TokenAssigner(Me)
        Dim tokens As TokenCollection
        Dim anEntry As CommandLineEntry
        If Me.CommandLine Is String.Empty Then
            If mEntries Is Nothing Then
                SetValid(True)
            Else
                For Each anEntry In Me.Entries
                    If anEntry.Required = True Then
                        Me.Errors.Add("The parse failed " _
                        & "because the command line is " _
                        & "an empty string, but required " _
                        & "entries were set.")
                        SetValid(False)
                        Exit For
                    End If
                Next
                ' if you get here, there are no required entries
                SetValid(True)
            End If
        Else
            Try
                ' obtain a set of tokens by parsing the command line
                tokens = tk.Tokenize(Me.CommandLine)
                ' AssignTokens returns an UnmatchedTokens collection 
                ' assigned to the local mBadTokens field via the 
                ' SetUnmatchedTokens(method)
                SetUnmatchedTokens(tkAssigner.AssignTokens( _
                   tokens, Me.Entries))

                ' check to ensure items are in their proper order 
                ' and position, if set
                Me.PerformFinalValidation()
            Catch ex As Exception
                Me.Errors.Add(ex.Message)
                Throw ex
            End Try
        End If
        ' return the final result
        Return Me.IsValid
    End Function
End Class
