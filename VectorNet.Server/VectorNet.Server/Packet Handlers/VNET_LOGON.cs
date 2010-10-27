using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lynxy.Network;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected void HandlePacket_VNET_LOGON(User user, PacketReader reader)
        {
            string username = reader.ReadStringNT();
            string password = reader.ReadStringNT();
            string client = reader.ReadStringNT();

            if (ContainsNonPrintable(username))
            {
                DisconnectUser(user, "Usernames cannot contain non-printable characters");
                return;
            }
            else if (ContainsNonPrintable(password))
            {
                DisconnectUser(user, "Passwords cannot contain non-printable characters");
                return;
            }
            else if (ContainsNonPrintable(client))
            {
                DisconnectUser(user, "Client names cannot contain non-printable characters");
                return;
            }


            //check client name


            //check username+pass combo
            AccountState state = GetAccountState(username, password);
            if (state == AccountState.InvalidPassword)
            {
                SendLogonResult(user, LogonResult.InvalidPassword);
                return;
            }
            if (state == AccountState.NewAccount)
                CreateNewAccount(username, password, user.IPAddress);

            //remember challenge


            if (GetUserByName(username) != null)
            {
                SendLogonResult(user, LogonResult.AccountInUse);
                return;
            }

            user.Username = username;
            user.Client = client;
            user.IsOnline = true;
            ServerStats.usersOnline++;
            ConsoleSendUserJoinServer(user);

            UpdateLastLogin(username);
            SendLogonResult(user, LogonResult.Success);
            if (state == AccountState.NewAccount)
                SendServerInfo(user, "New account created!");
            JoinUserToChannel(user, Channel_Main);
        }
    }
}
