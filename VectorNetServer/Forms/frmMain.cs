using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Net.Sockets;

using Lynxy.Network;
using Lynxy.Security;

namespace VectorNetServer
{
    public partial class frmMain : Form
    {
        TcpListenerWrapper listener;
        Packet packet = new Packet();

        public frmMain()
        {
            InitializeComponent();
            FormLoad();
        }

        void FormLoad()
        {
            SetupVars();

            listener.Listen(10);

            ConductTests();
        }

        void SetupVars()
        {
            listener = new TcpListenerWrapper(this, 4800);
            listener.OnClientConnected += new TcpListenerWrapper.ClientConnectedDelegate(listener_OnClientConnected);
        }

        void ConductTests()
        {
            TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", 4800);
        }

        void listener_OnClientConnected(System.Net.Sockets.TcpClient newSock)
        {
            byte[] dat = packet.Clear().InsertString("test");
            newSock.GetStream().BeginWrite(dat, 0, dat.Length, null, null);

        }
    }
}
