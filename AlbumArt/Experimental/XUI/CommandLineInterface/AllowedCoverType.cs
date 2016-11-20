using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlbumArtDownloader
{
	//Copied from Enums.cs

	/// <summary>
	/// Cover types that are allowed (filtered in)
	/// </summary>
	[Flags]
	public enum AllowedCoverType
	{
		Unknown = 0x1,
		Front = 0x2,
		Back = 0x4,
		Inside = 0x8,
		CD = 0x10,
		Any = Unknown | Front | Back | Inside | CD
	}
}
