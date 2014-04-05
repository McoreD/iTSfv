using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace iTSfvLib
{
    public class ReportWriter
    {
        public Dictionary<XmlTrack, List<string>> TracksMissingTags { get; private set; }     // track and list of tags missing
        public Dictionary<XmlTrack, List<string>> TracksLowResArtwork { get; private set; }   // track and WxH of artwork

        private string WorkingDir { get; set; }

        public ReportWriter()
        {
            TracksMissingTags = new Dictionary<XmlTrack, List<string>>();
            TracksLowResArtwork = new Dictionary<XmlTrack, List<string>>();
        }

        public void AddTrackLowResArtwork(XmlTrack track, List<string> artwork_dimensions)
        {
            TracksLowResArtwork.Add(track, artwork_dimensions);
        }

        public void AddTrackMissingTags(XmlTrack track, List<string> missingTags)
        {
            TracksMissingTags.Add(track, missingTags);
        }

        internal void Clear()
        {
            TracksMissingTags.Clear();
            TracksLowResArtwork.Clear();
        }

        public void Write(string workingDir)
        {
            this.WorkingDir = workingDir;
            new Thread(() => Write(GetReportPath("TracksMissingTags"), TracksMissingTags)).Start();
            new Thread(() => Write(GetReportPath("TracksLowResArtwork"), TracksLowResArtwork)).Start();
        }

        private string GetReportPath(string reportName)
        {
            return Path.Combine(WorkingDir, string.Format("{0}-{1}-{2}.log",
                Application.ProductName,
                reportName,
                DateTime.Now.ToString("yyyyMMddTHHmm")));
        }

        private void Write(string fp, Dictionary<XmlTrack, List<string>> dic)
        {
            if (dic.Count > 0)
            {
                using (StreamWriter sw = new StreamWriter(fp, false))
                {
                    IEnumerator i = dic.GetEnumerator();
                    KeyValuePair<XmlTrack, List<string>> kvp = new KeyValuePair<XmlTrack, List<string>>();

                    while (i.MoveNext())
                    {
                        kvp = (KeyValuePair<XmlTrack, List<string>>)i.Current;
                        XmlTrack track = kvp.Key;
                        List<string> missingTags = kvp.Value;
                        string errors = string.Join("; ", missingTags.ToArray());
                        sw.WriteLine(track.Location + " --> " + errors);
                    }
                }
            }
        }
    }
}
