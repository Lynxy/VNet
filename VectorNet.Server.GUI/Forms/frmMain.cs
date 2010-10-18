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
        protected bool serverMode = false;

        public frmMain()
        {
            InitializeComponent();
            DiscoverServerMode();
        }

        protected void DiscoverServerMode()
        {
            try
            {
                System.Reflection.Assembly.LoadFrom("VectorNet.Server.dll");
                serverMode = true;
            }
            catch (Exception) { }

            MessageBox.Show("serverMode = " + serverMode.ToString());
            if (serverMode)
                loader = new ServerLoader(RecvData);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (serverMode)
                loader.HandleConsoleCommand("join wtf");
        }

        private void RecvData(byte[] data)
        {
            MessageBox.Show("recv: " + Encoding.ASCII.GetString(data).Replace((char)0, '.'));
        }

    }
}
