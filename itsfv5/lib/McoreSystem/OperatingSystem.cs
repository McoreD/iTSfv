using System;

namespace McoreSystem
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class OperatingSystem
	{
		private string osInfo="";
		public OperatingSystem()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public string GetOSInfo()
		{  
			System.OperatingSystem m_os = System.Environment.OSVersion;  
			string m_osName = "Unknown";  

			switch(m_os.Platform) 
			{   
				case System.PlatformID.Win32Windows:     
				switch(m_os.Version.Minor)     
				{       
					case 0:         
						m_osName = "Windows 95";
						break;       
					case 10:         
						m_osName = "Windows 98";         
						break;       
					case 90:         
						m_osName = "Windows ME";         
						break;     
				}     
					break;   

				case System.PlatformID.Win32NT:     
				switch(m_os.Version.Major)     
				{       
					case 3:         
						m_osName = "Windws NT 3.51";
						break;       
					case 4:         
						m_osName = "Windows NT 4"; 
						break;       
					case 5:         
						if(m_os.Version.Minor == 0)
							m_osName = "Windows 2000"; 
						else if(m_os.Version.Minor == 1)
							m_osName = "Windows XP";
						else if(m_os.Version.Minor == 2)       
							m_osName = "Windows Server 2003";         
						break;       
					case 6:          
						m_osName = "Windows Vista";         
						break;     
				}     
					break;  
			}  
			this.osInfo = m_osName + ";" + m_os.Version.ToString();
			return osInfo;
			
		}

		public string getOSName()
		{
			GetOSInfo();
			string[] name = this.osInfo.Split(';');
			return name[0];
		}

		public string getOSVersion()
		{
			GetOSInfo();
			string[] name = this.osInfo.Split(';');
			return name[1];
		}




	}
}
