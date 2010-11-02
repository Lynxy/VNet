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

        protected void SetupRegex()
        {
            rxUsername = new Regex(Config.UsernameRegex);
        }

        protected bool CheckUsernameRegex(string username)
        {
            return rxUsername.IsMatch(username);
        }
    }
}
