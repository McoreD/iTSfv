
<Serializable()> Public Class cXmlTrack

    ' index is the same value the LibraryPlaylist.Item() would have
    ' usage: 
    ' Dim xt As cXmlTrack = mXmlLibParser.TrackCollection(i) where i starts from 0
    ' Dim track As IITFileOrCDTrack = CType(mMainLibraryTracks.Item(xt.Index), IITFileOrCDTrack)
    Private mIndex As Integer = 0
    Public Property Index() As Integer
        Get
            Return mIndex
        End Get
        Set(ByVal value As Integer)
            mIndex = value
        End Set
    End Property

    Private mTrackID As Integer = 0
    Public Property TrackID() As Integer
        Get
            Return mTrackID
        End Get
        Set(ByVal value As Integer)
            mTrackID = value
        End Set
    End Property

    Private mTrackNumber As Integer = 0
    Public Property TrackNumber() As Integer
        Get
            Return mTrackNumber
        End Get
        Set(ByVal value As Integer)
            mTrackNumber = value
        End Set
    End Property

    Private mName As String = String.Empty
    Public Property Name() As String
        Get
            Return mName
        End Get
        Set(ByVal value As String)
            mName = value
        End Set
    End Property

    Private mComposer As String = String.Empty
    Public Property Composer() As String
        Get
            Return mComposer
        End Get
        Set(ByVal value As String)
            mComposer = value
        End Set
    End Property

    Private mCompilation As Boolean = False
    Public Property Compilation() As Boolean
        Get
            Return mCompilation
        End Get
        Set(ByVal value As Boolean)
            mCompilation = value
        End Set
    End Property

    Private mGrouping As String = String.Empty
    Public Property Grouping() As String
        Get
            Return mGrouping
        End Get
        Set(ByVal value As String)
            mGrouping = value
        End Set
    End Property

    Private mConductor As String = String.Empty
    Public Property Conductor() As String
        Get
            Return mConductor
        End Get
        Set(ByVal value As String)
            mConductor = value
        End Set
    End Property

    Private mAlbumArtist As String = String.Empty
    Public Property AlbumArtist() As String
        Get
            Return mAlbumArtist
        End Get
        Set(ByVal value As String)
            mAlbumArtist = value
        End Set
    End Property

    Private mArtist As String = String.Empty
    Public Property Artist() As String
        Get
            Return mArtist
        End Get
        Set(ByVal value As String)
            mArtist = value
        End Set
    End Property

    Private mMetaVersion As String = String.Empty
    Public Property MetaVersion() As String
        Get
            Return mMetaVersion
        End Get
        Set(ByVal value As String)
            mMetaVersion = value
        End Set
    End Property

    Private mAlbum As String = String.Empty
    Public Property Album() As String
        Get
            Return mAlbum
        End Get
        Set(ByVal value As String)
            mAlbum = value
        End Set
    End Property

    Private mTrackCount As Integer = 0
    Public Property TrackCount() As Integer
        Get
            Return mTrackCount
        End Get
        Set(ByVal value As Integer)
            mTrackCount = value
        End Set
    End Property

    Private mDiscCount As Integer = 0
    Public Property DiscCount() As Integer
        Get
            Return mDiscCount
        End Get
        Set(ByVal value As Integer)
            mDiscCount = value
        End Set
    End Property

    Private mDiscNum As Integer = 0
    Public Property DiscNumber() As Integer
        Get
            Return mDiscNum
        End Get
        Set(ByVal value As Integer)
            mDiscNum = value
        End Set
    End Property

    Private mDuration As Integer = 0
    Public Property Duration() As Integer
        Get
            Return mDuration
        End Get
        Set(ByVal value As Integer)
            mDuration = value
        End Set
    End Property

    Private mYear As Integer = 0
    Public Property Year() As Integer
        Get
            Return mYear
        End Get
        Set(ByVal value As Integer)
            mYear = value
        End Set
    End Property

    Private mGenre As String = String.Empty
    Public Property Genre() As String
        Get
            Return mGenre
        End Get
        Set(ByVal value As String)
            mGenre = value
        End Set
    End Property

    Private mEQ As String = String.Empty
    Public Property EQ() As String
        Get
            Return mEQ
        End Get
        Set(ByVal value As String)
            mEQ = value
        End Set
    End Property

    Private mPersistantID As String = String.Empty
    Public Property PersistantID() As String
        Get
            Return mPersistantID
        End Get
        Set(ByVal value As String)
            mPersistantID = value
        End Set
    End Property

    Private mLocation As String = String.Empty
    Public Property Location() As String
        Get
            Return mLocation
        End Get
        Set(ByVal value As String)
            mLocation = value
        End Set
    End Property

    Private mRememberBookmark As Boolean = False
    Public ReadOnly Property RememberBookmark() As Boolean
        Get
            Return mRememberBookmark
        End Get
    End Property

    Private mEnabled As Boolean = True
    Public Property Enabled() As Boolean
        Get
            Return mEnabled
        End Get
        Set(ByVal value As Boolean)
            mEnabled = value
        End Set
    End Property

    Private mInLibrary As Boolean
    Public Property InLibrary() As Boolean
        Get
            Return mInLibrary
        End Get
        Set(ByVal value As Boolean)
            mInLibrary = value
        End Set
    End Property

    Private mExcludeFromShuffle As Boolean
    Public Property ExcludeFromShuffle() As Boolean
        Get
            Return mExcludeFromShuffle
        End Get
        Set(ByVal value As Boolean)
            mExcludeFromShuffle = value
        End Set
    End Property

    Private mSkippedCount As Integer
    Public Property SkippedCount() As Integer
        Get
            Return mSkippedCount
        End Get
        Set(ByVal value As Integer)
            mSkippedCount = value
        End Set
    End Property

    Private mPlayedCount As Integer
    Public Property PlayedCount() As Integer
        Get
            Return mPlayedCount
        End Get
        Set(ByVal value As Integer)
            mPlayedCount = value
        End Set
    End Property

    Private mRating As Integer
    Public Property Rating() As Integer
        Get
            Return mRating
        End Get
        Set(ByVal value As Integer)
            mRating = value
        End Set
    End Property

    Private mStart As Integer
    Public Property Start() As Integer
        Get
            Return mStart
        End Get
        Set(ByVal value As Integer)
            mStart = value
        End Set
    End Property

    Private mFinish As Integer
    Public Property Finish() As Integer
        Get
            Return mFinish
        End Get
        Set(ByVal value As Integer)
            mFinish = value
        End Set
    End Property

    Private mBookmarkTime As Integer
    Public Property BookmarkTime() As Integer
        Get
            Return mBookmarkTime
        End Get
        Set(ByVal value As Integer)
            mBookmarkTime = value
        End Set
    End Property

    Private mSkippedDate As Date
    Public Property SkippedDate() As Date
        Get
            Return mSkippedDate
        End Get
        Set(ByVal value As Date)
            mSkippedDate = value
        End Set
    End Property


    Private mLastPlayed As Date
    Public Property PlayedDate() As Date
        Get
            Return mLastPlayed
        End Get
        Set(ByVal value As Date)
            mLastPlayed = value
        End Set
    End Property

    Private mDateAdded As Date
    Public Property DateAdded() As Date
        Get
            Return mDateAdded
        End Get
        Set(ByVal value As Date)
            mDateAdded = value
        End Set
    End Property

    Private mModifiedDate As Date
    Public Property ModificationDate() As Date
        Get
            Return mModifiedDate
        End Get
        Set(ByVal value As Date)
            mModifiedDate = value
        End Set
    End Property

    Private mXmlArtwork As cXmlArtwork = Nothing
    Public Property Artwork() As cXmlArtwork
        Get
            Return mXmlArtwork
        End Get
        Set(ByVal value As cXmlArtwork)
            mXmlArtwork = value
        End Set
    End Property

    Private mNotes As String = String.Empty
    Public Property Notes() As String
        Get
            Return mNotes
        End Get
        Set(ByVal value As String)
            mNotes = value
        End Set
    End Property

    Private mTrackType As TrackTypeXML
    Public Property TrackType() As TrackTypeXML
        Get
            Return mTrackType
        End Get
        Set(ByVal value As TrackTypeXML)
            mTrackType = value
        End Set
    End Property

    Private mGoogleSearch As String = String.Empty
    Public ReadOnly Property GoogleSearchURL() As String
        Get
            Return mGoogleSearch
        End Get
    End Property

    Private Sub zFillTrack_iTunes(ByVal track As iTunesLib.IITTrack, ByVal booWidthHeight As Boolean)

        If track IsNot Nothing Then

            ' tags for backup/restore
            Me.Rating = track.Rating
            Me.PlayedDate = track.PlayedDate
            Me.PlayedCount = track.PlayedCount
            Me.Start = track.Start
            Me.Finish = track.Finish
            Me.Enabled = track.Enabled
            Me.EQ = track.EQ

            Me.Name = track.Name
            Me.TrackNumber = track.TrackNumber
            Me.Album = track.Album
            Me.Artist = track.Artist
            Me.Composer = track.Composer
            Me.Compilation = track.Compilation '' 5.37.1.2 Compilation tag was not read by XmlTrack implementation
            Me.Grouping = track.Grouping
            Me.Genre = track.Genre
            Me.Year = track.Year
            Me.TrackCount = track.TrackCount
            Me.DiscNumber = track.DiscNumber
            Me.DiscCount = track.DiscCount
            Me.Duration = track.Duration

            Me.DateAdded = track.DateAdded
            Me.ModificationDate = track.ModificationDate

            If track.Artwork.Count > 0 Then
                Me.Artwork = New cXmlArtwork(track, booWidthHeight)
            End If

            Me.ModificationDate = track.ModificationDate

        End If

    End Sub

    Private Sub zFillTrack2_iTunes(ByVal track As iTunesLib.IITFileOrCDTrack)

        If track IsNot Nothing Then

            ' tags for backup/restore
            Me.ExcludeFromShuffle = track.ExcludeFromShuffle
            Me.SkippedCount = track.SkippedCount
            Me.SkippedDate = track.SkippedDate
            Me.BookmarkTime = track.BookmarkTime
            Me.mRememberBookmark = track.RememberBookmark

            Me.AlbumArtist = track.AlbumArtist

            Try
                Dim f As TagLib.File = TagLib.File.Create(track.Location)
                Me.Conductor = f.Tag.Conductor
            Catch ex As Exception

            End Try

            Me.Location = track.Location

            ' generate google artwork search string
            Dim url As String = ""

            If track.AlbumArtist <> String.Empty Then
                url = String.Format("http://www.google.com/search?q={0}+%22{1}%22", track.Album, track.AlbumArtist)
                mGoogleSearch = String.Format("<a href={0}{1}{0}>{2} - {3}</a>", Chr(34), url, track.AlbumArtist, track.Album)
            Else
                url = String.Format("http://www.google.com/search?q=%22{0}%22+%22{1}%22", track.Album, track.Artist)
                mGoogleSearch = String.Format("<a href={0}{1}{0}>{2} - {3}</a>", Chr(34), url, track.Artist, track.Album)
            End If

        End If

    End Sub

    Public Sub New()

    End Sub

    Private mSize As Integer
    Public Property Size() As Integer
        Get
            Return mSize
        End Get
        Set(ByVal value As Integer)
            mSize = value
        End Set
    End Property

    Private mBitRate As Integer
    Public Property BitRate() As Integer
        Get
            Return mBitRate
        End Get
        Set(ByVal value As Integer)
            mBitRate = mBitRate
        End Set
    End Property

    Public Sub New(ByVal filePath As String, ByVal booWidthHeight As Boolean)

        Try

            Dim f As TagLib.File = TagLib.File.Create(filePath)

            Try
                Dim id32_tag As TagLib.Id3v2.Tag = CType(f.GetTag(TagLib.TagTypes.Id3v2, True), TagLib.Id3v2.Tag)
                If id32_tag IsNot Nothing Then
                    Me.Compilation = id32_tag.IsCompilation
                Else
                    Dim apple_tag As TagLib.Mpeg4.AppleTag = CType(f.GetTag(TagLib.TagTypes.Apple, True), TagLib.Mpeg4.AppleTag)
                    If apple_tag IsNot Nothing Then
                        Me.Compilation = apple_tag.IsCompilation
                    End If
                End If
            Catch ex As Exception
                ' dont care
            End Try

            Me.Conductor = f.Tag.Conductor
            Me.Composer = f.Tag.FirstComposer

            Me.Location = filePath
            Me.TrackNumber = CInt(f.Tag.Track)

            Me.Name = f.Tag.Title
            If f.Tag.FirstPerformer IsNot Nothing Then
                Me.Artist = f.Tag.FirstPerformer
            End If

            If Me.Compilation Then
                Me.AlbumArtist = VARIOUS_ARTISTS
            Else
                If f.Tag.FirstAlbumArtist <> String.Empty Then
                    Me.AlbumArtist = mfTagLibAlbumArtist(f.Tag.AlbumArtists).Trim
                Else
                    Me.AlbumArtist = f.Tag.FirstPerformer
                End If
            End If

            Me.Album = f.Tag.Album
            Me.Genre = f.Tag.FirstGenre
            If f.Tag.Pictures.Length > 0 Then
                Me.Artwork = New cXmlArtwork(Nothing, False)
            End If
            Me.TrackCount = CInt(f.Tag.TrackCount)
            Me.DiscNumber = CInt(f.Tag.Disc)
            Me.DiscCount = CInt(f.Tag.DiscCount)
            Me.Year = CInt(f.Tag.Year)

        Catch ex As Exception

            ' oh well

        End Try

    End Sub
    Public Sub New(ByVal track As iTunesLib.IITTrack, ByVal booWidthHeight As Boolean)
        Call zFillTrack_iTunes(track, booWidthHeight)
        If track.Kind = iTunesLib.ITTrackKind.ITTrackKindFile Then
            zFillTrack2_iTunes(CType(track, iTunesLib.IITFileOrCDTrack))
        End If
    End Sub

    Public Sub New(ByVal track As WMPTrack, ByVal booWidthHeight As Boolean)

        Me.TrackNumber = track.TrackNumber
        Me.TrackCount = track.TrackCount
        Me.DiscNumber = track.DiscNumber
        Me.DiscCount = track.DiscCount

        Me.Artist = track.Artist
        Me.AlbumArtist = track.AlbumArtist
        Me.Album = track.Album
        Me.Genre = track.Genre
        Me.Year = track.Year
        Me.Location = track.Location

    End Sub

    Public ReadOnly Property TagsComplete() As Boolean

        Get
            Return TrackNumber > 0 And _
                               Name IsNot Nothing And _
                               Artist IsNot Nothing And _
                               AlbumArtist IsNot Nothing And _
                               Album IsNot Nothing And _
                               Genre IsNot Nothing And _
                               Artwork IsNot Nothing And _
                               TrackCount > 0 And _
                               DiscNumber > 0 And _
                               DiscCount > 0 And _
                               Year > 0

        End Get

    End Property

End Class
