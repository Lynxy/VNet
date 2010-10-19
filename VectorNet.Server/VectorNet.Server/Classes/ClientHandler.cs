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

            public Dictionary<TcpClientWrapper, User> TcpClientUsers = new Dictionary<TcpClientWrapper, User>();
            public Dictionary<User, byte[]> UserBuffers = new Dictionary<User, byte[]>();

            protected Server _server;

            public ClientHandler(Server server)
            {
                _server = server;
            }

            public User AddNewClient(TcpClientWrapper client)
            {
                User newUser = new User(client, false);
                TcpClientUsers.Add(client, newUser);
                UserBuffers.Add(newUser, new byte[0]);
                client.DataRead += new TcpClientWrapper.DataReadDelegate(client_DataRead);
                client.Disconnected += new TcpClientWrapper.DisconnectedDelegate(client_Disconnected);
                client.AsyncRead(1024, true);
                return newUser;
            }

            protected void client_DataRead(TcpClientWrapper sender, byte[] data)
            {
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
            }

            protected void client_Disconnected(TcpClientWrapper sender)
            {
                if (!TcpClientUsers.ContainsKey(sender))
                    return;
                User user = TcpClientUsers[sender];
                user.IsOnline = false;
                _server.DisconnectUser(user);
                TcpClientUsers.Remove(sender);
                sender.Client.Dispose();
            }
        }
    }
}
