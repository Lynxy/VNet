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
            private bool disposed = false;
            protected TcpClientWrapper socket;
            protected Packet packet;
            protected PacketBuffer recvBufferer;
            protected UserFlags _Flags;
            protected bool _isConsole = false;
            protected bool _canSendData = true;

            public User(TcpClientWrapper clientSocket, bool isConsole)
            {
                socket = clientSocket;
                _isConsole = isConsole;
                packet = new Packet();
                packet.skipHeaders = isConsole;
                packet.DataSent += new Packet.SendDataDelegate(packet_SendData);
                PacketSendBufferer.AddSendHandler(this, SendDataFinal);
                recvBufferer = new PacketBuffer();

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
                        socket.Client.Dispose();
                        socket = null;
                        packet = null;
                        PacketSendBufferer.RemoveSendHandler(this);
                        recvBufferer = null;
                        Channel = null;
                    }
                    disposed = true;
                }
            }

            ~User()
            {
                Dispose(false);
            }

            /// <summary>
            /// An event that fires when a packet is requested to be sent to a user. The packet is queued until next send.
            /// </summary>
            /// <param name="packet">The packet that raised the event</param>
            /// <param name="data">Payload</param>
            protected void packet_SendData(Packet packet, ref byte[] data)
            {
                if (!_canSendData)
                    return;
                if (!_isConsole)
                    PacketSendBufferer.QueuePacket(this, ref data);
            }

            /// <summary>
            /// Forcibly sends the users packet buffer.
            /// </summary>
            public void SendBufferNow()
            {
                if (!_canSendData)
                    return;

                PacketSendBufferer.SendNow(this);
            }

            /// <summary>
            /// An event that fires every so often to send the packet buffer to the user.
            /// </summary>
            /// <param name="state">State object</param>
            /// <param name="data">Payload</param>
            protected void SendDataFinal(object state, byte[] data)
            {
                if (!_canSendData)
                    return;
                ServerStats.bytesSent += data.Length;
                socket.AsyncSend(data, data.Length);
            }

            public PacketBuffer RecvBufferer { get { return recvBufferer; } }
            public TcpClientWrapper Socket { get { return socket; } }
            public Packet Packet { get { return packet; } }
            public string IPAddress { get { return (socket == null ? null : ((IPEndPoint)socket.Client.RemoteEndPoint).Address.ToString()); } }
            
            /// <summary>
            /// The username of the user. Factors in their account nummber. Is read-only - use RealUsername to set the name.
            /// </summary>
            public string Username
            {
                get
                {
                    if (AccountNumber > 1)
                        return RealUsername + "#" + AccountNumber;
                    return RealUsername;
                }
            }
            public string RealUsername { get; set; }
            public int AccountNumber { get; set; }
            public string Client { get; set; }
            public UserFlags Flags
            {
                get { return _Flags; }
                set
                {
                    _Flags = value;
                    if ((_Flags & UserFlags.Admin) == UserFlags.Admin)
                    {
                        if ((_Flags & UserFlags.Moderator) == UserFlags.Moderator)
                            _Flags ^= UserFlags.Moderator;
                        if ((_Flags & UserFlags.Operator) == UserFlags.Operator)
                            _Flags ^= UserFlags.Operator;
                    }
                    else if ((_Flags & UserFlags.Moderator) == UserFlags.Moderator)
                    {
                        if ((_Flags & UserFlags.Operator) == UserFlags.Operator)
                            _Flags ^= UserFlags.Operator;
                    }
                        
                }
            }
            public Channel Channel { get; set; }

            // Strictly for queue sharing only
            public string BattleNetChannel { get; set; }

            public int Ping { get; set; }
            public bool IsOnline { get; set; }
            public bool CanSendData { get { return _canSendData; } set { _canSendData = value; } }
        }
    }
}
