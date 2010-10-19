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
                    if (aryCmd.Length < 2)
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

                case "who":
                    if (aryCmd.Length < 2)
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
                    //TODO: Check permissions to ban
                    if (aryCmd.Length < 2)
                        SendServerError(user, "You must specify a user to ban.");
                    else
                    {
                        targetUser = GetUserByName(aryCmd[1]);
                        if (targetUser == null)
                            SendServerError(user, "There is no user by the name " + aryCmd[1] + " online.");
                        else
                            BanUser(user, targetUser, user.Channel);
                    }
                    break;

                case "unban":
                    break;

                default:
                    SendServerError(user, "That is not a valid command.");
                    break;
            }
        }

        public void HandleConsoleCommand(string cmd)
        {
            HandleCommand(console, cmd);
        }

    }
}
