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

        protected int GetUsernameNumber(User user)
        {
            //TODO: Function needs improvement. Eg if 4 users connect,
            //it will display up to #4. if user #3 disconnects, then reconnects,
            //he will be assigned #4

            //IEnumerable<User> users = Users.Where(u => u.RealUsername.ToLower() == user.RealUsername.ToLower());
            //return users.Count();

            List<int> accountNumbers = new List<int>();
            foreach (User u in Users.Where(u => u.RealUsername.ToLower() == user.RealUsername.ToLower() && u.AccountNumber != 0))
                accountNumbers.Add(u.AccountNumber);
            
            if (accountNumbers.Count() > 0)
            {
                int[] aryAccountNumbers = new int[accountNumbers.Count()];

                // We need to sort the username numbers so that
                // we can find a break (if it exists) in the pattern
                accountNumbers.Sort();
                aryAccountNumbers = accountNumbers.ToArray();

                int increment = 1;
                for (int count = 0; count < aryAccountNumbers.Count(); count++)
                {
                    // If the current account number sequence5 is broken
                    // I.E. 1, 3, 4, 5, then retureen the next available number, Ex. 2
                    if (increment != aryAccountNumbers[count])
                        return increment;

                    increment++;
                }
                return increment;
            }
            else
                return 1;
        }
    }
}
