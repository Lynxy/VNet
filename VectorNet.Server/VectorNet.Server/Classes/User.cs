using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;
using System.Net;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected class User : IDisposable
        {
            public event Action<byte[]> SendData;

            private bool disposed = false;
            protected TcpClientWrapper socket;
            protected Packet packet;
            protected PacketBufferer bufferer;
            protected bool _isConsole = false;
            protected bool _canSendData = true;

            public User(TcpClientWrapper clientSocket, bool isConsole)
            {
                socket = clientSocket;
                _isConsole = isConsole;

                packet = new Packet();
                packet.skipHeaders = isConsole;
                packet.DataSent += new Packet.SendDataDelegate(packet_SendData);
                bufferer = new PacketBufferer(SendDataFinal, null, 200);

                Flags = UserFlags.Normal;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (!this.disposed)
                {
                    if (disposing)
                    {
                        socket = null;
                        packet = null;
                        bufferer = null;
                    }
                    disposed = true;
                }
            }

            ~User()
            {
                Dispose(false);
            }

            protected void packet_SendData(Packet packet, ref byte[] data)
            {
                if (!_canSendData)
                    return;
                if (_isConsole)
                {
                    if (SendData != null)
                        SendData(data);
                }
                else
                    bufferer.QueuePacket(ref data);
            }

            protected void SendDataFinal(object state, ref byte[] data)
            {
                if (!_canSendData)
                    return;
                ServerStats.bytesSent += data.Length;
                socket.AsyncSend(data, data.Length);
            }

            public TcpClientWrapper Socket { get { return socket; } }
            public Packet Packet { get { return packet; } }
            public string IPAddress { get { return (socket == null ? null : ((IPEndPoint)socket.Client.RemoteEndPoint).Address.ToString()); } }
            public string Username { get; set; }
            public string Client { get; set; }
            public UserFlags Flags { get; set; }
            public Channel Channel { get; set; }
            public int Ping { get; set; }
            public bool IsOnline { get; set; }
            public bool CanSendData { get { return _canSendData; } set { _canSendData = value; } }
        }
    }
}
