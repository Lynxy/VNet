using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;
using System.Data.SqlClient;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected SQLite database;

        protected void ConnectToDatabase(string databaseFile)
        {
            database = new SQLite(databaseFile);
        }
    }
}
