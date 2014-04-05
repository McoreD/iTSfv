using System;
using System.Runtime.InteropServices;

namespace McoreSystem
{
	/// <summary>
	/// Summary description for ServiceInstaller.
	/// </summary>
	public class ServiceAdmin
	{
		#region Private Variables
  
//		private string _servicePath;
//		private string _serviceName;
//		private string _serviceDisplayName;
 
		#endregion Private Variables
 
		#region DLLImport
  
		[DllImport("advapi32.dll")]
		private static extern IntPtr OpenSCManager(string lpMachineName,string lpSCDB, int scParameter);
		[DllImport("Advapi32.dll")]
		private static extern IntPtr CreateService(IntPtr SC_HANDLE,string lpSvcName,string lpDisplayName, 
			int dwDesiredAccess,int dwServiceType,int dwStartType,int dwErrorControl,string lpPathName, 
			string lpLoadOrderGroup,int lpdwTagId,string lpDependencies,string lpServiceStartName,string lpPassword);
		[DllImport("advapi32.dll")]
		private static extern void CloseServiceHandle(IntPtr SCHANDLE);
		[DllImport("advapi32.dll")]
		private static extern int StartService(IntPtr SVHANDLE,int dwNumServiceArgs,string lpServiceArgVectors);
  
		[DllImport("advapi32.dll",SetLastError=true)]
		private static extern IntPtr OpenService(IntPtr SCHANDLE,string lpSvcName,int dwNumServiceArgs);
		[DllImport("advapi32.dll")]
		private static extern int DeleteService(IntPtr SVHANDLE);
 
		[DllImport("kernel32.dll")]
		private static extern int GetLastError();
  
		#endregion DLLImport
 
  
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		#region Main method + testing code 
		[STAThread]
		static void Main(string[] args)
		{
     
			// TODO: Add code to start application here
     
    
			#region Testing
			//  Testing --------------
			string ServicePath;
			string ServiceName;
			string ServiceDisplayName;
     
			//path to the service that you want to install
			ServicePath = @"C:\build\service\Debug\Service.exe";
			ServiceName= "Service Name";
			ServiceDisplayName="Service Display Name";  
			ServiceAdmin c = new ServiceAdmin();
			c.InstallService(ServicePath, ServiceName, ServiceDisplayName);
  			Console.Read();
			// Testing --------------
			#endregion Testing
			}
		#endregion Main method + testing code - Commented
 

		/// <summary>
		/// Adds the service description to the registry.
		/// </summary>
		/// <param name="serviceName"></param>
		/// <param name="description"></param>
		public virtual void AddServiceDescriptionToRegistry(string serviceName, string description)
		{
			Microsoft.Win32.RegistryKey system;
			Microsoft.Win32.RegistryKey    currentControlSet; //HKEY_LOCAL_MACHINE\Services\CurrentControlSet
			Microsoft.Win32.RegistryKey services; //...\Services
			Microsoft.Win32.RegistryKey service; //...\<Service Name>
			Microsoft.Win32.RegistryKey config; //...\Parameters - this is where you can put service-specific configuration
			try
			{
				//Open the HKEY_LOCAL_MACHINE\SYSTEM key
				system = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("System");
				//Open CurrentControlSet
				currentControlSet = system.OpenSubKey("CurrentControlSet");
				//Go to the services key
				services = currentControlSet.OpenSubKey("Services");
				//Open the key for your service, and allow writing
				service = services.OpenSubKey(serviceName, true);
				//Add your service's description as a REG_SZ value named "Description"
				service.SetValue("Description", description);
				//(Optional) Add some custom information your service will use...
				config = service.CreateSubKey("Parameters");
			}
			catch(Exception)
			{
				//Log.Error("Error occurred while attempting to add a service description to the registry.", e);
			}
		}
		/// <summary>
		/// Removes the service description from the registry.
		/// </summary>
		/// <param name="serviceName"></param>
		public virtual void RemoveServiceDescriptionFromRegistry(string serviceName)
		{
			Microsoft.Win32.RegistryKey system;
			Microsoft.Win32.RegistryKey    currentControlSet; //HKEY_LOCAL_MACHINE\Services\CurrentControlSet
			Microsoft.Win32.RegistryKey services; //...\Services
			Microsoft.Win32.RegistryKey service; //...\<Service Name>
		    // Microsoft.Win32.RegistryKey config; //...\Parameters - this is where you can put service-specific configuration
			try
			{
				//Drill down to the service key and open it with write permission
				system = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("System");
				currentControlSet = system.OpenSubKey("CurrentControlSet");
				services = currentControlSet.OpenSubKey("Services");
				service = services.OpenSubKey(serviceName, true);
				//Delete any keys you created during installation (or that your service created)
				service.DeleteSubKeyTree("Parameters");
				//...
			}
			catch(Exception)
			{
				//Log.Error("Error occurred while trying to remove the service description from the registry.", e);
			}
		}
 
