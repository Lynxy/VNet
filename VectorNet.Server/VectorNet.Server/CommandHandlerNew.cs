using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorNet.Server
{
    public partial class Server
    {
        CommandTable cmdtbl_Main;

        protected void InitCommandTables()
        {
            cmdtbl_Main = new CommandTable();
            cmdtbl_Main.Add("admin", CommandType.General, UserFlags.Normal, "", null, cmd_Admin, 0, null);
            cmdtbl_Main.Add("mod", "moderator", CommandType.General, UserFlags.Normal, "", null, cmd_Mod, 0, null);
            cmdtbl_Main.Add("ban", CommandType.Moderation, UserFlags.Operator, "Bans a user from the channel", null, cmd_Ban, 1, CommandParameter.Users);
            cmdtbl_Main.Add("join", "j", CommandType.General, UserFlags.Normal, "", null, cmd_Join, 1, CommandParameter.Rest);
            
        }



        protected void cmd_Admin(User user, object[] args)
        {
            user.Flags |= UserFlags.Admin;
            SendServerInfo(user, "You have become an admin.");
            SendList(user, ListType.UsersFlagsUpdate);
        }

        protected void cmd_Mod(User user, object[] args)
        {
            user.Flags |= UserFlags.Moderator;
            SendServerInfo(user, "You have become a moderator.");
            SendList(user, ListType.UsersFlagsUpdate);
        }

        protected void cmd_Ban(User user, object[] args)
        {
            List<User> users = (List<User>)args[0];
            SendServerInfo(user, "[ban] " + args);
        }

        protected void cmd_Join(User user, object[] args)
        {
            Channel chan = GetChannelByName(user, (string)args[0], true);
            JoinUserToChannel(user, chan);
        }














        protected void HandleCommandNew(User user, string command)
        {
            string[] cmds = command.Split(' ');
            int argIdx = 0;
            CommandData cmd = GetCommandData(user, cmdtbl_Main, ref cmds, ref argIdx);
            if (cmd == null)
            { //command not found
                SendServerError(user, "That is not a valid command. NEW");
                return;
            }

            if (cmd.ProcMethod == null)
            { //uh oh
                SendServerError(user, "Command has no internal route. Please notify admin.");
                return;
            }

            object[] specifiedParameters = new object[cmd.ParameterCount + 1];

            for (int i = 1; i <= cmd.ParameterCount; i++)
            {
                if (argIdx > cmds.Length)
                    break;

                if (i >= cmds.Length || cmds[i].Length == 0)
                {
                    SendServerError(user, "You must specify a " + cmd.ParameterTypes[i - 1] + ".");
                    return;
                }

                switch (cmd.ParameterTypes[i - 1])
                {
                    case CommandParameter.User:
                        User targetUser = GetUserByName(cmds[i]);
                        if (targetUser == null)
                        {
                            SendServerError(user, "There is no user by the name \"" + cmds[i] + "\" online.");
                            return;
                        }
                        if (!RequireModerationRights(user, targetUser, false))
                            break;
                        specifiedParameters[i - 1] = targetUser;
                        argIdx++;
                        break;

                    case CommandParameter.Users:
                        List<User> targetUsers = GetUsersFromArgument(user, cmds[i]);
                        if (targetUsers == null) return;
                        for (int j = targetUsers.Count - 1; j > 0; j--)
                            if (!RequireModerationRights(user, targetUsers[j], false))
                                targetUsers.RemoveAt(j);
                        if (targetUsers.Count == 0)
                        {
                            SendServerError(user, "There is no user by the name \"" + cmds[i] + "\" online.");
                            return;
                        }

                        specifiedParameters[i - 1] = targetUsers;
                        argIdx++;
                        break;

                    case CommandParameter.Numeric:
                        int num = 0;
                        if (!int.TryParse(cmds[i], out num))
                        {
                            SendServerError(user, "Expected a number for parameter " + i + ".");
                            return;
                        }
                        specifiedParameters[i - 1] = num;
                        argIdx++;
                        break;

                    case CommandParameter.Word:
                        specifiedParameters[i - 1] = cmds[i];
                        argIdx++;
                        break;

                    case CommandParameter.Rest:
                        specifiedParameters[i - 1] = String.Join(" ", cmds, argIdx + 1, cmds.Length - argIdx - 1);
                        argIdx += cmds.Length - argIdx - 1;
                        break;

                }
            }

            specifiedParameters[cmd.ParameterCount] = String.Join(" ", cmds, argIdx + 1, cmds.Length - argIdx - 1);
            cmd.ProcMethod(user, specifiedParameters);
        }

        protected CommandData GetCommandData(User user, CommandTable tbl, ref string[] args, ref int argIdx)
        {
            CommandData cmd = tbl[args[argIdx].ToLower()];
            if (cmd != null)
            {
                if (cmd.SubCommandTable != null)
                {
                    if (++argIdx < args.Length)
                    {
                        CommandData cmd2 = GetCommandData(user, cmd.SubCommandTable, ref args, ref argIdx);
                        if (cmd2 != null && UserHasRankOrHigher(user, cmd2.FlagsRequired))
                            return cmd2;
                    }
                    argIdx--;
                }
                if (UserHasRankOrHigher(user, cmd.FlagsRequired))
                    return cmd;
            }
            return null;
        }

        protected List<User> GetUsersFromArgument(User user, string username)
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






















        protected class CommandData
        {
            public string[] CommandName;
            public CommandType CommandType;
            public UserFlags FlagsRequired;
            public string Description;
            public CommandTable SubCommandTable;
            public Action<User, object[]> ProcMethod;
            public int ParameterCount;
            public CommandParameter[] ParameterTypes;
        }

        protected enum CommandType
        {
            General = 0,
            Moderation
        }

        protected enum CommandParameter
        {
            Rest = 0,
            Word,
            Numeric,
            User,
            Users
        }

        protected class CommandTable
        {
            protected Dictionary<string[], CommandData> _dict;

            public CommandTable()
            {
                _dict = new Dictionary<string[], CommandData>();
            }

            public CommandData this[string CommandName]
            {
                get
                {
                    var ret = _dict.Where(d => d.Key.Contains(CommandName));
                    if (ret.Count() == 0)
                        return null;
                    return ret.First().Value;
                }
            }

            public void Add(string _Name, CommandType _CommandType, UserFlags _FlagsRequired, string _Description, CommandTable _SubCommandTable, Action<User, object[]> _ProcMethod, int _ParameterCount, params CommandParameter[] _ParameterTypes)
            {
                Add(new string[] { _Name }, _CommandType, _FlagsRequired, _Description, _SubCommandTable, _ProcMethod, _ParameterCount, _ParameterTypes);
            }

            public void Add(string _Name, string _Alias, CommandType _CommandType, UserFlags _FlagsRequired, string _Description, CommandTable _SubCommandTable, Action<User, object[]> _ProcMethod, int _ParameterCount, params CommandParameter[] _ParameterTypes)
            {
                Add(new string[] { _Name, _Alias }, _CommandType, _FlagsRequired, _Description, _SubCommandTable, _ProcMethod, _ParameterCount, _ParameterTypes);
            }

            public void Add(string _Name, string _Alias1, string _Alias2, CommandType _CommandType, UserFlags _FlagsRequired, string _Description, CommandTable _SubCommandTable, Action<User, object[]> _ProcMethod, int _ParameterCount, params CommandParameter[] _ParameterTypes)
            {
                Add(new string[] { _Name, _Alias1, _Alias2 }, _CommandType, _FlagsRequired, _Description, _SubCommandTable, _ProcMethod, _ParameterCount, _ParameterTypes);
            }

            public void Add(string[] _Name, CommandType _CommandType, UserFlags _FlagsRequired, string _Description, CommandTable _SubCommandTable, Action<User, object[]> _ProcMethod, int _ParameterCount, params CommandParameter[] _ParameterTypes)
            {
                if (_ParameterCount > 0)
                    if (_ParameterTypes == null || _ParameterTypes.Length != _ParameterCount)
                        throw new ArgumentException("ParameterTypes[] must match length of ParameterCount");
                CommandData data = new CommandData { CommandName = _Name, CommandType = _CommandType, FlagsRequired = _FlagsRequired, Description = _Description, SubCommandTable = _SubCommandTable, ProcMethod = _ProcMethod, ParameterCount = _ParameterCount, ParameterTypes = _ParameterTypes };
                _dict.Add(_Name, data);
            }
        }



    }
}
