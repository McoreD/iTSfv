Imports iTunesLib
Imports System.IO

<Serializable()> Public Class cXmlArtwork

    Public Sub New(ByVal track As IITTrack, ByVal booWidthHeight As Boolean)

        If booWidthHeight = True AndAlso track.Artwork.Count > 0 Then

            If track.Kind = ITTrackKind.ITTrackKindFile Then
                Try
                    Dim f As TagLib.File = TagLib.File.Create(CType(track, IITFileOrCDTrack).Location)
                    If f.Tag.Pictures.Length > 0 Then
                        Using img As Image = mTagLibJobs.mfPictureToImage(f.Tag.Pictures(0))
                            mWidth = img.Width
                            mHeight = img.Height
                        End Using
                    End If
                Catch ex As Exception
                    msAppendWarnings(ex.Message + " while cXmlTrack attempting to getting Artwork to memory")
                End Try
            End If

        End If

    End Sub

    Private mFlePath As String
    Public ReadOnly Property Location() As String
        Get
            Return mFlePath
        End Get
    End Property

    Private mHeight As Integer = 0
    Public Property Height() As Integer
        Get
            Return mHeight
        End Get
        Set(ByVal value As Integer)
            mHeight = value
        End Set
    End Property

    Private mWidth As Integer = 0
    Public Property Width() As Integer
        Get
            Return mWidth
        End Get
        Set(ByVal value As Integer)
            mWidth = value
        End Set
    End Property


End Class
