using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected void HandlePacket_VNET_CHATEVENT(User user, PacketReader reader)
        {
            string text = reader.ReadStringNT();

            if (ContainsNonPrintable(text))
            {
                SendServerError(user, "Chat events cannot contain non-printable characters");
                return;
            }

            if (text[0] == '/')
                HandleCommand(user, text.Substring(1));
            else
            {
                ConsoleSendUserTalk(user, text);
                SendUserTalk(user, text);
            }
        }
    }
}
