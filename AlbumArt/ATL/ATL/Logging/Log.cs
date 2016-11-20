using System;
using System.Collections;

namespace ATL.Logging
{
	/// <summary>
	/// This class handles the logging of the application's messages
	/// </summary>
	public class Log
	{
		// Definition of the four levels of logging
		public const int LV_DEBUG		= 0x00000008;
		public const int LV_INFO		= 0x00000004;
		public const int LV_WARNING		= 0x00000002;
		public const int LV_ERROR		= 0x00000001;

		
		// Definition of a message ("a line of the log") 
		public struct LogItem
		{
			public DateTime When;	// Date of the message
			public int Level;		// Logging level
			public String Message;	// Contents of the message
		}


		// Storage structure containing each LogItem logged since last reset 
		private ArrayList logItems;
		
		// Storage structure containing each LogDevice registered by this class
		private ArrayList logDevices;

		// ---------------------------------------------------------------------------
		 
		/// <summary>
		/// Constructor
		/// </summary>
		public Log()
		{
			logItems = new ArrayList();
			logDevices = new ArrayList();
		}

		/// <summary>
		/// Logs the provided message with the LV_DEBUG logging level
		/// </summary>
		/// <param name="msg">Contents of the message</param>
		public void Debug(String msg)
		{
			Write(LV_DEBUG,msg);
		}

		/// <summary>
		/// Logs the provided message with the LV_INFO logging level
		/// </summary>
		/// <param name="msg">Contents of the message</param>
		public void Info(String msg)
		{
			Write(LV_INFO,msg);
		}

		/// <summary>
		/// Logs the provided message with the LV_WARNING logging level
		/// </summary>
		/// <param name="msg">Contents of the message</param>
		public void Warning(String msg)
		{
			Write(LV_WARNING,msg);
		}

		/// <summary>
		/// Logs the provided message with the LV_ERROR logging level
		/// </summary>
		/// <param name="msg">Contents of the message</param>
		public void Error(String msg)
		{
			Write(LV_ERROR,msg);
		}
		

		/// <summary>
		/// Logs the provided message with the provided logging level
		/// </summary>
		/// <param name="level">Logging level of the new message</param>
		/// <param name="msg">Contents of the new message</param>
		public void Write(int level, String msg)
		{
			// Creation and filling of the new LogItem
			LogItem theItem;

			theItem.When = DateTime.Now;
			theItem.Level = level;
			theItem.Message = msg;

			// Adding to the list of logged items
			logItems.Add(theItem);
			
			// Asks each registered LogDevice to log the new LogItem
			foreach (LogDevice aLogger in logDevices)
			{
				aLogger.DoLog(theItem);
			}
		}

		
		/// <summary>
		/// Clears the whole list of logged items
		/// </summary>
		public void ClearAll()
		{
			logItems.Clear();
		}


		/// <summary>
		/// Gets all the logged items 
		/// </summary>
		/// <returns>List of all the logged items</returns>
		public ArrayList GetAllItems()
		{
			return GetAllItems(0x0000000F);
		}

		 
		/// <summary>
		/// Gets the logged items whose logging level matches the provided mask 
		/// </summary>
		/// <param name="levelMask">Logging level mask</param>
		/// <returns>List of the matching logged items</returns>
		public ArrayList GetAllItems(int levelMask)
		{
			ArrayList result = new ArrayList();

			foreach(LogItem anItem in logItems)
			{
				if ( (levelMask & anItem.Level) > 0)
				{
					result.Add(anItem);
				}
			}
			return result;
		}


		/// <summary>
		/// Registers a LogDevice
		/// A registered LogDevice will be called each time a new LogItem is received
		/// (see Write method) 
		/// </summary>
		/// <param name="aLogger">Device to register</param>
		public void Register(LogDevice aLogger)
		{
			logDevices.Add(aLogger);
		}
	}
}
