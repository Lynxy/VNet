using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace Lynxy.Network
{
    public class StateObject
    {
        public SocketWrapper workSocket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public byte[] largeBuffer = new byte[0];
    }

    public class SocketWrapper :  Socket
    {
        public delegate void ClientConnectedDelegate(SocketWrapper origSock, SocketWrapper newSock);
        public event ClientConnectedDelegate OnClientConnected;
        public delegate void ConnectedDelegate(SocketWrapper sock);
        public event ConnectedDelegate OnConnected;
        public delegate void DisconnectDelegate(SocketWrapper sock);
        public event DisconnectDelegate OnDisconnect;
        public delegate void DataReceivedDelegate(SocketWrapper sock, byte[] data);
        public event DataReceivedDelegate OnDataReceived;
        public delegate void DataSentDelegate(SocketWrapper sock, int bytesSent);
        public event DataSentDelegate OnDataSent;

        public bool wasConnected = false;

        public SocketWrapper(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
            : base(addressFamily, socketType, protocolType)
        {
        }
        public SocketWrapper(Socket sock)
            : base(sock.DuplicateAndClose(System.Diagnostics.Process.GetCurrentProcess().Id))
        {
        }

        public void AsyncListen(int backlog, string address, int port)
        {
            try
            {
                IPHostEntry IPHost = Dns.GetHostEntry(address);
                IPAddress[] addr = IPHost.AddressList;
                EndPoint ep = new IPEndPoint(addr[0], port);
                base.Bind(ep);

                base.Listen(backlog);
                base.BeginAccept(AsyncListenCallback, this);
            }
            catch (Exception ex)
            {
                this.HandleSocketError(ex);
            }
        }

        protected void AsyncListenCallback(IAsyncResult res)
        {
            SocketWrapper origSock = res.AsyncState as SocketWrapper;
            SocketWrapper newSock = new SocketWrapper(origSock.EndAccept(res));

            try
            {
                newSock.wasConnected = true;
                origSock.BeginAccept(AsyncListenCallback, origSock);

                WaitForData(newSock);
                if (OnClientConnected != null)
                    OnClientConnected(origSock, newSock);
            }
            catch (Exception ex)
            {
                newSock.HandleSocketError(ex);
            }
        }

        public void AsyncConnect(string host, int port)
        {
            try
            {
                base.BeginConnect(host, port, AsyncConnectCallback, this);
            }
            catch (Exception ex)
            {
                this.HandleSocketError(ex);
            }
        }

        protected void AsyncConnectCallback(IAsyncResult res)
        {
            SocketWrapper sock = res.AsyncState as SocketWrapper;

            try
            {
                sock.wasConnected = true;
                WaitForData(sock);
                if (OnConnected != null)
                    OnConnected(sock);
            }
            catch (Exception ex)
            {
                sock.HandleSocketError(ex);
            }
        }

        public void WaitForData(SocketWrapper sock)
        {
            try
            {
                StateObject state = new StateObject();
                state.workSocket = sock;
                sock.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, new AsyncCallback(WaitForDataCallBack), state);
            }
            catch (Exception ex)
            {
                sock.HandleSocketError(ex);
            }
        }

        protected void WaitForDataCallBack(IAsyncResult res)
        {
            StateObject state = (StateObject)res.AsyncState;
            SocketWrapper sock = state.workSocket;

            try
            {
                int read = sock.EndReceive(res);
                if (read > 0)
                {
                    byte[] allBuffer = new byte[state.largeBuffer.Length + read];
                    Array.Copy(state.largeBuffer, 0, allBuffer, 0, state.largeBuffer.Length);
                    Array.Copy(state.buffer, 0, allBuffer, state.largeBuffer.Length, read);
                    state.largeBuffer = allBuffer;
                    allBuffer = null;
                    byte[] packet = PacketBuffer.GetNextPacket(ref state.largeBuffer);
                    if (packet != null)
                        if (sock.OnDataReceived != null)
                            sock.OnDataReceived(sock, packet);

                    if (state.largeBuffer.Length > 0)
                        throw new Exception("unhandled socket thingy");
                }
                else
                {
                    throw new Exception("omg wtfh");
                }
                WaitForData(sock);
            }
            catch (Exception ex)
            {
                sock.HandleSocketError(ex);
            }
        }

        public void AsyncSend(byte[] data)
        {
            try
            {
                base.BeginSend(data, 0, data.Length, SocketFlags.None, AsyncSendCallback, this);
            }
            catch (Exception ex)
            {
                this.HandleSocketError(ex);
            }
        }

        protected void AsyncSendCallback(IAsyncResult res)
        {
            SocketWrapper sock = res.AsyncState as SocketWrapper;

            try
            {
                int sent = sock.EndSend(res);
                if (OnDataSent != null)
                    OnDataSent(sock, sent);
            }
            catch (Exception ex)
            {
                sock.HandleSocketError(ex);
            }
        }

        public void HandleSocketError(Exception ex)
        {
            Console.WriteLine("[ERR] Connected=" + (wasConnected ? "Yes" : "No") + " " + ex.Message);
            base.Close();
        }
    }
}
