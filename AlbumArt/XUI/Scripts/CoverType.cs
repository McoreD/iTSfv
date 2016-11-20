/*
 * 
 * User: daju
 * Date: 01.10.2008
 * Time: 19:01
 * 
 * 
 */
using System;

namespace AlbumArtDownloader.Scripts
{
	/// <summary>
	/// The Enum of the different types
	/// a cover picture may have.
	/// E.g. Front or CD
	/// </summary>
	public enum CoverType
	{
		//NOTE: These values are currently directly user-visible in the UI
		Unknown, 
		Front, 
		Back,
		Inside,
		Inlay = Inside,  //"Inlay" provided for backwards compatiblity. New term is "Inside"
		CD,
		Booklet
	}


}
