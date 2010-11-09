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
            user.Socket.Client.Close(Config.ClientCloseWait);
        }

        protected void DisconnectUser(User user, string message)
        {
            SendServerError(user, message);
            DisconnectUser(user);
        }

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

        protected bool UserHasFlags(User user, UserFlags flags)
        {
            return (user.Flags & flags) == flags;
        }

        protected void RemoveFlagsFromUser(User user, UserFlags flags)
        {
            if (UserHasFlags(user, flags))
                user.Flags ^= flags;
        }

        protected bool UserIsStaff(User user)
        {
            return (UserHasFlags(user, UserFlags.Admin) || UserHasFlags(user, UserFlags.Moderator));
        }

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

        protected void JoinUserToChannel(User user, Channel channel)
        {
            JoinUserToChannel(user, channel, false);
        }

        protected void JoinUserToChannel(User user, Channel channel, bool ForceJoin)
        {
            if (channel.IsUserBanned(user))
            {
                SendServerError(user, "You are banned from that channel.");
                return;
            }
            if (channel == Channel_Admin)
            {
                if (UserIsStaff(user) == false && !ForceJoin)
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

        protected void RemoveUserFromChannel(User user)
        {
            Channel chan = null;
            if ((chan = user.Channel) == null)
                return;
            SendUserLeftChannel(user);
            chan.RemoveUser(user);

            AttemptToCloseChannel(chan);
        }

        protected List<User> GetAllOnlineUsers()
        {
            List<User> ret = Users.Where(u => u.IsOnline == true).ToList();
            if (ret == null)
                ret = new List<User>();
            return ret;
        }

        protected List<User> GetAllOfflineUsers()
        {
            List<User> ret = Users.Where(u => u.IsOnline == false).ToList();
            if (ret == null)
                ret = new List<User>();
            return ret;
        }

        protected List<User> GetUsersByIP(string IPAddress)
        {
            List<User> ret = Users.Where(u => u.IPAddress == IPAddress).ToList();
            if (ret == null)
                ret = new List<User>();
            return ret;
        }

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
