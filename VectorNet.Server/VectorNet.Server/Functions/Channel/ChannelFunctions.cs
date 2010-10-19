using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        //Channel Functions
        //This class is for methods that deal with a single channel, not multiple channels
        protected Channel MainChannel;

        protected void CreateMainChannel(string name)
        {
            MainChannel = new Channel(name);
            Channels.Add(MainChannel);
        }

        protected Channel GetChannelByName(string channel, bool allowCreation)
        {
            Channel ret = Channels.Find(c =>
                c.Name.ToLower() == channel.ToLower());
            if (ret == null && allowCreation)
            {
                ret = new Channel(channel);
                Channels.Add(ret);
            }
            return ret;
        }

        protected List<User> GetUsersInChannel(User userPerspective, Channel channel, bool excludeUser)
        {
            //TODO: user perspective
            List<User> ret = channel.GetCompleteUserList();
            if (excludeUser && ret.Contains(userPerspective))
                ret.Remove(userPerspective);
            for (int i = ret.Count - 1; i >= 0; i--)
            {
                if (CanUserSeeUser(userPerspective, ret[i]) == false)
                    ret.RemoveAt(i);
            }
            if (userPerspective == console)
                if (ret.Contains(console) == false)
                    ret.Add(console);
            return ret;
        }
    }
}
