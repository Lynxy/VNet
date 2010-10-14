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


        public TcpListenerWrapper(int port)
            : base(IPAddress.Parse("0.0.0.0"), port)
        {
        }

        public void Listen(int backlog)
        {
            base.Start(backlog);
            StartListening();
        }

        protected void StartListening()
        {
            base.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), this);
        }

        public void AcceptTcpClientCallback(IAsyncResult ar)
		{
			TcpListenerWrapper listener = (TcpListenerWrapper)ar.AsyncState;
            TcpClientWrapper client = new TcpClientWrapper(listener.EndAcceptTcpClient(ar));
            //listener._invokeObject.Invoke((Action)delegate { listener.OnClientConnected(client); }); //on calling thread
            listener.OnClientConnected(client); //not on calling thread
			listener.StartListening();
		}


    }
}

