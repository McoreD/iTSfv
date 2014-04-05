Imports TagLib.Id3v2

Public Module mTagLibJobs

    ''' <summary>
    ''' Returns a POPM structure with PlayedCount and Rating
    ''' If POPM PlayedCount is zero then it will scan PCNT 
    ''' </summary>
    ''' <param name="filePath"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function mfGetPOPM(ByVal filePath As String) As FramePOPM

        Dim popm As New FramePOPM

        Try
            Dim f As TagLib.File = TagLib.File.Create(filePath)

            Dim id32_tag As TagLib.Id3v2.Tag = CType(f.GetTag(TagLib.TagTypes.Id3v2, True), TagLib.Id3v2.Tag)

            If id32_tag IsNot Nothing Then

                Dim userString As String = If(My.Settings.EmailAddress <> "", My.Settings.EmailAddress, EMAIL_ADDRESS)
                Dim pf As PopularimeterFrame = PopularimeterFrame.Get(id32_tag, userString, False)
                If pf IsNot Nothing Then
                    popm.PlayedCount = CInt(pf.PlayCount)
                    popm.Rating = CInt(100 * (pf.Rating / 255))
                End If

                '' this is the PCNT frame
                If popm.PlayedCount = 0 Then
                    Dim pc As PlayCountFrame = PlayCountFrame.Get(id32_tag, False)
                    If pc IsNot Nothing Then
                        popm.PlayedCount = CInt(pc.PlayCount)
                    End If
                End If

            End If

        Catch ex As Exception

        End Try

        Return popm

    End Function

    Public Function mfImageFromFile(ByVal filePath As String) As Image

        Try
            Dim f As TagLib.File = TagLib.File.Create(filePath)
            If f.Tag.Pictures.Length > 0 Then
                Return mfPictureToImage(f.Tag.Pictures(0))
            End If
        Catch ex As Exception
            'nothing yet
        End Try
        Return Nothing

    End Function

    Public Function mfPictureToImage(ByVal pic As TagLib.IPicture) As Image

        Dim img As Drawing.Image = Nothing

        If pic IsNot Nothing Then
            Using ms As New IO.MemoryStream(pic.Data.Data.Length)
                ms.Write(pic.Data.Data, 0, pic.Data.Data.Length)
                ms.Flush()
                img = Image.FromStream(ms)
                ms.Close()
            End Using
        End If

        Return img

    End Function

    Public Function mfImageToPicture(ByVal img As System.Drawing.Image) As TagLib.IPicture

        Dim pic As TagLib.Picture

        Dim fs As New IO.MemoryStream

        img.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg)

        Dim b() As Byte = fs.GetBuffer

        fs.Flush()
        fs.Close()
        fs.Dispose()

        pic = New TagLib.Picture(New TagLib.ByteVector(b))

        Return pic

    End Function

End Module
