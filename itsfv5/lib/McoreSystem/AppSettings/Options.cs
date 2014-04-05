using System;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace McoreSystem.AppSettings
{
	public class Options
	{
		private Options(){}

		public static void Save(string filePath, object options)
		{

			Byte[] buffer = new Byte[80];
			MemoryStream ms;
			BinaryFormatter bf = new BinaryFormatter();

			System.Xml.XmlTextWriter xmlwriter = 
				new XmlTextWriter(filePath, System.Text.Encoding.Default);

			xmlwriter.Formatting = Formatting.Indented;
			xmlwriter.WriteStartDocument();

			//xmlwriter.WriteComment("Option File. Do not edit! (c)llewellyn@pritchard.org");
			xmlwriter.WriteStartElement(options.ToString());
		
			PropertyInfo[] props = options.GetType().GetProperties(
				BindingFlags.Public | 
				BindingFlags.Instance | 
				BindingFlags.SetField);

			foreach (PropertyInfo prop in props)
			{
				xmlwriter.WriteStartElement(prop.Name);

				object da = prop.GetValue(options, null);

				if (da != null) 
				{
					xmlwriter.WriteAttributeString("Value", da.ToString());

					ms = new MemoryStream();
					try 
					{
						bf.Serialize(ms, da);
						ms.Position = 0;
						int count = 0;
						do 
						{
							count = ms.Read(buffer, 0, buffer.Length);
							xmlwriter.WriteBase64(buffer, 0, count);
						}
						while ( count == buffer.Length);
					} 
					catch (System.Runtime.Serialization.SerializationException)
					{
						Console.WriteLine("SERIALIZATION FAILED: {0}", prop.Name);
					}

				}
				else xmlwriter.WriteAttributeString("Value", "null");

				xmlwriter.WriteEndElement();
			}
			xmlwriter.WriteEndElement();
			xmlwriter.WriteEndDocument();
			xmlwriter.Flush();
			xmlwriter.Close();
		}

		public static void Load(string filePath, object options)
		{
			Byte[] buffer = new Byte[80];
			MemoryStream ms;
			BinaryFormatter bf = new BinaryFormatter();

			System.Xml.XmlTextReader reader = new XmlTextReader(filePath);

			while (reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:

						if (reader.HasAttributes)
						{
							string name = reader.Name;
							string val = reader.GetAttribute("Value");

							ms = new MemoryStream();
		
							int count = 0;
							do 
							{
								count = reader.ReadBase64(buffer, 0 , buffer.Length);
								ms.Write(buffer, 0,count);
							}
							while (count == buffer.Length);

							ms.Position = 0;

							if (val != "null") 
							{
								try 
								{
									object da = bf.Deserialize(ms);

									Console.Write("Applying {0} : ", name);
									options.GetType().GetProperty(name).SetValue(options, da, null);
									Console.WriteLine("OK");
								}
								catch (System.Runtime.Serialization.SerializationException e)
								{
									Console.WriteLine("FAIL: {0}",e.Message);
								}
							}
						}
						break;
				}
			}
			reader.Close();
		}

        public static void SaveToFile(string filePath, object options)
        {
   
            FileStream fs = new FileStream(filePath, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, options);
            fs.Close();

        }
        public static void LoadFromFile(string filePath, object options)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            options = bf.Deserialize(fs);
            fs.Close();
        }

	}


}
