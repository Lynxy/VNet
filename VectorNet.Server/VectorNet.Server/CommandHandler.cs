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
            string text;
            List<string> msgs;
            Channel channel;
            User targetUser;

            switch (aryCmd[0].ToLower())
            {
                case "join":
                case "j":
                    if (aryCmd.Length < 2 || aryCmd[1].Length == 0)
                        SendServerError(user, "You must specify a channel.");
                    else
                    {
                        text = cmd.Substring(cmd.IndexOf(' ') + 1);
                        channel = GetChannelByName(text, true);
                        if (channel.IsUserBanned(user))
                            SendServerError(user, "You are banned from that channel.");
                        else
                            JoinUserToChannel(user, channel);
                    }
                    break;

                case "me":
                case "em":
                case "emote":

                    break;

                case "who":
                    if (aryCmd.Length < 2 || aryCmd[1].Length == 0)
                        SendServerError(user, "You must specify a channel.");
                    else
                    {
                        text = cmd.Substring(cmd.IndexOf(' ') + 1);

                        channel = GetChannelByName(text, false);
                        if (channel == null)
                            SendServerError(user, "That channel doesn't exist.");
                        else
                        {
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
                        }
                    }
                    break;

                case "ban":
                    if (RequireOperator(user) == false) return;
                    if ((targetUser = ExtractUserFromParameterOne(user, ref aryCmd, "You must specify a user to ban.")) == null) return;
                    if (RequireModerationRights(user, targetUser) == false) return;

                    if (user.Channel.BannedUsers.Contains(targetUser.Username))
                        SendServerError(user, "That user is already banned from this channel.");
                    else
                        BanUserByUsername(user, targetUser, user.Channel);

                    break;

                case "banip":
                case "ipban":
                    if (user.Flags == UserFlags.Admin)
                    {
                        if (aryCmd.Length < 2 || aryCmd[1].Length == 0)
                            SendServerError(user, "You must specify a user to IP ban.");
                        else
                        {
                            targetUser = GetUserByName(aryCmd[1]);
                            if (targetUser == null)
                                SendServerError(user, "There is no user by the name " + aryCmd[1] + " online.");
                            else
                            {
                                if (targetUser.Flags == UserFlags.Normal)
                                {
                                    if (user.Channel.BannedIPs.Contains(targetUser.IPAddress))
                                        SendServerError(user, "That user's IP is already banned from this channel.");
                                    else
                                        BanUserByIP(user, targetUser, user.Channel);
                                }
                                else
                                    SendServerError(user, "You cannot IP that user.");
                            }
                        }
                    }
                    break;

                case "unipban":
                case "unbanip":
                    if (user.Flags == UserFlags.Admin)
                    {
                        if (aryCmd.Length < 2 || aryCmd[1].Length == 0)
                            SendServerError(user, "You must specify a user to Un-IPBan.");
                        else
                        {
                            targetUser = GetUserByName(aryCmd[1]);

                            if (targetUser == null)
                                SendServerError(user, "There is no user named " + aryCmd[1] + " online.");
                            else
                                if (!user.Channel.BannedIPs.Contains(aryCmd[1]))
                                    SendServerError(user, "That user is not IP banned.");
                                else
                                    UnbanUserByIP(user, targetUser, user.Channel);
                        }
                    }
                    break;
                case "unban":
                    if (user.Flags == UserFlags.Admin || user.Flags == UserFlags.Moderator || user.Flags == UserFlags.Operator)
                    {
                        if (aryCmd.Length < 2 || aryCmd[1].Length == 0)
                            SendServerError(user, "You must specify a user to unban.");
                        else
                        {
                            targetUser = GetUserByName(aryCmd[1]);
                            if (targetUser == null)
                                SendServerError(user, "There is no user by the name " + aryCmd[1] + " online.");
                            else
                            {
                                if (!user.Channel.IsUserBanned(targetUser))
                                    SendServerError(user, "That user is not banned from this channel.");
                                else
                                    UnbanUser(user, targetUser, user.Channel, false);
                            }
                        }
                    }
                    break;

                case "users":
                    SendList(user, ListType.UsersOnServer);
                    break;

                case "op":
                    if (user.Flags == UserFlags.Admin || user.Flags == UserFlags.Moderator || user.Flags == UserFlags.Operator)
                    {
                        targetUser = GetUserByName(aryCmd[1]);

                        if (user.Channel == targetUser.Channel)
                        { }
                        //TODO: Determine environment for opping users
                        else
                            SendServerError(user, "That user is not in the same channel as you.");
                    }

                    break;

                default:
                    SendServerError(user, "That is not a valid command.");
                    break;
            }
        }

        protected User ExtractUserFromParameterOne(User user, ref string[] str, string failMsgTooShort)
        {
            if (str.Length < 2 && str[1].Length == 0)
            {
                SendServerError(user, failMsgTooShort);
                return null;
            }

            User ret = GetUserByName(str[1]);
            if (ret == null)
                SendServerError(user, "There is no user by the name \"" + str[1] + "\" online.");

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
