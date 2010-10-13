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
        protected Packet packet;

        public User(TcpClientWrapper clientSocket)
        {
            socket = clientSocket;
            packet = new Packet(clientSocket);
        }

        public TcpClientWrapper Socket
        {
            get { return socket; }
        }

        public Packet Packet
        {
            get { return packet; }
        }
    }
}
