using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected void HandleCommand(User user, string cmd)
        {
            string[] aryCmd = cmd.ToString().Split(' ');
            string cmdRest = cmd.Substring(aryCmd[0].Length).TrimStart();
            string cmd1 = aryCmd[0].ToLower();
            List<string> msgs;
            Channel channel;
            User targetUser;

            switch (cmd1)
            {
                case "admin":
                    user.Flags |= UserFlags.Admin;
                    SendServerInfo(user, "You have become an admin.");
                    break;
                case "users":
                    SendList(user, ListType.UsersOnServer);
                    break;

                case "join":
                case "j":
                    if ((channel = ExtractChannelFromParameterOne(user, ref aryCmd, true, "You must specify a channel.")) == null) return;

                    if (channel.IsUserBanned(user))
                        SendServerError(user, "You are banned from that channel.");
                    else
                        JoinUserToChannel(user, channel);

                    break;

                case "w":
                case "whisper":
                    int LenToMsg = aryCmd[0].Length + aryCmd[1].Length;
                    string GetToWhisper = aryCmd[1];
                    string GetWhisper = cmd.Substring(LenToMsg + 2);

                    User toUser = GetUserByName(GetToWhisper);

                    if (toUser != null)
                        if (toUser.IsOnline)
                        {
                            if (GetWhisper != string.Empty)
                            {
                                System.Windows.Forms.MessageBox.Show("Sending to " + GetToWhisper + ": " + GetWhisper);

                                SendUserWhisperTo(user, toUser, GetWhisper);
                                SendUserWhisperFrom(toUser, user, GetWhisper);
                            }
                            else
                                SendServerError(user, "What do you want to say?");
                        }
                        else
                            SendServerError(user, "An unexpected error occured, and the whisper was not sent.");
                    else
                        SendServerError(user, "That user is not online.");

                    break;
                case "me":
                case "em":
                case "emote":
                    if (cmdRest == string.Empty)
                        return;

                    foreach(User cu in GetUsersInChannel(user.Channel))
                        cu.Packet.Clear().InsertByte((byte)ChatEventType.UserEmote)
                                        .InsertDWord(cu.Ping)
                                        .InsertByte((byte)cu.Flags)
                                        .InsertStringNT(cu.Username)
                                        .InsertStringNT(cmdRest)
                                        .Send(VNET_CHATEVENT);

                    break;

                case "who":
                    if ((channel = ExtractChannelFromParameterOne(user, ref aryCmd, false, "You must specify a channel.")) == null) return;

                    List<User> u = GetUsersInChannel(user, channel, false);
                    if (u.Count == 0)
                        SendServerError(user, "That channel doesn't exist.");
                    else
                    {
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
                    }

                    break;

                case "ban":
                case "banip":
                case "ipban":
                    if (RequireOperator(user) == false) return;
                    if ((targetUser = ExtractUserFromParameterOne(user, ref aryCmd, "You must specify a user to ban.")) == null) return;
                    if (RequireModerationRights(user, targetUser) == false) return;

                    if (cmd1 == "ban")
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
                    if ((targetUser = ExtractUserFromParameterOne(user, ref aryCmd, "You must specify a user to unban.")) == null) return;
                    if (RequireModerationRights(user, targetUser) == false) return;


                    if (cmd1 == "unban")
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
                    if ((targetUser = ExtractUserFromParameterOne(user, ref aryCmd, "You must specify a user to promote to Operator.")) == null) return;
                    if (RequireModerationRights(user, targetUser) == false) return;

                    if (user.Channel == targetUser.Channel)
                    {
                        if (user.Flags == UserFlags.Operator)
                            if (user.Channel.CountOperators() == 1)
                            {
                                user.Flags ^= UserFlags.Operator;
                                targetUser.Flags |= UserFlags.Operator;

                                SendServerInfo(user, "You have given up ops to " + cmdRest);
                                SendServerInfo(targetUser, "You have been given ops by " + user.Username);
                                foreach (User cu in GetUsersInChannel(user.Channel))
                                    SendList(user, ListType.UsersFlagsUpdate);
                            }
                            else
                                SendServerError(user, "There is more than one operator in the channel.");
                        else
                            targetUser.Flags |= UserFlags.Operator;

                        foreach (User cu in GetUsersInChannel(user.Channel))
                            SendList(user, ListType.UsersFlagsUpdate);
                    }
                    else
                        SendServerError(user, "That user is not in the same channel as you.");

                    break;

                default:
                    SendServerError(user, "That is not a valid command.");
                    break;
            }
        }

        protected User ExtractUserFromParameterOne(User user, ref string[] str, string failMsgTooShort)
        {
            if (RequireParameterOne(user, ref str, failMsgTooShort) == false)
                return null;

            User ret = GetUserByName(str[1]);
            if (ret == null)
                SendServerError(user, "There is no user by the name \"" + str[1] + "\" online.");

            return ret;
        }

        protected Channel ExtractChannelFromParameterOne(User user, ref string[] str, bool allowCreation, string failMsgTooShort)
        {
            if (RequireParameterOne(user, ref str, failMsgTooShort) == false)
                return null;

            string cmd = String.Join(" ", str);
            cmd = cmd.Substring(cmd.IndexOf(' ') + 1);

            Channel ret = GetChannelByName(user, cmd, allowCreation);
            if (ret == null)
                SendServerError(user, "That channel does not exist.");

            return ret;
        }

        protected bool RequireParameterOne(User user, ref string[] str, string failMsgTooShort)
        {
            if (str.Length >= 2 && str[1].Length > 0)
                return true;

            SendServerError(user, failMsgTooShort);
            return false;
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
