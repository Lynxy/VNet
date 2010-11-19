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
            Normal      = 0x00,
            Admin       = 0x01,
            Operator    = 0x02,
            Moderator   = 0x04,
            Ignored     = 0x08,
            Muted       = 0x10,
            Invisible   = 0x20
        }

        [Flags]
        protected enum ChannelFlags
        {
            Normal          = 0x00,
            Public          = 0x01,
            Administrative  = 0x02,
            Clan            = 0x04,
            Silent          = 0x08
        }
        protected enum AppFlags
        { 
            Unhandled   = 0x00,
            TicTacToe   = 0x01,
            VNetPad     = 0x02,
            Icons       = 0x03,
            FileSharing = 0x04
        }

        protected enum LogonResult
        {
            Success         = 0x00,
            InvalidPassword = 0x01,
            InvalidUsername = 0x02,
            AccountBanned   = 0x03,
            SendChallenge   = 0x04
        }

        protected enum ChatEventType
        {
            UserJoinedServer    = 0x01,
            UserLeftServer      = 0x02,
            UserTalk            = 0x03,
            UserEmote           = 0x04,
            ServerInfo          = 0x05,
            UserJoinedChannel   = 0x06,
            UserLeftChannel     = 0x07,
            UserWhisperTo       = 0x08,
            UserWhisperFrom     = 0x09
        }

        protected enum ListType
        {
            UsersInChannel          = 0x01,
            UsersBannedFromChannel  = 0x02,
            UsersOnServer           = 0x03,
            UsersFlagsUpdate = 0x04
        }

        protected enum QueueSharingIDs
        { 
            QueueToggle             = 0x01,
            QueueSend               = 0x02,
            QueueMasterChange       = 0x03,
            QueueNewChannel         = 0x04,
            QueuePoolToggle         = 0x05,
            QueueRemoveUser         = 0x06
        }
    }
}
