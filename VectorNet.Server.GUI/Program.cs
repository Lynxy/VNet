using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace VectorNet.Server.GUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool serverMode = false;
            try
            {
                System.Reflection.Assembly.LoadFrom("VectorNet.Server.dll");
                serverMode = true;
            }
            catch (Exception) { }

            Application.Run(new frmMain(serverMode));
        }
    }
}
