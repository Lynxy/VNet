using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        //User Pool Functions
        //This class is for methods that must look at the Users dictionary, not a single user
        protected User GetUserByName(string username)
        {
            User ret = Users.Find(u =>
                u.IsOnline == true
                && u.Username != null
                && u.Username.ToLower() == username.ToLower()
                );
            return ret;
        }
    }
}
