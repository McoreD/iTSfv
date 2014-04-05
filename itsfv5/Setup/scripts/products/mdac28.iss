[CustomMessages]
mdac28_title=MDAC 2.8

en.mdac28_size=5.4 MB
de.mdac28_size=5,4 MB


[Run]
Filename: "{ini:{tmp}{\}dep.ini,install,mdac28}"; Description: "{cm:mdac28_title}"; StatusMsg: "{cm:depinstall_status,{cm:mdac28_title}}"; Parameters: "/q:a /c:""install /q /l"""; Flags: skipifdoesntexist

[Code]
const
	mdac28_url = 'http://download.microsoft.com/download/c/d/f/cdfd58f1-3973-4c51-8851-49ae3777586f/MDAC_TYP.EXE';

procedure mdac28(MinVersion: string);
var
	version: string;
begin
	// Check for required MDAC installation
	RegQueryStringValue(HKLM, 'Software\Microsoft\DataAccess', 'FullInstallVer', version);
	if (version < MinVersion) then
	   InstallPackage('mdac28', 'mdac28.exe', CustomMessage('mdac28_title'), CustomMessage('mdac28_size'), mdac28_url);
end;