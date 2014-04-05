Imports iTunesLib
Imports UploadersLib.OtherServices

Public Class frmLyricsViewer

    Private WithEvents miTunesApp As iTunesLib.iTunesAppClass

    Private mSong As IITFileOrCDTrack = Nothing
    Private mArtwork As Image = Nothing
    Private mSongChanged As Boolean = False
    Private mLyrics As Lyrics = Nothing

    Private Sub frmLyricsViewer_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.LyricsViewerSize = Me.Size
        My.Settings.LyricsViewerLocation = Me.Location
        Me.TopMost = My.Settings.LyricsViewerTopMost
    End Sub

    Private Sub frmLyricsViewer_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Icon = My.Forms.frmMain.Icon
        miTunesApp = New iTunesLib.iTunesAppClass
    End Sub

    Private Sub mItunesApp_OnPlayerPlayEvent(ByVal iTrack As Object) Handles miTunesApp.OnPlayerPlayEvent

        If TypeOf iTrack Is IITTrack Then
            Dim track As IITTrack = CType(iTrack, IITTrack)
            If track.Kind = ITTrackKind.ITTrackKindFile Then
                mSong = CType(track, IITFileOrCDTrack)
            End If
        End If

        mSongChanged = True

    End Sub

    Private Sub tmrSecond_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrSecond.Tick

        If mSongChanged And mSong IsNot Nothing Then

            sUpdateDesc(mSong)

        End If

        mSongChanged = False

    End Sub

    Private Sub sUpdateDesc(ByVal song As IITFileOrCDTrack)

        Me.Text = String.Format("{0} - ({2}) {3} - {1}", song.Name, song.Artist, song.Year, song.Album)

        txtName.Text = mSong.Name
        txtArtist.Text = mSong.Artist
        txtAlbum.Text = mSong.Album
        txtAlbumArtist.Text = mSong.AlbumArtist
        txtYear.Text = mSong.Year.ToString
        txtGenre.Text = mSong.Genre.Replace("&", "&&")
        If mSong.Lyrics Is Nothing Then
            mLyrics = mfGetLyrics(New cXmlTrack(song, False))
            If mLyrics.Text IsNot Nothing Then
                Dim lyrics As String = mfGetFixedLyrics(mLyrics.Text)
                If String.IsNullOrEmpty(lyrics) = False Then
                    Dim fiTrack As New IO.FileInfo(song.Location)
                    Dim wasReadOnly As Boolean = fiTrack.IsReadOnly
                    If wasReadOnly Then fiTrack.IsReadOnly = False
                    mSong.Lyrics = lyrics
                    If wasReadOnly Then fiTrack.IsReadOnly = True
                End If
            End If
        End If
        btnEditLyrics.Enabled = mLyrics IsNot Nothing
        txtLyrics.Text = mSong.Lyrics
        If mSong.Artwork.Count > 0 Then
            pbArtwork.Image = mfImageFromFile(mSong.Location)
        Else
            pbArtwork.Image = Nothing
        End If

    End Sub

    Public Sub New(ByVal song As IITFileOrCDTrack)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Size = My.Settings.LyricsViewerSize
        Me.Location = My.Settings.LyricsViewerLocation

        Me.mSong = song
        sUpdateDesc(song)

    End Sub

    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Size = My.Settings.LyricsViewerSize
        Me.Location = My.Settings.LyricsViewerLocation
    End Sub

    Private Sub frmLyricsViewer_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize

        If Me.Height < 600 Then
            tlpMain.RowStyles(0).Height = 0.1!
        Else
            tlpMain.RowStyles(0).Height = 206
        End If

    End Sub

    Private Sub tlpDesc_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles tlpDesc.Paint

    End Sub

    Private Sub AlwaysOnTopToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmsTopMost.Click
        cmsTopMost.Checked = Not cmsTopMost.Checked
        Me.TopMost = cmsTopMost.Checked
    End Sub

    Private Sub btnEditLyrics_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEditLyrics.Click

        If Not mLyrics Is Nothing Then
            Dim lf As New Lyricsfly(Application.ProductName, Application.ProductName)
            lf.EditLyrics(mLyrics)
        End If

    End Sub
End Class