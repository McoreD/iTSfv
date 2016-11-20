using System;
using System.Configuration;

namespace AlbumArtDownloader
{
	public delegate SourceSettings SourceSettingsCreator(string name);
		
	public class SourceSettings : ApplicationSettingsBase
	{
		#region Creation
		//SourceSettings overrides should provide custom versions of these too
		public static SourceSettingsCreator Creator
		{
			get
			{
				return new SourceSettingsCreator(Create);
			}
		}
		private static SourceSettings Create(string name)
		{
			return new SourceSettings(name);
		}
		#endregion

		public SourceSettings(string sourceName) : base(sourceName)
		{
		}

		[DefaultSettingValueAttribute("True")]
		[UserScopedSetting]
		public bool Enabled
		{
			get
			{
				return ((bool)(this["Enabled"]));
			}
			set
			{
				this["Enabled"] = value;
			}
		}

		[DefaultSettingValueAttribute("10")]
		[UserScopedSetting]
		public int MaximumResults
		{
			get
			{
				return ((int)(this["MaximumResults"]));
			}
			set
			{
				this["MaximumResults"] = value;
			}
		}

		[DefaultSettingValueAttribute("True")]
		[UserScopedSetting]
		public bool UseMaximumResults
		{
			get
			{
				return ((bool)(this["UseMaximumResults"]));
			}
			set
			{
				this["UseMaximumResults"] = value;
			}
		}

		[DefaultSettingValueAttribute("False")]
		[UserScopedSetting]
		public bool IsPrimary
		{
			get
			{
				return ((bool)(this["IsPrimary"]));
			}
			set
			{
				this["IsPrimary"] = value;
			}
		}

		[DefaultSettingValueAttribute("False")]
		[UserScopedSetting]
		public bool FullSizeOnly
		{
			get
			{
				return ((bool)(this["FullSizeOnly"]));
			}
			set
			{
				this["FullSizeOnly"] = value;
			}
		}
	}
}