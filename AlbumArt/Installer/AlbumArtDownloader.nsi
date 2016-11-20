!include "DotNET.nsh"
!include LogicLib.nsh

!define PRODUCT_NAME "Album Art Downloader"
!define PRODUCT_VERSION "0.6-alpha2"
!define PRODUCT_WEB_SITE "https://sourceforge.net/projects/album-art"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\AlbumArt.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"
!define DOTNET_VERSION "2.0"

SetCompressor lzma

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"

OutFile "../Releases/AlbumArtDownloader-${PRODUCT_VERSION}.exe"
InstallDir "$PROGRAMFILES\AlbumArtDownloader"
Icon "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
UninstallIcon "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
LicenseData "..\AlbumArt\Resources\License.txt"
ShowInstDetails hide
ShowUnInstDetails show
XPStyle on

Page license
Page directory
Page components
Page instfiles

Section "!Album Art Downloader"
  !insertmacro CheckDotNET ${DOTNET_VERSION}
  
  SetOutPath "$INSTDIR"
  CreateDirectory "$INSTDIR\Scripts"
  File "..\AlbumArt\Resources\License.txt"
  File "..\AlbumArt\bin\Release\AlbumArt.exe"
  File "..\AlbumArt\bin\Release\*.dll"
  CreateDirectory "$SMPROGRAMS\Album Art Downloader"
  CreateShortCut "$SMPROGRAMS\Album Art Downloader\Album Art Downloader.lnk" "$INSTDIR\AlbumArt.exe"
SectionEnd

SectionGroup "Image Download Scripts"
Section -ScriptsPath
SetOutPath "$INSTDIR\Scripts"
SetOverwrite ifnewer
SectionEnd

Section "Amazon (US)"
  File "..\AlbumArt\Scripts\amazon.boo"
SectionEnd
Section "Google"
  File "..\AlbumArt\Scripts\google.boo"
SectionEnd
Section "Coveralia"
  File "..\AlbumArt\Scripts\coveralia.boo"
SectionEnd
Section "Cover-Paradies"
  File "..\AlbumArt\Scripts\cover-paradies.boo"
SectionEnd
Section "CD Universe"
  File "..\AlbumArt\Scripts\cduniverse.boo"
SectionEnd
Section "Discogs"
  File "..\AlbumArt\Scripts\discogs.boo"
SectionEnd
Section "Yes24"
  File "..\AlbumArt\Scripts\yes24.boo"
SectionEnd
Section "Juno Records"
  File "..\AlbumArt\Scripts\juno-records.boo"
SectionEnd
Section "CoverIsland"
  File "..\AlbumArt\Scripts\coverisland.boo"
SectionEnd
Section "Artists.Trivialbeing (artist images)"
  File "..\AlbumArt\Scripts\artists.trivialbeing.boo"
SectionEnd

Section "-Other Scripts"
  File /nonfatal "..\AlbumArt\Scripts\*.boo"
SectionEnd

SectionGroupEnd

Section -AdditionalIcons
  CreateShortCut "$SMPROGRAMS\Album Art Downloader\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\*.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\*.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
SectionEnd

Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully removed from your computer."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove $(^Name) and all of its components?" IDYES +2
  Abort
FunctionEnd

Section Uninstall
  Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\AlbumArt.exe"
  Delete "$INSTDIR\License.txt"
  Delete "$INSTDIR\*.dll"
  Delete "$INSTDIR\Scripts\*.boo"
  Delete "$INSTDIR\Scripts\ScriptCache.dll"
  Delete "$INSTDIR\Scripts\ScriptCache.pdb"
  
  Delete "$SMPROGRAMS\Album Art Downloader\Uninstall.lnk"
  Delete "$SMPROGRAMS\Album Art Downloader\Album Art Downloader.lnk"

  #delete local app data
  FindFirst $0 $1 "$LOCALAPPDATA\Marc_Landis\AlbumArt.exe_*"
  loop:
    StrCmp $1 "" done
    RMDir /r "$LOCALAPPDATA\Marc_Landis\$1"
    FindNext $0 $1
    Goto loop
  done:

  RMDir "$LOCALAPPDATA\Marc_Landis"

  RMDir "$SMPROGRAMS\Album Art Downloader"
  RMDir "$INSTDIR\Scripts"
  RMDir "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  SetAutoClose true
SectionEnd