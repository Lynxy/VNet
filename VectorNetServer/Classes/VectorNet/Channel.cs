using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorNetServer
{
    public class Channel
    {
        protected byte _Flags;
        protected string _Name;

        public Channel(string name) : this(name, 0x00)
        {
        }

        public Channel(string name, byte flags)
        {
            _Flags = flags;
            _Name = name;
        }
    }
}
