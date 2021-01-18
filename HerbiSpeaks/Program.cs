using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HerbiSpeaks
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

            string fileToOpen = "";

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                fileToOpen = args[1];
            }

            Application.Run(new HerbiSpeaks(fileToOpen));
        }
    }
}
