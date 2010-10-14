using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Lynxy.Network;

namespace VectorNet.Client
{
    public partial class frmMain : Form
    {
        TcpClientWrapper socket;
        Packet packet;

        public frmMain()
        {
            InitializeComponent();

            socket = new TcpClientWrapper();
            socket.ConnectionEstablished += new TcpClientWrapper.ConnectionEstablishedDelegate(socket_ConnectionEstablished);
            socket.DataRead += new TcpClientWrapper.DataReadDelegate(socket_DataRead);

            packet = new Packet();
            packet.DataSent += new Packet.SendDataDelegate(packet_DataSent);
        }

        void socket_ConnectionEstablished(TcpClientWrapper sender)
        {
            socket.AsyncRead(1024, true);
            packet.Clear().InsertString("huh").Send(0);
        }

        void socket_DataRead(TcpClientWrapper sender, byte[] data)
        {

        }

        void packet_DataSent(byte[] data)
        {
            socket.AsyncSend(data, data.Length);
        }

        private void mnuConnect_Click(object sender, EventArgs e)
        {
            socket.AsyncConnect("127.0.0.1", 4800);
        }
    }
}
