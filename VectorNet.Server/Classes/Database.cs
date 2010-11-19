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

            /// <summary>
            /// Establishes a connection to the database.
            /// </summary>
            /// <param name="databaseFile">The database file</param>
            public void Open(string databaseFile)
            {
                database = new SQLite(databaseFile);
            }

            /// <summary>
            /// Closes the database.
            /// </summary>
            public void Close()
            {
                database.Close();
            }

            /// <summary>
            /// Performs a query on the database. Returns an object that you do more with.
            /// </summary>
            /// <param name="query">The query text.</param>
            /// <returns></returns>
            public SQLite Query(string query)
            {
                database.Query = query;
                return database;
            }
        }
    }
}
