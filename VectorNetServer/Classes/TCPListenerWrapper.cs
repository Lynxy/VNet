using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;


namespace Lynxy.Network
{
    public class TcpListenerWrapper : TcpListener
    {

        public delegate void ClientConnectedDelegate(TcpClient newSock);
        public event ClientConnectedDelegate OnClientConnected;


        protected Control _invokeObject;
        public TcpListenerWrapper(Control invokeObject, int port)
            : base(IPAddress.Parse("127.0.0.1"), port)
        {
            _invokeObject = invokeObject;
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
			TcpClient client = listener.EndAcceptTcpClient(ar);
            listener._invokeObject.Invoke((Action)delegate { listener.OnClientConnected(client); });
			listener.StartListening();
		}


    }
}

