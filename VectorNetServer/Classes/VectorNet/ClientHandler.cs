using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNetServer
{
    static public class ClientHandler
    {
        public delegate void UserPacketReceivedDelegate(User user, PacketReader reader);
        static public event UserPacketReceivedDelegate UserPacketReceived;

        static public Dictionary<TcpClientWrapper, User> Users = new Dictionary<TcpClientWrapper, User>();
        static public Dictionary<User, byte[]> UserBuffers = new Dictionary<User, byte[]>();

        static public User AddNewClient(TcpClientWrapper client)
        {
            User newUser = new User(client);
            Users.Add(client, newUser);
            UserBuffers.Add(newUser, new byte[0]);
            client.DataRead += new TcpClientWrapper.DataReadDelegate(client_DataRead);
            client.AsyncRead(1024, true);
            return newUser;
        }

        static private void client_DataRead(TcpClientWrapper sender, byte[] data)
        {
            User user = Users[sender];
            byte[] buffer = UserBuffers[user];
            int oldLen = buffer.Length;
            Array.Resize(ref buffer, oldLen + data.Length);
            Array.Copy(data, 0, buffer, oldLen, data.Length);

            //check to see if whole packet is available
            byte[] completePacket;
            while ((completePacket = PacketBuffer.GetNextPacket(ref buffer)) != null)
            {
                UserPacketReceived(user, new PacketReader(completePacket));
            }
        }
    }
}
