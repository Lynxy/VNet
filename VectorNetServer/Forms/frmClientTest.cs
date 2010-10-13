using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Lynxy.Network;

namespace VectorNetServer
{
    public partial class frmClientTest : Form
    {
        ClientHandler clients = new ClientHandler();


        public frmClientTest()
        {
            InitializeComponent();
            clients.UserPacketReceived += new ClientHandler.UserPacketReceivedDelegate(clients_UserPacketReceived);

            TcpClientWrapper sock = new TcpClientWrapper();
            //try
            //{
                sock.ConnectionEstablished += new TcpClientWrapper.ConnectionEstablishedDelegate(sock_ConnectionEstablished);
                sock.AsyncConnect("127.0.0.1", 4800);
            //}
            //catch (Exception)
            //{ }

        }

        void sock_ConnectionEstablished(TcpClientWrapper sender)
        {
            User user = clients.AddNewClient(sender);
            user.Packet.Clear().InsertStringNT("TestUser").InsertStringNT("nopass").InsertStringNT("myClient").InsertByte(0).Send(0);
        }

        void clients_UserPacketReceived(User user, PacketReader reader)
        {
            byte packetId = reader.ReadByte();

            switch (packetId)
            {
                case 0x00:
                    MessageBox.Show("Data recv: packet id: " + packetId.ToString() + ": " + Encoding.ASCII.GetString(reader.ReadToEnd()).Replace((char)0, '.'));
                    break;
            }
        }
    }
}
