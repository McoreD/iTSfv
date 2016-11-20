!define PRODUCT_NAME "Album Art Downloader XUI"
!define PRODUCT_VERSION "0.37.1"
!define PRODUCT_WEB_SITE "http://sourceforge.net/projects/album-art"
!define PRODUCT_SUPPORT "http://www.hydrogenaudio.org/forums/index.php?showtopic=57392"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\AlbumArt.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

!include x64.nsh
!include Sections.nsh
!macro SecDisable SecId
 !insertmacro UnselectSection ${SecId}
 !insertmacro SetSectionFlag ${SecId} ${SF_RO}
!macroend
!define DisableSection '!insertmacro SecDisable'
!macro SecEnable SecId
 !insertmacro ClearSectionFlag ${SecId} ${SF_RO}
!macroend
!define EnableSection '!insertmacro SecEnable'

#Set Admin execution so that Vista and 7 install the start menu shortcuts to the right place, and uninstall them too.
RequestExecutionLevel admin

SetCompressor lzma

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"

OutFile "../../Releases/AlbumArtDownloaderXUI-${PRODUCT_VERSION}.exe"
InstallDir "$PROGRAMFILES64\AlbumArtDownloader"
Icon "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
UninstallIcon "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
LicenseData "License.txt"
ShowInstDetails hide
ShowUnInstDetails show
XPStyle on

Page license
Page directory
Page components
Page instfiles

Function .oninit
  #Check for .net presence
  ReadRegStr $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5" "Install"
  StrCmp $0 "1" dotnetok
  MessageBox MB_ICONEXCLAMATION|MB_YESNO "The Microsoft .NET Framework version 3.5 is not installed.$\nPlease download and install the framework before installing ${PRODUCT_NAME}.$\n$\nWould you like to visit the download page now?" IDNO +2
  ExecShell "open" "http://www.microsoft.com/downloads/details.aspx?FamilyId=AB99342F-5D1A-413D-8319-81DA479AB0D7"
  Abort
  dotnetok:
FunctionEnd

SectionGroup /e "Album Art Downloader"
  Section "!Program files" ProgramFiles
    SetOutPath "$INSTDIR"
    CreateDirectory "$INSTDIR\Scripts"
    
    #Remove the old MediaInfo dll, if present
    Delete "$INSTDIR\MediaInfo.dll"
    
    File "License.txt"
    File "..\AlbumArtDownloader\AlbumArtDownloader.ico"
    File "..\AlbumArtDownloader\bin\Release\AlbumArt.exe"
    File "..\AlbumArtDownloader\bin\Release\*.dll"
  SectionEnd
  Section "Add icon to Start Menu" StartMenu
    CreateDirectory "$SMPROGRAMS\Album Art Downloader"
    CreateShortCut "$SMPROGRAMS\Album Art Downloader\Album Art Downloader.lnk" "$INSTDIR\AlbumArt.exe"
    CreateShortCut "$SMPROGRAMS\Album Art Downloader\Uninstall.lnk" "$INSTDIR\uninst.exe"
  SectionEnd
  Section /o "Add to Explorer context menu" ContextMenu
    WriteRegStr HKCR "Folder\shell\AlbumArtDownloader" "" "Browse for Album Art..."
    WriteRegStr HKCR "Folder\shell\AlbumArtDownloader\command" "" "$\"$INSTDIR\AlbumArt.exe$\" /fileBrowser $\"%1$\""
  SectionEnd
SectionGroupEnd

Function .onSelChange
#Make Add icon to StartMenu and Context Menu dependent on having Program Files selected.
Push $R0

  SectionGetFlags ${ProgramFiles} $R0
  IntOp $R0 $R0 & ${SF_SELECTED}
  StrCmp $R0 ${SF_SELECTED} EnableDependents
  #DisableDependents
  ${DisableSection} ${StartMenu}
  ${DisableSection} ${ContextMenu}
  Goto EnableDependentsEnd
  #DisableDependentsEnd
  
  EnableDependents:
  ${EnableSection} ${StartMenu}
  ${EnableSection} ${ContextMenu}
  EnableDependentsEnd:

Pop $R0
FunctionEnd

