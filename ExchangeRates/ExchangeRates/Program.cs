using System;
using System.Windows.Forms;

namespace ExchangeRates
{
    internal static class Program
    {
        /// <summary>
        /// Developed by Eyüp Can Gedik
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
