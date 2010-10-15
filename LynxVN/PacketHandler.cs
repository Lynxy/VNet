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
        protected void HandlePacket(PacketReader reader)
        {
            string username;
            int ping;
            byte flags;

            try
            {
                byte packetId = reader.ReadByte();
                switch (packetId)
                {
                    case VNET_LOGON: //0x01
                        LogonResult logonResult = (LogonResult)reader.ReadByte();
                        byte challengeByte = reader.ReadByte();
                        string serverVersion = reader.ReadStringNT();
                        string hostedBy = reader.ReadStringNT();
                        ping = reader.ReadDword();
                        byte rank = reader.ReadByte();

                        if (logonResult == LogonResult.INVALID_PASSWORD)
                            AddChat(Brushes.Red, "Invalid password");
                        else if (logonResult == LogonResult.INVALID_USERNAME)
                            AddChat(Brushes.Red, "Invalid username");
                        else if (logonResult == LogonResult.ACCOUNT_IN_USE)
                            AddChat(Brushes.Red, "Account is in use");
                        else if (logonResult == LogonResult.INVALID_PROTOCOL)
                            AddChat(Brushes.Red, "Invalid protocol");
                        else
                        {
                            AddChat(Brushes.Blue, "Successfully logged on");
                        }
                        break;
                    case VNET_CHANNELLIST: //0x04
                        ListType listType = (ListType)reader.ReadByte();
                        if (listType == ListType.UsersInChannel)
                        {
                            lvChannel.Items.Clear();
                            while (!reader.EOF())
                            {
                                username = reader.ReadStringNT();
                                ping = reader.ReadDword();
                                flags = reader.ReadByte();
                                lvChannel.Items.Add(username);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
