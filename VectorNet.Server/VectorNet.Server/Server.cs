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

        protected void timerCheck_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Total connections = " + ServerStats.totalConnections + " (" + ServerStats.usersOnline + " online)");
            Console.WriteLine("KB sent = " + (ServerStats.bytesSent / 1024)
                + " (" + ((ServerStats.bytesSent - ServerStats.lastBytesSent) / 1024) + " kb/s) | " +
                "KB recv = " + (ServerStats.bytesRecv / 1024));

            int c1, c2;
            System.Threading.ThreadPool.GetAvailableThreads(out c1, out c2);
            Console.WriteLine("GetAvailableThreads = " + c1 + "/" + c2);

            string test = "";
            foreach (User u in GetAllOnlineUsers())
                test += u.Socket.ct1 + "/" + u.Socket.ct2 + " ";
            ServerStats.test = test;
            Console.WriteLine("Missed/Total packets = " + test);

            ServerStats.lastBytesSent = ServerStats.bytesSent;
            ServerStats.lastBytesRecv = ServerStats.bytesRecv;
        }

        protected void timerGarbage_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerGarbage.Stop();

            foreach (User user in GetAllOfflineUsers())
                if (user != console)
                {
                    if (user.CanSendData)
                        DisconnectUser(user, "Server has detected your status is no longer Online.");
                    Users.Remove(user);
                }

            foreach (Channel chan in Channels.ToList())
            {
                foreach (User user in chan.GetCompleteUserList())
                    if (user == null)
                        chan.RemoveUser(user);
                AttemptToCloseChannel(chan);
            }

            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(delegate
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    timerGarbage.Start();
                }));
        }
    }
}
