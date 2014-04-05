Imports System.IO
Imports iTunesLib

Public Class cArtworkSource

    ' Note: DO NOT have a Property for Image and store the image as it could cause 
    ' MASSIVE MEMORY LEAKS. AN IMAGE CAN BE AS LARGE 1 MiB SO STORING AN IMAGE FOR EACH 
    ' ALBUM MEANS THAT YOU ARE GOING TO CONSUME ABOUT 1 GiB of RAM for iTSfv WHEN YOU 
    ' VALIDATE ITUNES MUSIC LIBRARY

    Private mTrack As IITFileOrCDTrack

    Public Sub New(ByVal lArtworkPath As String)

        Me.ArtworkPath = lArtworkPath
        Me.ArtworkType = ArtworkSourceType.File
        sLoadDimensions(lArtworkPath)

    End Sub

    Private mArtworkImage As Image = Nothing
    ''' <summary>
    ''' Property to set a Artwork Image. Be sure to clear this after use to avoid memory leaks.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ArtworkImage() As Image
        Get
            Return mArtworkImage
        End Get
        Set(ByVal value As Image)
            mArtworkImage = value
        End Set
    End Property

    Public Sub New(ByVal track As IITFileOrCDTrack)
        mTrack = track
    End Sub

    Public Sub New(ByVal track As IITFileOrCDTrack, ByVal bLoadArtwork As Boolean)

        mTrack = track

        If track.Artwork.Count > 0 Then
            Try
                mArtworkPath = mfGetTempArtworkCachePath("ArtworkSource", track)
                track.Artwork(1).SaveArtworkToFile(mArtworkPath)
                If track.Artwork(1).IsDownloadedArtwork Then
                    mArtworkType = ArtworkSourceType.iTunes
                Else
                    mArtworkType = ArtworkSourceType.Track
                End If
                sLoadDimensions(mArtworkPath)
            Catch ex As Exception
                msAppendWarnings(ex.Message + " while loading Artwork for Artwork Source")
            End Try
        End If

    End Sub

    Public ReadOnly Property Track() As IITFileOrCDTrack
        Get
            Return mTrack
        End Get
    End Property

    Private mArtworkResized As Boolean = False
    Public Property ArtworkIsResized() As Boolean
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

    Private mArtworkPathResized As String = String.Empty
    Public Property ArtworkPathResized() As String
        Get
            Return mArtworkPathResized
        End Get
        Set(ByVal value As String)
            mArtworkPathResized = value
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

    Private mArtworkType As ArtworkSourceType = ArtworkSourceType.File
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

    Public Sub sLoadDimensions()
        sLoadDimensions(ArtworkPath)
    End Sub

    Private Function sLoadDimensions(ByVal fp As String) As Bitmap

        Dim mPicture As Bitmap = mfGetBitMapFromFilePath(fp)

        If mPicture IsNot Nothing Then
            Height = mPicture.Height
            Width = mPicture.Width
        End If

        Return mPicture

    End Function

    Public Function fGetPicture() As Bitmap
        Return sLoadDimensions(Me.ArtworkPath)
    End Function

End Class
