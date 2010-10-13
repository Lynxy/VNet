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
            SetupEvents();

            listener.Listen(10);

            ConductTests();
        }

        void SetupVars()
        {
            listener = new TcpListenerWrapper(this, 4800);
        }

        void SetupEvents()
        {
            listener.OnClientConnected += new TcpListenerWrapper.ClientConnectedDelegate(listener_OnClientConnected);

            ClientHandler.UserPacketReceived += new ClientHandler.UserPacketReceivedDelegate(ClientHandler_UserPacketReceived);
        }

        void ConductTests()
        {
            TcpClientWrapper client = new TcpClientWrapper();
            client.Connect("127.0.0.1", 4800);
            ClientHandler.AddNewClient(client);
        }

        void listener_OnClientConnected(TcpClientWrapper client)
        {
            ClientHandler.AddNewClient(client);

            byte[] dat = packet.Clear().InsertString("test");
            client.AsyncSend(dat, dat.Length);
        }

        void ClientHandler_UserPacketReceived(User user, byte[] data)
        {
            MessageBox.Show("Data recv: " + Encoding.ASCII.GetString(data));

            //byte[] dat = packet.Clear().InsertString("rawr");
            //user.Socket.AsyncSend(dat, dat.Length);
        }
    }
}
