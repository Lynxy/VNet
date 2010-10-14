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

            switch (aryCmd[0].ToLower())
            { 
                case "test":
                    //addchat vbgreen, "Hiya ^_^!";
                    user.Packet.InsertString("Hiya ^_^!").Send(0);
                    break;
            }
        }

        public void HandleConsoleCommand(string cmd)
        {
            HandleCommand(console, cmd);
        }

    }
}
