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
        protected ClientHandler clients = new ClientHandler();

        public void StartListening()
        {
            listener = new TcpListenerWrapper(Config.ListenPort);
            listener.OnClientConnected += new TcpListenerWrapper.ClientConnectedDelegate(listener_OnClientConnected);
            clients.UserPacketReceived += new ClientHandler.UserPacketReceivedDelegate(HandlePacket);
        }

        protected void listener_OnClientConnected(TcpClientWrapper client)
        {
            clients.AddNewClient(client);
        }
    }
}
