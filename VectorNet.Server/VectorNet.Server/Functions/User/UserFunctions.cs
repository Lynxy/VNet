using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        //User Functions
        //This class is for methods that deal with a single user, not multiple users
        protected void SendLogonResult(User user, LogonResult result)
        {
            //if (result == LogonResult.SUCCESS)
            //{
            //}
            //else
            //{
                user.Packet.Clear()
                    .InsertByte((byte)result)
                    .InsertStringNT("server ver")
                    .InsertStringNT("TestEnv")
                    .InsertStringNT(user.Username)
                    .InsertDWord(0) //ping
                    .InsertByte(0) //flag
                    .Send(VNET_LOGON);
            //}
        }

        protected void DisconnectUser(User user)
        {
            RemoveUserFromChannel(user);
            user.IsOnline = false;
            user.Socket.Close();
        }

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

        protected void JoinUserToChannel(User user, Channel channel)
        {
            RemoveUserFromChannel(user);
            channel.AddUser(user, false);
            SendUserJoinedChannel(user);
            SendChannelList(user);
        }

        protected void RemoveUserFromChannel(User user)
        {
            if (user.Channel == null)
                return;
            SendUserLeftChannel(user);
            user.Channel.RemoveUser(user);
        }
    }
}
