﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected void HandleAppID(User fromSend, PacketReader reader)
        {

            byte getAppID = reader.ReadByte();

            if (getAppID > 0x05)
            {
                SendServerError(fromSend, "You have send an invalid app ID.");
                return;
            }

            byte getAppFlagStatus = reader.ReadByte();
            User toSend = GetUserByName(reader.ReadStringNT());

            toSend.Packet.Clear().InsertByte(getAppID)
                                 .InsertByte(getAppFlagStatus)
                                 .InsertStringNT(fromSend.Username);
            switch ((AppFlags)getAppID)
            {
                case AppFlags.Unhandled:
                    toSend.Packet.Send(VNET_APPS);

                    break;
                case AppFlags.TicTacToe:
                    byte tttEventID = reader.ReadByte();

                    if (tttEventID == 0x06)
                    {
                        Random num = new Random();
                        int numRand = num.Next(1);

                        byte result = (byte)(numRand == 0 ? 0x01 : 0x02);
                        toSend.Packet.InsertByte(tttEventID)
                                     .InsertByte(result)
                                     .Send(VNET_APPS);
                    }
                    else
                        toSend.Packet.InsertByte(tttEventID).InsertString(reader.ReadToEnd().ToString());

                    break;
                case AppFlags.VNetPad:
                    toSend.Packet.Clear().InsertString(reader.ReadToEnd().ToString()).Send(VNET_APPS);

                    break;
            }
        }
    }
}
