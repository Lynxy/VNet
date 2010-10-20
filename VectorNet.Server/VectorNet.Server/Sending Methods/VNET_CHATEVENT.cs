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
            foreach (User u in Users)
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
            foreach (User u in Users)
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
            foreach (User u in GetUsersInChannel(console, user.Channel, false))
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
            foreach (User u in GetUsersInChannel(console, user.Channel, false))
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
            foreach (User u in Users)
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
            foreach (User u in GetUsersInChannel(console, user.Channel, false))
                if (u != user)
                    u.Packet.Clear()
                        .InsertByte((byte)ChatEventType.UserJoinedChannel)
                        .InsertDWord(user.Ping)
                        .InsertByte((byte)user.Flags)
                        .InsertStringNT(user.Username)
                        .InsertStringNT(user.Client)
                        .Send(VNET_CHATEVENT);
        }

        protected void SendUserLeftChannel(User user)
        {
            foreach (User u in GetUsersInChannel(console, user.Channel, false))
                if (u != user)
                    u.Packet.Clear()
                        .InsertByte((byte)ChatEventType.UserLeftChannel)
                        .InsertDWord(user.Ping)
                        .InsertByte((byte)user.Flags)
                        .InsertStringNT(user.Username)
                        .InsertStringNT(user.Client)
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
