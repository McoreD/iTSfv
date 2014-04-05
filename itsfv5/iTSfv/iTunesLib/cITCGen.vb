Imports System.IO
'
' Created by SharpDevelop.
' User: e80655
' Date: 2008-01-02
' Time: 11:02 AM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Public Class cITCgen

    Private mFilePath As String = String.Empty
    Private mLibraryPID As String = String.Empty

    Public Sub New(ByVal libraryPID As String, ByVal imagePath As String)

        mFilePath = imagePath
        mLibraryPID = libraryPID

    End Sub

    Public Sub Save(ByVal trackID As String)

        Dim baseDir As String = path.Combine(environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "iTunes\Album Artwork\Local\" + mLibraryPID)

        Dim sb As New System.Text.StringBuilder
        For i As Integer = 1 To 3
            sb.Append(Convert.ToInt32(trackId(trackId.Length - i), 16).ToString("00"))
            sb.Append(Path.DirectorySeparatorChar)
        Next

        Dim artworkDir As String = path.Combine(baseDir, sb.ToString)

        If directory.Exists(artworkDir) = False Then
            directory.CreateDirectory(artworkDir)
        End If

        Dim artworkName As String = String.Format("{0}-{1}.itc", mLibraryPID, trackID)
        Dim artworkPath As String = Path.Combine(artworkDir, artworkName)

        Dim artworkStream As Stream = File.Create(artworkPath)
        Dim bw As New BinaryWriter(artworkStream)

        ' 1 to 4
        bw.Write(Convert.ToByte(Convert.ToInt32("00", 16)))
        bw.Write(Convert.ToByte(Convert.ToInt32("00", 16)))
        bw.Write(Convert.ToByte(Convert.ToInt32("01", 16)))
        bw.Write(Convert.ToByte(Convert.ToInt32("1c", 16)))

        ' 5 to 8
        bw.Write(Convert.ToByte(Convert.ToInt32("69", 16)))
        bw.Write(Convert.ToByte(Convert.ToInt32("74", 16)))
        bw.Write(Convert.ToByte(Convert.ToInt32("63", 16)))
        bw.Write(Convert.ToByte(Convert.ToInt32("68", 16)))

        ' 	bytes 9-24: purpose unknown.
        For i As Integer = 1 To 3
            bw.Write(Convert.ToByte(Convert.ToInt32("00", 16)))
            bw.Write(Convert.ToByte(Convert.ToInt32("00", 16)))
            bw.Write(Convert.ToByte(Convert.ToInt32("00", 16)))
            bw.Write(Convert.ToByte(Convert.ToInt32("02", 16)))
        Next
        For i As Integer = 1 To 4
            bw.Write(Convert.ToByte(Convert.ToInt32("00", 16)))
        Next

        ' 	bytes 25-28 (chars): purpose unknown.
        ' Spells out "artw" (artwork?).
        bw.Write(Convert.ToByte(Convert.ToInt32("61", 16)))
        bw.Write(Convert.ToByte(Convert.ToInt32("72", 16)))
        bw.Write(Convert.ToByte(Convert.ToInt32("74", 16)))
        bw.Write(Convert.ToByte(Convert.ToInt32("77", 16)))

        ' 		In all cases seen so far, it is 256 consecutive null bytes (00).
        For i As Integer = 1 To 256
            bw.Write(Convert.ToByte(Convert.ToInt32("00", 16)))
        Next

        Dim fi As New FileInfo(mFilePath)
        Dim b() As Byte = BitConverter.GetBytes(fi.Length - 284)
        bw.Write(b(3))
        bw.Write(b(2))
        bw.Write(b(1))
        bw.Write(b(0))

        ' 	bytes 289-292 (chars): purpose unknown.
        ' Spells out "item".
        bw.Write(Convert.ToByte(Convert.ToInt32("69", 16)))
        bw.Write(Convert.ToByte(Convert.ToInt32("74", 16)))
        bw.Write(Convert.ToByte(Convert.ToInt32("65", 16)))
        bw.Write(Convert.ToByte(Convert.ToInt32("6d", 16)))

        ' bytes 293-296 (unsigned 32-bit integer):
        ' variable that self-describes the offset to
        ' the beginning of the image stream from the beginning of section 2.
        bw.Write(Convert.ToByte(Convert.ToInt32("00", 16)))
        bw.Write(Convert.ToByte(Convert.ToInt32("00", 16)))
        bw.Write(Convert.ToByte(Convert.ToInt32("00", 16)))
        bw.Write(Convert.ToByte(Convert.ToInt32("d8", 16)))

        ' Immediately following the Data Header length is 16 bytes of disposable information.
        For i As Integer = 1 To 4
            bw.Write(Convert.ToByte(Convert.ToInt32("00", 16)))
            bw.Write(Convert.ToByte(Convert.ToInt32("00", 16)))
            bw.Write(Convert.ToByte(Convert.ToInt32("00", 16)))
            bw.Write(Convert.ToByte(Convert.ToInt32("01", 16)))
        Next

        For i As Integer = 1 To 4
            bw.Write(Convert.ToByte(Convert.ToInt32("00", 16)))
        Next

        ' Library Persistent ID (8 bytes)
        Dim s As String = String.Empty
        For i As Integer = 0 To mLibraryPID.Length - 2
            s = String.Concat(mLibraryPID(i), mLibraryPID(i + 1))
            i += 1
            bw.Write(Convert.ToByte(Convert.ToInt32(s, 16)))
        Next

        ' Track Persistent ID (8 bytes)
        For i As Integer = 0 To trackID.Length - 2
            s = String.Concat(trackID(i), trackID(i + 1))
            i += 1
            'Console.Writeline(s)
            bw.Write(Convert.ToByte(Convert.ToInt32(s, 16)))
        Next

        ' Download/persistence indicator (4 bytes)
        bw.Write(Convert.ToByte(Convert.ToInt32("6c", 16)))
        bw.Write(Convert.ToByte(Convert.ToInt32("6f", 16)))
        bw.Write(Convert.ToByte(Convert.ToInt32("63", 16)))
        bw.Write(Convert.ToByte(Convert.ToInt32("6c", 16)))

        ' Pseudo-File Format (4 bytes)
        Dim ext As String = Path.GetExtension(mFilePath).ToLower
        If ext.Equals(".png") Then
            bw.Write(Convert.ToByte(Convert.ToInt32("50", 16)))
            bw.Write(Convert.ToByte(Convert.ToInt32("4e", 16)))
            bw.Write(Convert.ToByte(Convert.ToInt32("47", 16)))
            bw.Write(Convert.ToByte(Convert.ToInt32("66", 16)))
        Else
            bw.Write(Convert.ToByte(Convert.ToInt32("00", 16)))
            bw.Write(Convert.ToByte(Convert.ToInt32("00", 16)))
            bw.Write(Convert.ToByte(Convert.ToInt32("00", 16)))
            bw.Write(Convert.ToByte(Convert.ToInt32("0d", 16)))
        End If

        ' 00 00 00 03
        bw.Write(Convert.ToByte(Convert.ToInt32("00", 16)))
        bw.Write(Convert.ToByte(Convert.ToInt32("00", 16)))
        bw.Write(Convert.ToByte(Convert.ToInt32("00", 16)))
        bw.Write(Convert.ToByte(Convert.ToInt32("03", 16)))

        ' The next four bytes are an unsigned integer value indicating the width of the embedded image.

        Dim bp As Image = Bitmap.FromFile(mFilePath)
        b = BitConverter.GetBytes(bp.Width)
        bw.Write(b(3))
        bw.Write(b(2))
        bw.Write(b(1))
        bw.Write(b(0))

        b = BitConverter.GetBytes(bp.Height)
        bw.Write(b(3))
        bw.Write(b(2))
        bw.Write(b(1))
        bw.Write(b(0))
        bp.Dispose()

        ' 	bytes 349-360: purpose unknown.
        For i As Integer = 1 To 4
            bw.Write(Convert.ToByte(Convert.ToInt32("00", 16))) '349, 350, 351, 352
        Next
        bw.Write(Convert.ToByte(Convert.ToInt32("00", 16))) ' 353
        bw.Write(Convert.ToByte(Convert.ToInt32("00", 16))) ' 354
        bw.Write(Convert.ToByte(Convert.ToInt32("d9", 16))) ' 355
        bw.Write(Convert.ToByte(Convert.ToInt32("d9", 16))) ' 356

        bw.Write(Convert.ToByte(Convert.ToInt32("8a", 16))) ' 357
        bw.Write(Convert.ToByte(Convert.ToInt32("65", 16))) ' 358
        bw.Write(Convert.ToByte(Convert.ToInt32("00", 16))) ' 359
        bw.Write(Convert.ToByte(Convert.ToInt32("01", 16))) ' 360

        ' bytes 361-488: purpose unknown.
        ' In most cases seen so far, this is a string of null bytes.
        For i As Integer = 1 To 128
            bw.Write(Convert.ToByte(Convert.ToInt32("00", 16)))
        Next

        ' bytes 489-492: purpose unknown.
        ' In all cases viewed, spells "data". Probably used as the header end marker.
        bw.Write(Convert.ToByte(Convert.ToInt32("64", 16))) ' d
        bw.Write(Convert.ToByte(Convert.ToInt32("61", 16))) ' a
        bw.Write(Convert.ToByte(Convert.ToInt32("74", 16))) ' t
        bw.Write(Convert.ToByte(Convert.ToInt32("61", 16))) ' a

        For i As Integer = 1 To 4
            bw.Write(Convert.ToByte(Convert.ToInt32("00", 16))) '
        Next

        bw.Write(file.ReadAllBytes(mFilePath))


        bw.Close()

    End Sub

    '
    '	Filename is two sixteen character hexadecimal strings tied together with a hyphen
    '	followed by the filename extension .itc. The first hexadecimal string is always
    '	the Library Persistent ID (can be seen in the iTunes Music Library.xml file).
    '	The second hexadecimal string is the Track Persistent ID _if_ the file is located
    '	in the Local subfolder hierarchy of the Album Artwork folder.
    '
    '	Directory structure: On a Mac, the files will reside in: ~/Music/iTunes/Album Artwork/
    '	This folder has two subfolders, Download/ and Local/. Inside each of these folders
    '	will be one folder with the Library Persistent ID as its name. I assume that if
    '	you have multiple iTunes libraries, there will be additional folders with the
    '	corresponding Library Persistent IDs.
    '
    '	Then you must traverse three layers of folders, each with two digit decimal labels,
    '	corresponding in reverse order to the last three hexadecimal digits prior to the
    '	.itc filename extension. e.g., if your downloaded .itc file ends with A01.itc, it
    '	will reside in:
    '	~/Music/iTunes/Album Artwork/Download/"Library Persistent ID"/01/00/10/fooA01.itc

    ' for 3D82AC91DD2D58B0 as library id and 3D82AC91DD2D58B0 as trackID
    ' the path would be consideering 8B0 => 0/B/8 = 00/11/08
    ' /Music/iTunes/Album Artwork/Local/3D82AC91DD2D58B0/00/11/08/3D82AC91DD2D58B0-3D82AC91DD2D58B0.itc

End Class
