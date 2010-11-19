using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;
using System.Timers;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected void timerGarbage_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerGarbage.Stop();

            foreach (User user in GetAllOfflineUsers())
                if (user != console)
                {
                    if (user.CanSendData)
                        DisconnectUser(user, "Server has detected your status is no longer Online.");
                    Users.Remove(user);
                }

            foreach (Channel chan in Channels.ToList())
            {
                foreach (User user in chan.GetCompleteUserList())
                    if (user == null)
                        RemoveUserFromChannel(user);
                AttemptToCloseChannel(chan);
            }

            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(delegate
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    timerGarbage.Start();
                }));
        }
    }
}
