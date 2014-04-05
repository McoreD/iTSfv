#define MyAppName "iTSfv"
#define MyAppFile "iTSfvGUI.exe"
#define MyAppPath "itsfv6\iTSfvGUI\bin\Release\iTSfvGUI.exe"
#define MyAppVersion GetStringFileInfo(MyAppPath, "Assembly Version")
#define MyAppPublisher "iTSfv Developers"
#define MyAppURL "http://code.google.com/p/itsfv"

[Setup]
AllowNoIcons=true
AppCopyright=Copyright (C) 2013 {#MyAppPublisher}
AppId=BFCDFCFA-9A23-41E3-9022-10D289373C7A
AppMutex=Global\BFCDFCFA-9A23-41E3-9022-10D289373C7A
AppName={#MyAppName}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}/issues/list
AppUpdatesURL={#MyAppURL}/downloads/list
AppVerName={#MyAppName} {#MyAppVersion}
AppVersion={#MyAppVersion}
ArchitecturesAllowed=x86 x64 ia64
ArchitecturesInstallIn64BitMode=x64 ia64
Compression=lzma/ultra64
CreateAppDir=true
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DirExistsWarning=no
;InfoBeforeFile=Docs\VersionHistory.txt
;InfoBeforeFile=Docs\license.txt
InternalCompressLevel=ultra64
LanguageDetectionMethod=uilanguage
MinVersion=5.01sp3
OutputBaseFilename={#MyAppName}-{#MyAppVersion}-setup
OutputDir=Output\
PrivilegesRequired=none
ShowLanguageDialog=auto
ShowUndisplayableLanguages=false
SignedUninstaller=false
SolidCompression=true
Uninstallable=true
UninstallDisplayIcon={app}\{#MyAppFile}
UsePreviousAppDir=yes
UsePreviousGroup=yes
VersionInfoCompany={#MyAppPublisher}
VersionInfoTextVersion={#MyAppVersion}
VersionInfoVersion={#MyAppVersion}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "CreateDesktopIcon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"
Name: "CreateQuickLaunchIcon"; Description: "Create a quick launch shortcut"; GroupDescription: "Additional shortcuts:"; Flags: unchecked

[Files]
Source: "itsfv6\iTSfvGUI\bin\Release\*.exe"; Excludes: *.vshost.exe; DestDir: {app}; Flags: ignoreversion
Source: "itsfv6\iTSfvGUI\bin\Release\*.pdb"; DestDir: {app}; Flags: ignoreversion
Source: "itsfv6\iTSfvGUI\bin\Release\*.dll"; DestDir: {app}; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppFile}"; WorkingDir: "{app}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"; WorkingDir: "{app}"
Name: "{userdesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppFile}"; WorkingDir: "{app}"; Tasks: CreateDesktopIcon; Check: not DesktopIconExists
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppFile}"; WorkingDir: "{app}"; Tasks: CreateQuickLaunchIcon

[Run]
Filename: "{app}\{#MyAppFile}"; Description: {cm:LaunchProgram,{#MyAppName}}; Flags: nowait postinstall skipifsilent

[Code]
function DesktopIconExists(): Boolean;
begin
  Result := FileExists(ExpandConstant('{commondesktop}\{#MyAppName}.lnk'));
end;
