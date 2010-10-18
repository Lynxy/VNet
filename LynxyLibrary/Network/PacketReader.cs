using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lynxy.Network
{
    public sealed class PacketReader
    {
        private byte[] _data;
        private int _position = 0;

        public PacketReader(byte[] data)
        {
            _data = data;
        }

        public PacketReader Seek(int position)
        {
            _position = position;
            return this;
        }

        public byte ReadByte()
        {
            return _data[_position++];
        }

        public bool EOF()
        {
            return (_position >= _data.Length);
        }

        public short ReadWord()
        {
            short ret = BitConverter.ToInt16(_data, _position);
            _position += 2;
            return ret;
        }

        public int ReadDword()
        {
            int ret = BitConverter.ToInt32(_data, _position);
            _position += 4;
            return ret;
        }

        public string ReadString(int length)
        {
            byte[] str = new byte[length];
            Array.Copy(_data, _position, str, 0, length);
            _position += length;
            return Encoding.ASCII.GetString(str);
        }

        public string ReadStringNT()
        {
            int i;
            for (i = _position; i < _data.Length; i++)
                if (_data[i] == 0)
                    break;
            int length = i - _position;
            string ret = ReadString(length);
            _position++;
            return ret;
        }

        public byte[] ReadBytes(int length)
        {
            byte[] ret = new byte[length];
            Array.Copy(_data, _position, ret, 0, length);
            _position += length;
            return ret;
        }

        public byte[] ReadToEnd()
        {
            int length = _data.Length - _position;
            return ReadBytes(length);
        }

        public byte[] ReturnPacket()
        {
            return _data;
        }
    }
}
