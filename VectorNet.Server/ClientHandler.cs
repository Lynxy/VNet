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
        protected Dictionary<TcpClientWrapper, User> TcpClientUsers = new Dictionary<TcpClientWrapper, User>();


        /// <summary>
        /// Makes the server start listening on the socket.
        /// </summary>
        public void StartListeningForClients()
        {
            listener = new TcpListenerWrapper(Config.Network.ListenPort);
            listener.OnClientConnected += new TcpListenerWrapper.ClientConnectedDelegate(listener_OnClientConnected);

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

            //checks done, create a User object
            User newUser = new User(client, false);
            TcpClientUsers.Add(client, newUser);
            client.DataRead += new TcpClientWrapper.DataReadDelegate(client_DataRead);
            client.Disconnected += new TcpClientWrapper.DisconnectedDelegate(client_Disconnected);
            client.AsyncRead(Config.Network.ReadBufferSize, true);

            Users.Add(newUser);
        }

        /// <summary>
        /// An event that fires when a socket receives data. Data is stored in a buffer until a complete packet can be extracted.
        /// </summary>
        /// <param name="sender">Socket that send the data</param>
        /// <param name="data">Payload</param>
        protected void client_DataRead(TcpClientWrapper sender, ref byte[] data)
        {
            ServerStats.bytesRecv += data.Length;

            if (!TcpClientUsers.ContainsKey(sender))
                return;
            User user = TcpClientUsers[sender];
            PacketReader packet;
            user.RecvBufferer.AppendData(ref data);
            while ((packet = user.RecvBufferer.GetNextPacket()) != null)
                HandlePacket(user, packet);
        }

        /// <summary>
        /// An event that fires when a socket has disconnected. DO NOT CALL Shutdown()
        /// </summary>
        /// <param name="sender">The socket the disconnected</param>
        protected void client_Disconnected(TcpClientWrapper sender)
        { //does this need thread locking?
            if (!TcpClientUsers.ContainsKey(sender))
                return;
            User user = TcpClientUsers[sender];
            TcpClientUsers.Remove(sender);
            DisconnectUser(user);
            Users.Remove(user);
            user.Dispose();
            user = null;
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
