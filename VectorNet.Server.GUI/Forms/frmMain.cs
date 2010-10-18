using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using VectorNet.Server;

namespace VectorNet.Server.GUI
{
    public partial class frmMain : Form
    {
        protected ServerLoader loader = null;
        protected bool clientMode = false;

        public frmMain(bool serverMode)
        {
            MessageBox.Show("serverMode = " + serverMode.ToString());
            if (serverMode)
                loader = new ServerLoader();
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
        }

        
    }
}
