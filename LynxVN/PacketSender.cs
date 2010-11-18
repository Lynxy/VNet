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
using System.Management;

namespace LynxVN
{
    public partial class MainWindow : Window
    {
        protected void ConnectToServer(string ip, int port)
        {
            socket.AsyncConnect(ip, port);
        }

        protected void SendLogonPacket()
        {
            packet.Clear()
                .InsertStringNT("Lynxy")
                .InsertStringNT("pass2")
                .InsertStringNT("LynxVN")
                .Send(VNET_LOGON);
        }
    }
}
