using System;
using System.Windows.Forms;
using SpencerHakim;

namespace DWMThumbnailDemoApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ConsoleManager.Show();
            Console.WriteLine("You can't use the same window for source and destination, so let's use a console window!");

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
