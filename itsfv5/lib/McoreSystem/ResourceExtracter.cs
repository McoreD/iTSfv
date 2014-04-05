using System;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace McoreSystem
{
	/// <summary>
	/// Summary description for IO.
	/// </summary>
	public class ResourceExtracter
	{
		public ResourceExtracter()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public string GetText(System.Reflection.Assembly sender, string name)
		{
			Stream oStrm = sender.GetManifestResourceStream(sender.GetName().Name + "." + name);
			// read contents of embedded file
			StreamReader oRdr = new StreamReader(oStrm);
			return oRdr.ReadToEnd();
		}
      
		public Image GetImage(System.Reflection.Assembly sender, string name) 
		{
			//Assembly oAsm = System.Reflection.Assembly.GetExecutingAssembly();
			Stream oStrm = sender.GetManifestResourceStream(sender.GetName().Name + "." + name);

			return Image.FromStream(oStrm);
		}

		[DllImport("shell32.dll")]
		public static extern IntPtr ExtractIcon(
			IntPtr hWnd,			// handle to destination window
			string lpszExeFileName,			// first message parameter
			int nIconIndex			// second message parameter
			);


		public Image GetIcon(string filePath, int index) 
		{
							
			IntPtr hIcon = ExtractIcon(Process.GetCurrentProcess().Handle, filePath, index);
			Icon ic = Icon.FromHandle(hIcon);
			return ic.ToBitmap();

		}

		public string ExportAssembly(System.Reflection.Assembly sender, string name, string where)
		{
			byte[] byteExeFile;
			//Assembly myAssembly=Assembly.GetExecutingAssembly();
			Stream myStream = sender.GetManifestResourceStream(sender.GetName().Name + "." +name);
			byteExeFile=new Byte[myStream.Length];
			myStream.Read(byteExeFile,0,(int)myStream.Length);
			FileStream myTempFile=new FileStream(where+"\\"+name,FileMode.Create);
			myTempFile.Write(byteExeFile,0,(int)myStream.Length);
			myTempFile.Close();
			return where+"\\"+name;

		}

		public string GetTextFromFile(string FilePath)
		{
			FileStream str = null;
			StreamReader sr = null;

			try
				{
			
					str = new FileStream(FilePath, FileMode.Open);
					sr = new StreamReader(str);
					return sr.ReadToEnd();

				}
				catch (Exception)
				{

				}
				finally
				{
					sr.Close();
					str.Close();
				}

					return null;
				
			}
		}
	}

