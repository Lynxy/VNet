using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;

namespace Lynxy.Network
{
    /// <summary>
    /// An easy-to-use socket class.
    /// </summary>
    /// <remarks>easySocket is the underlying socket architecture behind all easySockets.
    /// easySocket is an inheritance class of TcpClient, so all core functions remain.
    /// However, new functions are included for fast and easy use of Sync and Async reads and writes.
    /// Use easySocket to build your own socket protocols.</remarks>
    public class TcpClientWrapper : TcpClient
    {
        public TcpClientWrapper()
        {
        }
        public TcpClientWrapper(TcpClient duplicateSocket)
        {
            this.Client = new Socket(duplicateSocket.Client.DuplicateAndClose(System.Diagnostics.Process.GetCurrentProcess().Id));
        }

        #region "Fields"

        private string myAddr;

        private int myPort;
        // Buffer for our Async Reads.
        private byte[] asReadBuffer;
        // Do we do Async Reads continuously forever?  Assume true.
        private bool asContReads;
        // How big are our Async Reads?  Assume 1024 bytes.
        private int asReadSize;

        #endregion

        #region "Properties"

        public string Address
        {
            get { return myAddr; }
            set { myAddr = value; }
        }

        public int Port
        {
            get { return myPort; }
            set { myPort = value; }
        }

        #endregion

        #region "Events" ' Events

        /// <summary>
        /// The socket was disconnected.
        /// </summary>
        /// <remarks>Raised when a disconnect has been detected.</remarks>
        public delegate void DisconnectedDelegate(TcpClientWrapper sender);
        public event DisconnectedDelegate Disconnected;

        /// <summary>
        /// Data has been read asynchronously and is available.
        /// </summary>
        /// <remarks>Raised when an Asynchronous read has completed and data is available.</remarks>
        public delegate void DataReadDelegate(TcpClientWrapper sender, byte[] data);
        public event DataReadDelegate DataRead;

        /// <summary>
        /// Data has been sent asynchronously.
        /// </summary>
        /// <remarks>Raised when an Asynchronous send has completed.</remarks>
        public delegate void DataSentDelegate(TcpClientWrapper sender);
        public event DataSentDelegate DataSent;

        /// <summary>
        /// A connected has been established asynchronously.
        /// </summary>
        /// <remarks>Raised when an Asynchronous connect has completed.</remarks>
        public delegate void ConnectionEstablishedDelegate(TcpClientWrapper sender);
        public event ConnectionEstablishedDelegate ConnectionEstablished;

        /// <summary>
        /// A connected has been refused asynchronously.
        /// </summary>
        /// <remarks>Raised when an Asynchronous connect has been refused.</remarks>
        public delegate void ConnectionRefusedDelegate(TcpClientWrapper sender);
        public event ConnectionRefusedDelegate ConnectionRefused;

        #endregion

        #region "Methods"

        #region "Event Raisers"

        protected virtual void OnDisconnected()
        {
            if (Connected)
                Close();
            if (Disconnected != null)
            {
                Disconnected(this);
            }
        }

        protected virtual void OnDataRead(byte[] e)
        {
            if (DataRead != null)
            {
                DataRead(this, e);
            }
        }

        protected virtual void OnDataSent()
        {
            if (DataSent != null)
            {
                DataSent(this);
            }
        }

        protected virtual void OnConnectionEstablished()
        {
            if (ConnectionEstablished != null)
            {
                ConnectionEstablished(this);
            }
        }

        protected virtual void OnConnectionRefused()
        {
            if (ConnectionRefused != null)
            {
                ConnectionRefused(this);
            }
        }

        #endregion

        #region "Connection Methods"

        /// <summary>
        /// Connect using a sycnrhonous method.
        /// </summary>
        /// <remarks><c>Address</c> and <c>Port</c> must have already been set.</remarks>
        public void Connect()
        {
            base.Connect(myAddr, myPort);
        }

        /// <summary>
        /// Connect using a synchronous method.
        /// </summary>
        /// <param name="addr">The hostname or IP to connect to.</param>
        /// <param name="port">The port number to connect to.</param>
        public new void Connect(string addr, int port)
        {
            myAddr = addr;
            myPort = port;
            this.Connect();
        }

        /// <summary>
        /// Connect using an asynchronous method.
        /// </summary>
        /// <param name="addr">The hostname or IP to connect to.</param>
        /// <param name="port">The port number to connect to.</param>
        /// <remarks></remarks>
        public void AsyncConnect(string addr, int port)
        {
            myAddr = addr;
            myPort = port;
            this.AsyncConnect();
        }

        /// <summary>
        /// Connect using an asynchronous method.
        /// </summary>
        /// <remarks><c>Address</c> and <c>Port</c> must have already been set.</remarks>
        public void AsyncConnect()
        {
            BeginConnect(myAddr, myPort, new AsyncCallback(EndASConnect), null);
        }

