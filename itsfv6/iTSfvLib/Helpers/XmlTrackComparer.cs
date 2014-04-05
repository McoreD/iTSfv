using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iTSfvLib
{
    public class XmlTrackComparer : IComparer<XmlTrack>
    {
        public int Compare(XmlTrack x, XmlTrack y)
        {
            return string.Compare(x.Location, y.Location);
        }

        public static class XmlTrackComparerMethods
        {
            public static int CompareByLocation(XmlTrack x, XmlTrack y)
            {
                return string.Compare(x.Location, y.Location);
            }

            public static int CompareByTrackNumber(XmlTrack x, XmlTrack y)
            {
                return string.Compare(x.TrackNumber.ToString("000"), y.TrackNumber.ToString("000"));
            }

            public static int CompareByArtworkSize(XmlTrack x, XmlTrack y)
            {
                return string.Compare(x.Artwork.Width.ToString("0000"), y.Artwork.Width.ToString("0000"));
            }

        }
    }
}
