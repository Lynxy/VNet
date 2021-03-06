﻿using System;
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
            protected bool _Closeable;
            protected List<User> _Users;
            protected List<string> _BannedIPs;
            protected List<string> _BannedUsers;

            public Channel(string name)
                : this(name, ChannelFlags.Normal, true, null)
            {
            }

            public Channel(string name, ChannelFlags flags)
                : this(name, flags, true, null)
            {
            }

            public Channel(string name, ChannelFlags flags, bool closeable, User owner)
            {
                _Name = name;
                _Flags = flags;
                _Closeable = closeable;
                if (closeable)
                    Owner = owner;
                _Users = new List<User>();
                _BannedIPs = new List<string>();
                _BannedUsers = new List<string>();
            }


            /// <summary>
            /// The channel's name. Can never change.
            /// </summary>
            public string Name
            {
                get { return _Name; }
            }

            public ChannelFlags Flags
            {
                get { return _Flags; }
                set { _Flags = value; }
            }

            /// <summary>
            /// Whether or not the channel is able to close when no users are in it.
            /// </summary>
            public bool Closeable
            {
                get { return _Closeable; }
                set { _Closeable = value; }
            }

            public User Owner { get; set; }
            public List<string> BannedIPs { get { return _BannedIPs; } set { _BannedIPs = value; } }
            public List<string> BannedUsers { get { return _BannedUsers; } set { _BannedUsers = value; } }
            public int UserCount { get { return _Users.Count; } }
            public DateTime OwnerLeft { get; set; }

            /// <summary>
            /// Adds a user to the member list.
            /// </summary>
            /// <param name="user">The user to add</param>
            public void AddUser(User user)
            {
                if (!_Users.Contains(user))
                    _Users.Add(user);
                if (Owner == null)
                    Owner = user;
                user.Channel = this;
                if (user == Owner)
                {
                    user.Flags |= UserFlags.Operator;
                }
            }

            /// <summary>
            /// Removes a user from the member list
            /// </summary>
            /// <param name="user">The user to remove</param>
            public void RemoveUser(User user)
            {
                if (_Users.Contains(user))
                    _Users.Remove(user);
                if ((user.Flags & UserFlags.Operator) == UserFlags.Operator)
                    user.Flags ^= UserFlags.Operator;
                if (Owner == user)
                    OwnerLeft = DateTime.Now;
            }

            /// <summary>
            /// Promotes the oldest user in the channel to the owner.
            /// </summary>
            public User PromoteNewOwner()
            {
                if (_Users.Count > 0)
                {
                    Owner = _Users[0];
                    Owner.Flags |= UserFlags.Operator;
                }
                return Owner;
            }

            /// <summary>
            /// The entire member list. Does not take a user perspective into account.
            /// </summary>
            public List<User> GetCompleteUserList()
            {
                return _Users.ToList();
            }

            /// <summary>
            /// Returns whether or not a user is banned by name or IP.
            /// </summary>
            /// <param name="user">The user to check</param>
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
