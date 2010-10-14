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
            public bool isAES { get; set; }
            public bool isChallenge { get; set; }
            public bool isIdleSystem { get; set; }

            public int ListenPort { get; set; }
            public string HostedBy { get; set; }
            public string MOTD { get; set; }
        }




        public ConfigurationData Config = new ConfigurationData();
        public string ConfigurationFile = "config.xml";

        public void SaveConfig()
        {
            SaveConfig(ConfigurationFile);
        }

        public void SaveConfig(string filename)
        {
            XmlSerializer x = new XmlSerializer(Config.GetType());
            using (StreamWriter sw = new StreamWriter(filename))
            {
                x.Serialize(sw, Config);
                sw.Close();
            }
        }

        public void LoadConfig()
        {
            LoadConfig(ConfigurationFile);
        }

        public void LoadConfig(string filename)
        {
            XmlSerializer x = new XmlSerializer(Config.GetType());
            using (StreamReader sr = new StreamReader(filename))
            {
                Config = (ConfigurationData)x.Deserialize(sr);
                sr.Close();
            }
        }
    }
}
