using System;
using System.Xml;

namespace McoreSystem.AppSettings
{
	/// <summary>
	/// Summary description for XmlConfig.
	/// </summary>
	public class XmlConfig
	{

		private XmlDocument doc;

		public XmlConfig()
		{
			//
			// TODO: Add constructor logic here
			//
			doc = new XmlDocument();
			try	{doc.Load(this.getConfigFilename());}
			catch {}
		}

		#region "Application Settings Manager via XML"
		public void setAppSettingToXML (string key, object value)
		{
			//load xml document
			XmlDocument document = new XmlDocument();
			document.Load(this.getConfigFilename());
			
			//query for value
			XmlElement Node = ((XmlElement)(document.DocumentElement.SelectSingleNode("/configuration/appSettings/add[@key=\"" + key + "\"]")));
			
			if (Node != null)
			{
				//node or key  found so just set the value
				Node.Attributes.GetNamedItem("value").Value = System.Convert.ToString(value);
			}
			else
			{
				//node or key not found so create it
				Node = document.CreateElement("add");
				Node.SetAttribute("key", key);
				Node.SetAttribute("value", System.Convert.ToString(value));
				
				//set back to root
				XmlNode Root = document.DocumentElement.SelectSingleNode("/configuration/appSettings");
				
				//add node
				if (Root != null)
				{
					Root.AppendChild(Node);
				}
				else
				{
					try
					{
						//appSettings does not exist yet so add it
						Root = document.DocumentElement.SelectSingleNode("/configuration");
						Root.AppendChild(document.CreateElement("appSettings"));
						Root = document.DocumentElement.SelectSingleNode("/configuration/appSettings");
						Root.AppendChild(Node);
					}
					catch (Exception ex)
					{
						//if adding appsettings fails then throw error
						throw (new Exception("Could not set value", ex));
					}
				}
			}
			
			//save results
			document.Save(this.getConfigFilename());
		}
		
		public void setAppSettingToXML (string group, string key, object value)
		{
			//load xml document
			XmlDocument document = new XmlDocument();
			document.Load(this.getConfigFilename());
			
			//query for value
			XmlElement Node = ((XmlElement)(document.DocumentElement.SelectSingleNode("/configuration/" + group + "/add[@key=\"" + key + "\"]")));
			
			if (Node != null)
			{
				//node or key  found so just set the value
				Node.Attributes.GetNamedItem("value").Value = System.Convert.ToString(value);
			}
			else
			{
				//node or key not found so create it
				Node = document.CreateElement("add");
				Node.SetAttribute("key", key);
				Node.SetAttribute("value", System.Convert.ToString(value));
				
				//set back to root
				XmlNode Root = document.DocumentElement.SelectSingleNode("/configuration/" + group);
				
				//add node
				if (Root != null)
				{
					Root.AppendChild(Node);
				}
				else
				{
					try
					{
						//appSettings does not exist yet so add it
						Root = document.DocumentElement.SelectSingleNode("/configuration");
						Root.AppendChild(document.CreateElement(group));
						Root = document.DocumentElement.SelectSingleNode("/configuration/" + group);
						Root.AppendChild(Node);
					}
					catch (Exception ex)
					{
						//if adding appsettings fails then throw error
						throw (new Exception("Could not set value", ex));
					}
				}
			}
			
			//save results
			document.Save(this.getConfigFilename());
		}

		public int getItemsCountInGroup(string group)
		{
			//load xml document
			XmlDocument doc = new XmlDocument();
			Console.WriteLine(this.getConfigFilename());
			doc.Load(this.getConfigFilename());
			XmlNodeList Node = doc.SelectNodes("/configuration/"+group);
			return Node.Count;	
	
            
		}
		public object getAppSettingFromXML(string group, string key)
		{
			//load xml document
			XmlDocument document = new XmlDocument();
			document.Load(this.getConfigFilename());
			
			//query for value
			XmlNode Node = document.DocumentElement.SelectSingleNode("/configuration/" +group +"/add[@key=\"" + key + "\"]");
			
			
			//return found value or nothing if not found
			if (Node != null)
			{
				return Node.Attributes.GetNamedItem("value").Value;
				
			}
			else
			{
				return null;
			}
		}
		
		public object getAppSettingFromXML(string key)
		{
			//load xml document
			XmlDocument document = new XmlDocument();
			document.Load(this.getConfigFilename());
			
			//query for value
			XmlNode Node = document.DocumentElement.SelectSingleNode("/configuration/appSettings/add[@key=\"" + key + "\"]");
			
			//return found value or nothing if not found
			if (Node != null)
			{
				return Node.Attributes.GetNamedItem("value").Value;
			}
			else
			{
				return null;
			}
		}

		private string getConfigFilename()
		{
			
			//return current config filename of calling app
			string configFile = string.Concat(System.Reflection.Assembly.GetEntryAssembly().Location, ".config");
			return configFile;
		}
		
		private void SetValue (string key, object value)
		{
			//use document loaded in the constructor
			//this allows you to perform multiple set/gets without loading the document each time
			
			//query for value
			XmlElement Node = ((XmlElement)(doc.DocumentElement.SelectSingleNode("/configuration/appSettings/add[@key=\"" + key + "\"]")));
			
			if (Node != null)
			{
				//node or key  found so just set the value
				Node.Attributes.GetNamedItem("value").Value = System.Convert.ToString(value);
			}
			else
			{
				//node or key not found so create it
				Node = doc.CreateElement("add");
				Node.SetAttribute("key", key);
				Node.SetAttribute("value", System.Convert.ToString(value));
				
				//set back to root
				XmlNode Root = doc.DocumentElement.SelectSingleNode("/configuration/appSettings");
				
				//add node
				if (Root != null)
				{
					Root.AppendChild(Node);
				}
				else
				{
					try
					{
						//appSettings does not exist yet so add it
						Root = doc.DocumentElement.SelectSingleNode("/configuration");
						Root.AppendChild(doc.CreateElement("appSettings"));
						Root = doc.DocumentElement.SelectSingleNode("/configuration/appSettings");
						Root.AppendChild(Node);
					}
					catch (Exception ex)
					{
						//if adding appsettings fails then throw error
						throw (new Exception("Could not set value", ex));
					}
				}
			}
			
			//save results
			doc.Save(this.getConfigFilename());
		}
		
		private object GetValue(string key)
		{
			//use document loaded in the constructor
			//this allows you to perform multiple set/gets without loading the document each time
			
			//query for value
			XmlNode Node = doc.DocumentElement.SelectSingleNode("/configuration/appSettings/add[@key=\"" + key + "\"]");
			
			//return found value or nothing if not found
			if (Node != null)
			{
				return Node.Attributes.GetNamedItem("value").Value;
			}
			else
			{
				return null;
			}
		}
		
		#endregion

	}
}
