using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lynxy.Network
{
    static public class PacketBuffer
    {
        static private readonly object _locker = new object();
        static public byte[] GetNextPacket(ref byte[] packet)
        {
            lock (_locker)
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

    public sealed class Packet
    {
        public delegate byte[] EncryptorDelegate(byte[] data);
        public delegate void SendDataDelegate(Packet packet, ref byte[] data);
        public event SendDataDelegate DataSent;

        private StringBuilder buffer;
        public bool skipHeaders = false;

        public Packet() 
        {
            buffer = new StringBuilder();
        }

        static public byte[] Parse(byte[] packet)
        {
            if (packet.Length < 4)
                return null;
            int length = BitConverter.ToInt32(packet, 0);
            if (length > packet.Length - 4)
                return null;
            byte[] data = new byte[length];
            Array.Copy(packet, 4, data, 0, length);
            return data;
        }

        static public implicit operator byte[](Packet pkt)
        {
            byte[] data = CharToByte(pkt.buffer.ToString().ToCharArray());
            if (pkt.Encryptor != null)
                data = pkt.Encryptor(data);

            byte[] ret = new byte[data.Length + 4];

            BitConverter.GetBytes(data.Length).CopyTo(ret, 0);
            Array.Copy(data, 0, ret, 4, data.Length);
            pkt.Clear();
            return ret;
        }

        static private byte[] CharToByte(char[] chr)
        {
            byte[] ret = new byte[chr.Length];
            for (int i = 0; i < chr.Length; i++)
                ret[i] = (byte)chr[i];
            return ret;
        }

        public override string ToString()
        {
            return buffer.ToString();
        }

        public Packet Clear()
        {
            if (buffer.Length > 0)
                buffer.Clear();
            return this;
        }

        public void Send(byte packetId)
        {
            InsertByte(packetId, 0);
            byte[] ret;
            if (skipHeaders)
                ret = CharToByte(buffer.ToString().ToCharArray());
            else
                ret = this;
            if (DataSent != null)
                DataSent(this, ref ret);
            Clear();
        }

        public int Length { get { return buffer.Length; } }
        public EncryptorDelegate Encryptor { get; set; }

        


        #region String
        public Packet InsertString(string data)
        {
            if (data == null) return this;
            return InsertString(data, buffer.Length);
        }

        public Packet InsertString(string data, int position)
        {
            if (data == null) return this;
            buffer.Insert(position, data);
            return this;
        }

        public Packet InsertStringNT(string data)
        {
            if (data == null) return this;
            return InsertStringNT(data, buffer.Length);
        }

        public Packet InsertStringNT(string data, int position)
        {
            if (data == null) return this;
            buffer.Insert(position, data + (char)0);
            return this;
        }
        #endregion

        #region Byte
        public Packet InsertByte(byte b)
        {
            return InsertByte(b, buffer.Length);
        }

        public Packet InsertByte(byte b, int position)
        {
            buffer.Insert(position, (char)b);
            return this;
        }

        public Packet InsertByte(byte[] bytes)
        {
            return InsertByte(bytes, buffer.Length);
        }

        public Packet InsertByte(byte[] bytes, int position)
        {
            foreach (byte b in bytes)
                buffer.Insert(position++, (char)b);
            return this;
        }
        #endregion

        #region Word
        public Packet InsertWord(short word)
        {
            return InsertWord(word, buffer.Length);
        }

        public Packet InsertWord(short word, int position)
        {
            return InsertByte(BitConverter.GetBytes(word), position);
        }

        public Packet InsertWord(short[] words)
        {
            return InsertWord(words, buffer.Length);
        }

        public Packet InsertWord(short[] words, int position)
        {
            foreach (short word in words)
                InsertByte(BitConverter.GetBytes(word), position);
            return this;
        }
        #endregion

        #region DWord
        public Packet InsertDWord(int dword)
        {
            return InsertDWord(dword, buffer.Length);
        }

        public Packet InsertDWord(int dword, int position)
        {
            return InsertByte(BitConverter.GetBytes(dword), position);
        }

        public Packet InsertDWord(int[] dwords)
        {
            return InsertDWord(dwords, buffer.Length);
        }

        public Packet InsertDWord(int[] dwords, int position)
        {
            foreach (int dword in dwords)
                InsertByte(BitConverter.GetBytes(dword), position);
            return this;
        }
        #endregion
    }
}
