// http://www.microsoft.com/technet/security/bulletin/ms04-011.mspx
// http://www.microsoft.com/downloads/details.aspx?FamilyId=0692C27E-F63A-414C-B3EB-D2342FBB6C00

[CustomMessages]
en.kb835732_title=Security Update (KB835732)
de.kb835732_title=Sicherheitsupdate (KB835732)

en.kb835732_size=6.8 MB
de.kb835732_size=6,8 MB


[Run]
Filename: "{ini:{tmp}{\}dep.ini,install,kb835732}"; Description: "{cm:kb835732_title}"; StatusMsg: "{cm:depinstall_status,{cm:kb835732_title}}"; Parameters: "/q:a /c:""install /q"""; Flags: skipifdoesntexist

[Code]
const
	kb835732_url = 'http://download.microsoft.com/download/f/a/a/faa796aa-399d-437a-9284-c3536e9f2e6e/Windows2000-KB835732-x86-ENU.EXE';

procedure kb835732();
begin
	if (exactwinversion(5, 0) and minspversion(5, 0, 2) and maxspversion(5, 0, 4)) then begin		
		if (not RegKeyExists(HKLM, 'SOFTWARE\Microsoft\Updates\Windows 2000\SP5\KB835732\Filelist')) then
			InstallPackage('kb835732', 'kb835732.exe', CustomMessage('kb835732_title'), CustomMessage('kb835732_size'), kb835732_url);
	end;
end;