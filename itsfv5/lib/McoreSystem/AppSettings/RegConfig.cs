using System;


namespace McoreSystem.AppSettings
{
	/// <summary>
	/// Summary description for McoreRegistry.
	/// </summary>
	public class RegConfig
	{

		private string strAppPath;

		public RegConfig()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public RegConfig(string CompanyName, string ProductName) 
		{
			this.strAppPath = "Software\\" + CompanyName + "\\" + ProductName;
		}

		public RegConfig(string CompanyName, string ProductName, string Version)
		{
			this.strAppPath = "Software\\" + CompanyName + "\\" + ProductName;

		}

		#region "Application Settings Manager via Registry"
		public void SaveSettings(string Title, string Value, Microsoft.Win32.RegistryValueKind kind)
		{
	
            
			Microsoft.Win32.RegistryKey regKey =
				Microsoft.Win32.Registry.CurrentUser.CreateSubKey(strAppPath);

			regKey.SetValue(Title, Value, kind);


		}

		public string GetSettings(string Title, string DefaultValue)

		{

			string strExport; 
	
			try
			{
				Microsoft.Win32.RegistryKey regKey =
					Microsoft.Win32.Registry.CurrentUser.OpenSubKey(strAppPath, true);

				if (regKey.GetValue(Title) != null)
				{
					strExport = System.Convert.ToString(regKey.GetValue(Title));

				}
				else
				{
					strExport = DefaultValue;
				}
			}
			catch (Exception)
			{
				strExport = DefaultValue;
			}

			return strExport;

		}

		public void SetRegPath(string CompanyName, string ProductName)

		{
			strAppPath = "Software\\" + CompanyName + "\\" + ProductName ;

		}

		public void DeleteSettings(string CompanyName, string ProductName)

		{
			strAppPath = "Software\\" + CompanyName ;

			Microsoft.Win32.RegistryKey regKey =
				Microsoft.Win32.Registry.CurrentUser.OpenSubKey(strAppPath, true);
			
			try
			{
				regKey.DeleteSubKeyTree(ProductName);
			}
			catch(Exception)
			{

			}
		}

		#endregion
	}
}
