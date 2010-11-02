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
        protected Timer timerGarbage;

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

        public void Shutdown()
        {
            database.Close();
        }

        protected void ServerInit()
        {
            Users = new List<User>();
            Channels = new List<Channel>();

            SetupTimers();
            CreateDefaultChannels();
            ConnectToDatabase("vnet.sqlite");

            console = new User(null, true);
            console.Username = "";
        }

        protected void SetupTimers()
        {
            timerCheck = new Timer(1000);
            timerCheck.Elapsed += new ElapsedEventHandler(timerCheck_Elapsed);
            timerCheck.Start();

            timerGarbage = new Timer(5000);
            timerGarbage.Elapsed += new ElapsedEventHandler(timerGarbage_Elapsed);
            timerGarbage.Start();
        }
    }
}
