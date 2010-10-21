using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected enum ConsolePacketID
        {
            UserJoinedServer = 0x00,
            UserLeftServer = 0x01,
            UserList = 0x02,
            UserTalk = 0x03,
            UserJoinedChannel = 0x04
        }

        protected void ConsoleSendUserTalk(User user, string msg)
        {
            console.Packet.Clear()
                .InsertStringNT(user.Username)
                .InsertByte((byte)user.Flags)
                .InsertStringNT(user.Channel.Name)
                .InsertStringNT(msg)
                .Send((byte)ConsolePacketID.UserTalk);
        }

        protected void ConsoleSendUserJoinedChannel(User user)
        {
            console.Packet.Clear()
                .InsertStringNT(user.Username)
                .InsertByte((byte)user.Flags)
                .InsertStringNT(user.Channel.Name)
                .Send((byte)ConsolePacketID.UserJoinedChannel);
        }
    }
}
