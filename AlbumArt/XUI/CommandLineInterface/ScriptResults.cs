using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlbumArtDownloader.Scripts;

namespace AlbumArtDownloader
{
	internal class ScriptResults : IScriptResults
	{
		private IScript mScript;
		private List<ScriptResult> mResults = new List<ScriptResult>();

		public ScriptResults(IScript script)
		{
			mScript = script;
		}

		#region Redirects for obsolete members
		//This region can be copied and pasted for reuse
		public void SetCountEstimate(int count)
		{
			EstimatedCount = count;
		}
		public void AddThumb(string thumbnailUri, string name, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
		{
			Add(thumbnailUri, name, fullSizeImageWidth, fullSizeImageHeight, fullSizeImageCallback);
		}
		public void AddThumb(System.IO.Stream thumbnailStream, string name, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
		{
			Add(thumbnailStream, name, fullSizeImageWidth, fullSizeImageHeight, fullSizeImageCallback);
		}
		#endregion

		public int EstimatedCount { get; set;}

		public void Add(object thumbnail, string name, object fullSizeImageCallback)
		{
			Add(thumbnail, name, -1, -1, fullSizeImageCallback);
		}
		public void Add(object thumbnail, string name, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
		{
			Add(thumbnail, name, String.Empty, fullSizeImageWidth, fullSizeImageHeight, fullSizeImageCallback);
		}
		public void Add(object thumbnail, string name, string infoUri, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback)
		{
			Add(thumbnail, name, infoUri, fullSizeImageWidth, fullSizeImageHeight, fullSizeImageCallback, CoverType.Unknown);
		}
		public void Add(object thumbnail, string name, string infoUri, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback, CoverType coverType)
		{
			Add(thumbnail, name, infoUri, fullSizeImageWidth, fullSizeImageHeight, fullSizeImageCallback, coverType, null);
		}
		
		public void Add(object thumbnail, string name, string infoUri, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback, CoverType coverType, string suggestedFilenameExtension)
		{
			//InfoUri and suggestedFilenameExtension are ignored.
			mResults.Add(new ScriptResult(mScript, thumbnail, name, fullSizeImageWidth, fullSizeImageHeight, fullSizeImageCallback, coverType));
		}
		
		public IEnumerable<ScriptResult> Results { get { return mResults; } }
		
		
		public void Add(object thumbnail, string name, object fullSizeImageCallback, CoverType coverType)
		{
			Add(thumbnail, name, String.Empty, -1, -1, fullSizeImageCallback, coverType);
		}
	}
}
