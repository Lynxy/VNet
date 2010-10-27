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
            user.Socket.Close();
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
            RemoveUserFromChannel(user);
            channel.AddUser(user, false);

            ConsoleSendUserJoinedChannel(user);
            SendUserJoinedChannel(user);
            SendJoinedChannelSuccessfully(user);
            SendList(user, ListType.UsersInChannel);
        }

        protected void RemoveUserFromChannel(User user)
        {
            if (user.Channel == null)
                return;
            SendUserLeftChannel(user);
            user.Channel.RemoveUser(user);
            AttemptToCloseChannel(user.Channel);
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
    }
}
