using System;
using System.Reflection;

namespace AlbumArtDownloader.Scripts
{
	/// <summary>
	/// Wraps a script that has been written as a set of static methods as an IScript implementing object
	/// Supports the static public members:
	/// Methods:
	/// void GetThumbs(IScriptResults results, string artist, string album)
	/// object GetResult()
	/// 
	/// Properties (only getter required)
	/// string SourceName
	/// string SourceVersion
	/// string SourceCreator
	/// </summary>
	public class StaticScript : IScript, ICategorised
	{
		private MethodInfo mGetThumbsMethod;
		private MethodInfo mGetResultMethod;
		private PropertyInfo mNameProperty;
		private PropertyInfo mVersionProperty;
		private PropertyInfo mCreatorProperty;
		private PropertyInfo mCategoryProperty;
		
		public StaticScript(Type staticMethodImplementer)
		{
			mGetThumbsMethod = staticMethodImplementer.GetMethod("GetThumbs", BindingFlags.Static | BindingFlags.Public);
			mGetResultMethod = staticMethodImplementer.GetMethod("GetResult", BindingFlags.Static | BindingFlags.Public);

			mNameProperty = staticMethodImplementer.GetProperty("SourceName", BindingFlags.Static | BindingFlags.Public);
			mVersionProperty = staticMethodImplementer.GetProperty("SourceVersion", BindingFlags.Static | BindingFlags.Public);
			mCreatorProperty = staticMethodImplementer.GetProperty("SourceCreator", BindingFlags.Static | BindingFlags.Public);
			mCategoryProperty = staticMethodImplementer.GetProperty("SourceCategory", BindingFlags.Static | BindingFlags.Public);

			if (mGetThumbsMethod == null || mGetResultMethod == null)
				throw new InvalidOperationException("Static method implementer must implement both 'void GetThumbs(IScriptResults results, string artist, string album)' and 'object GetResult()'");
		}

		public string Name
		{
			get 
			{
				if(mNameProperty != null)
					return mNameProperty.GetValue(null, null).ToString();

				return null;
			}
		}

		public string Author
		{
			get 
			{
				if(mCreatorProperty != null)
					return mCreatorProperty.GetValue(null, null).ToString();

				return null;
			}
		}

		public string Version
		{
			get 
			{
				if(mVersionProperty != null)
					return mVersionProperty.GetValue(null, null).ToString();

				return null;
			}
		}

		public string Category
		{
			get
			{
				if (mCategoryProperty != null)
					return mCategoryProperty.GetValue(null, null).ToString();

				return null;
			}
		}

		public void Search(string artist, string album, IScriptResults results)
		{
			mGetThumbsMethod.Invoke(null, new object[] { results, artist, album });
		}

		public object RetrieveFullSizeImage(object fullSizeCallbackParameter)
		{
			return mGetResultMethod.Invoke(null, new object[] { fullSizeCallbackParameter });
		}
	}
}
