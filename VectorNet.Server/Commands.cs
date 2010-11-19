using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorNet.Server
{
    public partial class Server
    {
        /// <summary>
        /// Sets up the command tables
        /// </summary>
        protected void InitCommandTables()
        {
            cmdTable = new CommandTable();

            //help commands
            cmdTable.Add("commands", "cmds", CommandType.General, UserFlags.Normal, "Returns a list of all the commands available to you", null, new Action<User, string>(cmd_Commands));

            //testing commands (temp?)
            cmdTable.Add("admin", CommandType.General, UserFlags.Normal, "", null, new Action<User, string>(cmd_Admin));
            cmdTable.Add("mod", "moderator", CommandType.General, UserFlags.Normal, "", null, new Action<User, string>(cmd_Mod));

            //normal user
            cmdTable.Add("join", "j", CommandType.General, UserFlags.Normal, "Joins a channel", null, new Action<User, string>(cmd_Join));
            cmdTable.Add("stats", CommandType.General, UserFlags.Normal, "", null, new Action<User, string>(cmd_Stats));
            cmdTable.Add("users", CommandType.General, UserFlags.Normal, "", null, new Action<User, string>(cmd_Users));
            cmdTable.Add("whisper", "w", "message", "msg", "m", CommandType.General, UserFlags.Normal, "", null, new Action<User, List<User>, string>(cmd_Whisper));
            cmdTable.Add("say", "talk", "s", CommandType.General, UserFlags.Normal, "", null, new Action<User, string>(cmd_Talk));
            cmdTable.Add("emote", "me", "em", CommandType.General, UserFlags.Normal, "", null, new Action<User, string>(cmd_Emote));
            cmdTable.Add("who", CommandType.General, UserFlags.Normal, "", null, new Action<User, string>(cmd_Who));
            
            //operator
            cmdTable.Add("kick", CommandType.Moderation, UserFlags.Operator, "Kicks a user from the channel", null, new Action<User, List<User>, string>(cmd_Kick));
            cmdTable.Add("ban", CommandType.Moderation, UserFlags.Operator, "Bans a user from the channel", null, new Action<User, List<User>, string>(cmd_Ban));
            cmdTable.Add("ipban", "banip", CommandType.Moderation, UserFlags.Operator, "Bans a user's IP from the channel", null, new Action<User, List<User>, string>(cmd_IPBan));
            cmdTable.Add("unban", "unipban", "unbanip", CommandType.Moderation, UserFlags.Operator, "Unbans a user & their IP from the channel", null, new Action<User, List<User>, string>(cmd_Unban));
            cmdTable.Add("op", "operator", CommandType.Moderation, UserFlags.Operator, "Gives up your operator status to another user", null, new Action<User, List<User>, string>(cmd_Op));
            cmdTable.Add("resign", CommandType.General, UserFlags.Operator, "Gives up your operator status", null, new Action<User, string>(cmd_Resign));
            
            //moderator

            //admin
            cmdTable.Add("invisible", "invis", CommandType.General, UserFlags.Admin, "", null, new Action<User, string>(cmd_Invisible));
            cmdTable.Add("visible", "vis", CommandType.General, UserFlags.Admin, "", null, new Action<User, string>(cmd_Visible));
            cmdTable.Add("pop", "move", CommandType.General, UserFlags.Admin, "", null, new Action<User, List<User>, string>(cmd_Pop));

        }

        protected void cmd_Commands(User user, string rest)
        {
            SendServerInfo(user, "----------------------------------");
            SendServerInfo(user, "Commands available to you:");
            for (int i = 0; i < cmdTable.Count; i++)
            {
                if (UserHasRankOrHigher(user, cmdTable[i].FlagsRequired))
                {
                    string desc = cmdTable[i].Description;
                    if (desc == "")
                        SendServerInfo(user, "   /" + cmdTable[i].CommandName[0]);
                    else
                        SendServerInfo(user, "   /" + cmdTable[i].CommandName[0] + " - " + desc);
                }
            }
            SendServerInfo(user, "----------------------------------");
        }

        protected void cmd_Admin(User user, string rest)
        {
            user.Flags |= UserFlags.Admin;
            SendServerInfo(user, "You have become an admin.");
            SendList(user, ListType.UsersFlagsUpdate);
        }

        protected void cmd_Mod(User user, string rest)
        {
            user.Flags |= UserFlags.Moderator;
            SendServerInfo(user, "You have become a moderator.");
            SendList(user, ListType.UsersFlagsUpdate);
        }

        protected void cmd_Kick(User user, List<User> users, string reason)
        {
            foreach (User u in users)
                if (u.Channel == user.Channel)
                    KickUserFromChannel(user, u, reason);
                else
                    SendServerError(user, "That user is not present in the channel.");
        }

        protected void cmd_Ban(User user, List<User> users, string reason)
        {
            foreach (User u in users)
                if (user.Channel.BannedUsers.Contains(u.Username))
                    SendServerError(user, u.Username + " is already banned from this channel.");
                else
                    BanUserByUsername(user, u, user.Channel);
        }

        protected void cmd_IPBan(User user, List<User> users, string reason)
        {
            foreach (User u in users)
                if (user.Channel.BannedIPs.Contains(u.IPAddress))
                    SendServerError(user, u.Username + "'s IP is already banned from this channel.");
                else
                    BanUserByIP(user, u, user.Channel);
        }

        protected void cmd_Unban(User user, List<User> users, string rest)
        {
            foreach (User u in users)
                if (!user.Channel.IsUserBanned(u))
                    SendServerError(user, u.Username + " is not banned from this channel.");
                else
                    UnbanUser(user, u, user.Channel, false);
        }

        protected void cmd_Op(User user, List<User> users, string rest)
        {
            bool isModOrHigher = UserHasRankOrHigher(user, UserFlags.Moderator);
            if (!isModOrHigher && CheckForMultipleUsersInSingleUserQuery(user, users)) return;

            foreach (User targetUser in users)
            {
                if (user.Channel != targetUser.Channel)
                {
                    SendServerError(user, "That user is not in the same channel as you.");
                    return;
                }

                if (isModOrHigher)
                {
                    targetUser.Flags |= UserFlags.Operator;
                    SendList(targetUser, ListType.UsersFlagsUpdate);
                }
                else
                {
                    RemoveFlagsFromUser(user, UserFlags.Operator);
                    targetUser.Flags |= UserFlags.Operator;

                    if (user.Channel.Owner == user)
                        user.Channel.Owner = targetUser;

                    SendList(user, ListType.UsersFlagsUpdate); //tell channel members to update flags for these people
                    SendList(targetUser, ListType.UsersFlagsUpdate);
                }

                SendServerInfo(user, "You have given up ops to " + targetUser.Username);
                SendServerInfo(targetUser, "You have been given ops by " + user.Username);
                foreach (User cu in GetUsersInChannel(user.Channel))
                    if (cu != user && cu != targetUser)
                        SendServerInfo(cu, user.Username + " has given Operator to " + targetUser.Username + ".");
            }
        }

        protected void cmd_Resign(User user, string rest)
        {
            RemoveFlagsFromUser(user, UserFlags.Operator);
            SendServerInfoToChannel(user.Channel, user.Username + " has resigned from Operator status.", user, "You have resigned from operator status.");
            SendList(user, ListType.UsersFlagsUpdate);
        }

        protected void cmd_Join(User user, string rest)
        {
            if (CheckIfParameterIsEmpty(user, ref rest, "You must specify a channel.")) return;
            Channel chan = GetChannelByName(user, (string)rest, true);
            JoinUserToChannel(user, chan);
        }

        protected void cmd_Stats(User user, string rest)
        {
            SendServerInfo(user, "There are currently " + Channels.Count + " channels with " + Users.Count + " users total on VectorNet.");
        }

        protected void cmd_Users(User user, string rest)
        {
            SendList(user, ListType.UsersOnServer);
        }

        protected void cmd_Whisper(User user, List<User> targetUsers, string rest)
        {
            if (CheckIfParameterIsEmpty(user, ref rest, "What do you want to say?")) return;

            foreach (User targ in targetUsers)
            {
                SendUserWhisperTo(user, targ, rest);
                SendUserWhisperFrom(targ, user, rest);
            }
        }

        protected void cmd_Talk(User user, string rest)
        {
            if (CheckIfParameterIsEmpty(user, ref rest, "What do you want to say?")) return;

            SendUserTalkSingle(user, user, rest, false);
            UserTalk(user, rest);
        }

        protected void cmd_Emote(User user, string rest)
        {
            if (CheckIfParameterIsEmpty(user, ref rest, "What do you want to emote?")) return;

            foreach (User cu in GetUsersInChannel(user.Channel))
                cu.Packet.Clear().InsertByte((byte)ChatEventType.UserEmote)
                    .InsertDWord(cu.Ping)
                    .InsertByte((byte)cu.Flags)
                    .InsertStringNT(cu.Username)
                    .InsertStringNT(rest)
                    .Send(VNET_CHATEVENT);
        }

        protected void cmd_Who(User user, string rest)
        {
            if (CheckIfParameterIsEmpty(user, ref rest, "What channel do you want to check?")) return;

            Channel channel = GetChannelByName(user, rest, false);
            if (channel == null)
            {
                SendServerError(user, "That channel doesn't exist.");
                return;
            }

            List<User> u = GetUsersInChannel(user, channel, false);
            if (u.Count == 0)
            { //channel may comtain invisible users
                SendServerError(user, "That channel doesn't exist.");
                return;
            }

            List<string> msgs = new List<string>();
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

        protected void cmd_Invisible(User user, string rest)
        { //TODO: Allow making a target user invisible
            if (UserHasFlags(user, UserFlags.Invisible))
            {
                SendServerError(user, "You are already invisible.");
                return;
            }

            user.Flags |= UserFlags.Invisible;
            SendServerInfo(user, "You have become invisible.");

            foreach (User cu in GetUsersInChannel(user.Channel))
            {
                if (UserIsStaff(cu))
                    SendList(cu, ListType.UsersFlagsUpdate);
                else
                    SendUserLeftChannelSingle(cu, user);
            }
        }

        protected void cmd_Visible(User user, string rest)
        {
            if (!UserHasFlags(user, UserFlags.Invisible))
            {
                SendServerError(user, "You are not invisible.");
                return;
            }

            RemoveFlagsFromUser(user, UserFlags.Invisible);
            SendServerInfo(user, "You have become visible.");

            foreach (User cu in GetUsersInChannel(user.Channel))
                if (UserIsStaff(cu))
                    SendList(cu, ListType.UsersFlagsUpdate);
                else
                    SendUserJoinedChannelSingle(cu, user);
        }

        protected void cmd_Pop(User user, List<User> targetUsers, string rest)
        {
            if (CheckIfParameterIsEmpty(user, ref rest, "Where do you want to pop?")) return;

            Channel channel = GetChannelByName(user, rest, true);
            foreach (User cu in targetUsers)
            {
                SendServerInfo(user, "You have moved " + cu.Username + " to the channel " + channel.Name + ".");
                if (cu != user)
                    SendServerInfo(cu, "You have been moved to " + channel.Name + " by " + user.Username + ".");
                JoinUserToChannel(cu, channel, true);
            }
        }

    }
}
