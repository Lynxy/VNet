﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorNet.Server
{
    public partial class Server
    {
        CommandTable cmdTable;

        /// <summary>
        /// Handles a command by the console
        /// </summary>
        /// <param name="cmd">Command text</param>
        public void HandleConsoleCommand(string cmd)
        {
            HandleCommand(console, cmd);
        }

        /// <summary>
        /// Handles a command by a user. Browses the command tables to find command and checks permissions to call the command.
        /// </summary>
        /// <param name="user">Calling user</param>
        /// <param name="command">Command text</param>
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
                    List<User> targetUsers = GetUsersByName(user, cmds[i]);
                    if (targetUsers == null) return;
                    if (CheckForMultipleUsersInSingleUserQuery(user, targetUsers)) return;
                    for (int j = targetUsers.Count - 1; j >= 0; j--)
                        if (cmd.CommandType == CommandType.Moderation && !RequireModerationRights(user, targetUsers[j], false))
                            targetUsers.RemoveAt(j);
                    if (targetUsers.Count == 0) return;


                    User targetUser = targetUsers[0];
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
                    List<User> targetUsers = GetUsersByName(user, cmds[i]);
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

        /// <summary>
        /// Returns true and sends an error to the user if the list contains more than 1 user.
        /// </summary>
        /// <param name="user">The calling user</param>
        /// <param name="users">The list of users to check</param>
        protected bool CheckForMultipleUsersInSingleUserQuery(User user, List<User> users)
        {
            if (users.Count > 1)
            {
                SendServerError(user, "Your user search returned multiple users. This parameter only accepts one user.");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true and sends an error to the user if the text is empty.
        /// </summary>
        /// <param name="user">The calling user</param>
        /// <param name="text">The check to check</param>
        /// <param name="errorMsg">The error to send if text is empty</param>
        /// <returns></returns>
        protected bool CheckIfParameterIsEmpty(User user, ref string text, string errorMsg)
        {
            if (text.Trim().Length == 0)
            {
                SendServerError(user, errorMsg);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Recursively explores children of a command table to return a command that a user can use with their permission.
        /// Returns null if no command was found.
        /// </summary>
        /// <param name="user">The calling user</param>
        /// <param name="tbl">The parent command table to use</param>
        /// <param name="args">An array containing the issued command</param>
        /// <param name="argIdx">The index of the current position in args[]</param>
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

        /// <summary>
        /// Returns true if a user can moderate another user, otherwise sends them a error informing them they can't moderate the user.
        /// </summary>
        /// <param name="user">The calling user</param>
        /// <param name="targetUser">The user to check moderation against</param>
        /// <param name="CanTargetSelf">Whether or not moderation can be applied to yourself</param>
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

        /// <summary>
        /// Returns true if the strings contains non-printable characters
        /// </summary>
        /// <param name="str">The test string</param>
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
