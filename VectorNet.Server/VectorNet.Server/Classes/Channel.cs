using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorNet.Server
{
    public partial class Server
    {
        [Flags]
        protected enum ChannelFlags
        {
            Normal = 0x00,
            Public = 0x01,
            Administrative = 0x02,
            Clan = 0x04,
            Silent = 0x08
        }

        protected class Channel
        {
            protected ChannelFlags _Flags;
            protected string _Name;
            protected List<User> _Users;
            protected List<User> _Operators;

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
            public List<User> Users { get { return _Users; } } //DO NOT REFERENCE THIS TYPE BY ITSELF! USE GetUsersInChannel() INSTEAD
            public List<User> Operators { get { return _Operators; } }


            public void AddUser(User user, bool isOperator)
            {
                if (!_Users.Contains(user))
                    _Users.Add(user);
                if (isOperator && !_Operators.Contains(user))
                    _Operators.Add(user);
                if (Owner == null)
                    Owner = user;
                user.Channel = this;
            }

            public void RemoveUser(User user)
            {
                if (_Users.Contains(user))
                    _Users.Remove(user);
                if (_Operators.Contains(user))
                    _Operators.Remove(user);
            }

        }
    }
}
