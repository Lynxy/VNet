using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNetServer
{
    static public class ClientHandler
    {
        public delegate void UserPacketReceivedDelegate(User user, byte[] data);
        static public event UserPacketReceivedDelegate UserPacketReceived;

        static public Dictionary<TcpClientWrapper, User> Users = new Dictionary<TcpClientWrapper, User>();

        static public void AddNewClient(TcpClientWrapper client)
        {
            Users.Add(client, new User(client));
            client.DataRead += new TcpClientWrapper.DataReadDelegate(client_DataRead);
            client.AsyncRead(1024, true);
        }

        static private void client_DataRead(TcpClientWrapper sender, byte[] data)
        {
            User user = Users[sender];
            int oldLen = user.Buffer.Length;
            Array.Resize(ref user.Buffer, oldLen + data.Length);
            Array.Copy(data, 0, user.Buffer, oldLen, data.Length);

            //check to see if whole packet is available
            byte[] completePacket;
            while ((completePacket = PacketBuffer.GetNextPacket(ref user.Buffer)) != null)
            {
                UserPacketReceived(user, completePacket);
            }
        }
    }
}
