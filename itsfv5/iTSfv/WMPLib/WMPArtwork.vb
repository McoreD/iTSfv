Imports System.IO
Imports WMPLib
Imports System.Drawing

Public Class WMPArtwork
    Implements WMPLib.IWMPMetadataPicture

    Private mArtwork As WMPLib.IWMPMetadataPicture
    Private mLocation As String
    Private mPicture As Image

    Public Sub New(ByVal pic As IWMPMetadataPicture)
        mArtwork = pic
    End Sub

    Public ReadOnly Property Description() As String Implements WMPLib.IWMPMetadataPicture.Description
        Get
            Return mArtwork.Description
        End Get
    End Property

    Public ReadOnly Property MimeType() As String Implements WMPLib.IWMPMetadataPicture.mimeType
        Get
            Return mArtwork.mimeType
        End Get
    End Property

    Public ReadOnly Property PictureType() As String Implements WMPLib.IWMPMetadataPicture.pictureType
        Get
            Return mArtwork.pictureType
        End Get
    End Property

    Public ReadOnly Property Location() As String Implements WMPLib.IWMPMetadataPicture.URL

        Get

            If mLocation = String.Empty Then

                Dim url As String = mArtwork.URL
                Dim fileName As String = Path.GetFileNameWithoutExtension(url) & "[1]." & Path.GetExtension(url)
                For Each dir As String In Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache) & "\Content.IE5", "*.*", SearchOption.AllDirectories)
                    If File.Exists(dir & "\" & fileName) Then
                        mLocation = dir & "\" & fileName
                        Return mLocation
                    End If
                Next

            End If

            Return mLocation

        End Get

    End Property

    Public ReadOnly Property Picture() As Image

        Get

            If mPicture Is Nothing Then

                If File.Exists(Me.Location) Then

                    Dim fs As FileStream = File.Open(Me.Location, FileMode.Open)
                    mPicture = Image.FromStream(fs)
                    fs.Close()
                    Return mPicture

                End If

            End If

            Return mPicture

        End Get

    End Property

    Public ReadOnly Property Count() As Integer
        Get
            Return 1
        End Get
    End Property

    Public Function SaveArtworkToFile(ByVal filePath As String) As Boolean

        Try
            My.Computer.FileSystem.CopyFile(Me.Location, filePath)
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

End Class
