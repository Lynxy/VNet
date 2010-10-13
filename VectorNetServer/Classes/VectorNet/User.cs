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
        protected PacketWrapper packet;

        public byte[] Buffer = new byte[0]; //this cannot be a property

        public User(TcpClientWrapper clientSocket)
        {
            socket = clientSocket;
            packet = new PacketWrapper(clientSocket);
        }

        public TcpClientWrapper Socket
        {
            get { return socket; }
        }

        public PacketWrapper Packet
        {
            get { return packet; }
        }
    }
}
