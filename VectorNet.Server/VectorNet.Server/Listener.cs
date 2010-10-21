using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected TcpListenerWrapper listener;
        protected ClientHandler clients;

        public void StartListening()
        {
            listener = new TcpListenerWrapper(Config.ListenPort);
            listener.OnClientConnected += new TcpListenerWrapper.ClientConnectedDelegate(listener_OnClientConnected);

            clients = new ClientHandler();
            clients.UserPacketReceived += new ClientHandler.UserPacketReceivedDelegate(HandlePacket);
            clients.UserDisconnected += new ClientHandler.UserDisconnectedDelegate(clients_UserDisconnected);

            listener.Listen(10);
        }

        protected void listener_OnClientConnected(TcpClientWrapper client)
        {
            Users.Add(clients.AddNewClient(client));
        }

        protected void clients_UserDisconnected(User user)
        {
            DisconnectUser(user);
            Users.Remove(user);
        }
    }
}
