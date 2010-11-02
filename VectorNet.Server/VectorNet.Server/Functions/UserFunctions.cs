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
            user.SendBufferNow();
            user.IsOnline = false;
            user.CanSendData = false;
            RemoveUserFromChannel(user);
            SendUserLeftServer(user);

            List<Channel> chans = Channels.Where(c => c.Owner == user).ToList();
            foreach (Channel chan in chans)
            {
                User newOwner = chan.PromoteNewOwner();
                SendList(newOwner, ListType.UsersFlagsUpdate);
            }

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

        protected void JoinUserToChannel(User user, Channel channel)
        {
            ConsoleSendUserLeftChannel(user);

            RemoveUserFromChannel(user);
            channel.AddUser(user);

            ConsoleSendUserJoinChannel(user);

            SendUserJoinedChannel(user);
            SendJoinedChannelSuccessfully(user);
            SendList(user, ListType.UsersInChannel);
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

        protected List<User> GetUsersBannedFromChannel(Channel channel)
        {
            List<User> ret = new List<User>();
            foreach (string ip in channel.BannedIPs)
                foreach (User tmp in GetUsersByIP(ip))
                    ret.Add(tmp);
            User usr;
            foreach (string name in channel.BannedUsers)
            {
                usr = GetUserByName(name);
                if (usr != null && !ret.Contains(usr))
                    ret.Add(usr);
            }
            return ret;
        }

        protected bool CanUserSeeUser(User user, User targetUser)
        {
            if (user == console)
                return true; //console can see all
            if (targetUser == console)
                return false; //no one can see console (not even admins)

            if (user == targetUser)
                return true; //user can always see themselves

            if (user.Flags == UserFlags.Admin)
                return true; //admin can see all else

            if (user.Flags == UserFlags.Moderator)
            {
                if (targetUser.Flags == UserFlags.Admin && targetUser.Flags == UserFlags.Invisible)
                    return false;
            }
            return true;
        }

        protected bool CanUserModerateUser(User user, User targetUser)
        {
            if (user == console) return true; //console can do all
            if (targetUser == console) return false; //no one can do anything to console


            if (targetUser.Flags == UserFlags.Admin) return false; //this level and below cant touch admins
            if (user.Flags == UserFlags.Admin) return true; //admin can do all


            if (targetUser.Flags == UserFlags.Moderator) return false; //this level and below cant touch moderators
            if (user.Flags == UserFlags.Moderator) return true;


            if (user.Flags == UserFlags.Operator)
            {
                if (targetUser.Flags == UserFlags.Operator && user.Channel == targetUser.Channel)
                    return false; //operator cant touch operator in same channel
                return true;
            }

            return false; //there will be no touching
        }

    }
}
