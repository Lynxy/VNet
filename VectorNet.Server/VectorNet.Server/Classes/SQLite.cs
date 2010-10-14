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
        protected class SQLite : IDisposable
        {
            private bool disposed = false;
            protected string dbFile = "";
            protected SQLiteConnection conn;
            protected SQLiteCommand cmd;

            public SQLite(string databaseFile)
            {
                dbFile = databaseFile;
                CheckDBExists();
                ConnectDB();
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (!this.disposed)
                {
                    if (disposing)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                    disposed = true;
                }
            }

            ~SQLite()
            {
                Dispose(false);
            }

            protected void CheckDBExists()
            {
                if (!System.IO.File.Exists(dbFile))
                {
                    SQLiteConnection.CreateFile(dbFile);
                    SetupDB();
                }
            }

            protected void ConnectDB()
            {
                conn = new SQLiteConnection(@"Data Source=" + dbFile);
                conn.Open();

                cmd = conn.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
            }

            protected void SetupDB()
            {
                ExecuteNonQuery(@"DROP TABLE IF EXISTS users");
                ExecuteNonQuery(@"CREATE TABLE users (username VARCHAR(50), password char(32))");
            }

            public SQLiteDataReader ExecuteReader(string query)
            {
                cmd.CommandText = query;
                return cmd.ExecuteReader();
            }

            public object ExecuteScalar(string query)
            {
                cmd.CommandText = query;
                return cmd.ExecuteScalar();
            }

            public int ExecuteNonQuery(string query)
            {
                cmd.CommandText = query;
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
