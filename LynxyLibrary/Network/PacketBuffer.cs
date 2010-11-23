using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lynxy.Network
{
    public class PacketBuffer
    {
        protected byte[] _buffer;
        private readonly object _lockerInstance = new object();
        static private readonly object _lockerStatic = new object();

        public PacketBuffer()
        {
            _buffer = new byte[0];
        }

        public void AppendData(ref byte[] data)
        {
            lock (_lockerInstance)
            {
                int oldLen = _buffer.Length;
                Array.Resize(ref _buffer, oldLen + data.Length);
                Array.Copy(data, 0, _buffer, oldLen, data.Length);
            }
        }

        public PacketReader GetNextPacket()
        {
            lock (_lockerInstance)
            {
                byte[] completePacket = PacketBuffer.GetNextPacket(ref _buffer);
                if (completePacket == null) return null;
                return new PacketReader(completePacket);
            }
        }

        static public byte[] GetNextPacket(ref byte[] packet)
        {
            lock (_lockerStatic)
            {
                byte[] ret = Packet.Parse(packet);
                if (ret == null)
                    return null;
                Array.Copy(packet, ret.Length + 4, packet, 0, packet.Length - ret.Length - 4);
                Array.Resize(ref packet, packet.Length - ret.Length - 4);
                return ret;
            }
        }
    }
}
