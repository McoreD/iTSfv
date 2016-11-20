using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlbumArtDownloader.TreeViewViewModel
{
    class DiscViewModel : TreeViewItemViewModel
    {
        #region Data

        readonly string _discName = string.Empty;

        readonly ReadOnlyCollection<TrackViewModel> _tracks;

        #endregion // Data

        #region Constructors

        public DiscViewModel(string discName, TagLib.File[] tracks)
            : base(null)
        {
            this._discName = discName;

            TreeViewItemViewModel[] tracksViewModel = new TreeViewItemViewModel[tracks.Length];

            for (int i = 0; i < tracks.Length; i++)
            {
                TagLib.File item = tracks[i];

                tracksViewModel[i] = new TrackViewModel(this, item);
            }

            this.Children = new ReadOnlyCollection<TreeViewItemViewModel>(tracksViewModel);
        }

        #endregion // Constructors

        #region Disc Properties

        public string DiscName
        {
            get { return _discName; }
        }

        #endregion Disc Properties

    }
}
