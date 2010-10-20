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
                    List<User> banned = user.Channel.Banned;

                    user.Packet.InsertByte((byte)ListType.UsersBannedFromChannel)
                        .InsertWord((short)banned.Count);

                    foreach (User tmp in banned)
                        user.Packet.InsertStringNT(tmp.Username)
                            .InsertStringNT(tmp.Client)
                            .InsertWord((short)tmp.Ping)
                            .InsertByte((byte)tmp.Flags);
                    user.Packet.Send(VNET_LIST);

                    break;

                case ListType.UsersOnServer:
                    List<Channel> ChannelList = Channels.ToList();

                    TempUserStruct temp;
                    List<TempUserStruct> lstTemp = new List<TempUserStruct>();

                    foreach (Channel c in ChannelList)
                    {
                        foreach (User usr in c.GetCompleteUserList())
                        {
                            temp = new TempUserStruct();

                            temp.Username = usr.Username;
                            temp.Channel = usr.Channel.Name;
                            temp.Client = usr.Client;
                            temp.ping = (short)usr.Ping;
                            temp.flags = (byte)usr.Flags;

                            if (c.IsUserBanned(usr))
                            {
                                temp.banned = 0x01;

                                if ((lstTemp.FindAll(f =>
                                    f.Username == usr.Username)) != null)

                                    lstTemp.Find(f => f.Username == usr.Username).bannedChannel.Add(c.Name);
                                else
                                {
                                    temp.bannedChannel.Add(c.Name);
                                    lstTemp.Add(temp);
                                }
                            }
                            else
                            {
                                temp.banned = 0x00;
                                lstTemp.Add(temp);
                            }
                        }
                    }

                    user.Packet.InsertByte((byte)ListType.UsersOnServer)
                        .InsertWord((short)lstTemp.Count);

                    foreach (TempUserStruct usr in lstTemp)
                    {
                        user.Packet.InsertStringNT(usr.Username)
                                   .InsertStringNT(usr.Client)
                                   .InsertStringNT(usr.Channel);

                        if (usr.banned == 0x01)
                            user.Packet.InsertByte(0x01)
                                       .InsertStringNT(string.Join(((char)1).ToString(), usr.bannedChannel.ToArray()));
                        else
                            user.Packet.InsertByte(0x00);

                        user.Packet.InsertWord((short)usr.ping)
                                   .InsertByte((byte)usr.flags);
                    }
                    user.Packet.Send(VNET_LIST);
                    break;
            }
        }

    }
}
