using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected void HandleVNET_QUEUESHARING(User user, PacketReader reader)
        {
            byte getQueueId = reader.ReadByte();

            switch (getQueueId)
            { 

            }
        }
    }
}
