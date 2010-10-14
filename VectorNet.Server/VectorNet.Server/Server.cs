﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        public Server(string XMLConfigFile)
            : this()
        {
            Config.File = XMLConfigFile;
            Config.LoadConfig();
        }
        public Server()
        {
            //nothing here yet
        }
    }
}