﻿using System;
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
                Channel chan = user.Channel;
                ConsoleSendUserTalk(user, text);
                if (ChannelHasFlags(chan, ChannelFlags.Administrative) == true)
                {
                    foreach (User u in GetUsersInChannel(chan))
                        if (UserIsStaff(u))
                            SendUserTalkSingle(user, u, text);
                }
                else
                    SendUserTalk(user, text); //send it always incase invis users

                if (GetUsersInChannel(user, chan, true).Count == 0)
                    SendServerInfo(user, "No one hears you.");
            }
        }
    }
}
