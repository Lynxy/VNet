using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected void HandleCommand(byte[] user, byte flag, byte[] cmd)
        {
            string[] aryCmd = cmd.ToString().Split(' ');

            switch (aryCmd[0].ToLower())
            { 
                case "test":
                    //addchat vbgreen, "Hiya ^_^!";
                    break;
            }
        }

    }
}
