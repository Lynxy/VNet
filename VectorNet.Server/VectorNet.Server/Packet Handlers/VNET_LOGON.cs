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

            string dcMsg = "";
            if ((dcMsg = GetDisconnectMessage(username, password, client)) != "")
            {
                DisconnectUser(user, dcMsg);
                return;
            }

            //check client name with saved client list


            //check username+pass combo
            AccountState state = GetAccountState(username, password);
            if (state == AccountState.InvalidPassword)
            {
                SendLogonResult(user, LogonResult.InvalidPassword);
                return;
            }
            if (state == AccountState.NewAccount)
                CreateNewAccount(username, password, user.IPAddress);

            //check challenge (if any)


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

        protected string GetDisconnectMessage(string username, string password, string client)
        {
            if (username.Length < Config.UsernameMinLength)
                return "Your username is too short. Minimum length: " + Config.UsernameMinLength;
            if (username.Length > Config.UsernameMaxLength)
                return "Your username is too long. Maximum length: " + Config.UsernameMaxLength;
            if (ContainsNonPrintable(username))
                return "Your username cannot contain non-printable characters";
            if (CheckUsernameRegex(username) == false)
                return "Your username did not pass the username test";
            if (ContainsNonPrintable(password))
                return "Your password cannot contain non-printable characters";
            if (ContainsNonPrintable(client))
                return "Your client name cannot contain non-printable characters";
            return "";
        }
    }
}
