using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Lynxy.Network;
using System.Management;

namespace LynxVN
{
    public partial class MainWindow : Window
    {
        static protected class BackgroundColors
        {
            static public Brush Main = Brushes.Black;
        }
        static protected class ForegroundColors
        {
            static public Brush Channel = Brushes.White;
            static public Brush Send = Brushes.White;
        }
        static protected class ChatColors
        {
            static public Brush TimeStamp = Brushes.White;

            static public Brush ConnectionGood = Brushes.LawnGreen;
            static public Brush ConnectionBad = Brushes.OrangeRed;

            static public Brush ServerError = Brushes.Crimson;
            static public Brush ServerInfo = Brushes.DodgerBlue;

            static public Brush UsernameLocal = Brushes.CornflowerBlue;
            static public Brush UsernameRemote = Brushes.Yellow;
            static public Brush UsernameAdmin = Brushes.White;
            static public Brush UsernameModerator = Brushes.White;
            static public Brush UsernameBroadcast = Brushes.White;

            static public Brush ChatMsg = Brushes.White;
            static public Brush ChatOther = Brushes.Yellow;
            static public Brush EmoteMsg = Brushes.Yellow;
            static public Brush EmoteOther = Brushes.Yellow;

            static public Brush UserJoinedChannel = Brushes.Chartreuse;
            static public Brush UserJoinedChannel_Channel = Brushes.Yellow;
            static public Brush UserJoinedChannel_Username = Brushes.DodgerBlue;
        }
    }
}
