using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Lynxy.Network;

namespace LynxVN
{
    public partial class MainWindow : Window
    {
        protected const int VNET_KEEPALIVE = 0x00;
        protected const int VNET_LOGON = 0x01;
        protected const int VNET_SERVERCHALLENGE = 0x02;
        protected const int VNET_CHATEVENT = 0x03;
        protected const int VNET_FEATUREQUERY = 0x04;
        protected const int VNET_APPS = 0x05;
        protected const int VNET_LIST = 0x06;
        protected const int VNET_QUEUESHARE = 0x07;

        protected enum LogonResult
        {
            SUCCESS = 0x00,
            INVALID_PASSWORD = 0x01,
            INVALID_USERNAME = 0x02,
            ACCOUNT_IN_USE = 0x03,
            SEND_CHALLENGE = 0x04
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
            UsersOnServer = 0x02,
            UsersBannedFromChannel = 0x03,
        }
    }
}
