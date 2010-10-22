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
using System.Windows.Shapes;

using Lynxy.Network;
using System.Threading;

namespace LynxVN
{
    /// <summary>
    /// Interaction logic for StressTest.xaml
    /// </summary>
    public partial class StressTest : Window
    {
        Dictionary<Packet, TcpClientWrapper> sockets = new Dictionary<Packet, TcpClientWrapper>();
        Dictionary<TcpClientWrapper, Packet> packets = new Dictionary<TcpClientWrapper, Packet>();
        Timer tmr;
        Random rnd = new Random();

        public StressTest()
        {
            InitializeComponent();
            tmr = new Timer(tmr_Elapsed, null, 500, 500);
        }

        void tmr_Elapsed(object state)
        {
            lock (sockets)
            {
                foreach (TcpClientWrapper sock in sockets.Values)
                {
                    if (sock.Client != null && sock.Connected)
                        packets[sock].Clear().InsertStringNT("wtf msg").Send(0x03);
                }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            lock (sockets)
            {
                for (int i = 0; i < 10; i++)
                {
                    TcpClientWrapper sock = new TcpClientWrapper();
                    sock.ConnectionEstablished += new TcpClientWrapper.ConnectionEstablishedDelegate(sock_ConnectionEstablished);

                    Packet pack = new Packet();
                    pack.DataSent += new Packet.SendDataDelegate(pack_DataSent);

                    sockets.Add(pack, sock);
                    packets.Add(sock, pack);

                    sock.AsyncConnect("127.0.0.1", 4800);
                }
            }
        }

        void sock_ConnectionEstablished(TcpClientWrapper sender)
        {
            packets[sender].Clear()
                .InsertStringNT("Lynxy" + rnd.Next(10000, 99999).ToString())
                .InsertStringNT("pass2")
                .InsertStringNT("LynxVN")
                .InsertByte(0)
                .Send(0x01);
        }

        void pack_DataSent(Packet packet, ref byte[] data)
        {
            sockets[packet].AsyncSend(data, data.Length);
        }
    }
}
