Imports System.Windows.Forms

Public Class dlgArtworkList

    Dim WithEvents mRadioBtn As New RadioButton
    Dim mArtworkSrc As cArtworkSource
    Dim mArtworkSrcs As New List(Of cArtworkSource)

    Public ReadOnly Property Artwork() As cArtworkSource
        Get
            Return mArtworkSrc
        End Get
    End Property

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Public Sub New(ByVal lst As List(Of cArtworkSource))

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        mArtworkSrcs = lst

        For i As Integer = 0 To lst.Count - 1

            Dim src As cArtworkSource = lst(i)

            mRadioBtn = New RadioButton
            mRadioBtn.Text = String.Format("{0}x{1} - ({2})", src.Width, src.Height, src.ArtworkType.ToString)

            mRadioBtn.Location = New Point(10 + 120 * i, 10)
            mRadioBtn.AutoSize = True
            mRadioBtn.Tag = i
            ttApp.SetToolTip(mRadioBtn, src.ArtworkPath)

            pArtwork.Controls.Add(mRadioBtn)
            AddHandler mRadioBtn.CheckedChanged, AddressOf mRadioBtn_CheckedChanged

            Dim pb As New PictureBox
            pb.BorderStyle = BorderStyle.FixedSingle
            pb.Image = src.fGetPicture
            pb.Size = New Drawing.Size(100, 100)
            pb.SizeMode = PictureBoxSizeMode.StretchImage
            pb.Location = New Point(10 + 120 * i, 35)
            ttApp.SetToolTip(pb, IO.Path.GetFileName(src.ArtworkPath))
            AddHandler pb.DoubleClick, AddressOf pbArtwork_DoubleClicked
            pb.Tag = src.ArtworkPath
            pArtwork.Controls.Add(pb)

            Dim lbl As New Label
            lbl.Text = IO.Path.GetFileName(src.ArtworkPath)
            lbl.Location = New Point(10 + 120 * i, 150)
            ttApp.SetToolTip(lbl, lbl.Text)
            pArtwork.Controls.Add(lbl)

        Next

        ' choose the last one 
        mRadioBtn.Checked = True

        ' Remove automatically added Handler
        RemoveHandler mRadioBtn.CheckedChanged, AddressOf mRadioBtn_CheckedChanged

    End Sub

    Private Sub pbArtwork_DoubleClicked(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim pbArtwork As PictureBox = CType(sender, PictureBox)
        Try
            Process.Start(pbArtwork.Tag.ToString)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub mRadioBtn_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles mRadioBtn.CheckedChanged

        Dim lRadioBtn As RadioButton = CType(sender, RadioButton)
        If lRadioBtn.Checked Then
            mArtworkSrc = mArtworkSrcs(CInt(lRadioBtn.Tag))
        End If

    End Sub

    Private Sub dlgArtworkList_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Icon = My.Forms.frmMain.Icon
    End Sub
End Class
