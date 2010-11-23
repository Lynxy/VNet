using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

namespace Lynxy.Network
{
    static public class PacketSendBufferer
    {
        public delegate void SendDataDelegate(object state, byte[] data);

        static private Dictionary<object, byte[]> buffers;
        static private Dictionary<object, SendDataDelegate> SendHandlers;
        static private Timer ticker;
        static private readonly object _locker = new object();

        static public void Init(int sendInterval)
        {
            buffers = new Dictionary<object, byte[]>();
            SendHandlers = new Dictionary<object, SendDataDelegate>();
            ticker = new Timer(ticker_Elapsed, null, sendInterval, sendInterval);
        }

        static public void AddSendHandler(object state, SendDataDelegate SendData)
        {
            if (SendHandlers.ContainsKey(state))
                SendHandlers[state] = SendData;
            else
                SendHandlers.Add(state, SendData);
        }

        static public void RemoveSendHandler(object state)
        {
            if (SendHandlers.ContainsKey(state))
                SendHandlers.Remove(state);
            if (buffers.ContainsKey(state))
                buffers.Remove(state);
        }

        static private void ticker_Elapsed(object obj)
        {
            lock (_locker)
            {
                foreach (object state in buffers.Keys)
                {
                    if (buffers[state].Length > 0)
                        SendHandlers[state](state, buffers[state]);
                }
                buffers.Clear();
            }
        }

        static public void QueuePacket(object state, ref byte[] packet)
        {
            if (SendHandlers.ContainsKey(state) == false)
                return;
            lock (_locker)
            {
                if (buffers.ContainsKey(state))
                {
                    byte[] buffer = buffers[state];
                    int origLen = buffer.Length;
                    Array.Resize(ref buffer, origLen + packet.Length);
                    Array.Copy(packet, 0, buffer, origLen, packet.Length);
                    buffers[state] = buffer;
                }
                else
                {
                    buffers.Add(state, packet);
                }
            }
        }

        static public void SendNow(object state)
        {
            lock (_locker)
            {
                if (buffers.ContainsKey(state))
                {
                    if (buffers[state].Length > 0)
                        SendHandlers[state](state, buffers[state]);
                    buffers.Remove(state);
                }
            }
        }
    }





    //public class PacketSendBufferer
    //{
    //    public delegate void SendDataDelegate(object state, ref byte[] data);
    //    protected SendDataDelegate _SendData;

    //    protected byte[] buffer = new byte[0];
    //    protected object _state;
    //    protected Timer ticker;
    //    private readonly object _locker = new object();

    //    public PacketSendBufferer(SendDataDelegate SendData, object state)
    //        : this(SendData, state, 100)
    //    {
    //    }
    //    public PacketSendBufferer(SendDataDelegate SendData, object state, int interval)
    //    {
    //        _SendData = SendData;
    //        _state = state;
    //        ticker = new Timer(ticker_Elapsed, null, interval, interval);
    //    }

    //    protected void ticker_Elapsed(object obj)
    //    {
    //        SendNow();
    //    }

    //    public void QueuePacket(ref byte[] packet)
    //    {
    //        lock (_locker)
    //        {
    //            int origLen = buffer.Length;
    //            Array.Resize(ref buffer, origLen + packet.Length);
    //            Array.Copy(packet, 0, buffer, origLen, packet.Length);
    //        }
    //    }

    //    public void SendNow()
    //    {
    //        lock (_locker)
    //        {
    //            if (buffer.Length > 0)
    //            {
    //                _SendData(_state, ref buffer);
    //                Array.Resize(ref buffer, 0);
    //            }
    //        }
    //    }
    //}



}
