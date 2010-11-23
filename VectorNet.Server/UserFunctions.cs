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

        /// <summary>
        /// Disconnects the user without warning.
        /// </summary>
        /// <param name="user">The user to disconnect</param>
        protected void DisconnectUser(User user)
        {
            if (user.IsOnline)
                ServerStats.usersOnline--;
            user.IsOnline = false;
            user.CanSendData = false;
            RemoveUserFromChannel(user);
            SendUserLeftServer(user);

            List<Channel> chans = Channels.Where(c => c.Owner == user).ToList();
            foreach (Channel chan in chans)
            {
                PromoteNewUserToOwner(chan);
            }

            user.SendBufferNow();
            user.Socket.Client.Close(Config.Network.ClientCloseWait);
        }

        /// <summary>
        /// Sends an error message to the user, then disconnects them.
        /// </summary>
        /// <param name="user">The user to disconnect</param>
        /// <param name="message">The error message</param>
        protected void DisconnectUser(User user, string message)
        {
            SendServerError(user, message);
            DisconnectUser(user);
        }

        /// <summary>
        /// Returns a single user matching username. Only searches users that are online.
        /// Returns null if user was not found.
        /// </summary>
        /// <param name="username">Username to find</param>
        protected User GetUserByName(string username)
        {
            User ret = Users.Find(u =>
                u != null
                && u.IsOnline == true
                && u.Username != null
                && u.Username.ToLower() == username.ToLower()
                );
            return ret;
        }

        /// <summary>
        /// Returns a list of users matching username. Only searches users that are online.
        /// Accepts wildcards. DOES NOT CHECK IF USER HAS PERMISSION TO USE WILDCARDS.
        /// Returns null if no users found.
        /// </summary>
        /// <param name="username">Username to find</param>
        /// <param name="limitChannel">Limit results to this channel. To search all channels, set to null</param>
        protected List<User> GetUsersByName(string username, Channel limitChannel)
        {
            System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex(ConvertStringToRegexSafe(username));
            List<User> ret = Users.Where(u =>
                u != null
                && u.IsOnline == true
                && u.Username != null
                && rx.IsMatch(u.Username)
                && (limitChannel == null ? true : u.Channel == limitChannel)
                ).ToList();
            return ret;
        }

        /// <summary>
        /// Returns a list of users matching username. Only searches users that are online.
        /// Accepts wildcards and performs a check whether or not the user can use wildcards.
        /// Limits channel to the users channel unless the @ flag is specified.
        /// Returns null if no users found.
        /// </summary>
        /// <param name="user">Calling user</param>
        /// <param name="username">Username to find</param>
        protected List<User> GetUsersByName(User user, string username)
        {
            Channel targetChan = user.Channel;
            if (username.Contains('*'))
            {
                if (UserIsStaff(user) == false)
                {
                    SendServerError(user, "You do not have permission to use the * flag in usernames.");
                    return null;
                }
            }
            if (username.Contains('@'))
            {
                if (UserIsStaff(user) == false)
                {
                    SendServerError(user, "You do not have permission to use the @ flag in usernames.");
                    return null;
                }
                string channel = username.Substring(username.IndexOf('@') + 1);
                Channel chan = null;
                if (channel == "*")
                {
                    if (!UserHasFlags(user, UserFlags.Admin))
                    {
                        SendServerError(user, "You do not have permission to use * as the channel name.");
                        return null;
                    }
                }
                else
                {
                    chan = GetChannelByName(user, channel, false);
                    if (chan == null)
                    {
                        SendServerError(user, "The channel " + channel + " was not found.");
                        return null;
                    }
                }
                targetChan = chan;
                username = username.Substring(0, username.IndexOf('@'));
            }

            List<User> ret = GetUsersByName(username, targetChan);
            if (ret.Count == 0)
            {
                SendServerError(user, "There is no user by the name \"" + username + "\" online.");
                return null;
            }

            for (int i = ret.Count - 1; i >= 0; i--)
            {
                if (CanUserSeeUser(user, ret[i]) == false)
                    ret.RemoveAt(i);
            }
            return ret;
        }

        /// <summary>
        /// Returns a string safe for regular expressions.
        /// </summary>
        /// <param name="str">The string to convert</param>
        protected string ConvertStringToRegexSafe(string str)
        {
            const string metaChars = @"\|()[{^$*+?.<>";
            StringBuilder sb = new StringBuilder(250);
            sb.Append("^(?i)");
            foreach (char c in str)
            {
                if (c == '*')
                    sb.Append(".*");
                else if (metaChars.Contains(c))
                {
                    sb.Append(@"\");
                    sb.Append(c);
                }
                else
                    sb.Append(c);
            }
            sb.Append('$');
            return sb.ToString();
        }


        /// <summary>
        /// Returns true if the user has the specified flags.
        /// </summary>
        /// <param name="user">The user to check</param>
        /// <param name="flags">The flags to check for</param>
        protected bool UserHasFlags(User user, UserFlags flags)
        {
            return (user.Flags & flags) == flags;
        }

        /// <summary>
        /// Removes flags from a user
        /// </summary>
        /// <param name="user">The user to remove flags from</param>
        /// <param name="flags">The flags to remove</param>
        protected void RemoveFlagsFromUser(User user, UserFlags flags)
        {
            if (UserHasFlags(user, flags))
                user.Flags ^= flags;
        }

        /// <summary>
        /// Returns true if user is a Moderator or higher
        /// </summary>
        /// <param name="user">The user to check</param>
        protected bool UserIsStaff(User user)
        {
            return (UserHasFlags(user, UserFlags.Admin) || UserHasFlags(user, UserFlags.Moderator));
        }

        /// <summary>
        /// Returns true if the user has the specified rank or higher
        /// </summary>
        /// <param name="user">The user to check</param>
        /// <param name="flags">The minimum flags the user must have</param>
        protected bool UserHasRankOrHigher(User user, UserFlags flags)
        {
            if (flags == UserFlags.Admin)
                return UserHasFlags(user, UserFlags.Admin);
            if (flags == UserFlags.Moderator)
                return (UserHasFlags(user, UserFlags.Admin) || UserHasFlags(user, UserFlags.Moderator));
            if (flags == UserFlags.Operator)
                return (UserHasFlags(user, UserFlags.Admin) || UserHasFlags(user, UserFlags.Moderator) || UserHasFlags(user, UserFlags.Operator));
            return true; //everyone else is normal
        }

        /// <summary>
        /// Joins a user to a channel. Will send the user left event to current channel if intenal checks pass.
        /// </summary>
        /// <param name="user">The user to join</param>
        /// <param name="channel">The channel to join the user to</param>
        protected void JoinUserToChannel(User user, Channel channel)
        {
            JoinUserToChannel(user, channel, false);
        }

        /// <summary>
        /// Joins a user to a channel. Will send the user left event to current channel if intenal checks pass.
        /// </summary>
        /// <param name="user">The user to join</param>
        /// <param name="channel">The channel to join the user to</param>
        /// <param name="ForceJoin">If true, will ignore all checks and foribly join the user</param>
        protected void JoinUserToChannel(User user, Channel channel, bool ForceJoin)
        {
            if (channel.IsUserBanned(user))
            {
                SendServerError(user, "You are banned from that channel.");
                return;
            }
            if (ForceJoin == false)
            { //all restrictive stuff goes in here
                if (user.Channel == channel)
                    return; //dont allow them to join their channel again
                if (channel == Channel_Admin && UserIsStaff(user) == false)
                {
                    SendServerError(user, "Only staff can enter that channel.");
                    return;
                }
            }

            ConsoleSendUserLeftChannel(user);

            RemoveUserFromChannel(user);
            channel.AddUser(user);

            ConsoleSendUserJoinChannel(user);

            SendUserJoinedChannel(user);
            SendJoinedChannelSuccessfully(user);
            SendList(user, ListType.UsersInChannel);

            if (ChannelHasFlags(channel, ChannelFlags.Administrative))
                SendServerInfo(user, "Only Moderators and Admin can hear you in this channel.");
            else if (ChannelHasFlags(channel, ChannelFlags.Silent))
                SendServerInfo(user, "This channel does not have any chat privileges.");
        }

        /// <summary>
        /// Removes the user from a channel. YOU SHOULD NEVER NEED TO CALL THIS FUNCTION.
        /// </summary>
        /// <param name="user">The target user</param>
        protected void RemoveUserFromChannel(User user)
        {
            Channel chan = null;
            if ((chan = user.Channel) == null)
                return;
            SendUserLeftChannel(user);
            chan.RemoveUser(user);

            AttemptToCloseChannel(chan);
        }

        /// <summary>
        /// Returns a list of all online users.
        /// </summary>
        protected List<User> GetAllOnlineUsers()
        {
            List<User> ret = Users.Where(u => u.IsOnline == true).ToList();
            if (ret == null)
                ret = new List<User>();
            return ret;
        }

        /// <summary>
        /// Returns a list of all offline users.
        /// Note that the server does not store offline users for very long!
        /// </summary>
        protected List<User> GetAllOfflineUsers()
        {
            List<User> ret = Users.Where(u => u.IsOnline == false).ToList();
            if (ret == null)
                ret = new List<User>();
            return ret;
        }

        /// <summary>
        /// Returns a list of users that have the specified IP address.
        /// </summary>
        /// <param name="IPAddress">The IP address</param>
        protected List<User> GetUsersByIP(string IPAddress)
        {
            List<User> ret = Users.Where(u => u.IPAddress == IPAddress).ToList();
            if (ret == null)
                ret = new List<User>();
            return ret;
        }

        /// <summary>
        /// Returns a list of users banned from a channel, from a users perspective.
        /// </summary>
        /// <param name="userPerspective">The user perspective to use</param>
        /// <param name="channel">The channel to get the banned list from</param>
        protected List<User> GetUsersBannedFromChannel(User userPerspective, Channel channel)
        {
            List<User> ret = new List<User>();
            foreach (string ip in channel.BannedIPs)
                foreach (User tmp in GetUsersByIP(ip))
                    if (CanUserSeeUser(userPerspective, tmp) == true)
                        ret.Add(tmp);
            User usr;
            foreach (string name in channel.BannedUsers)
            {
                usr = GetUserByName(name);
                if (usr != null && !ret.Contains(usr))
                    if (CanUserSeeUser(userPerspective, usr) == true)
                        ret.Add(usr);
            }
            return ret;
        }

        /// <summary>
        /// Returns true if a user can see another user.
        /// </summary>
        /// <param name="user">The calling user</param>
        /// <param name="targetUser">The user to test against</param>
        protected bool CanUserSeeUser(User user, User targetUser)
        {
            if (targetUser == null)
                return false; //cant see people who arent Users!

            if (user == console)
                return true; //console can see all
            if (targetUser == console)
                return false; //no one can see console (not even admins)

            if (user == targetUser)
                return true; //user can always see themselves

            if (UserHasFlags(user, UserFlags.Admin))
                return true; //admin can see all else

            if (UserHasFlags(targetUser, UserFlags.Admin) && UserHasFlags(targetUser, UserFlags.Invisible))
                    return false; //all else cant see invis admin
            
            if (UserHasFlags(user, UserFlags.Moderator))
                return true; //moderator can see all else

            if (UserHasFlags(targetUser, UserFlags.Moderator) && UserHasFlags(targetUser, UserFlags.Invisible))
                return false; //all else cant see invis moderator

            if (UserHasFlags(targetUser, UserFlags.Invisible))
                return false; //all else cant see any invisible user
            
            return true;
        }

        /// <summary>
        /// Returns true if a user has moderation rights over another user.
        /// </summary>
        /// <param name="user">The calling user</param>
        /// <param name="targetUser">The user to test against</param>
        protected bool CanUserModerateUser(User user, User targetUser)
        {
            if (targetUser == null)
                return false; //cant moderate people who arent Users!

            if (user == console) return true; //console can do all
            if (targetUser == console) return false; //no one can do anything to console

            if (UserHasFlags(targetUser, UserFlags.Admin)) return false; //this level and below cant touch admins
            if (UserHasFlags(user, UserFlags.Admin)) return true; //admin can do all


            if (UserHasFlags(targetUser, UserFlags.Moderator)) return false; //this level and below cant touch moderators
            if (UserHasFlags(user, UserFlags.Moderator)) return true;


            if (UserHasFlags(user, UserFlags.Operator))
            {
                if (UserHasFlags(targetUser, UserFlags.Operator) && user.Channel == targetUser.Channel)
                    return false; //operator cant touch operator in same channel
                return true;
            }

            return false; //there will be no touching
        }

    }
}
