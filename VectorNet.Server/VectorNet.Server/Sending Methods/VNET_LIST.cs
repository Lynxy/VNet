﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        

        protected void SendList(User user, ListType listType)
        {
            switch (listType)
            {
                case ListType.UsersInChannel:
                    List<User> u = GetUsersInChannel(user, user.Channel, false); //dont exclude current user from channel listing

                    if (u == null)
                        return;

                    user.Packet.Clear()
                        .InsertByte((byte)ListType.UsersInChannel)
                        .InsertWord((short)u.Count);

                    foreach (User tmp in u)
                        user.Packet.InsertStringNT(tmp.Username)
                            .InsertStringNT(tmp.Client)
                            .InsertDWord(tmp.Ping)
                            .InsertByte((byte)tmp.Flags);
                    user.Packet.Send(VNET_LIST);
                        
                    break;
                case ListType.UsersBannedFromChannel:
                    break;

                case ListType.UsersOnServer:
                    break;
            }
        }

    }
}
