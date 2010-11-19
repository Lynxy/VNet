using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected class ClientHandler
        {
            public delegate void UserPacketReceivedDelegate(User user, PacketReader reader);
            public event UserPacketReceivedDelegate UserPacketReceived;
            public delegate void UserDisconnectedDelegate(User user);
            public event UserDisconnectedDelegate UserDisconnected;

            public Dictionary<TcpClientWrapper, User> TcpClientUsers = new Dictionary<TcpClientWrapper, User>();
            public Dictionary<User, byte[]> UserBuffers = new Dictionary<User, byte[]>();

            private readonly object _locker = new object();

            /// <summary>
            /// Creates a new user bound to a socket.
            /// </summary>
            /// <param name="client">The socket the user is on</param>
            public User AddNewClient(TcpClientWrapper client)
            {
                User newUser = new User(client, false);
                TcpClientUsers.Add(client, newUser);
                UserBuffers.Add(newUser, new byte[0]);
                client.DataRead += new TcpClientWrapper.DataReadDelegate(client_DataRead);
                client.Disconnected += new TcpClientWrapper.DisconnectedDelegate(client_Disconnected);
                client.AsyncRead(Config.Network.ReadBufferSize, true);
                return newUser;
            }

            /// <summary>
            /// An event that fires when a socket receives data.
            /// </summary>
            /// <param name="sender">Socket that send the data</param>
            /// <param name="data">Payload</param>
            protected void client_DataRead(TcpClientWrapper sender, byte[] data)
            {
                ServerStats.bytesRecv += data.Length;
                lock (_locker)
                {
                    if (!TcpClientUsers.ContainsKey(sender))
                        return;
                    User user = TcpClientUsers[sender];
                    byte[] buffer = UserBuffers[user];
                    int oldLen = buffer.Length;
                    Array.Resize(ref buffer, oldLen + data.Length);
                    Array.Copy(data, 0, buffer, oldLen, data.Length);

                    //check to see if whole packet is available
                    byte[] completePacket;
                    while ((completePacket = PacketBuffer.GetNextPacket(ref buffer)) != null)
                    {
                        UserPacketReceived(user, new PacketReader(completePacket));
                    }
                    UserBuffers[user] = buffer;
                }
            }

            /// <summary>
            /// An event that fires when a socket has disconnected.
            /// </summary>
            /// <param name="sender">The socket the disconnected</param>
            protected void client_Disconnected(TcpClientWrapper sender)
            {
                lock (_locker)
                {
                    if (!TcpClientUsers.ContainsKey(sender))
                        return;
                    User user = TcpClientUsers[sender];
                    if (UserDisconnected != null)
                        UserDisconnected(user);
                    TcpClientUsers.Remove(sender);
                    UserBuffers.Remove(user);
                    user.Dispose();
                    user = null;
                }
            }
        }
    }
}
