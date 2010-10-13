using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorNetServer
{
    public class Channel
    {
        private byte Flag;
        private string name;

        public Channel(string chan) : this(0x00, chan)
        {
        }

        public Channel(byte f, string chan)
        {
            Flag = f;
            name = chan;
        }
    }
}
