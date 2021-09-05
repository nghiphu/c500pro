using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C500Pro
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

            Lib.ProxeServerImpl proxeServerImpl = new Lib.ProxeServerImpl(8000);
            proxeServerImpl.StartServer();
            // Thêm white list cho phép truy cập
            proxeServerImpl.WhiteListDomain.Add("facebook");
            System.Diagnostics.Process.Start(@"C:\Program Files\Google\Chrome\Application\chrome.exe", "--proxy-server=\"http://127.0.0.1:8000\"");

            Application.Run(new frMain());

            proxeServerImpl.StopServer();
        }

    }
}
