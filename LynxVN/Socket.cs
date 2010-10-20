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
        protected TcpClientWrapper socket;
        protected Packet packet;
        protected byte[] packetBuffer = new byte[0];

        protected void SetupSocket()
        {
            socket = new TcpClientWrapper();
            socket.ConnectionEstablished += new TcpClientWrapper.ConnectionEstablishedDelegate(socket_ConnectionEstablished);
            socket.ConnectionRefused += new TcpClientWrapper.ConnectionRefusedDelegate(socket_ConnectionRefused);
            socket.Disconnected += new TcpClientWrapper.DisconnectedDelegate(socket_Disconnected);
            socket.DataRead += new TcpClientWrapper.DataReadDelegate(socket_DataRead);

            packet = new Packet();
            packet.DataSent += new Packet.SendDataDelegate(packet_DataSent);
        }

        protected void socket_ConnectionEstablished(TcpClientWrapper sender)
        {
            AddChat(Brushes.Green, "Connection accepted. Sending login information.");
            socket.AsyncRead(1024, true);
            SendLogonPacket();
        }

        protected void socket_ConnectionRefused(TcpClientWrapper sender)
        {
            AddChat(Brushes.Red, "The connection was refused.");
        }

        protected void socket_Disconnected(TcpClientWrapper sender)
        {
            AddChat(Brushes.Red, "Disconnected from server.");
        }

        protected void socket_DataRead(TcpClientWrapper sender, byte[] data)
        {
            int oldLen = packetBuffer.Length;
            Array.Resize(ref packetBuffer, oldLen + data.Length);
            Array.Copy(data, 0, packetBuffer, oldLen, data.Length);

            //check to see if whole packet is available
            byte[] completePacket;
            while ((completePacket = PacketBuffer.GetNextPacket(ref packetBuffer)) != null)
            {
                this.Dispatcher.Invoke(new Action(delegate
                    {
                        HandlePacket(new PacketReader(completePacket));
                    }), null);
            }
        }

        protected void packet_DataSent(byte[] data)
        {
            socket.AsyncSend(data, data.Length);
        }
    }
}
