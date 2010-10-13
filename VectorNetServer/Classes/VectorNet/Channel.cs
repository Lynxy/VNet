using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VNET.Channel
{
    [Flags] public enum ChannelFlags
    {
        Normal =         0x00,
        Public =         0x01,
        Administrative = 0x02,
        Clan =           0x04,
        Silent =         0x08
    }

    public class Channel
    {
        protected ChannelFlags _Flags;
        protected string _Name;

        public Channel(string name) : this(name, ChannelFlags.Normal)
        {
        }

        public Channel(string name, ChannelFlags flags)
        {
            _Name = name;
            _Flags = flags;
        }


        public string Name
        {
            get { return _Name; }
        }

        public ChannelFlags Flags
        {
            get { return _Flags; }
            set { _Flags = value; }
        }
    }
}
