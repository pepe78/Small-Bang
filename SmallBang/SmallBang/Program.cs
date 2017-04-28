using System;
using System.Windows.Forms;

namespace SmallBang
{
    static class Program
    {
        static public Random r = new Random();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SmallBangLogin());
        }
    }
}
