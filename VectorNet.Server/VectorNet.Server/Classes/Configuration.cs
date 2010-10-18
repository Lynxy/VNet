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
            protected string _HostedBy = "";
            protected string _MOTD = "";
            protected bool _AllowAES = true;
            protected bool _AllowChallenges = true;
            protected bool _UseIdleSystem = false;

            public int ListenPort { get { return _ListenPort; } set { _ListenPort = value; } }
            public string HostedBy { get { return _HostedBy; } set { _HostedBy = value; } }
            public string MOTD { get { return _MOTD; } set { _MOTD = value; } }
            public bool AllowAES { get { return _AllowAES; } set { _AllowAES = value; } }
            public bool UseIdleSystem { get { return _UseIdleSystem; } set { _UseIdleSystem = value; } }
        }




        protected ConfigurationData Config = new ConfigurationData();
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
