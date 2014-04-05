Imports iTunesLib

Public Class frmAddArtwork

    Dim mArtwork As Image = Nothing
    Dim mArtworkCropped As Image = Nothing
    Dim mArtworkPath As String

    Private mDisc As cInfoDisc
    Private mResult As Windows.Forms.DialogResult = Windows.Forms.DialogResult.Cancel

    Public Overloads ReadOnly Property DialogResult() As Windows.Forms.DialogResult
        Get
            Return mResult
        End Get
    End Property

    Private Sub frmAddArtwork_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.Text = String.Format("Do you want to edit Artwork for {0}?", mDisc.AlbumName)
        For Each track As IITFileOrCDTrack In mDisc.Tracks
            lbTracks.Items.Add(String.Format("{0} {1}", track.TrackNumber.ToString("00"), track.Name))
        Next

    End Sub

    Public ReadOnly Property ArtworkPath() As String
        Get
            Return mArtworkPath
        End Get
    End Property

    Public Sub New(ByVal filePath As String, ByVal lDisc As cInfoDisc)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        mDisc = lDisc
        mArtwork = mfGetBitMapFromFilePath(filePath)

        If mArtwork Is Nothing Then
            Me.Close()
        End If

        picArtwork.Image = mArtwork
        mArtworkPath = filePath

    End Sub

    Private Sub sPreviewCrop()

        Dim width As Integer = CInt(mArtwork.Width - nudLeft.Value - nudRight.Value)
        Dim height As Integer = CInt(mArtwork.Height - nudTop.Value - nudBottom.Value)
        Dim startX As Integer = CInt(nudLeft.Value)
        Dim startY As Integer = CInt(nudTop.Value)

        If width > 0 AndAlso height > 0 Then
            mArtworkCropped = fCrop(mArtwork, startX, startY, width, height)
            picArtwork.Image = mArtworkCropped
        End If

    End Sub


    Private Sub btnYes_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnYes.Click

        sPreviewCrop()
        If mArtworkCropped IsNot Nothing Then
            mArtworkCropped.Save(mArtworkPath)
        End If

        mResult = Windows.Forms.DialogResult.Yes
        Me.Close()

    End Sub

    Public Function fCrop(ByVal _inputImage As Image, ByVal startX As Integer, ByVal startY As Integer, ByVal Height As Integer, ByVal Width As Integer) As Image

        Dim bmpImage As Bitmap
        Dim recCrop As Rectangle
        Dim bmpCrop As Bitmap = Nothing
        Dim gphCrop As Graphics
        Dim recDest As Rectangle

        Try
            bmpImage = New Bitmap(_inputImage)
            recCrop = New Rectangle(startX, startY, Width, Height)
            bmpCrop = New Bitmap(recCrop.Width, recCrop.Height, bmpImage.PixelFormat)
            gphCrop = Graphics.FromImage(bmpCrop)
            recDest = New Rectangle(0, 0, Width, Height)
            gphCrop.DrawImage(bmpImage, recDest, recCrop.X, recCrop.Y, recCrop.Width, _
              recCrop.Height, GraphicsUnit.Pixel)
        Catch er As Exception
            'Throw er
        End Try

        Return bmpCrop

    End Function

    Private Sub btnPreview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPreview.Click
        sPreviewCrop()
    End Sub

    Private Sub btnNo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNo.Click
        mResult = Windows.Forms.DialogResult.No
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        mResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub
End Class