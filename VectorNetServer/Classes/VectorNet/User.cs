using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNetServer
{
    public class User
    {
        protected TcpClientWrapper socket;
        public byte[] Buffer = new byte[0]; //this cannot be a property

        public User(TcpClientWrapper clientSocket)
        {
            socket = clientSocket;
        }

        public TcpClientWrapper Socket
        {
            get { return socket; }
        }
    }
}
