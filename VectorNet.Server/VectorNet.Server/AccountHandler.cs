using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SQLite;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected enum AccountState
        {
            NewAccount = 0,
            AccountOk,
            InvalidPassword,
            AccountBanned
        }

        protected AccountState GetAccountState(string username, string password)
        {
            using (SQLiteDataReader reader = database.ExecuteReader("SELECT * FROM users WHERE username = '" + username + "'"))
            {
                if (!reader.HasRows)
                    return AccountState.NewAccount;
                string dbPass = reader["password"] as string;
                if (password != dbPass)
                    return AccountState.InvalidPassword;
                return AccountState.AccountOk;
            }
        }

        protected string GetNow()
        {
            return String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
        }

        protected void CreateNewAccount(string username, string password, string IP)
        {
            database.ExecuteNonQuery("INSERT INTO users (username, password, reg_ip) VALUES ('" + username + "', '" + password + "', '" + IP + "')");
        }

        protected void UpdateLastLogin(string username)
        {
            database.ExecuteNonQuery("UPDATE users SET last_login = '" + GetNow() + "' WHERE username = '" + username + "'");
        }

    }
}
