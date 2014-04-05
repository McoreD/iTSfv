Option Strict On
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Public Class CommandLineEntry
   Private mRequired As Boolean = False
   Private mCommandType As CommandTypeEnum
   Private mRequiredPosition As Integer
   Private mMustFollow As CommandTypeEnum
   Private mMustFollowEntry As CommandLineEntry
   Private mMustPrecede As CommandTypeEnum
   Private mMustPrecedeEntry As CommandLineEntry
   Private mMustAppearAfter As CommandTypeEnum
   Private mMustAppearBefore As CommandTypeEnum
   Private mValue As String = String.Empty
   Private mIsValid As Boolean = False
   Private mIsCaseSensitive As Boolean = False
   Private mCompareValue As String = String.Empty
   Private mParser As CommandLineParser
   Friend Sub New(ByVal aCommandType As CommandTypeEnum, ByVal parser As CommandLineParser)
      Me.CommandType = aCommandType
      mParser = parser
   End Sub
   Friend Sub New(ByVal aCommandType As CommandTypeEnum, ByVal parser As CommandLineParser, ByVal aCompareValue As String)
      Me.CommandType = aCommandType
      Me.CompareValue = aCompareValue
      mParser = parser
   End Sub
   Friend Sub New(ByVal aCommandType As CommandTypeEnum, ByVal parser As CommandLineParser, ByVal aCompareValue As String, ByVal aValue As String)
      Me.CommandType = aCommandType
      Me.CompareValue = aCompareValue
      Me.Value = aValue
      mParser = parser
   End Sub
   Public Property Required() As Boolean
      Get
         Return Me.mRequired
      End Get
      Set(ByVal Value As Boolean)
         Me.mRequired = Value
      End Set
   End Property
   Public Property CommandType() As CommandTypeEnum
      Get
         Return Me.mCommandType
      End Get
      Set(ByVal Value As CommandTypeEnum)
         Me.mCommandType = Value
      End Set
   End Property
   Public Property MustFollow() As CommandTypeEnum
      Get
         Return Me.mMustFollow
      End Get
      Set(ByVal Value As CommandTypeEnum)
         Me.mMustFollow = Value
      End Set
   End Property
   Public Property MustFollowEntry() As CommandLineEntry
      Get
         Return Me.mMustFollowEntry
      End Get
      Set(ByVal Value As CommandLineEntry)
         Me.mMustFollowEntry = Value
      End Set
   End Property
   Public Property IsCaseSensitive() As Boolean
      Get
         Return Me.mIsCaseSensitive
      End Get
      Set(ByVal Value As Boolean)
         Me.mIsCaseSensitive = Value
      End Set
   End Property
   Public Property MustPrecede() As CommandTypeEnum
      Get
         Return Me.mMustPrecede
      End Get
      Set(ByVal Value As CommandTypeEnum)
         Me.mMustPrecede = Value
      End Set
   End Property
   Public Property MustPrecedeEntry() As CommandLineEntry
      Get
         Return Me.mMustPrecedeEntry
      End Get
      Set(ByVal Value As CommandLineEntry)
         Me.mMustPrecedeEntry = Value
      End Set
   End Property
   Public Property MustAppearAfter() As CommandTypeEnum
      Get
         Return Me.mMustAppearAfter
      End Get
      Set(ByVal Value As CommandTypeEnum)
         Me.mMustAppearAfter = Value
      End Set
   End Property
   Public Property MustAppearBefore() As CommandTypeEnum
      Get
         Return Me.mMustAppearBefore
      End Get
      Set(ByVal Value As CommandTypeEnum)
         Me.mMustAppearBefore = Value
      End Set
   End Property
   Public Property RequiredPosition() As Integer
      Get
         Return Me.mRequiredPosition
      End Get
      Set(ByVal Value As Integer)
         Me.mRequiredPosition = Value
      End Set
   End Property
   Public ReadOnly Property HasValue() As Boolean
      Get
         Return Not (mValue Is String.Empty)
      End Get
   End Property
   Public Property [Value]() As String
      Get
         Return mValue
      End Get
      Set(ByVal Value As String)
         If Not validateValue(Value) Then
            Dim msg As String
            msg = "The value " & Value & " is not valid for the command type " & Me.CommandType.ToString()
            If Not Me.CompareValue Is String.Empty Then
               msg += " and the Compare Value " & Me.CompareValue & "."
            End If
            Throw New ApplicationException(msg)
         Else
            mValue = Value
         End If
      End Set
   End Property
   Public Property CompareValue() As String
      Get
         Return mCompareValue
      End Get
      Set(ByVal Value As String)
         mCompareValue = Value
      End Set
   End Property

   Private Function validateValue(ByVal aValue As String) As Boolean
        Select Case Me.CommandType
            Case CommandTypeEnum.ExistingFolder
                Dim di As New DirectoryInfo(aValue)
                If di.Exists Then
                    If Me.CompareValue = String.Empty Then
                        setValid(True)
                        Return True
                    Else
                        If Me.CompareValue.ToLower <> aValue.ToLower Then
                            setValid(False)
                            Return False
                        Else
                            setValid(True)
                            Return True
                        End If
                    End If
                Else
                    setValid(False)
                    Return False
                End If
            Case CommandTypeEnum.ExistingFile
                Dim fi As New FileInfo(aValue)
                If fi.Exists Then
                    If Me.CompareValue = String.Empty Then
                        setValid(True)
                        Return True
                    Else
                        If Me.CompareValue.ToLower <> aValue.ToLower Then
                            setValid(False)
                            Return False
                        Else
                            setValid(True)
                            Return True
                        End If
                    End If
                Else
                    setValid(False)
                    Return False
                End If
            Case CommandTypeEnum.Filespec
                ' the entered parameter must be a well-formed file name 
                Dim fsv As New FileSpecValidator()
                Try
                    If FileSpecValidator.Validate(aValue, Me.CompareValue, Me.IsCaseSensitive) Then
                        setValid(True)
                        Return True
                    Else
                        setValid(False)
                        Return False
                    End If
                Catch ex As Exception
                    mParser.Errors.Add("FileSpec validation failed. " & ex.Message)
                    If Not ex.InnerException Is Nothing Then
                        mParser.Errors.Add(ex.InnerException.Message)
                    End If
                    setValid(False)
                    Return False
                End Try
            Case CommandTypeEnum.Flag
                If String.Compare(Me.CompareValue, aValue, Me.IsCaseSensitive) = 0 Then
                    setValid(True)
                    Return True
                Else
                    setValid(False)
                    Return False
                End If
            Case CommandTypeEnum.LongDateString
                Try
                    Dim aDate As DateTime = Date.ParseExact(aValue, System.Globalization.DateTimeFormatInfo.CurrentInfo.LongDatePattern, System.Globalization.DateTimeFormatInfo.CurrentInfo)
                    setValid(True)
                    Return True
                Catch ex As Exception
                    setValid(False)
                    Return False
                End Try
            Case CommandTypeEnum.RegExpression
                Try
                    Dim regOptions As RegexOptions
                    If Me.IsCaseSensitive Then
                        regOptions = RegexOptions.IgnoreCase
                    Else
                        regOptions = RegexOptions.None
                    End If
                    Try
                        If Regex.IsMatch(aValue, Me.CompareValue, regOptions) Then
                            setValid(True)
                            Return True
                        End If
                    Catch ex As Exception
                        ' invalid value
                        setValid(False)
                        Return False
                    End Try
                Catch ex As Exception
                    ' invalid reg exp
                    Throw ex
                End Try
            Case CommandTypeEnum.ShortDateString
                Try
                    Dim aDate As DateTime = Date.ParseExact(aValue, System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortDatePattern, System.Globalization.DateTimeFormatInfo.CurrentInfo)
                    setValid(True)
                    Return True
                Catch ex As Exception
                    ' invalid date
                    setValid(False)
                    Return False
                End Try
            Case CommandTypeEnum.Value
                ' make sure there's a value
                If aValue.Length > 0 Then
                    ' if there's a comparevalue, compare it
                    ' to the value
                    If Not Me.CompareValue Is String.Empty Then
                        If Me.CompareValue.ToLower = aValue.ToLower Then
                            setValid(True)
                            Return True
                        End If
                    Else
                        ' there's no compareValue, set valid for 
                        ' any value except an empty string
                        setValid(True)
                        Return True
                    End If
                Else
                    setValid(False)
                    Return False
                End If
        End Select
   End Function
   Public ReadOnly Property IsValid() As Boolean
      Get
         Return mIsValid
      End Get
   End Property
   Friend Sub setValid(ByVal b As Boolean)
      mIsValid = b
   End Sub
   Public Overrides Function ToString() As String
        Dim sb As New StringBuilder(1000)
      Dim newLine As String = System.Environment.NewLine
      sb.Append("CommandLineEntry" & newLine)
      sb.Append("CommandType: " & System.Enum.GetName(Me.CommandType.GetType, Me.CommandType) & newLine)
      sb.Append("CompareValue: ")
      If Me.CompareValue Is String.Empty Then
         sb.Append("Not Set" & newLine)
      Else
         sb.Append(Me.CompareValue & newLine)
      End If
      sb.Append("Value: ")
      If Me.HasValue = False Then
         sb.Append("Not Set" & newLine)
      Else
         sb.Append(Me.Value & newLine)
      End If
      ' even more info, if you want it
      'sb.Append("IsCaseSensitive: " & Me.IsCaseSensitive.ToString & newLine)
      'sb.Append("Required: " & Me.Required.ToString & newLine)
      'sb.Append("RequiredPosition: " & Me.RequiredPosition.ToString & newLine)
      sb.Append("Valid: " & Me.IsValid & newLine)
      Return sb.ToString
   End Function
End Class
