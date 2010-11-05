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
        protected void timerCheck_Elapsed(object sender, ElapsedEventArgs e)
        {
            IEnumerable<Channel> chans = Channels.Where(c => c.Owner != null
                && DateTime.Now.Subtract(c.OwnerLeft).Minutes >= Config.ChannelOwnerTimeout
                && c.Owner.Channel != c );
            foreach (Channel chan in chans)
                PromoteNewUserToOwner(chan);


            //DEBUG STUFFS:
            DoDebugStuffs();
        }

        protected void DoDebugStuffs()
        {
            int c1, c2, c3, c4;
            System.Threading.ThreadPool.GetAvailableThreads(out c1, out c2);
            System.Threading.ThreadPool.GetAvailableThreads(out c3, out c4);
            ServerStats.activeWorkerThreads = c3 - c1;
            ServerStats.activeIOThreads = c4 - c2;

            string test = "";
            foreach (User u in GetAllOnlineUsers())
                test += u.Socket.ct1 + "/" + u.Socket.ct2 + " ";
            ServerStats.test = test;

            ServerStats.lastBytesSent = ServerStats.bytesSent;
            ServerStats.lastBytesRecv = ServerStats.bytesRecv;
        }
    }
}
