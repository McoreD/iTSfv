// ***************************************************************************
//                                                                            
// Audio Tools Library (Freeware)                                             
// Class TVorbisComment - for manipulating with Vorbis Comments               
//                                                                            
// Copyright (c) 2003 by Erik Stenborg                                        
// 
// Version 1.0 (6 Jul 2003) Created                                           
//         1.1 (2 Oct 2003) Updated UTF-8 support to use Jcl Library
//		   14th May 2005 - Translated to C# by Zeugma 440
//                                                                            
// Todo: Fix array handling so that it allocates a longer array when inserting
//       a new tag field, or removing fields for that matter.                 
//                                                                            
// ***************************************************************************

using System;
using System.IO;

namespace ATL.AudioReaders.BinaryLogic
{
	class TVorbisComment
	{  
		protected String[] FComments;
		protected int[] FCommentLengths;
		protected int FCommentCount;
		protected String FVendor;
		protected int FVendorLength;
		protected bool FUpperCaseKeys;
		protected bool FUTF8Values;    
  
		// Due to the lack of [] indexers in C# properties, 
		// Key, Value and ValueI will be accessed from Getters and Setters

		public int Count
		{
			get { return this.FCommentCount; }
		}	
		public int Size
		{
			get { return this.GetSize(); }
		}	
		public String Vendor
		{
			get { return this.FVendor; }
		}

		// ---------------------------------------------------------------------------

		public int GetSize()
		{
			int result = 4 + FVendor.Length + 4 + FCommentCount * 4;
			for (int i=0; i< FCommentCount; i++)  
				result += FComments[i].Length;

			return result;
		}

		// ---------------------------------------------------------------------------

		public String GetKey(int index)
		{
			if (FUpperCaseKeys)
				return FComments[index].Substring(0, FComments[index].IndexOf("=") - 1).ToUpper();
			else
				return FComments[index].Substring(0, FComments[index].IndexOf("=") - 1);		
		}

		// ---------------------------------------------------------------------------

		public void SetKey(int Index, String Value)
		{
			if (Value != "")
				if (FUpperCaseKeys)
					FComments[Index] = Value.ToUpper() +
						FComments[Index].Substring( FComments[Index].IndexOf("="), Int32.MaxValue );
				else
					FComments[Index] = Value +
						FComments[Index].Substring( FComments[Index].IndexOf("="), Int32.MaxValue );
			else
				DeleteI(Index);
		}

		// ---------------------------------------------------------------------------

		public String GetValue(String Index)
		{
			int n;

			n = GetIndexOf(Index);
			if (n != -1)
				return GetValueI(n);
			else
				return "";
		}

		// ---------------------------------------------------------------------------

		public void SetValue(String Index, String Value)
		{
			int n;

			if (Index != "")
			{
				n = GetIndexOf(Index);
				if (n != -1)
					SetValueI(n, Value);
				else if ("" == Value)
				{
					FCommentCount = FCommentCount + 1;
					FComments[FCommentCount - 1] = Index.ToUpper() + "=" + Value;
				}
			}
		}

		// ---------------------------------------------------------------------------

		public String GetValueI(int Index)
		{
			if (FUTF8Values)
				return FComments[Index].Substring(1 + FComments[Index].IndexOf("="), Int32.MaxValue);
			else
				return FComments[Index].Substring(1 + FComments[Index].IndexOf("="), Int32.MaxValue);
		}

		// ---------------------------------------------------------------------------

		public void SetValueI(int Index, String Value)
		{
			if (Value != "")
				if (FUTF8Values)
					FComments[Index] = FComments[Index].Substring(1, FComments[Index].IndexOf("=")) + Value;
				else
					FComments[Index] = FComments[Index].Substring(1, FComments[Index].IndexOf("=")) + Value;
			else
				DeleteI(Index);
		}

		// ---------------------------------------------------------------------------

		public TVorbisComment()
		{  
			FUpperCaseKeys = true;
			FUTF8Values = true;
			Clear();
		}

		// ---------------------------------------------------------------------------

		// No explicit destructor with C#

		// ---------------------------------------------------------------------------

		public void Clear()
		{
			FVendor = "";
			FVendorLength = 0;
			/*SetLength(FComments, 0);
			SetLength(FCommentLengths, 0);*/
			FCommentCount = 0;
		}

		// ---------------------------------------------------------------------------

		public void LoadFromStream(BinaryReader Stream)
		{
			FVendorLength = Stream.ReadInt32();  
			char[] tempArray = new char[FVendorLength];

			for (int i=0; i<FVendorLength; i++) tempArray[i] = Stream.ReadChar();
		
			FVendor = new String(tempArray);

			FCommentCount = Stream.ReadInt32();  
	
			FComments = new String[FCommentCount];
			FCommentLengths = new int[FCommentCount];  

			for (int i=0; i<FCommentCount - 1; i++)
			{
				FCommentLengths[i] = Stream.ReadInt32();
				tempArray = new char[FCommentLengths[i]];
				for (int j=0; j<FCommentLengths[i]; j++) tempArray[j] = Stream.ReadChar();
			
				FComments[i] = new String(tempArray);
			}
		}

		// ---------------------------------------------------------------------------

		public void SaveToStream(BinaryWriter Stream)
		{
			int N;

			N = FVendor.Length;
			Stream.Write(N);
			for (int i=0; i<N; i++)
				Stream.Write(FVendor[i]); //# not null-terminated ??

			Stream.Write(FCommentCount);

			for (int i=0; i<FCommentCount; i++)
			{
				N = FComments[i].Length;
				Stream.Write(N);
				for (int j=0; j<N; j++)
					Stream.Write(FComments[i][j]);
			}
		}

		// ---------------------------------------------------------------------------

		public bool Valid()
		{
			return (10 <= FVendorLength && FVendorLength <= 50);
		}

		// ---------------------------------------------------------------------------

		public int GetIndexOf(String Index)
		{
			String S;

			S = Index.ToUpper();
			int result;
  
			for (result=0; result<Count; result++)
				if (S == GetKey(result).ToUpper())
					return result;
			return -1;
		}

		// ---------------------------------------------------------------------------

		public void DeleteI(int Index)
		{
			if ((Index >= 0) && (Index < Count))
			{
				FComments[Index] = FComments[Count - 1];
				FCommentCount = FCommentCount - 1;
			}
		}

		// ---------------------------------------------------------------------------

		public void Delete(String Index)
		{
			DeleteI(GetIndexOf(Index));
		}

		// ---------------------------------------------------------------------------

	}
}