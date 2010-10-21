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
using Lynxy.Network;

namespace VectorNet.Server.GUI
{
    public partial class frmMain : Form
    {
        protected Server VNet;

        public frmMain()
        {
            InitializeComponent();
            SetupServer();
        }

        protected void SetupServer()
        {
            VNet = new Server("config.xml");

            VNet.WireConsoleUserDataRecv(RecvData);
            VNet.StartListening();
        }

        private void RecvData(byte[] data)
        {
            return;

            PacketReader reader = new PacketReader(data);
            byte PacketID = reader.ReadByte();

            string username;
            string channel;
            string text;
            byte flags;

            switch (PacketID)
            {
                case 0x00: //user joined server
                    break;

                case 0x01: //user joined server
                    break;

                case 0x02: //user list
                    break;

                case 0x03: //user talk
                    username = reader.ReadStringNT();
                    flags = reader.ReadByte();
                    channel = reader.ReadStringNT();
                    text = reader.ReadStringNT();
                    AddChat("<" + channel + " - " + username + " [" + flags.ToString("X") + "]> " + text);
                    break;

                case 0x04: //user join channel
                    username = reader.ReadStringNT();
                    flags = reader.ReadByte();
                    channel = reader.ReadStringNT();
                    AddChat("-- " + username + " [" + flags.ToString("X") + "] joined channel " + channel + " --");
                    break;
            }
        }

        private void AddChat(string msg)
        {
            for (int i = 0; i < 32; i++)
                msg = msg.Replace((char)i, '.');
            rtbChat.Invoke(new Action(delegate
            {
                rtbChat.Text += "[" + DateTime.Now.ToShortTimeString() + "] " + msg + "\r\n";
                rtbChat.SelectionStart = rtbChat.Text.Length;
                rtbChat.ScrollToCaret();
            }));
        }

    }
}
