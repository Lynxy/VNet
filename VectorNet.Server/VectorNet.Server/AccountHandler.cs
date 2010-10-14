using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected byte GetAccountState(byte[] username)
        { 
           // Gets an account's state from database

            // 0x01 = User is ok
            // 0x02 = User is banned
            return 0x01;
        }


    }
}
