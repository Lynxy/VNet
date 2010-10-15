using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected void SendLogonResult(User user, LogonResult result)
        {
            //if (result == LogonResult.SUCCESS)
            //{
            //}
            //else
            //{
                user.Packet.Clear()
                    .InsertByte((byte)result)
                    .InsertByte(0) //challenge byte
                    .InsertStringNT("server ver")
                    .InsertStringNT("TestEnv")
                    .InsertDWord(0) //ping
                    .InsertByte(0) //rank
                    .Send(VNET_LOGON);
            //}
        }

        protected void SendServerInfo(User user)
        {

        }
    }
}
