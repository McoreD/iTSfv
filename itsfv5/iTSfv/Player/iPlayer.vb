Public Interface iPlayer


    '******************************
    '* LoadLibrary
    '******************************
    ' iterates through the player's library and 
    ' adds tracks to list of discs
    ' adds discs to list of albums
    ' adds albums to list of albumartist

    Function LoadLibrary() As Boolean
    Function SelectedTracks() As List(Of cXmlTrack)

    Overloads ReadOnly Property Progress() As Integer


End Interface
