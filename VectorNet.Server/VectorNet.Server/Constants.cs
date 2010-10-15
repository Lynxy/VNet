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

        protected enum LogonResult
        {
            SUCCESS          = 0x00,
            INVALID_PASSWORD = 0x01,
            INVALID_USERNAME = 0x02,
            ACCOUNT_IN_USE   = 0x03,
            SEND_CHALLENGE   = 0x04
        }

        protected enum ListType
        {
            UsersInChannel         = 0x01,
            UsersOnServer          = 0x02,
            UsersBannedFromChannel = 0x03,
        }
    }
}
