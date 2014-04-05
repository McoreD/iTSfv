Public Class cObjectWriter


    Private mObj As New Object
    Private mFilePath As String = ""

    Public Sub New(ByVal lObj As Object, ByVal lFilePath As String)
        mObj = lObj
        mFilePath = lFilePath
    End Sub

    Public Function WriteObjectToBF() As Boolean

        Dim suc As Boolean = True
        Try
            Dim fs As New IO.FileStream(mFilePath, IO.FileMode.Create)
            Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
            bf.Serialize(fs, mObj)
            fs.Close()
            msAppendDebug(String.Format("Wrote {0}", mFilePath))
        Catch ex As Exception
            suc = False
        End Try

        Return suc

    End Function


End Class
