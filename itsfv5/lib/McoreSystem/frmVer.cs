using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace McoreSystem
{
    public partial class frmVer : Form
    {
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern bool SetForegroundWindow(int hWnd);

        public frmVer(Icon pic, Image img, string ques, string[] ver)
        {
            InitializeComponent();

            if (pic != null)
                this.Icon = pic;
            if (img != null)
                this.pbApp.Image = img;

            this.lblVer.Text = ques;
            StringBuilder sb = new StringBuilder();
            foreach (string s in ver)
            {
                sb.AppendLine(s);
            }
            this.txtVer.Text = sb.ToString();
            SetForegroundWindow(this.Handle.ToInt32());
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

    }
}
