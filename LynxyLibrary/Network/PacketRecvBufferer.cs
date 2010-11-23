using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

namespace Lynxy.Network
{
    public class PacketRecvBufferer
    {
        protected byte[] _buffer;
        private readonly object _locker = new object();

        public PacketRecvBufferer()
        {
            _buffer = new byte[0];
        }

        public void AppendData(ref byte[] data)
        {
            lock (_locker)
            {
                int oldLen = _buffer.Length;
                Array.Resize(ref _buffer, oldLen + data.Length);
                Array.Copy(data, 0, _buffer, oldLen, data.Length);
            }
        }

        public PacketReader GetNextPacket()
        {
            lock (_locker)
            {
                byte[] completePacket = PacketBuffer.GetNextPacket(ref _buffer);
                if (completePacket == null) return null;
                return new PacketReader(completePacket);
            }
        }

    }
}
