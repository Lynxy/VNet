using System;
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
            List<User> users;
            switch (listType)
            {
                case ListType.UsersInChannel:
                    users = GetUsersInChannel(user, user.Channel, false); //dont exclude current user from channel listing

                    if (users == null)
                        return;

                    user.Packet.Clear()
                        .InsertByte((byte)ListType.UsersInChannel)
                        .InsertWord((short)users.Count);

                    foreach (User tmp in users)
                        user.Packet.InsertStringNT(tmp.Username)
                            .InsertStringNT(tmp.Client)
                            .InsertDWord(tmp.Ping)
                            .InsertByte((byte)tmp.Flags);
                    user.Packet.Send(VNET_LIST);
                        
                    break;
                case ListType.UsersBannedFromChannel:
                    List<User> banned = GetUsersBannedFromChannel(user, user.Channel);

                    user.Packet.Clear()
                        .InsertByte((byte)ListType.UsersBannedFromChannel)
                        .InsertWord((short)banned.Count);

                    foreach (User tmp in banned)
                        user.Packet.InsertStringNT(tmp.Username)
                            .InsertStringNT(tmp.Client)
                            .InsertDWord(tmp.Ping)
                            .InsertByte((byte)tmp.Flags);
                    user.Packet.Send(VNET_LIST);

                    break;

                case ListType.UsersOnServer:
                    Dictionary<User, List<string>> userBannedFrom = new Dictionary<User, List<string>>();
                    foreach (Channel chan in Channels.ToList()) //duplicate to avoid thread conflicts
                    {
                        foreach (User usr in GetUsersBannedFromChannel(user, chan))
                        {
                            if (!userBannedFrom.ContainsKey(usr))
                                userBannedFrom.Add(usr, new List<string>());
                            if (!userBannedFrom[usr].Contains(chan.Name))
                                userBannedFrom[usr].Add(chan.Name);
                        }
                    }

                    user.Packet.Clear()
                        .InsertByte((byte)ListType.UsersOnServer)
                        .InsertWord((short)Users.Count);

                    foreach (User usr in GetAllOnlineUsers())
                    {
                        user.Packet.InsertStringNT(usr.Username)
                                   .InsertStringNT(usr.Client)
                                   .InsertStringNT(usr.Channel.Name);

                        if (userBannedFrom.ContainsKey(usr))
                            user.Packet.InsertByte(0x01)
                                       .InsertStringNT(String.Join(((char)1).ToString(), userBannedFrom[usr]));
                        else
                            user.Packet.InsertByte(0x00);

                        user.Packet.InsertDWord(usr.Ping)
                                   .InsertByte((byte)usr.Flags);
                    }
                    user.Packet.Send(VNET_LIST);
                    break;

                case ListType.UsersFlagsUpdate:
                    users = GetUsersInChannel(user, user.Channel, false); //dont exclude current user from channel listing

                    if (users == null)
                        return;

                    foreach (User u in users)
                        u.Packet.InsertStringNT(user.Username)
                            .InsertStringNT(user.Client)
                            .InsertDWord(user.Ping)
                            .InsertByte((byte)user.Flags)
                            .Send(VNET_LIST);

                    break;

            }
        }

    }
}
