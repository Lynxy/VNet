using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorNet.Server
{
    class AccountHandlerClass
    {
        public byte GetAccState(byte[] username)
        { 
           // Gets user's rank from database

            // 0x01 = User is ok
            // 0x02 = User is banned
            return 0x01;
        }


    }
}
