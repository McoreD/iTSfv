using HelpersLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iTSfvLib
{
    public enum ETagFinderType
    {
        Artist,
        Genre,
        Year
    }

    public class TagFinderOptions
    {
        public bool MostCommonTagRatioActive { get; set; }
        public double ConfidenceRequired { get; set; }
        public bool ChooseMostCommonTag { get; set; }
    }

    /// <summary>
    /// Class to determine the most appropriate Album Artist of a track in a Disc
    /// </summary>
    public class MostCommonTagFinder
    {
        private Dictionary<string, int> _DiscTags = new Dictionary<string, int>();
        private string _DiscTag = string.Empty;
        private List<XmlTrack> _Tracks = new List<XmlTrack>();
        private double _Confidence = 0.0;

        public ETagFinderType Tag { get; set; }

        private TagFinderOptions Options { get; set; }

        public string MostCommonString
        {
            get { return _DiscTag; }
        }

        public MostCommonTagFinder(List<XmlTrack> lDisc, ETagFinderType lTag, TagFinderOptions lOptions)
        {
            this.Tag = lTag;
            this._Tracks = lDisc;
            this.Options = lOptions;

            if (Options.ChooseMostCommonTag)
            {
                for (int i = 0; i <= _Tracks.Count - 1; i++)
                {
                    string oTag = string.Empty;
                    switch (Tag)
                    {
                        case ETagFinderType.Artist:
                            oTag = ConstantStrings.VariousArtists;
                            if (string.Empty != _Tracks[i].AlbumArtist)
                            {
                                oTag = _Tracks[i].AlbumArtist;
                            }
                            else if (!string.IsNullOrEmpty(_Tracks[i].Artist))
                            {
                                oTag = _Tracks[i].Artist;
                            }
                            break;
                        case ETagFinderType.Genre:
                            oTag = ConstantStrings.UnknownGenre;
                            if (string.Empty != _Tracks[i].Genre)
                            {
                                oTag = _Tracks[i].Genre;
                            }
                            break;
                        case ETagFinderType.Year:
                            oTag = "0000";
                            if (_Tracks[i].Year > 0)
                            {
                                oTag = _Tracks[i].Year.ToString();
                            }
                            break;
                    }
                    AddTag(oTag);
                }
                _DiscTag = GetTopTag();
            }
            else
            {
                bool bIsTagSame = true;
                string oTag = _Tracks[0].Genre;

                for (int i = 0; i <= _Tracks.Count - 2; i++)
                {
                    string tag1 = string.Empty, tag2 = string.Empty;
                    switch (Tag)
                    {
                        case ETagFinderType.Artist:
                            tag1 = _Tracks[i].Artist;
                            tag2 = _Tracks[i + 1].Artist;
                            _DiscTag = ConstantStrings.VariousArtists;
                            break;
                        case ETagFinderType.Genre:
                            tag1 = _Tracks[i].Genre;
                            tag2 = _Tracks[i + 1].Genre;
                            _DiscTag = ConstantStrings.UnknownGenre;
                            break;
                        case ETagFinderType.Year:
                            tag1 = _Tracks[i].Year.ToString();
                            tag2 = _Tracks[i + 1].Year.ToString();
                            _DiscTag = tag1;
                            break;
                    }

                    if (string.Empty != tag1 && string.Empty != tag2)
                    {
                        bIsTagSame = bIsTagSame & tag1.Equals(tag2);
                    }
                }

                if (bIsTagSame)
                {
                    // this will not get assigned if strAlbumArtist is empty
                    _DiscTag = oTag;
                }
            }

            DebugHelper.WriteLine(string.Format("Chosen Most Common {0}: \"{1}\" with {2}% confidence",
                Tag.ToString(), _DiscTag, _Confidence.ToString("0.00")));
        }

        private string GetTopTag()
        {
            int topHit = 0;
            string topTag = string.Empty;

            if (_Tracks.Count > 0 & _DiscTags.Count > 0)
            {
                IEnumerator et = _DiscTags.GetEnumerator();
                KeyValuePair<string, int> de = default(KeyValuePair<string, int>);

                while (et.MoveNext())
                {
                    de = (KeyValuePair<string, int>)et.Current;
                    if (string.IsNullOrEmpty(de.Key) == false && (int)de.Value > topHit)
                    {
                        topHit = (int)de.Value;
                        topTag = (string)de.Key;
                    }
                }

                _Confidence = 100 * _DiscTags[topTag] / _Tracks.Count;

                if (Options.MostCommonTagRatioActive)
                {
                    // work out if top Tag has lost the election
                    if (_Confidence < Options.ConfidenceRequired)
                    {
                        switch (Tag)
                        {
                            case ETagFinderType.Artist:
                                topTag = ConstantStrings.VariousArtists;
                                break;
                            case ETagFinderType.Genre:
                                topTag = ConstantStrings.VariousGenre;
                                break;
                            case ETagFinderType.Year:
                                topTag = _Tracks[0].Genre;
                                break;
                        }
                    }
                }
            }

            return topTag;
        }

        private void AddTag(string tag)
        {
            if (_DiscTags.ContainsKey(tag))
            {
                _DiscTags[tag] += 1;
            }
            else
            {
                _DiscTags.Add(tag, 1);
            }
        }
    }
}