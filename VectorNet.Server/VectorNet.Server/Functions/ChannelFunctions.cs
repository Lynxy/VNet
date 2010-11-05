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

        protected void CreateDefaultChannels()
        {
            Channel_Main = new Channel(Config.MainChannel, ChannelFlags.Public, false, console);
            Channels.Add(Channel_Main);

            Channel_Admin = new Channel(Config.AdminChannel, ChannelFlags.Administrative, false, console);
            Channels.Add(Channel_Admin);

            Channel_Void = new Channel(Config.VoidChannel, ChannelFlags.Public | ChannelFlags.Silent, false, console);
            Channels.Add(Channel_Void);
        }

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

        protected bool ChannelHasFlags(Channel chan, ChannelFlags flags)
        {
            return (chan.Flags & flags) == flags;
        }

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

        protected List<User> GetUsersInChannel(Channel channel)
        {
            return GetUsersInChannel(console, channel, true);
        }

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

        protected void SendServerInfoToChannel(Channel channel, string message)
        {
            foreach (User user in GetUsersInChannel(channel))
                SendServerInfo(user, message);
        }

        protected void SendServerInfoToChannel(Channel channel, string message, User uniqueUser, string uniqueMessage)
        {
            foreach (User user in GetUsersInChannel(channel))
                if (user == uniqueUser)
                    SendServerInfo(user, uniqueMessage);
                else
                    SendServerInfo(user, message);
        }

        protected User PromoteNewUserToOwner(Channel chan)
        {
            User oldOwner = chan.Owner;
            User user = chan.PromoteNewOwner();
            SendList(user, ListType.UsersFlagsUpdate);
            SendServerInfoToChannel(chan, user.Username + " has been made the new channel owner due to previous owner " + oldOwner.Username + " being gone too long.");
            return user;
        }

        protected void BanUserByUsername(User user, User targetUser, Channel fromChannel)
        {
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

        protected void UnbanUserByIP(User user, User targetUser, Channel fromChannel)
        {
            if (fromChannel.BannedIPs.Contains(targetUser.IPAddress))
            {
                fromChannel.BannedIPs.Remove(targetUser.IPAddress);
                foreach (User u in GetUsersByIP(targetUser.IPAddress))
                {
                    //Unban every user in specified channel
                    UnbanUser(user, u, fromChannel, true);
                }
            }
        }

        protected void UnbanUser(User user, User targetUser, Channel fromChannel, bool wasIPBan)
        {
            bool wasBanned = fromChannel.IsUserBanned(targetUser);

            if (fromChannel.BannedUsers.Contains(targetUser.Username.ToLower()))
                fromChannel.BannedUsers.Remove(targetUser.Username.ToLower());
            if (fromChannel.BannedIPs.Contains(targetUser.IPAddress))
                fromChannel.BannedIPs.Remove(targetUser.IPAddress);

            if (wasBanned)
            {
                SendServerInfoToChannel(fromChannel, targetUser.Username + " was un" + (wasIPBan == true ? "ipban" : "ban") + "ned from the channel by " + user.Username + ".");
                SendServerInfo(targetUser, user.Username + " has un" + (wasIPBan == true ? "ipban" : "ban") + "ned you from channel " + fromChannel.Name + ".");
            }
        }
    }
}
