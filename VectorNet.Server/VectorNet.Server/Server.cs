using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;
using System.Timers;

namespace VectorNet.Server
{
    public partial class Server
    {
        public delegate void ReceiveDataDelegate(byte[] data);

        protected List<User> Users;
        protected List<Channel> Channels;
        protected Timer timerCheck;

        protected User console;

        public Server(string XMLConfigFile)
        {
            ConfigurationFile = XMLConfigFile;
            LoadConfig();
            SaveConfig();
            ServerInit();
        }
        public Server(string XMLConfigFile, ConfigurationData configData)
        {
            ConfigurationFile = XMLConfigFile;
            Config = configData;
            SaveConfig();
            ServerInit();
            
        }

        protected void ServerInit()
        {
            Users = new List<User>();
            Channels = new List<Channel>();

            timerCheck = new Timer(1000);
            timerCheck.Elapsed += new ElapsedEventHandler(timerCheck_Elapsed);
            timerCheck.Start();

            CreateDefaultChannels();
            ConnectToDatabase("vnet.sqlite");

            console = new User(null, true);
            console.Username = "";
        }

        protected void timerCheck_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Total connections = " + ServerStats.totalConnections);
            Console.WriteLine("Users online = " + ServerStats.usersOnline);
            Console.WriteLine("KB sent = " + (ServerStats.bytesSent / 1024)
                + " (" + ((ServerStats.bytesSent - ServerStats.lastBytesSent) / 1024) + " kb/s)");
            Console.WriteLine("KB recv = " + (ServerStats.bytesRecv / 1024));

            ServerStats.lastBytesSent = ServerStats.bytesSent;
            ServerStats.lastBytesRecv = ServerStats.bytesRecv;
        }

        public void WireConsoleUserDataRecv(Action<byte[]> ReceiveDataEvent)
        {
            console.SendData += ReceiveDataEvent;
        }
    }
}
