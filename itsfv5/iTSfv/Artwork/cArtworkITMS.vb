Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports System.Xml
Imports System.Net
Imports ICSharpCode.SharpZipLib
Imports System.Text.RegularExpressions
Imports System.Drawing.Imaging

Public Class cArtworkITMS
    Inherits cArtworkSearch

    ' This code is based on original code by:
    ' Thanks to david_dl and AlexVallat at HydrongenAudio 

    Public Sub New(ByVal artist As String, ByVal album As String)
        MyBase.New(artist, album)

        Me.mJpgFilePath = mDirArtwork + Path.DirectorySeparatorChar + "iTMS.jpg"

    End Sub

    Private Function GetStream(ByVal url As String, ByVal sid As String) As Stream

        Dim request As HttpWebRequest = CType(HttpWebRequest.Create(url), HttpWebRequest)
        Dim response As WebResponse = Nothing
        Try
            request.UserAgent = "iTunes/7.4 (Macintosh; U; PPC Mac OS X 10.4.7)"
            request.Headers.Add("X-Apple-Tz", "-21600")
            request.Headers.Add("X-Apple-Store-Front", sid)
            request.Headers.Add("Accept-Language", "en-us, en;q=0.50")
            request.Headers.Add("Accept-Encoding", "gzip, x-aes-cbc")
            request.Timeout = My.Settings.TimeoutITMS
            Return request.GetResponse().GetResponseStream()
        Catch ex As Exception
            msAppendWarnings(String.Format("{0} for accessing iTMS Store ID {1}", ex.Message, sid))
        End Try

        Return Nothing

    End Function

    Public Overrides Function GetArtworkPath() As String

        Try

            If File.Exists(mJpgFilePath) Then

                msAppendDebug("Found saved iTMS Artwork: " & mJpgFilePath)
                Return mJpgFilePath

            Else
                ' start searching itms artwork
                Dim listStores As New List(Of cITunesStore)
                listStores.Add(New cITunesStore("United States", 143441))
                listStores.Add(New cITunesStore("Australia", 143460))
                listStores.Add(New cITunesStore("United Kindom", 143444))
                listStores.Add(New cITunesStore("Norway", 143457))
                listStores.Add(New cITunesStore("Canada", 143455))
                listStores.Add(New cITunesStore("Germany", 143443))
                listStores.Add(New cITunesStore("New Zealand", 143452))
                listStores.Add(New cITunesStore("Japan", 143462))
                listStores.Add(New cITunesStore("China", 143459))
                listStores.Add(New cITunesStore("Denmark", 143458))
                listStores.Add(New cITunesStore("France", 143442))

                ' TODO: Add these to list
                '143445 => 'AT',
                '143446 => 'BE',
                '143447 => 'FI',
                '143448 => 'GR',
                '143449 => 'IE',
                '143450 => 'IT',
                '143451 => 'LU',
                '143453 => 'PT',
                '143454 => 'SP',
                '143456 => 'SE',

                For Each store As cITunesStore In listStores
                    Dim artworkPath As String = GetArtworkStream(store)
                    If artworkPath IsNot Nothing Then
                        Return mJpgFilePath
                    End If
                Next
                msAppendDebug("Could not find iTMS Artwork in any store.")
                Return String.Empty

            End If

        Catch ex As Exception

            msAppendDebug("Error getting iTMS Artwork")
            Return String.Empty

        End Try

    End Function

    Private Function GetArtworkStream(ByVal store As cITunesStore) As String

        msAppendDebug(String.Format("Searching iTMS Artwork for {0} - {1} using iTunes Store ({2})", mArtist, mAlbum, store.Country))
        'Console.Writeline(String.Format("Searching iTMS Artwork for {0} - {1} using iTunes Store ({2})", mArtist, mAlbum, store.Country))

        Dim x As XmlDocument = New System.Xml.XmlDocument()
        Dim art As Stream = Nothing

        Dim searchUrl As String = "http://ax.phobos.apple.com.edgesuite.net/WebObjects/MZSearch.woa/wa/coverArtMatch?an=" + mfEncodeUrl(mArtist) + "&pn=" + mfEncodeUrl(mAlbum)
        Dim xmlpage As Stream = GetStream(searchUrl, store.ID)

        Try
            x.Load(New ICSharpCode.SharpZipLib.GZip.GZipInputStream(xmlpage))
        Catch
            Return Nothing 'Throw New Exception("No results")
        Finally
            xmlpage.Close()
        End Try

        Dim tags As XmlNodeList = x.GetElementsByTagName("dict")

        If tags.Count = 0 Then
            Return Nothing
        End If

        Dim url As String = ""
        Dim tag As XmlNode
        For Each tag In tags(0).ChildNodes
            If tag.InnerText = "cover-art-url" Then
                url = tag.NextSibling.InnerText
            End If
        Next

        If url.Length = 0 Then
            msAppendDebug(String.Format("Could not find Artwork in iTMS ({0}).", store.Country))
            Return Nothing ' Throw New Exception("No results for search.")
        End If

        url = url.Substring(0, url.IndexOf("?")).Replace(".enc.jpg", "")

        Dim ext As String = ".jpg"
        art = GetStream(url + ext, store.ID)
        If art Is Nothing Then
            ext = ".tif"
            art = GetStream(url + ext, store.ID)
        End If

        If art Is Nothing Then
            msAppendDebug(String.Format("iTMS Artwork {0} was not retrievable from iTMS ({1}).", url + ext, store.Country))
            Return Nothing
        Else
            msAppendDebug(String.Format("Found iTMS Artwork using iTMS ({0})", store.Country))
        End If

        If ext = ".tif" Then
            'System.Drawing.Bitmap.FromStream(art).Save(mJpgFilePath + ".png", System.Drawing.Imaging.ImageFormat.Png)
            System.Drawing.Bitmap.FromStream(art).Save(mJpgFilePath, System.Drawing.Imaging.ImageFormat.Jpeg)
            msAppendDebug(String.Format("Successfully converted TIFF Artwork to JPG and saved as {0}", mJpgFilePath))
        Else ' save jpg
            Dim buf() As Byte = New Byte(4096) {}
            Dim f As FileStream = File.OpenWrite(mJpgFilePath)
            Dim bytesread As Integer = art.Read(buf, 0, buf.Length)
            While bytesread > 0
                f.Write(buf, 0, bytesread)
                bytesread = art.Read(buf, 0, buf.Length)
            End While
            f.Close()
        End If

        art.Close()
        Return mJpgFilePath

    End Function



End Class

