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
            public void AddOrModifyQueue(User user)
            {
                QueueSharingData ret = QSQueues.Find(f => f.channel == user.BattleNetChannel);

                if (ret == null)
                {
                    QueueSharingData temp = new QueueSharingData(user.Username, user.BattleNetChannel);
                    
                    temp.AddUserToQueue(user);
                    QSQueues.Add(temp);
                }
                else
                    ret.AddUserToQueue(user);
            }

            /// <summary>
            /// Returns a queue by channel name
            /// </summary>
            /// <param name="channel">The channel whose queue you wish to search for</param>
            public QueueSharingData GetQueueByName(string channel)
            {
                return QSQueues.Find(f => f.channel.ToLower() == channel.ToLower());
            }

            /// <summary>
            /// Returns a boolean indicating if the specified user is the queue master
            /// </summary>
            /// <param name="username">Name of the user being checked</param>
            /// <param name="channel">String which represents the channel for the queue</param>
            public bool isQueueMaster(string username, string channel)
            {
                QueueSharingData ret = QSQueues.Find(f => f.channel.ToLower() == channel.ToLower());

                if (ret != null && ret.master == username)
                    return true;
    
                return false;
            }

            /// <summary>
            /// Returns a boolean indicating whether or not a specific queue exists
            /// </summary>
            /// <param name="user"></param>
            public bool QueueExists(User user)
            {
                return (QSQueues.Find(f => f.channel == user.BattleNetChannel) != null ? true : false);
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
