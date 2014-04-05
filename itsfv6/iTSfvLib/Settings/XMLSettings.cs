using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using HelpersLib;
using System.Drawing.Design;

namespace iTSfvLib
{
    [XmlRoot("Settings")]
    public class XMLSettings : SettingsBase<XMLSettings>
    {
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(CsvConverter)), Category(MyStrings.App)]
        public List<string> SupportedFileTypes { get; set; }

        [Category(MyStrings.App), DefaultValue(true), Description("All the tracks in a folder are treated as having the same Album Artist")]
        public bool TreatAsOneBandPerFolder { get; set; }

        [Category(MyStrings.App), DefaultValue(true), Description("Produce reports after validating")]
        public bool ProduceReports { get; set; }

        [Category(MyStrings.App), DefaultValue("Folder"), Description("Artwork file name without extension e.g. Folder")]
        public string ArtworkFileNameWithoutExtension { get; set; }

        public string[] ArtworkLookupFileNames = new string[] { "Folder.jpg", "Cover.jpg", "Artwork.jpg" };

        [Category(MyStrings.App), DefaultValue(300), Description("Minimum width and height size in pixels for artwork")]
        public int LowResArtworkSize { get; set; }

        [EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
        [Category(MyStrings.App), Description("Music folder path")]
        public string MusicLibraryFolder { get; set; }

        [Category(MyStrings.App), DefaultValue(true), Description("Copy music to library folder")]
        public bool CopyMusicToLibrary { get; set; }

        [Category(MyStrings.App), DefaultValue(true), Description("Show files/folders dialog instead of using program logic to group files/folders before validating tracks")]
        public bool ShowAddFilesWizard { get; set; }

        [Category(MyStrings.App), DefaultValue(false), Description("Automatically validate tracks after adding files/folders")]
        public bool ValidateAfterAddingTracks { get; set; }

        public UserConfig UI = new UserConfig();

        public XMLSettings()
        {
            ApplyDefaultValues(this);
        }

        public static void ApplyDefaultValues(object self)
        {
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(self))
            {
                DefaultValueAttribute attr = prop.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;
                if (attr == null) continue;
                prop.SetValue(self, attr.Value);
            }
        }

        public static XMLSettings Read(string filePath)
        {
            XMLSettings settings = SettingsHelper.Load<XMLSettings>(filePath, SerializationType.Xml);

            if (settings.SupportedFileTypes == null || settings.SupportedFileTypes.Count == 0)
                settings.SupportedFileTypes = new List<string>() { "mp3", "m4a", "flac" };

            return settings;
        }

        public bool Write(string filePath)
        {
            return SettingsHelper.Save(this, filePath, SerializationType.Xml);
        }
    }
}