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

            /// <summary>
            /// Adds a queue, or includes a user into that queue
            /// </summary>
            /// <param name="user">The user to add to the queue</param>
            public void AddQueue(User user)
            {
                QueueSharingData ret = QSQueues.Find(f => f.channel == user.BattleNetChannel);

                if (ret == null)
                {
                    QueueSharingData temp = new QueueSharingData(user.BattleNetChannel);
                    
                    temp.AddUserToQueue(user);
                    QSQueues.Add(temp);
                }
                else
                    ret.AddUserToQueue(user);
            }

            /// <summary>
            /// Removes a user from the queue pool
            /// </summary>
            /// <param name="user">The user object to remove from the queue</param>
            public void RemQueue(User user)
             {
                QueueSharingData ret = QSQueues.Find(f => f.channel == user.BattleNetChannel);
                if (ret != null) QSQueues.Remove(ret);
            }
        }
    }
}
