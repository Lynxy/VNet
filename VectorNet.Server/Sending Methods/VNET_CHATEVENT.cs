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

        /// <summary>
        /// Sends a CHATEVENT packet to all users on server notifying that a user joined the server.
        /// </summary>
        /// <param name="user">The user that joined the server</param>
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

        /// <summary>
        /// Sends a CHATEVENT packet to all users on server notifying that a user left the server.
        /// </summary>
        /// <param name="user">The user that left the server</param>
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

        /// <summary>
        /// Sends a CHATEVENT packet to all users in a channel notifying that a user talked.
        /// </summary>
        /// <param name="user">The user that talked</param>
        /// <param name="message">The message</param>
        protected void SendUserTalk(User user, string message)
        {
            foreach (User u in GetUsersInChannel(user.Channel))
                SendUserTalkSingle(user, u, message);
                    
        }

        /// <summary>
        /// Sends a CHATEVENT packet to a single user notifying that a user talked. Will not send the message to yourself.
        /// </summary>
        /// <param name="user">The user that talked</param>
        /// <param name="targetUser">The user to receive the event</param>
        /// <param name="message">The message</param>
        protected void SendUserTalkSingle(User user, User targetUser, string message)
        {
            SendUserTalkSingle(user, targetUser, message, true);
        }

        /// <summary>
        /// Sends a CHATEVENT packet to a single user notifying that a user talked.
        /// </summary>
        /// <param name="user">The user that talked</param>
        /// <param name="targetUser">The user to receive the event</param>
        /// <param name="message">The message</param>
        /// <param name="ignoreSelf">Whether or not to ignore yourself</param>
        protected void SendUserTalkSingle(User user, User targetUser, string message, bool ignoreSelf)
        {
            if (ignoreSelf && user == targetUser)
                return;

            targetUser.Packet.Clear()
                .InsertByte((byte)ChatEventType.UserTalk)
                .InsertDWord(user.Ping)
                .InsertByte((byte)user.Flags)
                .InsertStringNT(user.Username)
                .InsertStringNT(message)
                .Send(VNET_CHATEVENT);
        }

        /// <summary>
        /// Sends a CHATEVENT packet to all users in a channel notifying that a user emoted.
        /// </summary>
        /// <param name="user">The user that emoted</param>
        /// <param name="message">The emote</param>
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

        /// <summary>
        /// Sends a CHATEVENT packet to a user notifying of an error.
        /// </summary>
        /// <param name="user">The user to send to</param>
        /// <param name="message">The error</param>
        protected void SendServerError(User user, string message)
        {
            user.Packet.Clear()
                .InsertByte((byte)ChatEventType.ServerInfo)
                .InsertDWord(0x01) //error
                .InsertByte(0) 
                .InsertStringNT("")
                .InsertStringNT(message)
                .Send(VNET_CHATEVENT);
        }

        /// <summary>
        /// Sends a CHATEVENT packet to a user notifying of an information message.
        /// </summary>
        /// <param name="user">The user to send to</param>
        /// <param name="message">The info message</param>
        protected void SendServerInfo(User user, string message)
        {
            user.Packet.Clear()
                .InsertByte((byte)ChatEventType.ServerInfo)
                .InsertDWord(0x02) //info
                .InsertByte(0) 
                .InsertStringNT("")
                .InsertStringNT(message)
                .Send(VNET_CHATEVENT);
        }

        /// <summary>
        /// Sends a CHATEVENT packet to a user notifying of account information
        /// </summary>
        /// <param name="user">The user to send to</param>
        /// <param name="message">The account info</param>
        protected void SendAccountInfo(User user, string message)
        {
            user.Packet.Clear()
                .InsertByte((byte)ChatEventType.ServerInfo)
                .InsertDWord(0x03) //account info
                .InsertByte(0) 
                .InsertStringNT("")
                .InsertStringNT(message)
                .Send(VNET_CHATEVENT);
        }

        /// <summary>
        /// Sends a CHATEVENT packet to all online users notifying of a server broadcast
        /// </summary>
        /// <param name="userBroadcasting">The user that is broadcasting</param>
        /// <param name="message">The broascast message</param>
        protected void SendServerBroadcast(User userBroadcasting, string message)
        {
            foreach (User u in GetAllOnlineUsers()) 
                u.Packet.Clear()
                    .InsertByte((byte)ChatEventType.ServerInfo)
                    .InsertDWord(0x04) //broadcast
                    .InsertByte(0) 
                    .InsertStringNT(userBroadcasting.Username)
                    .InsertStringNT(message)
                    .Send(VNET_CHATEVENT);
        }

        /// <summary>
        /// Sends a CHATEVENT packet to a user notifying they joined a channel.
        /// </summary>
        /// <param name="user">The user to send to</param>
        protected void SendJoinedChannelSuccessfully(User user)
        {
            user.Packet.Clear()
                .InsertByte((byte)ChatEventType.ServerInfo)
                .InsertDWord(0x05)//you joined channel
                .InsertByte((byte)user.Channel.Flags) 
                .InsertStringNT(user.Channel.Owner.Username)
                .InsertStringNT(user.Channel.Name)
                .Send(VNET_CHATEVENT);
        }

        /// <summary>
        /// Sends a CHATEVENT packet to all users in a channel notifying that a user joined the channel.
        /// </summary>
        /// <param name="user">The user that joined</param>
        protected void SendUserJoinedChannel(User user)
        {
            foreach (User u in GetUsersInChannel(user.Channel))
                if (CanUserSeeUser(u, user))
                    SendUserJoinedChannelSingle(u, user);
        }

        /// <summary>
        /// Sends a CHATEVENT packet to a user notifying a user joined their channel.
        /// </summary>
        /// <param name="user">The user that joined</param>
        /// <param name="userJoined">The user to send the event to</param>
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

        /// <summary>
        /// Sends a CHATEVENT packet to all users in a channel notifying that a user left the channel.
        /// </summary>
        /// <param name="user">The user that left the channel</param>
        protected void SendUserLeftChannel(User user)
        {
            foreach (User u in GetUsersInChannel(user.Channel))
                if (CanUserSeeUser(u, user))
                    SendUserLeftChannelSingle(u, user);
        }

        /// <summary>
        /// Sends a CHATEVENT packet to a user notifying that a user left their channel.
        /// </summary>
        /// <param name="user">The user that left the channel</param>
        /// <param name="userLeft">The user to send the event to</param>
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

        /// <summary>
        /// Sends a CHATEVENT packet to a user notifying them thay they sent a whisper.
        /// </summary>
        /// <param name="userFrom">The user that whispered</param>
        /// <param name="userTo">The user they whispered to</param>
        /// <param name="message">The whisper</param>
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

        /// <summary>
        /// Sends a CHATEVENT packet to a user notifying them thay they received a whisper.
        /// </summary>
        /// <param name="userTo">The user that is being whispered</param>
        /// <param name="userFrom">The user that the whisper is from</param>
        /// <param name="message">The whisper</param>
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