Section "Command Line Interface (aad.exe)"
  SetOutPath "$INSTDIR"
  File "..\CommandLineInterface\bin\Release\aad.exe"
  File "..\CommandLineInterface\bin\Release\*.dll"
SectionEnd

Section -ScriptsPath
  SetOutPath "$INSTDIR\Scripts"
  SetOverwrite ifnewer
  #delete old script cache file
  Delete "$INSTDIR\Scripts\boo script cache.dll"
  File "..\Scripts\Scripts\util.boo"
  
  #delete obsolete scripts
  Delete "$INSTDIR\Scripts\iTunes.boo"
  Delete "$INSTDIR\Scripts\rateyourmusic.boo"
  Delete "$INSTDIR\Scripts\amazon-com-mp3.boo"  
  Delete "$INSTDIR\Scripts\cdcoverhideout.boo"
SectionEnd

SectionGroup "Image Download Scripts"
	SectionGroup "General"
		SectionGroup "Amazon"
			Section "Amazon (US)"
			  File "..\Scripts\Scripts\amazon-common.boo"
			  File "..\Scripts\Scripts\amazon-com.boo"
			SectionEnd
			Section /o "Amazon (UK)"
			  File "..\Scripts\Scripts\amazon-common.boo"
			  File "..\Scripts\Scripts\amazon-co-uk.boo"
			SectionEnd
			Section /o "Amazon (CA)"
			  File "..\Scripts\Scripts\amazon-common.boo"
			  File "..\Scripts\Scripts\amazon-ca.boo"
			SectionEnd
			Section /o "Amazon (DE)"
			  File "..\Scripts\Scripts\amazon-common.boo"
			  File "..\Scripts\Scripts\amazon-de.boo"
			SectionEnd
			Section /o "Amazon (FR)"
			  File "..\Scripts\Scripts\amazon-common.boo"
			  File "..\Scripts\Scripts\amazon-fr.boo"
			SectionEnd
			Section /o "Amazon (JP)"
			  File "..\Scripts\Scripts\amazon-common.boo"
			  File "..\Scripts\Scripts\amazon-jp.boo"
			SectionEnd
		SectionGroupEnd
		Section "Google"
		  File "..\Scripts\Scripts\google.boo"
		SectionEnd
		Section "7digital"
		  File "..\Scripts\Scripts\7digital.boo"
		SectionEnd
		Section "Coveralia"
		  File "..\Scripts\Scripts\coveralia.boo"
		SectionEnd
		Section "Cover-Paradies"
		  File "..\Scripts\Scripts\cover-paradies.boo"
		SectionEnd
		Section "CD Universe"
		  File "..\Scripts\Scripts\cduniverse.boo"
		SectionEnd
		Section "Discogs"
		  File "..\Scripts\Scripts\discogs.boo"
		SectionEnd
		Section "CoverIsland"
		  File "..\Scripts\Scripts\coverisland.boo"
		SectionEnd
		Section "FreeCovers"
		  #Remove old freecovers script
		  Delete "$INSTDIR\Scripts\freecovers.boo"
		  File "..\Scripts\Scripts\freecovers-api.boo"
		SectionEnd
		#Rate Your Music script currently inoperational due to defensive site changes
		#Section "Rate Your Music"
		#  File "..\Scripts\Scripts\rateyourmusic.boo"
		#SectionEnd
		#AlbumArtExchange removed on request from site owner
		#Section "Album Art Exchange"
		#  File "..\Scripts\Scripts\albumartexchange.boo"
		#SectionEnd
		Section "DarkTown"
		  File "..\Scripts\Scripts\darktown.boo"
		SectionEnd
		Section "AllCdCover"
		  File "..\Scripts\Scripts\allcdcover.boo"
		SectionEnd
		Section "Buy.com"
		  File "..\Scripts\Scripts\buy-com.boo"
		SectionEnd
		Section "hitparade.ch"
		  File "..\Scripts\Scripts\hitparade.boo"
		SectionEnd
		Section "LastFM"
		  File "..\Scripts\Scripts\lastfm-cover.boo"
		SectionEnd
		Section "Archambault"
		  File "..\Scripts\Scripts\archambault.boo"
		SectionEnd
		Section "HMV Canada"
		  File "..\Scripts\Scripts\hmv-canada.boo"
		SectionEnd
		Section "Kalahari (South Africa)"
		  File "..\Scripts\Scripts\kalahari.boo"
		SectionEnd
		Section "Take2 (South Africa)"
		  File "..\Scripts\Scripts\take2.boo"
		SectionEnd
		#iTunes script currently blocked by apple, so don't include it
		#Section "iTunes Music Shop"
		#  File "..\Scripts\Scripts\iTunes.boo"
		#SectionEnd
	SectionGroupEnd
	
	SectionGroup "Dance, Trance"
		Section "Juno Records"
		  File "..\Scripts\Scripts\juno-records.boo"
		SectionEnd
		Section "PsyShop"
		  File "..\Scripts\Scripts\psyshop.boo"
		SectionEnd
	SectionGroupEnd
	
	SectionGroup "Punk, Metal, Rock"
		Section "Encyclopaedia Metallum"
		  File "..\Scripts\Scripts\metal-archives.boo"
		SectionEnd
		Section "Metal Library"
		  File "..\Scripts\Scripts\metallibrary.boo"
		SectionEnd
		Section "RevHQ"
		  File "..\Scripts\Scripts\revhq.boo"
		SectionEnd
		#Section "MusicMight"
		#  File "..\Scripts\Scripts\musicmight.boo"
		#SectionEnd
	SectionGroupEnd
	
	SectionGroup "Independent"
		Section "CDBaby"
		  File "..\Scripts\Scripts\cdbaby.boo"
		SectionEnd
	SectionGroupEnd

	SectionGroup "Eastern"
		Section "Yes24"
		  File "..\Scripts\Scripts\yes24.boo"
		SectionEnd
		Section "YesAsia"
		  File "..\Scripts\Scripts\yesasia.boo"
		SectionEnd
		Section "maniadb"
		  File "..\Scripts\Scripts\maniadb.boo"
		SectionEnd
	SectionGroupEnd

	SectionGroup "Classical"
		Section "ArkivMusic"
		  File "..\Scripts\Scripts\arkivmusic.boo"
		SectionEnd
	SectionGroupEnd
	
	SectionGroup "Video Game Music"
		Section "VGMdb"
		  File "..\Scripts\Scripts\vgmdb.boo"
		SectionEnd
	SectionGroupEnd
	
	SectionGroup "Artist Images"
		Section "LastFM Artist"
			File "..\Scripts\Scripts\lastfm-artist.boo"
		SectionEnd
	SectionGroupEnd
SectionGroupEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\AlbumArt.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\AlbumArtDownloader.ico"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "HelpLink" "${PRODUCT_SUPPORT}"
  WriteRegDword ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "NoModify" "1"
  WriteRegDword ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "NoRepair" "1"
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
  Delete "$INSTDIR\aad.exe"
  Delete "$INSTDIR\License.txt"
  Delete "$INSTDIR\errorlog.txt"
  Delete "$INSTDIR\AlbumArtDownloader.ico"
  Delete "$INSTDIR\*.dll"
  Delete "$INSTDIR\Scripts\*.boo"
  RMDir "$INSTDIR\Scripts"
  RMDir "$INSTDIR"

  Delete "$SMPROGRAMS\Album Art Downloader\Uninstall.lnk"
  Delete "$SMPROGRAMS\Album Art Downloader\Album Art Downloader.lnk"
  RMDir "$SMPROGRAMS\Album Art Downloader"

  #delete local app data
  FindFirst $0 $1 "$LOCALAPPDATA\AlbumArtDownloader\AlbumArt.exe_*"
  loopAlbumArt:
    StrCmp $1 "" doneAlbumArt
    RMDir /r "$LOCALAPPDATA\AlbumArtDownloader\$1"
    FindNext $0 $1
    Goto loopAlbumArt
  doneAlbumArt:

  FindFirst $0 $1 "$LOCALAPPDATA\AlbumArtDownloader\aad.exe_*"
  loopAAD:
    StrCmp $1 "" doneAAD
    RMDir /r "$LOCALAPPDATA\AlbumArtDownloader\$1"
    FindNext $0 $1
    Goto loopAAD
  doneAAD:

  RMDir "$LOCALAPPDATA\AlbumArtDownloader"

  DeleteRegKey HKCR "Folder\shell\AlbumArtDownloader"
  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  SetAutoClose true
SectionEnd