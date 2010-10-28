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
                CheckDBExistsAndConnect();
            }

            public void Close()
            {
                conn.Close();
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

            protected void CheckDBExistsAndConnect()
            {
                if (System.IO.File.Exists(dbFile))
                    ConnectDB();
                else
                {
                    SQLiteConnection.CreateFile(dbFile);
                    ConnectDB();
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
                System.Reflection.Assembly asm = this.GetType().Assembly;
                string asmName = asm.GetName().Name.ToString();

                System.IO.Stream SQLStream = asm.GetManifestResourceStream(asmName + ".Data.SQLite_Create_VnetDB.txt");
                System.IO.StreamReader sr = new System.IO.StreamReader(SQLStream);

                ExecuteNonQuery(sr.ReadToEnd());
            }

            public SQLiteDataReader ExecuteReader(string query)
            {
                //if (cmd.Transaction == null)
                //    cmd.Transaction = conn.BeginTransaction();
                cmd.CommandText = query;
                return cmd.ExecuteReader();
            }

            public object ExecuteScalar(string query)
            {
                //if (cmd.Transaction == null)
                //    cmd.Transaction = conn.BeginTransaction();
                cmd.CommandText = query;
                return cmd.ExecuteScalar();
            }

            public int ExecuteNonQuery(string query)
            {
                //if (cmd.Transaction == null)
                //    cmd.Transaction = conn.BeginTransaction();
                cmd.CommandText = query;
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
