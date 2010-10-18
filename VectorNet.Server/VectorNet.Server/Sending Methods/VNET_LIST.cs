using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected void SendChannelList(User user)
        {
            List<User> users = GetUsersInChannel(user, user.Channel, false);
            if (users == null)
                return;
            user.Packet.Clear()
                .InsertByte((int)ListType.UsersInChannel);
            foreach (User u in users)
                user.Packet.InsertStringNT(u.Username)
                    .InsertStringNT(u.Client)
                    .InsertDWord(u.Ping)
                    .InsertByte((byte)u.Flags);
            user.Packet.Send(VNET_LIST);
        }
        
    }
}
