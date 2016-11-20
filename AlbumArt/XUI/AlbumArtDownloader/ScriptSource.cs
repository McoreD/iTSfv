using System;
using System.Threading;
using AlbumArtDownloader.Scripts;

namespace AlbumArtDownloader
{
	internal class ScriptSource : Source
	{
		private IScript mScript;
		
		public ScriptSource(IScript script)
		{
			mScript = script;
		}

		public override string Name
		{
			get { return mScript.Name; }
		}
		public override string Author
		{
			get { return mScript.Author; }
		}
		public override string Version
		{
			get { return mScript.Version; }
		}
		public override string Category
		{
			get
			{
				var categorised = mScript as ICategorised;
				if (categorised != null)
				{
					return categorised.Category;
				}

				return null;
			}
		}

		protected override void SearchInternal(string artist, string album, IScriptResults results)
		{
			try
			{
				mScript.Search(artist, album, results);
			}
			catch (ThreadAbortException) { } //Script was cancelled
			catch (Exception e)
			{
				string message;
				if (e is System.Reflection.TargetInvocationException)
				{
					message = ((System.Reflection.TargetInvocationException)e).InnerException.Message;
				}
				else
				{
					message = e.Message;
				}
				System.Diagnostics.Debug.Fail(String.Format("Script {0} threw an exception while searching: {1}", mScript.Name, message));
			}

		}

		internal override byte[] RetrieveFullSizeImageData(object fullSizeCallbackParameter)
		{
			object fullSizeImage = null;
			try
			{
				fullSizeImage = mScript.RetrieveFullSizeImage(fullSizeCallbackParameter);
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.Fail(String.Format("Script {0} threw an exception while retreiving full sized image: {1}", mScript.Name, e.Message));
			}

			return BitmapHelpers.GetBitmapData(fullSizeImage);
		}
	}
}
