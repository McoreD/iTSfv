Imports TagLib
Imports TagLib.Id3v2
Imports System.IO

Public Class cTrackEditor

    Private f As TagLib.File
    Private wasReadOnly As Boolean
    Private fiTrack As FileInfo

    Public Function UseFile(ByVal filePath As String) As Boolean

        Dim succ As Boolean = True

        Try

            fiTrack = New FileInfo(filePath)
            wasReadOnly = fiTrack.IsReadOnly
            If fiTrack.IsReadOnly Then
                fiTrack.IsReadOnly = False
            End If
            f = TagLib.File.Create(filePath)
        Catch ex As Exception
            succ = False
            msAppendWarnings(ex.Message)
        End Try

        Return succ

    End Function

    Public Function SetFramePCNT(ByVal playedCount As Integer) As Boolean

        Dim id32_tag As TagLib.Id3v2.Tag = CType(f.GetTag(TagLib.TagTypes.Id3v2, True), TagLib.Id3v2.Tag)
        Dim succ As Boolean = True

        Try
            If id32_tag IsNot Nothing Then
                Dim pcnt As PlayCountFrame = PlayCountFrame.Get(id32_tag, True)
                pcnt.PlayCount = CULng(playedCount)
            End If
            'msAppendDebug(String.Format("Setting PCNT Frame to {0}", playedCount))
        Catch ex As Exception
            msAppendWarnings(ex.Message + " while writing PCNT frame")
            succ = False
        End Try

        Return succ

    End Function

    Public Function SetFramePOPM(ByVal playedCount As Integer, ByVal rating As Integer) As Boolean

        Dim id32_tag As TagLib.Id3v2.Tag = CType(f.GetTag(TagLib.TagTypes.Id3v2, True), TagLib.Id3v2.Tag)
        Dim succ As Boolean = False
        Try
            If id32_tag IsNot Nothing Then
                Dim userString As String = If(My.Settings.EmailAddress <> "", My.Settings.EmailAddress, EMAIL_ADDRESS)
                Dim popm As PopularimeterFrame = PopularimeterFrame.Get(id32_tag, userString, True)

                popm.Rating = Convert.ToByte(Math.Min(255, 255 * rating / 100))
                popm.PlayCount = CULng(playedCount)

                'msAppendDebug(String.Format("Setting PlayedCount|Rating for track as {0}|{1}", playedCount, rating))                
                succ = True
            End If
        Catch ex As Exception
            msAppendWarnings(ex.Message + " while writing POPM frame")
        End Try

        Return succ

    End Function

    Public Function SetArtworkTypeFront() As Boolean

        Dim succ As Boolean = False

        Try
            If f.Tag.Pictures.Length > 0 Then
                If f.Tag.Pictures(0).Type = PictureType.Other Then
                    msAppendDebug("Changing Picture type from Other to Cover (front) for " & f.Name)
                    f.Tag.Pictures(0).Type = PictureType.FrontCover
                End If
            End If
            succ = True
        Catch ex As Exception
            msAppendWarnings(ex.Message + "Changing Picture type from Other to Cover (front) for " & f.Name)
        End Try

        Return succ

    End Function

    Public Sub Save()

        Try
            f.Save()
            If wasReadOnly Then
                fiTrack.IsReadOnly = True
            End If
        Catch ex As Exception
            msAppendWarnings(ex.Message + " while saving taglib# file tags")
        End Try

    End Sub


End Class
