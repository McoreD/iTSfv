Imports System.Text
Imports System.IO
Imports System.Xml
Imports System.Net

Public Class cArtworkAmazon
    Inherits cArtworkSearch

    Public Sub New(ByVal artist As String, ByVal album As String)
        MyBase.New(artist, album)

        Me.mJpgFilePath = mDirArtwork + Path.DirectorySeparatorChar + "Amazon.jpg"

    End Sub

    Public Overrides Function GetArtworkPath() As String

        Dim fullSize As String = String.Empty

        Dim x As New System.Xml.XmlDocument
        Dim n As New System.Xml.XmlNamespaceManager(New NameTable)
        n.AddNamespace("a", "http://webservices.amazon.com/AWSECommerceService/2005-10-05")

        x.Load("http://ecs.amazonaws.com/onca/xml?Service=AWSECommerceService&AWSAccessKeyId=1MV23E34ARMVYMBDZB02&Operation=ItemSearch&SearchIndex=Music&ItemPage=1&ResponseGroup=ItemAttributes,Images&Keywords=" + mfEncodeUrl(Me.mArtist + " " + Me.mAlbum))

        Dim results As XmlNodeList = x.SelectNodes("a:ItemSearchResponse/a:Items/a:Item[a:LargeImage/a:URL]", n)
        For Each node As XmlNode In results
            Dim title As String = node.SelectSingleNode("a:ItemAttributes/a:Title", n).InnerText
            Dim artistNode As XmlNode = node.SelectSingleNode("a:ItemAttributes/a:Artist", n)
            If artistNode IsNot Nothing Then
                title = artistNode.InnerText + " - " + title
            End If
            Dim width As Integer = -1
            Dim height As Integer = -1
            Dim widthNode As XmlNode = node.SelectSingleNode("a:LargeImage/a:Width", n)
            Dim heightNode As XmlNode = node.SelectSingleNode("a:LargeImage/a:Height", n)
            If widthNode IsNot Nothing AndAlso heightNode IsNot Nothing Then
                fullSize = node.SelectSingleNode("a:LargeImage/a:URL", n).InnerText
            End If

        Next

        Return fullSize

    End Function





End Class
