
Namespace My
    
    'This class allows you to handle specific events on the settings class:
    ' The SettingChanging event is raised before a setting's value is changed.
    ' The PropertyChanged event is raised after a setting's value is changed.
    ' The SettingsLoaded event is raised after the setting values are loaded.
    ' The SettingsSaving event is raised before the setting values are saved.
    Partial Friend NotInheritable Class MySettings

        <Global.System.Configuration.UserScopedSettingAttribute(), _
   Global.System.Diagnostics.DebuggerNonUserCodeAttribute(), _
   Global.System.Configuration.DefaultSettingValueAttribute("COPY")> _
  Public Property AddFilesMode() As AddFilesType
            Get
                Return CType(Me("AddFilesMode"), AddFilesType)
            End Get
            Set(ByVal value As AddFilesType)
                Me("AddFilesMode") = value
            End Set
        End Property

    End Class
End Namespace
