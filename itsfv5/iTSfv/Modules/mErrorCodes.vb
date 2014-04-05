Public Class mErrors

    Private mErrorCodes As New Hashtable

    Public ReadOnly Property ErrorTable() As Hashtable

        Get
            Return mErrorCodes
        End Get

    End Property


    Public Sub New()

        mErrorCodes.Add("000001", "Error loading iTunes")
        mErrorCodes.Add("000002", "Error saving Artwork. Line 1120.")
        mErrorCodes.Add("000003", "Error getting iTunes downloaded artwork. Line 1313.")
        mErrorCodes.Add("000004", "Error getting Artwork from file. Line 1345.")

    End Sub


End Class
