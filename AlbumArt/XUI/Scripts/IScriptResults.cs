using System;
using System.IO;

namespace AlbumArtDownloader.Scripts
{
	public interface IScriptResults
	{
		#region Obsolete calls, for backwards compatibility with old scripts
		[Obsolete("Use EstimatedCount instead")]
		void SetCountEstimate(int count);
		
		[Obsolete("AddThumb has been renamed to Add")]
		void AddThumb(string thumbnailUri,
						string name, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback);
		
		[Obsolete("AddThumb has been renamed to Add")]
		void AddThumb(Stream thumbnailStream,
						string name, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback);
		#endregion

		int EstimatedCount { get; set;}
		void Add(object thumbnail, string name, object fullSizeImageCallback, CoverType coverType);
		void Add(object thumbnail, string name, object fullSizeImageCallback);
		void Add(object thumbnail, string name, 				int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback);
		void Add(object thumbnail, string name, string infoUri, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback);
		void Add(object thumbnail, string name, string infoUri, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback, CoverType coverType);
		void Add(object thumbnail, string name, string infoUri, int fullSizeImageWidth, int fullSizeImageHeight, object fullSizeImageCallback, CoverType coverType, string suggestedFilenameExtension);		
	}
}
