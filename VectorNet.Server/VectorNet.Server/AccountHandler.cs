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
            using (SQLiteDataReader reader = database.Query("SELECT * FROM [Users] WHERE [Username] = @Username")
                .AddParameter("@Username", username)
                .ExecuteReader())
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
            database.Query("INSERT INTO [Users] ([Username], [Password], [RegistrationIP], [Banned]) VALUES (@Username, @Password, @IP, 'false')")
                .AddParameter("@Username", username)
                .AddParameter("@Password", password)
                .AddParameter("@IP", IP)
                .ExecuteNonQuery();
        }

        protected void UpdateLastLogin(string username)
        {
            database.Query("UPDATE [Users] SET [LastLogin] = @LastLogin WHERE [Username] = @Username")
                .AddParameter("@LastLogin", GetNow())
                .AddParameter("@Username", username)
                .ExecuteNonQuery();
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

        protected int AttachAccountNumber(string user)
        {
            int accNumber = 0;

            foreach (User cu in Users)
                if (user == cu.RealName)
                    accNumber += 1;
            
            return (accNumber + 1);
        }
    }
}
