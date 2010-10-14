using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorNet.Server
{
    class ConfigClass
    {
        [DllImport("kernel32.dll")]
        public static extern int WritePrivateProfileString(string pSection, string pKey, string pValue, string pPath);
        [DllImport("kernel32.dll")]
        public static extern int GetPrivateProfileString(string pSection, string pName, string pKey, string returnStr, int nSize, string fileName);

        public bool isAES { get; set; }
        public bool isChallenge { get; set; }
        public bool isIdleSystem { get; set; }

        public string HostedBy { get; set; }
        public string MOTD { get; set; }

        public string ReadINI(string pSection, string pName, string pPath, object pDefault)
        {
             string rString = "";
             int clen = GetPrivateProfileString(pSection, pName, null, rString, 255, pPath);

             if (clen > 0)
                 return rString.Substring(1, clen);
             else
                 return (string)pDefault;
        }

        public void WriteINI(string pSection, string pName, string pValue, string pPath)
        {
            pPath = Environment.CurrentDirectory.ToString() + "\\" + pPath;
            
            WritePrivateProfileString(pSection, pName, pValue, pPath);
        }

        public void LoadConfig()
        {
            HostedBy = ReadINI("Main", "Hoster", "Config.ini", null).ToString();
            MOTD = ReadINI("Main", "MOTD", "Config.ini", null).ToString();

            isAES = Convert.ToBoolean(ReadINI("Main", "RequireAES", "Config.ini", false));
            isChallenge = Convert.ToBoolean (ReadINI("Main", "RequireChallenge", "Config.ini", false));
            isIdleSystem = Convert.ToBoolean(ReadINI("Main", "RequireIdleSystem", "Config.ini", false));
        }

        public void SaveConfig()
        { 

        }
    }
}
