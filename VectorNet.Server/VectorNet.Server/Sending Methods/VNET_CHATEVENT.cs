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
                        .InsertByte((byte)ChatEventType.USER_JOIN_VNET)
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
                        .InsertByte((byte)ChatEventType.USER_LEAVE_VNET)
                        .InsertDWord(user.Ping)
                        .InsertByte((byte)user.Flags)
                        .InsertStringNT(user.Username)
                        .InsertStringNT(user.Client)
                        .Send(VNET_CHATEVENT);
        }

        protected void SendUserTalk(User user, string message)
        {
            foreach (User u in user.Channel.Users)
                if (u != user)
                    u.Packet.Clear()
                        .InsertByte((byte)ChatEventType.USER_TALK)
                        .InsertDWord(user.Ping)
                        .InsertByte((byte)user.Flags)
                        .InsertStringNT(user.Username)
                        .InsertStringNT(message)
                        .Send(VNET_CHATEVENT);
        }

        protected void SendUserEmote(User user, string message)
        {
            foreach (User u in user.Channel.Users)
                if (u != user)
                    u.Packet.Clear()
                        .InsertByte((byte)ChatEventType.USER_EMOTE)
                        .InsertDWord(user.Ping)
                        .InsertByte((byte)user.Flags)
                        .InsertStringNT(user.Username)
                        .InsertStringNT(message)
                        .Send(VNET_CHATEVENT);
        }

        protected void SendServerError(User user, string message)
        {
            user.Packet.Clear()
                .InsertByte((byte)ChatEventType.SERVER_INFO)
                .InsertDWord(0)
                .InsertByte(0x01) //error
                .InsertStringNT("")
                .InsertStringNT(message)
                .Send(VNET_CHATEVENT);
        }

        protected void SendServerInfo(User user, string message)
        {
            user.Packet.Clear()
                .InsertByte((byte)ChatEventType.SERVER_INFO)
                .InsertDWord(0)
                .InsertByte(0x02) //info
                .InsertStringNT("")
                .InsertStringNT(message)
                .Send(VNET_CHATEVENT);
        }

        protected void SendAccountInfo(User user, string message)
        {
            user.Packet.Clear()
                .InsertByte((byte)ChatEventType.SERVER_INFO)
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
                    .InsertByte((byte)ChatEventType.SERVER_INFO)
                    .InsertDWord(userBroadcasting.Ping)
                    .InsertByte(0x04) //broadcast
                    .InsertStringNT(userBroadcasting.Username)
                    .InsertStringNT(message)
                    .Send(VNET_CHATEVENT);
        }

        protected void SendJoinedChannelSuccessfully(User user)
        {
            user.Packet.Clear()
                .InsertByte((byte)ChatEventType.SERVER_INFO)
                .InsertDWord(0)
                .InsertByte(0x05) //you joined channel
                .InsertStringNT("")
                .InsertStringNT(user.Channel.Name)
                .Send(VNET_CHATEVENT);
        }

        protected void SendUserJoinedChannel(User user)
        {
            foreach (User u in user.Channel.Users)
                if (u != user)
                    u.Packet.Clear()
                        .InsertByte((byte)ChatEventType.USER_JOIN_CHANNEL)
                        .InsertDWord(user.Ping)
                        .InsertByte((byte)user.Flags)
                        .InsertStringNT(user.Username)
                        .InsertStringNT(user.Client)
                        .Send(VNET_CHATEVENT);
        }

        protected void SendUserLeftChannel(User user)
        {
            foreach (User u in user.Channel.Users)
                if (u != user)
                    u.Packet.Clear()
                        .InsertByte((byte)ChatEventType.USER_LEAVE_CHANNEL)
                        .InsertDWord(user.Ping)
                        .InsertByte((byte)user.Flags)
                        .InsertStringNT(user.Username)
                        .InsertStringNT(user.Client)
                        .Send(VNET_CHATEVENT);
        }

        protected void SendUserWhisperTo(User userFrom, User userTo, string message)
        {
            userFrom.Packet.Clear()
                .InsertByte((byte)ChatEventType.WHISPER_TO)
                .InsertDWord(userTo.Ping)
                .InsertByte((byte)userTo.Flags)
                .InsertStringNT(userTo.Username)
                .InsertStringNT(message)
                .Send(VNET_CHATEVENT);
        }

        protected void SendUserWhisperFrom(User userTo, User userFrom, string message)
        {
            userTo.Packet.Clear()
                .InsertByte((byte)ChatEventType.WHISPER_FROM)
                .InsertDWord(userFrom.Ping)
                .InsertByte((byte)userFrom.Flags)
                .InsertStringNT(userFrom.Username)
                .InsertStringNT(message)
                .Send(VNET_CHATEVENT);
        }
        
    }
}
