using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        [Flags]
        protected enum UserFlags
        {
            Normal = 0x00,
            Admin = 0x01,
            Operator = 0x02,
            Moderator = 0x04,
            Ignored = 0x08,
            Muted = 0x10,
            Invisible = 0x20
        }

        protected class User
        {
            public event Action<byte[]> SendData;

            protected TcpClientWrapper socket;
            protected Packet packet;
            protected bool _isConsole = false;

            public User(TcpClientWrapper clientSocket, bool isConsole)
            {
                socket = clientSocket;
                _isConsole = isConsole;

                packet = new Packet();
                packet.skipHeaders = isConsole;
                packet.DataSent += new Packet.SendDataDelegate(packet_SendData);

                Flags = UserFlags.Normal;
            }

            protected void packet_SendData(byte[] data)
            {
                if (_isConsole)
                {
                    if (SendData != null)
                        SendData(data);
                }
                else
                    socket.AsyncSend(data, data.Length);
            }

            public TcpClientWrapper Socket
            {
                get { return socket; }
            }

            public Packet Packet
            {
                get { return packet; }
            }

            public string Username { get; set; }
            public string Client { get; set; }
            public UserFlags Flags { get; set; }
            public Channel Channel { get; set; }
            public int Ping { get; set; }
            public bool IsConsole { get; set; }
            public bool IsOnline { get; set; }
        }
    }
}
