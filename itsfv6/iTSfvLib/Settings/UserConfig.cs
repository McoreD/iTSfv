using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace iTSfvLib
{
    public class UserConfig : SettingsBase<UserConfig>
    {
        [Category(MyStrings.App), DefaultValue(true), Description("Check for missing tags")]
        public bool Checks_MissingTags { get; set; }

        [Category(MyStrings.App), DefaultValue(false), Description("Check for tracks with low resolution artwork")]
        public bool Checks_ArtworkLowRes { get; set; }

        [Category(MyStrings.App), DefaultValue(false), Description("Check for missing tags")]
        public bool Tracks_ArtworkFill { get; set; }

        [Category(MyStrings.App), DefaultValue(true), Description("Fill missing Track Count, Disc Number and Disc Count")]
        public bool Tracks_TrackCountFill { get; set; }

        [Category(MyStrings.App), DefaultValue(true), Description("Fill missing Album Artist")]
        public bool Tracks_AlbumArtistFill { get; set; }

        [Category(MyStrings.App), DefaultValue(true), Description("Fill missing Genre")]
        public bool Tracks_GenreFill { get; set; }

        [Category(MyStrings.App), DefaultValue(false), Description("Search for artwork using Album Art Downloader XUI")]
        public bool FileSystem_SearchArtworkUsingAAD { get; set; }

        [Category(MyStrings.App), DefaultValue(true), Description("Export embedded artwork to folder")]
        public bool FileSystem_ArtworkJpgExport { get; set; }

        public UserConfig()
        {
            this.ApplyDefaultPropertyValues();
        }

        public static bool IsConfigured(UserConfig self)
        {
            PropertyInfo[] properties = typeof(UserConfig).GetProperties();
            foreach (PropertyInfo pi in properties)
            {
                if (pi.PropertyType == typeof(Boolean) && (bool)pi.GetValue(self, null))
                {
                    return true;
                }
            }

            return false;
        }
    }
}