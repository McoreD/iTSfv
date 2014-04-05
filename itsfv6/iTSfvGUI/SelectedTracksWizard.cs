using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using iTSfvLib;

namespace iTSfvGUI
{
    public partial class SelectedTracksWizard : Form
    {
        public SelectedTracksWizard()
        {
            InitializeComponent();
        }

        private void SelectedTracksWizard_Shown(object sender, EventArgs e)
        {
            Program.Linker.LoadApplication();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            foreach (XmlTrack track in Program.Linker.GetSelectedTracks())
            {
                Console.WriteLine(track.Name);
            }
            MessageBox.Show(Program.Linker.SelectedTracksCount.ToString());
        }
    }
}
