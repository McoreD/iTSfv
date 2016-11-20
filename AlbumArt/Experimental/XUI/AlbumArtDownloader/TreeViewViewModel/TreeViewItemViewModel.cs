using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AlbumArtDownloader.TreeViewViewModel
{
    class TreeViewItemViewModel : INotifyPropertyChanged
    {
        #region Data

        ReadOnlyCollection<TreeViewItemViewModel> _children;
        TreeViewItemViewModel _parent;

        bool _isSelected;
        bool _isExpanded;

        #endregion

        #region Constructors

        public TreeViewItemViewModel(TreeViewItemViewModel parent)
        {
            this._parent = parent;
        }

        #endregion

        #region Properties

        public ReadOnlyCollection<TreeViewItemViewModel> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        public TreeViewItemViewModel Parent
        {
            get { return _parent; }
        }

        #endregion

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;
            }
        }

        #endregion // IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        #endregion // IsSelected

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members
    }
}
