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
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Total connections = " + ServerStats.totalConnections + " (" + ServerStats.usersOnline + " online)");
            Console.WriteLine("KB sent = " + (ServerStats.bytesSent / 1024)
                + " (" + ((ServerStats.bytesSent - ServerStats.lastBytesSent) / 1024) + " kb/s) | " +
                "KB recv = " + (ServerStats.bytesRecv / 1024));

            int c1, c2;
            System.Threading.ThreadPool.GetAvailableThreads(out c1, out c2);
            Console.WriteLine("GetAvailableThreads = " + c1 + "/" + c2);

            string test = "";
            foreach (User u in GetAllOnlineUsers())
                test += u.Socket.ct1 + "/" + u.Socket.ct2 + " ";
            ServerStats.test = test;
            Console.WriteLine("Missed/Total packets = " + test);

            ServerStats.lastBytesSent = ServerStats.bytesSent;
            ServerStats.lastBytesRecv = ServerStats.bytesRecv;
        }
    }
}
