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
            using (SQLiteDataReader reader = database.ExecuteReader("SELECT * FROM [Users] WHERE [Username] = '" + username + "'"))
            {
                if (!reader.HasRows)
                    return AccountState.NewAccount;
                string dbPass = reader["Password"] as string;
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
            database.ExecuteNonQuery("INSERT INTO [Users] ([Username], [Password], [RegistrationIP], [Banned]) VALUES ('" + username + "', '" + password + "', '" + IP + "', 'false')");
        }

        protected void UpdateLastLogin(string username)
        {
            database.ExecuteNonQuery("UPDATE [Users] SET [LastLogin] = '" + GetNow() + "' WHERE [Username] = '" + username + "'");
        }

        protected void InsertChallenge(string username, string challenge)
        { 
        
        }

        protected bool IsEmptyChallenge(string username, string challenge)
        {
            return false;
        }

        protected bool GetChallengeState(string username, string challenge)
        { 
            //database.ExecuteNonQuery("SELECT [Challenge] FROM [Users] WHERE [Username] = '" + username + "'");
            return true;
        }
    }
}
