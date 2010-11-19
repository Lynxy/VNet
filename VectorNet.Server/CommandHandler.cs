using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorNet.Server
{
    public partial class Server
    {
        CommandTable cmdTable;

        public void HandleConsoleCommand(string cmd)
        {
            HandleCommand(console, cmd);
        }

        protected void HandleCommand(User user, string command)
        {
            string[] cmds = command.Split(' ');
            int argIdx = 0;
            CommandData cmd = GetCommandData(user, cmdTable, ref cmds, ref argIdx);
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

            object[] specifiedParameters = new object[cmd.ParameterCount + 2];
            specifiedParameters[0] = user;

            for (int i = 1; i <= cmd.ParameterCount; i++)
            {
                if (argIdx > cmds.Length)
                    throw new IndexOutOfRangeException("argIdx is larger than cmds[]");

                Type paramType = cmd.ParameterInfo[i].ParameterType;

                if (i >= cmds.Length || cmds[i].Length == 0)
                {
                    string specType = "{unknown}";
                    if (paramType == typeof(User)) specType = "user";
                    if (paramType == typeof(List<User>)) specType = "user(s)";
                    if (paramType == typeof(int)) specType = "number";
                    if (paramType == typeof(string)) specType = "word";
                    SendServerError(user, "You must specify a " + specType + " for parameter " + i + ".");
                    return;
                }

                if (paramType == typeof(User))
                {
                    User targetUser = GetUserByName(cmds[i]);
                    if (targetUser == null)
                    {
                        SendServerError(user, "There is no user by the name \"" + cmds[i] + "\" online.");
                        return;
                    }
                    if (cmd.CommandType == CommandType.Moderation && !RequireModerationRights(user, targetUser, false))
                        return;

                    specifiedParameters[i] = targetUser;
                    argIdx++;
                }
                else if (paramType == typeof(List<User>))
                {
                    List<User> targetUsers = GetUsersFromArgument(user, cmds[i]);
                    if (targetUsers == null) return;
                    for (int j = targetUsers.Count - 1; j >= 0; j--)
                        if (cmd.CommandType == CommandType.Moderation && !RequireModerationRights(user, targetUsers[j], false))
                            targetUsers.RemoveAt(j);
                    if (targetUsers.Count == 0) return;

                    specifiedParameters[i] = targetUsers;
                    argIdx++;
                }
                else if (paramType == typeof(int))
                {
                    int num = 0;
                    if (!int.TryParse(cmds[i], out num))
                    {
                        SendServerError(user, "Expected a number for parameter " + i + ".");
                        return;
                    }
                    specifiedParameters[i] = num;
                    argIdx++;
                }
                else if (paramType == typeof(string))
                { //a single word
                    specifiedParameters[i] = cmds[i];
                    argIdx++;
                }
                else
                {
                    throw new NotSupportedException("The command method does not support this parameter type");
                }
            }

            specifiedParameters[cmd.ParameterCount + 1] = String.Join(" ", cmds, argIdx + 1, cmds.Length - argIdx - 1);
            cmd.ProcMethod.Invoke(this, specifiedParameters);
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

        protected bool RequireModerationRights(User user, User targetUser, bool CanTargetSelf)
        {
            if (user == targetUser && !CanTargetSelf)
            {
                SendServerError(user, "You cannot perform moderation actions on yourself!");
                return false;
            }
            if (CanUserModerateUser(user, targetUser))
                return true;
            SendServerError(user, "You do not have sufficient rights to performs actions on user \"" + targetUser.Username + "\".");
            return false;
        }

        protected bool ContainsNonPrintable(string str)
        {
            foreach (char c in str)
                if ((byte)c < 32)
                    return true;
            return false;
        }






















        protected class CommandData
        {
            public string[] CommandName;
            public CommandType CommandType;
            public UserFlags FlagsRequired;
            public string Description;
            public CommandTable SubCommandTable;
            public System.Reflection.MethodInfo ProcMethod;
            public System.Reflection.ParameterInfo[] ParameterInfo;
            public int ParameterCount;
        }

        protected enum CommandType
        {
            General = 0,
            Moderation
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

            public CommandData this[int index]
            {
                get
                {
                    if (index >= _dict.Count)
                        return null;
                    return _dict.Values.ElementAt(index);
                }
            }

            public int Count
            {
                get { return _dict.Count; }
            }

            public void Add(string _Name, CommandType _CommandType, UserFlags _FlagsRequired, string _Description, CommandTable _SubCommandTable, Delegate _ProcMethod)
            {
                Add(new string[] { _Name }, _CommandType, _FlagsRequired, _Description, _SubCommandTable, _ProcMethod);
            }

            public void Add(string _Name, string _Alias, CommandType _CommandType, UserFlags _FlagsRequired, string _Description, CommandTable _SubCommandTable, Delegate _ProcMethod)
            {
                Add(new string[] { _Name, _Alias }, _CommandType, _FlagsRequired, _Description, _SubCommandTable, _ProcMethod);
            }

            public void Add(string _Name, string _Alias1, string _Alias2, CommandType _CommandType, UserFlags _FlagsRequired, string _Description, CommandTable _SubCommandTable, Delegate _ProcMethod)
            {
                Add(new string[] { _Name, _Alias1, _Alias2 }, _CommandType, _FlagsRequired, _Description, _SubCommandTable, _ProcMethod);
            }

            public void Add(string _Name, string _Alias1, string _Alias2, string _Alias3, CommandType _CommandType, UserFlags _FlagsRequired, string _Description, CommandTable _SubCommandTable, Delegate _ProcMethod)
            {
                Add(new string[] { _Name, _Alias1, _Alias2, _Alias3 }, _CommandType, _FlagsRequired, _Description, _SubCommandTable, _ProcMethod);
            }

            public void Add(string _Name, string _Alias1, string _Alias2, string _Alias3, string _Alias4, CommandType _CommandType, UserFlags _FlagsRequired, string _Description, CommandTable _SubCommandTable, Delegate _ProcMethod)
            {
                Add(new string[] { _Name, _Alias1, _Alias2, _Alias3, _Alias4 }, _CommandType, _FlagsRequired, _Description, _SubCommandTable, _ProcMethod);
            }

            public void Add(string[] _Name, CommandType _CommandType, UserFlags _FlagsRequired, string _Description, CommandTable _SubCommandTable, Delegate _ProcMethod)
            {
                System.Reflection.MethodInfo mi = _ProcMethod.Method;
                System.Reflection.ParameterInfo[] pars = mi.GetParameters();
                CommandData data = new CommandData { CommandName = _Name, CommandType = _CommandType, FlagsRequired = _FlagsRequired, Description = _Description, SubCommandTable = _SubCommandTable, ProcMethod = mi, ParameterInfo = pars, ParameterCount = pars.Length - 2 };
                _dict.Add(_Name, data);
            }
        }



    }
}
