using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;
using System.Text.RegularExpressions;
using System.Timers;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected Dictionary<string, int> floodDictConnection;

        protected void SetupFloodDictionaries()
        {
            floodDictConnection = new Dictionary<string, int>();
        }

        protected bool CheckFloodingConnection(string IP)
        {
            if (floodDictConnection.ContainsKey(IP) == false)
                return false;
            return floodDictConnection[IP] >= Config.FloodConditions.UserConnectionThreshold;
        }

        protected void IncrementFloodingConnection(string IP)
        {
            if (floodDictConnection.ContainsKey(IP) == false)
                floodDictConnection.Add(IP, 1);
            else
                floodDictConnection[IP]++;
        }

        protected void timerFloodDecrement_Elapsed(object sender, ElapsedEventArgs e)
        {
            List<string> keys = floodDictConnection.Keys.ToList();
            foreach (string ip in keys)
            {
                if (--floodDictConnection[ip] == 0)
                    floodDictConnection.Remove(ip);
            }
        }



    }
}
