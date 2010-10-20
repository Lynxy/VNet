using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Timers;

namespace Lynxy.Network
{
    public class PacketBufferer
    {
        public delegate void SendDataDelegate(ref byte[] data);
        protected SendDataDelegate _SendData;

        protected byte[] buffer = new byte[0];
        protected Timer ticker;

        public PacketBufferer(SendDataDelegate SendData)
            : this(SendData, 100)
        {
        }
        public PacketBufferer(SendDataDelegate SendData, double interval)
        {
            _SendData = SendData;
            ticker = new Timer(interval);
            ticker.Elapsed += new ElapsedEventHandler(ticker_Elapsed);
        }

        protected void ticker_Elapsed(object sender, ElapsedEventArgs e)
        {
            ticker.Stop();
            _SendData(ref buffer);
            Array.Resize(ref buffer, 0);
        }

        public void QueuePacket(ref byte[] packet)
        {
            int origLen = buffer.Length;
            Array.Resize(ref buffer, origLen + packet.Length);
            Array.Copy(packet, 0, buffer, origLen, packet.Length);
            ticker.Start();
        }
    }
}
