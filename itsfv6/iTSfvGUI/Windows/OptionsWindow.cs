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
    public partial class OptionsWindow : Form
    {
        public OptionsWindow(XMLSettings settings)
        {
            InitializeComponent();
            pgOptions.SelectedObject = settings;
        }
    }
}