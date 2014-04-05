Imports WMPLib
Imports System.IO

Public Class WMPTrack    
    Implements WMPLib.IWMPMedia

    Private mTrack As WMPLib.IWMPMedia3
    Private mArtwork As WMPArtwork
    Private WithEvents player As WindowsMediaPlayer

    Public Sub New(ByVal track As WMPLib.IWMPMedia3)
        Me.mTrack = track
    End Sub

    Public Function fGetWMPMedia3() As IWMPMedia3
        Return Me.mTrack
    End Function

    Public Property GUID() As String
        Get
            Return mTrack.getItemInfo("WM/MediaClassPrimaryID")
        End Get
        Set(ByVal value As String)
            mTrack.setItemInfo("WM/MediaClassPrimaryID", value)
        End Set
    End Property

    Public Property Artist() As String
        Get
            Return mTrack.getItemInfo("Author")
        End Get
        Set(ByVal value As String)
            mTrack.setItemInfo("Author", value)
        End Set
    End Property

    Public Sub Play()

        player = New WindowsMediaPlayer
        player.URL = Me.Location
        player.controls.play()

    End Sub

    Public ReadOnly Property Artwork() As WMPArtwork

        Get

            If mArtwork Is Nothing Then
                mArtwork = sGetArtwork()
            End If

            Return mArtwork

        End Get

    End Property

    Private Function sGetArtwork() As WMPArtwork


        If mTrack.getAttributeCountByType("WM/Picture", "") > 0 Then

            mArtwork = New WMPArtwork(CType(mTrack.getItemInfoByType("WM/Picture", "", 0), IWMPMetadataPicture))

        End If

        Return mArtwork

    End Function

    Public Sub AddArtworkFromFile(ByVal filePath As String)

        'mTrack.setItemInfo("WM/Picture", filePath)
    End Sub

    Public Property Album() As String
        Get
            Return mTrack.getItemInfo("WM/AlbumTitle")
        End Get
        Set(ByVal value As String)
            mTrack.setItemInfo("WM/AlbumTitle", value)
        End Set
    End Property

    Private mCompilation As Boolean = False
    Public Property Compilation() As Boolean
        Get
            Return AlbumArtist = "Various Artists"
        End Get
        Set(ByVal value As Boolean)
            mCompilation = value
        End Set
    End Property
    Public Property AlbumArtist() As String
        Get
            Return mTrack.getItemInfo("WM/AlbumArtist")
        End Get
        Set(ByVal value As String)
            mTrack.setItemInfo("WM/AlbumArtist", value)
        End Set
    End Property

    Public Property Lyrics() As String
        Get
            Return mTrack.getItemInfo("WM/Lyrics")
        End Get
        Set(ByVal value As String)
            mTrack.setItemInfo("WM/Lyrics", value)
        End Set
    End Property
    Public Property PlayedCount() As Integer
        Get
            Return CInt(mTrack.getItemInfo("UserPlayCount"))
        End Get
        Set(ByVal value As Integer)
            mTrack.setItemInfo("UserPlayCount", CStr(value))
        End Set
    End Property


    Public Property Genre() As String
        Get
            Return mTrack.getItemInfo("WM/Genre")
        End Get
        Set(ByVal value As String)
            mTrack.setItemInfo("WM/Genre", value)
        End Set
    End Property

    Public Property TrackNumber() As Integer
        Get
            Dim temp As String = mTrack.getItemInfo("WM/TrackNumber")
            Dim trackNum As Integer = CInt(IIf(temp.Equals(""), 0, temp))
            Return trackNum
        End Get
        Set(ByVal value As Integer)
            mTrack.setItemInfo("WM/TrackNumber", CStr(value))
        End Set
    End Property
    Private ReadOnly Property attributeCount() As Integer Implements WMPLib.IWMPMedia.attributeCount
        Get
            Return mTrack.attributeCount
        End Get
    End Property

    Public ReadOnly Property Duration() As Double Implements WMPLib.IWMPMedia.duration
        Get
            Return mTrack.duration
        End Get
    End Property

    Private ReadOnly Property durationString() As String Implements WMPLib.IWMPMedia.durationString
        Get
            Return mTrack.durationString
        End Get
    End Property

    Private Function getAttributeName(ByVal lIndex As Integer) As String Implements WMPLib.IWMPMedia.getAttributeName
        Return mTrack.getAttributeName(lIndex)
    End Function

    Private Function getItemInfo(ByVal bstrItemName As String) As String Implements WMPLib.IWMPMedia.getItemInfo
        Return mTrack.getItemInfo(bstrItemName)
    End Function

    Private Function getItemInfoByAtom(ByVal lAtom As Integer) As String Implements WMPLib.IWMPMedia.getItemInfoByAtom
        Return mTrack.getItemInfoByAtom(lAtom)
    End Function

    Private Function getMarkerName(ByVal MarkerNum As Integer) As String Implements WMPLib.IWMPMedia.getMarkerName
        Return mTrack.getMarkerName(MarkerNum)
    End Function

    Private Function getMarkerTime(ByVal MarkerNum As Integer) As Double Implements WMPLib.IWMPMedia.getMarkerTime

    End Function

    Private ReadOnly Property imageSourceHeight() As Integer Implements WMPLib.IWMPMedia.imageSourceHeight
        Get

        End Get
    End Property

    Private ReadOnly Property imageSourceWidth() As Integer Implements WMPLib.IWMPMedia.imageSourceWidth
        Get

        End Get
    End Property

    Private ReadOnly Property isIdentical(ByVal pIWMPMedia As WMPLib.IWMPMedia) As Boolean Implements WMPLib.IWMPMedia.isIdentical
        Get

        End Get
    End Property

    Private Function isMemberOf(ByVal pPlaylist As WMPLib.IWMPPlaylist) As Boolean Implements WMPLib.IWMPMedia.isMemberOf

    End Function

    Public Function isReadOnlyItem(ByVal bstrItemName As String) As Boolean Implements WMPLib.IWMPMedia.isReadOnlyItem

        Return mTrack.isReadOnlyItem(bstrItemName)

    End Function

    Private ReadOnly Property markerCount() As Integer Implements WMPLib.IWMPMedia.markerCount
        Get

        End Get
    End Property

    Public Property Name() As String Implements WMPLib.IWMPMedia.name
        Get
            Return mTrack.name
        End Get
        Set(ByVal value As String)
            mTrack.name = value
        End Set
    End Property

    Public Property Year() As Integer
        Get
            Dim temp As String = mTrack.getItemInfo("WM/Year").Replace("�", "")
            Dim y As Integer = CInt(IIf(temp.Equals(""), 0, temp))
            Return y
        End Get
        Set(ByVal value As Integer)
            mTrack.setItemInfo("WM/Year", CStr(Year))
        End Set
    End Property

    Private Sub setItemInfo(ByVal bstrItemName As String, ByVal bstrVal As String) Implements WMPLib.IWMPMedia.setItemInfo
        mTrack.setItemInfo(bstrItemName, bstrVal)
    End Sub

    Public ReadOnly Property Kind() As MediaType
        Get
            Select Case mTrack.getItemInfo("MediaType")
                Case Is = "audio"
                    Return MediaType.AUDIO
                Case Is = "video"
                    Return MediaType.VIDEO
                Case Is = "radio"
                    Return MediaType.RADIO
                Case Else
                    Return MediaType.OTHER
            End Select
        End Get
    End Property

    Public Property EQ() As String
        Get
            Return String.Empty
        End Get
        Set(ByVal value As String)

        End Set
    End Property

    Public ReadOnly Property SkippedCount() As Integer
        Get
            Return 0
        End Get
    End Property

    Public Property PlayedDate() As Date
        Get

        End Get
        Set(ByVal value As Date)

        End Set
    End Property

    Public ReadOnly Property DateAdded() As Date
        Get

        End Get
    End Property

    Public Property DiscCount() As Integer
        Get

        End Get
        Set(ByVal value As Integer)

        End Set
    End Property

    Public Property DiscNumber() As Integer
        Get

        End Get
        Set(ByVal value As Integer)

        End Set
    End Property

    Public Property Grouping() As String
        Get
            Return String.Empty
        End Get
        Set(ByVal value As String)

        End Set
    End Property

    Public ReadOnly Property Rating() As Integer
        Get
            Return CInt(mTrack.getItemInfo("UserRating"))
        End Get
    End Property

    Public ReadOnly Property Location() As String Implements WMPLib.IWMPMedia.sourceURL

        Get
            Return mTrack.sourceURL
        End Get

    End Property

    Public Property TrackCount() As Integer
        Get

        End Get
        Set(ByVal value As Integer)

        End Set
    End Property

    Public Property UniqueFileIdentifier() As String
        Get
            Return mTrack.getItemInfo("WM/UniqueFileIdentifier")
        End Get
        Set(ByVal value As String)
            mTrack.setItemInfo("WM/UniqueFileIdentifier", value)
        End Set
    End Property

    Public Property WMCollectionID() As String
        Get
            Return mTrack.getItemInfo("WM/WMCollectionID")
        End Get
        Set(ByVal value As String)
            mTrack.setItemInfo("WM/WMCollectionID", value)
        End Set
    End Property

    Public Property WMCollectionGroupID() As String
        Get
            Return mTrack.getItemInfo("WM/WMCollectionGroupID")
        End Get
        Set(ByVal value As String)
            mTrack.setItemInfo("WM/WMCollectionGroupID", value)
        End Set
    End Property

    Public ReadOnly Property MediaClassSecondaryID() As String
        Get
            Return mTrack.getItemInfo("WM/MediaClassSecondaryID")
        End Get
    End Property

    Public ReadOnly Property WMContentID() As String
        Get
            Return mTrack.getItemInfo("WM/WMContentID")
        End Get
    End Property


End Class
