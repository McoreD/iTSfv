Imports System.IO

<Serializable()> Public Class cXmlArtworkSource

    Private mTrack As cXmlTrack

    Public Sub New(ByVal track As cXmlTrack)
        mTrack = track
    End Sub

    Public ReadOnly Property Track() As cXmlTrack
        Get
            Return mTrack
        End Get
    End Property

    Private mArtworkResized As Boolean = False
    Public Property ArtworkResized() As Boolean
        Get
            Return mArtworkResized
        End Get
        Set(ByVal value As Boolean)
            mArtworkResized = value
        End Set
    End Property

    Private mArtworkPath As String = String.Empty
    Public Property ArtworkPath() As String
        Get
            Return mArtworkPath
        End Get
        Set(ByVal value As String)
            mArtworkPath = value
        End Set
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

    Private mArtworkType As ArtworkSourceType
    Public Property ArtworkType() As ArtworkSourceType
        Get
            Return mArtworkType
        End Get
        Set(ByVal value As ArtworkSourceType)
            mArtworkType = value
        End Set
    End Property

    Private mReplaceArtwork As Boolean = False
    Public Property ReplaceArtwork() As Boolean
        Get
            Return mReplaceArtwork
        End Get
        Set(ByVal value As Boolean)
            mReplaceArtwork = value
        End Set
    End Property

    Public ReadOnly Property ArtworkName() As String
        Get
            Select Case mArtworkType
                Case ArtworkSourceType.File
                    Return Path.GetFileName(mArtworkPath)
                Case Else
                    Return mArtworkType.ToString
            End Select
        End Get
    End Property


End Class
