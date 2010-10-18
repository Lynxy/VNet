using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VectorNet.Server;

namespace VectorNet.Server.GUI
{
    public class ServerLoader
    {
        protected Server VNet;

        public ServerLoader()
        {
            VNet = new Server("config.xml");

            VNet.CreateConsoleUser(RecvData);
            VNet.StartListening();
            VNet.HandleConsoleCommand("test");
        }

        private void RecvData(byte[] data)
        {
            //MessageBox.Show("recv: " + Encoding.ASCII.GetString(data).Replace((char)0, '.'));
        }
    }
}
