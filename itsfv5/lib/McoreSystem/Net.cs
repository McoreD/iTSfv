using System;
using System.Data;
using System.Net;

namespace McoreSystem
{
	/// <summary>
	/// Summary description for Net.
	/// </summary>
	public class Net
	{
		public Net()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public string GetPrivateIP() 
		{
            IPHostEntry myIP = Dns.GetHostEntry(System.Net.Dns.GetHostName()); // VB7
           	return (myIP.AddressList.GetValue(0).ToString());
		}

		public string GetPublicIP()
		{
			try
			{
				DataSet ds = new DataSet();
				ds.ReadXml("http://www.showmyip.com/xml/");
				return System.Convert.ToString(ds.Tables[0].Rows[0]["ip"]);
			}
			catch
			{
				GetPrivateIP();
			}

			return null;

		}
	}
}
