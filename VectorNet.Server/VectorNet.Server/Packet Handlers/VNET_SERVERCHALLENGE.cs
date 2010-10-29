using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected void HandlePacket_VNET_SERVERCHALLENGE(User user, PacketReader reader)
        {
            string GrabChallenge = reader.ReadStringNT();

            if (IsEmptyChallenge(user.Username, GrabChallenge))
                // blah
            else
                if (!GetChallengeState(user.Username, GrabChallenge)
                {
                    DisconnectUser(user, "You have provided an incorrect challenge.");
                }
        }
    }
}
