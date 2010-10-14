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

namespace VectorNet.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpClientWrapper socket;
        Packet packet;

        public MainWindow()
        {
            InitializeComponent();

            socket = new TcpClientWrapper();
            socket.ConnectionEstablished += new TcpClientWrapper.ConnectionEstablishedDelegate(socket_ConnectionEstablished);
            socket.DataRead += new TcpClientWrapper.DataReadDelegate(socket_DataRead);

            packet = new Packet();
            packet.DataSent += new Packet.SendDataDelegate(packet_DataSent);
        }

        private void socket_ConnectionEstablished(TcpClientWrapper sender)
        {
            socket.AsyncRead(1024, true);
            packet.Clear().InsertString("huh").Send(0);
        }

        private void socket_DataRead(TcpClientWrapper sender, byte[] data)
        {

        }

        private void packet_DataSent(byte[] data)
        {
            socket.AsyncSend(data, data.Length);
        }

        private void mnuConnect_Click(object sender, RoutedEventArgs e)
        {
            socket.AsyncConnect("127.0.0.1", 4800);
        }
    }
}
