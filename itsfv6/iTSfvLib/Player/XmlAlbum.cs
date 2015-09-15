using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace iTSfvLib
{
    /// <summary>
    /// Holds one or more discs of an album
    /// </summary>
    public class XmlAlbum
    {
        public string Key { get; private set; }

        public Dictionary<string, XmlDisc> Discs = new Dictionary<string, XmlDisc>();

        public XmlAlbum(string key)
        {
            Key = key;
        }

        public void AddDisc(XmlDisc o)
        {
            if (!Discs.ContainsKey(o.Key))
                Discs.Add(o.Key, o);
        }

        public XmlDisc GetDisc(string key)
        {
            if (Discs.ContainsKey(key))
                return Discs[key];

            return null;
        }

        public void RemoveDisc(XmlDisc o)
        {
            if (Discs.ContainsKey(o.Key))
                Discs.Remove(o.Key);
        }

        public string Name
        {
            get
            {
                foreach (XmlTrack track in GetTracks().Where(track => !string.IsNullOrEmpty(track.Album)))
                {
                    return track.Album;
                }
                return ConstantStrings.UnknownAlbum;
            }
        }

        public List<XmlTrack> GetTracks()
        {
            List<XmlTrack> tracks = new List<XmlTrack>();
            IEnumerator i = this.Discs.GetEnumerator();

            while (i.MoveNext())
            {
                tracks.AddRange(((KeyValuePair<string, XmlDisc>)i.Current).Value.Tracks);
            }

            return tracks;
        }

        private string _albumArtist = string.Empty;
        public string AlbumArtist
        {
            get
            {
                if (!string.IsNullOrEmpty(_albumArtist))
                    return _albumArtist;

                List<string> albumArtists = new List<string>();
                IEnumerator e = this.Discs.GetEnumerator();
                while (e.MoveNext())
                {
                    string albumartist = ((KeyValuePair<string, XmlDisc>)e.Current).Value.AlbumArtist;
                    if (!albumArtists.Contains(albumartist))
                        albumArtists.Add((albumartist));
                }
                if (albumArtists.Count > 0)
                    return _albumArtist = string.Join(" / ", albumArtists.ToArray());

                return ConstantStrings.UnknownArtist;
            }
        }

        public string Location
        {
            get
            {
                List<XmlTrack> tracks = GetTracks();
                if (tracks.Count > 0)
                {
                    return Path.GetDirectoryName(tracks[0].Location);
                }
                return string.Empty;
            }
        }

        internal void SaveArtworkUsingAAD(string exePath, string pathArtwork, int minArtworkWidth)
        {
            if (!File.Exists(exePath)) return;

            Process p = new Process();
            ProcessStartInfo psi = new ProcessStartInfo(exePath);
            psi.WindowStyle = ProcessWindowStyle.Minimized;

            psi.Arguments = string.Format("/artist \"{0}\" /album \"{1}\" /minSize {2} /minAspect {3} /path \"{4}\"", AlbumArtist, Name, minArtworkWidth, 0.9, pathArtwork);
            p.StartInfo = psi;
            p.Start();
            p.WaitForExit();
        }
    }
}