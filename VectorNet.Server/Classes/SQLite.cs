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
            protected string _query = "";

            public SQLite(string databaseFile)
            {
                dbFile = databaseFile;
                CheckDBExistsAndConnect();
            }

            public string Query
            {
                get { return _query; }
                set
                {
                    cmd.Parameters.Clear();
                    _query = value;
                }
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

                cmd.CommandText = sr.ReadToEnd();
                cmd.ExecuteNonQuery();
            }

            public SQLite AddParameter(string parameterName, object parameterValue)
            {
                cmd.Parameters.Add(new SQLiteParameter(parameterName, parameterValue));
                return this;
            }

            public SQLiteDataReader ExecuteReader()
            {
                //if (cmd.Transaction == null)
                //    cmd.Transaction = conn.BeginTransaction();
                cmd.CommandText = Query;
                return cmd.ExecuteReader();
            }

            public object ExecuteScalar()
            {
                //if (cmd.Transaction == null)
                //    cmd.Transaction = conn.BeginTransaction();
                cmd.CommandText = Query;
                return cmd.ExecuteScalar();
            }

            public int ExecuteNonQuery()
            {
                //if (cmd.Transaction == null)
                //    cmd.Transaction = conn.BeginTransaction();
                cmd.CommandText = Query;
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
