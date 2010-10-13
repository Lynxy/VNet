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

using Lynxy.Network;
using Lynxy.Security;

namespace VectorNetServer
{
    public partial class frmMain : Form
    {
        TcpListenerWrapper listener;
        ClientHandler clients = new ClientHandler();

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

            //tests
            frmClientTest cli = new frmClientTest();
            cli.Show();
        }

        void SetupVars()
        {
            listener = new TcpListenerWrapper(this, 4800);
        }

        void SetupEvents()
        {
            listener.OnClientConnected += new TcpListenerWrapper.ClientConnectedDelegate(listener_OnClientConnected);

            clients.UserPacketReceived += new ClientHandler.UserPacketReceivedDelegate(ClientHandler_UserPacketReceived);
        }

        void listener_OnClientConnected(TcpClientWrapper client)
        {
            User user = clients.AddNewClient(client);
        }

        void ClientHandler_UserPacketReceived(User user, PacketReader reader)
        {
            byte packetId = reader.ReadByte();

            switch (packetId)
            {
                case 0x00:
                    user.Packet.Clear();
                    user.Packet.InsertByte(0); //logon result
                    user.Packet.InsertByte(0); //chalenge byte
                    user.Packet.InsertStringNT("VectorNet Server");
                    user.Packet.InsertStringNT("TestEnv");
                    user.Packet.InsertDWord(1337);
                    user.Packet.InsertByte(0);
                    user.Packet.Send(0);
                    break;
            }

            //byte[] dat = packet.Clear().InsertString("rawr");
            //user.Socket.AsyncSend(dat, dat.Length);
        }
    }
}
