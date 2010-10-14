using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using VectorNet.Server;

namespace VectorNet.Server.GUI
{
    public partial class frmMain : Form
    {
        protected Server VNet;

        public frmMain()
        {
            InitializeComponent();
            VNet = new Server("config.xml");
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            VNet.StartListening();
        }
    }
}
