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

            if (serverMode)
                loader = new ServerLoader(RecvData);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            AddChat("Server mode = " + serverMode.ToString());
            if (serverMode)
                loader.HandleConsoleCommand("join test");


        }

        private void RecvData(byte[] data)
        {
            AddChat(Encoding.ASCII.GetString(data) + "\r\n");
        }

        private void AddChat(string msg)
        {
            for (int i = 0; i < 32; i++)
                msg = msg.Replace((char)i, '.');
            rtbChat.Invoke(new Action(delegate
            {
                rtbChat.Text += "[" + DateTime.Now.ToShortTimeString() + "] " + msg + "\r\n";
            }));
        }

    }
}
