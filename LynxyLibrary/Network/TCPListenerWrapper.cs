using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace Lynxy.Network
{
    public class TcpListenerWrapper : TcpListener
    {

        public delegate void ClientConnectedDelegate(TcpClientWrapper newSock);
        public event ClientConnectedDelegate OnClientConnected;

        protected int _clientSendBacklog = 30;

        public TcpListenerWrapper(int port)
            : this("0.0.0.0", port)
        {
        }
        public TcpListenerWrapper(string bindIP, int port)
            : base(IPAddress.Parse(bindIP), port)
        {
        }

        public void Listen(int listenerBacklog, int clientSendBacklog)
        {
            base.Start(listenerBacklog);
            _clientSendBacklog = clientSendBacklog;
            StartListening();
        }

        protected void StartListening()
        {
            base.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), this);
        }

        public void AcceptTcpClientCallback(IAsyncResult ar)
		{
            TcpListenerWrapper listener = (TcpListenerWrapper)ar.AsyncState;
            TcpClientWrapper client = new TcpClientWrapper(_clientSendBacklog, listener.EndAcceptTcpClient(ar));
            
            System.Threading.ThreadPool.QueueUserWorkItem(delegate
            {
                listener.OnClientConnected(client);
            });

            listener.StartListening();
		}


    }
}

