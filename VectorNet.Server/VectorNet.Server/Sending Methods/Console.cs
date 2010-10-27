using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected void ConsoleSendServerException(Exception err)
        {
            if (EventServerException != null)
                EventServerException(err);
        }

        protected void ConsoleSendUserJoinServer(User user)
        {
            if (EventUserJoinServer != null)
                EventUserJoinServer(user.Username, (byte)user.Flags);
        }

        protected void ConsoleSendUserLeftServer(User user)
        {
            if (EventUserLeftServer != null)
                EventUserLeftServer(user.Username, (byte)user.Flags);
        }

        protected void ConsoleSendUserJoinChannel(User user)
        {
            if (EventUserJoinChannel != null)
                EventUserJoinChannel(user.Username, (byte)user.Flags, user.Channel.Name);
        }

        protected void ConsoleSendUserLeftChannel(User user)
        {
            if (EventUserLeftChannel != null)
                EventUserLeftChannel(user.Username, (byte)user.Flags, user.Channel.Name);
        }

        protected void ConsoleSendUserTalk(User user, string msg)
        {
            if (EventUserTalk != null)
                EventUserTalk(user.Username, (byte)user.Flags, user.Channel.Name, msg);
        }

        protected void ConsoleSendUserEmote(User user, string msg)
        {
            if (EventUserEmote != null)
                EventUserEmote(user.Username, (byte)user.Flags, user.Channel.Name, msg);
        }
    }
}
