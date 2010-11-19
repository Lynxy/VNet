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

        /// <summary>
        /// Retreives an account state from the database
        /// </summary>
        /// <param name="username">The username to lookup</param>
        /// <param name="password">Their password</param>
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

        /// <summary>
        /// Returns a string of the current time in the format: yyyy-MM-dd HH:mm:ss
        /// </summary>
        protected string GetNow()
        {
            return String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
        }

        /// <summary>
        /// Adds a new account to the database
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <param name="IP">The registration IP</param>
        protected void CreateNewAccount(string username, string password, string IP)
        {
            database.Query("INSERT INTO [Users] ([Username], [Password], [RegistrationIP], [Banned]) VALUES (@Username, @Password, @IP, 'false')")
                .AddParameter("@Username", username)
                .AddParameter("@Password", password)
                .AddParameter("@IP", IP)
                .ExecuteNonQuery();
        }

        /// <summary>
        /// Updates the last login time in database for a user. Sets it to the current time.
        /// </summary>
        /// <param name="username">The username to update</param>
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

        /// <summary>
        /// Gets a number for a username based on how many other users are logged on with that same username.
        /// </summary>
        /// <param name="user">The user to checking</param>
        protected int GetUsernameNumber(User user)
        {
            List<int> accountNumbers = new List<int>();
            foreach (User u in Users.Where(u => u.IsOnline == true && u.RealUsername.ToLower() == user.RealUsername.ToLower() && u.AccountNumber != 0))
                accountNumbers.Add(u.AccountNumber);
            
            if (accountNumbers.Count() == 0)
                return 1;

            // We need to sort the username numbers so that
            // we can find a break (if it exists) in the pattern
            accountNumbers.Sort();

            int count = 0;
            for (count = 0; count < accountNumbers.Count(); count++)
            {
                // If the current account number sequence5 is broken
                // I.E. 1, 3, 4, 5, then retureen the next available number, Ex. 2
                if ((count + 1) != accountNumbers[count])
                    return (count + 1);
            }
            return (count + 1);
        }
    }
}
