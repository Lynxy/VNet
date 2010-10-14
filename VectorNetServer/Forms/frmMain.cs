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

using VNET.RichText;
using VNET.Users;

namespace VNET.Main
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
            clients.AddNewClient(client);
        }

        void ClientHandler_UserPacketReceived(User user, PacketReader reader)
        {
            byte packetId = reader.ReadByte();

            switch (packetId)
            {
                case 0x00:
                    user.Packet.Clear()
                        .InsertByte(0) //logon result
                        .InsertByte(0) //chalenge byte
                        .InsertStringNT("VectorNet Server")
                        .InsertStringNT("TestEnv")
                        .InsertDWord(1337)
                        .InsertByte(0)
                        .Send(0);
                    break;
            }

            //byte[] dat = packet.Clear().InsertString("rawr");
            //user.Socket.AsyncSend(dat, dat.Length);
        }

    }
}
