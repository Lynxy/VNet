using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;

namespace LynxVN
{
    public partial class MainWindow
    {
        public class User
        {
            public string Username { get; set; }
            public string Icon { get; set; }
            public string Client { get; set; }
            public int Ping { get; set; }
            public UserFlags Flags { get; set; }
        }
    }
}
