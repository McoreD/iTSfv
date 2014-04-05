using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using iTSfvLib;

namespace iTSfvGUI
{
    public partial class AddFilesWizard : Form
    {
        private string[] FilesDirs;
        public List<XmlTrack> Tracks { get; private set; }
        public List<XmlTrack> TracksOrphaned { get; private set; }
        public List<XmlDisc> Discs { get; private set; }
        public bool CopyMusicToLibrary { get; private set; }

        public AddFilesWizard(string[] filesDirs)
        {
            InitializeComponent();
            FilesDirs = filesDirs;
            TracksOrphaned = new List<XmlTrack>();
            Discs = new List<XmlDisc>();
            Tracks = new List<XmlTrack>();

            chkCopyMusicToLibrary.Checked = Program.Config.CopyMusicToLibrary;
        }

        private void AddFilesWizard_Load(object sender, EventArgs e)
        {
            foreach (string pfd in FilesDirs.ToArray<string>())
            {
                if (Directory.Exists(pfd))
                {
                    AddDirToNode(pfd, tvBands.Nodes);
                }
                else if (File.Exists(pfd))
                {
                    XmlTrack track = new XmlTrack(pfd);
                    Tracks.Add(track);
                    TracksOrphaned.Add(track);
                }
            }

            XmlDisc discOrphaned = new XmlDisc(TracksOrphaned);
            foreach (XmlTrack track in TracksOrphaned)
            {
                TreeNode tnOrphaned = new TreeNode("Orphaned Tracks");
                tnOrphaned.Tag = discOrphaned;
                tvBands.Nodes.Add(tnOrphaned);
            }

            if (tvBands.Nodes.Count > 0 && tvBands.Nodes[0].Nodes.Count > 0)
                tvBands.SelectedNode = tvBands.Nodes[0].Nodes[0];
            else if (tvBands.Nodes.Count > 0)
                tvBands.SelectedNode = tvBands.Nodes[0];

        }

        private void AddDirToNode(string dirPath, TreeNodeCollection tnc)
        {
            TreeNode tn = new TreeNode(Path.GetFileName(dirPath));
            tnc.Add(tn);
            tn.Tag = GetDiscFromFolder(dirPath);

            foreach (string dp in Directory.GetDirectories(dirPath))
            {
                if (Directory.Exists(dp))
                {
                    AddDirToNode(dp, tn.Nodes);
                }
            }
        }

        private XmlDisc GetDiscFromFolder(string dirPath)
        {
            List<XmlTrack> tracks = new List<XmlTrack>();
            foreach (string ext in Program.Config.SupportedFileTypes)
            {
                Directory.GetFiles(dirPath, string.Format("*.{0}", ext),
                    SearchOption.TopDirectoryOnly).ToList().ForEach(delegate(string fp)
                {
                    tracks.Add(new XmlTrack(fp));
                });
            }

            XmlDisc tempDisc = new XmlDisc(tracks);
            Discs.Add(tempDisc);
            this.Tracks.AddRange(tempDisc.Tracks.ToArray());
            return tempDisc;
        }

        private void tvBands_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode tn = e.Node;
            if (tn != null && tn.Tag != null)
            {
                XmlDisc disc = tn.Tag as XmlDisc;
                lbTracks.Items.Clear();
                lbTracks.Items.AddRange(disc.Tracks.ToArray());

                cboAlbumArtist.Text = disc.AlbumArtist;
                txtAlbum.Text = disc.Album;
                cboGenre.Text = disc.Genre;
                nudYear.Value = disc.Year;

                nudDiscNumber.Value = disc.DiscNumber;
                nudDiscCount.Value = disc.DiscCount;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (lbTracks.Items.Count > 0)
            {
                if (!string.IsNullOrEmpty(cboAlbumArtist.Text))
                    lbTracks.Items.Cast<XmlTrack>().ToList().ForEach(x => x.AlbumArtist = cboAlbumArtist.Text);

                if (!string.IsNullOrEmpty(txtAlbum.Text))
                    lbTracks.Items.Cast<XmlTrack>().ToList().ForEach(x => x.Album = txtAlbum.Text);

                if (!string.IsNullOrEmpty(cboGenre.Text))
                    lbTracks.Items.Cast<XmlTrack>().ToList().ForEach(x => x.Genre = cboGenre.Text);

                if (nudYear.Value > 0)
                    lbTracks.Items.Cast<XmlTrack>().ToList().ForEach(x => x.Year = (uint)nudYear.Value);

                if (nudDiscNumber.Value > 0)
                    lbTracks.Items.Cast<XmlTrack>().ToList().ForEach(x => x.DiscNumber = (uint)nudDiscNumber.Value);

                if (nudDiscCount.Value > 0)
                    lbTracks.Items.Cast<XmlTrack>().ToList().ForEach(x => x.DiscCount = (uint)nudDiscCount.Value);

                if (tvBands.SelectedNode.Tag is XmlDisc)
                {
                    XmlDisc disc = tvBands.SelectedNode.Tag as XmlDisc;
                    disc.DiscNumber = (uint)nudDiscNumber.Value;
                    disc.DiscCount = (uint)nudDiscCount.Value;
                    disc.Year = (uint)nudYear.Value;
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.CopyMusicToLibrary = chkCopyMusicToLibrary.Checked;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}