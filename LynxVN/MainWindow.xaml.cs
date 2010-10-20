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

using System.Collections.ObjectModel;
using Lynxy.Network;
using System.Management;

namespace LynxVN
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected string MyName = "";
        protected Random rnd = new Random();
        protected ObservableCollection<User> Users = new ObservableCollection<User>();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        protected string GetProcessorID()
        {
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();
            string processorID = "";
            foreach (ManagementObject mo in moc)
                processorID += mo.Properties["processorID"].Value.ToString();
            return processorID;
        }

        protected void AddChat(params object[] val)
        {
            this.Dispatcher.Invoke(new Action(delegate
                {
                    AddChatB(val);
                }), null);
        }

        private void AddChatB(params object[] val)
        {
            if (val.Length % 2 != 0)
                throw new Exception("AddChat not passed even number of args");
            Brush col;
            string text;
            FlowDocument doc = rtbChat.Document;
            Paragraph para = new Paragraph();
            Run run;
            para.Margin = new Thickness(0);
            for (int i = 0; i < val.Length; i += 2)
            {
                col = (Brush)val[i];
                text = (string)val[i + 1];
                run = new Run(text, rtbChat.CaretPosition.DocumentEnd);
                run.Foreground = col;
                para.Inlines.Add(run);
            }
            doc.Blocks.Add(para);
            rtbChat.ScrollToEnd();
        }

        protected void mnuConnect_Click(object sender, RoutedEventArgs e)
        {
            if (socket != null)
                socket.Close();
            SetupSocket();
            //try
            //{
                socket.AsyncConnect("127.0.0.1", 4800);
            //}
            //catch (Exception ex)
            //{
            //    int stop = 0;
            //}
        }

        protected void ClearChannelList()
        {
            Users = new ObservableCollection<User>();
        }

        protected void AddUser(User user)
        {
            Users.Add(user);
            lvChannel.ItemsSource = Users;
        }

        protected void RemoveUser(string username)
        {
            User user = Users.First(u =>
                u.Username.ToLower() == username.ToLower()
                );
            if (user == null)
                return;
            Users.Remove(user);
            lvChannel.ItemsSource = Users;
        }

        private void txtSend_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string msg = txtSend.Text;
                txtSend.Text = "";

                if (msg.Length == 0)
                    return;

                if (msg == "test")
                {
                    packet.Clear().InsertStringNT("----------------------").Send(VNET_CHATEVENT);
                    for (int j = 0; j < 1000; j++)
                        packet.Clear().InsertStringNT("Test msg " + j).Send(VNET_CHATEVENT);
                    packet.Clear().InsertStringNT("----------------------").Send(VNET_CHATEVENT);
                    return;
                }

                packet.Clear().InsertStringNT(msg).Send(VNET_CHATEVENT);
                if (msg[0] != '/')
                    AddChat(Brushes.DarkCyan, "<" + MyName + "> ", Brushes.White, msg);
            }
        }
    }
}
