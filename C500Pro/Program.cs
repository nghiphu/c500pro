using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C500Pro
{
    static class Program
    {
        public static Lib.ProxeServerImpl ProxeServerImpl { private set; get; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ProxeServerImpl = new Lib.ProxeServerImpl(8000);
            ProxeServerImpl.StartServer();

            // Thêm tất cả link cdn trên genk.vn vào white list
            ProxeServerImpl.WhiteListDomain.AddRange(new[] { "genk.vn" });
            var test = Lib.ProxeServerImpl.DetectAllLink("https://genk.vn/");
            foreach (string l in test)
                ProxeServerImpl.WhiteListDomain.Add(l);

            // Trong white list ở trên có facebook => chặn riêng bằng back list 
            ProxeServerImpl.BlackListDomain.AddRange(new[] { "facebook", "static.xx.fbcdn"});

            // Khởi động chrome với proxy để test
            System.Diagnostics.Process.Start(@"C:\Program Files\Google\Chrome\Application\chrome.exe", "--proxy-server=\"http://127.0.0.1:8000\"");

            Application.Run(new frMain());

            ProxeServerImpl.StopServer();
        }

    }
}
