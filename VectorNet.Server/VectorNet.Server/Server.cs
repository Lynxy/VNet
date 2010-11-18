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

            SetupRegex();
            SetupTimers();
            CreateDefaultChannels();
            InitCommandTables();

            database = new Database();
            database.Open(Config.DatabaseFilename);

            console = new User(null, true);
            console.RealUsername = "";
        }

        protected void SetupTimers()
        {
            timerCheck = new Timer(Config.TimerCheckInterval);
            timerCheck.Elapsed += new ElapsedEventHandler(timerCheck_Elapsed);
            timerCheck.Start();

            timerGarbage = new Timer(Config.TimerGarbageInterval);
            timerGarbage.Elapsed += new ElapsedEventHandler(timerGarbage_Elapsed);
            timerGarbage.Start();
        }
    }
}
