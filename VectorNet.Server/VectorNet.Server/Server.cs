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

        protected User console;

        public Server(string XMLConfigFile)
        {
            ConfigurationFile = XMLConfigFile;
            LoadConfig();
            ServerInit();
        }
        public Server(string XMLConfigFile, ConfigurationData configData)
        {
            ConfigurationFile = XMLConfigFile;
            Config = configData;
            ServerInit();
        }

        protected void ServerInit()
        {

        }

        public void CreateConsoleUser(Action<byte[]> ReceiveDataEvent)
        {
            console = new User(null, true);
            console.SendData += ReceiveDataEvent;
        }
    }
}
