﻿using System;
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
        protected void ConnectToServer(string ip, int port)
        {
            socket.AsyncConnect(ip, port);
        }

        protected void SendLogonPacket()
        {
            packet.Clear()
                .InsertStringNT("Lynxy")
                .InsertStringNT("pass")
                .InsertStringNT("LynxVN")
                .InsertByte(0) //queue sharing
                .InsertByte(1) //protocol revision
                .Send(VNET_LOGON);
        }
    }
}