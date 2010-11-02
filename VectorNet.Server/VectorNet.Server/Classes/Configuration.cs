using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;

namespace VectorNet.Server
{
    public partial class Server
    {
        public class ConfigurationData
        {
            protected int _ListenPort = 4800;
            protected int _ReadBufferSize = 1024;
            protected int _SendBufferInterval = 200;
            protected int _MaxConnectionsBacklog = 20;
            protected int _MaxClientSendBacklog = 30;
            protected int _ClientCloseWait = 2000;
            protected string _DatabaseFilename = "vnet.sqlite";
            protected string _MainChannel = "Main";
            protected string _VoidChannel = "The Void";
            protected string _HostedBy = "";
            protected string _MOTD = "";
            protected int _UsernameMinLength = 2;
            protected int _UsernameMaxLength = 14;
            protected string _UsernameRegex = @"^([a-zA-Z0-9]+?[\._\-]{0,1})*?$"; //alpha numeric 1 or more times + optional other char 0 to 1 times, repeat
            protected bool _AllowAES = true;
            protected bool _AllowChallenges = true;
            protected bool _UseIdleSystem = false;
            protected int _TimerCheckInterval = 1000;
            protected int _TimerGarbageInterval = 5000;
            protected int _MaxUsersPerIP = 10;
            protected int _MaxUsersPerChannel = 50;

            public int ListenPort { get { return _ListenPort; } set { _ListenPort = value; } }
            public int ReadBufferSize { get { return _ReadBufferSize; } set { _ReadBufferSize = value; } }
            public int SendBufferInterval { get { return _SendBufferInterval; } set { _SendBufferInterval = value; } }
            public int MaxConnectionsBacklog { get { return _MaxConnectionsBacklog; } set { _MaxConnectionsBacklog = value; } }
            public int MaxClientSendBacklog { get { return _MaxClientSendBacklog; } set { _MaxClientSendBacklog = value; } }
            public int ClientCloseWait { get { return _ClientCloseWait; } set { _ClientCloseWait = value; } }
            public string DatabaseFilename { get { return _DatabaseFilename; } set { _DatabaseFilename = value; } }
            public string MainChannel { get { return _MainChannel; } set { _MainChannel = value; } }
            public string VoidChannel { get { return _VoidChannel; } set { _VoidChannel = value; } }
            public string HostedBy { get { return _HostedBy; } set { _HostedBy = value; } }
            public string MOTD { get { return _MOTD; } set { _MOTD = value; } }
            public int UsernameMinLength { get { return _UsernameMinLength; } set { _UsernameMinLength = value; } }
            public int UsernameMaxLength { get { return _UsernameMaxLength; } set { _UsernameMaxLength = value; } }
            public string UsernameRegex { get { return _UsernameRegex; } set { _UsernameRegex = value; } }
            public bool AllowAES { get { return _AllowAES; } set { _AllowAES = value; } }
            public bool UseIdleSystem { get { return _UseIdleSystem; } set { _UseIdleSystem = value; } }
            public int TimerCheckInterval { get { return _TimerCheckInterval; } set { _TimerCheckInterval = value; } }
            public int TimerGarbageInterval { get { return _TimerGarbageInterval; } set { _TimerGarbageInterval = value; } }
            public int MaxUsersPerIP { get { return _MaxUsersPerIP; } set { _MaxUsersPerIP = value; } }
            public int MaxUsersPerChannel { get { return _MaxUsersPerChannel; } set { _MaxUsersPerChannel = value; } }
        }

        static protected ConfigurationData Config = new ConfigurationData();
        protected string ConfigurationFile = "config.xml";

        protected void SaveConfig()
        {
            SaveConfig(ConfigurationFile);
        }

        protected void SaveConfig(string filename)
        {
            XmlSerializer x = new XmlSerializer(Config.GetType());
            using (StreamWriter sw = new StreamWriter(filename))
            {
                x.Serialize(sw, Config);
                sw.Close();
            }
        }

        protected void LoadConfig()
        {
            LoadConfig(ConfigurationFile);
        }

        protected void LoadConfig(string filename)
        {
            XmlSerializer x = new XmlSerializer(Config.GetType());
            if (!File.Exists(filename))
                return;
            using (StreamReader sr = new StreamReader(filename))
            {
                Config = (ConfigurationData)x.Deserialize(sr);
                sr.Close();
            }
        }
    }
}
