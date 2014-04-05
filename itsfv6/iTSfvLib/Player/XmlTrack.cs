using HelpersLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TagLib;

namespace iTSfvLib
{
    public class XmlTrack
    {
        #region "Read/Write Properties"


        public string AlbumArtist
        {
            get
            {
                if (Tags.AlbumArtists.Length > 0)
                    return string.Join(" / ", Tags.AlbumArtists);

                return string.Empty;
            }
            set
            {
                this.Tags.AlbumArtists = new string[] { value };
                IsModified = true;
                DebugHelper.WriteLine(this.FileName + " --> filled AlbumArtist with " + value);
            }
        }

        public string Album
        {
            get
            {
                return this.Tags.Album;
            }
            set
            {
                this.Tags.Album = value;
                IsModified = true;
            }
        }

        public uint TrackNumber
        {
            get
            {
                return this.Tags.Track;
            }
            set
            {
                this.Tags.Track = value;
                IsModified = true;
            }
        }

        public uint TrackCount
        {
            get
            {
                return this.Tags.TrackCount;
            }
            set
            {
                this.Tags.TrackCount = value;
                IsModified = true;
            }
        }

        public uint DiscNumber
        {
            get
            {
                return this.Tags.Disc;
            }
            set
            {
                this.Tags.Disc = value;
                IsModified = true;
            }
        }

        public uint DiscCount
        {
            get
            {
                return this.Tags.DiscCount;
            }
            set
            {
                this.Tags.DiscCount = value;
                IsModified = true;
            }
        }

        public string Genre
        {
            get
            {
                if (Tags.Genres.Length > 0)
                    return string.Join("/", Tags.Genres);

                return string.Empty;
            }
            set
            {
                Tags.Genres = new string[] { value };
                IsModified = true;
                DebugHelper.WriteLine(this.FileName + " --> filled Genre with " + value);
            }
        }

        public uint Year
        {
            get
            {
                return this.Tags.Year;
            }
            set
            {
                this.Tags.Year = value;
                IsModified = true;
            }
        }

        public DateTime PlayedDate { get; set; }
        public DateTime SkippedDate { get; set; }
        public bool Compilation { get; set; }
        public bool Enabled { get; set; }
        public bool ExcludeFromShuffle { get; set; }
        public bool PartOfGaplessAlbum { get; set; }
        public bool RememberBookmark { get; set; }
        public bool Unplayed { get; set; }
        public int AlbumRating { get; set; }
        public int BPM { get; set; }
        public int BookmarkTime { get; set; }
        public int EpisodeNumber { get; set; }
        public int Finish { get; set; }
        public int PlayedCount { get; set; }
        public int Rating { get; set; }
        public int SeasonNumber { get; set; }
        public int SkippedCount { get; set; }
        public int Start { get; set; }
        public int VolumeAdjustment { get; set; }
        public string Category { get; set; }
        public string Comment { get; set; }
        public string Composer { get; set; }
        public string Description { get; set; }
        public string EQ { get; set; }
        public string Grouping { get; set; }
        public string Location { get; set; }
        public string LongDescription { get; set; }
        public string Lyrics { get; set; }
        public string Show { get; set; }
        public string SortComposer { get; set; }
        public string SortName { get; set; }
        public string SortShow { get; set; }
        public string URL { get; set; }

        #endregion "Read/Write Properties"

        #region "Read Only Properties"

        public string AlbumArtistPathFriendly
        {
            get
            {
                return Path.GetInvalidFileNameChars().Aggregate(AlbumArtist,
                    (current, c) => current.Replace(c.ToString(), "_"));
            }
        }

        public string AlbumPathFriendly
        {
            get
            {
                return Path.GetInvalidFileNameChars().Aggregate(this.Tags.Album,
                  (current, c) => current.Replace(c.ToString(), "_"));
            }
        }

        public string Title
        {
            get
            {
                return this.Tags.Title;
            }
        }

        public string Artist
        {
            get
            {
                if (Tags.Performers.Length > 0)
                    return string.Join("/", Tags.Performers);

                return string.Empty;
            }
        }

        public string FileName
        {
            get
            {
                return !string.IsNullOrEmpty(Location) ? Path.GetFileName(Location) : string.Empty;
            }
        }

        public uint BitRate
        {
            get
            {
                if (this.Properties != null)
                    return (uint)this.Properties.AudioBitrate;

                return 0;
            }
        }

        public decimal Size { get; private set; }

        public XmlArtwork Artwork { get; private set; }

        public bool IsModified { get; private set; }

        #endregion "Read Only Properties"

        private TagLib.Tag Tags { get; set; }
        private TagLib.Properties Properties { get; set; }

        public XmlTrack(string fp)
        {
            Location = fp;
            UpdateInfoFromFile(fp);
            Artwork = new XmlArtwork(this.Tags);
        }

        public override string ToString()
        {
            return this.Location;
        }

