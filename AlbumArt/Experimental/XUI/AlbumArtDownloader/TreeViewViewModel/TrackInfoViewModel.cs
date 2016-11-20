using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using TagLib;
using System.IO;
using System.Windows.Media.Imaging;

namespace AlbumArtDownloader.TreeViewViewModel
{
    class TrackInfoViewModel : TreeViewItemViewModel
    {
        #region Data

        readonly TagLib.File _trackInfo;

        #endregion

        #region Constructors

        public TrackInfoViewModel(TrackViewModel parent, TagLib.File trackInfo)
            : base (parent)
        {
            this._trackInfo = trackInfo;
        }

        #endregion Constructors

        #region Properties

        public TagLib.File TrackInfo
        {
            get { return _trackInfo; }
        }

        public string Album
        {
            get { return _trackInfo.Tag.Album; }
            set 
            { 
                _trackInfo.Tag.Album = value;
            }
        }

        public BitmapFrame BitmapPicture
        {
            get
            {
                IPicture[] pictures = _trackInfo.Tag.Pictures;

                BitmapFrame bmp = null;

                if (pictures.Length > 0)
                {
                    MemoryStream stream = new MemoryStream(pictures[0].Data.Data);

                    bmp = BitmapFrame.Create(stream);
                }

                return bmp;
            }
        }

        public string TrackName
        {
            get { return _trackInfo.Tag.Title; }
            set 
            { 
                _trackInfo.Tag.Title = value;
            }
        }

        public string Genre
        {
            get { return _trackInfo.Tag.FirstGenre; }
            set 
            {
                List<string> genres = new List<string>(_trackInfo.Tag.Genres);

                if (genres.IndexOf(value) == -1)
                    genres.Add(value);
            }
        }

        public uint Year
        {
            get { return _trackInfo.Tag.Year; }
            set 
            { 
                _trackInfo.Tag.Year = value;
            }
        }

        public string Composer
        {
            get { return _trackInfo.Tag.FirstComposer; }
            set
            {
                List<string> composers = new List<string>(_trackInfo.Tag.Composers);

                if (composers.IndexOf(value) == -1)
                    composers.Add(value);
            }
        }

        public string Performer
        {
            get { return _trackInfo.Tag.FirstPerformer; }
            set
            {
                List<string> performers = new List<string>(_trackInfo.Tag.Performers);

                if (performers.IndexOf(value) == -1)
                    performers.Add(value);
            }
        }

        public string Comment
        {
            get { return _trackInfo.Tag.Comment; }
            set 
            { 
                _trackInfo.Tag.Comment = value;
            }
        }

        #region Height & Visibility

        public bool BitmapPictureVisible
        {
            get { return IsPropertyFilled(Tags.PICTURE); }
        }

        public bool AlbumVisible
        {
            get { return IsPropertyFilled(Tags.ALBUM); }
        }

        public bool TrackNameVisible
        {
            get { return IsPropertyFilled(Tags.TRACK_NAME); }
        }

        public bool YearVisible
        {
            get { return IsPropertyFilled(Tags.YEAR); }
        }

        public bool GenreVisible
        {
            get { return IsPropertyFilled(Tags.GENRE); }
        }

        public bool ComposerVisible
        {
            get { return IsPropertyFilled(Tags.COMPOSER); }
        }

        public bool PerformerVisible
        {
            get { return IsPropertyFilled(Tags.PERFORMER); }
        }

        public bool CommentVisible
        {
            get { return IsPropertyFilled(Tags.COMMENT); }
        }

        public int BitmapPictureHeight
        {
            get { return BitmapPictureVisible ? 100 : 0; }
        }

        public int BitmapPictureWidth
        {
            get { return BitmapPictureVisible  ? 100 : 0; }
        }

        public int AlbumHeight
        {
            get { return GetPropertyHeight(Tags.ALBUM); }
        }

        public int TrackNameHeight
        {
            get { return GetPropertyHeight(Tags.TRACK_NAME); }
        }

        public int GenreHeight
        {
            get { return GetPropertyHeight(Tags.GENRE); }
        }

        public int YearHeight
        {
            get { return GetPropertyHeight(Tags.YEAR); }
        }

        public int ComposerHeight
        {
            get { return GetPropertyHeight(Tags.COMPOSER); }
        }

        public int PerformerHeight
        {
            get { return GetPropertyHeight(Tags.PERFORMER); }
        }

        public int CommentHeight
        {
            get { return GetPropertyHeight(Tags.COMMENT); }
        }

        #endregion 

        enum Tags
        {
            ALBUM,
            TRACK_NAME,
            GENRE,
            YEAR,
            COMPOSER,
            PERFORMER,
            COMMENT,
            PICTURE
        }
        
        /// <summary>
        /// Get visibility flag for given tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns>True if given tag is filled in current track.Tag information</returns>
        private bool IsPropertyFilled(Tags tag)
        {
            bool result = false;

            switch (tag)
            {
                case Tags.ALBUM:
                    result = _trackInfo.Tag.Album != string.Empty;
                    break;
                case Tags.TRACK_NAME:
                    result = _trackInfo.Tag.Title != string.Empty;
                    break;
                case Tags.GENRE:
                    result = _trackInfo.Tag.FirstGenre != string.Empty;
                    break;
                case Tags.YEAR:
                    result = _trackInfo.Tag.Year != 0;
                    break;
                case Tags.COMPOSER:
                    if (_trackInfo.Tag.Composers != null)
                    {
                        result = _trackInfo.Tag.Composers.Length != 0;
                    }
                    break;
                case Tags.PERFORMER:
                    if (_trackInfo.Tag.Performers != null)
                    {
                        result = _trackInfo.Tag.Performers.Length != 0;
                    }
                    break;
                case Tags.COMMENT:
                    result = _trackInfo.Tag.Comment != string.Empty;
                    break;
                case Tags.PICTURE:
                    IPicture frontCover = null;

                    // go through all pictures and if find one front cover break searching
                    foreach (IPicture picture in _trackInfo.Tag.Pictures)
                    {
                        if (picture.Type == PictureType.FrontCover)
                        {
                            frontCover = picture;
                            break;
                        }
                    }

                    if (frontCover == null && _trackInfo.Tag.Pictures.Length > 0)
                    {
                        frontCover = _trackInfo.Tag.Pictures[0];
                    }

                    result = frontCover != null;
                    break;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        private int GetPropertyHeight(Tags tag)
        {
            int result = 0;

            switch (tag)
            {
                case Tags.ALBUM:
                    result = AlbumVisible ? 25 : 0;
                    break;
                case Tags.TRACK_NAME:
                    result = TrackNameVisible ? 25 : 0;
                    break;
                case Tags.GENRE:
                    result = GenreVisible ? 25 : 0;
                    break;
                case Tags.YEAR:
                    result = YearVisible ? 25 : 0;
                    break;
                case Tags.COMPOSER:
                    result = ComposerVisible ? 25 : 0;
                    break;
            }

            return result;
        }
        #endregion Properties

        #region Methods

        public void Save()
        {
            _trackInfo.Save();
        }

        #endregion
    }
}
