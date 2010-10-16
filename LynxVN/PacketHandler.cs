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
            User user;
            string username;
            string text;
            int ping;
            byte flags;
            byte id;

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
                        flags = reader.ReadByte();

                        if (logonResult == LogonResult.SUCCESS || logonResult == LogonResult.SEND_CHALLENGE)
                        {
                            AddChat(Brushes.Blue, "Successfully logged on");
                        }
                        else if (logonResult == LogonResult.INVALID_PASSWORD)
                            AddChat(Brushes.Red, "Invalid password");
                        else if (logonResult == LogonResult.INVALID_USERNAME)
                            AddChat(Brushes.Red, "Invalid username");
                        else if (logonResult == LogonResult.ACCOUNT_IN_USE)
                            AddChat(Brushes.Red, "Account is in use");
                        break;

                    case VNET_CHATEVENT: //0x03
                        id = reader.ReadByte();
                        ping = reader.ReadDword();
                        flags = reader.ReadByte();
                        username = reader.ReadStringNT();
                        text = reader.ReadStringNT();

                        if (id == (byte)ChatEventType.USER_JOIN_CHANNEL)
                        {
                            AddUser(new User() { Username = username, Client = text, Flags = flags, Ping = ping });
                            AddChat(Brushes.DarkGreen, "-- ", Brushes.Blue, username, Brushes.DarkGreen, " has joined the channel");
                        }
                        else if (id == (byte)ChatEventType.USER_LEAVE_CHANNEL)
                        {
                            RemoveUser(username);
                            AddChat(Brushes.DarkRed, "-- ", Brushes.Blue, username, Brushes.DarkRed, " has left the channel");
                        }
                        else if (id == (byte)ChatEventType.USER_TALK)
                        {
                            AddChat(Brushes.Orange, "<", Brushes.Orange, username, Brushes.Orange, "> " , Brushes.Black, text);
                        }
                        break;

                    case VNET_LIST: //0x06
                        id = reader.ReadByte();
                        if (id == (byte)ListType.UsersInChannel)
                        {
                            ClearChannelList();
                            while (!reader.EOF())
                            {
                                username = reader.ReadStringNT();
                                text = reader.ReadStringNT();
                                ping = reader.ReadDword();
                                flags = reader.ReadByte();
                                user = new User() { Username = username, Client = text, Ping = ping, Flags = flags };
                                AddUser(user);
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