        public string GetAlbumKey()
        {
            if (!string.IsNullOrEmpty(this.AlbumArtist) && !string.IsNullOrEmpty(this.Tags.Album))
            {
                return string.Format("{0} - {1}", this.Tags.Album, this.AlbumArtist);
            }
            return ConstantStrings.UnknownAlbum;
        }

        public string GetDiscKey()
        {
            if (!string.IsNullOrEmpty(this.AlbumArtist) && !string.IsNullOrEmpty(this.Tags.Album))
            {
                return string.Format("{0} Disc {1} - {2}", this.Tags.Album, Tags.Disc.ToString("000"), this.AlbumArtist);
            }

            return string.Format("{0} Disc {1} - {2}", ConstantStrings.UnknownDisc, Tags.Disc.ToString("000"), ConstantStrings.UnknownArtist);
        }

        private string CombineString(string[] array, string seperator = "/")
        {
            StringBuilder sb = new StringBuilder();
            foreach (string band in array)
            {
                if (!string.IsNullOrEmpty(band))
                {
                    sb.Append(band);
                    sb.Append(seperator);
                }
            }
            if (sb.Length > 1)
                sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }


        public void Play()
        {
            try
            {
                Process.Start(this.Location);
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex, "Error playing track");
            }
        }

        public void UpdateInfoFromFile(string fp)
        {
            try
            {
                using (TagLib.File f = TagLib.File.Create(Location))
                {
                    f.RemoveTags(f.TagTypes & ~f.TagTypesOnDisk); // read tags and their versions as is from file   
                    this.Size = (decimal)f.InvariantEndPosition;
                    this.Tags = f.Tag;
                    this.Properties = f.Properties;
                }
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex, "Error reading info from file");
            }
        }

        public void WriteTagsToFile()
        {
            try
            {
                FileInfo fileInfo = new FileInfo(Location);
                fileInfo.IsReadOnly = false;
                fileInfo.Refresh();

                using (TagLib.File f = TagLib.File.Create(Location))
                {
                    f.Tag.Track = Tags.Track;
                    f.Tag.TrackCount = Tags.TrackCount;
                    f.Tag.Title = Tags.Title;
                    f.Tag.AlbumArtists = Tags.AlbumArtists;
                    f.Tag.Performers = Tags.Performers;
                    f.Tag.Disc = Tags.Disc;
                    f.Tag.DiscCount = Tags.DiscCount;
                    f.Tag.Album = Tags.Album;
                    f.Tag.Genres = Tags.Genres;
                    f.Tag.Year = Tags.Year;
                    f.Tag.Pictures = Tags.Pictures;
                    f.Save();
                }
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex, "Error writing tags to file");
            }
        }

        public bool AddArtwork(string fp)
        {
            if (System.IO.File.Exists(fp))
            {
                TagLib.Picture picture = new Picture(fp);
                TagLib.Id3v2.AttachedPictureFrame albumCoverPictFrame = new TagLib.Id3v2.AttachedPictureFrame(picture);
                albumCoverPictFrame.MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
                albumCoverPictFrame.Type = TagLib.PictureType.FrontCover;
                TagLib.IPicture[] pictFrames = new IPicture[1];
                pictFrames[0] = (IPicture)albumCoverPictFrame;
                this.Tags.Pictures = pictFrames;

                DebugHelper.WriteLine(this.FileName + " --> embedded artwork");
                return IsModified = true;
            }

            return false;
        }

        public void AddArtwork(Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] myBytes = ms.ToArray();
            ByteVector byteVector = new ByteVector(myBytes, myBytes.Length);
            TagLib.Picture picture = new Picture(byteVector);
            TagLib.Id3v2.AttachedPictureFrame albumCoverPictFrame = new TagLib.Id3v2.AttachedPictureFrame(picture);
            albumCoverPictFrame.MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
            albumCoverPictFrame.Type = TagLib.PictureType.FrontCover;
            this.Tags.Pictures = new IPicture[1] { (IPicture)albumCoverPictFrame };

            DebugHelper.WriteLine(this.FileName + " --> embedded artwork");
            IsModified = true;
        }

        public bool CheckMissingTags(ReportWriter report)
        {
            List<string> missingTags = new List<string>();

            if (string.IsNullOrEmpty(this.Tags.Title))
            {
                missingTags.Add("Title");
            }

            if (string.IsNullOrEmpty(this.Artist))
            {
                missingTags.Add("Artist");
            }

            if (string.IsNullOrEmpty(this.AlbumArtist))
            {
                missingTags.Add("Album Artist");
            }

            if (string.IsNullOrEmpty(this.Genre))
            {
                missingTags.Add("Genre");
            }

            if (this.Tags.Pictures.Length == 0)
            {
                missingTags.Add("Artwork");
            }

            if (this.Tags.Track == 0)
            {
                missingTags.Add("Track Number");
            }

            if (this.Tags.TrackCount == 0)
            {
                missingTags.Add("Track Count");
            }

            if (this.Tags.Disc == 0)
            {
                missingTags.Add("Disc Number");
            }

            if (this.Tags.DiscCount == 0)
            {
                missingTags.Add("Disc Count");
            }

            if (this.Tags.Year == 0)
            {
                missingTags.Add("Year");
            }

            if (missingTags.Count > 0)
            {
                report.AddTrackMissingTags(this, missingTags);
            }

            return missingTags.Count == 0;
        }

        internal void FillTrackCount(List<XmlAlbum> Albums, List<XmlDisc> Discs, ReportWriter reportWriter)
        {
            XmlDisc disc = Discs.FirstOrDefault(x => x.Key == this.GetDiscKey());
            List<string> filledTags = new List<string>();

            if (disc != null)
            {
                if (this.Tags.TrackCount == 0)
                {
                    this.Tags.TrackCount = (uint)disc.Tracks.Count;
                    IsModified = true;
                    filledTags.Add("TrackCount");
                }
            }

            XmlAlbum album = Albums.FirstOrDefault(x => x.Key == this.GetAlbumKey());
            if (album != null)
            {
                if (this.Tags.Disc == 0 && album.Discs.Count == 1)
                {
                    this.Tags.Disc = 1; // for single disc albums you can specify DiscNumber
                    IsModified = true;
                    filledTags.Add("DiscNumber");
                }

                if (this.Tags.DiscCount == 0)
                {
                    this.Tags.DiscCount = (uint)album.Discs.Count;
                    IsModified = true;
                    filledTags.Add("DiscCount");
                }
            }

            if (filledTags.Count > 0)
                DebugHelper.WriteLine(this.FileName + " --> filled " + filledTags.ToArray().Join("; "));
        }

        internal void FillAlbumArtist(List<XmlAlbum> Albums, ReportWriter reportWriter)
        {
            XmlAlbum album = Albums.FirstOrDefault(x => x.Key == this.GetAlbumKey());
            if (album != null)
            {
                if (this.Tags.AlbumArtists.Length == 0)
                {
                    this.AlbumArtist = album.AlbumArtist;
                }
            }
        }

        internal void FillGenre(List<XmlDisc> Discs, ReportWriter reportWriter)
        {
            XmlDisc disc = Discs.FirstOrDefault(x => x.Key == this.GetDiscKey());
            if (disc != null)
            {
                if (this.Tags.Genres.Length == 0)
                {
                    this.Genre = disc.Genre;
                }
            }
        }

        internal bool ExportArtwork(XMLSettings config)
        {
            if (this.Tags.Pictures.Length > 0)
            {
                try
                {
                    string ext = "jpg";
                    string[] mime = this.Tags.Pictures[0].MimeType.ToString().Trim().Split('/');
                    if (mime.Length > 1)
                        ext = mime[1].Replace("jpeg", "jpg");

                    string fp = Path.Combine(Path.GetDirectoryName(this.Location), config.ArtworkFileNameWithoutExtension + "." + ext);

                    if (!System.IO.File.Exists(fp))
                    {
                        using (FileStream fs = new FileStream(fp, FileMode.Create))
                        {
                            fs.Write(this.Tags.Pictures[0].Data.Data, 0, this.Tags.Pictures[0].Data.Data.Length);
                            DebugHelper.WriteLine(this.FileName + " --> exported artwork");
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex, "while exporting artwork");
                }
            }

            return false;
        }

        internal void EmbedArtwork(XMLSettings xMLSettings, ReportWriter reportWriter)
        {
            if (this.Tags.Pictures.Length == 0)
            {
                foreach (string fileName in xMLSettings.ArtworkLookupFileNames)
                {
                    string fp = Path.Combine(Path.GetDirectoryName(this.Location), fileName);
                    if (AddArtwork(fp))
                        break;
                }
            }
        }

        internal void CheckLowResArtwork(XMLSettings xMLSettings, ReportWriter reportWriter)
        {
            List<string> artwork_low = new List<string>();

            if (this.Artwork.Width > 0 && this.Artwork.Width < xMLSettings.LowResArtworkSize)
            {
                artwork_low.Add(this.Artwork.Width.ToString());
            }

            if (this.Artwork.Height > 0 && this.Artwork.Height < xMLSettings.LowResArtworkSize)
            {
                artwork_low.Add(this.Artwork.Height.ToString());
            }

            if (artwork_low.Count > 0)
                reportWriter.AddTrackLowResArtwork(this, artwork_low);
        }
    }

    public class XmlArtwork
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public XmlArtwork(Tag tag)
        {
            if (tag.Pictures.Length > 0)
            {
                Image img = Image.FromStream(new MemoryStream(tag.Pictures[0].Data.Data));
                this.Width = img.Width;
                this.Height = img.Height;
            }
        }
    }
}