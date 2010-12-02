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
            byte getQueueStatus = reader.ReadByte();

            switch ((QueueSharingID)getQueueId)
            { 
                case QueueSharingID.QueuePoolToggle:

                    if (getQueueStatus == 0x01)
                        Queues.AddOrModifyQueue(user);
                    else
                        Queues.RemOrModifyQueue(user);

                    break;
            }
        }
    }
}
