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

        protected void SendServerInfo(User user)
        {

        }

        protected void SendChatEvent(User user, ChatEventType chatType)
        {
            SendChatEvent(user, chatType, "");
        }

        protected void SendChatEvent(User user, ChatEventType chatType, string message)
        {
            switch (chatType)
            {
                case ChatEventType.USER_JOIN_CHANNEL:
                case ChatEventType.USER_LEAVE_CHANNEL:
                case ChatEventType.USER_TALK:
                    if (message == "")
                        message = user.Client;
                    foreach (User u in GetUsersInChannel(user, user.Channel, true))
                        u.Packet.Clear()
                            .InsertByte((byte)chatType)
                            .InsertDWord(user.Ping)
                            .InsertByte((byte)user.Flags)
                            .InsertStringNT(user.Username)
                            .InsertStringNT(message)
                            .Send(VNET_CHATEVENT);
                    break;
            }
        }

        protected void SendChannelList(User user, Channel channel)
        {
            List<User> users = GetUsersInChannel(user, channel, false);
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

            SendChatEvent(user, ChatEventType.USER_JOIN_CHANNEL);
            SendChannelList(user, channel);
        }

        protected void RemoveUserFromChannel(User user)
        {
            if (user.Channel == null)
                return;
            SendChatEvent(user, ChatEventType.USER_LEAVE_CHANNEL);
            user.Channel.RemoveUser(user);
        }

        protected void UserChat(User user, string message)
        {
            SendChatEvent(user, ChatEventType.USER_TALK, message);
        }

    }
}
