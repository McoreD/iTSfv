using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace AlbumArtDownloader.TreeViewViewModel
{
    class TrackViewModel : TreeViewItemViewModel
    {
        #region Data

        readonly TagLib.File _track;

        readonly ReadOnlyCollection<TrackInfoViewModel> _children;

        #endregion // Data

        #region Constructors

        public TrackViewModel(DiscViewModel parent, TagLib.File track)
            : base(parent)
        {
            this._track = track;

            TreeViewItemViewModel[] tracksViewModel = new TreeViewItemViewModel[1];
            tracksViewModel[0] = new TrackInfoViewModel(this, track);
            
            this.Children = new ReadOnlyCollection<TreeViewItemViewModel>(tracksViewModel);
        }

        #endregion // Constructors

        #region Properties

        public string TrackName
        {
            get { return _track.Tag.Title; }
            set { _track.Tag.Title = value; }
        }

        public string Artist
        {
            get
            {
                string result = string.Empty;
                bool first = false;

                foreach (string item in _track.Tag.Performers)
                {
                    if (first)
                        result += ", ";

                    result += item;

                    if (!first)
                        first = true;
                }

                return result;
            }
        }

        #endregion properties
    }
}
