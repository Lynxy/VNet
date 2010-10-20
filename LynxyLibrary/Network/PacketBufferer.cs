using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

namespace Lynxy.Network
{
    public class PacketBufferer
    {
        public delegate void SendDataDelegate(ref byte[] data);
        protected SendDataDelegate _SendData;

        protected byte[] buffer = new byte[0];
        protected Timer ticker;
        private readonly object _locker = new object();

        public PacketBufferer(SendDataDelegate SendData)
            : this(SendData, 100)
        {
        }
        public PacketBufferer(SendDataDelegate SendData, int interval)
        {
            _SendData = SendData;
            ticker = new Timer(ticker_Elapsed, null, interval, interval);
        }

        protected void ticker_Elapsed(object obj)
        {
            lock (_locker)
            {
                if (buffer.Length > 0)
                {
                    _SendData(ref buffer);
                    Array.Resize(ref buffer, 0);
                }
            }
        }

        public void QueuePacket(ref byte[] packet)
        {
            lock (_locker)
            {
                int origLen = buffer.Length;
                Array.Resize(ref buffer, origLen + packet.Length);
                Array.Copy(packet, 0, buffer, origLen, packet.Length);
            }
        }
    }
}
