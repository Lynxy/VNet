﻿using System;
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

        /// <summary>
        /// Makes the server start listening on the socket.
        /// </summary>
        public void StartListening()
        {
            listener = new TcpListenerWrapper(Config.Network.ListenPort);
            listener.OnClientConnected += new TcpListenerWrapper.ClientConnectedDelegate(listener_OnClientConnected);

            clients = new ClientHandler();
            clients.UserPacketReceived += new ClientHandler.UserPacketReceivedDelegate(HandlePacket);
            clients.UserDisconnected += new ClientHandler.UserDisconnectedDelegate(clients_UserDisconnected);

            listener.Listen(Config.Network.MaxConnectionsBacklog, Config.Network.MaxClientSendBacklog);
        }

        /// <summary>
        /// An event that fires when a socket connects.
        /// </summary>
        /// <param name="client"></param>
        protected void listener_OnClientConnected(TcpClientWrapper client)
        {
            ServerStats.totalConnections++;

            string IP = EndpointToIP(client.Client.RemoteEndPoint);
            if (CheckFloodingConnection(IP))
            {
                client.Client.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                return;
            }
            IncrementFloodingConnection(IP);

            Users.Add(clients.AddNewClient(client));
        }

        /// <summary>
        /// An event that fires when a user disconnects.
        /// </summary>
        /// <param name="user"></param>
        protected void clients_UserDisconnected(User user)
        {
            DisconnectUser(user);
            Users.Remove(user);
        }

        /// <summary>
        /// Converts an EndPoint to a string (IP address, no port)
        /// </summary>
        /// <param name="point">The EndPoint to convert</param>
        protected string EndpointToIP(System.Net.EndPoint point)
        {
            return ((System.Net.IPEndPoint)point).Address.ToString();
        }
    }
}
