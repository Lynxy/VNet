using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VectorNet.Server;

namespace VectorNet.Server.GUI
{
    public class ServerLoader
    {
        //Server Loader
        //All server-related code must go in here, or risk causing the GUI/client to crash

        protected Server VNet;

        public ServerLoader(Action<byte[]> dataRecv)
        {
            VNet = new Server("config.xml");

            //VNet.CreateConsoleUser(dataRecv);
            VNet.StartListening();
        }

        public void HandleConsoleCommand(string cmd)
        {
            //VNet.HandleConsoleCommand(cmd);
        }

        private void RecvData(byte[] data)
        {
            //MessageBox.Show("recv: " + Encoding.ASCII.GetString(data).Replace((char)0, '.'));
        }
    }
}