        private void EndASConnect(IAsyncResult ar)
        {
            try
            {
                if (!Connected)
                {
                    OnConnectionRefused();
                    return;
                }

                EndConnect(ar);
                OnConnectionEstablished();
            }
            catch (Exception)
            {
                OnDisconnected();
            }
        }
        #endregion

        #region "Sending Methods"

        /// <summary>
        /// Send data using a synchronous operation.
        /// </summary>
        /// <param name="Data">The data to be sent, as a string.</param>
        public void Send(string data)
        {
            try
            {
                if (!Connected)
                {
                    OnDisconnected();
                    return;
                }

                System.IO.StreamWriter w = new System.IO.StreamWriter(GetStream());
                w.Write(data);
                w.Flush();
            }
            catch (Exception)
            {
                OnDisconnected();
            }
        }

        /// <summary>
        /// Send data using a synchronous operation.
        /// </summary>
        /// <param name="Data">The data to be sent, as a byte array.</param>
        /// <param name="size">The number of bytes in data to be sent.</param>
        public void Send(byte[] data, int size)
        {
            try
            {
                if (!Connected)
                {
                    OnDisconnected();
                    return;
                }

                GetStream().Write(data, 0, size);
            }
            catch (Exception)
            {
                OnDisconnected();
            }
        }

        /// <summary>
        /// Send data using an asynchronous operation.
        /// </summary>
        /// <param name="data">The data to be a sent, as a byte array.</param>
        /// <param name="size">The number of bytes in data to be sent.</param>
        /// <remarks></remarks>
        public void AsyncSend(byte[] data, int size)
        {
            try
            {
                if (!Connected)
                {
                    OnDisconnected();
                    return;
                }

                GetStream().BeginWrite(data, 0, size, new AsyncCallback(EndASSend), null);
            }
            catch (Exception)
            {
                OnDisconnected();
            }
        }

        //TODO: FIX ME
        private void EndASSend(IAsyncResult ar)
        {
            try
            {
                if (!Connected)
                {
                    OnDisconnected();
                    return;
                }

                GetStream().EndWrite(ar);
                OnDataSent();
            }
            catch (Exception)
            {
                OnDisconnected();
            }
        }

        #endregion

        #region "Reading Methods"

        /// <summary>
        /// Read data using a synchronous operation.
        /// </summary>
        /// <param name="size">The maximum size, in bytes, to be retrieved.  Default 1024.</param>
        /// <remarks>If the socket is disconnected, Read returns Nothing, and the
        /// Disconnected event will fire.</remarks>
        public byte[] Read(int size = 1024)
        {
            try
            {
                if (!Connected)
                {
                    OnDisconnected();
                    return null;
                }

                byte[] ReadBuf = { 0 };
                int Count = 0;

                Array.Resize(ref ReadBuf, size);
                Count = GetStream().Read(ReadBuf, 0, size);

                if (Count <= 0)
                {
                    OnDisconnected();
                    return null;
                }

                byte[] ret = new byte[Count];
                Array.Copy(ReadBuf, 0, ret, 0, Count);
                return ret;
            }
            catch (Exception)
            {
                OnDisconnected();
                return new byte[0];
            }
        }

        /// <summary>
        /// Read data using an asynchronous operation.
        /// </summary>
        /// <param name="size">The maximum size, in bytes, to be retrieved.  Default 1024.</param>
        /// <param name="continuous">Whether Asynchronous Reads should continue on forever.  Default true.</param>
        /// <remarks>When data comes in, the DataRead event will fire. If the socket is
        /// disconnected, the Disconnected event will fire.</remarks>
        public void AsyncRead(int size = 1024, bool continuous = true)
        {
            asContReads = continuous;
            asReadSize = size;
            StartASRead();
        }

        private void StartASRead()
        {
            try
            {
                if (!Connected)
                {
                    OnDisconnected();
                    return;
                }

                asReadBuffer = new byte[0];
                Array.Resize(ref asReadBuffer, asReadSize);
                GetStream().BeginRead(asReadBuffer, 0, asReadSize, new AsyncCallback(EndASRead), null);
            }
            catch (Exception)
            {
                OnDisconnected();
            }
        }

        private void EndASRead(IAsyncResult ar)
        {
            try
            {
                if (!Connected)
                {
                    OnDisconnected();
                    return;
                }

                int intCount = 0;
                intCount = GetStream().EndRead(ar);
                if (intCount < 1)
                {
                    // The only reason to have 0 bytes in an EndRead is disconnected.
                    // We were disconnected, stop async reads and raise Disconnected event.
                    OnDisconnected();
                    return;
                }

                //Let's put our stuff into a read result, and pass it back in the DataRead event.
                byte[] result = new byte[intCount];
                Array.Copy(asReadBuffer, 0, result, 0, intCount);
                OnDataRead(result);

                // Continuously Reading Asynchronously.
                if (asContReads)
                {
                    StartASRead();
                }
            }
            catch (Exception)
            {
                OnDisconnected();
            }
        }

        #endregion

        #endregion



    }

}

