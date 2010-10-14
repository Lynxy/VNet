using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;
//using System.Data.SQLite;

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
            //using (SQLiteConnection cn = new SQLiteConnection(@"DataSource=mydatabase.db"))
            //{
            //    cn.Open();
            //    using (SQLiteCommand cmd = cn.CreateCommand())
            //    {
            //        cmd.CommandText = @"";
            //        cmd.CommandType = System.Data.CommandType.Text;
            //        cmd.ExecuteReader();
            //    }
            //    cn.Clone();
            //}
        }
    }
}
