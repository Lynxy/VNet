using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    [Flags]
    public enum UserFlags
    {
        Normal =    0x00,
        Admin =     0x01,
        Operator =  0x02,
        Moderator = 0x04,
        Ignored =   0x08,
        Muted =     0x10,
        Invisible = 0x20
    }


    public class User
    {
        protected TcpClientWrapper socket;
        protected Packet packet;

        public User(TcpClientWrapper clientSocket)
        {
            socket = clientSocket;
            packet = new Packet(clientSocket);
            Flags = UserFlags.Normal;
        }

        public TcpClientWrapper Socket
        {
            get { return socket; }
        }

        public Packet Packet
        {
            get { return packet; }
        }

        public UserFlags Flags { get; set; }
    }
}
