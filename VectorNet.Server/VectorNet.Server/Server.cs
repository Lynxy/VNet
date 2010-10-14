using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
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
            CreateConsoleUser();
        }

        protected void CreateConsoleUser()
        {
            console = new User(null);
            console.IsConsole = true;
        }
    }
}
