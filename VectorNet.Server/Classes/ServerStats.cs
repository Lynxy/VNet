﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    static public class ServerStats
    {
        static public long bytesSent = 0;
        static public long bytesRecv = 0;
        static public long lastBytesSent = 0;
        static public long lastBytesRecv = 0;

        static public long totalConnections = 0;
        static public long usersOnline = 0;

        static public long activeWorkerThreads = 0;
        static public long activeIOThreads = 0;


        static public string test = "";
    }
}
