using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {

        [Flags]
        protected enum ListType
        {
            VNET_CHANNEL_LIST = 0x01,
            VNET_CHANNEL_BANNED = 0x02,
            VNET_SERVER_LIST = 0x03
        }

        protected void SendList(User user, ListType flags)
        {
            switch (flags)
            {
                case ListType.VNET_CHANNEL_LIST:
                    List<User> u = GetUsersInChannel(user, user.Channel, true);

                    if (u == null)
                        return;

                    user.Packet.Clear().InsertByte((byte)ListType.VNET_CHANNEL_LIST)
                        .InsertWord((short)u.Count);

                    foreach (User tmp in u)
                        user.Packet.InsertStringNT(tmp.Username)
                            .InsertStringNT(tmp.Client)
                            .InsertDWord(tmp.Ping)
                            .InsertByte((byte)tmp.Flags);
                    user.Packet.Send(VNET_LIST);
                        
                    break;
                case ListType.VNET_CHANNEL_BANNED:
                    break;

                case ListType.VNET_SERVER_LIST:
                    break;
            }
        }

    }
}
