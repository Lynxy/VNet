using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorNet.Server
{
    public partial class Server
    {
        public class QueueSharingData
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

            public bool ExistsIP(string IP)
            {
                User ret = QueueUsers.Find(f => f.IPAddress == IP);

                if (ret != null)
                    return true;

                return false;
            }

            public bool ExistsName(string name)
            {
                User ret = QueueUsers.Find(f => f.Username == name);

                if (ret != null)
                    return true;

                return false;
            }

            public void AddUserToQueue(User user)
            {
                QueueUsers.Add(user);
            }
        }
    }
}
