Imports System.IO
Imports iTunesLib
Imports System.Web
Imports System.Xml
Imports System.Xml.XPath
Imports iTSfv.cBwJob
Imports System.Collections.Specialized

Public Class cLibraryParser

    ' THIS IS AN ENHANCED VERSION OF ERIC DAUGHERTY'S ITUNES LIBRARY PARSER 
    ' UPDATES TO ORIGINAL CODE CAN BE FOUND IN 
    ' http://www.ericdaugherty.com/dev/itunesexport/changes.html

    Private mFilePath As String
    Private mMusicFolder As String
    Private mBwApp As System.ComponentModel.BackgroundWorker
    Private mListXmlTrackColl As New List(Of cXmlTrack)  
    Private mDictXmlTrackColl As New Dictionary(Of String, cXmlTrack)

    Public Property MusicFolder() As String
        Get
            Return mMusicFolder
        End Get
        Set(ByVal value As String)
            mMusicFolder = value
        End Set
    End Property

    Private mLibraryPersistantID As String
    Public ReadOnly Property LibraryPersistantID() As String
        Get
            Return mLibraryPersistantID
        End Get
    End Property

    Public ReadOnly Property TrackCollection() As List(Of cXmlTrack)
        Get
            Return mListXmlTrackColl
        End Get
    End Property

    Public Function fFindXmlTrack(ByVal track As IITFileOrCDTrack) As cXmlTrack

        '' 5.34.10.0 Much faster Recovery of Tags using a previous iTunes Music Library.xml 

        Dim key As String = fCreateTrackKey(track)

        mBwApp.ReportProgress(ProgressType.GETTING_TRACK_INFO, track.Artist & " - " & track.Name)

        If mDictXmlTrackColl.ContainsKey(key) Then
            Return mDictXmlTrackColl(key)
        End If

        Return Nothing

    End Function



    Public Sub New(ByRef bwApp As System.ComponentModel.BackgroundWorker, ByVal filePath As String)

        Me.mBwApp = bwApp
        mFilePath = filePath

        Dim xmlReader As New XmlTextReader(filePath)
        xmlReader.XmlResolver = Nothing
        Dim xPathDocument As XPathDocument = New XPathDocument(xmlReader)
        Dim xPathNavigator As XPathNavigator = xPathDocument.CreateNavigator

        Dim nodeIterator As XPathNodeIterator = xPathNavigator.Select("/plist/dict")
        nodeIterator.MoveNext()
        nodeIterator = nodeIterator.Current.SelectChildren(XPathNodeType.All)

        While (nodeIterator.MoveNext)

            If (nodeIterator.Current.Value.Equals("Music Folder")) Then
                If (nodeIterator.MoveNext()) Then
                    Dim musicFolder As String = nodeIterator.Current.Value
                    musicFolder = musicFolder.Replace("file://localhost/", String.Empty)
                    ' 5.11.0.2 Fixed problem with UNC paths having an extra leading slash removed [John]
                    If musicFolder.StartsWith("/") Then
                        musicFolder = String.Format("/{0}", musicFolder)
                    End If
                    musicFolder = HttpUtility.UrlDecode(musicFolder)
                    musicFolder = musicFolder.Replace("/", Path.DirectorySeparatorChar)
                    mMusicFolder = musicFolder
                End If
            End If

            If (nodeIterator.Current.Value.Equals("Library Persistent ID")) Then
                If (nodeIterator.MoveNext()) Then
                    mLibraryPersistantID = nodeIterator.Current.Value
                    Exit While
                End If
            End If

        End While

        nodeIterator = xPathNavigator.Select("/plist/dict/dict/dict")

        Dim i As Integer = 1
        While (nodeIterator.MoveNext())
            Dim xmlTrack As New cXmlTrack()
            xmlTrack.Index = i
            i += 1
            sParseTrack(nodeIterator.Current.SelectChildren(XPathNodeType.All), xmlTrack)
        End While

    End Sub

    Private Function fCreateTrackKey(ByVal track As IITFileOrCDTrack) As String
        Return track.Artist + track.Name + track.Album
    End Function
    Private Function fCreateTrackKey(ByVal track As cXmlTrack) As String
        Return track.Artist + track.Name + track.Album
    End Function

    Private Sub sParseTrack(ByVal nodeIterator As XPathNodeIterator, ByVal xmlTrack As cXmlTrack)

        Try

            Dim currentValue As String

            While (nodeIterator.MoveNext())

                currentValue = nodeIterator.Current.Value

                If String.IsNullOrEmpty(currentValue) = False Then

                    If currentValue.Equals("Track ID") Then
                        If nodeIterator.MoveNext() Then
                            Integer.TryParse(nodeIterator.Current.Value, xmlTrack.TrackID)
                        End If

                    ElseIf currentValue.Equals("Name") Then
                        If nodeIterator.MoveNext() Then
                            xmlTrack.Name = nodeIterator.Current.Value
                        End If

                    ElseIf currentValue.Equals("Artist") Then
                        If nodeIterator.MoveNext() Then
                            xmlTrack.Artist = nodeIterator.Current.Value
                        End If

                    ElseIf currentValue.Equals("Album Artist") Then
                        If nodeIterator.MoveNext() Then
                            xmlTrack.AlbumArtist = nodeIterator.Current.Value
                        End If

                    ElseIf currentValue.Equals("Composer") Then
                        If nodeIterator.MoveNext() Then
                            xmlTrack.Composer = nodeIterator.Current.Value
                        End If

                    ElseIf currentValue.Equals("Album") Then
                        If nodeIterator.MoveNext() Then
                            xmlTrack.Album = nodeIterator.Current.Value
                        End If

                    ElseIf currentValue.Equals("Genre") Then
                        If nodeIterator.MoveNext() Then
                            xmlTrack.Genre = nodeIterator.Current.Value
                        End If

                    ElseIf currentValue.Equals("Total Time") Then
                        If nodeIterator.MoveNext() Then
                            xmlTrack.Duration = CInt(CInt(nodeIterator.Current.Value) / 1000)
                        End If

                    ElseIf currentValue.Equals("Disc Number") Then
                        If nodeIterator.MoveNext() Then
                            xmlTrack.DiscNumber = Integer.Parse(nodeIterator.Current.Value)
                        End If

                    ElseIf currentValue.Equals("Disc Count") Then
                        If nodeIterator.MoveNext() Then
                            xmlTrack.DiscCount = Integer.Parse(nodeIterator.Current.Value)
                        End If

                    ElseIf currentValue.Equals("Track Number") Then
                        If nodeIterator.MoveNext() Then
                            xmlTrack.TrackNumber = Integer.Parse(nodeIterator.Current.Value)
                        End If

                    ElseIf currentValue.Equals("Track Count") Then
                        If nodeIterator.MoveNext() Then
                            xmlTrack.TrackCount = Integer.Parse(nodeIterator.Current.Value)
                        End If

                    ElseIf currentValue.Equals("Year") Then
                        If nodeIterator.MoveNext() Then
                            xmlTrack.Year = Integer.Parse(nodeIterator.Current.Value)
                        End If

                    ElseIf currentValue.Equals("Date Modified") Then
                        If nodeIterator.MoveNext() Then
                            xmlTrack.ModificationDate = CDate(nodeIterator.Current.Value)
                        End If


                    ElseIf currentValue.Equals("Date Added") Then
                        If nodeIterator.MoveNext() Then
                            xmlTrack.DateAdded = CDate(nodeIterator.Current.Value)
                        End If

                    ElseIf currentValue.Equals("Equalizer") Then
                        If nodeIterator.MoveNext() Then
                            xmlTrack.EQ = nodeIterator.Current.Value
                        End If

                    ElseIf currentValue.Equals("Play Count") Then
                        If nodeIterator.MoveNext() Then
                            xmlTrack.PlayedCount = CInt(nodeIterator.Current.Value)
                        End If

                    ElseIf currentValue.Equals("Play Date UTC") Then
                        If nodeIterator.MoveNext() Then
                            xmlTrack.PlayedDate = CDate(nodeIterator.Current.Value)
                        End If

                    ElseIf currentValue.Equals("Track Type") Then
                        If nodeIterator.MoveNext() Then
                            Select Case nodeIterator.Current.Value
                                Case Is = "File"
                                    xmlTrack.TrackType = TrackTypeXML.FILE
                                Case Else
                                    xmlTrack.TrackType = TrackTypeXML.URL
                            End Select
                        End If

                    ElseIf currentValue.Equals("Rating") Then
                        If nodeIterator.MoveNext() Then
                            xmlTrack.Rating = Integer.Parse(nodeIterator.Current.Value)
                        End If

                    ElseIf currentValue.Equals("Persistent ID") Then
                        If nodeIterator.MoveNext Then
                            xmlTrack.PersistantID = nodeIterator.Current.Value
                        End If

                    ElseIf currentValue.Equals("Location") Then

                        If nodeIterator.MoveNext() Then
                            Dim loc As String = nodeIterator.Current.Value
                            If (loc.IndexOf(MusicFolder) <> -1) Then
                                loc = loc.Replace(MusicFolder, String.Empty)
                            Else
                                loc = loc.Replace("file://localhost/", String.Empty)
                                If loc.StartsWith("/") Then
                                    loc = String.Format("/{0}", loc)
                                End If
                            End If

                            loc = loc.Replace("+", "%2B") ' this was fixed in iTunesExport 1.3.2

                            loc = System.Web.HttpUtility.UrlDecode(loc)
                            If (loc.Length > 0 AndAlso loc(loc.Length - 1) = "/") Then
                                loc = loc.Substring(0, loc.Length - 1)
                            End If
                            loc = loc.Replace("/", Path.DirectorySeparatorChar)
                            xmlTrack.Location = loc
                        End If

                    End If

                End If

            End While

            mListXmlTrackColl.Add(xmlTrack)
            Dim key As String = fCreateTrackKey(xmlTrack)
            If mDictXmlTrackColl.ContainsKey(key) = False Then
                mDictXmlTrackColl.Add(fCreateTrackKey(xmlTrack), xmlTrack)
            End If

        Catch ex As Exception

            msAppendWarnings(ex.Message + " while parsing at " & nodeIterator.Current.BaseURI + " for " + xmlTrack.Location)

        End Try

    End Sub

End Class
