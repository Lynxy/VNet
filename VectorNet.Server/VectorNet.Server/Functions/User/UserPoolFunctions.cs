using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected User GetUserByName(string username)
        {
            IEnumerable<User> ret = from u in Users
                       where u.Username.ToLower() == username.ToLower()
                       && u.IsOnline == true
                       select u;
            if (ret == null)
                return null;
            return ret.First();
        }
    }
}
