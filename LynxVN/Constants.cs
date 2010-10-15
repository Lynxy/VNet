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
        protected const int VNET_LOGON = 0x01;
        protected const int VNET_CHATEVENT = 0x02;
        protected const int VNET_TICTACTOE = 0x03;
        protected const int VNET_CHANNELLIST = 0x04;
        protected const int VNET_FEATUREQUERY = 0x05;
        protected const int VNET_KEEPALIVE = 0x06;
        protected const int VNET_CHANNELJOIN = 0x07;
        protected const int VNET_QUEUESHARE = 0x09;
        protected const int VNET_SERVERCHALLENGE = 0x0A;
        protected const int VNET_VNETPAD = 0x0B;

        protected enum LogonResult
        {
            SUCCESS = 0x00,
            INVALID_PASSWORD = 0x01,
            INVALID_USERNAME = 0x02,
            ACCOUNT_IN_USE = 0x03,
            INVALID_PROTOCOL = 0x04
        }
    }
}
