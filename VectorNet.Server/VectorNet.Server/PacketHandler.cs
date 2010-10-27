using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected void HandlePacket(User user, PacketReader reader)
        {
            //try
            //{
                byte packetId = reader.ReadByte();
                if (!user.IsOnline && packetId != VNET_LOGON)
                {
                    DisconnectUser(user, "You must logon first.");
                    return;
                }
                else if (!user.CanSendData)
                { //they're still trying to send data after we disconnected them
                    return;
                }

                switch (packetId)
                {
                    case VNET_LOGON: //0x01
                        HandlePacket_VNET_LOGON(user, reader);
                        break;

                    case VNET_SERVERCHALLENGE: //0x02
                        HandlePacket_VNET_SERVERCHALLENGE(user, reader);
                        break;

                    case VNET_CHATEVENT: //0x03
                        HandlePacket_VNET_CHATEVENT(user, reader);
                        break;

                    default:
                        SendServerError(user, "Your client sent an unknown packet (0x" + packetId.ToString("X") + ")");
                        break;
                }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("ERR - " + ex.Message);
            //    //TODO: log exception
            //}
        }

        

    }
}
