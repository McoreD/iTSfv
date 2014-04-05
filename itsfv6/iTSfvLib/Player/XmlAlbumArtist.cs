using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iTSfvLib
{
    /// <summary>
    /// Holds one or more albums by an AlbumArtist
    /// </summary>
    public class XmlAlbumArtist
    {
        /// <summary>
        /// Unique ID usually the Name of the Band
        /// </summary>
        public string Name { get; private set; }

        public Dictionary<string, XmlAlbum> Albums = new Dictionary<string, XmlAlbum>();

        public XmlAlbumArtist(string key)
        {
            this.Name = key;
        }

        public void AddAlbum(XmlAlbum o)
        {
            if (!Albums.ContainsKey(o.Key))
                Albums.Add(o.Key, o);
        }

        public XmlAlbum GetAlbum(string albumKey)
        {
            if (Albums.ContainsKey(albumKey))
                return Albums[albumKey];

            return null;
        }

        public void RemoveAlbum(XmlAlbum o)
        {
            if (Albums.ContainsKey(o.Key))
                Albums.Remove(o.Key);
        }

        public List<XmlTrack> GetTracks()
        {
            List<XmlTrack> tracks = new List<XmlTrack>();

            IEnumerator i = this.Albums.GetEnumerator();

            while (i.MoveNext())
            {
                tracks.AddRange(((KeyValuePair<string, XmlAlbum>)i.Current).Value.GetTracks());
            }

            return tracks;
        }
    }
}