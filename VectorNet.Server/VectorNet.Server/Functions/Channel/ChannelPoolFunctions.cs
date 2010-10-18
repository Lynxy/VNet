using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        //Channel Pool Functions
        //This class is for methods that must look at the Channels dictionary, not a single channel
        protected Channel GetChannelByName(string channel)
        {
            Channel ret = Channels.Find(c =>
                c.Name.ToLower() == channel.ToLower()
                );
            if (ret == null)
            {
                ret = new Channel(channel);
                Channels.Add(ret);
            }
            return ret;
        }
    }
}
