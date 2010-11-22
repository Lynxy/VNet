using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        //Channel Functions
        //This class is for methods that deal with a single channel, not multiple channels
        protected Channel Channel_Main;
        protected Channel Channel_Admin;
        protected Channel Channel_Void;

        /// <summary>
        /// Sets up the default channels.
        /// </summary>
        protected void CreateDefaultChannels()
        {
            Channel_Main = new Channel(Config.DefaultChannels.Main, ChannelFlags.Public, false, console);
            Channels.Add(Channel_Main);

            Channel_Admin = new Channel(Config.DefaultChannels.Admin, ChannelFlags.Administrative, false, console);
            Channels.Add(Channel_Admin);

            Channel_Void = new Channel(Config.DefaultChannels.Void, ChannelFlags.Public | ChannelFlags.Silent, false, console);
            Channels.Add(Channel_Void);
        }

        /// <summary>
        /// Returns a channel based off name.
        /// </summary>
        /// <param name="user">The calling user</param>
        /// <param name="channel">The channel name</param>
        /// <param name="allowCreation">Whether or not to create the channel if it doesn't exist</param>
        protected Channel GetChannelByName(User user, string channel, bool allowCreation)
        {
            Channel ret = Channels.Find(c =>
                c.Name.ToLower() == channel.ToLower());
            if (ret == null && allowCreation)
            {
                if (channel == "*")
                {
                    SendServerError(user, "You cannot join channel * as it is a special channel.");
                    return null;
                }
                ret = new Channel(channel);
                ret.Owner = user;
                Channels.Add(ret);
            }
            return ret;
        }

        /// <summary>
        /// Returns true if the channel has the specified flags.
        /// </summary>
        /// <param name="chan">The channel to check</param>
        /// <param name="flags">The flags to check for</param>
        protected bool ChannelHasFlags(Channel chan, ChannelFlags flags)
        {
            return (chan.Flags & flags) == flags;
        }

        /// <summary>
        /// If no users are left in channel, it will clear the banned list and owner,
        /// and delete the channel.
        /// </summary>
        /// <param name="channel"></param>
        protected void AttemptToCloseChannel(Channel channel)
        {
            if (channel.UserCount == 0)
            {
                if (channel.Closeable == false)
                {
                    channel.BannedIPs.Clear();
                    channel.BannedUsers.Clear();
                    channel.Owner = null;
                }
                else
                    Channels.Remove(channel);
            }
        }

        /// <summary>
        /// Gets the entire list of users in a channel.
        /// </summary>
        /// <param name="channel">The channel to get the user list of</param>
        protected List<User> GetUsersInChannel(Channel channel)
        {
            return GetUsersInChannel(console, channel, true);
        }

        /// <summary>
        /// Gets the list of users in a channel from a user's perspective.
        /// </summary>
        /// <param name="userPerspective">The user perspective to use</param>
        /// <param name="channel">The channel to get the user list of</param>
        /// <param name="excludeUser">Ignore the calling user from the list</param>
        protected List<User> GetUsersInChannel(User userPerspective, Channel channel, bool excludeUser)
        {
            List<User> ret = channel.GetCompleteUserList();
            if (excludeUser && ret.Contains(userPerspective))
                ret.Remove(userPerspective);
            for (int i = ret.Count - 1; i >= 0; i--)
            {
                if (CanUserSeeUser(userPerspective, ret[i]) == false)
                    ret.RemoveAt(i);
            }
            return ret;
        }

        /// <summary>
        /// Sends a server info message to every user in a channel
        /// </summary>
        /// <param name="channel">The channel to send to</param>
        /// <param name="message">The info message</param>
        protected void SendServerInfoToChannel(Channel channel, string message)
        {
            foreach (User user in GetUsersInChannel(channel))
                SendServerInfo(user, message);
        }

        /// <summary>
        /// Sends a server info message to every user in a channel,
        /// with a unique message to a specific user. Unique user does not hear the original message.
        /// </summary>
        /// <param name="channel">The channel to send to</param>
        /// <param name="message">The info message</param>
        /// <param name="uniqueUser">A unique user that will get a special message</param>
        /// <param name="uniqueMessage">The special message</param>
        protected void SendServerInfoToChannel(Channel channel, string message, User uniqueUser, string uniqueMessage)
        {
            foreach (User user in GetUsersInChannel(channel))
                if (user == uniqueUser)
                    SendServerInfo(user, uniqueMessage);
                else
                    SendServerInfo(user, message);
        }

        /// <summary>
        /// Finds a new owner for a channel and informs the channel a new owner was made.
        /// </summary>
        /// <param name="chan">The channel in question</param>
        protected User PromoteNewUserToOwner(Channel chan)
        {
            User oldOwner = chan.Owner;
            User user = chan.PromoteNewOwner();
            SendList(user, ListType.UsersFlagsUpdate);
            SendServerInfoToChannel(chan, user.Username + " has been made the new channel owner due to previous owner " + oldOwner.Username + " being gone too long.");
            return user;
        }

        /// <summary>
        /// Kicks a user from a channel.
        /// </summary>
        /// <param name="user">The user doing the kicking</param>
        /// <param name="targetUser">The user to kick</param>
        /// <param name="cmdRest">An optional kick message</param>
        protected void KickUserFromChannel(User user, User targetUser, string cmdRest)
        {
            SendServerInfoToChannel(user.Channel, targetUser.Username + " was kicked out of the channel by " + user.Username + (cmdRest != string.Empty ? " (" + cmdRest + ")" : "") + ".",
                        targetUser, "You have been kicked out of the channel by " + user.Username + (cmdRest != string.Empty ? " (" + cmdRest + ")" : "") + ".");
            JoinUserToChannel(targetUser, Channel_Void);
        }

        /// <summary>
        /// Bans a user from a channel by username.
        /// </summary>
        /// <param name="user">The user doing the banning</param>
        /// <param name="targetUser">The user to ban</param>
        /// <param name="fromChannel">The channel to ban from</param>
        protected void BanUserByUsername(User user, User targetUser, Channel fromChannel)
        { //TODO: Reason for ban
            if (!fromChannel.BannedUsers.Contains(targetUser.Username))
            {
                fromChannel.BannedUsers.Add(targetUser.Username.ToLower());

                SendServerInfoToChannel(fromChannel, targetUser.Username + " was banned from the channel by " + user.Username + "!",
                        targetUser, "You have been banned from the channel by " + user.Username + "!");
                if (targetUser.Channel == fromChannel)
                    JoinUserToChannel(targetUser, Channel_Void);
                else
                    SendServerInfo(targetUser, user.Username + " has banned you from channel " + fromChannel.Name + ".");
            }
        }

        /// <summary>
        /// Bans a user from a channel by IP address.
        /// </summary>
        /// <param name="user">The user doing the banning</param>
        /// <param name="targetUser">The user to IP ban</param>
        /// <param name="fromChannel">The channel to IP ban from</param>
        protected void BanUserByIP(User user, User targetUser, Channel fromChannel)
        {
            if (!fromChannel.BannedIPs.Contains(targetUser.IPAddress))
            {
                //ban target user first, ensuring their username gets added to channels banned list
                BanUserByUsername(user, targetUser, fromChannel);

                fromChannel.BannedIPs.Add(targetUser.IPAddress);

                //ban matching ips
                foreach (User target in GetUsersByIP(targetUser.IPAddress))
                {
                    if (target != targetUser)
                    {
                        SendServerInfoToChannel(fromChannel, target.Username + " was banned from the channel by " + user.Username + " [IP match]!",
                            target, "You have been banned from the channel by " + user.Username + " [IP match]!");
                        if (target.Channel == fromChannel)
                            JoinUserToChannel(target, Channel_Void);
                        else
                            SendServerInfo(target, user.Username + " has banned you from channel " + fromChannel.Name + ".");
                    }
                }
            }
        }

        /// <summary>
        /// Unbans a user by username and IP address from a channel.
        /// </summary>
        /// <param name="user">The user doing the unbanning</param>
        /// <param name="targetUser">The user to unban</param>
        /// <param name="fromChannel">The channel to unban from</param>
        /// <param name="wasIPBan">Specifies whether or not the ban was an IP ban</param>
        protected void UnbanUser(User user, User targetUser, Channel fromChannel, bool wasIPBan)
        {
            bool wasBanned = fromChannel.IsUserBanned(targetUser);

            if (fromChannel.BannedUsers.Contains(targetUser.Username.ToLower()))
                fromChannel.BannedUsers.Remove(targetUser.Username.ToLower());
            if (fromChannel.BannedIPs.Contains(targetUser.IPAddress))
            {
                fromChannel.BannedIPs.Remove(targetUser.IPAddress);
                foreach (User u in GetUsersByIP(targetUser.IPAddress))
                    UnbanUser(user, u, fromChannel, true);
            }

            if (wasBanned)
            {
                SendServerInfoToChannel(fromChannel, targetUser.Username + " was un" + (wasIPBan == true ? "ipban" : "ban") + "ned from the channel by " + user.Username + ".");
                SendServerInfo(targetUser, user.Username + " has un" + (wasIPBan == true ? "ipban" : "ban") + "ned you from channel " + fromChannel.Name + ".");
            }
        }
    }
}
