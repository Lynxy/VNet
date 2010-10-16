using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LynxVN
{
    public class User
    {
        public string Username { get; set; }
        public string Client { get; set; }
        public int Ping { get; set; }
        public byte Flags { get; set; }
    }
}
