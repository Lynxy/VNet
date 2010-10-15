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

namespace LynxVN
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

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
    }
}
