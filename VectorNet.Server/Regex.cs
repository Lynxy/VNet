using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;
using System.Text.RegularExpressions;

namespace VectorNet.Server
{
    public partial class Server
    {
        Regex rxUsername;

        /// <summary>
        /// Sets up regular expressions
        /// </summary>
        protected void SetupRegex()
        {
            rxUsername = new Regex(Config.NamingRules.UsernameRegex);
        }

        /// <summary>
        /// Checks a username against a regular expression
        /// </summary>
        /// <param name="username">The username to check</param>
        protected bool CheckUsernameRegex(string username)
        {
            return rxUsername.IsMatch(username);
        }
    }
}
