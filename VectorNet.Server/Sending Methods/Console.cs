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
            if (err == null) return;
            if (EventServerException != null)
                EventServerException(err);
        }

        protected void ConsoleSendUserJoinServer(User user)
        {
            if (user == null) return;
            if (EventUserJoinServer != null)
                EventUserJoinServer(user.Username, (byte)user.Flags);
        }

        protected void ConsoleSendUserLeftServer(User user)
        {
            if (user == null) return;
            if (EventUserLeftServer != null)
                EventUserLeftServer(user.Username, (byte)user.Flags);
        }

        protected void ConsoleSendUserJoinChannel(User user)
        {
            if (user == null || user.Channel == null) return;
            if (EventUserJoinChannel != null)
                EventUserJoinChannel(user.Username, (byte)user.Flags, user.Channel.Name);
        }

        protected void ConsoleSendUserLeftChannel(User user)
        {
            if (user == null || user.Channel == null) return;
            if (EventUserLeftChannel != null)
                EventUserLeftChannel(user.Username, (byte)user.Flags, user.Channel.Name);
        }

        protected void ConsoleSendUserTalk(User user, string msg)
        {
            if (user == null || user.Channel == null) return;
            if (EventUserTalk != null)
                EventUserTalk(user.Username, (byte)user.Flags, user.Channel.Name, msg);
        }

        protected void ConsoleSendUserEmote(User user, string msg)
        {
            if (user == null || user.Channel == null) return;
            if (EventUserEmote != null)
                EventUserEmote(user.Username, (byte)user.Flags, user.Channel.Name, msg);
        }
    }
}
