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
                UserTalk(user, text);
        }

        protected void UserTalk(User user, string text)
        {
            if (text.Length == 0)
            {
                SendServerError(user, "What do you want to say?");
                return;
            }

            Channel chan = user.Channel;
            int userCt = GetUsersInChannel(user, chan, true).Count;
            ConsoleSendUserTalk(user, text);

            if (UserIsStaff(user))
                SendUserTalk(user, text); //when staff talk, all can always hear
            else
            {
                if (ChannelHasFlags(chan, ChannelFlags.Silent) == true)
                    userCt = 0;

                if (ChannelHasFlags(chan, ChannelFlags.Administrative) == true
                    || ChannelHasFlags(chan, ChannelFlags.Silent) == true)
                {
                    foreach (User u in GetUsersInChannel(chan))
                        if (UserIsStaff(u))
                            SendUserTalkSingle(user, u, text);
                }
                else
                    SendUserTalk(user, text); //send it always incase invis users
            }

            if (userCt == 0)
                SendServerInfo(user, "No one hears you.");
        }
    }
}
