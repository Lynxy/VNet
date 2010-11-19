using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VectorNet.Server;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected class QueueSharing
        {
            protected List<QueueSharingData> QSQueues;

            public QueueSharing()
            {
                QSQueues = new List<QueueSharingData>();
            }

            public void AddQueue(User user)
            {
                QueueSharingData temp = new QueueSharingData(user.BattleNetChannel);

                temp.AddUserToQueue(user);
                QSQueues.Add(temp);
            }

            public void RemQueue()
            { 
            
            }
        }
    }
}
