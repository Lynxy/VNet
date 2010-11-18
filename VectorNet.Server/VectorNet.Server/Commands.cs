using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected void InitCommandTables()
        {
            cmdTable = new CommandTable();
            cmdTable.Add("commands", "cmds", CommandType.General, UserFlags.Normal, "Returns a list of all the commands available to you", null, new Action<User, string>(cmd_Commands));
            cmdTable.Add("admin", CommandType.General, UserFlags.Normal, "", null, new Action<User, string>(cmd_Admin));
            cmdTable.Add("mod", "moderator", CommandType.General, UserFlags.Normal, "", null, new Action<User, string>(cmd_Mod));
            cmdTable.Add("ban", CommandType.Moderation, UserFlags.Operator, "Bans a user from the channel", null, new Action<User, List<User>, string>(cmd_Ban));
            cmdTable.Add("join", "j", CommandType.General, UserFlags.Normal, "Joins a channel", null, new Action<User, string>(cmd_Join));
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

        protected void cmd_Ban(User user, List<User> users, string reason)
        {
            foreach (User u in users)
                BanUserByUsername(user, u, user.Channel);
        }

        protected void cmd_Join(User user, string rest)
        {
            Channel chan = GetChannelByName(user, (string)rest, true);
            JoinUserToChannel(user, chan);
        }


    }
}
