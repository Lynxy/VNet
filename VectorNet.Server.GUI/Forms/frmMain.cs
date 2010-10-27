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

            VNet.StartListening();

            VNet.EventServerException += new Server.ServerExceptionDelegate(VNet_EventServerException);

            VNet.EventUserJoinServer += new Server.UserJoinServerDelegate(VNet_EventUserJoinServer);
            VNet.EventUserLeftServer += new Server.UserLeftServerDelegate(VNet_EventUserLeftServer);
            
            VNet.EventUserJoinChannel += new Server.UserJoinChannelDelegate(VNet_EventUserJoinChannel);
            VNet.EventUserLeftChannel += new Server.UserLeftChannelDelegate(VNet_EventUserLeftChannel);

            VNet.EventUserTalk += new Server.UserTalkDelegate(VNet_EventUserTalk);
            VNet.EventUserEmote += new Server.UserEmoteDelegate(VNet_EventUserEmote);
        }

        void VNet_EventServerException(Exception err)
        {
            AddChat("[ERROR] " + err.Message); //stack trace anyone?
        }

        void VNet_EventUserJoinServer(string username, byte flags)
        {
            
        }

        void VNet_EventUserLeftServer(string username, byte flags)
        {

        }

        void VNet_EventUserJoinChannel(string username, byte flags, string channel)
        {
            AddChat("-- " + username + " [" + flags.ToString("X") + "] joined channel " + channel + " --");
        }

        void VNet_EventUserLeftChannel(string username, byte flags, string channel)
        {
            
        }

        void VNet_EventUserTalk(string username, byte flags, string channel, string message)
        {
            AddChat("<" + channel + " - " + username + " [" + flags.ToString("X") + "]> " + message);
        }

        void VNet_EventUserEmote(string username, byte flags, string channel, string message)
        {
            
        }

        private void AddChat(string msg)
        {
            for (int i = 0; i < 32; i++)
                msg = msg.Replace((char)i, '.');
            txtChat.Invoke(new Action(delegate
            {
                txtChat.Text += "[" + DateTime.Now.ToShortTimeString() + "] " + msg + "\r\n";
                txtChat.SelectionStart = txtChat.Text.Length;
                txtChat.ScrollToCaret();
            }));
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            VNet.Shutdown();
        }

        private void tmrTest_Tick(object sender, EventArgs e)
        {
            string str = "";
            str += "Total connections = " + ServerStats.totalConnections + " (" + ServerStats.usersOnline + " online)";
            str += "\r\nKB sent = " + (ServerStats.bytesSent / 1024)
                + " (" + ((ServerStats.bytesSent - ServerStats.lastBytesSent) / 1024) + " kb/s) | " +
                "KB recv = " + (ServerStats.bytesRecv / 1024);

            int c1, c2;
            System.Threading.ThreadPool.GetAvailableThreads(out c1, out c2);
            str += "\r\nGetAvailableThreads = " + c1 + "/" + c2;

            str += "\r\nMissed/Total packets = " + ServerStats.test;

            txtStatus.Text = str;
        }

    }
}
