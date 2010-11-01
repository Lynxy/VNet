using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorNet.Server
{
    public partial class Server
    {
        protected class Channel
        {
            protected ChannelFlags _Flags;
            protected string _Name;
            protected List<User> _Users;
            protected List<User> _Operators;
            protected List<string> _BannedIPs;
            protected List<string> _BannedUsers;

            public Channel(string name)
                : this(name, ChannelFlags.Normal)
            {
            }

            public Channel(string name, ChannelFlags flags)
            {
                _Name = name;
                _Flags = flags;
                _Users = new List<User>();
                _Operators = new List<User>();
                _BannedIPs = new List<string>();
                _BannedUsers = new List<string>();
            }


            public string Name
            {
                get { return _Name; }
            }

            public ChannelFlags Flags
            {
                get { return _Flags; }
                set { _Flags = value; }
            }

            public User Owner { get; set; }
            public List<User> Operators { get { return _Operators; } }
            public List<string> BannedIPs { get { return _BannedIPs; } set { _BannedIPs = value; } }
            public List<string> BannedUsers { get { return _BannedUsers; } set { _BannedUsers = value; } }
            public int UserCount { get { return _Users.Count; } }

            public void AddUser(User user, bool isOperator)
            {
                if (!_Users.Contains(user))
                    _Users.Add(user);
                if (isOperator && !_Operators.Contains(user))
                    _Operators.Add(user);
                if (Owner == null)
                    Owner = user;
                user.Channel = this;
                if (isOperator)
                    user.Flags |= UserFlags.Operator;
            }

            public void RemoveUser(User user)
            {
                if (_Users.Contains(user))
                    _Users.Remove(user);
                if (_Operators.Contains(user))
                {
                    _Operators.Remove(user);
                    user.Flags -= UserFlags.Operator;
                }
            }

            public List<User> GetCompleteUserList()
            {
                return _Users.ToList();
            }

            public bool IsUserBanned(User user)
            {
                if (_BannedIPs.Contains(user.IPAddress))
                    return true;
                if (_BannedUsers.Contains(user.Username.ToLower()))
                    return true;
                return false;
            }
        }
    }
}
