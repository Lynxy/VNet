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
        Dictionary<TcpClientWrapper, PacketBufferer> buffers = new Dictionary<TcpClientWrapper, PacketBufferer>();

        Timer tmr;
        Random rnd = new Random();
        bool doTalk = false;

        public StressTest()
        {
            InitializeComponent();
            tmr = new Timer(tmr_Elapsed, null, 1000, 1000);
        }

        void tmr_Elapsed(object state)
        {
            checkBox1.Dispatcher.Invoke(new Action(delegate
            {
                doTalk = (bool)checkBox1.IsChecked;
            }));

            if (!doTalk)
                return;
            lock (sockets)
            {
                foreach (TcpClientWrapper sock in sockets.Values)
                {
                    if (sock.Client != null && sock.Connected)
                    {
                        packets[sock].Clear().InsertStringNT("wtf msg").Send(0x03);
                    }
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
                    sock.DataRead += new TcpClientWrapper.DataReadDelegate(sock_DataRead);

                    Packet pack = new Packet();
                    pack.DataSent += new Packet.SendDataDelegate(pack_DataSent);

                    PacketBufferer bufferer = new PacketBufferer(SendDataFinal, sock);

                    sockets.Add(pack, sock);
                    packets.Add(sock, pack);
                    buffers.Add(sock, bufferer);

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
            packets[sender].Clear().InsertStringNT("/join spam").Send(0x03);
        }

        void sock_DataRead(TcpClientWrapper sender, byte[] data)
        {

        }

        void pack_DataSent(Packet packet, ref byte[] data)
        {
            buffers[sockets[packet]].QueuePacket(ref data);
        }

        void SendDataFinal(object state, ref byte[] data)
        {
            TcpClientWrapper sock = (TcpClientWrapper)state;
            sock.AsyncSend(data, data.Length);
        }
    }
}
