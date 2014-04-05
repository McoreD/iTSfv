// WARNING: express setup (downloads and installs the components depending on your OS)
// http://www.microsoft.com/downloads/details.aspx?FamilyID=1E1550CB-5E5D-48F5-B02B-20B602228DE

[CustomMessages]
ie6_title=Internet Explorer 6

en.ie6_size=46 MB
de.ie6_size=46 MB


[Run]
Filename: "{ini:{tmp}{\}dep.ini,install,ie6}"; Description: "{cm:ie6_title}"; StatusMsg: "{cm:depinstall_status,{cm:ie6_title}}"; Parameters: "/q:a /C:""setup /QNT"""; Flags: skipifdoesntexist

[Code]
const
	ie6_url = 'http://download.microsoft.com/download/ie6sp1/finrel/6_sp1/W98NT42KMeXP/EN-US/ie6setup.exe';

procedure ie6(MinVersion: string);
var
	version: string;
begin
	RegQueryStringValue(HKLM, 'Software\Microsoft\Internet Explorer', 'Version', version);
	if (version < MinVersion) then
		InstallPackage('ie6', 'ie6.exe', CustomMessage('ie6_title'), CustomMessage('ie6_size'), ie6_url);
end;