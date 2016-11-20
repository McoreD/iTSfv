using System;

namespace AlbumArtDownloader.Controls
{
	public struct PatternPlaceholder
	{
		public PatternPlaceholder(string menuItemHeader, string tooltip, string placeholder)
		{
			mMenuItemHeader = menuItemHeader;
			mToolTip = tooltip;
			mPlaceholder = placeholder;
		}

		private string mMenuItemHeader;
		public string MenuItemHeader
		{
			get { return mMenuItemHeader; }
			set { mMenuItemHeader = value; }
		}

		private string mToolTip;
		public string ToolTip
		{
			get { return mToolTip; }
			set { mToolTip = value; }
		}

		private string mPlaceholder;
		public string Placeholder
		{
			get { return mPlaceholder; }
			set { mPlaceholder = value; }
		}

	}
}