		/// <summary>
		/// This method installs and runs the service in the service conrol manager.
		/// </summary>
		/// <param name="ServicePath">The complete path of the service.</param>
		/// <param name="ServiceName">Name of the service.</param>
		/// <param name="ServiceDisplayName">Display name of the service.</param>
		/// <returns>True if the process went thro successfully. False if there was any error.</returns>
		public bool InstallService(string ServicePath, string ServiceName, string ServiceDisplayName)
		{
			#region Constants declaration.
			int SC_MANAGER_CREATE_SERVICE = 0x0002;
			int SERVICE_WIN32_OWN_PROCESS = 0x00000010;
			//int SERVICE_DEMAND_START = 0x00000003;
			int SERVICE_ERROR_NORMAL = 0x00000001;
 
			int STANDARD_RIGHTS_REQUIRED = 0xF0000;
			int SERVICE_QUERY_CONFIG       =    0x0001;
			int SERVICE_CHANGE_CONFIG       =   0x0002;
			int SERVICE_QUERY_STATUS           =  0x0004;
			int SERVICE_ENUMERATE_DEPENDENTS   = 0x0008;
			int SERVICE_START                  =0x0010;
			int SERVICE_STOP                   =0x0020;
			int SERVICE_PAUSE_CONTINUE         =0x0040;
			int SERVICE_INTERROGATE            =0x0080;
			int SERVICE_USER_DEFINED_CONTROL   =0x0100;
 
			int SERVICE_ALL_ACCESS             =  (STANDARD_RIGHTS_REQUIRED     | 
				SERVICE_QUERY_CONFIG         |
				SERVICE_CHANGE_CONFIG        |
				SERVICE_QUERY_STATUS         | 
				SERVICE_ENUMERATE_DEPENDENTS | 
				SERVICE_START                | 
				SERVICE_STOP                 | 
				SERVICE_PAUSE_CONTINUE       | 
				SERVICE_INTERROGATE          | 
				SERVICE_USER_DEFINED_CONTROL);
			int SERVICE_AUTO_START = 0x00000002;
			#endregion Constants declaration.
 
			try
			{
				IntPtr  sc_handle = OpenSCManager(null,null,SC_MANAGER_CREATE_SERVICE);
 
				if (sc_handle.ToInt32() != 0)
				{
					IntPtr sv_handle = CreateService(sc_handle,ServiceName,ServiceDisplayName,SERVICE_ALL_ACCESS,SERVICE_WIN32_OWN_PROCESS, SERVICE_AUTO_START,SERVICE_ERROR_NORMAL,ServicePath,null,0,null,null,null);
 
					if(sv_handle.ToInt32() ==0)
					{
 
						CloseServiceHandle(sc_handle);
						return false;
					}
					else
					{
						//now trying to start the service
						int i = StartService(sv_handle,0,null);
						// If the value i is zero, then there was an error starting the service.
						// note: error may arise if the service is already running or some other problem.
						if(i==0)
						{
							//Console.WriteLine("Couldnt start service");
							return false;
						}
						//Console.WriteLine("Success");
						CloseServiceHandle(sc_handle);
						return true;
					}
				}
				else
					//Console.WriteLine("SCM not opened successfully");
					return false;
 
			}
			catch(Exception e)
			{
				throw e;
			}
		}
 
  
		/// <summary>
		/// This method uninstalls the service from the service conrol manager.
		/// </summary>
		/// <param name="ServiceName">Name of the service to uninstall.</param>
		public bool UnInstallService(string ServiceName)
		{
			int GENERIC_WRITE = 0x40000000;
			IntPtr sc_hndl = OpenSCManager(null,null,GENERIC_WRITE);
 
			if(sc_hndl.ToInt32() !=0)
			{
				int DELETE = 0x10000;
				IntPtr svc_hndl = OpenService(sc_hndl,ServiceName,DELETE);
				//Console.WriteLine(svc_hndl.ToInt32());
				if(svc_hndl.ToInt32() !=0)
				{ 
					int i = DeleteService(svc_hndl);
					if (i != 0)
					{
						CloseServiceHandle(sc_hndl);
						return true;
					}
					else
					{
						CloseServiceHandle(sc_hndl);
						return false;
					}
				}
				else
					return false;
			}
			else
				return false;
		}
	}

}
