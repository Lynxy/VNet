using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Xml.Serialization;

namespace VectorNet.Server
{
    public partial class Server
    {
        public class ConfigData
        {
            public bool isAES { get; set; }
            public bool isChallenge { get; set; }
            public bool isIdleSystem { get; set; }

            public int ListenPort { get; set; }
            public string HostedBy { get; set; }
            public string MOTD { get; set; }
        }

        static public class Config
        {
            static public ConfigData Variables = new ConfigData();
            static public string File = "config.xml";

            static public void SaveConfig()
            {
                XmlSerializer x = new XmlSerializer(Variables.GetType());
                using (StreamWriter sw = new StreamWriter(File))
                {
                    x.Serialize(sw, Variables);
                    sw.Close();
                }
            }

            static public void LoadConfig()
            {
                XmlSerializer x = new XmlSerializer(Variables.GetType());
                using (StreamReader sr = new StreamReader(File))
                {
                    Variables = (ConfigData)x.Deserialize(sr);
                    sr.Close();
                }
            }
        }
    }
}
