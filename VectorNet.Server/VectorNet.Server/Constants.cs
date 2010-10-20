using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected const int VNET_KEEPALIVE      = 0x00;
        protected const int VNET_LOGON          = 0x01;
        protected const int VNET_SERVERCHALLENGE= 0x02;
        protected const int VNET_CHATEVENT      = 0x03;
        protected const int VNET_FEATUREQUERY   = 0x04;
        protected const int VNET_APPS           = 0x05;
        protected const int VNET_LIST           = 0x06;
        protected const int VNET_QUEUESHARE     = 0x07;

        [Flags]
        protected enum UserFlags
        {
            Normal = 0x00,
            Admin = 0x01,
            Operator = 0x02,
            Moderator = 0x04,
            Ignored = 0x08,
            Muted = 0x10,
            Invisible = 0x20
        }

        [Flags]
        protected enum ChannelFlags
        {
            Normal = 0x00,
            Public = 0x01,
            Administrative = 0x02,
            Clan = 0x04,
            Silent = 0x08
        }
        protected enum AppFlags
        { 
            APP_UNHANDLED = 0x00,
            APP_TICTACTOE = 0x01,
            APP_VNETPAD = 0x02,
            APP_ICONS = 0x03,
            APP_FILESHARING = 0x04
        }

        protected enum LogonResult
        {
            SUCCESS          = 0x00,
            INVALID_PASSWORD = 0x01,
            INVALID_USERNAME = 0x02,
            ACCOUNT_IN_USE   = 0x03,
            SEND_CHALLENGE   = 0x04
        }

        protected enum ChatEventType
        {
            USER_JOIN_VNET = 0x01,
            USER_LEAVE_VNET = 0x02,
            USER_TALK = 0x03,
            USER_EMOTE = 0x04,
            SERVER_INFO = 0x05,
            USER_JOIN_CHANNEL = 0x06,
            USER_LEAVE_CHANNEL = 0x07,
            WHISPER_TO = 0x08,
            WHISPER_FROM = 0x09
        }

        protected enum ListType
        {
            UsersInChannel = 0x01,
            UsersBannedFromChannel = 0x02,
            UsersOnServer = 0x03
        }

        protected struct TempUserStruct
        {
            public string Username;
            public string Channel;
            public string Client;
            public List<string> bannedChannel;
            public short ping;
            public byte flags;
            public byte banned;
        }
    }
}
