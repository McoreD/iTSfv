Public Class frmAddNewFiles

    Private mResult As Windows.Forms.DialogResult
    Private mTaglibFile As TagLib.File

    Public ReadOnly Property ForceTags() As Boolean
        Get
            Return chkOverwriteTags.Checked
        End Get
    End Property

    Public Overloads ReadOnly Property DialogResult() As Windows.Forms.DialogResult
        Get
            Return mResult
        End Get
    End Property

    Private Sub btnYes_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnYes.Click
        mResult = Windows.Forms.DialogResult.Yes
        'Console.Writeline(chkOverwriteTags.Checked)
        Me.Close()
    End Sub

    Private Sub btnNo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNo.Click
        mResult = Windows.Forms.DialogResult.No
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        mResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub frmAddNewFiles_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed

        If cboAlbumArtist.Text <> "" AndAlso Not My.Settings.ArtistsDecompile.Contains(cboAlbumArtist.Text) Then
            My.Settings.ArtistsDecompile.Add(cboAlbumArtist.Text)
        End If

        If cboArtist.Text <> "" AndAlso Not My.Settings.ArtistsDecompile.Contains(cboArtist.Text) Then
            My.Settings.ArtistsDecompile.Add(cboArtist.Text)
        End If

        If cboGenre.Text <> String.Empty AndAlso Not My.Settings.Genres.Contains(cboGenre.Text) Then
            My.Settings.Genres.Add(cboGenre.Text)
        End If

    End Sub

    Private Sub frmAddNewFiles_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        For Each item As String In My.Settings.ArtistsDecompile
            If item <> String.Empty Then
                If Not cboAlbumArtist.Items.Contains(item) Then cboAlbumArtist.Items.Add(item)
                If Not cboArtist.Items.Contains(item) Then cboArtist.Items.Add(item)
            End If
        Next

        For Each item As String In My.Settings.Genres
            If item <> String.Empty Then
                If Not cboGenre.Items.Contains(item) Then cboGenre.Items.Add(item)
            End If
        Next

    End Sub

    Private Sub cboAlbumArtist_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles cboAlbumArtist.KeyUp
        sAutoCompleteCombo_KeyUp(cboAlbumArtist, e)
    End Sub

    Private Sub cboAlbumArtist_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboAlbumArtist.Leave
        sAutoCompleteCombo_Leave(cboAlbumArtist)
    End Sub

    Private Sub cboAlbumArtist_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboAlbumArtist.TextChanged
        chkAlbumArtist.Checked = cboAlbumArtist.Text.Length > 0
    End Sub

    Private Sub txtAlbum_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtAlbum.TextChanged
        chkAlbum.Checked = txtAlbum.Text.Length > 0
    End Sub

    Private Sub chkOverwriteTags_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkOverwriteTags.CheckedChanged
        gbAlbumTags.Enabled = chkOverwriteTags.Checked
    End Sub

    <System.Runtime.InteropServices.DllImport("user32")> _
    Private Shared Function SetForegroundWindow(ByVal hWnd As Integer) As Boolean
    End Function

    Public Sub New(ByVal fileCount As Integer)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Text = String.Format("Are you sure you want to add {0} new files to Library?", fileCount)
        SetForegroundWindow(Me.Handle.ToInt32)

    End Sub

    Public Sub New(ByRef lTagFile As TagLib.File, ByVal fileCount As Integer)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Text = String.Format("Are you sure you want to add {0} new files to Library?", fileCount)
        cboAlbumArtist.Text = mfTagLibAlbumArtist(lTagFile.Tag.AlbumArtists)
        cboArtist.Text = lTagFile.Tag.FirstPerformer
        txtAlbum.Text = lTagFile.Tag.Album
        nudYear.Value = lTagFile.Tag.Year
        nudDiscCount.Value = lTagFile.Tag.DiscCount
        nudDiscNumber.Value = lTagFile.Tag.Disc
        cboGenre.Text = lTagFile.Tag.FirstGenre

    End Sub

    Private Sub nudDiscCount_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nudDiscCount.ValueChanged
        chkDisc.Checked = True
    End Sub

    Private Sub nudDiscNumber_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nudDiscNumber.ValueChanged
        chkDisc.Checked = True
    End Sub

    Private Sub nudYear_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles nudYear.ValueChanged
        chkYear.Checked = True
    End Sub

    Private Sub cboGenre_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboGenre.SelectedIndexChanged
        chkGenre.Checked = cboGenre.Text.Length > 0
    End Sub

    Private Sub cboGenre_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboGenre.TextChanged
        chkGenre.Checked = cboGenre.Text.Length > 0
    End Sub

    Private Sub chkAlbumArtist_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkAlbumArtist.CheckedChanged
        cboAlbumArtist.Enabled = chkAlbumArtist.Checked
    End Sub

    Private Sub chkAlbum_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkAlbum.CheckedChanged
        txtAlbum.Enabled = chkAlbum.Checked
    End Sub

    Private Sub chkYear_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkYear.CheckedChanged
        nudYear.Enabled = chkYear.Checked
    End Sub

    Private Sub chkGenre_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkGenre.CheckedChanged
        cboGenre.Enabled = chkGenre.Checked
    End Sub

    Private Sub chkDisc_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkDisc.CheckedChanged
        nudDiscNumber.Enabled = chkDisc.Checked
        nudDiscCount.Enabled = chkDisc.Checked
    End Sub

    Private Sub Label1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblOf.Click

    End Sub

    Private Sub chkArtist_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkArtist.CheckedChanged
        cboArtist.Enabled = chkArtist.Checked        
    End Sub

    Private Sub cboArtist_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboArtist.SelectedIndexChanged
        ' nothing
    End Sub

    Private Sub cboAlbumArtist_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboAlbumArtist.SelectedIndexChanged
        ' nothing
    End Sub

    Private Sub btnAutofill_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAutofill.Click
        chkOverwriteTags.Checked = True
        cboAlbumArtist.Text = txtAdviceAlbumArtist.Text
        If nudDiscCount.Value = 0 And nudDiscNumber.Value = 0 Then
            nudDiscCount.Value = 1
            nudDiscNumber.Value = 1
        End If
    End Sub

    Private Sub cboArtist_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboArtist.TextChanged
        chkArtist.Checked = True
    End Sub
End Class

