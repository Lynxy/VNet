using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        //Chat Events class
        //This class is for VNET_CHATEVENT sending methods
        protected void SendUserJoinedServer(User user)
        {
            foreach (User u in GetAllOnlineUsers())
                if (u != user)
                    u.Packet.Clear()
                        .InsertByte((byte)ChatEventType.UserJoinedServer)
                        .InsertDWord(user.Ping)
                        .InsertByte((byte)user.Flags)
                        .InsertStringNT(user.Username)
                        .InsertStringNT(user.Client)
                        .Send(VNET_CHATEVENT);
        }

        protected void SendUserLeftServer(User user)
        {
            ConsoleSendUserLeftServer(user);
            foreach (User u in GetAllOnlineUsers())
                if (u != user)
                    u.Packet.Clear()
                        .InsertByte((byte)ChatEventType.UserLeftServer)
                        .InsertDWord(user.Ping)
                        .InsertByte((byte)user.Flags)
                        .InsertStringNT(user.Username)
                        .InsertStringNT(user.Client)
                        .Send(VNET_CHATEVENT);
        }

        protected void SendUserTalk(User user, string message)
        {
            foreach (User u in GetUsersInChannel(user.Channel))
                if (u != user)
                    u.Packet.Clear()
                        .InsertByte((byte)ChatEventType.UserTalk)
                        .InsertDWord(user.Ping)
                        .InsertByte((byte)user.Flags)
                        .InsertStringNT(user.Username)
                        .InsertStringNT(message)
                        .Send(VNET_CHATEVENT);
        }

        protected void SendUserEmote(User user, string message)
        {
            foreach (User u in GetUsersInChannel(user.Channel))
                if (u != user)
                    u.Packet.Clear()
                        .InsertByte((byte)ChatEventType.UserEmote)
                        .InsertDWord(user.Ping)
                        .InsertByte((byte)user.Flags)
                        .InsertStringNT(user.Username)
                        .InsertStringNT(message)
                        .Send(VNET_CHATEVENT);
        }

        protected void SendServerError(User user, string message)
        {
            user.Packet.Clear()
                .InsertByte((byte)ChatEventType.ServerInfo)
                .InsertDWord(0)
                .InsertByte(0x01) //error
                .InsertStringNT("")
                .InsertStringNT(message)
                .Send(VNET_CHATEVENT);
        }

        protected void SendServerInfo(User user, string message)
        {
            user.Packet.Clear()
                .InsertByte((byte)ChatEventType.ServerInfo)
                .InsertDWord(0)
                .InsertByte(0x02) //info
                .InsertStringNT("")
                .InsertStringNT(message)
                .Send(VNET_CHATEVENT);
        }

        protected void SendAccountInfo(User user, string message)
        {
            user.Packet.Clear()
                .InsertByte((byte)ChatEventType.ServerInfo)
                .InsertDWord(0)
                .InsertByte(0x03) //account info
                .InsertStringNT("")
                .InsertStringNT(message)
                .Send(VNET_CHATEVENT);
        }

        protected void SendServerBroadcast(User userBroadcasting, string message)
        {
            foreach (User u in GetAllOnlineUsers()) 
                u.Packet.Clear()
                    .InsertByte((byte)ChatEventType.ServerInfo)
                    .InsertDWord(userBroadcasting.Ping)
                    .InsertByte(0x04) //broadcast
                    .InsertStringNT(userBroadcasting.Username)
                    .InsertStringNT(message)
                    .Send(VNET_CHATEVENT);
        }

        protected void SendJoinedChannelSuccessfully(User user)
        {
            user.Packet.Clear()
                .InsertByte((byte)ChatEventType.ServerInfo)
                .InsertDWord(0)
                .InsertByte(0x05) //you joined channel
                .InsertStringNT("")
                .InsertStringNT(user.Channel.Name)
                .Send(VNET_CHATEVENT);
        }

        protected void SendUserJoinedChannel(User user)
        {
            foreach (User u in GetUsersInChannel(user.Channel))
                if (u != user && CanUserSeeUser(u, user))
                    u.Packet.Clear()
                        .InsertByte((byte)ChatEventType.UserJoinedChannel)
                        .InsertDWord(user.Ping)
                        .InsertByte((byte)user.Flags)
                        .InsertStringNT(user.Username)
                        .InsertStringNT(user.Client)
                        .Send(VNET_CHATEVENT);
        }

        protected void SendUserJoinedChannelSingle(User user, User userJoined)
        {
            if (user == userJoined)
                return;

            user.Packet.Clear()
                .InsertByte((byte)ChatEventType.UserJoinedChannel)
                .InsertDWord(userJoined.Ping)
                .InsertByte((byte)userJoined.Flags)
                .InsertStringNT(userJoined.Username)
                .InsertStringNT(userJoined.Client)
                .Send(VNET_CHATEVENT);
        }

        protected void SendUserLeftChannel(User user)
        {
            foreach (User u in GetUsersInChannel(user.Channel))
                if (u != user && CanUserSeeUser(u, user))
                    u.Packet.Clear()
                        .InsertByte((byte)ChatEventType.UserLeftChannel)
                        .InsertDWord(user.Ping)
                        .InsertByte((byte)user.Flags)
                        .InsertStringNT(user.Username)
                        .InsertStringNT(user.Client)
                        .Send(VNET_CHATEVENT);
        }

        protected void SendUserLeftChannelSingle(User user, User userLeft)
        {
            if (user == userLeft)
                return;

            user.Packet.Clear()
                .InsertByte((byte)ChatEventType.UserLeftChannel)
                .InsertDWord(userLeft.Ping)
                .InsertByte((byte)userLeft.Flags)
                .InsertStringNT(userLeft.Username)
                .InsertStringNT(userLeft.Client)
                .Send(VNET_CHATEVENT);
        }

        protected void SendUserWhisperTo(User userFrom, User userTo, string message)
        {
            userFrom.Packet.Clear()
                .InsertByte((byte)ChatEventType.UserWhisperTo)
                .InsertDWord(userTo.Ping)
                .InsertByte((byte)userTo.Flags)
                .InsertStringNT(userTo.Username)
                .InsertStringNT(message)
                .Send(VNET_CHATEVENT);
        }

        protected void SendUserWhisperFrom(User userTo, User userFrom, string message)
        {
            userTo.Packet.Clear()
                .InsertByte((byte)ChatEventType.UserWhisperFrom)
                .InsertDWord(userFrom.Ping)
                .InsertByte((byte)userFrom.Flags)
                .InsertStringNT(userFrom.Username)
                .InsertStringNT(message)
                .Send(VNET_CHATEVENT);
        }


        
    }
}
