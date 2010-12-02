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

        //Timers
        protected Timer timerCheck;
        protected Timer timerGarbage;
        protected Timer timerFloodDecrement;

        protected User console;
        protected QueueSharing Queues;

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

        /// <summary>
        /// Server initialization
        /// </summary>
        protected void ServerInit()
        {
            Users = new List<User>();
            Channels = new List<Channel>();

            PacketSendBufferer.Init(Config.Network.SendBufferInterval);

            SetupRegex();
            SetupFloodDictionaries();
            SetupTimers();
            CreateDefaultChannels();
            InitCommandTables();

            database = new Database();
            database.Open(Config.DatabaseFilename);

            console = new User(null, true);
            console.RealUsername = "";
        }

        /// <summary>
        /// Sets up server timers
        /// </summary>
        protected void SetupTimers()
        {
            timerCheck = new Timer(Config.Timers.CheckInterval);
            timerCheck.Elapsed += new ElapsedEventHandler(timerCheck_Elapsed);
            timerCheck.Start();

            timerGarbage = new Timer(Config.Timers.GarbageInterval);
            timerGarbage.Elapsed += new ElapsedEventHandler(timerGarbage_Elapsed);
            timerGarbage.Start();

            timerFloodDecrement = new Timer(Config.Timers.FloodDecrementInterval);
            timerFloodDecrement.Elapsed += new ElapsedEventHandler(timerFloodDecrement_Elapsed);
            timerFloodDecrement.Start();
        }
    }
}
