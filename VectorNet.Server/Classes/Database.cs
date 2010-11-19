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
        protected Database database;

        protected class Database
        {
            protected SQLite database;

            public void Open(string databaseFile)
            {
                database = new SQLite(databaseFile);
            }

            public void Close()
            {
                database.Close();
            }

            public SQLite Query(string query)
            {
                database.Query = query;
                return database;
            }
        }
    }
}
