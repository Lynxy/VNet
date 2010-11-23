using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected class QueueSharingData
        {
            protected List<User> QueueUsers;
            protected string _channel;
            protected bool PoolStatus;
            
            public string channel { get { return _channel; } }

            public QueueSharingData(string channel)
            {
                PoolStatus = true;
                _channel = channel;
            }

            /// <summary>
            /// Checks to see if a particular IP exists in the queue pool
            /// </summary>
            /// <param name="IP">Represents an IP address</param>
            /// <returns>Returns true if an IP matches the one passed in, false otherwise</returns>
            public bool ExistsIP(string IP)
            {
                User ret = QueueUsers.Find(f => f.IPAddress == IP);

                if (ret != null)
                    return true;
                return false;
            }

            /// <summary>
            /// Checks to see if a particular user exists in the queue pool
            /// </summary>
            /// <param name="name">Name represents the name of a user</param>
            /// <returns>Returns true if name exists in the queue pool, false otherwise</returns>
            public bool ExistsName(string name)
            {
                User ret = QueueUsers.Find(f => f.Username.ToLower() == name.ToLower());

                if (ret != null)
                    return true;

                return false;
            }

            /// <summary>
            /// Adds a user to this queue pool
            /// </summary>
            /// <param name="user">User object to add to the queue</param>
            public void AddUserToQueue(User user)
            {
                if (!QueueUsers.Contains(user))
                    QueueUsers.Add(user);
            }

            /// <summary>
            /// Removes a user from this queue pool
            /// </summary>
            /// <param name="user">User object to remove from the queue</param>
            public void RemUserFromQueue(User user, bool isIPRequest)
            {
                if (isIPRequest)
                {
                    List<User> ret = new List<User>();
                    ret = QueueUsers.FindAll(f => f.IPAddress == user.IPAddress);

                    if (ret.Count > 0)
                        foreach (User u in ret)
                            QueueUsers.Remove(u);
                }
                else
                    if (QueueUsers.Contains(user))
                        QueueUsers.Remove(user);    
            }
        }
    }
}
