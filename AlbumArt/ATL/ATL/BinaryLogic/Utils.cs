using System;
using System.IO;

namespace ATL.AudioReaders.BinaryLogic
{
	/// <summary>
	/// Misc. utilities used by binary readers
	/// </summary>
	public class Utils
	{	
		// Size of the buffer used for memory stream copies
		// (see CopyMemoryStreamFrom method)
		private const int BUFFERSIZE = 1;


		/// <summary>
		/// Converts a whole char array to a string
		/// </summary>
		/// <param name="arr">Char array to be converted</param>
		/// <returns>String built from the whole char array</returns>
		public static String GetStringFromCharArray(char[] arr)
		{
			return GetStringFromCharArray(arr,0,arr.Length);
		}


		/// <summary>
		/// Converts a part of a char array to a string
		/// </summary>
		/// <param name="arr">Char array to be converted</param>
		/// <param name="offset">First character to be converted</param>
		/// <param name="count">Number of characters to convert</param>
		/// <returns>String built from the specified partof the array</returns>
		public static String GetStringFromCharArray(char[] arr, int offset, int count)
		{
			String result = "";

			for (int i=offset; (i<arr.Length) && (i<offset+count); i++) result += arr[i];

			return result;
		}


		/// <summary>
		/// Determines if the contents of a string (character by character) is the same
		/// as the contents of a char array
		/// </summary>
		/// <param name="a">String to be tested</param>
		/// <param name="b">Char array to be tested</param>
		/// <returns>True if both contain the same character sequence; false if not</returns>
		public static bool StringEqualsArr(String a, char[] b)
		{			
			if (b.Length != a.Length) return false;
			for (int i=0; i<b.Length; i++)
			{
				if (a[i] != b[i]) return false;
			}
			return true;
		}


		/// <summary>
		/// Determines if two char arrays have the same contents
		/// </summary>
		/// <param name="a">First array to be tested</param>
		/// <param name="b">Second array to be tested</param>
		/// <returns>True if both arrays have the same contents; false if not</returns>
		public static bool ArrEqualsArr(char[] a, char[] b)
		{			
			if (b.Length != a.Length) return false;
			for (int i=0; i<b.Length; i++)
			{
				if (a[i] != b[i]) return false;
			}
			return true;
		}


		/// <summary>
		/// Reads a given number of one-byte chars from the provided source
		/// (this method is there because the default behaviour of .NET's binary char reading
		/// tries to read unicode stuff, thus reading two bytes in a row from time to time :S)
		/// </summary>
		/// <param name="r">Source to read from</param>
		/// <param name="length">Number of one-byte chars to read</param>
		/// <returns>Array of chars read from the source</returns>
		public static char[] ReadTrueChars(BinaryReader r, int length)
		{
			byte[] byteArr;
			char[] result = new char[length];

			byteArr = r.ReadBytes(length);
			for (int i=0; i<length; i++)
			{
				result[i] = (char)byteArr[i];
			}

			return result;
		}

		/// <summary>
		/// Reads one one-byte char from the provided source
		/// </summary>
		/// <param name="r">Source to read from</param>
		/// <returns>Chars read from the source</returns>
		public static char ReadTrueChar(BinaryReader r)
		{
			return (char)r.ReadByte();
		}

		/// <summary>
		/// Copies a given number of bytes from a stream to another
		/// </summary>
		/// <param name="mTo">Target stream</param>
		/// <param name="mFrom">Source stream</param>
		/// <param name="length">Number of bytes to be copied</param>
		public static void CopyMemoryStreamFrom(Stream mTo, Stream mFrom, long length)
		{
			BinaryWriter w = new BinaryWriter(mTo);
			BinaryReader r = new BinaryReader(mFrom);
			CopyMemoryStreamFrom(w, r, length);
		}

		/// <summary>
		/// Builds a String from an array of chars, following the C null-terminated convention
		/// </summary>
		/// <param name="str">Array of chars to build the string from</param>
		/// <returns>Resulting string</returns>
		public static String BuildStringCStyle(char[] str)
		{
			String result = "";
			int i=0;
			while ((i<str.Length) && (str[i] != '\0'))
			{
				result = result + str[i];
				i++;
			}
			
			return result;
		}

		/// <summary>
		/// Writes a given number of bytes from a stream to a writer
		/// </summary>
		/// <param name="mTo">Writer to be used</param>
		/// <param name="mFrom">Source stream</param>
		/// <param name="length">Number of bytes to be copied</param>
		public static void CopyMemoryStreamFrom(BinaryWriter w, Stream mFrom, long length)
		{
			BinaryReader r = new BinaryReader(mFrom);			
			CopyMemoryStreamFrom(w, r, length);
		}

		/// <summary>
		/// Writes a given number of bytes from a reader to a stream
		/// </summary>
		/// <param name="mTo">Target stream</param>
		/// <param name="r">Reader to be used</param>
		/// <param name="length">Number of bytes to be copied</param>
		public static void CopyMemoryStreamFrom(Stream mTo, BinaryReader r, long length)
		{
			BinaryWriter w = new BinaryWriter(mTo);
			CopyMemoryStreamFrom(w, r, length);
		}

		/// <summary>
		/// Writes a given number of bytes from a reader to a writer
		/// </summary>
		/// <param name="mTo">Writer to be used</param>
		/// <param name="r">Reader to be used</param>
		/// <param name="length">Number of bytes to be copied</param>
		public static void CopyMemoryStreamFrom(BinaryWriter w, BinaryReader r, long length)
		{			
			long effectiveLength;
			long initialPosition;

			initialPosition = r.BaseStream.Position;
			if (0 == length) effectiveLength = r.BaseStream.Length; else effectiveLength = length;

			while (r.BaseStream.Position < initialPosition+effectiveLength)
				w.Write(r.ReadBytes(BUFFERSIZE));
		}
	}
}
