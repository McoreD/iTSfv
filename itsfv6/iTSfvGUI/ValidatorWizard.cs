using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace iTSfvGUI
{
    public partial class ValidatorWizard : Form
    {
        public ValidatorWizard()
        {
            InitializeComponent();
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
           Program.Linker.LoadApplication(); 
        }

        private void ValidatorWizard_Load(object sender, EventArgs e)
        {

        }
    }
}
