using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

namespace Lynxy.Network
{
    public class PacketBufferer
    {
        public delegate void SendDataDelegate(object state, ref byte[] data);
        protected SendDataDelegate _SendData;

        protected byte[] buffer = new byte[0];
        protected object _state;
        protected Timer ticker;
        private readonly object _locker = new object();

        public PacketBufferer(SendDataDelegate SendData, object state)
            : this(SendData, state, 100)
        {
        }
        public PacketBufferer(SendDataDelegate SendData, object state, int interval)
        {
            _SendData = SendData;
            _state = state;
            ticker = new Timer(ticker_Elapsed, null, interval, interval);
        }

        protected void ticker_Elapsed(object obj)
        {
            SendNow();
        }

        public void QueuePacket(ref byte[] packet)
        {
            //lock (_locker)
            //{
                int origLen = buffer.Length;
                Array.Resize(ref buffer, origLen + packet.Length);
                Array.Copy(packet, 0, buffer, origLen, packet.Length);
            //}
        }

        public void SendNow()
        {
            //lock (_locker)
            //{
                if (buffer.Length > 0)
                {
                    _SendData(_state, ref buffer);
                    Array.Resize(ref buffer, 0);
                }
            //}
        }
    }

    //static public class PacketBufferer
    //{
    //    public delegate void SendDataDelegate(object state, byte[] data);
    //    static private SendDataDelegate _SendData;

    //    static private Dictionary<object, byte[]> buffers;
    //    static private Timer ticker;
    //    static private readonly object _locker = new object();

    //    static public void Init(int sendInterval, SendDataDelegate SendData)
    //    {
    //        buffers = new Dictionary<object, byte[]>();
    //        ticker = new Timer(ticker_Elapsed, null, sendInterval, sendInterval);
    //        _SendData = SendData;
    //    }

    //    static private void ticker_Elapsed(object obj)
    //    {
    //        lock (buffers)
    //        {
    //            foreach (object state in buffers.Keys)
    //            {
    //                if (buffers[state].Length > 0)
    //                    _SendData(state, buffers[state]);
    //            }
    //            buffers.Clear();
    //        }
    //    }

    //    static public void QueuePacket(object state, ref byte[] packet)
    //    {
    //        lock (buffers)
    //        {
    //            if (buffers.ContainsKey(state))
    //            {
    //                byte[] buffer = buffers[state];
    //                int origLen = buffer.Length;
    //                Array.Resize(ref buffer, origLen + packet.Length);
    //                Array.Copy(packet, 0, buffer, origLen, packet.Length);
    //                buffers[state] = buffer;
    //            }
    //            else
    //            {
    //                buffers.Add(state, packet);
    //            }
    //        }
    //    }
    //}

}
