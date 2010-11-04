using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected void HandleCommand(User user, string command)
        {
            string[] aryCmd = command.Split(' ');
            string cmd = aryCmd[0].ToLower();
            string cmdRest = command.Substring(aryCmd[0].Length).TrimStart();
            aryCmd = cmdRest.Split(' ');
            List<string> msgs;
            Channel channel;
            User targetUser;

            switch (cmd)
            {
                case "admin":
                    user.Flags |= UserFlags.Admin;
                    SendServerInfo(user, "You have become an admin.");
                    break;

                case "invis":
                    user.Flags |= UserFlags.Invisible;
                    SendServerInfo(user, "You have become invisible.");
                    break;

                case "vis":
                    user.Flags ^= UserFlags.Invisible;
                    SendServerInfo(user, "You have become visible.");
                    break;

                case "stats":
                    SendServerInfo(user, "There are currently " + Channels.Count + " channels with " + Users.Count + " users total on VectorNet.");
                    break;

                case "users":
                    SendList(user, ListType.UsersOnServer);
                    break;

                case "join":
                case "j":
                    if ((channel = ExtractChannelFromText(user, ref cmdRest, true, "You must specify a channel.")) == null) return;
                    
                    if (channel.IsUserBanned(user))
                        SendServerError(user, "You are banned from that channel.");
                    else
                        JoinUserToChannel(user, channel);

                    break;

                case "w":
                case "whisper":
                    if ((targetUser = ExtractUserFromText(user, ref cmdRest, "You must specify a user to whisper.")) == null) return;
                    if (RequireParameter(user, ref cmdRest, "What do you want to say?") == false) return;

                    SendUserWhisperTo(user, targetUser, cmdRest);
                    SendUserWhisperFrom(targetUser, user, cmdRest);

                    break;
                case "me":
                case "em":
                case "emote":
                    if (RequireParameter(user, ref cmdRest, "What do you want to emote?") == false) return;
                    
                    foreach(User cu in GetUsersInChannel(user.Channel))
                        cu.Packet.Clear().InsertByte((byte)ChatEventType.UserEmote)
                            .InsertDWord(cu.Ping)
                            .InsertByte((byte)cu.Flags)
                            .InsertStringNT(cu.Username)
                            .InsertStringNT(cmdRest)
                            .Send(VNET_CHATEVENT);

                    break;

                case "who":
                    if ((channel = ExtractChannelFromText(user, ref cmdRest, false, "You must specify a channel.")) == null) return;

                    List<User> u = GetUsersInChannel(user, channel, false);
                    if (u.Count == 0)
                    { //channel may comtain invisible users
                        SendServerError(user, "That channel doesn't exist.");
                        return;
                    }

                    msgs = new List<string>();
                    msgs.Add("Users in channel " + channel.Name + ":");
                    for (int i = 0; i < u.Count; i++)
                    {
                        if (i % 2 == 0)
                            msgs.Add(u[i].Username);
                        else
                            msgs[msgs.Count - 1] += ", " + u[i].Username;
                    }
                    foreach (string msg in msgs)
                        SendServerInfo(user, msg);
                    msgs = null;

                    break;

                case "ban":
                case "banip":
                case "ipban":
                    if (RequireOperator(user) == false) return;
                    if ((targetUser = ExtractUserFromText(user, ref cmdRest, "You must specify a user to ban.")) == null) return;
                    if (RequireModerationRights(user, targetUser) == false) return;

                    if (cmd == "ban")
                    {
                        if (user.Channel.BannedUsers.Contains(targetUser.Username))
                            SendServerError(user, "That user is already banned from this channel.");
                        else
                            BanUserByUsername(user, targetUser, user.Channel);
                    }
                    else
                    {
                        if (user.Channel.BannedIPs.Contains(targetUser.IPAddress))
                            SendServerError(user, "That user's IP is already banned from this channel.");
                        else
                            BanUserByIP(user, targetUser, user.Channel);
                    }
                                    
                    break;

                case "unban":
                case "unipban":
                case "unbanip":
                    if (RequireOperator(user) == false) return;
                    if ((targetUser = ExtractUserFromText(user, ref cmdRest, "You must specify a user to unban.")) == null) return;
                    if (RequireModerationRights(user, targetUser) == false) return;

                    if (cmd == "unban")
                    {
                        if (!user.Channel.IsUserBanned(targetUser))
                            SendServerError(user, "That user is not banned from this channel.");
                        else
                            UnbanUser(user, targetUser, user.Channel, false);
                    }
                    else
                    {
                        if (!user.Channel.IsUserBanned(targetUser))
                            SendServerError(user, "That user is not IP banned.");
                        else
                            UnbanUserByIP(user, targetUser, user.Channel);
                    }

                    break;

                case "op":
                    if (RequireOperator(user) == false) return;
                    if ((targetUser = ExtractUserFromText(user, ref cmdRest, "You must specify a user to promote to Operator.")) == null) return;
                    if (RequireModerationRights(user, targetUser) == false) return;

                    if (user.Channel != targetUser.Channel)
                    {
                        SendServerError(user, "That user is not in the same channel as you.");
                        return;
                    }

                    if (user.Flags == UserFlags.Operator)
                    { //if (user.Channel.CountOperators() == 1) //not used "There is more than one operator in the channel."
                        user.Flags ^= UserFlags.Operator;
                        targetUser.Flags |= UserFlags.Operator;

                        if (user.Channel.Owner == user)
                            user.Channel.Owner = targetUser;

                        SendServerInfo(user, "You have given up ops to " + targetUser.Username);
                        SendServerInfo(targetUser, "You have been given ops by " + user.Username);
                        foreach (User cu in GetUsersInChannel(user.Channel))
                            if (cu != user && cu != targetUser)
                                SendServerInfo(cu, user.Username + " has given Operator to " + targetUser.Username + ".");

                        SendList(user, ListType.UsersFlagsUpdate); //tell channel members to update flags for these people
                        SendList(targetUser, ListType.UsersFlagsUpdate);
                    }
                    else
                    {
                        targetUser.Flags |= UserFlags.Operator;
                        SendList(targetUser, ListType.UsersFlagsUpdate);
                    }

                    break;

                case "resign":
                    if (RequireOperator(user) == false) return;
                    user.Flags ^= UserFlags.Operator;
                    SendServerInfoToChannel(user.Channel, user.Username + " has resigned from Operator status.", user, "You have resigned from operator status.");
                    SendList(user, ListType.UsersFlagsUpdate);
                    break;

                default:
                    SendServerError(user, "That is not a valid command.");
                    break;
            }
        }

        

        protected bool RequireParameter(User user, ref string str, string failMsgTooShort)
        {
            if (str == null || str.Length == 0)
            {
                SendServerError(user, failMsgTooShort);
                return false;
            }
            return true;
        }

        protected User ExtractUserFromText(User user, ref string str, string failMsgTooShort)
        {
            if (RequireParameter(user, ref str, failMsgTooShort) == false) return null;

            string[] str2 = str.Split(new char[1] { ' ' }, 2);
            str = (str2.Length == 1 ? "" : str2[1]);
             
            User ret = GetUserByName(str2[0]);
            if (ret != null && CanUserSeeUser(user, ret) == false)
                ret = null;
            if (ret == null)
                SendServerError(user, "There is no user by the name \"" + str2[0] + "\" online.");
            return ret;
        }

        protected Channel ExtractChannelFromText(User user, ref string str, bool allowCreation, string failMsgTooShort)
        {
            if (RequireParameter(user, ref str, failMsgTooShort) == false) return null;

            Channel ret = GetChannelByName(user, str, allowCreation);
            if (ret == null)
                SendServerError(user, "That channel does not exist.");

            return ret;
        }





        protected string[] StringToArrayNoBlanks(ref string str)
        { //splits on ' ' and removes blank lines
            string[] str2 = str.Split(' ');
            string[] ret = new string[str2.Length];
            int k = 0;
            for (int i = 0; i < str2.Length; i++)
                if (str2[i].Length > 0)
                    ret[k++] = str2[i];
            Array.Resize(ref ret, k);
            return ret;
        }

        protected bool RequireAdmin(User user)
        {
            if (user.Flags == UserFlags.Admin)
                return true;
            SendServerError(user, "You must be an Admin to use that command.");
            return false;
        }

        protected bool RequireModerator(User user)
        {
            if (user.Flags == UserFlags.Admin
                || user.Flags == UserFlags.Moderator)
                return true;
            SendServerError(user, "You must be a Moderator or higher to use that command.");
            return false;
        }

        protected bool RequireOperator(User user)
        {
            if (user.Flags == UserFlags.Admin
                || user.Flags == UserFlags.Moderator
                || user.Flags == UserFlags.Operator)
                return true;
            SendServerError(user, "You must be an Operator or higher to use that command.");
            return false;
        }

        protected bool RequireModerationRights(User user, User targetUser)
        {
            if (user == targetUser)
            {
                SendServerError(user, "You cannot perform moderation actions on yourself!");
                return true; //TODO: true for debugging
            }
            if (CanUserModerateUser(user, targetUser))
                return true;
            SendServerError(user, "You do not have sufficient rights to performs actions on that user.");
            return false;
        }

        public void HandleConsoleCommand(string cmd)
        {
            HandleCommand(console, cmd);
        }

        protected bool ContainsNonPrintable(string str)
        {
            foreach (char c in str)
                if ((byte)c < 32)
                    return true;
            return false;
        }

    }
}
