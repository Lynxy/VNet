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

            switch (aryCmd[0].ToLower())
            { 
                case "join":
                    if (aryCmd.Length < 2)
                        SendServerError(user, "You must specify a channel.");
                    else
                    {
                        text = cmd.Substring(cmd.IndexOf(' ') + 1);
                        JoinUserToChannel(user, GetChannelByName(text));
                    }
                    break;
                case "who":
                    if (aryCmd.Length < 2)
                        SendServerError(user, "You must specify a channel.");
                    else
                    {
                        text = cmd.Substring(cmd.IndexOf(' ') + 1);

                        List<User> u = GetUsersInChannel(user, GetChannelByName(text), false);

                        if (u.Count > 0)
                        {
                            string uString = null;
                            int idx = 1;

                            SendServerInfo(user, "Users in channel " + text + ":");
                            foreach (User tmp in u)
                                if (idx == u.Count)
                                {
                                    if (uString == null)
                                        uString = tmp.Username;
                                    else
                                        uString += ", " + tmp.Username;

                                    SendServerInfo(user, uString);
                                }
                                else if (idx % 2 == 0)
                                {
                                    uString += uString + tmp.Username;
                                    SendServerInfo(user, uString);
                                    uString = null;
                                }
                                else
                                    if (uString == null)
                                        uString = tmp.Username;
                                    else
                                        uString += ", " + tmp.Username;
                        }
                        else
                            SendServerError(user, "That channel doesn't exist.");
                    }
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
