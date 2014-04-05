Imports iTunesLib
Imports System.IO

Public Class cPlaylistWriter

    Private mTracks As New List(Of IITFileOrCDTrack)
    Private mFilePaths As New List(Of String)
    Private mDisc As cInfoDisc = Nothing

    Public Sub New(ByVal filePaths As List(Of String))
        Me.mFilePaths = filePaths
        mFilePaths.Sort()
    End Sub

    Public Sub New(ByVal lDisc As cInfoDisc)

        mDisc = lDisc
        mTracks.AddRange(mDisc.Tracks)

        For Each track As IITFileOrCDTrack In lDisc.Tracks
            mFilePaths.Add(track.Location)
        Next
        mFilePaths.Sort()

        Try
            mTracks.Sort(New cIITFileOrCDTrackComparer)
        Catch ex As Exception

        End Try

    End Sub

    Public Sub New(ByVal tracks As iTunesLib.IITTrackCollection)

        For Each track As IITTrack In tracks
            If track.Kind = ITTrackKind.ITTrackKindFile Then
                mTracks.Add(CType(track, IITFileOrCDTrack))
                mFilePaths.Add(CType(track, IITFileOrCDTrack).Location)
            End If
        Next

        mFilePaths.Sort()

        Try
            mTracks.Sort(New cIITFileOrCDTrackComparer)
        Catch ex As Exception

        End Try

    End Sub

    Public Sub New(ByVal tracks As List(Of cXmlTrack))

        For Each track As cXmlTrack In tracks
            mFilePaths.Add(track.Location)
        Next
        mFilePaths.Sort()

    End Sub

    Public Sub New(ByVal tracks As List(Of IITFileOrCDTrack))

        mTracks.AddRange(tracks)
        Try
            mTracks.Sort(New cIITFileOrCDTrackComparer)
        Catch ex As Exception

        End Try

        For Each track As IITFileOrCDTrack In tracks
            mFilePaths.Add(track.Location)
        Next
        mFilePaths.Sort()

    End Sub

    Public Function fCreatePlaylisXSPF(ByVal where As String) As Boolean


        Dim dir As String = Path.GetDirectoryName(where)
        If Directory.Exists(dir) = False Then
            Directory.CreateDirectory(dir)
        End If

        Dim lXmlTracks As New List(Of cXmlTrack)
        For Each track As IITFileOrCDTrack In mTracks
            Try
                lXmlTracks.Add(New cXmlTrack(track, False))
            Catch ex As Exception
                msAppendWarnings(ex.Message)
            End Try
        Next
        Dim xspfCreator As cXspfCreator
        If mDisc IsNot Nothing Then
            xspfCreator = New cXspfCreator(where, mDisc)
        Else
            xspfCreator = New cXspfCreator(where, lXmlTracks)
        End If
        xspfCreator.Save()

    End Function

    Public Function fCreatePlaylistM3U(ByVal where As String, ByVal relativeFileName As Boolean) As Boolean

        Dim success As Boolean = False

        If Path.GetExtension(where) <> ".m3u" Then
            where = Path.ChangeExtension(where, ".m3u")
        End If

        'If where.Contains(".m3u") = False Then
        '    where = where.Insert(where.Length, ".m3u")
        'End If

        Dim dir As String = Path.GetDirectoryName(where)
        If Directory.Exists(dir) = False Then
            Directory.CreateDirectory(dir)
        End If

        Try

            Using sw As New System.IO.StreamWriter(where)

                sw.WriteLine("#EXTM3U")

                For Each fPath As String In mFilePaths

                    sw.WriteLine(String.Format("#EXTINF:0,{0}", Path.GetFileName(fPath)))
                    If relativeFileName = True Then
                        sw.WriteLine(Path.GetFileName(fPath))
                    Else
                        sw.WriteLine(fPath)
                    End If

                    sw.WriteLine()

                Next

                success = True

            End Using

        Catch ex As Exception

            msAppendWarnings(ex.Message + " while creating M3U playlist.")

        End Try

        Return success

    End Function
End Class
