using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        public delegate void ReceiveDataDelegate(byte[] data);

        protected List<User> Users;
        protected List<Channel> Channels;

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
            CreateMainChannel("Main");
            ConnectToDatabase("vnet.sqlite");
        }

        public void CreateConsoleUser(Action<byte[]> ReceiveDataEvent)
        {
            console = new User(null, true);
            console.SendData += ReceiveDataEvent;
        }
    }
}
