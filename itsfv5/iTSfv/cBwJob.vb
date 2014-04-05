
Public Class cBwJob

    Public Sub New(ByVal myJob As JobType)
        mJobType = myJob
    End Sub

    Enum JobType

        NEW_TASK
        ADD_NEW_TRACKS
        ADJUST_RATINGS
        COMMAND_LINE
        COPY_TAGS_TO_CLIPBOARD
        DELETE_DEAD_FOREIGN_TRACKS
        DELETE_EMPTY_FOLDERS
        EDIT_SELECTED_TRACKS
        EXPORT_ARTWORK_MANUAL
        EXPORT_ARTWORK_BATCH
        EXPORT_TRACKS
        FIND_NEW_TRACKS_FROM_HDD
        IMPORT_PLAYEDCOUNT_LASTFM
        IMPORT_POPM_PCNT
        INITIALIZE_PLAYER
        RELOAD_DISCS_TO_ALBUM_BROWSER
        OFFSET_TRACKNUMBER
        OVERRIDE_TAGS
        RATINGS_BACKUP
        RATINGS_RESTORE
        RECOVER_TAGS
        REMOVE_DUPLICATE_TRACKS
        REPLACE_WORD
        REPLACE_TRACKS
        SCHEDULE_DO        
        STATISTICS_DO
        SYNCHROCLEAN
        SYNC_MEDIA_CENTER_CACHE
        TRACKNUMBER_RENUMBER
        TRIM_CHAR
        UPDATE_PROGRESS_BAR
        UPDATE_STATUS_BAR
        VALIDATE_TRACKS_SELECTED
        VALIDATE_DISC
        VALIDATE_DISC_ADVANCED
        VALIDATE_LIBRARY
        WRITE_POPM_PCNT

    End Enum

    Public Enum ProgressType


        ADD_DISC_TO_LISTBOX_DISCS
        ADD_TRACKS_TO_LIBRARY
        ADD_TRACKS_TO_LISTBOX_TRACKS
        ADJUSTING_RATING
        ANALYSING_ALBUM
        APPEND_PREPEND_CHAR
        BACKINGUP_RATINGS
        CAPITALIZE_FIRST_LETTER
        CLEANING_TEMP_DIR
        CLEAR_DISCS_LISTBOX
        CLEAR_TRACKS_LISTBOX
        DECOMPILE_TRACKS
        DELETE_ARTWORK
        DELETE_TRACKS_DEAD
        DELETE_TRACKS_DEAD_ALIEN
        DETERINE_WHERE_MOST_MUSIC_IS
        EDITING_SELECTED_TRACKS
        EMAIL_SENDING
        EMBEDDING_LYRICS
        EXPORT_TRACKS
        FOUND_LYRICS_FOR
        GETTING_TRACK_INFO
        IMPORTING_ARTWORK
        IMPORTING_POPM_PCNT
        INCREMENT_DISC_PROGRESS
        INCREMENT_TRACK_PROGRESS
        INITIALIZE_ITUNES_ERROR
        INITIALIZE_ITUNES_XML_DATABASE_START
        INITIALIZE_PLAYER_FINISH
        INITIALIZE_PLAYER_LIBRARY_START
        LOAD_ARTWORK_DIMENSIONS
        OFFSET_TRACKNUMBER
        PARSING_ITUNES_LIBRARY
        PARSING_ITUNES_XML_DATABASE
        READ_TRACKS_FROM_DISCS
        READY
        RECOVERING_TRACKS
        REMOVE_TRACK_FROM_LISTBOX
        REMOVING_TAGS
        REPLACING_TAGS
        REPLACING_TRACKS
        RESTORE_STATUS_BAR_MESSAGE
        RESTORING_RATINGS
        SAVE_PLAYLIST
        SCANNING_FILE_IN_HDD
        SCANNING_TRACK_IN_ITUNES_XML_DATABASE
        SCANNING_TRACK_IN_PLAYER_LIBRARY
        SEARCHING_ITMS_ARTWORK
        SEND_APP_TO_TRAY
        SET_ACTIVE_TAB
        SET_PROGRESS_BAR_CONTINUOUS
        SET_PROGRESS_BAR_MARQUEE
        SHOW_STATISTICS_WINDOW
        TRIMMING_CHAR
        UPDATE_APP_TITLE
        UPDATE_DISCS_PROGRESS_BAR_MAX
        UPDATE_STATUSBAR_TEXT
        UPDATE_THUMBNAIL_IN_ARTIST_FOLDER
        UPDATE_TRACKS_PROGRESS_BAR_MAX
        UPDATING_TRACK
        VALIDATING_DISC_IN_ITUNES_LIBRARY
        WRITE_APPLICATION_SETTINGS
        WRITE_ARTWORK_CACHE
        WRITING_TAGS_TO_TRACKS
        ZIPPING_FILES

    End Enum

    Private mJobType As JobType = JobType.UPDATE_PROGRESS_BAR
    Public mMessage As String

    Public Property Job() As JobType
        Get
            Return mJobType
        End Get
        Set(ByVal value As JobType)
            mJobType = value
        End Set
    End Property

    Private mJobCancelled As Boolean = False
    Public Property JobCancelled() As Boolean
        Get
            Return mJobCancelled
        End Get
        Set(ByVal value As Boolean)
            mJobCancelled = value
        End Set
    End Property

    Private mData As New Object
    Public Property TaskData() As Object
        Get
            Return mData
        End Get
        Set(ByVal value As Object)
            mData = value
        End Set
    End Property

End Class

