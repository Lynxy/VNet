using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Xml.Serialization;

namespace VNET.Config
{
    public class ConfigData
    {
        public bool isAES { get; set; }
        public bool isChallenge { get; set; }
        public bool isIdleSystem { get; set; }

        public string HostedBy { get; set; }
        public string MOTD { get; set; }
    }

    static public class Config
    {
        static public ConfigData Variables = new ConfigData();

        static public void SaveConfig()
        {
            XmlSerializer x = new XmlSerializer(Variables.GetType());
            using (StreamWriter sw = new StreamWriter("config.xml"))
            {
                x.Serialize(sw, Variables);
                sw.Close();
            }
        }

        static public void LoadConfig()
        {
            XmlSerializer x = new XmlSerializer(Variables.GetType());
            using (StreamReader sr = new StreamReader("config.xml"))
            {
                Variables = (ConfigData)x.Deserialize(sr);
                sr.Close();
            }
        }
    }
}
