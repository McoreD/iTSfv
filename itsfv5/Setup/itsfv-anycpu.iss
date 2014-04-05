#include "scripts\products.iss"

#include "scripts\products\winversion.iss"
#include "scripts\products\fileversion.iss"
//#include "scripts\products\iis.iss"
#include "scripts\products\kb835732.iss"

#include "scripts\products\msi20.iss"
#include "scripts\products\msi31.iss"
//#include "scripts\products\ie6.iss"

//#include "scripts\products\dotnetfx11.iss"
//#include "scripts\products\dotnetfx11lp.iss"
//#include "scripts\products\dotnetfx11sp1.iss"

#include "scripts\products\dotnetfx20.iss"
//#include "scripts\products\dotnetfx20lp.iss"
#include "scripts\products\dotnetfx20sp1.iss"
//#include "scripts\products\dotnetfx20sp1lp.iss"

#include "scripts\products\dotnetfx35.iss"
//#include "scripts\products\dotnetfx35lp.iss"
#include "scripts\products\dotnetfx35sp1.iss"
//#include "scripts\products\dotnetfx35sp1lp.iss"

//#include "scripts\products\mdac28.iss"
//#include "scripts\products\jet4sp8.iss"

#define ExeName "iTSfv"		
#define ExePath "..\iTSfv\bin\iTSfv.exe"
#define MyAppVersion GetFileVersion(ExePath)

[CustomMessages]
win2000sp3_title=Windows 2000 Service Pack 3
winxpsp2_title=Windows XP Service Pack 2


[Setup]
AppName={#ExeName}
AppVerName={#ExeName} {#MyAppVersion}
AppVersion={#MyAppVersion}
VersionInfoVersion={#MyAppVersion}
VersionInfoTextVersion={#MyAppVersion}
VersionInfoDescription=iTunes Store file validator
VersionInfoCompany=BetaONE
AppPublisher=BetaONE
AppPublisherURL=http://code.google.com/p/itsfv
AppSupportURL=http://code.google.com/p/itsfv
AppUpdatesURL=http://code.google.com/p/itsfv
DefaultDirName={pf}\iTSfv
DefaultGroupName=BetaONE\iTSfv
AllowNoIcons=yes
InfoBeforeFile=..\iTSfv\VersionHistory.txt
;InfoAfterFile=..\TorrentDescriptionMaker\ReleaseInfo.txt
SolidCompression=yes
PrivilegesRequired=none
OutputDir=..\..\Output\
OutputBaseFilename={#ExeName}-{#MyAppVersion}-setup
ArchitecturesInstallIn64BitMode=x64 ia64
DirExistsWarning=no
CreateAppDir=true
UsePreviousGroup=yes
UsePreviousAppDir=yes
ShowUndisplayableLanguages=no
LanguageDetectionMethod=uilanguage
InternalCompressLevel=ultra64
Compression=lzma

;required by products
MinVersion=4.1,5.0
;PrivilegesRequired=admin
;ArchitecturesAllowed=x86

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "de"; MessagesFile: "compiler:Languages\German.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\iTSfv\bin\iTSfv.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\iTSfv\bin\*.dll"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\iTSfv"; Filename: "{app}\iTSfv.exe"
;Name: "{group}\iTSfv Manual"; Filename: "{app}\iTSfv-manual.pdf"
Name: "{userdesktop}\iTSfv"; Filename: "{app}\iTSfv.exe"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\iTSfv"; Filename: "{app}\iTSfv.exe."; Tasks: quicklaunchicon

[Run]
Filename: "{app}\iTSfv.exe"; Description: "{cm:LaunchProgram,iTSfv}"; Flags: nowait postinstall skipifsilent
;Filename: "{app}\manual-iTSfv.pdf"; Description: "{cm:LaunchProgram,iTSfv Manual}"; Flags: nowait unchecked postinstall shellexec skipifsilent

[InstallDelete]
Type: files; Name: "{app}\CommandLineParser.dll"

[Code]
function InitializeSetup(): Boolean;
begin
	initwinversion();

	if (not minspversion(5, 0, 3)) then begin
		MsgBox(FmtMessage(CustomMessage('depinstall_missing'), [CustomMessage('win2000sp3_title')]), mbError, MB_OK);
		exit;
	end;
	if (not minspversion(5, 1, 2)) then begin
		MsgBox(FmtMessage(CustomMessage('depinstall_missing'), [CustomMessage('winxpsp2_title')]), mbError, MB_OK);
		exit;
	end;
	
	//if (not iis()) then exit;
	
	msi20('2.0');
	msi31('3.1');
//	ie6('5.0.2919');
	
	//dotnetfx11();
	//dotnetfx11lp();
	//dotnetfx11sp1();
	
	kb835732();
	
	if (minwinversion(5, 0) and minspversion(5, 0, 4)) then begin
		dotnetfx20sp1();
//		dotnetfx20sp1lp();
	end else begin
		dotnetfx20();
//		dotnetfx20lp();
	end;
	
//dotnetfx35();
	//dotnetfx35lp();
	//dotnetfx35sp1();
	//dotnetfx35sp1lp();
	
//	mdac28('2.7');
//	jet4sp8('4.0.8015');
	
	Result := true;
end;
