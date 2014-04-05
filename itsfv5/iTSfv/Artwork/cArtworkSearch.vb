Imports System.IO

Public MustInherit Class cArtworkSearch
    Implements iArtworkSearch


    ' This code is based on original code by:
    ' Thanks to david_dl and AlexVallat at HydrongenAudio 

    Protected mArtist As String
    Protected mAlbum As String
    Protected mDirArtwork As String
    Protected mJpgFilePath As String

    Public Sub New(ByVal artist As String, ByVal album As String)

        Me.mArtist = artist
        Me.mAlbum = album

        Me.mDirArtwork = My.Settings.ArtworkDir + mfGetLegalText(Me.mArtist) + Path.DirectorySeparatorChar + mfGetLegalText(Me.mAlbum)

        Try
            If Directory.Exists(mDirArtwork) = False Then
                'Console.Writeline(mDirArtwork)
                Directory.CreateDirectory(mDirArtwork)
            End If
        Catch ex As Exception
            msAppendWarnings(ex.Message + " while creating directory " + mDirArtwork)
        End Try

    End Sub


    ' overrides by itunes, amazon classes 

    Public Overridable Function GetArtworkPath() As String Implements iArtworkSearch.GetArtworkPath
        Return String.Empty
    End Function

End Class
