using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;
using System.Data.SQLite;

namespace VectorNet.Server
{
    public partial class Server
    {
        public Server(string XMLConfigFile)
            : this()
        {
            ConfigurationFile = XMLConfigFile;
            LoadConfig();
        }
        public Server()
        {
            string dbFile = @"vnet.db";
            if (!System.IO.File.Exists(dbFile))
                SQLiteConnection.CreateFile(dbFile);
            using (SQLiteConnection cn = new SQLiteConnection(@"Data Source=" + dbFile))
            {
                cn.Open();
                using (SQLiteCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = @"";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteReader();
                }
                cn.Close();
            }
        }
    }
}
