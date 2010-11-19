using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        public delegate void ServerExceptionDelegate(Exception err);
        public event ServerExceptionDelegate EventServerException;

        public delegate void UserJoinServerDelegate(string username, byte flags);
        public event UserJoinServerDelegate EventUserJoinServer;
        public delegate void UserLeftServerDelegate(string username, byte flags);
        public event UserLeftServerDelegate EventUserLeftServer;

        public delegate void UserJoinChannelDelegate(string username, byte flags, string channel);
        public event UserJoinChannelDelegate EventUserJoinChannel;
        public delegate void UserLeftChannelDelegate(string username, byte flags, string channel);
        public event UserLeftChannelDelegate EventUserLeftChannel;

        public delegate void UserTalkDelegate(string username, byte flags, string channel, string message);
        public event UserTalkDelegate EventUserTalk;
        public delegate void UserEmoteDelegate(string username, byte flags, string channel, string message);
        public event UserEmoteDelegate EventUserEmote;
    }
}
